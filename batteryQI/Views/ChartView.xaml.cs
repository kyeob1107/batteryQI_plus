using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScottPlot.WPF;
using ScottPlot;
using ScottPlot.Plottables;

namespace batteryQI.Views
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
        }

      
        private void ChartTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl) 
            {
               
                if (tabControl.SelectedItem == Tab1) 
                {
                    InitializeChart1();
                }
                else if (tabControl.SelectedItem == Tab2) 
                {
                    InitializeChart2();
                }
            }
        }

        private void InitializeChart1()
        {
            var plt = Chart1Plot.Plot; 
            plt.Clear(); 

            double[] xs1 = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
            double[] ys1 = xs1.Select(x => Math.Sin(x * 0.1)).ToArray();

            var scatter = plt.Add.Scatter(xs1, ys1);
            scatter.LineWidth = 2;              
            scatter.MarkerSize = 5;          

            plt.Title("chart 1"); // 한글 안됨..
            plt.XLabel("X-Axis");      
            plt.YLabel("Y-Axis");      
            plt.Legend.IsVisible = true;         
            Chart1Plot.Refresh();                
        }

        private void InitializeChart2()
        {
            var plt = Chart2Plot.Plot;
            plt.Clear();

            double[] values = { 30, 20, 50 }; 
            string[] labels = { "Type A", "Type B", "Type C" }; 

            var pie = plt.Add.Pie(values);

            for (int i = 0; i < values.Length; i++)
            {
                pie.Slices[i].Label = labels[i]; 
            }

            plt.Title("chart 2");
            plt.Legend.IsVisible = true;
            Chart2Plot.Refresh();
        }
    }
}
