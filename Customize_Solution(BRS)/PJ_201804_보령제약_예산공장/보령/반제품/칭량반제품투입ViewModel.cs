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
    public class 칭량반제품투입ViewModel :ViewModelBase
    {
        #region [Property]
        칭량반제품투입 _mainWnd;

        private string _VesselId;
        public string VesselId
        {
            get { return _VesselId; }
            set
            {
                _VesselId = value;
                OnPropertyChanged("VesselId");
            }
        }
        #endregion

        #region [BizRule]
        private BR_BRS_SEL_ProductionOrderDispenseOutput _BR_BRS_SEL_ProductionOrderDispenseOutput;
        public BR_BRS_SEL_ProductionOrderDispenseOutput BR_BRS_SEL_ProductionOrderDispenseOutput
        {
            get { return _BR_BRS_SEL_ProductionOrderDispenseOutput; }
            set
            {
                _BR_BRS_SEL_ProductionOrderDispenseOutput = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderDispenseOutput");
            }
        }
        private BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC;
        public BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC
        {
            get { return _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC; }
            set
            {
                _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC = value;
                OnPropertyChanged("BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC");
            }
        }

        #endregion

        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandCanExecutes["LoadedCommand"] = false;
                        CommandResults["LoadedCommand"] = false;

                        ///
                        if (arg != null && arg is 칭량반제품투입)
                            _mainWnd = arg as 칭량반제품투입;

                        _mainWnd.txtVesselId.Focus();
                        ///

                        CommandResults["LoadedCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LoadedCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadedCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        public ICommand SearchCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["SearchCommandAsync"] = false;
                        CommandResults["SearchCommandAsync"] = false;

                        ///
                        _BR_BRS_SEL_ProductionOrderDispenseOutput.INDATAs.Clear();
                        _BR_BRS_SEL_ProductionOrderDispenseOutput.OUTDATAs.Clear();

                        VesselId = arg as string;

                        if (string.IsNullOrWhiteSpace(VesselId))
                        {
                            MessageBox.Show("입력된 내용이 없습니다.");
                        }
                        else
                        {
                            _BR_BRS_SEL_ProductionOrderDispenseOutput.INDATAs.Add(new BR_BRS_SEL_ProductionOrderDispenseOutput.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                VESSELID = arg.ToString()
                            });

                            await _BR_BRS_SEL_ProductionOrderDispenseOutput.Execute();

                        }
                        ///

                        CommandResults["SearchCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["SearchCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["SearchCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SearchCommandAsync") ?
                        CommandCanExecutes["SearchCommandAsync"] : (CommandCanExecutes["SearchCommandAsync"] = true);
                });
            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["ConfirmCommandAsync"] = false;
                        CommandResults["ConfirmCommandAsync"] = false;

                        ///
                        if (BR_BRS_SEL_ProductionOrderDispenseOutput.OUTDATAs.Count > 0)
                        {

                            var authHelper = new iPharmAuthCommandHelper(); // function code 입력
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("칭량반제품투입"),
                                string.Format("칭량반제품투입"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.INDATAs.Clear();

                            foreach (var item in BR_BRS_SEL_ProductionOrderDispenseOutput.OUTDATAs)
                            {
                                BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.INDATA
                                {
                                    LANGID = "ko-KR",
                                    MSUBLOTID = item.MSUBLOTID,
                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI"),
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_INVEN_CHARGE = "N",
                                    CHECKINUSER = "N/A",
                                    IS_OUTPUT = "N",
                                    GUBUN = "RECYCLE",
                                    VESSELID = this.VesselId
                                });
                            }

                            if (await _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.Execute())
                            {
                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("원료코드"));
                                dt.Columns.Add(new DataColumn("원료명"));
                                dt.Columns.Add(new DataColumn("원료시험번호"));
                                dt.Columns.Add(new DataColumn("무게"));
                                dt.Columns.Add(new DataColumn("바코드"));

                                foreach (var item in BR_BRS_SEL_ProductionOrderDispenseOutput.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["원료코드"] = item.MTRLID != null ? item.MTRLID : "";
                                    row["원료명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                    row["원료시험번호"] = item.TST_REQ_NO != null ? item.TST_REQ_NO : "";
                                    row["무게"] = item.MSUBLOTQTYUOM != null ? item.MSUBLOTQTYUOM : "";
                                    row["바코드"] = item.MSUBLOTBCD != null ? item.MSUBLOTBCD : "";

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
                        }
                        else
                            OnMessage("조회된 데이터가 없습니다.");
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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region [Generator]
        public 칭량반제품투입ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderDispenseOutput = new BR_BRS_SEL_ProductionOrderDispenseOutput();
            _BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC = new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC();
        }
        #endregion
    }
}
