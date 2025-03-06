using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using batteryQI_plus.ViewModels;

namespace batteryQI_plus.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ManagerPage.xaml
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
            //this.DataContext = new ManagerViewModel();
        }
    }
}
