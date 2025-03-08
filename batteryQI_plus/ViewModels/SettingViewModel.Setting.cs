// 외부 네임스페이스 참조
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
// 내부 네임스페이스 참조
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using batteryQI_plus.Views;
using System.Windows;
using System.Reflection;


namespace batteryQI_plus.ViewModels
{
    // 설정 조회 페이지(Setting View) 및 공용 데이터 partial class
    public partial class SettingViewModel : ViewModelBases
    {
        // 필드 및 프로퍼티---------------------------------------------------------------------------
        private Employee _employee;
        private bool? _isLinePower; // 생산라인 전원
        private bool _isEditSettingRole; // 설정 편집창을 열 수 있는 권한 설정
        private int _selectedTabIndex; // 선택된 Tab index
        //private ObservableCollection<ProductionLine> _lineSettingCollection // Setting View 각 생산라인별 설정 저장
        //    = new ObservableCollection<ProductionLine>();
        private ObservableCollection<ProductionLine> _lineSettingCollection; // Setting View 각 생산라인별 설정 저장

        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
        public bool? IsLinePower // 생산라인 전원 프로퍼티
        {
            get => _isLinePower;
            set
            {
                SetProperty(ref _isLinePower, LinePower());
            }
        }
        public bool IsEditSettingRole // 설정 편집창을 열 수 있는 권한 프로퍼티
        {
            get => _isEditSettingRole;
            set => SetProperty(ref _isEditSettingRole, value);
        }
        public Visibility EditButtonVisibility { get; private set; }
        public int SelectedTabIndex // 선택된 Tab index 프로퍼티
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }
        public ObservableCollection<ProductionLine> LineSettingCollection // Setting View 각 생산라인별 설정 저장 프로퍼티
        {
            get => _lineSettingCollection;
            set => SetProperty(ref _lineSettingCollection, value);
        }

        // 생성자---------------------------------------------------------------------------
        public SettingViewModel() // 생성자
        {
            _employee = Employee.Instance(); // 로그인한 사용자 직원 정보
            _isEditSettingRole = Employee.EmployeeRole >= 10 ? true : false; // 로그인한 사용자 직원의 권한이 특정 기준을 넘으면 설정 편집창 열기 권한 허용.
            EditButtonVisibility = _isEditSettingRole ? Visibility.Visible : Visibility.Collapsed;
            _lineSettingCollection = new ObservableCollection<ProductionLine>();
            getLineSetting(); // 설정 조회창의 설정 목록 초기화
            getSettingItemList(); // 설정 편집창의 설정 옵션 목록 초기화
        }

        // 메소드----------------------------------------------------------------------------
        private bool? LinePower() // 생산라인 전원. 본래 전원을 끄고켜는 Command 였지만 프로퍼티 Set 내부 메소드로 전환 
        {
            if (_isLinePower == false)
            {
                if (System.Windows.Forms.MessageBox.Show($"생산라인의 전원을 켜시겠습니까?", "Yes-No", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _isLinePower = true;
                }
            }
            else if (_isLinePower == true)
            {
                if (System.Windows.Forms.MessageBox.Show($"정말로 생산라인의 전원을 끄시겠습니까?", "Yes-No", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _isLinePower = false;
                }
            }
            else
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"생산라인의 작동 여부가 저장되어있지 않습니다.\r\n현재 생산라인이 작동 중입니까?", "Yes-No", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    _isLinePower = true;
                else
                    _isLinePower = false;
            }
            return _isLinePower;
        }

        private string GetTabByLineId(int selectedTabIndex) // Setting View에서 선택한 Tab의 lineId를 찾아 반환하는 메소드
        {
            if (selectedTabIndex < 0) // 선택한 Tab의 Index가 아무것도 선택하지 않은 상태(-1)일 경우 빈 문자열 반환
                return string.Empty;
            return LineSettingCollection[selectedTabIndex].LineId;
        }

        private void getLineSetting() // DB에서 값을 가져와 Setting View에 사용할 프로퍼티 초기화
        {
            //_lineSettingCollection.Clear();
            List<Dictionary<string, object>> lineSettingCollection =
                _dblink.Select("SELECT pl.lineId, pl.usageName, pl.batteryType, pl.batteryShape, pl.buyerId, b.buyerName, pl.quota, pl.deadlineStart, pl.deadlineEnd " +
                "FROM productionLines pl LEFT JOIN buyers b ON pl.buyerId = b.buyerId WHERE lineId<> 0; "); // 0번 행(관리자 직책)은 실질적인 생산라인이 아니기 때문에 제외 
            foreach (var row in lineSettingCollection) // 생산라인 행별로 설정 저장. TryGetValue 사용시 불필요한 인덱싱과 예외 발생 가능성을 줄여 더 빠르다고함
            {
                string? lineId = row.TryGetValue("lineId", out var lineIdObj) && lineIdObj != null ? lineIdObj.ToString() : "";
                string? usageName = row.TryGetValue("usageName", out var usageNameObj) && usageNameObj != null ? usageNameObj.ToString() : "";
                string? batteryType = row.TryGetValue("batteryType", out var batteryTypeObj) && batteryTypeObj != null ? batteryTypeObj.ToString() : "";
                string? batteryShape = row.TryGetValue("batteryShape", out var batteryShapeObj) && batteryShapeObj != null ? batteryShapeObj.ToString() : "";
                string? buyerId = row.TryGetValue("buyerId", out var buyerIdObj) && buyerIdObj != null ? buyerIdObj.ToString() : "";
                string? buyerName = row.TryGetValue("buyerName", out var buyerNameObj) && buyerNameObj != null ? buyerNameObj.ToString() : "";
                string? quota = row.TryGetValue("quota", out var quotaObj) && quotaObj != null ? quotaObj.ToString() : "";
                string? deadlineStart = row.TryGetValue("deadlineStart", out var deadlineStartObj) && deadlineStartObj != null ? deadlineStartObj.ToString() : "";
                string? deadlineEnd = row.TryGetValue("deadlineEnd", out var deadlineEndObj) && deadlineEndObj != null ? deadlineEndObj.ToString() : "";

                _lineSettingCollection.Add(new ProductionLine()
                {
                    LineId = lineId,
                    UsageName = usageName,
                    BatteryType = batteryType,
                    BatteryShape = batteryShape,
                    BuyerId = new KeyValuePair<string, string>(buyerName, buyerId),
                    Quota = quota,
                    DeadlineStart = deadlineStart,
                    DeadlineEnd = deadlineEnd
                });
            }
        }

        // 커멘드---------------------------------------------------------------------------------------------
        [RelayCommand] private void OpenEditSetting() // 설정 편집창 열기
        {
            SelectedLineId = GetTabByLineId(_selectedTabIndex);
            EditSettingView editSettingPage = new EditSettingView();
            editSettingPage.ShowDialog();
        }
    }
}
