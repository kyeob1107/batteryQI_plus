using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Data.Common;
using batteryQI.Models;
using System.Windows.Forms;
using Mysqlx.Crud;
using batteryQI.ViewModels.Bases;
using System.Windows;
using System.Data;

namespace batteryQI.ViewModels
{
    // 관리자 페이지
    internal partial class ManagerViewModel : ViewModelBases
    {
        private Manager _manager = Manager.Instance();
        private string _manufacName = "";
        private IDictionary<string, string> _manufacDict = new Dictionary<string, string>();
        private int _newAmount;
        public IDictionary<string, string> ManufacDict
        {
            get => _manufacDict;
        }
        public string ManufacName
        {
            get => _manufacName;
            set => SetProperty(ref _manufacName, value);
        }
        DBlink DBConnection;
        public Manager Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public int NewAmount
        {
            get => _newAmount;
            set => SetProperty(ref _newAmount, value);
        }
        public ManagerViewModel()
        {
            DBConnection = DBlink.Instance(); // DB객체 연결
            _manager = Manager.Instance();

            _manager.TotalInspectNum = completeAmount();

            getManafactureNameID();
            _newAmount = _manager.WorkAmount;
        }


        private void getManafactureNameID() // DB에서 제조사 리스트 가져오기
        {
            // DB에서 가져와서 리스트 초기화하기, ID는 안 가져오고 Name만 추가
            List<Dictionary<string, object>> ManufactureList_Raw = _dblink.Select("SELECT * FROM manufacture;"); // 데이터 가져오기
            for (int i = 0; i < ManufactureList_Raw.Count; i++)
            {
                string Name = "";
                string ID = "";
                foreach (KeyValuePair<string, object> items in ManufactureList_Raw[i])
                {
                    // 제조사 이름 key, 제조사 id value
                    if (items.Key == "manufacName")
                    {
                        Name = items.Value.ToString();
                    }
                    else if (items.Key == "manufacId")
                    {
                        ID = items.Value.ToString();
                    }
                }
                _manufacDict.Add(Name, ID);
            }
        }

        [RelayCommand]
        private void ManufactInsert()
        {
            // 제조사 인풋
            try
            {
                if (_dblink.ConnectOk())
                {
                    if(ManufacName != "")
                    {
                        _dblink.Insert($"INSERT INTO manufacture (manufacId, manufacName) VALUES(0, '{ManufacName}');");
                        _manufacDict.Clear();
                        getManafactureNameID();
                        System.Windows.MessageBox.Show("완료");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("제조사를 입력해주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("입력 오류", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private void amountSaveButton_Click()
        {
            // 월 검사 할당량 수정 이벤트
            if (_dblink.ConnectOk())
            {
                if (System.Windows.MessageBox.Show($"할당량을 {_newAmount}로 변경할까요?", "warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _manager.WorkAmount = _newAmount;
                    _dblink.Update($"UPDATE manager SET workAmount={_manager.WorkAmount} WHERE managerId='{_manager.ManagerID}';");
                    System.Windows.MessageBox.Show($"할당량을 {_manager.WorkAmount}로 수정 완료!");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("DB 연결 오류", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string completeAmount()
        {
            try
            {
                // 분석 완료 개수 가져오기
                string query = @$"
                        SELECT COUNT(*) 
                        FROM batteryInfo
                        WHERE DATE_FORMAT(shootDate, '%Y-%m') = DATE_FORMAT(NOW(), '%Y-%m')
                        AND ManagerNum = {_manager.ManagerNum};
                        ";

                // 데이터베이스 연결 및 쿼리 실행
                var result = DBConnection.Select(query);

                // 데이터가 있는 경우
                if (result != null && result.Count > 0)
                {
                    // 첫 번째 결과를 문자열로 변환
                    return result[0]["COUNT(*)"]?.ToString() ?? "0";
                }
                else
                {
                    return "0"; // 데이터가 없는 경우
                }
            }
            catch (Exception ex)
            {
                // 오류 발생 시 처리
                System.Windows.MessageBox.Show($"작업량 데이터 가져오기 실패", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "Error";
            }
        }
    }
}
