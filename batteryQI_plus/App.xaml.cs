using System.Configuration;
using System.Data;
using System.Windows;
using batteryQI.Views;

namespace batteryQI
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
            var locator = (batteryQI.ViewModels.ViewModelLocator)Resources["Locator"];
            locator.Cleanup();

            base.OnExit(e);
        }
    }

}
