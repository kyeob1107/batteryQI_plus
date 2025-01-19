using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batteryQI.Views;
using System.Windows.Media.Imaging;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Forms;
using batteryQI.Models;

namespace batteryQI.ViewModels
{
    // 이미지 검사 이벤트
    internal partial class InspectViewModel : ObservableObject
    {
        // combox 리스트
        private IList<string> _manufacList = new List<string>(); // 제조사명 받아오기
        private Dictionary<string, string> ManufacDict = new Dictionary<string, string>(); // viewmodel에서만 사용하는 딕셔너리 가져오기
        private IList<string>? _batteryTypeList = new List<string>() {"Cell", "Module", "Pack" };
        private IList<string>? _batteryShapeList = new List<string>() { "Pouch", "Cylinder" };
        private IList<string>? _usageList = new List<string>() { "Household", "Industrial" }; // 사용처 리스트업
        private Battery _battery;
        private DBlink DBConnection;

        public IList<string>? ManufacList
        {
            get => _manufacList;
        }
        public IList<string>? BatteryTypeList
        {
            get => _batteryTypeList;
        }
        public IList<string>? BatteryShapeList
        {
            get => _batteryShapeList;
        }
        public IList<string>? UsageList
        {
            get => _usageList;
        }
        // ----------------
        public Battery battery
        {
            get => _battery;
            set => SetProperty(ref _battery, value);
        }
        public InspectViewModel()
        {
            // Manager 객체 생성
            _battery = Battery.Instance();
            // 대시보드 열며 DB 연결
            DBConnection = DBlink.Instance();
            DBConnection.Connect();

            getManafactureNameID();
        }

        // --------------------------------------------
        private void getManafactureNameID() // DB에서 제조사 리스트 가져오기
        {
            // DB에서 가져와서 리스트 초기화하기, ID는 안 가져오고 Name만 추가
            List<Dictionary<string, object>> ManufactureList_Raw = DBConnection.Select("SELECT * FROM manufacture;"); // 데이터 가져오기
            for(int i = 0; i < ManufactureList_Raw.Count; i++)
            {
                string Name = "";
                string ID = "";
                 foreach(KeyValuePair<string, object> items in ManufactureList_Raw[i])
                {
                    // 제조사 이름 key, 제조사 id value
                    //Name = items.
                    if(items.Key == "manufacName")
                    {
                        Name = items.Value.ToString();
                    }
                    else if(items.Key == "manufacId")
                    {
                        ID = items.Value.ToString(); 
                    }
                }
                _manufacList.Add(Name);
                ManufacDict.Add(Name, ID);
            }
        }

        // --------------------------------------------
        // 이벤트 핸들러
        [RelayCommand]
        private void ImageSelectButton_Click()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _battery.ImagePath = openFileDialog.FileName; // 배터리 객체에 이미지 경로 저장
            }
            // 배터리 객체 자체는 하나의 배터리 객체로 계속 재활용
        }

        // 이미지 검사 이벤트 핸들러
        [RelayCommand]
        private void ImageInspectionButton_Click()
        {
            // 이미지 정보, 제조사 아이디
            if(battery.ImagePath != "" && battery.ManufacName != "" && battery.BatteryShape != "" && battery.BatteryType != "" && battery.Usage != "")
            {
                // 이미지 검사 함수로 대체 예정
                //battery.BatteryBitmapImage = new BitmapImage(new Uri(_battery.ImagePath)); // 이미지를 bitmap으로 변환
                //imgProcessing
                battery.imgProcessing(); 
                // 정상 불량 판단 페이지로 넘어가게
                var inspectionImage = new InspectionImage();
                inspectionImage.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("모든 정보를 기입해주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // -------------------------------------- Inspection 결과 화면 이벤트 처리
        [RelayCommand]
        private void NomalButton_Click()
        {
            // DefectState는 정상인걸로
            battery.DefectStat = "정상";
            System.Windows.Application.Current.Windows[0]?.Close();
        }
        [RelayCommand]
        private void ErrorButton_Click()
        {
            //inspectionSection.Visibility = Visibility.Collapsed;

            //var errorReasonControl = new UserControls.ErrorReason();
            //errorReasonControl.ErrorConfirmed += OnErrorReasonConfirmed;

            //InspectionFrame.Content = errorReasonControl;
        }
    }
}
