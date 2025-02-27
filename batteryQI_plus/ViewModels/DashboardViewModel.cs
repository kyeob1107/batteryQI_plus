using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI_plus.ViewModels.Bases;
using batteryQI_plus.Models;
using System.Windows;
using System.Windows.Shapes;

namespace batteryQI_plus.ViewModels
{
    public class DashboardViewModel : ViewModelBases
    {
        // 접속 직원 정보용
        private Employee _employee = Employee.Instance();
        public Employee Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
        
        // 단위시간 검사 모니터링용
        int num; // 쿼리보내서 line 몇개 있는지 count 수
        private List<DashboardModel> _unitTest = new List<DashboardModel>();
        private List<string> _logContent = new List<string> ();
        
        // 단위시간 검사 합계용
        private List<DashboardModel> _totalUnitTest = new List<DashboardModel>();
        private List<string> _totalLogContent = new List<string>();

        public List<DashboardModel> UnitTest 
        {  
            get { return _unitTest; } 
            set { SetProperty(ref _unitTest, value); }
        }

        public List<string> LogContent
        {
            get { return _logContent; }
            set { SetProperty(ref _logContent, value); }
        }

        private void UpdateLogContent()
        {
            for (int line = 1; line < num; line++)
            {

                this.LogContent[0] += $"<Line{line} | "
                                    + $"{_unitTest[line].StartDatetime} ~ {_unitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_unitTest[line].InspectionCount}개 | 정상: {_unitTest[line].NormalCount}개 | "
                                    + $"불량: {_unitTest[line].DefectCount}개 (불량률: {_unitTest[line].DefectRate}%) "
                                    + "\r\n";

                this.LogContent[line] += $"<Line{line} | "
                                    + $"{_unitTest[line].StartDatetime} ~ {_unitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_unitTest[line].InspectionCount}개 | 정상: {_unitTest[line].NormalCount}개 | "
                                    + $"불량: {_unitTest[line].DefectCount}개 (불량률: {_unitTest[line].DefectRate}%) "
                                    + "\r\n";
            }
        }

        public List<DashboardModel> TotalUnitTest
        {
            get { return _totalUnitTest; }
            set { SetProperty(ref _totalUnitTest, value); }
        }

        public List<string> TotalLogContent
        {
            get { return _totalLogContent; }
            set { SetProperty(ref _totalLogContent, value); }
        }

        private void TotalUpdateLogContent()
        {
            // 전체탭 부분 지우기
            this.TotalLogContent[0] = "";

            for (int line = 1; line < num; line++)
            {

                this.TotalLogContent[0] += $"<Line{line} | "
                                    + $"{_totalUnitTest[line].StartDatetime} ~ {_totalUnitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_totalUnitTest[line].InspectionCount}개 | 정상: {_totalUnitTest[line].NormalCount}개 | "
                                    + $"불량: {_totalUnitTest[line].DefectCount}개 (불량률: {_totalUnitTest[line].DefectRate}%) "
                                    + "\r\n";

                this.TotalLogContent[line] = $"<Line{line} | "
                                    + $"{_totalUnitTest[line].StartDatetime} ~ {_totalUnitTest[line].EndDatetime}>"
                                    + "\r\n"
                                    + $"검사수: {_totalUnitTest[line].InspectionCount}개 | 정상: {_totalUnitTest[line].NormalCount}개 | "
                                    + $"불량: {_totalUnitTest[line].DefectCount}개 (불량률: {_totalUnitTest[line].DefectRate}%) "
                                    + "\r\n";
            }
        }


        public DashboardViewModel() 
        {
            // 로그아웃동안 검사결과에 대해 값 가져오기
            // 라인 몇개 있는지 확인해서 그것 수대로 model 생성
            var result = _dblink.Select($"SELECT COUNT(DISTINCT lineId) AS Count FROM batteryQIPlus.productionLines;");
            num = Convert.ToInt32(result[0]["Count"]);
            //MessageBox.Show(num.ToString());
            //MessageBox.Show(_unitTest.Count + "\r\n" + LogContent.Count);
            
            _unitTest.Add(new DashboardModel());
            _logContent.Add("");
            _totalUnitTest.Add(new DashboardModel());
            _totalLogContent.Add("");

            for (int line = 1; line < num; line++)
            {
                _unitTest.Add(new DashboardModel(_dblink, _employee, line));
                _logContent.Add("");
                _totalUnitTest.Add(new DashboardModel(_dblink, _employee, line, "total"));
                _totalLogContent.Add("");
            }
            UpdateLogContent();
            TotalUpdateLogContent();
        }
        
        
    }
}
