using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using batteryQI_plus.Models;
using batteryQI_plus.Views;
using System.Windows.Controls;
using System.Data.Common;

namespace batteryQI_plus.ViewModels.Bases
{
    public partial class ViewModelBases : ObservableObject, IDisposable
    {
        // DB 객체 생성
        protected DBlink _dblink;
        public ViewModelBases()
        {
            // 객체 연결
            _dblink = DBlink.Instance();
        }

        // Dispose 메서드 구현
        public virtual void Dispose()
        {
            if (_dblink != null)
            {
                _dblink.Dispose(); // 리소스 해제
                _dblink = null;
            }
        }
    }
}
