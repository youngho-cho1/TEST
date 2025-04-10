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
using 보령.UserControls;

namespace 보령
{
    public class SVP원료칭량및투입ViewModel : ViewModelBase
    {
        #region [Property]
        public SVP원료칭량및투입ViewModel()
        {
            _BR_BRS_SEL_Charging_Solvent_to_Dispense = new BR_BRS_SEL_Charging_Solvent_to_Dispense();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            _filteredComponents = new BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATACollection();
            _BR_BRS_REG_Dispense_Charging_Solvent = new BR_BRS_REG_Dispense_Charging_Solvent();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
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
        enum State
        {
            SetTare,
            Weighing
        };
        private State _curState = State.SetTare;
        SVP원료칭량및투입 _mainWnd;
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
        #region [자재정보]
        string _mtrlId;
        public string MtrlId
        {
            get { return _mtrlId; }
            set
            {
                _mtrlId = value;
                OnPropertyChanged("MtrlId");
            }
        }

        string _mtrlName;
        public string MtrlName
        {
            get { return _mtrlName; }
            set
            {
                _mtrlName = value;
                OnPropertyChanged("MtrlName");
            }
        }

        string _stdQty;
        public string StdQty
        {
            get { return _stdQty; }
            set
            {
                _stdQty = value;
                OnPropertyChanged("StdQty");
            }
        }
        BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATACollection _filteredComponents;
        /// <summary>
        /// 원료백 정보
        /// </summary>
        public BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set
            {
                _filteredComponents = value;
                OnPropertyChanged("FilteredComponents");
            }
        }
        private BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATA _curSeletedItem;
        /// <summary>
        /// 현재 칭량에 사용중인 원료백
        /// </summary>
        public BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATA curSeletedItem
        {
            get { return _curSeletedItem; }
            set
            {
                _curSeletedItem = value;
                OnPropertyChanged("curSeletedItem");
            }
        }
        #endregion
        #region [저울정보]
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
        private int _scalePrecision = 3;
        public int scalePrecision
        {
            set
            {
                _scalePrecision = value;
                _LowerWeight.Precision = _scalePrecision;
                _UpperWeight.Precision = _scalePrecision;
                OnPropertyChanged("ScaleWeight");
                OnPropertyChanged("UpperWeight");
                OnPropertyChanged("LowerWeight");
                OnPropertyChanged("TotalWeight");
            }
        }
        // 저울에러
        private bool _ScaleException;
        private string _ScaleExceptionMsg = "저울 연결 실패";
        string _scaleId;
        public string ScaleId
        {
            get { return _scaleId; }
            set
            {
                _scaleId = value;
                NotifyPropertyChanged();
            }
        }
        private Weight _ScaleWeight = new Weight();
        public string ScaleWeight
        {
            get
            {
                if (_ScaleException)
                    return _ScaleExceptionMsg;
                else
                    return _ScaleWeight.WeightUOMString;
            }
        }
        private Weight _TareWeight = new Weight();
        public string TareWeight
        {
            get
            {
                if (_ScaleException)
                    return _ScaleExceptionMsg;
                else
                    return _TareWeight.WeightUOMString;
            }
        }
        /// <summary>
        /// 지금까지 투입된 량(현재 선택된 원료의 투입량을 계산하기 위해 사용)
        /// </summary>
        private Weight _prevChagingWeight = new Weight();
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
        private bool _CANCHARGEFLAG;
        /// <summary>
        /// 투입버튼
        /// </summary>
        public bool CANCHARGEFLAG
        {
            get { return _CANCHARGEFLAG; }
            set
            {
                _CANCHARGEFLAG = value;
                OnPropertyChanged("CANCHARGEFLAG");
            }
        }
        private bool _CANRECORDFLAG;
        /// <summary>
        /// 기록버튼
        /// </summary>
        public bool CANRECORDFLAG
        {
            get { return _CANRECORDFLAG; }
            set
            {
                _CANRECORDFLAG = value;
                NotifyPropertyChanged();
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
        #endregion
        #endregion
        #region [BizRule]
        BR_BRS_SEL_Charging_Solvent_to_Dispense _BR_BRS_SEL_Charging_Solvent_to_Dispense;
        /// <summary>
        /// 용매 조회 비즈룰
        /// </summary>
        public BR_BRS_SEL_Charging_Solvent_to_Dispense BR_BRS_SEL_Charging_Solvent_to_Dispense

        {
            get { return _BR_BRS_SEL_Charging_Solvent_to_Dispense; }
            set
            {
                _BR_BRS_SEL_Charging_Solvent_to_Dispense = value;
                OnPropertyChanged("BR_BRS_SEL_Charging_Solvent_to_Dispense");
            }
        }
        /// <summary>
        /// 용매 소분 후 투입 비즈룰
        /// </summary>
        private BR_BRS_REG_Dispense_Charging_Solvent _BR_BRS_REG_Dispense_Charging_Solvent;
        /// <summary>
        /// 저울조회 비즈룰
        /// </summary>
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        /// <summary>
        /// 저울 Tare 비즈룰
        /// </summary>
        BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;
        BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
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
                            if (arg != null && arg is SVP원료칭량및투입)
                            {
                                _mainWnd = arg as SVP원료칭량및투입;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                _DspStartDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                string BomId = _mainWnd.CurrentInstruction.Raw.BOMID;
                                string ChgSeq = _mainWnd.CurrentInstruction.Raw.EXPRESSION;
                                ScaleId = _mainWnd.CurrentInstruction.Raw.EQPTID;
                                CANRECORDFLAG = false;
                                CANCHARGEFLAG = false;

                                // 지시문 Validation
                                if (string.IsNullOrWhiteSpace(BomId))
                                    throw new Exception(string.Format("해당 Instruction에 BOM ID가 설정되지 않았습니다."));
                                else if (string.IsNullOrWhiteSpace(ChgSeq))
                                    throw new Exception(string.Format("해당 Instruction에 Chg Seq가 설정되지 않았습니다."));

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
                                    ScaleId = "";

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

                                // 칭량 자재정보 조회
                                _BR_BRS_SEL_Charging_Solvent_to_Dispense.INDATAs.Add(new BR_BRS_SEL_Charging_Solvent_to_Dispense.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                    MTRLID = BomId,
                                    CHGSEQ = ChgSeq
                                });

                                if (await _BR_BRS_SEL_Charging_Solvent_to_Dispense.Execute() == true)
                                {
                                    //BOM 기준정보
                                    if (_BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATA_BOMs.Count > 0)
                                    {
                                        var item = _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATA_BOMs.Where(o => o.MTRLID == BomId && o.CHGSEQ == ChgSeq).FirstOrDefault();

                                        MtrlId = item.MTRLID;
                                        MtrlName = item.MTRLNAME;
                                        StdQty = item.STDQTY + " " + item.NOTATION;
                                    }

                                    // 자재목록
                                    if (_BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs.Count > 0)
                                    {
                                        foreach (var outdata in _BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs.Where(o => o.MTRLID == BomId && o.CHGSEQ == ChgSeq).ToList())
                                        {
                                            outdata.CHECK = "투입대기";
                                            outdata.INVQTY.SetWeight(outdata.MSUBLOTQTY.GetValueOrDefault(), outdata.UOMNAME, _scalePrecision);
                                            outdata.CHGQTY.SetWeight(0, outdata.UOMNAME, _scalePrecision);
                                            _filteredComponents.Add(outdata);
                                        }

                                        if (_filteredComponents[0].UPPERQTY.HasValue && _filteredComponents[0].LOWERQTY.HasValue)
                                        {
                                            _UpperWeight.SetWeight(_filteredComponents[0].UPPERQTY.Value, _filteredComponents[0].UOMNAME, _scalePrecision);
                                            _LowerWeight.SetWeight(_filteredComponents[0].LOWERQTY.Value, _filteredComponents[0].UOMNAME, _scalePrecision);
                                            OnPropertyChanged("UpperWeight"); OnPropertyChanged("LowerWeight");
                                            ScaleBackground = new SolidColorBrush(Colors.Gray);
                                        }
                                        else
                                            throw new Exception("칭량 범위가 설정되지 않았습니다.");
                                    }
                                    else
                                        throw new Exception("조회된 원료백 정보가 없습니다.");
                                }
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
        public ICommand ScanMtrlCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanMtrlCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ScanMtrlCommand"] = false;
                            CommandCanExecutes["ScanMtrlCommand"] = false;

                            ///
                            IsBusy = true;
                            _DispatcherTimer.Stop();

                            bool popupflag = false;
                            if (_ScaleInfo == null)
                                throw new Exception("저울정보가 없습니다.");

                            if (_curState == State.SetTare)
                            {                                
                                if (_ScaleInfo.TARE_MANDATORY == "Y")
                                    OnMessage("TARE 필수입니다.");
                                else
                                {
                                    _curState = State.Weighing;
                                    popupflag = true;
                                }
                            }                                
                            else
                                popupflag = true;

                            if (popupflag)
                            {
                                BarcodePopup popup = new BarcodePopup();
                                popup.tbMsg.Text = "원료바코드를 스캔하세요.";
                                popup.Closed += async (sender, e) =>
                                {
                                    if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                                    {
                                        string text = popup.tbText.Text.ToUpper();

                                        if (_filteredComponents.Count > 0)
                                        {
                                            var item = _filteredComponents.Where(o => o.MSUBLOTBCD == text).FirstOrDefault();
                                            bool changeSelectedCompoent = false;

                                            if (item != null)
                                            {
                                                if (curSeletedItem == null)
                                                {
                                                    if (item.CHECK == "투입완료")
                                                    {
                                                        if (await OnMessageAsync(string.Format("투입량을 변경하시겠습니까?\n현재투입량 : [{0}]", item.CHGQTY.WeightUOMString), true))
                                                        {
                                                            _prevChagingWeight = _prevChagingWeight.Subtract(item.CHGQTY);
                                                            changeSelectedCompoent = true;
                                                        }
                                                    }
                                                    else
                                                        changeSelectedCompoent = true;

                                                    if (changeSelectedCompoent)
                                                    {
                                                        item.CHECK = "투입가능";
                                                        curSeletedItem = item;
                                                        _curState = State.Weighing;
                                                        _DispatcherTimer.Start();
                                                    }
                                                }
                                                else
                                                {
                                                    if (item.CHECK == "투입완료")
                                                    {
                                                        if (await OnMessageAsync(string.Format("투입량을 변경하시겠습니까?\n현재투입량 : [{0}]", item.CHGQTY.WeightUOMString), true))
                                                        {
                                                            _prevChagingWeight = _prevChagingWeight.Subtract(item.CHGQTY);
                                                            changeSelectedCompoent = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (await OnMessageAsync("투입 원료백을 변경하시겠습니까?", true))
                                                            changeSelectedCompoent = true;
                                                    }

                                                    if (changeSelectedCompoent)
                                                    {
                                                        curSeletedItem.CHECK = curSeletedItem.CHECK == "투입가능" ? "투입대기" : curSeletedItem.CHECK;
                                                        item.CHECK = "투입가능";
                                                        curSeletedItem = item;
                                                        _curState = State.Weighing;
                                                        _DispatcherTimer.Start();
                                                    }
                                                }
                                            }
                                            else
                                                OnMessage("원료정보가 일치하지 않습니다.");
                                        }
                                        else
                                            OnMessage("조회된 원료정보가 없습니다.");
                                    }
                                    else
                                        curSeletedItem = null;

                                    _DispatcherTimer.Start();
                                };

                                popup.Show();

                            }

                            IsBusy = false;
                            ///

                            CommandResults["ScanMtrlCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            _DispatcherTimer.Start();
                            IsBusy = false;
                            CommandResults["ScanMtrlCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanMtrlCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanMtrlCommand") ?
                        CommandCanExecutes["ScanMtrlCommand"] : (CommandCanExecutes["ScanMtrlCommand"] = true);
                });
            }
        }
        public ICommand ScanScaleCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanScaleCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ScanScaleCommand"] = false;
                            CommandCanExecutes["ScanScaleCommand"] = false;

                            ///
                            IsBusy = true;
                            _DispatcherTimer.Stop();

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
                                        scalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                                        ScaleId = text;
                                        _DispatcherTimer.Start();
                                    }
                                }
                                else
                                    ScaleId = "";
                            };

                            popup.Show();

                            IsBusy = false;
                            ///

                            CommandResults["ScanScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ScanScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanScaleCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanScaleCommand") ?
                        CommandCanExecutes["ScanScaleCommand"] : (CommandCanExecutes["ScanScaleCommand"] = true);
                });
            }
        }
        public ICommand SetBeakerTareCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SetBeakerTareCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["SetBeakerTareCommand"] = false;
                            CommandCanExecutes["SetBeakerTareCommand"] = false;

                            ///
                            IsBusy = true;

                            _DispatcherTimer.Stop();
                            if (_curState == State.SetTare)
                            {
                                if (_curState == State.SetTare)
                                    _TareWeight = _ScaleWeight.Copy();

                                if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                                {
                                    var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.ST);

                                    if (result.code != "0")
                                        _curState = State.Weighing;
                                }
                                else
                                {
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA
                                    {
                                        ScaleID = ScaleId
                                    });

                                    if (await _BR_PHR_REG_ScaleSetTare.Execute())
                                        _curState = State.Weighing;
                                }

                                if (_curState != State.Weighing)
                                    _TareWeight.Value = 0m;
                            }

                            _DispatcherTimer.Start();

                            IsBusy = false;
                            ///

                            CommandResults["SetBeakerTareCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["SetBeakerTareCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SetBeakerTareCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SetBeakerTareCommand") ?
                        CommandCanExecutes["SetBeakerTareCommand"] : (CommandCanExecutes["SetBeakerTareCommand"] = true);
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

                            if (_curState == State.Weighing && _curSeletedItem != null && _curSeletedItem.CHECK == "투입가능")
                            {
                                Weight chargingqty = _ScaleWeight.Subtract(_prevChagingWeight);

                                if (await OnMessageAsync(string.Format("투입량 : [{0}]", chargingqty.WeightUOMString), true))
                                {
                                    _prevChagingWeight.SetWeight(_ScaleWeight.Value, _ScaleWeight.Uom, _ScaleWeight.Precision);
                                    curSeletedItem.CHGQTY = chargingqty;
                                    curSeletedItem.CHECK = "투입완료";
                                    curSeletedItem = null;

                                    if (_ScaleWeight.Subtract(_LowerWeight).Value >= 0 && _ScaleWeight.Subtract(_UpperWeight).Value <= 0)
                                    {
                                        CANRECORDFLAG = true;
                                        return;
                                    }
                                }
                            }

                            _DispatcherTimer.Start();
                            ///

                            CommandResults["ChargingCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ChargingCommandAsync"] = false;
                            _DispatcherTimer.Start();
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
                            //"칭량을 완료하시겠습니까?\n현재 칭량 중인 원료가 보충에 사용되는 원료인 경우 취소를 선택하세요."
                            bool WeighingCompleteflag = await OnMessageAsync(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "BRS0001").Replace("\\n", "\n"), true);

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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "원료(주사제) 투입",
                                "원료(주사제) 투입",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("자재ID"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("원료배치번호"));
                            dt.Columns.Add(new DataColumn("바코드"));
                            dt.Columns.Add(new DataColumn("무게"));
                            dt.Columns.Add(new DataColumn("단위"));

                            var dttm = AuthRepositoryViewModel.GetDateTimeByFunctionCode("OM_ProductionOrder_Charging");
                            DateTime chargingdttm = dttm.HasValue ? dttm.Value : await AuthRepositoryViewModel.GetDBDateTimeNow();
                            string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging");

                            _BR_BRS_REG_Dispense_Charging_Solvent.INDATAs.Clear();
                            foreach (var item in FilteredComponents)
                            {
                                if (item.CHECK == "투입완료")
                                {
                                    _BR_BRS_REG_Dispense_Charging_Solvent.INDATAs.Add(new BR_BRS_REG_Dispense_Charging_Solvent.INDATA
                                    {
                                        LANGID = "ko-KR",
                                        MSUBLOTID = item.MSUBLOTID ?? "",
                                        MSUBLOTBCD = item.MSUBLOTBCD ?? "",
                                        INSDTTM = chargingdttm,
                                        INSUSER = user,
                                        DEPLETFLAG = "P",
                                        VESSELID = "",
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        MSUBLOTTYPE = "DSP",
                                        COMPONENTGUID = item.COMPONENTGUID != null ? item.COMPONENTGUID : "",
                                        TARE = _TareWeight.Value,
                                        LOCATIONID = AuthRepositoryViewModel.Instance.RoomID,
                                        INVENTORYQTY = item.MSUBLOTQTY,
                                        DISPENSEQTY = item.CHGQTY.Value,
                                        ISDISPSTRT = "Y",
                                        ACTID = "Dispensing",
                                        CHECKINUSER = user,
                                        CHECKINDTTM = chargingdttm,
                                        WEIGHINGMETHOD = "WH007",
                                        UPPERVALUE = _UpperWeight.Value,
                                        LOWERVALUE = _LowerWeight.Value,
                                        LOTTYPE = string.Empty,
                                        OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                        TAREWEIGHT = _TareWeight.Value,
                                        TAREUOMID = item.UOMNAME,
                                        REASON = "현장칭량",
                                        SCALEID = this.ScaleId,
                                        INSSIGNATURE = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                                        DSPSTRTDTTM = _DspStartDttm.ToString("yyyy-MM-dd HH:mm:ss"),
                                        DSPENDDTTM = chargingdttm.ToString("yyyy-MM-dd HH:mm:ss"),
                                        WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID,
                                        SCALEPRECISION = _scalePrecision
                                    });

                                    var row = dt.NewRow();
                                    row["자재ID"] = item.MTRLID ?? "";
                                    row["자재명"] = item.MTRLNAME ?? "";
                                    row["원료배치번호"] = item.MSUBLOTID ?? "";
                                    row["바코드"] = item.MSUBLOTBCD ?? "";
                                    row["무게"] = item.CHGQTY.WeightString;
                                    row["단위"] = item.CHGQTY.Uom;
                                    dt.Rows.Add(row);
                                }
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            // 원료보충하는 Component는 마지막이력을 C로 보내지 않도록 수정
                            if (WeighingCompleteflag && _BR_BRS_REG_Dispense_Charging_Solvent.INDATAs.Count > 0)
                                _BR_BRS_REG_Dispense_Charging_Solvent.INDATAs[_BR_BRS_REG_Dispense_Charging_Solvent.INDATAs.Count - 1].ISDISPSTRT = "C";

                            if (await _BR_BRS_REG_Dispense_Charging_Solvent.Execute() == true)
                            {
                                if (_BR_BRS_REG_Dispense_Charging_Solvent.OUTDATAs.Count > 0 && curPrintName != "N/A")
                                {
                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                    {
                                        ReportPath = "/Reports/Label/LABEL_WEIGHING_REMAIN",
                                        PrintName = _selectedPrint.PRINTERNAME,
                                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                    });
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                    {
                                        ParamName = "MSUBLOTID",
                                        ParamValue = _BR_BRS_REG_Dispense_Charging_Solvent.OUTDATAs[0].DSPMSUBLOTID
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
        #endregion
        #region [Custom]
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (_ScaleInfo != null)
                {
                    bool success = false;
                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            _ScaleWeight.SetWeight(result.data, result.unit);                            
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
                            _ScaleWeight.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, _scalePrecision);
                        }
                    }

                    if (success)
                    {
                        CANCHARGEFLAG = true;
                        _ScaleException = false;
                        if (_LowerWeight.Value <= _ScaleWeight.Value && _ScaleWeight.Value <= _UpperWeight.Value)
                            ScaleBackground = new SolidColorBrush(Colors.Green);
                        else
                            ScaleBackground = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        CANCHARGEFLAG = false;
                        _ScaleException = true;
                        _ScaleWeight.SetWeight(0, _ScaleWeight.Uom, _scalePrecision);
                        ScaleBackground = new SolidColorBrush(Colors.Red);
                    }

                    OnPropertyChanged("ScaleWeight");
                    OnPropertyChanged("TareWeight");
                    OnPropertyChanged("TotalWeight");
                    OnPropertyChanged("UpperWeight");
                    OnPropertyChanged("LowerWeight");
                    _DispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                _DispatcherTimer.Stop();
                OnException(ex.Message, ex);
            }
        }
        #endregion
    }
}
