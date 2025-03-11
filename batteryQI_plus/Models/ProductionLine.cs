using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using batteryQI_plus.Models.Bases;
using CommunityToolkit.Mvvm.ComponentModel;
using MySql.Data.MySqlClient;

namespace batteryQI_plus.Models
{
    // 생산라인 설정 Model
    // IReadOnlyDictionary: 모든 키-값 쌍 기반 컬렉션의 표준 프로토콜을 정의.
    // Dictionary로 구현했던 기존 코드와 충돌 방지. 설정 항목 자체를 추가하거나 삭제할 일은 없기 때문에 ReadOnly형 상속.
    public class ProductionLine : ModelBase
    {
        // 내부 필드와 프로퍼티
        private int _lineId;
        private string _usageName;
        private string _batteryType;
        private string _batteryShape;
        private KeyValuePair<string, string> _buyerId; // Buyer를 통한 발주처 정보 필드. Key: 발주처 Name, Value: 발주처 Id 정보 저장
        private string _quota;
        private string _deadlineStart;
        private string _deadlineEnd;
        private bool _commandValue; 
        private bool processStateValue; // 생산라인 전원

        public ProductionLine()
        {
            _lineId = 0;
            _usageName = "";
            _batteryType = "";
            _batteryShape = "";
            _buyerId = new KeyValuePair<string, string>();
            _quota = "";
            _deadlineStart = "";
            _deadlineEnd = "";
        }
        public ProductionLine(int lineNum) // 기본 생성자
        {
            if (lineNum <= 0)
            {
                throw new ArgumentException("lineNum는 0보다 커야합니다.", nameof(lineNum));
            }

            _lineId = lineNum;
            //_usageName = "";
            //_batteryType = "";
            //_batteryShape = "";
            //_buyerId = new KeyValuePair<string, string>();
            //_quota = "";
            //_deadlineStart = "";
            //_deadlineEnd = "";
            ProductionLineInitialize();
            CheckInspectionState(); // _isLinePower 초기화 // _isLinePower = false;
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
            _commandValue = other._commandValue;
        }

        public int LineId
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
        public bool CommandValue // 생산라인 전원 프로퍼티
        {
            get => _commandValue;
            set
            {
                SetProperty(ref _commandValue, CommandToggleSetting());
            }
        }
         
        // 메소드-------------------------------------------------------------------------
        private void ProductionLineInitialize()
        {
            string prdoctionSettingSelectQuery = $@"SELECT 
                                                        pl.usageName, 
                                                        pl.batteryType, 
                                                        pl.batteryShape, 
                                                        pl.buyerId, 
                                                        b.buyerName, 
                                                        pl.quota, 
                                                        pl.deadlineStart, 
                                                        pl.deadlineEnd
                                                    FROM productionLines pl 
                                                    LEFT JOIN buyers b 
                                                        ON pl.buyerId = b.buyerId 
                                                    WHERE lineId = {_lineId};";
            var result = _dblink.Select(prdoctionSettingSelectQuery);
            _usageName = result[0].TryGetValue("usageName", out var usageNameObj) && usageNameObj != null ? usageNameObj.ToString() : "";
            _batteryType = result[0].TryGetValue("batteryType", out var batteryTypeObj) && batteryTypeObj != null ? batteryTypeObj.ToString() : "";
            _batteryShape = result[0].TryGetValue("batteryShape", out var batteryShapeObj) && batteryShapeObj != null ? batteryShapeObj.ToString() : "";
            string? buyerId = result[0].TryGetValue("buyerId", out var buyerIdObj) && buyerIdObj != null ? buyerIdObj.ToString() : "";
            string? buyerName = result[0].TryGetValue("buyerName", out var buyerNameObj) && buyerNameObj != null ? buyerNameObj.ToString() : "";
            _buyerId = new KeyValuePair<string, string>(buyerName, buyerId);
            _quota = result[0].TryGetValue("quota", out var quotaObj) && quotaObj != null ? quotaObj.ToString() : "";
            _deadlineStart = result[0].TryGetValue("deadlineStart", out var deadlineStartObj) && deadlineStartObj != null ? deadlineStartObj.ToString() : "";
            _deadlineEnd = result[0].TryGetValue("deadlineEnd", out var deadlineEndObj) && deadlineEndObj != null ? deadlineEndObj.ToString() : "";
        }

        private void CheckInspectionState()
        {
            string checkQuery = $@"SELECT processState 
                                    FROM inspectionCurrentState 
                                    WHERE lineId = {_lineId};";
            var result = _dblink.Select(checkQuery);
            int state = (int)(sbyte)result[0]["processState"];

            if (state > 0)
            {
                processStateValue = true; 
            }

            else
            {
                processStateValue = false;
            }
        }
        private void CheckCommandState()
        {
            string checkQuery = $@"SELECT onOffCommand 
                                    FROM inspectionCurrentState 
                                    WHERE lineId = {_lineId};";
            var result = _dblink.Select(checkQuery);
            // bool state = (bool)result[0]["onOffCommand"];
            // _commandValue = Convert.ToBoolean(state);
            _commandValue = (bool)result[0]["onOffCommand"];
        }

        private bool CommandToggleSetting() // 생산라인 전원. 본래 전원을 끄고켜는 Command 였지만 프로퍼티 Set 내부 메소드로 전환 
        {
            #region 테스트용
            //string testOnQuery = $@"UPDATE inspectionCurrentState
            //                       SET processState = 1
            //                       WHERE stateId = {_lineId};";
            //string testOffQuery = $@"UPDATE inspectionCurrentState
            //                       SET processState = -1
            //                       WHERE stateId = {_lineId};";
            //string testExecuteQuery = "";
            #endregion
            string onCommandQuery = $@"UPDATE inspectionCurrentState
                                   SET onOffCommand = 1
                                   WHERE stateId = {_lineId};";
            string offCommandQuery = $@"UPDATE inspectionCurrentState
                                   SET onOffCommand = 0
                                   WHERE stateId = {_lineId};";
            string executeQuery = "";

            bool needsUpdate = false;

            CheckInspectionState(); // 변경하기 전 DB에서 상태 조회하여 동기화 체크 
            CheckCommandState(); // 명령 값 체크 - selected바뀔때마다 해주기?
            Console.WriteLine($"상태:{processStateValue}, 명령:{_commandValue}");

            if (processStateValue == _commandValue)
            {
                if (processStateValue == false)
                {
                    if (System.Windows.Forms.MessageBox.Show($"생산라인의 전원을 켜시겠습니까?", "Command Input", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _commandValue = true;
                        //testExecuteQuery = testOnQuery;
                        executeQuery = onCommandQuery;
                        needsUpdate = true;
                    }
                }
                else if (processStateValue == true)
                {
                    if (System.Windows.Forms.MessageBox.Show($"정말로 생산라인의 전원을 끄시겠습니까?", "Command Input", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _commandValue = false;
                        //testExecuteQuery = testOffQuery;
                        executeQuery = offCommandQuery;
                        needsUpdate = true;
                    }
                }
            }
            // 명령 취소
            else
            {
                if (processStateValue == false)
                {
                    if (System.Windows.Forms.MessageBox.Show($"명령을 취소하시겠습니까?(명령: 검사시작)", "Command Cancel", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _commandValue = false;
                        //testExecuteQuery = testOffQuery;
                        executeQuery = offCommandQuery;
                        needsUpdate = true;
                    }
                }
                else if (processStateValue == true)
                {
                    if (System.Windows.Forms.MessageBox.Show($"명령을 취소하시겠습니까?(명령: 검사중단)", "Command Cancel", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _commandValue = true;
                        //testExecuteQuery = testOnQuery;
                        executeQuery = onCommandQuery;
                        needsUpdate = true;
                    }
                }
            }
            //else
            //{
            //    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"생산라인의 작동 여부가 저장되어있지 않습니다.\r\n현재 생산라인이 작동 중입니까?", "Yes-No", MessageBoxButtons.YesNo);
            //    if (dialogResult == DialogResult.Yes)
            //        _isLinePower = true;
            //    else
            //        _isLinePower = false;
            //}

            if (needsUpdate == true)
            {
                //_dblink.Update(testExecuteQuery); // 테스트용 상태 업데이트
                _dblink.Update(executeQuery); // 명령 업데이트

                // 업데이트 날짜시간 갱신
                string updateDateTimeQuery = $@"UPDATE inspectionCurrentState
                                            SET lastUpdateDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'
                                            WHERE stateId = {_lineId};";
                _dblink.Update(updateDateTimeQuery);
            }

            return _commandValue;
        }
    }
}
