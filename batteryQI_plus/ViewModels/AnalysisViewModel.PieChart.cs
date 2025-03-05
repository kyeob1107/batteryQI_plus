using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Wpf;
using LiveCharts;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel
    {
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
    }
}
