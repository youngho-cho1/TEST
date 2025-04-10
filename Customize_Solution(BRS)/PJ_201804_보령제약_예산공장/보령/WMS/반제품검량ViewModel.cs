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
using System.Collections.ObjectModel;
using System.Linq;
using C1.Silverlight.Data;
using System.Text;

namespace 보령
{
    public class 반제품검량ViewModel : ViewModelBase
    {
        #region [Property]
        public 반제품검량ViewModel()
        {
            _ListRequestOut = new ObservableCollection<IBCInfo>();
            _BR_BRS_GET_VESSEL_INFO_DETAIL = new BR_BRS_GET_VESSEL_INFO_DETAIL();
            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _BR_PHR_UPD_MaterialSubLot_CheckWeight = new BR_PHR_UPD_MaterialSubLot_CheckWeight();
        }

        private 반제품검량 _mainWnd;
        private string scaleid;

        private ObservableCollection<IBCInfo> _ListRequestOut;
        public ObservableCollection<IBCInfo> ListRequestOut
        {
            get { return _ListRequestOut; }
            set
            {
                _ListRequestOut = value;
                OnPropertyChanged("ListRequestOut");
            }
        }

        #endregion
        #region [Bizrule]
        // 작업장 저울 조회
        private BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo;
        public BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo
        {
            get { return _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo; }
            set
            {
                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = value;
                OnPropertyChanged("BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo");
            }
        }
        // 용기(반제품) 정보 조회
        private BR_BRS_GET_VESSEL_INFO_DETAIL _BR_BRS_GET_VESSEL_INFO_DETAIL;
        public BR_BRS_GET_VESSEL_INFO_DETAIL BR_BRS_GET_VESSEL_INFO_DETAIL
        {
            get { return _BR_BRS_GET_VESSEL_INFO_DETAIL; }
            set
            {
                _BR_BRS_GET_VESSEL_INFO_DETAIL = value;
                OnPropertyChanged("BR_BRS_GET_VESSEL_INFO_DETAIL");
            }
        }
        // 검량 확인
        private BR_PHR_UPD_MaterialSubLot_CheckWeight _BR_PHR_UPD_MaterialSubLot_CheckWeight;
        public BR_PHR_UPD_MaterialSubLot_CheckWeight BR_PHR_UPD_MaterialSubLot_CheckWeight
        {
            get { return _BR_PHR_UPD_MaterialSubLot_CheckWeight; }
            set
            {
                _BR_PHR_UPD_MaterialSubLot_CheckWeight = value;
                OnPropertyChanged("BR_PHR_UPD_MaterialSubLot_CheckWeight");
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
                            IsBusy = true;

                            if (arg != null && arg is 반제품검량)
                            {
                                _mainWnd = arg as 반제품검량;

                                // 반제품출고 기록 확인
                                var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                                if(inputValues.Count > 0)
                                {
                                    var inputValue = inputValues[0];
                                    if(inputValue.Raw.NOTE != null)
                                    {
                                        DataSet ds = new DataSet();
                                        DataTable dt = new DataTable();
                                        var bytearray = inputValue.Raw.NOTE;
                                        string xml = Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
                                        
                                        ds.ReadXmlFromString(xml);
                                        dt = ds.Tables["DATA"];

                                        foreach (var row in dt.Rows)
                                        {
                                            if (row["용기번호"] != null)
                                            {
                                                //Pallet에 적재된 반제품을 조회(소용량라인 용)
                                                //MaterialSubLotCustomAttribute.MTATID = 'LEADED_MSUBLOTID'에 값이 있는 경우
                                                var subLotBiz = new BR_BRS_SEL_MaterialSubLot_LoadOnPallet_BYVESSELID();
                                                subLotBiz.INDATAs.Add(new BR_BRS_SEL_MaterialSubLot_LoadOnPallet_BYVESSELID.INDATA()
                                                {
                                                    VESSELID = row["용기번호"] != null ? row["용기번호"].ToString() : "",
                                                });

                                                if (await subLotBiz.Execute() == true)
                                                {
                                                    if (subLotBiz.OUTDATAs.Count > 0)
                                                    {
                                                        //Pallet에 적재된 반제품 존재하면 적재된 반제품을 표시 함
                                                        foreach (var itm in subLotBiz.OUTDATAs)
                                                        {
                                                            ListRequestOut.Add(new IBCInfo
                                                            {
                                                                CHK = true,
                                                                VESSELID = itm.VESSELID,
                                                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                                                OPSGNAME = itm.OPSGNAME,
                                                                OUTPUTID = "",
                                                                OUTPUTGUID = "",
                                                                STATUS = "무게측정필요",
                                                                Weight = 0m
                                                            });
                                                        }

                                                        continue;
                                                    }
                                                }
                                            }

                                            // Pallet에 적재된 반제품이 없으면 기존 데이터 Add
                                            ListRequestOut.Add(new IBCInfo
                                            {
                                                CHK = true,
                                                VESSELID = row["용기번호"] != null ? row["용기번호"].ToString() : "",
                                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                                OPSGNAME = row["공정명"] != null ? row["공정명"].ToString() : "",
                                                OUTPUTID = "",
                                                OUTPUTGUID = "",
                                                STATUS = "무게측정필요",
                                                Weight = 0m
                                            });
                                        }
                                    }
                                }

                                // 작업장 저울 조회
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Clear();
                                BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Clear();
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATA
                                {
                                    EQPTID = AuthRepositoryViewModel.Instance.RoomID
                                });

                                if (await _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.Execute() && _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    scaleid = "";
                                    for (int idx = 0; idx < _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count; idx++)
                                    {
                                        if (_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[idx].MODEL.Contains("IFS4"))
                                            scaleid = _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[idx].EQPTID;
                                    }
                                }
                                _mainWnd.txtSelContainer.Focus();
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
        public ICommand SelectedChangedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelectedChangedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandCanExecutes["SelectedChangedCommandAsync"] = false;
                            CommandResults["SelectedChangedCommandAsync"] = false;

                            ///
                            IsBusy = true;

                            // 
                            if (_mainWnd.GridRequestOutList.SelectedItem != null)
                            {
                                if ((_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).STATUS.Equals("무게측정필요"))
                                {
                                    var popup = new 반제품출고Popup
                                    {
                                        ScaleId = scaleid,
                                        VesselId = _mainWnd.txtSelContainer.Text.ToUpper()
                                    };

                                    popup.Closed += (s, e) =>
                                    {
                                        if (popup.DialogResult == true)
                                        {
                                            (_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).STATUS = "완료";

                                            if (popup.curType == 반제품출고Popup.VesselType.IBC)
                                            {
                                                if (popup.BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count > 0)
                                                {
                                                    (_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).WIPs = popup.BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs;
                                                    (_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).DETAILs = null;
                                                }
                                            }
                                            else
                                            {
                                                if (popup.BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count > 0)
                                                {
                                                    (_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).WIPs = null;
                                                    (_mainWnd.GridRequestOutList.SelectedItem as IBCInfo).DETAILs = popup.BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs;
                                                }
                                            }
                                        }
                                        else
                                            _mainWnd.GridRequestOutList.SelectedItem = null;
                                    };
                                    popup.Show();
                                }
                            }

                            IsBusy = false;
                            ///

                            CommandResults["SelectedChangedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _mainWnd.GridRequestOutList.SelectedItem = null;
                            CommandResults["SelectedChangedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            _mainWnd.txtSelContainer.Text = "";
                            _mainWnd.txtSelContainer.Focus();
                            CommandCanExecutes["SelectedChangedCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelectedChangedCommandAsync") ?
                        CommandCanExecutes["SelectedChangedCommandAsync"] : (CommandCanExecutes["SelectedChangedCommandAsync"] = true);
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

                            int compCount = 0;
                            ObservableCollection<IBCInfo> compRequestOut = new ObservableCollection<IBCInfo>();

                            foreach (var item in _ListRequestOut)
                            {
                                if(item.STATUS == "완료")
                                {
                                    compRequestOut.Add(item);
                                    compCount++;
                                }
                            }

                            if (compCount != _ListRequestOut.Count && compCount != 0)
                            {
                                C1.Silverlight.C1MessageBox.Show("처리완료 되지 않은 용기가 있습니다.\n 계속 진행하시겠습니까?", "", C1.Silverlight.C1MessageBoxButton.OKCancel, async SEL =>
                                {
                                    if (SEL == MessageBoxResult.Cancel)
                                    {
                                        IsBusy = false;
                                    }
                                    else
                                    {
                                        var authHelper = new iPharmAuthCommandHelper();

                                        if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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

                                        // UPD Material State
                                        authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                        if (await authHelper.ClickAsync(
                                            Common.enumCertificationType.Function,
                                            Common.enumAccessType.Create,
                                            "반제품검량",
                                            "반제품검량",
                                            false,
                                            "OM_ProductionOrder_SUI",
                                            "", null, null) == false)
                                        {
                                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                        }

                                        _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Clear();
                                        _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Clear();

                                        foreach (var Vessel in compRequestOut)
                                        {
                                            if (Vessel.WIPs != null)
                                            {
                                                foreach (var item in Vessel.WIPs)
                                                {
                                                    _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                                    {
                                                        MLOTID = item.MLOTID != null ? item.MLOTID : "",
                                                        MLOTVER = item.MLOTVER != null ? item.MLOTVER : 1m,
                                                        INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                                    });

                                                    _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                                    {
                                                        MSUBLOTID = item.MSUBLOTID != null ? item.MSUBLOTID : "",
                                                        MSUBLOTVER = item.MSUBLOTVER != null ? item.MSUBLOTVER : 1m,
                                                        MSUBLOTSTAT = item.MSUBLOTSTAT != null ? item.MSUBLOTSTAT : ""
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                foreach (var item in Vessel.DETAILs)
                                                {
                                                    _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                                    {
                                                        MLOTID = item.MLOTID != null ? item.MLOTID : "",
                                                        MLOTVER = item.MLOTVER != null ? item.MLOTVER : 1m,
                                                        INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                                    });

                                                    _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                                    {
                                                        MSUBLOTID = item.MSUBLOTID != null ? item.MSUBLOTID : "",
                                                        MSUBLOTVER = item.MSUBLOTVER != null ? item.MSUBLOTVER : 1m,
                                                        MSUBLOTSTAT = item.MSUBLOTSTAT != null ? item.MSUBLOTSTAT : ""
                                                    });
                                                }
                                            }
                                        }

                                        if (await _BR_PHR_UPD_MaterialSubLot_CheckWeight.Execute())
                                        {
                                            DataSet ds = new DataSet();
                                            DataTable dt = new DataTable("DATA");
                                            ds.Tables.Add(dt);

                                            dt.Columns.Add(new DataColumn("관리번호"));
                                            dt.Columns.Add(new DataColumn("저울번호"));
                                            dt.Columns.Add(new DataColumn("무게"));
                                            dt.Columns.Add(new DataColumn("상한"));
                                            dt.Columns.Add(new DataColumn("기준"));
                                            dt.Columns.Add(new DataColumn("하한"));

                                            if (_ListRequestOut.Count(x => x.STATUS.Equals("완료")) > 0)
                                            {
                                                foreach (var Vessel in _ListRequestOut.Where(x => x.STATUS.Equals("완료")))
                                                {
                                                    var row = dt.NewRow();

                                                    row["관리번호"] = Vessel.VESSELID != null ? Vessel.VESSELID : "";
                                                    row["저울번호"] = scaleid != null ? scaleid : "";
                                                    row["무게"] = Vessel.WIPs[0].REALQTY.HasValue ? Vessel.WIPs[0].REALQTY.Value.ToString() : "";
                                                    row["상한"] = Vessel.WIPs[0].TOTALQTY_UPPER.HasValue ? Vessel.WIPs[0].TOTALQTY_UPPER.Value.ToString() : "";
                                                    row["기준"] = Vessel.WIPs[0].TOTALQTY.HasValue ? Vessel.WIPs[0].TOTALQTY.Value.ToString() : "";
                                                    row["하한"] = Vessel.WIPs[0].TOTALQTY_LOWER.HasValue ? Vessel.WIPs[0].TOTALQTY_LOWER.Value.ToString() : "";

                                                    dt.Rows.Add(row);
                                                }
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

                                            IsBusy = false;

                                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                                        }
                                    }
                                });
                            }
                            else if(compCount == 0)
                            {
                                IsBusy = false;

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            else
                            {
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
                                    "반제품검량",
                                    "반제품검량",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // UPD Tare Weight

                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Clear();
                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Clear();

                                foreach (var Vessel in compRequestOut)
                                {
                                    if (Vessel.WIPs != null)
                                    {
                                        foreach (var item in Vessel.WIPs)
                                        {
                                            _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                            {
                                                MLOTID = item.MLOTID != null ? item.MLOTID : "",
                                                MLOTVER = item.MLOTVER != null ? item.MLOTVER : 1m,
                                                INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                            });

                                            _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                            {
                                                MSUBLOTID = item.MSUBLOTID != null ? item.MSUBLOTID : "",
                                                MSUBLOTVER = item.MSUBLOTVER != null ? item.MSUBLOTVER : 1m,
                                                MSUBLOTSTAT = item.MSUBLOTSTAT != null ? item.MSUBLOTSTAT : ""
                                            });
                                        }
                                    }
                                    else
                                    {
                                        foreach (var item in Vessel.DETAILs)
                                        {
                                            _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                            {
                                                MLOTID = item.MLOTID != null ? item.MLOTID : "",
                                                MLOTVER = item.MLOTVER != null ? item.MLOTVER : 1m,
                                                INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI")
                                            });

                                            _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                            {
                                                MSUBLOTID = item.MSUBLOTID != null ? item.MSUBLOTID : "",
                                                MSUBLOTVER = item.MSUBLOTVER != null ? item.MSUBLOTVER : 1m,
                                                MSUBLOTSTAT = item.MSUBLOTSTAT != null ? item.MSUBLOTSTAT : ""
                                            });
                                        }
                                    }
                                }

                                if (await _BR_PHR_UPD_MaterialSubLot_CheckWeight.Execute())
                                {
                                    DataSet ds = new DataSet();
                                    DataTable dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);

                                    dt.Columns.Add(new DataColumn("관리번호"));
                                    dt.Columns.Add(new DataColumn("저울번호"));
                                    dt.Columns.Add(new DataColumn("무게"));
                                    dt.Columns.Add(new DataColumn("상한"));
                                    dt.Columns.Add(new DataColumn("기준"));
                                    dt.Columns.Add(new DataColumn("하한"));

                                    if (_ListRequestOut.Count(x => x.STATUS.Equals("완료")) > 0)
                                    {
                                        foreach (var Vessel in _ListRequestOut.Where(x => x.STATUS.Equals("완료")))
                                        {
                                            var row = dt.NewRow();

                                            row["관리번호"] = Vessel.VESSELID != null ? Vessel.VESSELID : "";
                                            row["저울번호"] = scaleid != null ? scaleid : "";
                                            row["무게"] = Vessel.WIPs[0].REALQTY.HasValue ? Vessel.WIPs[0].REALQTY.Value.ToString() : "";
                                            row["상한"] = Vessel.WIPs[0].TOTALQTY_UPPER.HasValue ? Vessel.WIPs[0].TOTALQTY_UPPER.Value.ToString() : "";
                                            row["기준"] = Vessel.WIPs[0].TOTALQTY.HasValue ? Vessel.WIPs[0].TOTALQTY.Value.ToString() : "";
                                            row["하한"] = Vessel.WIPs[0].TOTALQTY_LOWER.HasValue ? Vessel.WIPs[0].TOTALQTY_LOWER.Value.ToString() : "";

                                            dt.Rows.Add(row);
                                        }
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
                                else
                                    IsBusy = false;
                            }
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
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
        #region [User Define]
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

        public IBCInfo ChangeTargetIBC(string target)
        {
            foreach (var item in ListRequestOut)
            {
                if (item.VESSELID.ToUpper().Equals(target))
                    return item;
            }

            return null;
        }
        #endregion
    }
}
