using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using batteryQI_plus.Models;
using batteryQI_plus.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace batteryQI_plus.ViewModels.Bases
{
    public partial class LoginViewModel : ViewModelBases
    {
        private Employee _employee = Employee.Instance();
        private readonly ViewModelLocator _viewModelLocator;
        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
        public LoginViewModel(ViewModelLocator viewModelLocator)
        {
           //MessageBox.Show(DateTime.Now.ToString()); // 시간 확인용 나중에 확인할 때 제거 예정
            //Manager 객체 생성
           _employee = Employee.Instance();
            // 로그인 창 열면서 DB 연결
            _dblink = DBlink.Instance();
            _dblink.Connect();
            _viewModelLocator = viewModelLocator; // viewModelLocater에 구현된 메소드 사용을 위해
        }

        private void PrintLoginPopup()
        {
            string loginPopupInput = _viewModelLocator.BringlastInspectionResultDuringLogOut();
            string pattern = @"\[.*?\]";
            string loginPopupOutput = "로그아웃동안 검사결과\r\n\r\n" + Regex.Replace(loginPopupInput, pattern, "");
            Console.WriteLine(loginPopupOutput);
            MessageBox.Show(loginPopupOutput, "Login Sucess", MessageBoxButton.OK,MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Login(object obj)
        {
            // DB가 제대로 연결되어 있고 PassBox가 안 비어져 있으면 수행
            if (_dblink.ConnectOk() && obj is PasswordBox pw)
            {
                List<Dictionary<string, object>> login = _dblink.Select($"SELECT * FROM employees WHERE employeeId='{Employee.EmployeeID}';");
                if (login.Count != 0 && (pw.Password == login[0]["employeePw"].ToString()))
                {
                    //MessageBox.Show("로그인 완료", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _employee.EmployeeNum = (int)(sbyte)login[0]["employeeNum"]; // 관리자 번호 저장
                    _employee.EmployeeID = login[0]["employeeId"].ToString(); // 관리자 아이디 저장
                    //_employee.WorkAmount = (int)login[0]["workAmount"]; // DB에 저장된 작업량 가져옴
                    _employee.EmployeeRole = (int)(sbyte)login[0]["employeeRole"]; // 권한 정보 저장
                    _employee.LineId = (int)(sbyte)login[0]["lineId"]; // 할당된 생산 라인 저장
                    _employee.LastLogoutDateTime = (DateTime)login[0]["lastLogoutDateTime"];

                    PrintLoginPopup(); // 로그인 팝업
                    // 권한에 따라 화면 구성 및 기능들 활성화 조절할 위치
                    try
                    {
                        //_dblink.Update($"UPDATE employees SET loginStatus = 1 WHERE employeeId='{Employee.EmployeeID}';"); // 임시 보류
                        // 임시 예상 로그인 팝업 삽입할 위치
                        _dblink.Update($"UPDATE employees SET lastLoginDateTime = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE employeeId='{Employee.EmployeeID}';"); 
                    }
                    catch 
                    {
                        MessageBox.Show("로그인 후 작업에서 실패했습니다");
                    }


        var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // 현재 창 닫기
                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    MessageBox.Show("아이디 및 비밀번호를 확인해 주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
