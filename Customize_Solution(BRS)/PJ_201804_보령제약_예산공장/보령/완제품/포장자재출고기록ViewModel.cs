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
using C1.Silverlight.Data;
using ShopFloorUI;

namespace 보령
{
    public class 포장자재출고기록ViewModel : ViewModelBase
    {
        #region [Property]
        private 포장자재출고기록 _mainWnd;
        #endregion
        #region [BizRule]
        private BR_BRS_SEL_ProductionOrder_Component_PickingInfo _BR_BRS_SEL_ProductionOrder_Component_PickingInfo;
        public BR_BRS_SEL_ProductionOrder_Component_PickingInfo BR_BRS_SEL_ProductionOrder_Component_PickingInfo
        {
            get { return _BR_BRS_SEL_ProductionOrder_Component_PickingInfo; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_Component_PickingInfo = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_Component_PickingInfo");
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
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///

                            if (arg != null && arg is 포장자재출고기록)
                            {
                                _mainWnd = arg as 포장자재출고기록;

                                _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.Clear();

                                _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATA
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID != null ? _mainWnd.CurrentOrder.ProductionOrderID : "",
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID != null ? _mainWnd.CurrentOrder.OrderProcessSegmentID : ""                                            
                                        });

                                //var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                //if (inputValues.Count > 0)
                                //{
                                //    decimal temp;

                                //    foreach (var item in inputValues)
                                //    {
                                //        _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATA
                                //        {
                                //            POID = _mainWnd.CurrentOrder.ProductionOrderID != null ? _mainWnd.CurrentOrder.ProductionOrderID : "",
                                //            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID != null ? _mainWnd.CurrentOrder.OrderProcessSegmentID : "",
                                //            CHGSEQ = decimal.TryParse(item.Raw.EXPRESSION, out temp) ? temp : 0m,
                                //            MTRLID = item.Raw.BOMID != null ? item.Raw.BOMID : ""
                                //        });
                                //    }
                                //}
                                //else
                                //{
                                //    _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATA
                                //    {
                                //        POID = _mainWnd.CurrentOrder.ProductionOrderID != null ? _mainWnd.CurrentOrder.ProductionOrderID : "",
                                //        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID != null ? _mainWnd.CurrentOrder.OrderProcessSegmentID : "",
                                //        CHGSEQ = _mainWnd.CurrentInstruction.Raw.EXPRESSION != null ? Convert.ToDecimal(_mainWnd.CurrentInstruction.Raw.EXPRESSION) : 0m,
                                //        MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID != null ? _mainWnd.CurrentInstruction.Raw.BOMID : ""
                                //    });
                                //}
                               
                                await _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.Execute();
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
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }

        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            CommandResults["ComfirmCommandAsync"] = false;

                            ///
                            if (_BR_BRS_SEL_ProductionOrder_Component_PickingInfo != null && _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.Count > 0)
                            {
                                var authHelper = new iPharmAuthCommandHelper();

                                if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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

                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "포장자재출고기록",
                                    "포장자재출고기록",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                             
                                string ACQUIREUSERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                + "(" + AuthRepositoryViewModel.GetUserNameByFunctionCode("OM_ProductionOrder_SUI") + ")";
                                DateTime ACQUIREDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("자재코드"));
                                dt.Columns.Add(new DataColumn("자재명"));
                                dt.Columns.Add(new DataColumn("이론량"));
                                dt.Columns.Add(new DataColumn("시험번호"));
                                dt.Columns.Add(new DataColumn("조정량"));
                                dt.Columns.Add(new DataColumn("출고량"));
                                dt.Columns.Add(new DataColumn("추가량"));
                                dt.Columns.Add(new DataColumn("인계자"));
                                dt.Columns.Add(new DataColumn("인계시간"));
                                dt.Columns.Add(new DataColumn("인수자"));
                                dt.Columns.Add(new DataColumn("인수시간"));

                                foreach (var item in _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs)
                                {
                                    var row = dt.NewRow();

                                    row["자재코드"] = item.MTRLID != null ? item.MTRLID : "";
                                    row["자재명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                    row["이론량"] = item.STD != null ? item.STD : "";
                                    row["시험번호"] = item.MLOTID != null ? item.MLOTID : "";
                                    row["조정량"] = item.ADJUSTQTY != null ? item.ADJUSTQTY : "";
                                    row["출고량"] = item.PICKINGQTY != null ? item.PICKINGQTY : "";
                                    row["추가량"] = item.ADDQTY != null ? item.ADDQTY : "";
                                    row["인계자"] = item.HANDOVERUSERID != null ? item.HANDOVERUSERID : "";
                                    row["인계시간"] = item.HANDOVERDTTM != null ? Convert.ToDateTime(item.HANDOVERDTTM).ToString("yyyy-MM-dd HH:mm") : "";
                                    row["인수자"] = item.INSUERID;
                                    //row["인수시간"] = ACQUIREDTTM.ToString("yyyy-MM-dd HH:mm");
                                    row["인수시간"] = Convert.ToDateTime(item.INSDTTM).ToString("yyyy-MM-dd HH:mm");

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

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Constructor]
        public 포장자재출고기록ViewModel()
        {
            _BR_BRS_SEL_ProductionOrder_Component_PickingInfo = new BR_BRS_SEL_ProductionOrder_Component_PickingInfo();
        }
        #endregion
        #region [User Define]
        
        #endregion
    }
}
