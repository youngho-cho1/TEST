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
using Weighing;

namespace 보령칭량
{
    public class CustomUIDemoViewModel : ViewModelBase
    {
        #region property
        public CustomUIDemoViewModel()
        {
            CURCOMPONENT = new BufferedObservableCollection<Weighing.ViewModels.WeighingMainViewModel.WeighingComponent>();
        }

        private CustomUIDemo _mainWnd;

        private BufferedObservableCollection<Weighing.ViewModels.WeighingMainViewModel.WeighingComponent> _CURCOMPONENT;
        public BufferedObservableCollection<Weighing.ViewModels.WeighingMainViewModel.WeighingComponent> CURCOMPONENT
        {
            get { return _CURCOMPONENT; }
            set
            {
                _CURCOMPONENT = value;
                OnPropertyChanged("CURCOMPONENT");
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

                                // Current WeighingComponent
                                CURCOMPONENT.Add(_mainWnd.SelectedWeighingComponent);
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
