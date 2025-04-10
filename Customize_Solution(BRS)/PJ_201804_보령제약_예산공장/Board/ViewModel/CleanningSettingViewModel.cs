using C1.Silverlight.Excel;
using LGCNS.iPharmMES.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Board
{
    public class CleanningSettingViewModel : ViewModelBase
    {
        #region ##### property ##### 
        private CleanningSetting _mainWnd;
        private DateTime _PeriodSTDTTM;
        public DateTime PeriodSTDTTM
        {
            get { return _PeriodSTDTTM; }
            set
            {
                _PeriodSTDTTM = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _PeriodEDDTTM;
        public DateTime PeriodEDDTTM
        {
            get { return _PeriodEDDTTM; }
            set
            {
                _PeriodEDDTTM = value;
                NotifyPropertyChanged();
            }
        }

        private string _MTRLID;
        public string MTRLID
        {
            get { return _MTRLID; }
            set
            {
                _MTRLID = value;
                NotifyPropertyChanged();
            }
        }

        #endregion ##### property #####

        #region [BizRule]

        private BR_BRS_SEL_CleanningSettingList _BR_BRS_SEL_CleanningSettingList;
        public BR_BRS_SEL_CleanningSettingList BR_BRS_SEL_CleanningSettingList
        {
            get { return _BR_BRS_SEL_CleanningSettingList; }
            set
            {
                _BR_BRS_SEL_CleanningSettingList = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_OperationProcessSegmentReady");
            }
        }

        private BR_BRS_REG_UDT_OperationProcessSegmentReady _BR_BRS_REG_UDT_OperationProcessSegmentReady;
        public BR_BRS_REG_UDT_OperationProcessSegmentReady BR_BRS_REG_UDT_OperationProcessSegmentReady
        {
            get { return _BR_BRS_REG_UDT_OperationProcessSegmentReady; }
            set
            {
                _BR_BRS_REG_UDT_OperationProcessSegmentReady = value;
                OnPropertyChanged("BR_BRS_REG_UDT_OperationProcessSegmentReady");
            }
        }


        #endregion

        public CleanningSettingViewModel()
        {
            _BR_BRS_SEL_CleanningSettingList = new BR_BRS_SEL_CleanningSettingList();
            _BR_BRS_REG_UDT_OperationProcessSegmentReady = new BR_BRS_REG_UDT_OperationProcessSegmentReady();
        }       

        public ICommand LoadedCommandAsync
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
                            
                            
                            _mainWnd = arg as CleanningSetting;
                            

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

        

        public ICommand BtnSearchCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SearchCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SearchCommand"] = false;
                            CommandCanExecutes["SearchCommand"] = false;

                            _BR_BRS_SEL_CleanningSettingList.INDATAs.Clear();
                            _BR_BRS_SEL_CleanningSettingList.OUTDATAs.Clear();

                            _BR_BRS_SEL_CleanningSettingList.INDATAs.Add(new BR_BRS_SEL_CleanningSettingList.INDATA()
                            {
                                MTRLID = string.IsNullOrWhiteSpace(MTRLID) ? null : MTRLID
                            });

                            await _BR_BRS_SEL_CleanningSettingList.Execute();

                            CommandResults["SearchCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SearchCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SearchCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SearchCommand") ?
                        CommandCanExecutes["SearchCommand"] : (CommandCanExecutes["SearchCommand"] = true);
                });
            }
        }

        public ICommand BtnUpdateCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BtnUpdateCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["BtnUpdateCommand"] = false;
                            CommandCanExecutes["BtnUpdateCommand"] = false;

                            _BR_BRS_REG_UDT_OperationProcessSegmentReady.INDATAs.Clear();                            
                            foreach (var item in _BR_BRS_SEL_CleanningSettingList.OUTDATAs)
                            {
                                if (item.CHK == "Y")
                                {
                                    _BR_BRS_REG_UDT_OperationProcessSegmentReady.INDATAs.Add(new BR_BRS_REG_UDT_OperationProcessSegmentReady.INDATA
                                    {
                                        ODID = item.ODID,
                                        ODVER = item.ODVER,
                                        MTRLID = item.MTRLID,
                                        MTRLVER = item.MTRLVER,
                                        OPSGGUID = item.OPSGGUID,
                                        READYVAL = item.READYVAL,
                                        READYVER = item.READYVER,
                                        EQPTREADYVAL = item.EQPTREADYVAL,
                                        INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID
                                    });
                                }
                            }
                            await _BR_BRS_REG_UDT_OperationProcessSegmentReady.Execute();

                            _BR_BRS_SEL_CleanningSettingList.INDATAs.Clear();
                            _BR_BRS_SEL_CleanningSettingList.OUTDATAs.Clear();

                            _BR_BRS_SEL_CleanningSettingList.INDATAs.Add(new BR_BRS_SEL_CleanningSettingList.INDATA()
                            {
                                MTRLID = string.IsNullOrWhiteSpace(MTRLID) ? null : MTRLID
                            });

                            await _BR_BRS_SEL_CleanningSettingList.Execute();

                            CommandResults["BtnUpdateCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BtnUpdateCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BtnUpdateCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BtnUpdateCommand") ?
                        CommandCanExecutes["BtnUpdateCommand"] : (CommandCanExecutes["BtnUpdateCommand"] = true);
                });
            }
        }

        public ICommand RowEditCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RowEditCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["RowEditCommand"] = false;
                            CommandCanExecutes["RowEditCommand"] = false;

                            IsBusy = true;

                            var temp = _mainWnd.CleanningSettingGrid.SelectedItem as BR_BRS_SEL_CleanningSettingList.OUTDATA;
                            if (temp.READYVAL != 0)
                            {
                                temp.CHK = "Y";
                            }                                                       

                            IsBusy = false;

                            CommandResults["RowEditCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RowEditCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RowEditCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RowEditCommand") ?
                        CommandCanExecutes["RowEditCommand"] : (CommandCanExecutes["RowEditCommand"] = true);
                });
            }
        }

        //public ICommand RowAddCommand
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["RowAddCommand"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["RowAddCommand"] = false;
        //                    CommandCanExecutes["RowAddCommand"] = false;

        //                    IsBusy = true;

        //                    var temp = _mainWnd.CleanningSettingGrid.SelectedItem as BR_BRS_SEL_UDT_OperationProcessSegmentReady.OUTDATA;
        //                    temp.CHK = "Y";

        //                    IsBusy = false;

        //                    CommandResults["RowAddCommand"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    CommandResults["RowAddCommand"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["RowAddCommand"] = true;

        //                    IsBusy = false;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("RowAddCommand") ?
        //                CommandCanExecutes["RowAddCommand"] : (CommandCanExecutes["RowAddCommand"] = true);
        //        });
        //    }
        //}


        //public ICommand RowDeleteCommand
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["RowDeleteCommand"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["RowDeleteCommand"] = false;
        //                    CommandCanExecutes["RowDeleteCommand"] = false;

        //                    IsBusy = true;
        //                    ///
        //                    ///
        //                    IsBusy = false;

        //                    CommandResults["RowDeleteCommand"] = true;
        //                    return;
        //                }
        //                catch (Exception ex)
        //                {
        //                    CommandResults["RowDeleteCommand"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["RowDeleteCommand"] = true;

        //                    IsBusy = false;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("RowDeleteCommand") ?
        //                CommandCanExecutes["RowDeleteCommand"] : (CommandCanExecutes["RowDeleteCommand"] = true);
        //        });
        //    }
        //}

    }
}
