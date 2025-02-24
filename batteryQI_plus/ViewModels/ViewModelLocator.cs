using batteryQI.ViewModels.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batteryQI.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            //_loginViewModel = new LoginViewModel();
            //_mainWindowViewModel = new MainWindowViewModel();
            //_compositeViewModel = new CompositeViewModel();
            //_inspectViewModel = new InspectViewModel();
            //_managerViewModel = new ManagerViewModel();
            //_tabControlViewModel = new TabControlViewModel();
        }

        private LoginViewModel? _loginViewModel; // 로그인 View
        public LoginViewModel LoginViewModel
        {
            get
            {
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
                if (_tabControlViewModel == null)
                    _tabControlViewModel = new TabControlViewModel();
                return _tabControlViewModel;
            }
        }
    }
}
