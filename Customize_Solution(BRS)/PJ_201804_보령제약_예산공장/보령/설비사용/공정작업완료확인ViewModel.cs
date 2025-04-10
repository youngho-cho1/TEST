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
using System.Collections.Generic;

namespace 보령
{
    public class 공정작업완료확인ViewModel : ViewModelBase
    {
        #region[Property]
        private 공정작업완료확인 _mainWnd;

        public 공정작업완료확인ViewModel()
        {
            _BR_BRS_SEL_ProductOrder_JOB_HIST = new BR_BRS_SEL_ProductOrder_JOB_HIST();
        }
        #endregion
        #region [BizRule]
        // 현재 오더, 공정에서 기록한 JOB 이력을 조회.
        private BR_BRS_SEL_ProductOrder_JOB_HIST _BR_BRS_SEL_ProductOrder_JOB_HIST;
        public BR_BRS_SEL_ProductOrder_JOB_HIST BR_BRS_SEL_ProductOrder_JOB_HIST
        {
            get { return _BR_BRS_SEL_ProductOrder_JOB_HIST; }
            set
            {
                _BR_BRS_SEL_ProductOrder_JOB_HIST = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

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
                            if (arg == null || !(arg is 공정작업완료확인)) return;
                            else
                            {
                                _mainWnd = arg as 공정작업완료확인;

                                _BR_BRS_SEL_ProductOrder_JOB_HIST.INDATAs.Clear();
                                _BR_BRS_SEL_ProductOrder_JOB_HIST.OUTDATAs.Clear();

                                _BR_BRS_SEL_ProductOrder_JOB_HIST.INDATAs.Add(new BR_BRS_SEL_ProductOrder_JOB_HIST.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await BR_BRS_SEL_ProductOrder_JOB_HIST.Execute() == false)
                                    throw BR_BRS_SEL_ProductOrder_JOB_HIST.Exception;
                            }

                            IsBusy = false;

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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

                            // XML 변환
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("항목"));
                            dt.Columns.Add(new DataColumn("시작시간"));
                            dt.Columns.Add(new DataColumn("종료시간"));
                            dt.Columns.Add(new DataColumn("TackTime"));


                            // XML 기록 및 설비사용종료 비즈룰 호출
                            //string userid = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_BRS_EquipmentAction_PROCEND");
                            //userid = !string.IsNullOrWhiteSpace(userid) ? userid : AuthRepositoryViewModel.Instance.LoginedUserID;

                            _BR_BRS_SEL_ProductOrder_JOB_HIST.INDATAs.Clear();

                            foreach (var item in _BR_BRS_SEL_ProductOrder_JOB_HIST.OUTDATAs)
                            {
                                var row = dt.NewRow();

                                row["항목"] = item.EQSJUSER ?? "";
                                row["시작시간"] = item.EQSJSTDTTM;
                                row["종료시간"] = item.EQSJEDDTTM;
                                row["TackTime"] = item.TACKTIME ?? "";

                                dt.Rows.Add(row);
                            }


                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,공정작업완료 확인";
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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
    }
}
