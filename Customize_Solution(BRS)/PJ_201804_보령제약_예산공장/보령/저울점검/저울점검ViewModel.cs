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
    public class 저울점검ViewModel : ViewModelBase
    {
        public 저울점검ViewModel()
        {
            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi = new BR_BRS_SEL_EquipmentStatus_4Scale_Multi();
            _filteredComponents = new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATACollection();
            IsEnabled = false;
        }

        #region Property

        저울점검 _mainWnd;
        public 저울점검 mainWnd
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
  
        #endregion

        #region Data

        BR_BRS_SEL_EquipmentStatus_4Scale_Multi _BR_BRS_SEL_EquipmentStatus_4Scale_Multi;
        public BR_BRS_SEL_EquipmentStatus_4Scale_Multi BR_BRS_SEL_EquipmentStatus_4Scale_Multi
        {
            get { return _BR_BRS_SEL_EquipmentStatus_4Scale_Multi; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi = value;
                NotifyPropertyChanged();
            }
        }

        BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

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
                            if (arg != null)
                                _mainWnd = arg as 저울점검;

                            //화면이 띄워질때 Instruction UI또는 Class 등..
                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;
                            //레시피 디자이너 상에서 나를 참조하고 있는 Instruction을 얻어오는 것.
                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);


                            //참조값, UI, Class 아무것도 없는 경우   
                            if (inputValues.Count == 0 && instruction.Raw.ACTVAL == null && instruction.Raw.NOTE == null)
                                return;
                                
                            //BizRule 사용전 초기화
                            FilteredComponents.Clear();
                            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Clear();
                            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Clear();
                            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs.Clear();

                            //참조값 존재 UI 존재 하지 않음
                            if (inputValues.Count != 0 && instruction.Raw.ACTVAL == null && instruction.Raw.NOTE == null)
                            {   //Bizrule Indata추가
                                foreach (var inst in inputValues)
                                {
                                    _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA()
                                    {
                                        LANGID = LogInInfo.LangID,
                                        EQPTID = inst.Raw.ACTVAL != null ? inst.Raw.ACTVAL : inst.Raw.EQPTID
                                    });
                                }
                                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULT()
                                {
                                    RECIPEISTGUID = null,
                                    ACTIVITYID = null,
                                    IRTGUID = null,
                                    IRTRSTGUID = null,
                                    ACTVAL = null
                                });
                            }
                            else if (instruction.Raw.ACTVAL != null || instruction.Raw.NOTE != null)
                            {// UI가 존재하는 경우
                                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA()
                                {
                                    LANGID = null,
                                    EQPTID = null
                                });

                                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULT()
                                {
                                    RECIPEISTGUID = instruction.Raw.RECIPEISTGUID,
                                    ACTIVITYID = instruction.Raw.ACTIVITYID,
                                    IRTGUID = instruction.Raw.IRTGUID,
                                    IRTRSTGUID = instruction.Raw.IRTRSTGUID,
                                    ACTVAL = instruction.Raw.ACTVAL
                                });
                            }

                            if (_BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Count > 0 && await _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.Execute())
                            {
                                IsEnabled = true;  
                                foreach (var outdata in _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs)
                                {  
                                    FilteredComponents.Add(outdata);
                                    if (outdata.AVAIL_FLAG == "N")
                                        IsEnabled = false;
                                }
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
        //저울점검 - Close시 갱신필요
        public ICommand PerformScaleCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PerformScaleCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                        
                            CommandResults["PerformScaleCommand"] = false;
                            CommandCanExecutes["PerformScaleCommand"] = false;

                            ///
                            var check_scale = AuthRepositoryViewModel.Instance.MenuList.OUTDATAs.Where(o => "WM_CheckScale".Equals(o.MENUCODE)).FirstOrDefault();
                            var check_scale_multi = AuthRepositoryViewModel.Instance.MenuList.OUTDATAs.Where(o => "WM_CheckScaleMulti".Equals(o.MENUCODE)).FirstOrDefault();

                            if (check_scale != null && "Weighing".Equals(check_scale.NAMESPACE))
                            {
                                var popup = new SelectScaleCheckType();
                                popup.Closed += (s, e) =>
                                {
                                    if (popup.DialogResult == true)
                                    {
                                        var popCheck = new Weighing.CheckScale()
                                        {
                                            DataContext = new Weighing.CheckScaleViewModel()
                                            {
                                                enumCheckType = (Weighing.CheckScaleViewModel.enumScaleCheckType)popup.CheckType,
                                                ScaleID = ((BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATA)arg).EQPTID

                                            }
                                        };
                                        popCheck.Closed += actionPopup_Closed;
                                        popCheck.Show();
                                    }
                                };
                                popup.Show();
                            }
                            else if (check_scale_multi != null && "Weighing".Equals(check_scale_multi.NAMESPACE))
                            {
                                var popup = new SelectScaleCheckType();
                                popup.Closed += (s, e) =>
                                {
                                    if (popup.DialogResult == true)
                                    {
                                        var popCheck = new Weighing.CheckScaleMulti()
                                        {
                                            DataContext = new Weighing.CheckScaleMultiViewModel()
                                            {
                                                enumCheckType = (Weighing.CheckScaleMultiViewModel.enumScaleCheckType)popup.CheckType,
                                                ScaleID = ((BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATA)arg).EQPTID

                                            }
                                        };
                                        popCheck.Closed += actionPopup_Closed;
                                        popCheck.Show();
                                    }
                                };
                                popup.Show();
                            }
                            ///

                            CommandResults["PerformScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PerformScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PerformScaleCommand"] = true;
                            
                            IsBusy = false;
                        }
                    }
                },  arg =>
                {
                    return CommandCanExecutes.ContainsKey("PerformScaleCommand") ?
                        CommandCanExecutes["PerformScaleCommand"] : (CommandCanExecutes["PerformScaleCommand"] = true);
                });
            }
        }     
        //로그북 - Close시 갱신필요
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

                            var item = arg as BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATA;
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
        //사용시작
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
                            var item = arg as BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATA;
                            var bizRule = new BR_BRS_UPD_EquipmentAction_ShopFloor_PROCSTRT_MULTI();


                            // 전자서명 요청
                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_EquipmentManagement_Action");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("[{0}] 생산 시작", item.EQPTID),
                                string.Format("생산 시작 로그북 생성"),
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
                                PAVAL = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                //PAVAL = this.CurrentOrder.OrderProcessSegmentID,
                                EQPTID = item.EQPTID
                                //EQSTID = "PD_EQSTPROC",
                            });
                            ///
                            if (await bizRule.Execute() == true)
                            {
                                //재조회
                                Task<bool> result = ResetResult();
                            };

                            ///

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
                                if (_BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs.Count > 0)
                                {
                                    int Cnt = 0;
                                    foreach (var item in _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs)
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
                            dt.Columns.Add(new DataColumn("청소상태"));
                            dt.Columns.Add(new DataColumn("청소확인시간"));
                            dt.Columns.Add(new DataColumn("검교정일자"));
                            dt.Columns.Add(new DataColumn("차기교정일자"));
                            dt.Columns.Add(new DataColumn("점검상태"));
                            dt.Columns.Add(new DataColumn("점검일시"));

                            foreach (var item in FilteredComponents)
                            {
                                var row = dt.NewRow();
                                row["관리번호"] = item.EQPTID;
                                row["이름"] = item.EQPTNAME;
                                row["청소상태"] = item.CLEAN_STATUS != null ? item.CLEAN_STATUS : "";
                                row["청소확인시간"] = item.CLEAN_CHECKEDDTTM != null ? item.CLEAN_CHECKEDDTTM : "";
                                row["검교정일자"] = item.CALIBRATIONDTTM != null ? item.CALIBRATIONDTTM : "";
                                row["차기교정일자"] = item.NEXTCALIBRATIONDTTM != null ? item.NEXTCALIBRATIONDTTM : "";
                                row["점검상태"] = item.DAILYCHECKSTATUS != null ? item.DAILYCHECKSTATUS : "";
                                row["점검일시"] = item.DAILYCHECKDTTM != null ? item.DAILYCHECKDTTM : "";
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
                            var viewModel = new 저울스캔팝업ViewModel()
                            {
                                ParentVM = this
                            };

                            var ScanPopup = new 저울스캔팝업()
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
        #endregion

        public async Task<bool> ResetResult()
        {//기록 버튼 활성화 또는 비활성화
            if (FilteredComponents.Count > 0)
            {
                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Clear();
                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Clear();
                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs.Clear();

                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULT()
                {
                    RECIPEISTGUID = null,
                    ACTIVITYID = null,
                    IRTGUID = null,
                    IRTRSTGUID = null,
                    ACTVAL = null
                });
                foreach (var eqpt in FilteredComponents)
                {
                    _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA()
                    {
                        LANGID = LogInInfo.LangID,
                        EQPTID = eqpt.EQPTID,
                    });
                }
                FilteredComponents.Clear();

                if (await _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.Execute())
                {
                    IsEnabled = true;
                    foreach (var outdata in _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs)
                    {
                        FilteredComponents.Add(outdata);
                        if (outdata.AVAIL_FLAG == "N")
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
                if (outdata.AVAIL_FLAG == "N")
                    IsEnabled = false;
            }
        }

        void actionPopup_Closed(object sender, EventArgs e)
        {
            //로그북인경우
            if (sender is EM_EquipmentManagement_PerformAction)
            {
                EM_EquipmentManagement_PerformAction popup = sender as EM_EquipmentManagement_PerformAction;
                popup.Closed -= actionPopup_Closed;
            }
            else if (sender is ScaleCheck)
            {
                ScaleCheck popup = sender as ScaleCheck;
                popup.Closed -= actionPopup_Closed;
            }
            Task<bool> result = ResetResult();
        }

        void ScanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 저울스캔팝업;
            popup.Closed -= ScanPopup_Closed;

            //기록버튼 갱신
            ScanResult();
        }
    }
}
