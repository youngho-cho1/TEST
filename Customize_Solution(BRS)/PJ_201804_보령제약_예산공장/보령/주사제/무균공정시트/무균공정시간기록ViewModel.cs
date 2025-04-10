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
using System.Collections.Generic;
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;
using LGCNS.iPharmMES.Recipe.Common;


namespace 보령
{
    public class 무균공정시간기록ViewModel : ViewModelBase
    {
        #region [Property]
        public 무균공정시간기록ViewModel()
        {
            _BR_BRS_REG_SVP_ASEPTIC_PROCESS = new BR_BRS_REG_SVP_ASEPTIC_PROCESS();
        }

        무균공정시간기록 _mainWnd;

        DateTime _fromDt;
        public DateTime FromDt
        {
            get { return _fromDt; }
            set
            {
                _fromDt = value;
                NotifyPropertyChanged();
            }
        }

        private string _TargetVal;
        public string TargetVal
        {
            get { return _TargetVal; }
            set
            {
                _TargetVal = value;
                OnPropertyChanged("TargetVal");
            }
        }

        private BR_BRS_REG_SVP_ASEPTIC_PROCESS _BR_BRS_REG_SVP_ASEPTIC_PROCESS;
        public BR_BRS_REG_SVP_ASEPTIC_PROCESS BR_BRS_REG_SVP_ASEPTIC_PROCESS
        {
            get { return _BR_BRS_REG_SVP_ASEPTIC_PROCESS; }
            set
            {
                _BR_BRS_REG_SVP_ASEPTIC_PROCESS = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region [Bizrule]

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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is 무균공정시간기록)
                            {
                                _mainWnd = arg as 무균공정시간기록;

                                FromDt = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                string targetVal = _mainWnd.CurrentInstruction.Raw.TARGETVAL;

                                if(string.IsNullOrEmpty(targetVal))
                                {
                                    OnMessage("레시피 정보가 누락되었습니다. Target 정보 확인이 필요합니다.");
                                    return;
                                }

                                TargetVal = targetVal;

                            }

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
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///

                            var authHelper = new iPharmAuthCommandHelper();
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN == "Y" && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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
                            
                            //XML 생성. 비즈룰 INDATA생성
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("기록타입"));
                            dt.Columns.Add(new DataColumn("시각"));
                            dt.Columns.Add(new DataColumn("작업자"));

                            var row = dt.NewRow();

                            row["기록타입"] = TargetVal ?? "";
                            row["시각"] = FromDt.ToString("yyyy-MM-dd HH:mm:ss");
                            row["작업자"] = AuthRepositoryViewModel.Instance.LoginedUserID ?? "";
                            dt.Rows.Add(row);

                            _BR_BRS_REG_SVP_ASEPTIC_PROCESS.INDATAs.Add(new BR_BRS_REG_SVP_ASEPTIC_PROCESS.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.OrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                IRTSEQ = _mainWnd.CurrentInstruction.Raw.IRTSEQ,
                                GUBUN = _mainWnd.CurrentInstruction.Raw.TARGETVAL,
                                TARGETVAL = _fromDt.ToString("yyyy-MM-dd HH:mm:ss"),
                                INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                            });

                            if (await _BR_BRS_REG_SVP_ASEPTIC_PROCESS.Execute())
                            {
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

                            IsBusy = false;
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

                            ///

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
                            
                            //XML 생성. 비즈룰 INDATA생성
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("기록타입"));
                            dt.Columns.Add(new DataColumn("시각"));
                            dt.Columns.Add(new DataColumn("작업자"));

                            var row = dt.NewRow();

                            row["기록타입"] = "N/A";
                            row["시각"] = "N/A";
                            row["작업자"] = "N/A";
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

        #region User Define
        #endregion

    }
}
