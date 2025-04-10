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
using ShopFloorUI;
using LGCNS.iPharmMES.Common;
using System.Windows.Threading;
using C1.Silverlight.Data;
using System.Text;
using System.Threading.Tasks;
using 보령.UserControls;

namespace 보령
{
    public class 최종조제ViewModel : ViewModelBase
    {
        #region [Property]
        public 최종조제ViewModel()
        {
            _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
            _BR_PHR_SEL_System_Printer = new BR_PHR_SEL_System_Printer();
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();
            _BR_PHR_REG_ScaleSetTare = new BR_PHR_REG_ScaleSetTare();
            _BR_PHR_SEL_ProductionOrderOutput_Define = new BR_PHR_SEL_ProductionOrderOutput_Define();
            _BR_BRS_REG_ProductionOrderOutput_LastSoluction = new BR_BRS_REG_ProductionOrderOutput_LastSoluction();
            _BR_PHR_SEL_PRINT_LabelImage = new BR_PHR_SEL_PRINT_LabelImage();
            _BR_BRS_SEL_ProductionOrderOutputSubLot = new BR_BRS_SEL_ProductionOrderOutputSubLot();

            int interval = 2000;
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
        }

        private 최종조제 _mainWnd;
        private DispatcherTimer _DispatcherTimer = new DispatcherTimer(); // 저울값 타이머
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
        private BR_PHR_SEL_System_Printer.OUTDATA _selectedPrint;
        public string curPrintName
        {
            get
            {
                if (_selectedPrint != null)
                    return _selectedPrint.PRINTERNAME;
                else
                    return "N/A";
            }
        }
        private DateTime _DspStartDttm;

        /// <summary>
        /// setBeakerTare : 비커 Tare 측정단계, initial : 조제액 측정단계, add : 보충단계, end : 조제완료
        /// </summary>
        enum state
        {
            setBeakerTare, initial, add, end, exception
        };
        private state _curstate;
        public string CurrentStateSTR
        {
            get
            {
                switch (_curstate)
                {
                    case state.setBeakerTare:
                        return "용기무게측정";
                    case state.initial:
                        return "보충전조제액무게측정";
                    case state.add:
                        return "원료보충";
                    case state.end:
                        return "조제완료";
                    case state.exception:
                        return "N/A";
                    default:
                        return "N/A";
                }
            }
        }
        private Guid? _outputguid;
        private string _BeakerId;
        public string BeakerId
        {
            get { return _BeakerId; }
            set
            {
                _BeakerId = value;
                OnPropertyChanged("BeakerId");
            }
        }
        private Weight _BeakerTare = new Weight();
        public string BeakerTare
        {
            get { return _BeakerTare.WeightUOMString; }
        }

        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATA _ScaleInfo;
        private string _ScaleId;
        public string ScaleId
        {
            get { return _ScaleId; }
            set
            {
                _ScaleId = value;
                OnPropertyChanged("ScaleId");
            }
        }

        private Weight _ScaleWeight = new Weight();
        public string ScaleWeight
        {
            get { return _ScaleWeight.WeightUOMString; }
        }
        public int ScalePrecision
        {
            set
            {
                _ScaleWeight.Precision = value;
                _BeakerTare.Precision = value;
                _InitialWeight.Precision = value;
                _StandardWeight.Precision = value;
                _FinalWeight.Precision = value;
                _TargetWeight.Precision = value;
                _MinWeight.Precision = value;
                _MaxWeight.Precision = value;
                _AddWeight.Precision = value;

                WeightRefresh();
            }
        }
        public string ScaleUom
        {
            set
            {
                _ScaleWeight.Uom = value;
                _BeakerTare.Uom = value;
                _InitialWeight.Uom = value;
                _StandardWeight.Uom = value;
                _FinalWeight.Uom = value;
                _TargetWeight.Uom = value;
                _MinWeight.Uom = value;
                _MaxWeight.Uom = value;
                _AddWeight.Uom = value;

                WeightRefresh();
            }
        }       
        /// <summary>
        /// 조제액(최종조제 전)
        /// </summary>
        private Weight _InitialWeight = new Weight();
        public string InitialWeight
        {
            get { return _InitialWeight.WeightUOMString; }
        }
        /// <summary>
        /// 조제액 기준량(최종조제)
        /// </summary>
        private Weight _StandardWeight = new Weight();
        public string StandardWeight
        {
            get { return _StandardWeight.WeightUOMString; }
        }
        /// <summary>
        /// 최종조제량
        /// </summary>
        private Weight _FinalWeight = new Weight();
        public string FinalWeight
        {
            get { return _FinalWeight.WeightUOMString; }
        }
        /// <summary>
        /// 보충량(기준)
        /// </summary>
        private Weight _TargetWeight = new Weight();
        public string TargetWeight
        {
            get { return _TargetWeight.WeightUOMString; }
        }
        /// <summary>
        /// 보충량(기준) 최소값
        /// </summary>
        private Weight _MinWeight = new Weight();
        public string MinWeight
        {
            get { return _MinWeight.WeightUOMString; }
        }
        /// <summary>
        /// 보충량(기준) 최대값
        /// </summary>
        private Weight _MaxWeight = new Weight();
        public string MaxWeight
        {
            get { return _MaxWeight.WeightUOMString; }
        }
        /// <summary>
        /// 보충량
        /// </summary>
        private Weight _AddWeight = new Weight();
        public string AddWeight
        {
            get { return _AddWeight.WeightUOMString; }
        }
        
        private bool _btnNextEnable;
        public bool btnNextEnable
        {
            get { return _btnNextEnable; }
            set
            {
                _btnNextEnable = value;
                OnPropertyChanged("btnNextEnable");
            }
        }
        private bool _btnPrevEnable;
        public bool btnPrevEnable
        {
            get { return _btnPrevEnable; }
            set
            {
                _btnPrevEnable = value;
                OnPropertyChanged("btnPrevEnable");
            }
        }

        private bool _DispensingbtnEnable;
        /// <summary>
        /// 할당정보 선택 버튼 Enable
        /// </summary>
        public bool DispensingbtnEnable
        {
            get { return _DispensingbtnEnable; }
            set
            {
                _DispensingbtnEnable = value;
                OnPropertyChanged("DispensingbtnEnable");
            }
        }

        private bool _LabelPrintbtnEnable;
        /// <summary>
        /// 할당정보 선택 버튼 Enable
        /// </summary>
        public bool LabelPrintbtnEnable
        {
            get { return _LabelPrintbtnEnable; }
            set
            {
                _LabelPrintbtnEnable = value;
                OnPropertyChanged("LabelPrintbtnEnable");
            }
        }

        private bool _ConfirmbtnEnable;
        /// <summary>
        /// 할당정보 선택 버튼 Enable
        /// </summary>
        public bool ConfirmbtnEnable
        {
            get { return _ConfirmbtnEnable; }
            set
            {
                _ConfirmbtnEnable = value;
                OnPropertyChanged("ConfirmbtnEnable");
            }
        }


        private BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATA_SOLUTION _BR_BRS_REG_ProductionOrderOutput_LastSoluction_ori;
        #endregion
        #region [BizRule]
        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight;
        private BR_PHR_SEL_System_Printer _BR_PHR_SEL_System_Printer;
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        private BR_PHR_REG_ScaleSetTare _BR_PHR_REG_ScaleSetTare;
        private BR_PHR_SEL_ProductionOrderOutput_Define _BR_PHR_SEL_ProductionOrderOutput_Define;
        private BR_BRS_REG_ProductionOrderOutput_LastSoluction _BR_BRS_REG_ProductionOrderOutput_LastSoluction;
        private BR_PHR_SEL_PRINT_LabelImage _BR_PHR_SEL_PRINT_LabelImage;
        private BR_BRS_SEL_ProductionOrderOutputSubLot _BR_BRS_SEL_ProductionOrderOutputSubLot;
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
                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            ///
                            decimal chk;

                            if (arg != null && arg is 최종조제)
                            {
                                _mainWnd = arg as 최종조제;
                                _mainWnd.Closed += (s, e) =>
                                {
                                    if (_DispatcherTimer != null)
                                        _DispatcherTimer.Stop();

                                    _DispatcherTimer = null;
                                };
                                _curstate = state.setBeakerTare;
                                btnNextEnable = false;
                                btnPrevEnable = false;
                                _DspStartDttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

                                // 기준량 조회
                                var curinst = _mainWnd.CurrentInstruction.Raw;

                                if (!string.IsNullOrWhiteSpace(curinst.TARGETVAL) && decimal.TryParse(curinst.TARGETVAL, out chk))
                                {
                                    _StandardWeight.Value = chk;
                                    _StandardWeight.Uom = string.IsNullOrWhiteSpace(curinst.UOMNOTATION) ? "g" : curinst.UOMNOTATION;
                                    if (_StandardWeight.Uom.ToUpper() == "KG")
                                        _StandardWeight.Uom = "g";
                                }
                                else
                                    throw new Exception("최종조제기준량이 설정되지 않았습니다.");

                                // 공정중제품 정보 조회
                                _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID)
                                });
                                if (await _BR_PHR_SEL_ProductionOrderOutput_Define.Execute() == _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Count > 0)
                                    _outputguid = _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs[0].OUTPUTGUID;
                                else
                                    throw new Exception("공정중제품 정보를 조회하지 못했습니다.");

                                //최종조제 정보 조회
                                _BR_BRS_SEL_ProductionOrderOutputSubLot.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID)
                                });
                                //최종조제 정보가 있으면 소분버튼 비활성화.
                                //2024.10.23 박희돈 벨킨주 2.5MG 1080095 오더에서 증량으로 인한 조제를 2번 함. 이에따라 2번째 조제에서는 이미 반제품이 생성되어있어 해당 로직 비활성 시킴. DEV-24-0901
                                //if (await _BR_BRS_SEL_ProductionOrderOutputSubLot.Execute() == _BR_BRS_SEL_ProductionOrderOutputSubLot.OUTDATAs.Count > 0)
                                //    DispensingbtnEnable = false;
                                //else
                                //    DispensingbtnEnable = true;

                                // 화면 열리면 기록 버튼 비활성화
                                ConfirmbtnEnable = false;
                                DispensingbtnEnable = true;

                                // 프린트 조회
                                _BR_PHR_SEL_System_Printer.INDATAs.Add(new BR_PHR_SEL_System_Printer.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    ROOMID = AuthRepositoryViewModel.Instance.RoomID
                                });
                                if (await _BR_PHR_SEL_System_Printer.Execute() && _BR_PHR_SEL_System_Printer.OUTDATAs.Count > 0)
                                {
                                    _selectedPrint = _BR_PHR_SEL_System_Printer.OUTDATAs[0];
                                    OnPropertyChanged("curPrintName");
                                }                                    
                                else
                                    OnMessage("연결된 프린트가 없습니다.");

                                IsBusy = true;
                            }

                            WeightRefresh();
                            IsBusy = false;                            
                            ///

                            CommandResults["LoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _curstate = state.exception;                            
                            CommandResults["LoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["LoadedCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }
        public ICommand ConnectScaleCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConnectScaleCommand"].EnterAsync())
                    {
                        try
                        {

                            CommandResults["ConnectScaleCommand"] = false;
                            CommandCanExecutes["ConnectScaleCommand"] = false;

                            ///
                            IsBusy = true;

                            _DispatcherTimer.Stop();

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
                                        _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];
                                        ScalePrecision = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.HasValue ? Convert.ToInt32(_BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].PRECISION.Value) : 3;
                                        ScaleUom = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0].NOTATION;
                                        ScaleId = text;
                                        _DispatcherTimer.Start();
                                    }
                                }
                                else
                                    ScaleId = "";
                            };

                            popup.Show();


                            IsBusy = false;
                            ///

                            CommandResults["ConnectScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ConnectScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["ConnectScaleCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConnectScaleCommand") ?
                        CommandCanExecutes["ConnectScaleCommand"] : (CommandCanExecutes["ConnectScaleCommand"] = true);
                });
            }
        }
        public ICommand ScanBeakerCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanBeakerCommand"].EnterAsync())
                    {
                        try
                        {

                            CommandResults["ScanBeakerCommand"] = false;
                            CommandCanExecutes["ScanBeakerCommand"] = false;

                            ///
                            IsBusy = true;

                            _DispatcherTimer.Stop();

                            BarcodePopup popup = new BarcodePopup();
                            popup.tbMsg.Text = "비커코드를 입력하세요.";
                            popup.Closed += (sender, e) =>
                            {
                                if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))                                                                    
                                    BeakerId = popup.tbText.Text.ToUpper();

                                _DispatcherTimer.Start();
                            };

                            popup.Show();                           
                            IsBusy = false;
                            ///

                            CommandResults["ScanBeakerCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ScanBeakerCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["ScanBeakerCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanBeakerCommand") ?
                        CommandCanExecutes["ScanBeakerCommand"] : (CommandCanExecutes["ScanBeakerCommand"] = true);
                });
            }
        }
        public ICommand SetBeakerTareCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SetBeakerTareCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["SetBeakerTareCommand"] = false;
                            CommandCanExecutes["SetBeakerTareCommand"] = false;

                            ///
                            IsBusy = true;
                            _DispatcherTimer.Stop();

                            if (string.IsNullOrWhiteSpace(BeakerId))
                                throw new Exception("사용중인 비커 정보가 없습니다.");

                            if (_ScaleInfo != null && !string.IsNullOrWhiteSpace(_ScaleId))
                            {
                                if(_ScaleInfo.TARE_MANDATORY == "Y")
                                {
                                    if (_ScaleWeight.Value > 0)
                                    {
                                        // 현재 저울값을 비커용기무게로 저장
                                        _BeakerTare = _ScaleWeight.Copy();
                                        OnPropertyChanged("BeakerTare");

                                        // 저울에 Tare 명령어 전달
                                        bool success = false;
                                        if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                                        {
                                            var result = await _restScaleService.DownloadString(_ScaleId, ScaleCommand.ST);

                                            success = result.code == "1" ? true : false;
                                        }
                                        else
                                        {
                                            _BR_PHR_REG_ScaleSetTare.INDATAs.Clear();
                                            _BR_PHR_REG_ScaleSetTare.INDATAs.Add(new BR_PHR_REG_ScaleSetTare.INDATA
                                            {
                                                ScaleID = _ScaleId
                                            });

                                            if (await _BR_PHR_REG_ScaleSetTare.Execute())
                                                success = true;
                                        }

                                        // 정상완료
                                        if (success)
                                        {
                                            _curstate = state.initial;
                                            btnNextEnable = true;
                                            btnPrevEnable = true;
                                        }
                                        else
                                            _BeakerTare.Value = 0;
                                    }
                                }
                                else if(_ScaleInfo.TARE_MANDATORY == "N")
                                {
                                    // 현재 저울값을 비커용기무게로 저장
                                    _BeakerTare = _ScaleWeight.Copy();
                                    _BeakerTare.Value = 0m;
                                    OnPropertyChanged("BeakerTare");
                                    _curstate = state.initial;
                                    btnNextEnable = true;
                                    btnPrevEnable = true;
                                }
                            }

                            WeightRefresh();
                            _DispatcherTimer.Start();
                            IsBusy = false;
                            ///

                            CommandResults["SetBeakerTareCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["SetBeakerTareCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["SetBeakerTareCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SetBeakerTareCommand") ?
                        CommandCanExecutes["SetBeakerTareCommand"] : (CommandCanExecutes["SetBeakerTareCommand"] = true);
                });
            }
        }
        public ICommand NextPhaseCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NextPhaseCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NextPhaseCommand"] = false;
                            CommandCanExecutes["NextPhaseCommand"] = false;

                            ///
                            _DispatcherTimer.Stop();          

                            switch (_curstate)
                            {
                                case state.initial:
                                    if(_ScaleWeight.Subtract(_StandardWeight).Value > 0)
                                    {
                                        OnMessage("최종조제액 기준량 보다 클 수 없습니다.");
                                        _DispatcherTimer.Start();
                                        break;
                                    }

                                    // 조제액 무게 저장
                                    _InitialWeight = _ScaleWeight.Copy();

                                    // 보충량 범위 지정
                                    //_TargetWeight = _StandardWeight.Subtract(_InitialWeight);
                                    // 2022.09.16 박희돈 보충량 범위는 타겟에서 +- 0.5프로를 한다. 위 로직은 타겟 - 조재량임.                                    
                                    _TargetWeight = _StandardWeight;
                                    _MinWeight = _StandardWeight.Copy();
                                    _MaxWeight = _StandardWeight.Copy();
                                    _MinWeight.Value = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(_StandardWeight.Value * 0.995m) * 10) / 10);
                                    _MaxWeight.Value = Convert.ToDecimal(Math.Floor(Convert.ToDouble(_StandardWeight.Value * 1.005m) * 10) / 10);

                                    _curstate = state.add;
                                    _DispatcherTimer.Start();
                                    break;
                                case state.add:

                                    // 보충량 기준 범위 상하한 0.5%의 값을 보여주도록 로직 변경
                                    //var curAddweight = _ScaleWeight.Subtract(_InitialWeight);
                                    var curAddweight = _ScaleWeight;

                                    _FinalWeight = _ScaleWeight.Copy();

                                    if (curAddweight.Subtract(_MinWeight).Value >= 0 && curAddweight.Subtract(_MaxWeight).Value <= 0)
                                    {
                                        _BR_BRS_REG_ProductionOrderOutput_LastSoluction_ori = new BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATA_SOLUTION
                                        {
                                            LOTTYPE = "WIP",
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                            OUTPUTGUID = _outputguid,
                                            VESSELID = BeakerId,
                                            MSUBLOTQTY = _FinalWeight.Value,
                                            TAREWEIGHT = _BeakerTare.Value,
                                            TAREUOMID = _BeakerTare.Uom,
                                            REASON = "최종조제액 생성",
                                            INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                                            IS_NEED_VESSELID = "N",
                                            IS_ONLY_TARE = "N",
                                            IS_NEW = "Y",
                                            SCALEID = ScaleId,
                                            WEIGHINGMETHOD = "WH007",
                                            WEIGHINGROOM = AuthRepositoryViewModel.Instance.RoomID,
                                            DSPSTRTDTTM = _DspStartDttm.ToString("yyyy-MM-dd HH:mm:ss"),
                                            DSPENDDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                        };

                                        // 보충량 기준 범위 상하한 0.5%의 값을 보여주도록 로직 변경
                                        //_AddWeight = _ScaleWeight;
                                        _AddWeight = _ScaleWeight.Subtract(_InitialWeight);
                                        _curstate = state.end;
                                    }
                                    else
                                        OnMessage("보충량이 기준값을 벗어났습니다.");

                                    if (_curstate != state.end)
                                        _DispatcherTimer.Start();
                                    break;
                                default:
                                    break;
                            }

                            WeightRefresh();
                            IsBusy = false;
                            ///

                            CommandResults["NextPhaseCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["NextPhaseCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["NextPhaseCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NextPhaseCommand") ?
                        CommandCanExecutes["NextPhaseCommand"] : (CommandCanExecutes["NextPhaseCommand"] = true);
                });
            }
        }
        public ICommand PrevPhaseCommnad
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PrevPhaseCommnad"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PrevPhaseCommnad"] = false;
                            CommandCanExecutes["PrevPhaseCommnad"] = false;

                            ///
                            _DispatcherTimer.Stop();
   
                            switch (_curstate)
                            {
                                case state.initial: // 현재 저울값이 0이하면 Tare측정단계로 이동
                                    if (_ScaleWeight.Value <= 0)
                                    {
                                        _BeakerTare.Value = 0;
                                        _curstate = state.setBeakerTare;
                                    }
                                    else
                                        OnMessage("현재 저울값이 0보다 크면 Tare측정단계로 돌아갈 수 없습니다.");
                                     
                                    _DispatcherTimer.Start();
                                    break;
                                case state.add: // 보충전 조제액 무게보다 현재값이 작아야 이전단계로 변경 가능 
                                    if (_ScaleWeight.Subtract(_InitialWeight).Value <= 0)
                                    {
                                        _MinWeight.Value = 0;
                                        _MaxWeight.Value = 0;
                                        _TargetWeight.Value = 0;
                                        _curstate = state.initial;
                                    }
                                    else
                                        OnMessage("원료 보충전 조제액 무게보다 현재 저울값이 크면 보충전 조제액 측정단계로 돌아갈 수 없습니다.");

                                    _DispatcherTimer.Start();
                                    break;
                                case state.end:
                                    _AddWeight.Value = 0;
                                    _FinalWeight.Value = 0;
                                    _curstate = state.add;
                                    _DispatcherTimer.Start();
                                    break;
                                default:
                                    break;
                            }

                            WeightRefresh();
                            IsBusy = false;
                            ///

                            CommandResults["PrevPhaseCommnad"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            _DispatcherTimer.Start();
                            CommandResults["PrevPhaseCommnad"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
                            CommandCanExecutes["PrevPhaseCommnad"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PrevPhaseCommnad") ?
                        CommandCanExecutes["PrevPhaseCommnad"] : (CommandCanExecutes["PrevPhaseCommnad"] = true);
                });
            }
        }
        public ICommand DispensingCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["DispensingCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["DispensingCommandAsync"] = false;
                            CommandCanExecutes["DispensingCommandAsync"] = false;

                            if (_curstate != state.end)
                                throw new Exception("칭량이 완료되지 않았습니다.");

                            var authHelper = new iPharmAuthCommandHelper();                            
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "최종조제",
                                "최종조제",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // 최종 조제시 보충원료 소분 및 투입
                            _BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATA_SOLUTIONs.Clear();
                            _BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATA_SOLUTIONs.Add(_BR_BRS_REG_ProductionOrderOutput_LastSoluction_ori.Copy());

                            _BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATAs.Clear();
                            _BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_LastSoluction.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                WEIGHINGMETHOD = "WH007"
                            });

                            //if (await _BR_BRS_REG_ProductionOrderOutput_LastSoluction.Execute())
                            //{
                            //    if (_BR_BRS_REG_ProductionOrderOutput_LastSoluction.OUTDATAs.Count > 0 && curPrintName != "N/A")
                            //    {
                            //        _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                            //        _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                            //        _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                            //        {
                            //            ReportPath = "/Reports/Label/LABEL_C0402_018_10",
                            //            PrintName = _selectedPrint.PRINTERNAME,
                            //            USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            //        });
                            //        _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                            //        {
                            //            ParamName = "MSUBLOTID",
                            //            ParamValue = _BR_BRS_REG_ProductionOrderOutput_LastSoluction.OUTDATAs[0].MSUBLOTID
                            //        });
                            //        _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                            //        {
                            //            ParamName = "GUBUN",
                            //            ParamValue = "최종 조제량"
                            //        });

                            //        await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                            //    }
                            //}

                            if(await _BR_BRS_REG_ProductionOrderOutput_LastSoluction.Execute())
                            {
                                DispensingbtnEnable = false;
                                ConfirmbtnEnable = true;
                            }                            
                            else
                            {
                                DispensingbtnEnable = true;
                                ConfirmbtnEnable = false;
                            }

                            CommandResults["DispensingCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            DispensingbtnEnable = true;
                            ConfirmbtnEnable = false;
                            CommandResults["DispensingCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["DispensingCommandAsync"] = true;

                            IsBusy = false;
                        }

                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("DispensingCommandAsync") ?
                        CommandCanExecutes["DispensingCommandAsync"] : (CommandCanExecutes["DispensingCommandAsync"] = true);
                });
            }
        }

        public ICommand PrintLabelCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PrintLabelCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["PrintLabelCommandAsync"] = false;
                            CommandCanExecutes["PrintLabelCommandAsync"] = false;


                            //최종조제 정보 조회
                            //2021.08.26 박희돈 처음 소분 진행힐 경우는 데이터 없어서 라벨 출력 시 재조회 해서 확인하도록 변경.
                            _BR_BRS_SEL_ProductionOrderOutputSubLot.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID)
                            });
                            await _BR_BRS_SEL_ProductionOrderOutputSubLot.Execute();

                            var authHelper = new iPharmAuthCommandHelper();
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "최종조제라벨",
                                "최종조제라벨",
                                true,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //2021.08.26 박희돈 최종조제는 1개의 정보만 나오지만 혹시 여러개의 정보가 있을까봐 Loop로 라벨 다 출력하도록 변경.
                            if (_BR_BRS_SEL_ProductionOrderOutputSubLot.OUTDATAs.Count > 0 && curPrintName != "N/A")
                            {
                                foreach (var item in _BR_BRS_SEL_ProductionOrderOutputSubLot.OUTDATAs)
                                {
                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                                    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                                    {
                                        ReportPath = "/Reports/Label/LABEL_C0402_018_10",
                                        PrintName = _selectedPrint.PRINTERNAME,
                                        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                    });
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                    {
                                        ParamName = "MSUBLOTID",
                                        ParamValue = item.MSUBLOTID
                                    });
                                    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                                    {
                                        ParamName = "GUBUN",
                                        ParamValue = "최종 조제량"
                                    });

                                    await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                                }
                            }
                            else
                            {
                                if(curPrintName == "N/A")
                                {
                                    throw new Exception(string.Format("프린터 설정이 되어있지 않습니다."));
                                }
                                else
                                {
                                    throw new Exception(string.Format("최종투입을 하지 않았습니다."));
                                }                                
                            }

                            #region 기존 라벨출력 로직주석 처리
                            //if (_BR_BRS_REG_ProductionOrderOutput_LastSoluction.OUTDATAs.Count > 0 && curPrintName != "N/A")
                            //{
                            //    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Clear();
                            //    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Clear();

                            //    _BR_PHR_SEL_PRINT_LabelImage.INDATAs.Add(new BR_PHR_SEL_PRINT_LabelImage.INDATA
                            //    {
                            //        ReportPath = "/Reports/Label/LABEL_C0402_018_10",
                            //        PrintName = _selectedPrint.PRINTERNAME,
                            //        USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            //    });
                            //    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                            //    {
                            //        ParamName = "MSUBLOTID",
                            //        ParamValue = _BR_BRS_REG_ProductionOrderOutput_LastSoluction.OUTDATAs[0].MSUBLOTID
                            //    });
                            //    _BR_PHR_SEL_PRINT_LabelImage.Parameterss.Add(new BR_PHR_SEL_PRINT_LabelImage.Parameters
                            //    {
                            //        ParamName = "GUBUN",
                            //        ParamValue = "최종 조제량"
                            //    });

                            //    await _BR_PHR_SEL_PRINT_LabelImage.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent);
                            //}
                            //else
                            //{
                            //    throw new Exception(string.Format("최종투입을 하지 않았습니다."));
                            //}

                            # endregion 기존 라벨출력 로직 주석 처리

                            CommandResults["PrintLabelCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PrintLabelCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PrintLabelCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PrintLabelCommandAsync") ?
                        CommandCanExecutes["PrintLabelCommandAsync"] : (CommandCanExecutes["PrintLabelCommandAsync"] = true);
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

                            if (_curstate != state.end)                            
                                throw new Exception("칭량이 완료되지 않았습니다.");
                            
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
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "최종조제",
                                "최종조제",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // XML 변환
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("하한"));
                            dt.Columns.Add(new DataColumn("LoadCell지시값"));
                            dt.Columns.Add(new DataColumn("상한"));
                            dt.Columns.Add(new DataColumn("최종무게"));
                            dt.Columns.Add(new DataColumn("저울번호"));

                            DataRow row = dt.NewRow();
                            row["용기번호"] = BeakerId ?? ""; 
                            row["하한"] = MinWeight ?? "";
                            row["LoadCell지시값"] = AddWeight ?? "";
                            row["상한"] = MaxWeight ?? "";
                            row["최종무게"] = FinalWeight ?? "";
                            row["저울번호"] = ScaleId ?? "";
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

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            OnPropertyChanged("CurrentStateSTR");
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
        public ICommand ChangePrintCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["ChangePrintCommand"] = false;
                        CommandCanExecutes["ChangePrintCommand"] = false;

                        ///
                        SelectPrinterPopup popup = new SelectPrinterPopup();

                        popup.Closed += (s, e) =>
                        {
                            if (popup.DialogResult.GetValueOrDefault())
                            {
                                if (popup.SourceGrid.SelectedItem != null && popup.SourceGrid.SelectedItem is BR_PHR_SEL_System_Printer.OUTDATA)
                                {
                                    _selectedPrint = popup.SourceGrid.SelectedItem as BR_PHR_SEL_System_Printer.OUTDATA;
                                    OnPropertyChanged("curPrintName");
                                }
                            }
                        };

                        popup.Show();
                        ///
                        CommandResults["ChangePrintCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["ChangePrintCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        OnPropertyChanged("CurrentStateSTR");
                        CommandCanExecutes["ChangePrintCommand"] = true;
                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChangePrintCommand") ?
                        CommandCanExecutes["ChangePrintCommand"] : (CommandCanExecutes["ChangePrintCommand"] = true);
                });
            }
        }
        #endregion
        #region [Custom]
        private void WeightRefresh()
        {
            OnPropertyChanged("ScaleWeight");
            OnPropertyChanged("BeakerTare");
            OnPropertyChanged("StandardWeight");
            OnPropertyChanged("InitialWeight");
            OnPropertyChanged("TargetWeight");
            OnPropertyChanged("MinWeight");
            OnPropertyChanged("MaxWeight");
            OnPropertyChanged("MaxWeight");
            OnPropertyChanged("AddWeight");
        }
        private async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (_ScaleInfo != null && !string.IsNullOrWhiteSpace(_ScaleId))
                {                    
                    // 저울에 Tare 명령어 전달
                    bool success = false;
                    string curWeight = string.Empty;
                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleId, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            curWeight = result.data;
                            ScaleUom = result.unit;

                            // 저울 유효숫자 설정
                            if (curWeight.Contains("."))
                            {
                                var splt = curWeight.Split('.');
                                if (splt.Length > 1)
                                    ScalePrecision = splt[1].Length;
                                else
                                    ScalePrecision = 0;
                            }
                            else
                                ScalePrecision = 0;
                        }                        
                    }
                    else
                    {
                        _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                        _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();

                        _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA
                        {
                            ScaleID = _ScaleId
                        });

                        if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: Common.enumBizRuleXceptionHandleType.FailEvent) == true && _BR_BRS_SEL_CurrentWeight.OUTDATAs.Count > 0)
                        {
                            success = true;
                            curWeight = string.Format(("{0:N" + _ScaleWeight.Precision + "}"), _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight);
                            ScaleUom = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].UOM;
                        }                       
                            
                    }                    

                    if (success)
                        _ScaleWeight.SetWeight(curWeight, _ScaleWeight.Uom);                     
                    else
                        _ScaleWeight.Value = 0;

                    WeightRefresh();
                    _DispatcherTimer.Start();
                }                      
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
        
        #endregion        
    }
}
