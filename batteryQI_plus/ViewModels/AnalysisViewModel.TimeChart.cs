using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI_plus.Models;
using LiveCharts.Wpf;
using LiveCharts;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel
    {
        private ObservableCollection<AnalysisModel> _timeChart = new ObservableCollection<AnalysisModel>();
        public ObservableCollection<AnalysisModel> TimeChart
        {
            get { return _timeChart; }
            set { SetProperty(ref _timeChart, value); }
        }

        // 이름 수정해야할 듯, 추가로 numOfLine 그냥 private 변수인 numOfLine쓰게 할지 아니면 인자로 받게 할지 고민
        private void InitializeMultipleTimeCharts(int numOfLine, string filter = "TRUE", string filter2_notIN = "TRUE")
        {
            // 초기화 필요해서 일단 당장 이 방식으로 해줬음
            _timeChart = new ObservableCollection<AnalysisModel>();
            for (int line = 0; line < numOfLine; line++)
            {
                Console.WriteLine($"라인{line + 1}입니다");

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
                                                WHERE lineid = {line + 1} AND {filter} AND {filter2_notIN}
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

        private void DrawTimeChart(string filter = "TRUE", string filter2_notIN = "TRUE")
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
    }
}
