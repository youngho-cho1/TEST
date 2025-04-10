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
using Equipment;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace 보령
{
    public class Autoclave멸균확인ViewModel : ViewModelBase
    {

        #region [Property]
        public Autoclave멸균확인ViewModel()
        {
           
        }
        private Autoclave멸균확인 _mainWnd;
        private List<InstructionModel> refInst;
        
        #endregion
        #region [BizRule]
        private BR_BRS_SEL_EquipmentStatus_PROCEQPT _BR_BRS_SEL_EquipmentStatus_PROCEQPT;        
        #endregion

        #region [Command]
        public ICommand LoadCommandAsync // 생산설비 정보 조회
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadCommandAsync"] = false;
                            CommandCanExecutes["LoadCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (arg != null && arg is Autoclave멸균확인)
                            {
                                this._mainWnd = arg as Autoclave멸균확인;

                                refInst = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Clear();
                                if (refInst.Count > 0)
                                {
                                    foreach (InstructionModel Inst in refInst)
                                    {
                                        if (Inst.Raw.ACTVAL == null)
                                        {
                                            if (Inst.Raw.ACTVAL != "N/A")
                                            {
                                                _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
                                                {
                                                    EQPTID = Inst.Raw.EQPTID
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (Inst.Raw.ACTVAL.ToUpper() != "N/A")
                                            {
                                                _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
                                                {
                                                    EQPTID = Inst.Raw.EQPTID
                                                });
                                            }
                                        }

                                    }
                                }
                            }
                            IsBusy = false;
                            ///

                            CommandResults["LoadCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
                });
            }
        }

        public ICommand ConfirmCommandAsync // Datagird 정보를 XML로 변환하고 창 종료
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

                            //if (BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Count > 0)
                            //{
                            //    var authHelper = new iPharmAuthCommandHelper();

                            //    // Phase 종료후 값 수정 시 전자서명
                            //    if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
                            //    {
                            //        // 전자서명 요청
                            //        authHelper = new iPharmAuthCommandHelper();
                            //        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            //        if (await authHelper.ClickAsync(
                            //            Common.enumCertificationType.Function,
                            //            Common.enumAccessType.Create,
                            //            string.Format("기록값을 변경합니다."),
                            //            string.Format("기록값 변경"),
                            //            true,
                            //            "OM_ProductionOrder_SUI",
                            //            "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                            //        {
                            //            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            //        }
                            //    }

                            //    // Autoclave멸균확인 기록 전자서명
                            //    authHelper = new iPharmAuthCommandHelper();
                            //    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            //    if (await authHelper.ClickAsync(
                            //        Common.enumCertificationType.Role,
                            //        Common.enumAccessType.Create,
                            //        "Autoclave멸균확인",
                            //        "Autoclave멸균확인",
                            //        false,
                            //        "OM_ProductionOrder_SUI",
                            //        "",
                            //        null, null) == false)
                            //    {
                            //        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            //    }

                            //    // XML 변환
                            //    var ds = new DataSet();
                            //    var dt = new DataTable("DATA");
                            //    ds.Tables.Add(dt);
                            //    dt.Columns.Add(new DataColumn("장비명"));
                            //    dt.Columns.Add(new DataColumn("멸균확인시간"));

                            //    foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
                            //    {
                            //        var row = dt.NewRow();
                            //        row["장비명"] = item.EQPTNAME != null ? item.EQPTNAME : "";
                            //        row["멸균확인시간"] = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");
                            //        dt.Rows.Add(row);
                            //    }

                            //    var xml = BizActorRuleBase.CreateXMLStream(ds);
                            //    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            //    _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            //    _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            //    var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            //    if (result != enumInstructionRegistErrorType.Ok)
                            //    {
                            //        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            //    }

                            //    if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            //    else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            //}

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
        #endregion

        #region [Custom]
        #endregion
    }
}
