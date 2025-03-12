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
using batteryQI_plus.Models;

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

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.TotalInspectionResult();

                MessageBoxResult result = MessageBox.Show("정말 종료하시겠습니까?", "종료 확인", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true; // 종료 취소
                    viewModel.DBConnectButton();
                }
            }
        }
    }
}