using System.Windows;
using System.Collections.ObjectModel;
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using MySql.Data.MySqlClient;
using System.Data;
using System.ComponentModel;
using ZstdSharp.Unsafe;
using System.Windows.Shapes;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel : ViewModelBases
    {
        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private string _startTime;
        public string StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _endTime;
        public string EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private DateTime _dateRangeStart;
        public DateTime DateRangeStart
        {
            get => _dateRangeStart;
            set => SetProperty(ref _dateRangeStart, value);
        }

        private DateTime _dateRangeEnd;
        public DateTime DateRangeEnd
        {
            get => _dateRangeEnd;
            set => SetProperty(ref _dateRangeEnd, value);
        }

        private ObservableCollection<AnalysisModel> _timeChart = new ObservableCollection<AnalysisModel>();
        public ObservableCollection<AnalysisModel> TimeChart
        {
            get { return _timeChart; }
            set { SetProperty(ref _timeChart, value); }
        }

        // 이름 수정해야할 듯
        private void InitializeMultipleTimeCharts(int numOfLine, string filter = "TRUE")
        {
            // 초기화 필요해서 일단 당장 이 방식으로 해줬음
            _timeChart = new ObservableCollection<AnalysisModel>();
            for (int line = 0; line < numOfLine; line++)
            {
                Console.WriteLine($"라인{line+1}입니다");

                // 임시용
                string startDateForQuery = GetStartDateTime().ToString("yyyy-MM-dd HH:mm:ss"); // "2025-03-03 09:00";
                string endDateForQuery = GetEndDateTime().ToString("yyyy-MM-dd HH:mm:ss"); //"2025-03-03 23:50";
                string query_timeChart = @$"WITH RECURSIVE TimeIntervals AS (
                                                SELECT CAST('{startDateForQuery}' AS DATETIME) AS time_interval
                                                UNION ALL
                                                SELECT time_interval + INTERVAL 30 MINUTE
                                                FROM TimeIntervals
                                                WHERE time_interval < '{endDateForQuery}'
                                            )
                                            SELECT 
                                                t.time_interval,
                                                COALESCE(SUM(CASE WHEN subquery.Status = 'normal' THEN 1 ELSE 0 END), 0) AS normal_cnt,
                                                COALESCE(SUM(CASE WHEN subquery.Status = 'defect' THEN 1 ELSE 0 END), 0) AS defect_cnt
                                            FROM 
                                                TimeIntervals t
                                            LEFT JOIN (
                                                SELECT 
                                                    CONCAT(DATE_FORMAT(inspectionDatetime, '%Y-%m-%d %H:'), 
                                                            LPAD(FLOOR(MINUTE(inspectionDatetime) / 30) * 30, 2, '0')) AS time_interval,
                                                    batteryId, 
                                                    CASE
                                                        WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck) = 0 THEN 'normal'
                                                        ELSE 'defect'
                                                    END AS Status
                                                FROM batteryQIPlus.inspectionResults
                                                WHERE lineid = {line+1} AND {filter}
                                                GROUP BY time_interval, batteryId
                                            ) AS subquery ON t.time_interval = subquery.time_interval
                                            GROUP BY t.time_interval
                                            ORDER BY t.time_interval;";
                Console.WriteLine("타임차트: " + query_timeChart);
                var queryResult = _dblink.Select(query_timeChart);
                //foreach (string key in queryResult[0].Keys) { Console.WriteLine(key); }
                AnalysisModel model = new AnalysisModel("timeChart", queryResult);
                _timeChart.Add(model);
            }
            
            // 이부분 원래 함수 밖에 있다가 함수로 옮김 이부분 확인다시 해봐야함
            var dates = _timeChart[0].TimeList; // new List<DateTime>();
            DateTimeFormatter = value => dates[((int)value)].ToString("yyyy-MM-dd HH:mm:ss");
        }

        public AnalysisViewModel()
        {
            string query_dateRange = @"SELECT 
                                        MIN(Date(inspectionDatetime)) AS minDate, 
                                        MAX(Date(inspectionDatetime)) AS maxDate 
                                       FROM inspectionResults;";
            var result_DateRange = _dblink.Select(query_dateRange);
            DateRangeStart = (DateTime)result_DateRange[0]["minDate"];
            DateRangeEnd = (DateTime)result_DateRange[0]["maxDate"];
            // 초기값 설정
            //StartDate = DateTime.Today; // 값수정 필요
            StartDate = DateRangeStart;
            StartTime = "00:00:00";
            //EndDate = DateTime.Today; // 값수정 필요
            EndDate = DateRangeEnd;
            EndTime = "23:59:59";

            // Initialize UsageItems with sample data
            UsageItems = new ObservableCollection<SelectableItem>();
            var UsageFilterlist = SelectFilterValue("usageName");
            for (int i = 0; i < UsageFilterlist.Count; i++) 
            {
                UsageItems.Add(
                    new SelectableItem 
                    { 
                        Name = (string)UsageFilterlist[i]["usageName"], IsSelected = true 
                    });
            }

            //buy는 일단 통일성있게 해주려고 id로 해주고 buyer테이블 참고해서 이름으로 표시하거나 하는 과정 추가해주기
            BuyerItems = new ObservableCollection<SelectableItem>();
            var BuyerFilterlist = SelectFilterValue("buyerName", "batteryInfo bi INNER JOIN buyers b ON bi.buyerId = b.buyerId;");
            for (int i = 0; i < BuyerFilterlist.Count; i++)
            {
                BuyerItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BuyerFilterlist[i]["buyerName"],
                        IsSelected = true
                    });
            }

            BatteryTypeItems = new ObservableCollection<SelectableItem>();
            var BatteryTypeFilterlist = SelectFilterValue("batteryType");
            for (int i = 0; i < BatteryTypeFilterlist.Count; i++)
            {
                BatteryTypeItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BatteryTypeFilterlist[i]["batteryType"],
                        IsSelected = true
                    });
            }

            BatteryShapeItems = new ObservableCollection<SelectableItem>();
            var BatteryShapeFilterlist = SelectFilterValue("batteryShape");
            for (int i = 0; i < BatteryShapeFilterlist.Count; i++)
            {
                BatteryShapeItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BatteryShapeFilterlist[i]["batteryShape"],
                        IsSelected = true
                    });
            }

            StatusItems = new ObservableCollection<SelectableItem>
            {
                new SelectableItem { Name = "정상", IsSelected = true },
                new SelectableItem { Name = "오염", IsSelected = true },
                new SelectableItem { Name = "파손", IsSelected = true }
            };

            ProductionLineItems = new ObservableCollection<SelectableItem>();
            var ProductionLineFilterlist = SelectFilterValue("lineId");
            for (int i = 0; i < ProductionLineFilterlist.Count; i++)
            {
                ProductionLineItems.Add(
                    new SelectableItem
                    {
                        Name = "Line" + ProductionLineFilterlist[i]["lineId"].ToString(),
                        IsSelected = true
                    });
            }

            BatteryId = "";

            // 타임차트

            var lineCountquery = _dblink.Select($"SELECT COUNT(DISTINCT lineId) AS Count FROM productionLines;");
            numOfLine = Convert.ToInt32(lineCountquery[0]["Count"]) - 1;
            //for (int line = 0; line < numOfLine; line++)
            //{
            //    _timeCharts.Add(new AnalysisModel
            //    {
            //        TimeChartX.Add(),
            //        DefectRate.Add(),
            //    });
            //}
            //SeriesCollectionTimeChart = new SeriesCollection();
            
            InitializeMultipleTimeCharts(numOfLine);

            SeriesCollectionTimeChart = new SeriesCollection();
            for (int i = 0; i < numOfLine; i++)
            {
                var innerList = new List<double>();
                SeriesCollectionTimeChart.Add(
                new LineSeries
                {
                    Title = $"Line{i + 1}",
                    //Values = innerList.AsChartValues(),
                    Values = new ChartValues<double>(_timeChart[i].CountValue),
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }
            
            //DateTimeFormatter = value =>
            //{
            //    try
            //    {
            //        if (!(value is int))
            //            throw new InvalidCastException($"Value is not an integer: {value},{value.GetType()}");

            //        int index = (int)value;

            //        if (index < 0 || index >= dates.Count)
            //            throw new IndexOutOfRangeException($"Index {index} is out of range for dates array.");

            //        return dates[index].ToString("yyyy-MM-dd HH:mm:ss");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error occurred. Value: {value}, Exception: {ex}");
            //        throw;
            //    }
            //};
            YFormatter = value => value.ToString("N");

            // 파이차트
            // 값 가져오는 할당해서 만드는 함수화 하는게 보기 편할듯
            // 쿼리 엄밀하게 배터리 수가 아니라 그냥 이미지 수로 때려박는거라 정확하지 않음 다시짜줘야함
            // 할려면 미리 배터리별로 groupby해서 check결과 합해서 0인지 확인하는 방식으로 서브쿼리로 짜줘야할듯
            string query_Pie = @$"SELECT Status, COUNT(batteryId) AS batteryCount
	                            FROM (SELECT batteryId,
			                            CASE
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN 'normal'
				                            WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN 'pollution'
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN 'damage'
				                            ELSE 'pollution & damage'
			                            END AS Status
		                            FROM inspectionResults
		                            GROUP BY batteryId) AS subquery
	                            GROUP BY Status;";
            var result_Pie = _dblink.Select(query_Pie);

            SeriesCollectionPie = new SeriesCollection();
            
            foreach (var item in result_Pie)
            {
                SeriesCollectionPie.Add(new PieSeries
                {
                    Title = (string)item["Status"],
                    Values = new ChartValues<double> { Convert.ToDouble(item["batteryCount"]) },
                    DataLabels = true //default값이 true인듯
                });
            }

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            // column차트 - 따로 메소드 만들어서 사용하는 식으로 해야할듯
            string query_column = $@"WITH StatusList AS (
                                            SELECT * FROM (
                                            VALUES 
                                                ROW('normal'),
                                                ROW('pollution'),
                                                ROW('damage'),
                                                ROW('pollution & damage')
                                            ) AS t(Status)
                                        ),
                                        LineList AS (
                                            SELECT DISTINCT lineId
                                            FROM batteryQIPlus.inspectionResults
                                        )
                                        SELECT l.lineId, sl.Status, COALESCE(COUNT(s.batteryId), 0) AS Count
                                        FROM LineList l
                                        CROSS JOIN StatusList sl
                                        LEFT JOIN (
                                                SELECT lineId, batteryId,
                                                CASE
                                                    WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN 'normal'
                                                    WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN 'pollution'
                                                    WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN 'damage'
                                                    ELSE 'pollution & damage'
                                                END AS Status
                                            FROM batteryQIPlus.inspectionResults
                                            GROUP BY lineId, batteryId) AS s ON l.lineId = s.lineId AND sl.Status = s.Status
                                        GROUP BY l.lineId, sl.Status
                                        ORDER BY l.lineId, 
                                                CASE
                                                    WHEN sl.Status = 'normal' THEN 0
                                                    WHEN sl.Status = 'pollution' THEN 1
                                                    WHEN sl.Status = 'damage' THEN 2
                                                    ELSE 3
                                                END;";
            // LEFT JOIN 뒤에 적은 테이블부분쪽에 조건문 추가하기
            //     WHERE inspectionDatetime BETWEEN '2025-02-28 22:10:00' AND '2025-03-02 12:30:00'

            List<Dictionary<string, object>> result_column = _dblink.Select(query_column);
            
            int numOfStatus = 4;
            int numOfLineColumn = result_column.Count / numOfStatus; // 일단 임시로 선언, 나중에 통일시켜도 될듯
            SeriesCollectionColumn = new SeriesCollection();
            List<int> valueListColumn = new List<int>(); // 값 저장해서 chartvalues로 설정할 때 쓸 리스트
            for (int s = 0; s < numOfStatus; s++)
            {
                valueListColumn.Clear();
                for (int line = 0; line < numOfLineColumn; line++) 
                {
                    valueListColumn.Add(Convert.ToInt32(result_column[line * numOfStatus + s]["Count"]));
                }
                SeriesCollectionColumn.Add(new ColumnSeries
                {
                    Title = (string)result_column[s]["Status"],
                    //Values = valueListColumn.AsChartValues()
                    Values = new ChartValues<int>(valueListColumn),
                    DataLabels = true
                });
            }

            //LabelsColumn = new[] { "Line1", "Line2", "Line3"};
            #region 다른 방식
            //// LINQ사용방식
            //LabelsColumn = Enumerable.Range(1, numOfLineColumn)
            //                      .Select(i => $"Line{i}")
            //                      .ToArray();

            //// List<T>를 사용한 동적 생성
            //List<string> labelsList = new List<string>();
            //for (int i = 0; i < numOfLineColumn; i++)
            //{
            //    labelsList.Add($"Line{i + 1}");
            //}
            //LabelsColumn = labelsList.ToArray();

            // Array.ConvertAll 메서드 방식
            //LabelsColumn = Array.ConvertAll(new int[numOfLineColumn], i => $"Line{i + 1}");
            #endregion

            // 전통적인 for 루프
            LabelsColumn = new string[numOfLineColumn];
            for (int i = 0; i < numOfLineColumn; i++)
            {
                LabelsColumn[i] = $"Line{i + 1}";
            }
            FormatterColumn = value => value.ToString("N0");

            // 테이블
            LoadData();

        }

        // 시간을 파싱하고 유효성을 검사하는 메서드
        private DateTime CombineDateAndTime(DateTime date, string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan timeSpan))
            {
                return date.Date + timeSpan;
            }
            return date;
        }

        // 날짜와 시간을 결합하여 완전한 DateTime을 얻는 메서드
        public DateTime GetStartDateTime()
        {
            return CombineDateAndTime(StartDate, StartTime);
        }

        public DateTime GetEndDateTime()
        {
            return CombineDateAndTime(EndDate, EndTime);
        }

        private ObservableCollection<SelectableItem> _usageItems;
        public ObservableCollection<SelectableItem> UsageItems
        {
            get => _usageItems;
            set => SetProperty(ref _usageItems, value);
        }

        private ObservableCollection<SelectableItem> _buyerItems;
        public ObservableCollection<SelectableItem> BuyerItems
        {
            get => _buyerItems;
            set => SetProperty(ref _buyerItems, value);
        }

        private ObservableCollection<SelectableItem> _batteryTypeItems;
        public ObservableCollection<SelectableItem> BatteryTypeItems
        {
            get => _batteryTypeItems;
            set => SetProperty(ref _batteryTypeItems, value);
        }

        private ObservableCollection<SelectableItem> _batteryShapeItems;
        public ObservableCollection<SelectableItem> BatteryShapeItems
        {
            get => _batteryShapeItems;
            set => SetProperty(ref _batteryShapeItems, value);
        }

        private ObservableCollection<SelectableItem> _statusItems;
        public ObservableCollection<SelectableItem> StatusItems
        {
            get => _statusItems;
            set => SetProperty(ref _statusItems, value);
        }

        private ObservableCollection<SelectableItem> _productionLineItems;
        public ObservableCollection<SelectableItem> ProductionLineItems
        {
            get => _productionLineItems;
            set => SetProperty(ref _productionLineItems, value);
        }

        private List<Dictionary<string,object>> SelectFilterValue(string category, string table = "batteryInfo")
        {
            string query = $"SELECT DISTINCT {category} FROM {table};";
            var FilterValueList = _dblink.Select(query);
            return FilterValueList;
        }

        private string _batteryId;
        public string BatteryId
        {
            get { return _batteryId; }
            set 
            { 
                SetProperty(ref _batteryId, value);
                //MessageBox.Show(BatteryId); // 디버그용
            }
        }

        private List<string> FilterBatteryIds()
        {
            Console.WriteLine("필터메소드 작동\r\n");
            string filter = "";
            filter += @$"(inspectionDatetime BETWEEN '{GetStartDateTime().ToString("yyyy-MM-dd HH:mm:ss")}' 
                        AND '{GetEndDateTime().ToString("yyyy-MM-dd HH:mm:ss")}')";
            /*
            _usageItems
            _buyerItems
            _batteryTypeItems
            _batteryShapeItems
            ------------------------------------------------------------
            _statusItems - 이건 그냥하면 안됨
            _productionLineItems - pie는 라인을 데이터 가져올때 용
             */
            List<string> filteredBatteryIds =  new List<string>();
            Dictionary<string, List<string>> checkedList = new Dictionary<string, List<string>>
            {
                { "usageName", new List<string>()},
                { "buyerName", new List<string>()},
                { "batteryType", new List<string>()},
                { "batteryShape", new List<string>()}
            };
            // usageItems
            foreach (var item in _usageItems)
            {
                if (item.IsSelected) { checkedList["usageName"].Add(item.Name); }
            }
            foreach (var item in _buyerItems)
            {
                if (item.IsSelected) { checkedList["buyerName"].Add(item.Name); }
            }
            foreach (var item in _batteryTypeItems)
            {
                if (item.IsSelected) { checkedList["batteryType"].Add(item.Name); }
            }
            foreach (var item in _batteryShapeItems)
            {
                if (item.IsSelected) { checkedList["batteryShape"].Add(item.Name); }
            }

            foreach (KeyValuePair<string, List<string>> item in checkedList)
            {
                if (item.Value.Count > 0) 
                {
                    if (filter != "") filter += " AND ";
                    filter += $"{item.Key} IN ({string.Join(", ", item.Value.Select(v => $"\'{v}\'"))})";

                }
            }
            if (filter == "") { filter = "False"; }
            Console.WriteLine(filter);
            string query_filter = @$"SELECT batteryId 
                                     FROM batteryInfo bi 
                                     INNER JOIN buyers b ON bi.buyerId = b.buyerId
                                     WHERE {filter};";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query_filter, _dblink.connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object value = reader.GetValue(0);
                            filteredBatteryIds.Add(value.ToString());
                        }
                        Console.WriteLine($"값불러오는 것도 했음: {string.Join(", ", filteredBatteryIds)}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (_dblink.connection == null)
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message} 연결 안됨 에러위치: {ex.StackTrace}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Console.WriteLine(string.Join(", ", filteredBatteryIds));
            
            return filteredBatteryIds;
        }

        [RelayCommand]
        private void Search()
        {

            //Console.WriteLine($"{GetStartDateTime().ToString("yyyy-MM-dd HH:mm:ss")} ~ {GetEndDateTime().ToString("yyyy-MM-dd HH:mm:ss")}");
            // 검색 조건 설정 내용 출력
            var temp = FilterBatteryIds();
            //Console.WriteLine("디버깅:" + string.Join(", ", temp) + ": 여기까지");
            string filteredBatteryIds_string = string.Join(", ", FilterBatteryIds());
            //Console.WriteLine("필터로 쓰이는건" + filteredBatteryIds_string);
            string filterCondition = filteredBatteryIds_string.Length>0 ? $"batteryId IN ({filteredBatteryIds_string})" : "TRUE";
            DrawTimeChart(filterCondition);
            DrawPieChart(filterCondition);
            DrawColumnChart(filterCondition);
            LoadData(filterCondition);
            //MessageBox.Show(BatteryId);
        }


        // 차트 부분
        // 시간별 라인차트
        int numOfLine; // 전체 다 쓰일지도
        private List<AnalysisModel> _timeCharts = new List<AnalysisModel>();
        private SeriesCollection _seriesCollectionTimeChart;
        public SeriesCollection SeriesCollectionTimeChart
        {
            get { return _seriesCollectionTimeChart; }
            set { SetProperty(ref _seriesCollectionTimeChart, value); }
        }
        private Func<double, string> _dateTimeFormatter;
        public Func<double, string> DateTimeFormatter
        {
            get { return _dateTimeFormatter; }
            set { SetProperty(ref _dateTimeFormatter, value); }
        }
        public Func<double, string> YFormatter { get; set; }
        // 이건 굳이 할 필요 없는 듯하여 하지 않음
        //private Func<double, string> _yFormatter;
        //public Func<double, string> YFormatter
        //{
        //    get { return _yFormatter; }
        //    set { SetProperty(ref _yFormatter, value); }
        //}

        private void DrawTimeChart(string filter = "TRUE")
        {
            InitializeMultipleTimeCharts(numOfLine, filter);

            //var dates = _timeChart[0].TimeList; // new List<DateTime>();

            SeriesCollectionTimeChart = new SeriesCollection();
            for (int i = 0; i < numOfLine; i++)
            {
                var innerList = new List<double>();
                SeriesCollectionTimeChart.Add(
                new LineSeries
                {
                    Title = $"Line{i + 1}",
                    //Values = innerList.AsChartValues(),
                    Values = new ChartValues<double>(_timeChart[i].CountValue),
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }
        }

        // 파이차트
        private SeriesCollection _seriesCollectionPie;
        public SeriesCollection SeriesCollectionPie
        {
            get { return _seriesCollectionPie; }
            set { SetProperty(ref _seriesCollectionPie, value); }
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        // 이거 이렇게 하니까 작동안하는듯
        //[RelayCommand]
        //private void Chart_OnDataClick(ChartPoint chartpoint)
        //{
        //    var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

        //    foreach (PieSeries series in chart.Series)
        //        series.PushOut = 0;

        //    var selectedSeries = (PieSeries)chartpoint.SeriesView;
        //    selectedSeries.PushOut = 8;
        //}
        private void DrawPieChart(string filter = "TRUE")
        {
            string query_Pie = @$"SELECT Status, COUNT(batteryId) AS batteryCount
	                            FROM (SELECT batteryId,
			                            CASE
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN 'normal'
				                            WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN 'pollution'
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN 'damage'
				                            ELSE 'pollution & damage'
			                            END AS Status
		                            FROM inspectionResults
                                    WHERE {filter}
		                            GROUP BY batteryId) AS subquery
	                            GROUP BY Status
                                ORDER BY 
                                    CASE
                                        WHEN Status = 'normal' THEN 0
                                        WHEN Status = 'pollution' THEN 1
                                        WHEN Status = 'damage' THEN 2
                                        ELSE 3
                                    END;";
            Console.WriteLine("파이차트: " + query_Pie);
            var result_Pie = _dblink.Select(query_Pie);

            SeriesCollectionPie = new SeriesCollection();

            foreach (var item in result_Pie)
            {
                SeriesCollectionPie.Add(new PieSeries
                {
                    Title = (string)item["Status"],
                    Values = new ChartValues<double> { Convert.ToDouble(item["batteryCount"]) },
                    DataLabels = true //default값이 true인듯
                });
            }
        }

        // column차트
        private SeriesCollection _seriesCollectionColumn;
        public SeriesCollection SeriesCollectionColumn
        {
            get { return _seriesCollectionColumn; }
            set { SetProperty(ref _seriesCollectionColumn, value); }
        }
        public string[] LabelsColumn { get; set; }
        public Func<double, string> FormatterColumn { get; set; }

        private void DrawColumnChart(string filter = "TRUE")
        {
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            // column차트 - 따로 메소드 만들어서 사용하는 식으로 해야할듯
            string query_column = $@"WITH StatusList AS (
                                            SELECT * FROM (
                                            VALUES 
                                                ROW('normal'),
                                                ROW('pollution'),
                                                ROW('damage'),
                                                ROW('pollution & damage')
                                            ) AS t(Status)
                                        ),
                                        LineList AS (
                                            SELECT DISTINCT lineId
                                            FROM batteryQIPlus.inspectionResults
                                        )
                                        SELECT l.lineId, sl.Status, COALESCE(COUNT(s.batteryId), 0) AS Count
                                        FROM LineList l
                                        CROSS JOIN StatusList sl
                                        LEFT JOIN (
                                                SELECT lineId, batteryId,
                                                CASE
                                                    WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN 'normal'
                                                    WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN 'pollution'
                                                    WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN 'damage'
                                                    ELSE 'pollution & damage'
                                                END AS Status
                                            FROM batteryQIPlus.inspectionResults
                                            WHERE {filter}
                                            GROUP BY lineId, batteryId) AS s ON l.lineId = s.lineId AND sl.Status = s.Status                         
                                            GROUP BY l.lineId, sl.Status
                                        ORDER BY l.lineId, 
                                                CASE
                                                    WHEN sl.Status = 'normal' THEN 0
                                                    WHEN sl.Status = 'pollution' THEN 1
                                                    WHEN sl.Status = 'damage' THEN 2
                                                    ELSE 3
                                                END;";

            Console.WriteLine("콜롬차트: " + query_column);
            //     WHERE inspectionDatetime BETWEEN '2025-02-28 22:10:00' AND '2025-03-02 12:30:00'

            List<Dictionary<string, object>> result_column = _dblink.Select(query_column);

            int numOfStatus = 4;
            int numOfLineColumn = result_column.Count / numOfStatus; // 일단 임시로 선언, 나중에 통일시켜도 될듯
            SeriesCollectionColumn = new SeriesCollection();
            List<int> valueListColumn = new List<int>(); // 값 저장해서 chartvalues로 설정할 때 쓸 리스트
            for (int s = 0; s < numOfStatus; s++)
            {
                valueListColumn.Clear();
                for (int line = 0; line < numOfLineColumn; line++)
                {
                    valueListColumn.Add(Convert.ToInt32(result_column[line * numOfStatus + s]["Count"]));
                }
                SeriesCollectionColumn.Add(new ColumnSeries
                {
                    Title = (string)result_column[s]["Status"],
                    //Values = valueListColumn.AsChartValues()
                    Values = new ChartValues<int>(valueListColumn),
                    DataLabels = true
                });
            }
        }

        // 테이블 뷰
        private DataView _queryResultsTable;
        public DataView QueryResultsTable
        {
            get => _queryResultsTable;
            set => SetProperty(ref _queryResultsTable, value);
        }
        
        // 이름 DrawTable로 할까 고민중
        private void LoadData(string filter = "TRUE")
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) // 디자인 타임(모드) 동안 DB 연결이 수행되는것을 방지
                return;
            
            try
            {
                string tablequery = $"SELECT * FROM inspectionResults WHERE {filter}";
                Console.WriteLine("테이블: " + tablequery);
                // 이 부분 using 사용하는 것으로 수정하기
                MySqlCommand cmd = new MySqlCommand(tablequery, _dblink.connection);

                DataTable dataTable = new DataTable();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }

                QueryResultsTable = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
