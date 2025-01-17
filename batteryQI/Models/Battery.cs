using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    public struct BatteryData
    {
        int batteryId;
        DateTime shootDate;
        string usage;
        string batteryType;
        int manufacId;
        string batteryShape;
        string shootPlace;
        string imagePath;
        int managerNum;
        bool defectStat;
        string defectName;
    }

    internal class Battery : ObservableObject
    {
        BatteryData BD {  get; set; } = new BatteryData();
        void batteryInput( BatteryData bd)
        {
            //DB에 insert관련?
        }

        void imgProcessing()
        {
            //AI를 통해서 처리하는 부분
        }
    }
}
