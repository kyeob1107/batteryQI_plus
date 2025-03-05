using batteryQI_plus.Models;
using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace batteryQI_plus.ViewModels
{
    public partial class SettingViewModel : ViewModelBases // View에 출력할 생산라인 설정 프로퍼티 선언용
    {
        // Setting View 필드
        private bool _isEditSettingRole; // 설정 편집창을 열 수 있는 권한 설정
        private int _selectedTabIndex;
        private ObservableCollection<ProductionLine> _lineSettingCollection 
            = new ObservableCollection<ProductionLine>(); // Setting View 각 생산라인별 설정 저장

        // EditSetting View 필드
        // 설정 옵션 combobox Item 목록
        private IList<string> _lineIdList; 
        private IList<string> _usageNameList;
        private IList<string> _batteryTypeList;
        private IList<string> _batteryShapeList;
        private Dictionary<string, string> _buyerDataDic; // Key: 발주처 Id, Value: 발주처 이름을 저장한 딕셔너리. 같은 발주처를 의미하는 Id와 이름끼리 묶어 한 요소로 취급
        private ProductionLine _selectedLineSetting = new ProductionLine(); // 선택된 설정 옵션 필드
        private string _selectedLineId; // EditSetting View에서 선택한 LineId에 따라 나머지 설정 옵션들이 현재 설정값을 가리키도록 Triger를 설정하기 위한 프로퍼티

        // Setting View 프로퍼티
        public bool IsEditSettingRole
        {
            get => _isEditSettingRole;
            set => SetProperty(ref _isEditSettingRole, value);
        }
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }
        public ObservableCollection<ProductionLine> LineSettingCollection
        {
            get => _lineSettingCollection;
            set => SetProperty(ref _lineSettingCollection, value);
        }

        // EditSetting View 프로퍼티
        public IList<string> LineIdList
        {
            get => _lineIdList;
            set => SetProperty(ref _lineIdList, value);
        }
        public IList<string> UsageNameList
        {
            get => _usageNameList;
            set => SetProperty(ref _usageNameList, value);
        }
        public IList<string> BatteryTypeList
        {
            get => _batteryTypeList;
            set => SetProperty(ref _batteryTypeList, value);
        }
        public IList<string> BatteryShapeList
        {
            get => _batteryShapeList;
            set => SetProperty(ref _batteryShapeList, value);
        }
        public Dictionary<string, string> BuyerDataDic
        {
            get => _buyerDataDic;
            set => SetProperty(ref _buyerDataDic, value);
        }
        public ProductionLine SelectedLineSetting
        {
            get => _selectedLineSetting;
            set => SetProperty(ref _selectedLineSetting, value);
        }

        private void getLineSetting() // DB에서 값을 가져와 Setting View에 사용할 프로퍼티 초기화
        {
            _lineSettingCollection.Clear();
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
                    UsageName = usageName, BatteryType = batteryType, BatteryShape = batteryShape,
                    BuyerData = new KeyValuePair<string, string>(buyerId, buyerName), Quota = quota, DeadlineStart = deadlineStart, DeadlineEnd = deadlineEnd
                });
            }
        }

        private void getSettingItemList() // EditSetting View에 설정 옵션으로 사용할 프로퍼티 초기화
        {
            LineIdList = _lineSettingCollection
                .Where(dict => dict.ContainsKey("lineId"))
                .Select(dict => dict["lineId"]).ToList();
            UsageNameList = new List<string>() { "Industrial", "Household" };
            BatteryTypeList = new List<string>() { "Cell", "Module", "Pack" };
            BatteryShapeList = new List<string>() { "Cylinder", "Pouch" };
            BuyerDataDic = new Dictionary<string, string>();
            foreach (var dict in _dblink.Select("SELECT buyerId, buyerName FROM buyers order by buyerId ASC;"))
            {
                BuyerDataDic.Add(dict["buyerId"].ToString(), dict["buyerName"].ToString());
            }
        }
    }
}
