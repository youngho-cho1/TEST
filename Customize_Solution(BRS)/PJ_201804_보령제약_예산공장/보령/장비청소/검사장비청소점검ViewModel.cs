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
using C1.Silverlight.Data;
using ShopFloorUI;
using Equipment;
using System.Collections.Generic;
using System.Text;

namespace 보령
{
    public class 검사장비청소점검ViewModel : ViewModelBase
    {
        #region [Property]
        public 검사장비청소점검ViewModel()
        {
            _BR_BRS_SEL_EquipmentStatus_PROCEQPT = new BR_BRS_SEL_EquipmentStatus_PROCEQPT();
            _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi = new BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi();
        }
        private 검사장비청소점검 _mainWnd;
        private List<InstructionModel> refInst;

        private bool _CANRECORDFLAG;
        public bool CANRECORDFLAG
        {
            get { return _CANRECORDFLAG; }
            set
            {
                _CANRECORDFLAG = value;
                OnPropertyChanged("CANRECORDFLAG");
            }
        }
        #endregion
        #region [BizRule]
        //private BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi;
        //public BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi
        //{
        //    get { return _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi; }
        //    set
        //    {
        //        _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi = value;
        //        OnPropertyChanged("BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi ");
        //    }
        //}

        private BR_BRS_SEL_EquipmentStatus_PROCEQPT _BR_BRS_SEL_EquipmentStatus_PROCEQPT;
        public BR_BRS_SEL_EquipmentStatus_PROCEQPT BR_BRS_SEL_EquipmentStatus_PROCEQPT
        {
            get { return _BR_BRS_SEL_EquipmentStatus_PROCEQPT; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_PROCEQPT = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentStatus_PROCEQPT");
            }
        }
        private BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi;
        public BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi
        {
            get { return _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi = value;
                OnPropertyChanged("BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi");
            }
        }

        #endregion
        #region [Command]
        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //public ICommand LoadCommandAsync // 검사장비 정보 조회
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["LoadCommandAsync"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["LoadCommandAsync"] = false;
        //                    CommandCanExecutes["LoadCommandAsync"] = false;

        //                    ///
        //                    IsBusy = true;

        //                    if (arg != null && arg is 검사장비청소점검)
        //                    {
        //                        this._mainWnd = arg as 검사장비청소점검;
        //                        CANRECORDFLAG = false;

        //                        refInst = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

        //                        BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Clear();
        //                        if (refInst.Count > 0)
        //                        {
        //                            foreach (InstructionModel Inst in refInst)
        //                            {
        //                                BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA
        //                                {
        //                                    LANGID = "ko-KR",
        //                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
        //                                    BATCHNO = _mainWnd.CurrentOrder.BatchNo,
        //                                    EQPTID = !string.IsNullOrWhiteSpace(Inst.Raw.EQPTID) ? Inst.Raw.EQPTID : Inst.Raw.ACTVAL
        //                                });
        //                            }

        //                            await BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.Execute();
        //                        }

        //                        CheckRecordFlag();
        //                    }
        //                    IsBusy = false;
        //                    ///

        //                    CommandResults["LoadCommandAsync"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    IsBusy = false;
        //                    CommandResults["LoadCommandAsync"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["LoadCommandAsync"] = true;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
        //                CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
        //        });
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        public ICommand LoadCommandAsync // 검사장비 정보 조회
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadCommandAsync"] = false;
                            CommandCanExecutes["LoadCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (arg != null && arg is 검사장비청소점검)
                            {
                                this._mainWnd = arg as 검사장비청소점검;
                                CANRECORDFLAG = false;

                                refInst = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Clear();
                                if (refInst.Count > 0)
                                {
                                    foreach (InstructionModel Inst in refInst)
                                    {
                                        // 2022.01.05 김호연 N/A는 조회되지 않도록 로직 수정
                                        // 기존 로직
                                        //-------------------------------------------------------------------------------------------------------
                                        //_BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
                                        //{
                                        //    LANGID = "ko-KR",
                                        //    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        //    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        //    BATCHNO = _mainWnd.CurrentOrder.BatchNo,
                                        //    EQPTID = Inst.Raw.EQPTID,
                                        //    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        //    ACTVAL = Inst.Raw.ACTVAL
                                        //});
                                        //-------------------------------------------------------------------------------------------------------

                                        if (Inst.Raw.ACTVAL == null)
                                        {
                                            _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
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
                                        else
                                        {
                                            if (Inst.Raw.ACTVAL.ToUpper() != "N/A")
                                            {
                                                _BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
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
                                    }

                                    await BR_BRS_SEL_EquipmentStatus_PROCEQPT.Execute();
                                }

                                CheckRecordFlag();
                            }
                            IsBusy = false;
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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
                });
            }
        }

        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //public ICommand ActionPerformCommandAsync // 로그북 기록창
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["ActionPerformCommandAsync"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["ActionPerformCommandAsync"] = false;
        //                    CommandCanExecutes["ActionPerformCommandAsync"] = false;

        //                    ///
        //                    IsBusy = true;

        //                    if (arg is Button)
        //                    {
        //                        string selectedEqptId = (((arg as Button).Parent as C1.Silverlight.DataGrid.DataGridCellPresenter).Row.DataItem as BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATA).EQPTID;

        //                        if ((arg as Button).Name == "btnClean")
        //                        {
        //                            // ActionPerform 팝업창
        //                            var ActionPerformPopup = new EM_EquipmentManagement_PerformAction();
        //                            ActionPerformPopup.SelectedEquipmentData = selectedEqptId;
        //                            ActionPerformPopup.DataContext = new EM_EquipmentManagementPerformActionViewModel();
        //                            ActionPerformPopup.Closed += (s, e) =>
        //                            {
        //                                RefreshData(selectedEqptId, selectedEqptId);
        //                            };
        //                            ActionPerformPopup.Show();
        //                        }
        //                    }

        //                    IsBusy = false;
        //                    ///

        //                    CommandResults["ActionPerformCommandAsync"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    IsBusy = false;
        //                    CommandResults["ActionPerformCommandAsync"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["ActionPerformCommandAsync"] = true;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("ActionPerformCommandAsync") ?
        //                CommandCanExecutes["ActionPerformCommandAsync"] : (CommandCanExecutes["ActionPerformCommandAsync"] = true);
        //        });
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        public ICommand ActionPerformCommandAsync // 로그북 기록창
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ActionPerformCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ActionPerformCommandAsync"] = false;
                            CommandCanExecutes["ActionPerformCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (arg is Button)
                            {
                                string selectedEqptId = (((arg as Button).Parent as C1.Silverlight.DataGrid.DataGridCellPresenter).Row.DataItem as BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATA).EQPTID;

                                if ((arg as Button).Name == "btnClean")
                                {
                                    // ActionPerform 팝업창
                                    var ActionPerformPopup = new EM_EquipmentManagement_PerformAction();
                                    ActionPerformPopup.SelectedEquipmentData = selectedEqptId;
                                    ActionPerformPopup.DataContext = new EM_EquipmentManagementPerformActionViewModel();
                                    ActionPerformPopup.Closed += (s, e) =>
                                    {
                                        RefreshData(selectedEqptId, selectedEqptId);
                                    };
                                    ActionPerformPopup.Show();
                                }
                            }

                            IsBusy = false;
                            ///

                            CommandResults["ActionPerformCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ActionPerformCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ActionPerformCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ActionPerformCommandAsync") ?
                        CommandCanExecutes["ActionPerformCommandAsync"] : (CommandCanExecutes["ActionPerformCommandAsync"] = true);
                });
            }
        }


        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //public ICommand ChangeWorkRoomCommandAsync // 검사장비 변경
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["ChangeWorkRoomCommandAsync"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["ChangeWorkRoomCommandAsync"] = false;
        //                    CommandCanExecutes["ChangeWorkRoomCommandAsync"] = false;

        //                    ///
        //                    IsBusy = true;

        //                    string selectedEqptId = (((arg as Button).Parent as C1.Silverlight.DataGrid.DataGridCellPresenter).Row.DataItem as BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATA).EQPTID;
        //                    var Popup = new 설비상태팝업(_mainWnd.CurrentOrder.ProductionOrderID, _mainWnd.CurrentOrder.OrderProcessSegmentID, "INSPEQPT", selectedEqptId);
        //                    Popup.Closed += (s, e) =>
        //                    {
        //                        if (Popup.DialogResult.HasValue && Popup.DialogResult.Value)
        //                            RefreshData(Popup.RSLT.EQPTID, selectedEqptId);
        //                    };
        //                    Popup.Show();

        //                    IsBusy = false;
        //                    ///

        //                    CommandResults["ChangeWorkRoomCommandAsync"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    IsBusy = false;
        //                    CommandResults["ChangeWorkRoomCommandAsync"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["ChangeWorkRoomCommandAsync"] = true;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("ChangeWorkRoomCommandAsync") ?
        //                CommandCanExecutes["ChangeWorkRoomCommandAsync"] : (CommandCanExecutes["ChangeWorkRoomCommandAsync"] = true);
        //        });
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        public ICommand ChangeWorkRoomCommandAsync // 검사장비 변경
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ChangeWorkRoomCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ChangeWorkRoomCommandAsync"] = false;
                            CommandCanExecutes["ChangeWorkRoomCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            string selectedEqptId = (((arg as Button).Parent as C1.Silverlight.DataGrid.DataGridCellPresenter).Row.DataItem as BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATA).EQPTID;
                            var Popup = new 설비상태팝업(_mainWnd.CurrentOrder.ProductionOrderID, _mainWnd.CurrentOrder.OrderProcessSegmentID, "INSPEQPT", selectedEqptId);
                            Popup.Closed += (s, e) =>
                            {
                                if (Popup.DialogResult.HasValue && Popup.DialogResult.Value)
                                    RefreshData(Popup.RSLT.EQPTID, selectedEqptId);
                            };
                            Popup.Show();

                            IsBusy = false;
                            ///

                            CommandResults["ChangeWorkRoomCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ChangeWorkRoomCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ChangeWorkRoomCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangeWorkRoomCommandAsync") ?
                        CommandCanExecutes["ChangeWorkRoomCommandAsync"] : (CommandCanExecutes["ChangeWorkRoomCommandAsync"] = true);
                });
            }
        }

        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //public ICommand ConfirmCommandAsync // Datagird 정보를 XML로 변환하고 창 종료
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["ConfirmCommandAsync"] = false;
        //                    CommandCanExecutes["ConfirmCommandAsync"] = false;

        //                    ///
        //                    IsBusy = true;

        //                    if (BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs.Count > 0)
        //                    {
        //                        // 교정일자 및 차기 교정일자 Validation 추가. by phd 2020.11.10
        //                        foreach (var item in BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
        //                        {
        //                            if (string.IsNullOrWhiteSpace(item.CALIBATIONDTTM.Trim()))
        //                            {
        //                                throw new Exception(string.Format("{0}설비 교정일자가 존재하지 않습니다. 설비 상태를 확인해주세요.", item.EQPTID));
        //                            }

        //                            if (string.IsNullOrWhiteSpace(item.NEXTCALIBATIONDTTM.Trim()))
        //                            {
        //                                throw new Exception(string.Format("{0}설비 차기교정일자가 존재하지 않습니다. 설비 상태를 확인해주세요.", item.EQPTID));
        //                            }
        //                            else
        //                            {
        //                                DateTime toDateTime = await AuthRepositoryViewModel.GetDBDateTimeNow();
        //                                DateTime SelectDateTime = Convert.ToDateTime(item.NEXTCALIBATIONDTTM);
        //                                if (toDateTime > SelectDateTime)
        //                                {
        //                                    throw new Exception(string.Format("{0}설비 차기교정일이 지났습니다. 설비 교정일을 확인해주세요.", item.EQPTID));
        //                                }
        //                            }
        //                        }

        //                        var authHelper = new iPharmAuthCommandHelper();

        //                        // Phase 종료후 값 수정 시 전자서명
        //                        if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
        //                        {
        //                            // 전자서명 요청
        //                            authHelper = new iPharmAuthCommandHelper();
        //                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

        //                            if (await authHelper.ClickAsync(
        //                                Common.enumCertificationType.Function,
        //                                Common.enumAccessType.Create,
        //                                string.Format("기록값을 변경합니다."),
        //                                string.Format("기록값 변경"),
        //                                true,
        //                                "OM_ProductionOrder_SUI",
        //                                "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
        //                            {
        //                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
        //                            }
        //                        }

        //                        // 검사장비청소점검 기록 전자서명
        //                        authHelper = new iPharmAuthCommandHelper();
        //                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
        //                        if (await authHelper.ClickAsync(
        //                            Common.enumCertificationType.Role,
        //                            Common.enumAccessType.Create,
        //                            "검사장비청소점검",
        //                            "검사장비청소점검",
        //                            false,
        //                            "OM_ProductionOrder_SUI",
        //                            "",
        //                            null, null) == false)
        //                        {
        //                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
        //                        }

        //                        // XML 변환
        //                        var ds = new DataSet();
        //                        var dt = new DataTable("DATA");
        //                        ds.Tables.Add(dt);

        //                        dt.Columns.Add(new DataColumn("설비코드"));
        //                        dt.Columns.Add(new DataColumn("설비명"));
        //                        dt.Columns.Add(new DataColumn("교정일자"));
        //                        dt.Columns.Add(new DataColumn("차기교정일자"));
        //                        dt.Columns.Add(new DataColumn("점검상태"));

        //                        foreach (var item in BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
        //                        {
        //                            var row = dt.NewRow();

        //                            row["설비코드"] = item.EQPTID != null ? item.EQPTID : "";
        //                            row["설비명"] = item.EQPTNAME != null ? item.EQPTNAME : "";
        //                            row["교정일자"] = item.CALIBATIONDTTM != null ? item.CALIBATIONDTTM : "";
        //                            row["차기교정일자"] = item.NEXTCALIBATIONDTTM != null ? item.NEXTCALIBATIONDTTM : "";
        //                            row["점검상태"] = item.SCALEDAILYSTATUS != null ? item.SCALEDAILYSTATUS : "";

        //                            dt.Rows.Add(row);
        //                        }

        //                        var xml = BizActorRuleBase.CreateXMLStream(ds);
        //                        var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

        //                        _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
        //                        _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

        //                        var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
        //                        if (result != enumInstructionRegistErrorType.Ok)
        //                        {
        //                            throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
        //                        }

        //                        if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
        //                        else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
        //                    }

        //                    IsBusy = false;
        //                    ///

        //                    CommandResults["ConfirmCommandAsync"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    IsBusy = false;
        //                    CommandResults["ConfirmCommandAsync"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["ConfirmCommandAsync"] = true;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
        //                CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
        //        });
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        public ICommand ConfirmCommandAsync // Datagird 정보를 XML로 변환하고 창 종료
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if (BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Count > 0)
                            {
                                // 교정일자 및 차기 교정일자 Validation 추가. by phd 2020.11.10
                                foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
                                {
                                    if (string.IsNullOrWhiteSpace(item.CALIBATIONDTTM.Trim()))
                                    {
                                        throw new Exception(string.Format("{0}설비 교정일자가 존재하지 않습니다. 설비 상태를 확인해주세요.", item.EQPTID));
                                    }

                                    if (string.IsNullOrWhiteSpace(item.NEXTCALIBATIONDTTM.Trim()))
                                    {
                                        throw new Exception(string.Format("{0}설비 차기교정일자가 존재하지 않습니다. 설비 상태를 확인해주세요.", item.EQPTID));
                                    }
                                    else
                                    {
                                        DateTime toDateTime = await AuthRepositoryViewModel.GetDBDateTimeNow();
                                        DateTime SelectDateTime = Convert.ToDateTime(item.NEXTCALIBATIONDTTM);
                                        if (toDateTime > SelectDateTime)
                                        {
                                            throw new Exception(string.Format("{0}설비 차기교정일이 지났습니다. 설비 교정일을 확인해주세요.", item.EQPTID));
                                        }
                                    }
                                }

                                var authHelper = new iPharmAuthCommandHelper();

                                // Phase 종료후 값 수정 시 전자서명
                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
                                {
                                    // 전자서명 요청
                                    authHelper = new iPharmAuthCommandHelper();
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

                                // 검사장비청소점검 기록 전자서명
                                authHelper = new iPharmAuthCommandHelper();
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "검사장비청소점검",
                                    "검사장비청소점검",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "",
                                    null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // XML 변환
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                //2021.08.24 박희돈 EBR작업으로 인한 설비명 -> 장비 명, 설비코드 -> 장비 번호 로 변경.
                                dt.Columns.Add(new DataColumn("장비번호"));
                                dt.Columns.Add(new DataColumn("장비명"));
                                dt.Columns.Add(new DataColumn("교정일자"));
                                dt.Columns.Add(new DataColumn("차기교정일자"));
                                dt.Columns.Add(new DataColumn("점검상태"));
                                dt.Columns.Add(new DataColumn("SOP문서번호"));

                                foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
                                {
                                    var row = dt.NewRow();

                                    row["장비번호"] = item.EQPTID != null ? item.EQPTID : "";
                                    row["장비명"] = item.EQPTNAME != null ? item.EQPTNAME : "";
                                    row["교정일자"] = item.CALIBATIONDTTM != null ? item.CALIBATIONDTTM : "";
                                    row["차기교정일자"] = item.NEXTCALIBATIONDTTM != null ? item.NEXTCALIBATIONDTTM : "";
                                    row["점검상태"] = item.SCALEDAILYSTATUS != null ? item.SCALEDAILYSTATUS : "";
                                    row["SOP문서번호"] = item.SOPDOC ?? "";

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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //private void CheckRecordFlag()
        //{
        //    if (BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi != null && BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs.Count > 0)
        //    {
        //        CANRECORDFLAG = true;
        //        foreach (var item in BR_BRS_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs)
        //        {
        //            if (item.AVAILFLAG == "N")
        //            {
        //                CANRECORDFLAG = false;
        //                break;
        //            }
        //        }
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        private void CheckRecordFlag()
        {
            if (BR_BRS_SEL_EquipmentStatus_PROCEQPT != null && BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Count > 0)
            {
                CANRECORDFLAG = true;
                foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
                {
                    if (item.AVAILFLAG == "N")
                    {
                        CANRECORDFLAG = false;
                        break;
                    }
                }
            }
        }
        private async void RefreshData(string prevEQPTID, string curEQPTID)
        {
            BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Clear();
            BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Clear();

            BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
            {
                LANGID = "ko-KR",
                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                BATCHNO = _mainWnd.CurrentOrder.BatchNo,
                EQPTID = prevEQPTID,
                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
            });

            if (await BR_BRS_SEL_EquipmentStatus_PROCEQPT.Execute() == true && BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Count > 0)
            {
                foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
                {
                    if (item.EQPTID == curEQPTID)
                    {
                        item.EQPTID = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EQPTID;
                        item.EQPTNAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EQPTNAME;
                        item.BATCHNO = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].BATCHNO;
                        item.MTRLNAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].MTRLNAME;
                        item.CHECKED = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CHECKED;
                        item.ISCURRENT = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].ISCURRENT;
                        item.ATTACHGUID = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].ATTACHGUID;
                        item.FILENAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].FILENAME;
                        item.STATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].STATUS;
                        item.CLEANINGSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANINGSTATUS;
                        item.CLEANDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANDTTM;
                        item.CLEANAVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANAVAILFLAG;
                        item.PROCAVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].PROCAVAILFLAG;
                        item.EXPIREDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EXPIREDTTM;
                        item.AVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].AVAILFLAG;
                        item.CHECKEDDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CHECKEDDTTM;
                        item.CLEANING_ONTIME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANING_ONTIME;
                        item.VALIDFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].VALIDFLAG;
                        item.SCALEDAILYSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].SCALEDAILYSTATUS;
                        item.WEEKLYCHKSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].WEEKLYCHKSTATUS;
                        item.MONTHCHKSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].MONTHCHKSTATUS;
                        item.CALIBRATIONSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CALIBRATIONSTATUS;
                        item.CALIBATIONDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CALIBATIONDTTM;
                        item.NEXTCALIBATIONDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].NEXTCALIBATIONDTTM;                        
                    }
                }
                
            }
        }

        #region 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd
        //private async void RefreshData(string prevEQPTID, string curEQPTID)
        //{
        //    BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Clear();
        //    BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Clear();

        //    BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_PROCEQPT.INDATA
        //    {
        //        LANGID = "ko-KR",
        //        EQPTID = prevEQPTID,
        //        POID = _mainWnd.CurrentOrder.ProductionOrderID,
        //        BATCHNO = _mainWnd.CurrentOrder.BatchNo
        //    });

        //    if (await BR_BRS_SEL_EquipmentStatus_PROCEQPT.Execute() == true && BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs.Count > 0)
        //    {
        //        foreach (var item in BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs)
        //        {
        //            if (item.EQPTID == curEQPTID)
        //            {
        //                item.EQPTID = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EQPTID;
        //                item.EQPTNAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EQPTNAME;
        //                item.BATCHNO = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].BATCHNO;
        //                item.MTRLNAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].MTRLNAME;
        //                item.CHECKED = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CHECKED;
        //                item.ISCURRENT = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].ISCURRENT;
        //                item.ATTACHGUID = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].ATTACHGUID;
        //                item.FILENAME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].FILENAME;
        //                item.STATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].STATUS;
        //                item.CLEANINGSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANINGSTATUS;
        //                item.CLEANDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANDTTM;
        //                item.CLEANAVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANAVAILFLAG;
        //                item.PROCAVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].PROCAVAILFLAG;
        //                item.EXPIREDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].EXPIREDTTM;
        //                item.AVAILFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].AVAILFLAG;
        //                item.CHECKEDDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CHECKEDDTTM;
        //                item.CLEANING_ONTIME = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CLEANING_ONTIME;
        //                item.VALIDFLAG = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].VALIDFLAG;
        //                item.SCALEDAILYSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].SCALEDAILYSTATUS;
        //                item.WEEKLYCHKSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].WEEKLYCHKSTATUS;
        //                item.MONTHCHKSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].MONTHCHKSTATUS;
        //                item.CALIBRATIONSTATUS = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CALIBRATIONSTATUS;
        //                item.CALIBATIONDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].CALIBATIONDTTM;
        //                item.NEXTCALIBATIONDTTM = BR_BRS_SEL_EquipmentStatus_PROCEQPT.OUTDATAs[0].NEXTCALIBATIONDTTM;
        //            }
        //        }
        //    }
        //}
        #endregion 설비정보가 없을 경우 Room에 묶여있는 설비를 조회하기 위해 기존 로직 주석 2020.12.21 phd

        #endregion
    }
}