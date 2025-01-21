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
        private string _managerID; // 담당자 아이디
        private string _managerPW; // 담당자 비번
        private int _workAmount; // 담당자 할당량
        private int _totalInspectNum; // 오늘 수행량 저장 변수
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
        public int WorkAmount
        {
            get { return _workAmount; }
            set
            {
                SetProperty(ref _workAmount, value);
            }
        }
        public int TotalInspectNum
        {
            get { return _totalInspectNum; }
            set
            {
                SetProperty(ref _totalInspectNum, value);
            }
        }
    }
}
