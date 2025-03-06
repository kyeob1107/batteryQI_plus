using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Data.Common;
using batteryQI_plus.Models;
using System.Windows.Forms;
using Mysqlx.Crud;
using batteryQI_plus.ViewModels.Bases;
using System.Windows;
using System.Data;
using System.Collections.ObjectModel;
using batteryQI_plus.Views;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Windows.Shapes;

namespace batteryQI_plus.ViewModels
{
    // 설정 조회, 설정 편집 페이지
    public partial class SettingViewModel : ViewModelBases
    {
        private Employee _employee;
        private bool? _isLinePower; // 생산라인 전원
        
        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
        
        public SettingViewModel()
        {
            _employee = Employee.Instance();
            _isEditSettingRole = _employee.EmployeeRole >= 10 ? true : false; // _employee.EmployeeRole이 10 이상이라면 설정 편집창 열기 권한 허용.
            getLineSetting(); // 설정 조회창의 설정 목록 초기화
            getSettingItemList(); // 설정 편집창의 설정 옵션 목록 초기화
        }
        public bool? IsLinePower // 생산라인 전원 프로퍼티
        {
            get => _isLinePower;
            set
            {
                SetProperty(ref _isLinePower, LinePower());
            }
        }
        public Action? CloseAction { get; set; } // 페이지 닫기 프로퍼티

        [RelayCommand] private void OpenEditSetting() // 설정 편집창 열기
        {
            SelectedLineId = GetTabByLineId(_selectedTabIndex);
            EditSettingView editSettingPage = new EditSettingView();
            editSettingPage.ShowDialog();
        }

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

        [RelayCommand] private void SaveLineSettingButtonClick(object sender) // 변경된 설정 저장 메소드
        {
            if (_selectedLineSetting.LineId != "") // 생산라인 Id를 선택했을 때
            {
                if (System.Windows.Forms.MessageBox.Show($"설정을 저장하시겠습니까?", "Yes-No", MessageBoxButtons.YesNo) == DialogResult.Yes) // 설정 저장 여부를 묻는 메시지
                {
                    // 설정 옵션 선택을 통해 설정을 변경한 생산라인을 실제 프로퍼티, DB에 반영
                    for (int lineSequence = 0; lineSequence < LineSettingCollection.Count; lineSequence++)
                    {
                        // 설정을 변경한 생산라인을 기존 설정의 생산라인과 매칭
                        if (LineSettingCollection[lineSequence]["lineId"] == _selectedLineSetting["lineId"])
                        {
                            // View의 DatePicker의 SelectedDate 값이 미국식 날짜 format인 "MM/dd/yyyy hh:mm:ss tt"로만 반환되는 문제.
                            // string -> DateTime -> string 변환으로 원하는 "yyyy-MM-dd" format으로 변환.
                            // CultureInfo.InvariantCulture: 날짜 변환 중 문화권 형식에 의존하지 않도록 강제하는 옵션
                            _selectedLineSetting["deadlineStart"] = _selectedLineSetting["deadlineStart"] != "" ? DateTime.Parse(_selectedLineSetting["deadlineStart"], CultureInfo.InvariantCulture)
                                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                            _selectedLineSetting["deadlineEnd"] = _selectedLineSetting["deadlineEnd"] != "" ? DateTime.Parse(_selectedLineSetting["deadlineEnd"], CultureInfo.InvariantCulture)
                                .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";

                            // 변경된 설정 DB 업데이트
                            // DB 업데이트에 사용할 쿼리문 구성(Set)에 사용할 Key 목록
                            // lineId는 별도 처리, buyerName은 DB의 ProductionLine 테이블에 없는 컬럼이기 때문에 생략
                            string[] updateKeys = _selectedLineSetting.Keys.Where(key => key != "lineId" && key != "buyerName").ToArray();
                            // 업데이트할 열의 할당 구문을 저장할 리스트
                            List<string> updateSettingColumns = new List<string>();
                            foreach (var key in updateKeys)
                            {
                                // 기존 값과 변경된 값이 다를 때만 업데이트
                                if (_selectedLineSetting[key] != LineSettingCollection[lineSequence][key])
                                {
                                    string newSettingValue = _selectedLineSetting[key];

                                    if (string.IsNullOrEmpty(newSettingValue)) // 값이 빈 문자열이면 NULL로 업데이트
                                    {
                                        updateSettingColumns.Add($"{key} = NULL");
                                    }
                                    else
                                    {
                                        if (int.TryParse(newSettingValue, out int newSettingValueNum)) // 숫자형 DB 데이터일 경우
                                            updateSettingColumns.Add($"{key} = {newSettingValueNum}");
                                        else
                                            updateSettingColumns.Add($"{key} = '{newSettingValue}'");
                                    }
                                }
                            }
                            if (updateSettingColumns.Count > 0) // 업데이트할 열이 하나 이상
                            {
                                string setClause = string.Join(", ", updateSettingColumns); // SET 구문 생성
                                                                                            // 최종 sql 명령문 완성. Set 구문과 lineId 기준 Where 구문 결합.
                                string sql = $"UPDATE productionLines SET {setClause} WHERE lineId = {_selectedLineSetting["lineId"]}";

                                // 최종 완성한 sql을 사용하여 DB 업데이트 수행
                                _dblink.Update(sql);
                            }

                            // 업데이트된 Setting 데이터와 Setting View UI 매칭(현재는 C# 내부에서 매칭 수행, 시간 여유 있으면 DB에서 가져오는 걸로 변경)
                            int tempSelectedTabIndex = SelectedTabIndex; // 설정 편집 이전 조회중이던 Tab 위치 저장
                            SelectedTabIndex = -1; // 설정 Tab 선택 초기화.
                            foreach (var setting in _selectedLineSetting) // 얕은 복사
                            {
                                if (LineSettingCollection[lineSequence][setting.Key] != setting.Value)
                                    LineSettingCollection[lineSequence][setting.Key] = setting.Value;
                            }
                            SelectedTabIndex = tempSelectedTabIndex; // 설정 편집 이전 조회중이던 Tab 위치로 재이동. 재이동을 통해 Tab의 새로고침 유도

                            break;
                        }
                    }
                    CloseAction?.Invoke(); // EditSetting View 닫기
                }
            }
        }

        private string GetTabByLineId(int selectedTabIndex) // Setting View에서 선택한 Tab의 lineId를 찾아 반환하는 메소드
        {
            if (selectedTabIndex < 0) // 선택한 Tab의 Index가 아무것도 선택하지 않은 상태(-1)일 경우 빈 문자열 반환
                return string.Empty;
            return LineSettingCollection[selectedTabIndex].LineId;
        }
    }
}
