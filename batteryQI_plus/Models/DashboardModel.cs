using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using batteryQI_plus.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI_plus.Models
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 주어진 DateTime을 지정된 분 단위로 내림(Floor)하는 확장 메서드
        /// </summary>
        /// <param name="dt">원본 DateTime</param>
        /// <param name="minutes">반올림할 분 단위 (예: 10분)</param>
        /// <returns>지정된 분 단위로 내림된 새 DateTime</returns>
        public static DateTime FloorToNearestMinutes(this DateTime dt, int minutes)
        {
            // 지정된 분을 TimeSpan으로 변환
            var timeSpan = TimeSpan.FromMinutes(minutes);

            // 원본 DateTime의 Ticks 값을 가져옴
            var ticks = dt.Ticks;

            // 결과를 저장할 long 타입 변수 초기화
            //var roundedTicks = 0L; // 이 방식 어색해서 수정
            long roundedTicks = 0;

            // Ticks를 지정된 분 단위로 나누고 내림한 후, 다시 Ticks로 변환
            roundedTicks = ((long)Math.Floor((double)ticks / timeSpan.Ticks)) * timeSpan.Ticks;

            // 계산된 Ticks로 새 DateTime 객체를 생성하여 반환
            // dt.Kind를 사용하여 원본과 동일한 종류(Local, UTC 등)의 DateTime을 생성
            return new DateTime(roundedTicks, dt.Kind);
        }
    }

    public class DashboardModel : ObservableObject
    {
        //private int _lineId;
        private DateTime _startDatetime;
        private DateTime _endDatetime;
        private int _inspectionCount;
        private int _normalCount;
        private int _defectCount;
        private double _defectRate;

        public DashboardModel() 
        {
            //_lineId = 1;
            _startDatetime = DateTime.Now.AddHours(-1);
            _endDatetime = DateTime.Now;
            _inspectionCount = 0;
            _normalCount = 0;
            _defectCount = 0;
            _defectRate = 0;
        }

        public DashboardModel(DBlink db, Employee empl, int line) 
        {
            //_lineId = 1;
            //_startDatetime = DateTime.Now.AddHours(-1);
            //_endDatetime = DateTime.Now;
            //_inspectionCount = 0;
            //_normalCount = 0;
            //_defectCount = 0;
            //_defectRate = 0;
            //_lineId = empl.LineId;
            _startDatetime = empl.LastLogoutDateTime.FloorToNearestMinutes(10);
            _endDatetime = DateTime.Now.FloorToNearestMinutes(10);

            // 일단 임시로 해둔 것
            #region 검사 수 & 불량 수
            string unitTestQuery1 = $@"SELECT 
                                            COUNT(lineId) AS cnt
                                        FROM 
	                                        inspectionResults
                                        WHERE 
	                                        lineId = {line} 
	                                        AND (inspectionDatetime BETWEEN '{_startDatetime}' AND '{_endDatetime}')
                                        GROUP BY lineId";
            string unitTestQuery2 = $@"SELECT 
	                                         COUNT(lineId) AS cnt
                                        FROM 
	                                        inspectionResults
                                        WHERE 
	                                        lineId = {line} 
	                                        AND (inspectionDatetime BETWEEN '{_startDatetime}' AND '{_endDatetime}')
	                                        AND (fastPollutionCheck = 1 OR fastDamageCheck = 1)
                                        GROUP BY lineId;";
            #endregion
            List<Dictionary<string, object>> result1 = db.Select(unitTestQuery1);
            List<Dictionary<string, object>> result2 = db.Select(unitTestQuery2);
            //MessageBox.Show(_startDatetime.ToString() + "," + _endDatetime.ToString());
            #region 디버깅 용
            // 디버깅 용
            // 결과를 문자열로 변환
            //string message = "Query Result:\n";
            //foreach (var row in result1)
            //{
            //    foreach (var key in row.Keys)
            //    {
            //        message += $"{key}: {row[key]}\n";
            //    }
            //    message += "----------------------\n"; // 행 구분선
            //}

            //// 메시지 박스에 출력
            //MessageBox.Show(message, "Query Result");

            //string message2 = "Query Result:\n";
            //foreach (var row in result2)
            //{
            //    foreach (var key in row.Keys)
            //    {
            //        message2 += $"{key}: {row[key]}\n";
            //    }
            //    message2 += "----------------------\n"; // 행 구분선
            //}

            //// 메시지 박스에 출력
            //MessageBox.Show(message2, "Query Result");
            #endregion

            _inspectionCount = Convert.ToInt32(result1[0]["cnt"]);
            _defectCount = Convert.ToInt32(result2[0]["cnt"]);
            _normalCount = _inspectionCount - _defectCount;
            //_defectRate = 0;
            UpdateDefectRate();
        }

        // 급해서 일단 임시로 만듦
        public DashboardModel(DBlink db, Employee empl, int line, string total)
        {
            //MessageBox.Show("total용");
            //_lineId = empl.LineId;
            _startDatetime = DateTime.Now.FloorToNearestMinutes(10);
            _endDatetime = DateTime.Now.FloorToNearestMinutes(10);
            _inspectionCount = 0;
            _defectCount = 0;
            _normalCount = 0;
            _defectRate = 0;
            //UpdateDefectRate();
        }

        //public int LineId 
        //{ 
        //    get { return _lineId; } 
        //    set { SetProperty(ref _lineId, value); } 
        //}

        public DateTime StartDatetime
        {
            get { return _startDatetime; }
            set { SetProperty(ref _startDatetime, value); }
        }

        public DateTime EndDatetime
        {
            get { return _endDatetime; }
            set { SetProperty(ref _endDatetime, value); }
        }

        public int InspectionCount
        {
            get { return _inspectionCount; }
            set 
            { 
                SetProperty(ref _inspectionCount, value);
                UpdateDefectRate();
            }
        }

        public int NormalCount
        {
            get { return _normalCount; }
            set 
            { 
                SetProperty(ref _normalCount, value);
                UpdateDefectRate();
            }
        }

        public int DefectCount
        {
            get { return _defectCount; }
            set 
            { 
                SetProperty(ref _defectCount, value);
                UpdateDefectRate();
            }
        }

        public double DefectRate
        {
            get { return _defectRate; }
            set 
            { 
                SetProperty(ref _defectRate, value); 
            }
        }

        private void UpdateDefectRate()
        {
            if (this._inspectionCount == 0)
            {
                DefectRate = 0;
            }
            else
            {
                DefectRate = 100 * (double)this._defectCount / this._inspectionCount;
            }
        }
        //public struct monitoringData
        //{
        //    public int lineId;
        //    public DateTime startDatetime;
        //    public DateTime endDatetime;
        //    public int inspectionCount;
        //    public int normalCount;
        //    public int defectCount;
        //    public double defectRate;
        //}
    }
}
