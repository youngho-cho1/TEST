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

namespace 보령
{
    public class 평균질량측정ViewModel : ViewModelBase
    {
        #region Property
        public 평균질량측정ViewModel()
        {
            _ScaleInfo = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _BR_BRS_REG_IPC_AVG_WEIGHT_MULTI = new BR_BRS_REG_IPC_AVG_WEIGHT_MULTI();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            _filteredComponents = new ObservableCollection<AVG_INDATA>();

            isSaveEnable = false;
            isBtnEnable = false;
            sampleCount = 1;

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;
        }

        private 평균질량측정 _mainWnd;
        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 2000;
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();

        #region 저울
        private string _eqptID;
        public string eqptID
        {
            get { return _eqptID; }
            set
            {
                _eqptID = value;
                OnPropertyChanged("eqptID");
            }
        }
        private int _ScalePrecision;
        public int ScalePrecision
        {
            get { return _ScalePrecision; }
            set
            {
                _ScalePrecision = value;
                OnPropertyChanged("ScalePrecision");
                OnPropertyChanged("curWeight");
            }
        }
        private string _ScaleUom;
        public string ScaleUom
        {
            get { return _ScaleUom; }
            set
            {
                _ScaleUom = value;
                OnPropertyChanged("ScaleUom");
                OnPropertyChanged("curWeight");
            }
        }
        private Weight _curWeight = new Weight();
        public string curWeight
        {
            get { return _curWeight.WeightUOMString; }
        }
        #endregion

        #region AVG_IPC
        private decimal _sampleCount;
        public decimal sampleCount
        {
            get { return _sampleCount; }
            set
            {
                if(value > 0)
                    _sampleCount = value;
                else
                {
                    _sampleCount = 1;
                    OnMessage("샘플수량이 0보다 작을 수 없습니다.");
                }
                
                calAvgWeight();
                OnPropertyChanged("sampleCount");
            }
        }
        private string _avgWeighing;
        public string avgWeighing
        {
            get { return _avgWeighing; }
            set
            {
                _avgWeighing = value;
                OnPropertyChanged("avgWeighing");
            }
        }

        private ObservableCollection<AVG_INDATA> _filteredComponents;
        public ObservableCollection<AVG_INDATA> filteredComponents
        {
            get { return _filteredComponents; }
            set
            {
                _filteredComponents = value;
                OnPropertyChanged("filteredComponents");
            }
        }
        #endregion

        #region Control
        private bool _isBtnEnable;
        public bool isBtnEnable
        {
            get { return _isBtnEnable; }
            set
            {
                _isBtnEnable = value;
                OnPropertyChanged("isBtnEnable");
            }
        }

        private bool _isSaveEnable;
        public bool isSaveEnable
        {
            get { return _isSaveEnable; }
            set
            {
                _isSaveEnable = value;
                OnPropertyChanged("isSaveEnable");
            }
        }
        #endregion

        // Not Use Scale Property
        private string _popupWeight;
        public string popupWeight
        {
            get { return _popupWeight; }
            set
            {
                _popupWeight = value;
                OnPropertyChanged("popupWeight");
            }
        }
        private string _popupUOM;
        public string popupUOM
        {
            get { return _popupUOM; }
            set
            {
                _popupUOM = value;
                OnPropertyChanged("popupUOM");
            }
        }
        #endregion

        #region BizRule
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _ScaleInfo;
        private BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;
        private BR_BRS_REG_IPC_AVG_WEIGHT_MULTI _BR_BRS_REG_IPC_AVG_WEIGHT_MULTI;
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

                            if (arg != null && arg is 평균질량측정)
                            {
                                _mainWnd = arg as 평균질량측정;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_repeater != null)
                                        _repeater.Stop();

                                    _repeater = null;
                                };

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

                            if(!string.IsNullOrWhiteSpace(arg.ToString()))
                            {
                                string eqpt = arg.ToString();

                                // 저울정보 확인
                                _ScaleInfo.INDATAs.Clear();
                                _ScaleInfo.OUTDATAs.Clear();

                                _ScaleInfo.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA()
                                {
                                    EQPTID = eqpt
                                });

                                if (await _ScaleInfo.Execute() && _ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    ScaleUom = _ScaleInfo.OUTDATAs[0].NOTATION;
                                    ScalePrecision = Convert.ToInt32(_ScaleInfo.OUTDATAs[0].PRECISION.GetValueOrDefault());
                                    _curWeight.SetWeight(0, ScaleUom, ScalePrecision);

                                    avgWeighing = "";
                                    eqptID = _ScaleInfo.OUTDATAs[0].EQPTID;
                                    _repeater.Start();
                                }
                                else
                                {
                                    avgWeighing = "";
                                    eqptID = "";
                                    _mainWnd.txtEQPTID.Focus();
                                    _repeater.Stop();
                                }
                            }
                           
                            ///

                            CommandResults["BarcodeChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
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
                            if (!string.IsNullOrWhiteSpace(_eqptID) && _ScaleInfo.OUTDATAs[0] != null)
                            {
                                _repeater.Stop();
                                // 저울에 Tare 명령어 전달
                                bool success = false;
                                if (_ScaleInfo.OUTDATAs[0].INTERFACE.ToUpper() == "REST")
                                {
                                    var result = await _restScaleService.DownloadString(_eqptID, ScaleCommand.ST);

                                    success = result.code == "1" ? true : false;
                                }
                                else
                                {
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                                    _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA
                                    {
                                        ScaleID = _eqptID
                                    });

                                    if (await _BR_PHR_REG_ScaleSetTare.Execute())
                                        success = true;
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

                            DateTime record = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            filteredComponents.Add(new AVG_INDATA()
                            {
                                CHK = "N",
                                RECORDDTTM = record,
                                INX = filteredComponents.Count > 0 ? filteredComponents.Count + 1 : 1,
                                SCALEID = eqptID,
                                SMPQTY = sampleCount.ToString("0"),
                                CUR_WEIGHT = _curWeight.WeightUOMString,
                                AVG_WEIGHT = avgWeighing
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

                            if (filteredComponents.Count > 0)
                                isSaveEnable = true;
                            else isSaveEnable = false;

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

                            var elements = (from data in filteredComponents
                                            where data.CHK == "N"
                                            select data).ToList();

                            filteredComponents.Clear();

                            int inx = 1;
                            foreach (var data in elements)
                            {
                                data.INX = inx++;
                                filteredComponents.Add(data);
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

                            if (filteredComponents.Count == 0)
                                isSaveEnable = false;

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
                            if (filteredComponents.Count > 0)
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
                                    "평균질량측정",
                                    "평균질량측정",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                _BR_BRS_REG_IPC_AVG_WEIGHT_MULTI.INDATAs.Clear();

                                //XML 형식으로 저장
                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("순번"));
                                dt.Columns.Add(new DataColumn("장비번호"));
                                dt.Columns.Add(new DataColumn("수량"));
                                dt.Columns.Add(new DataColumn("현재무게"));
                                dt.Columns.Add(new DataColumn("평균무게"));

                                foreach (var rowdata in filteredComponents)
                                {
                                    var row = dt.NewRow();
                                    row["순번"] = rowdata.INX.ToString();
                                    row["장비번호"] = rowdata.SCALEID != null ? rowdata.SCALEID : "";
                                    row["수량"] = rowdata.SMPQTY != null ? rowdata.SMPQTY : "";
                                    row["현재무게"] = rowdata.CUR_WEIGHT != null ? rowdata.CUR_WEIGHT : "";
                                    row["평균무게"] = rowdata.AVG_WEIGHT != null ? rowdata.AVG_WEIGHT : "";
                                    dt.Rows.Add(row);

                                    _BR_BRS_REG_IPC_AVG_WEIGHT_MULTI.INDATAs.Add(new BR_BRS_REG_IPC_AVG_WEIGHT_MULTI.INDATA()
                                    {
                                        SCALEID = rowdata.SCALEID,
                                        POID = ProductionOrderInfo.OrderID,
                                        OPSGGUID = ProductionOrderInfo.OrderProcessSegmentID,
                                        SMPQTY = short.Parse(rowdata.SMPQTY.Replace(" T", "")),
                                        AVG_WEIGHT = (rowdata.AVG_WEIGHT.Substring(0, rowdata.AVG_WEIGHT.Length - 2)).Replace(",", ""),
                                        SMPQTYUOMID = "",
                                        USERID = ProductionOrderInfo.UserID,
                                        LOCATIONID = "",
                                        STRTDTTM = rowdata.RECORDDTTM
                                    });
                                }

                                if(await _BR_BRS_REG_IPC_AVG_WEIGHT_MULTI.Execute() == true)
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
                                "평균질량측정",
                                "평균질량측정",
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
                            dt.Columns.Add(new DataColumn("샘플수량"));
                            dt.Columns.Add(new DataColumn("현재무게"));
                            dt.Columns.Add(new DataColumn("평균무게"));

                            var row = dt.NewRow();
                            row["순번"] = "N/A";
                            row["장비번호"] = "N/A";
                            row["샘플수량"] = "N/A";
                            row["현재무게"] = "N/A";
                            row["평균무게"] = "N/A";
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

                            ///

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

        public ICommand UnuseScaleCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["UnuseScaleCommand"].EnterAsync())
                    {
                        try
                        {
                            ///
                            _repeater.Stop();

                            var popup = new InputWeightpopup();
                            popup.Closed += (s, e) =>
                            {
                                if(popup.DialogResult.GetValueOrDefault())
                                {
                                    eqptID = "";

                                    _curWeight.SetWeight(Convert.ToDecimal(popup.txtWeight.Value), "g", 3);
                                    calAvgWeight();
                                    isBtnEnable = true;
                                    OnPropertyChanged("curWeight");
                                }
                            };

                            popup.Show();
                            ///
                            CommandResults["UnuseScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["UnuseScaleCommand"] = false;    
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["UnuseScaleCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("UnuseScaleCommand") ?
                        CommandCanExecutes["UnuseScaleCommand"] : (CommandCanExecutes["UnuseScaleCommand"] = true);
                });
            }
        }
        #endregion
        async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                if (!string.IsNullOrWhiteSpace(_eqptID) && _ScaleInfo.OUTDATAs[0] != null)
                {
                    bool success = false;
                    string curWeight = string.Empty;
                    if (_ScaleInfo.OUTDATAs[0].INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_eqptID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            curWeight = result.data;
                            ScaleUom = result.unit;

                            // 저울 유효숫자 설정
                            if (curWeight.Contains("."))
                            {
                                var splt = curWeight.Split('.');
                                if (splt.Length > 1)
                                    ScalePrecision = splt[1].Length;
                                else
                                    ScalePrecision = 0;
                            }
                            else
                                ScalePrecision = 0;
                        }
                    }
                    else
                    {
                        _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                        _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                        _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                        {
                            ScaleID = eqptID
                        });

                        if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                        {
                            success = true;
                            curWeight = string.Format(("{0:N" + _curWeight.Precision + "}"), _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight);
                            ScaleUom = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                        }
                    }

                    if (success)
                    {
                        _curWeight.SetWeight(curWeight, ScaleUom);
                        isBtnEnable = true;
                    }
                    else
                    {
                        _curWeight.Value = 0;
                        isBtnEnable = false;
                    }
                        
                    calAvgWeight();
                    OnPropertyChanged("curWeight");

                    _repeater.Start();
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
                _repeater.Start();
            }
        }
        /// <summary>
        /// 평균값 계산
        /// </summary>
        public void calAvgWeight()
        {
            if(sampleCount > 0 && _curWeight.Value > 0)
                avgWeighing = string.Format("{0:#,0} mg", (Weight.Add(0, "mg", _curWeight.Value, _curWeight.Uom) / Convert.ToDecimal(_sampleCount)));  
        }

        public partial class AVG_INDATA : BizActorDataSetBase
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

            private string _SMPQTY;
            public string SMPQTY
            {
                get { return _SMPQTY; }
                set
                {
                    _SMPQTY = value;
                    OnPropertyChanged("SMPQTY");
                }
            }

            private string _CUR_WEIGHT;
            public string CUR_WEIGHT
            {
                get { return _CUR_WEIGHT; }
                set
                {
                    _CUR_WEIGHT = value;
                    OnPropertyChanged("CUR_WEIGHT");
                }
            }

            private string _AVG_WEIGHT;
            public string AVG_WEIGHT
            {
                get { return _AVG_WEIGHT; }
                set
                {
                    _AVG_WEIGHT = value;
                    OnPropertyChanged("AVG_WEIGHT");
                }
            }
        }
    }
}
