using batteryQI_plus.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using batteryQI_plus.Views.UserControls;

namespace batteryQI_plus.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBases
    {
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }
        public Action? CloseAction { get; set; }

        public MainWindowViewModel()
        {
            // 초기 화면 설정
            _currentPage = new DashboardView();
        }
        
        [RelayCommand]
        private void HomeButton()
        {
            CurrentPage = new DashboardView();
        }
        
        [RelayCommand]
        private void ChartButton()
        {
            CurrentPage = new AnalysisView();
        }
        
        [RelayCommand]
        private void ManagerButton()
        {
            CurrentPage = new SettingView();
        }

        [RelayCommand]
        private void ExitButton()
        {
            _dblink.Dispose(); // DB 연결 끊기
            CloseAction?.Invoke();
        }
    }
}
