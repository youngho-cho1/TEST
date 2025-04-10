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
    public class SVP수동검사불량유형입력ViewModel : ViewModelBase
    {
        #region Properties
        SVP수동검사불량유형입력 _mainWnd;

        private string _INSUSER;
        public string INSUSER
        {
            get { return _INSUSER; }
            set
            {
                _INSUSER = value;
                OnPropertyChanged("INSUSER");
            }
        }
        private string _INSPECTIONDTTM;
        public string INSPECTIONDTTM
        {
            get { return _INSPECTIONDTTM; }
            set
            {
                _INSPECTIONDTTM = value;
                OnPropertyChanged("INSPECTIONDTTM");
            }
        }
        private decimal _INSPECTCNT;
        public decimal INSPECTCNT
        {
            get { return _INSPECTCNT; }
            set
            {
                _INSPECTCNT = value;
                OnPropertyChanged("INSPECTCNT");
            }
        }
        private decimal _INSPECTQTY;
        public decimal INSPECTQTY
        {
            get { return _INSPECTQTY; }
            set
            {
                _INSPECTQTY = value;
                OnPropertyChanged("INSPECTQTY");
            }
        }
        private decimal _TOTAL_GOODQTY;
        public decimal TOTAL_GOODQTY
        {
            get { return _TOTAL_GOODQTY; }
            set
            {
                _TOTAL_GOODQTY = value;
                OnPropertyChanged("TOTAL_GOODQTY");
            }
        }
        private decimal _TOTAL_REJECTQTY;
        public decimal TOTAL_REJECTQTY
        {
            get { return _TOTAL_REJECTQTY; }
            set
            {
                _TOTAL_REJECTQTY = value;
                OnPropertyChanged("TOTAL_REJECTQTY");
            }
        }
        private decimal _CRITICAL_REJECTQTY;
        public decimal CRITICAL_REJECTQTY
        {
            get { return _CRITICAL_REJECTQTY; }
            set
            {
                _CRITICAL_REJECTQTY = value;
                OnPropertyChanged("CRITICAL_REJECTQTY");
            }
        }
        private decimal _MIDDLE_REJECTQTY;
        public decimal MIDDLE_REJECTQTY
        {
            get { return _MIDDLE_REJECTQTY; }
            set
            {
                _MIDDLE_REJECTQTY = value;
                OnPropertyChanged("MIDDLE_REJECTQTY");
            }
        }
        private decimal _LIGHT_REJECTQTY;
        public decimal LIGHT_REJECTQTY
        {
            get { return _LIGHT_REJECTQTY; }
            set
            {
                _LIGHT_REJECTQTY = value;
                OnPropertyChanged("LIGHT_REJECTQTY");
            }
        }
        private decimal _REJECT_NO1;
        public decimal REJECT_NO1
        {
            get { return _REJECT_NO1; }
            set
            {
                _REJECT_NO1 = value;
                OnPropertyChanged("REJECT_NO1");
            }
        }
        private decimal _REJECT_NO2;
        public decimal REJECT_NO2
        {
            get { return _REJECT_NO2; }
            set
            {
                _REJECT_NO2 = value;
                OnPropertyChanged("REJECT_NO2");
            }
        }
        private decimal _REJECT_NO3;
        public decimal REJECT_NO3
        {
            get { return _REJECT_NO3; }
            set
            {
                _REJECT_NO3 = value;
                OnPropertyChanged("REJECT_NO3");
            }
        }
        private decimal _REJECT_NO4;
        public decimal REJECT_NO4
        {
            get { return _REJECT_NO4; }
            set
            {
                _REJECT_NO4 = value;
                OnPropertyChanged("REJECT_NO4");
            }
        }
        private decimal _REJECT_NO5;
        public decimal REJECT_NO5
        {
            get { return _REJECT_NO5; }
            set
            {
                _REJECT_NO5 = value;
                OnPropertyChanged("REJECT_NO5");
            }
        }
        private decimal _REJECT_NO6;
        public decimal REJECT_NO6
        {
            get { return _REJECT_NO6; }
            set
            {
                _REJECT_NO6 = value;
                OnPropertyChanged("REJECT_NO6");
            }
        }
        private decimal _REJECT_NO7;
        public decimal REJECT_NO7
        {
            get { return _REJECT_NO7; }
            set
            {
                _REJECT_NO7 = value;
                OnPropertyChanged("REJECT_NO7");
            }
        }
        private decimal _REJECT_NO8;
        public decimal REJECT_NO8
        {
            get { return _REJECT_NO8; }
            set
            {
                _REJECT_NO8 = value;
                OnPropertyChanged("REJECT_NO8");
            }
        }
        private decimal _REJECT_NO9;
        public decimal REJECT_NO9
        {
            get { return _REJECT_NO9; }
            set
            {
                _REJECT_NO9 = value;
                OnPropertyChanged("REJECT_NO9");
            }
        }
        private decimal _REJECT_NO10;
        public decimal REJECT_NO10
        {
            get { return _REJECT_NO10; }
            set
            {
                _REJECT_NO10 = value;
                OnPropertyChanged("REJECT_NO10");
            }
        }
        private decimal _REJECT_NO11;
        public decimal REJECT_NO11
        {
            get { return _REJECT_NO11; }
            set
            {
                _REJECT_NO11 = value;
                OnPropertyChanged("REJECT_NO11");
            }
        }
        private decimal _REJECT_NO12;
        public decimal REJECT_NO12
        {
            get { return _REJECT_NO12; }
            set
            {
                _REJECT_NO12 = value;
                OnPropertyChanged("REJECT_NO12");
            }
        }
        private decimal _REJECT_NO13;
        public decimal REJECT_NO13
        {
            get { return _REJECT_NO13; }
            set
            {
                _REJECT_NO13 = value;
                OnPropertyChanged("REJECT_NO13");
            }
        }
        private decimal _REJECT_NO14;
        public decimal REJECT_NO14
        {
            get { return _REJECT_NO14; }
            set
            {
                _REJECT_NO14 = value;
                OnPropertyChanged("REJECT_NO14");
            }
        }
        private decimal _REJECT_NO15;
        public decimal REJECT_NO15
        {
            get { return _REJECT_NO15; }
            set
            {
                _REJECT_NO15 = value;
                OnPropertyChanged("REJECT_NO15");
            }
        }
        private decimal _REJECT_NO16;
        public decimal REJECT_NO16
        {
            get { return _REJECT_NO16; }
            set
            {
                _REJECT_NO16 = value;
                OnPropertyChanged("REJECT_NO16");
            }
        }
        private decimal _REJECT_NO17;
        public decimal REJECT_NO17
        {
            get { return _REJECT_NO17; }
            set
            {
                _REJECT_NO17 = value;
                OnPropertyChanged("REJECT_NO17");
            }
        }
        private decimal _REJECT_NO18;
        public decimal REJECT_NO18
        {
            get { return _REJECT_NO18; }
            set
            {
                _REJECT_NO18 = value;
                OnPropertyChanged("REJECT_NO18");
            }
        }
        private decimal _REJECT_NO19;
        public decimal REJECT_NO19
        {
            get { return _REJECT_NO19; }
            set
            {
                _REJECT_NO19 = value;
                OnPropertyChanged("REJECT_NO19");
            }
        }
        private decimal _REJECT_NO20;
        public decimal REJECT_NO20
        {
            get { return _REJECT_NO20; }
            set
            {
                _REJECT_NO20 = value;
                OnPropertyChanged("REJECT_NO20");
            }
        }
        private decimal _REJECT_NO21;
        public decimal REJECT_NO21
        {
            get { return _REJECT_NO21; }
            set
            {
                _REJECT_NO21 = value;
                OnPropertyChanged("REJECT_NO21");
            }
        }

        private string _COMMENTS;
        public string COMMENTS
        {
            get { return _COMMENTS; }
            set
            {
                _COMMENTS = value;
                OnPropertyChanged("COMMENTS");
            }
        }

        bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region [Constructor]

        public SVP수동검사불량유형입력ViewModel()
        {
            _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO = new BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO();
            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO = new BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO();
            IsEnabled = false;
        }

        #endregion

        #region [BizRule]
        private BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO;
        public BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO
        {
            get { return _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO; }
            set
            {
                _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO = value;
                OnPropertyChanged("BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO");
            }
        }

        private BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO;
        public BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO
        {
            get { return _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO; }
            set
            {
                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO");
            }
        }
        #endregion

        #region [Command]
        public ICommand LoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadCommandAsync"] = false;
                            CommandCanExecutes["LoadCommandAsync"] = false;
                            IsBusy = true;

                            if (arg != null && arg is SVP수동검사불량유형입력)
                            {
                                _mainWnd = arg as SVP수동검사불량유형입력;

                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATAs.Clear();
                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.OUTDATAs.Clear();

                                BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATAs.Add(new BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.INDATA
                                {
                                    RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                    ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                    IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                    IRTSEQ = _mainWnd.CurrentInstruction.Raw.IRTSEQ,
                                    POID = _mainWnd.CurrentOrder.OrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (await _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.Execute() && _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.OUTDATAs.Count == 0)
                                {
                                    INSUSER = AuthRepositoryViewModel.Instance.LoginedUserName + "(" + AuthRepositoryViewModel.Instance.LoginedUserID + ")";
                                    INSPECTIONDTTM = DateTime.Now.ToString("yyyy-MM-dd");

                                    IsEnabled = true;
                                }
                                else
                                {
                                    var outData = _BR_BRS_SEL_UDT_BRS_SVP_REJECT_INFO.OUTDATAs[0];
                                    
                                    INSUSER = outData.INSUSER;
                                    INSPECTIONDTTM = outData.INSPECTIONDTTM;
                                    INSPECTCNT = Convert.ToDecimal(outData.INSPECTCNT);
                                    INSPECTQTY = Convert.ToDecimal(outData.INSPECTQTY);
                                    TOTAL_GOODQTY = Convert.ToDecimal(outData.TOTAL_GOODQTY);
                                    TOTAL_REJECTQTY = Convert.ToDecimal(outData.TOTAL_REJECTQTY);
                                    CRITICAL_REJECTQTY = Convert.ToDecimal(outData.CRITICAL_REJECTQTY);
                                    MIDDLE_REJECTQTY = Convert.ToDecimal(outData.MIDDLE_REJECTQTY);
                                    LIGHT_REJECTQTY = Convert.ToDecimal(outData.LIGHT_REJECTQTY);
                                    REJECT_NO1 = Convert.ToDecimal(outData.REJECT_NO1);
                                    REJECT_NO2 = Convert.ToDecimal(outData.REJECT_NO2);
                                    REJECT_NO3 = Convert.ToDecimal(outData.REJECT_NO3);
                                    REJECT_NO4 = Convert.ToDecimal(outData.REJECT_NO4);
                                    REJECT_NO5 = Convert.ToDecimal(outData.REJECT_NO5);
                                    REJECT_NO6 = Convert.ToDecimal(outData.REJECT_NO6);
                                    REJECT_NO7 = Convert.ToDecimal(outData.REJECT_NO7);
                                    REJECT_NO8 = Convert.ToDecimal(outData.REJECT_NO8);
                                    REJECT_NO9 = Convert.ToDecimal(outData.REJECT_NO9);
                                    REJECT_NO10 = Convert.ToDecimal(outData.REJECT_NO10);
                                    REJECT_NO11 = Convert.ToDecimal(outData.REJECT_NO11);
                                    REJECT_NO12 = Convert.ToDecimal(outData.REJECT_NO12);
                                    REJECT_NO13 = Convert.ToDecimal(outData.REJECT_NO13);
                                    REJECT_NO14 = Convert.ToDecimal(outData.REJECT_NO14);
                                    REJECT_NO15 = Convert.ToDecimal(outData.REJECT_NO15);
                                    REJECT_NO16 = Convert.ToDecimal(outData.REJECT_NO16);
                                    REJECT_NO17 = Convert.ToDecimal(outData.REJECT_NO17);
                                    REJECT_NO18 = Convert.ToDecimal(outData.REJECT_NO18);
                                    REJECT_NO19 = Convert.ToDecimal(outData.REJECT_NO19);
                                    REJECT_NO20 = Convert.ToDecimal(outData.REJECT_NO20);
                                    REJECT_NO21 = Convert.ToDecimal(outData.REJECT_NO21);
                                    COMMENTS = outData.COMMENTS;

                                    IsEnabled = false;
                                }
                                
                            }

                            IsBusy = false;

                            CommandResults["LoadCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["LoadCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
                });
            }
        }

        public ICommand NoRecordConfirmCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            IsBusy = true;

                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, false, false, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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

        public ICommand SaveCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                using (await AwaitableLocks["SaveCommandAsync"].EnterAsync())
                {
                    try
                    {
                        CommandResults["SaveCommandAsync"] = false;
                        CommandCanExecutes["SaveCommandAsync"] = false;
                            
                        var authHelper = new iPharmAuthCommandHelper();                            
                        // 조회내용 기록
                        authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                        if (await authHelper.ClickAsync(
                            Common.enumCertificationType.Function,
                            Common.enumAccessType.Create,
                            "SVP수동검사불량유형입력",
                            "SVP수동검사불량유형입력",
                            false,
                            "OM_ProductionOrder_SUI",
                            "", null, null) == false)
                        {
                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                        }
                        
                            _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO.INDATAs.Clear();
                            _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO.INDATAs.Add(new BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO.INDATA
                            {
                                RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                IRTSEQ = _mainWnd.CurrentInstruction.Raw.IRTSEQ,
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                BATCHNO = _mainWnd.CurrentOrder.BatchNo,
                                SEQ = 1,    //2023.01.10 박희돈 HIST 테이블에 값이 없을경우 SEQ값을 1로한다. 값이 있다면 비즈룰에서 HIST테이블의 SEQ값을 조회함.
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                INSPECTCNT = INSPECTCNT,
                                INSPECTIONDTTM = Convert.ToString(INSPECTIONDTTM.ToString()).Substring(0, 10),
                                INSPECTQTY = INSPECTQTY,
                                TOTAL_GOODQTY = INSPECTQTY - (REJECT_NO1 + REJECT_NO2 + REJECT_NO3 + REJECT_NO4 + REJECT_NO5 + REJECT_NO6 + REJECT_NO7 + REJECT_NO8
                                                + REJECT_NO9 + REJECT_NO10 + REJECT_NO11 + REJECT_NO12 + REJECT_NO13 + REJECT_NO14 + REJECT_NO15 + REJECT_NO16
                                                + REJECT_NO17 + REJECT_NO18 + REJECT_NO19 + REJECT_NO20 + REJECT_NO21),
                                TOTAL_REJECTQTY = REJECT_NO1 + REJECT_NO2 + REJECT_NO3 + REJECT_NO4 + REJECT_NO5 + REJECT_NO6 + REJECT_NO7 + REJECT_NO8
                                                + REJECT_NO9 + REJECT_NO10 + REJECT_NO11 + REJECT_NO12 + REJECT_NO13 + REJECT_NO14 + REJECT_NO15 + REJECT_NO16
                                                + REJECT_NO17 + REJECT_NO18 + REJECT_NO19 + REJECT_NO20 + REJECT_NO21,
                                CRITICAL_REJECTQTY = REJECT_NO1 + REJECT_NO2 + REJECT_NO3 + REJECT_NO4 + REJECT_NO5 + REJECT_NO6 + REJECT_NO9 + REJECT_NO16 + REJECT_NO17,
                                MIDDLE_REJECTQTY = REJECT_NO7 + REJECT_NO8 + REJECT_NO10 + REJECT_NO13 + REJECT_NO14 + REJECT_NO18,
                                LIGHT_REJECTQTY = REJECT_NO11 + REJECT_NO12 + REJECT_NO15,
                                REJECT_NO1 = REJECT_NO1,
                                REJECT_NO2 = REJECT_NO2,
                                REJECT_NO3 = REJECT_NO3,
                                REJECT_NO4 = REJECT_NO4,
                                REJECT_NO5 = REJECT_NO5,
                                REJECT_NO6 = REJECT_NO6,
                                REJECT_NO7 = REJECT_NO7,
                                REJECT_NO8 = REJECT_NO8,
                                REJECT_NO9 = REJECT_NO9,
                                REJECT_NO10 = REJECT_NO10,
                                REJECT_NO11 = REJECT_NO11,
                                REJECT_NO12 = REJECT_NO12,
                                REJECT_NO13 = REJECT_NO13,
                                REJECT_NO14 = REJECT_NO14,
                                REJECT_NO15 = REJECT_NO15,
                                REJECT_NO16 = REJECT_NO16,
                                REJECT_NO17 = REJECT_NO17,
                                REJECT_NO18 = REJECT_NO18,
                                REJECT_NO19 = REJECT_NO19,
                                REJECT_NO20 = REJECT_NO20,
                                REJECT_NO21 = REJECT_NO21,
                                COMMENTS = COMMENTS,
                                INSUSER = AuthRepositoryViewModel.Instance.LoginedUserName + "(" + AuthRepositoryViewModel.Instance.LoginedUserID + ")",
                                INSDTTM = Convert.ToString(await AuthRepositoryViewModel.GetDBDateTimeNow())
                            });

                            if (await _BR_BRS_REG_UDT_BRS_SVP_REJECT_INFO.Execute() == true)
                            {
                                Brush background = _mainWnd.PrintArea.Background;
                                _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                                _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                                _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, false, false, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                                CommandResults["SaveCommandAsync"] = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveCommandAsync") ?
                        CommandCanExecutes["SaveCommandAsync"] : (CommandCanExecutes["SaveCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region User Define
        public byte[] imageToByteArray()
        {
            try
            {

                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
                System.IO.Stream stream = bitmap.GetStream(ImageFormat.Png, true);

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
