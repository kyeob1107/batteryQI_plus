using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace batteryQI_plus.Models
{
    public class Column : ObservableObject
    {
        string filter;
        string filter2_notIN;
        string query_column;
        private SeriesCollection _seriesCollectionColumnModel;
        int numOfStatus;
        public string[] LabelsColumnModel { get; set; }
        public SeriesCollection SeriesCollectionColumnModel
        {
            get { return _seriesCollectionColumnModel; }
            set { SetProperty(ref _seriesCollectionColumnModel, value); }
        }
        public Column() 
        {
            filter = "TRUE";
            filter2_notIN = "TRUE";
            query_column = "";
            _seriesCollectionColumnModel = new SeriesCollection();
            numOfStatus = 4;
            LabelsColumnModel = new string[numOfStatus];
        }

        public void ConfigureChart(DBlink link, string filterContent, string filter2_notINContent)
        {
            filter = filterContent;
            filter2_notIN = filter2_notINContent;
            string query_column =
            $@"WITH StatusList AS (
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
                                WHERE {filter} AND {filter2_notIN}
                                GROUP BY lineId, batteryId) AS s ON l.lineId = s.lineId AND sl.Status = s.Status                         
                        GROUP BY l.lineId, sl.Status
                        ORDER BY l.lineId, 
                                CASE
                                    WHEN sl.Status = 'normal' THEN 0
                                    WHEN sl.Status = 'pollution' THEN 1
                                    WHEN sl.Status = 'damage' THEN 2
                                    ELSE 3
                                END;";

            //Console.WriteLine("콜롬차트: " + query_column);
            //     WHERE inspectionDatetime BETWEEN '2025-02-28 22:10:00' AND '2025-03-02 12:30:00'

            List<Dictionary<string, object>> result_column = link.Select(query_column);
            int numOfLineColumn = result_column.Count / numOfStatus; // 일단 임시로 선언, 나중에 통일시켜도 될듯
            _seriesCollectionColumnModel = new SeriesCollection();
            List<int> valueListColumn = new List<int>(); // 값 저장해서 chartvalues로 설정할 때 쓸 리스트
            for (int s = 0; s < numOfStatus; s++)
            {
                valueListColumn.Clear();
                for (int line = 0; line < numOfLineColumn; line++)
                {
                    valueListColumn.Add(Convert.ToInt32(result_column[line * numOfStatus + s]["Count"]));
                }
                _seriesCollectionColumnModel.Add(new ColumnSeries
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
            LabelsColumnModel = new string[numOfLineColumn];
            for (int i = 0; i < numOfLineColumn; i++)
            {
                LabelsColumnModel[i] = $"Line{i + 1}";
            }
            //FormatterColumn = value => value.ToString("N0");
        }
    }
}
