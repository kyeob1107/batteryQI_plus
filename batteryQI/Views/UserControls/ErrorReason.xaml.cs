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

namespace batteryQI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorReason.xaml
    /// </summary>
    public partial class ErrorReason : UserControl
    {
        // 선택된 데이터 전달 이벤트
        public event Action<string> ErrorConfirmed;

        public ErrorReason()
        {
            InitializeComponent();
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

        //private void ShowErrorInfo()
        //{
        //    var errorInfoView = new ErrorInfoView();

        //    //if (PreviewImage.Source is BitmapImage previewImageSource)
        //    //{
        //    //    inspectionImage.SetImage(previewImageSource);
        //    //}

        //    Window.GetWindow(this).Close();
        //    errorInfoView.ShowDialog();
        //}

        private void ErrorConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (errorReasonCombo.SelectedIndex > 0)
            {
                // 선택된 콤보박스 값 전달
                string selectedReason = (errorReasonCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
                ErrorConfirmed?.Invoke(selectedReason);
            }
            else
            {
                MessageBox.Show("불량 유형을 선택해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
