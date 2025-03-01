using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using System.Collections.ObjectModel;

namespace batteryQI_plus.ViewModels
{
    public partial class DashboardViewModel : ViewModelBases
    {
        // 접속 직원 정보용
        private Employee _employee = Employee.Instance();
        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }

        #region 타이머 부분
        // 주기적으로 동작하기 위한 Timer
        private DispatcherTimer _timer;
        public DispatcherTimer Timer
        {
            get => _timer;
            set => SetProperty(ref _timer, value);
        }
        private void InitializeTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;

            // 다음 실행 시간을 계산하고 타이머 시작
            ScheduleNextExecution();
        }

        private void ScheduleNextExecution()
        {
            DateTime now = DateTime.Now;
            int minutesUntilNextExecution = 10 - (now.Minute % 10);
            if (minutesUntilNextExecution == 0 && now.Second > 0)
            {
                minutesUntilNextExecution = 10;
            }

            DateTime nextExecution = now.AddMinutes(minutesUntilNextExecution)
                                        .AddSeconds(-now.Second)
                                        .AddMilliseconds(-now.Millisecond);

            TimeSpan delay = nextExecution - now;

            Timer.Interval = delay;
            Timer.Start();
        }
        #endregion

        int unitTestTimeMinites = 10; // 단위시간 검사 단위시간 값('분'단위 값) - 이 부분은 타이머랑 연동시킬 수 있으면 좋겠음

        // 단위시간 검사 모니터링용
        int numOfLinePlusOne; // 쿼리보내서 line 몇개 있는지 count 수
        private ObservableCollection<DashboardModel> _unitTest = new ObservableCollection<DashboardModel>();
        private ObservableCollection<string> _logContent = new ObservableCollection<string> ();
        
        // 단위시간 검사 합계용
        private ObservableCollection<DashboardModel> _totalUnitTest = new ObservableCollection<DashboardModel>();
        private ObservableCollection<string> _totalLogContent = new ObservableCollection<string>();

        public ObservableCollection<DashboardModel> UnitTest 
        {  
            get { return _unitTest; } 
            set { SetProperty(ref _unitTest, value); }
        }

        public ObservableCollection<string> LogContent
        {
            get { return _logContent; }
            set { SetProperty(ref _logContent, value); }
        }

        private void UpdateLogContent()
        {
            //for (int line = 1; line < numOfLinePlusOne; line++)
            //{

            //    this.LogContent[0] += $"<Line{line} | "
            //                        + $"{_unitTest[line].StartDatetime} ~ {_unitTest[line].EndDatetime}>"
            //                        + "\r\n"
            //                        + $"검사수: {_unitTest[line].InspectionCount}개 | 정상: {_unitTest[line].NormalCount}개 | "
            //                        + $"불량: {_unitTest[line].DefectCount}개 (불량률: {_unitTest[line].DefectRate}%) "
            //                        + "\r\n";

            //    this.LogContent[line] += $"<Line{line} | "
            //                        + $"{_unitTest[line].StartDatetime} ~ {_unitTest[line].EndDatetime}>"
            //                        + "\r\n"
            //                        + $"검사수: {_unitTest[line].InspectionCount}개 | 정상: {_unitTest[line].NormalCount}개 | "
            //                        + $"불량: {_unitTest[line].DefectCount}개 (불량률: {_unitTest[line].DefectRate}%) "
            //                        + "\r\n";
            //}
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            for (int line = 1; line < numOfLinePlusOne; line++)
            {
                string newLogEntry = $"[{timestamp}] <Line{line} | "
                                    + $"{_unitTest[line].StartDatetime} ~ {_unitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_unitTest[line].InspectionCount}개 | 정상: {_unitTest[line].NormalCount}개 | "
                                    + $"불량: {_unitTest[line].DefectCount}개 (불량률: {_unitTest[line].DefectRate}%) "
                                    + "\r\n\r\n";

                this.LogContent[0] += newLogEntry;
                this.LogContent[line] += newLogEntry;
            }
        }

        public ObservableCollection<DashboardModel> TotalUnitTest
        {
            get { return _totalUnitTest; }
            set { SetProperty(ref _totalUnitTest, value); }
        }

        public ObservableCollection<string> TotalLogContent
        {
            get { return _totalLogContent; }
            set { SetProperty(ref _totalLogContent, value); }
        }

        private void TotalUpdateLogContent()
        {
            // 전체탭 부분 지우기
            this.TotalLogContent[0] = "";

            // 전체에 그냥 합한 값으로 나오는 것도 추가할지 고민
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            for (int line = 1; line < numOfLinePlusOne; line++)
            {

                string newLogEntry = $"[{timestamp}] <Line{line} | "
                                    + $"{_totalUnitTest[line].StartDatetime} ~ {_totalUnitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_totalUnitTest[line].InspectionCount}개 | 정상: {_totalUnitTest[line].NormalCount}개 | "
                                    + $"불량: {_totalUnitTest[line].DefectCount}개 (불량률: {_totalUnitTest[line].DefectRate}%) "
                                    + "\r\n";
                this.TotalLogContent[0] += newLogEntry;
                this.TotalLogContent[line] = newLogEntry;
            }
        }


        public DashboardViewModel() 
        {
            // 로그아웃동안 검사결과에 대해 값 가져오기
            // 라인 몇개 있는지 확인해서 그것 수대로 model 생성
            var result = _dblink.Select($"SELECT COUNT(DISTINCT lineId) AS Count FROM batteryQIPlus.productionLines;");
            numOfLinePlusOne = Convert.ToInt32(result[0]["Count"]);
            //MessageBox.Show(num.ToString());
            //MessageBox.Show(_unitTest.Count + "\r\n" + LogContent.Count);

            InitializeTimer(); // 타이머 초기화

            _unitTest.Add(new DashboardModel());
            _logContent.Add("");
            _totalUnitTest.Add(new DashboardModel());
            _totalLogContent.Add("");

            for (int line = 1; line < numOfLinePlusOne; line++)
            {
                _unitTest.Add(new DashboardModel(_dblink, _employee, line));
                _logContent.Add("");
                _totalUnitTest.Add(new DashboardModel(_dblink, _employee, line, "total"));
                _totalLogContent.Add("");
            }
            UpdateLogContent();
            TotalUpdateLogContent();

            // 차트 데이터 초기화
            //var values = new ChartValues<double> { 3, 5, 2, 6, 2, 7, 1 };
            // 일단 0으로 초기화 후에 여유 있으면 이전 단위시간으로 쪼갠 값 가져와서 초기화
            var values = new ChartValues<double>(Enumerable.Repeat(0.0, numOfLinePlusOne));
            var values2 = new ChartValues<double>(Enumerable.Repeat(0.0, numOfLinePlusOne));
            var dates = new List<DateTime>
        {
            DateTime.Now.AddMinutes(-60),
            DateTime.Now.AddMinutes(-50),
            DateTime.Now.AddMinutes(-40),
            DateTime.Now.AddMinutes(-30),
            DateTime.Now.AddMinutes(-20),
            DateTime.Now.AddMinutes(-10),
            DateTime.Now
        };
            // 실시간 차트 부분
            checkTimeRangeMinites = 60; // 범위 : 60분
            numOfData = checkTimeRangeMinites / unitTestTimeMinites;
            dataListLiveChart = new List<List<double>>(numOfLinePlusOne - 1); // LiveChart y축 저장용
            SeriesCollectionLiveChart = new SeriesCollection();

            for (int i = 0; i < numOfLinePlusOne-1; i++)
            {
                var innerList = new List<double>(numOfData);
                innerList.AddRange(new double[numOfData]); // 일단 현재는 초기값 0으로 초기화
                dataListLiveChart.Add(innerList);
                SeriesCollectionLiveChart.Add(
                new LineSeries
                {
                    Title = $"Line{i+1}",
                    Values = innerList.AsChartValues(),
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }

            DateTimeLabels = dates.ConvertAll(d => d.ToString("yyyy-MM-dd HH:mm:ss")); // 이거 안쓰는듯?  

            DateTimeFormatter = value => dates[((int)value)].ToString("yyyy-MM-dd HH:mm:ss");
            YFormatter = value => value.ToString("N");
        }
        
        private void UpdateMonitoringLog()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int line = 1; line < numOfLinePlusOne; line++)
                {
                    //_unitTest[line] = new DashboardModel(_dblink, _employee, line); // 나중에 이부분 함수로 깔끔하게 다듬기
                    _unitTest[line].StartDatetime = _unitTest[line].EndDatetime;
                    _unitTest[line].EndDatetime = DateTime.Now.FloorToNearestMinutes(10);
                    // 일단 임시로 해둔 것
                    #region 검사 수 & 불량 수
                    string unitTestQuery1 = $@"SELECT 
                                                COUNT(DISTINCT batteryId) AS cnt
                                            FROM 
	                                            inspectionResults
                                            WHERE 
	                                            lineId = {line} 
	                                            AND (inspectionDatetime BETWEEN '{_unitTest[line].StartDatetime.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_unitTest[line].EndDatetime.ToString("yyyy-MM-dd HH:mm:ss")}');";
                    string unitTestQuery2 = $@"SELECT 
                                                COUNT(DISTINCT batteryId) AS cnt
                                            FROM 
	                                            inspectionResults
                                            WHERE 
	                                            lineId = {line} 
	                                            AND (inspectionDatetime BETWEEN '{_unitTest[line].StartDatetime.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_unitTest[line].EndDatetime.ToString("yyyy-MM-dd HH:mm:ss")}')
	                                            AND (fastPollutionCheck = 1 OR fastDamageCheck = 1);";
                    #endregion
                    List<Dictionary<string, object>> result1 = _dblink.Select(unitTestQuery1);
                    List<Dictionary<string, object>> result2 = _dblink.Select(unitTestQuery2);
                    _unitTest[line].InspectionCount = (result1.Count > 0) ? Convert.ToInt32(result1[0]["cnt"]) : 0;
                    _unitTest[line].DefectCount = (result2.Count > 0) ? Convert.ToInt32(result2[0]["cnt"]) : 0;
                    _unitTest[line].NormalCount = _unitTest[line].InspectionCount - _unitTest[line].DefectCount;
                }
                UpdateLogContent();
            });
        }

        private void UpdateProgressNTotalUnitTest()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int line = 1; line < numOfLinePlusOne; line++)
                {
                    _totalUnitTest[line].EndDatetime = _unitTest[line].EndDatetime;
                    _totalUnitTest[line].InspectionCount += _unitTest[line].InspectionCount;
                    _totalUnitTest[line].NormalCount += _unitTest[line].NormalCount;
                    _totalUnitTest[line].DefectCount += _unitTest[line].DefectCount;
                    // 불량률은 일단 위 값들 변경되면 자동 계산 다시해서 갱신하도록 설정 해뒀음
                }
                TotalUpdateLogContent();
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("타이머 Tick 작동\r\n" + $"{DateTime.Now}");
                // 화면에 보이는 값들 갱신
                UpdateMonitoringLog();
                UpdateProgressNTotalUnitTest();
                UpdateLiveChart();
            });
            
            // 다음 실행 시간 재설정
            ScheduleNextExecution();
        }

        [RelayCommand]
        private void RefreshButton()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("버튼 작동\r\n" + $"{DateTime.Now}");
                UpdateMonitoringLog();
                UpdateProgressNTotalUnitTest();
                UpdateLiveChart();
            });
        }

        public override void Dispose()
        {
            // DashboardViewModel의 리소스 정리
            Timer?.Stop();
            Timer = null;

            //// 기본 클래스의 Dispose 호출 (이것이 DBlink를 정리합니다)
            //base.Dispose();
        }

        int checkTimeRangeMinites; // 차트에 그릴 시간 범위('분'단위 값)
        int numOfData; // 차트에 그려질 값의 갯수 = checkTimeRangeMinites / unitTestTimeMinites
        private List<List<double>> dataListLiveChart; // 각 라인들별로 데이터값 담고 있는 리스트

        // 차트 그리는 파트 (솔직히 순서도 뒤죽박죽이라 리팩토링 확실히 필요할듯)
        private SeriesCollection _seriesCollectionLiveChart;
        public SeriesCollection SeriesCollectionLiveChart
        {
            get => _seriesCollectionLiveChart;
            set => SetProperty(ref _seriesCollectionLiveChart, value);
        }

        private List<string> _dateTimeLabels;
        public List<string> DateTimeLabels
        {
            get => _dateTimeLabels;
            set => SetProperty(ref _dateTimeLabels, value);
        }

        public Func<double, string> DateTimeFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void UpdatedataListLiveChart()
        {
            for (int i = 0; i < numOfLinePlusOne-1; i++)
            {
                Console.WriteLine();
                //int line = i + 1; 
                if (dataListLiveChart[i].Count >= numOfData)
                {
                    dataListLiveChart[i].RemoveAt(0); // 가장 오래된 데이터 제거
                }
                dataListLiveChart[i].Add(_unitTest[i+1].DefectRate); // 새 데이터 추가
            }
        }

        private void UpdateLiveChart()
        {
            UpdatedataListLiveChart();
            for (int i = 0; i < numOfLinePlusOne-1; i++)
            {
                SeriesCollectionLiveChart[i].Values = dataListLiveChart[i].AsChartValues();
            }
        }


    }
}
