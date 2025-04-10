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
using System.Collections.Generic;

namespace 보령
{
    public class 칭량Pallet구성확인ViewModel : ViewModelBase
    {
        #region [Property]

        칭량Pallet구성확인 _mainWnd;

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private List<Vessel> _VesselList;
        public List<Vessel> VesselList
        {
            get { return _VesselList; }
            set
            {
                _VesselList = value;
                OnPropertyChanged("VesselList");
            }
        }
        #endregion 

        #region[Bizrule]

        private BR_BRS_ProductionOrderDispense_VesselInfo _BR_BRS_ProductionOrderDispense_VesselInfo;
        public BR_BRS_ProductionOrderDispense_VesselInfo BR_BRS_ProductionOrderDispense_VesselInfo
        {
            get { return _BR_BRS_ProductionOrderDispense_VesselInfo; }
            set
            {
                _BR_BRS_ProductionOrderDispense_VesselInfo = value;
                OnPropertyChanged("BR_BRS_ProductionOrderDispense_VesselInfo");
            }
        }

        #region 
        void _BR_BRS_ProductionOrderDispense_VesselInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATAs.Count > 0)
            {
                _mainWnd.txtPalltet.Focus();
            }
        }
        #endregion 
        #endregion

        #region [Command]

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

                            if (arg == null || !(arg is 칭량Pallet구성확인))
                                return;

                            IsEnabled = false;

                            _mainWnd = arg as 칭량Pallet구성확인;

                            _BR_BRS_ProductionOrderDispense_VesselInfo.INDATAs.Clear();
                            _BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATAs.Clear();

                            _BR_BRS_ProductionOrderDispense_VesselInfo.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_ProductionOrderDispense_VesselInfo.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.OrderID,
                                OPSGNAME = _mainWnd.CurrentInstruction.Raw.TARGETVAL,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID
                            });

                            if (await _BR_BRS_ProductionOrderDispense_VesselInfo.Execute() && _BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATAs.Count > 0 && _BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATA_DETAILs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATAs)
                                {
                                    VesselList.Add(new Vessel
                                     {
                                         CONFIRMYN = item.CONFIRMYN,
                                         OPSGNAME = item.OPSGNAME,
                                         VESSELID = item.VESSELID,
                                         Detail = new List<VesselDetail>()
                                     });
                                }

                                foreach (var item in _BR_BRS_ProductionOrderDispense_VesselInfo.OUTDATA_DETAILs)
                                {
                                    foreach (var vessel in VesselList)
                                    {
                                        if (vessel.VESSELID.Equals(item.VESSELID))
                                        {
                                            vessel.Detail.Add(new VesselDetail
                                            {
                                                MTRLID = item.MTRLID,
                                                MTRLNAME = item.MTRLNAME,
                                                MLOTID = item.MLOTID,
                                                DSPQTY = item.DSPQTY
                                            });
                                        }
                                    }
                                }
                            }
                            _mainWnd.dgMon.ItemsSource = VesselList;
                            _mainWnd.txtPalltet.Focus();

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

                            if (arg == null || !(arg is TextBox))
                                return;

                            IsEnabled = false;

                            var tmparam = (arg as TextBox).Text;
                            int count = 0;

                            if (VesselList.Count > 0)
                            {
                                foreach (var item in VesselList)
                                {
                                    if (item.VESSELID == tmparam)
                                    {
                                        item.CONFIRMYN = "확인완료";
                                        _mainWnd.dgDetail.ItemsSource = item.Detail;
                                    }

                                    if (item.CONFIRMYN != "확인완료")
                                        count++;
                                }
                            }

                            _mainWnd.txtPalltet.Text = "";
                            if (count == 0)
                            {
                                IsEnabled = true;
                            }
                            else
                            {
                                _mainWnd.txtPalltet.Focus();
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
                            CommandCanExecutes["ConfirmCommand"] = false;

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
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            // 전자서명 요청
                            var AuthHelper = new iPharmAuthCommandHelper();
                            AuthHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await AuthHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format(""),
                                string.Format("칭량Pallet 구성확인"),
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("확인여부"));
                            dt.Columns.Add(new DataColumn("공정명"));
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("원료코드"));
                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("성적번호"));
                            dt.Columns.Add(new DataColumn("칭량결과"));

                            if (VesselList.Count > 0)
                            {
                                foreach (var item in VesselList)
                                {
                                    foreach (var mtrl in item.Detail)
                                    {
                                        var row = dt.NewRow();

                                        row["확인여부"] = item.CONFIRMYN != null ? item.CONFIRMYN : "";
                                        row["공정명"] = item.OPSGNAME != null ? item.OPSGNAME : "";
                                        row["용기번호"] = item.VESSELID != null ? item.VESSELID : "";
                                        row["원료코드"] = mtrl.MTRLID != null ? mtrl.MTRLID : "";
                                        row["원료명"] = mtrl.MTRLNAME != null ? mtrl.MTRLNAME : "";
                                        row["성적번호"] = mtrl.MLOTID != null ? mtrl.MLOTID : "";
                                        row["칭량결과"] = mtrl.DSPQTY != null ? mtrl.DSPQTY : "";

                                        dt.Rows.Add(row);
                                    }
                                }
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,칭량Pallet구성확인";
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

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
        #endregion

        #region [Constructor]

        public 칭량Pallet구성확인ViewModel()
        {
            _BR_BRS_ProductionOrderDispense_VesselInfo = new BR_BRS_ProductionOrderDispense_VesselInfo();
            _BR_BRS_ProductionOrderDispense_VesselInfo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_ProductionOrderDispense_VesselInfo_OnExecuteCompleted);
            _VesselList = new List<Vessel>();
        }


        #endregion 

        #region [User Define]
        public class Vessel : ViewModelBase
        {
            private string _CONFIRMYN;
            public string CONFIRMYN
            {
                get { return _CONFIRMYN; }
                set
                {
                    _CONFIRMYN = value;
                    NotifyPropertyChanged();
                }
            }

            private string _OPSGNAME;
            public string OPSGNAME
            {
                get { return _OPSGNAME; }
                set
                {
                    _OPSGNAME = value;
                    NotifyPropertyChanged();
                }
            }

            private string _VESSELID;
            public string VESSELID
            {
                get { return _VESSELID; }
                set
                {
                    _VESSELID = value;
                    NotifyPropertyChanged();
                }
            }

            private List<VesselDetail> _Detail;
            public List<VesselDetail> Detail
            {
                get { return _Detail; }
                set
                {
                    _Detail = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public class VesselDetail : ViewModelBase
        {
            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    NotifyPropertyChanged();
                }
            }
            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    NotifyPropertyChanged();
                }
            }
            private string _MLOTID;
            public string MLOTID
            {
                get { return _MLOTID; }
                set
                {
                    _MLOTID = value;
                    NotifyPropertyChanged();
                }
            }
            private string _DSPQTY;
            public string DSPQTY
            {
                get { return _DSPQTY; }
                set
                {
                    _DSPQTY = value;
                    NotifyPropertyChanged();
                }
            }
            
        }
        #endregion
    }
}
