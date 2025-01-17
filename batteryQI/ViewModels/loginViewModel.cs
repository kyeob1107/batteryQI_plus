using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.Models;
using batteryQI.ViewModels.Bases;

namespace batteryQI.ViewModels
{
    internal class loginViewModel : ViewModelBases
    {
        public loginViewModel()
        {
            // 로그인 창 열면서 DB 연결
            DBlink DBConnection = new DBlink();
            DBConnection.Connect();
        }
    }
}
