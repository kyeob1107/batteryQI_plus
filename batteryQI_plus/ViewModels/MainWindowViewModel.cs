using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using batteryQI_plus.Views.UserControls;
using batteryQI_plus.Models;
using System.Windows.Media;
using System.Windows;

namespace batteryQI_plus.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBases
    {
        // MainWindowView 우측 직원 리스트
        private Employee _employee; // 로그인한 직원
        private Dictionary<string, string> _employeeList; // 직원 Id, 같이 출력할 Image 경로
        // MainWindowView 중앙 화면 프레임
        private object _currentPage;
        private string _dbConnectState; // 연결상태 메시지
        private string _dbConnectionIcon; // 아이콘 이름 (FontAwesome 아이콘)
        private Brush _dbConnectionColor; // 아이콘 색상
        private readonly ViewModelLocator _viewModelLocator;

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
        public string DbConnectState
        {
            get => _dbConnectState;
            set => SetProperty(ref _dbConnectState, value);
        }
        public string DbConnectionIcon
        {
            get => _dbConnectionIcon;
            set => SetProperty(ref _dbConnectionIcon, value);
        }
        public Brush DbConnectionColor
        {
            get => _dbConnectionColor;
            set => SetProperty(ref _dbConnectionColor, value);
        }
        public Action? CloseAction { get; set; }

        public MainWindowViewModel(ViewModelLocator viewModelLocator)
        {
            // 로그인 직원, 직원 리스트 초기화
            _employee = Employee.Instance();
            _employeeList = EmployeeListInit();
            
            // 이벤트 구독
            _dblink.ConnectionStateChanged += OnDbConnectionStateChanged;
            // 연결상태 초기 상태 설정
            UpdateDbConnectionState();
            // 연결 모니터링 시작
            _dblink.MonitorConnection();
            //
            _viewModelLocator = viewModelLocator; // viewModelLocater에 구현된 메소드 사용을 위해

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

        // 로그아웃시 검사 결과 정리
        public void TotalInspectionResult()
        {
            string result = _viewModelLocator.BringTotalInspectionResultDuringLogIn();
            MessageBox.Show(result,"검사결과 요약",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        // 이벤트 핸들러
        private void OnDbConnectionStateChanged(object sender, EventArgs e)
        {
            UpdateDbConnectionState();
        }

        private void UpdateDbConnectionState()
        {
            if (_dblink != null && _dblink.ConnectOk())
            {
                DbConnectState = "Connected";
                DbConnectionIcon = "Link"; // "Database"; // FontAwesome 데이터베이스 아이콘
                DbConnectionColor = Brushes.Green;
            }
            else
            {
                DbConnectState = "Disconnected";
                DbConnectionIcon = "Unlink"; // "TimesCircle"; // FontAwesome 'X' 아이콘
                DbConnectionColor = Brushes.Red;
            }
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

        [RelayCommand]
        public void DBConnectButton()
        {
            //MessageBox.Show("연결 동작");
            _dblink.Connect();
        }
        [RelayCommand]
        private void DBDisconnectButton()
        {
            //MessageBox.Show("연결 해제");
            _dblink.Dispose();
        }
    }
}
