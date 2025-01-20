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

namespace batteryQI.ViewModels
{
    // 관리자 페이지
    internal partial class ManagerViewModel : ObservableObject
    {
        private string _manufacName;
        public string ManufacName
        {
            get => _manufacName;
            set => SetProperty(ref _manufacName, value);
        }
        DBlink DBConnection;
        Manager _manager;
        public Manager Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public ManagerViewModel()
        {
            DBConnection = DBlink.Instance(); // DB객체 연결
            _manager = Manager.Instance();
        }
        [RelayCommand]
        private void ManufactInsert()
        {
            // 제조사 인풋
            try
            {
                if (DBConnection.ConnectOk())
                {
                    DBConnection.Insert($"INSERT INTO manufacture (manufacId, manufacName) VALUES(0, '{ManufacName}');");
                    MessageBox.Show("완료");
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
            if (DBConnection.ConnectOk())
            {
                DBConnection.Update($"UPDATE manager SET workAmount={_manager.WorkAmount} WHERE managerId='{_manager.ManagerID}';");
                MessageBox.Show("수정 완료!");
            }
            else
            {
                MessageBox.Show("DB 연결 오류", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
