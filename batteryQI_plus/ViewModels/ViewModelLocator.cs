using batteryQI.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace batteryQI.ViewModels
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

        private CompositeViewModel? _compositeViewModel; // 데시보드, 불량정보확인 View
        public CompositeViewModel CompositeViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_compositeViewModel == null)
                    _compositeViewModel = new CompositeViewModel();
                return _compositeViewModel;
            }
        }

        private InspectViewModel? _inspectViewModel; // 불량유형확인 View
        public InspectViewModel InspectViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_inspectViewModel == null)
                    _inspectViewModel = new InspectViewModel();
                return _inspectViewModel;
            }
        }

        private ManagerViewModel? _managerViewModel; // 관리자 View
        public ManagerViewModel ManagerViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_managerViewModel == null)
                    _managerViewModel = new ManagerViewModel();
                return _managerViewModel;
            }
        }

        private TabControlViewModel? _tabControlViewModel; // 차트 View
        public TabControlViewModel TabControlViewModel
        {
            get
            {
                // if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;
                if (_tabControlViewModel == null)
                    _tabControlViewModel = new TabControlViewModel();
                return _tabControlViewModel;
            }
        }

        public void Cleanup()
        {
            if (_loginViewModel != null)
            {
                _loginViewModel.Dispose();
                _loginViewModel = null;
            }

            if (_mainWindowViewModel != null)
            {
                _mainWindowViewModel.Dispose();
                _mainWindowViewModel = null;
            }

            // 얘는 다른 방식으로 처리할 수 있으면 처리 시도해야함 응급처치로 ViewModelBase상속하게 해서 해결했음
            if (_compositeViewModel != null) 
            {
                _compositeViewModel.Dispose();
                _compositeViewModel = null;
            }

            if (_inspectViewModel != null)
            {
                _inspectViewModel.Dispose();
                _inspectViewModel = null;
            }

            if (_managerViewModel != null)
            {
                _managerViewModel.Dispose();
                _managerViewModel = null;
            }

            if (_tabControlViewModel != null)
            {
                _tabControlViewModel.Dispose();
                _tabControlViewModel = null;
            }
        }
    }
}
