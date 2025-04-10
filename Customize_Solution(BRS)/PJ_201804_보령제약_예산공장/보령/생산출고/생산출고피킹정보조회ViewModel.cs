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
using ShopFloorUI;
using System.Collections.Generic;
using System.Linq;
using C1.Silverlight.Data;

namespace 보령
{
    public class 생산출고피킹정보조회ViewModel : ViewModelBase
    {
        #region [Property]

        생산출고피킹정보조회 _MainWnd;

        private string _batchNo;
        public string BatchNo
        {
            get { return _batchNo; }
            set
            {
                _batchNo = value;
                OnPropertyChanged("BatchNo");
            }
        }

        private string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
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
        private bool _btnOkEnabled;
        public bool btnOkEnabled
        {
            get { return _btnOkEnabled; }
            set
            {
                _btnOkEnabled = value;
                OnPropertyChanged("btnOkEnabled");
            }
        }
        #endregion

        #region [Bizrule]

        private BR_BRS_GET_UDT_ProductionOrderPickingInfo _BR_BRS_GET_UDT_ProductionOrderPickingInfo;
        public BR_BRS_GET_UDT_ProductionOrderPickingInfo BR_BRS_GET_UDT_ProductionOrderPickingInfo
        {
            get { return _BR_BRS_GET_UDT_ProductionOrderPickingInfo; }
            set
            {
                _BR_BRS_GET_UDT_ProductionOrderPickingInfo = value;
                OnPropertyChanged("BR_BRS_GET_UDT_ProductionOrderPickingInfo");
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
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///
                            int res;

                            if (arg != null && arg is 생산출고피킹정보조회)
                            {
                                _MainWnd = arg as 생산출고피킹정보조회;

                                var paramInsts = InstructionModel.GetParameterSender(_MainWnd.CurrentInstruction, _MainWnd.Instructions); 

                                this.OrderNo = _MainWnd.CurrentOrder.OrderID;
                                this.BatchNo = _MainWnd.CurrentOrder.BatchNo;
                                this.ProcessSegmentName = _MainWnd.CurrentOrder.OrderProcessSegmentName;
                                btnOkEnabled = false;

                                // 피킹목록조회
                                _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Clear();
                                _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Clear();

                                _BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATAs.Add(new BR_BRS_GET_UDT_ProductionOrderPickingInfo.INDATA
                                {
                                    POID = _MainWnd.CurrentOrder.OrderID,
                                    OPSGGUID = _MainWnd.CurrentOrder.OrderProcessSegmentID,
                                    MTRLID = _MainWnd.CurrentInstruction.Raw.BOMID,
                                    CHGSEQ = Int32.TryParse(_MainWnd.CurrentInstruction.Raw.EXPRESSION,out res) ? (int?)res : null
                                });

                                await _BR_BRS_GET_UDT_ProductionOrderPickingInfo.Execute();

                                if (_BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Count > 0)
                                {
                                    btnOkEnabled = true;
                                }
                                else
                                {
                                    throw new Exception(MessageTable.M[CommonMessageCode._10006].Replace("%1","0"));
                                }
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

                            
                            if (_MainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _MainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                var authHelper = new iPharmAuthCommandHelper();
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("기록값을 변경합니다."),
                                    string.Format("기록값 변경"),
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", _MainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            // 전자서명 요청
                            var AuthHelper = new iPharmAuthCommandHelper();
                            AuthHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await AuthHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format(""),
                                string.Format("생산출고피킹정보조회"),
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("자재코드"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("원료성적번호"));
                            dt.Columns.Add(new DataColumn("피킹순번"));
                            dt.Columns.Add(new DataColumn("자재바코드"));
                            dt.Columns.Add(new DataColumn("출고량"));
                            dt.Columns.Add(new DataColumn("투입준비여부"));

                            if (_BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_GET_UDT_ProductionOrderPickingInfo.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["자재코드"] = item.MTRLID != null ? item.MTRLID : "";
                                    row["자재명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                    row["원료성적번호"] = item.MLOTID != null ? item.MLOTID : "";
                                    row["피킹순번"] = item.QCT_NO_SEQ != null ? item.QCT_NO_SEQ : "";
                                    row["자재바코드"] = item.MSUBLOTBCD != null ? item.MSUBLOTBCD : "";
                                    row["출고량"] = item.PICKING_RESV_QTY != null ? item.PICKING_RESV_QTY : "";
                                    row["투입준비여부"] = item.ISAVAIL != null ? item.ISAVAIL : "";
                                    dt.Rows.Add(row);
                                }
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _MainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,생산출고피킹정보조회";
                            _MainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _MainWnd.Phase.RegistInstructionValue(_MainWnd.CurrentInstruction);

                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _MainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_MainWnd.Dispatcher.CheckAccess()) _MainWnd.DialogResult = true;
                            else _MainWnd.Dispatcher.BeginInvoke(() => _MainWnd.DialogResult = true);
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

        #endregion

        #region [Generator]

        public 생산출고피킹정보조회ViewModel()
        {
            _BR_BRS_GET_UDT_ProductionOrderPickingInfo = new BR_BRS_GET_UDT_ProductionOrderPickingInfo();
        }

        #endregion
    }
}
