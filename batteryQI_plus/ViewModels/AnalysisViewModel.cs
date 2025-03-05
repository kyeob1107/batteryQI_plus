using System.Windows;
using System.Collections.ObjectModel;
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using MySql.Data.MySqlClient;
using System.Data;
using System.ComponentModel;
using ZstdSharp.Unsafe;
using System.Windows.Shapes;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Microsoft.ML.OnnxRuntime;
using System.Collections.Generic;

namespace batteryQI_plus.ViewModels
{
    public partial class AnalysisViewModel : ViewModelBases
    {
        public AnalysisViewModel()
        {
            #region 필터 초기화
            // 필터 초기화 부분
            string query_dateRange = @"SELECT 
                                        MIN(Date(inspectionDatetime)) AS minDate, 
                                        MAX(Date(inspectionDatetime)) AS maxDate 
                                       FROM inspectionResults;";
            var result_DateRange = _dblink.Select(query_dateRange);
            DateRangeStart = (DateTime)result_DateRange[0]["minDate"];
            DateRangeEnd = (DateTime)result_DateRange[0]["maxDate"];
            // 초기값 설정
            //StartDate = DateTime.Today; // 값수정 필요
            StartDate = DateRangeStart;
            StartTime = "00:00:00";
            //EndDate = DateTime.Today; // 값수정 필요
            EndDate = DateRangeEnd;
            EndTime = "23:59:59";

            // Initialize UsageItems with sample data
            UsageItems = new ObservableCollection<SelectableItem>();
            var UsageFilterlist = SelectFilterValue("usageName");
            for (int i = 0; i < UsageFilterlist.Count; i++) 
            {
                UsageItems.Add(
                    new SelectableItem 
                    { 
                        Name = (string)UsageFilterlist[i]["usageName"], IsSelected = true 
                    });
            }

            //buy는 일단 통일성있게 해주려고 id로 해주고 buyer테이블 참고해서 이름으로 표시하거나 하는 과정 추가해주기
            BuyerItems = new ObservableCollection<SelectableItem>();
            var BuyerFilterlist = SelectFilterValue("buyerName", "batteryInfo bi INNER JOIN buyers b ON bi.buyerId = b.buyerId;");
            for (int i = 0; i < BuyerFilterlist.Count; i++)
            {
                BuyerItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BuyerFilterlist[i]["buyerName"],
                        IsSelected = true
                    });
            }

            BatteryTypeItems = new ObservableCollection<SelectableItem>();
            var BatteryTypeFilterlist = SelectFilterValue("batteryType");
            for (int i = 0; i < BatteryTypeFilterlist.Count; i++)
            {
                BatteryTypeItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BatteryTypeFilterlist[i]["batteryType"],
                        IsSelected = true
                    });
            }

            BatteryShapeItems = new ObservableCollection<SelectableItem>();
            var BatteryShapeFilterlist = SelectFilterValue("batteryShape");
            for (int i = 0; i < BatteryShapeFilterlist.Count; i++)
            {
                BatteryShapeItems.Add(
                    new SelectableItem
                    {
                        Name = (string)BatteryShapeFilterlist[i]["batteryShape"],
                        IsSelected = true
                    });
            }

            StatusItems = new ObservableCollection<SelectableItem>
            {
                new SelectableItem { Name = "정상", IsSelected = true },
                new SelectableItem { Name = "오염", IsSelected = true },
                new SelectableItem { Name = "손상", IsSelected = true },
                new SelectableItem { Name = "오염 & 손상", IsSelected = true }
            };

            ProductionLineItems = new ObservableCollection<SelectableItem>();
            var ProductionLineFilterlist = SelectFilterValue("lineId");
            for (int i = 0; i < ProductionLineFilterlist.Count; i++)
            {
                ProductionLineItems.Add(
                    new SelectableItem
                    {
                        Name = "Line" + ProductionLineFilterlist[i]["lineId"].ToString(),
                        IsSelected = true
                    });
            }

            BatteryId = "";
            #endregion

            // 타임차트
            var lineCountquery = _dblink.Select($"SELECT COUNT(DISTINCT lineId) AS Count FROM productionLines;");
            numOfLine = Convert.ToInt32(lineCountquery[0]["Count"]) - 1;
            DrawTimeChart();
            // 타임차트 y값 포맷
            YFormatter = value => value.ToString("N");

            // 파이차트
            DrawPieChart();
            // 파이차트 라벨 표시관련
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            // column차트 - 따로 메소드 만들어서 사용하는 식으로 해야할듯
            DrawColumnChart();

            // 테이블
            LoadData();

        }

        [RelayCommand]
        private void Search()
        {

            //Console.WriteLine($"{GetStartDateTime().ToString("yyyy-MM-dd HH:mm:ss")} ~ {GetEndDateTime().ToString("yyyy-MM-dd HH:mm:ss")}");
            // 검색 조건 설정 내용 출력
            //Console.WriteLine("디버깅:" + string.Join(", ", temp) + ": 여기까지");
            //수정필요
            var filterResults = FilterBatteryIds();
            string filteredBatteryIds_string = string.Join(", ", filterResults[0]);
            string filteredBatteryIds_string2 = string.Join(", ", filterResults[1]);
            //Console.WriteLine("리스트비어있으면" + filterResults[1].Count.ToString() + filteredBatteryIds_string2 + $"길이는 {filteredBatteryIds_string2.Length}");
            //Console.WriteLine("필터로 쓰이는건" + filteredBatteryIds_string);
            string filterCondition = filteredBatteryIds_string.Length>0 ? $"batteryId IN ({filteredBatteryIds_string})" : "TRUE";
            string filterCondition2 = filteredBatteryIds_string2.Length > 0 ? $"batteryId NOT IN ({filteredBatteryIds_string2})" : "TRUE";
            DrawTimeChart(filterCondition, filterCondition2);
            DrawPieChart(filterCondition, filterCondition2);
            DrawColumnChart(filterCondition, filterCondition2);
            LoadData(filterCondition, filterCondition2);
            //MessageBox.Show(BatteryId);
        }
    }
}
