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
        public AnalysisViewModel()
        {
            // 필터 초기화 부분
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

        //private List<Dictionary<string,object>> SelectFilterValue(string category, string table = "batteryInfo")
        //{
        //    string query = $"SELECT DISTINCT {category} FROM {table};";
        //    var FilterValueList = _dblink.Select(query);
        //    return FilterValueList;
        //}

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
    }
}
