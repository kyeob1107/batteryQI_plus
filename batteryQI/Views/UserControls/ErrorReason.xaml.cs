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
using batteryQI.Views;
using batteryQI.ViewModels;

namespace batteryQI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorReason.xaml
    /// </summary>
    public partial class ErrorReason : UserControl
    {
        public ErrorReason()
        {
            InitializeComponent();
            //this.DataContext = new InspectViewModel();
        }

        //private void ErrorConfirmButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (errorReasonCombo.SelectedIndex != 0)
        //    {
        //        ShowErrorInfo();
        //    }
        //    else
        //    {
        //        MessageBox.Show("불량 유형을 선택해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}
    }
}
