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

namespace batteryQI.ViewModels
{
    // 관리자 페이지
    internal partial class ManagerViewModel : ViewModelBases
    {
        private Manager _manager = Manager.Instance();
        private string _manufacName = "";
        private IDictionary<string, string> _manufacDict = new Dictionary<string, string>();
        public IDictionary<string, string> ManufacDict
        {
            get => _manufacDict;
        }
        public string ManufacName
        {
            get => _manufacName;
            set => SetProperty(ref _manufacName, value);
        }
        public Manager Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public ManagerViewModel()
        {
            getManafactureNameID();
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
                        MessageBox.Show("완료");
                    }
                    else
                    {
                        MessageBox.Show("제조사를 입력해주세요", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("입력 오류", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        [RelayCommand]
        private void SaveButton_Click()
        {
            // 월 검사 할당량 수정 이벤트
            if (_dblink.ConnectOk())
            {
                _dblink.Update($"UPDATE manager SET workAmount={_manager.WorkAmount} WHERE managerId='{_manager.ManagerID}';");
                MessageBox.Show("수정 완료!");
            }
            else
            {
                MessageBox.Show("DB 연결 오류", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
