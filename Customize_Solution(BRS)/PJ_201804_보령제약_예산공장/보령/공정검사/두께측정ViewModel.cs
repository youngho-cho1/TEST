using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;

namespace 보령
{
    public class 두께측정ViewModel : ViewModelBase
    {
        #region Property
        public 두께측정ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderIPCStandard = new BR_BRS_SEL_ProductionOrderIPCStandard();
            _BR_BRS_REG_ProductionOrderTestResult = new BR_BRS_REG_ProductionOrderTestResult();
            _IPC_RESULTS = new ObservableCollection<EACH_INDATA>();
        }

        private 두께측정 _mainWnd;
        private string IPC_TSID = "두께측정";
        /// <summary>
        /// 기록 회차
        /// </summary>
        private int inx;
        
        private Decimal _MINTHICKNESS;
        public Decimal MINTHICKNESS
        {
            get { return _MINTHICKNESS; }
            set
            {
                _MINTHICKNESS = value;
                OnPropertyChanged("MINTHICKNESS");
            }
        }
        private Decimal _MAXTHICKNESS;
        public Decimal MAXTHICKNESS
        {
            get { return _MAXTHICKNESS; }
            set
            {
                _MAXTHICKNESS = value;
                OnPropertyChanged("MAXTHICKNESS");
            }
        }
        private Decimal _AVERAGETHICKNESS;
        public Decimal AVERAGETHICKNESS
        {
            get { return _AVERAGETHICKNESS; }
            set
            {
                _AVERAGETHICKNESS = value;
                OnPropertyChanged("AVERAGETHICKNESS");
            }
        }

        private string _MINSTANDARD;
        public string MINSTANDARD
        {
            get { return _MINSTANDARD; }
        }

        private string _MAXSTANDARD;
        public string MAXSTANDARD
        {
            get { return _MAXSTANDARD; }
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
        /// <summary>
        /// 개별질량 IPC저장
        /// </summary>

        private BR_BRS_SEL_ProductionOrderIPCStandard _BR_BRS_SEL_ProductionOrderIPCStandard;
        private BR_BRS_REG_ProductionOrderTestResult _BR_BRS_REG_ProductionOrderTestResult;        
        #endregion

        #region Command

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
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///

                            if (arg != null && arg is 두께측정)
                            {
                                _mainWnd = arg as 두께측정;

                                // IPC 기준정보 조회
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderIPCStandard.INDATAs.Add(new BR_BRS_SEL_ProductionOrderIPCStandard.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    TSID = IPC_TSID
                                });

                                await _BR_BRS_SEL_ProductionOrderIPCStandard.Execute();

                                _MINSTANDARD = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].LSL.ToString() != null ? _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].LSL.ToString() : "";
                                _MAXSTANDARD = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].USL.ToString() != null ? _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].USL.ToString() : "";
                                OnPropertyChanged("MINSTANDARD");
                                OnPropertyChanged("MAXSTANDARD");

                                inx = 1;
                            }
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
       

        public ICommand RecordCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RecordCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RecordCommand"] = false;
                            CommandCanExecutes["RecordCommand"] = false;

                            ///   

                            _MINTHICKNESS = Convert.ToDecimal(_mainWnd.txtMINTHICKNESS.Text);
                            _MAXTHICKNESS = Convert.ToDecimal(_mainWnd.txtMAXTHICKNESS.Text);
                            _AVERAGETHICKNESS = Convert.ToDecimal(_mainWnd.txtAVERAGETHICKNESS.Text);

                            string chk = "";

                            if (_MINSTANDARD != "" && _MAXSTANDARD != "")
                            {
                                if (Convert.ToDecimal(_MINSTANDARD) <= _MINTHICKNESS && _MAXTHICKNESS <= Convert.ToDecimal(_MAXSTANDARD))
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }                                    
                            }
                            else if (_MINSTANDARD != "")
                            {
                                if (Convert.ToDecimal(_MINSTANDARD) <= _MINTHICKNESS)
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }
                            }
                            else if (_MAXSTANDARD != "")
                            {
                                if (_MAXTHICKNESS <= Convert.ToDecimal(_MAXSTANDARD))
                                    chk = "적합";
                                else
                                {
                                    chk = "부적합";
                                    OnMessage("기준값을 벗어났습니다.");
                                }
                            }

                            IPC_RESULTS.Add(new EACH_INDATA()
                            {
                                CHK = "N",
                                INX = inx++,
                                MINTHICKNESS = _MINTHICKNESS,
                                MAXTHICKNESS = _MAXTHICKNESS,
                                AVERAGETHICKNESS = _AVERAGETHICKNESS,
                                IPCVALUEYN = chk
                            });

                            ///

                            CommandResults["RecordCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RecordCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RecordCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecordCommand") ?
                        CommandCanExecutes["RecordCommand"] : (CommandCanExecutes["RecordCommand"] = true);
                });
            }
        }

        public ICommand RowDeleteCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RowDeleteCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RowDeleteCommand"] = false;
                            CommandCanExecutes["RowDeleteCommand"] = false;

                            ///
                            var elements = (from data in _IPC_RESULTS
                                            where data.CHK == "N"
                                            select data).ToList();

                            _IPC_RESULTS.Clear();

                            inx = 1;
                            foreach (var data in elements)
                            {
                                data.INX = inx++;
                                _IPC_RESULTS.Add(data);
                            }

                            ///
                            CommandResults["RowDeleteCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RowDeleteCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RowDeleteCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RowDeleteCommand") ?
                        CommandCanExecutes["RowDeleteCommand"] : (CommandCanExecutes["RowDeleteCommand"] = true);
                });
            }
        }

        public ICommand ClickConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickConfirmCommand"] = false;
                            CommandCanExecutes["ClickConfirmCommand"] = false;

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
                                    "두께측정",
                                    "두께측정",
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
                                dt.Columns.Add(new DataColumn("최소값"));
                                dt.Columns.Add(new DataColumn("최대값"));
                                dt.Columns.Add(new DataColumn("평균값"));
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
                                    row["최소값"] = rowdata.MINTHICKNESS != 0 ? rowdata.MINTHICKNESS : 0;
                                    row["최대값"] = rowdata.MAXTHICKNESS != 0 ? rowdata.MAXTHICKNESS : 0;
                                    row["평균값"] = rowdata.AVERAGETHICKNESS != 0 ? rowdata.AVERAGETHICKNESS : 0;
                                    row["적합여부"] = rowdata.IPCVALUEYN != null ? rowdata.IPCVALUEYN : "";

                                    dt.Rows.Add(row);

                                    // 시험상세결과 기록
                                    _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                    {
                                        POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                        OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].OPTSIGUID),
                                        POTSIRGUID = Guid.NewGuid(),
                                        ACTVAL = rowdata.AVERAGETHICKNESS.ToString(),
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
                            CommandResults["ClickConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickConfirmCommand") ?
                        CommandCanExecutes["ClickConfirmCommand"] : (CommandCanExecutes["ClickConfirmCommand"] = true);
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "두께측정",
                                "두께측정",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //XML 형식으로 저장
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("순번"));
                            dt.Columns.Add(new DataColumn("최소값"));
                            dt.Columns.Add(new DataColumn("최대값"));
                            dt.Columns.Add(new DataColumn("평균값"));
                            dt.Columns.Add(new DataColumn("적합여부"));

                            var row = dt.NewRow();
                            row["순번"] = "N/A";
                            row["최소값"] = "N/A";
                            row["최대값"] = "N/A";
                            row["평균값"] = "N/A";
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
            private string _CHK;
            public string CHK
            {
                get { return _CHK; }
                set
                {
                    _CHK = value;
                    OnPropertyChanged("CHK");
                }
            }

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

            private Decimal _MINTHICKNESS;
            public Decimal MINTHICKNESS
            {
                get { return _MINTHICKNESS; }
                set
                {
                    _MINTHICKNESS = value;
                    OnPropertyChanged("MINTHICKNESS");
                }
            }

            private Decimal _MAXTHICKNESS;
            public Decimal MAXTHICKNESS
            {
                get { return _MAXTHICKNESS; }
                set
                {
                    _MAXTHICKNESS = value;
                    OnPropertyChanged("MAXTHICKNESS");
                }
            }

            private Decimal _AVERAGETHICKNESS;
            public Decimal AVERAGETHICKNESS
            {
                get { return _AVERAGETHICKNESS; }
                set
                {
                    _AVERAGETHICKNESS = value;
                    OnPropertyChanged("AVERAGETHICKNESS");
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
