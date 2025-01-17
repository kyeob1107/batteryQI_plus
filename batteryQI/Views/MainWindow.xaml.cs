using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using batteryQI.UserControls;
using batteryQI.ViewModels;
using Microsoft.Win32;

namespace batteryQI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 초기 화면 설정
            MainFrame.Content = new DashboardView();
        }



        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DashboardView();
        }

        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ChartView();
        }

        private void ManagerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ManagerView();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}