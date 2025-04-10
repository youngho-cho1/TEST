using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Net;
using System.Threading;
using System.Windows.Threading;
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
using System.Collections.Generic;
using C1.Silverlight.Chart;
using System.IO;
using C1.Silverlight.Chart.Extended;
using LGCNS.iPharmMES.Recipe.Common;
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;

namespace 보령
{
    public class 장비이력데이터멀티ViewModel : ViewModelBase
    {

        private int ErrorChk = 0;

        Visibility _isProtoSeriesVisible;
        public Visibility IsProtoSeriesVisible
        {
            get { return _isProtoSeriesVisible; }
            set
            {
                _isProtoSeriesVisible = value;
                NotifyPropertyChanged();
            }
        }

        string _protoSeries;
        public string ProtoSeries
        {
            get { return _protoSeries; }
            set
            {
                _protoSeries = value;
                NotifyPropertyChanged();
            }
        }

        DateTime _fromDt;
        public DateTime FromDt
        {
            get { return _fromDt; }
            set
            {
                _fromDt = value;

                if (_fromDt != null) this.KeyPadFromTIme = String.Format("{0:HHmmss}", _fromDt);

                NotifyPropertyChanged();
            }
        }

        DateTime _toDt;
        public DateTime ToDt
        {
            get { return _toDt; }
            set
            {
                _toDt = value;
                if (_toDt != null) this.KeyPadToTIme = String.Format("{0:HHmmss}", _toDt);

                NotifyPropertyChanged();
            }
        }

        string _txtEQPTID;
        public string txtEQPTID
        {
            get { return _txtEQPTID; }
            set
            {
                _txtEQPTID = value;
                NotifyPropertyChanged();
            }
        }

        string _SelectedFromDt;
        public string SelectedFromDt
        {
            get { return _SelectedFromDt; }
            set
            {
                _SelectedFromDt = value;
                refreshSummaryList();
                NotifyPropertyChanged();
            }
        }
        string _SelectedToDt;
        public string SelectedToDt
        {
            get { return _SelectedToDt; }
            set
            {
                _SelectedToDt = value;
                refreshSummaryList();
                NotifyPropertyChanged();
            }
        }

        string _KeyPadFromTIme;
        public string KeyPadFromTIme
        {
            get { return _KeyPadFromTIme; }
            set
            {
                _KeyPadFromTIme = value;
                NotifyPropertyChanged();
            }
        }

        string _KeyPadToTIme;
        public string KeyPadToTIme
        {
            get { return _KeyPadToTIme; }
            set
            {
                _KeyPadToTIme = value;
                NotifyPropertyChanged();
            }
        }

        String _AnnoFormat = "HH;mm";
        public String AnnoFormat
        {
            get { return _AnnoFormat; }
            set
            {
                _AnnoFormat = value;
                NotifyPropertyChanged();
            }
        }

        Int32 _AnnoAngle = 0;
        public Int32 AnnoAngle
        {
            get { return _AnnoAngle; }
            set
            {
                _AnnoAngle = value;
                NotifyPropertyChanged();
            }
        }

        Int32 _AnnoFontsize = 8;
        public Int32 AnnoFontsize
        {
            get { return _AnnoFontsize; }
            set
            {
                _AnnoFontsize = value;
                NotifyPropertyChanged();
            }
        }

        double _ChartYMax;
        public double ChartYMax
        {
            get { return _ChartYMax; }
            set
            {
                _ChartYMax = value;
                NotifyPropertyChanged();
            }
        }

        double _ChartYMin;
        public double ChartYMin
        {
            get { return _ChartYMin; }
            set
            {
                _ChartYMin = value;
                NotifyPropertyChanged();
            }
        }

        int _SummarySelectedIndex;
        public int SummarySelectedIndex
        {
            get { return _SummarySelectedIndex; }
            set
            {
                _SummarySelectedIndex = value;
                NotifyPropertyChanged();
            }
        }

        Visibility _IsVisibleISaveImage = Visibility.Visible;
        public Visibility IsVisibleISaveImage
        {
            get { return _IsVisibleISaveImage; }
            set
            {
                _IsVisibleISaveImage = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<string, byte[]> TagImageList = new Dictionary<string, byte[]>();

        bool isDeviation = false;

        장비이력데이터멀티 _mainWnd;

        IEnumerable<dynamic> _tagIDs;
        List<InstructionModel> _ReceiveValues;
        List<InstructionModel> _inputValues;
        List<BR_BRS_SEL_HistoryData.OUTDATA> outdatas;

        BR_PHR_SEL_Element_ELMNAME _BR_PHR_SEL_Element_ELMNAME;
        BR_PHR_SEL_Element_Variable _BR_PHR_SEL_Element_Variable;
        ObservableCollection<BR_PHR_SEL_Element_Variable.OUTDATA> _BR_PHR_SEL_Element_VariableOUTDATAs;        

        BR_BRS_SEL_HistoryData.OUTDATA_SUMMARYCollection _filteredComponents;
        public BR_BRS_SEL_HistoryData.OUTDATA_SUMMARYCollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

        static Color[] _seriesColors =
        {
            Colors.Green,
            Colors.Orange,
            Colors.Brown,
            Colors.Purple,
            Colors.Black,
            Colors.Cyan,
        };

        public ICommand LoadedCommandAsync
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
                            _mainWnd = arg as 장비이력데이터멀티;

                            FromDt = (await AuthRepositoryViewModel.GetDBDateTimeNow()).AddHours(-1);
                            ToDt = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            _ReceiveValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            List<InstructionModel> equipmentRunTime = _ReceiveValues.Where(o => o.Raw.IRTTYPE.Equals("IT004")).OrderBy(o=>o.Raw.IRTSEQ).ToList();
                            if (equipmentRunTime != null && equipmentRunTime.Count() == 2)
                            {
                                DateTime instFromDT;
                                DateTime instToDT;

                                if (!DateTime.TryParse(equipmentRunTime[0].Raw.ACTVAL, out instFromDT))
                                {
                                    OnMessage("설비 가동 시작일시 또는 종료일시가 기록되지않았습니다!");
                                }
                                else if (!DateTime.TryParse(equipmentRunTime[1].Raw.ACTVAL, out instToDT))
                                {
                                    OnMessage("설비 가동 시작일시 또는 종료일시가 기록되지않았습니다!");
                                }
                                else
                                {
                                    FromDt = instFromDT;
                                    ToDt = instToDT;
                                }
                            }
                            else
                            {
                                OnMessage("설비 가동 시작일시 또는 종료일시가 기록되지않았습니다!");
                            }

                            _inputValues = _ReceiveValues.Where(o => !Convert.ToString(o.Raw.TAGID).Equals("")).OrderBy(o => o.Raw.IRTSEQ).ToList();
                            if (_inputValues.Count <= 0)
                            {
                                OnMessage("조회할 태그 정보가 1개 이상이어야 합니다");
                                return;
                            }
                            
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
        
        public ICommand SaveSelectionTabItemCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveSelectionTabItemCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SaveSelectionTabItemCommandAsync"] = false;
                            CommandCanExecutes["SaveSelectionTabItemCommandAsync"] = false;

                            ///
                            IsVisibleISaveImage = Visibility.Collapsed;

                            if (SummarySelectedIndex >= 0)
                            {
                                TabItem tabItem = _mainWnd.tabTags.Items[SummarySelectedIndex] as TabItem;

                                if (tabItem != null)
                                {
                                    byte[] img = null;

                                    TagImageList.TryGetValue(tabItem.Header.ToString(), out img);
                                    if (img != null) TagImageList.Remove(FilteredComponents[SummarySelectedIndex].TAGID);

                                    Border printArea = tabItem.FindName("PrintArea") as Border;

                                    if (printArea != null)
                                    {
                                        
                                        Brush oldBorderBrush = printArea.BorderBrush;
                                        Thickness oldBorderThickness = printArea.BorderThickness;
                                        Brush oldBackground = printArea.Background;

                                        printArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                                        printArea.BorderThickness = new System.Windows.Thickness(1);
                                        printArea.Background = new SolidColorBrush(Colors.White);

                                        TagImageList.Add(FilteredComponents[SummarySelectedIndex].TAGID, imageToByteArray(printArea).Result);

                                        printArea.BorderBrush = oldBorderBrush;
                                        printArea.BorderThickness = oldBorderThickness;
                                        printArea.Background = oldBackground;
                                    }
                                }
                            }

                            CommandResults["SaveSelectionTabItemCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveSelectionTabItemCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveSelectionTabItemCommandAsync"] = true;

                            IsVisibleISaveImage = Visibility.Visible;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveSelectionTabItemCommandAsync") ?
                        CommandCanExecutes["SaveSelectionTabItemCommandAsync"] : (CommandCanExecutes["SaveSelectionTabItemCommandAsync"] = true);
                });
            }
        }

        public ICommand SummarySelectionChangedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SummarySelectionChangedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SummarySelectionChangedCommandAsync"] = false;
                            CommandCanExecutes["SummarySelectionChangedCommandAsync"] = false;

                            ///
                            DataGrid dg = arg as DataGrid;

                            if (dg != null)
                            {
                                _mainWnd.tabTags.SelectedIndex = dg.SelectedIndex;
                            }

                            CommandResults["SummarySelectionChangedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SummarySelectionChangedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SummarySelectionChangedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SummarySelectionChangedCommandAsync") ?
                        CommandCanExecutes["SummarySelectionChangedCommandAsync"] : (CommandCanExecutes["SummarySelectionChangedCommandAsync"] = true);
                });
            }
        }
            
        public ICommand FromTimeChangedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["FromTimeChangedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["FromTimeChangedCommandAsync"] = false;
                            CommandCanExecutes["FromTimeChangedCommandAsync"] = false;

                            ///

                            var formats = new[] { "yyyyMMddHHmmss" };
                            DateTime dtRtn;

                            if (!DateTime.TryParseExact(String.Format("{0:yyyyMMdd}{1:D6}",this.FromDt,Convert.ToInt32(this.KeyPadFromTIme)), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRtn))
                            {
                                OnMessage("잘못된 시간 형식입니다! 시작일시를 정확하게 입력하세요.");
                                return;
                            }

                            this.FromDt = dtRtn;

                            ///

                            CommandResults["FromTimeChangedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["FromTimeChangedCommandAsync"] = false;
                            if (ErrorChk != 1)
                                OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["FromTimeChangedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("FromTimeChangedCommandAsync") ?
                        CommandCanExecutes["FromTimeChangedCommandAsync"] : (CommandCanExecutes["FromTimeChangedCommandAsync"] = true);
                });
            }
        }

        public ICommand ToTimeChangedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ToTimeChangedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ToTimeChangedCommandAsync"] = false;
                            CommandCanExecutes["ToTimeChangedCommandAsync"] = false;

                            ///

                            var formats = new[] { "yyyyMMddHHmmss" };
                            DateTime dtRtn;

                            if (!DateTime.TryParseExact(String.Format("{0:yyyyMMdd}{1:D6}", this.ToDt, Convert.ToInt32(this.KeyPadToTIme)), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRtn))
                            {
                                OnMessage("잘못된 시간 형식입니다! 종료일시를 정확하게 입력하세요.");
                                return;
                            }

                            this.ToDt = dtRtn;

                            ///

                            CommandResults["ToTimeChangedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ToTimeChangedCommandAsync"] = false;
                            if (ErrorChk != 1)
                                OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ToTimeChangedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ToTimeChangedCommandAsync") ?
                        CommandCanExecutes["ToTimeChangedCommandAsync"] : (CommandCanExecutes["ToTimeChangedCommandAsync"] = true);
                });
            }
        }

        public ICommand SearchCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SearchCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SearchCommand"] = false;
                            CommandCanExecutes["SearchCommand"] = false;

                            ///
                            ErrorChk = 1;
                            txtEQPTID = txtEQPTID.ToUpper();

                            if (await checkEqptInfo(txtEQPTID) == true)
                            {
                                await GetValues(this.FromDt, this.ToDt, txtEQPTID);
                            };

                            ///

                            CommandResults["SearchCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SearchCommand"] = false;
                            if (ErrorChk != 1)
                                OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SearchCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SearchCommand") ?
                        CommandCanExecutes["SearchCommand"] : (CommandCanExecutes["SearchCommand"] = true);
                });
            }
        }

        private void WaitTime(int milliseconds)
        {
            DateTime startDT = DateTime.Now;

            while (true)
            {
                TimeSpan ts = DateTime.Now - startDT;

                if (ts.TotalMilliseconds >= milliseconds)
                {
                    break;
                }
            }
        }

        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommand"] = false;

                            ///
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                var authHelper = new iPharmAuthCommandHelper();
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

                            bool isDeviationconfirm = false;
                            isDeviation = false;

                            if (FilteredComponents.Where(o => o.STATUS == "NG").FirstOrDefault() != null) isDeviation = true;

                            if (isDeviation == true &&
                                _mainWnd.CurrentInstruction.Raw.DVTPASSYN != "Y" &&
                                _mainWnd.CurrentInstruction.Raw.VLTTYPE != enumValidationTypeCode.QMVLTNONE.ToString())
                            {
                                if (await OnMessageAsync("입력값이 기준값을 벗어났습니다. 기록을 진행하시겟습니까?", true) == false) return;

                                var authHelper = new iPharmAuthCommandHelper();

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

                                isDeviationconfirm = true;
                            }

                            #region 결과 수신 지시문 기록
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "장비이력데이터";

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            #region Equipment Tag별 이미지 저장
                            //Tag별 이미지 생성
                            List<byte[]> imageList = new List<byte[]>();
                            int imageIndex = 0;
                            string errorMsg = string.Empty;

                            foreach (var item in FilteredComponents)
                            {
                                byte[] img = null;

                                TagImageList.TryGetValue(item.TAGID, out img);

                                if (img == null) errorMsg += string.Format("[{0}] TAG 확인되지않음. \n", item.TAGID);
                            }

                            if (errorMsg != string.Empty)
                            {
                                await OnMessageAsync(errorMsg);

                                return;
                            }

                            //Tag별 지시문 이미지 저장
                            var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            foreach (var irt in outputValues.Where(o=>o.Raw.EQPTID != null && o.Raw.TAGID != null).OrderBy(o=> o.Raw.IRTSEQ).ToList())
                            {
                                byte[] img = null;
                                string tagID = irt.Raw.TAGID.Replace(irt.Raw.EQPTID, txtEQPTID);

                                if (TagImageList.TryGetValue(tagID, out img))
                                {
                                    //string CMT = string.IsNullOrWhiteSpace(FilteredComponents[imageIndex].COMMENT) ? "" : ", Comment : " + FilteredComponents[imageIndex].COMMENT;

                                    //irt.Raw.ACTVAL = "TAG 명 : " + FilteredComponents[imageIndex].TAGNAME + ", AVG : " + FilteredComponents[imageIndex].AVG +
                                    //                  ", Min : " + FilteredComponents[imageIndex].MIN + ", Max : " + FilteredComponents[imageIndex].MAX + CMT;

                                    irt.Raw.NOTE = img;
                                    irt.Raw.ACTVAL = "장비이력데이터";

                                    result = await _mainWnd.Phase.RegistInstructionValue(irt);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", irt.Raw.IRTGUID, result));
                                    }
                                }
                            }
                            #endregion                                                       

                            #region Deviation comment 기록
                            if (isDeviationconfirm)
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
                            #endregion

                            #endregion

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            ///

                            CommandResults["ConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommand"] = false;
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

        async Task<bool> checkEqptInfo(string eqptid)
        {
            _BR_PHR_SEL_Element_ELMNAME.INDATAs.Add(new BR_PHR_SEL_Element_ELMNAME.INDATA()
            {
                EQPTID = eqptid,
            });

            if (await _BR_PHR_SEL_Element_ELMNAME.Execute() == false)
            {
                OnMessage(string.Format("{0} 장비 정보를 조회할 수 없습니다", "[" + eqptid + "]"));
                return false;
            }

            if (_BR_PHR_SEL_Element_ELMNAME.OUTDATAs.Count <= 0)
            {
                OnMessage(string.Format("{0} 장비 정보를 조회할 수 없습니다", "[" + eqptid + "]"));
                return false;
            }

            _BR_PHR_SEL_Element_VariableOUTDATAs.Clear();

            foreach (var indata in _inputValues)
            {
                _BR_PHR_SEL_Element_Variable.INDATAs.Add(new BR_PHR_SEL_Element_Variable.INDATA()
                {
                    ELMNO = _BR_PHR_SEL_Element_ELMNAME.OUTDATAs[0].ELMNO,
                    TAGID = indata.Raw.TAGID.Replace(indata.Raw.EQPTID, eqptid)
                });

                if (await _BR_PHR_SEL_Element_Variable.Execute() == false || _BR_PHR_SEL_Element_Variable.OUTDATAs.Count <= 0)
                {
                    OnMessage(string.Format("{0}의 {1} 태그 정보를 조회할 수 없습니다", "[" + eqptid + "]", "[" + indata.Raw.TAGID.Replace(indata.Raw.EQPTID, eqptid) + "]"));
                    return false;
                }
                if (_BR_PHR_SEL_Element_Variable.OUTDATAs.Count > 0)
                {
                    foreach (var item in _BR_PHR_SEL_Element_Variable.OUTDATAs)
                    {
                        _BR_PHR_SEL_Element_VariableOUTDATAs.Add(new BR_PHR_SEL_Element_Variable.OUTDATA
                        {
                            ELMNO = item.ELMNO,
                            VARNO = item.VARNO,
                            VARCATNO = item.VARCATNO,
                            VARNAME = item.VARNAME,
                            ACCTYPENO = item.ACCTYPENO,
                            DATTYPENO = item.DATTYPENO,
                            ENID = item.ENID,
                            STRUCTID = item.STRUCTID,
                            VARCNNINFO = item.VARCNNINFO,
                            VARLENGTH = item.VARLENGTH,
                            VARMAX = item.VARMAX,
                            VARMIN = item.VARMIN,
                            VARVALUE = item.VARVALUE,
                            DRVELMNO = item.DRVELMNO,
                            DRVNO = item.DRVNO,
                            MONITORING = item.MONITORING,
                            TRACE = item.TRACE,
                            TAG = item.TAG,
                            VARDESC = item.VARDESC,
                            ADDITIONAL = item.ADDITIONAL,
                            LINKOPTION = item.LINKOPTION,
                            LINKINFO = item.LINKINFO
                        });
                    }
                }
            }

            return true;
        }

        private void SetMinMax(List<InstructionModel> inputValues)
        {
            inputValues.ForEach(o =>
            {
                decimal minVal, maxVal, precision;
                string tagName = string.Empty; 
                string tagid = o.Raw.TAGID.Replace(o.Raw.EQPTID, txtEQPTID);

                var tagInfo = FilteredComponents.Where(o2 => o2.TAGID == tagid).FirstOrDefault();
                if (tagInfo != null) tagName = tagInfo.TAGNAME;

                if (decimal.TryParse(o.Raw.PRECISION.ToString(), out precision))
                {
                    if (decimal.TryParse(o.Raw.MINVAL, out minVal))
                    {
                        minVal = minVal - (minVal * precision / 100);
                        _mainWnd.AddMinMarker(tagName, (double)minVal);
                    }

                    if (decimal.TryParse(o.Raw.MAXVAL, out maxVal))
                    {
                        maxVal = maxVal + (maxVal * precision / 100);
                        _mainWnd.AddMaxMarker(tagName, (double)maxVal);
                    }
                }
                else
                {
                    if (decimal.TryParse(o.Raw.MINVAL, out minVal))
                    {
                        _mainWnd.AddMinMarker(tagName, (double)minVal);
                    }

                    if (decimal.TryParse(o.Raw.MAXVAL, out maxVal))
                    {
                        _mainWnd.AddMaxMarker(tagName, (double)maxVal);
                    }
                }
            });
        }

        private void SetMinMax(List<InstructionModel> inputValues, string TargetTagID)
        {
            foreach(var invalue in inputValues)
            {
                decimal minVal, maxVal, precision;
                string tagName = string.Empty;
                string tagid = invalue.Raw.TAGID.Replace(invalue.Raw.EQPTID, txtEQPTID);

                if (tagid == TargetTagID)
                {
                    var tagInfo = FilteredComponents.Where(o2 => o2.TAGID == tagid).FirstOrDefault();
                    if (tagInfo != null) tagName = tagInfo.TAGNAME;

                    if (decimal.TryParse(invalue.Raw.PRECISION.ToString(), out precision))
                    {
                        if (decimal.TryParse(invalue.Raw.MINVAL, out minVal))
                        {
                            minVal = minVal - (minVal * precision / 100);
                            _mainWnd.AddMinMarker(tagName, (double)minVal);
                        }

                        if (decimal.TryParse(invalue.Raw.MAXVAL, out maxVal))
                        {
                            maxVal = maxVal + (maxVal * precision / 100);
                            _mainWnd.AddMaxMarker(tagName, (double)maxVal);
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(invalue.Raw.MINVAL, out minVal))
                        {
                            _mainWnd.AddMinMarker(tagName, (double)minVal);
                        }

                        if (decimal.TryParse(invalue.Raw.MAXVAL, out maxVal))
                        {
                            _mainWnd.AddMaxMarker(tagName, (double)maxVal);
                        }
                    }
                }
            }
        }
        
        private void SetMinMax(ChartPanel chartPanel, List<InstructionModel> inputValues, string TargetTagID)
        {
            foreach (var invalue in inputValues)
            {
                decimal minVal, maxVal, precision;
                string tagName = string.Empty;
                string tagid = invalue.Raw.TAGID.Replace(invalue.Raw.EQPTID, txtEQPTID);

                if (tagid == TargetTagID)
                {
                    var tagInfo = FilteredComponents.Where(o2 => o2.TAGID == tagid).FirstOrDefault();
                    if (tagInfo != null) tagName = tagInfo.TAGNAME;

                    if (decimal.TryParse(invalue.Raw.PRECISION.ToString(), out precision))
                    {
                        if (decimal.TryParse(invalue.Raw.MINVAL, out minVal))
                        {
                            minVal = minVal - (minVal * precision / 100);
                            _mainWnd.AddMinMarker(chartPanel, tagName, (double)minVal);
                        }

                        if (decimal.TryParse(invalue.Raw.MAXVAL, out maxVal))
                        {
                            maxVal = maxVal + (maxVal * precision / 100);
                            _mainWnd.AddMaxMarker(chartPanel, tagName, (double)maxVal);
                        }
                    }
                    else
                    {
                        if (decimal.TryParse(invalue.Raw.MINVAL, out minVal))
                        {
                            _mainWnd.AddMinMarker(chartPanel, tagName, (double)minVal);
                        }

                        if (decimal.TryParse(invalue.Raw.MAXVAL, out maxVal))
                        {
                            _mainWnd.AddMaxMarker(chartPanel, tagName, (double)maxVal);
                        }
                    }
                }
            }
        }

        async Task<DateTime> GetOrderSegmentInfo()
        {
            var bizRule = new BR_PHR_SEL_ProductionOrderDetail_Info();

            bizRule.INDATAs.Add(new BR_PHR_SEL_ProductionOrderDetail_Info.INDATA()
            {
                POID = _mainWnd.CurrentOrder.OrderID,
                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
            });

            if (await bizRule.Execute())
            {
                return bizRule.OUTDATAs[0].ACTSTDTTM.Value;
            }

            return (await AuthRepositoryViewModel.GetDBDateTimeNow()).AddMinutes(-5);
        }

        async Task GetValues(DateTime fromDT, DateTime toDT, string EQPTID)
        {
            ProtoSeries = string.Empty;

            _tagIDs = _inputValues.Select(o2 =>
            {
                ProtoSeries += string.Format("[{0}] ", o2.Raw.TAGID);

                return new
                {
                    EQPTID = EQPTID,
                    TAGID = o2.Raw.TAGID.Replace(o2.Raw.EQPTID, EQPTID),
                    UNIT = o2.Raw.UOMNOTATION,
                    TARGETVALUE = o2.Raw.TARGETVAL,
                    MINVALUE = o2.Raw.MINVAL,
                    MAXVALUE = o2.Raw.MAXVAL,
                    DATATYPE = o2.Raw.DATATYPE,
                    PRECISION = o2.Raw.PRECISION,
                    ROUNDTYPE = o2.Raw.ROUNDTYPE
                };
            }).ToList();

            this.outdatas = new List<BR_BRS_SEL_HistoryData.OUTDATA>();
            FilteredComponents.Clear();
            _mainWnd.ChartZoomStoryBoard = null;
            _mainWnd.tabTags.Items.Clear();
            TagImageList.Clear();

            var bizRule = new BR_BRS_SEL_HistoryData();
            foreach (dynamic tagID in _tagIDs)
            {
                bizRule.INDATAs.Add(new BR_BRS_SEL_HistoryData.INDATA()
                {
                    EQPTID = tagID.EQPTID as string,
                    TAGID = tagID.TAGID as string,
                    FROMDT = fromDT,
                    TODT = toDT,
                    TARGETVALUE = tagID.TARGETVALUE as string,
                    MINVALUE = tagID.MINVALUE as string,
                    MAXVALUE = tagID.MAXVALUE as string,
                    DATATYPE = tagID.DATATYPE as string,
                    PRECISION = tagID.PRECISION as int?,
                    ROUNDTYPE = tagID.ROUNDTYPE as string,
                });

                if (await bizRule.Execute() == false) throw new Exception(string.Format("Eqpt Id = {0}, Tag Id = {1}", tagID.EQPTID, tagID.TAGID));

                bizRule.OUTDATAs.ToList().ForEach(o =>
                {
                    var matched = _BR_PHR_SEL_Element_VariableOUTDATAs.Where(o2 => o2.VARNAME == o.TAGID).FirstOrDefault();

                    if (matched != null) o.TAGNAME = matched.VARDESC;
                });

                outdatas.AddRange(bizRule.OUTDATAs.ToList());

                bizRule.OUTDATA_SUMMARYs.ToList().ForEach(o =>
                {
                    var matched = _BR_PHR_SEL_Element_VariableOUTDATAs.Where(o2 => o2.VARNAME == o.TAGID).FirstOrDefault();

                    if (matched != null) o.TAGNAME = matched.VARDESC;

                    foreach (var input in _inputValues)
                    {
                        if (input.Raw.TAGID.Replace(input.Raw.EQPTID, _txtEQPTID) == o.TAGID)
                        {
                            if (input.Raw.MINVAL != null && input.Raw.MINVAL != "" && Convert.ToDecimal(o.MIN) < Convert.ToDecimal(input.Raw.MINVAL))
                            {
                                o.MINCOL = "#FFFF0000";
                                isDeviation = true;
                            }
                            else
                            {
                                o.MINCOL = "#FF000000";
                            }

                            if (input.Raw.MAXVAL != null && input.Raw.MAXVAL != "" && Convert.ToDecimal(o.MAX) > Convert.ToDecimal(input.Raw.MAXVAL))
                            {
                                o.MAXCOL = "#FFFF0000";
                                isDeviation = true;
                            }
                            else
                            {
                                o.MAXCOL = "#FF000000";
                            }

                            o.TARGET = String.Format("{0} ~ {1}", input.Raw.MINVAL, input.Raw.MAXVAL);
                        }
                    }

                    // Add Tag Info                    
                    FilteredComponents.Add(o);
                });
            }

            //GetChartZoomControls();

            double fromTime = outdatas.Min(o => o.INSDTTM).Value.ToOADate();
            double toTime = outdatas.Max(o => o.INSDTTM).Value.ToOADate();

            SelectedFromDt = DateTime.FromOADate(fromTime).ToString("yyyy-MM-dd HH:mm:ss");
            SelectedToDt = DateTime.FromOADate(toTime).ToString("yyyy-MM-dd HH:mm:ss");

            // Set Main Chart
            SetChartMain(fromTime, toTime); 

            // Set Tag charts
            SetChartTag(fromTime, toTime);

            _mainWnd.SelectedFromTime = fromTime;
            _mainWnd.SelectedToTime = toTime;           

            IsProtoSeriesVisible = Visibility.Collapsed;
        }

        /// <summary>
        /// 메인 챠트 설정
        /// </summary>
        private void SetChartMain(double fromDt, double toDt)
        {
            try
            {
                var chartMain = _mainWnd.ChartMain;
                var legend = _mainWnd.Legend;

                chartMain.Data.Children.Clear();
                isDeviation = false;
                chartMain.View.Children.Clear();

                double checker;
                double Ymax;
                double Ymin;
                double? actualYmax = null;
                double? actualYmin = null;
                double? defUCL = null;
                double? defLCL = null;
                double? Ytolerance = null;
                List<double?> lstBase = new List<double?>();

                var groups = outdatas.GroupBy(o => o.TAGID).ToList();

                actualYmax = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker)).Max(o => Convert.ToDouble(o.ACTVAL));
                actualYmin = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker)).Min(o => Convert.ToDouble(o.ACTVAL));
                foreach (var ucl in _inputValues.Where(o => (double.TryParse(o.Raw.MAXVAL, out checker)) == true))
                {
                    lstBase.Add(Convert.ToDouble(ucl.Raw.MAXVAL));
                }
                foreach (var lcl in _inputValues.Where(o => (double.TryParse(o.Raw.MINVAL, out checker)) == true))
                {
                    lstBase.Add(Convert.ToDouble(lcl.Raw.MINVAL));
                }

                if (actualYmax != null) lstBase.Add(actualYmax);
                if (actualYmin != null) lstBase.Add(actualYmin);

                _ChartYMax = Ymax = Convert.ToDouble(lstBase.Max());
                _ChartYMin = Ymin = Convert.ToDouble(lstBase.Min());

                Ytolerance = (Ymax - Ymin) * Convert.ToDouble(AuthRepositoryViewModel.GetSystemOptionValue("SYS_EQPTCHART_SCOPE")); // 시스템옵션사용
                Ytolerance = Ytolerance == 0 ? 1 : Ytolerance;

                chartMain.BeginUpdate();

                legend.Chart = _mainWnd.ChartMain;
                CreateDataSeries(chartMain, groups);
                chartMain.ChartType = ChartType.Step;
                chartMain.View.AxisX.AnnoFormat = _AnnoFormat;
                chartMain.View.AxisX.FontSize = _AnnoFontsize;
                chartMain.View.AxisX.AnnoAngle = _AnnoAngle;
                chartMain.View.AxisX.Min =  fromDt;
                chartMain.View.AxisX.Max = toDt;
                chartMain.View.AxisY.Max = Math.Round(Ymax + Convert.ToDouble(Ytolerance));
                chartMain.View.AxisY.Min = Math.Round(Ymin - Convert.ToDouble(Ytolerance));

                chartMain.EndUpdate();

                _mainWnd.AddMouseMarker();

                initChartSlider();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void SetChartTag(double fromDt, double toDt)
        {
            try
            {
                double checker;
                double Ymax;
                double Ymin;
                double? actualYmax = null;
                double? actualYmin = null;
                double? defUCL = null;
                double? defLCL = null;
                double? Ytolerance = null;
                int index = 0;

                _mainWnd.TagCharts.Clear();

                var groups = outdatas.GroupBy(o => o.TAGID).ToList();
                foreach (var tag in groups)
                {
                    List<double?> lstBase = new List<double?>();

                    string chartName = String.Format("ChartZoom{0}", index);
                    C1Chart chartZoom = _mainWnd.AddTabItemChart(tag.Key, String.Format("ChartZoom{0}", index));

                    if (chartZoom != null)
                    {
                        _mainWnd.TagCharts.Add(tag.Key, chartZoom);    
                        
                        actualYmax = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker) && o.TAGID == tag.Key).Max(o => Convert.ToDouble(o.ACTVAL));
                        actualYmin = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker) && o.TAGID == tag.Key).Min(o => Convert.ToDouble(o.ACTVAL));
                       
                        foreach (var ucl in _inputValues.Where(o => (double.TryParse(o.Raw.MAXVAL, out checker)== true && o.Raw.TAGID == tag.Key)))
                        {
                            lstBase.Add(Convert.ToDouble(ucl.Raw.MAXVAL));
                        }
                        foreach (var lcl in _inputValues.Where(o => (double.TryParse(o.Raw.MINVAL, out checker) && o.Raw.TAGID == tag.Key)))
                        {
                            lstBase.Add(Convert.ToDouble(lcl.Raw.MINVAL));
                        }

                        if (actualYmax != null) lstBase.Add(actualYmax);
                        if (actualYmin != null) lstBase.Add(actualYmin);
 
                        _ChartYMax = Ymax = Convert.ToDouble(lstBase.Max());
                        _ChartYMin = Ymin = Convert.ToDouble(lstBase.Min());

                        Ytolerance = (Ymax - Ymin) * Convert.ToDouble(AuthRepositoryViewModel.GetSystemOptionValue("SYS_EQPTCHART_SCOPE")); // 시스템옵션사용
                        Ytolerance = Ytolerance == 0 ? 1 : Ytolerance;

                        chartZoom.BeginUpdate();

                        CreateDataSeries(chartZoom, tag.Key, index, groups);
                        chartZoom.ChartType = ChartType.Step;
                        chartZoom.View.AxisX.AnnoFormat = _AnnoFormat;
                        chartZoom.View.AxisX.FontSize = _AnnoFontsize;
                        chartZoom.View.AxisX.AnnoAngle = _AnnoAngle;
                        chartZoom.View.AxisX.Min = fromDt;
                        chartZoom.View.AxisX.Max = toDt;   
                        chartZoom.View.AxisY.Max = Math.Round(Ymax + Convert.ToDouble(Ytolerance));
                        chartZoom.View.AxisY.Min = Math.Round(Ymin - Convert.ToDouble(Ytolerance));
                        chartZoom.EndUpdate();

                        SetMinMax(chartZoom.View.Layers[0] as ChartPanel, _inputValues, tag.Key);

                        _mainWnd.AddMouseMarker(chartZoom.View.Layers[0] as ChartPanel);
                    }
                    ++index;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        private void initChartSlider()
        {
            Thickness mars = _mainWnd.ChartMainSilder.Margin;
            double w = _mainWnd.ChartMainSilder.ThumbWidth;

            mars.Left = _mainWnd.ChartMain.View.PlotRect.Left - w;
            _mainWnd.ChartMainSilder.Margin = mars;

            _mainWnd.ChartMainSilder.LowerValue = 0;
            _mainWnd.ChartMainSilder.UpperValue = 1;
        }

        private void refreshSummaryList()
        {
            try
            {
                DateTime stdDTTM;
                DateTime endDTTM;

                if (((outdatas == null) || outdatas.Count == 0) || FilteredComponents.Count == 0)
                    return;

                if (!DateTime.TryParse(SelectedFromDt, out stdDTTM)) return;
                if (!DateTime.TryParse(SelectedToDt, out endDTTM)) return;

                isDeviation = false;

                FilteredComponents.ToList().ForEach(o =>
                {
                    var matched = outdatas.Where(o2 => o2.TAGID == o.TAGID && (o2.INSDTTM >= stdDTTM && o2.INSDTTM <= endDTTM)).ToList();

                    if (matched.Count == 0)
                    {
                        o.MIN = "N/A";
                        o.MAX = "N/A";
                        o.AVG = "N/A";

                        o.MINCOL = "#FFFF0000";
                        o.MAXCOL = "#FFFF0000";
                        o.STATUS = "NG";
                        isDeviation = true;
                    }
                    else
                    {
                        decimal checker;
                        Decimal minVal = 0m;
                        Decimal maxVal = 0m;
                        Decimal avgVal = 0m;

                        var nonEmptyData = matched.Where(o3 => decimal.TryParse(o3.ACTVAL, out checker));

                        if (nonEmptyData != null && nonEmptyData.Count() > 0)
                        {
                            minVal = nonEmptyData.Min(o3 => Convert.ToDecimal(o3.ACTVAL));
                            maxVal = nonEmptyData.Max(o3 => Convert.ToDecimal(o3.ACTVAL));
                            avgVal = nonEmptyData.Average(o3 => Convert.ToDecimal(o3.ACTVAL));
                        }

                        foreach (var input in _inputValues)
                        {
                            if (input.Raw.TAGID.Replace(input.Raw.EQPTID, _txtEQPTID) == o.TAGID)
                            {
                                if (input.Raw.MINVAL != null && input.Raw.MINVAL != "" && minVal < Convert.ToDecimal(input.Raw.MINVAL))
                                {
                                    o.MINCOL = "#FFFF0000";
                                    isDeviation = true;
                                }
                                else
                                {
                                    o.MINCOL = "#FF000000";
                                }

                                if (input.Raw.MAXVAL != null && input.Raw.MAXVAL != "" && maxVal > Convert.ToDecimal(input.Raw.MAXVAL))
                                {
                                    o.MAXCOL = "#FFFF0000";
                                    isDeviation = true;
                                }
                                else
                                {
                                    o.MAXCOL = "#FF000000";
                                }

                                o.STATUS = (o.MINCOL == "#FFFF0000" || o.MAXCOL == "#FFFF0000") ? "NG" : "OK";
                            }
                        }

                        o.MIN = minVal.ToString();
                        o.MAX = maxVal.ToString();
                        o.AVG = avgVal.ToString();
                        o.DATACOUNT = matched.Count.ToString();
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void CreateDataSeries(C1Chart chart, List<IGrouping<string, BR_BRS_SEL_HistoryData.OUTDATA>> groups)
        {
            chart.Data.Children.Clear();

            int idx = 0;
            double min = double.MinValue;
            double max = double.MaxValue;

            foreach (var series in groups)
            {
                int cnt = series.Count();          

                int i = 0;
                double?[] Value = new double?[cnt];

                foreach (var s in series.ToList().OrderBy(o => o.INSDTTM).Select(o => o.ACTVAL).ToList())
                {
                    if (s != null && s != "")
                        Value[i] = Convert.ToDouble(s);
                    else
                        Value[i] = null;

                    i++;
                }

                var color = _seriesColors[idx++];

                XYDataSeries dataSeries = new XYDataSeries();
                dataSeries.Label = series.First().TAGNAME;
 
                dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Select(o => o.INSDTTM).ToList();
                dataSeries.ValuesSource = Value;

                var tmpMin = dataSeries.ValuesSource.Cast<double?>().Min();
                var tmpMax = dataSeries.ValuesSource.Cast<double?>().Max();

                if (min == double.MinValue || min > Convert.ToDouble(tmpMin)) min = Convert.ToDouble(tmpMin);
                if (max == double.MaxValue || max < Convert.ToDouble(tmpMax)) max = Convert.ToDouble(tmpMax);

                dataSeries.ConnectionStroke = new SolidColorBrush(color);
                dataSeries.Display = SeriesDisplay.ShowNaNGap;
                chart.Data.Children.Add(dataSeries);

                _mainWnd.AddLegendMarker(Convert.ToDouble(tmpMax), series.First().TAGNAME, color);
            }

            foreach (DataSeries ser in chart.Data.Children)
                ser.ConnectionStrokeThickness = 1;

            chart.View.AxisX.IsTime = true;
            chart.View.AxisX.AnnoPosition = AnnoPosition.Near;
        }

        void CreateDataSeries(C1Chart chart, string tagID, int index, List<IGrouping<string, BR_BRS_SEL_HistoryData.OUTDATA>> groups)
        {
            chart.Data.Children.Clear();

            double min = double.MinValue;
            double max = double.MaxValue;

            foreach (var series in groups.Where(o=>o.Key == tagID).ToList())
            {
                int cnt = series.Count();

                int i = 0;
                double?[] Value = new double?[cnt];

                foreach (var s in series.ToList().OrderBy(o => o.INSDTTM).Select(o => o.ACTVAL).ToList())
                {
                    if (s != null && s != "")
                        Value[i] = Convert.ToDouble(s);
                    else
                        Value[i] = null;

                    i++;
                }

                var color = _seriesColors[index];

                XYDataSeries dataSeries = new XYDataSeries();
                dataSeries.Label = series.First().TAGNAME;

                dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Select(o => o.INSDTTM).ToList();
                dataSeries.ValuesSource = Value;

                var tmpMin = dataSeries.ValuesSource.Cast<double?>().Min();
                var tmpMax = dataSeries.ValuesSource.Cast<double?>().Max();

                if (min == double.MinValue || min > Convert.ToDouble(tmpMin)) min = Convert.ToDouble(tmpMin);
                if (max == double.MaxValue || max < Convert.ToDouble(tmpMax)) max = Convert.ToDouble(tmpMax);

                dataSeries.ConnectionStroke = new SolidColorBrush(color);
                dataSeries.Display = SeriesDisplay.ShowNaNGap;
                chart.Data.Children.Add(dataSeries);

                _mainWnd.AddLegendMarker(chart.View.Layers[0] as ChartPanel, Convert.ToDouble(tmpMax), series.First().TAGNAME, color);
            }

            foreach (DataSeries ser in chart.Data.Children)
                ser.ConnectionStrokeThickness = 1;

            chart.View.AxisX.IsTime = true;
            chart.View.AxisX.AnnoPosition = AnnoPosition.Near;
        }

        public async Task<byte[]> imageToByteArray()
        {
            try
            {

                //C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
                C1Bitmap bitmap = new C1Bitmap();
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);

                Exception exception = await ShowWindowAsync(bitmap);
                if (exception != null)
                {
                    throw exception;
                }

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);

                stream.Read(datas, 0, datas.Length);

                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<byte[]> imageToByteArray(Border printArea)
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(printArea, null));
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);

                //Exception exception = await ShowWindowAsync(bitmap);
                //if (exception != null)
                //{
                //    throw exception;
                //}

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);

                stream.Read(datas, 0, datas.Length);

                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<Exception> ShowWindowAsync(C1Bitmap Img)
        {
            var completion = new TaskCompletionSource<Exception>();

            장비이력데이터저장Popup popup = new 장비이력데이터저장Popup(Img);

            popup.Closed += (s, e) =>
            {
                try
                {
                    if (popup.DialogResult == true)
                    {
                        completion.TrySetResult(null);
                    }
                    else
                    {
                        completion.TrySetResult(new Exception("취소되었습니다."));
                    }
                }
                catch (Exception ex)
                {
                    completion.TrySetResult(ex);
                }
            };

            popup.Show();

            var result = await completion.Task;
            return result;
        }

        List<string> cname = new List<string>();
        private void GetChartZoomControls()
        {
            try
            {
                int tabItemIdx = 0;

                _mainWnd.TagCharts.Clear();

                foreach (var itm in _mainWnd.tabTags.Items)
                {
                    _mainWnd.tabTags.SelectedIndex = tabItemIdx++;

                    _mainWnd.tabTags.UpdateLayout();

                    FindChartZoomControls(_mainWnd.tabTags, "ChartZoom");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        private void FindChartZoomControls(UIElement parent, string controlName)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                    string childTypeName = Convert.ToString(child.GetType());
                    string childName = Convert.ToString(child.GetType().GetProperty("Name").GetValue(child, null));
                    string childTag = Convert.ToString(child.GetType().GetProperty("Tag").GetValue(child, null));

                    cname.Add(string.Format("{0}, {1}, {2}", childTypeName, childName, childTag));
                    if (controlName == childName && child is C1Chart)
                    {
                        _mainWnd.TagCharts.Add(childTag, child as C1Chart);
                    }

                    FindChartZoomControls(child, controlName);
                }
            }
        }

        public 장비이력데이터멀티ViewModel()
        {
            _BR_PHR_SEL_Element_ELMNAME = new BR_PHR_SEL_Element_ELMNAME();
            _BR_PHR_SEL_Element_Variable = new BR_PHR_SEL_Element_Variable();
            _BR_PHR_SEL_Element_VariableOUTDATAs = new ObservableCollection<BR_PHR_SEL_Element_Variable.OUTDATA>();

            _filteredComponents = new BR_BRS_SEL_HistoryData.OUTDATA_SUMMARYCollection();            
        }
    }
}