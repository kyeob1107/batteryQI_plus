using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.ViewModels;
using ScottPlot.WPF;
using System.Windows;
using ScottPlot;
using System.Diagnostics;

namespace batteryQI.Extensions
{
    public static class PlotExtensions
    {
        public static readonly DependencyProperty PlotDataProperty =
            DependencyProperty.RegisterAttached(
                                                "PlotData",
                                                typeof(object),
                                                typeof(PlotExtensions),
                                                new PropertyMetadata(null, OnPlotDataChanged)
                                                );

        public static void SetPlotData(DependencyObject element, object value)
        {
            element.SetValue(PlotDataProperty, value);
        }

        public static object GetPlotData(DependencyObject element)
        {
            return element.GetValue(PlotDataProperty);
        }

        private static void OnPlotDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot wpfPlot)
            {
                var data = e.NewValue;
                wpfPlot.Plot.Clear();

                if (data is Chart1ViewModel chart1Data)
                {
                    wpfPlot.Plot.Clear();
                    wpfPlot.Plot.Add.Scatter(chart1Data.XData, chart1Data.YData);
                    wpfPlot.Plot.Title("Chart 1");
                    wpfPlot.Plot.XLabel("X-Axis");
                    wpfPlot.Plot.YLabel("Y-Axis");
                    //wpfPlot.Plot.Axes.AutoScaleX();
                    //wpfPlot.Plot.Axes.SetLimitsY(-1.1, 1.1);
                    //wpfPlot.Plot.Axes.SetLimits(0, 100, -1.1, 1.1);
                }
                else if (data is Chart2ViewModel chart2Data)
                {
                    wpfPlot.Plot.Clear();
                    wpfPlot.Plot.Add.Pie(chart2Data.Values);
                    wpfPlot.Plot.Title("Chart 2");
                    //wpfPlot.Plot.Axes.AutoScale();
                }
                
                //그래프 축범위 설정이 제대로 안되서 스케일 건드려볼려고 한것인데 잘 안되서 일단 남겨두고 다른 부분
                wpfPlot.Plot.Axes.SetLimits(wpfPlot.Plot.Axes.GetDataLimits());
                AxisLimits limits = wpfPlot.Plot.Axes.GetLimits();
                Debug.WriteLine($"여기가 보고 싶은 값이다 휴먼 \r\n Axis Limits: Bottom={limits.Bottom}, Top={limits.Top}, Left={limits.Left}, Right={limits.Right}");

                wpfPlot.Refresh();
            }
        }
    }

}
