using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI_plus.Models
{
    //public List<object>? analysisComboxValues; // 이건 그냥 viewmodel에서 바로 쓰면 될 것 같아서
    //public struct filterData
    //{
    //    public DateTime startDatetime; // 여러개 선택 불가
    //    public DateTime endDatetime; // 여러개 선택 불가
    //    public List<string> status; // 정상, 오염, 파손
    //    public List<string> usageNmae; //
    //    public List<string> buyer;
    //    public List<string> batteryType;
    //    public List<string> batteryShape;
    //    public List<string> productionLine; // 근무자는 제한되게 볼 수 있도록?
    //}

    public class AnalysisModel : ObservableObject
    {
        // 아예 chartModel로 빼거나 TimeChartModel로 더 세부적으로 나눌까 고민중
        private List<DateTime>? _timeChartX;
        private List<double>? _defectRate;
        public List<DateTime>? TimeChartX
        {
            get { return _timeChartX; }
            set { SetProperty(ref _timeChartX, value); }
        }

        public List<double>? DefectRate
        {
            get { return _defectRate; }
            set { SetProperty(ref _defectRate, value); }
        }

        // 초기화 어떻게 해야할지 안떠올라서 일단 틀만 해둠
        public AnalysisModel(string optioin) 
        { 
            if (optioin == "timeChart") { InitializeTimeChart(); }
        }

        private void InitializeTimeChart()
        {
            //_timeChartX = DateTime.Now;
            //_defectRate = 0.0;
        }
    }
}
