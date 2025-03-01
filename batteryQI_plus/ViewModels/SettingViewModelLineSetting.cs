using batteryQI_plus.Models;
using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace batteryQI_plus.ViewModels
{
    public partial class SettingViewModel : ViewModelBases // View에 출력할 생산라인 설정 프로퍼티 선언용
    {
        private bool _isEditSettingRole; // 설정 편집창을 열 수 있는 권한 설정
        private ObservableCollection<Dictionary<string, string>> _lineSettingCollection 
            = new ObservableCollection<Dictionary<string, string>>(); // Setting View 각 생산라인별 설정 저장
        // EditSetting 선택된 값 프로퍼티
        private ObservableCollection<KeyValuePair<string, string>> _selectedLineSetting
            = new ObservableCollection<KeyValuePair<string, string>>
            {
                new ("lineId", ""),
                new ("usageName", ""),
                new ("batteryType", "")
            };

        public bool IsEditSettingRole
        {
            get => _isEditSettingRole;
            set => SetProperty(ref _isEditSettingRole, value);
        }
        public ObservableCollection<Dictionary<string, string>> LineSettingCollection
        {
            get => _lineSettingCollection;
            set => SetProperty(ref _lineSettingCollection, value);
        }
        public ObservableCollection<KeyValuePair<string, string>> SelectedLineSetting
        {
            get => _selectedLineSetting;
            set => SetProperty(ref _selectedLineSetting, value);
        }

        private void getLineSetting()
        {
            _lineSettingCollection.Clear();
            List<Dictionary<string, object>> lineSettingCollection = 
                _dblink.Select("SELECT pl.lineId, pl.usageName, pl.batteryType, pl.batteryShape, b.buyerName, pl.quota, pl.deadlineStart, pl.deadlineEnd FROM productionLines pl LEFT JOIN buyers b ON pl.buyerId = b.buyerId WHERE lineId<> 0; "); // 0번 행(관리자 직책)은 실질적인 생산라인이 아니기 때문에 제외 
            foreach (var row in lineSettingCollection) // 생산라인 행별로 설정 저장. TryGetValue 사용시 불필요한 인덱싱과 예외 발생 가능성을 줄여 더 빠르다고함
            {
                string? lineId = row.TryGetValue("lineId", out var lineIdObj) && lineIdObj != null ? lineIdObj.ToString() : "null";
                string? usageName = row.TryGetValue("usageName", out var usageNameObj) && usageNameObj != null ? usageNameObj.ToString() : "null";
                string? batteryType = row.TryGetValue("batteryType", out var batteryTypeObj) && batteryTypeObj != null ? batteryTypeObj.ToString() : "null";
                string? batteryShape = row.TryGetValue("batteryShape", out var batteryShapeObj) && batteryShapeObj != null ? batteryShapeObj.ToString() : "null";
                string? buyerName = row.TryGetValue("buyerName", out var buyerIdObj) && buyerIdObj != null ? buyerIdObj.ToString() : "null";
                string? quota = row.TryGetValue("quota", out var quotaObj) && quotaObj != null ? quotaObj.ToString() : "null";
                string? deadlineStart = row.TryGetValue("deadlineStart", out var deadlineStartObj) && deadlineStartObj != null ? deadlineStartObj.ToString() : "null";
                string? deadlineEnd = row.TryGetValue("deadlineEnd", out var deadlineEndObj) && deadlineEndObj != null ? deadlineEndObj.ToString() : "null";

                _lineSettingCollection.Add(new Dictionary<string, string> 
                { { "lineId", lineId },
                    { "usageName", usageName }, { "batteryType",batteryType }, { "batteryShape", batteryShape },
                    { "buyerName", buyerName }, { "quota", quota }, { "deadlineStart", deadlineStart }, { "deadlineEnd", deadlineEnd } }
                );
            }
        }
    }
}
