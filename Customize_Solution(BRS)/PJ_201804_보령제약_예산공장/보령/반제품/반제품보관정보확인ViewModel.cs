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
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 반제품보관정보확인ViewModel : ViewModelBase
    {
        #region [Property]
        public 반제품보관정보확인ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderOutput_State_Info = new BR_BRS_SEL_ProductionOrderOutput_State_Info();
        }

        반제품보관정보확인 _mainWnd;

        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_ProductionOrderOutput_State_Info _BR_BRS_SEL_ProductionOrderOutput_State_Info;
        public BR_BRS_SEL_ProductionOrderOutput_State_Info BR_BRS_SEL_ProductionOrderOutput_State_Info
        {
            get { return _BR_BRS_SEL_ProductionOrderOutput_State_Info; }
            set
            {
                _BR_BRS_SEL_ProductionOrderOutput_State_Info = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderOutput_State_Info ");
            }
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
                                _mainWnd = arg as 반제품보관정보확인;

                                _BR_BRS_SEL_ProductionOrderOutput_State_Info.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderOutput_State_Info.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderOutput_State_Info.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_State_Info.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                await _BR_BRS_SEL_ProductionOrderOutput_State_Info.Execute();
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

                            if (_BR_BRS_SEL_ProductionOrderOutput_State_Info.OUTDATAs.Count == 0)
                            {
                                OnMessage("기록할 데이터가 없습니다.");
                                return;
                            }

                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

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

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("공정명"));
                            dt.Columns.Add(new DataColumn("관리번호"));
                            dt.Columns.Add(new DataColumn("보관기간기준"));
                            dt.Columns.Add(new DataColumn("보관기간"));
                            dt.Columns.Add(new DataColumn("일탈여부"));

                            string checkDeviation = "N";

                            if (_BR_BRS_SEL_ProductionOrderOutput_State_Info.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrderOutput_State_Info.OUTDATAs)
                                {
                                    if ("Y".Equals(item.DEVIATIONYN))
                                    {
                                        checkDeviation = "Y";
                                    }
                                    var row = dt.NewRow();

                                    row["공정명"] = item.OPSGNAME ?? "";
                                    row["관리번호"] = item.VESSELNAME ?? "";
                                    row["보관기간기준"] = item.STRGSTANDARD ?? "";
                                    row["보관기간"] = item.STRGDATE ?? "";
                                    row["일탈여부"] = item.DEVIATIONYN ?? "";

                                    dt.Rows.Add(row);

                                }

                                if (checkDeviation == "N")
                                {
                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "반제품보관정보확인",
                                        "반제품보관정보확인",
                                        false,
                                        "OM_ProductionOrder_SUI",
                                        "",
                                        null, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }
                                else
                                {
                                    if (await OnMessageAsync("입력값이 기준값을 벗어났습니다. 기록을 진행하시겟습니까?", true) == false) return;

                                    authHelper = new iPharmAuthCommandHelper();

                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                    enumRoleType inspectorRole = enumRoleType.ROLE001;
                                    if (_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE != null && Enum.TryParse<enumRoleType>(_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                                    {
                                    }

                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "기록값 일탈에 대해 서명후 기록을 진행합니다 ",
                                        "Deviation Sign",
                                        true,
                                        "OM_ProductionOrder_Deviation",
                                        "",
                                        this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                        this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                    {
                                        return;
                                    }
                                    _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                    _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");


                                    foreach (var item2 in _BR_BRS_SEL_ProductionOrderOutput_State_Info.OUTDATAs)
                                    {
                                        if ("Y".Equals(item2.DEVIATIONYN))
                                        {
                                            //2023.05.15 박희돈 반제품 보관기간 변경. +1달
                                            var bizrule2 = new BR_BRS_UPD_MaterialSubLot_ChangeQCStateExpiryDate();
                                            bizrule2.INDATA_MAILs.Clear();
                                            bizrule2.INDATA_MLOTs.Clear();
                                            bizrule2.INDATA_MSUBLOTs.Clear();

                                            bizrule2.INDATA_MAILs.Add(
                                                new BR_BRS_UPD_MaterialSubLot_ChangeQCStateExpiryDate.INDATA_MAIL
                                                {
                                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                                    MTRLNAME = _mainWnd.CurrentOrder.MaterialName,
                                                    OLDEXPIRYDTTM = item2.EXPIRYDTTM.Value,
                                                    OPSGNAME = _mainWnd.CurrentOrder.OrderProcessSegmentName,
                                                    BATCHNO = _mainWnd.CurrentOrder.BatchNo,
                                                    USERNAME = AuthRepositoryViewModel.GetUserNameByFunctionCode("OM_ProductionOrder_Deviation")
                                                }
                                                );

                                            bizrule2.INDATA_MLOTs.Add(
                                                new BR_BRS_UPD_MaterialSubLot_ChangeQCStateExpiryDate.INDATA_MLOT
                                                {
                                                    MTRLID = item2.MTRLID,
                                                    MLOTID = item2.MLOTID,
                                                    MLOTVER = item2.MLOTVER,
                                                    REASON = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation"),
                                                    INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation")
                                                }
                                                );

                                            bizrule2.INDATA_MSUBLOTs.Add(
                                                new BR_BRS_UPD_MaterialSubLot_ChangeQCStateExpiryDate.INDATA_MSUBLOT
                                                {
                                                    MSUBLOTID = item2.MSUBLOTID,
                                                    MSUBLOTVER = item2.MSUBLOTVER,
                                                    MSUBLOTSEQ = Convert.ToInt16(item2.MSUBLOTSEQ),
                                                    MSUBLOTSTAT = "Accept",
                                                    //EXPIRYDTTM = item2.EXPIRYDTTM.Value.AddMonths(1)
                                                    EXPIRYDTTM = DateTime.Now.AddMonths(1)
                                                }
                                                );

                                            await bizrule2.Execute();
                                        }
                                    }
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

                                if (checkDeviation == "Y")
                                {

                                    var bizrule = new BR_PHR_REG_InstructionComment();

                                    bizrule.IN_Comments.Add(
                                        new BR_PHR_REG_InstructionComment.IN_Comment
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            COMMENTTYPE = "CM008",
                                            COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation")
                                        }
                                        );
                                    bizrule.IN_IntructionResults.Add(
                                        new BR_PHR_REG_InstructionComment.IN_IntructionResult
                                        {
                                            RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                            ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                            IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                            IRTRSTGUID = _mainWnd.CurrentInstruction.Raw.IRTRSTGUID,
                                            IRTSEQ = (int)_mainWnd.CurrentInstruction.Raw.IRTSEQ
                                        }
                                        );

                                    await bizrule.Execute();
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }

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