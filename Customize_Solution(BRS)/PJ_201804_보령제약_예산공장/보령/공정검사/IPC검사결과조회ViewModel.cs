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

namespace 보령
{
    public class IPC검사결과조회ViewModel : ViewModelBase
    {
        #region [Property]
        private IPC검사결과조회 _mainWnd;
        public IPC검사결과조회ViewModel()
        {
            _BR_BRS_GET_ProductionOrderTestSpecificationResult = new BR_BRS_GET_ProductionOrderTestSpecificationResult();
        }
        #endregion
        #region [BizRule]
        private BR_BRS_GET_ProductionOrderTestSpecificationResult _BR_BRS_GET_ProductionOrderTestSpecificationResult;
        public BR_BRS_GET_ProductionOrderTestSpecificationResult BR_BRS_GET_ProductionOrderTestSpecificationResult
        {
            get { return _BR_BRS_GET_ProductionOrderTestSpecificationResult; }
            set
            {
                _BR_BRS_GET_ProductionOrderTestSpecificationResult = value;
                NotifyPropertyChanged();
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
                    using (await AwaitableLocks["ConfrimCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            ///
                            IsBusy = true;

                            if(arg != null && arg is IPC검사결과조회)
                            {
                                _mainWnd = arg as IPC검사결과조회;

                                BR_BRS_GET_ProductionOrderTestSpecificationResult.INDATAs.Clear();
                                BR_BRS_GET_ProductionOrderTestSpecificationResult.OUTDATAs.Clear();
                                BR_BRS_GET_ProductionOrderTestSpecificationResult.INDATAs.Add(new BR_BRS_GET_ProductionOrderTestSpecificationResult.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                await BR_BRS_GET_ProductionOrderTestSpecificationResult.Execute();

                                if (BR_BRS_GET_ProductionOrderTestSpecificationResult.OUTDATAs.Count < 1)
                                    throw new Exception("조회된 결과가 없습니다.");
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

        public ICommand ConfrimCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfrimCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfrimCommandAsync"] = false;
                            CommandCanExecutes["ConfrimCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (BR_BRS_GET_ProductionOrderTestSpecificationResult.OUTDATAs.Count > 0)
                            {
                                var authHelper = new iPharmAuthCommandHelper();
                                // Phase 종료 후 기록 수정 시 전자서명
                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
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
                                // 기록 시 전자서명
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "IPC검사결과조회",
                                    "IPC검사결과조회",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("IPCNAME"));
                                dt.Columns.Add(new DataColumn("IPC항목"));
                                dt.Columns.Add(new DataColumn("검사유형"));
                                dt.Columns.Add(new DataColumn("ITEM"));
                                dt.Columns.Add(new DataColumn("BaseLine"));
                                dt.Columns.Add(new DataColumn("Sample"));
                                dt.Columns.Add(new DataColumn("Actual"));
                                dt.Columns.Add(new DataColumn("작업자"));
                                dt.Columns.Add(new DataColumn("입력시간"));
                                dt.Columns.Add(new DataColumn("샘플시간"));
                                dt.Columns.Add(new DataColumn("Comment"));

                                foreach (var item in BR_BRS_GET_ProductionOrderTestSpecificationResult.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["IPCNAME"] = item.TSID ?? "";
                                    row["IPC항목"] = item.TSNAME ?? "";
                                    row["검사유형"] = item.IPCTYPE ?? "";
                                    row["ITEM"] = item.TINAME ?? "";
                                    row["BaseLine"] = item.BASELINE ?? "";
                                    row["Sample"] = item.SMPQTY.HasValue ? item.SMPQTY.Value.ToString() : "";
                                    row["Actual"] = item.ACTVAL ?? "";
                                    row["작업자"] = item.OPERATOR ?? "";
                                    row["입력시간"] = item.INSDATE ?? "";
                                    row["샘플시간"] = item.SMPDATE ?? "";
                                    row["Comment"] = item.COMMENT ?? "";

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
                            else
                            {
                                throw new Exception("조회된 결과가 없습니다.");
                            }
                            ///

                            CommandResults["ConfrimCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfrimCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfrimCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfrimCommandAsync") ?
                        CommandCanExecutes["ConfrimCommandAsync"] : (CommandCanExecutes["ConfrimCommandAsync"] = true);
                });
            }
        }
        #endregion
    }
}
