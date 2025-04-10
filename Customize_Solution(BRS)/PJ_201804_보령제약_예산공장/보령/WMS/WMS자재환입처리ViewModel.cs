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

namespace 보령
{
    public class WMS자재환입처리ViewModel : ViewModelBase
    {
        #region [Property]
        private WMS자재환입처리 _mainWnd;
        private string PrintName;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        /// <summary>
        /// Scale : 저울I/F, Quantity : 작업자 KeyIn
        /// </summary>
        enum enumScanType
        {
            Scale,
            Quantity
        };

        #region [0.상단피킹정보]        
        private string _MTRLBCD;
        /// <summary>
        /// 작업자 입력값
        /// </summary>
        public string MTRLBCD
        {
            get { return _MTRLBCD; }
            set
            {
                _MTRLBCD = value;
                OnPropertyChanged("MTRLBCD");
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
        private decimal _MSUBLOTQTY;
        public decimal MSUBLOTQTY
        {
            get { return _MSUBLOTQTY; }
            set
            {
                _MSUBLOTQTY = value;
                OnPropertyChanged("MSUBLOTQTY");
            }
        }
        #endregion
        #region [1.우측환입정보]
        // 저울 or 키인한 정보
        private string _SCALEID;
        public string SCALEID // 저울을 사용하지 않는 경우 "저울미사용"으로 기록함 
        {
            get { return _SCALEID; }
            set
            {
                _SCALEID = value;
                NotifyPropertyChanged();
            }
        }
        private string _SCALEVAL;
        public string SCALEVAL
        {
            get { return _SCALEVAL; }
            set
            {
                _SCALEVAL = value;
                NotifyPropertyChanged();
            }
        }
        private decimal? _ScaleWeight;
        public decimal? ScaleWeight
        {
            get { return _ScaleWeight; }
            set
            {
                _ScaleWeight = value;
                NotifyPropertyChanged();
            }
        }
        private string _ScaleUOM;
        public string ScaleUOM
        {
            get { return _ScaleUOM; }
            set
            {
                _ScaleUOM = value;
                NotifyPropertyChanged();
            }
        }
        // 환산한 환입량 정보
        private string _PARAM;
        public string PARAM
        {
            get { return _PARAM; }
            set
            {
                _PARAM = value;
                NotifyPropertyChanged();
            }
        }
        private string _RETURNQTY;
        public string RETURNQTY
        {
            get { return _RETURNQTY; }
            set
            {
                _RETURNQTY = value;
                NotifyPropertyChanged();
            }
        }
        private string _ReturnUOM;
        public string ReturnUOM
        {
            get { return _ReturnUOM; }
            set
            {
                _ReturnUOM = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region [2.기타]
        private bool _IsBusyForWeight;
        public bool IsBusyForWeight
        {
            get { return _IsBusyForWeight; }
            set
            {
                _IsBusyForWeight = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PackingMTRL> _PackingMTRLList;
        /// <summary>
        /// 피킹된 자재목록
        /// </summary>
        public ObservableCollection<PackingMTRL> PackingMTRLList
        {
            get { return _PackingMTRLList; }
            set
            {
                _PackingMTRLList = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region [BizRule]
        private BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD;
        public BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD
        {
            get { return _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD; }
            set
            {
                _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD = value;
                NotifyPropertyChanged();
            }
        }
        private BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI;
        public BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI
        {
            get { return _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI; }
            set
            {
                _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_GET_UDT_ProductionOrderPickingInfo _BR_BRS_GET_UDT_ProductionOrderPickingInfo;
        public BR_BRS_GET_UDT_ProductionOrderPickingInfo BR_BRS_GET_UDT_ProductionOrderPickingInfo
        {
            get { return _BR_BRS_GET_UDT_ProductionOrderPickingInfo; }
            set
            {
                _BR_BRS_GET_UDT_ProductionOrderPickingInfo = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        public BR_PHR_SEL_PRINT_LabelImage BR_PHR_SEL_PRINT_LabelImage
        {
            get { return _BR_PHR_SEL_PRINT_LabelImage; }
            set
            {
                _BR_PHR_SEL_PRINT_LabelImage = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        public BR_PHR_SEL_System_Printer BR_PHR_SEL_System_Printer
        {
            get { return _BR_PHR_SEL_System_Printer; }
            set
            {
                _BR_PHR_SEL_System_Printer = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region [ICommand]
        public ICommand LoadedCommandAsync
        {
            get { return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["LoadedCommandAsync"] = false;
                        CommandCanExecutes["LoadedCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        if (arg != null && arg is WMS자재환입처리)
                        {
                            _mainWnd = arg as WMS자재환입처리;
                            _mainWnd.Closed += (s, e) =>
                            {
                                if (_DispatcherTimer != null)
                                    _DispatcherTimer.Stop();

                                _DispatcherTimer = null;
                            };

                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            // 현재 작업장의 프린터 정보 조회
                            _BR_PHR_SEL_System_Printer.INDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.OUTDATAs.Clear();

                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = AuthRepositoryViewModel.Instance.RoomID
                            });

                            if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                                PrintName = _BR_PHR_SEL_System_Printer.OUTDATAs[0].PRINTERNAME;
                            else
                                PrintName = "";

                            // 피킹된 자재목록 조회
                            _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Clear();
                            _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Clear();

                            // CurrentInstruction
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
                            // RefInstruction
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

                            if (await _BR_BRS_GET_UDT_ProductionOrderPickingInfo.Execute() == true && _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs)
                                {
                                    PackingMTRLList.Add(new PackingMTRL
                                    {
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        MSUBLOTBCD = item.MSUBLOTBCD,
                                        RESULT = PackingMTRL.enumStatusType.대기
                                    });
                                }
                            }

                            _mainWnd.MTRLGrid.ItemsSource = PackingMTRLList;
                            _mainWnd.btnReturn.IsEnabled = false;
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
                        CommandCanExecutes["BCDCheckCommand"] = false;
                        
                        CommandResults["BCDCheckCommand"] = false;

                        ///
                        IsBusy = true;
                        _mainWnd.btnReturn.IsEnabled = false;
                        _DispatcherTimer.Stop();

                        if (arg != null)
                            MTRLBCD = arg.ToString();

                        _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATAs.Clear();
                        _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs.Clear();

                        if (!string.IsNullOrWhiteSpace(MTRLBCD))
                        {
                            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATAs.Add(new BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                COMPONENTGUID = null,
                                MSUBLOTBCD = MTRLBCD
                            });

                            if (await _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.Execute() == true && _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs.Count > 0)
                            {
                                if (_PackingMTRLList.Count(x => x.MSUBLOTBCD.Equals(_BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTBCD)) > 0)
                                {
                                    MSUBLOTQTYSTR = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTQTYSTR;
                                    MSUBLOTBCD = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTBCD;
                                    MSUBLOTQTY = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTQTY.HasValue ? _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTQTY.Value : 0m;

                                    // Grid에서 ComponentGUID 검색해서 seleteditem으로 넣어야함
                                    foreach (var item in _PackingMTRLList)
                                    {
                                        if (item.MSUBLOTBCD == _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].MSUBLOTBCD)
                                        {
                                            _mainWnd.MTRLGrid.SelectedItem = item;
                                            item.RETURNUOM = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].UOMNAME;
                                            item.COMPONENTGUID = _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD.OUTDATAs[0].COMPONENTGUID;
                                            _mainWnd.txtReturnUOM.Text = item.RETURNUOM;
                                        }
                                    }

                                    this.SCALEVAL = "";
                                    this.ScaleWeight = null;
                                    this.PARAM = "";
                                }
                                else
                                {
                                    C1.Silverlight.C1MessageBox.Show(MessageTable.M[CommonMessageCode._10006].Replace("[%1]", "0"));
                                    return;
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

        public ICommand ScaleScanCommand
        {
            get
            {
                return new CommandBase(arg =>
                    {
                        try
                        {
                            CommandCanExecutes["ScaleScanCommand"] = false;
                            CommandResults["ScaleScanCommand"] = false;

                            ///
                            var viewmodel = new 자재환입저울스캔팝업ViewModel(enumScanType.Scale)
                            {
                                ParentVM = this,
                                parentODInfo = _mainWnd.CurrentOrder
                            };

                            자재환입저울스캔팝업 ScanPopup = new 자재환입저울스캔팝업()
                            {
                                DataContext = viewmodel
                            };
                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();
                            ///

                            CommandResults["ScaleScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScaleScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScaleScanCommand"] = true;
                        }
                    }, arg =>
                  {
                      return CommandCanExecutes.ContainsKey("ScaleScanCommand") ?
                          CommandCanExecutes["ScaleScanCommand"] : (CommandCanExecutes["ScaleScanCommand"] = true);
                  });
            }
        }
        public ICommand QuantityInsertCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandCanExecutes["QuantityInsertCommand"] = false;
                        CommandResults["QuantityInsertCommand"] = false;

                        ///
                        var viewmodel = new 자재환입저울스캔팝업ViewModel(enumScanType.Quantity)
                        {
                            ParentVM = this,
                            parentODInfo = _mainWnd.CurrentOrder
                        };

                        자재환입저울스캔팝업 ScanPopup = new 자재환입저울스캔팝업()
                        {
                            DataContext = viewmodel
                        };
                        ScanPopup.Closed += ScanPopup_Closed;
                        ScanPopup.Show();
                        ///

                        CommandResults["QuantityInsertCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["QuantityInsertCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["QuantityInsertCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("QuantityInsertCommand") ?
                        CommandCanExecutes["QuantityInsertCommand"] : (CommandCanExecutes["QuantityInsertCommand"] = true);
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
                        IsBusy = true;
                        _DispatcherTimer.Stop();

                        var ReturnList = PackingMTRLList.Where(x => x.RESULT == PackingMTRL.enumStatusType.환입대상 || x.RESULT == PackingMTRL.enumStatusType.프린트실패);

                        if (ReturnList.Count() != PackingMTRLList.Count)
                        {
                            if (await OnMessageAsync("처리완료 되지 않은 자재가 있습니다.\n환입불요 처리 후 계속 진행하시겠습니까?", true) == false)
                                throw new Exception("취소했습니다.");
                        }

                        var authHelper = new iPharmAuthCommandHelper();

                        // 재기록 하는 경우 
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
                            string.Format("WMS자재환입처리"),
                            string.Format("WMS자재환입처리"),
                            false,
                            "OM_ProductionOrder_SUI",
                            _mainWnd.CurrentOrder.EquipmentID, _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                        {
                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                        }

                        _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATAs.Clear();
                        _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.OUTDATAs.Clear();

                        decimal temp;
                        foreach (var item in ReturnList)
                        {
                            if(item.RESULT == PackingMTRL.enumStatusType.환입대상)
                            {
                                _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATAs.Add(new BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    COMPONENTGUID = item.COMPONENTGUID,
                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                    RETURNQTY = decimal.TryParse(item.RETURNQTY, out temp) ? temp : 0m,
                                    RTN_NOTE = "",
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                });
                            }
                        }

                        if (await _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.Execute() == true)
                        {
                            foreach (var item in ReturnList)
                                item.RESULT = PackingMTRL.enumStatusType.자재환입;                            
                        }
                        else
                            throw _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI.Exception;


                        // 포장자재반납라벨 출력
                        if (!string.IsNullOrWhiteSpace(this.PrintName))
                        {
                            foreach (var item in PackingMTRLList)
                            {
                                if (item.RESULT == PackingMTRL.enumStatusType.자재환입)
                                {
                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                    _BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();
                                    DateTime NOW = await AuthRepositoryViewModel.GetDBDateTimeNow();


                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                    {
                                        ReportPath = "/Reports/LABEL/LABEL_C0402_018_16",
                                        PrintName = PrintName,
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

                                    if (await _BR_PHR_SEL_PRINT_LabelImage.Execute() != true)
                                        item.RESULT = PackingMTRL.enumStatusType.프린트실패;
                                }
                            }
                        }

                        // XML 생성
                        DataSet ds = new DataSet();
                        DataTable dt = new DataTable("DATA");
                        ds.Tables.Add(dt);

                        dt.Columns.Add(new DataColumn("MTRLID"));
                        dt.Columns.Add(new DataColumn("MTRLNAME"));
                        dt.Columns.Add(new DataColumn("MSUBLOTBCD"));
                        dt.Columns.Add(new DataColumn("PARAM"));
                        dt.Columns.Add(new DataColumn("WEIGHT"));
                        dt.Columns.Add(new DataColumn("UOM"));
                        dt.Columns.Add(new DataColumn("RETURNRSLT"));
                        dt.Columns.Add(new DataColumn("RETURNUOM"));
                        dt.Columns.Add(new DataColumn("STATUS"));

                        foreach (var item in PackingMTRLList)
                        {
                            if (item.RESULT == PackingMTRL.enumStatusType.자재환입 || item.RESULT == PackingMTRL.enumStatusType.프린트실패)
                            {
                                var row = dt.NewRow();
                                row["MTRLID"] = item.MTRLID ?? "";
                                row["MTRLNAME"] = item.MTRLNAME ?? "";
                                row["MSUBLOTBCD"] = item.MSUBLOTBCD ?? "";
                                row["PARAM"] = item.PARAM ?? "";
                                row["WEIGHT"] = item.WEIGHT ?? "";
                                row["UOM"] = item.UOM ?? "";
                                row["RETURNRSLT"] = item.RETURNQTY ?? "";
                                row["RETURNUOM"] = item.RETURNUOM ?? "";
                                row["STATUS"] = "환입완료";
                                dt.Rows.Add(row);
                            }
                        }

                        var xml = BizActorRuleBase.CreateXMLStream(ds);
                        var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                        if(ds.Tables["DATA"].Rows.Count > 0)
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

                        _DispatcherTimer.Stop();
                        _DispatcherTimer = null;

                        if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                        else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

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
        #endregion

        #region [Constructor]
        public WMS자재환입처리ViewModel()
        {
            int interval = 2000;

            _BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD = new BR_BRS_CHK_ProductionOrderPickingInfo_MSUBLOTBCD();
            _BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI = new BR_BRS_REG_WMS_S_MATERIALRETURN_MULTI();
            _BR_BRS_GET_UDT_ProductionOrderPickingInfo = new BR_BRS_GET_UDT_ProductionOrderPickingInfo();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;

            _PackingMTRLList = new ObservableCollection<PackingMTRL>();
        }
        #endregion

        #region [Events]
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                _DispatcherTimer.Stop();

                if (SCALEID != null && SCALEID != "저울미사용")
                {
                    BR_PHR_SEL_CurrentWeight current_wight = new BR_PHR_SEL_CurrentWeight();
                    current_wight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                    {
                        ScaleID = SCALEID.ToUpper()
                    });

                    if (await current_wight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true)
                    {
                        ScaleWeight = decimal.Parse(current_wight.OUTDATAs[0].Weight.ToString());
                        ScaleUOM = current_wight.OUTDATAs[0].UOM;
                        SCALEVAL = current_wight.OUTDATAs[0].Weight.ToString();
                    }

                    if (IsBusyForWeight)
                    {
                        _DispatcherTimer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusyForWeight = false;
            }

        }
        void ScanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 자재환입저울스캔팝업;
            popup.Closed -= ScanPopup_Closed;

            //저울타이머
            if (SCALEID != null)
            {
                _DispatcherTimer.Start();
                IsBusyForWeight = true;
            }
            else
            {
                _DispatcherTimer.Stop();
                IsBusyForWeight = false;
            }
        }
        #endregion

        #region [User Define Function]
        public class PackingMTRL : ViewModelBase
        {
            public enum enumStatusType
            {
                대기,
                환입대상,
                자재환입,
                프린트실패
            };

            #region 0.자재정보
            private string _MTRLID = "";
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    NotifyPropertyChanged();
                }
            }
            private string _MTRLNAME = "";
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    NotifyPropertyChanged();
                }
            }
            private string _MSUBLOTBCD = "";
            public string MSUBLOTBCD
            {
                get { return _MSUBLOTBCD; }
                set
                {
                    _MSUBLOTBCD = value;
                    NotifyPropertyChanged();
                }
            }
            private string _COMPONENTGUID = "";
            public string COMPONENTGUID
            {
                get { return _COMPONENTGUID; }
                set
                {
                    _COMPONENTGUID = value;
                    NotifyPropertyChanged();
                }
            }
            #endregion

            private enumStatusType _RESULT = 0;
            public enumStatusType RESULT
            {
                get { return _RESULT; }
                set
                {
                    _RESULT = value;
                    NotifyPropertyChanged();
                }
            }
            #region 1.저울 or keyin 정보
            private string _SCALEID = "";
            public string SCALEID
            {
                get { return _SCALEID; }
                set
                {
                    _SCALEID = value;
                    NotifyPropertyChanged();
                }
            }

            private string _WEIGHT = "";
            public string WEIGHT
            {
                get { return _WEIGHT; }
                set
                {
                    _WEIGHT = value;
                    NotifyPropertyChanged();
                }
            }
            #endregion
            #region 2.환입정보
            private string _UOM = "";
            public string UOM
            {
                get { return _UOM; }
                set
                {
                    _UOM = value;
                    NotifyPropertyChanged();
                }
            }

            private string _PARAM = "";
            public string PARAM
            {
                get { return _PARAM; }
                set
                {
                    _PARAM = value;
                    NotifyPropertyChanged();
                }
            }

            private string _RETURNQTY = "";
            public string RETURNQTY
            {
                get { return _RETURNQTY; }
                set
                {
                    _RETURNQTY = value;
                    NotifyPropertyChanged();
                }
            }

            private string _RETURNUOM = "";
            public string RETURNUOM
            {
                get { return _RETURNUOM; }
                set
                {
                    _RETURNUOM = value;
                    NotifyPropertyChanged();
                }
            }
            #endregion
            /// <summary>
            /// status : 자재환입 or 대기
            /// </summary>
            public void setPackingMTRL(WMS자재환입처리ViewModel vm, enumStatusType status)
            {
                this.RESULT = status;// status;
                this.SCALEID = vm.SCALEID;
                this.WEIGHT = vm.SCALEVAL;
                this.UOM = vm.ScaleUOM;
                this.PARAM = vm.PARAM;
                this.RETURNQTY = vm.RETURNQTY;
                this.RETURNUOM = vm.ReturnUOM;
            }
            public void getPackingMTRL(WMS자재환입처리ViewModel vm)
            {
                vm.SCALEID = this.SCALEID;
                vm.SCALEVAL = this.WEIGHT;
                vm.ScaleUOM = this.UOM;
                vm.PARAM = this.PARAM;
                vm.RETURNQTY = this.RETURNQTY;
                vm.ReturnUOM = this.RETURNUOM;
            }
        }
        
        public void ConvertValue()
        {
            decimal param;
            if (ScaleWeight.HasValue)
            {
                if (decimal.TryParse(PARAM, out param) && param > 0)
                {
                    RETURNQTY = (Convert.ToDecimal(ScaleWeight.Value) / param).ToString("0.##0");
                    _mainWnd.btnReturn.IsEnabled = true;
                }
            }
        }
        /// <summary>
        /// 피킹수량과 환입량을 비교
        /// </summary>
        public async void CompareRETRUNQTYwithINVQTY(PackingMTRL target)
        {
            try
            {
                _DispatcherTimer.Stop();

                // 피킹수량보다 환입량이 큰 경우 피킹수량으로 환입정보가 전송됨(해당 로직은 BR에서 수행)
                decimal invQTY = MSUBLOTQTY;
                decimal returnQTY = decimal.TryParse(this.RETURNQTY, out returnQTY) ? returnQTY : 0m;  
                if (returnQTY > invQTY)
                {
                    if (await OnMessageAsync(string.Format("피킹수량 보다 환입량이 많습니다.\n(피킹수량 : {0}, 환입량 : {1})", invQTY, returnQTY), true) == true)
                    {
                        RETURNQTY = MSUBLOTQTY.ToString();
                        target.setPackingMTRL(this, PackingMTRL.enumStatusType.환입대상);
                    }                        
                }
                else
                    target.setPackingMTRL(this, PackingMTRL.enumStatusType.환입대상);
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        /// <summary>
        /// 리스트에서 선택한 자재가 변경된 경우 우측 데이터를 새로고침
        /// </summary>
        public void RefreshList(PackingMTRL target)
        {
            try
            {
                _DispatcherTimer.Stop();
                              
                 target.getPackingMTRL(this);
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }        
        #endregion
    }
    public class ListStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WMS자재환입처리ViewModel.PackingMTRL.enumStatusType)value;
            return status.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "NG";
        }
    }
}
