using LGCNS.iPharmMES.Common;

using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;

namespace 보령
{
    public class 소분원료전량투입ViewModel : ViewModelBase
    {
        #region Properties

        소분원료전량투입 _mainWnd;
        public 소분원료전량투입 MainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
        }

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

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        List<string> _currentBOMID;
        #endregion

        BR_BRS_SEL_ProductionOrder_Component_Summary _BR_BRS_SEL_ProductionOrder_Component_Summary;
        public BR_BRS_SEL_ProductionOrder_Component_Summary BR_BRS_SEL_ProductionOrder_Component_Summary
        {
            get { return _BR_BRS_SEL_ProductionOrder_Component_Summary; }
            set { _BR_BRS_SEL_ProductionOrder_Component_Summary = value; }
        }

        BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATACollection filteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///

                            if (arg == null)
                                return;

                            MainWnd = arg as 소분원료전량투입;
                            _currentBOMID = new List<string>();
                            var paramInsts = InstructionModel.GetParameterSender(MainWnd.CurrentInstruction, MainWnd.Instructions); 
                            decimal tempEXPRESSION;

                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                            this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;
                            
                            filteredComponents.Clear();

                            if (decimal.TryParse(MainWnd.CurrentInstruction.Raw.EXPRESSION, out tempEXPRESSION))
                            {
                                BR_BRS_SEL_ProductionOrder_Component_Summary.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_Summary.INDATA()
                                {
                                    POID = MainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = MainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = tempEXPRESSION,
                                    MTRLID = MainWnd.CurrentInstruction.Raw.BOMID
                                });
                            }

                            foreach (var instruction in paramInsts)
                            {
                                if (decimal.TryParse(instruction.Raw.EXPRESSION, out tempEXPRESSION))
                                {
                                    BR_BRS_SEL_ProductionOrder_Component_Summary.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_Summary.INDATA()
                                    {
                                        POID = MainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = MainWnd.CurrentOrder.OrderProcessSegmentID,
                                        CHGSEQ = decimal.Parse(instruction.Raw.EXPRESSION),
                                        MTRLID = instruction.Raw.BOMID
                                    });
                                }
                            }

                            if (await BR_BRS_SEL_ProductionOrder_Component_Summary.Execute() == false) return;

                            foreach (var outdata in BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATAs)
                            {
                                if (string.IsNullOrWhiteSpace(outdata.MSUBLOTBCD))
                                    outdata.IS_CAN_CHARGING_CHECKED_NAME = "소분안됨";
                                else
                                    outdata.IS_CAN_CHARGING_CHECKED_NAME = (Convert.ToDecimal(outdata.REMAINQTY) <= 0) ? "투입완료" : "투입대기";

                                filteredComponents.Add(outdata);
                            }

                            var notCompletedItem = filteredComponents.Where(o =>
                              o.IS_CAN_CHARGING_CHECKED_NAME == "투입대기").FirstOrDefault();

                            if (notCompletedItem != null)
                            {
                                var viewmodel = new 소분원료전량투입팝업ViewModel()
                                {
                                    ParentVM = this,
                                };
                                var popup = new 소분원료전량투입팝업()
                                {
                                    DataContext = viewmodel
                                };

                                popup.Closed += (s, e) =>
                                {
                                    MainWnd.dgMaterials.Refresh();
                                };

                                popup.Show();
                            }
                            else
                            {
                                OnMessage("투입할 원료가 없습니다.");
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
                            CommandCanExecutes["LoadedCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }

        public ICommand ScanCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ScanCommand"] = false;
                            CommandCanExecutes["ScanCommand"] = false;

                            ///
                            var notCompletedItem = filteredComponents.Where(o =>
                              o.IS_CAN_CHARGING_CHECKED_NAME == "투입대기").FirstOrDefault();

                            if (notCompletedItem != null)
                            {
                                var viewmodel = new 소분원료전량투입팝업ViewModel()
                                {
                                    ParentVM = this,
                                };
                                var popup = new 소분원료전량투입팝업()
                                {
                                    DataContext = viewmodel
                                };
                                popup.Closed += (s, e) =>
                                {
                                    MainWnd.dgMaterials.Refresh();
                                };

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

                            IsBusy = false;
                        }
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
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;

                            ///
                            if (filteredComponents.Count(x => x.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입가능") || x.IS_CAN_CHARGING_CHECKED_NAME.Equals("투입완료")) == filteredComponents.Count)
                            {
                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                                {
                                    // 전자서명 요청
                                    var authHelper = new iPharmAuthCommandHelper();
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

                                        item.IS_CAN_CHARGING_CHECKED_NAME = "투입완료";
                                        item.CHGQTY = item.REMAINQTY;
                                    }
                                }

                                foreach (var item in filteredComponents)
                                {
                                    if (item.IS_CAN_CHARGING_CHECKED_NAME == "투입완료")
                                    {
                                        var row = dt.NewRow();
                                        row["원료코드"] = item.MTRLID != null ? item.MTRLID : "";
                                        row["원료명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                        row["원료시험번호"] = item.MLOTID != null ? item.MLOTID : "";
                                        row["바코드"] = item.MSUBLOTBCD != null ? item.MSUBLOTBCD : "";
                                        row["투입량"] = item.CHGQTY != null ? item.CHGQTY + " " + item.UOM : "";
                                        row["상태"] = item.IS_CAN_CHARGING_CHECKED_NAME != null ? item.IS_CAN_CHARGING_CHECKED_NAME : "";
                                        dt.Rows.Add(row);
                                    }
                                }

                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,원료투입";
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            else
                                OnMessage("스캔하지 않은 원료가 있습니다.");
                            ///

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

                            // 전자서명
                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "전량투입",
                                "전량투입",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //XML 형식으로 저장
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("원료코드"));
                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("원료시험번호"));
                            dt.Columns.Add(new DataColumn("바코드"));
                            dt.Columns.Add(new DataColumn("투입량"));
                            dt.Columns.Add(new DataColumn("상태"));

                            var row = dt.NewRow();       
                            row["원료코드"] = "N/A";
                            row["원료명"] = "N/A";
                            row["원료시험번호"] = "N/A";
                            row["바코드"] = "N/A";
                            row["투입량"] = "N/A";
                            row["상태"] = "N/A";                            
                            dt.Rows.Add(row);

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,원료투입";
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            _mainWnd.Close();

                            //
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

        public 소분원료전량투입ViewModel()
        {
            _BR_BRS_SEL_ProductionOrder_Component_Summary = new BR_BRS_SEL_ProductionOrder_Component_Summary();
            _filteredComponents = new BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATACollection();

            Is_EnableOKBtn = true;
        }
    }
}


