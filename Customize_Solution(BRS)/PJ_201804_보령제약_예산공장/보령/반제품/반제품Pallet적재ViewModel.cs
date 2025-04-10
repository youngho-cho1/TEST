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
    public class 반제품Pallet적재ViewModel : ViewModelBase
    {
        #region Property
        private 반제품Pallet적재 _mainWnd;

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

        private BR_PHR_GET_BIN_INFO _BR_PHR_GET_BIN_INFO;
        public BR_PHR_GET_BIN_INFO BR_PHR_GET_BIN_INFO
        {
            get { return _BR_PHR_GET_BIN_INFO; }
            set
            {
                _BR_PHR_GET_BIN_INFO = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_SEL_ProductionOrderOutput_LoadedPallet _LoadedPalletMaterialSubLot;
        public BR_BRS_SEL_ProductionOrderOutput_LoadedPallet LoadedPalletMaterialSubLot
        {
            get { return _LoadedPalletMaterialSubLot; }
            set
            {
                _LoadedPalletMaterialSubLot = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_SEL_ProductionOrderOutput_LoadedPallet _NonLoadedPalletMaterialSubLot;
        public BR_BRS_SEL_ProductionOrderOutput_LoadedPallet NonLoadedPalletMaterialSubLot
        {
            get { return _NonLoadedPalletMaterialSubLot; }
            set
            {
                _NonLoadedPalletMaterialSubLot = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Instructor

        public 반제품Pallet적재ViewModel()
        {
            _BR_PHR_GET_BIN_INFO = new BR_PHR_GET_BIN_INFO();
            _LoadedPalletMaterialSubLot = new BR_BRS_SEL_ProductionOrderOutput_LoadedPallet();
            _NonLoadedPalletMaterialSubLot = new BR_BRS_SEL_ProductionOrderOutput_LoadedPallet();

            _BR_PHR_SEL_System_Option_OPTIONTYPE = new BR_PHR_SEL_System_Option_OPTIONTYPE();
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
                            if (arg != null && arg is 반제품Pallet적재)
                            {
                                _mainWnd = arg as 반제품Pallet적재;

                                #region Campaign Order
                                OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                                CanSelectOrder = OrderList.Count > 0 ? true : false;
                                #endregion

                                _mainWnd.txtContainer.Focus();
                            }

                            HeaderTextComponentName = string.Concat(_mainWnd.CurrentOrder.ProductionOrderID, " / ", _mainWnd.CurrentOrder.BatchNo, " / ", _mainWnd.CurrentOrder.MaterialID, " / ", _mainWnd.CurrentOrder.MaterialName);
                            ///

                            // 공정 MaterialSublot 조회
                            await RetrieveMaterialSublotWithPallet();

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

                            BR_PHR_GET_BIN_INFO.INDATAs.Clear();
                            BR_PHR_GET_BIN_INFO.OUTDATAs.Clear();

                            BR_PHR_GET_BIN_INFO.INDATAs.Add(new BR_PHR_GET_BIN_INFO.INDATA()
                            {
                                LANGID = null,
                                EQPTID = _mainWnd.txtContainer.Text,
                                POID = null,
                                BATCHNO = null,
                                TYPE = null
                            });

                            lblEqptID = null;

                            if (!await BR_PHR_GET_BIN_INFO.Execute()) return;

                            if (BR_PHR_GET_BIN_INFO.OUTDATAs.Count > 0)
                                lblEqptID = BR_PHR_GET_BIN_INFO.OUTDATAs[0].EQPTID;

                            /// 다른 오더 또는 공정 사용여부 확인
                            var subLotBiz = new BR_BRS_SEL_MaterialSubLot_LoadOnPallet_BYVESSELID();
                            subLotBiz.INDATAs.Add(new BR_BRS_SEL_MaterialSubLot_LoadOnPallet_BYVESSELID.INDATA()
                            {
                                VESSELID = lblEqptID,
                            });

                            if (await subLotBiz.Execute() == true)
                            {
                                string poid = _mainWnd.CurrentOrder.ProductionOrderID;
                                string opsgguid = _mainWnd.CurrentOrder.OrderProcessSegmentID;

                                var loaded = subLotBiz.OUTDATAs.FirstOrDefault(o => o.POID != poid || o.OPSGGUID != opsgguid);

                                if (loaded != null)
                                {
                                    _mainWnd.txtContainer.Text = String.Empty;
                                    this.lblEqptID = String.Empty;
                                    _mainWnd.txtMaterial.Focus();

                                    throw new Exception(string.Format("생산오더[{0}] 공정[{1}]에서 사용 중인 파렛트입니다!", loaded.POID, loaded.OPSGNAME));
                                }
                            }

                            /// 제조회
                            await RetrieveMaterialSublotWithPallet();

                            _mainWnd.txtMaterial.Focus();

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
                            foreach (var outdata in _NonLoadedPalletMaterialSubLot.OUTDATAs)
                            {
                                if (outdata.VESSELID.ToUpper().Equals(_mainWnd.txtMaterial.Text))
                                {
                                    var isChecked = _NonLoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.SEL == "Y" && o.VESSELID.ToUpper().Equals(_mainWnd.txtMaterial.Text)).FirstOrDefault();

                                    if (isChecked != null)
                                    {
                                        throw new Exception(string.Format("이미 선택된 반제품입니다."));
                                    }

                                    bSeach = true;
                                    outdata.SEL = "Y";
                                }
                            }

                            if (bSeach == false)
                            {
                                // 적재취소 선택
                                var isChecked = _LoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.VESSELID.ToUpper().Equals(_mainWnd.txtMaterial.Text)).FirstOrDefault();

                                if (isChecked != null)
                                {
                                    isChecked.SEL = "Y";
                                }
                                else
                                {
                                    throw new Exception(string.Format("해당 반제품이 리스트에 없습니다."));
                                }
                            }
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
                            if (lblEqptID == null || lblEqptID.Equals(""))
                            {
                                throw new Exception(string.Format("적재 파렛트번호가 없습니다."));
                            }

                            if (_NonLoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.SEL == "Y").Count() == 0)
                            {
                                throw new Exception(string.Format("적재 할 대상이 선택되지 않았습니다."));
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

                            var loadBiz = new BR_BRS_REG_MaterialSubLot_LoadOnPallet();

                            loadBiz.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_LoadOnPallet.INDATA() {
                                POID = _NonLoadedPalletMaterialSubLot.OUTDATAs[0].POID,
                                OPSGGUID = _NonLoadedPalletMaterialSubLot.OUTDATAs[0].OPSGGUID,
                                INDUSER = operatorID,
                                VESSELID = lblEqptID,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                REASON = "반제품 적재",
                                ISCANCEL = "N",
                            });                          

                            foreach (var outdata in _NonLoadedPalletMaterialSubLot.OUTDATAs.Where(o=>o.SEL.Equals("Y")))
                            {
                                loadBiz.INDATA_MSUBLOTs.Add(new BR_BRS_REG_MaterialSubLot_LoadOnPallet.INDATA_MSUBLOT()
                                {
                                    MTRLID = outdata.MTRLID,
                                    MLOTID = outdata.MLOTID,
                                    MLOTVER = outdata.MLOTVER,
                                    MSUBLOTID = outdata.MSUBLOTID,
                                    MSUBLOTBCD = outdata.MSUBLOTBCD,
                                    MSUBLOTSEQ = outdata.MSUBLOTSEQ,
                                    MSUBLOTVER = outdata.MSUBLOTVER,                                    
                                });
                            }

                            if (!await loadBiz.Execute()) return;

                            // 재조회
                            await RetrieveMaterialSublotWithPallet();

                            // Container initial
                            _mainWnd.txtContainer.Text = string.Empty;
                            this.lblEqptID = string.Empty;

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
                            if (lblEqptID == null || lblEqptID.Equals(""))
                            {
                                throw new Exception(string.Format("적재 파렛트번호가 없습니다."));
                            }

                            if (_LoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.SEL == "Y").Count() == 0)
                            {
                                throw new Exception(string.Format("취소 할 대상이 선택되지 않았습니다."));
                            }

                            if (_LoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.SEL == "Y" && o.LOADED_VESSELID.Equals(this.lblEqptID)).Count() == 0)
                            {
                                throw new Exception(string.Format("입력한 파렛트에 적재된 반제이 없습니다!\n파렛트번호를 확인하십시요."));
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

                            var loadBiz = new BR_BRS_REG_MaterialSubLot_LoadOnPallet();
                            loadBiz.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_LoadOnPallet.INDATA()
                            {
                                POID = _LoadedPalletMaterialSubLot.OUTDATAs[0].POID,
                                OPSGGUID = _LoadedPalletMaterialSubLot.OUTDATAs[0].OPSGGUID,
                                INDUSER = operatorID,
                                VESSELID = lblEqptID,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                REASON = "반제품 적재 취소",
                                ISCANCEL = "Y",
                            });

                            foreach (var outdata in _LoadedPalletMaterialSubLot.OUTDATAs.Where(o => o.SEL.Equals("Y")))
                            {
                                loadBiz.INDATA_MSUBLOTs.Add(new BR_BRS_REG_MaterialSubLot_LoadOnPallet.INDATA_MSUBLOT()
                                {
                                    MTRLID = outdata.MTRLID,
                                    MLOTID = outdata.MLOTID,
                                    MLOTVER = outdata.MLOTVER,
                                    MSUBLOTID = outdata.MSUBLOTID,
                                    MSUBLOTBCD = outdata.MSUBLOTBCD,
                                    MSUBLOTSEQ = outdata.MSUBLOTSEQ,
                                    MSUBLOTVER = outdata.MSUBLOTVER,
                                });
                            }

                            if (!await loadBiz.Execute()) return;

                            // 재조회
                            await RetrieveMaterialSublotWithPallet();

                            // Container initial
                            _mainWnd.txtContainer.Text = string.Empty;
                            this.lblEqptID = string.Empty;

                            ///
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

                            ///

                            ///

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

                            ////ListContainer.Clear();

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

                            if (_LoadedPalletMaterialSubLot.OUTDATAs.Count == 0)
                            {
                                OnMessage("적재된 대상이 없습니다.");
                            }
                            else
                            {
                                DataSet ds = new DataSet();
                                DataTable dt = new DataTable("DATA");
                                ds.Tables.Add(dt);
                                
                                dt.Columns.Add(new DataColumn("Pallet번호"));
                                dt.Columns.Add(new DataColumn("용기번호"));
                                dt.Columns.Add(new DataColumn("무게"));
                                dt.Columns.Add(new DataColumn("단위"));

                                foreach (var item in _LoadedPalletMaterialSubLot.OUTDATAs)
                                {
                                    var row = dt.NewRow();

                                    row["Pallet번호"] = item.LOADED_VESSELID ?? "";
                                    row["용기번호"] = item.VESSELID ?? "";
                                    row["무게"] = Convert.ToString(item.MSUBLOTQTY) ?? "";
                                    row["단위"] = item.UOMNAME ?? "";
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

        private async Task RetrieveMaterialSublotWithPallet()
        {
            try
            {
                //기 로드된 MaterialSubLot 조회
                _LoadedPalletMaterialSubLot.INDATAs.Clear();
                _LoadedPalletMaterialSubLot.OUTDATAs.Clear();
                _LoadedPalletMaterialSubLot.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_LoadedPallet.INDATA()
                {
                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                    ISLOADED = "Y",
                    EXCEPT_ZERO = "Y"
                });

                await _LoadedPalletMaterialSubLot.Execute();

                //로드되지않은 MaterialSubLot 조회
                _NonLoadedPalletMaterialSubLot.INDATAs.Clear();
                _NonLoadedPalletMaterialSubLot.OUTDATAs.Clear();
                _NonLoadedPalletMaterialSubLot.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_LoadedPallet.INDATA()
                {
                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                    ISLOADED = "N",
                    EXCEPT_ZERO = "Y"
                });

                await _NonLoadedPalletMaterialSubLot.Execute();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
    }
}