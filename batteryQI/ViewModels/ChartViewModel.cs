﻿using System;
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
    public class ChartViewModel
    {
        protected DBlink _dblink = DBlink.Instance(); //DB연결 사용
        public string[] Labels { get; protected set; }
        public double[] Values { get; protected set; }
        public DateTime[] TimeStamps { get; protected set; }

        public ChartViewModel() 
        {
            // CountQuery 메소드 호출 및 결과 저장
            var chartData = _dblink.CountQuery("batteryInfo", "defectName");
            Values = chartData.counts.ToArray();
            Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => x?.ToString() ?? string.Empty);
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => (x ?? "").ToString() ?? string.Empty); //cs8602경고에 대한 해결책1
            //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => ((object)x)?.ToString() ?? string.Empty); //cs8602경고에 대한 해결책2
        }

        public ChartViewModel(string table, string groupingCriteria, string XAxis = "label")
        {
            // CountQuery 메소드 호출 및 결과 저장
            // CountQuery에서는 mode라는 매개변수명으로 되어 있으나 XAis와 동일한 키워드를 가짐
            var chartData = _dblink.CountQuery(table, groupingCriteria, XAxis);
            Values = chartData.counts.ToArray();
            if (XAxis == "label")
            {
                Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => x?.ToString() ?? string.Empty);
                //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => (x ?? "").ToString() ?? string.Empty); //cs8602경고에 대한 해결책1
                //Labels = Array.ConvertAll<object, string>(chartData.defectGroups.ToArray(), x => ((object)x)?.ToString() ?? string.Empty); //cs8602경고에 대한 해결책2
            }
            else if (XAxis == "timestamp")
            {
                TimeStamps = Array.ConvertAll(chartData.defectGroups.ToArray(), x =>
                {
                    if (x != null && DateTime.TryParse(x.ToString(), out DateTime result))
                    {
                        return result;
                    }
                    //return DateTime.MinValue; // 또는 다른 기본값
                    throw new FormatException("값이 DateTime형태로 Parsing할 수 없습니다");
                });
            }
        }

        public void ConfigureChart(Plot plot)
        {
            // 아직 공통적으로 구현할 것 없어서 형식만 둠
        }
    }
    public class TabControlViewModel : ViewModelBases
    {

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new ObservableCollection<TabItemViewModel>();

        public TabControlViewModel() // 이부분 생성자 인자 들억가는 것 따로 구조체 같은 것으로 필드해서 만들면 더 좋을듯
        {

            Tabs.Add(new TabItemViewModel { Header = "시간대별 불량수", Content = new HourlyDefectChartViewModel("batteryInfo", "shootDate", "timestamp") });
            Tabs.Add(new TabItemViewModel { Header = "불량유형", Content = new DefectTypePieViewModel() });
            Tabs.Add(new TabItemViewModel { Header = "기준별 불량유형", Content = new DefectTypeChartByCategoryViewModel("batteryInfo", "defectName") });
        }
    }
    
    //Tabitem 구조
    public class TabItemViewModel
    {
        public string Header { get; set; }
        public object Content { get; set; }
    }

    
    // 불량 유형 파이차트 관리
    public class DefectTypePieViewModel : ChartViewModel
    {

        public DefectTypePieViewModel() {}

        public DefectTypePieViewModel(string table, string groupingCriteria) : base(table, groupingCriteria) { }

        // 차트 그리는 방식 결정
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
            // Labels와 Values 갯수 동일하다는 가정하에 동작
            if (Values.Length == Labels.Length)
            {
                var pie = plot.AddPie(Values);
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

    // 시간대별 불량수 바차트 관리
    public class HourlyDefectChartViewModel : ChartViewModel
    {

        public HourlyDefectChartViewModel() {}
        public HourlyDefectChartViewModel(string table, string groupingCriteria, string XAxis) : base(table, groupingCriteria, XAxis) { }

        // 차트 그리는 방식 결정
        public void ConfigureChart(Plot plot)
        {
            #region 4.1.74
            // 시간대별로 최소와 최대의 차를 구하여 +1 하여 bar의 갯수 계산
            TimeSpan timeDifference = TimeStamps.Max() - TimeStamps.Min();
            int pointCount = (int)timeDifference.TotalHours + 1;

            // 시작시간대(최소 시간대) 부터 매 1시간씩 간격으로 bar를 그릴 x축 위치 설정
            DateTime start = TimeStamps.Min();
            double[] positions = new double[pointCount];
            for (int i = 0; i < pointCount; i++)
                positions[i] = start.AddHours(i).ToOADate();

            // display the bar plot using a time axis
            var bar = plot.AddBar(Values, positions);
            plot.XAxis.DateTimeFormat(true); //x축 포멧을 DateTime으로 설정
            plot.Frame(true);
            plot.XAxis2.Ticks(false);
            plot.YAxis2.Ticks(false);

            // indicate each bar width should be 1/바갯수 of a day then shrink sligtly to add spacing between bars
            bar.BarWidth = (1.0 / pointCount) * .8;

            // adjust axis limits so there is no padding below the bar graph
            plot.SetAxisLimits(yMin: 0);
            plot.Layout(right: 20); // add room for the far right date tick
            #endregion
        }
    }

    public class DefectTypeChartByCategoryViewModel : ChartViewModel
    {

        public DefectTypeChartByCategoryViewModel() { }
        public DefectTypeChartByCategoryViewModel(string table, string groupingCriteria) : base(table, groupingCriteria) { }

        // 차트 그리는 방식 결정
        public void ConfigureChart(Plot plot)
        {
            #region 4.1.74
            if (Values.Length == Labels.Length)
            {
                // 더 많은 조건 있을 경우 AddBarGroups이용하면 될듯
                double[][] ValuesArray = Values.Select(d => new double[] { d }).ToArray();
                //Debug.WriteLine(ValuesArray.Select(d => d.ToString()));
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
                plot.XAxis.DateTimeFormat(false); //x축 포멧을 DateTime으로 설정
                plot.Legend(location: Alignment.UpperRight);
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
