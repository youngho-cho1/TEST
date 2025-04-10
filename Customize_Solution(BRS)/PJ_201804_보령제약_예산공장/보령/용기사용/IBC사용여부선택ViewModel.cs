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
using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;

namespace 보령
{
    public class IBC사용여부선택ViewModel : ViewModelBase
    {
        #region [Property]
        private IBC사용여부선택 _mainWnd;

        private ObservableCollection<IBCInfo> _IBCCollection;
        public ObservableCollection<IBCInfo> IBCCollection
        {
            get { return _IBCCollection; }
            set
            {
                _IBCCollection = value;
                OnPropertyChanged("IBCCollection");
            }
        }
        #endregion

        #region [BizRule]

        #endregion

        #region [Command]
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        try
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is IBC사용여부선택)
                                _mainWnd = arg as IBC사용여부선택;

                            // 조회 BR 추가

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
                        }
                    }, arg =>
                        {
                            return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ? CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                        });
            }
        }

        public ICommand RecycleCommandAsync // 선택한 용기 재사용(Result > 현재 공정 반제품으로 변경)
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["RecycleCommandAsync"] = false;
                        CommandResults["RecycleCommandAsync"] = false;

                        ///
                        _mainWnd.MainBusy.IsBusy = true;

                        var item = arg;


                        ///

                        CommandResults["RecycleCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["RecycleCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        _mainWnd.MainBusy.IsBusy = false;
                        CommandCanExecutes["RecycleCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecycleCommandAsync") ? CommandCanExecutes["RecycleCommandAsync"] : (CommandCanExecutes["RecycleCommandAsync"] = true);
                });
            }
        }

        public ICommand CompletedCommandAsync // 선택한 용기 사용종료
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["CompletedCommandAsync"] = false;
                        CommandResults["CompletedCommandAsync"] = false;

                        ///
                        _mainWnd.MainBusy.IsBusy = true;

                        ///

                        CommandResults["CompletedCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["CompletedCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        _mainWnd.MainBusy.IsBusy = false;
                        CommandCanExecutes["CompletedCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("CompletedCommandAsync") ? CommandCanExecutes["CompletedCommandAsync"] : (CommandCanExecutes["CompletedCommandAsync"] = true);
                });
            }
        }

        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["ComfirmCommandAsync"] = false;
                        CommandResults["ComfirmCommandAsync"] = false;

                        ///
                        

                        ///

                        CommandResults["ComfirmCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ComfirmCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ComfirmCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ? CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region [Generator]
        public IBC사용여부선택ViewModel()
        {
            
        }
        #endregion

        #region [User Define]
        public class IBCInfo : ViewModelBase
        {
            private string _Result;
            public string Result
            {
                get { return _Result; }
                set
                {
                    _Result = value;
                    OnPropertyChanged("Result");
                }
            }
        }
        #endregion
    }
}
