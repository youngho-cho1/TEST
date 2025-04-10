using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Board
{
    public class BoardSettingPopupViewModel : ViewModelBase
    {
        #region Property

        private BoardSettingPopup _mainWnd;

        private int _timerCount;
        public int timerCount
        {
            get { return _timerCount; }
            set
            {
                _timerCount = value;
                NotifyPropertyChanged();
            }
        }

        #endregion 

        #region Command


        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///

                            if (arg == null || !(arg is BoardSettingPopup))
                                return;

                            _mainWnd = arg as BoardSettingPopup;
                            ///

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

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        
        public ICommand ClickConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickConfirmCommand"] = false;
                            CommandCanExecutes["ClickConfirmCommand"] = false;

                            ///
                            this._mainWnd.DialogResult = true;
                            this._mainWnd.Close();
                            ///

                            CommandResults["ClickConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickConfirmCommand") ?
                        CommandCanExecutes["ClickConfirmCommand"] : (CommandCanExecutes["ClickConfirmCommand"] = true);
                });
            }
        }

        public ICommand ClickCancelCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickCancelCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickCancelCommand"] = false;
                            CommandCanExecutes["ClickCancelCommand"] = false;

                            ///
                            this._mainWnd.DialogResult = false;
                            this._mainWnd.Close();
                            ///

                            CommandResults["ClickCancelCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickCancelCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickCancelCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickCancelCommand") ?
                        CommandCanExecutes["ClickCancelCommand"] : (CommandCanExecutes["ClickCancelCommand"] = true);
                });
            }
        }
        
        #endregion
    }
}
