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
    public class ProductionLine : ObservableObject, IReadOnlyDictionary<string, string> 
    {
        // 내부 필드와 프로퍼티
        private string _lineId;
        private string _usageName;
        private string _batteryType;
        private string _batteryShape;
        private string _buyerName;
        private string _quota;
        private string _deadlineStart;
        private string _deadlineEnd;

        public ProductionLine() { } // 기본 생성자
        public ProductionLine(ProductionLine other) // 깊은 복사를 수행하는 생성자
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            _lineId = other._lineId;
            _usageName = other._usageName;
            _batteryType = other._batteryType;
            _batteryShape = other._batteryShape;
            _buyerName = other._buyerName;
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
        public string BuyerName
        {
            get => _buyerName;
            set => SetProperty(ref _buyerName, value);
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

        // IReadOnlyDictionary 인터페이스 구현에 사용할 키 목록
        private static readonly string[] _allowedKeys = new[]
        {
        "lineId", "usageName", "batteryType", "batteryShape", "buyerName", "quota", "deadlineStart", "deadlineEnd"
        };

        // 인덱서 구현 (Dictionary 스타일 접근)
        public string this[string key]
        {
            get
            {
                switch (key)
                {
                    case "lineId": case "LineId":
                        return LineId;
                    case "usageName": case "UsageName":
                        return UsageName;
                    case "batteryType": case "BatteryType":
                        return BatteryType;
                    case "batteryShape": case "BatteryShape":
                        return BatteryShape;
                    case "buyerName": case "BuyerName":
                        return BuyerName;
                    case "quota": case "Quota":
                        return Quota;
                    case "deadlineStart": case "DeadlineStart":
                        return DeadlineStart;
                    case "deadlineEnd": case "DeadlineEnd":
                        return DeadlineEnd;
                    default:
                        throw new KeyNotFoundException($"키 '{key}'가 ProductionLine에 존재하지 않습니다.");
                }
            }
            set
            {
                switch (key)
                {
                    case "lineId": case "LineId":
                        LineId = value;
                        break;
                    case "usageName": case "UsageName":
                        UsageName = value;
                        break;
                    case "batteryType": case "BatteryType":
                        BatteryType = value;
                        break;
                    case "batteryShape": case "BatteryShape":
                        BatteryShape = value;
                        break;
                    case "buyerName": case "BuyerName":
                        BuyerName = value;
                        break;
                    case "quota": case "Quota":
                        Quota = value;
                        break;
                    case "deadlineStart": case "DeadlineStart":
                        DeadlineStart = value;
                        break;
                    case "deadlineEnd": case "DeadlineEnd":
                        DeadlineEnd = value;
                        break;
                    default:
                        throw new KeyNotFoundException($"키 '{key}'가 ProductionLine에 존재하지 않습니다.");
                }
            }
        }

        // IReadOnlyDictionary 멤버 구현
        public IEnumerable<string> Keys => _allowedKeys;

        public IEnumerable<string> Values => _allowedKeys.Select(k => this[k]);

        public int Count => _allowedKeys.Length;

        public bool ContainsKey(string key) => _allowedKeys.Contains(key);

        public bool TryGetValue(string key, out string value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var key in _allowedKeys)
            {
                yield return new KeyValuePair<string, string>(key, this[key]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
