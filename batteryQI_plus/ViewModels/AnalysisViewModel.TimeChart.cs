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
            #region model부분
            //InitializeMultipleTimeCharts(numOfLine, filter);

            //var dates = _timeChart[0].TimeList; // new List<DateTime>();

            //SeriesCollectionTimeChart = new SeriesCollection();
            //for (int i = 0; i < numOfLine; i++)
            //{
            //    var innerList = new List<double>();
            //    SeriesCollectionTimeChart.Add(
            //    new LineSeries
            //    {
            //        Title = $"Line{i + 1}",
            //        //Values = innerList.AsChartValues(),
            //        Values = new ChartValues<double>(_timeChart[i].CountValue),
            //        Fill = System.Windows.Media.Brushes.Transparent
            //    });
            //}
            #endregion
            TimeChartModel timechart = new TimeChartModel(GetStartDateTime(), GetEndDateTime(), numOfLine);
            timechart.ConfigureChart(_dblink, filter, filter2_notIN);
            SeriesCollectionTimeChart = timechart.SeriesCollectionTimeChartModel;
            DateTimeFormatter = timechart.DateTimeFormatterMdl;
        }
    }
}
