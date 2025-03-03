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
        // 생산라인 설정 템플릿
        private static readonly Dictionary<string, string> _settingTemplate
            = new Dictionary<string, string>
            {
                { "lineId", "" }, // 생산라인 Id
                { "usageName", "" }, { "batteryType", "" }, { "batteryShape", "" }, // 생산라인 담당 배터리 정보
                { "buyerName", ""}, { "quota", "" }, { "deadlineStart", ""}, { "deadlineEnd", "" }, // 생산라인 담당 발주처 정보
            };

        // Setting View 필드
        private bool _isEditSettingRole; // 설정 편집창을 열 수 있는 권한 설정
        private int _selectedTabIndex;
        private ObservableCollection<Dictionary<string, string>> _lineSettingCollection 
            = new ObservableCollection<Dictionary<string, string>>(); // Setting View 각 생산라인별 설정 저장

        // EditSetting View 필드
        private IList<string> _lineIdList; // 설정 옵션 컨트롤 목록(combobox, textbox, Datepicker) items 필드
        private IList<string> _usageNameList;
        private IList<string> _batteryTypeList;
        private IList<string> _batteryShapeList;
        private IList<string> _buyerNameList;
        private IList<string> _quotaList;
        private IList<string> _deadlineStartList;
        private IList<string> _deadlineEndList;
        private Dictionary<string, string> _selectedLineSetting = new Dictionary<string, string>(_settingTemplate); // 선택된 설정 옵션 필드
            

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
        public ObservableCollection<Dictionary<string, string>> LineSettingCollection
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
        public IList<string> BuyerNameList
        {
            get => _buyerNameList;
            set => SetProperty(ref _buyerNameList, value);
        }
        public IList<string> QuotaList
        {
            get => _quotaList;
            set => SetProperty(ref _quotaList, value);
        }
        public IList<string> DeadlineStartList
        {
            get => _deadlineStartList;
            set => SetProperty(ref _deadlineStartList, value);
        }
        public IList<string> DeadlineEndList
        {
            get => _deadlineEndList;
            set => SetProperty(ref _deadlineEndList, value);
        }
        public Dictionary<string, string> SelectedLineSetting
        {
            get => _selectedLineSetting;
            set => SetProperty(ref _selectedLineSetting, value);
        }

        private void getLineSetting() // DB에서 값을 가져와 Setting View에 사용할 프로퍼티 초기화
        {
            _lineSettingCollection.Clear();
            List<Dictionary<string, object>> lineSettingCollection = 
                _dblink.Select("SELECT pl.lineId, pl.usageName, pl.batteryType, pl.batteryShape, b.buyerName, pl.quota, pl.deadlineStart, pl.deadlineEnd FROM productionLines pl LEFT JOIN buyers b ON pl.buyerId = b.buyerId WHERE lineId<> 0; "); // 0번 행(관리자 직책)은 실질적인 생산라인이 아니기 때문에 제외 
            foreach (var row in lineSettingCollection) // 생산라인 행별로 설정 저장. TryGetValue 사용시 불필요한 인덱싱과 예외 발생 가능성을 줄여 더 빠르다고함
            {
                string? lineId = row.TryGetValue("lineId", out var lineIdObj) && lineIdObj != null ? lineIdObj.ToString() : "";
                string? usageName = row.TryGetValue("usageName", out var usageNameObj) && usageNameObj != null ? usageNameObj.ToString() : "";
                string? batteryType = row.TryGetValue("batteryType", out var batteryTypeObj) && batteryTypeObj != null ? batteryTypeObj.ToString() : "";
                string? batteryShape = row.TryGetValue("batteryShape", out var batteryShapeObj) && batteryShapeObj != null ? batteryShapeObj.ToString() : "";
                string? buyerName = row.TryGetValue("buyerName", out var buyerIdObj) && buyerIdObj != null ? buyerIdObj.ToString() : "";
                string? quota = row.TryGetValue("quota", out var quotaObj) && quotaObj != null ? quotaObj.ToString() : "";
                string? deadlineStart = row.TryGetValue("deadlineStart", out var deadlineStartObj) && deadlineStartObj != null ? deadlineStartObj.ToString() : "";
                string? deadlineEnd = row.TryGetValue("deadlineEnd", out var deadlineEndObj) && deadlineEndObj != null ? deadlineEndObj.ToString() : "";

                _lineSettingCollection.Add(new Dictionary<string, string>(_settingTemplate)
                {
                    ["lineId"] = lineId,
                    ["usageName"] = usageName, ["batteryType"] = batteryType, ["batteryShape"] = batteryShape,
                    ["buyerName"] = buyerName, ["quota"] = quota, ["deadlineStart"] = deadlineStart, ["deadlineEnd"] = deadlineEnd
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
            BuyerNameList = _lineSettingCollection
                .Where(dict => dict.ContainsKey("buyerName"))
                .Select(dict => dict["buyerName"])
                .Distinct().ToList();
            QuotaList = _lineSettingCollection
                .Where(dict => dict.ContainsKey("quota"))
                .Select(dict => dict["quota"]).ToList();
            DeadlineStartList = _lineSettingCollection
                .Where(dict => dict.ContainsKey("deadlineStart"))
                .Select(dict => dict["deadlineStart"]).ToList();
            DeadlineEndList = _lineSettingCollection
                .Where(dict => dict.ContainsKey("deadlineEnd"))
                .Select(dict => dict["deadlineEnd"]).ToList();
        }
    }
}
