using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령
{
    public class 반제품입고ViewModel : ViewModelBase
    {
        #region [Property]
        public 반제품입고ViewModel()
        {
            _BR_PHR_GET_BIN_INFO = new BR_PHR_GET_BIN_INFO();
            _BR_BRS_REG_WMS_Request_IN = new BR_BRS_REG_WMS_Request_IN();
            _BR_BRS_GET_VESSEL_WMS_IN = new BR_BRS_GET_VESSEL_WMS_IN();
            _IBCList = new ObservableCollection<ChargedContainer>();
        }

        private 반제품입고 _mainWnd;

        #region Campaign Production
        private BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection _OrderList;
        public BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection OrderList
        {
            get { return _OrderList; }
            set
            {
                _OrderList = value;
                OnPropertyChanged("OrderList");
            }
        }
        private bool _CanSelectOrder;
        public bool CanSelectOrder
        {
            get { return _CanSelectOrder; }
            set
            {
                _CanSelectOrder = value;
                OnPropertyChanged("CanSelectOrder");
            }
        }
        #endregion

        private string _VesselId;
        public string VesselId
        {
            get { return _VesselId; }
            set
            {
                _VesselId = value;
                OnPropertyChanged("VesselId");
            }
        }

        private string _IBCNo;
        public string IBCNo
        {
            get { return _IBCNo; }
            set
            {
                _IBCNo = value;
                OnPropertyChanged("IBCNo");
            }
        }

        private ObservableCollection<ChargedContainer> _IBCList;
        public ObservableCollection<ChargedContainer> IBCList
        {
            get { return _IBCList; }
            set
            {
                _IBCList = value;
                OnPropertyChanged("IBCList");
            }
        }

        #endregion

        #region [BizRule]
        private BR_PHR_GET_BIN_INFO _BR_PHR_GET_BIN_INFO;
        public BR_PHR_GET_BIN_INFO BR_PHR_GET_BIN_INFO
        {
            get { return _BR_PHR_GET_BIN_INFO; }
            set
            {
                _BR_PHR_GET_BIN_INFO = value;
                OnPropertyChanged("BR_PHR_GET_BIN_INFO");
            }
        }

        private BR_BRS_REG_WMS_Request_IN _BR_BRS_REG_WMS_Request_IN;

        private BR_BRS_GET_VESSEL_WMS_IN _BR_BRS_GET_VESSEL_WMS_IN;
        
        #endregion

        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["LoadedCommand"] = false;
                        CommandResults["LoadedCommand"] = false;

                        ///
                        if (arg != null && arg is 반제품입고)
                        {
                            _mainWnd = arg as 반제품입고;

                            #region Campaign Order
                            OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                            CanSelectOrder = OrderList.Count > 0 ? true : false;
                            #endregion

                            // 현재 페이즈가 완료 상태이고 기록이 있는 경우 XML기록 결과를 보여줌
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP") && _mainWnd.CurrentInstruction.Raw.NOTE != null)
                            {
                                var bytearray = _mainWnd.CurrentInstruction.Raw.NOTE;
                                string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                                DataSet ds = new DataSet();
                                ds.ReadXmlFromString(xml);

                                if (ds.Tables.Count == 1 && ds.Tables[0].TableName == "DATA")
                                {
                                    foreach (DataRow row in ds.Tables[0].Rows)
                                    {
                                        _IBCList.Add(new ChargedContainer
                                        {
                                            PoId = row["오더번호"] != null ? row["오더번호"].ToString() : "",
                                            VesselId = row["용기번호"] != null ? row["용기번호"].ToString() : "",
                                            STRGDAY = row["보관기간"] != null ? row["보관기간"].ToString() : "",
                                            EXPIREDTTM = row["유효기한"] != null ? row["유효기한"].ToString() : ""
                                            // 2021.08.18 박희돈 사용안함. 최병인팀장 확인
                                            //MLOTID = row["MLOTID"] != null ? row["MLOTID"].ToString() : ""
                                        });
                                    }
                                    OnPropertyChanged("IBCList");
                                }
                            } 

                            _mainWnd.txtVesselId.Focus();
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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        public ICommand CheckIBCInfoCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] = false;
                        CommandResults["CheckIBCInfoCommandAsync"] = false;

                        ///
                        VesselId = arg as string;

                        _BR_PHR_GET_BIN_INFO.INDATAs.Clear();
                        _BR_PHR_GET_BIN_INFO.OUTDATAs.Clear();

                        _BR_PHR_GET_BIN_INFO.INDATAs.Add(new BR_PHR_GET_BIN_INFO.INDATA
                        {
                           POID = _mainWnd.CurrentOrder.ProductionOrderID,
                           EQPTID = VesselId
                        });

                        if (await _BR_PHR_GET_BIN_INFO.Execute())
                        {
                            if (_BR_PHR_GET_BIN_INFO.OUTDATAs.Count > 0)
                            {
                                IBCNo = _BR_PHR_GET_BIN_INFO.OUTDATAs[0].EQPTID;
                            }
                            else
                                OnMessage("BIN 정보가 없습니다.");
                        }
                        ///

                        CommandResults["CheckIBCInfoCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["CheckIBCInfoCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("CheckIBCInfoCommandAsync") ?
                        CommandCanExecutes["CheckIBCInfoCommandAsync"] : (CommandCanExecutes["CheckIBCInfoCommandAsync"] = true);
                });
            }
        }

        public ICommand StorageOutCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["StorageOutCommandAsync"] = false;
                        CommandResults["StorageOutCommandAsync"] = false;

                        ///
                        if(!CheckIBCId(IBCNo))
                        {
                            OnMessage("이미 요청한 용기입니다.");
                            return;
                        }

                        if (_VesselId == _IBCNo)
                        {
                            _BR_BRS_REG_WMS_Request_IN.INDATAs.Clear();
                            _BR_BRS_REG_WMS_Request_IN.OUTDATAs.Clear();

                            _BR_BRS_REG_WMS_Request_IN.INDATAs.Add(new BR_BRS_REG_WMS_Request_IN.INDATA
                            {
                                VESSELID = IBCNo,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                            });

                            if (await _BR_BRS_REG_WMS_Request_IN.Execute())
                            {
                                if (_BR_BRS_REG_WMS_Request_IN.OUTDATAs.Count > 0)
                                {
                                    IBCList.Add(new ChargedContainer
                                    {
                                        PoId = _mainWnd.CurrentOrder.ProductionOrderID,
                                        VesselId = IBCNo,
                                        STRGDAY = _BR_BRS_REG_WMS_Request_IN.OUTDATAs[0].STRGDAY,
                                        EXPIREDTTM = _BR_BRS_REG_WMS_Request_IN.OUTDATAs[0].EXPIREDTTM
                                        // 2021.08.18 박희돈 사용안함. 최병인팀장 확인
                                        //MLOTID = _BR_BRS_REG_WMS_Request_IN.OUTDATAs[0].MLOTID
                                    });
                                }
                            }
                        }
                        else
                            OnMessage("입력한 내용과 검색한 IBC ID가 다릅니다.");

                        ///

                        CommandResults["StorageOutCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["StorageOutCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["StorageOutCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("StorageOutCommandAsync") ?
                        CommandCanExecutes["StorageOutCommandAsync"] : (CommandCanExecutes["StorageOutCommandAsync"] = true);
                });
            }
        }

        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandCanExecutes["ComfirmCommandAsync"] = false;
                        CommandResults["ComfirmCommandAsync"] = false;

                        ///
                        if (_IBCList.Count > 0)
                        {
                            var authHelper = new iPharmAuthCommandHelper(); // function code 입력
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                string.Format("반제품입고"),
                                string.Format("반제품입고"),
                                false,
                                "OM_ProductionOrder_SUI",
                                _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                            dt.Columns.Add(new DataColumn("오더번호"));
                            //-------------------------------------------------------------------------------------------------------
                            //dt.Columns.Add(new DataColumn("제조번호"));
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("보관기간"));
                            dt.Columns.Add(new DataColumn("유효기한"));
                            // 2021.08.18 박희돈 사용안함. 최병인팀장 확인
                            //dt.Columns.Add(new DataColumn("MLOTID"));

                            foreach (var item in _IBCList)
                            {
                                var row = dt.NewRow();
                                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                row["오더번호"] = item.PoId != null ? item.PoId : "";
                                //-------------------------------------------------------------------------------------------------------
                                //row["제조번호"] = item.PoId != null ? item.PoId : "";
                                row["용기번호"] = item.VesselId != null ? item.VesselId : "";
                                row["보관기간"] = item.STRGDAY != null ? item.STRGDAY : "";
                                row["유효기한"] = item.EXPIREDTTM != null ? item.EXPIREDTTM : "";
                                // 2021.08.18 박희돈 사용안함. 최병인팀장 확인
                                //row["MLOTID"] = item.MLOTID != null ? item.MLOTID : "";

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
                        ///

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
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ?
                        CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region [etc]
        private bool CheckIBCId(string Id)
        {
            string ID = Id.ToUpper();

            foreach (ChargedContainer item in _IBCList)
            {
                if (ID == item.VesselId.ToUpper())
                    return false;
            }
            return true;
        }

        public class ChargedContainer : WIPContainer
        {
            private string _STRGDAY;
            public string STRGDAY
            {
                get { return this._STRGDAY; }
                set
                {
                    this._STRGDAY = value;
                    this.OnPropertyChanged("STRGDAY");
                }
            }
            private string _EXPIREDTTM;
            public string EXPIREDTTM
            {
                get { return this._EXPIREDTTM; }
                set
                {
                    this._EXPIREDTTM = value;
                    this.OnPropertyChanged("EXPIREDTTM");
                }
            }
            private string _MLOTID;
            public string MLOTID
            {
                get { return this._MLOTID; }
                set
                {
                    this._MLOTID = value;
                    this.OnPropertyChanged("MLOTID");
                }
            }
        }
        #endregion
    }
}
