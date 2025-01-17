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
            PasswordBox pw = obj as PasswordBox;

            if (DBConnection.ConnectOk())
            {
                string sql = "";
                DBConnection.Select(sql);
                MessageBox.Show("Hello World");
                // MainWindow로 Navigation 처리
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // 현재 창 닫기
                Application.Current.Windows[0]?.Close();  // 첫 번째 창 닫기 (로그인 창)
            }
            else
            {
                MessageBox.Show("DB 연결 오류! 다시 시도해주세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
