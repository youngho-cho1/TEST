using LGCNS.iPharmMES.Common;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.ObjectModel;

namespace 보령
{
    public class WMS일반자재환입처리팝업ViewModel : ViewModelBase
    {
        #region [Property]       
        private WMS일반자재환입처리팝업 _mainWnd;

        private WMSPickingSource _CurSourceContainer;
        public WMSPickingSource CurSourceContainer
        {
            get { return _CurSourceContainer; }
            set
            {
                _CurSourceContainer = value;
                OnPropertyChanged("CurSourceContainer");
            }
        }

        private ObservableCollection<WMSPickingSource> _MaterialReturn;
        public ObservableCollection<WMSPickingSource> MaterialReturn
        {
            get { return _MaterialReturn; }
            set
            {
                _MaterialReturn = value;
                OnPropertyChanged("MaterialReturn");
            }
        }
        #endregion
        #region [ICommand]
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;                      
                        CommandResults["LoadedCommand"] = false;
                        CommandCanExecutes["LoadedCommand"] = false;

                        if (arg != null && arg is WMS일반자재환입처리팝업)
                        {
                            _mainWnd = arg as WMS일반자재환입처리팝업;
                        }
                        IsBusy = false;

                        CommandResults["LoadedCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LoadedCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadedCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        public ICommand ConfrimCommandAsync
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;
                        CommandResults["ConfrimCommandAsync"] = false;
                        CommandCanExecutes["ConfrimCommandAsync"] = false;


                        if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                        else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                        CommandResults["ConfrimCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ConfrimCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ConfrimCommandAsync"] = true;
                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        #endregion

        public WMS일반자재환입처리팝업ViewModel()
        {
            _MaterialReturn = new ObservableCollection<WMSPickingSource>();
        }

        public class WMSPickingSource : BizActorDataSetBase
        {
            private string _STATUS = "대기";
            /// <summary>
            /// 대기, 환입대상, 자재환입
            /// </summary>
            public string STATUS
            {
                get { return _STATUS; }
                set
                {
                    _STATUS = value;
                    OnPropertyChanged("STATUS");
                }
            }

            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    OnPropertyChanged("MTRLID");
                }
            }
            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    OnPropertyChanged("MTRLNAME");
                }
            }
            private string _MSUBLOTBCD;
            public string MSUBLOTBCD
            {
                get { return _MSUBLOTBCD; }
                set
                {
                    _MSUBLOTBCD = value;
                    OnPropertyChanged("MSUBLOTBCD");
                }
            }
            private string _COMPONENTGUID;
            public string COMPONENTGUID
            {
                get { return _COMPONENTGUID; }
                set
                {
                    _COMPONENTGUID = value;
                    OnPropertyChanged("COMPONENTGUID");
                }
            }
            private decimal _RETURNQTY;
            public decimal RETURNQTY
            {
                get { return _RETURNQTY; }
                set
                {
                    _RETURNQTY = value;
                    OnPropertyChanged("RETURNQTY");
                }
            }
            private string _UOM;
            public string UOM
            {
                get { return _UOM; }
                set
                {
                    _UOM = value;
                    OnPropertyChanged("UOM");
                }
            }
        }
    }
}
