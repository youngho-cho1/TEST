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
using 보령.UserControls;
using System.Configuration;


namespace 보령
{
    public class 정제수확인및투입ViewModel : ViewModelBase
    {
        #region [Property]
        public 정제수확인및투입ViewModel()
        {
            _BR_BRS_SEL_Charging_Solvent = new BR_BRS_SEL_Charging_Solvent();
            _filteredComponents = new BR_BRS_SEL_Charging_Solvent.OUTDATACollection();
            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW = new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT = new 보령.BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();

            int interval = 2000;

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }
            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
        }
        enum enumScanType
        {
            Material,
            Scale
        };
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
        private DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        private 정제수확인및투입 _mainWnd;
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
        private BR_BRS_SEL_Charging_Solvent.OUTDATA _selectedComponent;
        public BR_BRS_SEL_Charging_Solvent.OUTDATA selectedComponent
        {
            get { return _selectedComponent; }
            set
            {
                _selectedComponent = value;
                OnPropertyChanged("selectedComponent");
            }
        }
        private BR_BRS_SEL_Charging_Solvent.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_Charging_Solvent.OUTDATACollection filteredComponents
        {
            get { return _filteredComponents; }
            set
            {
                _filteredComponents = value;
                NotifyPropertyChanged();
            }
        }
        #region [Material]
        private string _mtrlId;
        public string MtrlId
        {
            get { return _mtrlId; }
            set
            {
                _mtrlId = value;
                OnPropertyChanged("MtrlId");
            }
        }
        private string _mtrlName;
        public string MtrlName
        {
            get { return _mtrlName; }
            set
            {
                _mtrlName = value;
                OnPropertyChanged("MtrlName");
            }
        }
        private string _stdQty;
        public string StdQty
        {
            get { return _stdQty; }
            set
            {
                _stdQty = value;
                OnPropertyChanged("StdQty");
            }
        }
        #endregion
        #region [Scale]
        private int _scalePrecision = 3;
        public int scalePrecision
        {
            set
            {
                _scalePrecision = value;
                _LowerWeight.Precision = _scalePrecision;
                _UpperWeight.Precision = _scalePrecision;
                OnPropertyChanged("ScaleValue");
                OnPropertyChanged("UpperWeight");
                OnPropertyChanged("LowerWeight");
            }
        }
        private string _scaleId;
        public string ScaleId
        {
            get { return _scaleId; }
            set
            {
                _scaleId = value;
                OnPropertyChanged("ScaleId");
            }
        }
        private Weight _ScaleValue = new Weight();
        public string ScaleValue
        {
            get { return _ScaleValue.WeightUOMString; }
        }
        private Weight _TotalWeight = new Weight();
        public string TotalWeight
        {
            get
            {
                if (_ScaleException)
                    return _ScaleExceptionMsg;
                else
                    return _TotalWeight.WeightUOMString;
            }
        }
        private SolidColorBrush _ScaleBackground;
        public SolidColorBrush ScaleBackground
        {
            get { return _ScaleBackground; }
            set
            {
                _ScaleBackground = value;
                OnPropertyChanged("ScaleBackground");
            }
        }
        private Weight _UpperWeight = new Weight();
        public string UpperWeight
        {
            get { return _UpperWeight.WeightUOMString; }
        }
        private Weight _LowerWeight = new Weight();
        public string LowerWeight
        {
            get { return _LowerWeight.WeightUOMString; }
        }

        // 저울에러
        private bool _ScaleException;
        private string _ScaleExceptionMsg = "저울 연결 실패";
        #endregion
        /// <summary>
        /// 투입버튼
        /// </summary>
        private bool _IsCharging;
        public bool IsCharging
        {
            get { return _IsCharging; }
            set
            {
                _IsCharging = value;
                OnPropertyChanged("IsCharging");
            }
        }
        /// <summary>
        /// 기록버튼
        /// </summary>
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }       
        #endregion
        #region [Bizrule]
        private BR_BRS_SEL_Charging_Solvent _BR_BRS_SEL_Charging_Solvent;
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        private BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW;
        private BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT;

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
        /// <summary>
        /// 라벨발행
        /// </summary>
        BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        /// <summary>
        /// 프린터 조회
        /// </summary>
        BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
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
                            if (arg != null && arg is 정제수확인및투입)
                            {
                                _mainWnd = arg as 정제수확인및투입;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                string bomid = _mainWnd.CurrentInstruction.Raw.BOMID;
                                string chgseq = _mainWnd.CurrentInstruction.Raw.EXPRESSION;
                                ScaleId = _mainWnd.CurrentInstruction.Raw.EQPTID;

                                if (string.IsNullOrWhiteSpace(bomid))
                                    throw new Exception(string.Format("해당 Instruction에 BOM ID가 설정되지 않았습니다."));

                                // 저울 설정
                                if (!string.IsNullOrWhiteSpace(ScaleId))
                                {
                                    _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                                    _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                                    _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                                    {
                                        LANGID = AuthRepositoryViewModel.Instance.LangID,
                                        EQPTID = ScaleId
                                    });

                                    if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute() && _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Count > 0)
                                    {
                                        _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                        scalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                                        _DispatcherTimer.Start();
                                    }
                                }
                                else
                                    ScaleId = "미설정";

                                //BOM 기준정보
                                _BR_BRS_SEL_Charging_Solvent.INDATAs.Add(new BR_BRS_SEL_Charging_Solvent.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = chgseq,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                    MTRLID = bomid
                                });

                                if (await _BR_BRS_SEL_Charging_Solvent.Execute() && _BR_BRS_SEL_Charging_Solvent.OUTDATA_BOMs.Count > 0)
                                {
                                    var item = _BR_BRS_SEL_Charging_Solvent.OUTDATA_BOMs.Where(o => o.MTRLID == bomid && o.CHGSEQ == chgseq).FirstOrDefault();

                                    MtrlId = item.MTRLID;
                                    MtrlName = item.MTRLNAME;
                                    StdQty = string.Format("{0:#,0}", Convert.ToDecimal(item.STDQTY)) + " " + item.NOTATION;
                                }

                                if (_BR_BRS_SEL_Charging_Solvent.OUTDATAs.Count > 0)
                                {
                                    foreach (var outdata in _BR_BRS_SEL_Charging_Solvent.OUTDATAs.Where(o => o.MTRLID == bomid && o.CHGSEQ == chgseq).ToList())
                                    {
                                        outdata.CHECK = "투입대기";
                                        filteredComponents.Add(outdata);
                                    }

                                    if (_filteredComponents[0].UPPERQTY.HasValue && _filteredComponents[0].LOWERQTY.HasValue)
                                    {
                                        _UpperWeight.SetWeight(_filteredComponents[0].UPPERQTY.Value, _filteredComponents[0].UOMNAME, _scalePrecision);
                                        _LowerWeight.SetWeight(_filteredComponents[0].LOWERQTY.Value, _filteredComponents[0].UOMNAME, _scalePrecision);
                                        OnPropertyChanged("UpperWeight"); OnPropertyChanged("LowerWeight");
                                    }
                                    else
                                        throw new Exception("칭량 범위가 설정되지 않았습니다.");
                                }
                                else
                                    throw new Exception(string.Format("조회된 결과가 없습니다."));

                                // 프린터 설정
                                _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    ROOMID = AuthRepositoryViewModel.Instance.RoomID,
                                    IPADDRESS = Common.ClientIP
                                });
                                if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                                {
                                    _selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                                    OnPropertyChanged("curPrintName");
                                }
                                else
                                    OnMessage("연결된 프린트가 없습니다.");

                                // 칭량 준비
                                if (filteredComponents.Count > 0)
                                    selectedComponent = _filteredComponents[0];

                                // 버튼 설정
                                IsCharging = false;
                                IsEnabled = false;
                            }                        
                            ///
                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            _DispatcherTimer.Stop();
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
        public ICommand ScanScaleCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanScaleCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            CommandResults["ScanScaleCommandAsync"] = false;
                            CommandCanExecutes["ScanScaleCommandAsync"] = false;

                            ///
                            _DispatcherTimer.Stop();
                            var ScanPopup = new BarcodePopup();
                            ScanPopup.tbMsg.Text = "저울 바코드를 스캔하세요.";
                            ScanPopup.Closed += async (sender, e) =>
                            {
                                if(ScanPopup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(ScanPopup.tbText.Text))
                                {
                                    string text = ScanPopup.tbText.Text;

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
                                        scalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                                        ScaleId = text;
                                        _DispatcherTimer.Start();
                                    }   
                                }
                                else
                                    ScaleId = "미설정";
                            };

                            ScanPopup.Show();
                            ///

                            CommandResults["ScanScaleCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _DispatcherTimer.Stop();
                            CommandResults["ScanScaleCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanScaleCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanScaleCommandAsync") ?
                        CommandCanExecutes["ScanScaleCommandAsync"] : (CommandCanExecutes["ScanScaleCommandAsync"] = true);
                });
            }
        }
        public ICommand ChargingCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ChargingCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ChargingCommandAsync"] = false;
                            CommandCanExecutes["ChargingCommandAsync"] = false;

                            ///
                            _DispatcherTimer.Stop();
                                                        
                            // 칭량범위를 넘어가지 않은 경우
                            if (_TotalWeight.Value <= _UpperWeight.Value)
                            {
                                // 칭량정보 저장여부 확인
                                if (await OnMessageAsync(string.Format("현재 칭량값은 {0} 입니다.\n저장하시겠습니까?", ScaleValue), true))
                                {
                                    selectedComponent.CHECK = "투입완료";
                                    selectedComponent.REALQTY = _ScaleValue.Value;
                                    selectedComponent.TOTALQTY = _TotalWeight.Value;

                                    // 기록버튼 활성화 여부
                                    if(_LowerWeight.Value <= _TotalWeight.Value && await OnMessageAsync("칭량을 종료하시겠습니까?", true))
                                    {
                                        IsEnabled = true;
                                        IsCharging = false;
                                        _DispatcherTimer.Stop();
                                        _ScaleValue.SetWeight(0, _ScaleValue.Uom, _scalePrecision);
                                        _TotalWeight.SetWeight(_selectedComponent.TOTALQTY + _ScaleValue.Value, _ScaleValue.Uom, _scalePrecision);
                                        OnPropertyChanged("TotalWeight");
                                        OnPropertyChanged("ScaleValue");
                                    }
                                    else
                                    {
                                        var temp = new BR_BRS_SEL_Charging_Solvent.OUTDATA
                                        {
                                            MTRLID = selectedComponent.MTRLID,
                                            MTRLNAME = selectedComponent.MTRLNAME,
                                            MLOTID = selectedComponent.MLOTID,
                                            MSUBLOTID = selectedComponent.MSUBLOTID,
                                            MSUBLOTQTY = selectedComponent.MSUBLOTQTY,
                                            UOMNAME = selectedComponent.UOMNAME,
                                            ACTID = selectedComponent.ACTID,
                                            COMPONENTGUID = selectedComponent.COMPONENTGUID,
                                            MSUBLOTBCD = selectedComponent.MSUBLOTBCD,
                                            CHECK = "투입대기",
                                            SEQ = (Convert.ToInt32(selectedComponent.SEQ) + 1).ToString(),
                                            CHGSEQ = selectedComponent.CHGSEQ,
                                            UPPERQTY = selectedComponent.UPPERQTY,
                                            LOWERQTY = selectedComponent.LOWERQTY,
                                            ISBCDSCAN = selectedComponent.ISBCDSCAN,
                                            REALQTY = 0m,
                                            TOTALQTY = selectedComponent.TOTALQTY
                                        };
                                        filteredComponents.Add(temp);
                                        selectedComponent = filteredComponents[filteredComponents.Count - 1];
                                        _DispatcherTimer.Start();
                                    }
                                    
                                }
                                else
                                {
                                    _DispatcherTimer.Start();
                                    selectedComponent.CHECK = "투입대기";
                                }    
                            }
                            else
                            {
                                _DispatcherTimer.Start();
                                throw new Exception("칭량 범위를 넘어갔습니다.");
                            }
                            ///

                            CommandResults["ChargingCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ChargingCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ChargingCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("ChargingCommandAsync") ?
                       CommandCanExecutes["ChargingCommandAsync"] : (CommandCanExecutes["ChargingCommandAsync"] = true);
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

                            _DispatcherTimer.Stop();

                            var authHelper = new iPharmAuthCommandHelper();
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

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
                                "원료(정제수) 투입",
                                "원료(정제수) 투입",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // XML schema
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("원료코드"));
                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("저울번호"));
                            dt.Columns.Add(new DataColumn("원료시험번호"));
                            dt.Columns.Add(new DataColumn("바코드"));
                            dt.Columns.Add(new DataColumn("하한"));
                            dt.Columns.Add(new DataColumn("기준"));
                            dt.Columns.Add(new DataColumn("상한"));
                            dt.Columns.Add(new DataColumn("무게"));

                            // 정제수 소분및투입
                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATAs.Clear();
                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA_INVs.Clear();

                            var lastWeighingInfo = filteredComponents[filteredComponents.Count - 1];
                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA()
                            {
                                INSUSER = string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging")) ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                MSUBLOTBCD = lastWeighingInfo.MSUBLOTBCD,
                                MSUBLOTQTY = lastWeighingInfo.TOTALQTY,
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                // IS_NEED_CHKWEIGHT = "Y",
                                IS_NEED_CHKWEIGHT = "N",
                                IS_FULL_CHARGE = "Y",
                                IS_CHECKONLY = "N",
                                IS_INVEN_CHARGE = "Y",
                                IS_OUTPUT = "N",
                                MSUBLOTID = lastWeighingInfo.MSUBLOTID,
                                CHECKINUSER = AuthRepositoryViewModel.GetSecondUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                            });

                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA_INVs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA_INV()
                            {
                                COMPONENTGUID = _BR_BRS_SEL_Charging_Solvent.OUTDATA_BOMs[0].COMPONENTGUID
                            });
                            
                            // XML 저장
                            if (await _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.Execute() == true)
                            {
                                //2021.10.27 박희돈 정제수 칭량 후 생성된 msublotid로 라벨 출력하도록 로직 추가.
                                _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.INDATAs.Add(new BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    COMPONENTGUID = _BR_BRS_SEL_Charging_Solvent.OUTDATA_BOMs[0].COMPONENTGUID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await _BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.Execute() == true)
                                {
                                    // 라벨 출력
                                    await LabelPrint(_BR_BRS_SEL_ProductionOrderDispenseSubLot_OPSG_COMPONENT.OUTDATAs[0].MSUBLOTID);
                                }else
                                {
                                    OnMessage("라벨 출력 오류");
                                }

                                var row = dt.NewRow();
                                row["원료코드"] = lastWeighingInfo.MTRLID != null ? lastWeighingInfo.MTRLID : "";
                                row["원료명"] = lastWeighingInfo.MTRLNAME != null ? lastWeighingInfo.MTRLNAME : "";
                                row["저울번호"] = _ScaleInfo.EQPTID;
                                row["원료시험번호"] = lastWeighingInfo.MSUBLOTID != null ? lastWeighingInfo.MSUBLOTID : "";
                                row["바코드"] = lastWeighingInfo.MSUBLOTBCD != null ? lastWeighingInfo.MSUBLOTBCD : "";
                                row["하한"] = _LowerWeight.WeightUOMString;
                                row["기준"] = _stdQty;
                                row["상한"] = _UpperWeight.WeightUOMString;
                                row["무게"] = string.Format(("{0:N" + _scalePrecision + "}"), lastWeighingInfo.TOTALQTY) + _TotalWeight.Uom;
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
        #endregion
        #region [Custom]
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();
                
                if(_ScaleInfo != null && _selectedComponent != null)
                {
                    bool success = false;

                    decimal weight;
                    if(_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);
                               
                        if (result.code == "1" && Decimal.TryParse(result.data, out weight) == true)
                        {
                            success = true;
                            _ScaleValue.SetWeight(result.data, result.unit);
                            _TotalWeight.SetWeight(_selectedComponent.TOTALQTY + _ScaleValue.Value, _ScaleValue.Uom, _ScaleValue.Precision);
                            scalePrecision = _ScaleValue.Precision;
                        }
                    }
                    else
                    {
                        BR_BRS_SEL_CurrentWeight current_wight = new BR_BRS_SEL_CurrentWeight();
                        current_wight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA()
                        {
                            ScaleID = _ScaleInfo.EQPTID
                        });

                        if (await current_wight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) == true
                            && current_wight.OUTDATAs.Count > 0 && current_wight.OUTDATAs[0].Weight.HasValue)
                        {
                            success = true;
                            _ScaleValue.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, _scalePrecision);
                            _TotalWeight.SetWeight(_selectedComponent.TOTALQTY + _ScaleValue.Value, _ScaleValue.Uom, _scalePrecision);
                        }
                    }

                    // 저울 연결 시 투입버튼 활성화
                    if (success)
                    {
                        IsCharging = true;
                        _ScaleException = false;
                        decimal total = _ScaleValue.Value + selectedComponent.TOTALQTY;
                        if (_LowerWeight.Value <= _TotalWeight.Value && _TotalWeight.Value <= _UpperWeight.Value)
                            ScaleBackground = new SolidColorBrush(Colors.Green);
                        else
                            ScaleBackground = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        IsCharging = false;
                        _ScaleException = true;
                        _ScaleValue.SetWeight(0, _ScaleValue.Uom, _scalePrecision);
                        _TotalWeight.SetWeight(_selectedComponent.TOTALQTY + _ScaleValue.Value, _ScaleValue.Uom, _scalePrecision);
                        ScaleBackground = new SolidColorBrush(Colors.Red);
                    }
                       
                    OnPropertyChanged("ScaleValue");
                    OnPropertyChanged("TotalWeight");
                    OnPropertyChanged("UpperWeight");
                    OnPropertyChanged("LowerWeight");
                    _DispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                IsCharging = false;
                OnException(ex.Message, ex);
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

        /// <summary>
        /// 라벨발행
        /// </summary>
        /// <param name="msublotid"></param>
        private async Task LabelPrint(string msublotid)
        {
            try
            {
                if (_selectedPrint != null)
                {
                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                    {
                        ReportPath = "/Reports/Label/LABEL_WEIGHING_SOLUTION_SUI",
                        PrintName = _selectedPrint.PRINTERNAME,
                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                    });
                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                    {
                        ParamName = "MSUBLOTID",
                        ParamValue = msublotid
                    });

                    await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                }

                return;
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
                return;
            }
        }
        #endregion
    }

}
