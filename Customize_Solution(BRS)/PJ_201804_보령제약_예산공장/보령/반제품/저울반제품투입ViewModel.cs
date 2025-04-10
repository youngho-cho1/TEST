using C1.Silverlight.Data;
using Equipment;
using LGCNS.EZMES.Common;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Common = LGCNS.iPharmMES.Common.Common;

namespace 보령
{
    public class 저울반제품투입ViewModel : ViewModelBase
    {
        enum enumScanType
        {
            Output,
            Scale
        };

        public 저울반제품투입ViewModel()
        {
            _BR_BRS_SEL_Scale_ProductionOrderOutput = new BR_BRS_SEL_Scale_ProductionOrderOutput();
            IsEnabled = false;
            LabelChecked = false;

            int interval = 2000;

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);

            //_DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
        }

        저울반제품투입 _mainWnd;

        DispatcherTimer _DispatcherTimer = new DispatcherTimer();

        BR_BRS_SEL_Scale_ProductionOrderOutput _BR_BRS_SEL_Scale_ProductionOrderOutput;
        public BR_BRS_SEL_Scale_ProductionOrderOutput BR_BRS_SEL_Scale_ProductionOrderOutput
        {
            get { return _BR_BRS_SEL_Scale_ProductionOrderOutput; }
            set
            {
                _BR_BRS_SEL_Scale_ProductionOrderOutput = value;
                NotifyPropertyChanged();
            }
        }

        #region [Property]
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


        string _processSegmentID;
        public string ProcessSegmentID
        {
            get { return _processSegmentID; }
            set
            {
                _processSegmentID = value;
                NotifyPropertyChanged();
            }
        }

        string _vesselId;
        public string VesselId
        {
            get { return _vesselId; }
            set
            {
                _vesselId = value;
                NotifyPropertyChanged();
            }
        }

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

        string _scaleValue;
        public string ScaleValue
        {
            get { return _scaleValue; }
            set
            {
                _scaleValue = value;
                NotifyPropertyChanged();
            }
        }

        string _scaleUOM;
        public string ScaleUOM
        {
            get { return _scaleUOM; }
            set
            {
                _scaleUOM = value;
                NotifyPropertyChanged();
            }
        }

        string _scale;
        public string Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                NotifyPropertyChanged();
            }
        }


        string _tareWeight;
        public string TareWeight
        {
            get { return _tareWeight; }
            set
            {
                _tareWeight = value;
                NotifyPropertyChanged();
            }
        }


        string _tareUOM;
        public string TareUOM
        {
            get { return _tareUOM; }
            set
            {
                _tareUOM = value;
                NotifyPropertyChanged();
            }
        }


        string _tare;
        public string Tare
        {
            get { return _tare; }
            set
            {
                _tare = value;
                NotifyPropertyChanged();
            }
        }


        bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        bool _LabelChecked;
        public bool LabelChecked
        {
            get { return _LabelChecked; }
            set
            {
                _LabelChecked = value;
                NotifyPropertyChanged();
            }
        }

        bool _IsBusyForWeight = false;
        public bool IsBusyForWeight
        {
            get { return _IsBusyForWeight; }
            set
            {
                _IsBusyForWeight = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                _DispatcherTimer.Stop();

                if (ScaleId != null)
                {
                    BR_PHR_SEL_CurrentWeight current_wight = new BR_PHR_SEL_CurrentWeight();
                    current_wight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                    {
                        ScaleID = ScaleId.ToUpper()
                    });

                    if (await current_wight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true)
                    {
                        Scale = string.Format("{0}{1}", decimal.Parse(current_wight.OUTDATAs[0].Weight.ToString()).ToString("0.##0"), current_wight.OUTDATAs[0].UOM);
                    }

                    //처음 저울 바코드 인식 후 ScaleValue값 갱신 안됨.
                    ScaleValue = current_wight.OUTDATAs[0].Weight.ToString();

                    if (ScaleValue != null && BR_BRS_SEL_Scale_ProductionOrderOutput.OUTDATAs.Count > 1)
                        IsEnabled = true;
                    else
                        IsEnabled = false;

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
                            if (arg != null && arg is 저울반제품투입)
                            {
                                _mainWnd = arg as 저울반제품투입;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                var instruction = _mainWnd.CurrentInstruction;
                                var phase = _mainWnd.Phase;

                                this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                                this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                                this.ProcessSegmentID = _mainWnd.CurrentOrder.OrderProcessSegmentID;

                                BR_BRS_SEL_Scale_ProductionOrderOutput.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_Scale_ProductionOrderOutput.INDATA()
                                {
                                    LANGID = LogInInfo.LangID,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OUTPUTTYPE = "WIP",
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await BR_BRS_SEL_Scale_ProductionOrderOutput.Execute() == false) return;


                                if (BR_BRS_SEL_Scale_ProductionOrderOutput.OUTDATAs.Count > 0)
                                {

                                    if (ScaleId == null || ScaleId.ToString() == string.Empty)
                                    {
                                        ScanScaleCommandAsync.Execute(this);
                                    }

                                    ScanBinCommandAsync.Execute(this);
                                }
                                else
                                {
                                    throw new Exception(string.Format("투입된 원료가 없습니다."));
                                }
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

        public ICommand ScanBinCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanBinCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ScanBinCommand"] = false;
                            CommandCanExecutes["ScanBinCommand"] = false;

                            ///
                            var viewmodel = new 반제품저울스캔팝업ViewModel(enumScanType.Output)
                            {
                                InsertVM = this,
                            };

                            var ScanPopup = new 반제품저울스캔팝업()
                            {
                                DataContext = viewmodel
                            };
                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();
                            ///

                            CommandResults["ScanBinCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanBinCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanBinCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanBinCommand") ?
                        CommandCanExecutes["ScanBinCommand"] : (CommandCanExecutes["ScanBinCommand"] = true);
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
                            IsBusy = true;

                            CommandResults["ScanScaleCommand"] = false;
                            CommandCanExecutes["ScanScaleCommand"] = false;

                            ///
                            var viewmodel = new 반제품저울스캔팝업ViewModel(enumScanType.Scale)
                            {
                                InsertVM = this,
                            };

                            var ScanPopup = new 반제품저울스캔팝업()
                            {
                                DataContext = viewmodel
                            };
                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();
                            ///

                            CommandResults["ScanScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanScaleCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanScaleCommand") ?
                        CommandCanExecutes["ScanScaleCommand"] : (CommandCanExecutes["ScanScaleCommand"] = true);
                });
            }
        }

        public ICommand LabelPrintClickCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["LabelPrintClickCommand"] = false;
                        CommandCanExecutes["LabelPrintClickCommand"] = false;

                        ///
                        if (arg is bool)
                            LabelChecked = (bool)arg;
                        ///

                        CommandResults["LabelPrintClickCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LabelPrintClickCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LabelPrintClickCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LabelPrintClickCommand") ?
                        CommandCanExecutes["LabelPrintClickCommand"] : (CommandCanExecutes["LabelPrintClickCommand"] = true);
                });
            }
        }




        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;

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

                            string outputID = _mainWnd.CurrentInstruction.Raw.BOMID;
                            if (outputID == null) throw new Exception("레시피에 정의된 OUTPUT이 없습니다.");

                            var bizRuleSelectOutput = new BR_PHR_SEL_ProductionOrderOutput_Define();
                            bizRuleSelectOutput.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                OUTPUTTYPE = "WIP",
                            });

                            if (await bizRuleSelectOutput.Execute() == false) throw bizRuleSelectOutput.Exception;

                            if (bizRuleSelectOutput.OUTDATAs.Count <= 0) throw new Exception("정의된 OUTPUT이 없습니다");

                            var matchedComponent = bizRuleSelectOutput.OUTDATAs.Where(o => o.OUTPUTID == outputID).FirstOrDefault();
                            if (matchedComponent == null) throw new Exception(string.Format("[{0}] 은 정의된 OUTPUT이 아닙니다.", outputID));


                            ///////////////////////////반제품생성//////////////////////////////////////////////////////////////////////
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Output");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "반제품 투입",
                                "반제품 투입",
                                false,
                                "OM_ProductionOrder_Output",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }


                            var item = BR_BRS_SEL_Scale_ProductionOrderOutput.OUTDATA_OUTPUTs.Where(o => o.OUTPUTGUID == matchedComponent.OUTPUTGUID.ToString()).FirstOrDefault();

                            if (item == null) throw new Exception("레시피에 정의된 OUTPUT과 생성된 OUTPUT이 다릅니다.");

                            var bizRule = new BR_BRS_REG_EquipmentStatus_Output_Scale();

                            bizRule.O_INDATAs.Add(new BR_BRS_REG_EquipmentStatus_Output_Scale.O_INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                OUTPUTGUID = matchedComponent.OUTPUTGUID,
                                IS_NEED_VESSELID = "N",
                                IS_ONLY_TARE = "N",
                                INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Output"),
                                VESSELID = item.VESSELID,
                                LOTTYPE = bizRuleSelectOutput.OUTDATAs[0].OUTPUTTYPE,
                                MSUBLOTID = item.MSUBLOTID,
                                MSUBLOTBCD = item.MSUBLOTBCD,
                                MSUBLOTQTY = float.Parse(ScaleValue),
                                REASON = "Adjust Output",
                                TAREUOMID = bizRuleSelectOutput.OUTDATAs[0].UOMID,
                                TAREWEIGHT = float.Parse(TareWeight),
                                IS_NEW = "N",
                            });



                            if (await bizRule.Execute() == false) throw bizRule.Exception;

                            ///////////////////반제품 표시라벨 발행//////////////////////////////////////////////////////////////////////////

                            if (LabelChecked)
                            {
                                var viewModel = new PrinterSelectPopupViewModel() { RoomID = _mainWnd.CurrentOrder.EquipmentID };
                                var popup = new PrinterSelectPopup() { DataContext = viewModel };

                                popup.Closed += async (s, e) =>
                                {
                                    if (popup.DialogResult == true)
                                    {

                                        authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_ReprintLabel");

                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Function,
                                            Common.enumAccessType.Create,
                                            string.Format("반제품 표시라벨 발행"),
                                            "반제품 표시라벨 발행",
                                            false,
                                            "OM_ProductionOrder_ReprintLabel",
                                            "", null, null) == false)
                                        {
                                            _DispatcherTimer.Start();
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        var PrintBizRule = new BR_PHR_SEL_PRINT_LabelImage();
                                        PrintBizRule.INDATAs.Clear();
                                        PrintBizRule.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                                        {
                                            ReportPath = "/Reports/Label/DWS_LABEL_INPROCESS",
                                            PrintName = System.Windows.Browser.HttpUtility.UrlEncode(viewModel.SelectedPrinter.PRINTERNAME),
                                            USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Output")
                                        });

                                        string[,] labelParam = new string[,] { { "POID", System.Windows.Browser.HttpUtility.UrlEncode(_mainWnd.CurrentOrder.OrderID) }, { "VESSELID", "" } };

                                        for (int i = 0; i < labelParam.Rank; i++)
                                        {
                                            PrintBizRule.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                                            {
                                                ParamName = labelParam[i, 0],
                                                ParamValue = labelParam[i, 1]
                                            });
                                        }

                                        if (await PrintBizRule.Execute() == false)
                                        {
                                            _DispatcherTimer.Start();
                                            throw PrintBizRule.Exception;
                                        }

                                        //System.Windows.Browser.ScriptObject startTrans = (System.Windows.Browser.ScriptObject)System.Windows.Browser.HtmlPage.Window.GetProperty("doPrint");

                                        //startTrans.InvokeSelf(
                                        //    "REPORT_PATH=/Reports/Label/DWS_LABEL_INPROCESS" +
                                        //    "&POID=" + System.Windows.Browser.HttpUtility.UrlEncode(_mainWnd.CurrentOrder.OrderID) +
                                        //    "&VESSELID=" + item.VESSELID +
                                        //    "&ORIENTATION=Landscape" +
                                        //    //"&PAGEWIDTH=29.7" +
                                        //    //"&PAGEHEIGHT=21" +
                                        //    "&PRINTER_NAME=" + System.Windows.Browser.HttpUtility.UrlEncode(viewModel.SelectedPrinter.PRINTERNAME) +
                                        //    "&COPIES=" + viewModel.Copies.ToString());

                                    }
                                    _DispatcherTimer.Start();
                                };
                                popup.Show();

                            }

                            /////////////////OUTDATA/////////////////////////////////////////////////////////////////////////////////
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("용기관리번호"));
                            dt.Columns.Add(new DataColumn("자재ID"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("원료배치번호"));
                            dt.Columns.Add(new DataColumn("원료무게"));
                            dt.Columns.Add(new DataColumn("총무게"));


                            foreach (var mtrlitem in _BR_BRS_SEL_Scale_ProductionOrderOutput.OUTDATAs)
                            {
                                var row = dt.NewRow();
                                row["용기관리번호"] = item.VESSELID != null ? item.VESSELID : "";
                                row["자재ID"] = mtrlitem.MTRLID != null ? mtrlitem.MTRLID : "";
                                row["자재명"] = mtrlitem.MTRLNAME != null ? mtrlitem.MTRLNAME : "";
                                row["원료배치번호"] = mtrlitem.MLOTID != null ? mtrlitem.MLOTID : "";
                                row["원료무게"] = mtrlitem.MSUBLOTQTY != null ? decimal.Parse(mtrlitem.MSUBLOTQTY.ToString()).ToString("0.##0") : "";
                                row["총무게"] = Scale != null ? Scale.ToString() : "";
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

                            IsBusyForWeight = false;
                            _DispatcherTimer.Stop();

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);


                            CommandResults["ConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            _DispatcherTimer.Start();

                            CommandResults["ConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommand"] = true;
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


        public ICommand LabelPrintCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["LabelPrintCommand"] = false;
                        CommandCanExecutes["LabelPrintCommand"] = false;

                        ///
                        if (arg != null && arg is BR_BRS_SEL_PMS_ProductionOrderOutput.OUTDATA)
                        {
                            var viewModel = new PrinterSelectPopupViewModel() { RoomID = _mainWnd.CurrentOrder.EquipmentID };
                            var popup = new PrinterSelectPopup() { DataContext = viewModel };

                            popup.Closed += async (s, e) =>
                            {
                                if (popup.DialogResult == true)
                                {
                                    var authHelper = new iPharmAuthCommandHelper();
                                    authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_ReprintLabel");

                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("반제품 표시라벨 발행"),
                                        "반제품 표시라벨 발행",
                                        false,
                                        "OM_ProductionOrder_ReprintLabel",
                                        "", null, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }

                                    var PrintBizRule = new BR_PHR_SEL_PRINT_LabelImage();
                                    PrintBizRule.INDATAs.Clear();
                                    PrintBizRule.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                                    {
                                        ReportPath = "/Reports/Label/DWS_LABEL_INPROCESS",
                                        PrintName = System.Windows.Browser.HttpUtility.UrlEncode(viewModel.SelectedPrinter.PRINTERNAME),
                                        USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_ReprintLabel")
                                    });

                                    string[,] labelParam = new string[,] { { "POID", System.Windows.Browser.HttpUtility.UrlEncode(_mainWnd.CurrentOrder.OrderID) }, { "VESSELID", (arg as BR_BRS_SEL_PMS_ProductionOrderOutput.OUTDATA).VESSELID } };

                                    for (int i = 0; i < labelParam.Rank; i++)
                                    {
                                        PrintBizRule.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                                        {
                                            ParamName = labelParam[i, 0],
                                            ParamValue = labelParam[i, 1]
                                        });
                                    }
                                    if (await PrintBizRule.Execute() == false) throw PrintBizRule.Exception;



                                }
                            };
                            popup.Show();
                        }
                        ///

                        CommandResults["LabelPrintCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LabelPrintCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LabelPrintCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LabelPrintCommand") ?
                        CommandCanExecutes["LabelPrintCommand"] : (CommandCanExecutes["LabelPrintCommand"] = true);
                });
            }
        }


        void actionPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as EM_EquipmentManagement_PerformAction;
            popup.Closed -= actionPopup_Closed;

            //this.LoadedCommand.Execute(null);
        }



        void ScanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 반제품저울스캔팝업;
            popup.Closed -= ScanPopup_Closed;

            //저울타이머
            if (ScaleId != null)
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
    }


}