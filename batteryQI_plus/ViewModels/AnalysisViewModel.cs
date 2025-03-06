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
        int numOfLine;
        public AnalysisViewModel()
        {
            // 필터 초기화 부분
            InitializeFilter();

           // 체크한 line 갯수 기록
           List<string>lineIds = new List<string>();
            foreach (var item in _productionLineItems)
            {
                if (item.IsSelected) { lineIds.Add(item.Name.Replace("Line", "")); }
            }
            string lineIdCondition = lineIds.Count > 0 ? $"lineId IN({string.Join(',', lineIds)})" : "FALSE";
            //string lineCountQuery = $@"SELECT COUNT(DISTINCT lineId) AS Count 
            //               FROM productionLines
            //               WHERE {lineIdCondition};";
            string lineCountQuery = $@"SELECT COUNT(DISTINCT lineId) AS Count 
                           FROM productionLines;";
            var lineCountResult = _dblink.Select(lineCountQuery);
            numOfLine = Convert.ToInt32(lineCountResult[0]["Count"]) - 1;
            
            // 타임차트
            DrawTimeChart();
            // 타임차트 y값 포맷
            YFormatter = value => value.ToString("N");

            // 파이차트
            DrawPieChart();
            // 파이차트 라벨 표시관련 - 초기화로 넘겨도 될듯?
            PointLabelPie = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            // column차트 - 따로 메소드 만들어서 사용하는 식으로 해야할듯
            DrawColumnChart();
            // 콜롬차트 라벨 표시관련 - 초기화로 넘겨도 될듯?
            PointLabelColumn = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            FormatterColumn = value => value.ToString("N0");

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

        // view쪽 컨트롤 설정 관련
        private int _selectedTabIndex;
        public bool IsTextBoxEnabled => SelectedTabIndex == 3;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (SetProperty(ref _selectedTabIndex, value))
                {
                    OnPropertyChanged(nameof(IsTextBoxEnabled));
                }
            }
        }
    }
}
