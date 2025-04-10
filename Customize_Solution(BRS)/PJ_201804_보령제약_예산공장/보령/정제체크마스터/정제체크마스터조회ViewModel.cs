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

namespace 보령
{
    public class 정제체크마스터조회ViewModel : ViewModelBase
    {
        #region [Property]
        public 정제체크마스터조회ViewModel()
        {
            _BR_PHR_SEL_CODE = new BR_PHR_SEL_CODE();
            _BR_BRS_GET_Selector_Check_Master = new BR_BRS_GET_Selector_Check_Master();
            _BR_BRS_REG_IPC_CHECKMASTER_MULTI = new BR_BRS_REG_IPC_CHECKMASTER_MULTI();
        }

        정제체크마스터조회 _mainWnd;

        private string _EQPTID;
        public string EQPTID
        {
            get { return _EQPTID; }
            set
            {
                _EQPTID = value;
                OnPropertyChanged("EQPTID");
            }
        }

        private string _EQPTNAME;
        public string EQPTNAME
        {
            get { return _EQPTNAME; }
            set
            {
                _EQPTNAME = value;
                OnPropertyChanged("EQPTNAME");
            }
        }

        private DateTime _FROMDATE;
        public DateTime FROMDATE
        {
            get { return _FROMDATE; }
            set
            {
                _FROMDATE = value;
                OnPropertyChanged("FROMDATE");
            }
        }
        private string _FROMHOUR;
        public string FROMHOUR
        {
            get { return _FROMHOUR; }
            set
            {
                _FROMHOUR = value;
                OnPropertyChanged("FROMHOUR");
            }
        }
        private string _FROMMINUTE;
        public string FROMMINUTE
        {
            get { return _FROMMINUTE; }
            set
            {
                _FROMMINUTE = value;
                OnPropertyChanged("FROMMINUTE");
            }
        }

        private DateTime _TODATE;
        public DateTime TODATE
        {
            get { return _TODATE; }
            set
            {
                _TODATE = value;
                OnPropertyChanged("TODATE");
            }
        }
        private string _TOHOUR;
        public string TOHOUR
        {
            get { return _TOHOUR; }
            set
            {
                _TOHOUR = value;
                OnPropertyChanged("TOHOUR");
            }
        }
        private string _TOMINUTE;
        public string TOMINUTE
        {
            get { return _TOMINUTE; }
            set
            {
                _TOMINUTE = value;
                OnPropertyChanged("TOMINUTE");
            }
        }

        private bool _SEARCH_ENABLE;
        public bool SEARCH_ENABLE
        {
            get { return _SEARCH_ENABLE; }
            set
            {
                _SEARCH_ENABLE = value;
                OnPropertyChanged("SEARCH_ENABLE");
            }
        }
        private bool _RECORD_ENABLE;
        public bool RECORD_ENABLE
        {
            get { return _RECORD_ENABLE; }
            set
            {
                _RECORD_ENABLE = value;
                OnPropertyChanged("RECORD_ENABLE");
            }
        }
        #endregion

        #region [Bizrule]
        private BR_PHR_SEL_CODE _BR_PHR_SEL_CODE;

        private BR_BRS_GET_Selector_Check_Master _BR_BRS_GET_Selector_Check_Master;
        public BR_BRS_GET_Selector_Check_Master BR_BRS_GET_Selector_Check_Master
        {
            get { return _BR_BRS_GET_Selector_Check_Master; }
            set
            {
                _BR_BRS_GET_Selector_Check_Master = value;
                OnPropertyChanged("BR_BRS_GET_Selector_Check_Master");
            }
        }

        private BR_BRS_REG_IPC_CHECKMASTER_MULTI _BR_BRS_REG_IPC_CHECKMASTER_MULTI;
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

                            ///
                            if (arg != null && arg is 정제체크마스터조회)
                            {
                                _mainWnd = arg as 정제체크마스터조회;

                                EQPTID = "";

                                DateTime toDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                DateTime fromDttm = toDttm.AddDays(-1);

                                FROMDATE = fromDttm.Date;
                                FROMHOUR = fromDttm.Hour.ToString("00");
                                FROMMINUTE = fromDttm.Minute.ToString("00");
                                TODATE = toDttm.Date;
                                TOHOUR = toDttm.Hour.ToString("00");
                                TOMINUTE = toDttm.Minute.ToString("00");
                            }

                            SEARCH_ENABLE = false;
                            RECORD_ENABLE = false;
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


        public ICommand SearchEquipmentCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SearchEquipmentCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SearchEquipmentCommandAsync"] = false;
                            CommandCanExecutes["SearchEquipmentCommandAsync"] = false;

                            ///
                            if (arg != null && arg is string)
                            {
                                string id = arg as string;

                                // 설비 체크
                                _BR_PHR_SEL_CODE.INDATAs.Clear();
                                _BR_PHR_SEL_CODE.OUTDATAs.Clear();
                                _BR_PHR_SEL_CODE.INDATAs.Add(new BR_PHR_SEL_CODE.INDATA()
                                {
                                    TYPE = "Equipment",
                                    LANGID = "ko-KR",
                                    CODE = id
                                });

                                if (await _BR_PHR_SEL_CODE.Execute() && _BR_PHR_SEL_CODE.OUTDATAs.Count > 0)
                                {
                                    EQPTID = _BR_PHR_SEL_CODE.OUTDATAs[0].CODE;
                                    EQPTNAME = _BR_PHR_SEL_CODE.OUTDATAs[0].NAME.Replace("[" + _EQPTID + "]", "");
                                    SEARCH_ENABLE = true;
                                }
                                else
                                    SEARCH_ENABLE = false;

                            }
                            ///

                            CommandResults["SearchEquipmentCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SearchEquipmentCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SearchEquipmentCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SearchEquipmentCommandAsync") ?
                        CommandCanExecutes["SearchEquipmentCommandAsync"] : (CommandCanExecutes["SearchEquipmentCommandAsync"] = true);
                });
            }
        }


        public ICommand NumericPopupCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["NumericPopupCommand"] = false;
                        CommandCanExecutes["NumericPopupCommand"] = false;

                        ///
                        if (arg != null && arg is TextBox)
                        {
                            TextBox tar = arg as TextBox;

                            ShopFloorUI.KeyPadPopUp popup = new ShopFloorUI.KeyPadPopUp();
                            popup.Closed += (s, e) =>
                            {
                                if (popup.DialogResult.GetValueOrDefault())
                                {
                                    int chk;
                                    if (Int32.TryParse(popup.Value, out chk))
                                    {
                                        if (tar.Name == "txtFromHour" || tar.Name == "txtToHour")
                                        {
                                            if (0 <= chk && chk < 24)
                                            {
                                                if (tar.Name == "txtFromHour")
                                                    FROMHOUR = chk.ToString("00");
                                                else if (tar.Name == "txtToHour")
                                                    TOHOUR = chk.ToString("00");
                                            }
                                            else
                                                OnMessage("잘못된 형식 입니다.");
                                        }
                                        else if (tar.Name == "txtFromMinute" || tar.Name == "txtToMinute")
                                        {
                                            if (0 <= chk && chk < 60)
                                            {
                                                if (tar.Name == "txtFromMinute")
                                                    FROMMINUTE = chk.ToString("00");
                                                else if (tar.Name == "txtToMinute")
                                                    TOMINUTE = chk.ToString("00");
                                            }
                                            else
                                                OnMessage("잘못된 형식 입니다.");
                                        }
                                    }
                                }
                            };
                            popup.Show();
                        }
                        ///

                        CommandResults["NumericPopupCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["NumericPopupCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["NumericPopupCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NumericPopupCommand") ?
                        CommandCanExecutes["NumericPopupCommand"] : (CommandCanExecutes["NumericPopupCommand"] = true);
                });
            }
        }


        public ICommand GetIPCResultCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["GetIPCResultCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["GetIPCResultCommandAsync"] = false;
                            CommandCanExecutes["GetIPCResultCommandAsync"] = false;

                            ///
                            BR_BRS_GET_Selector_Check_Master.INDATAs.Clear();
                            BR_BRS_GET_Selector_Check_Master.OUTDATAs.Clear();
                            BR_BRS_GET_Selector_Check_Master.INDATAs.Add(new BR_BRS_GET_Selector_Check_Master.INDATA
                            {
                                EQPTID = EQPTID,
                                FROMDATETIME = string.Format("{0} {1}:{2}", _FROMDATE.ToString("yyyy-MM-dd"), _FROMHOUR, _FROMMINUTE),
                                TODATETIME = string.Format("{0} {1}:{2}", _TODATE.ToString("yyyy-MM-dd"), _TOHOUR, _TOMINUTE),
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                            });

                            if (await BR_BRS_GET_Selector_Check_Master.Execute())
                            {
                                RECORD_ENABLE = true;
                            }
                            else
                            {
                                RECORD_ENABLE = false;
                            }
                            ///

                            CommandResults["GetIPCResultCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["GetIPCResultCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["GetIPCResultCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("GetIPCResultCommandAsync") ?
                        CommandCanExecutes["GetIPCResultCommandAsync"] : (CommandCanExecutes["GetIPCResultCommandAsync"] = true);
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
                            if (BR_BRS_GET_Selector_Check_Master.OUTDATAs.Count > 0)
                            {
                                var authHelper = new iPharmAuthCommandHelper();
                                // 재기록
                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                                {
                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("기록값을 변경합니다."),
                                        string.Format("기록값 변경"),
                                        false,
                                        "OM_ProductionOrder_SUI",
                                        "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }

                                // 전자서명 후 BR 실행
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("정제체크마스터조회"),
                                    string.Format("정제체크마스터조회"),
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // BR_BRS_REG_IPC_CHECKMASTER_MULTI IPC 결과 테이블에 저장
                                _BR_BRS_REG_IPC_CHECKMASTER_MULTI.INDATAs.Clear();
                                foreach (var item in BR_BRS_GET_Selector_Check_Master.OUTDATAs)
                                {
                                    decimal chk;
                                    decimal avgWeight = 0, avgThick = 0, avgHardness = 0, avgDiameter = 0;

                                    if (decimal.TryParse(item.AVG_WEIGHT.Replace(item.WEIGHTUOM, ""), out chk))
                                        avgWeight = chk;
                                    if (decimal.TryParse(item.AVG_THICK.Replace(item.THICKUOM, ""), out chk))
                                        avgThick = chk;
                                    if (decimal.TryParse(item.AVG_HARDNESS.Replace(item.HARDNESSUOM, ""), out chk))
                                        avgHardness = chk;
                                    if (decimal.TryParse(item.AVG_DIAMETER.Replace(item.DIAMETERUOM, ""), out chk))
                                        avgDiameter = chk;

                                    _BR_BRS_REG_IPC_CHECKMASTER_MULTI.INDATAs.Add(new BR_BRS_REG_IPC_CHECKMASTER_MULTI.INDATA
                                    {
                                        EQPTID = item.EQPTID != null ? item.EQPTID : "",
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        SMPQTY = item.SMPQTY,
                                        USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI"),
                                        STRTDTTM = item.STDTTM,
                                        LOCATIONID = AuthRepositoryViewModel.Instance.RoomID,
                                        AVG_WEIGHT = avgWeight.ToString(),
                                        AVG_THICKNESS = avgThick.ToString(),
                                        AVG_HARDNESS = avgHardness.ToString(),
                                        AVG_DIAMETER = avgDiameter.ToString()
                                    });

                                }

                                if (await _BR_BRS_REG_IPC_CHECKMASTER_MULTI.Execute())
                                {

                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);

                                    dt.Columns.Add(new DataColumn("장비번호"));
                                    dt.Columns.Add(new DataColumn("점검일시"));
                                    dt.Columns.Add(new DataColumn("평균질량"));
                                    dt.Columns.Add(new DataColumn("개별최소질량"));
                                    dt.Columns.Add(new DataColumn("개별최대질량"));
                                    dt.Columns.Add(new DataColumn("평균두께"));
                                    dt.Columns.Add(new DataColumn("최소두께"));
                                    dt.Columns.Add(new DataColumn("최대두께"));
                                    dt.Columns.Add(new DataColumn("평균경도"));
                                    dt.Columns.Add(new DataColumn("최소경도"));
                                    dt.Columns.Add(new DataColumn("최대경도"));
                                    dt.Columns.Add(new DataColumn("평균직경"));
                                    dt.Columns.Add(new DataColumn("최소직경"));
                                    dt.Columns.Add(new DataColumn("최대직경"));

                                    foreach (var rowdata in BR_BRS_GET_Selector_Check_Master.OUTDATAs)
                                    {
                                        var row = dt.NewRow();
                                        row["장비번호"] = rowdata.EQPTID;
                                        row["점검일시"] = rowdata.STDATETIME != null ? rowdata.STDATETIME : "";
                                        row["평균질량"] = rowdata.AVG_WEIGHT != null ? rowdata.AVG_WEIGHT : "";
                                        row["개별최소질량"] = rowdata.MIN_WEIGHT != null ? rowdata.MIN_WEIGHT : "";
                                        row["개별최대질량"] = rowdata.MAX_WEIGHT != null ? rowdata.MAX_WEIGHT : "";
                                        row["평균두께"] = rowdata.AVG_THICK != null ? rowdata.AVG_THICK : "";
                                        row["최소두께"] = rowdata.MIN_THICK != null ? rowdata.MIN_THICK : "";
                                        row["최대두께"] = rowdata.MAX_THICK != null ? rowdata.MAX_THICK : "";
                                        row["평균경도"] = rowdata.AVG_HARDNESS != null ? rowdata.AVG_HARDNESS : "";
                                        row["최소경도"] = rowdata.MIN_HARDNESS != null ? rowdata.MIN_HARDNESS : "";
                                        row["최대경도"] = rowdata.MAX_HARDNESS != null ? rowdata.MAX_HARDNESS : "";
                                        row["평균직경"] = rowdata.AVG_DIAMETER != null ? rowdata.AVG_DIAMETER : "";
                                        row["최소직경"] = rowdata.MIN_DIAMETER != null ? rowdata.MIN_DIAMETER : "";
                                        row["최대직경"] = rowdata.MAX_DIAMETER != null ? rowdata.MAX_DIAMETER : "";

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
                                OnMessage("조회결과가 없습니다.");
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
        #endregion
    }
}
