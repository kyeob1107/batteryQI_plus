using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batteryQI.ViewModels
{
    // 주석 달아주시죠.. 이거 만드신 분.. 
    class CompositeViewModel
    {
        public InspectViewModel InspectViewModel { get; set; }
        public ManagerViewModel ManagerViewModel { get; set; }

        public CompositeViewModel()
        {
            InspectViewModel = new InspectViewModel();
            ManagerViewModel = new ManagerViewModel();
        }
    }
}
