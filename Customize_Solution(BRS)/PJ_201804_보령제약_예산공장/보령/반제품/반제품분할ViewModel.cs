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

namespace 보령
{
    public class 반제품분할ViewModel : ViewModelBase
    {
        #region [Property]
        private 반제품분할 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();

        enum enumScanType
        {
            Output,
            Scale
        };

        private string _FromVesselID;
        public string FromVesselID
        {
            get { return _FromVesselID; }
            set
            {
                _FromVesselID = value;
                OnPropertyChanged("FromVesselID");
            }
        }
        private string _ToVesselID;
        public string ToVesselID
        {
            get { return _ToVesselID; }
            set
            {
                _ToVesselID = value;
                OnPropertyChanged("ToVesselID");
            }
        }
        private string _Tare; // weight + UOM
        public string Tare
        {
            get { return _Tare; }
            set
            {
                _Tare = value;
                OnPropertyChanged("Tare");
            }
        }
        private string _TareWeight;
        public string TareWeight
        {
            get { return _TareWeight; }
            set
            {
                _TareWeight = value;
                NotifyPropertyChanged();
            }
        }
        private string _TareUOM;
        public string TareUOM
        {
            get { return _TareUOM; }
            set
            {
                _TareUOM = value;
                NotifyPropertyChanged();
            }
        }
        private string _TareUOMId;
        public string TareUOMId
        {
            get { return _TareUOMId; }
            set
            {
                _TareUOMId = value;
                OnPropertyChanged("TareUOMId");
            }
        }
        private string _ScaleID;
        public string ScaleID
        {
            get { return _ScaleID; }
            set
            {
                _ScaleID = value;
                OnPropertyChanged("ScaleID");
            }
        }
        private string _Scale; // weight + UOM
        public string Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
                OnPropertyChanged("Scale");
            }
        }
        private string _ScaleWeight;
        public string ScaleWeight
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
        private string _OrderNo;
        public string OrderNo
        {
            get { return _OrderNo; }
            set
            {
                _OrderNo = value;
                NotifyPropertyChanged();
            }
        }
        private string _ProcessSegmentID;
        public string ProcessSegmentID
        {
            get { return _ProcessSegmentID; }
            set
            {
                _ProcessSegmentID = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region [BizRule]
        private BR_BRS_GET_VESSEL_INFO_MTRL _BR_BRS_GET_VESSEL_INFO_MTRL;
        public BR_BRS_GET_VESSEL_INFO_MTRL BR_BRS_GET_VESSEL_INFO_MTRL
        {
            get { return _BR_BRS_GET_VESSEL_INFO_MTRL; }
            set
            {
                _BR_BRS_GET_VESSEL_INFO_MTRL = value;
                NotifyPropertyChanged();
            }
        }
        private BR_BRS_REG_ProductionOrderOUtput_Split _BR_BRS_REG_ProductionOrderOUtput_Split;
        public BR_BRS_REG_ProductionOrderOUtput_Split BR_BRS_REG_ProductionOrderOUtput_Split
        {
            get { return _BR_BRS_REG_ProductionOrderOUtput_Split; }
            set
            {
                _BR_BRS_REG_ProductionOrderOUtput_Split = value;
                NotifyPropertyChanged();
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
                            if (arg != null && arg is 반제품분할)
                            {
                                _mainWnd = arg as 반제품분할;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };

                                FromVesselID = "";
                                ToVesselID = "";
                                Tare = "";
                                TareWeight = "";
                                TareUOM = "";
                                ScaleID = "";
                                Scale = "";
                                ScaleWeight = "";
                                ScaleUOM = "";
                                OrderNo = _mainWnd.CurrentOrder.OrderID;
                                ProcessSegmentID = _mainWnd.CurrentOrder.OrderProcessSegmentID;
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

        public ICommand FromVesselScanCommand
        {
            get
            {
                return new CommandBase(arg =>
                    {
                        try
                        {
                            CommandCanExecutes["FromVesselScanCommand"] = false;
                            CommandResults["FromVesselScanCommand"] = false;

                            ///
                            if (arg != null)
                                FromVesselID = arg.ToString();
                            

                            if (string.IsNullOrWhiteSpace(FromVesselID))
                                throw new Exception("코드를 입력하세요");

                            _BR_BRS_GET_VESSEL_INFO_MTRL.INDATAs.Clear();
                            _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs.Clear();
                            _BR_BRS_GET_VESSEL_INFO_MTRL.INDATAs.Add(new BR_BRS_GET_VESSEL_INFO_MTRL.INDATA
                            {
                                VESSELID = _FromVesselID,
                                POID = _mainWnd.CurrentOrder.OrderID
                            });
                            
                            _BR_BRS_GET_VESSEL_INFO_MTRL.Execute();
                            ///

                            CommandResults["FromVesselScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["FromVesselScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["FromVesselScanCommand"] = true;
                        }
                    }, arg =>
                  {
                      return CommandCanExecutes.ContainsKey("FromVesselScanCommand") ?
                          CommandCanExecutes["FromVesselScanCommand"] : (CommandCanExecutes["FromVesselScanCommand"] = true);
                  });
            }
        }

        public ICommand VesselScanCommand
        {
            get
            {
                return new CommandBase(arg =>
                    {
                        try
                        {
                            CommandCanExecutes["VesselScanCommand"] = false;
                            CommandResults["VesselScanCommand"] = false;
                            
                            ///
                            var viewmodel = new 반제품저울스캔팝업ViewModel(enumScanType.Output)
                            {
                                DivisionVM = this
                            };

                            반제품저울스캔팝업 ScanPopup = new 반제품저울스캔팝업()
                            {
                                DataContext = viewmodel
                            };
                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();
                            ///

                            CommandResults["VesselScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["VesselScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["VesselScanCommand"] = true;
                        }
                    }, arg =>
                  {
                      return CommandCanExecutes.ContainsKey("VesselScanCommand") ?
                          CommandCanExecutes["VesselScanCommand"] : (CommandCanExecutes["VesselScanCommand"] = true);
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
                        var viewmodel = new 반제품저울스캔팝업ViewModel(enumScanType.Scale)
                        {
                            DivisionVM = this,
                        };

                        반제품저울스캔팝업 ScanPopup = new 반제품저울스캔팝업()
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

        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        try
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            CommandResults["ConfirmCommandAsync"] = false;

                            ///
                            float res;

                            if (_BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs.Count <= 0)
                                throw new Exception("조회된 내용이 없습니다.");

                            if (string.IsNullOrWhiteSpace(ToVesselID) || string.IsNullOrWhiteSpace(ScaleID) || !float.TryParse(TareWeight, out res) || !float.TryParse(ScaleWeight, out res))
                                throw new Exception("스캔 결과가 없습니다.");

                            var authHelper = new iPharmAuthCommandHelper(); // 전자서명 후 BR 실행
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("반제품 분할"),
                                string.Format("반제품 분할"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MLOTs.Clear();
                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MSUBLOTs.Clear();
                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_POs.Clear();
                            _BR_BRS_REG_ProductionOrderOUtput_Split.OUTDATAs.Clear();

                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MLOTs.Add(new BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MLOT
                            {
                                MLOTID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MLOTID,
                                MLOTVER = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MLOTVER,
                                MSUBLOTID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MSUBLOTID,
                                MSUBLOTVER = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MSUBLOTVER,
                                USERID = AuthRepositoryViewModel.Instance.LastLoginedUserID,
                                LOCATIONID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].LOCATIONID,
                            });
                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MSUBLOTs.Add(new BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_MSUBLOT
                            {
                                MSUBLOTQTY = float.TryParse(_ScaleWeight,out res) ? (float?)res : null,
                                UOMID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].UOMID,
                                TAREWEIGHT = float.TryParse(_TareWeight,out res) ? (float?)res : null,
                                TAREUOMID = _TareUOMId,
                                TO_VESSELID = _ToVesselID,
                                OLDMSUBLOTQTY = (float?)_BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MSUBLOTQTY,
                                FROM_VESSELID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].VESSELID
                            });
                            _BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_POs.Add(new BR_BRS_REG_ProductionOrderOUtput_Split.INDATA_PO
                            {
                                POID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].POID,
                                OPSGGUID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].OPSGGUID,
                                OUTPUTGUID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].OUTPUTGUID,
                                MTRLID = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].MTRLID,
                                BATCHNO = _BR_BRS_GET_VESSEL_INFO_MTRL.OUTDATAs[0].BATCHNO
                            });

                            if (await _BR_BRS_REG_ProductionOrderOUtput_Split.Execute())
                            {

                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("FROMVESSELID"));
                                dt.Columns.Add(new DataColumn("TOVESSELID"));
                                dt.Columns.Add(new DataColumn("MSUBLOTID"));
                                dt.Columns.Add(new DataColumn("MSUBLOTBCD"));
                                dt.Columns.Add(new DataColumn("MTRLID"));
                                dt.Columns.Add(new DataColumn("BATCHNO"));
                                dt.Columns.Add(new DataColumn("OLDMSUBLOTQTY"));
                                dt.Columns.Add(new DataColumn("MSUBLOTQTY"));
                                dt.Columns.Add(new DataColumn("NOTATION"));

                                foreach (var rowdata in _BR_BRS_REG_ProductionOrderOUtput_Split.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["FROMVESSELID"] = rowdata.FROM_VESSELID != null ? rowdata.FROM_VESSELID : "";
                                    row["TOVESSELID"] = rowdata.TO_VESSELID != null ? rowdata.TO_VESSELID : "";
                                    row["MSUBLOTID"] = rowdata.MSUBLOTID != null ? rowdata.MSUBLOTID : "";
                                    row["MSUBLOTBCD"] = rowdata.MSUBLOTBCD != null ? rowdata.MSUBLOTBCD : "";
                                    row["MTRLID"] = rowdata.MTRLID != null ? rowdata.MTRLID : "";
                                    row["BATCHNO"] = rowdata.BATCHNO != null ? rowdata.BATCHNO : "";
                                    row["OLDMSUBLOTQTY"] = rowdata.OLDMSUBLOTQTY != null ? rowdata.OLDMSUBLOTQTY : 0;
                                    row["MSUBLOTQTY"] = rowdata.MSUBLOTQTY != null ? rowdata.MSUBLOTQTY : 0;
                                    row["NOTATION"] = rowdata.NOTATION != null ? rowdata.NOTATION : "";

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
        #endregion

        #region [Events]
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                _DispatcherTimer.Stop();

                if (ScaleID != null)
                {
                    BR_PHR_SEL_CurrentWeight current_wight = new BR_PHR_SEL_CurrentWeight();
                    current_wight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                    {
                        ScaleID = ScaleID.ToUpper()
                    });

                    if (await current_wight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true)
                    {
                        Scale = string.Format("{0}{1}", decimal.Parse(current_wight.OUTDATAs[0].Weight.ToString()).ToString("0.##0"), current_wight.OUTDATAs[0].UOM);
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
            var popup = sender as 반제품저울스캔팝업;
            popup.Closed -= ScanPopup_Closed;

            //저울타이머
            if (ScaleID != null)
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

        #region [Constructor]
        public 반제품분할ViewModel()
        {
            _BR_BRS_GET_VESSEL_INFO_MTRL = new BR_BRS_GET_VESSEL_INFO_MTRL();
            _BR_BRS_REG_ProductionOrderOUtput_Split = new BR_BRS_REG_ProductionOrderOUtput_Split();

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

        
    }
}
