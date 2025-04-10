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
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace 보령
{
    public class 용기중량확인ViewModel : ViewModelBase
    {
        #region [Property]

        용기중량확인 _mainWnd;

        private string _txtIBCID;
        public string txtIBCID
        {
            get { return _txtIBCID; }
            set
            {
                _txtIBCID = value;
                OnPropertyChanged("txtIBCID");
            }
        }

        private bool _btnOKisEnabled;
        public bool btnOKisEnabled
        {
            get { return _btnOKisEnabled; }
            set
            {
                _btnOKisEnabled = value;
                OnPropertyChanged("btnOKisEnabled");
            }
        }

        private ObservableCollection<InputIBCInfo> _ChargingMaterial;
        public ObservableCollection<InputIBCInfo> ChargingMaterial
        {
            get { return _ChargingMaterial; }
            set
            {
                _ChargingMaterial = value;
                OnPropertyChanged("ChargingMaterial");
            }
        }
        
        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel;
        public BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel
        {
            get { return _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel; }
            set
            {
                _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel");
            }
        }

        private BR_BRS_CHK_ProductionOrderSubLot_Valid _BR_BRS_CHK_ProductionOrderSubLot_Valid;
        public BR_BRS_CHK_ProductionOrderSubLot_Valid BR_BRS_CHK_ProductionOrderSubLot_Valid
        {
            get { return _BR_BRS_CHK_ProductionOrderSubLot_Valid; }
            set
            {
                _BR_BRS_CHK_ProductionOrderSubLot_Valid = value;
                OnPropertyChanged("BR_BRS_CHK_ProductionOrderSubLot_Valid");
            }
        }

        #endregion

        #region [Constructor]

        public 용기중량확인ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel = new BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel();
            _BR_BRS_CHK_ProductionOrderSubLot_Valid = new BR_BRS_CHK_ProductionOrderSubLot_Valid();
            _ChargingMaterial = new ObservableCollection<InputIBCInfo>();
        }

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
                            CommandCanExecutes["LoadedCommand"] = false;
                            CommandResults["LoadedCommand"] = false;
                            IsBusy = true;

                            if (arg != null)
                            {
                                _mainWnd = arg as 용기중량확인;

                                btnOKisEnabled = false;
                                txtIBCID = "";

                                _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.Execute() != false)
                                {
                                    if (_BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs.Count > 0)
                                    {
                                        foreach (var item in _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs)
                                        {
                                            ChargingMaterial.Add(new InputIBCInfo
                                            {
                                                IBCID = item.VESSELID,
                                                SCALEID = item.SCALEID,
                                                IBCWEIGHT = item.TAREWEIGHTSTR,
                                                CHK = "대기"
                                            });
                                        }
                                    }
                                }

                            }
                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["LoadedCommand"] = false;
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

        public ICommand SerachCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SerachCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["SerachCommandAsync"] = false;
                            CommandResults["SerachCommandAsync"] = false;

                            txtIBCID = arg as string;

                            _BR_BRS_CHK_ProductionOrderSubLot_Valid.INDATAs.Clear();
                            _BR_BRS_CHK_ProductionOrderSubLot_Valid.OUTDATAs.Clear();

                            _BR_BRS_CHK_ProductionOrderSubLot_Valid.INDATAs.Add(new BR_BRS_CHK_ProductionOrderSubLot_Valid.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                VESSELID = txtIBCID,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            });

                            await _BR_BRS_CHK_ProductionOrderSubLot_Valid.Execute();

                            if (_BR_BRS_CHK_ProductionOrderSubLot_Valid.OUTDATAs.Count > 0 && _BR_BRS_CHK_ProductionOrderSubLot_Valid.OUTDATAs[0].VALIDFLAG.Equals("OK"))
                            {
                                foreach (var item in ChargingMaterial)
                                {
                                    if (item.IBCID.Equals(txtIBCID))
                                        item.CHK = "확인";
                                }
                            }

                            CheckInputIBCInfo();
                       
                            
                            CommandResults["SerachCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["SerachCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SerachCommandAsync"] = true;                            
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SerachCommandAsync") ?
                        CommandCanExecutes["SerachCommandAsync"] : (CommandCanExecutes["SerachCommandAsync"] = true);
                });
            }
        }

        public ICommand ConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;
                            IsBusy = true;

                            if (_BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs.Count == 0)
                            {
                                OnMessage("기록할 데이터가 없습니다.");
                                return;
                            }

                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

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
                                "용기중량확인",
                                "용기중량확인",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");

                            dt.Columns.Add(new DataColumn("IBCID"));
                            dt.Columns.Add(new DataColumn("저울번호"));
                            dt.Columns.Add(new DataColumn("IBC무게"));

                            if (_BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs.Count > 0)
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrderOutput_Emtpy_Vessel.OUTDATAs)
                                {
                                    var row = dt.NewRow();
                                    row["IBCID"] = item.VESSELID != null ? item.VESSELID.ToString() : "";
                                    row["저울번호"] = item.SCALEID != null ? item.SCALEID.ToString() : "";
                                    row["IBC무게"] = item.TAREWEIGHTSTR != null ? item.TAREWEIGHTSTR : "";
                                    dt.Rows.Add(row);
                                }

                                ds.Tables.Add(dt);

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

                            CommandResults["ConfirmCommand"] = false;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["ConfirmCommand"] = false;
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

        #region [User Define]
        public class InputIBCInfo : ViewModelBase
        {
            private string _IBCID;
            public string IBCID
            {
                get { return _IBCID; }
                set
                {
                    _IBCID = value;
                    OnPropertyChanged("IBCID");
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

            private string _IBCWEIGHT;
            public string IBCWEIGHT
            {
                get { return _IBCWEIGHT; }
                set
                {
                    _IBCWEIGHT = value;
                    OnPropertyChanged("IBCWEIGHT");
                }
            }

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
        }

        public void CheckInputIBCInfo()
        {
            foreach (var item in _ChargingMaterial)
            {
                if (item.CHK.Equals("확인"))
                {
                    btnOKisEnabled = true;
                    return;
                }
                btnOKisEnabled = false;
            }
        }
        #endregion
    }
}
