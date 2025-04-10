using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace 보령
{
    public class 용액냉각및보충ViewModel : ViewModelBase
    {
        #region [Property]
        public 용액냉각및보충ViewModel()
        {         
            int interval = 2000;
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;

            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
        }

        용액냉각및보충 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer(); // 저울값 타이머

        /// <summary>
        /// initial : 용액 초기중량 측정, frozen : 냉각 후 중량 측정 단계, add : 보충 단계, end : 종료
        /// </summary>
        enum state
        {
            initial, frozen, add, end
        };
        private state _curstate;

        private string _ScaleId;
        public string ScaleId
        {
            get { return _ScaleId; }
            set
            {
                _ScaleId = value;
                OnPropertyChanged("ScaleId");
            }
        }
        private string _UOM;
        private string _InitialWeight;
        public string InitialWeight
        {
            get { return _InitialWeight; }
            set
            {
                _InitialWeight = value;
                OnPropertyChanged("InitialWeight");
            }
        }
        private string _FrozenWeight;
        public string FrozenWeight
        {
            get { return _FrozenWeight; }
            set
            {
                _FrozenWeight = value;
                OnPropertyChanged("FrozenWeight");
            }
        }
        private string _AddWeight;
        public string AddWeight
        {
            get { return _AddWeight; }
            set
            {
                _AddWeight = value;
                OnPropertyChanged("AddWeight");
            }
        }
        private string _FinalWeight;
        public string FinalWeight
        {
            get { return _FinalWeight; }
            set
            {
                _FinalWeight = value;
                OnPropertyChanged("FinalWeight");
            }
        }
        private bool _btnNextIsEnable;
        public bool btnNextIsEnable
        {
            get { return _btnNextIsEnable; }
            set
            {
                _btnNextIsEnable = value;
                OnPropertyChanged("btnNextIsEnable");
            }
        }
        private bool _btnPrevIsEnable;
        public bool btnPrevIsEnable
        {
            get { return _btnPrevIsEnable; }
            set
            {
                _btnPrevIsEnable = value;
                OnPropertyChanged("btnPrevIsEnable");
            }
        }
        private bool _btnConfrimEnable;
        public bool btnConfrimEnable
        {
            get { return _btnConfrimEnable; }
            set
            {
                _btnConfrimEnable = value;
                OnPropertyChanged("btnConfrimEnable");
            }
        }
        #endregion
        #region [BizRule]
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;

        #endregion
        #region [Command]
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
                            if (arg != null && arg is 용액냉각및보충)
                            {
                                _mainWnd = arg as 용액냉각및보충;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                _curstate = state.initial;
                                btnNextIsEnable = true;
                                btnPrevIsEnable = false;
                                btnConfrimEnable = false;
                                _mainWnd.txtScaleId.Focus();
                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        public ICommand ConnectScaleCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConnectScaleCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConnectScaleCommand"] = false;
                            CommandCanExecutes["ConnectScaleCommand"] = false;

                            ///
                            if (arg is string && !string.IsNullOrWhiteSpace(arg as string))
                            {
                                ScaleId = (arg as string).ToUpper().Trim();

                                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                                _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                                {
                                    ScaleID = this.ScaleId
                                });

                                if (await _BR_BRS_SEL_CurrentWeight.Execute() == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                {
                                    _UOM = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                                    setWeight(_BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : 0m);

                                    _DispatcherTimer.Start();
                                }                                
                            }

                            IsBusy = false;
                            ///

                            CommandResults["ConnectScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ConnectScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConnectScaleCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConnectScaleCommand") ?
                        CommandCanExecutes["ConnectScaleCommand"] : (CommandCanExecutes["ConnectScaleCommand"] = true);
                });
            }
        }
        public ICommand NextPhaseCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NextPhaseCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NextPhaseCommand"] = false;
                            CommandCanExecutes["NextPhaseCommand"] = false;

                            ///
                            _curstate = _curstate + 1;

                            if (_curstate == state.end)
                            {
                                _DispatcherTimer.Stop();
                                btnNextIsEnable = false;
                                btnPrevIsEnable = true;
                                btnConfrimEnable = true;
                            }
                            else
                            {
                                _DispatcherTimer.Start();
                                btnNextIsEnable = true;
                                btnPrevIsEnable = true;
                            }

                            IsBusy = false;
                            ///

                            CommandResults["NextPhaseCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["NextPhaseCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NextPhaseCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NextPhaseCommand") ?
                        CommandCanExecutes["NextPhaseCommand"] : (CommandCanExecutes["NextPhaseCommand"] = true);
                });
            }
        }
        public ICommand PrevPhaseCommnad
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PrevPhaseCommnad"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PrevPhaseCommnad"] = false;
                            CommandCanExecutes["PrevPhaseCommnad"] = false;

                            ///
                            _curstate = _curstate - 1;

                            switch (_curstate)
                            {
                                case state.initial:
                                    btnNextIsEnable = true;
                                    btnPrevIsEnable = false;
                                    _DispatcherTimer.Start();
                                    break;                    
                                case state.frozen:
                                    btnNextIsEnable = true;
                                    btnPrevIsEnable = true;
                                    FrozenWeight = "";
                                    AddWeight = "";
                                    FinalWeight = "";
                                    _DispatcherTimer.Start();
                                    break;
                                case state.add:
                                    btnNextIsEnable = true;
                                    btnPrevIsEnable = true;
                                    AddWeight = "";
                                    FinalWeight = "";
                                    _DispatcherTimer.Start();
                                    break;
                                default:
                                    break;
                            }

                            IsBusy = false;
                            ///

                            CommandResults["PrevPhaseCommnad"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["PrevPhaseCommnad"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PrevPhaseCommnad"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PrevPhaseCommnad") ?
                        CommandCanExecutes["PrevPhaseCommnad"] : (CommandCanExecutes["PrevPhaseCommnad"] = true);
                });
            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    true,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "용액냉각및보충",
                                "용액냉각및보충",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("조재액무게", typeof(string)));
                            dt.Columns.Add(new DataColumn("냉각후무게", typeof(string)));
                            dt.Columns.Add(new DataColumn("보충량", typeof(string)));
                            dt.Columns.Add(new DataColumn("보충후무게", typeof(string)));

                            DataRow row = dt.NewRow();
                            row["조재액무게"] = InitialWeight ?? "";
                            row["냉각후무게"] = FrozenWeight ?? "";
                            row["보충량"] = AddWeight ?? "";
                            row["보충후무게"] = FinalWeight ?? "";
                            dt.Rows.Add(row);

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _DispatcherTimer.Start();

                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (!string.IsNullOrWhiteSpace(ScaleId))
                {
                    _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                    _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                    _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                    {
                        ScaleID = this.ScaleId
                    });

                    if (await _BR_BRS_SEL_CurrentWeight.Execute() == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                    {
                        _UOM = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                        setWeight(_BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : 0m);
                    }

                    _DispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                _DispatcherTimer.Stop();
                OnException(ex.Message, ex);
            }
        }
        public void setWeight(decimal weight)
        {
            try
            {
                switch (_curstate)
                {
                    case state.initial:
                        InitialWeight = weight.ToString("0.##0") + _UOM;
                        break;
                    case state.frozen:
                        FrozenWeight = weight.ToString("0.##0") + _UOM;
                        break;
                    case state.add:
                        decimal initialvalue;
                        if (decimal.TryParse(FrozenWeight.Replace(_UOM, ""), out initialvalue))
                        {
                            AddWeight = (weight - initialvalue).ToString("0.##0") + _UOM;
                            FinalWeight = weight.ToString("0.##0") + _UOM;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        #endregion
    }


}
