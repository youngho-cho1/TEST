using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.ObjectModel;

namespace 보령
{
    public class 붕해측정ViewModel : ViewModelBase
    {
        #region Property
        public 붕해측정ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderIPCResult = new BR_BRS_SEL_ProductionOrderIPCResult();
            _BR_BRS_SEL_ProductionOrderIPCStandard = new BR_BRS_SEL_ProductionOrderIPCStandard();
            _IPC_RESULTS = new ObservableCollection<EACH_INDATA>();
            _BR_BRS_REG_ProductionOrderTestResult = new BR_BRS_REG_ProductionOrderTestResult();
        }

        private 붕해측정 _mainWnd;
        private string IPC_TSID = "붕해측정";

        private int inx;

        private IPCControlData _IPCData;
        public IPCControlData IPCData
        {
            get { return _IPCData; }
            set
            {
                _IPCData = value;
                OnPropertyChanged("IPCData");
            }
        }

        private ObservableCollection<EACH_INDATA> _IPC_RESULTS;
        public ObservableCollection<EACH_INDATA> IPC_RESULTS
        {
            get { return _IPC_RESULTS; }
            set
            {
                _IPC_RESULTS = value;
                OnPropertyChanged("IPC_RESULTS");
            }
        }

        #endregion
        #region BizRule
        private BR_BRS_SEL_ProductionOrderIPCResult _BR_BRS_SEL_ProductionOrderIPCResult;
        private BR_BRS_SEL_ProductionOrderIPCStandard _BR_BRS_SEL_ProductionOrderIPCStandard;
        private BR_BRS_REG_ProductionOrderTestResult _BR_BRS_REG_ProductionOrderTestResult;
        #endregion
        #region Command

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

                            ///
                            if(arg != null && arg is 붕해측정)
                            {
                                _mainWnd = arg as 붕해측정;

                                // IPC 기준정보 조회
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Add(new BR_BRS_SEL_ProductionOrderIPCStandard.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    TSID = IPC_TSID
                                });

                                if(await _BR_BRS_SEL_ProductionOrderIPCStandard.Execute() && _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs.Count == 1)
                                {
                                    IPCData = IPCControlData.SetIPCControlData(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0]);
                                }

                                inx = 1;
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


        public ICommand RegisterIPCCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RegisterIPCCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RegisterIPCCommandAsync"] = false;
                            CommandCanExecutes["RegisterIPCCommandAsync"] = false;

                            ///

                            string chk = "";

                            if (IPCData.LSL.HasValue && IPCData.USL.HasValue)
                            {
                                if (IPCData.LSL.Value <= Convert.ToDecimal(_IPCData.GetACTVAL) && Convert.ToDecimal(_IPCData.GetACTVAL) <= IPCData.USL.Value)
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }
                            }
                            else if (IPCData.LSL.HasValue)
                            {
                                if (IPCData.LSL.Value <= Convert.ToDecimal(_IPCData.GetACTVAL))
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }
                            }
                            else if (IPCData.USL.HasValue)
                            {
                                if (Convert.ToDecimal(_IPCData.GetACTVAL) <= IPCData.USL.Value)
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }
                            }

                            IPC_RESULTS.Add(new EACH_INDATA()
                            {
                                INX = inx++,
                                IPCVALUE = _IPCData.GetACTVAL,
                                IPCVALUEYN = chk
                            });

                            ///

                            CommandResults["RegisterIPCCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RegisterIPCCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RegisterIPCCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("RegisterIPCCommandAsync") ?
                       CommandCanExecutes["RegisterIPCCommandAsync"] : (CommandCanExecutes["RegisterIPCCommandAsync"] = true);
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
                            if (_IPC_RESULTS.Count > 0)
                            {
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

                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "붕해측정",
                                    "붕해측정",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Clear();
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Clear();
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Clear();

                                string confirmguid = AuthRepositoryViewModel.Instance.ConfirmedGuid;
                                DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_IPC");

                                //XML 형식으로 저장
                                DataSet ds = new DataSet();

                                // DATA : 측정내용
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);
                                dt.Columns.Add(new DataColumn("순번"));
                                dt.Columns.Add(new DataColumn("붕해측정값"));
                                dt.Columns.Add(new DataColumn("적합여부"));

                                // 시험명세 기록
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                                {
                                    POTSRGUID = Guid.NewGuid(),
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPTSGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].OPTSGUID),
                                    OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                    TESTSEQ = null,
                                    STRDTTM = curDttm,
                                    ENDDTTM = curDttm,
                                    EQPTID = null,
                                    INSUSER = user,
                                    INSDTTM = curDttm,
                                    EFCTTIMEIN = curDttm,
                                    EFCTTIMEOUT = curDttm,
                                    MSUBLOTID = null,
                                    REASON = null,
                                    ISUSE = "Y",
                                    ACTIVEYN = "Y",
                                    SMPQTY = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].SMPQTY,
                                    SMPQTYUOMID = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].SMPQTYUOMID,
                                });

                                // 전자서명 코멘트
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                                {
                                    COMMENTTYPE = "CM001",
                                    COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                    TSTYPE = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].TSTYPE,
                                    LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                                });

                                // 측정내용 XML, BR indata
                                foreach (var rowdata in _IPC_RESULTS)
                                {
                                    var row = dt.NewRow();
                                    row["순번"] = rowdata.INX.ToString();
                                    row["붕해측정값"] = rowdata.IPCVALUE != null ? rowdata.IPCVALUE : "";
                                    row["적합여부"] = rowdata.IPCVALUEYN != null ? rowdata.IPCVALUEYN : "";

                                    dt.Rows.Add(row);                                    

                                    // 시험상세결과 기록
                                    _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                    {
                                        POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                        OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].OPTSIGUID),
                                        POTSIRGUID = Guid.NewGuid(),
                                        ACTVAL = rowdata.IPCVALUE.ToString(),
                                        INSUSER = user,
                                        INSDTTM = curDttm,
                                        EFCTTIMEIN = curDttm,
                                        EFCTTIMEOUT = curDttm,
                                        COMMENTGUID = !string.IsNullOrWhiteSpace(confirmguid) ? new Guid(confirmguid) : (Guid?)null,
                                        REASON = null,
                                        ISUSE = "Y",
                                        ACTIVEYN = "Y"
                                    });
                                }


                                if (await _BR_BRS_REG_ProductionOrderTestResult.Execute() == true)
                                {
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
                                }
                            }
                            else
                            {
                                throw new Exception("입력한 정보가 없습니다. 기록 버튼을 클릭하여 추가해 주시기 바랍니다.");
                            }
                            
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


                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("순번"));
                            dt.Columns.Add(new DataColumn("붕해측정"));
                            dt.Columns.Add(new DataColumn("적합여부"));

                            var row = dt.NewRow();
                            row["순번"] = "N/A";
                            row["붕해측정값"] = "N/A";
                            row["적합여부"] = "N/A";
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

        public class EACH_INDATA : BizActorDataSetBase
        {
            private int _INX;
            public int INX
            {
                get { return _INX; }
                set
                {
                    _INX = value;
                    OnPropertyChanged("INX");
                }
            }

            private string _IPCVALUE;
            public string IPCVALUE
            {
                get { return _IPCVALUE; }
                set
                {
                    _IPCVALUE = value;
                    OnPropertyChanged("IPCVALUE");
                }
            }

            private string _IPCVALUEYN;
            public string IPCVALUEYN
            {
                get { return _IPCVALUEYN; }
                set
                {
                    _IPCVALUEYN = value;
                    OnPropertyChanged("IPCVALUEYN");
                }
            }
        }

    }
}
