using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Linq;
using 보령.UserControls;

namespace 보령
{
    public class 칭량IBC반제품무게측정ViewModel : ViewModelBase
    {
        #region Property
        private 칭량IBC반제품무게측정 _mainWnd;
        public 칭량IBC반제품무게측정ViewModel()
        {
            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_BRS_SEL_VESSEL_Info = new BR_BRS_SEL_VESSEL_Info();
            _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging = new BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging();
            _IBCList = new ObservableCollection<ChargedWIPContainer>();
            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi = new BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi();

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;
        }

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

        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 2000;
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
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
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
        private string _ScaleUom = "g";
        private int _ScalePrecision = 3;


        private Weight _MAXVALUE = new Weight();
        public string MAXVALUE
        {
            get { return _MAXVALUE.WeightUOMString; }
        }
        private Weight _CURVALUE = new Weight();
        public string CURVALUE
        {
            get { return _CURVALUE.WeightUOMString; }
        }
        private Weight _MINVALUE = new Weight();
        public string MINVALUE
        {
            get { return _MINVALUE.WeightUOMString; }
        }
           

        private bool _btnSaveWeightEnable;
        /// <summary>
        /// 중량확인 버튼
        /// </summary>
        public bool btnSaveWeightEnable
        {
            get { return _btnSaveWeightEnable; }
            set
            {
                _btnSaveWeightEnable = value;
                OnPropertyChanged("btnSaveWeightEnable");
            }
        }

        /// <summary>
        /// 중량확인한 용기 목록(XML로 EBR에 기록됨)
        /// </summary>
        private ObservableCollection<ChargedWIPContainer> _IBCList;
        public ObservableCollection<ChargedWIPContainer> IBCList
        {
            get { return _IBCList; }
            set
            {
                _IBCList = value;
                OnPropertyChanged("IBCList");
            }
        }

        decimal GorssSum;
        #endregion
        #region BizRule
        /// <summary>
        /// 작업장 저울 조회
        /// </summary>
        private BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo;
        /// <summary>
        /// 저울 사용자 속성 조회
        /// </summary>
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        /// <summary>
        /// 용기 정보 조회(Tare)
        /// </summary>
        private BR_BRS_SEL_VESSEL_Info _BR_BRS_SEL_VESSEL_Info;
        /// <summary>
        /// 용기에 적재된 SubLot 조회
        /// </summary>
        private BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging;
        public BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging
        {
            get { return _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging; }
            set
            {
                _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging = value;
                OnPropertyChanged("BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging");
            }
        }
        /// <summary>
        /// OutputSubLot 무게 저장(내용물 무게 저장)
        /// </summary>
        private BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi;

        #endregion
        #region Command
        /// <summary>
        /// 화면 로딩
        /// </summary>
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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if(arg != null && arg is 칭량IBC반제품무게측정)
                            {
                                _mainWnd = arg as 칭량IBC반제품무게측정;

                                // 창이 닫히는 경우 timer를 정지, 초기화
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_repeater != null)
                                        _repeater.Stop();

                                    _repeater = null;
                                };

                                #region Campaign Order
                                OrderList = await CampaignProduction.GetProductionOrderList(_mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, _mainWnd.CurrentOrder.ProductionOrderID);
                                CanSelectOrder = OrderList.Count > 0 ? true : false;
                                #endregion

                                // 기본값 setup
                                btnSaveWeightEnable = false;

                                // 현재 작업장의 기본저울 조회
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Clear();
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Clear();
                                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATA
                                {
                                    EQPTID = AuthRepositoryViewModel.Instance.RoomID,
                                    LANGID = AuthRepositoryViewModel.Instance.LangID
                                });

                                await _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.Execute();

                                if(_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    string scaleid = "";
                                    foreach (var item in _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs)
                                    {
                                        if (item.MODEL.Contains("IFS4"))
                                            scaleid = item.EQPTID;
                                    }

                                    if (scaleid == "")
                                        scaleid = _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[0].EQPTID;

                                    ChangeScaleCommand.Execute(scaleid);
                                }

                            }
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                       CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
               });
            }
        }

        /// <summary>
        /// 저울 변경
        /// </summary>
        public ICommand ChangeScaleCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangeScaleCommand"] = false;
                        CommandCanExecutes["ChangeScaleCommand"] = false;

                        /// 
                        _repeater.Stop();

                        BarcodePopup popup = new BarcodePopup();
                        popup.tbMsg.Text = "저울바코드를 스캔하세요.";
                        popup.Closed += async (sender, e) =>
                        {
                            if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                            {
                                string text = popup.tbText.Text.ToUpper();

                                // 저울 정보 조회
                                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    EQPTID = text
                                });

                                if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute() && _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Count > 0)
                                {
                                    int chk;

                                    _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                    _ScaleUom = _ScaleInfo.NOTATION;
                                    _ScalePrecision = int.TryParse(_ScaleInfo.PRECISION.ToString(), out chk) ? chk : 3;
                                    SCALEID = _ScaleInfo.EQPTID;

                                    _repeater.Start();
                                }
                            }
                            else
                                _ScaleInfo = null;

                            OnPropertyChanged("ScaleId");
                        };

                        popup.Show();
                        ///

                        CommandResults["ChangeScaleCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangeScaleCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        IsBusy = false;
                        CommandCanExecutes["ChangeScaleCommand"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangeScaleCommand") ?
                        CommandCanExecutes["ChangeScaleCommand"] : (CommandCanExecutes["ChangeScaleCommand"] = true);
                });
            }
        }

        /// <summary>
        /// 용기정보 조회
        /// </summary>
        public ICommand GetVesselInfoCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["GetVesselInfoCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["GetVesselInfoCommandAsync"] = false;
                            CommandCanExecutes["GetVesselInfoCommandAsync"] = false;

                            ///
                            if(_ScaleInfo == null || string.IsNullOrWhiteSpace(_SCALEID))
                            {
                                OnMessage("저울을 먼저 스캔하세요.");
                                return;
                            }

                            if(arg != null && arg is string)
                            {
                                string vessel = arg as string;
                                decimal tare = 0m;
                                GorssSum = 0m;  // 이론량. 용기에 담긴 제품의 무게의 합.

                                // 용기정보 조회
                                _BR_BRS_SEL_VESSEL_Info.INDATAs.Clear();
                                _BR_BRS_SEL_VESSEL_Info.OUTDATAs.Clear();
                                _BR_BRS_SEL_VESSEL_Info.INDATAs.Add(new BR_BRS_SEL_VESSEL_Info.INDATA
                                {
                                    VESSELID = vessel
                                });

                                if (await _BR_BRS_SEL_VESSEL_Info.Execute() && _BR_BRS_SEL_VESSEL_Info.OUTDATAs.Count == 1)
                                {
                                    VESSELID = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].VESSELID;
                                    tare = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].TAREWEIGHT.GetValueOrDefault();                                   
                                }
                                else
                                    OnMessage("용기정보를 조회하지 못했습니다.");

                                // 적재된 원료 조회
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATAs.Clear();
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Clear();
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATAs.Add(new BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    VESSELID = _VESSELID
                                });

                                if (await _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.Execute() && _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Count > 0)
                                {
                                    foreach (var item in _BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs)
                                    {
                                        GorssSum += Weight.Add(0, _CURVALUE.Uom, item.MSUBLOTQTY.GetValueOrDefault(), item.UOMNAME);
                                    }
                                }
                                else
                                    OnMessage("적재된 자재가 없습니다.");

                                if (tare > 0)
                                {
                                    if (GorssSum > 0)
                                    {
                                        decimal min = tare + GorssSum - 300, max = tare + GorssSum + 300;

                                        _MINVALUE.SetWeight(min > 0 ? min : 0m, _CURVALUE.Uom, _CURVALUE.Precision);                                        
                                        _MAXVALUE.SetWeight(max > 0 ? max : 0m, _CURVALUE.Uom, _CURVALUE.Precision);
                                        OnPropertyChanged("MINVALUE");
                                        OnPropertyChanged("MAXVALUE");
                                    }
                                    else
                                        OnMessage("적재된 원료무게 합이 0 보다 작을 수 없습니다.");                               
                                }
                                else
                                    OnMessage("TARE가 0 입니다.");

                                // 이미 측정한 용기인지 확인
                                if(_IBCList.Count > 0 && _IBCList.Where(x => x.VesselId == _VESSELID).Count() > 0)
                                {
                                    OnMessage("이미 추가된 용기입니다.");
                                    InitializeData();
                                }
                                
                            }
                            else
                            {
                                VESSELID = "";
                            }
                            ///

                            CommandResults["GetVesselInfoCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["GetVesselInfoCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["GetVesselInfoCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("GetVesselInfoCommandAsync") ?
                       CommandCanExecutes["GetVesselInfoCommandAsync"] : (CommandCanExecutes["GetVesselInfoCommandAsync"] = true);
               });
            }
        }

        /// <summary>
        /// 중량 확인한 용기무게 저장(저장되는 무게는 실제 저울값 사용)
        /// </summary>
        public ICommand SaveWeightCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveWeightCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SaveWeightCommandAsync"] = false;
                            CommandCanExecutes["SaveWeightCommandAsync"] = false;

                            // 2021-12-03 김호연 
                            // 저울 무게 기록하는 시점에 무게 변경되는 현상이 발생
                            // 저울 무게는 변경이 되니 변수에 담아서 무게 저장 및 기록
                            Decimal CurWeightValue = _CURVALUE.Value;

                            btnSaveWeightEnable = false;
                            _repeater.Stop();

                            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATAs.Clear();
                            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.INDATA
                            {
                                VESSELID = _VESSELID,
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID,
                                //GROSSWEIGHT = _CURVALUE.Value,
                                // 2021-12-03 김호연 
                                GROSSWEIGHT = CurWeightValue,
                                SCALEID = _SCALEID,
                                ROOMNO = AuthRepositoryViewModel.Instance.RoomID
                            });

                            if (await _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi.Execute())
                            {
                                Weight tare = new Weight();
                                tare.SetWeight(_BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].TAREWEIGHT.GetValueOrDefault(), _CURVALUE.Uom, _CURVALUE.Precision);

                                IBCList.Add(new ChargedWIPContainer
                                {
                                    PoId = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].POID,
                                    VesselId = _BR_BRS_SEL_VESSEL_Info.OUTDATAs[0].VESSELID,
                                    ScaleId = _SCALEID,
                                    Uom = _CURVALUE.Uom,
                                    Precision = _CURVALUE.Precision,
                                    TareWeight = Convert.ToDecimal(tare.Value.ToString("F" + tare.Precision)),
                                    NetWeight = GorssSum,
                                    MinWeight = MINVALUE,
                                    MaxWeight = MAXVALUE,
                                    //CurWeight = _CURVALUE.Value
                                    // 2021-12-03 김호연 
                                    CurWeight = CurWeightValue
                                });

                                InitializeData();
                            }
                            else
                            {                                 
                                btnSaveWeightEnable = true;
                            }

                            _repeater.Start();
                            ///

                            CommandResults["SaveWeightCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveWeightCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveWeightCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("SaveWeightCommandAsync") ?
                       CommandCanExecutes["SaveWeightCommandAsync"] : (CommandCanExecutes["SaveWeightCommandAsync"] = true);
               });
            }
        }

        /// <summary>
        /// XML 저장
        /// </summary>
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

                            ///
                            if (_IBCList.Count > 0)
                            {
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

                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("반제품무게측정"),
                                    string.Format("반제품무게측정"),
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
                                dt.Columns.Add(new DataColumn("용기번호"));
                                dt.Columns.Add(new DataColumn("저울번호"));
                                dt.Columns.Add(new DataColumn("용기중량"));
                                dt.Columns.Add(new DataColumn("내용물중량"));
                                dt.Columns.Add(new DataColumn("하한"));
                                dt.Columns.Add(new DataColumn("전체중량"));
                                dt.Columns.Add(new DataColumn("상한"));
                                dt.Columns.Add(new DataColumn("무게"));   // 2021.11.01 박희돈 무게(측정값)으로 컬럼명 만들어 달라는 요청(남정이) 해당 이름으로는 컬럼을 못만들어 EBR표시할때만 바꿔줌.

                                foreach (var item in _IBCList)
                                {
                                    var row = dt.NewRow();

                                    //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                    row["오더번호"] = item.PoId != null ? item.PoId : "";
                                    //-------------------------------------------------------------------------------------------------------
                                    row["용기번호"] = item.VesselId != null ? item.VesselId : "";
                                    row["저울번호"] = item.ScaleId != null ? item.ScaleId : "";
                                    row["용기중량"] = item != null ? item.TareWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";
                                    row["내용물중량"] = item != null ? item.NetWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";
                                    row["하한"] = item != null ? item.MinWeight : "";
                                    row["전체중량"] = item != null ? item.GrossWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";
                                    row["상한"] = item != null ? item.MaxWeight : "";
                                    row["무게"] = item != null ? item.CurWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";

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
                            else
                                OnMessage("조회된 데이터가 없습니다.");
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

        #endregion

        private async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                if (_ScaleInfo != null)
                {
                    bool success = false;
                    Weight val = new Weight();

                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            val.SetWeight(result.data, result.unit);                            
                        }
                    }
                    else
                    {
                        BR_BRS_SEL_CurrentWeight current_wight = new BR_BRS_SEL_CurrentWeight();
                        current_wight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA()
                        {
                            ScaleID = _ScaleInfo.EQPTID
                        });

                        if (await current_wight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) == true
                            && current_wight.OUTDATAs.Count > 0 && current_wight.OUTDATAs[0].Weight.HasValue)
                        {
                            success = true;
                            val.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, _ScalePrecision);
                        }
                    }
                   
                    if (success)
                    {                                                
                        _CURVALUE = val;
                        _ScaleUom = val.Uom;
                        _ScalePrecision = val.Precision;
                        if(_MINVALUE.Value >= 0 && _MAXVALUE.Value > 0)
                        {
                            if (Weight.Add(0, _CURVALUE.Uom, _MINVALUE.Value, _MINVALUE.Uom) <= _CURVALUE.Value
                                && _CURVALUE.Value <= Weight.Add(0, _CURVALUE.Uom, _MAXVALUE.Value, _MAXVALUE.Uom))
                                btnSaveWeightEnable = true;
                            else
                                btnSaveWeightEnable = false;
                        }
                        else
                            btnSaveWeightEnable = false;
                    }
                    else
                    {
                        val.SetWeight(0, _ScaleUom, _ScalePrecision);
                        _CURVALUE = val;
                        btnSaveWeightEnable = false;               
                    }
                    OnPropertyChanged("CURVALUE");

                    _repeater.Start();
                }
            }
            catch (Exception ex)
            {
                _repeater.Stop();
                OnException(ex.Message, ex);
            }
        }

        private void InitializeData()
        {
            try
            {
                // 중량확인 완료 후
                // 용기정보, 적재된 원료, MIN&MAX 범위 초기화

                VESSELID = "";
                BR_BRS_GET_MaterialSubLot_ContainerInfo_LayerCharging.OUTDATAs.Clear();
                _MINVALUE.Value = 0;
                _MAXVALUE.Value = 0;

                OnPropertyChanged("MINVALUE");
                OnPropertyChanged("MAXVALUE");

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        
        public class ChargedWIPContainer : WIPContainer
        {
            private string _ScaleId;
            public string ScaleId
            {
                get { return this._ScaleId; }
                set
                {
                    this._ScaleId = value;
                    this.OnPropertyChanged("ScaleId");
                }
            }
        }
    }

    /// <summary>
    /// 저울 현재값 배경색 ( yellow : 범위 벗어남, green : 적합 )
    /// </summary>
    public class CheckWeightBoolBurshConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                bool validationResult = (bool)value;
                if (validationResult)
                    return new SolidColorBrush(Colors.Green);

            }

            return new SolidColorBrush(Colors.Yellow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
