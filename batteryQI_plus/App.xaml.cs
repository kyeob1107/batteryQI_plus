using System.Configuration;
using System.Data;
using System.Windows;
using batteryQI_plus.Models;
using batteryQI_plus.Views;

namespace batteryQI_plus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    MainWindow main = new();
        //    main.ShowDialog();
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // ViewModelLocator의 Cleanup 호출
            var locator = (batteryQI_plus.ViewModels.ViewModelLocator)Resources["Locator"];
            locator.Cleanup();

            base.OnExit(e);
        }
    }

}
