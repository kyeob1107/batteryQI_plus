using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
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
        private bool? _isLinePower; // 생산라인 전원

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
        public bool? IsLinePower // 생산라인 전원 프로퍼티
        {
            get => _isLinePower;
            set
            {
                SetProperty(ref _isLinePower, LinePower());
            }
        }
         
        // 메소드-------------------------------------------------------------------------
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
    }
}
