using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.ViewModels.Bases;
using batteryQI.Models;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Diagnostics;
using ScottPlot.Ticks;
using System.Collections;
using MySqlX.XDevAPI.Common;

namespace batteryQI.ViewModels
{
    public class ChartViewModel : ViewModelBases //이거 원래 internal이였음
    {
        //private object _selectedTab;
        //public object SelectedTab
        //{
        //    get => _selectedTab;
        //    set => SetProperty(ref _selectedTab, value);
        //}

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new ObservableCollection<TabItemViewModel>();

        public ChartViewModel()
        {
            Tabs.Add(new TabItemViewModel { Header = "불량률", Content = new Chart1ViewModel() });
            Tabs.Add(new TabItemViewModel { Header = "불량유형", Content = new Chart2ViewModel() });
        }
    }
    
    //Tabitem 구조
    public class TabItemViewModel
    {
        public string Header { get; set; }
        public object Content { get; set; }
    }

    public class Chart1ViewModel //chart1 그릴 데이터 관리
    {
        private DBlink _dblink = DBlink.Instance(); //DB연결 사용
        public string[] Labels { get; }
        public double[] Values { get; }


        public Chart1ViewModel()
        {
            // CountQuery 메소드 호출 및 결과 저장
            var chartData = _dblink.CountQuery("manager", "workAmount");
            Values = chartData.counts.ToArray();
            Labels = Array.ConvertAll<object,string>(chartData.defectGroups.ToArray(), x => x?.ToString() ?? string.Empty);
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => (x ?? "").ToString() ?? string.Empty); //cs8602경고에 대한 해결책1
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => ((object)x)?.ToString() ?? string.Empty); //cs8602경고에 대한 해결책2
        }

        public void ConfigureChart(Plot plot)
        {
            #region 5.0.53
            ////plot.Add.Pie(Values);
            ////plot.Title("Chart 1");
            ////plot.XLabel("X-Axis");
            ////plot.YLabel("Y-Axis");
            //// create a pie chart
            //var pie = plot.Add.Pie(Values);
            ////pie.ExplodeFraction = 0.1;
            //pie.SliceLabelDistance = 0.5;

            //// set different labels for slices and legend
            //// Labels와 Values 갯수 동일하다는 가정 -> 조건문으로 추가예정
            //double total = pie.Slices.Select(x => x.Value).Sum();
            //for (int i = 0; i < pie.Slices.Count; i++)
            //{
            //    pie.Slices[i].LabelFontSize = 30;
            //    pie.Slices[i].Label = $"{Labels[i]}\r\n" + $"({pie.Slices[i].Value / total:p1})";
            //    pie.Slices[i].LegendText = $"{Labels[i]}" +
            //        $"({pie.Slices[i].Value})";
            //}

            //// hide unnecessary plot components
            ////plot.Axes.Frameless();
            //plot.HideGrid();
            #endregion
            #region 4.1.74
            var pie = plot.AddPie(Values);
            // Labels와 Values 갯수 동일하다는 가정하에 동작
            if (Values.Length == Labels.Length)
            {
                double total = Values.Sum();
                pie.SliceLabels = Enumerable.Range(0, Values.Length)
                                   .Select(i => $"{Labels[i]}\r\n({Values[i]/total:P1})").ToArray(); ;
                pie.SliceLabelPosition = 0.3;
                pie.SliceFont.Size = 30;
                //pie.ShowPercentages = true; //%값표시
                //pie.ShowValues = true; // 값표시
                pie.ShowLabels = true;
                pie.LegendLabels = Enumerable.Range(0, Values.Length)
                                   .Select(i => $"{Labels[i]} ({Values[i]})").ToArray();
                    plot.Legend();
            }
            else 
            { 
                throw new ArgumentException("(Userdefined)Values와 Labels의 길이가 일치하지 않습니다."); 
            }
            #endregion
        }
    }

    public class Chart2ViewModel //chart2 그릴 데이터 관리
    {
        private DBlink _dblink = DBlink.Instance(); //DB연결 사용
        public double[] Values { get; }
        public string[] Labels { get; }

        public Chart2ViewModel() 
        {
            // CountQuery 메소드 호출 및 결과 저장
            //var chartData = _dblink.CountQuery("manager", "managerPw"); //테스트용
            var chartData = _dblink.CountQuery("manager", "workAmount");
            Values = chartData.counts.ToArray();
            Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => x?.ToString() ?? string.Empty);
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => (x ?? "").ToString() ?? string.Empty); //cs8602경고에 대한 해결책1
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => ((object)x)?.ToString() ?? string.Empty); //cs8602경고에 대한 해결책2
        }

        public void ConfigureChart(Plot plot)
        {
            #region 5.0.53
            ////plot.Add.Pie(Values);
            ////plot.Title("Chart 2");
            //Tick[] ticks = new Tick[Labels.Length];
            //for (int i = 0; i < Values.Length; i++)
            //{
            //    plot.Add.Bar(position: i + 1, value: Values[i]);
            //    ticks[i] = new Tick(i + 1, Labels[i]);
            //}

            //plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            //plot.Axes.Bottom.MajorTickStyle.Length = 0;
            ////plot.HideGrid();
            //// 자동 스케일 설정
            ////plot.Axes.AutoScale();
            ////plot.Axes.Frameless(true);

            //// tell the plot to autoscale with no padding beneath the bars
            //plot.Axes.Margins(bottom: 0);
            #endregion
            #region 4.1.74
            if (Values.Length == Labels.Length)
            {
                // 더 많은 조건 있을 경우 AddBarGroups이용하면 될듯
                double[][] ValuesArray = Values.Select(d => new double[] { d }).ToArray();
                Debug.WriteLine(ValuesArray.Select(d => d.ToString()));
                double[][] PostionArray = Enumerable.Range(1, Values.Length)
                                                    .Select(i => new double[] { i }).ToArray();
                // 반복문을 통한 바 그래프 생성
                for (int i = 0; i < Values.Length; i++)
                {
                    var bar = plot.AddBar(ValuesArray[i], PostionArray[i]);
                    bar.Label = Labels[i];
                }

                // 그래프 축관련 설정
                plot.AxisAuto();
                plot.SetAxisLimits(yMin: 0);
                plot.Frame(true);
                plot.XAxis2.Ticks(false);
                plot.YAxis2.Ticks(false);

                plot.Legend(location:Alignment.UpperRight);
                //// X축 레이블 설정
                //plot.XAxis.ManualTickPositions(Enumerable.Range(0, Labels.Length).Select(i => (double)i).ToArray(), Labels);
                //plot.XAxis.TickLabelStyle(rotation: 45);
            }
            else
            {
                throw new ArgumentException("(Userdefined)Values와 Labels의 길이가 일치하지 않습니다.");
            }
            #endregion
        }
    }
}
