using LGCNS.iPharmMES.Common;
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
using System.Windows.Threading;
using C1.Silverlight.Data;
using ShopFloorUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using 보령.UserControls;

namespace 보령
{
    public class WMS일반자재환입처리ViewModel : ViewModelBase
    {
        #region [Property]
        public WMS일반자재환입처리ViewModel()
        {
            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD = new BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD();
            _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI = new BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI();
            _BR_BRS_GET_UDT_ProductionOrderPickingInfo = new BR_BRS_GET_UDT_ProductionOrderPickingInfo();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();

            _PickingSourceContainers = new ObservableCollection<WMSPickingSource>();
        }
        private WMS일반자재환입처리 _mainWnd;

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

        private string _MSUBLOTBCD;
        public string MSUBLOTBCD
        {
            get { return _MSUBLOTBCD; }
            set
            {
                _MSUBLOTBCD = value;
                OnPropertyChanged("MSUBLOTBCD");
            }
        }
        private decimal _MsubLotqty;
        private string _MSUBLOTQTYSTR;
        public string MSUBLOTQTYSTR
        {
            get { return _MSUBLOTQTYSTR; }
            set
            {
                _MSUBLOTQTYSTR = value;
                OnPropertyChanged("MSUBLOTQTYSTR");
            }
        }

        private bool _btnReturnEnable;
        public bool btnReturnEnable
        {
            get { return _btnReturnEnable; }
            set
            {
                _btnReturnEnable = value;
                OnPropertyChanged("btnReturnEnable");
            }
        }

        private WMSPickingSource _CurSourceContainer;
        public WMSPickingSource CurSourceContainer
        {
            get { return _CurSourceContainer; }
            set
            {
                _CurSourceContainer = value;
                OnPropertyChanged("CurSourceContainer");
            }
        }
        private ObservableCollection<WMSPickingSource> _PickingSourceContainers;
        public ObservableCollection<WMSPickingSource> PickingSourceContainers
        {
            get { return _PickingSourceContainers; }
            set
            {
                _PickingSourceContainers = value;
                OnPropertyChanged("PickingSourceContainers");
            }
        }

        private decimal _RETURNQTY;
        public decimal RETURNQTY
        {
            get { return _RETURNQTY; }
            set
            {
                decimal val = value;

                if (_CurSourceContainer != null)
                {
                    if (val > _MsubLotqty)
                    {
                        OnMessage(string.Format("피킹수량 보다 환입량이 많습니다.\n(피킹수량 : {0}, 환입량 : {1})", _MsubLotqty, val));
                        _RETURNQTY = _MsubLotqty;
                    }
                    else
                        _RETURNQTY = val;

                    btnReturnEnable = true;
                }
                else
                {
                    OnMessage("선택한 자재가 없습니다.");
                    _RETURNQTY = 0m;
                    btnReturnEnable = false;
                }

                OnPropertyChanged("RETURNQTY");
            }
        }
        #endregion

        #region [BizRule]
        /// <summary>
        /// 피킹정보 조회
        /// </summary>
        private BR_BRS_GET_UDT_ProductionOrderPickingInfo _BR_BRS_GET_UDT_ProductionOrderPickingInfo;

        private BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD;

        /// <summary>
        /// 자재환입
        /// </summary>
        private BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI;

        /// <summary>
        /// 라벨 발행
        /// </summary>
        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;

        /// <summary>
        /// 프린터 조회
        /// </summary>
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        #endregion

        #region [ICommand]
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["LoadedCommandAsync"] = false;
                        CommandCanExecutes["LoadedCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        if (arg != null && arg is WMS일반자재환입처리)
                        {
                            _mainWnd = arg as WMS일반자재환입처리;

                            // 현재 작업장의 프린터 정보 조회
                            _BR_PHR_SEL_System_Printer.INDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.OUTDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = AuthRepositoryViewModel.Instance.RoomID,
                                IPADDRESS = Common.ClientIP
                            });

                            if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                                _selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                            else
                                _selectedPrint = null;
                            OnPropertyChanged("curPrintName");

                            // 피킹된 자재목록 조회
                            _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Clear();
                            _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Clear();
                            // 현재지시문
                            if (!(string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.EXPRESSION) || string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.BOMID)))
                            {
                                _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Add(new BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.OrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = Convert.ToInt32(_mainWnd.CurrentInstruction.Raw.EXPRESSION),
                                    MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID
                                });
                            }
                            // REF지시문
                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            foreach (var item in inputValues)
                            {
                                _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Add(new BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.OrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = Convert.ToInt32(item.Raw.EXPRESSION),
                                    MTRLID = item.Raw.BOMID
                                });
                            }

                            if (await _BR_BRS_GET_UDT_ProductionOrderPickingInfo.Execute())
                            {
                                foreach (var item in _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs)
                                {
                                    _PickingSourceContainers.Add(new WMSPickingSource
                                    {
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        MSUBLOTBCD = item.MSUBLOTBCD
                                    });
                                }

                                OnPropertyChanged("PickingSourceContainers");
                            }

                            btnReturnEnable = false;
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
        public ICommand BCDCheckCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["BCDCheckCommand"] = false;
                        CommandCanExecutes["BCDCheckCommand"] = false;

                        ///
                        IsBusy = true;

                        if (arg != null && arg is string)
                        {
                            string barcode = arg.ToString();

                            // 바코드 체크
                            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATAs.Clear();
                            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs.Clear();
                            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATAs.Add(new BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                COMPONENTGUID = null,
                                MSUBLOTBCD = barcode
                            });

                            if (await _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.Execute())
                            {
                                if (_BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs.Count > 0)
                                {
                                    MSUBLOTBCD = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTBCD;
                                    MSUBLOTQTYSTR = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTQTYSTR;
                                    _MsubLotqty = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTQTY.GetValueOrDefault();

                                    if (_MsubLotqty > 0)
                                    {
                                        foreach (var item in _PickingSourceContainers)
                                        {
                                            if (item.MSUBLOTBCD == _MSUBLOTBCD)
                                            {
                                                CurSourceContainer = item;
                                                item.COMPONENTGUID = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].COMPONENTGUID;
                                                item.UOM = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].UOMNAME;
                                                _RETURNQTY = _MsubLotqty;
                                                OnPropertyChanged("RETURNQTY");
                                            }
                                        }
                                    }
                                    else
                                        throw new Exception(MessageTable.M[CommonMessageCode._10006].Replace("[%1]", "0"));
                                }
                            }
                        }
                        ///

                        CommandResults["BCDCheckCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["BCDCheckCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["BCDCheckCommand"] = true;
                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BCDCheckCommand") ?
                        CommandCanExecutes["BCDCheckCommand"] : (CommandCanExecutes["BCDCheckCommand"] = true);
                });
            }
        }


        public ICommand ChangeReturnQtyCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangeReturnQtyCommand"] = false;
                        CommandCanExecutes["ChangeReturnQtyCommand"] = false;

                        ///
                        if (arg != null && arg is string)
                        {
                            string returnqty = arg as string;

                            decimal chk;
                            if (decimal.TryParse(returnqty, out chk))
                                RETURNQTY = chk;
                        }
                        ///

                        CommandResults["ChangeReturnQtyCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangeReturnQtyCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ChangeReturnQtyCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangeReturnQtyCommand") ?
                        CommandCanExecutes["ChangeReturnQtyCommand"] : (CommandCanExecutes["ChangeReturnQtyCommand"] = true);
                });
            }
        }

        public ICommand KeyPadPopupCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["KeyPadPopupCommand"] = false;
                        CommandCanExecutes["KeyPadPopupCommand"] = false;

                        ///
                        ShopFloorUI.KeyPadPopUp popup = new ShopFloorUI.KeyPadPopUp();
                        popup.Value = RETURNQTY.ToString();
                        popup.Closed += (s, e) =>
                        {
                            if (popup.DialogResult.GetValueOrDefault())
                            {
                                decimal chk;
                                if (decimal.TryParse(popup.Value, out chk))
                                    RETURNQTY = chk;
                            }
                        };

                        popup.Show();

                        ///

                        CommandResults["KeyPadPopupCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["KeyPadPopupCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["KeyPadPopupCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("KeyPadPopupCommand") ?
                        CommandCanExecutes["KeyPadPopupCommand"] : (CommandCanExecutes["KeyPadPopupCommand"] = true);
                });
            }
        }


        public ICommand SetReturnQtyCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SetReturnQtyCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SetReturnQtyCommandAsync"] = false;
                            CommandCanExecutes["SetReturnQtyCommandAsync"] = false;

                            ///

                            if (_CurSourceContainer != null)
                            {
                                if (await OnMessageAsync(string.Format("[{0}] 자재를 {1:#,0} {2} 환입합니다.", _CurSourceContainer.MSUBLOTBCD, _RETURNQTY, _CurSourceContainer.UOM), true))
                                {
                                    _CurSourceContainer.RETURNQTY = _RETURNQTY;
                                    _CurSourceContainer.STATUS = "환입대상";
                                }
                            }

                            ///

                            CommandResults["SetReturnQtyCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SetReturnQtyCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SetReturnQtyCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SetReturnQtyCommandAsync") ?
                        CommandCanExecutes["SetReturnQtyCommandAsync"] : (CommandCanExecutes["SetReturnQtyCommandAsync"] = true);
                });
            }
        }


        public ICommand ConfrimCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["ConfrimCommandAsync"] = false;
                        CommandResults["ConfrimCommandAsync"] = false;

                        ///

                        var ReturnList = _PickingSourceContainers.Where(x => x.STATUS == "환입대상");

                        if (ReturnList.Count() != _PickingSourceContainers.Count)
                        {
                            if (await OnMessageAsync("처리완료 되지 않은 자재가 있습니다.\n환입불요 처리 후 계속 진행하시겠습니까?", true))
                            {
                                foreach (var item in _PickingSourceContainers)
                                    item.STATUS = item.STATUS != "환입대상" ? "환입불요" : item.STATUS;
                            }
                            else
                                throw new Exception("취소했습니다.");
                        }

                        // 전자서명
                        var authHelper = new iPharmAuthCommandHelper();

                        // 재기록
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
                            string.Format("WMS일반자재환입처리"),
                            string.Format("WMS일반자재환입처리"),
                            false,
                            "OM_ProductionOrder_SUI",
                            _mainWnd.CurrentOrder.EquipmentID, _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                        {
                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                        }

                        var viewmodel = new WMS일반자재환입처리팝업ViewModel();

                        WMS일반자재환입처리팝업 popup = new WMS일반자재환입처리팝업()
                        {
                            DataContext = viewmodel
                        };



                        // 환입처리
                        _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATAs.Clear();
                        _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.OUTDATAs.Clear();

                        foreach (var item in _PickingSourceContainers)
                        {
                            if (item.STATUS == "환입대상")
                            {
                                _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATAs.Add(new BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    COMPONENTGUID = item.COMPONENTGUID,
                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                    RETURNQTY = item.RETURNQTY,
                                    RTN_NOTE = "",
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                });

                                viewmodel.MaterialReturn.Add(new WMS일반자재환입처리팝업ViewModel.WMSPickingSource
                                {
                                    MTRLID = item.MTRLID,
                                    MTRLNAME = item.MTRLNAME,
                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                    RETURNQTY = item.RETURNQTY
                                });
                            }
                        }


                        popup.Closed += async (s, e) =>
                        {
                            if (popup.DialogResult == true)
                            {
                                if (await _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.Execute() == true)
                                {
                                    // 포장자재반납라벨 출력
                                    if (curPrintName != "N/A")
                                    {
                                        foreach (var item in _PickingSourceContainers)
                                        {
                                            if (item.STATUS == "환입대상")
                                            {
                                                _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                                _BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();
                                                DateTime NOW = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                                _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                                {
                                                    ReportPath = "/Reports/LABEL/LABEL_C0402_018_16",
                                                    PrintName = curPrintName,
                                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                                });
                                                _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                                {
                                                    ParamName = "MSUBLOTBCD",
                                                    ParamValue = item.MSUBLOTBCD
                                                });
                                                _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                                {
                                                    ParamName = "PACKING",
                                                    ParamValue = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                                });
                                                _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                                {
                                                    ParamName = "TRANSPORTDATE",
                                                    ParamValue = NOW.ToString("yyyy-MM-dd HH:mm:ss")
                                                });
                                                _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                                {
                                                    ParamName = "TRANSPORTTIME",
                                                    ParamValue = NOW.ToString("HHmm")
                                                });

                                                // 라벨 발행이 실패해도 exception 발생하지 않도록 설정
                                                await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                                            }
                                        }
                                    }


                                    IsBusy = true;

                                    // XML 생성
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);

                                    dt.Columns.Add(new DataColumn("MTRLID"));
                                    dt.Columns.Add(new DataColumn("MTRLNAME"));
                                    dt.Columns.Add(new DataColumn("MSUBLOTBCD"));
                                    dt.Columns.Add(new DataColumn("RETURNQTY"));
                                    dt.Columns.Add(new DataColumn("UOM"));
                                    dt.Columns.Add(new DataColumn("STATUS"));

                                    foreach (var item in _PickingSourceContainers)
                                    {
                                        if (item.STATUS == "환입대상")
                                        {
                                            var row = dt.NewRow();
                                            row["MTRLID"] = item.MTRLID ?? "";
                                            row["MTRLNAME"] = item.MTRLNAME ?? "";
                                            row["MSUBLOTBCD"] = item.MSUBLOTBCD ?? "";
                                            row["RETURNQTY"] = item.RETURNQTY.ToString("#,0");
                                            row["UOM"] = item.UOM ?? "";
                                            row["STATUS"] = "환입완료";
                                            dt.Rows.Add(row);
                                        }
                                    }

                                    var xml = BizActorRuleBase.CreateXMLStream(ds);
                                    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                    if (ds.Tables["DATA"].Rows.Count > 0)
                                    {
                                        _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                        _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;
                                    }
                                    else
                                    {
                                        _mainWnd.CurrentInstruction.Raw.ACTVAL = "자재환입없음";
                                        _mainWnd.CurrentInstruction.Raw.NOTE = null;
                                    }

                                    var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                    }

                                    if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                    else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                                }
                            }

                        };
                        popup.Show();

                        IsBusy = false;
                        ///
                        CommandResults["ConfrimCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        IsBusy = false;
                        CommandResults["ConfrimCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ConfrimCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfrimCommandAsync") ?
                        CommandCanExecutes["ConfrimCommandAsync"] : (CommandCanExecutes["ConfrimCommandAsync"] = true);
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

        public class WMSPickingSource : BizActorDataSetBase
        {
            private string _STATUS = "대기";
            /// <summary>
            /// 대기, 환입대상, 자재환입
            /// </summary>
            public string STATUS
            {
                get { return _STATUS; }
                set
                {
                    _STATUS = value;
                    OnPropertyChanged("STATUS");
                }
            }

            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    OnPropertyChanged("MTRLID");
                }
            }
            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    OnPropertyChanged("MTRLNAME");
                }
            }
            private string _MSUBLOTBCD;
            public string MSUBLOTBCD
            {
                get { return _MSUBLOTBCD; }
                set
                {
                    _MSUBLOTBCD = value;
                    OnPropertyChanged("MSUBLOTBCD");
                }
            }
            private string _COMPONENTGUID;
            public string COMPONENTGUID
            {
                get { return _COMPONENTGUID; }
                set
                {
                    _COMPONENTGUID = value;
                    OnPropertyChanged("COMPONENTGUID");
                }
            }
            private decimal _RETURNQTY;
            public decimal RETURNQTY
            {
                get { return _RETURNQTY; }
                set
                {
                    _RETURNQTY = value;
                    OnPropertyChanged("RETURNQTY");
                }
            }
            private string _UOM;
            public string UOM
            {
                get { return _UOM; }
                set
                {
                    _UOM = value;
                    OnPropertyChanged("UOM");
                }
            }
        }
    }
}
