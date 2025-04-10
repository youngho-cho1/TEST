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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Linq;
using System.Threading.Tasks;

namespace 보령
{
    public class 세척장비액션수행ViewModel : ViewModelBase
    {
        #region [Property]
        public 세척장비액션수행ViewModel()
        {
            _BR_PHR_SVP_UPD_EquipmentAction_Multi = new BR_PHR_SVP_UPD_EquipmentAction_Multi();
            //_BR_PHR_UPD_EquipmentAction_Multi = new BR_PHR_UPD_EquipmentAction_Multi();
            _BR_BRS_SEL_EquipmentStatus_SVP_WashParts = new BR_BRS_SEL_EquipmentStatus_SVP_WashParts();
            _BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus = new BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus();
            _filteredComponents = new BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATACollection();
            _EmptyContainerList = new ObservableCollection<EmptyWIPContainer>();
            _BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME = new BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME();

        }

        private 세척장비액션수행 _mainWnd;
        
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

        private BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATA _ActionList;
        public BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATA ActionList
        {
            get { return _ActionList; }
            set
            {
                foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs)
                {
                    item.CHK = "";
                    item.CHK2 = "";
                    item.CHK3 = "";
                    item.CHK4 = "";
                    item.CHK5 = "";
                }

                _ActionList = value;
                OnPropertyChanged("ActionList");
            }
        }

        private BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME.OUTDATA _EqclList;
        public BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME.OUTDATA EqclList
        {
            get { return _EqclList; }
            set
            {
                BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Clear();
                FilteredComponents.Clear();

                _EqclList = value;
                OnPropertyChanged("EqclList");
            }
        }

        private ObservableCollection<EmptyWIPContainer> _EmptyContainerList;
        public ObservableCollection<EmptyWIPContainer> EmptyContainerList
        {
            get { return _EmptyContainerList; }
            set
            {
                _EmptyContainerList = value;
                OnPropertyChanged("EmptyContainerList");
            }
        }

        BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATACollection _filteredComponents;
        public BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }
        
        #endregion

        #region [BizRule]
        //private BR_PHR_UPD_EquipmentAction_Multi _BR_PHR_UPD_EquipmentAction_Multi;
        private BR_PHR_SVP_UPD_EquipmentAction_Multi _BR_PHR_SVP_UPD_EquipmentAction_Multi;

        BR_BRS_SEL_EquipmentStatus_SVP_WashParts _BR_BRS_SEL_EquipmentStatus_SVP_WashParts;
        public BR_BRS_SEL_EquipmentStatus_SVP_WashParts BR_BRS_SEL_EquipmentStatus_SVP_WashParts
        {
            get { return _BR_BRS_SEL_EquipmentStatus_SVP_WashParts; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_SVP_WashParts = value;
                NotifyPropertyChanged();
            }
        }
        
        BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME _BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME;
        public BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME
        {
            get { return _BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME; }
            set
            {
                _BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus _BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus;
        public BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus
        {
            get { return _BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus; }
            set
            {
                _BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus = value;
                NotifyPropertyChanged();
            }
        }
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
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is 세척장비액션수행)
                            {
                                _mainWnd = arg as 세척장비액션수행;

                                IsBusy = true;

                                FromDt = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME.INDATAs.Clear();
                                BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME.OUTDATAs.Clear();

                                if (await BR_BRS_SEL_SVP_PartWash_EquipmentClass_EQCLNAME.Execute() == false) return;

                                //_EmptyContainerList.Clear();
                                //BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Clear();
                                //BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Clear();

                                //// 2023.08.28 박희돈 청소액션 값 조회
                                //BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Clear();
                                //BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Clear();
                                //BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Add(new BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATA
                                //{
                                //    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                //    EQCLID = AuthRepositoryViewModel.GetSystemOptionValue("SVP_PARTWASH_EQUIPEMENTGROUP"),
                                //    EQACIUSE = "Y"
                                //});

                                //if (await BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.Execute())
                                //{
                                //    if (BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Count > 0)
                                //    {
                                //        foreach (var item in BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs)
                                //        {
                                //            if (item.EQACNAME.Contains("청소"))
                                //            {
                                //                FilteredComponents.Add(item);
                                //            }
                                //        }
                                //    }
                                //}

                                //2023.08.28 박희돈 세척설비파츠 설비목록 조회
                                //BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATAs.Clear();
                                //BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Clear();

                                //BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATA
                                //{
                                //    EQCLID = AuthRepositoryViewModel.GetSystemOptionValue("SVP_PARTWASH_EQUIPEMENTGROUP")
                                //}
                                //);

                                //if (await BR_BRS_SEL_EquipmentStatus_SVP_WashParts.Execute() == false) return;

                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }

        public ICommand EqptListSearchCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["EqptListSearchCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["EqptListSearchCommandAsync"] = false;
                            CommandResults["EqptListSearchCommandAsync"] = false;

                            IsBusy = true;


                            if (this.EqclList == null || string.IsNullOrEmpty(this.EqclList.EQCLNAME))
                            {
                                throw new Exception(string.Format("설비군을 선택해주세요"));
                            }

                            _EmptyContainerList.Clear();
                            BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Clear();
                            BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Clear();

                            // 2023.08.28 박희돈 청소액션 값 조회
                            BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Clear();
                            BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Clear();
                            BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATAs.Add(new BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.INDATA
                            {
                                EQCLID = EqclList.EQCLID,
                                EQCLIUSE = "Y"
                            });

                            if (await BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.Execute())
                            {
                                if (BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs.Count > 0)
                                {
                                    foreach (var item in BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus.OUTDATAs)
                                    {
                                        if (item.EQACNAME.Contains("청소"))
                                        {
                                            FilteredComponents.Add(item);
                                        }
                                    }
                                }
                            }

                            //2023.08.28 박희돈 세척설비파츠 설비목록 조회
                            BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATAs.Clear();
                            BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Clear();

                            BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_SVP_WashParts.INDATA
                            {
                                EQCLID = EqclList.EQCLID
                            }
                            );

                            if (await BR_BRS_SEL_EquipmentStatus_SVP_WashParts.Execute() == false) return;

                            IsBusy = false;

                            CommandResults["EqptListSearchCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["EqptListSearchCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["EqptListSearchCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("EqptListSearchCommandAsync") ?
                        CommandCanExecutes["EqptListSearchCommandAsync"] : (CommandCanExecutes["EqptListSearchCommandAsync"] = true);
                });
            }
        }

        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            CommandResults["ComfirmCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            if(this.ActionList == null || string.IsNullOrEmpty(this.ActionList.EQACNAME))
                            {
                                throw new Exception(string.Format("액션을 선택해주세요"));
                            }

                            if (BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK == "Y").Count() <= 0 
                            && BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK2 == "Y").Count() <= 0
                            && BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK3 == "Y").Count() <= 0
                            && BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK4 == "Y").Count() <= 0
                            && BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK5 == "Y").Count() <= 0)
                            {
                                throw new Exception(string.Format("대상이 선택되지 않았습니다."));
                            }

                            var authHelper = new iPharmAuthCommandHelper();
                            
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "세척장비액션수행",
                                "세척장비액션수행",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }
                            
                            //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Clear();
                            //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Clear();

                            _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Clear();
                            _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Clear();

                            // 고정값 조회
                            DateTime curDTTM = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            //string USERID = !string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")) ? AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI") : AuthRepositoryViewModel.Instance.LoginedUserID;
                            // 2025.01.24 박희돈 로그북에 코멘트 조회를 가능하게 하기위해 수정함.
                            string USERID = AuthRepositoryViewModel.Instance.ConfirmedGuid;

                            string EQACID = string.Empty;
                            string EQSTID = string.Empty;
                            string EQPAID = string.Empty;
                            int eqptCnt = 0;

                            foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK == "Y"))
                            {
                                if (!string.IsNullOrEmpty(item.EQPTID1))
                                {
                                    eqptCnt++;

                                    if (this.ActionList.EQACNAME.Contains("청소시작"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANINGSTATUS.ToString();
                                        EQPAID = item.EQPAID1;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소완료"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANSTATUS.ToString();
                                        EQPAID = item.EQPAID2;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소확인(확인자)"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANCHECKSTATUS.ToString();
                                        EQPAID = item.EQPAID3;
                                    }

                                    if (!string.IsNullOrEmpty(item.EQPTID1))
                                    {
                                        //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                        _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATA
                                        {
                                            EQPTID = item.EQPTID1,
                                            EQACID = this.ActionList.EQACID,
                                            USER = USERID,
                                            DTTM = FromDt
                                        });

                                        //_BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                        _BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATA
                                        {
                                            EQPTID = item.EQPTID1,
                                            EQACID = this.ActionList.EQACID,
                                            EQSTID = EQSTID
                                        });

                                        //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                        _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATA
                                        {
                                            EQPTID = item.EQPTID1,
                                            EQSTID = EQSTID,
                                            EQPAID = EQPAID,
                                            PAVAL = FromDt.ToString("yyyy-MM-dd HH:mm:ss")
                                        });
                                    }
                                }
                            }

                            foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK2 == "Y"))
                            {
                                if (!string.IsNullOrEmpty(item.EQPTID2))
                                {
                                    eqptCnt++;

                                    if (this.ActionList.EQACNAME.Contains("청소시작"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANINGSTATUS.ToString();
                                        EQPAID = item.EQPAID1;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소완료"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANSTATUS.ToString();
                                        EQPAID = item.EQPAID2;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소확인(확인자)"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANCHECKSTATUS.ToString();
                                        EQPAID = item.EQPAID3;
                                    }

                                    //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATA
                                    {
                                        EQPTID = item.EQPTID2,
                                        EQACID = EQACID,
                                        USER = USERID,
                                        DTTM = FromDt
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATA
                                    {
                                        EQPTID = item.EQPTID2,
                                        EQACID = EQACID,
                                        EQSTID = EQSTID
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATA
                                    {
                                        EQPTID = item.EQPTID2,
                                        EQSTID = EQSTID,
                                        EQPAID = EQPAID,
                                        PAVAL = FromDt.ToString("yyyy-MM-dd HH:mm:ss")
                                    });
                                }
                            }

                            foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK3 == "Y"))
                            {
                                if (!string.IsNullOrEmpty(item.EQPTID3))
                                {
                                    eqptCnt++;

                                    if (this.ActionList.EQACNAME.Contains("청소시작"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANINGSTATUS.ToString();
                                        EQPAID = item.EQPAID1;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소완료"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANSTATUS.ToString();
                                        EQPAID = item.EQPAID2;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소확인(확인자)"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANCHECKSTATUS.ToString();
                                        EQPAID = item.EQPAID3;
                                    }

                                    //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATA
                                    {
                                        EQPTID = item.EQPTID3,
                                        EQACID = EQACID,
                                        USER = USERID,
                                        DTTM = FromDt
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATA
                                    {
                                        EQPTID = item.EQPTID3,
                                        EQACID = EQACID,
                                        EQSTID = EQSTID
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATA
                                    {
                                        EQPTID = item.EQPTID3,
                                        EQSTID = EQSTID,
                                        EQPAID = EQPAID,
                                        PAVAL = FromDt.ToString("yyyy-MM-dd HH:mm:ss")
                                    });
                                }
                            }

                            foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK4 == "Y"))
                            {
                                if (!string.IsNullOrEmpty(item.EQPTID4))
                                {
                                    eqptCnt++;

                                    if (this.ActionList.EQACNAME.Contains("청소시작"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANINGSTATUS.ToString();
                                        EQPAID = item.EQPAID1;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소완료"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANSTATUS.ToString();
                                        EQPAID = item.EQPAID2;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소확인(확인자)"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANCHECKSTATUS.ToString();
                                        EQPAID = item.EQPAID3;
                                    }

                                    //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATA
                                    {
                                        EQPTID = item.EQPTID4,
                                        EQACID = EQACID,
                                        USER = USERID,
                                        DTTM = FromDt
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATA
                                    {
                                        EQPTID = item.EQPTID4,
                                        EQACID = EQACID,
                                        EQSTID = EQSTID
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATA
                                    {
                                        EQPTID = item.EQPTID4,
                                        EQSTID = EQSTID,
                                        EQPAID = EQPAID,
                                        PAVAL = FromDt.ToString("yyyy-MM-dd HH:mm:ss")
                                    });
                                }
                            }
                            
                            foreach (var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs.Where(o => o.CHK5 == "Y"))
                            {
                                if (!string.IsNullOrEmpty(item.EQPTID5))
                                {
                                    eqptCnt++;

                                    if (this.ActionList.EQACNAME.Contains("청소시작"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANINGSTATUS.ToString();
                                        EQPAID = item.EQPAID1;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소완료"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANSTATUS.ToString();
                                        EQPAID = item.EQPAID2;
                                    }
                                    else if (this.ActionList.EQACNAME.Contains("청소확인(확인자)"))
                                    {
                                        EQACID = ActionList.EQACID;
                                        EQSTID = item.CLEANCHECKSTATUS.ToString();
                                        EQPAID = item.EQPAID3;
                                    }

                                    //_BR_PHR_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.INDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.INDATA
                                    {
                                        EQPTID = item.EQPTID5,
                                        EQACID = EQACID,
                                        USER = USERID,
                                        DTTM = FromDt
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.STATUSDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.STATUSDATA
                                    {
                                        EQPTID = item.EQPTID5,
                                        EQACID = EQACID,
                                        EQSTID = EQSTID
                                    });
                                    //_BR_PHR_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_UPD_EquipmentAction_Multi.PARAMDATA
                                    _BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATAs.Add(new BR_PHR_SVP_UPD_EquipmentAction_Multi.PARAMDATA
                                     {
                                        EQPTID = item.EQPTID5,
                                        EQSTID = EQSTID,
                                        EQPAID = EQPAID,
                                        PAVAL = FromDt.ToString("yyyy-MM-dd HH:mm:ss")
                                    });
                                }
                            }

                            if(eqptCnt > 0)
                            {
                                //if (await _BR_PHR_UPD_EquipmentAction_Multi.Execute())
                                if (await _BR_PHR_SVP_UPD_EquipmentAction_Multi.Execute())
                                {
                                    OnMessage("설비 " + this.ActionList.EQACNAME + " 액션을 수행했습니다.");

                                    foreach(var item in BR_BRS_SEL_EquipmentStatus_SVP_WashParts.OUTDATAs)
                                    {
                                        item.CHK = "";
                                        item.CHK2 = "";
                                        item.CHK3 = "";
                                        item.CHK4 = "";
                                        item.CHK5 = "";
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format("설비 " + this.ActionList.EQACNAME + " 액션을 수행하지 못했습니다."));
                                }
                            }
                            else
                            {
                                OnMessage("선택한 설비가 없습니다.");
                            }
                            
                            IsBusy = false;

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region [User Define]

        public class EmptyWIPContainer : WIPContainer
        {
            private string _EQACID;
            public string EQACID
            {
                get { return _EQACID; }
                set
                {
                    _EQACID = value;
                    OnPropertyChanged("EQACID");
                }
            }
            private string _EQCLID;
            public string EQCLID
            {
                get { return _EQCLID; }
                set
                {
                    _EQCLID = value;
                    OnPropertyChanged("EQCLID");
                }
            }

            private string _EQCANAME;
            public string EQCANAME
            {
                get { return _EQCANAME; }
                set
                {
                    _EQCANAME = value;
                    OnPropertyChanged("EQCANAME");
                }
            }
        }
        #endregion
    }
}