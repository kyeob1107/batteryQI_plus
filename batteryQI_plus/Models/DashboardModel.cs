using System.CodeDom.Compiler;
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

        // 실시간 갱신 관련
        private int _autoRefreshUnitTime; // 자동 실시간 시간반올림 단위
        private int _manualRefreshUnitTime; // 수동 실시간 시간반올림 단위

        // 진행도 관련
        private int? _previousInspectionCount; // 이전 검사한 량 batteryId counts
        private int? _quota; // 할당량
        private int? _totalInspectionCount; // 이전 검사량 + 현재 단위검사로 추가 검사한 량
        private double? _previousProgress; // 이전 검사량으로 계산한 진행도
        private double? _totalProgress;

        public DashboardModel() 
        {
            //_lineId = 1;
            _startDatetime = DateTime.Now.AddHours(-1);
            _endDatetime = DateTime.Now;
            _inspectionCount = 0;
            _normalCount = 0;
            _defectCount = 0;
            _defectRate = 0;
            _previousInspectionCount = 0;
            _quota = 0;
            _totalInspectionCount = 0;
            _previousProgress = 0;
            _totalProgress = 0;
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
            //MessageBox.Show(_startDatetime.ToString());
            //MessageBox.Show(_endDatetime.ToString());
            //Console.WriteLine(_startDatetime.ToString()+"~"+_endDatetime.ToString());

            // 일단 임시로 해둔 것
            #region 검사 수 & 불량 수
            string unitTestQuery1 = $@"WITH StatusList AS (
                                                    SELECT 'normal' AS Status
                                                    UNION ALL
                                                    SELECT 'defect'
                                                )
                                                SELECT s.Status, COALESCE(COUNT(subquery.batteryId), 0) AS cnt
                                                FROM StatusList s
                                                LEFT JOIN (
                                                    SELECT batteryId,
                                                        CASE
                                                            WHEN SUM(fastPollutionCheck) = 0 AND SUM(fastDamageCheck) = 0 THEN 'normal'
                                                            ELSE 'defect'
                                                        END AS Status
                                                    FROM batteryQIPlus.inspectionResults ir
                                                    WHERE lineId = {line}
                                                        AND (inspectionDatetime BETWEEN '{_startDatetime.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_endDatetime.ToString("yyyy-MM-dd HH:mm:ss")}')
                                                    GROUP BY batteryId
                                                ) AS subquery ON s.Status = subquery.Status
                                                GROUP BY s.Status
                                                ORDER BY CASE WHEN s.Status = 'normal' THEN 0 ELSE 1 END;";

            

            // 디버깅용 쿼리
            //string unitTestQuery1 = $@"SELECT 
            //                                 DISTINCT batteryId, lineId, inspectionDatetime, fastPollutionCheck, fastDamageCheck
            //                                FROM 
            //                                 inspectionResults
            //                                WHERE 
            //                                 lineId = {line} 
            //                                 AND (inspectionDatetime BETWEEN '{_startDatetime.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_endDatetime.ToString("yyyy-MM-dd HH:mm:ss")}');";
            //Console.WriteLine(unitTestQuery1);

            //string unitTestQuery2 = $@"SELECT 
            //                                 DISTINCT batteryId, lineId, inspectionDatetime, fastPollutionCheck, fastDamageCheck
            //                                FROM 
            //                                 inspectionResults
            //                                WHERE 
            //                                 lineId = {line} 
            //                                 AND (inspectionDatetime BETWEEN '{_startDatetime.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{_endDatetime.ToString("yyyy-MM-dd HH:mm:ss")}')
            //                                 AND (fastPollutionCheck = 1 OR fastDamageCheck = 1);";
            #endregion
            List<Dictionary<string, object>> result1 = db.Select(unitTestQuery1);
            //MessageBox.Show(_startDatetime.ToString() + "," + _endDatetime.ToString());
            #region 디버깅 용
            ////디버깅 용
            //// 결과를 문자열로 변환
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
            ////MessageBox.Show(message, "Query Result");
            //Console.WriteLine(message);

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
            ////MessageBox.Show(message2, "Query Result");
            //Console.WriteLine(message2);

            #endregion
            _normalCount = Convert.ToInt32(result1[0]["cnt"]);
            _defectCount = Convert.ToInt32(result1[1]["cnt"]);
            _inspectionCount = _normalCount + _defectCount;
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

            // previous쿼리의 경우 나중에는 taskid로 나눠서 세도록 변경해야함
            string previousInspectionCountQuery = $@"SELECT COUNT(*) AS cnt
                                                    FROM batteryInfo 
                                                    WHERE lineId = {line};";

            string quotaQuery = $@"SELECT quota 
                                    FROM productionLines 
                                    WHERE lineId = {line};";

            List<Dictionary<string, object>> result_precount = db.Select(previousInspectionCountQuery);
            List<Dictionary<string, object>> result_quota = db.Select(quotaQuery);
            _previousInspectionCount = Convert.ToInt32(result_precount[0]["cnt"]);
            _quota = Convert.ToInt32(result_quota[0]["quota"]);
            _totalInspectionCount = _previousInspectionCount;
            if (_quota > 0 && _previousInspectionCount.HasValue && _previousInspectionCount >= 0)
            {
                double? tempValue = 100 * (double)this._previousInspectionCount / this._quota;
                _previousProgress = tempValue <= 100 ? tempValue : 100;
            }
            else { _previousProgress = 0; }
            UpdateProgress();
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

        public int? PreviousInspectionCount
        {
            get { return _previousInspectionCount; }
            set 
            { 
                SetProperty(ref _previousInspectionCount, value);
                UpdateProgress();
            }
        }

        public int? Quota
        {
            get { return _quota; }
            set 
            { 
                SetProperty(ref _quota, value);
                UpdateProgress();
            }
        }

        public int? TotalInspectionCount
        {
            get { return _totalInspectionCount; }
            set 
            { 
                SetProperty(ref _totalInspectionCount, value);
                UpdateProgress();
            }
        }

        public double? PreviousProgress
        {
            get { return _previousProgress; }
            set { SetProperty(ref _previousProgress, value); } 
        }

        public double? TotalProgress
        {
            get { return _totalProgress; }
            set { SetProperty(ref _totalProgress, value); }
        }

        public int AutoRefreshUnitTime
        {
            get { return _autoRefreshUnitTime; }
            //set { SetProperty(ref _autoRefreshUnitTime, value); }
        }

        public int ManualRefreshUnitTime
        {
            get { return _manualRefreshUnitTime; }
            //set { SetProperty(ref _manualRefreshUnitTime, value); }
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

        private void UpdateProgress()
        {
            if (this._quota == 0 || !(_totalInspectionCount.HasValue))
            {
                //PreviousProgress = 0;
                TotalProgress = 0;
            }
            else
            {
                //PreviousProgress = 100 * (double)this._previousInspectionCount / this._quota;
                TotalProgress = (100 * (double)this._totalInspectionCount / this._quota)<=100 ? 
                                            100 * (double)this._totalInspectionCount / this._quota : 100;
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
