using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using batteryQI.ViewModels;

namespace batteryQI.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.DataContext = new loginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;


            //if (username == "manager1" && password == "1234")
            //{
            //    MessageBox.Show("로그인 완료", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            //    var mainWindow = new MainWindow();
            //    mainWindow.Show();

            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("아이디 및 비밀번호를 확인해 주세요", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            MessageBox.Show("로그인 완료", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();


        }
    }
}
