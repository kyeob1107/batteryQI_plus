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
using batteryQI.ViewModels.Bases;

namespace batteryQI.ViewModels
{
    // 이미지 검사 이벤트
    internal partial class InspectViewModel : ViewModelBases
    {
        
        // --------------------------------------------
        private void getManafactureNameID() // DB에서 제조사 리스트 가져오기
        {
            // DB에서 가져와서 리스트 초기화하기, ID는 안 가져오고 Name만 추가
            List<Dictionary<string, object>> ManufactureList_Raw = _dblink.Select("SELECT * FROM manufacture;"); // 데이터 가져오기
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

        private string GetNextAutoIncrementId()
        {
            try
            {
                // 다음 AUTO_INCREMENT 값 가져오기
                string query = @"
                                SELECT AUTO_INCREMENT
                                       FROM INFORMATION_SCHEMA.TABLES
                                 WHERE TABLE_SCHEMA = 'batteryQI' 
                                       AND TABLE_NAME = 'batteryInfo';
                                 ";

                // 데이터베이스 연결 및 쿼리 실행
                var result = _dblink.Select(query);

                if (result.Count > 0)
                {
                    // AUTO_INCREMENT 값을 문자열로 반환
                    return result[0]["AUTO_INCREMENT"].ToString();
                }
                else
                {
                    return "알 수 없음"; // 데이터가 없는 경우
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 처리
                System.Windows.MessageBox.Show($"Battery ID 가져오기 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return "오류";
            }
        }
    }
}
