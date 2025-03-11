using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI_plus.Models.Bases
{
    public partial class ModelBase : ObservableObject
    {
        // DB 객체 생성
        protected DBlink _dblink;
        public ModelBase()
        {
            // 객체 연결
            _dblink = DBlink.Instance();
        }
    }
}
