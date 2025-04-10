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
    public class SVP소분원료확인및무게측정ViewModel : ViewModelBase
    {
        #region Property
        private SVP소분원료확인및무게측정 _mainWnd;
        public SVP소분원료확인및무게측정ViewModel()
        {
            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing = new BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing();
            _DISPMSUBLOTList = new ObservableCollection<ChargedWIPContainer>();
            _BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi = new BR_BRS_REG_ProductionOrderOutput_Scale_Weight_Multi();
            _BR_BRS_UPD_MaterialSubLot_ChangeQuantity = new BR_BRS_UPD_MaterialSubLot_ChangeQuantity();

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

        private string _MSUBLOTBCD;
        public string MSUBLOTBCD
        {
            get { return _MSUBLOTBCD; }
            set
            {
                _MSUBLOTBCD = value;
                OnPropertyChanged("MSUBLOTBCD");
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

        private string _MtrlId;
        public string MtrlId
        {
            get { return _MtrlId; }
            set
            {
                _MtrlId = value;
                OnPropertyChanged("MtrlId");
            }
        }
        private string _MtrlName;
        public string MtrlName
        {
            get { return _MtrlName; }
            set
            {
                _MtrlName = value;
                OnPropertyChanged("MtrlName");
            }
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
        /// 중량확인한 원료 목록(XML로 EBR에 기록됨)
        /// </summary>
        private ObservableCollection<ChargedWIPContainer> _DISPMSUBLOTList;
        public ObservableCollection<ChargedWIPContainer> DISPMSUBLOTList
        {
            get { return _DISPMSUBLOTList; }
            set
            {
                _DISPMSUBLOTList = value;
                OnPropertyChanged("DISPMSUBLOTList");
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
        /// 소분된 SubLot 조회
        /// </summary>
        private BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing;
        public BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing
        {
            get { return _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing; }
            set
            {
                _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing = value;
                OnPropertyChanged("BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing");
            }
        }
        /// <summary>
        /// 무게 변경
        /// </summary>
        private BR_BRS_UPD_MaterialSubLot_ChangeQuantity _BR_BRS_UPD_MaterialSubLot_ChangeQuantity;
        public BR_BRS_UPD_MaterialSubLot_ChangeQuantity BR_BRS_UPD_MaterialSubLot_ChangeQuantity
        {
            get { return _BR_BRS_UPD_MaterialSubLot_ChangeQuantity; }
            set
            {
                _BR_BRS_UPD_MaterialSubLot_ChangeQuantity = value;
                OnPropertyChanged("BR_BRS_UPD_MaterialSubLot_ChangeQuantity");
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
                            if (arg != null && arg is SVP소분원료확인및무게측정)
                            {
                                _mainWnd = arg as SVP소분원료확인및무게측정;

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

                                if (_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count > 0)
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
        /// 원료정보 조회
        /// </summary>
        public ICommand GetMsublotInfoCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["GetMsublotInfoCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["GetMsublotInfoCommandAsync"] = false;
                            CommandCanExecutes["GetMsublotInfoCommandAsync"] = false;

                            ///
                            if (_ScaleInfo == null || string.IsNullOrWhiteSpace(_SCALEID))
                            {
                                OnMessage("저울을 먼저 스캔하세요.");
                                return;
                            }

                            if (arg != null && arg is string)
                            {
                                string mSubLotBcd = arg as string;
                                decimal tare = 0m;
                                GorssSum = 0m;  // 이론량. 용기에 담긴 제품의 무게의 합.

                                // 적재된 원료 조회
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.INDATAs.Clear();
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.OUTDATAs.Clear();
                                _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.INDATAs.Add(new BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    MSUBLOTBCD = mSubLotBcd
                                });

                                if (await _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.Execute() && _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.OUTDATAs.Count > 0)
                                {
                                    foreach (var item in _BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.OUTDATAs)
                                    {
                                        GorssSum += Weight.Add(0, _CURVALUE.Uom, item.MSUBLOTQTY.GetValueOrDefault() + item.TAREWEIGHT.GetValueOrDefault(), item.UOMNAME);
                                        //2023.05.15 박희돈 Y-MC팀 이정연팀장, 김진수매니저 요청. EBR에 자재코드와 자재명 표현.
                                        MtrlId = item.MTRLID;
                                        MtrlName = item.MTRLNAME;
                                        MSUBLOTBCD = item.MSUBLOTBCD;
                                    }
                                }
                                else
                                    OnMessage("적재된 자재가 없습니다.");

                                if (GorssSum > 0)
                                {
                                    int precision = 0;
                                    //precision = Convert.ToInt32(Math.Pow(10, Scal.Precision));
                                    precision = Convert.ToInt32(Math.Pow(10, Convert.ToInt16(_ScaleInfo.PRECISION)));
                                    // 2023.05.09 박희돈 저울 1자리일 경우 소수점이 3자리이면 올림 시 값이 달라짐.
                                    // EX) 내용물 97g일 경우 하한값은 97 * 0.998 = 96.806이 나오며 계산상 소수점 1자리는 96.9로 나옴.
                                    //     3번째 자리 버리고 2번째 자리에서 올림이 아닌 전체 소수점 자리로 계산 됨.(엑셀 올림 버림도 동일) 
                                    //     주사제 민웅 매니저, 이강선 매니저, Y-MC김진수 매니저 확인.
                                    decimal min = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble((tare + GorssSum) * 0.998m) * precision) / precision);
                                    decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble((tare + GorssSum) * 1.002m) * precision) / precision);
                                    //decimal min = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble((tare + GorssSum) * 0.998m) * 10) / 10);
                                    //decimal max = Convert.ToDecimal(Math.Floor(Convert.ToDouble((tare + GorssSum) * 1.002m) * 10) / 10);

                                    //_MINVALUE.SetWeight(min > 0 ? min : 0m, _CURVALUE.Uom, _CURVALUE.Precision);
                                    //_MAXVALUE.SetWeight(max > 0 ? max : 0m, _CURVALUE.Uom, _CURVALUE.Precision);

                                    _MINVALUE.SetWeight(min > 0 ? min : 0m, _ScaleInfo.NOTATION, Convert.ToInt16(_ScaleInfo.PRECISION));
                                    _MAXVALUE.SetWeight(max > 0 ? max : 0m, _ScaleInfo.NOTATION, Convert.ToInt16(_ScaleInfo.PRECISION));

                                    OnPropertyChanged("MINVALUE");
                                    OnPropertyChanged("MAXVALUE");
                                }
                                else
                                    OnMessage("적재된 원료무게 합이 0 보다 작을 수 없습니다.");
                                
                                // 이미 측정한 용기인지 확인
                                if (_DISPMSUBLOTList.Count > 0 && _DISPMSUBLOTList.Where(x => x.MsubLotBcd == _MSUBLOTBCD).Count() > 0)
                                {
                                    OnMessage("이미 추가된 용기입니다.");
                                    InitializeData();
                                }

                            }
                            else
                            {
                                MSUBLOTBCD = "";
                            }
                            ///

                            CommandResults["GetMsublotInfoCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["GetMsublotInfoCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["GetMsublotInfoCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("GetMsublotInfoCommandAsync") ?
                        CommandCanExecutes["GetMsublotInfoCommandAsync"] : (CommandCanExecutes["GetMsublotInfoCommandAsync"] = true);
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
                            // 저울 무게는 변경이 되니 변수에 담음
                            Decimal CurWeightValue = _CURVALUE.Value;

                            btnSaveWeightEnable = false;
                            _repeater.Stop();
                            
                            Weight tare = new Weight();
                            tare.SetWeight(0, _CURVALUE.Uom, _CURVALUE.Precision);

                            DISPMSUBLOTList.Add(new ChargedWIPContainer
                            {
                                PoId = _mainWnd.CurrentOrder.ProductionOrderID,
                                ScaleId = _SCALEID,
                                MtrlId = MtrlId,
                                MtrlName = MtrlName,
                                MsubLotBcd = MSUBLOTBCD,
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
                            _repeater.Start();

                            //btnSaveWeightEnable = true;

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
                            if (_DISPMSUBLOTList.Count > 0)
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
                                    string.Format("SVP소분원료확인및무게측정"),
                                    string.Format("SVP소분원료확인및무게측정"),
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
                                dt.Columns.Add(new DataColumn("원료코드"));
                                dt.Columns.Add(new DataColumn("원료명"));
                                dt.Columns.Add(new DataColumn("저울번호"));
                                //Y-MC팀 이정연팀장, 김진수매니저 요청. 용기중량 내용물중량 주석
                                //dt.Columns.Add(new DataColumn("용기중량"));
                                //dt.Columns.Add(new DataColumn("내용물중량"));
                                dt.Columns.Add(new DataColumn("하한"));
                                dt.Columns.Add(new DataColumn("전체중량"));
                                dt.Columns.Add(new DataColumn("상한"));
                                dt.Columns.Add(new DataColumn("무게"));   // 2021.11.01 박희돈 무게(측정값)으로 컬럼명 만들어 달라는 요청(남정이) 해당 이름으로는 컬럼을 못만들어 EBR표시할때만 바꿔줌.

                                foreach (var item in _DISPMSUBLOTList)
                                {
                                    var row = dt.NewRow();

                                    //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                                    row["오더번호"] = item.PoId != null ? item.PoId : "";
                                    //-------------------------------------------------------------------------------------------------------
                                    row["원료코드"] = item.MtrlId != null ? item.MtrlId : "";
                                    row["원료명"] = item.MtrlName != null ? item.MtrlName : "";
                                    row["저울번호"] = item.ScaleId != null ? item.ScaleId : "";
                                    //Y-MC팀 이정연팀장, 김진수매니저 요청. 용기중량 내용물중량 주석
                                    //row["용기중량"] = item != null ? item.TareWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";
                                    //row["내용물중량"] = item != null ? item.NetWeight.ToString("F" + item.Precision) + " " + _ScaleUom : "";
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

        public ICommand NoRecordConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommandAsync"] = false;
                            CommandCanExecutes["NoRecordConfirmCommandAsync"] = false;

                            ///
                            _repeater.Stop();

                            // 전자서명
                            iPharmAuthCommandHelper authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Role,
                                Common.enumAccessType.Create,
                                "SVP소분원료확인및무게측정",
                                "SVP소분원료확인및무게측정",
                                false,
                                "OM_ProductionOrder_SUI",
                                "",
                                null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //XML 형식으로 저장
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            //2023.01.03 김호연 원료별 칭량을 하면 2개 이상의 배치가 동시에 기록되므로 EBR 확인할때 오더로 구분해야함
                            dt.Columns.Add(new DataColumn("오더번호"));
                            //-------------------------------------------------------------------------------------------------------
                            dt.Columns.Add(new DataColumn("저울번호"));
                            dt.Columns.Add(new DataColumn("용기중량"));
                            dt.Columns.Add(new DataColumn("내용물중량"));
                            dt.Columns.Add(new DataColumn("하한"));
                            dt.Columns.Add(new DataColumn("전체중량"));
                            dt.Columns.Add(new DataColumn("상한"));
                            dt.Columns.Add(new DataColumn("무게"));

                            var row = dt.NewRow();
                            row["오더번호"] = "N/A";
                            row["저울번호"] = "N/A";
                            row["용기중량"] = "N/A";
                            row["내용물중량"] = "N/A";
                            row["하한"] = "N/A";
                            row["전체중량"] = "N/A";
                            row["상한"] = "N/A";
                            row["무게"] = "N/A";
                            dt.Rows.Add(row);

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

                            _mainWnd.Close();

                            //
                            CommandResults["NoRecordConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NoRecordConfirmCommandAsync"] = false;
                            _repeater.Start();
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommandAsync") ?
                        CommandCanExecutes["NoRecordConfirmCommandAsync"] : (CommandCanExecutes["NoRecordConfirmCommandAsync"] = true);
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
                        if (_MINVALUE.Value >= 0 && _MAXVALUE.Value > 0)
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

                        new SolidColorBrush(Colors.Red);
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

                MSUBLOTBCD = "";
                BR_BRS_GET_MaterialSubLot_ContainerInfo_Dispensing.OUTDATAs.Clear();
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
    //public class CheckWeightBoolBurshConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value != null && value is bool)
    //        {
    //            bool validationResult = (bool)value;
    //            if (validationResult)
    //                return new SolidColorBrush(Colors.Green);

    //        }

    //        return new SolidColorBrush(Colors.Yellow);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
