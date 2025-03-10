// 외부 네임스페이스 참조
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows.Forms;
// 내부 네임스페이스 참조
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using System.Reflection;
using System.Windows.Shapes;


namespace batteryQI_plus.ViewModels
{
    // 설정 편집 페이지(EditSetting View) partial class
    public partial class SettingViewModel : ViewModelBases
    {
        // 필드 및 프로퍼티-----------------------------------------------------------------------------------------
        // 설정 옵션 combobox Item 목록
        private IList<string> _lineIdList; // 생산라인 Id 옵션 목록
        private IList<string> _usageNameList; // 배터리 사용처 옵션 목록
        private IList<string> _batteryTypeList; // 배터리 타입 옵션 목록
        private IList<string> _batteryShapeList; // 배터리 형태 옵션 목록
        private Dictionary<string, string> _buyerDataDic; // Key: 발주처 Id, Value: 발주처 이름을 저장한 딕셔너리. 같은 발주처를 의미하는 Id와 이름끼리 묶어 한 요소로 취급
        private ProductionLine _selectedLineSetting = new ProductionLine(); // 선택된 설정 옵션 필드
        private string _selectedLineId; // EditSetting View에서 선택한 LineId에 따라 나머지 설정 옵션들이 현재 설정값을 가리키도록 Triger를 설정하기 위한 필드

        public IList<string> LineIdList // 생산라인 Id 프로퍼티
        {
            get => _lineIdList;
            set => SetProperty(ref _lineIdList, value);
        }
        public IList<string> UsageNameList // 배터리 사용처 프로퍼티
        {
            get => _usageNameList;
            set => SetProperty(ref _usageNameList, value);
        }
        public IList<string> BatteryTypeList // 배터리 타입 프로퍼티
        {
            get => _batteryTypeList;
            set => SetProperty(ref _batteryTypeList, value); 
        }
        public IList<string> BatteryShapeList // 배터리 형태 프로퍼티
        {
            get => _batteryShapeList;
            set => SetProperty(ref _batteryShapeList, value);
        }
        public Dictionary<string, string> BuyerDataDic  // Key: 발주처 Id, Value: 발주처 이름을 저장한 딕셔너리 프로퍼티.
        {
            get => _buyerDataDic;
            set => SetProperty(ref _buyerDataDic, value);
        }
        public ProductionLine SelectedLineSetting // 선택된 설정 옵션 프로퍼티
        {
            get => _selectedLineSetting;
            set => SetProperty(ref _selectedLineSetting, value);
        }
        public string SelectedLineId // 생산라인 Id 기반 Triger 프로퍼티
        {
            get => _selectedLineId;
            set // EditSetting View에서 특정 생산라인을 선택했을 경우 나머지 설정 옵션들이 해당 생산라인의 현재 설정값을 가리키도록 Triger 설정
            {
                if (value != null && SetProperty(ref _selectedLineId, value))
                {
                    var selectedLine = _lineSettingCollection.FirstOrDefault(line => line.LineId == value);
                    if (selectedLine != null)
                    {
                        SelectedLineSetting.LineId = selectedLine.LineId;
                        SelectedLineSetting.UsageName = selectedLine.UsageName;
                        SelectedLineSetting.BatteryType = selectedLine.BatteryType;
                        SelectedLineSetting.BatteryShape = selectedLine.BatteryShape;
                        SelectedLineSetting.BuyerId = selectedLine.BuyerId;
                        SelectedLineSetting.Quota = selectedLine.Quota;
                        SelectedLineSetting.DeadlineStart = selectedLine.DeadlineStart;
                        SelectedLineSetting.DeadlineEnd = selectedLine.DeadlineEnd;
                    }
                }
            }
        }
        public Action? CloseAction { get; set; } // 설정 편집 페이지 닫기 프로퍼티

        // 메소드-----------------------------------------------------------------------------------------
        private void getSettingItemList() // EditSetting View에 설정 옵션으로 사용할 프로퍼티 초기화
        {
            LineIdList = _lineSettingCollection
                .Select(x => x.LineId).ToList();
            UsageNameList = new List<string>() { "Industrial", "Household" };
            BatteryTypeList = new List<string>() { "Cell", "Module", "Pack" };
            BatteryShapeList = new List<string>() { "Cylinder", "Pouch" };
            BuyerDataDic = new Dictionary<string, string>();
            var buyerDataResult = _dblink.Select("SELECT buyerId, buyerName FROM buyers order by buyerId ASC;");
            foreach (var dict in buyerDataResult)
            {
                BuyerDataDic.Add(dict["buyerName"].ToString(), dict["buyerId"].ToString());
            }
        }

        // 커멘드------------------------------------------------------------------------------------------
        [RelayCommand] private void SaveLineSettingButtonClick(object sender) // 변경된 설정 저장 커멘드
        {
            if (_selectedLineSetting.LineId != "") // 생산라인 Id를 선택했을 때만 저장
            {
                if (System.Windows.Forms.MessageBox.Show($"설정을 저장하시겠습니까?", "Yes-No", MessageBoxButtons.YesNo) == DialogResult.Yes) // 설정 저장 여부를 묻는 메시지
                {
                    // 설정 옵션 선택을 통해 설정을 변경한 생산라인을 실제 프로퍼티, DB에 반영
                    for (int lineSequence = 0; lineSequence < LineSettingCollection.Count; lineSequence++)
                    {
                        // 설정을 변경한 생산라인을 기존 설정의 생산라인과 매칭
                        if (LineSettingCollection[lineSequence].LineId == _selectedLineSetting.LineId)
                        {
                            // View의 DatePicker의 SelectedDate 값이 미국식 날짜 format인 "MM/dd/yyyy hh:mm:ss tt"로만 반환되는 문제.
                            // string -> DateTime -> string 변환으로 원하는 "yyyy-MM-dd" format으로 변환.
                            // CultureInfo.InvariantCulture: 날짜 변환 중 문화권 형식에 의존하지 않도록 강제하는 옵션
                            _selectedLineSetting.DeadlineStart = _selectedLineSetting.DeadlineStart != "" ? DateTime.Parse(_selectedLineSetting.DeadlineStart, CultureInfo.InvariantCulture)
                                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                            _selectedLineSetting.DeadlineEnd = _selectedLineSetting.DeadlineEnd != "" ? DateTime.Parse(_selectedLineSetting.DeadlineEnd, CultureInfo.InvariantCulture)
                                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";

                            // 변경된 설정 DB 업데이트 및 View 내용 업데이트
                            // lineId별로 확인하기 때문에 프로퍼티 중 LineId는 제외
                            // 프로퍼티 정보를 동적으로 저장할 변수
                            IEnumerable<PropertyInfo> properties = _selectedLineSetting.GetType()
                                                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                            .Where(p => p.Name != "LineId");
                            // 업데이트할 열의 할당 구문을 저장할 리스트
                            List<string> updateSettingColumns = new List<string>();
                            Dictionary<string, string> sqlColumnMapping = new Dictionary<string, string>
                            {
                                //{ "LineId", "lineId" },
                                { "UsageName", "usageName" },
                                { "BatteryType", "batteryType" },
                                { "BatteryShape","batteryShape" },
                                //{ "BuyerData", "buyerData"},
                                { "BuyerId", "buyerId" },
                                //{ "BuyerName", "buyerName" },
                                { "Quota", "quota"},
                                { "DeadlineStart", "deadlineStart"},
                                { "DeadlineEnd", "deadlineEnd" }
                            };
                            
                            foreach (var property in properties)
                            {

                                // DB 업데이트에 사용할 쿼리문 구성(Set)에 사용할 부분 작성
                                // 기존 값과 변경된 값이 다를 때만 업데이트
                                object currentValue = property.GetValue(_selectedLineSetting);
                                object originalValue = LineSettingCollection[lineSequence].GetType().GetProperty(property.Name)
                                                                                .GetValue(LineSettingCollection[lineSequence]);
                                if (property.Name == "BuyerId")
                                {
                                    var buyerIdPair = (KeyValuePair<string, string>)currentValue;
                                    var originalBuyerIdPair = (KeyValuePair<string, string>)originalValue;

                                    if (buyerIdPair.Value != originalBuyerIdPair.Value)
                                    {
                                        updateSettingColumns.Add($"buyerId = '{buyerIdPair.Value}'");
                                    }
                                }
                                else
                                {
                                    if (currentValue != originalValue)
                                    {
                                        string newSettingValue = currentValue.ToString();

                                        if (string.IsNullOrEmpty(newSettingValue)) // 값이 빈 문자열이면 NULL로 업데이트
                                        {
                                            updateSettingColumns.Add($"{sqlColumnMapping[property.Name]} = NULL");
                                        }
                                        else
                                        {
                                            if (int.TryParse(newSettingValue, out int newSettingValueNum)) // 숫자형 DB 데이터일 경우
                                                updateSettingColumns.Add($"{sqlColumnMapping[property.Name]} = {newSettingValueNum}");
                                            else
                                                updateSettingColumns.Add($"{sqlColumnMapping[property.Name]} = '{newSettingValue}'");
                                        }
                                    }
                                }

                                // 바뀐 부분 설정 조회용 view에 표시되는 내용 업데이트
                                if (originalValue != currentValue)
                                {
                                    // LineSettingCollection[lineSequence]의 프로퍼티 값을 직접 변경
                                    LineSettingCollection[lineSequence].GetType().GetProperty(property.Name)
                                                          .SetValue(LineSettingCollection[lineSequence], currentValue);
                                }

                            }
                            // DB에 테이블 Update부분
                            if (updateSettingColumns.Count > 0) // 업데이트할 열이 하나 이상
                            {
                                string setClause = string.Join(", ", updateSettingColumns); // SET 구문 생성
                                                                                            // 최종 sql 명령문 완성. Set 구문과 lineId 기준 Where 구문 결합.
                                string sql = $"UPDATE productionLines SET {setClause} WHERE lineId = {_selectedLineSetting.LineId}";

                                // 최종 완성한 sql을 사용하여 DB 업데이트 수행
                                _dblink.Update(sql);
                            }
                            break;
                        }
                    }
                    CloseAction?.Invoke(); // EditSetting View 닫기
                    _viewModelLocator.ResetDashboardViewModel(); // 대시보드 뷰모델 초기화
                }
            }
        }
    }
}
