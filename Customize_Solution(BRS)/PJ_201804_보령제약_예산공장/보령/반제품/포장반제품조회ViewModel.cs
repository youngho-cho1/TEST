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
    public class 포장반제품조회ViewModel : ViewModelBase
    {
        #region [Property]
        private 포장반제품조회 _mainWnd;
        #endregion

        #region [BizRule]
        // 포장반제품 조회
        private BR_BRS_SEL_ProductionOrder_HALB _BR_BRS_SEL_ProductionOrder_HALB;
        public BR_BRS_SEL_ProductionOrder_HALB BR_BRS_SEL_ProductionOrder_HALB
        {
            get { return _BR_BRS_SEL_ProductionOrder_HALB; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_HALB = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_HALB");
            }
        }
        #endregion

        #region [Command]
        //초기 데이터 세팅 및 그리드 정보 조회
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
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if (arg == null || !(arg is 포장반제품조회)) return;
                            else
                            {
                                _mainWnd = arg as 포장반제품조회;

                                _mainWnd.BusyIn.IsBusy = true;

                                _BR_BRS_SEL_ProductionOrder_HALB.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrder_HALB.OUTDATAs.Clear();

                                _BR_BRS_SEL_ProductionOrder_HALB.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_HALB.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID
                                });

                                if (await _BR_BRS_SEL_ProductionOrder_HALB.Execute() == true)
                                    _mainWnd.MainDataGrid.Refresh();

                                _mainWnd.BusyIn.IsBusy = false;
                            }
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _mainWnd.BusyIn.IsBusy = false;
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
        // 조회 결과 기록
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
                            _mainWnd.BusyIn.IsBusy = true;

                            if (_BR_BRS_SEL_ProductionOrder_HALB.OUTDATAs.Count > 0)
                            {
                                // 전자서명(기록값 변경)
                                var authHelper = new iPharmAuthCommandHelper();

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

                                // 조회내용 기록
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "포장반제품조회",
                                    "포장반제품조회",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // XML 변환
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);
                                
                                dt.Columns.Add(new DataColumn("자재코드"));
                                dt.Columns.Add(new DataColumn("자재명"));
                                dt.Columns.Add(new DataColumn("유효기한"));
                                dt.Columns.Add(new DataColumn("제조연월일"));

                                foreach (var item in _BR_BRS_SEL_ProductionOrder_HALB.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    
                                    row["자재코드"] = item.MTRLID != null ? item.MTRLID : "";
                                    row["자재명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                    row["유효기한"] = item.VALIDDATE != null ? item.VALIDDATE : "";
                                    row["제조연월일"] = item.PRODDATE != null ? item.PRODDATE : "";

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

                            _mainWnd.BusyIn.IsBusy = false;
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            _mainWnd.BusyIn.IsBusy = false;
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;
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

        #region [Constructor]
        public 포장반제품조회ViewModel()
        {
            _BR_BRS_SEL_ProductionOrder_HALB = new BR_BRS_SEL_ProductionOrder_HALB();
        }
        #endregion
    }
}
