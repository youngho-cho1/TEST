using LGCNS.iPharmMES.Common;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using C1.Silverlight.Data;
using ShopFloorUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace 보령
{
    public class 칭량원료Pallet적재ViewModel : ViewModelBase
    {
        #region Property
        private 칭량원료Pallet적재 _mainWnd;

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

        private string _HeaderTextComponentName;
        public string HeaderTextComponentName
        {
            get
            {
                return _HeaderTextComponentName;
            }
            set
            {
                _HeaderTextComponentName = value;
                OnPropertyChanged("HeaderTextComponentName");
            }
        }

        private string _lblEqptID;
        public string lblEqptID
        {
            get { return _lblEqptID; }
            set
            {
                _lblEqptID = value;
                OnPropertyChanged("lblEqptID");
            }
        }


        private BR_PHR_SEL_System_Option_OPTIONTYPE _BR_PHR_SEL_System_Option_OPTIONTYPE;
        
        #endregion

        #region Data

        private BR_BRS_SEL_Equipment_GetInfo _BR_BRS_SEL_Equipment_GetInfo;

        private BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging;
        public BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging
        {
            get { return _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging; }
            set
            {
                _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI _BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI;
        public BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI
        {
            get { return _BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI; }
            set
            {
                _BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI = value;
                NotifyPropertyChanged();
            }
        }

        private BR_UDB_REG_PMS_FILLED_BIN_IN _BR_UDB_REG_PMS_FILLED_BIN_IN;
        public BR_UDB_REG_PMS_FILLED_BIN_IN BR_UDB_REG_PMS_FILLED_BIN_IN
        {
            get { return _BR_UDB_REG_PMS_FILLED_BIN_IN; }
            set
            {
                _BR_UDB_REG_PMS_FILLED_BIN_IN = value;
                NotifyPropertyChanged();
            }
        }
        private BR_BRS_GET_DispenseSubLot_VESSID_ISNULL _BR_BRS_GET_DispenseSubLot_VESSID_ISNULL;
        public BR_BRS_GET_DispenseSubLot_VESSID_ISNULL BR_BRS_GET_DispenseSubLot_VESSID_ISNULL
        {
            get { return _BR_BRS_GET_DispenseSubLot_VESSID_ISNULL; }
            set
            {
                _BR_BRS_GET_DispenseSubLot_VESSID_ISNULL = value;
                NotifyPropertyChanged();
            }
        }

        private DataTable _DtLayerCharging;
        public DataTable DtLayerCharging
        {
            get { return _DtLayerCharging; }
            set
            {
                _DtLayerCharging = value;
                NotifyPropertyChanged();
                //OnPropertyChanged("EQPTID");
                //OnPropertyChanged("BATCHNO");
                //OnPropertyChanged("OPSGNAME");
                //OnPropertyChanged("MTRLID");
                //OnPropertyChanged("MTRLNAME");
                //OnPropertyChanged("MLOTID");
                //OnPropertyChanged("MSUBLOTQTY");
                //OnPropertyChanged("UOMNAME");
                //OnPropertyChanged("DtLayerCharging");
            }
        }

        private ObservableCollection<LayerCharging> _ListContainer;
        public ObservableCollection<LayerCharging> ListContainer
        {
            get { return _ListContainer; }
            set
            {
                _ListContainer = value;
                OnPropertyChanged("ListContainer");
            }
        }
        #endregion

        #region Instructor

        public 칭량원료Pallet적재ViewModel()
        {
            _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging = new BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging();
            _BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI = new BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI();
            _BR_UDB_REG_PMS_FILLED_BIN_IN = new BR_UDB_REG_PMS_FILLED_BIN_IN();
            _BR_BRS_GET_DispenseSubLot_VESSID_ISNULL = new BR_BRS_GET_DispenseSubLot_VESSID_ISNULL();
            _BR_BRS_SEL_Equipment_GetInfo = new BR_BRS_SEL_Equipment_GetInfo();
            _BR_PHR_SEL_System_Option_OPTIONTYPE = new BR_PHR_SEL_System_Option_OPTIONTYPE();
            ListContainer = new ObservableCollection<보령.칭량원료Pallet적재ViewModel.LayerCharging>();
            //_DtLayerCharging = new DataTable();
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
                            if (arg != null && arg is 칭량원료Pallet적재)
                            {
                                _mainWnd = arg as 칭량원료Pallet적재;

                                #region Campaign Order
                                OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                                CanSelectOrder = OrderList.Count > 0 ? true : false;
                                #endregion

                                _mainWnd.txtContainer.Focus();
                            }

                            HeaderTextComponentName = string.Concat(_mainWnd.CurrentOrder.ProductionOrderID, " / ", _mainWnd.CurrentOrder.BatchNo, " / ", _mainWnd.CurrentOrder.MaterialID, " / ", _mainWnd.CurrentOrder.MaterialName);
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

        /// <summary>
        /// 적재할 용기 스캔 Command
        /// </summary>
        public ICommand ContainerBarcodeChangedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ContainerBarcodeChangedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ContainerBarcodeChangedCommand"] = false;
                            CommandCanExecutes["ContainerBarcodeChangedCommand"] = false;

                            ///
                            _mainWnd.txtContainer.Text = _mainWnd.txtContainer.Text.ToUpper();

                            _BR_BRS_SEL_Equipment_GetInfo.INDATAs.Clear();
                            _BR_BRS_SEL_Equipment_GetInfo.OUTDATAs.Clear();

                            _BR_BRS_SEL_Equipment_GetInfo.INDATAs.Add(new BR_BRS_SEL_Equipment_GetInfo.INDATA()
                            {
                               EQPTID = _mainWnd.txtContainer.Text
                            });

                            lblEqptID = null;

                            if (await _BR_BRS_SEL_Equipment_GetInfo.Execute() && _BR_BRS_SEL_Equipment_GetInfo.OUTDATAs.Count > 0)
                            {
                                if(string.IsNullOrWhiteSpace(_mainWnd.CurrentInstruction.Raw.EQCLID) || 
                                    (_mainWnd.CurrentInstruction.Raw.EQCLID == _BR_BRS_SEL_Equipment_GetInfo.OUTDATAs[0].EQCLID || _mainWnd.CurrentInstruction.Raw.EQCLID == _BR_BRS_SEL_Equipment_GetInfo.OUTDATAs[0].PARENTEQCLID))
                                {
                                    lblEqptID = _BR_BRS_SEL_Equipment_GetInfo.OUTDATAs[0].EQPTID;
                                    await RetrieveMaterialSublotWithVessel();
                                    _mainWnd.txtMaterial.Focus();
                                }
                                else
                                {
                                    BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Clear();
                                    BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs.Clear();
                                    OnMessage("사용할 수 없는 용기입니다.");
                                }
                            }
                            ///

                            CommandResults["ContainerBarcodeChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ContainerBarcodeChangedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ContainerBarcodeChangedCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ContainerBarcodeChangedCommand") ?
                        CommandCanExecutes["ContainerBarcodeChangedCommand"] : (CommandCanExecutes["ContainerBarcodeChangedCommand"] = true);
                });
            }
        }

        /// <summary>
        /// 소분원료 바코드 스캔 Command
        /// </summary>
        public ICommand MaterialBarcodeChangedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["MaterialBarcodeChangedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["MaterialBarcodeChangedCommand"] = false;
                            CommandCanExecutes["MaterialBarcodeChangedCommand"] = false;

                            ///
                            _mainWnd.txtMaterial.Text = _mainWnd.txtMaterial.Text.ToUpper();

                            bool bSeach = false;
                            foreach (var outdata in BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs)
                            {
                                if (outdata.MSUBLOTBCD.Equals(_mainWnd.txtMaterial.Text))
                                {
                                    var isChecked = BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs.Where(o => o.CHK == "Y").FirstOrDefault();

                                    // 2019.03.27 차요한
                                    // 이미 선택한 적재대상 소분 원료가 있다면
                                    // 동일 POID와 OPSGGUID 인지 Validation
                                    if (isChecked != null)
                                    {
                                        var valid = BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs.Where(o => o.CHK == "Y" && o.POID == outdata.POID && o.OPSGGUID == outdata.OPSGGUID).FirstOrDefault();

                                        if (valid == null)
                                            throw new Exception(string.Format("이전 선택한 소분원료의 PO와 공정이 상이한 소분 원료 입니다."));
                                    }

                                    bSeach = true;
                                    outdata.CHK = "Y";
                                }
                            }

                            if (bSeach == false)
                                throw new Exception(string.Format("해당 소분 원료가 리스트에 없습니다."));
                            ///

                            CommandResults["MaterialBarcodeChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["MaterialBarcodeChangedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            _mainWnd.txtMaterial.Text = "";
                            _mainWnd.txtMaterial.Focus();

                            CommandCanExecutes["MaterialBarcodeChangedCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("MaterialBarcodeChangedCommand") ?
                        CommandCanExecutes["MaterialBarcodeChangedCommand"] : (CommandCanExecutes["MaterialBarcodeChangedCommand"] = true);
                });
            }
        }

        /// <summary>
        /// 소분원료 적재
        /// </summary>
        public ICommand ClickBinLoadCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickBinLoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickBinLoadCommand"] = false;
                            CommandCanExecutes["ClickBinLoadCommand"] = false;

                            ///
                            if (BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs.Where(o => o.CHK == "Y").Count() <= 0)
                            {
                                throw new Exception(string.Format("대상이 선택되지 않았습니다."));
                            }

                            if (lblEqptID == null && lblEqptID.Length == 0)
                            {
                                throw new Exception(string.Format("용기번호가 없습니다."));
                            }

                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "WM_mgtWeighing_UI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "",
                                "BIN(Pallet) 적재",
                                false,
                                "WM_mgtWeighing_UI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var operatorID = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_mgtWeighing_UI") ?? AuthRepositoryViewModel.Instance.LoginedUserID;

                            BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOTs.Clear();
                            BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOTs.Clear();

                            foreach (var outdata in BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs)
                            {
                                if (outdata.CHK == "Y")
                                {
                                    BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOT()
                                    {
                                        POID = outdata.POID,
                                        OPSGGUID = outdata.OPSGGUID,
                                        COMPONENTGUID = outdata.COMPONENTGUID,
                                        MTRLID = outdata.MTRLID,
                                        MLOTID = outdata.MLOTID,
                                        MLOTVER = outdata.MLOTVER,
                                        REASON = AuthRepositoryViewModel.GetCommentByFunctionCode("WM_mgtWeighing_UI") ?? "",
                                        INDUSER = operatorID,
                                        VESSELID = lblEqptID,
                                        WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID
                                    });

                                    BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOT()
                                    {
                                        MLOTID = outdata.MLOTID,
                                        MSUBLOTID = outdata.MSUBLOTID,
                                        MSUBLOTVER = outdata.MSUBLOTVER,
                                        MSUBLOTSEQ = outdata.MSUBLOTSEQ,
                                        MSUBLOTBCD = outdata.MSUBLOTBCD,
                                        COMPONENTGUID = outdata.COMPONENTGUID
                                    });
                                }
                            }

                            if (!await BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.Execute()) return;

                            await RetrieveMaterialSublotWithVessel();
                            ///


                            CommandResults["ClickBinLoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickBinLoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickBinLoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickBinLoadCommand") ?
                        CommandCanExecutes["ClickBinLoadCommand"] : (CommandCanExecutes["ClickBinLoadCommand"] = true);
                });
            }
        }
        /// <summary>
        /// 소분원료 적재취소 Command
        /// </summary>
        public ICommand ClickCancelBinLoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ClickCancelBinLoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ClickCancelBinLoadCommandAsync"] = false;
                            CommandCanExecutes["ClickCancelBinLoadCommandAsync"] = false;

                            ///

                            if (BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Where(o => o.CHK == "Y").Count() <= 0)
                            {
                                throw new Exception(string.Format("대상이 선택되지 않았습니다."));
                            }

                            if (lblEqptID == null && lblEqptID.Length == 0)
                            {
                                throw new Exception(string.Format("용기번호가 없습니다."));
                            }

                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "WM_mgtWeighing_UI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "",
                                "BIN(Pallet) 적재 취소",
                                false,
                                "WM_mgtWeighing_UI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var operatorID = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_mgtWeighing_UI");

                            BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOTs.Clear();
                            BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOTs.Clear();

                            foreach (var outdata in BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs)
                            {
                                if (outdata.CHK == "Y")
                                {
                                    BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MLOT()
                                    {
                                        POID = outdata.POID,
                                        OPSGGUID = outdata.OPSGGUID,
                                        COMPONENTGUID = outdata.COMPONENTGUID,
                                        MTRLID = outdata.MTRLID,
                                        MLOTID = outdata.MLOTID,
                                        MLOTVER = outdata.MLOTVER.GetValueOrDefault(),
                                        REASON = null,
                                        INDUSER = operatorID,
                                        VESSELID = null,
                                        WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID
                                    });

                                    BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.INDATA_MSUBLOT()
                                    {
                                        MLOTID = outdata.MLOTID,
                                        MSUBLOTID = outdata.MSUBLOTID,
                                        MSUBLOTVER = outdata.MSUBLOTVER,
                                        MSUBLOTSEQ = outdata.MSUBLOTSEQ,
                                        MSUBLOTBCD = outdata.MSUBLOTBCD,
                                        COMPONENTGUID = outdata.COMPONENTGUID
                                    });
                                }
                            }

                            if (!await BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI.Execute()) return;

                            await RetrieveMaterialSublotWithVessel();

                            bool checkFlag = false;
                            // 적재 취소 할 경우 기록 준비 항목에서 동일한 VessalID 삭제. 2021.01.12 phd
                            for(int i = ListContainer.Count - 1; i >= 0; i--)
                            {
                                if (ListContainer[i].VESSELID.Equals(_lblEqptID))
                                {
                                    ListContainer.RemoveAt(i);
                                    checkFlag = true;
                                }
                            }

                            if(checkFlag)
                            {
                                OnMessage("적재 쥐소 항목이 존재하여 기록 준비항목에서 동일 용기의 기록 준비를 취소했습니다.\r\n기록 준비를 확인 해주세요.");
                            }

                            CommandResults["ClickCancelBinLoadCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ClickCancelBinLoadCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ClickCancelBinLoadCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ClickCancelBinLoadCommandAsync") ?
                        CommandCanExecutes["ClickCancelBinLoadCommandAsync"] : (CommandCanExecutes["ClickCancelBinLoadCommandAsync"] = true);
                });
            }
        }
        /// <summary>
        /// 여러 용기를 기록하기 위해 추가. 2021.01.12 phd
        /// </summary>
        public ICommand ConfirmReadyCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmReadyCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmReadyCommand"] = false;
                            CommandCanExecutes["ConfirmReadyCommand"] = false;

                            if (BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Count > 0 && !string.IsNullOrEmpty(lblEqptID))
                            {
                                foreach (var item in BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs)
                                {
                                    ListContainer.Add(new LayerCharging
                                    {
                                        VESSELID = lblEqptID,
                                        POID = item.POID ?? "",
                                        BATCHNO = item.BATCHNO ?? "",
                                        OPSGNAME = item.OPSGNAME ?? "",
                                        MTRLID = item.MTRLID ?? "",
                                        MTRLNAME = item.MTRLNAME ?? "",
                                        TSTREQNO = item.TSTREQNO ?? "",
                                        MSUBLOTQTY = item.MSUBLOTQTY.ToString() ?? "",
                                        UOMNAME = item.UOMNAME ?? ""
                                    });
                                }
                            }

                            CommandResults["ConfirmReadyCommand"] = true;

                            IsBusy = false;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmReadyCommand"] = false;
                            OnException(ex.Message, ex);
                            IsBusy = false;
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmReadyCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                        CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
                });
            }
        }

        public ICommand ConfirmReadyCancelCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmReadyCancelCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmReadyCancelCommand"] = false;
                            CommandCanExecutes["ConfirmReadyCancelCommand"] = false;

                            ListContainer.Clear();

                            CommandResults["ConfirmReadyCancelCommand"] = true;

                            IsBusy = false;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmReadyCancelCommand"] = false;
                            OnException(ex.Message, ex);
                            IsBusy = false;
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmReadyCancelCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmReadyCancelCommand") ?
                        CommandCanExecutes["ConfirmReadyCancelCommand"] : (CommandCanExecutes["ConfirmReadyCancelCommand"] = true);
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
                            IsBusy = true;

                            CommandResults["ConfirmCommand"] = false;
                            CommandCanExecutes["ConfirmCommand"] = false;

                            if (ListContainer.Count == 0)
                            {
                                OnMessage("기록 준비 항목이 없습니다.");
                            }
                            else
                            {
                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);

                                //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                dt.Columns.Add(new DataColumn("오더번호"));
                                //-------------------------------------------------------------------------------------------------------
                                dt.Columns.Add(new DataColumn("용기번호"));
                                dt.Columns.Add(new DataColumn("원료코드"));
                                dt.Columns.Add(new DataColumn("원료명"));
                                dt.Columns.Add(new DataColumn("원료시험번호"));
                                dt.Columns.Add(new DataColumn("칭량결과"));

                                foreach (var item in ListContainer)
                                {
                                    var row = dt.NewRow();
                                    //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                    row["오더번호"] = item.POID != null ? item.POID : "";
                                    //-------------------------------------------------------------------------------------------------------
                                    row["용기번호"] = item.VESSELID ?? "";
                                    row["원료코드"] = item.MTRLID ?? "";
                                    row["원료명"] = item.MTRLNAME ?? "";
                                    row["원료시험번호"] = item.TSTREQNO ?? "";
                                    row["칭량결과"] = item.MSUBLOTQTYSTR ?? "";
                                    dt.Rows.Add(row);
                                }

                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }

                            CommandResults["ConfirmCommand"] = true;

                            IsBusy = false;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                            IsBusy = false;
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                        CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
                });
            }
        }

        /// <summary>
        /// 오더 변경 시 HeaderText 수정하는 Command
        /// </summary>
        public ICommand OrderChagedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["OrderChagedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["OrderChagedCommandAsync"] = false;
                            CommandCanExecutes["OrderChagedCommandAsync"] = false;

                            ///
                            HeaderTextComponentName = string.Concat(_mainWnd.CurrentOrder.ProductionOrderID, " / ", _mainWnd.CurrentOrder.BatchNo, " / ", _mainWnd.CurrentOrder.MaterialID, " / ", _mainWnd.CurrentOrder.MaterialName);
                            ///

                            CommandResults["OrderChagedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["OrderChagedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["OrderChagedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("OrderChagedCommandAsync") ?
                       CommandCanExecutes["OrderChagedCommandAsync"] : (CommandCanExecutes["OrderChagedCommandAsync"] = true);
               });
            }
        }
        #endregion

        //private void SetDtCol()
        //{
        //    _DtLayerCharging = new DataTable();
        //    _DtLayerCharging.Columns.Add(new DataColumn("VESSELID"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("BATCHNO"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("OPSGNAME"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("MTRLID"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("MTRLNAME"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("MLOTID"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("MSUBLOTQTY"));
        //    _DtLayerCharging.Columns.Add(new DataColumn("UOMNAME"));
        //}

        private async Task RetrieveMaterialSublotWithVessel()
        {
            try
            {
                if (!string.IsNullOrEmpty(lblEqptID))
                {
                    // 적재된 원료 조회
                    BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATAs.Clear();
                    BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Clear();
                    BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATAs.Add(new BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATA()
                    {
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        MSUBLOTID = null,
                        MSUBLOTBCD = null,
                        VESSELID = lblEqptID
                    });

                    await BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.Execute();

                    // 소분된 원료 조회
                    BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.INDATAs.Clear();
                    BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.OUTDATAs.Clear();
                    BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.INDATAs.Add(new BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.INDATA()
                    {
                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                        OPSGGUID = null,
                        MTRLID = null,
                        CHGSEQ = 0,
                        OPSGNAME = _mainWnd.CurrentInstruction.Raw.TARGETVAL,
                        VESSELID = lblEqptID
                    });

                    await BR_BRS_GET_DispenseSubLot_VESSID_ISNULL.Execute();
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        #region [UserDefine]
        public class LayerCharging : ViewModelBase
        {
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

            private string _BATCHNO;
            public string BATCHNO
            {
                get { return _BATCHNO; }
                set
                {
                    _BATCHNO = value;
                    OnPropertyChanged("BATCHNO");
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

            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    OnPropertyChanged("MTRLID");
                }
            }

            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    OnPropertyChanged("MTRLNAME");
                }
            }
            private string _TSTREQNO;
            public string TSTREQNO
            {
                get { return _TSTREQNO; }
                set
                {
                    _TSTREQNO = value;
                    OnPropertyChanged("TSTREQNO");
                }
            }
            
            private string _MSUBLOTQTY;
            public string MSUBLOTQTY
            {
                get { return _MSUBLOTQTY; }
                set
                {
                    _MSUBLOTQTY = value;
                    OnPropertyChanged("MSUBLOTQTY");
                }
            }

            private string _UOMNAME;
            public string UOMNAME
            {
                get { return _UOMNAME; }
                set
                {
                    _UOMNAME = value;
                    OnPropertyChanged("UOMNAME");
                }
            }

            public string MSUBLOTQTYSTR
            {
                get
                {
                    if (_MSUBLOTQTY != null && _UOMNAME != null)
                    {
                        return string.Format("{0} {1}", _MSUBLOTQTY, _UOMNAME);
                    }
                    else
                        return "N/A";
                }
            }
        }
        #endregion
    }
}