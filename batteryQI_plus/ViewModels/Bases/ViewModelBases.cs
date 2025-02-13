using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using batteryQI.Models;
using batteryQI.Views;
using System.Windows.Controls;

namespace batteryQI.ViewModels.Bases
{
    public partial class ViewModelBases : ObservableObject
    {
        // DB 객체 생성
        protected DBlink _dblink;
        public ViewModelBases()
        {
            // 객체 연결
            _dblink = DBlink.Instance(); 
        }
    }
}
