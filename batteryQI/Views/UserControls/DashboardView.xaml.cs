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
using Microsoft.Win32;

namespace batteryQI.UserControls
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void ImageSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PreviewImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }


        private void ImageInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (PreviewImage.Source == null)
            {
                MessageBox.Show("배터리 이미지가 업로드되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if(manufacture.SelectedIndex == 0 || batteryType.SelectedIndex == 0 || batteryShape.SelectedIndex == 0 || usage.SelectedIndex == 0)
            {
                MessageBox.Show("배터리 이미지 정보를 모두 기입해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                InspectionImage();
            }
        }

        private void InspectionImage()
        {
            var inspectionImage = new InspectionImage();

            if (PreviewImage.Source is BitmapImage previewImageSource)
            {
                inspectionImage.SetImage(previewImageSource);
            }

            inspectionImage.ShowDialog();
        }

    }
}
