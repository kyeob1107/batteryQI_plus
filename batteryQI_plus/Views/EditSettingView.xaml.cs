using batteryQI_plus.ViewModels;
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
using System.Windows.Shapes;

namespace batteryQI_plus.Views
{
    /// <summary>
    /// Interaction logic for EditSettingView.xaml
    /// </summary>
    public partial class EditSettingView : Window
    {
        public EditSettingView()
        {
            InitializeComponent();

            if (DataContext is SettingViewModel sw)
            {
                sw.CloseAction = () => this.Close(); // 창 닫기 기능을 ViewModel에 연결
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
