using System;
using System.Collections.Generic;
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
        public Manager Manager
        {
            get => _manager;
            set => SetProperty(ref _manager, value);
        }
        public LoginViewModelBases()
        {
            // Model 객체 초기화
            _manager = new Manager();
            // 로그인 창 열면서 DB 연결
            DBlink DBConnection = new DBlink();
            DBConnection.Connect();
        }
        [RelayCommand]
        private void Login(object obj)
        {
            PasswordBox pw = obj as PasswordBox;

            if (Manager.ManagerID == "manager1" && pw.Password == "1234")
            {
                MessageBox.Show("로그인 완료", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
