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
using batteryQI_plus.Views.UserControls;
using batteryQI_plus.ViewModels;
using Microsoft.Win32;

namespace batteryQI_plus.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var viewModel = new MainWindowViewModel();
            //viewModel.CloseAction = () => this.Close();
            //this.DataContext = viewModel;
            if (DataContext is MainWindowViewModel vm)
            {
                vm.CloseAction = () => this.Close(); // 창 닫기 기능을 ViewModel에 연결
            }
        }
    }
}