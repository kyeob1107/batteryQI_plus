using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;
using static System.Windows.Forms.LinkLabel;

namespace batteryQI_plus.Models
{
    public partial class Pie : ObservableObject
    {
        string filter;
        string filter2_notIN;

        string query_Pie;
        private SeriesCollection _seriesCollectionPieModel;
        public SeriesCollection SeriesCollectionPieModel
        {
            get { return _seriesCollectionPieModel; }
            set { SetProperty(ref _seriesCollectionPieModel, value); }
        }

        public Pie() 
        {
            filter = "TRUE";
            filter2_notIN = "TRUE";
            query_Pie = "";
            _seriesCollectionPieModel = new SeriesCollection();
        }

        public void ConfigureChart(DBlink link, string filterContent, string filter2_notINContent)
        {
            filter = filterContent;
            filter2_notIN = filter2_notINContent;
            query_Pie = @$"SELECT Status, COUNT(batteryId) AS batteryCount
	                            FROM (SELECT batteryId,
			                            CASE
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN 'normal'
				                            WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN 'pollution'
				                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN 'damage'
				                            ELSE 'pollution & damage'
			                            END AS Status
		                            FROM inspectionResults
                                    WHERE {filter} AND {filter2_notIN}
		                            GROUP BY batteryId) AS subquery
	                            GROUP BY Status
                                ORDER BY 
                                    CASE
                                        WHEN Status = 'normal' THEN 0
                                        WHEN Status = 'pollution' THEN 1
                                        WHEN Status = 'damage' THEN 2
                                        ELSE 3
                                    END;
            ";

            List<Dictionary<string, object>> result_Pie = link.Select(query_Pie);
            _seriesCollectionPieModel = new SeriesCollection();

            foreach (var item in result_Pie)
            {
                _seriesCollectionPieModel.Add(new PieSeries
                {
                    Title = (string)item["Status"],
                    Values = new ChartValues<double> { Convert.ToDouble(item["batteryCount"]) },
                    DataLabels = true //default값이 true인듯
                });
            }
        }
        
    }
}
