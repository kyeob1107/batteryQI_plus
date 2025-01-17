using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    public class BatteryData
    {
        public int batteryId { get; set; }
        public DateTime shootDate { get; set; }
        public string usage { get; set; }
        public string batteryType { get; set; }
        public int manufacId { get; set; }
        public string batteryShape { get; set; }
        public string shootPlace { get; set; }
        public string? imagePath { get; set; }
        public int managerNum { get; set; }
        public bool defectStat { get; set; }
        public string defectName { get; set; }

        public BatteryData()
        {
            this.shootDate = DateTime.Now;
            this.batteryType = "";
            this.usage = "";
            this.batteryShape = "";
            this.shootPlace = "";
            this.imagePath = null;
            this.defectName = "";
        }
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
