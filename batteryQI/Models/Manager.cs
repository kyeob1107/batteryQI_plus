using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    // 싱글톤 패턴
    internal class Manager : ObservableObject
    {
        private int _managerNum;
        private string _managerID; // 담당자 아이디
        private string _managerPW; // 담당자 비번
        private int _workAmount; // 담당자 할당량
        private string _totalInspectNum; // 오늘 수행량 저장 변수
        private double _workProgress; // 검사 완료 비율
        static Manager manager; // singleton

        private Manager() { } // 생성자 접근 제어 변경
        public static Manager Instance()
        {
            if(manager == null)
            {
                manager = new Manager(); // Manager 객체 생성
            }
            return manager;
        }
        public string ManagerID
        {
            get { return _managerID; }
            set
            {
                SetProperty(ref _managerID, value);
            }
        }
        public int ManagerNum
        {
            get { return _managerNum; }
            set
            {
                SetProperty(ref _managerNum, value);
            }
        }
        public string ManagerPW
        {
            get { return _managerPW; }
            set
            {
                SetProperty(ref _managerPW, value);
            }
        }
        public int WorkAmount
        {
            get { return _workAmount; }
            set
            {
                SetProperty(ref _workAmount, value);
                UpdateWorkProgress(); // 할당량 변경 시 진행률 업데이트
            }
        }

        public string TotalInspectNum
        {
            get { return _totalInspectNum; }
            set
            {
                SetProperty(ref _totalInspectNum, value);
                UpdateWorkProgress(); // 검사 완료량 변경 시 진행률 업데이트
            }
        }

        public double WorkProgress
        {
            get => _workProgress;
            private set
            {
                _workProgress = value;
                OnPropertyChanged(nameof(WorkProgress));
            }
        }

        private void UpdateWorkProgress()
        {
            try
            {
                // TotalInspectNum을 정수로 변환
                int totalInspectNum = int.TryParse(TotalInspectNum, out var parsedValue) ? parsedValue : 0;

                if (WorkAmount > 0)
                {
                    WorkProgress = Math.Round((double)totalInspectNum / WorkAmount * 100, 2);
                }
                else
                {
                    WorkProgress = 0; // 할당량이 0일 경우 진행률은 0
                }
                if (WorkProgress > 100) WorkProgress = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"작업 진행률 계산 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
