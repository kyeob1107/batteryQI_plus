using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using batteryQI.Models;

namespace batteryQI.ViewModels.Bases
{
    internal partial class ViewModelBases : ObservableObject
    {
        // 클릭 이벤트 등록
        [RelayCommand]
        private void LinkDB()
        {
            DBlink x = new();
            x.Connect(); // 링크
        }
    }
}
