using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using batteryQI.ViewModels.Bases;
using batteryQI.Views;
using CommunityToolkit.Mvvm.Input;

namespace batteryQI.ViewModels
{
    internal partial class InspectViewModel : ViewModelBases
    {
        // InspectViewModel 이벤트 핸들러 정리 파일
        [RelayCommand]
        private void ImageSelectButton_Click()
        {
            // 이미지 파일 선택 함수
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png;";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _battery.ImagePath = openFileDialog.FileName; // 배터리 객체에 이미지 경로 저장
            }
            // 배터리 객체 자체는 하나의 배터리 객체로 계속 재활용
        }

        // 이미지 검사 이벤트 핸들러
        [RelayCommand]
        private void ImageInspectionButton_Click()
        {
            // 콤보박스에서 선택한 값들을 저장함.
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
        private void confirmErrorReasonSelectButton_Click(System.Windows.Window window)
        {
            // 선택한 값이 "불량 유형을 선택하세요"이거나 null일 경우 예외 처리
            if (string.IsNullOrEmpty(battery.DefectName) || battery.DefectName == "불량 유형을 선택하세요")
            {
                System.Windows.MessageBox.Show(
                    "불량 유형을 선택해주세요.",
                    "입력 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return; // 동작 중단
            }


            // 세 번째 페이지로 이동
            var errorInfoView = new ErrorInfoView();
            errorInfoView.Show();

            // 현재 창 닫기
            window?.Close();
        }
        [RelayCommand]
        private void confirmErrorInfoCheckButton_Click(System.Windows.Window window)
        {
            // DB 정보 인서트

            if (_dblink.ConnectOk())
            {
                int defectState = -1;
                if (battery.DefectStat == "정상")
                    defectState = 1;
                else
                    defectState = 0;

                if (_dblink.Insert($"INSERT INTO batteryInfo (batteryId, shootDate, usageName, batteryType, manufacId, batteryShape, shootPlace, imagePath, managerNum, defectStat, defectName)" +
                    $"VALUES(0, '{_battery.ShootDate}', '{_battery.Usage}', '{_battery.BatteryType}', {ManufacDict[battery.ManufacName]}, '{_battery.BatteryShape}', 'CodingOn', NULL, {_manager.ManagerNum}, {defectState}, '{_battery.DefectName}');"))
                {
                    System.Windows.MessageBox.Show("완료!");
                    // 데이터 초기화
                    _battery.Usage = "";
                    _battery.BatteryType = "";
                    _battery.ManufacName = "";
                    _battery.BatteryShape = "";
                    _battery.DefectName = "";
                    _battery.ImagePath = "";
                    _battery.BatteryBitmapImage = null; // bitmap 이미지 초기화
                }
                else
                    System.Windows.MessageBox.Show("실패");
            }
            window?.Close(); // 데이터 info 창 닫기
        }
    }
}
