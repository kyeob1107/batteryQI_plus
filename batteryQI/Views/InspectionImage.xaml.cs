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
using batteryQI.ViewModels;
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
            this.DataContext = new InspectViewModel();
        }
    }
}
