using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace batteryQI.Models
{
    internal class Battery : ObservableObject
    {
        // 배터리 가용 변수 선언(변동사항 있음. Id나 date 데이터 등은 DB에 기입할 때 필요하지만 관리자가 건들이는 부분은 없음
        //private int _batteryId; // 자동 기입 -> DB에서 기입될 뿐 코드에선 사용 x
        //private DateTime _shootDate; // 자동 기입, 굳이 private으로 선언할 필요 x
        private string _usage; // 사용처, 직접 기입
        private string _batteryType; // 직접 기입
        private int _manufacId; // 직접 기입(제조사 id를 가져옴)
        private string _batteryShape; // 직접 기입
        private string _shootPlace = "line1"; // 자동 기입(일단은)
        private string? _imagePath; // 파일 로드 경로 저장 -> 이미지 로드
        //private int _managerNum; // 자동 기입(현재 로그인 되어 있는 관리자로)
        private bool _defectStat; // 직접 기입(불량, 정상 선택으로)
        private string _defectName; // 직접 기입
        // -------------
        // 배터리 객체도 싱글톤으로 수행
        // 사유: 여러 개의 배터리 객체를 관리하는 것이 아닌 순차적으로 배터리 객체를 활용하기 때문
        // (현재 검사 결과를 수행하지 않으면 다음 검사 불가, 이전 검사 결과는 휘발됨), 메모리 활용성을 위해서 재사용 하는 것으로 로직 변경
        static Battery staticBattery;
        public static Battery Instance()
        {
            if (staticBattery == null)
                staticBattery = new Battery();
            return staticBattery;
        }
        // ----------------
        // ViewModel에서 사용할 변수 선언, imagePath 필요..(이미지 변수에 이미지를 넣을 때, 파일 경로로 로드)
        public string ShootDate
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); }
        }
        public string Usage
        {
            get { return _usage; }
            set {SetProperty(ref _usage, value); }
        }
        public string BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        public int ManufacId
        {
            get { return _manufacId; }
            set { SetProperty(ref _manufacId, value); }
        }
        public string ShootPlace
        {
            get { return _shootPlace; }
            set { SetProperty(ref _shootPlace, value); }
        }
        public string BatteryShape
        {
            get { return _batteryShape; }
            set { SetProperty(ref _batteryShape, value); }
        }
        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty(ref _imagePath, value); }
        }
        public bool DefectStat
        {
            get { return _defectStat; }
            set { SetProperty(ref _defectStat, value); }
        }
        public string DefectName
        {
            get { return _defectName; }
            set { SetProperty(ref _defectName, value); }
        }
        public BitmapImage BatteryBitmapImage;

        // --------------------------
        // 멤버 메소드
        public void batteryInput()
        {
            //DB에 insert관련?
        }
        public void imgProcessing()
        {
            //AI를 통해서 처리하는 부분
        }
    }
}
