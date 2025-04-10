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
    public class 부스점검ViewModel : ViewModelBase
    {
        public 부스점검ViewModel()
        {
            _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi = new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi();
            _filteredComponents = new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATACollection();
            IsEnabled = false;
        }

        #region Property

        부스점검 _mainWnd;
        public 부스점검 mainWnd
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

        #endregion property

        #region Data
        //BR_DWS_SEL_EquipmentStatus_4CleanBooth_Multi
        BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi;
        public BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi
        {
            get { return _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi = value;
                NotifyPropertyChanged();
            }
        }

        BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATACollection _filteredComponents;
        public BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

        #endregion Data

        #region command


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
                            if (arg != null) _mainWnd = arg as 부스점검;
                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;
                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            var noValueInstruction = inputValues.Where(o => o.Raw.INSERTEDYN == "N" && o.Raw.EQPTID == null).FirstOrDefault();
                            if (noValueInstruction != null) throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", noValueInstruction.Raw.IRTGUID));


                            //바코드로 추가한 내용 저장 및 참조값 조회
                            _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Clear();

                            foreach (InstructionModel inst in inputValues)
                            {
                                if (string.IsNullOrWhiteSpace(inst.Raw.ACTVAL) && string.IsNullOrWhiteSpace(inst.Raw.EQPTID))
                                {
                                    throw new Exception(string.Format("{0} 참조 지시문에 값이 없습니다.", inst.Raw.IRTGUID));
                                }

                                _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATA()
                                {
                                    LANGID = LogInInfo.LangID,
                                    EQPTID = !string.IsNullOrWhiteSpace(inst.Raw.ACTVAL) ? inst.Raw.ACTVAL : inst.Raw.EQPTID,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });
                            }

                            if (_BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Count > 0 && await _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.Execute())
                            {
                                IsEnabled = true;
                                //어떤 조건에서 기록 버튼을 Enable 시킬지 아직 안정해져있음.
                                foreach (var outdata in _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs)
                                {
                                    FilteredComponents.Add(outdata);
                                    if (outdata.AVAIL_FLAG == "N")
                                    {
                                        IsEnabled = false;
                                    }
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

                            enumRoleType inspectorRole = enumRoleType.ROLE001;

                            if (_mainWnd.CurrentInstruction.Raw.DVTPASSYN != "Y" &&
                                Enum.TryParse<enumRoleType>(_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                            {
                                if (_BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs.Count > 0)
                                {
                                    int Cnt = 0;
                                    foreach (var item in _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs)
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
                            dt.Columns.Add(new DataColumn("가동상태"));
                            dt.Columns.Add(new DataColumn("가동시작시간"));
                            dt.Columns.Add(new DataColumn("풍속"));

                            foreach (var item in FilteredComponents)
                            {
                                var row = dt.NewRow();
                                row["관리번호"] = item.EQPTID;
                                row["이름"] = item.EQPTNAME;
                                row["청소상태"] = item.CLEAN_STATUS != null ? item.CLEAN_STATUS : "";
                                row["청소확인시간"] = item.CLEAN_CHECKEDDTTM != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CLEAN_CHECKEDDTTM) : "";
                                row["가동상태"] = item.PROC_STATUS != null ? item.PROC_STATUS : "";
                                row["가동시작시간"] = item.PROC_CHECKEDDTTM != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.PROC_CHECKEDDTTM) : "";
                                row["풍속"] = item.WINDSPEED_STATUS != null ? item.WINDSPEED_STATUS : "";
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
                            CommandCanExecutes["PerformCommand"] = false;

                            ///
                            // 로그북 관련 ActionPerform을 새로 만들어야 되는듯..
                            var item = arg as BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATA;
                            var actionPopup = new EM_EquipmentManagement_PerformAction();

                            actionPopup.SelectedEquipmentData = item.EQPTID;
                            actionPopup.DataContext = new EM_EquipmentManagementPerformActionViewModel();
                            actionPopup.Closed += actionPopup_Closed;
                            actionPopup.Show();
                            ///

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
                            var viewModel = new 부스점검팝업ViewModel()
                            {
                                ParentVM = this
                            };

                            var ScanPopup = new 부스점검팝업()
                            {
                                DataContext = viewModel
                            };
                            ScanPopup.Closed += scanPopup_Closed;
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
        {
            if (FilteredComponents.Count > 0)
            {
                _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Clear();
                _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs.Clear();

                foreach (var eqpt in FilteredComponents)
                {
                    _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATA()
                    {
                        LANGID = LogInInfo.LangID,
                        EQPTID = eqpt.EQPTID,
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                    });     
                }
                FilteredComponents.Clear();

                if (await _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.Execute())
                {
                    IsEnabled = true;
                    foreach (var outdata in _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs)
                    {
                        FilteredComponents.Add(outdata);
                        if (outdata.AVAIL_FLAG == "N")
                        {
                            IsEnabled = false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion command

        void actionPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as EM_EquipmentManagement_PerformAction;
            popup.Closed -= actionPopup_Closed;
            //this.LoadedCommand.Execute(null);
            Task<bool> result = ResetResult();
        }

        void scanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 부스점검팝업;
            popup.Closed -= scanPopup_Closed;

            IsEnabled = true;

            foreach (var item in FilteredComponents)
            {
                if (item.AVAIL_FLAG.Equals("N"))
                {
                    IsEnabled = false;
                }
            }
        }
    }
}
