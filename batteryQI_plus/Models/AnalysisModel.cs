using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batteryQI_plus.Models
{
    public class AnalysisModel
    {
        //public List<object>? analysisComboxValues; // 이건 그냥 viewmodel에서 바로 쓰면 될 것 같아서

        public struct filterData
        {
            public DateTime startDatetime; // 여러개 선택 불가
            public DateTime endDatetime; // 여러개 선택 불가
            public List<string> status; // 정상, 오염, 파손
            public List<string> usageNmae; //
            public List<string> buyer;
            public List<string> batteryType;
            public List<string> batteryShape;
            public List<string> productionLine; // 근무자는 제한되게 볼 수 있도록?
        }
    }
}
