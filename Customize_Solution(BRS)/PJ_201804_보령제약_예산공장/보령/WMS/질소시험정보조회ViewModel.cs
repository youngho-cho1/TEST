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
using C1.Silverlight.Data;

namespace 보령
{
    public class 질소시험정보조회ViewModel : ViewModelBase
    {
        #region [Property]

        질소시험정보조회 _mainWnd;
        
        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_IFS_SEL_TSTNO _BR_BRS_SEL_IFS_SEL_TSTNO;
        public BR_BRS_SEL_IFS_SEL_TSTNO BR_BRS_SEL_IFS_SEL_TSTNO
        {
            get { return _BR_BRS_SEL_IFS_SEL_TSTNO; }
            set
            {
                _BR_BRS_SEL_IFS_SEL_TSTNO = value;
                OnPropertyChanged("BR_BRS_SEL_IFS_SEL_TSTNO");
            }
        }

        #endregion

        #region [Constructor]

        public 질소시험정보조회ViewModel()
        {
            _BR_BRS_SEL_IFS_SEL_TSTNO = new BR_BRS_SEL_IFS_SEL_TSTNO();
        }

        #endregion

        #region [Command]

        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["LoadedCommand"] = false;
                            CommandResults["LoadedCommand"] = false;
                            IsBusy = true;

                            if (arg != null)
                            {
                                _mainWnd = arg as 질소시험정보조회;

                                _BR_BRS_SEL_IFS_SEL_TSTNO.INDATAs.Clear();
                                _BR_BRS_SEL_IFS_SEL_TSTNO.OUTDATAs.Clear();
                                // 질소 코드는 7910044, 8010215 고정
                                _BR_BRS_SEL_IFS_SEL_TSTNO.INDATAs.Add(new BR_BRS_SEL_IFS_SEL_TSTNO.INDATA
                                {
                                    MTRLID = "7910044"
                                });

                                _BR_BRS_SEL_IFS_SEL_TSTNO.INDATAs.Add(new BR_BRS_SEL_IFS_SEL_TSTNO.INDATA
                                {
                                    MTRLID = "8010215"
                                });

                                await _BR_BRS_SEL_IFS_SEL_TSTNO.Execute();
                            }
                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["LoadedCommand"] = false;
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

        public ICommand ConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;
                            IsBusy = true;


                            // 전자서명 요청
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

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            dt.Columns.Add(new DataColumn("원료코드"));
                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("원료배치번호"));
                            dt.Columns.Add(new DataColumn("시험번호"));

                            if (_BR_BRS_SEL_IFS_SEL_TSTNO.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_SEL_IFS_SEL_TSTNO.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["원료코드"] = item.MTRLID != null ? item.MTRLID.ToString() : "";
                                    row["원료명"] = item.MTRLNAME != null ? item.MTRLNAME.ToString() : "";
                                    row["원료배치번호"] = item.MLOTID != null ? item.MLOTID.ToString() : "";
                                    row["시험번호"] = item.MSUBLOTID != null ? item.MSUBLOTID.ToString() : "";
                                    dt.Rows.Add(row);
                                }
                                ds.Tables.Add(dt);
                            }
                            else
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "기록값 일탈에 대해 서명후 기록을 진행합니다.",
                                    "Deviation Sign",
                                    false,
                                    "OM_ProductionOrder_Deviation",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                var row = dt.NewRow();
                                row["원료코드"] = "N/A";
                                row["원료명"] = "N/A";
                                row["원료배치번호"] = "N/A";
                                row["시험번호"] = "N/A";
                                dt.Rows.Add(row);
                                ds.Tables.Add(dt);
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

                            CommandResults["ConfirmCommand"] = false;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["ConfirmCommand"] = false;
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
    }
}
