using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
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
using System.Linq;

namespace 보령
{
    public class 현장칭량실적확인WMS전송ViewModel : ViewModelBase
    {
        #region [Property]
        private 현장칭량실적확인WMS전송 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        private bool IsBusyForWeight;
        private decimal ScaleWeight;

        private string _BatchNo;
        public string BatchNo
        {
            get { return _BatchNo; }
            set
            {
                _BatchNo = value;
                OnPropertyChanged("BatchNo");
            }
        }

        private string _POID;
        public string POID
        {
            get { return _POID; }
            set
            {
                _POID = value;
                OnPropertyChanged("POID");
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

        private string _Scale;
        public string Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
                OnPropertyChanged("Scale");
            }
        }

        string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                OnPropertyChanged("Barcode");
            }
        }

        private bool _isEnConfrim;
        public bool isEnConfrim
        {
            get { return _isEnConfrim; }
            set
            {
                _isEnConfrim = value;
                OnPropertyChanged("isEnConfrim");
            }
        }

        private Visibility _isVOk;
        public Visibility isVOk
        {
            get { return _isVOk; }
            set
            {
                _isVOk = value;
                OnPropertyChanged("isVOk");
            }
        }
        #endregion

        #region [BizRule]

        private BR_BRS_SEL_ProductionOrderWeighingResult _BR_BRS_SEL_ProductionOrderWeighingResult;
        public BR_BRS_SEL_ProductionOrderWeighingResult BR_BRS_SEL_ProductionOrderWeighingResult
        {
            get { return _BR_BRS_SEL_ProductionOrderWeighingResult; }
            set
            {
                _BR_BRS_SEL_ProductionOrderWeighingResult = value;
                NotifyPropertyChanged();
            }
        }
        private BR_BRS_SEND_WMS_WEIGHINGRESULT _BR_BRS_SEND_WMS_WEIGHINGRESULT;
        public BR_BRS_SEND_WMS_WEIGHINGRESULT BR_BRS_SEND_WMS_WEIGHINGRESULT
        {
            get { return _BR_BRS_SEND_WMS_WEIGHINGRESULT; }
            set
            {
                _BR_BRS_SEND_WMS_WEIGHINGRESULT = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_SEL_UDT_ProductionOrderAllocation_Info _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info;
        public BR_BRS_SEL_UDT_ProductionOrderAllocation_Info BR_BRS_SEL_UDT_ProductionOrderAllocation_Info
        {
            get { return _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info; }
            set
            {
                _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_ProductionOrderAllocation_Info");
            }
        }

        private BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY;
        public BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY
        {
            get { return _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY; }
            set
            {
                _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY = value;
                OnPropertyChanged("BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY");
            }
        }

        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        public BR_PHR_SEL_System_Printer BR_PHR_SEL_System_Printer
        {
            get { return _BR_PHR_SEL_System_Printer; }
            set
            {
                _BR_PHR_SEL_System_Printer = value;
                OnPropertyChanged("BR_PHR_SEL_System_Printer");
            }
        }

        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        public BR_PHR_SEL_PRINT_LabelImage BR_PHR_SEL_PRINT_LabelImage
        {
            get { return _BR_PHR_SEL_PRINT_LabelImage; }
            set
            {
                _BR_PHR_SEL_PRINT_LabelImage = value;
                OnPropertyChanged("BR_PHR_SEL_PRINT_LabelImage");
            }
        }

        #endregion

        #region [ICommand]
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandResults["LoadedCommand"] = false;
                        CommandCanExecutes["LoadedCommand"] = false;

                        ///

                        if (arg != null && arg is 현장칭량실적확인WMS전송)
                        {
                            _mainWnd = arg as 현장칭량실적확인WMS전송;
                            _mainWnd.Closed += (s, e) =>
                            {
                                if (_DispatcherTimer != null)
                                    _DispatcherTimer.Stop();

                                _DispatcherTimer = null;
                            };

                            POID = _mainWnd.CurrentOrder.OrderID;
                            BatchNo = _mainWnd.CurrentOrder.BatchNo;

                            _BR_BRS_SEL_ProductionOrderWeighingResult.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderWeighingResult.OUTDATAs.Clear();

                            if (!string.IsNullOrWhiteSpace(_BatchNo) && !string.IsNullOrWhiteSpace(_POID))
                            {
                                _BR_BRS_SEL_ProductionOrderWeighingResult.INDATAs.Add(new BR_BRS_SEL_ProductionOrderWeighingResult.INDATA
                                {
                                    POID = _POID,
                                    WEIGHINGMETHOD = "WH009"
                                });

                                _BR_BRS_SEL_ProductionOrderWeighingResult.Execute();
                            }

                            _BR_PHR_SEL_System_Printer.INDATAs.Clear();
                            _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                ROOMID = AuthRepositoryViewModel.Instance.RoomID
                            });

                            _BR_PHR_SEL_System_Printer.Execute();
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
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        public ICommand ConfirmCommandAsync
        {
             get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["ConfirmCommandAsync"] = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = false;

                        ///
                        if (_BR_BRS_SEL_ProductionOrderWeighingResult.OUTDATAs.Count <= 0)
                            throw new Exception("전송할 내용이 없습니다.");

                        _BR_BRS_SEND_WMS_WEIGHINGRESULT.INDATAs.Clear();

                        _BR_BRS_SEND_WMS_WEIGHINGRESULT.INDATAs.Add(new BR_BRS_SEND_WMS_WEIGHINGRESULT.INDATA
                        {
                            POID = POID,
                            WEIGHINGMETHOD = "WH009"
                        });

                        if (await _BR_BRS_SEND_WMS_WEIGHINGRESULT.Execute())
                        {
                            var authHelper = new iPharmAuthCommandHelper(); // function code 입력
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("칭량실적 WMS전송"),
                                string.Format("칭량실적 WMS전송"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("자재코드"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("원료성적번호"));
                            dt.Columns.Add(new DataColumn("원료시험번호"));
                            dt.Columns.Add(new DataColumn("출고실적수량"));
                            dt.Columns.Add(new DataColumn("기타입고수량"));
                            dt.Columns.Add(new DataColumn("폐기출고수량"));
                            dt.Columns.Add(new DataColumn("WMS전송여부"));

                            foreach (var rowdata in _BR_BRS_SEL_ProductionOrderWeighingResult.OUTDATAs)
                            {
                                var row = dt.NewRow();
                                row["자재코드"] = rowdata.MTRLID != null ? rowdata.MTRLID : "";
                                row["자재명"] = rowdata.MTRLNAME != null ? rowdata.MTRLNAME : "";
                                row["원료성적번호"] = rowdata.MLOTID != null ? rowdata.MLOTID : "";
                                row["원료시험번호"] = rowdata.INSPECTIONNO != null ? rowdata.INSPECTIONNO : "";
                                row["출고실적수량"] = rowdata.PRODUCTQTY != null ? rowdata.PRODUCTQTY : 0;
                                row["기타입고수량"] = rowdata.OTHERINQTY != null ? rowdata.OTHERINQTY : 0;
                                row["폐기출고수량"] = rowdata.SCRAPQTY != null ? rowdata.SCRAPQTY : 0;
                                row["WMS전송여부"] = rowdata.WMS_IF_FLAG != null ? rowdata.WMS_IF_FLAG : "";

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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        public ICommand RemainQtyConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["RemainQtyConfirmCommand"] = false;
                        CommandCanExecutes["RemainQtyConfirmCommand"] = false;

                        var popup = new 잔량확인();
                        popup.DataContext = this;
                        isEnConfrim = false;
                        isVOk = Visibility.Collapsed;
                        Barcode = string.Empty;
                        Scale = string.Empty;
                        ScaleId = string.Empty;

                        popup.Show();


                        CommandResults["RemainQtyConfirmCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["RemainQtyConfirmCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["RemainQtyConfirmCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RemainQtyConfirmCommand") ?
                        CommandCanExecutes["RemainQtyConfirmCommand"] : (CommandCanExecutes["RemainQtyConfirmCommand"] = true);
                });
            }
        }

        public ICommand KeyDownCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["KeyDownCommand"] = false;
                        CommandCanExecutes["KeyDownCommand"] = false;

                        if (arg != null)
                        {
                            var Text = arg as TextBox;
                            Barcode = Text.Text;
                        }

                        _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.INDATAs.Clear();
                        _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.OUTDATAs.Clear();

                        _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.INDATA
                        {
                            POID = _mainWnd.CurrentOrder.OrderID,
                            COMPONENTGUID = null,
                            MSUBLOTBCD = Barcode,
                            MSUBLOTID = null,
                        });

                        await _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.Execute();

                        if (_BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.OUTDATAs.Count > 0)
                        {
                            isVOk = Visibility.Visible;

                            if (Scale != null)
                                isEnConfrim = true;
                        }

                        CommandResults["KeyDownCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["KeyDownCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["KeyDownCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("KeyDownCommand") ?
                        CommandCanExecutes["KeyDownCommand"] : (CommandCanExecutes["KeyDownCommand"] = true);
                });
            }
        }

        public ICommand ScanScaleCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["ScanScaleCommand"] = false;
                        CommandCanExecutes["ScanScaleCommand"] = false;

                        var viewmodel = new ScaleScanPopupViewModel()
                        {
                            Scale = this.Scale,
                            ScaleId = this.ScaleId
                        };

                        var popup = new ScaleScanPopup()
                        {
                            DataContext = viewmodel
                        };
                        popup.Closed += popup_Closed;
                        popup.Show();

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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanScaleCommand") ?
                        CommandCanExecutes["ScanScaleCommand"] : (CommandCanExecutes["ScanScaleCommand"] = true);
                });
            }
        }

        public ICommand ConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["ConfirmCommand"] = false;
                        CommandCanExecutes["ConfirmCommand"] = false;


                        if (ScaleWeight != null && ScaleWeight > 0)
                        {
                            BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                            BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();
                            BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();

                            BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                            {
                                ReportPath = "/Reports/Label/LABEL_REMAINEDQTY",
                                PrintName = System.Windows.Browser.HttpUtility.UrlEncode(_BR_PHR_SEL_System_Printer.OUTDATAs[0].PRINTERNAME),
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            });

                            BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                            {
                                ParamName = "MSUBLOTID",
                                ParamValue = _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.OUTDATAs[0].MSUBLOTID
                            });

                            BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                            {
                                ParamName = "RETURNQTY",
                                ParamValue = Scale
                            });


                            if (await BR_PHR_SEL_PRINT_LabelImage.Execute())
                            {
                                _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY.INDATAs.Clear();
                                _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY.INDATAs.Add(new BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.OrderID,
                                    ALCTGUID = _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.OUTDATAs.Select(o => o.ALCTGUID).FirstOrDefault(),
                                    PACKINGNO = _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info.OUTDATAs.Select(o => o.PACKINGNO).FirstOrDefault(),
                                    OTHERINQTY = ScaleWeight,
                                    SCRAPQTY = null,
                                });

                                await _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY.Execute();

                                _DispatcherTimer.Stop();


                                _BR_BRS_SEL_ProductionOrderWeighingResult.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderWeighingResult.OUTDATAs.Clear();

                                _BR_BRS_SEL_ProductionOrderWeighingResult.INDATAs.Add(new BR_BRS_SEL_ProductionOrderWeighingResult.INDATA
                                {
                                    POID = _POID,
                                    WEIGHINGMETHOD = "WH009"
                                });

                                await _BR_BRS_SEL_ProductionOrderWeighingResult.Execute();
                            }

                        }

                        CommandResults["ConfirmCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ConfirmCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ConfirmCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                        CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
                });
            }
        }

        
        #endregion

        #region [Constructor]
        public 현장칭량실적확인WMS전송ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderWeighingResult = new BR_BRS_SEL_ProductionOrderWeighingResult();
            _BR_BRS_SEND_WMS_WEIGHINGRESULT = new BR_BRS_SEND_WMS_WEIGHINGRESULT();
            _BR_BRS_SEL_UDT_ProductionOrderAllocation_Info = new BR_BRS_SEL_UDT_ProductionOrderAllocation_Info();
            _BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY = new BR_BRS_UPD_WMS_S_WEIGHINGRESULT_QTY();
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
        #endregion

        #region [User Custom Method && Event]

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
                        if (current_wight.OUTDATAs.Count > 0)
                        {
                            ScaleWeight = (decimal)current_wight.OUTDATAs.Select(o => o.Weight).FirstOrDefault();
                            Scale = string.Format("{0}{1}", decimal.Parse(current_wight.OUTDATAs[0].Weight.ToString()).ToString("0.##0"), current_wight.OUTDATAs[0].UOM);
                        }
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

        void popup_Closed(object sender, EventArgs e)
        {
            var popup = sender as ScaleScanPopup;
            popup.Closed -= popup_Closed;

            this.ScaleId = (popup.DataContext as ScaleScanPopupViewModel).ScaleId;
            this.Scale = (popup.DataContext as ScaleScanPopupViewModel).Scale;
            ScaleWeight = (popup.DataContext as ScaleScanPopupViewModel).ScaleWeight;

            //저울타이머
            if (ScaleId != null)
            {
                _DispatcherTimer.Start();
                IsBusyForWeight = true;
                if (isVOk == Visibility.Visible)
                    isEnConfrim = true;
            }
            else
            {
                _DispatcherTimer.Stop();
                IsBusyForWeight = false;
                isEnConfrim = false;
            }
        }

        #endregion
    }
}
