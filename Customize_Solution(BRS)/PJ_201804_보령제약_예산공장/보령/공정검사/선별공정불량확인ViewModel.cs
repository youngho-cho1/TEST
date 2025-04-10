using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public class 선별공정불량확인ViewModel : ViewModelBase
    {
        #region [Property]
        private 선별공정불량확인 _mainWnd;
        public 선별공정불량확인ViewModel()
        {
            _BR_BRS_GET_AutoInspection_Reject_Ratio = new BR_BRS_GET_AutoInspection_Reject_Ratio();
        }
        #endregion
        #region [BizRule]
        private BR_BRS_GET_AutoInspection_Reject_Ratio _BR_BRS_GET_AutoInspection_Reject_Ratio;
        public BR_BRS_GET_AutoInspection_Reject_Ratio BR_BRS_GET_AutoInspection_Reject_Ratio
        {
            get { return _BR_BRS_GET_AutoInspection_Reject_Ratio; }
            set
            {
                _BR_BRS_GET_AutoInspection_Reject_Ratio = value;
                OnPropertyChanged("BR_BRS_GET_AutoInspection_Reject_Ratio");
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

                            if (arg != null && arg is 선별공정불량확인)
                            {
                                _mainWnd = arg as 선별공정불량확인;

                                await SelectTagValue();
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


        public ICommand RequestCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RequestCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RequestCommand"] = false;
                            CommandCanExecutes["RequestCommand"] = false;

                            ///
                            await SelectTagValue(true);
                            ///

                            CommandResults["RequestCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RequestCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RequestCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("RequestCommand") ?
                       CommandCanExecutes["RequestCommand"] : (CommandCanExecutes["RequestCommand"] = true);
               });
            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            var authHelper = new iPharmAuthCommandHelper();
                            // 전자서명 요청
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "선별공정불량확인",
                                "선별공정불량확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("설비코드"));
                            dt.Columns.Add(new DataColumn("태그종류"));
                            dt.Columns.Add(new DataColumn("결과"));

                            foreach (var item in BR_BRS_GET_AutoInspection_Reject_Ratio.OUTDATAs)
                            {
                                var row = dt.NewRow();

                                row["설비코드"] = item.EQPTID ?? "";
                                row["태그종류"] = item.REJTYPE ?? "";
                                row["결과"] = item.RESULT ?? "";

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
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [User Define]
        private async Task SelectTagValue(bool RequestFlag = false)
        {
            try
            {
                var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                BR_BRS_GET_AutoInspection_Reject_Ratio.OUTDATAs.Clear();

                if (RequestFlag == false && bytearray != null)
                {
                    string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                    DataSet ds = new DataSet();
                    ds.ReadXmlFromString(xml);

                    if (ds.Tables.Count == 1 && ds.Tables[0].TableName == "DATA")
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            BR_BRS_GET_AutoInspection_Reject_Ratio.OUTDATAs.Add(new BR_BRS_GET_AutoInspection_Reject_Ratio.OUTDATA
                            {
                                EQPTID = row["설비코드"].ToString(),
                                REJTYPE = row["태그종류"].ToString(),
                                RESULT = row["결과"].ToString()
                            });
                        }
                    }
                }
                else
                {
                    BR_BRS_GET_AutoInspection_Reject_Ratio.INDATAs.Clear();
                    BR_BRS_GET_AutoInspection_Reject_Ratio.OUTDATAs.Clear();
                    BR_BRS_GET_AutoInspection_Reject_Ratio.INDATAs.Add(new BR_BRS_GET_AutoInspection_Reject_Ratio.INDATA
                    {
                        POID = _mainWnd.CurrentOrder.ProductionOrderID ?? "",
                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID ?? "",
                        EQCLID = _mainWnd.CurrentInstruction.Raw.EQCLID ?? ""
                    });

                    await BR_BRS_GET_AutoInspection_Reject_Ratio.Execute(); ;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
