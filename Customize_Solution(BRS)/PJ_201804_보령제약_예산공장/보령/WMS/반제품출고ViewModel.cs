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
using System.Linq;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Text;
using 보령.UserControls;

namespace 보령
{
    public class 반제품출고ViewModel : ViewModelBase
    {
        #region [Property]
        public 반제품출고ViewModel()
        {
            _BR_BRS_SEL_ProductionOrder_IBCList = new BR_BRS_SEL_ProductionOrder_IBCList();
            _BR_BRS_REG_WMS_Request_OUT = new BR_BRS_REG_WMS_Request_OUT();
            _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT = new BR_BRS_CHK_VESSEL_INFO_REQUESTOUT();
            _ListContainer = new ObservableCollection<IBCInfo>();
            _CompleteComponent = new ObservableCollection<IBCInfo>();
        }
        private 반제품출고 _mainWnd;

        // 반제품 목록
        private ObservableCollection<IBCInfo> _ListContainer;
        public ObservableCollection<IBCInfo> ListContainer
        {
            get { return _ListContainer; }
            set
            {
                _ListContainer = value;
                OnPropertyChanged("ListContainer");
            }
        }

        // 출고 목록
        private ObservableCollection<IBCInfo> _CompleteComponent;
        public ObservableCollection<IBCInfo> CompleteComponent
        {
            get { return _CompleteComponent; }
            set
            {
                _CompleteComponent = value;
                OnPropertyChanged("CompleteComponent");
            }
        }
        #endregion
        #region [BizRule]
        private BR_BRS_SEL_ProductionOrder_IBCList _BR_BRS_SEL_ProductionOrder_IBCList;
        public BR_BRS_SEL_ProductionOrder_IBCList BR_BRS_SEL_ProductionOrder_IBCList
        {
            get { return _BR_BRS_SEL_ProductionOrder_IBCList; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_IBCList = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_IBCList");
            }
        }
        private BR_BRS_REG_WMS_Request_OUT _BR_BRS_REG_WMS_Request_OUT;
        private BR_BRS_CHK_VESSEL_INFO_REQUESTOUT _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT;
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
                                IsBusy = true;

                                if (arg != null && arg is 반제품출고)
                                {
                                    _mainWnd = arg as 반제품출고;

                                    // 출고 가능한 용기 목록 조회
                                    _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs.Clear();

                                    _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_IBCList.INDATA
                                    {
                                        TYPE = "FILLED",
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                    });

                                    if (await _BR_BRS_SEL_ProductionOrder_IBCList.Execute() == true)
                                    {
                                        foreach (var item in _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs)
                                        {
                                            ListContainer.Add(new IBCInfo
                                            {
                                                CHK = false,
                                                VESSELID = item.EQPTID,
                                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                                OPSGNAME = item.OPSGNAME,
                                                OUTPUTID = item.OUTPUTID,
                                                OUTPUTGUID = "",
                                                PRODDTTM = item.PRODDTTM,
                                                STATUS = "대기"
                                            });
                                        }
                                    }

                                    // 현재 페이즈가 완료 상태이고 기록이 있는 경우 XML기록 결과를 보여줌
                                    if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP") && _mainWnd.CurrentInstruction.Raw.NOTE != null)
                                    {
                                        DataSet ds = new DataSet();
                                        DataTable dt = new DataTable();

                                        var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                                        string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                                        ds.ReadXmlFromString(xml);
                                        dt = ds.Tables["DATA"];

                                        foreach (var row in dt.Rows)
                                        {
                                            CompleteComponent.Add(new IBCInfo
                                            {
                                                CHK = false,
                                                VESSELID = row["용기번호"] != null ? row["용기번호"].ToString() : "",
                                                OPSGNAME = row["공정명"] != null ? row["공정명"].ToString() : "",
                                                STATUS = row["상태"] != null ? row["상태"].ToString() : ""
                                            });
                                        }
                                    }                                     
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

        public ICommand RequestOUTCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RequestOUTCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["RequestOUTCommandAsync"] = false;
                            CommandResults["RequestOUTCommandAsync"] = false;
                            ///
                            IsBusy = true;
                            _mainWnd.btnRequestOut.IsEnabled = false;

                            // 출고 실패나 출고 가능한 용기가 있는 경우에만 수행
                            if (ListContainer.Count(x => x.CHK && (x.STATUS.Equals("대기") || x.STATUS.Equals("실패"))) > 0)
                            {
                                var authelper = new iPharmAuthCommandHelper();

                                authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_InOut");

                                if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                    , Common.enumAccessType.Create
                                    , "WMS 출고요청"
                                    , "WMS 출고요청"
                                    , false
                                    , "EM_Equipment_InOut"
                                    , ""
                                    , null
                                    , null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                var RequestList = _ListContainer.Where(x => x.CHK && (x.STATUS.Equals("대기") || x.STATUS.Equals("실패")));

                                foreach (var item in RequestList)
                                {
                                    _BR_BRS_REG_WMS_Request_OUT.INDATAs.Clear();
                                    _BR_BRS_REG_WMS_Request_OUT.OUTDATAs.Clear();

                                    _BR_BRS_REG_WMS_Request_OUT.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_REG_WMS_Request_OUT.INDATA
                                    {
                                        VESSELID = item.VESSELID,
                                        ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut"),
                                        POID = item.POID,
                                        OPSGGUID = item.OPSGGUID,
                                        OUTPUTGUID = item.OUTPUTGUID,
                                        OUTGUBUN = "FILLED",
                                        GROSSWEIGHT = null,
                                        UOMID = null
                                    });

                                    if (await _BR_BRS_REG_WMS_Request_OUT.Execute() == true)
                                    {
                                        item.STATUS = "출고완료";
                                        item.CHK = false;
                                        CompleteComponent.Add(item);
                                    }
                                    else
                                    {
                                        item.STATUS = "실패";
                                        item.CHK = false;
                                    }
                                }
                            }
                            
                            ///

                                CommandResults["RequestOUTCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RequestOUTCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RequestOUTCommandAsync"] = true;
                            IsBusy = false;
                            _mainWnd.btnRequestOut.IsEnabled = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RequestOUTCommandAsync") ?
                        CommandCanExecutes["RequestOUTCommandAsync"] : (CommandCanExecutes["RequestOUTCommandAsync"] = true);
                });
            }
        }

        public ICommand CheckVesselCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["CheckVesselCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["CheckVesselCommand"] = false;
                            CommandCanExecutes["CheckVesselCommand"] = false;

                            ///
                            var ScanPopup = new BarcodePopup();
                            ScanPopup.tbMsg.Text = "용기를 스캔하세요";
                            ScanPopup.Closed += async (sender, e) =>
                            {
                                try
                                {
                                    if (ScanPopup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(ScanPopup.tbText.Text))
                                    {
                                        string text = ScanPopup.tbText.Text;

                                        // 용기 정보 조회
                                        _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.INDATAs.Clear();
                                        _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs.Clear();
                                        _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.INDATAs.Add(new BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.INDATA
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            VESSELID = text
                                        });

                                        if (await _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.Execute())
                                        {
                                            if (_BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs.Count == 1) 
                                            {
                                                if (_BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs[0].TYPE.Equals("FILLED"))
                                                {
                                                    bool hasVessel = false;
                                                    foreach (var item in CompleteComponent)
                                                    {
                                                        if (item.VESSELID == text)
                                                            hasVessel = true;
                                                    }

                                                    if (!hasVessel)
                                                    {
                                                        CompleteComponent.Add(new IBCInfo
                                                        {
                                                            STATUS = "출고완료",
                                                            VESSELID = _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs[0].EQPTID,
                                                            OPSGNAME = string.IsNullOrWhiteSpace(_BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs[0].VESSEL_DESC) ? _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs[0].OPSGNAME : _BR_BRS_CHK_VESSEL_INFO_REQUESTOUT.OUTDATAs[0].VESSEL_DESC
                                                        });
                                                    }
                                                    else
                                                        throw new Exception("이미 기록된 용기 입니다.");
                                                }
                                                else
                                                    throw new Exception("반제품 용기가 아닙니다.");
                                            }
                                            else
                                                throw new Exception("용기 정보를 다시 확인하세요.");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    OnException(ex.Message, ex);
                                }
                            };

                            ScanPopup.Show();

                            
                            ///

                            CommandResults["CheckVesselCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["CheckVesselCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["CheckVesselCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("CheckVesselCommand") ?
                       CommandCanExecutes["CheckVesselCommand"] : (CommandCanExecutes["CheckVesselCommand"] = true);
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
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            CommandResults["ConfirmCommandAsync"] = false;
                            ///
                            IsBusy = true;

                            if (CompleteComponent.Count <= 0)
                                throw new Exception("출고완료된 용기정보가 없습니다.");

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "반제품출고",
                                "반제품출고",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("상태"));
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("공정명"));

                            foreach (var item in CompleteComponent)
                            {
                                var row = dt.NewRow();

                                row["상태"] = item.STATUS != null ? item.STATUS : "";
                                row["용기번호"] = item.VESSELID != null ? item.VESSELID : "";
                                row["공정명"] = item.OPSGNAME != null ? item.OPSGNAME : "";

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
        #region [UserDefine]
        public class IBCInfo : ViewModelBase
        {
            private bool _CHK;
            public bool CHK
            {
                get { return _CHK; }
                set
                {
                    _CHK = value;
                    OnPropertyChanged("CHK");
                }
            }

            private string _VESSELID;
            public string VESSELID
            {
                get { return _VESSELID; }
                set
                {
                    _VESSELID = value;
                    OnPropertyChanged("VESSELID");
                }
            }

            private string _POID;
            public string POID
            {
                get { return _POID; }
                set
                {
                    _POID = value;
                    OnPropertyChanged("POID");
                }
            }

            private string _OPSGGUID;
            public string OPSGGUID
            {
                get { return _OPSGGUID; }
                set
                {
                    _OPSGGUID = value;
                    OnPropertyChanged("OPSGGUID");
                }
            }
            private string _OPSGNAME;
            public string OPSGNAME
            {
                get { return _OPSGNAME; }
                set
                {
                    _OPSGNAME = value;
                    OnPropertyChanged("OPSGNAME");
                }
            }

            private string _OUTPUTID;
            public string OUTPUTID
            {
                get { return _OUTPUTID; }
                set
                {
                    _OUTPUTID = value;
                    OnPropertyChanged("OUTPUTID");
                }
            }

            private string _OUTPUTGUID;
            public string OUTPUTGUID
            {
                get { return _OUTPUTGUID; }
                set
                {
                    _OUTPUTGUID = value;
                    OnPropertyChanged("OUTPUTGUID");
                }
            }
            
            private string _STATUS;
            public string STATUS
            {
                get { return _STATUS; }
                set
                {
                    _STATUS = value;
                    OnPropertyChanged("STATUS");
                }
            }

            private decimal _Weight;
            public decimal Weight
            {
                get { return _Weight; }
                set
                {
                    _Weight = value;
                    OnPropertyChanged("Weight");
                }
            }

            private string _PRODDTTM;
            public string PRODDTTM
            {
                get { return _PRODDTTM; }
                set
                {
                    _PRODDTTM = value;
                    OnPropertyChanged("PRODDTTM");
                }
            }

            private BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPCollection _WIPs;
            public BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPCollection WIPs
            {
                get { return _WIPs; }
                set
                {
                    _WIPs = value;
                    OnPropertyChanged("WIPs");
                }
            }

            private BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILCollection _DETAILs;
            public BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILCollection DETAILs
            {
                get { return _DETAILs; }
                set
                {
                    _DETAILs = value;
                    OnPropertyChanged("DETAILs");
                }
            }
        }
        #endregion
    }
}
