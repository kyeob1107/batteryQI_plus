using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;

namespace batteryQI_plus.Models
{
    //public List<object>? analysisComboxValues; // 이건 그냥 viewmodel에서 바로 쓰면 될 것 같아서
    //public struct filterData
    //{
    
    //    public List<string> status; // 정상, 오염, 파손
    //    public List<string> usageNmae; //
    //    public List<string> buyer;
    //    public List<string> batteryType;
    //    public List<string> batteryShape;
    //    public List<string> productionLine; // 근무자는 제한되게 볼 수 있도록?
    //}

    public class TimeChartModel : ObservableObject
    {
        // 아예 chartModel로 빼거나 TimeChartModel로 더 세부적으로 나눌까 고민중
        #region 처음 설계했던 것 - 안씀
        //private List<DateTime>? _timeChartX;
        //private List<double>? _defectRate;
        //public List<DateTime>? TimeChartX
        //{
        //    get { return _timeChartX; }
        //    set { SetProperty(ref _timeChartX, value); }
        //}

        //public List<double>? DefectRate
        //{
        //    get { return _defectRate; }
        //    set { SetProperty(ref _defectRate, value); }
        //}
        #endregion
        //private List<DateTime>? _timeList;
        //private List<double>? _countValue;
        private DateTime startDatetime;
        private DateTime endDatetime;

        string filter;
        string filter2_notIN;

        // 시간별 라인차트
        int numOfLineModel; // 전체 다 쓰일지도

        private SeriesCollection _seriesCollectionTimeChartModel;
        public SeriesCollection SeriesCollectionTimeChartModel
        {
            get { return _seriesCollectionTimeChartModel; }
            set { SetProperty(ref _seriesCollectionTimeChartModel, value); }
        }
        private Func<double, string> _dateTimeFormatterMdl;
        public Func<double, string> DateTimeFormatterMdl
        {
            get { return _dateTimeFormatterMdl; }
            set { SetProperty(ref _dateTimeFormatterMdl, value); }
        }
        public Func<double, string> YFormatter { get; set; }

        // 초기화 어떻게 해야할지 안떠올라서 일단 틀만 해둠
        public TimeChartModel(DateTime sdt, DateTime edt, int nfl) 
        {
            startDatetime = sdt; //GetStartDateTime()
            endDatetime = edt; //GetEndDateTime()
            numOfLineModel = nfl;
            filter = "TRUE";
            filter2_notIN = "TRUE";
        }

        private ObservableCollection<TimeSeriesModel> _timeChart;
        public ObservableCollection<TimeSeriesModel> TimeChart
        {
            get { return _timeChart; }
            set { SetProperty(ref _timeChart, value); }
        }

        public void ConfigureChart(DBlink link, string filterContent, string filter2_notINContent)
        {
            filter = filterContent;
            filter2_notIN = filter2_notINContent;
            InitializeMultipleTimeCharts(link, filter, filter2_notIN);

            SeriesCollectionTimeChartModel = new SeriesCollection();
            for (int i = 0; i < numOfLineModel; i++)
            {
                var innerList = new List<double>();
                SeriesCollectionTimeChartModel.Add(
                new LineSeries
                {
                    Title = $"Line{i + 1}",
                    //Values = innerList.AsChartValues(),
                    Values = new ChartValues<double>(_timeChart[i].CountValue),
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            }

        }

        // 이름 수정해야할 듯, 추가로 numOfLine 그냥 private 변수인 numOfLine쓰게 할지 아니면 인자로 받게 할지 고민
        private void InitializeMultipleTimeCharts(DBlink link, string filter = "TRUE", string filter2_notIN = "TRUE")
        {
            // 초기화 필요해서 일단 당장 이 방식으로 해줬음
            _timeChart = new ObservableCollection<TimeSeriesModel>();
            for (int line = 0; line < numOfLineModel; line++)
            {
                //Console.WriteLine($"라인{line + 1}입니다");

                // 임시용
                string startDateForQuery = startDatetime.ToString("yyyy-MM-dd HH:mm:ss"); // "2025-03-03 09:00";
                string endDateForQuery = endDatetime.ToString("yyyy-MM-dd HH:mm:ss"); //"2025-03-03 23:50";
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
                                                WHERE lineid = {line + 1} AND {filter} AND {filter2_notIN}
                                                GROUP BY time_interval, batteryId
                                            ) AS subquery ON t.time_interval = subquery.time_interval
                                            GROUP BY t.time_interval
                                            ORDER BY t.time_interval;";
                //Console.WriteLine("타임차트: " + query_timeChart);
                var queryResult = link.Select(query_timeChart);
                //foreach (string key in queryResult[0].Keys) { Console.WriteLine(key); }
                TimeSeriesModel model = new TimeSeriesModel(queryResult);
                _timeChart.Add(model);
            }

            // 이부분 원래 함수 밖에 있다가 함수로 옮김 이부분 확인다시 해봐야함
            var dates = _timeChart[0].TimeList; // new List<DateTime>();
            DateTimeFormatterMdl = value => dates[((int)value)].ToString("yyyy-MM-dd HH:mm:ss");
            #region 에러상황방지 - 현재는 이렇게까진 필요없을 듯하여 보류
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
            #endregion
        }
    }
}
