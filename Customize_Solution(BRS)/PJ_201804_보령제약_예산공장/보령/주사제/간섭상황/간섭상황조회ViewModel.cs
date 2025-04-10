using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using C1.Silverlight.Imaging;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 간섭상황조회ViewModel : ViewModelBase
    {
        #region [Property]
        public 간섭상황조회ViewModel()
        {
            _BR_BRS_SEL_INTERFER_SITUATION_SUM = new BR_BRS_SEL_INTERFER_SITUATION_SUM();
            _ListInterfer = new ObservableCollection<InterferSituation>();
        }

        간섭상황조회 _mainWnd;

        //간섭상황조회List
        private ObservableCollection<InterferSituation> _ListInterfer;
        public ObservableCollection<InterferSituation> ListInterfer
        {
            get { return _ListInterfer; }
            set
            {
                _ListInterfer = value;
                OnPropertyChanged("ListInterfer");
            }
        }
        private bool _OVER_FLAG;
        public bool OVER_FLAG
        {
            get { return _OVER_FLAG; }
            set
            {
                _OVER_FLAG = value;
                OnPropertyChanged("OVER_FLAG");
            }
        }
        #endregion

        #region [Bizrule]
        private BR_BRS_SEL_INTERFER_SITUATION_SUM _BR_BRS_SEL_INTERFER_SITUATION_SUM;
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

                            if (arg != null && arg is 간섭상황조회)
                            {
                                _mainWnd = arg as 간섭상황조회;

                                Decimal CHECK_SUM = 0;
                                OVER_FLAG = false;

                                _BR_BRS_SEL_INTERFER_SITUATION_SUM.INDATAs.Clear();
                                _BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs.Clear();
                                _BR_BRS_SEL_INTERFER_SITUATION_SUM.INDATAs.Add(new BR_BRS_SEL_INTERFER_SITUATION_SUM.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID
                                });

                                if (await _BR_BRS_SEL_INTERFER_SITUATION_SUM.Execute() == false)
                                {
                                    throw _BR_BRS_SEL_INTERFER_SITUATION_SUM.Exception;
                                }
                                //2024.11.05 김도연 [샘플 검체 채취] + [환경모니터링(부유입자) 기준 초과] 두 항목의 간섭 횟수를 합친 총합으로 Validation을 진행함.
                                var elements = (from data in _BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs
                                                where data.SEQ == 13 | data.SEQ == 18
                                                select data).ToList();
                                foreach (var ele in elements)
                                {
                                    CHECK_SUM += Convert.ToDecimal(ele.FREQUENCY);
                                }
                                foreach (var check in _BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs)
                                {
                                    if (check.SEQ == 13 | check.SEQ == 18)
                                    {
                                        if (Convert.ToDecimal(_BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs[Convert.ToInt32(check.SEQ)].CRITERIA) < CHECK_SUM)
                                        {
                                            OVER_FLAG = true;
                                        }
                                        _ListInterfer.Add(new InterferSituation
                                        {
                                            SEQ = Convert.ToDecimal(check.SEQ),
                                            SITUATION = check.CONTENTS,
                                            GUBUN = check.GUBUN,
                                            CRITERIA = Convert.ToDecimal(check.CRITERIA),
                                            SUMNO = Convert.ToDecimal(check.FREQUENCY),
                                            OVER_COLOR = Convert.ToDecimal(_BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs[Convert.ToInt32(check.SEQ)].CRITERIA) < CHECK_SUM ? "Yellow" : "Tranparent"
                                        });
                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(_BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs[Convert.ToInt32(check.SEQ)].CRITERIA) < Convert.ToDecimal(check.FREQUENCY))
                                        {
                                            OVER_FLAG = true;
                                        }
                                        _ListInterfer.Add(new InterferSituation
                                        {
                                            SEQ = Convert.ToDecimal(check.SEQ),
                                            SITUATION = check.CONTENTS,
                                            GUBUN = check.GUBUN,
                                            CRITERIA = Convert.ToDecimal(check.CRITERIA),
                                            SUMNO = Convert.ToDecimal(check.FREQUENCY),
                                            OVER_COLOR = Convert.ToDecimal(_BR_BRS_SEL_INTERFER_SITUATION_SUM.OUTDATAs[Convert.ToInt32(check.SEQ)].CRITERIA) < Convert.ToDecimal(check.FREQUENCY) ? "Yellow" : "Tranparent"
                                        });
                                    }
                                }
                            }
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

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "Image attached";

                            var authHelper = new iPharmAuthCommandHelper();

                            if (OVER_FLAG)
                            {
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                enumRoleType inspectorRole = enumRoleType.ROLE001;
                                if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "최대 간섭 횟수를 초과하였습니다.",
                                        "최대 간섭 횟수 초과",
                                        true,
                                        "OM_ProductionOrder_Deviation",
                                        "",
                                        this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                        this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                {
                                    return;
                                }
                                _mainWnd.CurrentInstruction.Raw.DVTYN = "Y";
                                _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");
                                _mainWnd.CurrentInstruction.Raw.DVTCOMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation");
                            }

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                enumRoleType inspectorRole = enumRoleType.ROLE001;
                                if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("기록값을 변경합니다."),
                                        string.Format("기록값 변경"),
                                        true,
                                        "OM_ProductionOrder_SUI",
                                        "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }
                            else
                            {
                                // 전자서명 후 BR 실행
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    string.Format("간섭상황기록"),
                                    string.Format("간섭상황기록"),
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    _mainWnd.CurrentOrderInfo.EquipmentID, _mainWnd.CurrentOrderInfo.RecipeID, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }
                            }

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);

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
        #region User Define
        public class InterferSituation : ViewModelBase
        {
            private decimal _SEQ;
            public decimal SEQ
            {
                get { return _SEQ; }
                set
                {
                    _SEQ = value;
                    OnPropertyChanged("SEQ");
                }
            }
            private string _SITUATION;
            public string SITUATION
            {
                get { return _SITUATION; }
                set
                {
                    _SITUATION = value;
                    OnPropertyChanged("SITUATION");
                }
            }
            private string _GUBUN;
            public string GUBUN
            {
                get { return _GUBUN; }
                set
                {
                    _GUBUN = value;
                    OnPropertyChanged("GUBUN");
                }
            }
            private Decimal _CRITERIA;
            public Decimal CRITERIA
            {
                get { return _CRITERIA; }
                set
                {
                    _CRITERIA = value;
                    OnPropertyChanged("CRITERIA");
                }
            }
            private Decimal _SUMNO;
            public Decimal SUMNO
            {
                get { return _SUMNO; }
                set
                {
                    _SUMNO = value;
                    OnPropertyChanged("SUMNO");
                }
            }
            private String _OVER_COLOR;
            public String OVER_COLOR
            {
                get { return _OVER_COLOR; }
                set
                {
                    _OVER_COLOR = value;
                    OnPropertyChanged("OVER_COLOR");
                }
            }
        }
        #endregion
        #region imageToByteArray
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