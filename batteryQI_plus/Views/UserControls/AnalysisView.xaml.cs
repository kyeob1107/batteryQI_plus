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
using batteryQI_plus.ViewModels;
using LiveCharts.Wpf;
using LiveCharts;

namespace batteryQI_plus.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class AnalysisView : UserControl
    {
        public AnalysisView()
        {
            InitializeComponent();
            //this.DataContext = new TabControlViewModel();
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
