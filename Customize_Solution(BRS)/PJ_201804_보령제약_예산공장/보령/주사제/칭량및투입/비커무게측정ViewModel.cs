using C1.Silverlight.Data;
using Equipment;
using LGCNS.EZMES.Common;
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
using System.Windows.Media;
using System.Windows.Threading;
using Common = LGCNS.iPharmMES.Common.Common;
using System.Configuration;
using System.Collections.ObjectModel;

namespace 보령
{
    public class 비커무게측정ViewModel : ViewModelBase
    {
        #region [Property]
        public 비커무게측정ViewModel()
        {
            int interval = 2000;
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;

            _BeakerCollection = new ObservableCollection<BeakerTare>();
            _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE = new BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE();
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _BR_PHR_UPD_EquipmentAction_Multi = new BR_PHR_UPD_EquipmentAction_Multi();
        }

        비커무게측정 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer(); // 저울값 타이머

        private bool _isBeaker = false;
        private string _BeakerId;
        public string BeakerId
        {
            get { return _BeakerId; }
            set
            {
                _BeakerId = value;
                OnPropertyChanged("BeakerId");
            }
        }
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
        private decimal _ScaleValue;
        private string _ScaleUOM;
        private string _Weight;
        public string Weight
        {
            get { return _Weight; }
            set
            {
                _Weight = value;
                OnPropertyChanged("Weight");
            }
        }
        private ObservableCollection<BeakerTare> _BeakerCollection = new ObservableCollection<BeakerTare>();
        public ObservableCollection<BeakerTare> BeakerCollection
        {
            get { return _BeakerCollection; }
            set
            {
                _BeakerCollection = value;
                OnPropertyChanged("BeakerCollection");
            }
        }
        #endregion
        #region [BizRule]
        private BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE;
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;
        private BR_PHR_UPD_EquipmentAction_Multi _BR_PHR_UPD_EquipmentAction_Multi;
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
                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///
                            if (arg != null && arg is 비커무게측정)
                            {
                                _mainWnd = arg as 비커무게측정;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };
                            }
                                
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
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        public ICommand CheckBeakerCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["CheckBeakerCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["CheckBeakerCommand"] = false;
                            CommandCanExecutes["CheckBeakerCommand"] = false;

                            ///
                            IsBusy = true;
                            if (arg is string && !string.IsNullOrWhiteSpace(arg as string))
                            {
                                BeakerId = (arg as string).ToUpper();

                                // 입력한 코드가 MES에 등록된 비커 그룹인지 확인
                                _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.INDATAs.Clear();
                                _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.OUTDATAs.Clear();
                                _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.INDATAs.Add(new BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.INDATA
                                {
                                    EQPTID = _BeakerId
                                });
                                if (await _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.Execute() && _BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.OUTDATAs.Count > 0)
                                {
                                    if (_BR_PHR_SEL_EquipmentAttribute_GROUP_TYPE.OUTDATAs[0].EQPTGRPID == "BEAKER")
                                        _isBeaker = true;
                                    else
                                        throw new Exception(string.Format("설비 [{0}]은 비커가 아닙니다.", _BeakerId));
                                }
                                else
                                {
                                    throw new Exception(MessageTable.M[CommonMessageCode._3000].Replace("%1", _BeakerId));
                                }
                            }
                            IsBusy = false;
                            ///

                            CommandResults["CheckBeakerCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            BeakerId = "";
                            _isBeaker = false;
                            IsBusy = false;
                            CommandResults["CheckBeakerCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["CheckBeakerCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("CheckBeakerCommand") ?
                        CommandCanExecutes["CheckBeakerCommand"] : (CommandCanExecutes["CheckBeakerCommand"] = true);
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
                            CommandResults["ConnectScaleCommand"] = false;
                            CommandCanExecutes["ConnectScaleCommand"] = false;

                            ///
                            IsBusy = true;

                            if (arg is string && !string.IsNullOrWhiteSpace(arg as string))
                            {
                                ScaleId = (arg as string).ToUpper();

                                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                                _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                                {
                                    ScaleID = _ScaleId
                                });

                                if (await _BR_BRS_SEL_CurrentWeight.Execute() == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                {
                                    _ScaleValue = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : -9999;
                                    _ScaleUOM = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                    Weight = _ScaleValue == -9999 ? "연결실패" : string.Format("{0:0.##0} {1}", _ScaleValue, _ScaleUOM);

                                    _DispatcherTimer.Start();
                                }
                                else
                                    _DispatcherTimer.Stop();
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
        public ICommand SaveBeakerTareWeightCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveBeakerTareWeightCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["SaveBeakerTareWeightCommand"] = false;
                            CommandCanExecutes["SaveBeakerTareWeightCommand"] = false;

                            ///
                            IsBusy = true;
                            _DispatcherTimer.Stop();
                            if(_isBeaker && Weight != "연결실패")
                            {
                                foreach (var item in _BeakerCollection)
                                {
                                    if (item.EQPTID == _BeakerId)
                                        throw new Exception("이미 기록된 비커입니다.");
                                }

                                _BeakerCollection.Add(new BeakerTare
                                {
                                    EQPTID = _BeakerId,
                                    TAREWEIGHT = _Weight,
                                    SCALEVALUE = _ScaleValue,
                                    SCALEUOM = _ScaleUOM
                                });
                            }
                            else if(!_isBeaker)
                                    throw new Exception("비커를 다시 조회하세요.");
                            
                            _DispatcherTimer.Start();
                            IsBusy = false;
                            ///

                            CommandResults["SaveBeakerTareWeightCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["SaveBeakerTareWeightCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            BeakerId = "";
                            _isBeaker = false;
                            CommandCanExecutes["SaveBeakerTareWeightCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveBeakerTareWeightCommand") ?
                        CommandCanExecutes["SaveBeakerTareWeightCommand"] : (CommandCanExecutes["SaveBeakerTareWeightCommand"] = true);
                });
            }
        }
        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            ///
                            IsBusy = true;
                            _DispatcherTimer.Stop();

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
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
                                "비커무게측정",
                                "비커무게측정",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // XML 기록 및 빈용기무게측정 액션 수행
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("비커관리번호"));
                            dt.Columns.Add(new DataColumn("용기무게"));

                            _BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Clear();
                            _BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Clear();

                            // 고정값 조회
                            DateTime curDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            string USERID = !string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")) ? AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI") : AuthRepositoryViewModel.Instance.LoginedUserID;
                            string EQACID = AuthRepositoryViewModel.GetSystemOptionValue("CONTAINER_SETTARE_ACTION");
                            string EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("CONTAINER_TARE_STATUS");
                            string EQPAID_WEIGHT = AuthRepositoryViewModel.GetSystemOptionValue("CONTAINER_TARE_PARA_WEIGHT");
                            string EQPAID_UOM = AuthRepositoryViewModel.GetSystemOptionValue("CONTAINER_TARE_PARA_UOM");

                            foreach (var item in _BeakerCollection)
                            {
                                _BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                {
                                    EQPTID = item.EQPTID,
                                    EQACID = EQACID,
                                    USER = USERID,
                                    DTTM = curDTTM
                                });
                                _BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                {
                                    EQPTID = item.EQPTID,
                                    EQACID = EQACID,
                                    EQSTID = EQSTID
                                });
                                _BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                {
                                    EQPTID = item.EQPTID,
                                    EQSTID = EQSTID,
                                    EQPAID = EQPAID_WEIGHT,
                                    PAVAL = item.SCALEVALUE.ToString()
                                });
                                _BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                {
                                    EQPTID = item.EQPTID,
                                    EQSTID = EQSTID,
                                    EQPAID = EQPAID_UOM,
                                    PAVAL = item.SCALEUOM
                                });

                                DataRow row = dt.NewRow();
                                row["비커관리번호"] = item.EQPTID ?? "";
                                row["용기무게"] = item.TAREWEIGHT ?? "";
                                dt.Rows.Add(row);
                            }

                            if(await _BR_PHR_UPD_EquipmentAction_Multi.Execute())
                            {
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
                            }

                            IsBusy = false;
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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ?
                        CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        public async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                {
                    ScaleID = _ScaleId
                });

                if (await _BR_BRS_SEL_CurrentWeight.Execute() == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                {
                    _ScaleValue = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : -9999;
                    _ScaleUOM = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;

                    Weight = _ScaleValue == -9999 ? "연결실패" : string.Format("{0:0.##0} {1}", _ScaleValue, _ScaleUOM);

                    _DispatcherTimer.Start();
                }
                else
                    _DispatcherTimer.Stop();
            }
            catch (Exception ex)
            {
                _DispatcherTimer.Stop();
                OnException(ex.Message, ex);
            }
        }
        public class BeakerTare : ViewModelBase
        {
            private string _EQPTID;
            public string EQPTID
            {
                get { return _EQPTID; }
                set
                {
                    _EQPTID = value;
                    OnPropertyChanged("EQPTID");
                }
            }
            private string _TAREWEIGHT;
            public string TAREWEIGHT
            {
                get { return _TAREWEIGHT; }
                set
                {
                    _TAREWEIGHT = value;
                    OnPropertyChanged("TAREWEIGHT");
                }
            }
            private decimal _SCALEVALUE;
            public decimal SCALEVALUE
            {
                get { return _SCALEVALUE; }
                set
                {
                    _SCALEVALUE = value;
                    OnPropertyChanged("SCALEVALUE");
                }
            }
            private string _SCALEUOM;
            public string SCALEUOM
            {
                get { return _SCALEUOM; }
                set
                {
                    _SCALEUOM = value;
                    OnPropertyChanged("SCALEUOM");
                }
            }
        }
        #endregion
    }


}
