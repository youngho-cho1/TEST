using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;
using ShopFloorUI;

using 보령.DataModel;
using 보령.DataModel.ContainerModel;
using 보령.DataModel.WeighPrepareModel;

using Browser = System.Windows.Browser;

using LGCNS.EZMES.ControlsLib;
using LGCNS.iPharmMES.Common;

using 보령.UserControls;


using C1.Silverlight;
using System.Windows.Media;

namespace 보령
{
    public class SystemInformation
    {
        public static string ComputerName { get; set; }

        public static string GetComputerName()
        {
            return ComputerName;
        }

        public static AuthorityContainer AuthContainer { get; set; }
    }

    public class 원료칭량ViewModel : ViewModelBase
    {
        #region [Property]

        private 원료칭량 _mainControl;
        public 원료칭량 mainControl
        {
            get { return _mainControl; }
            set { _mainControl = value; OnPropertyChanged("mainControl"); }
        }

        private string _dspstrtDTTM;

        private decimal _currentGageValue = 0;
        private int _trustGridRownumber = 0;
        private enum weighingMethodStatus { none, split, complete };
        private weighingMethodStatus _wStatus;

        private 보령.DataModel.WeighPrepareModel.Scale _SelectedScale;
        private 보령.DataModel.WeighPrepareModel.Scale _SelectedBottomScale;

        private bool _GageActivate = false;
        private bool _allCompleteYn = false;
        private bool _sourcePopupOpened = false;

        private DispatcherTimer _repeater;
        private int _repeaterInterval = 5000;   // 100ms -> 500ms -> 1000ms
        
        private string _SelectedSourcePrinterName;
        
        ObservableCollection<SourceContainerInformation> _mainSourceList;
        ObservableCollection<UsedSourceContainerInfo> _mainUsedList = new ObservableCollection<UsedSourceContainerInfo>();

        private SourceContainerInformation _sourceContainerInformation;
        public SourceContainerInformation SourceContainerInfo { get { return _sourceContainerInformation; } }

        private string developMode = "N";
        private string tempUnit = string.Empty; // Unstable 한 상태에서 저울단위가 안 넘어올 경우 사용되는 단위
        private string sourceChangeSubLot = string.Empty;

        private bool _isMaterialWeightInTolerance;
                
        private int _currentComponentIndex;
        private List<ComponentInformation> _componentInformation;
        public int ComponentInfoIndex { get { return _componentInformation.Count > _currentComponentIndex ? _currentComponentIndex : _componentInformation.Count - 1; } }
        public ComponentInformation ComponentInfo { get { return _componentInformation[ComponentInfoIndex]; } }
        public WeighContainerInformation WeighContainerInfo { get { return ComponentInfo.CurrentContainer; } }                
        private WeighInformation _weighInformation;
        public WeighInformation WeighInfo { get { return _weighInformation; } }

        private BinBarcodePopup binPopup;

        //[신뢰칭량] 바코드 리스트 ItemSource 
        private ObservableCollection<TrustListParameter> _TParameters;
        public ObservableCollection<TrustListParameter> TParameters
        {
            get
            {
                return _TParameters;
            }
            set
            {
                _TParameters = value;
                OnPropertyChanged("TParameters");
            }
        }

        private string _POID;
        public string POID
        {
            get { return _POID; }
            set { _POID = value; OnPropertyChanged("POID"); }
        }

        //현재 OPSGGUID
        private string _OPSGGUID;
        public string OPSGGUID
        {
            get { return _OPSGGUID; }
            set { _OPSGGUID = value; OnPropertyChanged("OPSGGUID"); }
        }
        
        //현재 BATCHNO
        private string _BATCHNO;
        public string BATCHNO
        {
            get { return _BATCHNO; }
            set { _BATCHNO = value; OnPropertyChanged("BATCHNO"); }
        }

        //현재 MTRLID
        private string _MTRLID;
        public string MTRLID
        {
            get { return _MTRLID; }
            set { _MTRLID = value; OnPropertyChanged("MTRLID"); }
        }

        //현재 MTRLNAME
        private string _MTRLNAME;
        public string MTRLNAME
        {
            get { return _MTRLNAME; }
            set { _MTRLNAME = value; OnPropertyChanged("MTRLNAME"); }
        }

        //원료GUID
        private string _COMPONENTGUID;
        public string COMPONENTGUID
        {
            get
            {
                return _COMPONENTGUID;
            }
            set { _COMPONENTGUID = value; OnPropertyChanged("COMPONENTGUID"); }
        }

        //칭량방법
        private string _WEIGHINGMETHOD;
        public string WEIGHINGMETHOD
        {
            get
            {
                return _WEIGHINGMETHOD;
            }
            set { _WEIGHINGMETHOD = value; OnPropertyChanged("WEIGHINGMETHOD"); }
        }

        private string _SelectedDispensePrinterName = "프린터 없음.";
        public string SelectedDispensePrinterName
        {
            get { return _SelectedDispensePrinterName; }
            set { _SelectedDispensePrinterName = value; OnPropertyChanged("SelectedDispensePrinterName"); }
        }
        
        //IsProcess
        private bool _IsProcess = false;
        public bool IsProcess
        {
            get
            {
                return _IsProcess;
            }
            set
            {
                _IsProcess = value;
                OnPropertyChanged("IsProcess");
            }
        }

        //원료변경 버튼 Content
        private string _BTCOMPONENT;
        public string BTCOMPONENT
        {
            get { return _BTCOMPONENT; }
            set { _BTCOMPONENT = value; OnPropertyChanged("BTCOMPONENT"); }
        }
        
        //[저울게이지바] 현재게이지(BAR 사이즈)
        private decimal _SC_VALUE = 0;
        public decimal SC_VALUE
        {
            get
            {
                return _SC_VALUE;
            }
            set
            {
                _SC_VALUE = value;
                
                OnPropertyChanged("SC_VALUE");
            }
        }

        //[저울게이지바] 현재저울 수치(BAR위에 수치)
        private decimal _SC_TOTALVALUE = 0;
        public decimal SC_TOTALVALUE
        {
            get
            {
                return _SC_TOTALVALUE;
            }
            set
            {
                _SC_TOTALVALUE = value;
                OnPropertyChanged("SC_TOTALVALUE");
            }
        }
        
        //원료에 성적번호
        private string _MLOTID = "(성적번호)";
        public string MLOTID
        {
            get { return _MLOTID; }
            set { _MLOTID = value; OnPropertyChanged("MLOTID"); }
        }

        //[Barcode입력화면 바코드]
        private string _S_BARCODE;
        public string S_BARCODE
        {
            get { return _S_BARCODE; }
            set { _S_BARCODE = value; OnPropertyChanged("S_BARCODE"); }
        }

        //[Barcode입력화면 성적번호]
        private string _S_MLOTID;
        public string S_MLOTID
        {
            get { return _S_MLOTID; }
            set { _S_MLOTID = value; OnPropertyChanged("S_MLOTID"); }
        }
        
        //원료바코드
        private string _INSP_NO = "Barcode";
        public string INSP_NO
        {
            get { return _INSP_NO; }
            set { _INSP_NO = value; OnPropertyChanged("INSP_NO"); }
        }
        
        //입력된 원료중량(실시간)
        private string _COMP_WT;
        public string COMP_WT
        {
            get { return _COMP_WT; }
            set { _COMP_WT = value; OnPropertyChanged("COMP_WT"); }
        }
        
        //입력된 원료중량
        private string _COMP_TG_WT;
        public string COMP_TG_WT
        {
            get { return _COMP_TG_WT; }
            set { _COMP_TG_WT = value; OnPropertyChanged("COMP_TG_WT"); }
        }
        
        //입력된 원료총중량
        private double _COMP_TOTAL = 0;
        public double COMP_TOTAL
        {
            get { return _COMP_TOTAL; }
            set { _COMP_TOTAL = value; OnPropertyChanged("COMP_TOTAL"); }
        }

        //[Barcode입력화면 중량]
        private string _S_WEIGHT = "0.000";
        public string S_WEIGHT
        {
            get { return _S_WEIGHT; }
            set { _S_WEIGHT = value; OnPropertyChanged("S_WEIGHT"); }
        }
        
        //[저울게이지바] 저울ID
        private string _scaleID = "저울없음";
        public string ScaleID
        {
            get { return _scaleID; }
            set
            {
                _scaleID = value;
                OnPropertyChanged("ScaleID");
            }
        }

        //[저울게이지바] UnderTolerance
        private decimal _UC_VALUE = Convert.ToDecimal(0.1);
        public decimal UC_VALUE
        {
            get { return _UC_VALUE; }
            set { _UC_VALUE = value; OnPropertyChanged("UC_VALUE"); }
        }

        //[저울게이지바] 목표치
        private decimal _SC_TARGET = 0;
        public decimal SC_TARGET
        {
            get { return _SC_TARGET; }
            set
            {
                _SC_TARGET = value;
                OnPropertyChanged("SC_TARGET");
            }
        }

        //[저울게이지바] OverTolerance
        private decimal _OC_VALUE = Convert.ToDecimal(0.1);
        public decimal OC_VALUE
        {
            get { return _OC_VALUE; }
            set { _OC_VALUE = value; OnPropertyChanged("OC_VALUE"); }
        }
        
        //Main저울 Tare 중량
        private string _SC_TAREWT = "0.000";
        public string SC_TAREWT
        {
            get { return _SC_TAREWT; }
            set { _SC_TAREWT = value; OnPropertyChanged("SC_TAREWT"); }
        }
        
        //실시간 Main저울 저울수치
        private string _SC_CURRENTWT = "0.000";
        public string SC_CURRENTWT
        {
            get
            {
                return _SC_CURRENTWT;
            }
            set
            {
                double calculateCompWeight = 0.00;
                _SC_CURRENTWT = value;

                //저울량 마이너스 && 칭량목표구간을 벗어나는 경우
                if ((Convert.ToDouble(value) <= 0) || ((SC_TARGET + OC_VALUE) < SC_VALUE))
                {
                    BTDISPENSE = false;
                    _wStatus = weighingMethodStatus.none;
                }
                else
                {
                    if (!_allCompleteYn)
                        BTDISPENSE = true;

                    //목표치에 다다르면 완료, 아니면 소분
                    if (((SC_TARGET + OC_VALUE) >= SC_VALUE) && ((SC_TARGET - UC_VALUE) <= SC_VALUE))
                    {
                        BtnDispenseContent = "완료";
                        _wStatus = weighingMethodStatus.complete;
                    }
                    else
                    {
                        BtnDispenseContent = "소분";
                        _wStatus = weighingMethodStatus.split;
                    }
                }

                if (INSP_NO != "Barcode" && Convert.ToDouble(COMP_TG_WT) > 0)
                {
                    if (Convert.ToDouble(value) > 0)
                    {
                        //calculateCompWeight = COMP_TOTAL - Convert.ToDouble(value);
                        calculateCompWeight = COMP_TOTAL - Convert.ToDouble(ConvertWeightByUnit(Convert.ToDecimal(value)));

                        COMP_WT = Convert.ToString(Math.Round(calculateCompWeight, 3));
                    }
                }

                OnPropertyChanged("SC_CURRENTWT");
            }
        }

        //실시간 바닥저울중량
        private string _SC_BTCURRENTWT = "0.000";
        public string SC_BTCURRENTWT
        {
            get { return _SC_BTCURRENTWT; }
            set { _SC_BTCURRENTWT = value; OnPropertyChanged("SC_BTCURRENTWT"); }
        }

        //바닥저울 TARE
        private string _SC_BTTAREWT = "0.000";
        public string SC_BTTAREWT
        {
            get { return _SC_BTTAREWT; }
            set { _SC_BTTAREWT = value; OnPropertyChanged("SC_BTTAREWT"); }
        }

        //Main저울 단위
        private string _SC_UNIT = "kg";
        public string SC_UNIT
        {
            get { return _SC_UNIT; }
            set
            {
                _SC_UNIT = value;
                OnPropertyChanged("SC_UNIT");
            }
        }

        //Main저울 단위
        private string _SC_BTUNIT = "kg";
        public string SC_BTUNIT
        {
            get { return _SC_BTUNIT; }
            set
            {
                _SC_BTUNIT = value;
                OnPropertyChanged("SC_BTUNIT");
            }
        }
        
        //소분 버튼 IsEnabled
        private bool _BTDISPENSE = false;
        public bool BTDISPENSE
        {
            get { return _BTDISPENSE; }
            set { _BTDISPENSE = value; OnPropertyChanged("BTDISPENSE"); }
        }
                
        //BOM저울 단위
        private string _SC_BOM_UNIT = "kg";
        public string SC_BOM_UNIT
        {
            get { return _SC_BOM_UNIT; }
            set
            {
                _SC_BOM_UNIT = value;
                OnPropertyChanged("SC_BOM_UNIT");
            }
        }

        //[신뢰칭량] 입력된 원료중량
        private string _TR_CURRENTWT = "0.000";
        public string TR_CURRENTWT
        {
            get { return _TR_CURRENTWT; }
            set { _TR_CURRENTWT = value; OnPropertyChanged("TR_CURRENTWT"); }
        }

        //[신뢰칭량] 바코드
        private string _TRUST_BARCODE;
        public string TRUST_BARCODE
        {
            get { return _TRUST_BARCODE; }
            set { _TRUST_BARCODE = value; OnPropertyChanged("TRUST_BARCODE"); }
        }
                                
        //용기바코드
        private string _DEST_NO;
        public string DEST_NO
        {
            get
            {

                return _DEST_NO;
            }
            set { _DEST_NO = value; OnPropertyChanged("DEST_NO"); }
        }
        
        //선택된 Component GUID
        private string _SelectedComponentguid;
        public string SelectedComponentguid
        {
            get { return _SelectedComponentguid; }
            set { _SelectedComponentguid = value; OnPropertyChanged("SelectedComponentguid"); }
        }
        
        //선택된 Component MLOTID
        private string _SelectedMLotID;
        public string SelectedMLotID
        {
            get { return _SelectedMLotID; }
            set { _SelectedMLotID = value; OnPropertyChanged("SelectedMLotID"); }
        }

        //저울중량 텍스트
        private string _SC_TEXT = "(저울중량)";
        public string SC_TEXT
        {
            get { return _SC_TEXT; }
            set
            {
                if (string.Compare(_SC_TEXT, value) != 0)
                {
                    _SC_TEXT = value;
                    NotifyPropertyChanged();
                    OnPropertyChanged("SC_TEXT_FOREGROUND");
                    OnPropertyChanged("SC_CURRENTWT_FOREGROUND");
                }
            }
        }

        private SolidColorBrush _SC_TEXT_FOREGROUND_WHITE = new SolidColorBrush(Colors.White);
        private SolidColorBrush _SC_TEXT_FOREGROUND_RED = new SolidColorBrush(Colors.Red);
        public SolidColorBrush SC_TEXT_FOREGROUND
        {
            get
            {
                return ("(저울중량)".Equals(SC_TEXT)) ? _SC_TEXT_FOREGROUND_WHITE : _SC_TEXT_FOREGROUND_RED;
            }
        }

        private SolidColorBrush SC_CURRENTWT_FOREGROUND_GRAY = new SolidColorBrush(Colors.DarkGray);
        public SolidColorBrush SC_CURRENTWT_FOREGROUND
        {
            get
            {
                return ("(저울중량)".Equals(SC_TEXT)) ? _SC_TEXT_FOREGROUND_WHITE : SC_CURRENTWT_FOREGROUND_GRAY;
            }
        }

        //초기화버튼 IsEnabled
        private bool _BTRESET = false;
        public bool BTRESET
        {
            get { return _BTRESET; }
            set { _BTRESET = value; OnPropertyChanged("BTRESET"); }
        }

        //TARE 버튼 IsEnabled
        private bool _BTTARE = false;
        public bool BTTARE
        {
            get { return _BTTARE; }
            set { _BTTARE = value; OnPropertyChanged("BTTARE"); }
        }

        //저울변경 버튼 IsEnabled
        private bool _BTSCALECHANGE = false;
        public bool BTSCALECHANGE
        {
            get { return _BTSCALECHANGE; }
            set { _BTSCALECHANGE = value; OnPropertyChanged("BTSCALECHANGE"); }
        }
        
        //신뢰칭량 버튼 IsEnabled
        private bool _BTTRUST = false;
        public bool BTTRUST
        {
            get { return _BTTRUST; }
            set { _BTTRUST = value; OnPropertyChanged("BTTRUST"); }
        }

        //소분버튼 Contennt
        private string _BtnDispenseContent = "소분";
        public string BtnDispenseContent
        {
            get { return _BtnDispenseContent; }
            set { _BtnDispenseContent = value; OnPropertyChanged("BtnDispenseContent"); }
        }
        
        //칭량완료 Enabled State
        private bool _CMPLENABLED = false;
        public bool CMPLENABLED
        {
            get { return _CMPLENABLED; }
            set { _CMPLENABLED = value; OnPropertyChanged("CMPLENABLED"); }
        }

        #endregion

        #region [Bizrule]

        private BR_PHR_SEL_CurrentWeight _BR_PHR_SEL_CurrentWeight;
        private BR_PHR_REG_ScaleReset _BR_PHR_REG_ScaleReset;
        private BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;

        private BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense brComList;
        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        public BR_PHR_SEL_PRINT_LabelImage BR_PHR_SEL_PRINT_LabelImage
        {
            get { return _BR_PHR_SEL_PRINT_LabelImage; }
            set { _BR_PHR_SEL_PRINT_LabelImage = value; }
        }

        private BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ;
        public BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ
        {
            get { return _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ; }
            set
            {
                _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ = value;
                OnPropertyChanged("BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ");
            }

        }

        #endregion

        #region [Constructor]

        public 원료칭량ViewModel()
        {
            _repeaterInterval = 2000;

            createDefaultViewmodels();

            LoadedWindowCommand = new CommandBase(LoadedWindow);
            ClosedWindowCommand = new CommandBase(ClosedWindow);
            SourceBarcodePopupCommand = new CommandBase(SourceBarcodePopupCmd);
            BinBarcodePopupCommand = new CommandBase(BinBarcodePopupcmd);
            ResetCommand = new CommandBase(Reset);
            TareCommand = new CommandBase(Tare);
            TrustPopupCommand = new CommandBase(TrustPopup);
            DispenseCommand = new CommandBase(Dispense);
            BtnClickCommand = new CommandBase(BtnClick);
            BtnCancelCommand = new CommandBase(BtnCancel);

            _BR_PHR_SEL_CurrentWeight = new BR_PHR_SEL_CurrentWeight();
            _BR_PHR_REG_ScaleReset = new BR_PHR_REG_ScaleReset();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            brComList = new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();
            _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ = new BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ();

            _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ_OnExecuteCompleted);
        }

        private void createDefaultViewmodels()
        {
            _componentInformation = new List<ComponentInformation>();
            _componentInformation.Add(new ComponentInformation());


            //컴포넌트
            ComponentInfo.PropertyChanged += (sender, arg) =>
            {
                OnPropertyChanged("ComponentInfo");
                OnPropertyChanged("ComponentInfo_IsEnableMix");

                if (arg.PropertyName.Equals("RequiredQuantity"))
                    OnPropertyChanged("ScaleInfo_RemainQty");

                if (arg.PropertyName.Equals("IsPrimeMaterial"))
                    OnPropertyChanged("ComponentInfo_IsActiveMaterial");
            };

            //원료컨테이너
            _sourceContainerInformation = new SourceContainerInformation();
            SourceContainerInfo.PropertyChanged += (sender, arg) =>
            {
                OnPropertyChanged("SourceContainerInfo");
            };

            //용기컨테이너
            WeighContainerInfo.PropertyChanged += (sender, arg) =>
            {
                OnPropertyChanged("WeighContainerInfo");
            };

            _weighInformation = new WeighInformation();
            //_weighPrepareStatusInformation = new WeighPrepareStatusInformation();

        }

        #endregion

        #region [ICommand]

        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }
        public ICommand SourceBarcodePopupCommand { get; set; }
        public ICommand BinBarcodePopupCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand TareCommand { get; set; }
        public ICommand BalanceSelectPopupCommand { get; set; }
        public ICommand TrustPopupCommand { get; set; }
        public ICommand DispenseCommand { get; set; }
        public ICommand BtnClickCommand { get; set; }
        public ICommand BtnCancelCommand { get; set; }

        public ICommand ScaleStateChangeCmd
        {
            get
            {
                return new CommandBase(new Action<object>((param) =>
                {
                    if (param is ExecuteCommandBehaviorParam)
                    {
                        param = (param as ExecuteCommandBehaviorParam).EventArg;
                        if (param is StateChangedEventArg)
                        {
                            StateChangedEventArg arg = param as StateChangedEventArg;
                            if (arg.NewState == ScaleControl.State.UNDER || arg.NewState == ScaleControl.State.OVER)
                                _isMaterialWeightInTolerance = false;
                            else
                                _isMaterialWeightInTolerance = true;

                            OnPropertyChanged("CanDispense");
                        }
                    }
                }
                ));
            }
        }
        
        #endregion

        #region [Command Method]

        private async void LoadedWindow(object param)
        {
            try
            {
                _mainControl = (원료칭량)param;
                _mainControl.Closed += (s, e) =>
                {
                    if (_repeater != null)
                        _repeater.Stop();

                    _repeater = null;
                };
                

                BATCHNO = _mainControl.CurrentOrder.BatchNo;

                _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.INDATAs.Add(
                    new BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.INDATA
                    {
                        POID = _mainControl.CurrentOrder.ProductionOrderID,
                        MTRLID = _mainControl.CurrentInstruction.Raw.BOMID,
                        CHGSEQ = Convert.ToInt16(_mainControl.CurrentInstruction.Raw.EXPRESSION)
                    }
                    );

                await _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.Execute();

                //사전점검 삭제
                //OpenWeighingCheck();

                //Printer Setup
                DefaultPrinterSetup();

                bool isDspCompl = true;

                _BR_PHR_SEL_CurrentWeight = new BR_PHR_SEL_CurrentWeight();
                _BR_PHR_REG_ScaleReset = new BR_PHR_REG_ScaleReset();
                _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();

                //Component List 조회
                brComList = new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense();
                brComList.INDATAs.Clear();
                brComList.OUTDATAs.Clear();
                brComList.INDATAs.Add(
                new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense.INDATA
                {
                    POID = POID,
                    OPSGGUID = OPSGGUID,
                    WEIGHINGMETHOD = WEIGHINGMETHOD,
                    COMPONENTGUID = COMPONENTGUID
                });

                if (await brComList.Execute() == false) return;

                if (brComList.OUTDATAs.Count > 0)
                {
                    foreach (BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense.OUTDATA outRow in brComList.OUTDATAs)
                    {
                        string disPenseCompleteYN = Math.Round(Convert.ToDecimal(outRow.REQQTY), 3) <= Math.Round(Convert.ToDecimal(outRow.DSPQTY), 3) ? "C" : "Y";

                        if ((outRow.SUMMARYYN.ToUpper().Equals("Y") && disPenseCompleteYN.Equals("C")) || outRow.SUMMARYYN.ToUpper().Equals("N"))
                            continue;
                        
                        //선택된 원료로 Binding
                        setSelectComponent(
                            outRow.COMPONENTGUID,
                            outRow.COMPONENTCODE,
                            outRow.MTRLNAME,
                            Convert.ToDecimal(outRow.REQQTY),
                            Convert.ToDecimal(outRow.DSPQTY),
                            outRow.MLOTID,
                            outRow.NOTATION,
                            outRow.DSPCNT > 0 ? true : false
                            );

                        isDspCompl = false;
                        break;
                    }
                }
                else
                {
                    //OnMessage("칭량할 원료가 없습니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10633"));

                    _mainControl.DialogResult = true;

                    return;
                }

                if (isDspCompl)
                {
                    await setBinAreaGrid("Init", string.Empty);
                    this.setButtonInit(false);
                }
                else
                {
                    //버튼 enable 초기화
                    this.setButtonInit(true);
                }

                //Reset 하세요.
                //this.SetMessageArea(weighingMessage.reset, string.Empty);

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        private void ClosedWindow(object param)
        {
            try
            {
                if (_repeater != null)
                {
                    _repeater.Stop();
                    _repeater = null;
                }
                //if (_balanceService != null)
                //{
                //    _balanceService.DisposeAsync();
                //    _balanceService.DisposeCompleted += (s, e) =>
                //    {
                //        _balanceService = null;
                //    };
                //}
                //if (_bottomrepeater != null)
                //{
                //    _bottomrepeater.Stop();
                //    _bottomrepeater = null;
                //}
                //if (_bottomBalanceService != null)
                //{
                //    _bottomBalanceService.DisposeAsync();
                //    _bottomBalanceService.DisConnectCompleted += (s, e) =>
                //    {
                //        _bottomBalanceService = null;
                //    };
                //}

                /*ssuchung20170728
                if (_modScale != null)
                {
                    _modScale.Disconnect();
                    _modScale = null;
                }
                if (_modBottomScale != null)
                {
                    _modBottomScale.Disconnect();
                    _modBottomScale = null;
                }
                */
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message, ex.StackTrace);
            }
        }

        public void SourceBarcodePopupCmd(object param)//원료바코드 scan popup
        {
            try
            {
                IsProcess = true;


                if (_SelectedScale == null)
                {
                    //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                    return;
                }

                SourceBarcodePopup();

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsProcess = false;
            }

        }

        public void BinBarcodePopupcmd(object param)
        {
            try
            {
                IsBusy = true;

                BinBarcodePopup();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Reset(object parma)
        {
             try
             {
                 IsBusy = true;

                 if (_SelectedScale == null)
                 {
                     //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                     OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                     return;
                 }

                 ResetExecute(null);
             }
             catch (Exception ex)
             {
                 OnException(ex.Message, ex);
             }
             finally
             {
                 IsBusy = false;
             }
        }

        public void Tare(object param)
        {
            try
            {
                IsBusy = true;

                IsProcess = true;
                OnPropertyChanged("IsProcess");

                if (_SelectedScale == null)
                {
                    //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                    return;
                }

                TareExecute(null);
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsProcess = false;
                OnPropertyChanged("IsProcess");

                IsBusy = false;
            }
        }

        public async Task BalanceSelectPopup(object param)
        {
            try
            {
                IsBusy = true;
                ///
                BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo br = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
                br.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATA()
                {
                    EQPTID = AuthRepositoryViewModel.Instance.RoomID,
                    LANGID = "ko-KR"
                });
                if (await br.Execute() == false) return;

                List<LGCNS.EZMES.ControlsLib.DataColumn> header = new List<LGCNS.EZMES.ControlsLib.DataColumn>();
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "저울 ID", DataField = "ID", HeaderWidth = 130 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "저울명", DataField = "Model", HeaderWidth = 360 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "타입", DataField = "ScaleTypeName", HeaderWidth = 100 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "정밀도", DataField = "Precision", HeaderWidth = 80 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "사용범위", DataField = "ScaleRange", HeaderWidth = 130 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "교정상태", DataField = "CalibrationStatus", HeaderWidth = 100 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "일일점검상태", DataField = "DailyCheckStatus", HeaderWidth = 100 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "월간점검상태", DataField = "MonthlyCheckStatus", HeaderWidth = 100 });
                header.Add(new LGCNS.EZMES.ControlsLib.DataColumn() { HeadName = "마지막 교정일", DataField = "LastCalibrationDate", HeaderWidth = 210 });

                List<object> datas = new List<object>();
                var bizRuleStatus = new BR_PHR_SEL_EquipmentStatus_CurrStatus();

                foreach (BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATA outdata in br.OUTDATAs)
                {
                    string monthlyStatus = string.Empty;
                    string dailyStatus = string.Empty;

                    bizRuleStatus.INDATAs.Add(new BR_PHR_SEL_EquipmentStatus_CurrStatus.INDATA()
                    {
                        LANGID = AuthRepositoryViewModel.Instance.LangID,
                        EQPTID = outdata.EQPTID,
                        EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_MONTHLY_SCALECHECKED_STATUS"),
                    });

                    if (await bizRuleStatus.Execute() == false) return;

                    if (bizRuleStatus.OUTDATAs.Count > 0)
                        monthlyStatus = bizRuleStatus.OUTDATAs[0].CAPTIONID;

                    bizRuleStatus.INDATAs.Add(new BR_PHR_SEL_EquipmentStatus_CurrStatus.INDATA()
                    {
                        LANGID = AuthRepositoryViewModel.Instance.LangID,
                        EQPTID = outdata.EQPTID,
                        EQSTID = AuthRepositoryViewModel.GetSystemOptionValue("LOGBOOK_DAILY_SCALECHECKED_STATUS"),
                    });

                    if (await bizRuleStatus.Execute() == false) return;

                    if (bizRuleStatus.OUTDATAs.Count > 0)
                        dailyStatus = bizRuleStatus.OUTDATAs[0].CAPTIONID;

                    datas.Add(new 보령.DataModel.WeighPrepareModel.Scale()
                    {
                        ID = outdata.EQPTID,
                        Precision = outdata.PRECISION.ToString(),
                        ScaleRange = outdata.RANGEMIN + "~" + outdata.RANGEMAX + " " + outdata.NOTATION,

                        CalibrationStatus = outdata.CalibrationStatus,
                        MonthlyCheckStatus = monthlyStatus,
                        DailyCheckStatus = dailyStatus,

                        LastCalibrationDate = outdata.EQSTONDTTM.Value,
                        Model = outdata.MODEL,
                        IP = outdata.IP,
                        Port = int.Parse(outdata.PORT),
                        ScaleMin = double.Parse(outdata.RANGEMIN),
                        ScaleMax = double.Parse(outdata.RANGEMAX),
                        UnitID = outdata.UOMID,
                        Interface = outdata.INTERFACE,
                        ScaleType = outdata.TYPE,
                        ScaleTypeName = outdata.TYPE == "B" ? "바닥저울" : "일반저울"
                    });
                }

                List<ExtraInformation> extraInfos = new List<ExtraInformation>();
                extraInfos.Add(new ExtraInformation() { Header = "Room No", Value = AuthRepositoryViewModel.Instance.RoomID });

                SelectListPopupViewModel model = new SelectListPopupViewModel();
                model.Headers = header;
                model.Datas = datas;
                model.PopupTitle = "저울 선택";
                model.Title = "Scale List";
                model.ExtraInfos = extraInfos;

                SelectListPopup popup = new SelectListPopup();
                popup.SetBinding(SelectListPopup.DataContextProperty, new Binding() { Source = model });
                popup.Closed += (sender, arg2) =>
                {
                    SelectListPopup listPopup = sender as SelectListPopup;
                    if (listPopup.DialogResult.HasValue && listPopup.DialogResult.Value)
                    {
                        var popupScale = model.SelectedData as 보령.DataModel.WeighPrepareModel.Scale;

                        _SelectedScale = popupScale;
                        ScaleID = _SelectedScale.ID;

                        //저울접속 
                        Connect();
                    }
                };
                popup.OKButton.Click += (sender, arg2) =>
                {
                    var popupScale = model.SelectedData as 보령.DataModel.WeighPrepareModel.Scale;

                    if (popupScale != null)
                    {
                        if (popupScale.ScaleType == "B")
                        {
                            if (_SelectedBottomScale != null)
                            {
                                if (_SelectedBottomScale.IP == popupScale.IP)
                                {
                                    //OnMessage("이미 선택된 저울입니다.");
                                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10632"));
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (_SelectedScale != null)
                            {
                                if (_SelectedScale.IP == popupScale.IP)
                                {
                                    //OnMessage("이미 선택된 저울입니다.");
                                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10632"));
                                    return;
                                }
                            }
                        }


                        if (popupScale.CalibrationStatus.Equals("OFF") || popupScale.DailyCheckStatus.Equals("OFF") || popupScale.MonthlyCheckStatus.Equals("OFF"))
                        {
                            //C1MessageBox.Show("일일점검/월간점검/검교정상태가 부적합입니다. 그래도 진행하시겠습니까?", "Confirm", C1MessageBoxButton.YesNo, C1MessageBoxIcon.Question,
                            C1MessageBox.Show(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10629"), "Confirm", C1MessageBoxButton.YesNo, C1MessageBoxIcon.Question,
                            (result) =>
                            {
                                if (result == MessageBoxResult.Yes)
                                {
                                    popup.DialogResult = true;
                                }
                            });
                        }
                        else
                        {
                            popup.DialogResult = true;
                        }
                    }
                };

                popup.Show();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //신뢰칭량
        private void TrustPopup(object param)
        {
            _trustGridRownumber = 1;
            TR_CURRENTWT = "0.000";
            TRUST_BARCODE = string.Empty;

            TrustScaleWeightPopup view = new TrustScaleWeightPopup();
            _TParameters = new ObservableCollection<TrustListParameter>();

            view.DataContext = this;
            _GageActivate = false;  //신뢰팝업 오픈시 게이지는 미 연동

            view.trustDispense.Click += (s, ar) =>
            {
                if (TParameters.Count == 0)
                {
                    //OnMessage("저장할 Data가 없습니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10409"));
                    return;
                }
                try
                {
                    TrustDispense();
                }
                catch (Exception eex)
                {
                    OnMessage(eex.Message, eex.StackTrace);
                }

                view.DialogResult = true;
            };
            view.Closed += (sc, ar2) =>
            {
                _GageActivate = true;
                TParameters = null;
            };
            view.Show();
        }

        private void Dispense(object param)
        {
            try
            {
                if (_SelectedScale == null)
                {
                    //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                    if (_repeater != null && !_repeater.IsEnabled)
                        _repeater.Start();
                    return;
                }

                _dspstrtDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DispenseExcute();

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        private void BtnClick(object param)
        {
            try
            {
                IsBusy = true;

                if (_repeater != null && _repeater.IsEnabled)
                    _repeater.Stop();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void BtnCancel(object param)
        {
            try
            {
                IsBusy = true;

                if (_repeater != null && !_repeater.IsEnabled)
                    _repeater.Start();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region [OnExecuteCompleted]

        public void _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ_OnExecuteCompleted(string ruleName)
        {
            if (_BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.OUTDATAs.Count > 0)
            {
                foreach (var outdata in _BR_PHR_SEL_ProductionOrderBOM_MTRLID_CHGSEQ.OUTDATAs)
                {
                    POID = outdata.POID;
                    OPSGGUID = outdata.OPSGGUID;
                    MTRLID = outdata.MTRLID;
                    MTRLNAME = outdata.MTRLNAME;
                    COMPONENTGUID = outdata.COMPONENTGUID;
                    WEIGHINGMETHOD = outdata.WEIGHINGMETHOD;
                }
            }

        }

        #endregion 

        #region [User Define Method]

        /// <summary>
        /// 선택된 원료 셋업
        /// </summary>
        /// <param name="selectccGuid">Component GUID</param>
        /// <param name="selectCCode">Component CODE</param>
        /// <param name="selectCName">Component NAME</param>
        /// <param name="reqQty">지시중량</param>
        /// <param name="dspQty">칭량중량</param>
        /// <param name="selectMlotID">MLOTID</param>
        private void setSelectComponent(string selectccGuid, string selectCCode, string selectCName, decimal reqQty, decimal dspQty, string selectMlotID, string bomUnit, bool dspGageApply = true)
        {
            //선택된 원료 Property 
            SelectedComponentguid = selectccGuid;
            //SelectedComponentcode = selectCCode;
            //SelectedComponentname = selectCName;
            SelectedMLotID = selectMlotID;
            SC_BOM_UNIT = bomUnit;

            //버튼 Content
            BTCOMPONENT = string.Concat(selectCCode, " - ", selectCName);

            //게이지 지시량
            SC_TARGET = reqQty;

            // 게이지 초기화(소분량을 넣어준다.)     
            if (dspGageApply)
                _currentGageValue = dspQty;

            setScaleGageValue(Convert.ToDecimal(0.00));

            //성적번호/원료용량 초기화.
            INSP_NO = "Barcode"; ;
            MLOTID = "(성적번호)";
            COMP_WT = "0.000";
            COMP_TG_WT = "0.000";
            COMP_TOTAL = 0;

            //칭량방법 SET        
            ComponentInfo.ComponentGUID = selectccGuid;
            ComponentInfo.OPSGGUID = OPSGGUID;
            ComponentInfo.POID = POID;

            WeighInfo.ComponentGUID = selectccGuid;
            WeighInfo.OPSGGUID = OPSGGUID;
            WeighInfo.POID = POID;
            WeighInfo.RoomID = AuthRepositoryViewModel.Instance.RoomID;

            BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP br = new BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP();
            br.INDATAs.Clear();
            br.INDATAs.Add(new BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP.INDATA() { COMPONENTGUID = selectccGuid, OPSGGUID = OPSGGUID, POID = POID });
            br.OUTDATAs.Clear();
            br.OnExecuteCompleted += (brname) =>
            {
                if (br.OUTDATAs.Count > 0)
                {
                    ComponentInfo.MaterialCode = br.OUTDATAs[0].MTRLID;
                    //IsPrimeMaterial = false;
                    ComponentInfo.IsEnableMix = (br.OUTDATAs[0].ISBATCHCOMBINATION != null && br.OUTDATAs[0].ISBATCHCOMBINATION.Equals("Y")) ? true : false;
                    ComponentInfo.RawMaterialName = br.OUTDATAs[0].MTRLNAME;
                    ComponentInfo.RequiredQuantity = br.OUTDATAs[0].REQQTY.Value;
                    ComponentInfo.RequiredQuantityUnit = br.OUTDATAs[0].NOTATION;
                    ComponentInfo.RequiredQuantityUnitID = br.OUTDATAs[0].UOMID;
                    ComponentInfo.VesselTypeID = br.OUTDATAs[0].CONTAINERTYPE;
                    ComponentInfo.VesselTypeName = br.OUTDATAs[0].EQPTGRPNAME;

                    WeighInfo.VesselType = br.OUTDATAs[0].EQPTGRPNAME;
                    WeighInfo.ScalePrecision = br.OUTDATAs[0].TOLERANCE;
                    WeighInfo.WeighNote = br.OUTDATAs[0].ATTENTIONNOTE;


                    if (br.OUTDATAs[0].OVERTOLERANCE.HasValue)
                        WeighInfo.OverTolerance = (br.OUTDATAs[0].OVERTOLERANCE.Value - br.OUTDATAs[0].REQQTY.Value);
                    else
                        WeighInfo.OverTolerance = Convert.ToDecimal(0.000);

                    if (br.OUTDATAs[0].UNDERTOLERANCE.HasValue)
                        WeighInfo.UnderTolerance = (br.OUTDATAs[0].REQQTY.Value - br.OUTDATAs[0].UNDERTOLERANCE.Value);
                    else
                        WeighInfo.UnderTolerance = Convert.ToDecimal(0.000);

                    //게이지 tolerance set
                    OC_VALUE = Math.Round(WeighInfo.OverTolerance, 3);
                    UC_VALUE = Math.Round(WeighInfo.UnderTolerance, 3);

                    BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR br2 = new BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR();
                    br2.INDATAs.Clear();
                    br2.INDATAs.Add(new BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR.INDATA() { MTRLID = ComponentInfo.MaterialCode, MTATID = "SYS_M006" });
                    br2.OUTDATAs.Clear();
                    br2.OnExecuteCompleted += (br2name) =>
                    {
                        if (br.OUTDATAs.Count > 0)
                        {
                            double temp;
                            if (double.TryParse(br2.OUTDATAs[0].MTATVAL1, out temp))
                                ComponentInfo.Potency = temp;
                        }
                    };
                    br2.Execute();
                }
            };
            br.Execute();

        }

        /// <summary>
        /// 버튼 초기화
        /// </summary>
        /// <param name="toggle"></param>
        private void setButtonInit(bool toggle)
        {
            BTRESET = toggle;
            BTTARE = toggle;
            BTSCALECHANGE = toggle;
            BTTRUST = toggle;
            BTDISPENSE = toggle;
            CMPLENABLED = !toggle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brList">재조회된 Bin Area 영역</param>
        /// <returns>칭량완료여부</returns>
        private async Task<bool> setBinAreaGrid(string currentMlot, string currentComponentGuid, bool dspcnt = false)
        {
            int selectComponetCnt = 0;
            string selectMlotID = string.Empty;
            string selectComponentGuid = string.Empty;

            BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense br = new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense();

            br.INDATAs.Add(
            new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense.INDATA
            {
                POID = POID,
                OPSGGUID = OPSGGUID,
                WEIGHINGMETHOD = WEIGHINGMETHOD,
                COMPONENTGUID = COMPONENTGUID
            });
            if (await br.Execute())
            {
                try
                {
                    foreach (BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense.OUTDATA outRow in br.OUTDATAs)
                    {
                        string disPenseCompleteYN = Math.Round(Convert.ToDecimal(outRow.REQQTY), 3) <= Math.Round(Convert.ToDecimal(outRow.DSPQTY), 3) ? "C" : "Y";

                        if ((outRow.SUMMARYYN.ToUpper().Equals("Y") && disPenseCompleteYN.Equals("C"))
                            || outRow.SUMMARYYN.ToUpper().Equals("N")
                            || (!(outRow.MLOTID == currentMlot && outRow.COMPONENTGUID == currentComponentGuid) && "Init" != currentMlot)
                            )
                            continue;

                        selectMlotID = outRow.MLOTID;
                        selectComponentGuid = outRow.COMPONENTGUID;

                        if (dspcnt)
                            outRow.DSPCNT = 0;

                        //선택된 원료로 Binding
                        setSelectComponent(
                            outRow.COMPONENTGUID,
                            outRow.COMPONENTCODE,
                            outRow.MTRLNAME,
                            Convert.ToDecimal(outRow.REQQTY),
                            Convert.ToDecimal(outRow.DSPQTY),
                            selectMlotID,
                            outRow.NOTATION,
                            outRow.DSPCNT > 0 ? true : false
                            );


                        selectComponetCnt++;
                        break;
                    }
                    
                    return selectComponetCnt == 0 ? true : false;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
                return false;
        }

        private decimal ConvertWeightByUnit(decimal weight)
        {

            if (developMode.Equals("Y"))
                return weight;

            decimal returnWeight = 0;
            string uBom = SC_BOM_UNIT.ToLower();

            //단위 환산
            if (!string.IsNullOrEmpty(tempUnit) && !string.IsNullOrEmpty(uBom) && tempUnit != uBom)
            {
                if (uBom.Equals("mg"))
                {
                    if (tempUnit.Equals("kg"))
                    {
                        returnWeight = Math.Round((weight * 1000000), 3);
                    }
                    else if (tempUnit.Equals("g"))
                    {
                        returnWeight = Math.Round((weight * 1000), 3);
                    }
                }
                else if (uBom.Equals("g"))
                {
                    if (tempUnit.Equals("kg"))
                    {
                        returnWeight = Math.Round((weight * 1000), 3);
                    }
                    else if (tempUnit.Equals("mg"))
                    {
                        returnWeight = Math.Round((weight / 1000), 3);
                    }
                }
                else if (uBom.Equals("kg"))
                {
                    if (tempUnit.Equals("g"))
                    {
                        returnWeight = Math.Round((weight / 1000), 3);
                    }
                    else if (tempUnit.Equals("mg"))
                    {
                        returnWeight = Math.Round((weight / 1000000), 3);
                    }
                }
                else
                    returnWeight = weight;
            }
            else
            {
                returnWeight = weight;
            }

            return returnWeight;
        }

        private decimal caculateCurrentDspQty(decimal scaleWeight)
        {
            decimal returnQty = 0;
            decimal userTotalQty = 0;

            if (_mainUsedList == null || _mainUsedList.Count == 0 || scaleWeight <= 0)
                returnQty = scaleWeight;
            else
            {
                foreach (UsedSourceContainerInfo info in _mainUsedList)
                {
                    userTotalQty += info.UsedWeight;
                }
                returnQty = scaleWeight - userTotalQty;
            }

            return returnQty;

        }
        
        private UsedSourceContainerInfo createUsedSourceContainer(SourceContainerInformation info, decimal currentDspQty)
        {
            if (!string.IsNullOrEmpty(info.SourceContainerNo))
            {
                return new UsedSourceContainerInfo()
                {
                    OrderNo = POID,
                    ComponentCode = ComponentInfo.MaterialCode,
                    MaterialName = ComponentInfo.RawMaterialName,

                    SourceContainerNo = info.SourceContainerNo, //
                    ContainerWeight = info.ContainerWeight,
                    SourceWeight = Convert.ToDecimal(COMP_TG_WT),  //
                    Unit = info.Unit,
                    UsedWeight = Math.Round(ConvertWeightByUnit(currentDspQty), info.Precision), //단위가 다른경우 환산 반영 
                    Potency = info.Potency,
                    SourceBarcode = info.BarCode,  //
                    Ver = info.Ver,
                    RemainedWeight = Math.Round(currentDspQty - Convert.ToDecimal(COMP_TG_WT), info.Precision),
                    PotencyCoeifficient = WeighContainerInfo.CurrentPotencyCoefficient
                };
            }
            return null;
        }
        
        private async Task DoSourceChangeWeighing(UsedSourceContainerInfo uInfo)
        {
            _allCompleteYn = false;
            BR_RHR_REG_MaterialSubLot_Dispense_Multi br = new BR_RHR_REG_MaterialSubLot_Dispense_Multi();
            br.INDATAs.Clear();

            string depleteFlag = "";
            if (uInfo.IsDeplete)
                depleteFlag = "D";

            else if (uInfo.IsPrint)
                depleteFlag = "R";

            else if (uInfo.IsScrap)
                depleteFlag = "S";

            else if (uInfo.IsPass)
                depleteFlag = "P";

            string compFlag = "Y";

            string paramVessel = string.Empty;
            if (!string.IsNullOrEmpty(sourceChangeSubLot))
                paramVessel = sourceChangeSubLot;
            else if (DEST_NO != "BIN ID" && !string.IsNullOrEmpty(DEST_NO))
                paramVessel = DEST_NO;

            br.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Multi.INDATA()
            {
                COMPONENTGUID = SelectedComponentguid,
                DEPLETFLAG = depleteFlag,
                DISPENSEQTY = Convert.ToSingle(uInfo.UsedWeight),
                INSDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                INSSIGNATURE = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                ISDISPSTRT = compFlag, //_wStatus == weighingMethodStatus.complete ? "C" : "Y",
                LOCATIONID = WeighInfo.RoomID, //WeighInitListInfo.RoomName,                    
                MSUBLOTID = uInfo.SourceContainerNo,
                MSUBLOTBCD = uInfo.SourceBarcode,
                MSUBLOTTYPE = "DSP",
                POID = POID,
                TARE = Convert.ToSingle(SC_TAREWT),
                VESSELID = paramVessel,
                ACTID = "Dispensing",
                //Biz Rule이 2명 서명이나, 1명만 허용토록 하므로 똑같이 넣어준다....
                CHECKINDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                CHECKINUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                INVENTORYQTY = Convert.ToSingle(uInfo.SourceWeight),
                WEIGHINGMETHOD = WEIGHINGMETHOD,
                UPPERVALUE = Convert.ToDecimal(OC_VALUE),
                LOWERVALUE = Convert.ToDecimal(UC_VALUE),
                LOTTYPE = string.Empty,
                OPSGGUID = new Guid(OPSGGUID),
                TAREWEIGHT = Convert.ToSingle(SC_TAREWT),
                TAREUOMID = SC_UNIT,
                REASON = uInfo.Comments,
                SCALEID = ScaleID,

                //칭량시작시간(초기화버튼 누른 시간), 칭량완료시간(현재시간) , 칭량실 ID
                DSPSTRTDTTM = null,
                DSPENDDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID
            });

            br.OUTDATAs.Clear();

            try
            {
                if (await br.Execute() == false) return;
                br.OnExecuteFailed += (executebrname, ex) =>
                {
                    IsProcess = false;
                    //OnMessage(ex.Message, ex.StackTrace);
                };

                if (br.OUTDATAs.Count > 0)
                {
                    if (uInfo.IsPrint)
                        //showSourceLabelPrinting(SystemInformation.AuthContainer, uInfo.SourceContainerNo);
                        showSourceLabelPrinting(SystemInformation.AuthContainer, br.OUTDATAs[0].DSPMSUBLOTID);

                    sourceChangeSubLot = br.OUTDATAs[0].DSPMSUBLOTID;
                }

            }
            catch (Exception ex)
            {
                IsProcess = false;
                throw ex;
            }

        }

        private async Task saveTempWeighing(UsedSourceContainerInfo tempInfo)
        {
            //임시저장 및 Gare Bin Area 반영
            //임시저장 execute, loadcomplete 후            

            //원료백변경 시 소분 저장
            try
            {
                IsProcess = true;
                await DoSourceChangeWeighing(tempInfo);
            }
            catch (Exception e)
            {
                OnMessage(e.Message);
            }
            finally
            {
                IsProcess = false;
            }
        }

        async private void SetReset()
        {
            try
            {
                if (_SelectedScale != null)
                {
                    if (_SelectedScale.Interface.ToUpper().Equals("MODBUS") || _SelectedScale.Interface.ToUpper().Equals("SBI"))
                    {
                        _BR_PHR_REG_ScaleReset.INDATAs.Clear();
                        _BR_PHR_REG_ScaleReset.INDATAs.Add(new BR_PHR_REG_ScaleReset.INDATA()
                        {
                            ScaleID = _SelectedScale.ID
                        });
                        await _BR_PHR_REG_ScaleReset.Execute();
                        _GageActivate = false;
                    }
                }
                else
                {
                    //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                }
            }
            catch (Exception) { }
        }

        async private void SetTare()
        {
            try
            {
                //현재 무게 용기 설정 
                SC_TAREWT = SC_CURRENTWT;

                if (_SelectedScale != null)
                {
                    if (_SelectedScale.Interface.ToUpper().Equals("MODBUS") || _SelectedScale.Interface.ToUpper().Equals("SBI"))
                    {
                        _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                        _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA()
                        {
                            ScaleID = _SelectedScale.ID
                        });

                        await _BR_PHR_REG_ScaleSetTare.Execute();
                        _GageActivate = true;
                    }
                }
                else
                {
                    //OnMessage("저울이 선택되지 않았거나, 저울 접속이 필요합니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10642"));
                }
            }
            catch (Exception) { }
        }
        /// <summary>
        /// 원료바코드, 원료중량, 성적번호, TARE 중량 초기화
        /// </summary>
        private void setInitInsp()
        {
            this.INSP_NO = "Barcode";
            this.COMP_WT = "0.000";
            this.COMP_TG_WT = "0.000";
            this.COMP_TOTAL = 0;
            this.MLOTID = "(성적번호)";
            this.SC_TAREWT = "0.000";
            this.SC_BTTAREWT = "0.000";

            if (WEIGHINGMETHOD == "WH002")
            {
                if (string.IsNullOrEmpty(DEST_NO))
                    this.DEST_NO = "BIN ID";
            }

            else
            {
                this.DEST_NO = "BIN ID";
            }


            _sourcePopupOpened = false;
            //WeighContainerInfo.WeighContainerNo = string.Empty;
        }

        private void setScaleValue(string weight)
        {
            SC_CURRENTWT = weight;
        }

        private void setScaleGageValue(decimal weight)
        {
            if (!string.IsNullOrEmpty(SC_UNIT))
                tempUnit = SC_UNIT.ToLower();

            decimal convertWeight = ConvertWeightByUnit(weight);

            //BOM과 실제무게 단위가 틀린경우 변환
            SC_VALUE = _currentGageValue + convertWeight;
            SC_TOTALVALUE = _currentGageValue + convertWeight;
        }

        private void GetWeight()
        {
            setScaleValue(DateTime.Now.Second.ToString());

            if (_SelectedScale != null)
            {
                if (_repeater == null || _repeater.IsEnabled == false)
                {
                    _repeater = new DispatcherTimer();
                    _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
                    _repeater.Tick += async (s, e) =>
                    {
                        try
                        {
                            _BR_PHR_SEL_CurrentWeight.INDATAs.Clear();
                            _BR_PHR_SEL_CurrentWeight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                            {
                                ScaleID = _SelectedScale.ID
                            });

                            if (await _BR_PHR_SEL_CurrentWeight.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent, Common.enumBizRuleInDataParsingType.Property, false) && _BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                            {
                                string curWeight = string.Format("{0:F3}", _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight);
                                this.SC_UNIT = _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                if (_repeater != null && _repeater.IsEnabled)
                                    setScaleValue(curWeight);
                                if (_GageActivate)
                                    setScaleGageValue(Convert.ToDecimal(curWeight));
                                else
                                    setScaleGageValue(0);

                                SC_TEXT = "(저울중량)";
                            }
                            else
                            {
                                SC_TEXT = "(저울중량) - 통신오류";
                            }
                        }
                        catch (TimeoutException er)
                        {
                            _repeater.Stop();
                            _repeater = null;
                            OnMessage(er.Message);
                        }
                        catch (FaultException ef)
                        {
                            _repeater.Stop();
                            _repeater = null;
                            OnMessage(ef.Message);
                        }
                    };
                    _repeater.Start();
                }
                else
                {
                    _repeater.Stop();
                    Thread.Sleep(100);
                    _repeater.Start();
                }
            }
        }

        private void Connect()
        {
            try
            {
                IsProcess = true;
                if (_SelectedScale != null)
                {
                    if (_repeater != null)
                    {
                        _repeater.Stop();
                        _repeater = null;
                    }
                    if (_SelectedScale.Interface.ToUpper().Equals("MODBUS") || _SelectedScale.Interface.ToUpper().Equals("SBI"))
                    {
                        if (_repeater != null)
                        {
                            _repeater.Stop();
                            _repeater = null;
                        }
                            
                        GetWeight();
                    }

                }
                else
                    //OnMessage("저울을 선택하세요.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10628"));
            }
            catch (FaultException fex)
            {
                OnMessage(fex.Message, fex.StackTrace);
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message, ex.StackTrace);
            }
            finally
            {
                IsProcess = false;
            }
        }

        //원료바코드 Popup
        private void SourceBarcodePopup()
        {
            this.S_BARCODE = string.Empty;
            this.S_MLOTID = string.Empty;

            SourceBarcodePopup sourcePopup = new SourceBarcodePopup();
            sourcePopup.DataContext = this;

            sourcePopup.OKButton.Click += (sender, arg) =>
            {
                //성적번호가 없는 경우, 중량입력 불가
                if (S_BARCODE.Length == 0 || S_MLOTID.Length == 0)
                {
                    //OnMessage("원료 바코드를 Scan하세요.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10631"));
                    return;
                }
                else if (Convert.ToDouble(S_WEIGHT) <= 0)
                {
                    //OnMessage("원료 중량을 입력하세요.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10636"));
                    return;
                }
                else
                {
                    //다음원료 Scan확인
                    if (_mainSourceList != null && _mainSourceList.Count > 0)
                    {
                        int currentIdx = _mainSourceList.Count - 1;   //마지막 소스 인덱스

                        if (Convert.ToDecimal(SC_CURRENTWT) > 0)
                        {
                            decimal curDspQty = caculateCurrentDspQty(Convert.ToDecimal(SC_CURRENTWT));

                            C1MessageBox.Show(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10645"), "Confirm", C1MessageBoxButton.YesNo, C1MessageBoxIcon.Question,
                            (result) =>
                            {
                                // 저장
                                if (result == MessageBoxResult.Yes)
                                {
                                    UsedSourceContainerInfo info = createUsedSourceContainer(_mainSourceList[currentIdx], curDspQty);

                                    info.IsPrint = false;
                                    info.IsPass = true;
                                    info.IsDeplete = false;
                                    info.IsScrap = false;

                                    //첫번째 칭량 Used List 저장 
                                    _mainUsedList.Add(info);
                                    //임시저장
                                    saveTempWeighing(info);

                                    // 두번째 원료 Add 
                                    _mainSourceList.Add(SourceContainerInfo);
                                    //}
                                }
                                //칭량 원료 저장하시겠습니까? "아니오"
                                else
                                {
                                    // 첫번째 원료 Remove
                                    _mainSourceList.RemoveAt(currentIdx);
                                    // 두번째 원료 Add
                                    _mainSourceList.Add(SourceContainerInfo);
                                }

                                this.INSP_NO = S_BARCODE;
                                this.COMP_WT = S_WEIGHT;
                                this.COMP_TG_WT = S_WEIGHT;
                                this.COMP_TOTAL += Convert.ToDouble(S_WEIGHT);
                                this.MLOTID = S_MLOTID;

                            });
                        }
                        //저울의 값이 0인 경우, 첫번째 원료 삭제 후, 두번째 원료 저장
                        else
                        {
                            // 첫번째 원료 Remove
                            _mainSourceList.RemoveAt(currentIdx);

                            // 두번째 원료 Add
                            _mainSourceList.Add(SourceContainerInfo);

                            this.INSP_NO = S_BARCODE;
                            this.COMP_WT = S_WEIGHT;
                            this.COMP_TG_WT = S_WEIGHT;
                            this.COMP_TOTAL += Convert.ToDouble(S_WEIGHT);
                            this.MLOTID = S_MLOTID;
                        }
                    }
                    //맨 처음원료 저장
                    else
                    {
                        ////임시저장된 데이타 반영
                        //if (_mainUsedList != null && _mainUsedList.Count > 0)
                        //{
                        //    double curDspQty = caculateCurrentDspQty(Convert.ToDouble(SC_CURRENTWT));                        
                        //}

                        _mainSourceList = new ObservableCollection<SourceContainerInformation>();
                        _mainSourceList.Add(SourceContainerInfo);

                        this.INSP_NO = S_BARCODE;
                        this.COMP_WT = S_WEIGHT;
                        this.COMP_TG_WT = S_WEIGHT;
                        this.COMP_TOTAL += Convert.ToDouble(S_WEIGHT);
                        this.MLOTID = S_MLOTID;
                    }

                    //SetMessageArea(weighingMessage.input, string.Empty);
                    sourcePopup.DialogResult = true;
                }
                _sourcePopupOpened = false;
            };
            sourcePopup.CancelButton.Click += (sender, arg1) =>
            {
                _sourcePopupOpened = true;
                sourcePopup.DialogResult = true;
            };

            sourcePopup.Closed += (sender, arg2) =>
            {

            };
            sourcePopup.Show();
        }

        //용기바코드 Popup
        private void BinBarcodePopup()
        {
            binPopup = new BinBarcodePopup();
            binPopup.DataContext = this;
            binPopup.txtBinBarcode.Text = string.Empty;

            binPopup.OKButton.Click += (sender, arg) =>
            {
                if (WeighContainerInfo.WeighContainerNo == null || binPopup.txtBinBarcode.Text.Length == 0)
                {
                    //OnMessage("소분용기를 Scan하세요.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10389"));
                    return;
                }
                else
                {
                    DEST_NO = binPopup.txtBinBarcode.Text;
                    binPopup.DialogResult = true;
                }
            };
            binPopup.CancelButton.Click += (sender, arg1) =>
            {
                binPopup.DialogResult = false;
            };
            binPopup.Closed += (sender, arg2) =>
            {
                //this.SetMessageArea(weighingMessage.tare, string.Empty);
            };
            binPopup.Show();
        }

        private async void ResetExecute(object param)
        {
            try
            {
                if (_mainSourceList != null && _mainSourceList.Count > 0)
                {
                    C1MessageBox.Show(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10643"), "Confirm", C1MessageBoxButton.YesNo, C1MessageBoxIcon.Question,
                    (result) =>
                    {
                        if (result == MessageBoxResult.Yes)
                        {
                            _mainSourceList = null;
                            _mainUsedList = null;
                            _mainUsedList = new ObservableCollection<UsedSourceContainerInfo>();

                            //임시데이타 삭제
                            //removeTempDspData(POID, SelectedComponentguid, SelectedMLotID);

                            SetReset();
                            setInitInsp();
                            //SetMessageArea(weighingMessage.scanWeighCont, string.Empty);

                            setBinAreaGrid(SelectedMLotID, SelectedComponentguid);

                        }
                        else
                            return;
                    });
                }
                else
                {
                    SetReset();
                    setInitInsp();
                    //SetMessageArea(weighingMessage.scanWeighCont, string.Empty);

                    if (!string.IsNullOrEmpty(SelectedMLotID))
                        await setBinAreaGrid(SelectedMLotID, SelectedComponentguid);

                    else
                        await setBinAreaGrid("Init", string.Empty);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void TareExecute(object param)
        {
            if (Convert.ToDouble(SC_CURRENTWT) < 0)
            {
                //OnMessage("용기를 올리세요.");
                OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10634"));
                return;
            }
            SetTare();
            //SetMessageArea(weighingMessage.scan, string.Empty);
        }

        private async Task TrustWeigh(ObservableCollection<UsedSourceContainerInfo> infos)
        {
            decimal totalUsedWeight = 0;
            string m_componentGuid = SelectedComponentguid;
            string m_mlotid = SelectedMLotID;
            string compFlag = "Y";

            BR_RHR_REG_MaterialSubLot_Dispense_Multi br = new BR_RHR_REG_MaterialSubLot_Dispense_Multi();
            br.INDATAs.Clear();

            int cnt = 0;
            foreach (UsedSourceContainerInfo uInfo in infos)
            {
                compFlag = "Y";
                if (infos.Count - 1 == cnt && _wStatus == weighingMethodStatus.complete)
                    compFlag = "C";

                br.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Multi.INDATA()
                {
                    COMPONENTGUID = SelectedComponentguid,
                    DEPLETFLAG = "P",
                    DISPENSEQTY = Convert.ToSingle(uInfo.UsedWeight),
                    INSDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                    INSSIGNATURE = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                    ISDISPSTRT = compFlag,//"Y",
                    LOCATIONID = WeighInfo.RoomID, //WeighInitListInfo.RoomName,
                    //MSUBLOTID = ComponentInfo.MaterialSublotID, // 오송요청수정    
                    MSUBLOTID = uInfo.SourceContainerNo,
                    MSUBLOTBCD = uInfo.SourceBarcode,
                    MSUBLOTTYPE = "DSP",
                    POID = POID,
                    TARE = Convert.ToSingle(SC_TAREWT),
                    VESSELID = (DEST_NO == "BIN ID" ? string.Empty : DEST_NO),
                    ACTID = "Dispensing",
                    //Biz Rule이 2명 서명이나, 1명만 허용토록 하므로 똑같이 넣어준다....
                    CHECKINDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                    CHECKINUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                    INVENTORYQTY = Convert.ToSingle(uInfo.UsedWeight),
                    WEIGHINGMETHOD = "WH004",
                    UPPERVALUE = Convert.ToDecimal(OC_VALUE),
                    LOWERVALUE = Convert.ToDecimal(UC_VALUE),
                    LOTTYPE = string.Empty,
                    OPSGGUID = new Guid(OPSGGUID),
                    TAREWEIGHT = Convert.ToSingle(SC_TAREWT),
                    TAREUOMID = SC_UNIT,
                    REASON = string.Empty,
                    SCALEID = ScaleID,

                    //칭량시작시간(초기화버튼 누른 시간), 칭량완료시간(현재시간) , 칭량실 ID
                    DSPSTRTDTTM = null,
                    DSPENDDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID
                });

                cnt++;
                totalUsedWeight += uInfo.UsedWeight;
            }
            br.OUTDATAs.Clear();

            if (await br.Execute() == false) return;

            br.OnExecuteFailed += (executebrname, ex) =>
            {
                IsProcess = false;
                //OnMessage(ex.Message, ex.StackTrace);
            };

            _currentGageValue += totalUsedWeight;

            if (compFlag.Equals("C"))
            {
                _allCompleteYn = await setBinAreaGrid("Init", string.Empty);     //true 칭량 완료   

                if (_allCompleteYn)
                {
                    //CMPLENABLED = true;
                    //SetMessageArea(weighingMessage.complete, string.Empty);
                }
                else
                {
                    _currentGageValue = Convert.ToDecimal(0.000);
                    //this.SetMessageArea(weighingMessage.reset, string.Empty);
                }
            }
            else
            {
                //BIN AREA 재 조회 
                brComList.INDATAs.Clear();
                brComList.OUTDATAs.Clear();
                brComList.INDATAs.Add(
                new BR_PHR_SEL_WeighingSchedule_ComponentList_Dispense.INDATA
                {
                    POID = POID,
                    OPSGGUID = OPSGGUID,
                    WEIGHINGMETHOD = WEIGHINGMETHOD,
                    COMPONENTGUID = COMPONENTGUID
                });

                if (await brComList.Execute() == false) return;
            }

            foreach (BR_RHR_REG_MaterialSubLot_Dispense_Multi.OUTDATA oput in br.OUTDATAs)
            {
                await showDispensingPrinting(SystemInformation.AuthContainer, oput.DSPMSUBLOTID);
                Thread.Sleep(100);
            }
        }

        private async System.Threading.Tasks.Task ScaleUseHistory(UsedSourceContainerInfo info)
        {
            try
            {
                if (_SelectedScale != null)
                {
                    var bizRuleUse = _SelectedScale.PrepareScaleUse(
                        BATCHNO,
                        info.MaterialName,
                        info.UsedWeight.ToString(),
                        info.SourceContainerNo,
                        AuthRepositoryViewModel.GetCommentByFunctionCode("WM_Weighing_Dispensing"),
                        "WM_Weighing_Dispensing");

                    var bizRuleComplete = _SelectedScale.CompleteScaleUse();

                    if (await bizRuleUse.Execute() == false) return;
                    if (await bizRuleComplete.Execute() == false) return;
                }
            }
            catch (Exception Ex)
            {
                OnMessage(Ex.ToString(), Ex.StackTrace);
            }
        }

        private async System.Threading.Tasks.Task ScaleUseHistory(ObservableCollection<UsedSourceContainerInfo> infos)
        {
            try
            {
                foreach (UsedSourceContainerInfo info in infos)
                {
                    await ScaleUseHistory(info);
                }
            }
            catch (Exception Ex)
            {
                IsProcess = false;
                OnMessage(Ex.ToString(), Ex.StackTrace);
            }
        }

        private async Task TrustDispense()
        {
            if (_TParameters.Count == 0)
            {
                //OnMessage("신뢰소분할 대상이 없습니다.");
                OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10641"));
            }
            else
            {
                //SC_TARGET 목표중량
                decimal trustTotalWeight = 0;

                foreach (TrustListParameter trInfo in _TParameters)
                {
                    await SourceContainerInfo.LoadDataTrustCmd(WeighInfo, trInfo.BarCode);

                    if (SourceContainerInfo.SourceContainerNo != null)
                    {
                        UsedSourceContainerInfo info = new UsedSourceContainerInfo()
                        {
                            OrderNo = POID,
                            ComponentCode = ComponentInfo.MaterialCode,
                            MaterialName = ComponentInfo.RawMaterialName,

                            SourceContainerNo = SourceContainerInfo.SourceContainerNo,
                            ContainerWeight = SourceContainerInfo.ContainerWeight,
                            SourceWeight = Convert.ToDecimal(trInfo.MWeight),
                            Unit = SourceContainerInfo.Unit,
                            UsedWeight = Convert.ToDecimal(trInfo.MWeight),
                            Potency = SourceContainerInfo.Potency,
                            SourceBarcode = trInfo.BarCode,
                            Ver = SourceContainerInfo.Ver,
                            RemainedWeight = Convert.ToDecimal(0.000),
                            PotencyCoeifficient = WeighContainerInfo.CurrentPotencyCoefficient
                        };

                        _mainUsedList.Add(info);

                        trustTotalWeight += Convert.ToDecimal(trInfo.MWeight);
                    }
                }

                // 신뢰칭량 총 중량이 목표치 이상이면 완료 
                if (trustTotalWeight + _currentGageValue >= SC_TARGET)
                    _wStatus = weighingMethodStatus.complete;
            }

            try
            {
                IsProcess = true;
                await TrustWeigh(_mainUsedList);
                await ScaleUseHistory(_mainUsedList);
            }

            catch (Exception e)
            {
                OnMessage(e.Message);
            }
            finally
            {
                IsProcess = false;

                _mainSourceList = null;
                _mainUsedList.Clear();
                _mainUsedList = new ObservableCollection<UsedSourceContainerInfo>();
            }
        }

        /// <summary>
        /// 일반칭량 소분
        /// </summary>
        /// <param name="info">UsedSourceContainerInfo</param>
        /// <param name="depleteFlag">잔량처리FLAG</param>
        /// <returns></returns>
        private async Task DoWeighing(ObservableCollection<UsedSourceContainerInfo> infos)
        {
            _allCompleteYn = false;
            BR_RHR_REG_MaterialSubLot_Dispense_Multi br = new BR_RHR_REG_MaterialSubLot_Dispense_Multi();
            br.INDATAs.Clear();

            int cnt = 0;
            foreach (UsedSourceContainerInfo uInfo in infos)
            {
                //매 원료백변경마다 저장하므로 마지막것만 저장한다.
                if (cnt != infos.Count - 1)
                {
                    cnt++;
                    continue;
                }

                string depleteFlag = "";
                if (uInfo.IsDeplete)
                    depleteFlag = "D";

                else if (uInfo.IsPrint)
                    depleteFlag = "R";

                else if (uInfo.IsScrap)
                    depleteFlag = "S";

                else if (uInfo.IsPass)
                    depleteFlag = "P";

                string compFlag = "Y";
                if (infos.Count - 1 == cnt && _wStatus == weighingMethodStatus.complete)
                    compFlag = "C";

                string paramVessel = string.Empty;
                if (!string.IsNullOrEmpty(sourceChangeSubLot))
                    paramVessel = sourceChangeSubLot;
                else if (DEST_NO != "BIN ID" && !string.IsNullOrEmpty(DEST_NO))
                    paramVessel = DEST_NO;

                br.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Multi.INDATA()
                {
                    COMPONENTGUID = SelectedComponentguid,
                    DEPLETFLAG = depleteFlag,
                    DISPENSEQTY = Convert.ToSingle(uInfo.UsedWeight),
                    INSDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                    INSSIGNATURE = AuthRepositoryViewModel.Instance.ConfirmedGuid,
                    ISDISPSTRT = compFlag, //_wStatus == weighingMethodStatus.complete ? "C" : "Y",
                    LOCATIONID = WeighInfo.RoomID, //WeighInitListInfo.RoomName,                    
                    MSUBLOTID = uInfo.SourceContainerNo,
                    MSUBLOTBCD = uInfo.SourceBarcode,
                    MSUBLOTTYPE = "DSP",
                    POID = POID,
                    TARE = Convert.ToSingle(SC_TAREWT),
                    VESSELID = paramVessel,
                    ACTID = "Dispensing",
                    //Biz Rule이 2명 서명이나, 1명만 허용토록 하므로 똑같이 넣어준다....
                    CHECKINDTTM = AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing") == null ? DateTime.Now : AuthRepositoryViewModel.GetDateTimeByFunctionCode("WM_Weighing_Dispensing"),
                    CHECKINUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing") == null ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("WM_Weighing_Dispensing"),
                    INVENTORYQTY = Convert.ToSingle(uInfo.SourceWeight),
                    WEIGHINGMETHOD = WEIGHINGMETHOD,
                    UPPERVALUE = Convert.ToDecimal(OC_VALUE),
                    LOWERVALUE = Convert.ToDecimal(UC_VALUE),
                    LOTTYPE = string.Empty,
                    OPSGGUID = new Guid(OPSGGUID),
                    TAREWEIGHT = Convert.ToSingle(SC_TAREWT),
                    TAREUOMID = SC_UNIT,
                    REASON = uInfo.Comments,
                    SCALEID = ScaleID,

                    //칭량시작시간(초기화버튼 누른 시간), 칭량완료시간(현재시간) , 칭량실 ID
                    DSPSTRTDTTM = _dspstrtDTTM,
                    DSPENDDTTM = (await AuthRepositoryViewModel.GetDBDateTimeNow()).ToString("yyyy-MM-dd HH:mm:ss"),
                    WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID
                });
                cnt++;
            }
            br.OUTDATAs.Clear();

            try
            {
                if (await br.Execute() == false) return;
                br.OnExecuteFailed += (executebrname, ex) =>
                {
                    IsProcess = false;
                    //OnMessage(ex.Message, ex.StackTrace);
                };


                bool IsSourcePrint = false;
                if (infos.Count > 0 && infos[infos.Count - 1].IsPrint)
                    IsSourcePrint = true;

                //소분
                if (_wStatus == weighingMethodStatus.split)
                {
                    await setBinAreaGrid(SelectedMLotID, SelectedComponentguid, true);

                    //this.SetMessageArea(weighingMessage.reset, string.Empty);

                    OnMessage("소분되었습니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10638"));
                }
                //완료
                else if (_wStatus == weighingMethodStatus.complete)
                {
                    BR_PHR_UPD_ProductionOrderBOMVersion_ISDISPSTRT startbr = new BR_PHR_UPD_ProductionOrderBOMVersion_ISDISPSTRT();
                    //BR_PHR_REG_ProductionOrderBOMVersion_ISDISPSTRT startbr = new BR_PHR_REG_ProductionOrderBOMVersion_ISDISPSTRT();
                    startbr.INDATAs.Clear();
                    startbr.INDATAs.Add(new BR_PHR_UPD_ProductionOrderBOMVersion_ISDISPSTRT.INDATA() { ISDISPSTRT = "C", POID = POID, COMPONENTGUID = new Guid(SelectedComponentguid) });

                    // 칭량 완료처리
                    if (await startbr.Execute() == false) return;

                    _allCompleteYn = await setBinAreaGrid("Init", string.Empty);     //true 칭량 완료   

                    if (_allCompleteYn)
                    {
                        ////레이어 칭량시 등록해준다.
                        //if (WEIGHINGMETHOD == "WH002")
                        //{
                        //    BR_PHR_CHECK_WeighingSchedule_NEW nextbr = new BR_PHR_CHECK_WeighingSchedule_NEW();
                        //    nextbr.INDATAs.Clear();
                        //    nextbr.INDATAs.Add(new BR_PHR_CHECK_WeighingSchedule_NEW.INDATA() { ROOMID = WeighInfo.RoomID, COMPONENTGUID = SelectedComponentguid, OPSGGUID = OPSGGUID, POID = POID, USERID = LGCNS.EZMES.Common.LogInInfo.UserID });
                        //    nextbr.INDATA2s.Clear();
                        //    nextbr.INDATA2s.Add(new BR_PHR_CHECK_WeighingSchedule_NEW.INDATA2()
                        //    {
                        //        EQSJUSER = LGCNS.EZMES.Common.LogInInfo.UserID
                        //    });
                        //    nextbr.Execute();
                        //}

                        CMPLENABLED = true;
                        //SetMessageArea(weighingMessage.complete, string.Empty);
                    }
                    else
                    {
                        //this.SetMessageArea(weighingMessage.reset, string.Empty);
                        _currentGageValue = Convert.ToDecimal(0.000);
                    }

                    //OnMessage("완료되었습니다.");
                    OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10639"));

                    if (WEIGHINGMETHOD == "WH002")
                        OnMessage("Bin에 원료를 넣으세요.");
                }

                INSP_NO = "Barcode"; //성적번호 초기화
                MLOTID = "(성적번호)";
                COMP_WT = "0.000"; //원료중량 초기화
                COMP_TG_WT = "0.000"; //원료중량 초기화
                COMP_TOTAL = 0;
                _GageActivate = false;

                //잔량라벨 출력         
                if (IsSourcePrint)
                {
                    if (br.OUTDATAs.Count > 0)
                        //showSourceLabelPrinting(SystemInformation.AuthContainer, infos[infoIdx].SourceContainerNo);
                        showSourceLabelPrinting(SystemInformation.AuthContainer, br.OUTDATAs[0].DSPMSUBLOTID);
                }

                //소분라벨 출력 
                if (WEIGHINGMETHOD == "WH001")
                {
                    if (br.OUTDATAs.Count > 0)
                    {
                        await showDispensingPrinting(SystemInformation.AuthContainer, br.OUTDATAs[0].DSPMSUBLOTID);
                    }
                    else
                        //OnMessage("칭량Label을 위한 소분ID가 없습니다.");
                        OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10640"));
                }

                _wStatus = weighingMethodStatus.none;
                sourceChangeSubLot = string.Empty;  //원료백변경 Sublot 초기화
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 소분처리 
        /// </summary>
        /// <param name="info">UsedSourceContainerInfo</param>
        /// <returns></returns>
        private async Task DoDispense(ObservableCollection<UsedSourceContainerInfo> infos)
        {
            try
            {
                IsProcess = true;
                await DoWeighing(infos);
                await ScaleUseHistory(infos);
            }
            catch (Exception e)
            {
                OnMessage(e.Message);
            }
            finally
            {
                IsProcess = false;

                // Container 초기화
                _mainSourceList = null;
                _mainUsedList.Clear();
                _mainUsedList = new ObservableCollection<UsedSourceContainerInfo>();
            }
        }

        private void DispenseExcute()
        {
            if (string.IsNullOrEmpty(SourceContainerInfo.BarCode))
            {
                //OnMessage("칭량원료가 없습니다.");
                OnMessage(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10633"));
                if (_repeater != null && !_repeater.IsEnabled)
                    _repeater.Start();
            }
            else
            {
                decimal curDspQty = caculateCurrentDspQty(Convert.ToDecimal(SC_CURRENTWT)); // 현재 무게에서 소분할 무게만 추출            

                UsedSourceContainerInfo info = createUsedSourceContainer(SourceContainerInfo, curDspQty);

                //완료이거나 차이가 있는 경우, 잔량처리 Popup
                if (_wStatus == weighingMethodStatus.complete || info.RemainedWeight != 0)
                {
                    if (_wStatus == weighingMethodStatus.complete)
                    {
                        // 입력중량보다 칭량중량이 작으면 BLOCKING
                        if (SC_VALUE < SC_TARGET)
                        {
                            string targetValue = string.Format("{0:F3}", SC_TARGET);
                            //OnMessage(string.Concat("지시중량보다 작게 칭량 할 수 없습니다.\r\n(입력중량:", SC_VALUE, ", 지시중량:", targetValue, ")"));
                            string phMessage = LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10635");
                            OnMessage(string.Format(phMessage, SC_VALUE, targetValue));
                            if (_repeater != null && !_repeater.IsEnabled)
                                _repeater.Start();
                            return;
                        }
                    }

                    RemainedSourceHandlingWindow popup = new RemainedSourceHandlingWindow();
                    popup.SetBinding(RemainedSourceHandlingWindow.DataContextProperty, new Binding() { Source = info });
                    popup.Closed += (s, e) =>
                    {
                        RemainedSourceHandlingWindow owner = s as RemainedSourceHandlingWindow;
                        if (owner.DialogResult.HasValue && owner.DialogResult.Value)
                        {
                            //완료버튼 누른 후 허용범위를 벗어나는 저울중량에 대한 Validation
                            if ((SC_TARGET + OC_VALUE) < SC_VALUE)
                            {
                                OnMessage(string.Format(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10646"), SC_VALUE.ToString()));
                            }
                            else
                            {
                                _mainUsedList.Add(info);
                                DoDispense(_mainUsedList);
                            }
                        }
                        if (_repeater != null && !_repeater.IsEnabled)
                            _repeater.Start();
                    };
                    popup.Show();
                }
                //소분
                else
                {
                    info.IsDeplete = false;
                    info.IsScrap = false;
                    info.IsPass = true;

                    //소분버튼 누른 후 변경되는 저울중량에 대한 Validation
                    if (((SC_TARGET + OC_VALUE) < SC_VALUE) || SC_VALUE <= 0)
                    {
                        OnMessage(string.Format(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "PHM10646"), SC_VALUE.ToString()));
                    }
                    else
                    {
                        _mainUsedList.Add(info);
                        DoDispense(_mainUsedList);
                    }
                    if (_repeater != null && !_repeater.IsEnabled)
                        _repeater.Start();
                }
            }
        }

        #endregion

        #region [Print Method]
        
        private void DefaultPrinterSetup()
        {
            BR_PHR_SEL_System_Printer br = new BR_PHR_SEL_System_Printer();
            br.INDATAs.Clear();
            br.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA()
            {
                ROOMID = AuthRepositoryViewModel.Instance.RoomID
            });

            br.OnExecuteCompleted += (bname) =>
            {
                foreach (var item in br.OUTDATAs)
                {
                    if (item.PRINTERTYPE.Equals("PRNTP001"))
                    {
                        _SelectedSourcePrinterName = item.PRINTERNAME;
                        SelectedDispensePrinterName = item.PRINTERNAME;
                    }
                }
            };
            br.Execute();
        }

        private void showSourceLabelPrinting(AuthorityContainer ac, string msublotid)
        {
            //OnMessage("[" + msublotid + "]잔량라벨 발행");
            //var result =
            //    Browser.HtmlPage.Window.Invoke("ReportPageBridge",
            //    "REPORT_PATH=/Reports/Label/DWS_LABEL_REMAINEDQTY" +
            //    "&MSUBLOTID=" + msublotid +
            //    "&PRINTER_NAME=" + Browser.HttpUtility.UrlEncode(_SelectedSourcePrinterName));

            for (int i = 0; i < 2; i++)
            {
                BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();
                BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();

                BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                {
                    ReportPath = "/Reports/Label/DWS_LABEL_REMAINEDQTY",
                    PrintName = Browser.HttpUtility.UrlEncode(_SelectedSourcePrinterName)
                });

                BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                {
                    ParamName = "MSUBLOTID",
                    ParamValue = msublotid
                });

                BR_PHR_SEL_PRINT_LabelImage.Execute();
            }
        }

        private async Task showDispensingPrinting(AuthorityContainer ac, string msublotid)
        {
            //BR_PHR_REG_ProductionOrderDispenseSubLot_Label.INDATAs.Add(new BR_PHR_REG_ProductionOrderDispenseSubLot_Label.INDATA()
            //{
            //    MSUBLOTID =  uInfo.SourceContainerNo
            //});
            //BR_PHR_REG_ProductionOrderDispenseSubLot_Label.execute();

            var bizRule = new BR_PHR_REG_ProductionOrderDispenseSubLot_Label();
            bizRule.INDATAs.Add(new BR_PHR_REG_ProductionOrderDispenseSubLot_Label.INDATA()
            {
                MSUBLOTID = msublotid
            });
            if (!await bizRule.Execute()) throw new Exception();

            //var result =
            //        Browser.HtmlPage.Window.Invoke("ReportPageBridge",
            //    //"REPORT_PATH=/Label/OUTPUTLABEL_NOIMAGE" +
            //        "REPORT_PATH=/Reports/Label/DWS_LABEL_WEIGHING_HISTORY" +
            //        "&MSUBLOTID=" + msublotid +
            //        "&PRINTER_NAME=" + Browser.HttpUtility.UrlEncode(SelectedDispensePrinterName));
            /*
                        //OnMessage("[" + msublotid + "]칭량라벨 발행");
                        //BR_PHR_SEL_MaterialAttachment_MSUBLOTID br = new BR_PHR_SEL_MaterialAttachment_MSUBLOTID();
                        //br.INDATAs.Clear();
                        //br.INDATAs.Add(
                        //    new BR_PHR_SEL_MaterialAttachment_MSUBLOTID.INDATA
                        //    {
                        //        MSUBLOTID = msublotid
                        //    });
                        //br.OnExecuteCompleted += (brname) =>
                        //{
                        //    if (br.OUTDATAs.Count > 0)
                        //    {
                        //        var result =
                        //            Browser.HtmlPage.Window.Invoke("ReportPageBridge",
                        //            "REPORT_PATH=/Reports/Label/Injection_06_OutputLabel" +
                        //            "&MSUBLOTID=" + msublotid +
                        //            "&PRINTER_NAME=" + Browser.HttpUtility.UrlEncode(_SelectedDispensePrinterName));
                        //    }
                        //    else
                        //    {
                        //    var result =
                        //        Browser.HtmlPage.Window.Invoke("ReportPageBridge",
                        //        "REPORT_PATH=/Label/OUTPUTLABEL_NOIMAGE" +
                        //        "REPORT_PATH=/Reports/Label/DWS_LABEL_WEIGHING" +
                        //        "&MSUBLOTID=" + msublotid +
                        //        "&PRINTER_NAME=" + Browser.HttpUtility.UrlEncode(SelectedDispensePrinterName));
                        //     "&PRINTER_NAME=" + Browser.HttpUtility.UrlEncode("ZDesigner ZT410-300dpi ZPL"));
                        //    }
            */

            //    BizRule을 이용한 라벨 발행 (바토드 인식이 안됨)
            for (int i = 0; i < 2; i++)
            {
                BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();
                BR_PHR_SEL_PRINT_LabelImage.OUTDATAs.Clear();

                BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA()
                {
                    ReportPath = "/Reports/Label/DWS_LABEL_WEIGHING",
                    PrintName = Browser.HttpUtility.UrlEncode(SelectedDispensePrinterName)
                });

                BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters()
                {
                    ParamName = "MSUBLOTID",
                    ParamValue = msublotid
                });

                await BR_PHR_SEL_PRINT_LabelImage.Execute();
            }
            //};
            //br.Execute();
        }

        #endregion

        #region [Etc ViewModelBase]

        public class WeighingComponent : ViewModelBase
        {
            private bool _isExpanded = true;

            public WeighingComponent()
            {
                SubTasks = new List<WeighingComponent>();
                IsExpanded = true;
            }

            private string _POID;
            public string POID
            {
                get
                {
                    return _POID;
                }
                set
                {
                    _POID = value;
                    OnPropertyChanged("POID");
                }
            }
            private string _WEIGHINGSEQ;
            public string WEIGHINGSEQ
            {
                get
                {
                    return _WEIGHINGSEQ;
                }
                set
                {
                    _WEIGHINGSEQ = value;
                    OnPropertyChanged("WEIGHINGSEQ");
                }
            }
            private string _COMPONENTGUID;
            public string COMPONENTGUID
            {
                get
                {
                    return _COMPONENTGUID;
                }
                set
                {
                    _COMPONENTGUID = value;
                    OnPropertyChanged("COMPONENTGUID");
                }
            }
            private string _COMPONENTCODE;
            public string COMPONENTCODE
            {
                get
                {
                    return _COMPONENTCODE;
                }
                set
                {
                    _COMPONENTCODE = value;
                    OnPropertyChanged("COMPONENTCODE");
                }
            }
            private string _MTRLNAME;
            public string MTRLNAME
            {
                get
                {
                    return _MTRLNAME;
                }
                set
                {
                    _MTRLNAME = value;
                    OnPropertyChanged("MTRLNAME");
                }
            }
            private decimal? _REQQTY;
            public decimal? REQQTY
            {
                get
                {
                    return _REQQTY;
                }
                set
                {
                    _REQQTY = value;
                    OnPropertyChanged("REQQTY");
                }
            }
            private string _NOTATION;
            public string NOTATION
            {
                get
                {
                    return _NOTATION;
                }
                set
                {
                    _NOTATION = value;
                    OnPropertyChanged("NOTATION");
                }
            }
            private string _SUMMARYYN;
            public string SUMMARYYN
            {
                get
                {
                    return _SUMMARYYN;
                }
                set
                {
                    _SUMMARYYN = value;
                    OnPropertyChanged("SUMMARYYN");
                }
            }
            private string _MLOTID;
            public string MLOTID
            {
                get
                {
                    return _MLOTID;
                }
                set
                {
                    _MLOTID = value;
                    OnPropertyChanged("MLOTID");
                }
            }
            private string _DISPENSINGCOMPLETED;
            public string DISPENSINGCOMPLETED
            {
                get
                {
                    return _DISPENSINGCOMPLETED;
                }
                set
                {
                    _DISPENSINGCOMPLETED = value;
                    OnPropertyChanged("DISPENSINGCOMPLETED");
                }
            }
            private decimal _DSPQTY;
            public decimal DSPQTY
            {
                get
                {
                    return _DSPQTY;
                }
                set
                {
                    _DSPQTY = value;
                    OnPropertyChanged("DSPQTY");
                }
            }
            private int _DSPCNT;
            public int DSPCNT
            {
                get
                {
                    return _DSPCNT;
                }
                set
                {
                    _DSPCNT = value;
                    OnPropertyChanged("DSPCNT");
                }
            }
            private string _CONTAINERCNT;
            public string CONTAINERCNT
            {
                get
                {
                    return _CONTAINERCNT;
                }
                set
                {
                    _CONTAINERCNT = value;
                    OnPropertyChanged("CONTAINERCNT");
                }
            }

            private string _CODEMATERIAL;
            public string CODEMATERIAL
            {
                get { return _CODEMATERIAL; }
                set
                {
                    _CODEMATERIAL = value;
                    OnPropertyChanged("CODEMATERIAL");
                }
            }

            [Display(AutoGenerateField = false)]
            public WeighingComponent ParentTask { get; set; }

            [Display(AutoGenerateField = false)]
            public List<WeighingComponent> SubTasks { get; set; }

            [Display(AutoGenerateField = false)]
            public bool IsExpanded
            {
                get
                {
                    return _isExpanded;
                }
                set
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnIsVisibleChanged();
                }
            }

            private void OnIsVisibleChanged()
            {
                OnPropertyChanged("IsVisible");
                foreach (var subTask in SubTasks)
                {
                    subTask.OnIsVisibleChanged();
                }
            }

            [Display(AutoGenerateField = false)]
            public bool IsVisible
            {
                get
                {
                    if (ParentTask == null)
                    {
                        return true;
                    }
                    else if (!ParentTask.IsExpanded)
                    {
                        return false;
                    }
                    else
                    {
                        return ParentTask.IsVisible;
                    }
                }
            }

            [Display(AutoGenerateField = false)]
            public int Level
            {
                get
                {
                    if (ParentTask == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return ParentTask.Level + 1;
                    }
                }
            }
        }

        public class TrustListParameter : ViewModelBase
        {
            private string _RowEditSec;
            public string RowEditSec
            {
                get
                {
                    return _RowEditSec;
                }
                set
                {
                    _RowEditSec = value;
                    OnPropertyChanged("RowEditSec");
                }
            }

            private int _RowIndex;
            public int RowIndex
            {
                get
                {
                    return _RowIndex;
                }
                set
                {
                    _RowIndex = value;
                    OnPropertyChanged("RowIndex");
                }
            }

            private string _BarCode;
            public string BarCode
            {
                get
                {
                    return _BarCode;
                }
                set
                {
                    _BarCode = value;
                    OnPropertyChanged("BarCode");
                }
            }

            private string _MWeight;
            public string MWeight
            {
                get
                {
                    return _MWeight;
                }
                set
                {
                    _MWeight = value;
                    OnPropertyChanged("MWeight");
                }
            }
        }

        #endregion
                
        

    }

}