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
    public class 빈용기무게측정ViewModel : ViewModelBase
    {
        #region [Property]
        public 빈용기무게측정ViewModel()
        {
            _BR_PHR_SEL_ProductionOrderOutput_Define = new BR_PHR_SEL_ProductionOrderOutput_Define();
            _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType = new BR_PHR_SEL_ProductionOrderOutput_Define_AnyType();
            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();    
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _BR_BRS_CHK_EMPTY_VESSEL_Info = new BR_BRS_CHK_EMPTY_VESSEL_Info();
            _BR_BRS_REG_VESSEL_WEIGHT = new BR_BRS_REG_VESSEL_WEIGHT();

            _IBCList = new ObservableCollection<TareCheckedContainer>();

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;
        }

        빈용기무게측정 _mainWnd;
        private string _Outputguid = "";

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
        private Weight _TareWeight = new Weight();
        public string TareWeight
        {
            get
            {
                if(!_VesselChecked)
                    return "용기정보없음";
                else if (_ScaleException)
                    return "연결실패";
                else
                    return _TareWeight.WeightUOMString;
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
        #endregion

        private ObservableCollection<TareCheckedContainer> _IBCList;
        public ObservableCollection<TareCheckedContainer> IBCList
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
        /// 공정중제품 정보 조회(OUTPUTID가 없는 경우)
        /// </summary>
        private BR_PHR_SEL_ProductionOrderOutput_Define _BR_PHR_SEL_ProductionOrderOutput_Define;
        /// <summary>
        /// 공정중제품 정보 조회(레시피에 지정한 OUTPUTID가 있는경우)
        /// </summary>
        private BR_PHR_SEL_ProductionOrderOutput_Define_AnyType _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType;
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
        /// 용기정보 조회(빈용기인지 확인)
        /// </summary>                     
        private BR_BRS_CHK_EMPTY_VESSEL_Info _BR_BRS_CHK_EMPTY_VESSEL_Info;
        /// <summary>
        /// 측정된 TARE 무게 반영
        /// </summary>
        private 보령.BR_BRS_REG_VESSEL_WEIGHT _BR_BRS_REG_VESSEL_WEIGHT;       
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

                        if (arg != null && arg is 빈용기무게측정)
                        {
                            _mainWnd = arg as 빈용기무게측정;
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

                            // 공정중제품 정보 조회
                            if (string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.BOMID))
                            {
                                _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Clear();
                                _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Clear();

                                _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID != null ? new Guid?(new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID)) : null
                                });

                                if (await _BR_PHR_SEL_ProductionOrderOutput_Define.Execute() == true && _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Count > 0)
                                    _Outputguid = _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs[0].OUTPUTGUID != null ? _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs[0].OUTPUTGUID.ToString() : "";
                            }
                            else
                            {
                                _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATAs.Clear();
                                _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs.Clear();
                                _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID != null ? new Guid?(new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID)) : null,
                                    OUTPUTID = string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.BOMID) ? null : _mainWnd.CurrentInstruction.Raw.BOMID
                                });

                                if (await _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.Execute() && _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs.Count > 0)
                                {
                                    _Outputguid = _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs[0].OUTPUTGUID.GetValueOrDefault().ToString();
                                }
                            }
                                                      
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

                                                // 2021.08.22 박희돈. EBR수정요청 내용. 빈용기는 소수점이 없어야함.
                                                //_ScalePrecision = int.TryParse(_ScaleInfo.PRECISION.ToString(), out chk) ? chk : 3;
                                                _ScalePrecision = 0;

                                                _TareWeight.SetWeight(0, _ScaleUom, _ScalePrecision);
                                                OnPropertyChanged("TareWeight");
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

                        _repeater.Stop();

                        VesselId = arg as string;

                        _BR_BRS_CHK_EMPTY_VESSEL_Info.INDATAs.Clear();
                        _BR_BRS_CHK_EMPTY_VESSEL_Info.INDATAs.Add(new BR_BRS_CHK_EMPTY_VESSEL_Info.INDATA
                        {
                            VESSELID = _VesselId
                        });

                        if (await _BR_BRS_CHK_EMPTY_VESSEL_Info.Execute())
                        {
                            if (CheckVesselId(_VesselId))
                            {
                                _VesselChecked = true;
                                _repeater.Start();
                                OnPropertyChanged("TareWeight");
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

                        if (!string.IsNullOrWhiteSpace(_VesselId) && _VesselChecked && !_ScaleException && _TareWeight.Value > 0)
                        {
                            _repeater.Stop();
                            btnRecordEnable = false;

                            // 2021-12-03 김호연 
                            // 저울 무게 기록하는 시점에 무게 변경되는 현상이 발생
                            // 저울 무게는 변경이 되니 변수에 담아서 무게 저장 및 기록
                            Decimal TareWeightValue = _TareWeight.Value;

                            _BR_BRS_REG_VESSEL_WEIGHT.INDATAs.Clear();
                            _BR_BRS_REG_VESSEL_WEIGHT.INDATAs.Add(new BR_BRS_REG_VESSEL_WEIGHT.INDATA
                            {
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                OUTPUTGUID = _Outputguid,
                                OUTGUBUN = "EMPTY",
                                //GROSSWEIGHT = _TareWeight.Value,
                                // 2021-12-03 김호연 
                                GROSSWEIGHT = TareWeightValue,
                                UOMID = _ScaleUom,
                                VESSELID = _VesselId,
                                SCALEID = ScaleId,
                                PRINTNAME = curPrintName == "N/A" ? "" : curPrintName
                            });

                            if (await _BR_BRS_REG_VESSEL_WEIGHT.Execute())
                            {
                                IBCList.Add(new TareCheckedContainer
                                {
                                    PoId = _mainWnd.CurrentOrder.ProductionOrderID,
                                    VesselId = _VesselId,
                                    ScaleId = ScaleId,
                                    //TareWeight = _TareWeight.Value,
                                    // 2021-12-03 김호연 
                                    TareWeight = TareWeightValue,
                                    NetWeight = 0,
                                    Precision = _TareWeight.Precision,
                                    Uom = _TareWeight.Uom
                                });

                                InitializeData();
                            }
                            else
                            {
                                _repeater.Start();
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

                        if (IBCList.Count > 0)
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
                                string.Format("빈용기무게측정"),
                                string.Format("빈용기무게측정"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrder.EquipmentID, _mainWnd.CurrentOrder.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            } 

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                            dt.Columns.Add(new DataColumn("오더번호"));
                            //-------------------------------------------------------------------------------------------------------
                            //dt.Columns.Add(new DataColumn("제조번호"));
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("저울번호"));
                            dt.Columns.Add(new DataColumn("용기무게"));

                            foreach (var item in IBCList)
                            {
                                var row = dt.NewRow();
                                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                row["오더번호"] = item.PoId != null ? item.PoId : "";
                                //-------------------------------------------------------------------------------------------------------
                                //row["제조번호"] = _mainWnd.CurrentOrder.BatchNo;
                                row["용기번호"] = item.VesselId != null ? item.VesselId : "";
                                row["저울번호"] = item.ScaleId != null ? item.ScaleId : "";
                                row["용기무게"] = !string.IsNullOrWhiteSpace(item.TareWeightStr) ? item.TareWeightStr : "";

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

                                    _TareWeight.SetWeight(0, _ScaleUom, _ScalePrecision);
                                    OnPropertyChanged("TareWeight");
                                    OnPropertyChanged("ScaleId");

                                    _repeater.Start();
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

                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            _TareWeight.SetWeight(result.data, result.unit);
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
                            _TareWeight.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, _ScalePrecision);
                        }
                    }

                    if (success)
                    {
                        _ScaleException = false;
                        _ScaleUom = _TareWeight.Uom;
                        _ScalePrecision = _TareWeight.Precision;
                        if(_VesselChecked)
                            btnRecordEnable = true;
                    }
                    else
                    {
                        _ScaleException = true;
                        _ScaleUom = _TareWeight.Uom;
                        _ScalePrecision = _TareWeight.Precision;
                    }

                    OnPropertyChanged("TareWeight");
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
            string ID = Id.ToUpper();
            foreach (TareCheckedContainer item in _IBCList)
            {
                if (ID == item.VesselId)
                    return false;
            }

            return true;
        }
        private void InitializeData()
        {
            _VesselChecked = false;
            VesselId = "";
            _TareWeight.Value = 0;
            btnRecordEnable = false;
            _mainWnd.txtVesselId.Focus();
            OnPropertyChanged("TareWeight");
        }

        public class TareCheckedContainer : WIPContainer
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

            public string TareWeightStr
            {
                get
                {
                    Weight tare = new Weight();
                    tare.SetWeight(TareWeight, Uom, Precision);

                    return tare.WeightUOMStringWithSeperator;
                }
            }
        }
        #endregion
    }
}
