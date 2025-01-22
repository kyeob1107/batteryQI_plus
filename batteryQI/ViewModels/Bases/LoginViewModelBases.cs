using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using batteryQI.Models;
using batteryQI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace batteryQI.ViewModels.Bases
{
    internal partial class LoginViewModelBases : ObservableObject
    {
        private Manager _manager;
        private DBlink DBConnection;
        public Manager Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public LoginViewModelBases()
        {
            // Manager 객체 생성
            _manager = Manager.Instance();
            // 로그인 창 열면서 DB 연결
            DBConnection = DBlink.Instance();
            DBConnection.Connect();
        }

        [RelayCommand]

        private void Login(object obj)
        {
            // DB가 제대로 연결되어 있고 PassBox가 안 비어져 있으면 수행
            if (DBConnection.ConnectOk() && obj is PasswordBox pw)
            {
                List<Dictionary<string, object>> login = DBConnection.Select($"SELECT * FROM manager WHERE managerId='{Manager.ManagerID}';");
                if (login.Count != 0 && (pw.Password == login[0]["managerPw"].ToString()))
                {
                    MessageBox.Show("로그인 완료", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _manager.ManagerNum = (int)login[0]["managerNum"];
                    _manager.ManagerID = login[0]["managerId"].ToString();
                    _manager.WorkAmount = (int)login[0]["managerWorkAmount"];

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // 현재 창 닫기
                    Application.Current.Windows[0]?.Close();
                }
                else
                {
                    MessageBox.Show("아이디 및 비밀번호를 확인해 주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
