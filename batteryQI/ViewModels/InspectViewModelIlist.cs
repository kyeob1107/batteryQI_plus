using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.Models;
using System.Windows;
using batteryQI.ViewModels.Bases;

namespace batteryQI.ViewModels
{
    internal partial class InspectViewModel : ViewModelBases
    {
        // combox 리스트

        private IList<string> _manufacList = new List<string>(); // 제조사명 받아오기
        private IDictionary<string, string> ManufacDict = new Dictionary<string, string>(); // viewmodel에서만 사용하는 딕셔너리 가져오기
        private IList<string>? _batteryTypeList = new List<string>() { "Cell", "Module", "Pack" };
        private IList<string>? _batteryShapeList = new List<string>() { "Pouch", "Cylinder" };
        private IList<string>? _usageList = new List<string>() { "Household", "Industrial" }; // 사용처 리스트업
        private IList<string>? _defectList = new List<string>() { "Damage", "Pollution", "Damage and Pollution", "Etc.." };

        private Battery _battery = Battery.Instance();
        private Manager _manager = Manager.Instance();

        private Visibility _errorInspectionVisibility = Visibility.Visible; // 첫 번째 UserControl (ErrorInspection) Visibility 제어
        private Visibility _errorReasonVisibility = Visibility.Collapsed; // 두 번째 UserControl (ErrorReason) Visibility 제어

        public IList<string>? ManufacList
        {
            get => _manufacList;
        }
        public IList<string>? BatteryTypeList
        {
            get => _batteryTypeList;
        }
        public IList<string>? BatteryShapeList
        {
            get => _batteryShapeList;
        }
        public IList<string>? UsageList
        {
            get => _usageList;
        }
        public IList<string>? DefectList
        {
            get => _defectList;
        }
        // ----------------
        public Battery battery
        {
            get => _battery;
            set => SetProperty(ref _battery, value);
        }
        public InspectViewModel()
        {
            // 다음 AUTO_INCREMENT 값 가져오기
            _battery.BatteryID = GetNextAutoIncrementId();
            getManafactureNameID();
        }

    }
}
