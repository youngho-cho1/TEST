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

namespace 보령
{
    public class 포장완제품입고ViewModel : ViewModelBase
    {
        #region [Property]

        포장완제품입고 _MainWnd;

        private string _txtPbarcode;
        public string txtPbarcode
        {
            get { return _txtPbarcode; }
            set
            {
                _txtPbarcode = value;
                OnPropertyChanged("txtPbarcode");
            }
        }

        private string _txtPallet;
        public string txtPallet
        {
            get { return _txtPallet; }
            set
            {
                _txtPallet = value;
                OnPropertyChanged("txtPallet");
            }
        }

        private string _txtIN_LOCATION;
        public string txtIN_LOCATION
        {
            get { return _txtIN_LOCATION; }
            set
            {
                _txtIN_LOCATION = value;
                OnPropertyChanged("txtIN_LOCATION");
            }
        }

        private string _ScanData;
        public string ScanData
        {
            get { return _ScanData; }
            set
            {
                _ScanData = value;
                OnPropertyChanged("ScanData");
            }
        }

        private bool _isEbConfirm;
        public bool isEbConfirm
        {
            get { return _isEbConfirm; }
            set
            {
                _isEbConfirm = value;
                OnPropertyChanged("isEbConfirm");
            }
        }
        #endregion
        
        #region[Bizlue]

        private BR_BRS_GET_SCANINFO_PALLET_IN _BR_BRS_GET_SCANINFO_PALLET_IN;
        public BR_BRS_GET_SCANINFO_PALLET_IN BR_BRS_GET_SCANINFO_PALLET_IN
        {
            get { return _BR_BRS_GET_SCANINFO_PALLET_IN; }
            set
            {
                _BR_BRS_GET_SCANINFO_PALLET_IN = value;
                OnPropertyChanged("BR_BRS_GET_SCANINFO_PALLET_IN");
            }
        }

        private BR_BRS_REG_WMS_PACK_PALLET_IN _BR_BRS_REG_WMS_PACK_PALLET_IN;
        public BR_BRS_REG_WMS_PACK_PALLET_IN BR_BRS_REG_WMS_PACK_PALLET_IN
        {
            get { return _BR_BRS_REG_WMS_PACK_PALLET_IN; }
            set
            {
                _BR_BRS_REG_WMS_PACK_PALLET_IN = value;
                OnPropertyChanged("BR_BRS_REG_WMS_PACK_PALLET_IN");
            }
        }

        void _BR_BRS_GET_SCANINFO_PALLET_IN_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs)
                {
                    if (item.SCANDATATYPE == "PBARCODE")
                    {
                        if (string.IsNullOrWhiteSpace(txtPbarcode))
                            txtPbarcode = item.SCANDATA;
                    }
                    else if (item.SCANDATATYPE == "PALLET")
                    {
                        if (string.IsNullOrWhiteSpace(txtPallet))
                            txtPallet = item.SCANDATA;
                        
                    }
                    else if (item.SCANDATATYPE == "IN_LOCATION")
                    {
                        if (string.IsNullOrWhiteSpace(txtIN_LOCATION))
                            txtIN_LOCATION = item.SCANDATA;
                    }
                }
                ScanData = "";

                if (!(string.IsNullOrEmpty(txtPbarcode)) && !(string.IsNullOrEmpty(txtPallet)) && !(string.IsNullOrEmpty(txtIN_LOCATION)))
                    isEbConfirm = true;
            }
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
                                IsBusy = true;
                                CommandResults["LoadedCommand"] = false;
                                CommandCanExecutes["LoadedCommand"] = false;

                                if (arg != null)
                                    _MainWnd = arg as 포장완제품입고;

                                isEbConfirm = false;

                                CommandResults["LoadedCommand"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["LoadedCommand"] = true;
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

        public ICommand ScanKeyDownCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        using (await AwaitableLocks["ScanKeyDownCommand"].EnterAsync())
                        {
                            try
                            {
                                IsBusy = true;
                                CommandResults["ScanKeyDownCommand"] = false;
                                CommandCanExecutes["ScanKeyDownCommand"] = false;

                                if (arg != null)
                                {
                                    var temparm = (arg as TextBox).Text;

                                    _BR_BRS_GET_SCANINFO_PALLET_IN.INDATAs.Clear();
                                    _BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs.Clear();

                                    _BR_BRS_GET_SCANINFO_PALLET_IN.INDATAs.Add(new BR_BRS_GET_SCANINFO_PALLET_IN.INDATA
                                    {
                                        POID = _MainWnd.CurrentOrder.ProductionOrderID,
                                        OPGSGUID = _MainWnd.CurrentOrder.OrderProcessSegmentID,
                                        ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        SCANDATA = temparm
                                    });

                                    if (_BR_BRS_GET_SCANINFO_PALLET_IN.INDATAs.Count > 0)
                                        await _BR_BRS_GET_SCANINFO_PALLET_IN.Execute();
                                }

                                CommandResults["ScanKeyDownCommand"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["ScanKeyDownCommand"] = false;
                                OnException(ex.Message, ex);
                            }
                            finally
                            {
                                CommandCanExecutes["ScanKeyDownCommand"] = true;
                                IsBusy = false;
                            }
                        }
                    }, arg =>
                        {
                            return CommandCanExecutes.ContainsKey("ScanKeyDownCommand") ?
                                CommandCanExecutes["ScanKeyDownCommand"] : (CommandCanExecutes["ScanKeyDownCommand"] = true);
                        });
            }
        }

        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                    {
                        using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                        {
                            try
                            {
                                IsBusy = true;
                                CommandResults["ConfirmCommandAsync"] = false;
                                CommandCanExecutes["ConfirmCommandAsync"] = false;

                                var authHelper = new iPharmAuthCommandHelper();

                                if (_MainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _MainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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
                                        "", _MainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }


                                ///////////////////////////포장완제품입고//////////////////////////////////////////////////////////////////////
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_InOut");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "포장 완제품 입고",
                                    "포장 완제품 입고",
                                    false,
                                    "EM_Equipment_InOut",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                _BR_BRS_REG_WMS_PACK_PALLET_IN.INDATAs.Clear();
                                _BR_BRS_REG_WMS_PACK_PALLET_IN.INDATAs.Add(new BR_BRS_REG_WMS_PACK_PALLET_IN.INDATA
                                {
                                    PBARCODE = txtPbarcode,
                                    VESSELID = txtPallet,
                                    IN_LOCATION = txtIN_LOCATION,
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut"),
                                    POID = _MainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _MainWnd.CurrentOrder.OrderProcessSegmentID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID
                                });

                                if (await _BR_BRS_REG_WMS_PACK_PALLET_IN.Execute())
                                {

                                    var ds = new DataSet();
                                    var dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);

                                    dt.Columns.Add(new DataColumn("PBARCODE"));
                                    dt.Columns.Add(new DataColumn("Pallet"));
                                    dt.Columns.Add(new DataColumn("작업대"));


                                    var row = dt.NewRow();
                                    row["PBARCODE"] = txtPbarcode != null ? txtPbarcode : "";
                                    row["Pallet"] = txtPallet != null ? txtPallet : "";
                                    row["작업대"] = txtIN_LOCATION != null ? txtIN_LOCATION : "";

                                    dt.Rows.Add(row);

                                    var xml = BizActorRuleBase.CreateXMLStream(ds);
                                    var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                    _MainWnd.CurrentInstruction.Raw.ACTVAL = _MainWnd.TableTypeName;
                                    _MainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                    var result = await _MainWnd.Phase.RegistInstructionValue(_MainWnd.CurrentInstruction, true);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _MainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                    }

                                    if (_MainWnd.Dispatcher.CheckAccess()) _MainWnd.DialogResult = true;
                                    else _MainWnd.Dispatcher.BeginInvoke(() => _MainWnd.DialogResult = true);
                                }
                                CommandResults["ConfirmCommandAsync"] = true;
                            }
                            catch (Exception ex)
                            {
                                CommandCanExecutes["ConfirmCommandAsync"] = false;
                                OnException(ex.Message, ex);
                            }
                            finally
                            {
                                CommandCanExecutes["ConfirmCommandAsync"] = true;
                                IsBusy = false;
                            }
                        }
                    }, arg =>
                        {
                            return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                                CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                        });
            }
        }

        public ICommand initialCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["initialCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;
                            CommandResults["initialCommandAsync"] = false;
                            CommandCanExecutes["initialCommandAsync"] = false;

                            txtPbarcode = "";
                            txtPallet = "";
                            txtIN_LOCATION = "";

                            CommandResults["initialCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandCanExecutes["initialCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["initialCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("initialCommandAsync") ?
                        CommandCanExecutes["initialCommandAsync"] : (CommandCanExecutes["initialCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region[Construct]

        public 포장완제품입고ViewModel()
        {
            _BR_BRS_GET_SCANINFO_PALLET_IN = new BR_BRS_GET_SCANINFO_PALLET_IN();
            _BR_BRS_GET_SCANINFO_PALLET_IN.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_GET_SCANINFO_PALLET_IN_OnExecuteCompleted);
            _BR_BRS_REG_WMS_PACK_PALLET_IN = new BR_BRS_REG_WMS_PACK_PALLET_IN();
        }

        #endregion
    }
}
