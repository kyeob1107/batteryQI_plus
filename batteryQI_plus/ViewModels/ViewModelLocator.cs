using batteryQI_plus.ViewModels.Bases;

namespace batteryQI_plus.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator() { }

        private LoginViewModel? _loginViewModel; // 로그인 View
        public LoginViewModel LoginViewModel
        {
            get
            {
                // 디자인 타임때 ViewModel 인스턴스 생성 자체를 차단. 
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;

                if (_loginViewModel == null)
                    _loginViewModel = new LoginViewModel();
                return _loginViewModel;
            }
        }

        private MainWindowViewModel? _mainWindowViewModel; // 메인화면 View
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = new MainWindowViewModel();
                return _mainWindowViewModel;
            }
        }

        //private CompositeViewModel? _compositeViewModel; // 데시보드, 불량정보확인 View
        //public CompositeViewModel CompositeViewModel
        //{
        //    get
        //    {
        //        // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
        //        if (_compositeViewModel == null)
        //            _compositeViewModel = new CompositeViewModel();
        //        return _compositeViewModel;
        //    }
        //}

        //private InspectViewModel? _inspectViewModel; // 불량유형확인 View
        //public InspectViewModel InspectViewModel
        //{
        //    get
        //    {
        //        // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
        //        if (_inspectViewModel == null)
        //            _inspectViewModel = new InspectViewModel();
        //        return _inspectViewModel;
        //    }
        //}

        private SettingViewModel? _SettingViewModel; // 설정 View
        public SettingViewModel SettingViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_SettingViewModel == null)
                    _SettingViewModel = new SettingViewModel();
                return _SettingViewModel;
            }
        }

        //private TabControlViewModel? _tabControlViewModel; // 차트 View
        //public TabControlViewModel TabControlViewModel
        //{
        //    get
        //    {
        //        // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
        //        if (_tabControlViewModel == null)
        //            _tabControlViewModel = new TabControlViewModel();
        //        return _tabControlViewModel;
        //    }
        //}

        private DashboardViewModel? _dashboardViewModel; // 대시보드 View
        public DashboardViewModel DashboardViewModel
        {
            get
            {
                if (_dashboardViewModel == null)
                    _dashboardViewModel = new DashboardViewModel();
                return _dashboardViewModel;
            }
        }

        private AnalysisViewModel? _analysisViewModel; // 분석 View
        public AnalysisViewModel AnalysisViewModel
        {
            get
            {
                // 디자인 타임때 ViewModel 인스턴스 생성 자체를 차단. 
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;

                if (_analysisViewModel == null)
                    _analysisViewModel = new AnalysisViewModel();
                return _analysisViewModel;
            }
        }

        public void Cleanup()
        {
            if (_loginViewModel != null)
            {
                //_loginViewModel.Dispose();
                _loginViewModel = null;
            }

            if (_mainWindowViewModel != null)
            {
                _mainWindowViewModel.Dispose();
                _mainWindowViewModel = null;
            }

            // 얘는 다른 방식으로 처리할 수 있으면 처리 시도해야함 응급처치로 ViewModelBase상속하게 해서 해결했음
            //if (_compositeViewModel != null) 
            //{
            //    _compositeViewModel.Dispose();
            //    _compositeViewModel = null;
            //}

            //if (_inspectViewModel != null)
            //{
            //    _inspectViewModel.Dispose();
            //    _inspectViewModel = null;
            //}

            //if (_managerViewModel != null)
            //{
            //    _managerViewModel.Dispose();
            //    _managerViewModel = null;
            //}

            //if (_tabControlViewModel != null)
            //{
            //    _tabControlViewModel.Dispose();
            //    _tabControlViewModel = null;
            //}

            //if (_dashboardViewModel != null)
            //{
            //    _dashboardViewModel.Dispose();
            //    _dashboardViewModel = null;
            //}

            // 왜인진 모르겠지만 이거 살려두면 _dblink인스턴스 날라가서 지움(확인 필요할듯)
            //if (_analysisViewModel != null)
            //{
            //    _analysisViewModel.Dispose();
            //    _analysisViewModel = null;
            //}
        }
    }
}
