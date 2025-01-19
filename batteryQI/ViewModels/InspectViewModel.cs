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
                //_battery.BatteryBitmapImage = new BitmapImage(new Uri(_battery.ImagePath)); // 이미지를 bitmap으로 변환
            }
            // 배터리 객체 자체는 하나의 배터리 객체로 계속 재활용
        }

        // 이미지 검사 이벤트 핸들러
        private void ImageInspectionButton_Click()
        {
            //if (PreviewImage.Source == null)
            //{
            //    MessageBox.Show("배터리 이미지가 업로드되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}
            //else if (manufacture.SelectedIndex == 0 || batteryType.SelectedIndex == 0 || batteryShape.SelectedIndex == 0 || usage.SelectedIndex == 0)
            //{
            //    MessageBox.Show("배터리 이미지 정보를 모두 기입해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}
            //else
            //{
            //    InspectionImage();
            //}
        }

        private void InspectionImage()
        {
            //var inspectionImage = new InspectionImage();

            //if (PreviewImage.Source is BitmapImage previewImageSource)
            //{
            //    //inspectionImage.SetImage(previewImageSource);
            //}

            //inspectionImage.ShowDialog();
        }

        public void SetImage(BitmapImage image)
        {
            //AnalyzeImage.Source = image;
        }

        // -------------------------------------- Inspection 결과 화면 이벤트 처리
        [RelayCommand]
        private void NomalButton_Click()
        {
            // DefectState는 정상인걸로
            //Application.Current.Windows[0]?.Close(); // 왜 현재창을 닫지?
            //MessageBox.Show("정상"); // 코드 정상 실행
        }
        [RelayCommand]
        private void ErrorButton_Click()
        {
            //inspectionSection.Visibility = Visibility.Collapsed;

            //var errorReasonControl = new UserControls.ErrorReason();
            //errorReasonControl.ErrorConfirmed += OnErrorReasonConfirmed;

            //InspectionFrame.Content = errorReasonControl;
        }
        private void OnErrorReasonConfirmed(string selectedReason)
        {
            //if (AnalyzeImage.Source is BitmapImage analyzeImage)
            //{
            //    var errorInfoView = new ErrorInfoView();
            //    errorInfoView.SetErrorInfo(analyzeImage);
            //    errorInfoView.ShowDialog();
            //}
            //else
            //{
            //    MessageBox.Show("이미지를 로드할 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
