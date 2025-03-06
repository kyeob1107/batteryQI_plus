using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class TimeSeriesModel : ObservableObject
    {
        // 아예 chartModel로 빼거나 TimeChartModel로 더 세부적으로 나눌까 고민중
        #region 처음 설계했던 것 - 안씀
        //private List<DateTime>? _timeChartX;
        //private List<double>? _defectRate;
        //public List<DateTime>? TimeChartX
        //{
        //    get { return _timeChartX; }
        //    set { SetProperty(ref _timeChartX, value); }
        //}

        //public List<double>? DefectRate
        //{
        //    get { return _defectRate; }
        //    set { SetProperty(ref _defectRate, value); }
        //}
        #endregion
        private List<DateTime>? _timeList;
        private List<double>? _countValue;

        // 초기화 어떻게 해야할지 안떠올라서 일단 틀만 해둠
        public TimeSeriesModel(List<Dictionary<string, Object>> queryresult)
        {
            //_timeChartX = DateTime.Now;
            //_defectRate = 0.0;
            _timeList = new List<DateTime>();
            _countValue = new List<double>();
            for (int i = 0; i < queryresult.Count; i++)
            {
                //_timeList.Add((DateTime)queryresult[i]["time_interval"]);
                // "time_interval" 값을 문자열로 가져오기
                string timeIntervalString = queryresult[i]["time_interval"].ToString();
                //Console.WriteLine(timeIntervalString);
                // 문자열을 DateTime으로 변환
                if (DateTime.TryParse(timeIntervalString, out DateTime parsedDate))
                {
                    // 변환 성공 시 _timeList에 추가
                    _timeList.Add(parsedDate);
                }
                else
                {
                    // 변환 실패 시 로그 출력 또는 예외 처리
                    Console.WriteLine($"Invalid date format: {timeIntervalString}");
                }
                //Console.WriteLine("값은" + queryresult[i]["normal_cnt"].ToString() + "," + queryresult[i]["defect_cnt"].ToString());
                int normalCount = Convert.ToInt32(queryresult[i]["normal_cnt"]);
                int defectCount = Convert.ToInt32(queryresult[i]["defect_cnt"]);
                double defectRate_timeChart = (defectCount + normalCount) > 0 ? 100 * (double)defectCount / (defectCount + normalCount) : -0.001;
                _countValue.Add(defectRate_timeChart);
            }
        }

        public List<DateTime>? TimeList
        {
            get { return _timeList; }
            set { SetProperty(ref _timeList, value); }
        }
        public List<double>? CountValue
        {
            get { return _countValue; }
            set { SetProperty(ref _countValue, value); }
        }
    }
}
