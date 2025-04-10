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
    public class 라인클리어런스조회ViewModel : ViewModelBase
    {
        #region [Property]
        private 라인클리어런스조회 _mainWnd;
        #endregion
        #region [BizRule]
        // 라인클리어런스 정보 조회
        private BR_BRS_GET_Equipment_LineClearance _BR_BRS_GET_Equipment_LineClearance;
        public BR_BRS_GET_Equipment_LineClearance BR_BRS_GET_Equipment_LineClearance
        {
            get { return _BR_BRS_GET_Equipment_LineClearance; }
            set
            {
                _BR_BRS_GET_Equipment_LineClearance = value;
                OnPropertyChanged("BR_BRS_GET_Equipment_LineClearance");
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
                            if (arg == null || !(arg is 라인클리어런스조회)) return;
                            else
                            {
                                _mainWnd = arg as 라인클리어런스조회;

                                _mainWnd.BusyIn.IsBusy = true;

                                _BR_BRS_GET_Equipment_LineClearance.INDATAs.Clear();
                                _BR_BRS_GET_Equipment_LineClearance.OUTDATAs.Clear();

                                var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                                if (inputValues.Count > 0 && !string.IsNullOrWhiteSpace(inputValues[0].Raw.ACTVAL))
                                {
                                    _BR_BRS_GET_Equipment_LineClearance.INDATAs.Add(new BR_BRS_GET_Equipment_LineClearance.INDATA
                                    {
                                        EQPTID = inputValues[0].Raw.ACTVAL
                                    });
                                }
                                else
                                {
                                    _BR_BRS_GET_Equipment_LineClearance.INDATAs.Add(new BR_BRS_GET_Equipment_LineClearance.INDATA
                                    {
                                        EQPTID = AuthRepositoryViewModel.Instance.RoomID
                                    });
                                }

                                if (await _BR_BRS_GET_Equipment_LineClearance.Execute() == true)
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

                            if (_BR_BRS_GET_Equipment_LineClearance.OUTDATAs.Count > 0)
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
                                    "라인클리어런스조회",
                                    "라인클리어런스조회",
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

                                // 2021.08.20 박희돈 순번 ebr에 안나오도록 변경
                                //dt.Columns.Add(new DataColumn("순번"));
                                dt.Columns.Add(new DataColumn("점검사항"));
                                dt.Columns.Add(new DataColumn("결과"));

                                foreach (var item in _BR_BRS_GET_Equipment_LineClearance.OUTDATAs)
                                {
                                    var row = dt.NewRow();

                                    // 2021.08.20 박희돈 순번 ebr에 안나오도록 변경
                                    //row["순번"] = item.NO != null ? item.NO : "";
                                    row["점검사항"] = item.ITEMNAME != null ? item.ITEMNAME : "";
                                    row["결과"] = item.RESULT != null ? item.RESULT : "";

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
        public 라인클리어런스조회ViewModel()
        {
            _BR_BRS_GET_Equipment_LineClearance = new BR_BRS_GET_Equipment_LineClearance();
        }
        #endregion
    }
}