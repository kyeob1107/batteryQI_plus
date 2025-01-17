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

namespace batteryQI.Views
{
    /// <summary>
    /// Interaction logic for ErrorInfoView.xaml
    /// </summary>
    public partial class ErrorInfoView : Window
    {
        public ErrorInfoView()
        {
            InitializeComponent();
        }

        public void SetErrorInfo(BitmapImage image)
        {
            ErrorInfoImage.Source = image;
        }

        private void confirmErrorInfoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
