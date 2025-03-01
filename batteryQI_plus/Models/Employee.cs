using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI_plus.Models
{
    // 싱글톤 패턴
    public class Employee : ObservableObject
    {
        private int _employeeNum;
        private string _employeeID; // 담당자 아이디
        private string _employeePW; // 담당자 비번
        //private int _workAmount; // 담당자 할당량
        //private string _totalInspectNum; // 오늘 수행량 저장 변수
        //private double _workProgress; // 검사 완료 비율
        private int _employeeRole; // 권한 정보
        private int _lineId; // 할당된 생산 라인
        private DateTime _lastLogoutDateTime; // 마지막 로그아웃한 시간



        static Employee employee; // singleton

        private Employee() { } // 생성자 접근 제어 변경
        public static Employee Instance()
        {
            if (employee == null)
            {
                employee = new Employee(); // Manager 객체 생성
            }
            return employee;
        }
        public string EmployeeID
        {
            get { return _employeeID; }
            set
            {
                SetProperty(ref _employeeID, value);
            }
        }
        public int EmployeeNum
        {
            get { return _employeeNum; }
            set
            {
                SetProperty(ref _employeeNum, value);
            }
        }
        public string EmployeePW
        {
            get { return _employeePW; }
            set
            {
                SetProperty(ref _employeePW, value);
            }
        }
        //public int WorkAmount
        //{
        //    get { return _workAmount; }
        //    set
        //    {
        //        SetProperty(ref _workAmount, value);
        //        UpdateWorkProgress(); // 할당량 변경 시 진행률 업데이트
        //    }
        //}

        //public string TotalInspectNum
        //{
        //    get { return _totalInspectNum; }
        //    set
        //    {
        //        SetProperty(ref _totalInspectNum, value);
        //        UpdateWorkProgress(); // 검사 완료량 변경 시 진행률 업데이트
        //    }
        //}

        //public double WorkProgress
        //{
        //    get => _workProgress;
        //    private set
        //    {
        //        _workProgress = value;
        //        OnPropertyChanged(nameof(WorkProgress));
        //    }
        //}

        //private void UpdateWorkProgress()
        //{
        //    try
        //    {
        //        // TotalInspectNum을 정수로 변환
        //        int totalInspectNum = int.TryParse(TotalInspectNum, out var parsedValue) ? parsedValue : 0;

        //        if (WorkAmount > 0)
        //        {
        //            WorkProgress = Math.Round((double)totalInspectNum / WorkAmount * 100, 2);
        //        }
        //        else
        //        {
        //            WorkProgress = 0; // 할당량이 0일 경우 진행률은 0
        //        }
        //        if (WorkProgress > 100) WorkProgress = 100;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"작업 진행률 계산 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        public int EmployeeRole
        {
            get { return _employeeRole; }
            set
            {
                SetProperty(ref _employeeRole, value);
            }
        }

        public int LineId
        {
            get { return _lineId; }
            set
            {
                SetProperty(ref _lineId, value);
            }
        }

        public DateTime LastLogoutDateTime
        {
            get { return _lastLogoutDateTime; }
            set 
            { 
                SetProperty(ref _lastLogoutDateTime, value); 
            }
        }
    }
}
