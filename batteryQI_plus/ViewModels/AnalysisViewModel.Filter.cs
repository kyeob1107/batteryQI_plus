using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using batteryQI_plus.Models;
using batteryQI_plus.ViewModels.Bases;
using MySql.Data.MySqlClient;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel
    {
        private DateTime _startDate;
        private string _startTime;
        private DateTime _endDate;
        private string _endTime;
        private DateTime _dateRangeStart;
        private DateTime _dateRangeEnd;
        private string _batteryId;
        private ObservableCollection<SelectableItem> _usageItems;
        private ObservableCollection<SelectableItem> _buyerItems;
        private ObservableCollection<SelectableItem> _batteryTypeItems;
        private ObservableCollection<SelectableItem> _batteryShapeItems;
        private ObservableCollection<SelectableItem> _statusItems;
        private ObservableCollection<SelectableItem> _productionLineItems;

        private List<List<string>> FilterBatteryIds()
        {
            Console.WriteLine("필터메소드 작동\r\n");
            string filter = "";
            filter += @$"(inspectionDatetime BETWEEN '{GetStartDateTime().ToString("yyyy-MM-dd HH:mm:ss")}' 
                        AND '{GetEndDateTime().ToString("yyyy-MM-dd HH:mm:ss")}')";
            /*
            _usageItems
            _buyerItems
            _batteryTypeItems
            _batteryShapeItems
            ------------------------------------------------------------
            _statusItems - 이건 그냥하면 안됨
            _productionLineItems - pie는 라인을 데이터 가져올때 용
             */
            List<string> filteredBatteryIds = new List<string>();
            List<string> filteredBatteryIds_status = new List<string>();
            Dictionary<string, List<string>> checkedList = new Dictionary<string, List<string>>
            {
                { "usageName", new List<string>() },
                { "buyerName", new List<string>() },
                { "batteryType", new List<string>() },
                { "batteryShape", new List<string>() },
                { "lineId", new List<string>() }
            };
            // 체크내용 확인
            foreach (var item in _usageItems)
            {
                if (item.IsSelected) { checkedList["usageName"].Add(item.Name); }
            }
            foreach (var item in _buyerItems)
            {
                if (item.IsSelected) { checkedList["buyerName"].Add(item.Name); }
            }
            foreach (var item in _batteryTypeItems)
            {
                if (item.IsSelected) { checkedList["batteryType"].Add(item.Name); }
            }
            foreach (var item in _batteryShapeItems)
            {
                if (item.IsSelected) { checkedList["batteryShape"].Add(item.Name); }
            }
            foreach (var item in _productionLineItems)
            {
                if (item.IsSelected) { checkedList["lineId"].Add(item.Name.Replace("Line","")); }
            }

            foreach (KeyValuePair<string, List<string>> item in checkedList)
            {
                if (item.Value.Count > 0)
                {
                    if (filter != "") filter += " AND ";
                    filter += $"{item.Key} IN ({string.Join(", ", item.Value.Select(v => $"\'{v}\'"))})";

                }
            }
            if (filter == "") { filter = "False"; }
            Console.WriteLine(filter);
            string query_filter = @$"SELECT batteryId 
                                     FROM batteryInfo bi 
                                     INNER JOIN buyers b ON bi.buyerId = b.buyerId
                                     WHERE {filter};";

            // 상태에 따른 batteryId값 조건에 추가 - 얘는 보통 다 체크해두고 쓸듯하여 체크해제한 얘를 제거하는 방식으로 구현
            List<string> notCheckedList_Status = new List<string>();
            foreach (var item in _statusItems)
            {
                if (!item.IsSelected) { notCheckedList_Status.Add(item.Name); }
            }

            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _dblink.connection;
                    // 종합적인 필터부분
                    cmd.CommandText = query_filter;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object value = reader.GetValue(0);
                            filteredBatteryIds.Add(value.ToString());
                        }
                        //Console.WriteLine($"값불러오는 것도 했음: {string.Join(", ", filteredBatteryIds)}");
                    }
                    if (notCheckedList_Status.Count > 0)
                    {
                        // status 필터부분
                        string query_statusfilter = @$"SELECT batteryId,
                                                        CASE
                                                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) = 0 THEN '정상'
                                                            WHEN SUM(fastPollutionCheck) <> 0 AND SUM(fastDamageCheck ) = 0 THEN '오염'
                                                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck ) <> 0 THEN '손상'
                                                            ELSE '오염 & 손상'
                                                        END AS Status
                                                    FROM inspectionResults
                                                    GROUP BY batteryId
                                                    HAVING Status IN ({string.Join(',', notCheckedList_Status.Select(v => $"\'{v}\'"))});";
                        cmd.CommandText = query_statusfilter;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object value = reader.GetValue(0);
                                filteredBatteryIds_status.Add(value.ToString());
                            }
                            //Console.WriteLine($"값불러오는 것도 했음: {string.Join(", ", filteredBatteryIds)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_dblink.connection == null)
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message} 연결 안됨 에러위치: {ex.StackTrace}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show($"데이터베이스 접속 오류 \r\n 에러메시지: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //Console.WriteLine(string.Join(", ", filteredBatteryIds));
            List<List<string>> result = new List<List<string>> { filteredBatteryIds, filteredBatteryIds_status };
            return result;
        }

        // 시간을 파싱하고 유효성을 검사하는 메서드
        private DateTime CombineDateAndTime(DateTime date, string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan timeSpan))
            {
                return date.Date + timeSpan;
            }
            return date;
        }

        // 날짜와 시간을 결합하여 완전한 DateTime을 얻는 메서드
        public DateTime GetStartDateTime()
        {
            return CombineDateAndTime(StartDate, StartTime);
        }
        public DateTime GetEndDateTime()
        {
            return CombineDateAndTime(EndDate, EndTime);
        }
        
        private List<Dictionary<string, object>> SelectFilterValue(string category, string table = "batteryInfo")
        {
            string query = $"SELECT DISTINCT {category} FROM {table};";
            var FilterValueList = _dblink.Select(query);
            return FilterValueList;
        }

        // 프로퍼티
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }
        public string StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }
        public string EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }
        public DateTime DateRangeStart
        {
            get => _dateRangeStart;
            set => SetProperty(ref _dateRangeStart, value);
        }
        public DateTime DateRangeEnd
        {
            get => _dateRangeEnd;
            set => SetProperty(ref _dateRangeEnd, value);
        }
        public string BatteryId
        {
            get { return _batteryId; }
            set
            {
                SetProperty(ref _batteryId, value);
                //MessageBox.Show(BatteryId); // 디버그용
            }
        }
        public ObservableCollection<SelectableItem> UsageItems
        {
            get => _usageItems;
            set => SetProperty(ref _usageItems, value);
        }
        public ObservableCollection<SelectableItem> BuyerItems
        {
            get => _buyerItems;
            set => SetProperty(ref _buyerItems, value);
        }
        public ObservableCollection<SelectableItem> BatteryTypeItems
        {
            get => _batteryTypeItems;
            set => SetProperty(ref _batteryTypeItems, value);
        }
        public ObservableCollection<SelectableItem> BatteryShapeItems
        {
            get => _batteryShapeItems;
            set => SetProperty(ref _batteryShapeItems, value);
        }
        public ObservableCollection<SelectableItem> StatusItems
        {
            get => _statusItems;
            set => SetProperty(ref _statusItems, value);
        }
        public ObservableCollection<SelectableItem> ProductionLineItems
        {
            get => _productionLineItems;
            set => SetProperty(ref _productionLineItems, value);
        }
    }
}
