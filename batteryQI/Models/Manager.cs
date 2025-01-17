using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    // 싱글톤 패턴
    internal class Manager : ObservableObject
    {
        private string _managerID;
        private string _managerPW;
        static Manager manager; // singleton
        public static Manager Instance()
        {
            if(manager == null)
            {
                manager = new Manager(); // Manager 객체 생성
            }
            return manager;
        }
        public string ManagerID
        {
            get { return _managerID; }
            set
            {
                SetProperty(ref _managerID, value);
            }
        }
        public string ManagerPW
        {
            get { return _managerPW; }
            set
            {
                SetProperty(ref _managerPW, value);
            }
        }

    }
}
