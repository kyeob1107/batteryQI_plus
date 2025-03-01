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
using batteryQI_plus.ViewModels;
using batteryQI_plus.Views.UserControls;

namespace batteryQI_plus.Views
{
    /// <summary>
    /// Interaction logic for InspectionImage.xaml
    /// </summary>
    public partial class InspectionImage : Window
    {
        public InspectionImage()
        {
            InitializeComponent();
            //this.DataContext = new InspectViewModel();
        }
    }
}
