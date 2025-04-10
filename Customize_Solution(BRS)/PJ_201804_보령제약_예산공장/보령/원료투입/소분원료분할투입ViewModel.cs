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
using System.Windows.Threading;
using System.Linq;
using 보령.UserControls;
using C1.Silverlight.Data;
using ShopFloorUI;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace 보령
{
    public class 소분원료분할투입ViewModel : ViewModelBase
    {
        #region [Property]
        public 소분원료분할투입ViewModel()
        {
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            
            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW = new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW();

            _BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary();
            _filteredComponents = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection();
            _BR_BRS_REG_ProductionOrder_Dispence_Split_Charging = new BR_BRS_REG_ProductionOrder_Dispence_Split_Charging();
            _BR_BRS_SEL_Charging_Split_Dispence_History = new BR_BRS_SEL_Charging_Split_Dispence_History();

            int interval = 2000;
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }
            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
        }

        소분원료분할투입 _mainWnd;
        public 소분원료분할투입 MainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
        }

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

        public bool _YeldFlag = false;
        public string _Text = string.Empty;

        #region [자재정보]
        string _MTRLID;
        public string MTRLID
        {
            get { return _MTRLID; }
            set
            {
                _MTRLID = value;
                OnPropertyChanged("MTRLID");
            }
        }

        string _MTRLNAME;
        public string MTRLNAME
        {
            get { return _MTRLNAME; }
            set
            {
                _MTRLNAME = value;
                OnPropertyChanged("MTRLNAME");
            }
        }

        string _RESERVEQTY;
        public string RESERVEQTY
        {
            get { return _RESERVEQTY; }
            set
            {
                _RESERVEQTY = value;
                OnPropertyChanged("RESERVEQTY");
            }
        }

        string _YIELD;
        public string YIELD
        {
            get { return _YIELD; }
            set
            {
                _YIELD = value;
                OnPropertyChanged("YIELD");
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

        private BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATA _curSeletedSourceContainer;
        /// <summary>
        /// 현재 칭량에 사용중인 원료(칭량에서 소분한 소분 정보)
        /// </summary>
        public BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATA curSeletedSourceContainer
        {
            get { return _curSeletedSourceContainer; }
            set
            {
                _curSeletedSourceContainer = value;
                OnPropertyChanged("curSeletedSourceContainer");
            }
        }
        
        private bool _MtrlbtnEnable;
        /// <summary>
        /// 원료바코드 스캔 버튼 Enable
        /// </summary>
        public bool MtrlbtnEnable
        {
            get { return _MtrlbtnEnable; }
            set
            {
                _MtrlbtnEnable = value;
                OnPropertyChanged("MtrlbtnEnable");
            }
        }

        bool _is_EnableOKBtn;
        public bool Is_EnableOKBtn
        {
            get { return _is_EnableOKBtn; }
            set
            {
                _is_EnableOKBtn = value;
                NotifyPropertyChanged();
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
                OnPropertyChanged("DspWeight");
                OnPropertyChanged("UpperWeight");
                OnPropertyChanged("LowerWeight");
            }
        }

        // 저울에러
        /// <summary>
        /// True : TARE 실시, False : TARE 미실시
        /// </summary>
        private bool _SetTare;
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
        public string DspWeight
        {
            get
            {
                if (_ScaleException)
                    return _ScaleExceptionMsg;
                else
                    return _ScaleWeight.Add(_DisepenQty).WeightUOMString;
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

        private bool _ScalebtnEnable;
        /// <summary>
        /// 저울스캔 Enable
        /// </summary>
        public bool ScalebtnEnable
        {
            get { return _ScalebtnEnable; }
            set
            {
                _ScalebtnEnable = value;
                OnPropertyChanged("ScalebtnEnable");
            }
        }

        private bool _TarebtnEnable;
        /// <summary>
        /// Tare버튼 Enable
        /// </summary>
        public bool TarebtnEnable
        {
            get { return _TarebtnEnable; }
            set
            {
                _TarebtnEnable = value;
                OnPropertyChanged("TarebtnEnable");
            }
        }
        #endregion

        #region [칭량정보]

        private Weight _DisepenQty = new Weight();
        
        private bool _ChargebtnEnable;
        /// <summary>
        /// 투입버튼
        /// </summary>
        public bool ChargebtnEnable
        {
            get { return _ChargebtnEnable; }
            set
            {
                _ChargebtnEnable = value;
                OnPropertyChanged("ChargebtnEnable");
            }
        }

        private bool _AllChargebtnEnable;
        /// <summary>
        /// 전량투입버튼
        /// </summary>
        public bool AllChargebtnEnable
        {
            get { return _AllChargebtnEnable; }
            set
            {
                _AllChargebtnEnable = value;
                OnPropertyChanged("AllChargebtnEnable");
            }
        }
        
        #endregion

        #endregion

        #region [BizRule]
        /// <summary>
        /// 저울 조회
        /// </summary>
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        /// <summary>
        /// 저울 Tare
        /// </summary>
        BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;
        /// <summary>
        /// 원료투입 비즈룰
        /// </summary>
        private BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW;

        /// <summary>
        /// 소분원료 조회 비즈룰
        /// </summary>
        BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary _BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary;
        public BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary
        {
            get { return _BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary; }
            set {
                    _BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary = value;
                    NotifyPropertyChanged();
                }
        }
        /// <summary>
        /// 분할 후 투입 비즈룰
        /// </summary>
        BR_BRS_REG_ProductionOrder_Dispence_Split_Charging _BR_BRS_REG_ProductionOrder_Dispence_Split_Charging;
        BR_BRS_REG_ProductionOrder_Dispence_Split_Charging BR_BRS_REG_ProductionOrder_Dispence_Split_Charging
        {
            get { return _BR_BRS_REG_ProductionOrder_Dispence_Split_Charging; }
            set { _BR_BRS_REG_ProductionOrder_Dispence_Split_Charging = value; }
        }
        /// <summary>
        /// 소분정보
        /// </summary>
        BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection filteredComponents
        {
            get { return _filteredComponents; }
            set {
                    _filteredComponents = value;
                    OnPropertyChanged("filteredComponents");
                }
        }
        
        /// <summary>
        /// 투입정보 조회
        /// </summary>
        BR_BRS_SEL_Charging_Split_Dispence_History _BR_BRS_SEL_Charging_Split_Dispence_History;
        public BR_BRS_SEL_Charging_Split_Dispence_History BR_BRS_SEL_Charging_Split_Dispence_History
        {
            get { return _BR_BRS_SEL_Charging_Split_Dispence_History; }
            set
            {
                _BR_BRS_SEL_Charging_Split_Dispence_History = value;
            }
        }

        List<string> _currentBOMID;

        string _batchNo;
        public string BatchNo
        {
            get { return _batchNo; }
            set
            {
                _batchNo = value;
                NotifyPropertyChanged();
            }
        }
        string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                NotifyPropertyChanged();
            }
        }

        string _ProcessSegmentName;
        public string ProcessSegmentName
        {
            get { return _ProcessSegmentName; }
            set
            {
                _ProcessSegmentName = value;
                NotifyPropertyChanged();
            }
        }
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
                            if (arg != null && arg is 소분원료분할투입)
                            {
                                _mainWnd = arg as 소분원료분할투입;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                _currentBOMID = new List<string>();
                                var paramInsts = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                                decimal tempEXPRESSION;

                                this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                                this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                                this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;

                                filteredComponents.Clear();

                                if (decimal.TryParse(_mainWnd.CurrentInstruction.Raw.EXPRESSION, out tempEXPRESSION))
                                {
                                    BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.INDATA()
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        CHGSEQ = tempEXPRESSION,
                                        MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID
                                    });
                                }

                                foreach (var instruction in paramInsts)
                                {
                                    if (decimal.TryParse(instruction.Raw.EXPRESSION, out tempEXPRESSION))
                                    {
                                        BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.INDATA()
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            CHGSEQ = decimal.Parse(instruction.Raw.EXPRESSION),
                                            MTRLID = instruction.Raw.BOMID
                                        });
                                    }
                                }

                                if (await BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.Execute() == false) return;

                                foreach (var outdata in BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs)
                                {
                                    if(outdata.YIELD >= 95)
                                    {
                                        _YeldFlag = true;

                                        MtrlbtnEnable = true;
                                        AllChargebtnEnable = true;
                                        ChargebtnEnable = false;
                                        ScalebtnEnable = false;
                                        TarebtnEnable = false;
                                    }
                                    else
                                    {
                                        _YeldFlag = false;

                                        MtrlbtnEnable = false;
                                        AllChargebtnEnable = false;
                                        ChargebtnEnable = true;
                                        ScalebtnEnable = true;
                                        TarebtnEnable = true;
                                    }

                                    if (string.IsNullOrWhiteSpace(outdata.MSUBLOTBCD))
                                        outdata.IS_CAN_CHARGING_CHECKED_NAME = "소분안됨";
                                    else
                                        outdata.IS_CAN_CHARGING_CHECKED_NAME = (Convert.ToDecimal(outdata.REMAINQTY) <= 0) ? "투입완료" : "투입대기";

                                    YIELD = string.Format("{0:0.0}", outdata.YIELD);

                                    filteredComponents.Add(outdata);
                                }
                                
                                // 저울 설정
                                ScaleId = _mainWnd.CurrentInstruction.Raw.EQPTID;
                                if (string.IsNullOrWhiteSpace(ScaleId) || !await GetScaleInfo(ScaleId))
                                    ScaleId = "";

                                await GetDispenseHistory();
                            }

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

                            if(!_YeldFlag)
                            {
                                // Tare 확인 메세지
                                if (!_SetTare && !await OnMessageAsync("Tare가 0입니다. 진행하시겠습니까?", true))
                                {
                                    return;
                                }
                                else
                                {
                                    if (!_SetTare)
                                    {
                                        _TareWeight = _ScaleWeight.Copy();
                                        _TareWeight.Value = 0;
                                        OnPropertyChanged("TareWeight");
                                    }
                                }
                            }
                            
                            if(_YeldFlag)
                            {
                                var notCompletedItem = filteredComponents.Where(o =>
                              o.IS_CAN_CHARGING_CHECKED_NAME == "투입대기").FirstOrDefault();

                                if (notCompletedItem != null)
                                {
                                    var viewmodel = new 소분원료분할투입팝업ViewModel()
                                    {
                                        ParentVM = this,
                                    };
                                    var pop = new 소분원료분할투입팝업()
                                    {
                                        DataContext = viewmodel
                                    };
                                    pop.Closed += (s, e) =>
                                    {
                                        //2023.10.01 박희돈 투입가능 text가 Refresh 안됨. 스크롤을 움직여야 보여짐. 그래서 새로 바인딩 함.
                                        //filteredComponents = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection();
                                        //filteredComponents = BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs;

                                        filteredComponents.NotifyOfPropertyChange("filteredComponents");
                                    };

                                    pop.Show();
                                }
                                else
                                {
                                    OnMessage("투입할 원료가 없습니다.");
                                }
                            }
                            else
                            {
                                BarcodePopup popup = new BarcodePopup();
                                popup.tbMsg.Text = "원료바코드를 스캔하세요.";
                                popup.Closed += (sender, e) =>
                                {
                                    if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                                    {
                                        _Text = popup.tbText.Text.ToUpper();

                                        if (filteredComponents.Count > 0)
                                        {
                                            var select = filteredComponents.Where(o => o.MSUBLOTBCD == _Text).FirstOrDefault();

                                            if (curSeletedSourceContainer != null)
                                            {
                                                if (curSeletedSourceContainer.MSUBLOTBCD == _Text)
                                                {
                                                    OnMessage("현재 칭량중인 원료입니다.");
                                                    curSeletedSourceContainer.UsedWeight = 0;
                                                    _DispatcherTimer.Start();
                                                    return;
                                                }
                                            }

                                            if (select != null)
                                            {
                                                if (select.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입완료"))
                                                {
                                                    OnMessage("이미 사용한 소분원료입니다.");
                                                    if (curSeletedSourceContainer != null)
                                                        curSeletedSourceContainer.UsedWeight = 0;
                                                    _DispatcherTimer.Start();
                                                    return;
                                                }

                                                //if (!_YeldFlag)
                                                //{
                                                    // 원료 변경
                                                    // 2023.10.01 박희돈 전량투입은 투입대기로 바뀌면 안됨. 95프로 미만 수율에만 변경 함.
                                                    select.IsSelected = true;
                                                    foreach (var item in filteredComponents)
                                                    {
                                                        if (item.MSUBLOTBCD != select.MSUBLOTBCD)
                                                            item.IsSelected = false;

                                                        if (Convert.ToDecimal(item.UsedWeight) == 0)
                                                        {
                                                            item.IS_CAN_CHARGING_CHECKED_NAME = "투입대기";
                                                        }
                                                    }

                                                    _UpperWeight.SetWeight(select.UPPER.GetValueOrDefault().ToString("F" + _ScaleInfo.PRECISION), select.UOM);
                                                    _LowerWeight.SetWeight(select.LOWER.GetValueOrDefault().ToString("F" + _ScaleInfo.PRECISION), select.UOM);
                                                    OnPropertyChanged("UpperWeight");
                                                    OnPropertyChanged("LowerWeight");
                                                //}

                                                MTRLID = select.MTRLID;
                                                MTRLNAME = select.MTRLNAME;
                                                //RESERVEQTY = select.REMAINQTY;
                                                YIELD = string.Format("{0:0.0}", select.YIELD);
                                                select.IS_CAN_CHARGING_CHECKED_NAME = "투입가능";

                                                //2023.10.01 박희돈 투입가능 text가 Refresh 안됨. 스크롤을 움직여야 보여짐. 그래서 새로 바인딩 함.
                                                //filteredComponents = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection();
                                                //filteredComponents = BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs;
                                                
                                                curSeletedSourceContainer = select;

                                                TarebtnEnable = false;
                                                MtrlbtnEnable = false;

                                                OnPropertyChanged("filteredComponents");

                                                _mainWnd.dgSourceContainer.Refresh();

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (curSeletedSourceContainer != null)
                                        {
                                            curSeletedSourceContainer.IsSelected = false;
                                            curSeletedSourceContainer = null;
                                        }
                                        OnPropertyChanged("filteredComponents");
                                    }

                                    _DispatcherTimer.Start();
                                };

                                popup.Show();
                            }
                            CommandResults["ScanMtrlCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanMtrlCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            IsBusy = false;
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

                                    if (await GetScaleInfo(text))
                                    {
                                        _DispatcherTimer.Start();
                                    }
                                    else
                                        ScaleId = "";
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

        public ICommand SetTareCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SetTareCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["SetTareCommand"] = false;
                            CommandCanExecutes["SetTareCommand"] = false;

                            ///
                            IsBusy = true;

                            _DispatcherTimer.Stop();

                            if (_TareWeight.Value <= 0)
                            {
                                OnMessage("TARE가 0보다 작을 수 없습니다.");
                                return;
                            }

                            bool success = false;

                            if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                            {
                                var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.ST);

                                if (result.code != "0")
                                    success = true;
                            }
                            else
                            {
                                _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                                _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA
                                {
                                    ScaleID = ScaleId
                                });

                                if (await _BR_PHR_REG_ScaleSetTare.Execute())
                                    success = true;
                            }

                            if (!success)
                                _TareWeight.Value = 0m;
                            else
                            {
                                _SetTare = true;
                                TarebtnEnable = false;

                                MtrlbtnEnable = true;
                            }


                            OnPropertyChanged("TareWeight");
                            ///

                            CommandResults["SetTareCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SetTareCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandCanExecutes["SetTareCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SetTareCommand") ?
                        CommandCanExecutes["SetTareCommand"] : (CommandCanExecutes["SetTareCommand"] = true);
                });
            }
        }
        
        public ICommand AllChargingCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["AllChargingCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["AllChargingCommandAsync"] = false;
                            CommandCanExecutes["AllChargingCommandAsync"] = false;
                            _DispatcherTimer.Stop();

                            ///
                            if (filteredComponents.Count(x => x.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입가능") || x.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입완료")) == filteredComponents.Count)
                            {
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);
                                dt.Columns.Add(new DataColumn("원료코드"));
                                dt.Columns.Add(new DataColumn("원료명"));
                                dt.Columns.Add(new DataColumn("원료시험번호"));
                                dt.Columns.Add(new DataColumn("바코드"));
                                dt.Columns.Add(new DataColumn("투입량"));
                                dt.Columns.Add(new DataColumn("상태"));

                                var bizRule = new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW();

                                foreach (var item in filteredComponents)
                                {
                                    if (item.IS_CAN_CHARGING_CHECKED_NAME == "투입가능")
                                    {
                                        bizRule.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA()
                                        {
                                            INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                            LANGID = AuthRepositoryViewModel.Instance.LangID,
                                            MSUBLOTBCD = item.MSUBLOTBCD,
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            // IS_NEED_CHKWEIGHT = "Y",
                                            IS_NEED_CHKWEIGHT = "N",
                                            IS_FULL_CHARGE = "Y",
                                            IS_CHECKONLY = "N",
                                            IS_INVEN_CHARGE = "N",
                                            IS_OUTPUT = "N",
                                            MSUBLOTID = item.MSUBLOTID,
                                            CHECKINUSER = AuthRepositoryViewModel.GetSecondUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                        });

                                        if (await bizRule.Execute() != true) return;
                                        item.UsedWeight = Convert.ToDecimal(item.TARGETWEIGHT);

                                        item.IS_CAN_CHARGING_CHECKED_NAME = "투입완료";
                                        item.CHGQTY = item.REMAINQTY;
                                    }
                                }

                                //2023.10.01 박희돈 투입가능 text가 Refresh 안됨. 스크롤을 움직여야 보여짐. 그래서 새로 바인딩 함.
                                //filteredComponents = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection();
                                //filteredComponents = BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs;


                                OnPropertyChanged("filteredComponents");

                                _mainWnd.dgSourceContainer.Refresh();

                                await GetDispenseHistory();
                            }
                            else
                                OnMessage("스캔하지 않은 원료가 있습니다.");
                            ///

                            CommandResults["AllChargingCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["AllChargingCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["AllChargingCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                        CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
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

                            _DispatcherTimer.Stop();

                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATAs.Clear();
                            _BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA_INVs.Clear();

                            StringBuilder msg = new StringBuilder("투입처리된 원료 목록\n");
                            decimal weight = _ScaleWeight.Value;

                            var select = BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs.Where(o => o.MSUBLOTBCD == _Text).FirstOrDefault();
                            
                            if (select.UPPER < weight || select.LOWER > weight)
                            {
                                throw new Exception(string.Format("소분할 무게를 확인하세요."));
                            }

                            if (weight > 0 && select.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입가능"))
                            {
                                BR_BRS_REG_ProductionOrder_Dispence_Split_Charging.INDATAs.Add(new BR_BRS_REG_ProductionOrder_Dispence_Split_Charging.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    MSUBLOTID = select.MSUBLOTID ,
                                    MSUBLOTBCD = select.MSUBLOTBCD,
                                    MSUBLOTQTY = weight,
                                    INSUSER = "",
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    INSDTTM = null,
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_INVEN_CHARGE = "N",
                                    CHECKINUSER = "",
                                    IS_OUTPUT = "N",
                                    TAREWEIGHT = _TareWeight.Value,
                                    UOMID = _ScaleWeight.Uom,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                msg.AppendLine(select.MSUBLOTBCD);

                                if (BR_BRS_REG_ProductionOrder_Dispence_Split_Charging.INDATAs.Count > 0)
                                {
                                    // 투입전자서명
                                    var authHelper = new iPharmAuthCommandHelper();
                                    authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");
                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        "소분백 투입",
                                        "소분백 투입",
                                        false,
                                        "OM_ProductionOrder_Charging",
                                        "", null, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }

                                    DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                    foreach (var item in BR_BRS_REG_ProductionOrder_Dispence_Split_Charging.INDATAs)
                                    {
                                        item.INSDTTM = curDttm;
                                        item.INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging");
                                        item.CHECKINUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging");
                                    }

                                    if (!await BR_BRS_REG_ProductionOrder_Dispence_Split_Charging.Execute()) throw new Exception(string.Format("투입처리 중 오류가 발생했습니다."));

                                    select.UsedWeight = weight;

                                    OnMessage(msg.ToString());

                                    select.IS_CAN_CHARGING_CHECKED_NAME = "투입완료";
                                    OnPropertyChanged("BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary");

                                    //filteredComponents = new BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATACollection();
                                    //filteredComponents = BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary.OUTDATAs;


                                    OnPropertyChanged("filteredComponents");

                                    _mainWnd.dgSourceContainer.Refresh();

                                    TarebtnEnable = true;
                                    MtrlbtnEnable = false;

                                }
                                else
                                    OnMessage("투입할 원료가 없습니다.");
                            }
                            else
                                OnMessage("투입할 원료를 선택해주세요.");

                            await GetDispenseHistory();

                            _DispatcherTimer.Start();

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

                            if (filteredComponents.Count(x => x.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입가능")) == filteredComponents.Count)
                            {
                                if (!await OnMessageAsync("투입가능 원료가 남아있습니다. 기록 하시겠습니까", true)) return;
                            }

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
                                "투입내용기록",
                                "투입내용기록",
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

                            foreach (var item in BR_BRS_SEL_Charging_Split_Dispence_History.OUTDATAs)
                            {
                                if (item.CHGQTY > 0)
                                {
                                    var row = dt.NewRow();
                                    row["자재ID"] = item.MTRLID ?? "";
                                    row["자재명"] = item.MTRLNAME ?? "";
                                    row["원료배치번호"] = item.MSUBLOTID ?? "";
                                    row["바코드"] = item.MSUBLOTBCD ?? "";
                                    row["무게"] = item.CHGQTY;
                                    row["단위"] = item.NOTATION;
                                    dt.Rows.Add(row);
                                }
                            }

                            if (dt.Rows.Count > 0)
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

                                _DispatcherTimer.Stop();
                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            else
                                OnMessage("투입된 소분백이 없습니다.");
                            ///

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

        public ICommand NoRecordConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommand"] = false;
                            CommandCanExecutes["NoRecordConfirmCommand"] = false;

                            _DispatcherTimer.Stop();

                            // 전자서명
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
                                "투입내용기록",
                                "투입내용기록",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //XML 형식으로 저장
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("자재ID"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("원료배치번호"));
                            dt.Columns.Add(new DataColumn("바코드"));
                            dt.Columns.Add(new DataColumn("무게"));
                            dt.Columns.Add(new DataColumn("단위"));

                            var row = dt.NewRow();
                            row["자재ID"] = "N/A";
                            row["자재명"] = "N/A";
                            row["원료배치번호"] = "N/A";
                            row["바코드"] = "N/A";
                            row["무게"] = "N/A";
                            row["단위"] = "N/A";
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

                            //_mainWnd.Close();

                            CommandResults["NoRecordConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NoRecordConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommand") ?
                        CommandCanExecutes["NoRecordConfirmCommand"] : (CommandCanExecutes["NoRecordConfirmCommand"] = true);
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
                        if (!_SetTare)
                            _TareWeight = _ScaleWeight.Copy();

                        _ScaleException = false;

                        if (_LowerWeight.Value <= _ScaleWeight.Add(_DisepenQty).Value && _ScaleWeight.Add(_DisepenQty).Value <= _UpperWeight.Value)
                        {
                            ScaleBackground = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            ScaleBackground = new SolidColorBrush(Colors.Yellow);
                        }

                    }
                    else
                    {
                        _ScaleException = true;
                        _ScaleWeight.SetWeight(0, _ScaleWeight.Uom, _scalePrecision);
                        ScaleBackground = new SolidColorBrush(Colors.Red);
                    }

                    OnPropertyChanged("ScaleWeight");
                    OnPropertyChanged("DspWeight");
                    OnPropertyChanged("TareWeight");
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

        /// <summary>
        /// 소분정보 조회
        /// </summary>
        private async Task GetDispenseHistory()
        {
            try
            {
                var paramInsts = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                decimal tempEXPRESSION;

                this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;

                _BR_BRS_SEL_Charging_Split_Dispence_History.INDATAs.Clear();
                _BR_BRS_SEL_Charging_Split_Dispence_History.OUTDATAs.Clear();

                if (decimal.TryParse(_mainWnd.CurrentInstruction.Raw.EXPRESSION, out tempEXPRESSION))
                {
                    BR_BRS_SEL_Charging_Split_Dispence_History.INDATAs.Add(new BR_BRS_SEL_Charging_Split_Dispence_History.INDATA()
                    {
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                        MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID,
                        CHGSEQ = tempEXPRESSION
                    });
                }

                foreach (var instruction in paramInsts)
                {
                    if (decimal.TryParse(instruction.Raw.EXPRESSION, out tempEXPRESSION))
                    {
                        BR_BRS_SEL_Charging_Split_Dispence_History.INDATAs.Add(new BR_BRS_SEL_Charging_Split_Dispence_History.INDATA()
                        {
                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                            MTRLID = instruction.Raw.BOMID,
                            CHGSEQ = decimal.Parse(instruction.Raw.EXPRESSION)
                        });
                    }
                }

                if (!await BR_BRS_SEL_Charging_Split_Dispence_History.Execute()) return;

                OnPropertyChanged("BR_BRS_SEL_Charging_Split_Dispence_History");
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 저울정보 조회
        /// </summary>
        /// <param name="scaleid"></param>
        /// <returns>True:조회성공</returns>
        private async Task<bool> GetScaleInfo(string scaleid)
        {
            try
            {
                // 저울 정보 조회
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                {
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    EQPTID = scaleid
                });

                if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute() && _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Count > 0)
                {
                    _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                    TarebtnEnable = true;

                    scalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                    ScaleId = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].EQPTID;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
                return false;
            }
        }
        
        #endregion
    }
}
