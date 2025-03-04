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
            getLineSetting();
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
            getSettingItemList(); // 설정 편집창의 설정 옵션 목록 초기화
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

        [RelayCommand] private void CheckButtonClick(object sender) // 변경된 설정 저장 메소드
        {
            if (System.Windows.Forms.MessageBox.Show($"변경된 설정을 저장하시겠습니까?", "Yes-No", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 선택한 설정 옵션들을 실제 프로퍼티, DB에 반영
                for (int i = 0; i < LineSettingCollection.Count; i++)
                {
                    if (LineSettingCollection[i]["lineId"] == _selectedLineSetting["lineId"])
                    {
                        // View의 DatePicker의 SelectedDate 값이 미국식 날짜 format인 "MM/dd/yyyy hh:mm:ss tt"로만 반환되는 문제.
                        // string -> DateTime -> string 변환으로 원하는 "yyyy-MM-dd" format으로 변환.
                        // CultureInfo.InvariantCulture: 날짜 변환 중 문화권 형식에 의존하지 않도록 강제하는 옵션
                        _selectedLineSetting["deadlineStart"] = _selectedLineSetting["deadlineStart"] != "" ? DateTime.Parse(_selectedLineSetting["deadlineStart"], CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                        _selectedLineSetting["deadlineEnd"] = _selectedLineSetting["deadlineEnd"] != "" ? DateTime.Parse(_selectedLineSetting["deadlineEnd"], CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";

                        // 변경된 설정 DB 업데이트


                        // 업데이트된 Setting 데이터를 가져와 Setting View UI 업데이트(현재는 C# 내부에서 업데이트, 시간 여유 있으면 DB에서 가져오는 걸로 변경)
                        int tempSelectedTabIndex = SelectedTabIndex; // 설정 편집 이전 조회중이던 Tab 위치 저장
                        SelectedTabIndex = -1; // 설정 Tab 선택 초기화.
                        foreach (var setting in _selectedLineSetting) // 얕은 복사
                        {
                            LineSettingCollection[i][setting.Key] = setting.Value;
                        }
                        //LineSettingCollection[i] = new ProductionLine(_selectedLineSetting); // 깊은 복사
                        SelectedTabIndex = tempSelectedTabIndex; // 설정 편집 이전 조회중이던 Tab 위치로 재이동. 재이동을 통해 Tab의 새로고침 유도

                        break;
                    }
                }
                CloseAction?.Invoke();
            }
        }
    }

    // 더이상 사용하지않는 레거시 코드 정리용.
    public partial class SettingViewModel : ViewModelBases 
    {
        //private string _manufacName = "";
        //private IDictionary<string, string> _manufacDict = new Dictionary<string, string>();
        //private ObservableCollection<KeyValuePair<string, string>> _manufacCollection = new ObservableCollection<KeyValuePair<string, string>>();
        //private int _newAmount;

        //public IDictionary<string, string> ManufacDict
        //{
        //    get => _manufacDict;
        //}
        //public string ManufacName
        //{
        //    get => _manufacName;
        //    set => SetProperty(ref _manufacName, value);
        //}

        //public ObservableCollection<KeyValuePair<string, string>> ManufacCollection
        //{
        //    get => _manufacCollection;
        //    set => SetProperty(ref _manufacCollection, value);
        //}
        //public int NewAmount
        //{
        //    get => _newAmount;
        //    set => SetProperty(ref _newAmount, value);
        //}
        //private void getManafactureNameID() // DB에서 제조사 리스트 가져오기
        //{
        //    // DB에서 가져와서 리스트 초기화하기, ID는 안 가져오고 Name만 추가
        //    _manufacCollection.Clear();
        //    List<Dictionary<string, object>> ManufactureList_Raw = _dblink.Select("SELECT * FROM manufacture order by manufacId ASC;");
        //    foreach (var row in ManufactureList_Raw)
        //    {
        //        string name = row["manufacName"].ToString();
        //        string id = row["manufacId"].ToString();
        //        _manufacCollection.Add(new KeyValuePair<string, string>(name, id));
        //    }
        //}

        //[RelayCommand]
        //private void ManufactInsert()
        //{
        //    // 제조사 인풋
        //    try
        //    {
        //        if (_dblink.ConnectOk())
        //        {
        //            if (ManufacName != "")
        //            {
        //                _dblink.Insert($"INSERT INTO manufacture (manufacId, manufacName) VALUES(0, '{ManufacName}');");
        //                _manufacDict.Clear();
        //                getManafactureNameID();
        //                System.Windows.MessageBox.Show("완료");
        //            }
        //            else
        //            {
        //                System.Windows.MessageBox.Show("제조사를 입력해주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        System.Windows.MessageBox.Show("입력 오류", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //[RelayCommand]
        //private void amountSaveButton_Click()
        //{
        //    // 월 검사 할당량 수정 이벤트
        //    if (_dblink.ConnectOk())
        //    {
        //        if (System.Windows.MessageBox.Show($"할당량을 {_newAmount}로 변경할까요?", "warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //        {
        //            //_manager.WorkAmount = _newAmount;
        //            _dblink.Update($"UPDATE manager SET workAmount={_manager.WorkAmount} WHERE managerId='{_manager.EmployeeID}';");
        //            System.Windows.MessageBox.Show($"할당량을 {_manager.WorkAmount}로 수정 완료!");
        //        }
        //    }
        //    else
        //    {
        //        System.Windows.MessageBox.Show("DB 연결 오류", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //public string completeAmount()
        //{
        //    try
        //    {
        //// 분석 완료 개수 가져오기
        //string query = @$"
        //        SELECT COUNT(*) 
        //        FROM batteryInfo
        //        WHERE DATE_FORMAT(shootDate, '%Y-%m') = DATE_FORMAT(NOW(), '%Y-%m')
        //        AND ManagerNum = {_manager.EmployeeNum};
        //        ";

        // 데이터베이스 연결 및 쿼리 실행
        //var result = _dblink.Select(query);

        //// 데이터가 있는 경우
        //if (result != null && result.Count > 0)
        //{
        //    // 첫 번째 결과를 문자열로 변환
        //    return result[0]["COUNT(*)"]?.ToString() ?? "0";
        //}
        //else
        //{
        //    return "0"; // 데이터가 없는 경우
        //}
        //}
        //catch (Exception ex)
        //{
        //    // 오류 발생 시 처리
        //    System.Windows.MessageBox.Show($"작업량 데이터 가져오기 실패", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    return "Error";
        //}
        //}
    }
}
