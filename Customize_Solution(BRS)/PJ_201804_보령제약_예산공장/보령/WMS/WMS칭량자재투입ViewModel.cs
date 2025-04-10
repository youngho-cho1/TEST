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
using System.Collections.ObjectModel;
using System.Linq;
using ShopFloorUI;
using C1.Silverlight.Data;

namespace 보령
{
    public class WMS칭량자재투입ViewModel : ViewModelBase
    {
        #region [Property]
        private WMS칭량자재투입 _mainWnd;

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
        private string _OrderNo;
        public string OrderNo
        {
            get { return _OrderNo; }
            set
            {
                _OrderNo = value;
                OnPropertyChanged("OrderNo");
            }
        }
        private string _ProcessSegmentName;
        public string ProcessSegmentName
        {
            get { return _ProcessSegmentName; }
            set
            {
                _ProcessSegmentName = value;
                OnPropertyChanged("ProcessSegmentName");
            }
        }
        private bool _IsEnable_OKBtn;
        public bool IsEnable_OKBtn
        {
            get { return _IsEnable_OKBtn; }
            set
            {
                _IsEnable_OKBtn = value;
                OnPropertyChanged("IsEnable_OKBtn");
            }
        }
        private Visibility _ScanVisibility;
        public Visibility ScanVisibility
        {
            get { return _ScanVisibility; }
            set
            {
                _ScanVisibility = value;
                OnPropertyChanged("ScanVisibility");
            }
        }

        public bool isConfrim = false;
        #endregion

        #region [Bizrule]
        private BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing;
        public BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing
        {
            get { return _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing; }
            set
            {
                _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing");
            }
        }
        private BR_BRS_REG_MaterialSubLot_Dispense_Charging _BR_BRS_REG_MaterialSubLot_Dispense_Charging;
        public BR_BRS_REG_MaterialSubLot_Dispense_Charging BR_BRS_REG_MaterialSubLot_Dispense_Charging
        {
            get { return _BR_BRS_REG_MaterialSubLot_Dispense_Charging; }
            set
            {
                _BR_BRS_REG_MaterialSubLot_Dispense_Charging = value;
                OnPropertyChanged("BR_BRS_REG_MaterialSubLot_Dispense_Charging");
            }
        }
        private BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.OUTDATACollection filteredComponents
        {
            get { return _filteredComponents; }
            set
            {
                _filteredComponents = value;
                OnPropertyChanged("filteredComponents");
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
                        CommandCanExecutes["LoadedCommand"] = false;
                        CommandResults["LoadedCommand"] = false;
                        ///
                        decimal res;

                        if (arg != null)
                            _mainWnd = arg as WMS칭량자재투입;

                        BatchNo = _mainWnd.CurrentOrder.BatchNo;
                        OrderNo = _mainWnd.CurrentOrder.OrderID;
                        ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;
                        ScanVisibility = Visibility.Visible;
                        var paramInsts = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                        filteredComponents.Clear();

                        _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.INDATAs.Clear();
                        _BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATAs.Clear();

                        _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.INDATAs.Add(new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.INDATA
                        {
                            POID = OrderNo,
                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                            CHGSEQ = decimal.TryParse(_mainWnd.CurrentInstruction.Raw.EXPRESSION, out res) ? res : 0,
                            MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID
                        });

                        foreach (var instruction in paramInsts)
                        {
                            if (decimal.TryParse(instruction.Raw.EXPRESSION, out res))
                            {
                                BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.INDATAs.Add(new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.INDATA()
                                {
                                    POID = OrderNo,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = res,
                                    MTRLID = instruction.Raw.BOMID
                                });
                            }
                        }

                        if (await _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.Execute())
                        {
                            foreach (var item in _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.OUTDATAs)
                            {
                                filteredComponents.Add(item);
                            }
                        }

                        var notCompletedItem = filteredComponents.Where(o =>
                            o.ISAVAIL.Equals("N")).FirstOrDefault();

                        if (notCompletedItem != null)
                        {
                            var viewmodel = new WMS칭량자재투입팝업ViewModel()
                            {
                                ParentVM = this,
                                currentComponent = notCompletedItem
                            };
                            var popup = new WMS칭량자재투입팝업()
                            {
                                DataContext = viewmodel
                            };
                            popup.Closed += (s, e) =>
                            {
                                var notComItem = filteredComponents.Where(o => o.ISAVAIL.Equals("N")).FirstOrDefault();

                                if (notComItem != null && isConfrim)
                                {
                                    var viewmodels = new WMS칭량자재투입팝업ViewModel()
                                    {
                                        ParentVM = this,
                                        currentComponent = notComItem
                                    };
                                    var popups = new WMS칭량자재투입팝업()
                                    {
                                        DataContext = viewmodels
                                    };
                                    isConfrim = false;
                                    popup.Show();
                                }
                            };
                            isConfrim = false;
                            popup.Show();

                        }
                        else
                        {
                            OnMessage("투입할 원료가 없습니다.");
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
        public ICommand ScanCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandCanExecutes["ScanCommand"] = false;
                        CommandResults["ScanCommand"] = false;
                        ///
                        var notCompletedItem = filteredComponents.Where(o =>
                            o.ISAVAIL.Equals("N")).FirstOrDefault();

                        if (notCompletedItem != null)
                        {
                            var viewmodel = new WMS칭량자재투입팝업ViewModel()
                            {
                                ParentVM = this,
                                currentComponent = notCompletedItem
                            };
                            var popup = new WMS칭량자재투입팝업()
                            {
                                DataContext = viewmodel
                            };
                            //popup.btnConfirm.Click += (s, e) =>
                            //{
                            //    var notComItem = filteredComponents.Where(o => o.ISAVAIL.Equals("N")).FirstOrDefault();

                            //    if (notComItem != null && isConfrim)
                            //    {
                            //        var viewmodels = new WMS칭량자재투입팝업ViewModel()
                            //        {
                            //            ParentVM = this,
                            //            currentComponent = notComItem
                            //        };
                            //        var popups = new WMS칭량자재투입팝업()
                            //        {
                            //            DataContext = viewmodels
                            //        };
                            //        isConfrim = false;
                            //        popup.Show();
                            //    }
                            //};
                            popup.Closed += (s, e) =>
                            {
                                var notComItem = filteredComponents.Where(o => o.ISAVAIL.Equals("N")).FirstOrDefault();

                                if (notComItem != null && isConfrim)
                                {
                                    var viewmodels = new WMS칭량자재투입팝업ViewModel()
                                    {
                                        ParentVM = this,
                                        currentComponent = notComItem
                                    };
                                    var popups = new WMS칭량자재투입팝업()
                                    {
                                        DataContext = viewmodels
                                    };
                                    isConfrim = false;
                                    popup.Show();
                                }
                            };
                            isConfrim = false;
                            popup.Show();
                        }
                        else
                        {
                            OnMessage("투입할 원료가 없습니다.");
                        }

                        ///
                        CommandResults["ScanCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ScanCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ScanCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanCommand") ?
                        CommandCanExecutes["ScanCommand"] : (CommandCanExecutes["ScanCommand"] = true);
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
                        _mainWnd.BusyIndicator.IsBusy = true;

                        var chargingItem = filteredComponents.Where(o => o.ISAVAIL.Equals("Y")).ToList();
                        if (chargingItem.Count <= 0) throw new Exception("투입준비가 완료되지 않았습니다.");

                        decimal ckecker;

                        //
                        int cnt = 0;
                        foreach (var item in chargingItem)
                        {
                            _BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                MSUBLOTID = item.MSUBLOTID,
                                MSUBLOTBCD = item.MSUBLOTBCD,
                                MSUBLOTQTY = decimal.TryParse(item.DSP, out ckecker) ? ckecker : 0m,
                                INSUSER = AuthRepositoryViewModel.Instance.LastLoginedUserID,
                                POID = OrderNo,
                                INSDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow(),
                                IS_INVEN_CHARGE = "Y",
                                CHECKINUSER = AuthRepositoryViewModel.Instance.LastLoginedUserID,
                                OPSGGUID = item.OPSGGUID
                            });

                            _BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATA_INVs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATA_INV
                            {
                                COMPONENTGUID = item.COMPONENTGUID
                            });

                            if (cnt == 5)
                            {
                                if (await _BR_BRS_REG_MaterialSubLot_Dispense_Charging.Execute() == true)
                                {
                                    _BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATAs.Clear();
                                    _BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATA_INVs.Clear();
                                    cnt = 0;
                                }

                                else
                                    throw new Exception(_BR_BRS_REG_MaterialSubLot_Dispense_Charging.Exception.Message, _BR_BRS_REG_MaterialSubLot_Dispense_Charging.Exception);
                            }

                            cnt++;
                        }

                        if (_BR_BRS_REG_MaterialSubLot_Dispense_Charging.INDATAs.Count > 0)
                        {
                            if (await _BR_BRS_REG_MaterialSubLot_Dispense_Charging.Execute() == false)
                                throw new Exception(_BR_BRS_REG_MaterialSubLot_Dispense_Charging.Exception.Message, _BR_BRS_REG_MaterialSubLot_Dispense_Charging.Exception);
                        }


                        DataSet ds = new DataSet();
                        DataTable dt = new DataTable("DATA");
                        ds.Tables.Add(dt);

                        dt.Columns.Add(new DataColumn("자재코드"));
                        dt.Columns.Add(new DataColumn("자재명"));
                        dt.Columns.Add(new DataColumn("시험번호"));
                        dt.Columns.Add(new DataColumn("피킹순번")); // 수정 필요
                        dt.Columns.Add(new DataColumn("자재바코드"));
                        dt.Columns.Add(new DataColumn("출고량"));
                        // 2021.08.20 박희돈 투입준비여부 ebr에 안나오도록 변경
                        //dt.Columns.Add(new DataColumn("투입준비여부"));


                        foreach (var rowdata in chargingItem)
                        {
                            var row = dt.NewRow();
                            row["자재코드"] = rowdata.MTRLID != null ? rowdata.MTRLID : "";
                            row["자재명"] = rowdata.MTRLNAME != null ? rowdata.MTRLNAME : "";
                            row["시험번호"] = rowdata.MLOTID != null ? rowdata.MLOTID : "";
                            row["피킹순번"] = rowdata.QCT_NO_SEQ != null ? rowdata.QCT_NO_SEQ : "";
                            row["자재바코드"] = rowdata.MSUBLOTBCD != null ? rowdata.MSUBLOTBCD : "";
                            row["출고량"] = rowdata.DSP != null ? rowdata.DSP + rowdata.UOM : "";
                            // 2021.08.20 박희돈 투입준비여부 ebr에 안나오도록 변경
                            //row["투입준비여부"] = rowdata.ISAVAIL;

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

                        ///
                        CommandResults["ConfirmCommandAsync"] = false;
                    }
                    catch (Exception ex)
                    {
                        _mainWnd.BusyIndicator.IsBusy = false;
                        OnException(ex.Message, ex);
                        CommandResults["ConfirmCommandAsync"] = false;
                    }
                    finally
                    {
                        CommandCanExecutes["ConfirmCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                }
                );
            }
        }
        #endregion

        public WMS칭량자재투입ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing = new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing();
            _BR_BRS_REG_MaterialSubLot_Dispense_Charging = new BR_BRS_REG_MaterialSubLot_Dispense_Charging();
            _filteredComponents = new BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.OUTDATACollection();
            IsEnable_OKBtn = true;
            isConfrim = false;
        }
    }
}
