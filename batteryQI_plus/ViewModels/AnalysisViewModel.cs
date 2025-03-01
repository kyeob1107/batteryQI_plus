using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Diagnostics;
using ScottPlot.Ticks;
using System.Collections;
using MySqlX.XDevAPI.Common;
using System.Configuration;
using ScottPlot.Renderable;
using ScottPlot.Drawing.Colormaps;
using System.Drawing;
using batteryQI_plus.ViewModels;
using batteryQI_plus.Models;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.ComponentModel;
using LiveCharts.Helpers;

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

        public AnalysisViewModel()
        {
            // 초기값 설정
            StartDate = DateTime.Today; // 값수정 필요
            StartTime = "00:00:00";
            EndDate = DateTime.Today; // 값수정 필요
            EndTime = "23:59:59";

            // Initialize UsageItems with sample data
            UsageItems = new ObservableCollection<SelectableItem>();
            var UsageFilterlist = SelectFilterValue("usageName");
            for (int i = 0; i < UsageFilterlist.Count; i++) 
            {
                UsageItems.Add(
                    new SelectableItem 
                    { 
                        Name = (string)UsageFilterlist[i]["usageName"], IsSelected = false 
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
                        IsSelected = false
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
                        IsSelected = false
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
                        IsSelected = false
                    });
            }

            StatusItems = new ObservableCollection<SelectableItem>
            {
                new SelectableItem { Name = "정상", IsSelected = false },
                new SelectableItem { Name = "오염", IsSelected = false },
                new SelectableItem { Name = "파손", IsSelected = false }
            };

            ProductionLineItems = new ObservableCollection<SelectableItem>();
            var ProductionLineFilterlist = SelectFilterValue("lineId");
            for (int i = 0; i < ProductionLineFilterlist.Count; i++)
            {
                ProductionLineItems.Add(
                    new SelectableItem
                    {
                        Name = "Line" + ProductionLineFilterlist[i]["lineId"].ToString(),
                        IsSelected = false
                    });
            }

            BatteryId = "";

            // 타임차트
            var queryTimeChart = _dblink.Select($"SELECT COUNT(DISTINCT lineId) AS Count FROM batteryQIPlus.productionLines;");
            numOfLine = Convert.ToInt32(queryTimeChart[0]["Count"]) - 1;
            //for (int line = 0; line < numOfLine; line++)
            //{
            //    _timeCharts.Add(new AnalysisModel
            //    {
            //        TimeChartX.Add(),
            //        DefectRate.Add(),
            //    });
            //}
            //SeriesCollectionTimeChart = new SeriesCollection();
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
            SeriesCollectionTimeChart = new SeriesCollection();
            //for (int i = 0; i < numOfLine; i++)
            //{
            //    var innerList = new List<double>();
            //    SeriesCollectionTimeChart.Add(
            //    new LineSeries
            //    {
            //        Title = $"Line{i + 1}",
            //        //Values = innerList.AsChartValues(),
            //        Values = new ChartValues<double> { 10.0, 11.0, 9.0, 5.0, 10.0, 60.0, 12.0 },
            //        Fill = System.Windows.Media.Brushes.Transparent
            //    });
            //}
            SeriesCollectionTimeChart.Add(new LineSeries
                {
                    Title = $"Line1",
                    //Values = innerList.AsChartValues(),
                    Values = new ChartValues<double> { 10.0, 11.0, 9.0, 5.0, 10.0, 60.0, 12.0 },
                    Fill = System.Windows.Media.Brushes.Transparent
                });
            SeriesCollectionTimeChart.Add(new LineSeries
            {
                Title = $"Line2",
                //Values = innerList.AsChartValues(),
                Values = new ChartValues<double> { 10.0, 11.0, 9.0, 5.0, 10.0, 30.0, 12.0 },
                Fill = System.Windows.Media.Brushes.Transparent
            });
            SeriesCollectionTimeChart.Add(new LineSeries
            {
                Title = $"Line3",
                //Values = innerList.AsChartValues(),
                Values = new ChartValues<double> { 10.0, 11.0, 9.0, 5.0, 25.0, 10.0, 12.0 },
                Fill = System.Windows.Media.Brushes.Transparent
            });

            DateTimeFormatter = value => dates[((int)value)].ToString("yyyy-MM-dd HH:mm:ss");
            YFormatter = value => value.ToString("N");

            // 파이차트
            // 값 가져오는 할당해서 만드는 함수화 하는게 보기 편할듯
            // 쿼리 엄밀하게 배터리 수가 아니라 그냥 이미지 수로 때려박는거라 정확하지 않음 다시짜줘야함
            // 할려면 미리 배터리별로 groupby해서 check결과 합해서 0인지 확인하는 방식으로 서브쿼리로 짜줘야할듯
            string query_Pie = @$"SELECT 
                                    CASE 
                                        WHEN fastPollutionCheck = 0 AND fastDamageCheck = 0 THEN '오염'
                                        WHEN fastPollutionCheck = 0 AND fastDamageCheck = 1 THEN '손상'
                                        WHEN fastPollutionCheck = 1 AND fastDamageCheck = 1 THEN '오염과 손상'
                                        ELSE '정상'
                                    END AS Status,
                                    COUNT(DISTINCT batteryId) AS batteryCount
                                FROM 
                                    inspectionResults
                                GROUP BY 
                                    Status;";
            var result_Pie = _dblink.Select(query_Pie);

            SeriesCollectionPie = new SeriesCollection();
            foreach(var item in result_Pie)
            {
                SeriesCollectionPie.Add(new PieSeries
                {
                    Title = (string)item["Status"],
                    Values = new ChartValues<double> { Convert.ToDouble(item["batteryCount"]) },
                    DataLabels = true
                });
            }
            #region 예제 삭제예정
            // 원래 생성자 쪽에 달려있던 고정 예시 초기값
            //{
            //    new PieSeries
            //    {
            //        Title = "Maria",
            //        Values = new ChartValues<double> { 3 },
            //        DataLabels = true
            //    },
            //    new PieSeries
            //    {
            //        Title = "Charles",
            //        Values = new ChartValues<double> { 4 },
            //        DataLabels = true
            //    },
            //    new PieSeries
            //    {
            //        Title = "Frida",
            //        Values = new ChartValues<double> { 6 },
            //        DataLabels = true
            //    },
            //    new PieSeries
            //    {
            //        Title = "Frederic",
            //        Values = new ChartValues<double> { 2 },
            //        DataLabels = true
            //    }
            //};
            #endregion

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            // column차트
            SeriesCollectionColumn = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Normal",
                    Values = new ChartValues<double> { 100, 250, 239 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollectionColumn.Add(new ColumnSeries
            {
                Title = "Pollution",
                Values = new ChartValues<double> { 11, 56, 42 }
            }
            );
            SeriesCollectionColumn.Add(new ColumnSeries
            {
                Title = "Damage",
                Values = new ChartValues<double> { 15, 30, 33 }
            }
            );
            SeriesCollectionColumn.Add(new ColumnSeries
            {
                Title = "Pollution & Damage",
                Values = new ChartValues<double> { 9, 26, 20 }
            }
            );


            LabelsColumn = new[] { "Line1", "Line2", "Line3"};
            FormatterColumn = value => value.ToString("N");

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

        [RelayCommand]
        private void Search()
        {
            // 검색 조건 설정 내용 출력
            MessageBox.Show(BatteryId);
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
        public Func<double, string> DateTimeFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        // 파이차트
        public SeriesCollection SeriesCollectionPie { get; set; }

        public Func<ChartPoint, string> PointLabel { get; set; }

        // 이거 이렇게 하니까 작동안하는듯
        [RelayCommand]
        private void Chart_OnDataClick(ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
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

        // 테이블 뷰
        private DataView _queryResultsTable;
        public DataView QueryResultsTable
        {
            get => _queryResultsTable;
            set => SetProperty(ref _queryResultsTable, value);
        }

        private void LoadData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) // 디자인 타임(모드) 동안 DB 연결이 수행되는것을 방지
                return;
            
            try
            {
                string tablequery = "SELECT * FROM inspectionResults";
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
