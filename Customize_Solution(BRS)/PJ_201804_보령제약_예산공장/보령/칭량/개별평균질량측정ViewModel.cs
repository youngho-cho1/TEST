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
    public class 개별평균질량측정ViewModel : ViewModelBase
    {
        #region Property
        public 개별평균질량측정ViewModel()
        {
            _ScaleInfo = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _IPC_RESULTS = new ObservableCollection<EACH_INDATA>();
            _BR_BRS_SEL_ProductionOrderIPCStandard = new BR_BRS_SEL_ProductionOrderIPCStandard();
            _BR_BRS_REG_ProductionOrderTestResult = new BR_BRS_REG_ProductionOrderTestResult();

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;
        }

        private 개별평균질량측정 _mainWnd;
        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 2000;
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
        private string IPC_TSID = "개별질량측정";
        private bool Standard = true;
        /// <summary>
        /// 기록 회차
        /// </summary>
        private int inx;
        private DateTime _IPCDateTime;

        private string _SCALEID;
        public string SCALEID
        {
            get { return _SCALEID; }
            set
            {
                _SCALEID = value;
                OnPropertyChanged("SCALEID");
            }
        }
        private int _scalePresion = 1;
        private string _IPCUOM = "mg";

        private string _SCALE_ALERT;
        public string SCALE_ALERT
        {
            get { return _SCALE_ALERT; }
            set
            {
                _SCALE_ALERT = value;
                OnPropertyChanged("SCALE_ALERT");
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

        // 저울값
        private Weight _CURWEIGHT = new Weight();
        public string CURWEIGHT
        {
            get { return _CURWEIGHT.WeightUOMString; }
        }

        // 요약
        private Weight _MINWEIGHT = new Weight();
        public string MINWEIGHT
        {
            get { return _MINWEIGHT.WeightUOMString; }
        }
        private Weight _MAXWEIGHT = new Weight();
        public string MAXWEIGHT
        {
            get { return _MAXWEIGHT.WeightUOMString; }
        }
        private Weight _AVGWEIGHT = new Weight();
        public string AVGWEIGHT
        {
            get { return _AVGWEIGHT.WeightUOMString; }
        }

        private string _INDIVIDUALMINWEIGHT;
        public string INDIVIDUALMINWEIGHT
        {
            get { return _INDIVIDUALMINWEIGHT; }
        }

        private string _INDIVIDUALMAXWEIGHT;
        public string INDIVIDUALMAXWEIGHT
        {
            get { return _INDIVIDUALMAXWEIGHT; }
        }

        private string _AVGMINWEIGHT;
        public string AVGMINWEIGHT
        {
            get { return _AVGMINWEIGHT; }
        }

        private string _AVGMAXWEIGHT;
        public string AVGMAXWEIGHT
        {
            get { return _AVGMAXWEIGHT; }
        }

        // 화면 컨트롤
        private bool _IsBtnEnable;
        /// <summary>
        /// TARE, 기록, 삭제
        /// </summary>
        public bool IsBtnEnable
        {
            get { return _IsBtnEnable; }
            set
            {
                _IsBtnEnable = value;
                OnPropertyChanged("IsBtnEnable");
            }
        }

        private bool _IsSaveEnable;
        /// <summary>
        /// IPC 결과 저장 및 XML 기록버튼
        /// </summary>
        public bool IsSaveEnable
        {
            get { return _IsSaveEnable; }
            set
            {
                _IsSaveEnable = value;
                OnPropertyChanged("IsSaveEnable");
            }
        }
        #endregion

        #region BizRule
        /// <summary>
        /// 저울정보 조회
        /// </summary>
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _ScaleInfo;

        /// <summary>
        /// 저울IF(SBI)
        /// </summary>
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;

        
        /// <summary>
        /// 저울 Tare 신호 송신(SBI)
        /// </summary>
        private BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;


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

                            if (arg != null && arg is 개별평균질량측정)
                            {
                                _mainWnd = arg as 개별평균질량측정;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_repeater != null)
                                        _repeater.Stop();

                                    _repeater = null;
                                };

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

                                _INDIVIDUALMINWEIGHT = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].LSL.ToString();
                                _INDIVIDUALMAXWEIGHT = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].USL.ToString();
                                _AVGMINWEIGHT = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1].LSL.ToString();
                                _AVGMAXWEIGHT = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1].USL.ToString();
                                OnPropertyChanged("INDIVIDUALMINWEIGHT");
                                OnPropertyChanged("INDIVIDUALMAXWEIGHT");
                                OnPropertyChanged("AVGMINWEIGHT");
                                OnPropertyChanged("AVGMAXWEIGHT");

                                inx = 1;
                                IsBtnEnable = false;
                                IsSaveEnable = false;
                                
                                _mainWnd.txtEQPTID.Focus();
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

        public ICommand BarcodeChangedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BarcodeChangedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["BarcodeChangedCommand"] = false;
                            CommandCanExecutes["BarcodeChangedCommand"] = false;

                            ///

                            if (!string.IsNullOrWhiteSpace(arg.ToString()))
                            {
                                string eqpt = arg.ToString();

                                // 저울기준정보 조회
                                _ScaleInfo.INDATAs.Clear();
                                _ScaleInfo.OUTDATAs.Clear();

                                _ScaleInfo.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA()
                                {
                                    EQPTID = eqpt
                                });

                                if (await _ScaleInfo.Execute() && _ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    SCALEID = _ScaleInfo.OUTDATAs[0].EQPTID;
                                    _scalePresion = Convert.ToInt32(_ScaleInfo.OUTDATAs[0].PRECISION.GetValueOrDefault());

                                    IsBtnEnable = true;
                                    SCALE_ALERT = "저울 조회 성공";
                                    _repeater.Start();
                                }
                                else
                                {
                                    SCALEID = "";
                                    IsBtnEnable = false;
                                    SCALE_ALERT = "저울 조회 실패";
                                    _repeater.Stop();
                                }

                                _CURWEIGHT = new Weight();
                                _CURWEIGHT.SetWeight(0, _IPCUOM, 1);
                                OnPropertyChanged("CURWEIGHT");
                            }

                            ///

                            CommandResults["BarcodeChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            SCALEID = "";
                            IsBtnEnable = false;
                            SCALE_ALERT = "저울 조회 실패";
                            _repeater.Stop();

                            CommandResults["BarcodeChangedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BarcodeChangedCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BarcodeChangedCommand") ?
                        CommandCanExecutes["BarcodeChangedCommand"] : (CommandCanExecutes["BarcodeChangedCommand"] = true);
                });
            }
        }

        public ICommand SetTareCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SetTareCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SetTareCommand"] = false;
                            CommandCanExecutes["SetTareCommand"] = false;

                            ///

                            if (!string.IsNullOrWhiteSpace(_SCALEID) && _ScaleInfo.OUTDATAs[0] != null)
                            {
                                _repeater.Stop();

                                // 저울에 Tare 명령어 전달
                                bool success = false;
                                if (_ScaleInfo.OUTDATAs[0].INTERFACE.ToUpper() == "REST")
                                {
                                    var result = await _restScaleService.DownloadString(_SCALEID, ScaleCommand.ST);

                                    success = result.code == "1" ? true : false;
                                }
                                else
                                {
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA
                                    {
                                        ScaleID = _SCALEID
                                    });

                                    if (await _BR_PHR_REG_ScaleSetTare.Execute())
                                    {
                                        success = true;
                                    }
                                }

                                if(success)
                                {
                                    SCALE_ALERT = "Tare 성공";

                                    _CURWEIGHT = new Weight();
                                    _CURWEIGHT.SetWeight(0, _IPCUOM, 1);
                                    OnPropertyChanged("CURWEIGHT");
                                }
                                else
                                {
                                    SCALE_ALERT = "Tare 실패";
                                }

                                _repeater.Start();
                            }

                            ///

                            CommandResults["SetTareCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SetTareCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SetTareCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SetTareCommand") ?
                        CommandCanExecutes["SetTareCommand"] : (CommandCanExecutes["SetTareCommand"] = true);
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

                            if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].LSL > _CURWEIGHT.Value)
                            {
                                OnMessage("개별질량 하한값 보다 낮습니다.");                                
                            }
                            else if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].USL < _CURWEIGHT.Value)
                            {
                                OnMessage("개별질량 상한값 보다 높습니다");                                
                            }


                            if(_IPC_RESULTS.Count == 0)
                            {
                                _MINWEIGHT.SetWeight(0, "mg", 1);
                                _MAXWEIGHT.SetWeight(0, "mg", 1);
                                _AVGWEIGHT.SetWeight(0, "mg", 1);
                            }

                            _IPCDateTime = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            IPC_RESULTS.Add(new EACH_INDATA()
                            {
                                CHK = "N",
                                INX = inx++,
                                RECORDDTTM = _IPCDateTime,
                                SCALEID = _SCALEID,
                                RESULT = _CURWEIGHT.Value,
                                UOM = _IPCUOM
                            });

                            if (_IPC_RESULTS.Count > 0)
                                IsSaveEnable = true;

                            summaryIPCResult();
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

                            if (_IPC_RESULTS.Count == 0)
                                IsSaveEnable = false;

                            summaryIPCResult();
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
                            _repeater.Stop();

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

                                // 개별질량 평균값
                                
                                decimal avg = _IPC_RESULTS.Average(x => x.RESULT);
                                foreach (var item in _IPC_RESULTS)
                                {
                                    if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].LSL > item.RESULT)
                                    {
                                        Standard = false;
                                    }
                                    else if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[0].USL < item.RESULT)
                                    {
                                        Standard = false;
                                    }
                                }

                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Clear();
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Clear();
                                _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Clear();

                                string confirmguid = AuthRepositoryViewModel.Instance.ConfirmedGuid;
                                DateTime curDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_IPC");

                                if (!Standard)
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

                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "개별평균질량측정",
                                        "개별평균질량측정",
                                        false,
                                        "OM_ProductionOrder_SUI",
                                        "",
                                        null, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }


                                    //XML 형식으로 저장
                                    DataSet ds = new DataSet();

                                    // DATA : 측정내용
                                    DataTable dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);
                                    dt.Columns.Add(new DataColumn("순번"));
                                    dt.Columns.Add(new DataColumn("장비번호"));
                                    dt.Columns.Add(new DataColumn("현재무게"));

                                    // DATA2 : 요약 (최대값, 최소값, 평균값)
                                    DataTable dt2 = new DataTable("DATA2");
                                    ds.Tables.Add(dt2);
                                    dt2.Columns.Add(new DataColumn("최소값"));
                                    dt2.Columns.Add(new DataColumn("최대값"));
                                    dt2.Columns.Add(new DataColumn("평균값"));

                                    // 시험명세 기록
                                    _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                                    {
                                        POTSRGUID = Guid.NewGuid(),
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPTSGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSGUID),
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
                                        SMPQTY = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTY,
                                        SMPQTYUOMID = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTYUOMID,
                                    });

                                    // 전자서명 코멘트
                                    _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                                    {
                                        COMMENTTYPE = "CM001",
                                        COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                        TSTYPE = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].TSTYPE,
                                        LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                                    });

                                    // 측정내용 XML, BR indata
                                    foreach (var rowdata in _IPC_RESULTS)
                                    {
                                        var row = dt.NewRow();
                                        row["순번"] = rowdata.INX.ToString();
                                        row["장비번호"] = rowdata.SCALEID != null ? rowdata.SCALEID : "";
                                        row["현재무게"] = rowdata.RESULTSTR != null ? rowdata.RESULTSTR : "";
                                        dt.Rows.Add(row);                                       

                                        // 시험상세결과 기록
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                        {
                                            POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                            OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSIGUID),
                                            POTSIRGUID = Guid.NewGuid(),
                                            ACTVAL = rowdata.RESULT.ToString("0.0"),
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
                                    // 요약 XML
                                    dt2.Rows.Add(MINWEIGHT, MAXWEIGHT, AVGWEIGHT);

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
                                    if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1].LSL > avg)
                                    {
                                        MessageBox.Show("평균질량 하한값 보다 낮습니다.");

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

                                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Role,
                                            Common.enumAccessType.Create,
                                            "개별평균질량측정",
                                            "개별평균질량측정",
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "",
                                            null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        //XML 형식으로 저장
                                        DataSet ds = new DataSet();

                                        // DATA : 측정내용
                                        DataTable dt = new DataTable("DATA");
                                        ds.Tables.Add(dt);
                                        dt.Columns.Add(new DataColumn("순번"));
                                        dt.Columns.Add(new DataColumn("장비번호"));
                                        dt.Columns.Add(new DataColumn("현재무게"));

                                        // DATA2 : 요약 (최대값, 최소값, 평균값)
                                        DataTable dt2 = new DataTable("DATA2");
                                        ds.Tables.Add(dt2);
                                        dt2.Columns.Add(new DataColumn("최소값"));
                                        dt2.Columns.Add(new DataColumn("최대값"));
                                        dt2.Columns.Add(new DataColumn("평균값"));

                                        // 시험명세 기록
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                                        {
                                            POTSRGUID = Guid.NewGuid(),
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPTSGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSGUID),
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
                                            SMPQTY = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTY,
                                            SMPQTYUOMID = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTYUOMID,
                                        });

                                        // 전자서명 코멘트
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                                        {
                                            COMMENTTYPE = "CM001",
                                            COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                            TSTYPE = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].TSTYPE,
                                            LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                                        });

                                        // 측정내용 XML, BR indata
                                        foreach (var rowdata in _IPC_RESULTS)
                                        {
                                            var row = dt.NewRow();
                                            row["순번"] = rowdata.INX.ToString();
                                            row["장비번호"] = rowdata.SCALEID != null ? rowdata.SCALEID : "";
                                            row["현재무게"] = rowdata.RESULTSTR != null ? rowdata.RESULTSTR : "";
                                            dt.Rows.Add(row);

                                            // 시험상세결과 기록
                                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                            {
                                                POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                                OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSIGUID),
                                                POTSIRGUID = Guid.NewGuid(),
                                                ACTVAL = rowdata.RESULT.ToString("0.0"),
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
                                        // 요약 XML
                                        dt2.Rows.Add(MINWEIGHT, MAXWEIGHT, AVGWEIGHT);

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
                                    else if (_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[1].USL < avg)
                                    {
                                        MessageBox.Show("평균질량 상한값 보다 높습니다.");

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

                                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Role,
                                            Common.enumAccessType.Create,
                                            "개별평균질량측정",
                                            "개별평균질량측정",
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "",
                                            null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        //XML 형식으로 저장
                                        DataSet ds = new DataSet();

                                        // DATA : 측정내용
                                        DataTable dt = new DataTable("DATA");
                                        ds.Tables.Add(dt);
                                        dt.Columns.Add(new DataColumn("순번"));
                                        dt.Columns.Add(new DataColumn("장비번호"));
                                        dt.Columns.Add(new DataColumn("현재무게"));

                                        // DATA2 : 요약 (최대값, 최소값, 평균값)
                                        DataTable dt2 = new DataTable("DATA2");
                                        ds.Tables.Add(dt2);
                                        dt2.Columns.Add(new DataColumn("최소값"));
                                        dt2.Columns.Add(new DataColumn("최대값"));
                                        dt2.Columns.Add(new DataColumn("평균값"));

                                        // 시험명세 기록
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                                        {
                                            POTSRGUID = Guid.NewGuid(),
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPTSGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSGUID),
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
                                            SMPQTY = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTY,
                                            SMPQTYUOMID = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTYUOMID,
                                        });

                                        // 전자서명 코멘트
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                                        {
                                            COMMENTTYPE = "CM001",
                                            COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                            TSTYPE = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].TSTYPE,
                                            LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                                        });

                                        // 측정내용 XML, BR indata
                                        foreach (var rowdata in _IPC_RESULTS)
                                        {
                                            var row = dt.NewRow();
                                            row["순번"] = rowdata.INX.ToString();
                                            row["장비번호"] = rowdata.SCALEID != null ? rowdata.SCALEID : "";
                                            row["현재무게"] = rowdata.RESULTSTR != null ? rowdata.RESULTSTR : "";
                                            dt.Rows.Add(row);

                                            // 시험상세결과 기록
                                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                            {
                                                POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                                OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSIGUID),
                                                POTSIRGUID = Guid.NewGuid(),
                                                ACTVAL = rowdata.RESULT.ToString("0.0"),
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
                                        // 요약 XML
                                        dt2.Rows.Add(MINWEIGHT, MAXWEIGHT, AVGWEIGHT);

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
                                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Role,
                                            Common.enumAccessType.Create,
                                            "개별평균질량측정",
                                            "개별평균질량측정",
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "",
                                            null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        //XML 형식으로 저장
                                        DataSet ds = new DataSet();

                                        // DATA : 측정내용
                                        DataTable dt = new DataTable("DATA");
                                        ds.Tables.Add(dt);
                                        dt.Columns.Add(new DataColumn("순번"));
                                        dt.Columns.Add(new DataColumn("장비번호"));
                                        dt.Columns.Add(new DataColumn("현재무게"));

                                        // DATA2 : 요약 (최대값, 최소값, 평균값)
                                        DataTable dt2 = new DataTable("DATA2");
                                        ds.Tables.Add(dt2);
                                        dt2.Columns.Add(new DataColumn("최소값"));
                                        dt2.Columns.Add(new DataColumn("최대값"));
                                        dt2.Columns.Add(new DataColumn("평균값"));

                                        // 시험명세 기록
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_SPEC
                                        {
                                            POTSRGUID = Guid.NewGuid(),
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPTSGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSGUID),
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
                                            SMPQTY = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTY,
                                            SMPQTYUOMID = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].SMPQTYUOMID,
                                        });

                                        // 전자서명 코멘트
                                        _BR_BRS_REG_ProductionOrderTestResult.INDATA_ETCs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ETC
                                        {
                                            COMMENTTYPE = "CM001",
                                            COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_IPC"),
                                            TSTYPE = _BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].TSTYPE,
                                            LOCATIONID = AuthRepositoryViewModel.Instance.RoomID
                                        });

                                        // 측정내용 XML, BR indata
                                        foreach (var rowdata in _IPC_RESULTS)
                                        {
                                            var row = dt.NewRow();
                                            row["순번"] = rowdata.INX.ToString();
                                            row["장비번호"] = rowdata.SCALEID != null ? rowdata.SCALEID : "";
                                            row["현재무게"] = rowdata.RESULTSTR != null ? rowdata.RESULTSTR : "";
                                            dt.Rows.Add(row);                                            

                                            // 시험상세결과 기록
                                            _BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEMs.Add(new BR_BRS_REG_ProductionOrderTestResult.INDATA_ITEM
                                            {
                                                POTSRGUID = _BR_BRS_REG_ProductionOrderTestResult.INDATA_SPECs[0].POTSRGUID,
                                                OPTSIGUID = new Guid(_BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATAs[2].OPTSIGUID),
                                                POTSIRGUID = Guid.NewGuid(),
                                                ACTVAL = rowdata.RESULT.ToString("0.0"),
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
                                        // 요약 XML
                                        dt2.Rows.Add(MINWEIGHT, MAXWEIGHT, AVGWEIGHT);

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
                                }                                                                            
                            }
                            else
                            {
                                throw new Exception("입력한 정보가 없습니다. 기록 버튼을 클릭하여 추가해 주시기 바랍니다.");
                            }
                            //
                            CommandResults["ClickConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickConfirmCommand"] = false;
                            _repeater.Start();
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
                            _repeater.Stop();

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
                                "개별평균질량측정",
                                "개별평균질량측정",
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
                            dt.Columns.Add(new DataColumn("장비번호"));
                            dt.Columns.Add(new DataColumn("현재무게"));

                            var row = dt.NewRow();
                            row["순번"] = "N/A";
                            row["장비번호"] = "N/A";
                            row["현재무게"] = "N/A";
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
                            _repeater.Start();
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
        async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                if (!string.IsNullOrWhiteSpace(_SCALEID) && _ScaleInfo.OUTDATAs[0] != null)
                {
                    bool success = false;
                    string curWeight = string.Empty;
                    string curUOM = string.Empty;

                    if (_ScaleInfo.OUTDATAs[0].INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_SCALEID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            curWeight = result.data;
                            curUOM = result.unit;
                        }
                    }
                    else
                    {
                        _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                        _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                        _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                        {
                            ScaleID = _SCALEID
                        });

                        if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                        {
                            success = true;
                            curWeight = string.Format(("{0:F" + _scalePresion + "}"), _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight);
                            curUOM = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                        }
                    }

                    if (success)
                    {
                        _CURWEIGHT.SetWeight(curWeight, curUOM);
                        _CURWEIGHT.Value = Weight.Add(0, _IPCUOM, _CURWEIGHT.Value, _CURWEIGHT.Uom);
                        _CURWEIGHT.Uom = _IPCUOM;
                        _CURWEIGHT.Precision = 1;
                        
                        SCALE_ALERT = "저울 연결 성공";
                    }
                    else
                    {
                        SCALE_ALERT = "저울 연결 실패";
                    }

                    OnPropertyChanged("CURWEIGHT");
                    _repeater.Start();
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
                _repeater.Start();
            }
        }       

        public void summaryIPCResult()
        {
            decimal minval, maxval, avgval;
            
            if (_IPC_RESULTS.Count > 0)
            {
                minval = _IPC_RESULTS.Min(x => x.RESULT);
                maxval = _IPC_RESULTS.Max(x => x.RESULT);
                avgval = _IPC_RESULTS.Average(x => x.RESULT);
            }
            else
            {
                minval = 0m;
                maxval = 0m;
                avgval = 0m;
            }

            _MINWEIGHT.Value = minval;
            _MAXWEIGHT.Value = maxval;
            _AVGWEIGHT.Value = avgval;

            OnPropertyChanged("MINWEIGHT");
            OnPropertyChanged("MAXWEIGHT");
            OnPropertyChanged("AVGWEIGHT");
        }
        
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

            private DateTime _RECORDDTTM;
            public DateTime RECORDDTTM
            {
                get { return _RECORDDTTM; }
                set
                {
                    _RECORDDTTM = value;
                    OnPropertyChanged("RECORDDTTM");
                }
            }

            private string _SCALEID;
            public string SCALEID
            {
                get { return _SCALEID; }
                set
                {
                    _SCALEID = value;
                    OnPropertyChanged("SCALEID");
                }
            }

            public string SMPQTY
            {
                get { return "1"; }
            }

            private decimal _RESULT;
            public decimal RESULT
            {
                get { return _RESULT; }
                set
                {
                    _RESULT = value;
                    OnPropertyChanged("RESULT");
                }
            }

            private string _UOM;
            public string UOM
            {
                get { return _UOM; }
                set { _UOM = value; }
            }

            public string RESULTSTR
            {
                get
                {
                    Weight str = new Weight();
                    str.SetWeight(_RESULT, _UOM, 1);

                    return str.WeightUOMString;
                }
            }
        }
    }
}
