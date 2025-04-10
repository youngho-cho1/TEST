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
using System.Windows.Threading;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Collections.ObjectModel;

namespace 보령
{
    public class 설비가동시간체크_장비번호ViewModel : ViewModelBase
    {
        #region [Property]
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        private 설비가동시간체크_장비번호 _mainWnd;
        private InstructionModel curInst;
        private int? MIN = null, MAX = null;
        private string _UOM = "min";// 2021.08.02 박희돈 대문자 MIN -> 소문자 min으로 변경(제조기록서 표현방법)

        private ObservableCollection<LayerCharging> _ListContainer;
        public ObservableCollection<LayerCharging> ListContainer
        {
            get { return _ListContainer; }
            set
            {
                _ListContainer = value;
                OnPropertyChanged("ListContainer");
            }
        }

        private DateTime _STRTDTTM;
        public DateTime STRTDTTM
        {
            get { return _STRTDTTM; }
            set
            {
                _STRTDTTM = value;
                OnPropertyChanged("STRTDTTM");
            }
        }
        private DateTime _CURDTTM;
        public DateTime CURDTTM
        {
            get { return _CURDTTM; }
            set
            {
                _CURDTTM = value;
                OnPropertyChanged("CURDTTM");
            }
        }
        private string _WORKTIME;
        public string WORKTIME
        {
            get { return _WORKTIME; }
            set
            {
                _WORKTIME = value;
                OnPropertyChanged("WORKTIME");
            }
        }
        private string _BASELINE;
        public string BASELINE
        {
            get { return _BASELINE; }
            set
            {
                _BASELINE = value;
                OnPropertyChanged("BASELINE");
            }
        }
        #endregion

        #region [BizRule]
        private BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST _BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST;
        public BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST
        {
            get { return _BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST");
            }
        }
        #endregion
        public 설비가동시간체크_장비번호ViewModel()
        {
            _DispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
            _BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST = new 보령.BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST();
            ListContainer = new ObservableCollection<보령.설비가동시간체크_장비번호ViewModel.LayerCharging>();
        }

        #region [Command]
        public ICommand LoadCommandAsync
        {
            get
            {
                return new CommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["LoadCommandAsync"] = false;
                        CommandCanExecutes["LoadCommandAsync"] = false;

                        IsBusy = true;
                        ///
                        if (arg != null && arg is 설비가동시간체크_장비번호)
                        {
                            _mainWnd = arg as 설비가동시간체크_장비번호;
                            _mainWnd.Closed += (s, e) =>
                            {
                                if (_DispatcherTimer != null)
                                    _DispatcherTimer.Stop();

                                _DispatcherTimer = null;
                            };

                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                            curInst = _mainWnd.CurrentInstruction;

                            if (inputValues.Count < 0)
                                throw new Exception("시작시간이 없습니다.");
                            else
                                STRTDTTM = Convert.ToDateTime(inputValues[0].Raw.ACTVAL);

                            if (!string.IsNullOrWhiteSpace(curInst.Raw.UOMNOTATION) && curInst.Raw.UOMNOTATION == "분")
                                _UOM = "분";

                            setMinMaxValue();

                            // 초기값 세팅
                            CURDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            TimeSpan WorkingTime = _CURDTTM - _STRTDTTM;
                            WORKTIME = string.Format("{0} {1}", Math.Floor(WorkingTime.TotalMinutes), _UOM);
                            _DispatcherTimer.Start();

                            BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.INDATAs.Clear();
                            BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.OUTDATAs.Clear();

                            foreach (InstructionModel Inst in inputValues)
                            {
                                if("IT004" != Inst.InstructionType.ToString() && "IT005" != Inst.InstructionType.ToString())
                                {
                                    _BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.INDATA
                                    {
                                        LANGID = "ko-KR",
                                        ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        BATCHNO = _mainWnd.CurrentOrder.BatchNo,
                                        EQPTID = Inst.Raw.EQPTID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        ACTVAL = Inst.Raw.ACTVAL
                                    });
                                }
                            }

                            if (!await BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.Execute()) return;

                            if (BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.OUTDATAs.Count > 0)
                            {
                                foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.OUTDATAs)
                                {
                                    ListContainer.Add(new LayerCharging
                                    {
                                        EQPTID = item.EQPTID,
                                        STRTDTTM = STRTDTTM,
                                        CURDTTM = CURDTTM,
                                        WORKTIME = WORKTIME,
                                        BASELINE = BASELINE
                                    });
                                }
                            }

                            IsBusy = false;
                        }
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
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
                }
                    );

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
                        CommandResults["ConfirmCommandAsync"] = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        if (ValidationCheck())
                        {
                            _DispatcherTimer.Stop();

                            var authHelper = new iPharmAuthCommandHelper();

                            // 전자서명 요청
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "설비가동시간체크_장비번호",
                                "설비가동시간체크_장비번호",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }


                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            dt.Columns.Add(new DataColumn("장비번호"));
                            dt.Columns.Add(new DataColumn("시작시각"));
                            dt.Columns.Add(new DataColumn("종료시각"));
                            dt.Columns.Add(new DataColumn("필수가동시간"));
                            dt.Columns.Add(new DataColumn("가동시각"));
                            ds.Tables.Add(dt);

                            foreach(var item in ListContainer)
                            {
                                var row = dt.NewRow();
                                row["장비번호"] = item.EQPTID ?? "";
                                row["시작시각"] = item.STRTDTTM != null ? item.STRTDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                row["종료시각"] = item.CURDTTM != null ? item.CURDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                row["필수가동시간"] = item.BASELINE ?? "";
                                row["가동시각"] = item.WORKTIME ?? "";

                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            _DispatcherTimer.Stop();

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                        }
                        else
                            OnMessage("가동시간을 만족하지 못했습니다.");

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
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                }
                    );

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
                            dt.Columns.Add(new DataColumn("장비번호"));
                            dt.Columns.Add(new DataColumn("시작시각"));
                            dt.Columns.Add(new DataColumn("현재시각"));
                            dt.Columns.Add(new DataColumn("가동시각"));
                            dt.Columns.Add(new DataColumn("가동범위"));

                            var row = dt.NewRow();
                            row["장비번호"] = "N/A";
                            row["시작시각"] = "N/A";
                            row["현재시각"] = "N/A";
                            row["가동시각"] = "N/A";
                            row["가동범위"] = "N/A";
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

        #region [User Define]
        public class LayerCharging : ViewModelBase
        {
            private DateTime _STRTDTTM;
            public DateTime STRTDTTM
            {
                get { return _STRTDTTM; }
                set
                {
                    _STRTDTTM = value;
                    OnPropertyChanged("STRTDTTM");
                }
            }
            private DateTime _CURDTTM;
            public DateTime CURDTTM
            {
                get { return _CURDTTM; }
                set
                {
                    _CURDTTM = value;
                    OnPropertyChanged("CURDTTM");
                }
            }
            private string _WORKTIME;
            public string WORKTIME
            {
                get { return _WORKTIME; }
                set
                {
                    _WORKTIME = value;
                    OnPropertyChanged("WORKTIME");
                }
            }
            private string _BASELINE;
            public string BASELINE
            {
                get { return _BASELINE; }
                set
                {
                    _BASELINE = value;
                    OnPropertyChanged("BASELINE");
                }
            }
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
        }
        public void TimerStop()
        {
            _DispatcherTimer.Stop();
        }
        private async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                CURDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();
                TimeSpan WorkingTime = _CURDTTM - _STRTDTTM;
                WORKTIME = string.Format("{0} {1}", Math.Floor(WorkingTime.TotalMinutes), _UOM);

                if (BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.OUTDATAs.Count > 0)
                {
                    ListContainer.Clear();

                    foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT_LIST.OUTDATAs)
                    {
                        ListContainer.Add(new LayerCharging
                        {
                            EQPTID = item.EQPTID,
                            STRTDTTM = STRTDTTM,
                            CURDTTM = CURDTTM,
                            WORKTIME = WORKTIME,
                            BASELINE = BASELINE
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        private void setMinMaxValue()
        {
            try
            {
                int tar;

                if (string.IsNullOrWhiteSpace(curInst.Raw.MINVAL) && string.IsNullOrWhiteSpace(curInst.Raw.MAXVAL) && string.IsNullOrWhiteSpace(curInst.Raw.TARGETVAL))
                    BASELINE = "N/A";
                else
                {
                    if (!string.IsNullOrWhiteSpace(curInst.Raw.TARGETVAL) && Int32.TryParse(curInst.Raw.TARGETVAL, out tar))
                    {
                        MIN = tar;
                        BASELINE = string.Format("{0} ~ {1}", tar, _UOM);
                    }
                    else
                    {
                        BASELINE = curInst.Raw.BASELINE;
                        if (!string.IsNullOrWhiteSpace(curInst.Raw.MINVAL) && Int32.TryParse(curInst.Raw.MINVAL, out tar))
                            MIN = tar;
                        if (!string.IsNullOrWhiteSpace(curInst.Raw.MAXVAL) && Int32.TryParse(curInst.Raw.MAXVAL, out tar))
                            MAX = tar;
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        private bool ValidationCheck()
        {
            // 현재 WorkTime
            var numWorkTime = Convert.ToInt32(WORKTIME.Replace(_UOM, ""));


            if (BASELINE == "N/A")
                return true;

            if (MIN.HasValue && MAX.HasValue)
            {
                if (MIN.Value <= numWorkTime && numWorkTime <= MAX.Value)
                    return true;
            }
            else if (MIN.HasValue)
            {
                if (MIN.Value <= numWorkTime)
                    return true;
            }
            else if (MAX.HasValue)
            {
                if (numWorkTime <= MAX.Value)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
