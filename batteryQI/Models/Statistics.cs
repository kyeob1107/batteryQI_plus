using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    internal class Statistics : ObservableObject
    {
        private string _manageID;
        private List<string> _defectList; //
        private int _totalInspectNum; // 검사한 갯수 카운트 기록

        public string ManageID
        {
            get { return _manageID; }
            set
            {
                SetProperty(ref _manageID, value);
            }
        }

        //public List<string> DefectList { }
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
