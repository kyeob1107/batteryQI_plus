using System.ComponentModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using MySql.Data.MySqlClient;

namespace batteryQI_plus.Models
{
    // 생산라인 설정 Model
    // IReadOnlyDictionary: 모든 키-값 쌍 기반 컬렉션의 표준 프로토콜을 정의.
    // Dictionary로 구현했던 기존 코드와 충돌 방지. 설정 항목 자체를 추가하거나 삭제할 일은 없기 때문에 ReadOnly형 상속.
    public class ProductionLine : ObservableObject
    {
        // 내부 필드와 프로퍼티
        private string _lineId;
        private string _usageName;
        private string _batteryType;
        private string _batteryShape;
        private KeyValuePair<string, string> _buyerId; // Buyer를 통한 발주처 정보 필드. Key: 발주처 Name, Value: 발주처 Id 정보 저장
        private string _quota;
        private string _deadlineStart;
        private string _deadlineEnd;

        public ProductionLine() // 기본 생성자
        {
            _lineId = "";
            _usageName = "";
            _batteryType = "";
            _batteryShape = "";
            _buyerId = new KeyValuePair<string, string>();
            _quota = "";
            _deadlineStart = "";
            _deadlineEnd = "";
        } 
        public ProductionLine(ProductionLine other) // 깊은 복사를 수행하는 생성자
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            _lineId = other._lineId;
            _usageName = other._usageName;
            _batteryType = other._batteryType;
            _batteryShape = other._batteryShape;
            _buyerId = other._buyerId;
            _quota = other._quota;
            _deadlineStart = other._deadlineStart;
            _deadlineEnd = other._deadlineEnd;
        }

        public string LineId
        {
            get => _lineId;
            set => SetProperty(ref _lineId, value);
        }
        public string UsageName
        {
            get => _usageName;
            set => SetProperty(ref _usageName, value);
        }
        public string BatteryType
        {
            get => _batteryType;
            set => SetProperty(ref _batteryType, value);
        }
        public string BatteryShape
        {
            get => _batteryShape;
            set => SetProperty(ref _batteryShape, value);
        }
        public KeyValuePair<string, string> BuyerId
        {
            get => _buyerId;
            set => SetProperty(ref _buyerId, value);
        }
        public string Quota
        {
            get => _quota;
            set => SetProperty(ref _quota, value);
        }
        public string DeadlineStart
        {
            get => _deadlineStart;
            set => SetProperty(ref _deadlineStart, value);
        }
        public string DeadlineEnd
        {
            get => _deadlineEnd;
            set => SetProperty(ref _deadlineEnd, value);
        }
    }
}
