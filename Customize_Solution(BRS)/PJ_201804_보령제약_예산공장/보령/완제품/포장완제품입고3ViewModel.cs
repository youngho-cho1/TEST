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
    public class 포장완제품입고3ViewModel : ViewModelBase
    {
        #region [Property]
        public 포장완제품입고3ViewModel()
        {
            _LotCollection = new BufferedObservableCollection<PaperTubeLot>();

            _BR_BRS_SEL_ProductionOrder_FERT_Summary = new BR_BRS_SEL_ProductionOrder_FERT_Summary();
            _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE = new BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE();
            _BR_BRS_GET_SCANINFO_PALLET_IN = new BR_BRS_GET_SCANINFO_PALLET_IN();
            _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL = new BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL();
        }
        private 포장완제품입고3 _mainWnd;

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

        #region StorageIn
        private int _OUTPUTQTY;
        public int OUTPUTQTY
        {
            get { return _OUTPUTQTY; }
            set
            {
                _OUTPUTQTY = value;
                OnPropertyChanged("OUTPUTQTY");
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
        #endregion

        #region WMS Input Summary
        private string _storageInUom;
        private decimal _convertParam;
        private int _PRODQTY;
        public string PRODQTY
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageInUom))
                    return _PRODQTY.ToString();
                else
                    return _PRODQTY.ToString() + _storageInUom;
            }
            set
            {
                int chk;
                if (int.TryParse(value, out chk))
                    _PRODQTY = chk;
                else
                    _PRODQTY = 0;

                OnPropertyChanged("PRODQTY");
            }
        }
        private int _STORAGEINQTY;
        public string STORAGEINQTY
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageInUom))
                    return _STORAGEINQTY.ToString();
                else
                    return _STORAGEINQTY.ToString() + _storageInUom;
            }
            set
            {
                int chk;
                if (int.TryParse(value, out chk))
                    _STORAGEINQTY = chk;
                else
                    _STORAGEINQTY = 0;

                OnPropertyChanged("STORAGEINQTY");
            }
        }
        private int _REMAINQTY;
        public string REMAINQTY
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_storageInUom))
                    return _REMAINQTY.ToString();
                else
                    return _REMAINQTY.ToString() + _storageInUom;
            }
            set
            {
                int chk;
                if (int.TryParse(value, out chk))
                    _REMAINQTY = chk;
                else
                    _REMAINQTY = 0;

                OnPropertyChanged("REMAINQTY");
            }
        }
        private BufferedObservableCollection<PaperTubeLot> _LotCollection;
        public BufferedObservableCollection<PaperTubeLot> LotCollection
        {
            get { return _LotCollection; }
            set
            {
                _LotCollection = value;
                OnPropertyChanged("LotCollection");
            }
        }
        #endregion
        #endregion

        #region[Bizlue]
        private BR_BRS_SEL_ProductionOrder_FERT_Summary _BR_BRS_SEL_ProductionOrder_FERT_Summary;
        private BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE;
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

        private BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL;       
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
                                
                                if (arg != null && arg is 포장완제품입고3)
                                {
                                    _mainWnd = arg as 포장완제품입고3;

                                    // WMS 입고실적 조회
                                    _BR_BRS_SEL_ProductionOrder_FERT_Summary.INDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrder_FERT_Summary.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_FERT_Summary.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                    });

                                    if(await _BR_BRS_SEL_ProductionOrder_FERT_Summary.Execute() && _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs.Count > 0)
                                    {
                                        _storageInUom = _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].UOM;
                                        _convertParam = _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].CONVERTPARAM.GetValueOrDefault();
                                        PRODQTY = _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].PRODQTY.GetValueOrDefault().ToString();
                                        STORAGEINQTY = _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].STORAGEINQTY.GetValueOrDefault().ToString();
                                        REMAINQTY = (_BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].PRODQTY.GetValueOrDefault()
                                                     - _BR_BRS_SEL_ProductionOrder_FERT_Summary.OUTDATAs[0].STORAGEINQTY.GetValueOrDefault()).ToString();

                                        if(_PRODQTY <= 0)
                                            throw new Exception("생산량이 0보다 작을 수 없습니다.");
                                        else if (_convertParam <= 0)
                                            throw new Exception("평균질량 조회결과가 음수 입니다.");

                                    }

                                    // 현재 공정의 공정중제품(투입공정이 설정된 WIP을 조회)
                                    _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.INDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.OUTDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                    });
                                    if(await _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.Execute())
                                    {
                                        _LotCollection.Clear();

                                        foreach (var item in _BR_BRS_SEL_ProductionOrderOutput_PAPERTUBE.OUTDATAs)
                                        {
                                            _LotCollection.Add(new PaperTubeLot
                                            {
                                                MsubLotId = item.MSUBLOTID,
                                                Barcode = item.MSUBLOTBCD,
                                                VesselId = item.VESSELID,
                                                Uom = item.NOTATION,
                                                Precision = Convert.ToInt32(item.PRECISION.GetValueOrDefault()),
                                                NetWeight = item.MSUBLOTQTY.GetValueOrDefault(),
                                                TareWeight = item.TAREWEIGHT.GetValueOrDefault(),
                                                chargingYN = false,
                                                storageInYN = item.MSUBLOTQTY.GetValueOrDefault() <= 0 ? true : false
                                            });
                                        }
                                        OnPropertyChanged("LotCollection");
                                    }
                                }
                                else
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
                                    // 스캔한 데이터
                                    var temparm = (arg as TextBox).Text;

                                    // 지관 공정중제품 리스트 확인
                                    foreach (var item in LotCollection)
                                    {
                                        if(item.Barcode == temparm && item.State == "미입고")
                                        {
                                            item.chargingYN = true;
                                            ChargePaperTube(item);
                                            ScanData = "";
                                            return;
                                        }
                                    }

                                    // 입출고대, 물류팀 파레트 여부 확인
                                    _BR_BRS_GET_SCANINFO_PALLET_IN.INDATAs.Clear();
                                    _BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs.Clear();

                                    _BR_BRS_GET_SCANINFO_PALLET_IN.INDATAs.Add(new BR_BRS_GET_SCANINFO_PALLET_IN.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPGSGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        SCANDATA = temparm
                                    });

                                    if (await _BR_BRS_GET_SCANINFO_PALLET_IN.Execute())
                                    {
                                        if (_BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs.Count > 0)
                                        {
                                            foreach (var item in _BR_BRS_GET_SCANINFO_PALLET_IN.OUTDATAs)
                                            {
                                                if (item.SCANDATATYPE == "PALLET")
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

                                            if (OUTPUTQTY > 0 && !(string.IsNullOrEmpty(txtPallet)) && !(string.IsNullOrEmpty(txtIN_LOCATION)))
                                                isEbConfirm = true;
                                        }
                                    }
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

                                // 잔량과 입고량이 동일한데 미입고 상태인 Lot이 있는 경우 에러 메세지 출력
                                if(_REMAINQTY == _OUTPUTQTY)
                                {
                                    foreach (var item in _LotCollection)
                                    {
                                        if (item.State == "미입고")
                                            throw new Exception("미입고 상태인 반제품이 있습니다.");
                                    }
                                }

                                var authHelper = new iPharmAuthCommandHelper();

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

                                _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATAs.Clear();
                                _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATAs.Add(new BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATA
                                {
                                    OUTPUTQTY = _OUTPUTQTY,
                                    VESSELID = txtPallet,
                                    IN_LOCATION = txtIN_LOCATION,
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut"),
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID
                                });
                                _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATA_MSUBLOTs.Clear();
                                foreach (var item in _LotCollection)
                                {
                                    if(item.State == "입고대기")
                                    {
                                        _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATA_MSUBLOTs.Add(new BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.INDATA_MSUBLOT
                                        {
                                            MSUBLOTID = item.MsubLotId,
                                            MSUBLOTBCD = item.Barcode,
                                            MSUBLOTQTY = item.NetWeight
                                        });
                                    }
                                }

                                if (await _BR_BRS_REG_WMS_PACK_PALLET_IN_MANUAL.Execute())
                                {

                                    var ds = new DataSet();
                                    var dt = new DataTable("DATA");
                                    ds.Tables.Add(dt);

                                    dt.Columns.Add(new DataColumn("포장실적"));
                                    dt.Columns.Add(new DataColumn("Pallet"));
                                    dt.Columns.Add(new DataColumn("작업대"));


                                    var row = dt.NewRow();
                                    row["포장실적"] = _OUTPUTQTY.ToString();
                                    row["Pallet"] = txtPallet != null ? txtPallet : "";
                                    row["작업대"] = txtIN_LOCATION != null ? txtIN_LOCATION : "";

                                    dt.Rows.Add(row);

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

                            // 스캔데이터 초기화
                            OUTPUTQTY = 0;
                            txtPallet = "";
                            txtIN_LOCATION = "";

                            // 입고대기 Lot 초기화
                            foreach (var item in LotCollection)
                                item.chargingYN = false;

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

        #region Custom
        public class PaperTubeLot : Container
        {
            private bool _storageInYN;
            public bool storageInYN
            {
                get { return _storageInYN; }
                set
                {
                    _storageInYN = value;
                    OnPropertyChanged("storageInYN");
                    OnPropertyChanged("State");
                }
            }
            private bool _chargingYN;
            public bool chargingYN
            {
                get { return _chargingYN; }
                set
                {
                    _chargingYN = value;
                    OnPropertyChanged("chargingYN");
                    OnPropertyChanged("State");
                }
            }
            public string State
            {
                get
                {
                    if (_storageInYN)
                        return "입고완료";
                    else if (!_storageInYN && _chargingYN)
                        return "입고대기";
                    else
                        return "미입고";
                }
            }
        }
        private void ChargePaperTube(PaperTubeLot tar)
        {
            try
            {
                bool isLastPallet = true;
                //마지막 팔레트 여부확인(조회된 지관 공정중제품 목록에 상태가 전부 입고완료, 입고대기인 경우 마지막팔레트로 판단
                foreach (var item in _LotCollection)
                {
                    if (!(item.State == "입고완료" || item.State == "입고대기"))
                        isLastPallet = false;
                }

                if(_PRODQTY <= 0 || _REMAINQTY < 0)
                    throw new Exception("생산량이 0보다 작을 수 없습니다.");
                
                //마지막 팔레트인 경우 입고량은 잔량과 동일
                if(isLastPallet)
                    OUTPUTQTY = _REMAINQTY;
                else
                {
                    int chg = Convert.ToInt32(Math.Floor(Convert.ToDouble(tar.NetWeight / _convertParam)));

                    if (_OUTPUTQTY + chg > _REMAINQTY)
                        OUTPUTQTY = _REMAINQTY;
                    else
                        OUTPUTQTY = _OUTPUTQTY + chg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}