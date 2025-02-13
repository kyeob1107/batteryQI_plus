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
using batteryQI.Views.UserControls;
using batteryQI.Views;
using System.Data.Common;

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
            CurrentPage = new ChartView();
        }
        
        [RelayCommand]
        private void ManagerButton()
        {
            CurrentPage = new ManagerView();
        }

        [RelayCommand]
        private void ExitButton()
        {
            _dblink.Disconnect(); // DB 연결 끊기
            CloseAction?.Invoke();
        }
    }
}
