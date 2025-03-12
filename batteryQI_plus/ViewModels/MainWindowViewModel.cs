using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using batteryQI_plus.Views.UserControls;
using batteryQI_plus.Models;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace batteryQI_plus.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBases
    {
        // MainWindowView 우측 직원 리스트
        private Employee _employee; // 로그인한 직원
        //private Dictionary<int, List<KeyValuePair<string, string>>> _employeeList; // Line: list<pair직원 Id, 같이 출력할 Image 경로>
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
        //public Dictionary<int, List<KeyValuePair<string, string>>> EmployeeList
        //{
        //    get => _employeeList;
        //    set => SetProperty(ref _employeeList, value);
        //}
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
            EmployeeListInit();
            
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

        #region 직원 리스트 부분(급하게 하느라 정리 없이 하드코딩)
        public class EmployeeGroup
        {
            public int LineId { get; set; }
            public string DisplayRole => LineId == 0 ? "Manager" : $"Line {LineId}";
            public List<Employee_List> Employees { get; set; }
        }

        public class Employee_List
        {
            public string EmployeeId { get; set; }
            public string ImagePath { get; set; }
            public bool IsCurrentUser { get; set; } // 접속 여부, bool값은 기본값이 false
            public string BackgroundColor => IsCurrentUser ? "#FFD700" : "#FFFFFF";
        }

        private ObservableCollection<EmployeeGroup> _employeeGroups;
        public ObservableCollection<EmployeeGroup> EmployeeGroups
        {
            get => _employeeGroups;
            set => SetProperty(ref _employeeGroups, value);
        }
        
        // 직원 리스트 초기화 메소드
        public void EmployeeListInit()
        {
            _employeeGroups = new ObservableCollection<EmployeeGroup>();
            // employeeNum 테이블에서 employeeId 컬럼 조회.
            string sql = @$"SELECT employeeId, LineId FROM employees ORDER BY LineId;";
            List<Dictionary<string, object>> rows = _dblink.Select(sql);

            // 결과를 딕셔너리로 변환
            var employeeList = new Dictionary<int, List<KeyValuePair<string, string>>>();

            //foreach (var row in rows)
            //{
            //    int lineId = (int)(sbyte)row["LineId"];
            //    string employeeId = row["employeeId"].ToString();

            //    // LineId에 해당하는 리스트가 없으면 생성
            //    if (!employeeList.ContainsKey(lineId))
            //    {
            //        employeeList[lineId] = new List<KeyValuePair<string, string>>();
            //    }

            //    // 이미지 파일명 생성 (i + 2 대신 row의 인덱스를 사용하지 않고, 고유한 파일명 생성)
            //    string imageFile = string.Format("/Images/{0}.png", employeeId);

            //    // 딕셔너리에 추가
            //    employeeList[lineId].Add(new KeyValuePair<string, string>(employeeId, imageFile));
            //}

            for (int i = 0; i < rows.Count; i++)
            {
                int lineId = (int)(sbyte)rows[i]["LineId"];
                string employeeId = rows[i]["employeeId"].ToString();

                //// LineId에 해당하는 리스트가 없으면 생성
                //if (!employeeList.ContainsKey(lineId))
                //{
                //    employeeList[lineId] = new List<KeyValuePair<string, string>>();
                //}

                //// 이미지 파일명 생성
                //string imageFile = string.Format("/Images/{0}.png", i + 2);

                //// 딕셔너리에 추가
                //employeeList[lineId].Add(new KeyValuePair<string, string>(employeeId, imageFile));

                // LineId에 해당하는 그룹이 없으면 생성
                var group = _employeeGroups.FirstOrDefault(g => g.LineId == lineId);
                if (group == null)
                {
                    group = new EmployeeGroup { LineId = lineId, Employees = new List<Employee_List>() };
                    _employeeGroups.Add(group);
                }

                // 직원 정보 추가
                string imageFile = string.Format("/Images/{0}.png", i + 2);
                var employee = new Employee_List { EmployeeId = employeeId, ImagePath = imageFile };

                // 접속한 사용자 여부 설정
                if (employeeId == Employee.EmployeeID) // CurrentUserId는 접속한 사용자 ID
                {
                    employee.IsCurrentUser = true;
                }
                else { employee.IsCurrentUser = false; } // 굳이 할 필요 없는 듯하지만 혹시 몰라 안전장치로

                group.Employees.Add(employee);
            }

            //// EmployeeGroups에 데이터 할당
            //_employeeGroups = new ObservableCollection<EmployeeGroup>();
            //foreach (var kvp in employeeList)
            //{
            //    var group = new EmployeeGroup
            //    {
            //        LineId = kvp.Key,
            //        Employees = kvp.Value.Select(e => new Employee_List
            //        {
            //            EmployeeId = e.Key,
            //            ImagePath = e.Value
            //        }).ToList()
            //    };

            //    _employeeGroups.Add(group);
            //}
        }
        #endregion

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
