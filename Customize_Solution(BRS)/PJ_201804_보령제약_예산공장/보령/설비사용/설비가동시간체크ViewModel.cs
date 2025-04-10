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
    public class 설비가동시간체크ViewModel : ViewModelBase
    {
        #region [Property]
        public 설비가동시간체크ViewModel()
        {
            _DispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
        }

        private 설비가동시간체크 _mainWnd;
        private InstructionModel curInst;
        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        private int? MIN = null, MAX = null;
        private string _UOM = "min";// 2021.08.02 박희돈 대문자 MIN -> 소문자 min으로 변경(제조기록서 표현방법)

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

        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(async arg =>
                    {
                        try
                        {
                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            IsBusy = true;
                            ///
                            if (arg != null && arg is 설비가동시간체크)
                            {
                                _mainWnd = arg as 설비가동시간체크;
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

                                IsBusy = false;
                            }                            
                            ///
                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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
                        }
                    );
                
            }
        }
        public ICommand RecordCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        try
                        {
                            CommandResults["RecordCommandAsync"] = false;
                            CommandCanExecutes["RecordCommandAsync"] = false;

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
                                    "설비가동시간체크",
                                    "설비가동시간체크",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }


                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                dt.Columns.Add(new DataColumn("시작시각"));
                                dt.Columns.Add(new DataColumn("종료시각"));
                                dt.Columns.Add(new DataColumn("필수가동시간"));
                                dt.Columns.Add(new DataColumn("가동시각"));
                                ds.Tables.Add(dt);

                                var row = dt.NewRow();
                                row["시작시각"] = _STRTDTTM != null ? _STRTDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                row["종료시각"] = _CURDTTM != null ? _CURDTTM.ToString("yyyy-MM-dd HH:mm") : "";
                                row["필수가동시간"] = _BASELINE ?? "";
                                row["가동시각"] = _WORKTIME ?? "";

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

                                _DispatcherTimer.Stop();

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            else
                                OnMessage("가동시간을 만족하지 못했습니다.");

                            IsBusy = false;
                            ///
                            CommandResults["RecordCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["RecordCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RecordCommandAsync"] = true;
                        }
                    }, arg =>
                        {
                            return CommandCanExecutes.ContainsKey("RecordCommandAsync") ?
                                CommandCanExecutes["RecordCommandAsync"] : (CommandCanExecutes["RecordCommandAsync"] = true);
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
                            dt.Columns.Add(new DataColumn("시작시간"));
                            dt.Columns.Add(new DataColumn("현재시각"));
                            dt.Columns.Add(new DataColumn("가동시각"));
                            dt.Columns.Add(new DataColumn("가동범위"));

                            var row = dt.NewRow();
                            row["시작시간"] = "N/A";
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
                    if(!string.IsNullOrWhiteSpace(curInst.Raw.TARGETVAL) && Int32.TryParse(curInst.Raw.TARGETVAL, out tar))
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
            var numWorkTime = Convert.ToInt32(WORKTIME.Replace(_UOM,""));


            if (BASELINE == "N/A")
                return true;           

            if(MIN.HasValue && MAX.HasValue)
            {
                if (MIN.Value <= numWorkTime && numWorkTime <= MAX.Value)
                    return true;
            }
            else if(MIN.HasValue)
            {
                if (MIN.Value <= numWorkTime)
                    return true;
            }
            else if(MAX.HasValue)
            {
                if (numWorkTime <= MAX.Value)
                    return true;
            }
            
            return false;
        } 
        #endregion
    }
}
