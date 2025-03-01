using System;
using System.Collections.Generic;
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
using batteryQI_plus.ViewModels;
using batteryQI_plus.Views;
using Microsoft.Win32;

namespace batteryQI_plus.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            //this.DataContext = new CompositeViewModel(); // ViewModel 연결
            Unloaded += DashboardView_Unloaded; // 타이머 리소스 해제
        }

        // 페이지 언로드 될 때 타이머 리소스 해제
        private void DashboardView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DashboardViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }

    }
}
