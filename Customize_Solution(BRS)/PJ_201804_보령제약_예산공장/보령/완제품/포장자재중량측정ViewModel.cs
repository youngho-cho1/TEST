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
using C1.Silverlight.Data;
using ShopFloorUI;

namespace 보령
{
    public class 포장자재중량측정ViewModel : ViewModelBase
    {
        #region [Property]
        포장자재중량측정 _mainWnd;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        private bool IsBusyForWeight;
        private decimal ScaleWeight;

        BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATA selectedItem;

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
        private string _Barcode;
        public string Barcode
        {
            get { return _Barcode; }
            set
            {
                _Barcode = value;
                OnPropertyChanged("Barcode");
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
        #endregion

        #region [BizRule]
        private BR_BRS_SEL_ProductionOrderPickingInfo_4Weight _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight;
        public BR_BRS_SEL_ProductionOrderPickingInfo_4Weight BR_BRS_SEL_ProductionOrderPickingInfo_4Weight
        {
            get { return _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight; }
            set
            {
                _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderPickingInfo_4Weight");
            }
        }
        private BR_PHR_INS_MaterialSubLotCustomAttributes_Multi _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi;
        public BR_PHR_INS_MaterialSubLotCustomAttributes_Multi BR_PHR_INS_MaterialSubLotCustomAttributes_Multi
        {
            get { return _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi; }
            set
            {
                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi = value;
                OnPropertyChanged("BR_PHR_INS_MaterialSubLotCustomAttributes_Multi");
            }
        }

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
                        CommandResults["LoadedCommandAsync"] = false;
                        CommandCanExecutes["LoadedCommandAsync"] = false;

                        ///

                        if (arg != null || arg is 포장자재중량측정)
                        {
                            _mainWnd = arg as 포장자재중량측정;
                            _mainWnd.Closed += (s, e) =>
                            {
                                if (_DispatcherTimer != null)
                                    _DispatcherTimer.Stop();

                                _DispatcherTimer = null;
                            };                          

                            _mainWnd.btnRecord.IsEnabled = false;

                            _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs.Clear();

                            _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.INDATAs.Add(new BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID
                            });

                            await _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.Execute();
                        }                           

                        ///
                        CommandResults["LoadedCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandCanExecutes["LoadedCommandAsync"] = true;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadedCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        public ICommand MTRLCheckCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandResults["MTRLCheckCommand"] = false;
                        CommandCanExecutes["MTRLCheckCommand"] = false;

                        ///

                        var popup = new 포장자재확인();
                        popup.DataContext = this;
                        isEnConfrim = false;
                        isVOk = Visibility.Collapsed;
                        Barcode = string.Empty;
                        Scale = string.Empty;
                        ScaleId = string.Empty;

                        popup.Show();

                        ///
                        CommandResults["MTRLCheckCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandCanExecutes["MTRLCheckCommand"] = true;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["MTRLCheckCommand"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("MTRLCheckCommand") ?
                        CommandCanExecutes["MTRLCheckCommand"] : (CommandCanExecutes["MTRLCheckCommand"] = true);
                });
            }
        }
        public ICommand RecordCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["RecordCommandAsync"] = false;
                        CommandCanExecutes["RecordCommandAsync"] = false;

                        ///

                        if (_BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs.Count > 0)
                        {
                            var authHelper = new iPharmAuthCommandHelper(); // function code 입력

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
                                string.Format("포장자재중량측정"),
                                string.Format("포장자재중량측정"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATAs.Clear();
                            _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOTs.Clear();

                            foreach (var item in _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs)
                            {
                                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATAs.Add(new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA
                                {
                                    MTATID = "WEIGHT",
                                    MTATVAL1 = item.WEIGHT != null ? item.WEIGHT : "", // 현재 저울값
                                    MTATVAL2 = null,
                                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                });
                                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOTs.Add(new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOT
                                {
                                    MSUBLOTID = item.MSUBLOTID != null ? item.MSUBLOTID : "",
                                    MSUBLOTVERFROM = item.MSUBLOTVER != null ? item.MSUBLOTVER.ToString() : ""
                                });
                            }

                            if (await _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.Execute() == false) return;

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("자재코드"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("성적번호"));
                            dt.Columns.Add(new DataColumn("자재바코드"));
                            dt.Columns.Add(new DataColumn("피킹수량"));
                            dt.Columns.Add(new DataColumn("중량"));
                            dt.Columns.Add(new DataColumn("무게측정여부"));

                            foreach (var item in _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs)
                            {
                                var row = dt.NewRow();
                                row["자재코드"] = item.MTRLID != null ? item.MTRLID.ToString() : "";
                                row["자재명"] = item.MTRLNAME != null ? item.MTRLNAME.ToString() : "";
                                row["성적번호"] = item.MLOTID != null ? item.MLOTID.ToString() : "";
                                row["자재바코드"] = item.MSUBLOTBCD != null ? item.MSUBLOTBCD.ToString() : "";
                                row["피킹수량"] = item.PICKING_QTY != null ? item.PICKING_QTY.ToString() : "";
                                row["중량"] = item.WEIGHT != null ? item.WEIGHT.ToString() : "";
                                row["무게측정여부"] = item.NEED_WEIGHT != null ? item.NEED_WEIGHT.ToString() : "";

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
                        CommandResults["RecordCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandCanExecutes["RecordCommandAsync"] = true;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["MTRLCheckCommand"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecordCommandAsync") ?
                        CommandCanExecutes["RecordCommandAsync"] : (CommandCanExecutes["RecordCommandAsync"] = true);
                });
            }
        }
        public ICommand KeyDownCommandAsync
        {
            get
            {
                return new CommandBase(arg =>
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

                        foreach (var item in _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs)
                        {
                            if (item.MSUBLOTBCD == Barcode)
                            {
                                selectedItem = item;
                                isVOk = Visibility.Visible;

                                if (Scale != null)
                                    isEnConfrim = true;
                            }
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
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandResults["ScanScaleCommand"] = false;
                        CommandCanExecutes["ScanScaleCommand"] = false;

                        var viewmodel = new 포장자재저울스캔ViewModel()
                        {
                            Scale = this.Scale,
                            ScaleId = this.ScaleId
                        };

                        var popup = new 포장자재저울스캔()
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
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandResults["ConfirmCommand"] = false;
                        CommandCanExecutes["ConfirmCommand"] = false;


                        if (ScaleWeight > 0)
                        {
                            selectedItem.WEIGHT = Scale;
                            selectedItem.NEED_WEIGHT = "Y";
                        }

                        if (CheckNEED_WEIGHT())
                            _mainWnd.btnRecord.IsEnabled = true;

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
        public 포장자재중량측정ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight = new BR_BRS_SEL_ProductionOrderPickingInfo_4Weight();            
            _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi = new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi();

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

        #region [User Define]
        void popup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 포장자재저울스캔;
            popup.Closed -= popup_Closed;

            this.ScaleId = (popup.DataContext as 포장자재저울스캔ViewModel).ScaleId;
            this.Scale = (popup.DataContext as 포장자재저울스캔ViewModel).Scale;
            ScaleWeight = (popup.DataContext as 포장자재저울스캔ViewModel).ScaleWeight;

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

        bool CheckNEED_WEIGHT()
        {
            foreach (var item in _BR_BRS_SEL_ProductionOrderPickingInfo_4Weight.OUTDATAs)
            {
                if (item.NEED_WEIGHT == null || !item.NEED_WEIGHT.Equals("Y"))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
