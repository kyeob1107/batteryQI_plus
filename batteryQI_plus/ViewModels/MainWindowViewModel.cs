using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using batteryQI_plus.Views.UserControls;
using batteryQI_plus.Models;

namespace batteryQI_plus.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBases
    {
        // MainWindowView 우측 직원 리스트
        private Employee _employee; // 로그인한 직원
        private Dictionary<string, string> _employeeList; // 직원 Id, 같이 출력할 Image 경로
        // MainWindowView 중앙 화면 프레임
        private object _currentPage;

        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
        public Dictionary<string, string> EmployeeList
        {
            get => _employeeList;
            set => SetProperty(ref _employeeList, value);
        }
        public object CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }
        public Action? CloseAction { get; set; }

        public MainWindowViewModel()
        {
            // 로그인 직원, 직원 리스트 초기화
            _employee = Employee.Instance();
            _employeeList = EmployeeListInit();

            // 초기 화면 설정
            _currentPage = new DashboardView();
        }

        // 직원 리스트 초기화 메소드
        public Dictionary<string, string> EmployeeListInit()
        {
            // employeeNum 테이블에서 employeeId 컬럼 조회.
            string sql = $"SELECT employeeId FROM employees WHERE employeeNum <> {Employee.EmployeeNum};";
            List<Dictionary<string, object>> rows = _dblink.Select(sql);

            // 각 행에서 "employeeId" 값만 추출하여 List<string>으로 변환
            //IList<string> employeeIds = rows.Select(row => row["employeeId"].ToString()).ToList();

            Dictionary<string, string> employeeList = new Dictionary<string, string>();
            for (int i = 0; i < rows.Count; i++)
            {
                employeeList.Add(rows[i]["employeeId"].ToString(), string.Format("/Images/{0}.png", i + 2));
            }
            return employeeList;
        }
        
        [RelayCommand]
        private void HomeButton()
        {
            CurrentPage = new DashboardView();
        }
        
        [RelayCommand]
        private void ChartButton()
        {
            CurrentPage = new AnalysisView();
        }
        
        [RelayCommand]
        private void ManagerButton()
        {
            CurrentPage = new SettingView();
        }

        [RelayCommand]
        private void ExitButton()
        {
            _dblink.Dispose(); // DB 연결 끊기
            CloseAction?.Invoke();
        }
    }
}
