using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.ViewModels.Bases;

namespace batteryQI.ViewModels
{
    public class ChartViewModel : ViewModelBases //이거 원래 internal이였음
    {
        private object _selectedTab;
        public object SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public ObservableCollection<TabItemViewModel> Tabs { get; } = new ObservableCollection<TabItemViewModel>();

        public ChartViewModel()
        {
            Tabs.Add(new TabItemViewModel { Header = "불량률", Content = new Chart1ViewModel() });
            Tabs.Add(new TabItemViewModel { Header = "불량유형", Content = new Chart2ViewModel() });
        }
    }
    public class TabItemViewModel
    {
        public string Header { get; set; }
        public object Content { get; set; }
    }

    public class Chart1ViewModel //chart1 그릴 데이터 관리
    {
        public double[] XData { get; } = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
        public double[] YData { get; } = Enumerable.Range(0, 100).Select(i => Math.Sin(i * 0.1)).ToArray();
    }

    public class Chart2ViewModel //chart2 그릴 데이터 관리
    {
        public double[] Values { get; } = { 30, 20, 50 };
        public string[] Labels { get; } = { "Type A", "Type B", "Type C" };
    }
}
