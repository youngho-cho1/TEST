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
using System.Collections.Generic;
using C1.Silverlight.Chart;
using System.IO;
using C1.Silverlight.Chart.Extended;
using LGCNS.iPharmMES.Recipe.Common;
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;


namespace 보령
{
    public class 장비이력데이터ViewModel : ViewModelBase
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

        bool isDeviation = false;

        장비이력데이터 _mainWnd;

        IEnumerable<dynamic> _tagIDs;
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
                            _mainWnd = arg as 장비이력데이터;

                            FromDt = (await AuthRepositoryViewModel.GetDBDateTimeNow()).AddHours(-1);
                            ToDt = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            _inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            if (_inputValues.Count <= 0)
                            {
                                OnMessage("조회할 태그 정보가 1개 이상이어야 합니다");
                                return;
                            }

                            // 2021.08.17 박희돈 주석처리 - 폼 로드시 설비 값이 없어야 한다고 정남호D 요청. 
                            //txtEQPTID = (_inputValues[0].Raw.ACTVAL != null && _inputValues[0].Raw.ACTVAL.Length > 0) ? _inputValues[0].Raw.ACTVAL : _inputValues[0].Raw.EQPTID;

                            //await checkEqptInfo(txtEQPTID);
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

                            var chartMain = _mainWnd.ChartMain;
                            var chartZoom = _mainWnd.ChartZoom;
                            var legend = _mainWnd.Legend;

                            FilteredComponents.Clear();
                            chartMain.Data.Children.Clear();
                            chartZoom.Data.Children.Clear();

                            isDeviation = false;
                            //if(legend.Chart != null)
                            //legend.Chart.Children.Clear();
                            chartMain.View.Children.Clear();
                            chartZoom.View.Children.Clear();

                            ErrorChk = 1;
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

                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);


                            _mainWnd.CurrentInstruction.Raw.NOTE = await imageToByteArray();

                            var dateTimeInstructions = _mainWnd.Instructions.Where(o =>
                            {
                                return string.Compare(o.Raw.REF_IRTGUID, _mainWnd.CurrentInstruction.Raw.IRTGUID) == 0 &&
                                    ((enumVariableType)Enum.Parse(typeof(enumVariableType), o.Raw.IRTTYPE, false)) == enumVariableType.IT004;
                            }).OrderBy(o => o.Raw.IRTSEQ).ToList();

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "장비이력데이터";

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }


                            if (dateTimeInstructions.Count == 2)
                            {
                                dateTimeInstructions[0].Raw.ACTVAL = DateTime.FromOADate(_mainWnd.SelectedFromTime).ToString();
                                dateTimeInstructions[1].Raw.ACTVAL = DateTime.FromOADate(_mainWnd.SelectedToTime).ToString();

                                result = await _mainWnd.Phase.RegistInstructionValue(dateTimeInstructions[0]);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", dateTimeInstructions[0].Raw.IRTGUID, result));
                                }

                                result = await _mainWnd.Phase.RegistInstructionValue(dateTimeInstructions[1]);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", dateTimeInstructions[1].Raw.IRTGUID, result));
                                }
                            }

                            var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            int idx = 0;
                            foreach (var item in outputValues)
                            {
                                string CMT = string.IsNullOrWhiteSpace(FilteredComponents[idx].COMMENT) ? "" : ", Comment : " + FilteredComponents[idx].COMMENT;

                                item.Raw.ACTVAL = "TAG 명 : " + FilteredComponents[idx].TAGNAME + ", AVG : " + FilteredComponents[idx].AVG +
                                                  ", Min : " + FilteredComponents[idx].MIN + ", Max : " + FilteredComponents[idx].MAX + CMT;

                                result = await _mainWnd.Phase.RegistInstructionValue(item);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", item.Raw.IRTGUID, result));
                                }

                                idx++;
                            }

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

            //if (await _BR_PHR_SEL_Element_ELMNAME.Execute() == false || _BR_PHR_SEL_Element_ELMNAME.OUTDATAs.Count <= 0)
            //{
            //}

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

                if (decimal.TryParse(o.Raw.PRECISION.ToString(), out precision))
                {
                    if (decimal.TryParse(o.Raw.MINVAL, out minVal))
                    {
                        minVal = minVal - (minVal * precision / 100);
                        _mainWnd.AddMinMarker((double)minVal);
                    }

                    if (decimal.TryParse(o.Raw.MAXVAL, out maxVal))
                    {
                        maxVal = maxVal + (maxVal * precision / 100);
                        _mainWnd.AddMaxMarker((double)maxVal);
                    }
                }
                else
                {
                    if (decimal.TryParse(o.Raw.MINVAL, out minVal))
                    {
                        _mainWnd.AddMinMarker((double)minVal);
                    }

                    if (decimal.TryParse(o.Raw.MAXVAL, out maxVal))
                    {
                        _mainWnd.AddMaxMarker((double)maxVal);
                    }
                }
            });
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
            var bizRule = new BR_BRS_SEL_HistoryData();

            FilteredComponents.Clear();

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
                        }
                    }

                    FilteredComponents.Add(o);
                });

            }

            var chartMain = _mainWnd.ChartMain;
            var chartZoom = _mainWnd.ChartZoom;
            var legend = _mainWnd.Legend;

            if (outdatas.Count <= 0)
            {
                chartMain.Data.Children.Clear();
                chartZoom.Data.Children.Clear();
                return;
            }

            var fromDt = outdatas.Min(o => o.INSDTTM).Value.ToOADate();
            var toDt = outdatas.Max(o => o.INSDTTM).Value.ToOADate();

            _mainWnd.SelectedFromTime = fromDt;
            _mainWnd.SelectedToTime = toDt;

            var groups = outdatas.GroupBy(o => o.TAGID).ToList();

            double checker;
            double Ymax = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker)).Max(o => Convert.ToDouble(o.ACTVAL));
            double Ymin = outdatas.Where(o => double.TryParse(o.ACTVAL, out checker)).Min(o => Convert.ToDouble(o.ACTVAL));
            double Ytolerance = (Ymax - Ymin) * Convert.ToDouble(AuthRepositoryViewModel.GetSystemOptionValue("SYS_EQPTCHART_SCOPE")); // 시스템옵션사용
            Ytolerance = Ytolerance == 0 ? 1 : Ytolerance;

            chartMain.BeginUpdate();
            legend.Chart = _mainWnd.ChartMain;
            CreateDataSeries(chartMain, groups);
            chartMain.ChartType = ChartType.Step;
            chartMain.View.AxisX.AnnoFormat = "MM-dd HH:mm";
            chartMain.View.AxisX.FontSize = 8;
            chartMain.View.AxisX.AnnoAngle = 315;
            chartMain.View.AxisX.Min = fromDt;
            chartMain.View.AxisX.Max = toDt;
            chartMain.View.AxisY.Max = Math.Round(Ymax + Ytolerance);
            chartMain.View.AxisY.Min = Math.Round(Ymin - Ytolerance);

            chartMain.EndUpdate();

            chartZoom.BeginUpdate();
            CreateDataSeries(chartZoom, groups);
            chartZoom.ChartType = ChartType.Step;
            chartZoom.View.AxisX.AnnoFormat = "MM-dd HH:mm";
            chartZoom.View.AxisX.FontSize = 8;
            chartZoom.View.AxisX.AnnoAngle = 315;
            chartZoom.View.AxisX.Max = _mainWnd.SelectedFromTime + 0.25 * (toDt - fromDt); // main chart 의 1/4            
            chartZoom.View.AxisY.Max = Math.Round(Ymax + Ytolerance);
            chartZoom.View.AxisY.Min = Math.Round(Ymin - Ytolerance);
            chartZoom.EndUpdate();

            SetMinMax(_inputValues);

            _mainWnd.AddMouseMarker();

            initChartSlider();

            IsProtoSeriesVisible = Visibility.Collapsed;
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
        static Color[] _seriesColors =
        {
            //Colors.Red,
            //Colors.Blue,
            Colors.Green,
            Colors.Orange,
            Colors.Brown,
            Colors.Purple,
            Colors.Black,
            Colors.Cyan,
        };
        void CreateDataSeries(C1Chart chart, List<IGrouping<string, BR_BRS_SEL_HistoryData.OUTDATA>> groups)
        {
            chart.Data.Children.Clear();

            //var rand = new Random();
            int idx = 0;
            double min = double.MinValue;
            double max = double.MaxValue;
            double interval = 0;

            foreach (var series in groups)
            {
                int cnt = series.Count();


                //foreach (int s in series.ToList().OrderBy(o => o.INSDTTM).Select(o => Convert.ToInt32(o.RowIndex)).ToList())
                //{
                //    var s1 = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToInt32(o.RowIndex) == s).Select(o => o.ACTVAL).First();

                //    if (s == 0)
                //    {
                //        start = 0;
                //    }
                //    else if (series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToInt32(o.RowIndex) == s).Select(o => o.ACTVAL).First() != null)
                //    {
                //        if (series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToInt32(o.RowIndex) == s - 1).Select(o => o.ACTVAL).First() == null)
                //        {
                //            if (s == series.Count() - 1)
                //            {
                //                start = s;
                //                end = s;
                //                var color = _seriesColors[idx];

                //                XYDataSeries dataSeries = new XYDataSeries();
                //                dataSeries.Label = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) == start).First().TAGNAME;
                //                //dataSeries.label = (DataTemplate)_mainWnd.Resources["series"];

                //                dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => o.INSDTTM).ToList();
                //                dataSeries.ValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => Convert.ToDouble(o.ACTVAL)).ToList();

                //                var tmpMin = dataSeries.ValuesSource.Cast<double>().Min();
                //                var tmpMax = dataSeries.ValuesSource.Cast<double>().Max();

                //                if (tmpMin < tmpMinT || tmpMinT == 0)
                //                    tmpMinT = tmpMin;

                //                if (tmpMax < tmpMaxT || tmpMaxT == 0)
                //                    tmpMaxT = tmpMax;

                //                dataSeries.ConnectionStroke = new SolidColorBrush(color);
                //                chart.Data.Children.Add(dataSeries);
                //            }
                //            else
                //            {
                //                start = s;
                //            }
                //        }
                //        else if (s == series.Count() - 1)
                //        {
                //            if (start > end)
                //            {
                //                end = s;
                //                var color = _seriesColors[idx];

                //                XYDataSeries dataSeries = new XYDataSeries();
                //                dataSeries.Label = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) == start).First().TAGNAME;
                //                //dataSeries.label = (DataTemplate)_mainWnd.Resources["series"];

                //                dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => o.INSDTTM).ToList();
                //                dataSeries.ValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => Convert.ToDouble(o.ACTVAL)).ToList();

                //                var tmpMin = dataSeries.ValuesSource.Cast<double>().Min();
                //                var tmpMax = dataSeries.ValuesSource.Cast<double>().Max();

                //                if (tmpMin < tmpMinT || tmpMinT == 0)
                //                    tmpMinT = tmpMin;

                //                if (tmpMax < tmpMaxT || tmpMaxT == 0)
                //                    tmpMaxT = tmpMax;

                //                dataSeries.ConnectionStroke = new SolidColorBrush(color);
                //                chart.Data.Children.Add(dataSeries);
                //            }

                //        }

                //    }
                //    else if (series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToInt32(o.RowIndex) == s).Select(o => o.ACTVAL).First() == null)
                //    {
                //        if (series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToInt32(o.RowIndex) == s - 1).Select(o => o.ACTVAL).First() != null)
                //        {
                //            end = s - 1;
                //            var color = _seriesColors[0];

                //            XYDataSeries dataSeries = new XYDataSeries();
                //            dataSeries.Label = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) == start).First().TAGNAME;
                //            //dataSeries.label = (DataTemplate)_mainWnd.Resources["series"];

                //            dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => o.INSDTTM).ToList();
                //            dataSeries.ValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Where(o => Convert.ToDecimal(o.RowIndex) >= start && Convert.ToDecimal(o.RowIndex) <= end).Select(o => Convert.ToDouble(o.ACTVAL)).ToList();

                //            var tmpMin = dataSeries.ValuesSource.Cast<double>().Min();
                //            var tmpMax = dataSeries.ValuesSource.Cast<double>().Max();

                //            if (tmpMin < tmpMinT || tmpMinT == 0)
                //                tmpMinT = tmpMin;

                //            if (tmpMax < tmpMaxT || tmpMaxT == 0)
                //                tmpMaxT = tmpMax;

                //            dataSeries.ConnectionStroke = new SolidColorBrush(color);
                //            chart.Data.Children.Add(dataSeries);

                //        }

                //    }
                //}

                //var color = Color.FromArgb(0, (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
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
                //dataSeries.label = (DataTemplate)_mainWnd.Resources["series"];

                dataSeries.XValuesSource = series.ToList().OrderBy(o => o.INSDTTM).Select(o => o.INSDTTM).ToList();
                dataSeries.ValuesSource = Value;

                var tmpMin = dataSeries.ValuesSource.Cast<double?>().Min();
                var tmpMax = dataSeries.ValuesSource.Cast<double?>().Max();

                if (min == double.MinValue || min > Convert.ToDouble(tmpMin)) min = Convert.ToDouble(tmpMin);
                if (max == double.MaxValue || max < Convert.ToDouble(tmpMax)) max = Convert.ToDouble(tmpMax);

                dataSeries.ConnectionStroke = new SolidColorBrush(color);
                dataSeries.Display = SeriesDisplay.ShowNaNGap;
                chart.Data.Children.Add(dataSeries);



                //idx++;
            }

            interval = (max - min) / idx;
            idx = 0;
            foreach (var series in groups)
            {
                var color = _seriesColors[idx++];
                _mainWnd.AddLegendMarker(min + (interval * idx), series.First().TAGNAME, color);
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

                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
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
        public 장비이력데이터ViewModel()
        {
            _BR_PHR_SEL_Element_ELMNAME = new BR_PHR_SEL_Element_ELMNAME();
            _BR_PHR_SEL_Element_Variable = new BR_PHR_SEL_Element_Variable();
            _BR_PHR_SEL_Element_VariableOUTDATAs = new ObservableCollection<BR_PHR_SEL_Element_Variable.OUTDATA>();

            _filteredComponents = new BR_BRS_SEL_HistoryData.OUTDATA_SUMMARYCollection();
        }
    }
}