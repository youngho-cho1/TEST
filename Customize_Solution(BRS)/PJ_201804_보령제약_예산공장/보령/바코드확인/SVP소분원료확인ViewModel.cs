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
    public class SVP소분원료확인ViewModel : ViewModelBase
    {
        #region Properties

        ShopFloorCustomWindow _mainWnd;
        public ShopFloorCustomWindow MainWnd
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
        #endregion

        BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD _BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD;
        public BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD
        {
            get { return _BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD; }
            set { _BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD = value; }
        }
        
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

                            ///
                            if(arg == null)
                                return;

                            MainWnd = arg as ShopFloorCustomWindow;

                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                            this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;

                            // 지시문 Validation
                            if (string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.BOMID))
                                throw new Exception(string.Format("해당 Instruction에 BOM ID가 설정되지 않았습니다."));
                            else if (string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.EXPRESSION))
                                throw new Exception(string.Format("해당 Instruction에 Chg Seq가 설정되지 않았습니다."));

                            BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.INDATAs.Add(new BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.INDATA()
                            {
                                POID = MainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = MainWnd.CurrentOrder.OrderProcessSegmentID,
                                CHGSEQ = MainWnd.CurrentInstruction.Raw.EXPRESSION,
                                MTRLID = MainWnd.CurrentInstruction.Raw.BOMID
                            });

                            if (await BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.Execute() == false)
                            {
                                throw BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.Exception;
                            }
                            else
                            {
                                for (int i = 0; i < BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs.Count; i++)
                                {
                                    BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs[i].UsedWeight = Convert.ToDecimal(BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs[i].MSUBLOTQTY) + Convert.ToDecimal(BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs[i].VESSELWEIGHT);
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

                            if (BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs.Count <= 0) throw new Exception("확인할 원료가 없습니다.");

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
                            dt.Columns.Add(new DataColumn("실중량"));
                            dt.Columns.Add(new DataColumn("용기중량"));
                            dt.Columns.Add(new DataColumn("총중량"));
                            //dt.Columns.Add(new DataColumn("단위"));


                            foreach (var item in BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD.OUTDATAs)
                            {
                                var row = dt.NewRow();
                                row["원료코드"] = item.MTRLID;
                                row["원료명"] = item.MTRLNAME;
                                row["원료시험번호"] = item.MLOTID;
                                row["바코드"] = item.MSUBLOTBCD;
                                row["실중량"] = item.MSUBLOTQTY;
                                row["용기중량"] = item.VESSELWEIGHT;
                                row["총중량"] = item.UsedWeight;
                                dt.Rows.Add(row);

                            }
                            if (dt.Rows.Count > 0)
                            {
                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,원료확인";
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                                ///
                            }else
                            {
                                OnMessage("기록할 원료 정보가 없습니다.");
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

        public SVP소분원료확인ViewModel()
        {
            _BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD = new BR_BRS_SEL_POAllocation_AreaWeighing_CHG_STD();
        }
    }
}


