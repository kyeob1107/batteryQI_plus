using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using batteryQI.Models;
using batteryQI.ViewModels.Bases;
using CommunityToolkit.Mvvm.Input;
using batteryQI.UserControls;
using batteryQI.Views;

namespace batteryQI.ViewModels
{
    internal partial class MainWindowViewModel : ViewModelBases
    {
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public MainWindowViewModel()
        {
            // 초기 화면 설정
            CurrentPage = new DashboardView();    
        }
        
        [RelayCommand]
        private void HomeButton()
        {
            CurrentPage = new DashboardView();
        }
        
        [RelayCommand]
        private void ChartButton()
        {
            CurrentPage = new ChartView();
        }
        
        [RelayCommand]
        private void ManagerButton()
        {
            CurrentPage = new ManagerView();
        }

        public Action CloseAction { get; set; }

        [RelayCommand]
        private void ExitButton()
        {
            CloseAction?.Invoke();
        }


    }
}
