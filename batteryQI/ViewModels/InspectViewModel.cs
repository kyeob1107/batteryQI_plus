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
using batteryQI.Views.UserControls;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        private IList<string>? _defectList = new List<string>() { "Damage", "Pollution", "Damage and Pollution", "Etc.." };
        private Battery _battery;
        private Manager _manager;
        private DBlink DBConnection;

        private Visibility _errorInspectionVisibility = Visibility.Visible; // 첫 번째 UserControl (ErrorInspection) Visibility 제어
        private Visibility _errorReasonVisibility = Visibility.Collapsed; // 두 번째 UserControl (ErrorReason) Visibility 제어

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
        public IList<string>? DefectList
        {
            get => _defectList;
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
            _manager = Manager.Instance();
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

        // 첫 번째 UserControl (ErrorInspection) Visibility 제어
        public Visibility ErrorInspectionVisibility
        {
            get => _errorInspectionVisibility;
            set => SetProperty(ref _errorInspectionVisibility, value);
        }

        // 두 번째 UserControl (ErrorReason) Visibility 제어
        public Visibility ErrorReasonVisibility
        {
            get => _errorReasonVisibility;
            set => SetProperty(ref _errorReasonVisibility, value);
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
            List<string> emptyFields = new List<string>();

            if (string.IsNullOrEmpty(battery.ImagePath)) emptyFields.Add("이미지");
            if (string.IsNullOrEmpty(battery.ManufacName)) emptyFields.Add("제조사명");
            if (string.IsNullOrEmpty(battery.BatteryShape)) emptyFields.Add("배터리 형태");
            if (string.IsNullOrEmpty(battery.BatteryType)) emptyFields.Add("배터리 타입");
            if (string.IsNullOrEmpty(battery.Usage)) emptyFields.Add("사용 용도");

            // 이미지 정보, 제조사 아이디
            if (battery.ImagePath != "" && battery.ManufacName != "" && battery.BatteryShape != "" && battery.BatteryType != "" && battery.Usage != "")
            {
                // 이미지 검사 함수로 대체 예정
                battery.imgProcessing(); 
                // 정상 불량 판단 페이지로 넘어가게
                var inspectionImage = new InspectionImage();
                inspectionImage.ShowDialog();
            }
            else
            {
                string emptyFieldsMessage = string.Join(", ", emptyFields);
                System.Windows.MessageBox.Show(
                    $"다음 정보를 기입해주세요: {emptyFieldsMessage}",
                    "입력 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        // -------------------------------------- Inspection 결과 화면 이벤트 처리
        [RelayCommand]
        private void NomalButton_Click(System.Windows.Window window)
        {
            // DefectState는 정상인걸로
            battery.DefectStat = "정상";
            battery.DefectName = "Normal";
            var errorInfoView = new ErrorInfoView();
            errorInfoView.Show();

            window?.Close(); // 현재 창 닫기
        }
        [RelayCommand]
        private void ErrorButton_Click()
        {
            battery.DefectStat = "불량";
            // 버튼 영역(정상/불량 버튼 숨기기)
            ErrorInspectionVisibility = Visibility.Collapsed;
            // Frame 영역 보이기
            ErrorReasonVisibility = Visibility.Visible;
        }

        // ------------------------
        // ErrorInfo.xaml 이벤트 핸들링 (데이터 가용성을 위해서 여기서 코딩함..)

        [RelayCommand]
        private void ConfirmErrorReasonButton_Click(System.Windows.Window window)
        {
            //// 선택된 불량 유형을 배터리 구조체에 반영 구현 중

            // 세 번째 페이지로 이동
            var errorInfoView = new ErrorInfoView();
            errorInfoView.Show();

            // 현재 창 닫기
            window?.Close();
        }
        [RelayCommand]
        private void confirmErrorInfoButton_Click(System.Windows.Window window)
        {
            // DB 정보 인서트

            if (DBConnection.ConnectOk())
            {
                int defectState = -1;
                if (battery.DefectStat == "정상")
                    defectState = 1;
                else
                    defectState = 0;

                //DBConnection.Insert($"INSERT INTO manufacture (manufacId, manufacName) VALUES(0, '{ManufacName}');");

                if (DBConnection.Insert($"INSERT INTO batteryInfo (batteryId, shootDate, usageName, batteryType, manufacId, batteryShape, shootPlace, imagePath, managerNum, defectStat, defectName)" +
                    $"VALUES(0, '{battery.ShootDate}', '{battery.Usage}', '{battery.BatteryType}', {ManufacDict[battery.ManufacName]}, '{battery.BatteryShape}', 'CodingOn', NULL, {_manager.ManagerNum}, {defectState}, '{battery.DefectName}');"))
                    System.Windows.MessageBox.Show("완료!");
                else
                    System.Windows.MessageBox.Show("실패");
            }
            //System.Windows.Application.Current.Windows[0]?.Close();
            window?.Close();
        }
    }
}
