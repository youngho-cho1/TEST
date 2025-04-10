using LGCNS.iPharmMES.Common;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Site
{
    public class CustomUIDemoViewModel : ViewModelBase
    {
        #region property
        public CustomUIDemoViewModel()
        {
            CURORDER = new BufferedObservableCollection<CurrentOrderInfo>();
            CURINST = new BufferedObservableCollection<BR_PHR_SEL_PhaseInstructions_ShopFloor.OUTDATA>();
        }

        private CustomUIDemo _mainWnd;

        private BufferedObservableCollection<CurrentOrderInfo> _CURORDER;
        public BufferedObservableCollection<CurrentOrderInfo> CURORDER
        {
            get { return _CURORDER; }
            set
            {
                _CURORDER = value;
                OnPropertyChanged("CURORDER");
            }
        }
        private BufferedObservableCollection<BR_PHR_SEL_PhaseInstructions_ShopFloor.OUTDATA> _CURINST;
        public BufferedObservableCollection<BR_PHR_SEL_PhaseInstructions_ShopFloor.OUTDATA> CURINST
        {
            get { return _CURINST; }
            set
            {
                _CURINST = value;
                OnPropertyChanged("CURINST");
            }
        }
        #endregion
        #region bizrule
        #endregion
        #region command

        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if(arg != null && arg is CustomUIDemo)
                            {
                                _mainWnd = arg as CustomUIDemo;

                                // CurrentOrder
                                CURORDER.Add(_mainWnd.CurrentOrder);
                                // CurrentInstruction
                                CURINST.Add(_mainWnd.CurrentInstruction.Raw);
                            }
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                       CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
               });
            }
        }

        #endregion
    }
}
