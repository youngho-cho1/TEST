using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;

namespace 보령
{
    public class 미사용원료실적전송ViewModel : ViewModelBase
    {
        #region Property
        public 미사용원료실적전송ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp = new BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp();
            _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp = new BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp();
        }
        ShopFloorCustomWindow _mainWnd;
        public ShopFloorCustomWindow MainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
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
        string _MTRLID;
        public string MTRLID
        {
            get { return _MTRLID; }
            set
            {
                _MTRLID = value;
                NotifyPropertyChanged();
            }
        }
        string _MLOTID;
        public string MLOTID
        {
            get { return _MLOTID; }
            set
            {
                _MLOTID = value;
                NotifyPropertyChanged();
            }
        }
        string _MTRLNAME;
        public string MTRLNAME
        {
            get { return _MTRLNAME; }
            set
            {
                _MTRLNAME = value;
                NotifyPropertyChanged();
            }
        }
        string _ATTENTIONNOTE;
        public string ATTENTIONNOTE
        {
            get { return _ATTENTIONNOTE; }
            set
            {
                _ATTENTIONNOTE = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        #region BizRule
        private BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp;
        public BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp
        {
            get { return _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp; }
            set
            {
                _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp;
        public BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp
        {
            get { return _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp; }
            set
            {
                _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// 화면 로딩
        /// </summary>
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
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            MainWnd = arg as ShopFloorCustomWindow;

                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                            this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;

                            _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                WEIGHINGMETHOD = "WH007"
                            });

                            if (await _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.Execute() && _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs)
                                {
                                    MLOTID = item.INSPECTIONNO;
                                    MTRLID = item.MTRLID;
                                    MTRLNAME = item.MTRLNAME;
                                    ATTENTIONNOTE = item.ATTENTIONNOTE;
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
                            //CommandCanExecutes["ConfirmCommand"] = false;

                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            
                            if(BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs.Count > 0)
                            {
                                if (await OnMessageAsync("미사용 원료 실적 전송을 진행하시겠습니까?", true))
                                {
                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "미사용원료실적전송",
                                        "미사용원료실적전송",
                                        false,
                                        "OM_ProductionOrder_SUI",
                                        "",
                                        null, null) == false)

                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }
                                else
                                {
                                    throw new Exception("취소했습니다.");
                                }

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

                                _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.INDATAs.Clear();
                                _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs.Clear();
                                _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    WEIGHINGMETHOD = "WH007"
                                });

                                if (await BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.Execute() && _BR_BRS_REG_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs.Count < 0)
                                {
                                    OnMessage("미사용 원료가 존재하지 않습니다.");
                                }

                                var ds = new DataSet();
                                var dt = new DataTable();
                                ds.Tables.Add(dt);
                                dt.Columns.Add(new DataColumn("자재코드"));
                                dt.Columns.Add(new DataColumn("자재명"));
                                dt.Columns.Add(new DataColumn("성분"));
                                dt.Columns.Add(new DataColumn("원료시험번호"));

                                foreach (var item in BR_BRS_SEL_ProductionOrderOutput_LastSoluction_NosDsp.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["자재코드"] = item.MTRLID;
                                    row["자재명"] = item.MTRLNAME;
                                    row["성분"] = item.ATTENTIONNOTE;
                                    row["원료시험번호"] = item.INSPECTIONNO;
                                    dt.Rows.Add(row);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    var xml = BizActorRuleBase.CreateXMLStream(ds);
                                    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                    _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,미사용원료실적전송";
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
                                {
                                    OnMessage("기록할 원료 정보가 없습니다.");
                                }
                            }
                            else
                            {
                                OnMessage("미사용 원료 정보가 존재하지 않습니다.");
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

                            //XML 형식으로 저장
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("자재코드"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("성분"));
                            dt.Columns.Add(new DataColumn("원료시험번호"));
                            
                            var row = dt.NewRow();
                            row["자재코드"] = "N/A";
                            row["자재명"] = "N/A";
                            row["성분"] = "N/A";
                            row["원료시험번호"] = "N/A";
                            dt.Rows.Add(row);

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

                            _mainWnd.Close();

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


        #endregion
    }
}


