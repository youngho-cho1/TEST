using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using 보령.UserControls;

namespace 보령
{
    //[ShopFloorCustomHidden]
    public class 반제품무게측정ViewModel : ViewModelBase
    {
        #region [Property]
        public 반제품무게측정ViewModel()
        {
            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _BR_BRS_SEL_VESSEL_Info = new BR_BRS_SEL_VESSEL_Info();
            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi = new BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi();
            _IBCList = new ObservableCollection<ChargedWIPContainer>();

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;
        }

        반제품무게측정 _mainWnd;

        #region Campaign Production
        private BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection _OrderList;
        public BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection OrderList
        {
            get { return _OrderList; }
            set
            {
                _OrderList = value;
                OnPropertyChanged("OrderList");
            }
        }
        private bool _CanSelectOrder;
        public bool CanSelectOrder
        {
            get { return _CanSelectOrder; }
            set
            {
                _CanSelectOrder = value;
                OnPropertyChanged("CanSelectOrder");
            }
        }
        #endregion

        #region Scale
        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 2000;
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();

        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
        public string ScaleId
        {
            get
            {
                if (_ScaleInfo != null)
                    return _ScaleInfo.EQPTID;
                else
                    return "N/A";
            }
        }
        private bool _ScaleException = true;
        private bool _VesselChecked = false;
        private string _ScaleUom = "g";
        private int _ScalePrecision = 3;

        private Weight _TotalWeight = new Weight();
        public string TotalWeight
        {
            get
            {
                if (!_VesselChecked)
                    return "용기정보없음";
                else if (_ScaleException)
                    return "연결실패";
                else
                    return _TotalWeight.WeightUOMString;
            }
        }
        #endregion

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

        private string _VesselId;
        public string VesselId
        {
            get { return _VesselId; }
            set
            {
                _VesselId = value;
                OnPropertyChanged("VesselId");
            }
        }
       
        private bool _btnRecordEnable;
        public bool btnRecordEnable
        {
            get { return _btnRecordEnable; }
            set
            {
                _btnRecordEnable = value;
                OnPropertyChanged("btnRecordEnable");
            }
        }

        private ObservableCollection<ChargedWIPContainer> _IBCList;
        public ObservableCollection<ChargedWIPContainer> IBCList
        {
            get { return _IBCList; }
            set
            {
                _IBCList = value;
                OnPropertyChanged("IBCList");
            }
        }
        #endregion

        #region [BizRule]
        /// <summary>
        /// 작업장 저울 조회
        /// </summary>
        private BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo;
        /// <summary>
        /// 저울 정보 조회
        /// </summary>
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        /// <summary>
        /// 작업장 프린터 조회
        /// </summary>
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        /// <summary>
        /// 저울값 IF
        /// </summary>
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;       
        /// <summary>
        /// 용기정보조회
        /// </summary>
        private BR_BRS_SEL_VESSEL_Info _BR_BRS_SEL_VESSEL_Info;
        /// <summary>
        /// 측정된 무게 반영
        /// </summary>
        private BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi;       
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
                        IsBusy = true;

                        if (arg != null && arg is 반제품무게측정)
                        {
                            _mainWnd = arg as 반제품무게측정;
                            _mainWnd.Closed += (s, e) =>
                            {
                                if (_repeater != null)
                                    _repeater.Stop();

                                _repeater = null;
                            };

                            #region Campaign Order
                            OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                            CanSelectOrder = OrderList.Count > 0 ? true : false;
                            #endregion

                            // 작업장 저울목록 조회
                            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Clear();
                            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Clear();
                            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATA
                            {
                                EQPTID = AuthRepositoryViewModel.Instance.RoomID,
                                LANGID = AuthRepositoryViewModel.Instance.LangID
                            });

                            if (await _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.Execute())
                            {
                                if (_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    //_SelectedScale = _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[0].EQPTID;
                                    //바닥저울을 찾도록 로직 변경. 2021.01.18 phd
                                    for (int idx = 0; idx < _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count; idx++)
                                    {
                                        if (_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[idx].MODEL.Contains("IFS4"))
                                        {
                                            // 저울정보 조회
                                            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                                            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                                            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                                            {
                                                EQPTID = _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[idx].EQPTID
                                            });

                                            if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute())
                                            {
                                                int chk;

                                                _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                                _ScaleUom = _ScaleInfo.NOTATION;
                                                _ScalePrecision = int.TryParse(_ScaleInfo.PRECISION.ToString(), out chk) ? chk : 3;

                                                _TotalWeight.SetWeight(0, _ScaleUom, _ScalePrecision);
                                                OnPropertyChanged("TotalWeight");
                                                OnPropertyChanged("ScaleId");
                                            }
                                        }
                                    }
                                }
                            }

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

                            // 버튼세팅
                            btnRecordEnable = false;
                            _mainWnd.txtVesselId.Focus();
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
                        IsBusy = false;
                        CommandCanExecutes["LoadedCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        public ICommand WeighingCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["WeighingCommandAsync"] = false;
                        CommandResults["WeighingCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        VesselId = arg as string;

                        _BR_BRS_SEL_VESSEL_Info.INDATAs.Clear();
                        _BR_BRS_SEL_VESSEL_Info.OUTDATAs.Clear();

                        _BR_BRS_SEL_VESSEL_Info.INDATAs.Add(new BR_BRS_SEL_VESSEL_Info.INDATA
                        {
                            VESSELID = _VesselId
                        });

                        if (await _BR_BRS_SEL_VESSEL_Info.Execute())
                        {
                            if (CheckVesselId(VesselId))
                            {
                                _VesselChecked = true;
                                _TotalWeight.Value = 0;
                                _repeater.Start();
                                OnPropertyChanged("TotalWeight");
                            }
                                
                            else
                            {
                                OnMessage("중복된 용기번호 입니다.");
                                InitializeData();
                            }
                        }
                        else
                        {
                            OnMessage("조회된 BIN정보가 없습니다.");
                            InitializeData();
                        }

                        ///

                        CommandResults["WeighingCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["WeighingCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["WeighingCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("WeighingCommandAsync") ?
                        CommandCanExecutes["WeighingCommandAsync"] : (CommandCanExecutes["WeighingCommandAsync"] = true);
                });
            }
        }
        public ICommand RecordingCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["RecordingCommand"] = false;
                        CommandResults["RecordingCommand"] = false;

                        ///
                        IsBusy = true;

                        if (!string.IsNullOrWhiteSpace(_VesselId) && _VesselChecked && !_ScaleException && _TotalWeight.Value > 0)
                        {
                            _repeater.Stop();
                            btnRecordEnable = false;

                            // 2021-12-03 김호연 
                            // 저울 무게 기록하는 시점에 무게 변경되는 현상이 발생
                            // 저울 무게는 변경이 되니 변수에 담아서 무게 저장 및 기록
                            Decimal TotalWeightValue = _TotalWeight.Value;

                            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATAs.Clear();
                            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATA
                            {
                                VESSELID = VesselId,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                                //GROSSWEIGHT = _TotalWeight.Value,
                                // 2021-12-03 김호연 
                                GROSSWEIGHT = TotalWeightValue,
                                SCALEID = ScaleId,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                PRINTNAME = curPrintName == "N/A" ? "" : curPrintName
                            });

                            if (await _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.Execute())
                            {
                                Weight tare = new Weight();
                                tare.SetWeight(_BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].TAREWEIGHT.GetValueOrDefault(), _TotalWeight.Uom, _TotalWeight.Precision);

                                IBCList.Add(new ChargedWIPContainer
                                {
                                    PoId = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].POID,
                                    VesselId = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].VESSELID,
                                    ScaleId = ScaleId,
                                    Uom = _TotalWeight.Uom,
                                    Precision = _TotalWeight.Precision,
                                    TareWeight = tare.Value,
                                    //NetWeight = _TotalWeight.Value - tare.Value
                                    // 2021-12-03 김호연 
                                    NetWeight = TotalWeightValue - tare.Value
                                });

                                InitializeData();
                            }
                            else
                            {
                                //_repeater.Start();
                                btnRecordEnable = true;
                            }
                        }
                     
                        ///

                        CommandResults["RecordingCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["RecordingCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["RecordingCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecordingCommand") ?
                        CommandCanExecutes["RecordingCommand"] : (CommandCanExecutes["RecordingCommand"] = true);
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
                        IsBusy = true;

                        if (_IBCList.Count > 0)
                        {
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
                                string.Format("반제품무게측정"),
                                string.Format("반제품무게측정"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                            dt.Columns.Add(new DataColumn("오더번호"));
                            //-------------------------------------------------------------------------------------------------------
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("저울번호"));
                            dt.Columns.Add(new DataColumn("총무게"));
                            dt.Columns.Add(new DataColumn("용기무게"));
                            dt.Columns.Add(new DataColumn("내용물무게"));

                            foreach (var item in _IBCList)
                            {
                                var row = dt.NewRow();

                                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                row["오더번호"] = item.PoId != null ? item.PoId : "";
                                //-------------------------------------------------------------------------------------------------------
                                row["용기번호"] = item.VesselId != null ? item.VesselId : "";
                                row["저울번호"] = item.ScaleId != null ? item.ScaleId : "";
                                row["총무게"] = item != null ? item.GrossWeight.ToString("F" + item.Precision) : "";
                                row["용기무게"] = item != null ? item.TareWeight.ToString("F" + item.Precision) : "";
                                row["내용물무게"] = item != null ? item.NetWeight.ToString("F" + item.Precision) : "";

                                dt.Rows.Add(row);
                            }

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
                        else
                            OnMessage("조회된 데이터가 없습니다.");
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
                        IsBusy = false;
                        CommandCanExecutes["ComfirmCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ?
                        CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
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
                        IsBusy = false;
                        CommandCanExecutes["ChangePrintCommand"] = true;                        
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangePrintCommand") ?
                        CommandCanExecutes["ChangePrintCommand"] : (CommandCanExecutes["ChangePrintCommand"] = true);
                });
            }
        }
        public ICommand ChangeScaleCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangeScaleCommand"] = false;
                        CommandCanExecutes["ChangeScaleCommand"] = false;

                        /// 
                        _repeater.Stop();

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
                                    int chk;
                                    _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                    _ScaleUom = _ScaleInfo.NOTATION;
                                    _ScalePrecision = int.TryParse(_ScaleInfo.PRECISION.ToString(), out chk) ? chk : 3;

                                    _TotalWeight.SetWeight(0, _ScaleUom, _ScalePrecision);
                                    _TotalWeight.Value = 0;
                                    OnPropertyChanged("TotalWeight");
                                    OnPropertyChanged("ScaleId");

                                    //_repeater.Start();
                                }
                            }
                            else
                                _ScaleInfo = null;

                            OnPropertyChanged("ScaleId");
                        };

                        popup.Show();
                        ///

                        CommandResults["ChangeScaleCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangeScaleCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["ChangeScaleCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangeScaleCommand") ?
                        CommandCanExecutes["ChangeScaleCommand"] : (CommandCanExecutes["ChangeScaleCommand"] = true);
                });
            }
        }
        #endregion

        #region [etc]
        private async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                if (_ScaleInfo != null)
                {
                    bool success = false;
                    _restScaleService.Dispose();

                    _restScaleService = new ScaleWebAPIHelper();

                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            _TotalWeight.SetWeight(result.data, result.unit);
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
                            _TotalWeight.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, _ScalePrecision);
                        }
                    }

                    if (success)
                    {
                        _ScaleException = false;
                        _ScaleUom = _TotalWeight.Uom;
                        _ScalePrecision = _TotalWeight.Precision;

                        if (_VesselChecked)
                            btnRecordEnable = true;
                    }
                    else
                    {
                        _ScaleException = true;
                        _ScaleUom = _TotalWeight.Uom;
                        _ScalePrecision = _TotalWeight.Precision;
                    }

                    OnPropertyChanged("TotalWeight");
                    _repeater.Start();
                }
            }
            catch (Exception ex)
            {
                _repeater.Stop();
                OnException(ex.Message, ex);
            }
        }
        private bool CheckVesselId(string Id)
        {
            foreach (ChargedWIPContainer item in _IBCList)
            {
                if (Id == item.VesselId)
                    return false;
            }
            return true;
        }
        private void InitializeData()
        {
            VesselId = "";
            _TotalWeight.Value = 0;
            btnRecordEnable = false;
            _mainWnd.txtVesselId.Focus();
            OnPropertyChanged("TotalWeight");         
        }

        public class ChargedWIPContainer : WIPContainer
        {
            private string _ScaleId;
            public string ScaleId
            {
                get { return this._ScaleId; }
                set
                {
                    this._ScaleId = value;
                    this.OnPropertyChanged("VesselId");
                }
            }
        }
        #endregion
    }
}
