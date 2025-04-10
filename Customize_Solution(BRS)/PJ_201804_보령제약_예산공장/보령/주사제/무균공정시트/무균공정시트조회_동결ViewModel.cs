using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using C1.Silverlight.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using LGCNS.iPharmMES.Recipe.Common;
using System.Threading.Tasks;

namespace 보령
{
    public class 무균공정시트조회_동결ViewModel : ViewModelBase
    {
        #region [Property]
        public 무균공정시트조회_동결ViewModel()
        {
            _BR_BRS_SEL_SVP_ASEPTIC_PROCESS = new BR_BRS_SEL_SVP_ASEPTIC_PROCESS();
        }

        무균공정시트조회_동결 _mainWnd;

        DateTime _fromDt;
        public DateTime FromDt
        {
            get { return _fromDt; }
            set
            {
                _fromDt = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// 충전노즐사용1
        /// </summary>
        private bool _Nozzle1Check;
        public bool Nozzle1Check
        {
            get { return _Nozzle1Check; }
            set
            {
                _Nozzle1Check = value;
                OnPropertyChanged("Nozzle1Check");
            }
        }
        /// <summary>
        /// 충전노즐사용2
        /// </summary>
        private bool _Nozzle2Check;
        public bool Nozzle2Check
        {
            get { return _Nozzle2Check; }
            set
            {
                _Nozzle2Check = value;
                OnPropertyChanged("Nozzle2Check");
            }
        }
        /// <summary>
        /// 충전노즐사용3
        /// </summary>
        private bool _Nozzle3Check;
        public bool Nozzle3Check
        {
            get { return _Nozzle3Check; }
            set
            {
                _Nozzle3Check = value;
                OnPropertyChanged("Nozzle3Check");
            }
        }
        /// <summary>
        /// 충전노즐사용4
        /// </summary>
        private bool _Nozzle4Check;
        public bool Nozzle4Check
        {
            get { return _Nozzle4Check; }
            set
            {
                _Nozzle4Check = value;
                OnPropertyChanged("Nozzle4Check");
            }
        }

        /// <summary>
        /// 충전시간
        /// </summary>
        private string _ChargDttm;
        public string ChargDttm
        {
            get { return _ChargDttm; }
            set
            {
                _ChargDttm = value;
                OnPropertyChanged("ChargDttm");
            }
        }
        /// <summary>
        /// 언로딩시간
        /// </summary>
        private string _UnloadingDttm;
        public string UnloadingDttm
        {
            get { return _UnloadingDttm; }
            set
            {
                _UnloadingDttm = value;
                OnPropertyChanged("UnloadingDttm");
            }
        }
        /// <summary>
        /// 충전 멸균 물품 Holding Time(Autoclave) 시간
        /// </summary>
        private string _AutoClaveHoldTime;
        public string AutoClaveHoldTime
        {
            get { return _AutoClaveHoldTime; }
            set
            {
                _AutoClaveHoldTime = value;
                OnPropertyChanged("AutoClaveHoldTime");
            }
        }
        /// <summary>
        /// 충전 멸균 물품 Holding Time(Autoclave) 시간 합계
        /// </summary>
        private string _SumAutoClaveHoldTime;
        public string SumAutoClaveHoldTime
        {
            get { return _SumAutoClaveHoldTime; }
            set
            {
                _SumAutoClaveHoldTime = value;
                OnPropertyChanged("SumAutoClaveHoldTime");
            }
        }
        /// <summary>
        /// Isolator 훈증 Holding Time
        /// </summary>
        private string _IsolatorHoldTime;
        public string IsolatorHoldTime
        {
            get { return _IsolatorHoldTime; }
            set
            {
                _IsolatorHoldTime = value;
                OnPropertyChanged("IsolatorHoldTime");
            }
        }
        /// <summary>
        /// Isolator 훈증 Holding Time 합계
        /// </summary>
        private string _SumIsolatorHoldTime;
        public string SumIsolatorHoldTime
        {
            get { return _SumIsolatorHoldTime; }
            set
            {
                _SumIsolatorHoldTime = value;
                OnPropertyChanged("SumIsolatorHoldTime");
            }
        }
        /// <summary>
        /// 세팅작업시간 - 시작시간
        /// </summary>
        private string _SetStartTime;
        public string SetStartTime
        {
            get { return _SetStartTime; }
            set
            {
                _SetStartTime = value;
                OnPropertyChanged("SetStartTime");
            }
        }
        /// <summary>
        /// 세팅작업시간 - 종료시간
        /// </summary>
        private string _SetEndTime;
        public string SetEndTime
        {
            get { return _SetEndTime; }
            set
            {
                _SetEndTime = value;
                OnPropertyChanged("SetEndTime");
            }
        }
        /// <summary>
        /// 세팅작업시간 - 합계
        /// </summary>
        private string _SumSetTime;
        public string SumSetTime
        {
            get { return _SumSetTime; }
            set
            {
                _SumSetTime = value;
                OnPropertyChanged("SumSetTime");
            }
        }
        /// <summary>
        /// 안정화 시간
        /// </summary>
        private string _StableTime;
        public string StableTime
        {
            get { return _StableTime; }
            set
            {
                _StableTime = value;
                OnPropertyChanged("StableTime");
            }
        }
        /// <summary>
        /// 안정화시간 합계 (min으로 계산 필요)
        /// </summary>
        private string _SumStableTime;
        public string SumStableTime
        {
            get { return _SumStableTime; }
            set
            {
                _SumStableTime = value;
                OnPropertyChanged("SumStableTime");
            }
        }
        /// <summary>
        /// 충전시간
        /// </summary>
        private string _FillTIME;
        public string FillTIME
        {
            get { return _FillTIME; }
            set
            {
                _FillTIME = value;
                OnPropertyChanged("FillTIME");
            }
        }
        /// <summary>
        /// 충전시간 - 합계
        /// </summary>
        private string _SumFillTIME;
        public string SumFillTIME
        {
            get { return _SumFillTIME; }
            set
            {
                _SumFillTIME = value;
                OnPropertyChanged("SumFillTIME");
            }
        }
        /// <summary>
        /// 무균공정시간
        /// </summary>
        private string _AsepticTime;
        public string AsepticTime
        {
            get { return _AsepticTime; }
            set
            {
                _AsepticTime = value;
                OnPropertyChanged("AsepticTime");
            }
        }
        /// <summary>
        /// 무균공정시간 - 합계
        /// </summary>
        private string _SumAsepticTime;
        public string SumAsepticTime
        {
            get { return _SumAsepticTime; }
            set
            {
                _SumAsepticTime = value;
                OnPropertyChanged("SumAsepticTime");
            }
        }
        /// <summary>
        /// 동결건조 SIP Holding Time
        /// </summary>
        private string _DryHoldTime;
        public string DryHoldTime
        {
            get { return _DryHoldTime; }
            set
            {
                _DryHoldTime = value;
                OnPropertyChanged("DryHoldTime");
            }
        }
        /// <summary>
        /// 동결건조 SIP Holding Time - 합계
        /// </summary>
        private string _SumDryHoldTime;
        public string SumDryHoldTime
        {
            get { return _SumDryHoldTime; }
            set
            {
                _SumDryHoldTime = value;
                OnPropertyChanged("SumDryHoldTime");
            }
        }
        /// <summary>
        /// 동결건조 시간
        /// </summary>
        private string _FreezeTime;
        public string FreezeTime
        {
            get { return _FreezeTime; }
            set
            {
                _FreezeTime = value;
                OnPropertyChanged("FreezeTime");
            }
        }
        /// <summary>
        /// 동결건조 시간 - 합계
        /// </summary>
        private string _SumFreezeTime;
        public string SumFreezeTime
        {
            get { return _SumFreezeTime; }
            set
            {
                _SumFreezeTime = value;
                OnPropertyChanged("SumFreezeTime");
            }
        }
        /// <summary>
        /// 밀전 후 Holding Time
        /// </summary>
        private string _BlockTime;
        public string BlockTime
        {
            get { return _BlockTime; }
            set
            {
                _BlockTime = value;
                OnPropertyChanged("BlockTime");
            }
        }
        /// <summary>
        /// 밀전 후 Holding Time - 합계
        /// </summary>
        private string _SumBlockTime;
        public string SumBlockTime
        {
            get { return _SumBlockTime; }
            set
            {
                _SumBlockTime = value;
                OnPropertyChanged("SumBlockTime");
            }
        }
        /// <summary>
        /// 무균 공정에 참여한 모든 작업자
        /// </summary>
        private string _WorkUserId;
        public string WorkUserId
        {
            get { return _WorkUserId; }
            set
            {
                _WorkUserId = value;
                OnPropertyChanged("WorkUserId");
            }
        }
        /// <summary>
        /// 실시간 파티클 모니터링 측정시간 - 측정 시작
        /// </summary>
        private string _PMSStartDttm;
        public string PMSStartDttm
        {
            get { return _PMSStartDttm; }
            set
            {
                _PMSStartDttm = value;
                OnPropertyChanged("PMSStartDttm");
            }
        }
        /// <summary>
        /// 실시간 파티클 모니터링 측정시간 - 측정 종료
        /// </summary>
        private string _PMSEndDttm;
        public string PMSEndDttm
        {
            get { return _PMSEndDttm; }
            set
            {
                _PMSEndDttm = value;
                OnPropertyChanged("PMSEndDttm");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 1번째
        /// </summary>
        private string _DropGermsFirst;
        public string DropGermsFirst
        {
            get { return _DropGermsFirst; }
            set
            {
                _DropGermsFirst = value;
                OnPropertyChanged("DropGermsFirst");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 2번째
        /// </summary>
        private string _DropGermsSecond;
        public string DropGermsSecond
        {
            get { return _DropGermsSecond; }
            set
            {
                _DropGermsSecond = value;
                OnPropertyChanged("DropGermsSecond");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 3번째
        /// </summary>
        private string _DropGermsThird;
        public string DropGermsThird
        {
            get { return _DropGermsThird; }
            set
            {
                _DropGermsThird = value;
                OnPropertyChanged("DropGermsThird");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 4번째
        /// </summary>
        private string _DropGermsFourth;
        public string DropGermsFourth
        {
            get { return _DropGermsFourth; }
            set
            {
                _DropGermsFourth = value;
                OnPropertyChanged("DropGermsFourth");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 6번째
        /// </summary>
        private string _DropGermsFifth;
        public string DropGermsFifth
        {
            get { return _DropGermsFifth; }
            set
            {
                _DropGermsFifth = value;
                OnPropertyChanged("DropGermsFifth");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 6번째
        /// </summary>
        private string _DropGermsSixth;
        public string DropGermsSixth
        {
            get { return _DropGermsSixth; }
            set
            {
                _DropGermsSixth = value;
                OnPropertyChanged("DropGermsSixth");
            }
        }

        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 7번째
        /// </summary>
        private string _DropGermsSeventh;
        public string DropGermsSeventh
        {
            get { return _DropGermsSeventh; }
            set
            {
                _DropGermsSeventh = value;
                OnPropertyChanged("DropGermsSeventh");
            }
        }
        /// <summary>
        /// 미생물환경모니터링 (M1~M5)&#10;충전시 낙하균(S) 측정시간 8번째
        /// </summary>
        private string _DropGermsEighth;
        public string DropGermsEighth
        {
            get { return _DropGermsEighth; }
            set
            {
                _DropGermsEighth = value;
                OnPropertyChanged("DropGermsEighth");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 부유균(A) 측정시간 1번째
        /// </summary>
        private string _microbeA1;
        public string microbeA1
        {
            get { return _microbeA1; }
            set
            {
                _microbeA1 = value;
                OnPropertyChanged("microbeA1");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 부유균(A) 측정시간 2번째
        /// </summary>
        private string _microbeA2;
        public string microbeA2
        {
            get { return _microbeA2; }
            set
            {
                _microbeA2 = value;
                OnPropertyChanged("microbeA2");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 부유균(A) 측정시간 3번째
        /// </summary>
        private string _microbeA3;
        public string microbeA3
        {
            get { return _microbeA3; }
            set
            {
                _microbeA3 = value;
                OnPropertyChanged("microbeA3");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 (M4,M5)&#10;언로딩시 낙하균(S) 측정시간 1번째
        /// </summary>
        private string _MicrobeUnloadingFirst;
        public string MicrobeUnloadingFirst
        {
            get { return _MicrobeUnloadingFirst; }
            set
            {
                _MicrobeUnloadingFirst = value;
                OnPropertyChanged("MicrobeUnloadingFirst");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 (M4,M5)&#10;언로딩시 낙하균(S) 측정시간 2번째
        /// </summary>
        private string _MicrobeUnloadingSecond;
        public string MicrobeUnloadingSecond
        {
            get { return _MicrobeUnloadingSecond; }
            set
            {
                _MicrobeUnloadingSecond = value;
                OnPropertyChanged("MicrobeUnloadingSecond");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 (M4,M5)&#10;언로딩시 낙하균(S) 측정시간 3번째
        /// </summary>
        private string _MicrobeUnloadingThird;
        public string MicrobeUnloadingThird
        {
            get { return _MicrobeUnloadingThird; }
            set
            {
                _MicrobeUnloadingThird = value;
                OnPropertyChanged("MicrobeUnloadingThird");
            }
        }
        /// <summary>
        /// 미생물 환경 모니터링 (M4,M5)&#10;언로딩시 낙하균(S) 측정시간 4번째
        /// </summary>
        private string _MicrobeUnloadingFourth;
        public string MicrobeUnloadingFourth
        {
            get { return _MicrobeUnloadingFourth; }
            set
            {
                _MicrobeUnloadingFourth = value;
                OnPropertyChanged("MicrobeUnloadingFourth");
            }
        }

        #region [항목별 일탈여부 판단 파라미터]
        private string _AutoDiff;
        public string AutoDiff
        {
            get { return _AutoDiff; }
            set
            {
                _AutoDiff = value;
                OnPropertyChanged("AutoDiff");
            }
        }

        private string _IsolatorHoldDiff;
        public string IsolatorHoldDiff
        {
            get { return _IsolatorHoldDiff; }
            set
            {
                _IsolatorHoldDiff = value;
                OnPropertyChanged("IsolatorHoldDiff");
            }
        }

        private string _SetDiff;
        public string SetDiff
        {
            get { return _SetDiff; }
            set
            {
                _SetDiff = value;
                OnPropertyChanged("SetDiff");
            }
        }

        private string _StableDiff;
        public string StableDiff
        {
            get { return _StableDiff; }
            set
            {
                _StableDiff = value;
                OnPropertyChanged("StableDiff");
            }
        }

        private string _FillDiff_FDR;
        public string FillDiff_FDR
        {
            get { return _FillDiff_FDR; }
            set
            {
                _FillDiff_FDR = value;
                OnPropertyChanged("FillDiff_FDR");
            }
        }

        private string _AsepticDiff_FDR;
        public string AsepticDiff_FDR
        {
            get { return _AsepticDiff_FDR; }
            set
            {
                _AsepticDiff_FDR = value;
                OnPropertyChanged("AsepticDiff_FDR");
            }
        }

        private string _DryHoldDiff;
        public string DryHoldDiff
        {
            get { return _DryHoldDiff; }
            set
            {
                _DryHoldDiff = value;
                OnPropertyChanged("DryHoldDiff");
            }
        }

        private string _FreezeDiff;
        public string FreezeDiff
        {
            get { return _FreezeDiff; }
            set
            {
                _FreezeDiff = value;
                OnPropertyChanged("FreezeDiff");
            }
        }

        private string _BlockDiff;
        public string BlockDiff
        {
            get { return _BlockDiff; }
            set
            {
                _BlockDiff = value;
                OnPropertyChanged("BlockDiff");
            }
        }

        #endregion [항목별 일탈여부 판단 파라미터]

        #endregion

        #region [Bizrule]

        private BR_BRS_SEL_SVP_ASEPTIC_PROCESS _BR_BRS_SEL_SVP_ASEPTIC_PROCESS;
        public BR_BRS_SEL_SVP_ASEPTIC_PROCESS BR_BRS_SEL_SVP_ASEPTIC_PROCESS
        {
            get { return _BR_BRS_SEL_SVP_ASEPTIC_PROCESS; }
            set
            {
                _BR_BRS_SEL_SVP_ASEPTIC_PROCESS = value;
                NotifyPropertyChanged();
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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is 무균공정시트조회_동결)
                            {
                                _mainWnd = arg as 무균공정시트조회_동결;
                                // 2024.08.14 박희돈 기록정보 조회
                                _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.INDATAs.Add(new BR_BRS_SEL_SVP_ASEPTIC_PROCESS.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.OrderID
                                });
                                // 2024.08.14 박희돈 조회정보 맵핑
                                if (await _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.Execute() && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs.Count > 0)
                                {
                                    ValueMapping();
                                }
                            }

                            IsBusy = false;
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
                            bool checkDeviation = false;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            if(!string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AutoDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AutoDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].IsolatorHoldDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].IsolatorHoldDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SETDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SETDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].StableDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].StableDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FillDiff_FDR) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FillDiff_FDR.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AsepticDiff_FDR) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AsepticDiff_FDR.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DryHoldDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DryHoldDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FreezeDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FreezeDiff.ToString().Equals("Y")
                               || !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].BlockDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].BlockDiff.ToString().Equals("Y")
                               )
                            {
                                checkDeviation = true;
                            }

                            //이미지 저장시 서명화면으로 인해 이미지가 잘 안보임.그에 따른 이미지 데이터만 먼저 생성해 놓도록 함.
                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "Image attached";

                            var authHelper = new iPharmAuthCommandHelper();

                            if (checkDeviation)
                            {
                                if (await OnMessageAsync("입력값이 기준값을 벗어났습니다. 기록을 진행하시겟습니까?", true) == false) return;

                                authHelper = new iPharmAuthCommandHelper();

                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                enumRoleType inspectorRole = enumRoleType.ROLE001;
                                if (_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE != null && Enum.TryParse<enumRoleType>(_mainWnd.Phase.CurrentPhase.INSPECTOR_ROLE, out inspectorRole))
                                {
                                }

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Role,
                                    Common.enumAccessType.Create,
                                    "기록값 일탈에 대해 서명후 기록을 진행합니다 ",
                                    "Deviation Sign",
                                    true,
                                    "OM_ProductionOrder_Deviation",
                                    "",
                                    this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                    this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                {
                                    return;
                                }
                                _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");
                            }
                            else
                            {
                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN == "Y" && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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
                            }                            

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (checkDeviation)
                            {

                                var bizrule = new BR_PHR_REG_InstructionComment();

                                bizrule.IN_Comments.Add(
                                    new BR_PHR_REG_InstructionComment.IN_Comment
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        COMMENTTYPE = "CM008",
                                        COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation")
                                    }
                                    );
                                bizrule.IN_IntructionResults.Add(
                                    new BR_PHR_REG_InstructionComment.IN_IntructionResult
                                    {
                                        RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                        ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                        IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                        IRTRSTGUID = _mainWnd.CurrentInstruction.Raw.IRTRSTGUID,
                                        IRTSEQ = (int)_mainWnd.CurrentInstruction.Raw.IRTSEQ
                                    }
                                    );

                                await bizrule.Execute();
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
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

        public ICommand NoRecordConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NoRecordConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NoRecordConfirmCommand"] = false;
                            CommandCanExecutes["NoRecordConfirmCommand"] = false;

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

                            //이미지 저장시 서명화면으로 인해 이미지가 잘 안보임.그에 따른 이미지 데이터만 먼저 생성해 놓도록 함.
                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "Image attached";

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NoRecordConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NoRecordConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("NoRecordConfirmCommand") ?
                        CommandCanExecutes["NoRecordConfirmCommand"] : (CommandCanExecutes["NoRecordConfirmCommand"] = true);
                });
            }
        }

        #endregion

        #region User Define

        public void ValueMapping()
        {
            try
            {
                ChargDttm = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].ChargDttm;
                UnloadingDttm = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].UnloadingDttm;
                AutoClaveHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AutoClaveHoldTime;
                SumAutoClaveHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SUMAUTOCLAVEHOLDTIME;
                AutoDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AutoDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AutoDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                IsolatorHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].IsolatorHoldTime;
                SumIsolatorHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SUMISOLATORHOLDTIME;
                IsolatorHoldDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].IsolatorHoldDiff) &&_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].IsolatorHoldDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                SetStartTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SetStartTime;
                SetEndTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SetEndTime;
                SumSetTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SUMSETTIME;
                SetDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SETDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SETDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                StableTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].StableTime;
                SumStableTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SUMSTABLETIME;
                StableDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].StableDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].StableDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                FillTIME = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FillTIME_FDR;
                SumFillTIME = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SumFillTIME_FDR;
                FillDiff_FDR = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FillDiff_FDR) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FillDiff_FDR.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                AsepticTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AsepticTime_FDR;
                SumAsepticTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SumAsepticTime_FDR;
                AsepticDiff_FDR = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AsepticDiff_FDR) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].AsepticDiff_FDR.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                DryHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DryHoldTime;
                SumDryHoldTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SumDryHoldTime;
                DryHoldDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DryHoldDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DryHoldDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                FreezeTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FreezeTime;
                SumFreezeTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SumFreezeTime;
                FreezeDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FreezeDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].FreezeDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                BlockTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].BlockTime;
                SumBlockTime = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].SumBlockTime;
                BlockDiff = !string.IsNullOrEmpty(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].BlockDiff) && _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].BlockDiff.ToString().Equals("Y") ? "Yellow" : "#FFEBEEEE";
                WorkUserId = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].WORKUSERID;
                PMSStartDttm = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].PMSStartDttm;
                PMSEndDttm = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].PMSEndDttm;
                DropGermsFirst = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsFirst;
                DropGermsSecond = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsSecond;
                DropGermsThird = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsThird;
                DropGermsFourth = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsFourth;
                DropGermsFifth = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsFifth;
                DropGermsSixth = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsSixth;
                DropGermsSeventh = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsSeventh;
                DropGermsEighth = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].DropGermsEighth;
                microbeA1 = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].microbeA1;
                microbeA2 = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].microbeA2;
                microbeA3 = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].microbeA3;
                Nozzle1Check = Convert.ToBoolean(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].Nozzle1Check);
                Nozzle2Check = Convert.ToBoolean(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].Nozzle2Check);
                Nozzle3Check = Convert.ToBoolean(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].Nozzle3Check);
                Nozzle4Check = Convert.ToBoolean(_BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].Nozzle4Check);
                MicrobeUnloadingFirst = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].MicrobeUnloadingFirst;
                MicrobeUnloadingSecond = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].MicrobeUnloadingSecond;
                MicrobeUnloadingThird = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].MicrobeUnloadingThird;
                MicrobeUnloadingFourth = _BR_BRS_SEL_SVP_ASEPTIC_PROCESS.OUTDATAs[0].MicrobeUnloadingFourth;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public byte[] imageToByteArray()
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(datas, 0, datas.Length);

                return datas;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}