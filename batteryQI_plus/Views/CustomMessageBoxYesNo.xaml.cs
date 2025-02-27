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
    /// Interaction logic for CustomMessageBoxYesNo.xaml
    /// </summary>
    public partial class CustomMessageBoxYesNo : Window
    {
        public bool Result { get; private set; }

        public CustomMessageBoxYesNo(string message)
        {
            InitializeComponent();
            //MessageText.Text = message;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        public static bool Show(string message)
        {
            CustomMessageBoxYesNo box = new CustomMessageBoxYesNo(message);
            box.ShowDialog();
            return box.Result;
        }
    }
}
