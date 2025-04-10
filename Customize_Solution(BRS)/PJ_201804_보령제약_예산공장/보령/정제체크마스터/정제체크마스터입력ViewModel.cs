using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.ObjectModel;
using LGCNS.iPharmMES.Recipe.Common;
using System.Windows;

namespace 보령
{
    public class 정제체크마스터입력ViewModel : ViewModelBase
    {
        #region [Property]
        public 정제체크마스터입력ViewModel()
        {
            _BR_PHR_SEL_CODE = new BR_PHR_SEL_CODE();
            _BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN = new BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN();
            _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD = new BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD();
            _IPCResultSections = new IPCResultSection.OUTDATACollection();
            _IPC_RESULTS = new ObservableCollection<EACH_INDATA>();
            _IPC_STANDARDS = new ObservableCollection<EACH_INDATA>();

        }

        정제체크마스터입력 _mainWnd;

        private string _comment = string.Empty;

        private IPCResultSection.OUTDATACollection _IPCResultSections;
        public IPCResultSection.OUTDATACollection IPCResultSections
        {
            get { return this._IPCResultSections; }
            set
            {
                this._IPCResultSections = value;
                this.OnPropertyChanged("IPCResultSections");
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
        private ObservableCollection<EACH_INDATA> _IPC_STANDARDS;
        public ObservableCollection<EACH_INDATA> IPC_STANDARDS
        {
            get { return _IPC_STANDARDS; }
            set
            {
                _IPC_STANDARDS = value;
                OnPropertyChanged("IPC_STANDARDS");
            }
        }
        private string _EQPTID;
        public string EQPTID
        {
            get { return this._EQPTID; }
            set
            {
                this._EQPTID = value;
                this.OnPropertyChanged("EQPTID");
            }
        }

        private string _EQPTNAME;
        public string EQPTNAME
        {
            get { return this._EQPTNAME; }
            set
            {
                this._EQPTNAME = value;
                this.OnPropertyChanged("EQPTNAME");
            }
        }
        private bool _INPUT_ENABLE;
        public bool INPUT_ENABLE
        {
            get { return _INPUT_ENABLE; }
            set
            {
                _INPUT_ENABLE = value;
                OnPropertyChanged("INPUT_ENABLE");
            }
        }
        private bool _EQPTID_ENABLE;
        public bool EQPTID_ENABLE
        {
            get { return _EQPTID_ENABLE; }
            set
            {
                _EQPTID_ENABLE = value;
                OnPropertyChanged("EQPTID_ENABLE");
            }
        }
        private DateTime _AVG_DTTM;
        public DateTime AVG_DTTM
        {
            get { return this._AVG_DTTM; }
            set
            {
                this._AVG_DTTM = value;
                this.OnPropertyChanged("AVG_DTTM");
            }
        }
        private bool _CIRCLE_FLAG; // 비원형과 그외 구분
        public bool CIRCLE_FLAG
        {
            get { return _CIRCLE_FLAG; }
            set
            {
                _CIRCLE_FLAG = value;
                OnPropertyChanged("CIRCLE_FLAG");
            }
        }
        private Visibility _CIRCLE_CHECK;
        public Visibility CIRCLE_CHECK
        {
            get { return _CIRCLE_CHECK; }
            set
            {
                _CIRCLE_CHECK = value;
                OnPropertyChanged("CIRCLE_CHECK");
            }
        }
        private bool _RSD_FLAG; // RSD 미입력 대상 구분
        public bool RSD_FLAG
        {
            get { return _RSD_FLAG; }
            set
            {
                _RSD_FLAG = value;
                OnPropertyChanged("RSD_FLAG");
            }
        }
        private Visibility _RSD_CHECK;
        public Visibility RSD_CHECK
        {
            get { return _RSD_CHECK; }
            set
            {
                _RSD_CHECK = value;
                OnPropertyChanged("RSD_CHECK");
            }
        }
        private bool _KeepGoing = false;
        public bool KeepGoing
        {
            get { return _KeepGoing; }
            set
            {
                _KeepGoing = value;
                OnPropertyChanged("KeepGoing");
            }
        }
        #endregion

        #region [Bizrule]

        private BR_PHR_SEL_CODE _BR_PHR_SEL_CODE;
        private BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN _BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN;
        private BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD;
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
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is 정제체크마스터입력)
                            {
                                _mainWnd = arg as 정제체크마스터입력;
                                IsBusy = true;
                                IPCResultSections.Clear();
                            }

                            INPUT_ENABLE = true;
                            EQPTID_ENABLE = false;
                            CIRCLE_FLAG = true;
                            RSD_FLAG = true;


                            _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD.INDATAs.Add(new BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD.INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                            });

                            if (await _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD.Execute())
                            {
                                IPC_STANDARDS.Clear();
                                String AVG_STD_HARDNESS = "";
                                String MIN_STD_HARDNESS = "";
                                String MAX_STD_HARDNESS = "";
                                String AVG_STD_THICKNESS = "";
                                String MIN_STD_THICKNESS = "";
                                String MAX_STD_THICKNESS = "";
                                String SD_STD_WEIGHT = "";
                                String AVG_STD_WEIGHT = "";
                                String MIN_STD_WEIGHT = "";
                                String MAX_STD_WEIGHT = "";

                                foreach (var std in _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD.OUTDATAs)
                                {
                                    switch (std.TIID)
                                    {
                                        case "IPC-023": // 평균질량
                                            AVG_STD_WEIGHT = std.CSL != "" ? std.CSL : "N/A";
                                            break;
                                        case "IPC-024": // 최소질량
                                            MIN_STD_WEIGHT = std.LSL != "" ? std.LSL : "N/A";
                                            break;
                                        case "IPC-025": // 최대질량
                                            MAX_STD_WEIGHT = std.USL != "" ? std.USL : "N/A";
                                            break;
                                        case "IPC-026": // 개별질량RSD
                                            SD_STD_WEIGHT = std.USL != "" ? std.USL : "N/A";
                                            break;
                                        case "IPC-027": // 평균두께
                                            AVG_STD_THICKNESS = std.CSL != "" ? std.CSL : "N/A";
                                            break;
                                        case "IPC-028": // 최소두께
                                            MIN_STD_THICKNESS = std.LSL != "" ? std.LSL : "N/A";
                                            break;
                                        case "IPC-029": // 최대두께
                                            MAX_STD_THICKNESS = std.USL != "" ? std.USL : "N/A";
                                            break;
                                        case "IPC-030": // 평균경도
                                            AVG_STD_HARDNESS = std.CSL != "" ? std.CSL : "N/A";
                                            break;
                                        case "IPC-031": //최소경도
                                            MIN_STD_HARDNESS = std.LSL != "" ? std.LSL : "N/A";
                                            break;
                                        case "IPC-032": //최대경도
                                            MAX_STD_HARDNESS = std.USL != "" ? std.USL : "N/A";
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                if (SD_STD_WEIGHT == "N/A") { RSD_FLAG = false; RSD_CHECK = Visibility.Collapsed; } // RSD 미입력해야 될 경우
                                if (AVG_STD_HARDNESS == "" & MIN_STD_HARDNESS == "" & MAX_STD_HARDNESS == "")
                                {
                                    AVG_STD_HARDNESS = "N/A";
                                    MIN_STD_HARDNESS = "N/A";
                                    MAX_STD_HARDNESS = "N/A";
                                    CIRCLE_FLAG = false;
                                    CIRCLE_CHECK = Visibility.Collapsed;
                                } // 경도 미입력해야 될 경우

                                IPC_STANDARDS.Add(new EACH_INDATA()
                                {
                                    RSLT_AVG_WEIGHT = AVG_STD_WEIGHT,
                                    RSLT_MIN_WEIGHT = MIN_STD_WEIGHT,
                                    RSLT_MAX_WEIGHT = MAX_STD_WEIGHT,
                                    RSLT_SD_WEIGHT = SD_STD_WEIGHT,
                                    RSLT_AVG_THICKNESS = AVG_STD_THICKNESS,
                                    RSLT_MIN_THICKNESS = MIN_STD_THICKNESS,
                                    RSLT_MAX_THICKNESS = MAX_STD_THICKNESS,
                                    RSLT_AVG_HARDNESS = AVG_STD_HARDNESS,
                                    RSLT_MIN_HARDNESS = MIN_STD_HARDNESS,
                                    RSLT_MAX_HARDNESS = MAX_STD_HARDNESS
                                });
                            }
                            else
                            {
                                throw _BR_BRS_SEL_ProductionOrderTestSpecification_STANDARD.Exception;
                            }

                            // 이전 기록 조회
                            if (_mainWnd.CurrentInstruction.Raw.ACTVAL == _mainWnd.TableTypeName && _mainWnd.CurrentInstruction.Raw.NOTE != null)
                            {
                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable();
                                var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                                string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);

                                ds.ReadXmlFromString(xml);
                                if (ds.Tables[0].TableName == "DATA")
                                {
                                    dt = ds.Tables[0];
                                    EQPTID = dt.Rows[0]["장비번호"].ToString();

                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        if (i < dt.Rows.Count - 1)
                                        {
                                            _IPCResultSections.Add(new IPCResultSection.OUTDATA
                                            {
                                                STRTDTTM = Convert.ToDateTime(dt.Rows[i]["점검일시"].ToString()),
                                                AVG_WEIGHT = Convert.ToDecimal(dt.Rows[i]["평균질량"]),
                                                MIN_WEIGHT = Convert.ToDecimal(dt.Rows[i]["개별최소질량"]),
                                                MAX_WEIGHT = Convert.ToDecimal(dt.Rows[i]["개별최대질량"]),
                                                SD_WEIGHT = dt.Rows[i]["개별질량RSD"].Equals("N/A") ? 0 : Convert.ToDecimal(dt.Rows[i]["개별질량RSD"]),
                                                AVG_THICKNESS = Convert.ToDecimal(dt.Rows[i]["평균두께"]),
                                                MIN_THICKNESS = Convert.ToDecimal(dt.Rows[i]["최소두께"]),
                                                MAX_THICKNESS = Convert.ToDecimal(dt.Rows[i]["최대두께"]),
                                                AVG_HARDNESS = dt.Rows[i]["평균경도"].Equals("N/A") ? 0 : Convert.ToDecimal(dt.Rows[i]["평균경도"]),
                                                MIN_HARDNESS = dt.Rows[i]["최소경도"].Equals("N/A") ? 0 : Convert.ToDecimal(dt.Rows[i]["최소경도"]),
                                                MAX_HARDNESS = dt.Rows[i]["최대경도"].Equals("N/A") ? 0 : Convert.ToDecimal(dt.Rows[i]["최대경도"]),
                                                RowEditSec = "INS"
                                            });
                                        }
                                        //2024.05.24 김도연 : 화면에 평균을 띄우지 않음. 작업자가 평균 버튼을 눌러 확인할 수 있게 변경.(작업자의 실수를 줄이기 위함)
                                    }
                                }
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


        public ICommand InputEquipmentCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["InputEquipmentCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["InputEquipmentCommandAsync"] = false;
                            CommandCanExecutes["InputEquipmentCommandAsync"] = false;

                            // 설비 체크
                            _BR_PHR_SEL_CODE.INDATAs.Clear();
                            _BR_PHR_SEL_CODE.OUTDATAs.Clear();
                            if (String.IsNullOrWhiteSpace(EQPTID))
                            {
                                throw new Exception(string.Format("타정기 정보를 입력하십시오."));
                            }
                            else
                            {
                                _BR_PHR_SEL_CODE.INDATAs.Add(new BR_PHR_SEL_CODE.INDATA()
                                {
                                    TYPE = "Equipment",
                                    LANGID = "ko-KR",
                                    CODE = _EQPTID
                                });

                                await _BR_PHR_SEL_CODE.Execute();

                                if (_BR_PHR_SEL_CODE.OUTDATAs.Count > 0)
                                {
                                    EQPTID = _BR_PHR_SEL_CODE.OUTDATAs[0].CODE;
                                    EQPTNAME = _BR_PHR_SEL_CODE.OUTDATAs[0].NAME.Replace("[" + _EQPTID + "]", "");
                                    INPUT_ENABLE = false;
                                    EQPTID_ENABLE = true;
                                }
                                else
                                {
                                    throw new Exception(string.Format("타정기 정보가 없습니다."));
                                }

                                CommandResults["InputEquipmentCommandAsync"] = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            CommandResults["InputEquipmentCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["InputEquipmentCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("InputEquipmentCommandAsync") ?
                        CommandCanExecutes["InputEquipmentCommandAsync"] : (CommandCanExecutes["InputEquipmentCommandAsync"] = true);
                });
            }
        }
        public ICommand AVGCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["AVGCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            int count = 0;
                            string VAL_MES = "";

                            CommandResults["AVGCommandAsync"] = false;
                            CommandCanExecutes["AVGCommandAsync"] = false;

                            if (String.IsNullOrWhiteSpace(EQPTNAME))
                            {
                                throw new Exception(string.Format("타정기 정보를 입력하십시오."));
                            }
                            else
                            {
                                AVG_DTTM = System.DateTime.Now;
                                IPC_RESULTS.Clear();
                                decimal AVG_RESULT_WEIGHT = 0;
                                decimal MIN_RESULT_WEIGHT = IPCResultSections[0].MIN_WEIGHT;
                                decimal MAX_RESULT_WEIGHT = IPCResultSections[0].MAX_WEIGHT;
                                decimal SD_RESULT_WEIGHT = 0;
                                decimal AVG_RESULT_THICKNESS = 0;
                                decimal MIN_RESULT_THICKNESS = IPCResultSections[0].MIN_THICKNESS;
                                decimal MAX_RESULT_THICKNESS = IPCResultSections[0].MAX_THICKNESS;
                                decimal AVG_RESULT_HARDNESS = 0;
                                decimal MIN_RESULT_HARDNESS = IPCResultSections[0].MIN_HARDNESS;
                                decimal MAX_RESULT_HARDNESS = IPCResultSections[0].MAX_HARDNESS;

                                foreach (var ipc in IPCResultSections)
                                {
                                    count++;

                                    VAL_MES += Validation(ipc, count);

                                    AVG_RESULT_WEIGHT += ipc.AVG_WEIGHT;
                                    MIN_RESULT_WEIGHT = Math.Min(MIN_RESULT_WEIGHT, ipc.MIN_WEIGHT);
                                    MAX_RESULT_WEIGHT = Math.Max(MAX_RESULT_WEIGHT, ipc.MAX_WEIGHT);
                                    SD_RESULT_WEIGHT += ipc.SD_WEIGHT;
                                    AVG_RESULT_THICKNESS += ipc.AVG_THICKNESS;
                                    MIN_RESULT_THICKNESS = Math.Min(MIN_RESULT_THICKNESS, ipc.MIN_THICKNESS);
                                    MAX_RESULT_THICKNESS = Math.Max(MAX_RESULT_THICKNESS, ipc.MAX_THICKNESS);
                                    AVG_RESULT_HARDNESS += ipc.AVG_HARDNESS;
                                    MIN_RESULT_HARDNESS = Math.Min(MIN_RESULT_HARDNESS, ipc.MIN_HARDNESS);
                                    MAX_RESULT_HARDNESS = Math.Max(MAX_RESULT_HARDNESS, ipc.MAX_HARDNESS);

                                }

                                if (VAL_MES != "")
                                {
                                    VAL_MES += "계속 진행하시겠습니까?";
                                    if (await OnMessageAsync(VAL_MES, true))
                                    {
                                        var authHelper = new iPharmAuthCommandHelper();
                                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                        enumRoleType inspectorRole = enumRoleType.ROLE001;
                                        if (await authHelper.ClickAsync(
                                                Common.enumCertificationType.Role,
                                                Common.enumAccessType.Create,
                                                "기준에 부합하지 않는 값을 기록합니다. ",
                                                "부합하지 않는 값 입력",
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

                                        KeepGoing = true;

                                        _comment = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation");
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }

                                int num = IPCResultSections.Count;
                                
                                if(RSD_FLAG.Equals(false) & CIRCLE_FLAG.Equals(false))
                                {
                                    IPC_RESULTS.Add(new EACH_INDATA()
                                    {
                                        RSLT_AVG_DTTM = AVG_DTTM.ToString("yyyy-MM-dd HH:mm"),
                                        RSLT_AVG_WEIGHT = String.Format("{0:0.00}", Math.Round((AVG_RESULT_WEIGHT / num), 2)),
                                        RSLT_MIN_WEIGHT = String.Format("{0:0.00}", Math.Round(MIN_RESULT_WEIGHT, 2)),
                                        RSLT_MAX_WEIGHT = String.Format("{0:0.00}", Math.Round(MAX_RESULT_WEIGHT, 2)),
                                        RSLT_SD_WEIGHT = "N/A",
                                        RSLT_AVG_THICKNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_THICKNESS / num), 2)),
                                        RSLT_MIN_THICKNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_THICKNESS, 2)),
                                        RSLT_MAX_THICKNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_THICKNESS, 2)),
                                        RSLT_AVG_HARDNESS = "N/A",
                                        RSLT_MIN_HARDNESS = "N/A",
                                        RSLT_MAX_HARDNESS = "N/A"

                                    });
                                }
                                else if (RSD_FLAG.Equals(false))
                                {
                                    IPC_RESULTS.Add(new EACH_INDATA()
                                    {
                                        RSLT_AVG_DTTM = AVG_DTTM.ToString("yyyy-MM-dd HH:mm"),
                                        RSLT_AVG_WEIGHT = String.Format("{0:0.00}", Math.Round((AVG_RESULT_WEIGHT / num), 2)),
                                        RSLT_MIN_WEIGHT = String.Format("{0:0.00}", Math.Round(MIN_RESULT_WEIGHT, 2)),
                                        RSLT_MAX_WEIGHT = String.Format("{0:0.00}", Math.Round(MAX_RESULT_WEIGHT, 2)),
                                        RSLT_SD_WEIGHT = "N/A",
                                        RSLT_AVG_THICKNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_THICKNESS / num), 2)),
                                        RSLT_MIN_THICKNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_THICKNESS, 2)),
                                        RSLT_MAX_THICKNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_THICKNESS, 2)),
                                        RSLT_AVG_HARDNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_HARDNESS / num), 2)),
                                        RSLT_MIN_HARDNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_HARDNESS, 2)),
                                        RSLT_MAX_HARDNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_HARDNESS, 2))

                                    });
                                }
                                else if (CIRCLE_FLAG.Equals(false))
                                {
                                    IPC_RESULTS.Add(new EACH_INDATA()
                                    {
                                        RSLT_AVG_DTTM = AVG_DTTM.ToString("yyyy-MM-dd HH:mm"),
                                        RSLT_AVG_WEIGHT = String.Format("{0:0.00}", Math.Round((AVG_RESULT_WEIGHT / num), 2)),
                                        RSLT_MIN_WEIGHT = String.Format("{0:0.00}", Math.Round(MIN_RESULT_WEIGHT, 2)),
                                        RSLT_MAX_WEIGHT = String.Format("{0:0.00}", Math.Round(MAX_RESULT_WEIGHT, 2)),
                                        RSLT_SD_WEIGHT = String.Format("{0:0.00}", Math.Round((SD_RESULT_WEIGHT / num), 2)),
                                        RSLT_AVG_THICKNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_THICKNESS / num), 2)),
                                        RSLT_MIN_THICKNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_THICKNESS, 2)),
                                        RSLT_MAX_THICKNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_THICKNESS, 2)),
                                        RSLT_AVG_HARDNESS = "N/A",
                                        RSLT_MIN_HARDNESS = "N/A",
                                        RSLT_MAX_HARDNESS = "N/A"

                                    });
                                }
                                else
                                {
                                    IPC_RESULTS.Add(new EACH_INDATA()
                                    {
                                        RSLT_AVG_DTTM = AVG_DTTM.ToString("yyyy-MM-dd HH:mm"),
                                        RSLT_AVG_WEIGHT = String.Format("{0:0.00}", Math.Round((AVG_RESULT_WEIGHT / num), 2)),
                                        RSLT_MIN_WEIGHT = String.Format("{0:0.00}", Math.Round(MIN_RESULT_WEIGHT, 2)),
                                        RSLT_MAX_WEIGHT = String.Format("{0:0.00}", Math.Round(MAX_RESULT_WEIGHT, 2)),
                                        RSLT_SD_WEIGHT = String.Format("{0:0.00}", Math.Round((SD_RESULT_WEIGHT / num), 2)),
                                        RSLT_AVG_THICKNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_THICKNESS / num), 2)),
                                        RSLT_MIN_THICKNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_THICKNESS, 2)),
                                        RSLT_MAX_THICKNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_THICKNESS, 2)),
                                        RSLT_AVG_HARDNESS = String.Format("{0:0.00}", Math.Round((AVG_RESULT_HARDNESS / num), 2)),
                                        RSLT_MIN_HARDNESS = String.Format("{0:0.00}", Math.Round(MIN_RESULT_HARDNESS, 2)),
                                        RSLT_MAX_HARDNESS = String.Format("{0:0.00}", Math.Round(MAX_RESULT_HARDNESS, 2))

                                    });
                                }
                                CommandResults["AVGCommandAsync"] = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            CommandResults["AVGCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["AVGCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("AVGCommandAsync") ?
                        CommandCanExecutes["AVGCommandAsync"] : (CommandCanExecutes["AVGCommandAsync"] = true);
                });
            }
        }

        public string Validation(IPCResultSection.OUTDATA ipc, int count)
        {
            string message = "";
            // 개별 질량 Validation
            if (IPC_STANDARDS[0].RSLT_MIN_WEIGHT != "N/A" & IPC_STANDARDS[0].RSLT_MAX_WEIGHT != "N/A")
            {
                if (ipc.AVG_WEIGHT < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_WEIGHT))
                {
                    message += count.ToString() + "행 : 평균질량이 개별최소질량의 기준 값보다 작습니다.\n";
                }
                if (ipc.MIN_WEIGHT < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_WEIGHT))
                {
                    message += count.ToString() + "행 : 개별최소질량이 개별최소질량의 기준 값보다 작습니다.\n";
                }
                if (ipc.MAX_WEIGHT < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_WEIGHT))
                {
                    message += count.ToString() + "행 : 개별최대질량이 개별최소질량의 기준 값보다 작습니다.\n";
                }

                if (ipc.AVG_WEIGHT > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_WEIGHT))
                {
                    message += count.ToString() + "행 : 평균질량이 개별최대질량의 기준 값보다 큽니다.\n";
                }
                if (ipc.MIN_WEIGHT > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_WEIGHT))
                {
                    message += count.ToString() + "행 : 개별최소질량이 개별최대질량의 기준 값보다 큽니다.\n";
                }
                if (ipc.MAX_WEIGHT > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_WEIGHT))
                {
                    message += count.ToString() + "행 : 개별최대질량이 개별최대질량의 기준 값보다 큽니다.\n";
                }
            }
            // 개별 질량 RSD Validation
            if (IPC_STANDARDS[0].RSLT_SD_WEIGHT != "N/A")
            {
                if (ipc.SD_WEIGHT > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_SD_WEIGHT))
                {
                    message += count.ToString() + "행 : 개별질량RSD의 기준 값보다 큽니다.\n";
                }
            }
            // 두께 Validation
            if (IPC_STANDARDS[0].RSLT_MIN_THICKNESS != "N/A" & IPC_STANDARDS[0].RSLT_MAX_THICKNESS != "N/A")
            {
                if (ipc.AVG_THICKNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_THICKNESS))
                {
                    message += count.ToString() + "행 : 평균두께가 최소두께의 기준 값보다 작습니다.\n";
                }
                if (ipc.MIN_THICKNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_THICKNESS))
                {
                    message += count.ToString() + "행 : 최소두께가 최소두께의 기준 값보다 작습니다.\n";
                }
                if (ipc.MAX_THICKNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_THICKNESS))
                {
                    message += count.ToString() + "행 : 최대두께가 최소두께의 기준 값보다 작습니다.\n";
                }
                if (ipc.AVG_THICKNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_THICKNESS))
                {
                    message += count.ToString() + "행 : 평균두께가 최대두께의 기준 값보다 큽니다.\n";
                }
                if (ipc.MIN_THICKNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_THICKNESS))
                {
                    message += count.ToString() + "행 : 최소두께가 최대두께의 기준 값보다 큽니다.\n";
                }
                if (ipc.MAX_THICKNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_THICKNESS))
                {
                    message += count.ToString() + "행 : 최대두께가 최대두께의 기준 값보다 큽니다.\n";
                }
            }
            // 경도 Validation
            if (IPC_STANDARDS[0].RSLT_MIN_HARDNESS != "N/A" & IPC_STANDARDS[0].RSLT_MAX_HARDNESS != "N/A")
            {
                if (ipc.AVG_HARDNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_HARDNESS))
                {
                    message += count.ToString() + "행 : 평균경도가 최소경도의 기준 값보다 작습니다.\n";
                }
                if (ipc.MIN_HARDNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_HARDNESS))
                {
                    message += count.ToString() + "행 : 최소경도가 최소경도의 기준 값보다 작습니다.\n";
                }
                if (ipc.MAX_HARDNESS < Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MIN_HARDNESS))
                {
                    message += count.ToString() + "행 : 최대경도가 최소경도의 기준 값보다 작습니다.\n";
                }
                if (ipc.AVG_HARDNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_HARDNESS))
                {
                    message += count.ToString() + "행 : 평균경도가 최대경도의 기준 값보다 큽니다.\n";
                }
                if (ipc.MIN_HARDNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_HARDNESS))
                {
                    message += count.ToString() + "행 : 최소경도가 최대경도의 기준 값보다 큽니다.\n";
                }
                if (ipc.MAX_HARDNESS > Convert.ToDecimal(IPC_STANDARDS[0].RSLT_MAX_HARDNESS))
                {
                    message += count.ToString() + "행 : 최대경도가 최대경도의 기준 값보다 큽니다.\n";
                }
            }

            return message;
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

                            //2024.05.27 김도연 : 평균 정보가 없을 때 기록이 안되도록 수정
                            if (IPC_RESULTS.Count == 0)
                            {
                                throw new Exception(string.Format("평균 정보가 없습니다."));
                            }
                            else
                            {
                                var authHelper = new iPharmAuthCommandHelper();

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
                                }else
                                {
                                    // 전자서명 후 BR 실행
                                    authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("정제체크마스터입력"),
                                        string.Format("정제체크마스터입력"),
                                        false,
                                        "OM_ProductionOrder_SUI",
                                        _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }

                                // BR_BRS_REG_IPC_CHECKMASTER_MULTI IPC 결과 테이블에 저장
                                _BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN.INDATAs.Clear();

                                _BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN.INDATAs.Add(new BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN.INDATA
                                {
                                    EQPTID = EQPTID,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    SMPQTY = 1,
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI"),
                                    STRTDTTM = AVG_DTTM,
                                    LOCATIONID = AuthRepositoryViewModel.Instance.RoomID,
                                    AVG_WEIGHT = IPC_RESULTS[0].RSLT_AVG_WEIGHT,
                                    MIN_WEIGHT = IPC_RESULTS[0].RSLT_MIN_WEIGHT,
                                    MAX_WEIGHT = IPC_RESULTS[0].RSLT_MAX_WEIGHT,
                                    SD_WEIGHT = IPC_RESULTS[0].RSLT_SD_WEIGHT,
                                    AVG_THICKNESS = IPC_RESULTS[0].RSLT_AVG_THICKNESS,
                                    MIN_THICKNESS = IPC_RESULTS[0].RSLT_MIN_THICKNESS,
                                    MAX_THICKNESS = IPC_RESULTS[0].RSLT_MAX_THICKNESS,
                                    AVG_HARDNESS = IPC_RESULTS[0].RSLT_AVG_HARDNESS,
                                    MIN_HARDNESS = IPC_RESULTS[0].RSLT_MIN_HARDNESS,
                                    MAX_HARDNESS = IPC_RESULTS[0].RSLT_MAX_HARDNESS,
                                    UINAME = "정제체크마스터입력"
                                });


                                if (await _BR_BRS_REG_IPC_CHECKMASTER_MULTI_GUBUN.Execute())
                                {
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable("DATA");
                                    DataTable dtEbr = new DataTable("DATA2");
                                    ds.Tables.Add(dt);
                                    ds.Tables.Add(dtEbr);

                                    dt.Columns.Add(new DataColumn("장비번호"));
                                    dt.Columns.Add(new DataColumn("점검일시"));
                                    dt.Columns.Add(new DataColumn("평균질량"));
                                    dt.Columns.Add(new DataColumn("개별최소질량"));
                                    dt.Columns.Add(new DataColumn("개별최대질량"));
                                    dt.Columns.Add(new DataColumn("개별질량RSD"));
                                    dt.Columns.Add(new DataColumn("평균두께"));
                                    dt.Columns.Add(new DataColumn("최소두께"));
                                    dt.Columns.Add(new DataColumn("최대두께"));
                                    dt.Columns.Add(new DataColumn("평균경도"));
                                    dt.Columns.Add(new DataColumn("최소경도"));
                                    dt.Columns.Add(new DataColumn("최대경도"));

                                    //2024.05.27 김도연 : EBR에 평균값을 기록하기 위해 DataTable 따로 생성
                                    dtEbr.Columns.Add(new DataColumn("장비번호"));
                                    dtEbr.Columns.Add(new DataColumn("점검일시"));
                                    dtEbr.Columns.Add(new DataColumn("평균질량"));
                                    dtEbr.Columns.Add(new DataColumn("개별최소질량"));
                                    dtEbr.Columns.Add(new DataColumn("개별최대질량"));
                                    dtEbr.Columns.Add(new DataColumn("개별질량RSD"));
                                    dtEbr.Columns.Add(new DataColumn("평균두께"));
                                    dtEbr.Columns.Add(new DataColumn("최소두께"));
                                    dtEbr.Columns.Add(new DataColumn("최대두께"));
                                    dtEbr.Columns.Add(new DataColumn("평균경도"));
                                    dtEbr.Columns.Add(new DataColumn("최소경도"));
                                    dtEbr.Columns.Add(new DataColumn("최대경도"));

                                    var row = dt.NewRow();
                                    foreach (var rowdata in IPCResultSections)
                                    {
                                        row = dt.NewRow();
                                        row["장비번호"] = EQPTID;
                                        row["점검일시"] = rowdata.STRTDTTM != null ? rowdata.STRTDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                        row["평균질량"] = rowdata.AVG_WEIGHT.ToString();
                                        row["개별최소질량"] = rowdata.MIN_WEIGHT.ToString();
                                        row["개별최대질량"] = rowdata.MAX_WEIGHT.ToString();
                                        if (RSD_FLAG.Equals(false))
                                        {
                                            row["개별질량RSD"] = "N/A";
                                        }
                                        else
                                        {
                                            row["개별질량RSD"] = rowdata.SD_WEIGHT.ToString();
                                        }
                                        row["평균두께"] = rowdata.AVG_THICKNESS.ToString();
                                        row["최소두께"] = rowdata.MIN_THICKNESS.ToString();
                                        row["최대두께"] = rowdata.MAX_THICKNESS.ToString();
                                        if (CIRCLE_FLAG.Equals(false))
                                        {
                                            row["평균경도"] = "N/A";
                                            row["최소경도"] = "N/A";
                                            row["최대경도"] = "N/A";
                                        }
                                        else
                                        {
                                            row["평균경도"] = rowdata.AVG_HARDNESS.ToString();
                                            row["최소경도"] = rowdata.MIN_HARDNESS.ToString();
                                            row["최대경도"] = rowdata.MAX_HARDNESS.ToString();
                                        }

                                        dt.Rows.Add(row);
                                    }

                                    //2024.05.24 김도연 : 지시문에 평균값 도출
                                    row = dt.NewRow();
                                    row["장비번호"] = "결과";
                                    row["점검일시"] = AVG_DTTM != null ? AVG_DTTM.ToString("yyyy-MM-dd HH:mm") : ""; ;
                                    row["평균질량"] = IPC_RESULTS[0].RSLT_AVG_WEIGHT;
                                    row["개별최소질량"] = IPC_RESULTS[0].RSLT_MIN_WEIGHT;
                                    row["개별최대질량"] = IPC_RESULTS[0].RSLT_MAX_WEIGHT;
                                    row["개별질량RSD"] = IPC_RESULTS[0].RSLT_SD_WEIGHT;
                                    row["평균두께"] = IPC_RESULTS[0].RSLT_AVG_THICKNESS;
                                    row["최소두께"] = IPC_RESULTS[0].RSLT_MIN_THICKNESS;
                                    row["최대두께"] = IPC_RESULTS[0].RSLT_MAX_THICKNESS;
                                    row["평균경도"] = IPC_RESULTS[0].RSLT_AVG_HARDNESS;
                                    row["최소경도"] = IPC_RESULTS[0].RSLT_MIN_HARDNESS;
                                    row["최대경도"] = IPC_RESULTS[0].RSLT_MAX_HARDNESS;
                                    dt.Rows.Add(row);

                                    //2024.06.27 김도연 : EBR에 기준값 보이도록 데이터 input
                                    var rowEbr_STD = dtEbr.NewRow();
                                    rowEbr_STD["장비번호"] = "";
                                    rowEbr_STD["점검일시"] = "기준";
                                    rowEbr_STD["평균질량"] = IPC_STANDARDS[0].RSLT_AVG_WEIGHT.ToString();
                                    rowEbr_STD["개별최소질량"] = IPC_STANDARDS[0].RSLT_MIN_WEIGHT.ToString();
                                    rowEbr_STD["개별최대질량"] = IPC_STANDARDS[0].RSLT_MAX_WEIGHT.ToString();
                                    rowEbr_STD["개별질량RSD"] = IPC_STANDARDS[0].RSLT_SD_WEIGHT.ToString();
                                    rowEbr_STD["평균두께"] = IPC_STANDARDS[0].RSLT_AVG_THICKNESS.ToString();
                                    rowEbr_STD["최소두께"] = IPC_STANDARDS[0].RSLT_MIN_THICKNESS.ToString();
                                    rowEbr_STD["최대두께"] = IPC_STANDARDS[0].RSLT_MAX_THICKNESS.ToString();
                                    rowEbr_STD["평균경도"] = IPC_STANDARDS[0].RSLT_AVG_HARDNESS.ToString();
                                    rowEbr_STD["최소경도"] = IPC_STANDARDS[0].RSLT_MIN_HARDNESS.ToString();
                                    rowEbr_STD["최대경도"] = IPC_STANDARDS[0].RSLT_MAX_HARDNESS.ToString();
                                    dtEbr.Rows.Add(rowEbr_STD);

                                    //2024.05.24 김도연 : EBR에 평균값 보이도록 데이터 input
                                    var rowEbr_AVG = dtEbr.NewRow();
                                    rowEbr_AVG["장비번호"] = EQPTID;
                                    rowEbr_AVG["점검일시"] = AVG_DTTM != null ? AVG_DTTM.ToString("yyyy-MM-dd HH:mm") : ""; ;
                                    rowEbr_AVG["평균질량"] = IPC_RESULTS[0].RSLT_AVG_WEIGHT;
                                    rowEbr_AVG["개별최소질량"] = IPC_RESULTS[0].RSLT_MIN_WEIGHT;
                                    rowEbr_AVG["개별최대질량"] = IPC_RESULTS[0].RSLT_MAX_WEIGHT;
                                    rowEbr_AVG["개별질량RSD"] = IPC_RESULTS[0].RSLT_SD_WEIGHT;
                                    rowEbr_AVG["평균두께"] = IPC_RESULTS[0].RSLT_AVG_THICKNESS;
                                    rowEbr_AVG["최소두께"] = IPC_RESULTS[0].RSLT_MIN_THICKNESS;
                                    rowEbr_AVG["최대두께"] = IPC_RESULTS[0].RSLT_MAX_THICKNESS;
                                    rowEbr_AVG["평균경도"] = IPC_RESULTS[0].RSLT_AVG_HARDNESS;
                                    rowEbr_AVG["최소경도"] = IPC_RESULTS[0].RSLT_MIN_HARDNESS;
                                    rowEbr_AVG["최대경도"] = IPC_RESULTS[0].RSLT_MAX_HARDNESS;
                                    dtEbr.Rows.Add(rowEbr_AVG);

                                    var xml = BizActorRuleBase.CreateXMLStream(ds);
                                    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                    _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                    _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                    var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                    }

                                    if (KeepGoing)
                                    {
                                        var bizrule = new BR_PHR_REG_InstructionComment();

                                        bizrule.IN_Comments.Add(
                                            new BR_PHR_REG_InstructionComment.IN_Comment
                                            {
                                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                                COMMENTTYPE = "CM008",
                                                COMMENT = _comment,
                                                INSUSER = _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER
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
                                IsBusy = false;

                                CommandResults["ConfirmCommandAsync"] = true;
                            }

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

        #endregion
        #region User Define
        public class IPCResultSection : BizActorRuleBase
        {
            public sealed partial class OUTDATACollection : BufferedObservableCollection<OUTDATA>
            {
            }
            private OUTDATACollection _OUTDATAs;
            [BizActorOutputSetAttribute()]
            public OUTDATACollection OUTDATAs
            {
                get
                {
                    return this._OUTDATAs;
                }
            }
            [BizActorOutputSetDefineAttribute(Order = "0")]
            [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
            public partial class OUTDATA : BizActorDataSetBase
            {
                public OUTDATA()
                {
                    RowLoadedFlag = false;
                }
                private bool _RowLoadedFlag;
                public bool RowLoadedFlag
                {
                    get
                    {
                        return this._RowLoadedFlag;
                    }
                    set
                    {
                        this._RowLoadedFlag = value;
                        this.OnPropertyChanged("_RowLoadedFlag");
                    }
                }
                private string _RowIndex;
                public string RowIndex
                {
                    get
                    {
                        return this._RowIndex;
                    }
                    set
                    {
                        this._RowIndex = value;
                        this.OnPropertyChanged("RowIndex");
                    }
                }
                private string _RowEditSec;
                public string RowEditSec
                {
                    get
                    {
                        return this._RowEditSec;
                    }
                    set
                    {
                        this._RowEditSec = value;
                        this.OnPropertyChanged("RowEditSec");
                    }
                }
                private DateTime _STRTDTTM = DateTime.Now;
                [BizActorOutputItemAttribute()]
                public DateTime STRTDTTM
                {
                    get
                    {
                        return this._STRTDTTM;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._STRTDTTM = value;
                            this.CheckIsOriginal("STRTDTTM", value);
                            this.OnPropertyChanged("STRTDTTM");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _AVG_WEIGHT;
                [BizActorOutputItemAttribute()]
                public decimal AVG_WEIGHT
                {
                    get
                    {
                        return this._AVG_WEIGHT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._AVG_WEIGHT = value;
                            this.CheckIsOriginal("AVG_WEIGHT", value);
                            this.OnPropertyChanged("AVG_WEIGHT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _MIN_WEIGHT;
                [BizActorOutputItemAttribute()]
                public decimal MIN_WEIGHT
                {
                    get
                    {
                        return this._MIN_WEIGHT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MIN_WEIGHT = value;
                            this.CheckIsOriginal("MIN_WEIGHT", value);
                            this.OnPropertyChanged("MIN_WEIGHT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _MAX_WEIGHT;
                [BizActorOutputItemAttribute()]
                public decimal MAX_WEIGHT
                {
                    get
                    {
                        return this._MAX_WEIGHT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MAX_WEIGHT = value;
                            this.CheckIsOriginal("MAX_WEIGHT", value);
                            this.OnPropertyChanged("MAX_WEIGHT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _SD_WEIGHT;
                [BizActorOutputItemAttribute()]
                public decimal SD_WEIGHT
                {
                    get
                    {
                        return this._SD_WEIGHT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._SD_WEIGHT = value;
                            this.CheckIsOriginal("SD_WEIGHT", value);
                            this.OnPropertyChanged("SD_WEIGHT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _AVG_THICKNESS;
                [BizActorOutputItemAttribute()]
                public decimal AVG_THICKNESS
                {
                    get
                    {
                        return this._AVG_THICKNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._AVG_THICKNESS = value;
                            this.CheckIsOriginal("AVG_THICKNESS", value);
                            this.OnPropertyChanged("AVG_THICKNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _MIN_THICKNESS;
                [BizActorOutputItemAttribute()]
                public decimal MIN_THICKNESS
                {
                    get
                    {
                        return this._MIN_THICKNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MIN_THICKNESS = value;
                            this.CheckIsOriginal("MIN_THICKNESS", value);
                            this.OnPropertyChanged("MIN_THICKNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _MAX_THICKNESS;
                [BizActorOutputItemAttribute()]
                public decimal MAX_THICKNESS
                {
                    get
                    {
                        return this._MAX_THICKNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MAX_THICKNESS = value;
                            this.CheckIsOriginal("MAX_THICKNESS", value);
                            this.OnPropertyChanged("MAX_THICKNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _AVG_HARDNESS;
                [BizActorOutputItemAttribute()]
                public decimal AVG_HARDNESS
                {
                    get
                    {
                        return this._AVG_HARDNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._AVG_HARDNESS = value;
                            this.CheckIsOriginal("AVG_HARDNESS", value);
                            this.OnPropertyChanged("AVG_HARDNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }

                private decimal _MIN_HARDNESS;
                [BizActorOutputItemAttribute()]
                public decimal MIN_HARDNESS
                {
                    get
                    {
                        return this._MIN_HARDNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MIN_HARDNESS = value;
                            this.CheckIsOriginal("MIN_HARDNESS", value);
                            this.OnPropertyChanged("MIN_HARDNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _MAX_HARDNESS;
                [BizActorOutputItemAttribute()]
                public decimal MAX_HARDNESS
                {
                    get
                    {
                        return this._MAX_HARDNESS;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._MAX_HARDNESS = value;
                            this.CheckIsOriginal("MAX_HARDNESS", value);
                            this.OnPropertyChanged("MAX_HARDNESS");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
            }
            public IPCResultSection()
            {
                _OUTDATAs = new OUTDATACollection();
            }
        }

        public class EACH_INDATA : BizActorDataSetBase
        {

            private String _RSLT_AVG_WEIGHT;
            public String RSLT_AVG_WEIGHT
            {
                get { return _RSLT_AVG_WEIGHT; }
                set
                {
                    _RSLT_AVG_WEIGHT = value;
                    OnPropertyChanged("RSLT_AVG_WEIGHT");
                }
            }
            private String _RSLT_MIN_WEIGHT;
            public String RSLT_MIN_WEIGHT
            {
                get { return _RSLT_MIN_WEIGHT; }
                set
                {
                    _RSLT_MIN_WEIGHT = value;
                    OnPropertyChanged("RSLT_MIN_WEIGHT");
                }
            }
            private String _RSLT_MAX_WEIGHT;
            public String RSLT_MAX_WEIGHT
            {
                get { return _RSLT_MAX_WEIGHT; }
                set
                {
                    _RSLT_MAX_WEIGHT = value;
                    OnPropertyChanged("RSLT_MAX_WEIGHT");
                }
            }
            private String _RSLT_SD_WEIGHT;
            public String RSLT_SD_WEIGHT
            {
                get { return _RSLT_SD_WEIGHT; }
                set
                {
                    _RSLT_SD_WEIGHT = value;
                    OnPropertyChanged("RSLT_SD_WEIGHT");
                }
            }
            private String _RSLT_AVG_THICKNESS;
            public String RSLT_AVG_THICKNESS
            {
                get { return _RSLT_AVG_THICKNESS; }
                set
                {
                    _RSLT_AVG_THICKNESS = value;
                    OnPropertyChanged("RSLT_AVG_THICKNESS");
                }
            }
            private String _RSLT_MIN_THICKNESS;
            public String RSLT_MIN_THICKNESS
            {
                get { return _RSLT_MIN_THICKNESS; }
                set
                {
                    _RSLT_MIN_THICKNESS = value;
                    OnPropertyChanged("RSLT_MIN_THICKNESS");
                }

            }
            private String _RSLT_MAX_THICKNESS;
            public String RSLT_MAX_THICKNESS
            {
                get { return _RSLT_MAX_THICKNESS; }
                set
                {
                    _RSLT_MAX_THICKNESS = value;
                    OnPropertyChanged("RSLT_MAX_THICKNESS");
                }

            }
            private String _RSLT_AVG_HARDNESS;
            public String RSLT_AVG_HARDNESS
            {
                get { return _RSLT_AVG_HARDNESS; }
                set
                {
                    _RSLT_AVG_HARDNESS = value;
                    OnPropertyChanged("RSLT_AVG_HARDNESS");
                }

            }
            private String _RSLT_MIN_HARDNESS;
            public String RSLT_MIN_HARDNESS
            {
                get { return _RSLT_MIN_HARDNESS; }
                set
                {
                    _RSLT_MIN_HARDNESS = value;
                    OnPropertyChanged("RSLT_MIN_HARDNESS");
                }

            }
            private String _RSLT_MAX_HARDNESS;
            public String RSLT_MAX_HARDNESS
            {
                get { return _RSLT_MAX_HARDNESS; }
                set
                {
                    _RSLT_MAX_HARDNESS = value;
                    OnPropertyChanged("RSLT_MAX_HARDNESS");
                }
            }
            private String _RSLT_AVG_DTTM;
            public String RSLT_AVG_DTTM
            {
                get { return _RSLT_AVG_DTTM; }
                set
                {
                    _RSLT_AVG_DTTM = value;
                    OnPropertyChanged("RSLT_AVG_DTTM");
                }
            }
        }
        #endregion
    }
}