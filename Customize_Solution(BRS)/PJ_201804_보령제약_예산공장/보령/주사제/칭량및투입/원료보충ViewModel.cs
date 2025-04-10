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
using 보령.UserControls;

namespace 보령
{
    public class 원료보충ViewModel : ViewModelBase
    {
        #region [Property]
        public 원료보충ViewModel()
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
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ = new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_BRS_SEL_Charging_Solvent_to_Dispense = new BR_BRS_SEL_Charging_Solvent_to_Dispense();
            _BR_BRS_REG_Dispense_Charging_Solvent_Add = new BR_BRS_REG_Dispense_Charging_Solvent_Add();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();
        }

        원료보충 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer(); // 저울값 타이머
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
        private BR_PHR_SEL_System_Printer.OUTDATA _selectedPrint;
        public string curPrintName
        {
            get
            {
                if (_selectedPrint != null)
                    return _selectedPrint.PRINTERNAME;
                else
                    return "N/A";
            }
        }
        private DateTime _DspStartDttm;

        /// <summary>
        /// frozen : 냉각 후 중량 측정 단계, add : 보충 단계, end : 종료
        /// </summary>
        enum state
        {
            frozen, add, end, exception
        };
        private state _curstate;
        public string CurrentStateSTR
        {
            get
            {
                switch (_curstate)
                {
                    case state.frozen:
                        return "냉각된조제액";
                    case state.add:
                        return "원료보충";
                    case state.end:
                        return "보충완료";
                    case state.exception:
                        return "N/A";
                    default:
                        return "N/A";
                }
            }
        }

        private string _HEADER1;
        public string HEADER1
        {
            get { return _HEADER1; }
            set
            {
                _HEADER1 = value;
                OnPropertyChanged("HEADER1");
            }
        }
        private string _HEADER2;
        public string HEADER2
        {
            get { return _HEADER2; }
            set
            {
                _HEADER2 = value;
                OnPropertyChanged("HEADER2");
            }
        }

        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
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
        private Weight _ScaleWeight = new Weight();
        public string ScaleWeight
        {
            get { return _ScaleWeight.WeightUOMString; }
        }
        public int ScalePrecision
        {
            set
            {
                _ScaleWeight.Precision = value;
                _FrozenWeight.Precision = value;
                _StandardWeight.Precision = value;
                _LossWeight.Precision = value;
                _MinWeight.Precision = value;
                _MaxWeight.Precision = value;
                _AddWeight.Precision = value;

                WeightRefresh();
            }
        }
        public string ScaleUom
        {
            set
            {
                _ScaleWeight.Uom = value;
                _FrozenWeight.Uom = value;
                _StandardWeight.Uom = value;
                _LossWeight.Uom = value;
                _MinWeight.Uom = value;
                _MaxWeight.Uom = value;
                _AddWeight.Uom = value;

                WeightRefresh();
            }
        }

        /// <summary>
        /// 냉각된 조제액
        /// </summary>
        private Weight _FrozenWeight = new Weight();
        public string FrozenWeight
        {
            get { return _FrozenWeight.WeightUOMString; }
        }
        /// <summary>
        /// 조제액 기준
        /// </summary>
        private Weight _StandardWeight = new Weight();
        public string StandardWeight
        {
            get { return _StandardWeight.WeightUOMString; }
        }

        /// <summary>
        /// 손실량(보충량 기준)
        /// </summary>
        private Weight _LossWeight = new Weight();
        public string LossWeight
        {
            get { return _LossWeight.WeightUOMString; }
        }
        /// <summary>
        /// 최대값(보충량 기준 최대값)
        /// </summary>
        private Weight _MaxWeight = new Weight();
        public string MaxWeight
        {
            get { return _MaxWeight.WeightUOMString; }
        }
        /// <summary>
        /// 최소값(보충량 기준 최소값)
        /// </summary>
        private Weight _MinWeight = new Weight();
        public string MinWeight
        {
            get { return _MinWeight.WeightUOMString; }
        }

        /// <summary>
        /// 보충량
        /// </summary>
        private Weight _AddWeight = new Weight();
        public string AddWeight
        {
            get { return _AddWeight.WeightUOMString; }
        }

        /// <summary>
        /// 최종저울값
        /// </summary>
        private Weight _FinalWeight = new Weight();       


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

        /// <summary>
        /// 설정된 원료백 정보
        /// </summary>
        private BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATA _SourceContainer;
        #endregion
        #region [BizRule]
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;
        private BR_BRS_SEL_ProductionOrderBOM_CHGSEQ _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ;
        public BR_BRS_SEL_ProductionOrderBOM_CHGSEQ BR_BRS_SEL_ProductionOrderBOM_CHGSEQ
        {
            get { return _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ; }
            set
            {
                _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderBOM_CHGSEQ");
            }
        }
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        private BR_BRS_SEL_Charging_Solvent_to_Dispense _BR_BRS_SEL_Charging_Solvent_to_Dispense;
        private BR_BRS_REG_Dispense_Charging_Solvent_Add _BR_BRS_REG_Dispense_Charging_Solvent_Add;
        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
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
                            if (arg != null && arg is 원료보충)
                            {
                                _mainWnd = arg as 원료보충;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                btnPrevIsEnable = true;
                                btnNextIsEnable = true;
                                _DspStartDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                // 조제액은 Parmeter로 전달받은 BOM의 기준량 합. 현재 Instrction의 원료가 보충하는 원료. 헤더 문구 조회
                                var inputvalue = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                                _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.OUTDATAs.Clear();
                                if (inputvalue.Count > 0)
                                {
                                    // Header Text Set
                                    if(string.IsNullOrWhiteSpace(inputvalue[0].Raw.TARGETVAL))
                                        throw new Exception("헤더가 정해지지 않았습니다");
                                    else
                                    {
                                        var headers = inputvalue[0].Raw.TARGETVAL.Split(',');
                                        if(headers.Count() == 2)
                                        {
                                            HEADER1 = headers[0].ToString();
                                            HEADER2 = headers[1].ToString();
                                        }
                                        else
                                            throw new Exception("헤더가 잘못 정해졌습니다.");
                                    }

                                    // 조제액 원료
                                    foreach (var item in inputvalue)
                                    {
                                        _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.INDATAs.Add(new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.INDATA
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            MTRLID = item.Raw.BOMID,
                                            CHGSEQ = item.Raw.EXPRESSION
                                        });
                                    }
                                }
                                else
                                    throw new Exception("파라미터가 설정되지 않았습니다.");

                                // 보충원료 추가
                                _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.INDATAs.Add(new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID,
                                    CHGSEQ = _mainWnd.CurrentInstruction.Raw.EXPRESSION
                                });

                                if (await BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.Execute() && BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.OUTDATAs.Count > 0)
                                {
                                    Weight temp = new Weight();
                                    foreach (var item in BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.OUTDATAs)
                                    {
                                        // 기준량 합산
                                        if (temp.SetWeight(item.STDQTY, item.UOMID))
                                            _StandardWeight = _StandardWeight.Add(temp);
                                        else
                                            throw new Exception(string.Format("기준 조회 실패 : [{0}]", item.MTRLID));
                                    }
                                }

                                // 보충원료 피킹정보 조회
                                _BR_BRS_SEL_Charging_Solvent_to_Dispense.INDATAs.Clear();
                                _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs.Clear();
                                _BR_BRS_SEL_Charging_Solvent_to_Dispense.INDATAs.Add(new BR_BRS_SEL_Charging_Solvent_to_Dispense.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                    MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID,
                                    CHGSEQ = _mainWnd.CurrentInstruction.Raw.EXPRESSION
                                });

                                if (await _BR_BRS_SEL_Charging_Solvent_to_Dispense.Execute() && _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs.Count > 0)
                                {
                                    bool flag = true; // 투입가능한 원료가 없는 경우 true
                                    string msg = "투입가능한 원료백 목록 : [";
                                     
                                    foreach (var item in _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs)
                                    {
                                        if (item.MSUBLOTQTY > 0)
                                        {
                                            msg += item.MSUBLOTBCD + " ";
                                            flag = false;
                                        }                                            
                                    }

                                    if (flag)
                                        throw new Exception("투입 가능한 원료백이 없습니다.");
                                }
                                else
                                    throw new Exception("조회된 원료백 정보가 없습니다.");


                                // 프린트 조회
                                _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    ROOMID = AuthRepositoryViewModel.Instance.RoomID
                                });

                                if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                                    _selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                                else
                                    OnMessage("연결된 프린트가 없습니다.");
                                OnPropertyChanged("curPrintName");

                                _curstate = state.frozen;

                                IsBusy = true;
                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _curstate = state.exception;
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            WeightRefresh();
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

                            CommandResults["ConnectScaleCommand"] = false;
                            CommandCanExecutes["ConnectScaleCommand"] = false;

                            ///
                            IsBusy = true;

                            _DispatcherTimer.Stop();
                            if (_curstate != state.exception)
                            {
                                BarcodePopup popup = new BarcodePopup();
                                popup.tbMsg.Text = "저울바코드를 스캔하세요.";
                                popup.Closed += async (sender, e) =>
                                {
                                    if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                                    {
                                        string text = popup.tbText.Text.ToUpper();

                                        // 저울 정보 조회
                                        _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                                        _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                                        _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                                        {
                                            LANGID = AuthRepositoryViewModel.Instance.LangID,
                                            EQPTID = text
                                        });

                                        if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute() && _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Count > 0)
                                        {
                                            _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                            ScalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                                            ScaleUom = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].NOTATION;
                                            ScaleId = text;
                                            _DispatcherTimer.Start();
                                        }
                                    }
                                    else
                                        ScaleId = "";
                                };

                                popup.Show();
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
                            OnPropertyChanged("CurrentStateSTR");
                            WeightRefresh();
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
                            _DispatcherTimer.Stop();

                            switch (_curstate)
                            {
                                case state.frozen:
                                    if (_ScaleWeight.Subtract(_StandardWeight).Value > 0)
                                    {
                                        OnMessage("냉각된 조제액 무게가 기준량보다 클 수 없습니다.");
                                        _DispatcherTimer.Start();
                                        break;
                                    }

                                    BarcodePopup popup = new BarcodePopup();
                                    popup.tbMsg.Text = "보충할 원료 바코드를 스캔하세요.";
                                    popup.Closed += (sender, e) =>
                                    {
                                        if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                                        {
                                            string text = popup.tbText.Text.ToUpper();

                                            foreach (var item in _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs)
                                            {
                                                if (item.MSUBLOTBCD.ToUpper() == text)
                                                {
                                                    if (item.MSUBLOTQTY > 0)
                                                        _SourceContainer = item;
                                                }
                                            }

                                            if (_SourceContainer == null)
                                                OnMessage("스캔한 바코드가 존재하지 않거나 잔량이 0입니다.");
                                            else
                                            {
                                                // 냉각된 조제액 무게 설정
                                                _FrozenWeight = _ScaleWeight.Copy();

                                                // 보충 기준 설정
                                                _LossWeight = _StandardWeight.Subtract(_FrozenWeight);
                                                _MinWeight = _LossWeight.Copy();
                                                _MaxWeight = _LossWeight.Copy();
                                                _MinWeight.Value = _MinWeight.Value * 0.99m;
                                                _MaxWeight.Value = _MaxWeight.Value * 1.01m;

                                                _curstate = state.add;
                                                WeightRefresh();
                                                OnPropertyChanged("CurrentStateSTR");
                                                _DispatcherTimer.Start();
                                            }
                                        }
                                        else
                                            OnMessage("취소 되었습니다.");
                                    };
                                    popup.Show();
                                    break;
                                case state.add:
                                    var curAddweight = _ScaleWeight.Subtract(_FrozenWeight);

                                    if (curAddweight.Subtract(_MinWeight).Value >= 0 && curAddweight.Subtract(_MaxWeight).Value <= 0)
                                    {
                                        // 보충량 저장
                                        _AddWeight = curAddweight;
                                        _curstate = state.end;
                                    }
                                    else
                                        OnMessage("보충량이 기준값을 벗어났습니다.");

                                    if (_curstate != state.end)
                                        _DispatcherTimer.Start();
                                    break;
                                default:
                                    break;
                            }

                            IsBusy = false;
                            ///

                            CommandResults["NextPhaseCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["NextPhaseCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            WeightRefresh();
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
                            _DispatcherTimer.Stop();

                            switch (_curstate)
                            {
                                case state.add:
                                    if (_ScaleWeight.Subtract(_FrozenWeight).Value <= 0)
                                    {
                                        _FrozenWeight.Value = 0m;
                                        _LossWeight.Value = 0m;
                                        _MinWeight.Value = 0m;
                                        _MaxWeight.Value = 0m;
                                        _SourceContainer = null;

                                        _curstate = state.frozen;
                                        _DispatcherTimer.Start();
                                    }
                                    else
                                        OnMessage("현재 저울값이 냉각된조제액무게 보다 크면 이전단계로 돌아갈 수 없습니다.");

                                    _DispatcherTimer.Start();
                                    break;
                                case state.end:
                                    _AddWeight.Value = 0m;
                                    _curstate = state.add;
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
                            _DispatcherTimer.Start();
                            CommandResults["PrevPhaseCommnad"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            WeightRefresh();
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

                            ///
                            _DispatcherTimer.Stop();

                            if(_curstate != state.end)
                                throw new Exception("칭량이 완료되지 않았습니다.");

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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "원료보충",
                                "원료보충",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // XML 저장
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("냉각된무게"));
                            dt.Columns.Add(new DataColumn("하한"));
                            dt.Columns.Add(new DataColumn("손실량"));
                            dt.Columns.Add(new DataColumn("상한"));
                            dt.Columns.Add(new DataColumn("보충량"));
                            dt.Columns.Add(new DataColumn("저울번호"));

                            DataRow row = dt.NewRow();
                            row["냉각된무게"] = FrozenWeight ?? "";
                            row["하한"] = MinWeight ?? "";
                            row["손실량"] = LossWeight ?? "";
                            row["상한"] = MaxWeight ?? "";
                            row["보충량"] = AddWeight ?? "";
                            row["저울번호"] = ScaleId ?? "";
                            dt.Rows.Add(row);

                            // BOM 정보
                            dt = new DataTable("DATA1");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("기준량"));

                            foreach (var item in BR_BRS_SEL_ProductionOrderBOM_CHGSEQ.OUTDATAs)
                            {
                                if (item.MTRLNAME != "비커")
                                {
                                    var row1 = dt.NewRow();
                                    row1["원료명"] = item.MTRLNAME ?? "";
                                    row1["기준량"] = item.STDQTY ?? "";
                                    dt.Rows.Add(row1);
                                }
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            // 원료 보충 및 칭량이력(냉각된 조제액 무게) 저장
                            _BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATAs.Clear();
                            _BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATA_SUBs.Clear();
                            _BR_BRS_REG_Dispense_Charging_Solvent_Add.OUTDATAs.Clear();

                            var dttm = AuthRepositoryViewModel.GetDateTimeByFunctionCode("OM_ProductionOrder_Charging");
                            DateTime chargingdttm = dttm.HasValue ? dttm.Value : await AuthRepositoryViewModel.GetDBDateTimeNow();
                            string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging");

                            _BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATAs.Add(new BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATA
                            {
                                LANGID = "ko-KR",
                                MSUBLOTID = _SourceContainer.MSUBLOTID ?? "",
                                MSUBLOTBCD = _SourceContainer.MSUBLOTBCD ?? "",
                                INSDTTM = chargingdttm,
                                INSUSER = user,
                                DEPLETFLAG = "P",
                                VESSELID = "",
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                MSUBLOTTYPE = "DSP",
                                COMPONENTGUID = _SourceContainer.COMPONENTGUID != null ? _SourceContainer.COMPONENTGUID : "",
                                TARE = 0m,
                                LOCATIONID = AuthRepositoryViewModel.Instance.RoomID,
                                INVENTORYQTY = _SourceContainer.MSUBLOTQTY,
                                DISPENSEQTY = _AddWeight.Value,
                                ISDISPSTRT = "C",
                                ACTID = "Dispensing",
                                CHECKINUSER = user,
                                CHECKINDTTM = chargingdttm,
                                WEIGHINGMETHOD = "WH007",
                                UPPERVALUE = _MaxWeight.Value,
                                LOWERVALUE = _MinWeight.Value,
                                LOTTYPE = string.Empty,
                                OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                TAREWEIGHT = 0,
                                TAREUOMID = _AddWeight.Uom,
                                REASON = "원료보충",
                                SCALEID = this.ScaleId,
                                INSSIGNATURE = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                                DSPSTRTDTTM = _DspStartDttm.ToString("yyyy-MM-dd HH:mm:ss"),
                                DSPENDDTTM = chargingdttm.ToString("yyyy-MM-dd HH:mm:ss"),
                                WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID,
                                SCALEPRECISION = _AddWeight.Precision
                            });

                            _BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATA_SUBs.Add(new BR_BRS_REG_Dispense_Charging_Solvent_Add.INDATA_SUB
                            {
                                HEADER1 = _HEADER1,
                                HEADER2 = _HEADER2,
                                FROZENQTY = _FrozenWeight.WeightString
                            });

                            if (await _BR_BRS_REG_Dispense_Charging_Solvent_Add.Execute())
                            {
                                if (_BR_BRS_REG_Dispense_Charging_Solvent_Add.OUTDATAs.Count > 0 && curPrintName != "N/A")
                                {
                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                    {
                                        ReportPath = "/Reports/Label/LABEL_C0402_018_10",
                                        PrintName = _selectedPrint.PRINTERNAME,
                                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                    });
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                    {
                                        ParamName = "MSUBLOTID",
                                        ParamValue = _BR_BRS_REG_Dispense_Charging_Solvent_Add.OUTDATAs[0].DSPMSUBLOTID
                                    });
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                    {
                                        ParamName = "GUBUN",
                                        ParamValue = "원료보충"
                                    });

                                    await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                                }

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                _DispatcherTimer.Stop();
                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }                            

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
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
        public ICommand ChangePrintCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangePrintCommand"] = false;
                        CommandCanExecutes["ChangePrintCommand"] = false;

                        ///
                        SelectPrinterPopup popup = new SelectPrinterPopup();

                        popup.Closed += (s, e) =>
                        {
                            if (popup.DialogResult.GetValueOrDefault())
                            {
                                if (popup.SourceGrid.SelectedItem != null && popup.SourceGrid.SelectedItem is BR_PHR_SEL_System_Printer.OUTDATA)
                                {
                                    _selectedPrint = popup.SourceGrid.SelectedItem as BR_PHR_SEL_System_Printer.OUTDATA;
                                    OnPropertyChanged("curPrintName");
                                }
                            }
                        };

                        popup.Show();
                        ///

                        CommandResults["ChangePrintCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangePrintCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        WeightRefresh();
                        OnPropertyChanged("CurrentStateSTR");
                        CommandCanExecutes["ChangePrintCommand"] = true;
                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangePrintCommand") ?
                        CommandCanExecutes["ChangePrintCommand"] : (CommandCanExecutes["ChangePrintCommand"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        private void WeightRefresh()
        {
            OnPropertyChanged("ScaleWeight");
            OnPropertyChanged("FrozenWeight");
            OnPropertyChanged("StandardWeight");
            OnPropertyChanged("LossWeight");
            OnPropertyChanged("MinWeight");
            OnPropertyChanged("MaxWeight");
            OnPropertyChanged("AddWeight");
        }
        private async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (_ScaleInfo != null && !string.IsNullOrWhiteSpace(_ScaleId))
                {
                    // 저울에 Tare 명령어 전달
                    bool success = false;
                    string curWeight = string.Empty;
                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleId, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            curWeight = result.data;
                            ScaleUom = result.unit;

                            // 저울 유효숫자 설정
                            if (curWeight.Contains("."))
                            {
                                var splt = curWeight.Split('.');
                                if (splt.Length > 1)
                                    ScalePrecision = splt[1].Length;
                                else
                                    ScalePrecision = 0;
                            }
                            else
                                ScalePrecision = 0;
                        }
                    }
                    else
                    {
                        _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                        _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                        _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                        {
                            ScaleID = _ScaleId
                        });

                        if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                        {
                            success = true;
                            curWeight = string.Format(("{0:N" + _ScaleWeight.Precision + "}"), _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight);
                            ScaleUom = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                        }
                    }

                    if (success)
                        _ScaleWeight.SetWeight(curWeight, _ScaleWeight.Uom);
                    else
                        _ScaleWeight.Value = 0;

                    WeightRefresh();
                    _DispatcherTimer.Start();
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
