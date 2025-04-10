using LGCNS.iPharmMES.Common;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Linq;
using System.ComponentModel;
using 보령.UserControls;

namespace 보령
{
    public class 공정검사결과입력ViewModel : ViewModelBase
    {
        public 공정검사결과입력ViewModel(공정검사결과입력 wnd, IPCControlData data)
        {
            _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID = new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID();

            if (wnd != null && wnd is 공정검사결과입력)
            {
                _mainWnd = wnd as 공정검사결과입력;

                if (data != null && data is IPCControlData)
                {
                    _curIPCData = data;
                    TestDesc = _curIPCData.TINAME;
                    TestStandard = _curIPCData.Standard;
                    switch (_curIPCData.Type)
                    {
                        case IPCControlData.IPCType.Boolean:
                            BooleanInputVisble = true;
                            NumericInputVisble = false;
                            FriabilityInputVisble = false;
                            break;
                        case IPCControlData.IPCType.Numeric:
                            BooleanInputVisble = false;
                            NumericInputVisble = true;
                            FriabilityInputVisble = false;
                            break;
                        case IPCControlData.IPCType.Friability:
                            BooleanInputVisble = false;
                            NumericInputVisble = false;
                            FriabilityInputVisble = true;
                            ScaleValueSaveButtonContent = "시험전무게 저장";
                            break;
                        default:
                            BooleanInputVisble = false;
                            NumericInputVisble = false;
                            FriabilityInputVisble = false;
                            break;
                    }
                }
                else
                {
                    OnMessage("검사 정보가 없습니다.");
                    TestDesc = "N/A";
                    TestStandard = "N/A";
                    BooleanInputVisble = false;
                    NumericInputVisble = false;
                    FriabilityInputVisble = false;
                }
            }
        }

        private 공정검사결과입력 _mainWnd;
        private IPCControlData _curIPCData;

        private string _TestDesc;
        public string TestDesc
        {
            get { return _TestDesc; }
            set
            {
                _TestDesc = value;
                OnPropertyChanged("TestDesc");
            }
        }
        private string _TestStandard;
        public string TestStandard
        {
            get { return _TestStandard; }
            set
            {
                _TestStandard = value;
                OnPropertyChanged("TestStandard");
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

                            ///

                            switch (_curIPCData.Type)
                            {
                                case IPCControlData.IPCType.Numeric:
                                    _curIPCData.ACTVAL = GetNumericIPCRslt();
                                    break;
                                case IPCControlData.IPCType.Friability:
                                    _curIPCData.ACTVAL = _FriabilityRslt;
                                    break;
                                default:
                                    _mainWnd.DialogResult = false;
                                    return;
                            }

                            if(_curIPCData.DEVIATIONFLAG != null)
                            {
                                _mainWnd.DialogResult = true;
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

        #region Boolean Type
        #region Property
        private bool _BooleanInputVisble;
        public bool BooleanInputVisble
        {
            get { return _BooleanInputVisble; }
            set
            {
                _BooleanInputVisble = value;
                OnPropertyChanged("BooleanInputVisble");
            }
        }
        #endregion
        #region Command
        public ICommand BooleanButtonClickCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["BooleanButtonClickCommand"] = false;
                        CommandCanExecutes["BooleanButtonClickCommand"] = false;

                        ///
                        if(arg != null && arg is bool)
                        {
                            if((bool)arg)
                                _curIPCData.ACTVAL = 1m;
                            else
                                _curIPCData.ACTVAL = 0m;

                            _mainWnd.DialogResult = true;
                        }
                        ///

                        CommandResults["BooleanButtonClickCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["BooleanButtonClickCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["BooleanButtonClickCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("BooleanButtonClickCommand") ?
                       CommandCanExecutes["BooleanButtonClickCommand"] : (CommandCanExecutes["BooleanButtonClickCommand"] = true);
               });
            }
        }
        #endregion
        #endregion
        #region Numeric Type
        #region Property
        private bool _NumericInputVisble;
        public bool NumericInputVisble
        {
            get { return _NumericInputVisble; }
            set
            {
                _NumericInputVisble = value;
                OnPropertyChanged("NumericInputVisble");
            }
        }
        private string _NumericRslt;
        public string NumericRslt
        {
            get { return _NumericRslt; }
            set
            {
                _NumericRslt = value;
                OnPropertyChanged("NumericRslt");
            }
        }
        #endregion
        #region Command & Function
        public ICommand NumericPadOpenCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["NumericPadOpenCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["NumericPadOpenCommandAsync"] = false;
                            CommandCanExecutes["NumericPadOpenCommandAsync"] = false;

                            ///

                            ShopFloorUI.KeyPadPopUp popup = new ShopFloorUI.KeyPadPopUp();
                            popup.Closed += (sender1, e1) =>
                            {
                                if (popup.DialogResult.GetValueOrDefault())
                                {
                                    string rslt = popup.Value;
                                    decimal val;

                                    if (!string.IsNullOrWhiteSpace(rslt))
                                    {
                                        if (decimal.TryParse(rslt, out val))
                                        {
                                            //NumericRslt = decimal.Round(val, _curIPCData.PRECISION).ToString();
                                            NumericRslt = MathExt.Round(val, _curIPCData.PRECISION, MidpointRounding.AwayFromZero).ToString();
                                        }
                                        else
                                            OnMessage("숫자로 변환 실패");
                                    }
                                }
                            };

                            popup.Show();

                            ///

                            CommandResults["NumericPadOpenCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["NumericPadOpenCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["NumericPadOpenCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("NumericPadOpenCommandAsync") ?
                       CommandCanExecutes["NumericPadOpenCommandAsync"] : (CommandCanExecutes["NumericPadOpenCommandAsync"] = true);
               });
            }
        }
        private decimal? GetNumericIPCRslt()
        {
            decimal val;

            if (!string.IsNullOrWhiteSpace(NumericRslt))
            {
                if (decimal.TryParse(NumericRslt, out val))
                {
                    return val;
                }
                else
                    OnMessage("숫자로 변환 실패");
            }

            return null;
        }
        #endregion
        #endregion
        #region Friability Type
        private bool _FriabilityInputVisble;
        public bool FriabilityInputVisble
        {
            get { return _FriabilityInputVisble; }
            set
            {
                _FriabilityInputVisble = value;
                OnPropertyChanged("FriabilityInputVisble");
            }
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
        private Weight _ScaleValue = new Weight();
        public string ScaleValue
        {
            get { return _ScaleValue.WeightUOMStringWithSeperator; }
        }
        private string _ScaleValueSaveButtonContent;
        public string ScaleValueSaveButtonContent
        {
            get { return _ScaleValueSaveButtonContent; }
            set
            {
                _ScaleValueSaveButtonContent = value;
                OnPropertyChanged("ScaleValueSaveButtonContent");
            }
        }
        private Weight _PrevScaleValue = new Weight();
        public string PrevScaleValue
        {
            get { return _PrevScaleValue.WeightUOMStringWithSeperator; }
        }
        private Weight _AfterScaleValue = new Weight();
        public string AfterScaleValue
        {
            get { return _AfterScaleValue.WeightUOMStringWithSeperator; }
        }
        private decimal? _FriabilityRslt;
        public string FriabilityRslt
        {
            get { return _FriabilityRslt.HasValue ? _FriabilityRslt.Value.ToString() : ""; }
        }

        private DispatcherTimer _DispatcherTimer;
        private ScaleWebAPIHelper _restScaleService = new ScaleWebAPIHelper();
        #region BizRule
        private BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID;
        #endregion
        #region Command & Function
        public ICommand ScanScaleCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanScaleCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ScanScaleCommand"] = false;
                            CommandCanExecutes["ScanScaleCommand"] = false;

                            ///
                            IsBusy = true;
                            ScaleTimerSetup(false);

                            BarcodePopup popup = new BarcodePopup();
                            popup.tbMsg.Text = "저울바코드를 스캔하세요.";
                            popup.Closed += async (sender, e) =>
                            {
                                if (popup.DialogResult.GetValueOrDefault() && !string.IsNullOrWhiteSpace(popup.tbText.Text))
                                {
                                    string text = popup.tbText.Text.ToUpper();

                                    if (await GetScaleInfo(text))
                                    {
                                        ScaleId = _ScaleInfo.EQPTID;
                                        ScaleTimerSetup(true);
                                    }
                                    else
                                        ScaleId = "";
                                }
                                else
                                    ScaleId = "";
                            };

                            popup.Show();

                            IsBusy = false;
                            ///

                            CommandResults["ScanScaleCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
                            CommandResults["ScanScaleCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanScaleCommand"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanScaleCommand") ?
                        CommandCanExecutes["ScanScaleCommand"] : (CommandCanExecutes["ScanScaleCommand"] = true);
                });
            }
        }

        public ICommand SaveScaleValueCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["SaveScaleValueCommand"] = false;
                        CommandCanExecutes["SaveScaleValueCommand"] = false;

                        ///
                        if(ScaleValueSaveButtonContent == "시험전무게 저장")
                        {
                            _PrevScaleValue = _ScaleValue.Copy();
                            ScaleValueSaveButtonContent = "시험후무게 저장";
                            OnPropertyChanged("PrevScaleValue");

                            var item = _curIPCData.RawDatas.FirstOrDefault(o => o.COLLECTID.Equals("시험전무게"));
                            if (item == null)
                            {
                                _curIPCData.RawDatas.Add(new IPCControlRawData()
                                {
                                    COLLECTID = "시험전무게",
                                    ACTVAL = _PrevScaleValue.WeightUOMString
                                });
                            }
                            else
                            {
                                item.ACTVAL = _PrevScaleValue.WeightUOMString;
                            }
                        } 
                        else if(ScaleValueSaveButtonContent == "시험후무게 저장")
                        {
                            _AfterScaleValue = _ScaleValue.Copy();
                            _FriabilityRslt = Math.Round(decimal.Divide((_PrevScaleValue.Value - _AfterScaleValue.Value), (_PrevScaleValue.Value)) * 100m, _curIPCData.PRECISION);
                            OnPropertyChanged("FriabilityRslt");
                            OnPropertyChanged("AfterScaleValue");

                            var item = _curIPCData.RawDatas.FirstOrDefault(o => o.COLLECTID.Equals("시험후무게"));
                            if (item == null)
                            {
                                _curIPCData.RawDatas.Add(new IPCControlRawData()
                                {
                                    COLLECTID = "시험후무게",
                                    ACTVAL = _AfterScaleValue.WeightUOMString
                                });
                            }
                            else
                            {
                                item.ACTVAL = _AfterScaleValue.WeightUOMString;
                            }

                            // 결과 설명
                            _curIPCData.ACTVALDESC = String.Format("{0}% = {1} - {2} / {1} * 100",
                                _FriabilityRslt, _PrevScaleValue.WeightUOMString, _AfterScaleValue.WeightUOMString);

                            var actDESC = _curIPCData.RawDatas.FirstOrDefault(o => o.COLLECTID.Equals("결과값설명"));
                            if (actDESC == null)
                            {
                                _curIPCData.RawDatas.Add(new IPCControlRawData()
                                {
                                    COLLECTID = "결과값설명",
                                    ACTVAL = _curIPCData.ACTVALDESC
                                });
                            }
                            else
                            {
                                actDESC.ACTVAL = _curIPCData.ACTVALDESC;
                            }

                            if (_DispatcherTimer.IsEnabled)
                                _DispatcherTimer.Stop();

                        }
                        ///

                        CommandResults["SaveScaleValueCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["SaveScaleValueCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["SaveScaleValueCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("SaveScaleValueCommand") ?
                       CommandCanExecutes["SaveScaleValueCommand"] : (CommandCanExecutes["SaveScaleValueCommand"] = true);
               });
            }
        }


        /// <summary>
        /// 저울정보 조회
        /// </summary>
        /// <param name="scaleid"></param>
        /// <returns></returns>
        private async Task<bool> GetScaleInfo(string scaleid)
        {
            try
            {
                // 저울 정보 조회
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Clear();
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Clear();
                _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATAs.Add(new BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.INDATA
                {
                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                    EQPTID = scaleid
                });

                if (await _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.Execute() && _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs.Count > 0)
                {
                    _ScaleInfo = _BR_BRS_SEL_EquipmentCustomAttributeValue_ScaleInfo_EQPTID.OUTDATAs[0];

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
                return false;
            }
        }
        /// <summary>
        /// 저울타이머 세팅
        /// </summary>
        /// <param name="flag">True : Start, False : Stop</param>
        private void ScaleTimerSetup(bool flag)
        {
            if (flag)
            {
                if (_DispatcherTimer == null)
                    _DispatcherTimer = new DispatcherTimer();

                int interval = 2000;
                string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
                if (int.TryParse(interval_str, out interval) == false)
                {
                    interval = 2000;
                }
                _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
                _DispatcherTimer.Tick += _DispatcherTimer_Tick;
                _DispatcherTimer.Start();
            }
            else
            {
                if(_DispatcherTimer != null)
                    _DispatcherTimer.Stop();

                _DispatcherTimer = null;
            }
        }
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (_ScaleInfo != null)
                {
                    bool success = false;
                    Weight _ScaleWeight = new Weight();
                    if (_ScaleInfo.INTERFACE.ToUpper() == "REST")
                    {
                        var result = await _restScaleService.DownloadString(_ScaleInfo.EQPTID, ScaleCommand.GW);

                        if (result.code == "1")
                        {
                            success = true;
                            _ScaleWeight.SetWeight(result.data, result.unit);
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
                            _ScaleWeight.SetWeight(current_wight.OUTDATAs[0].Weight.Value, current_wight.OUTDATAs[0].UOM, Convert.ToInt32(_ScaleInfo.PRECISION.GetValueOrDefault()));
                        }
                    }

                    if (success)
                    {
                        _ScaleValue = _ScaleWeight;
                        OnPropertyChanged("ScaleValue");
                    }

                    if (_DispatcherTimer != null)
                        _DispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                _DispatcherTimer.Stop();
                OnException(ex.Message, ex);
            }
        }
        #endregion
        #endregion
    }
}
