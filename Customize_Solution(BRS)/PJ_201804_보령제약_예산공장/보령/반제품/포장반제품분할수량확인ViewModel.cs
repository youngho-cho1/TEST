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
    public class 포장반제품분할수량확인ViewModel : ViewModelBase
    {
        #region [Property]
        private 포장반제품분할수량확인 _mainWnd;
        #endregion

        #region [BizRule]
        // 포장반제품 조회
        private BR_BRS_SEL_PACKING_SPLIT_QTY _BR_BRS_SEL_PACKING_SPLIT_QTY;
        public BR_BRS_SEL_PACKING_SPLIT_QTY BR_BRS_SEL_PACKING_SPLIT_QTY
        {
            get { return _BR_BRS_SEL_PACKING_SPLIT_QTY; }
            set
            {
                _BR_BRS_SEL_PACKING_SPLIT_QTY = value;
                OnPropertyChanged("BR_BRS_SEL_PACKING_SPLIT_QTY");
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
                            if (arg == null || !(arg is 포장반제품분할수량확인)) return;
                            else
                            {
                                _mainWnd = arg as 포장반제품분할수량확인;

                                _mainWnd.BusyIn.IsBusy = true;

                                _BR_BRS_SEL_PACKING_SPLIT_QTY.INDATAs.Clear();
                                _BR_BRS_SEL_PACKING_SPLIT_QTY.OUTDATAs.Clear();

                                _BR_BRS_SEL_PACKING_SPLIT_QTY.INDATAs.Add(new BR_BRS_SEL_PACKING_SPLIT_QTY.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID
                                });

                                if (await _BR_BRS_SEL_PACKING_SPLIT_QTY.Execute() == true)
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

                            if (_BR_BRS_SEL_PACKING_SPLIT_QTY.OUTDATAs.Count > 0)
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

                                // XML 변환
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);
                                
                                dt.Columns.Add(new DataColumn("오더번호"));
                                dt.Columns.Add(new DataColumn("오더수량"));
                                dt.Columns.Add(new DataColumn("포장단위"));
                                dt.Columns.Add(new DataColumn("이론량"));
                                dt.Columns.Add(new DataColumn("반제품무게"));
                                dt.Columns.Add(new DataColumn("분할무게하한"));
                                dt.Columns.Add(new DataColumn("분할무게"));
                                dt.Columns.Add(new DataColumn("분할무게상한"));

                                foreach (var item in _BR_BRS_SEL_PACKING_SPLIT_QTY.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    
                                    row["오더번호"] = item.POID != null ? item.POID : "";
                                    row["오더수량"] = item.PLANQTY != null ? item.PLANQTY : "";
                                    row["포장단위"] = item.PACK_QTY != null ? item.PACK_QTY : "";
                                    row["이론량"] = item.STDPRODQTY != null ? item.STDPRODQTY : "";
                                    row["반제품무게"] = item.NET_SUM_WEIGHT != null ? item.NET_SUM_WEIGHT : "";
                                    row["분할무게하한"] = item.MIN_CALCULATE != null ? item.MIN_CALCULATE : "";
                                    row["분할무게"] = item.CALCULATE != null ? item.CALCULATE : "";
                                    row["분할무게상한"] = item.MAX_CALCULATE != null ? item.MAX_CALCULATE : "";

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


                            // XML 변환
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("오더번호"));
                            dt.Columns.Add(new DataColumn("오더수량"));
                            dt.Columns.Add(new DataColumn("포장단위"));
                            dt.Columns.Add(new DataColumn("이론량"));
                            dt.Columns.Add(new DataColumn("반제품무게"));
                            dt.Columns.Add(new DataColumn("분할무게하한"));
                            dt.Columns.Add(new DataColumn("분할무게"));
                            dt.Columns.Add(new DataColumn("분할무게상한"));

                            var row = dt.NewRow();

                            row["오더번호"] = "NA";
                            row["오더수량"] = "NA";
                            row["포장단위"] = "NA";
                            row["이론량"] = "NA";
                            row["반제품무게"] = "NA";
                            row["분할무게하한"] = "NA";
                            row["분할무게"] = "NA";
                            row["분할무게상한"] = "NA";

                            dt.Rows.Add(row);

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
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
        #endregion

        #region [Constructor]
        public 포장반제품분할수량확인ViewModel()
        {
            _BR_BRS_SEL_PACKING_SPLIT_QTY = new BR_BRS_SEL_PACKING_SPLIT_QTY();
        }
        #endregion
    }
}
