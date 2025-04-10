using C1.Silverlight.Data;
using Equipment;
using LGCNS.EZMES.Common;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Threading.Tasks;
using Common = LGCNS.iPharmMES.Common.Common;

namespace 보령
{
    public class 장비청소상태ViewModel : ViewModelBase
    {
        public 장비청소상태ViewModel()
        {
            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi = new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi();
            _filteredComponents = new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATACollection();
            IsEnabled = false;

        }

        장비청소상태 _mainWnd;
        public 장비청소상태 mainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
        }

        bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }


        BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi;
        public BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi
        {
            get { return _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi; }
            set
            {
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATACollection _filteredComponents;
        public BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

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
                            if (arg != null)
                                _mainWnd = arg as 장비청소상태;

                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;
                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);


                            FilteredComponents.Clear();
                            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Clear();
                            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Clear();


                            await SelEquipmentSatatus(instruction, phase, inputValues);
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

        private async Task<bool> SelEquipmentSatatus(InstructionModel instruction, PhaseViewModel phase, List<InstructionModel> inputValues)
        {
            if (inputValues.Count > 0)
            {
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULT()
                {
                    RECIPEISTGUID = instruction.Raw.RECIPEISTGUID,
                    ACTIVITYID = instruction.Raw.ACTIVITYID,
                    IRTGUID = instruction.Raw.IRTGUID,
                    IRTRSTGUID = instruction.Raw.IRTRSTGUID,
                    ACTVAL = instruction.Raw.ACTVAL
                });

                foreach (InstructionModel inst in inputValues)
                {
                    _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA()
                    {
                        LANGID = LogInInfo.LangID,
                        EQPTID = inst.Raw.ACTVAL != null ? inst.Raw.ACTVAL : inst.Raw.EQPTID,
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        BATCHNO = _mainWnd.CurrentOrder.BatchNo
                    });
                }

                int numOfIndata = _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Count + _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Count;

                if (numOfIndata > 0 && await _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.Execute())
                {
                    IsEnabled = true;

                    foreach (var outdata in _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
                    {
                        FilteredComponents.Add(outdata);
                        //기록버튼
                        if (outdata.AVAILFLAG != "Y")
                            IsEnabled = false;
                    }
                }
            }
            else
            {
                if (instruction.Raw.ACTVAL != null)
                {
                    _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULT()
                    {
                        RECIPEISTGUID = instruction.Raw.RECIPEISTGUID,
                        ACTIVITYID = instruction.Raw.ACTIVITYID,
                        IRTGUID = instruction.Raw.IRTGUID,
                        IRTRSTGUID = instruction.Raw.IRTRSTGUID,
                        ACTVAL = instruction.Raw.ACTVAL
                    });

                    if (await _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.Execute())
                    {
                        IsEnabled = true;

                        foreach (var outdata in _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
                        {
                            FilteredComponents.Add(outdata);
                            //기록버튼
                            if (outdata.AVAILFLAG != "Y")
                                IsEnabled = false;
                        }
                    }
                }
            }

            return true;

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
                            //CommandCanExecutes["ConfirmCommand"] = false;

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

                            enumRoleType inspectorRole = enumRoleType.ROLE001;

                            if (_mainWnd.CurrentInstruction.Raw.DVTPASSYN != "Y" &&
                                Enum.TryParse<enumRoleType>(_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                            {
                                if (_BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs.Count > 0)
                                {
                                    int Cnt = 0;
                                    foreach (var item in _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
                                    {
                                        if (item.VALIDFLAG == "N")
                                        {
                                            Cnt++;
                                        }
                                    }
                                    if (Cnt > 0)
                                    {
                                        var authHelper = new iPharmAuthCommandHelper();
                                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");
                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Role,
                                            Common.enumAccessType.Create,
                                            "기록값 일탈에 대해 서명후 기록을 진행합니다.",
                                            "Deviation Sign",
                                            true,
                                            "OM_ProductionOrder_Deviation",
                                            "",
                                            _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                            _mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }
                                        _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                        _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");
                                    }
                                }

                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("관리번호"));
                            dt.Columns.Add(new DataColumn("이름"));
                            dt.Columns.Add(new DataColumn("이전제품명"));
                            dt.Columns.Add(new DataColumn("이전제조번호"));
                            dt.Columns.Add(new DataColumn("청소상태"));
                            dt.Columns.Add(new DataColumn("청소일시"));
                            dt.Columns.Add(new DataColumn("점검일시"));
                            dt.Columns.Add(new DataColumn("유효기간"));

                            foreach (var item in FilteredComponents)
                            {
                                var row = dt.NewRow();
                                row["관리번호"] = item.EQPTID;
                                row["이름"] = item.EQPTNAME;
                                row["이전제품명"] = item.MTRLNAME != null ? item.MTRLNAME : "";
                                row["이전제조번호"] = item.BATCHNO != null ? item.BATCHNO : "";
                                row["청소상태"] = item.STATUS != null ? item.STATUS : "";
                                row["청소일시"] = item.CLEANING_ONTIME != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CLEANING_ONTIME) : "";
                                row["점검일시"] = item.CHECKEDDTTM != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CHECKEDDTTM) : "";
                                row["유효기간"] = item.EXPIREDTTM != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.EXPIREDTTM) : ""; 
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

        public ICommand PerformCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PerformCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PerformCommand"] = false;
                            //CommandCanExecutes["LoadedCommand"] = false;

                            var item = arg as BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATA;
                            var actionPopup = new EM_EquipmentManagement_PerformAction();

                            actionPopup.SelectedEquipmentData = item.EQPTID;
                            actionPopup.DataContext = new EM_EquipmentManagementPerformActionViewModel();
                            actionPopup.Closed += actionPopup_Closed;
                            actionPopup.Show();

                            CommandResults["PerformCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PerformCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PerformCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PerformCommand") ?
                        CommandCanExecutes["PerformCommand"] : (CommandCanExecutes["PerformCommand"] = true);
                });
            }
        }

        public ICommand PerformProduceCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PerformProduceCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PerformProduceCommand"] = false;
                            CommandCanExecutes["PerformProduceCommand"] = false;

                            ///
                            var instruction = _mainWnd.CurrentInstruction;
                            var item = arg as BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATA;
                            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI();


                            // 전자서명 요청
                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_EquipmentManagement_Action");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("[{0}] 생산 시작", item.EQPTID),
                                string.Format("생산 시작"),
                                false,
                                "EM_EquipmentManagement_Action",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            bizRule.INDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI.INDATA()
                            {
                                //EQACNAME = "생산시작",
                                EQPTID = item.EQPTID,
                                USER = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_EquipmentManagement_Action"),
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                DTTM = await AuthRepositoryViewModel.GetDBDateTimeNow()
                            });

                            bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI.PARAMDATA()
                            {
                                EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_ORDER"),
                                PAVAL = _mainWnd.CurrentOrder.OrderID,
                                //PAVAL = this.CurrentOrder.OrderID,
                                EQPTID = item.EQPTID
                                // EQSTID = "PD_EQSTPROC",
                            });
                            bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI.PARAMDATA()
                            {
                                EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_BATCHNO"),
                                PAVAL = _mainWnd.CurrentOrder.BatchNo,
                                //PAVAL = this.CurrentOrder.BatchNo,
                                EQPTID = item.EQPTID
                                // EQSTID = "PD_EQSTPROC",
                            });
                            bizRule.PARAMDATAs.Add(new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI.PARAMDATA()
                            {
                                EQPAID = LGCNS.iPharmMES.Common.AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_PRODUCTION_PROCESS"),
                                PAVAL =_mainWnd.CurrentOrder.OrderProcessSegmentID,
                                //PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                                EQPTID = item.EQPTID
                                //EQSTID = "PD_EQSTPROC",
                            });
                            ///

                            if (await bizRule.Execute() == true)
                            {
                                Task<bool> result = ResetResult();
                            }

                            CommandResults["PerformProduceCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PerformProduceCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PerformProduceCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PerformProduceCommand") ?
                        CommandCanExecutes["PerformProduceCommand"] : (CommandCanExecutes["PerformProduceCommand"] = true);
                });
            }
        }
        
        public ICommand ScanClickCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanClickCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ScanClickCommand"] = false;
                            CommandCanExecutes["ScanClickCommand"] = false;

                            ///
                            var viewModel = new 장비청소팝업ViewModel()
                            {
                                ParentVM = this
                            };

                            var ScanPopup = new 장비청소팝업()
                            {
                                DataContext = viewModel
                            };

                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();
                            ///

                            CommandResults["ScanClickCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanClickCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanClickCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanClickCommand") ?
                        CommandCanExecutes["ScanClickCommand"] : (CommandCanExecutes["ScanClickCommand"] = true);
                });
            }
        }
  
        public async Task<bool> ResetResult()
        {//기록 버튼 활성화 또는 비활성화
            if (FilteredComponents.Count > 0)
            {
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Clear();
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Clear();
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs.Clear();

                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULT()
                {
                    RECIPEISTGUID = null,
                    ACTIVITYID = null,
                    IRTGUID = null,
                    IRTRSTGUID = null,
                    ACTVAL = null
                });

                foreach (var eqpt in FilteredComponents)
                {
                    _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA()
                    {
                        LANGID = LogInInfo.LangID,
                        EQPTID = eqpt.EQPTID,
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        BATCHNO = _mainWnd.CurrentOrder.BatchNo
                    });
                }

                FilteredComponents.Clear();

                if (await _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.Execute())
                {
                    IsEnabled = true;
                    foreach (var outdata in _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
                    {
                        FilteredComponents.Add(outdata);
                        //기록버튼
                        if (outdata.AVAILFLAG != "Y")
                            IsEnabled = false;
                    }
                }
            }
            return true;
        }

        public void ScanResult()
        {
            IsEnabled = true;
            foreach (var outdata in FilteredComponents)
            {
                //기록버튼
                if (outdata.AVAILFLAG != "Y")
                    IsEnabled = false;
            }
        }

        void actionPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as EM_EquipmentManagement_PerformAction;
            popup.Closed -= actionPopup_Closed;

            Task<bool> result = ResetResult();
        }

        void ScanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 장비청소팝업;
            popup.Closed -= ScanPopup_Closed;

            //기록버튼 갱신
            ScanResult();
        }

       
    }

    public class IsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString() == "Y")
                {
                    return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
