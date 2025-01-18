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
using batteryQI.Views.UserControls;

namespace batteryQI.Views
{
    /// <summary>
    /// Interaction logic for InspectionImage.xaml
    /// </summary>
    public partial class InspectionImage : Window
    {
        public InspectionImage()
        {
            InitializeComponent();
        }

        public void SetImage(BitmapImage image)
        {
            AnalyzeImage.Source = image;
        }

        private void NomalButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ErrorButton_Click(object sender, RoutedEventArgs e)
        {
            inspectionSection.Visibility = Visibility.Collapsed;

            var errorReasonControl = new UserControls.ErrorReason();
            errorReasonControl.ErrorConfirmed += OnErrorReasonConfirmed;

            InspectionFrame.Content = errorReasonControl;
        }

        private void OnErrorReasonConfirmed(string selectedReason)
        {
            if (AnalyzeImage.Source is BitmapImage analyzeImage)
            {
                var errorInfoView = new ErrorInfoView();
                errorInfoView.SetErrorInfo(analyzeImage);
                errorInfoView.ShowDialog();
            }
            else
            {
                MessageBox.Show("이미지를 로드할 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
