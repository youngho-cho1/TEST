using LGCNS.iPharmMES.Common;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using ShopFloorUI;
using LGCNS.EZMES.Common;

namespace 보령
{
    public class 반제품저울스캔팝업ViewModel : ViewModelBase
    {

        #region [Property]
        enum enumScanType
        {
            Output,
            Scale
        };

        enum enumCheckStep
        {
            WaitBarcode
        };

        enumCheckStep _checkStep;
        private enumCheckStep CheckStep
        {
            get { return _checkStep; }
            set
            {
                _checkStep = value;

                switch (value)
                {
                    case enumCheckStep.WaitBarcode:
                        IsBarcodeReaderVisible = Visibility.Visible;
                        if (_ScanType.ToString() == enumScanType.Output.ToString())
                        {
                            Message = "용기 바코드를 스캔하세요";
                        }
                        else if (_ScanType.ToString() == enumScanType.Scale.ToString())
                        {
                            Message = "저울 바코드를 스캔하세요";
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        enumScanType _ScanType;
        private enumScanType ScanType
        {
            get { return _ScanType; }
            set
            {
                _ScanType = value;
                NotifyPropertyChanged();
            }
        }


        string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                NotifyPropertyChanged();
            }
        }

        string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        Visibility _isBarcodeReaderVisible;
        public Visibility IsBarcodeReaderVisible
        {
            get { return _isBarcodeReaderVisible; }
            set
            {
                _isBarcodeReaderVisible = value;
                NotifyPropertyChanged();
            }
        }

        저울반제품투입ViewModel _InsertVM;
        public 저울반제품투입ViewModel InsertVM
        {
            get { return _InsertVM; }
            set { _InsertVM = value; }
        }

        반제품분할ViewModel _DivisionVM;
        public 반제품분할ViewModel DivisionVM
        {
            get { return _DivisionVM; }
            set { _DivisionVM = value; }
        }

        iPharmMESChildWindow _popup;
        public iPharmMESChildWindow Popup
        {
            get { return _popup; }
            set { _popup = value; }
        }
        #endregion

        async Task<bool> IsMatchedComponent(string barcode)
        {
            ///
            if (ScanType.Equals(enumScanType.Output))
            {
                var bizRule = new BR_PHR_SEL_ProductionOrderOutput_PO_OPSG();
                bizRule.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_PO_OPSG.INDATA()
                {
                    VESSELID = barcode,
                    POID = DivisionVM != null ? DivisionVM.OrderNo : InsertVM.OrderNo,
                    OPSGID = DivisionVM != null ? DivisionVM.ProcessSegmentID : InsertVM.ProcessSegmentID
                });

                if (await bizRule.Execute() == false) return false;

                if (bizRule.OUTDATAs.Count <= 0)
                {
                    Message = "생성된 Output이 존재하지 않습니다.";
                    return false;
                }

                if (DivisionVM != null)
                {
                    DivisionVM.ToVesselID = bizRule.OUTDATAs[0].VESSELID;
                    DivisionVM.TareWeight = bizRule.OUTDATAs[0].TAREWEIGHT.ToString();
                    DivisionVM.TareUOM = bizRule.OUTDATAs[0].TAREUOM.ToString();
                    DivisionVM.TareUOMId = bizRule.OUTDATAs[0].TAREUOMID;
                    DivisionVM.Tare = string.Format("{0}{1}", decimal.Parse(bizRule.OUTDATAs[0].TAREWEIGHT.ToString()).ToString("0.##0"), bizRule.OUTDATAs[0].TAREUOM.ToString());
                }
                else
                {
                    InsertVM.VesselId = bizRule.OUTDATAs[0].VESSELID;
                    InsertVM.TareWeight = bizRule.OUTDATAs[0].TAREWEIGHT.ToString();
                    InsertVM.TareUOM = bizRule.OUTDATAs[0].TAREUOM.ToString();
                    InsertVM.Tare = string.Format("{0}{1}", decimal.Parse(bizRule.OUTDATAs[0].TAREWEIGHT.ToString()).ToString("0.##0"), bizRule.OUTDATAs[0].TAREUOM.ToString());
                }
            }
            else if (ScanType.Equals(enumScanType.Scale))
            {

                var bizRule = new BR_BRS_CHK_Equipment_Is_Scale();
                bizRule.INDATAs.Add(new BR_BRS_CHK_Equipment_Is_Scale.INDATA()
                {
                    EQPTID = barcode
                });

                if (await bizRule.Execute() == false) return false;

                if (DivisionVM != null)
                    DivisionVM.ScaleID = barcode;
                else
                    InsertVM.ScaleId = barcode;
            }
            ///

            return true;
        }
        #region [Command]
        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        IsBusy = true;

                        CommandResults["LoadedCommand"] = false;
                        CommandCanExecutes["LoadedCommand"] = false;

                        CheckStep = enumCheckStep.WaitBarcode;
                        ///

                        CommandResults["LoadedCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LoadedCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadedCommand"] = true;

                        IsBusy = false;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        //바코드 입력
        public ICommand BarcodeChangedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BarcodeChangedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["BarcodeChangedCommand"] = false;
                            CommandCanExecutes["BarcodeChangedCommand"] = false;

                            ///
                            if (await IsMatchedComponent(Barcode) == true)
                            {
                                if (ScanType.Equals(enumScanType.Scale))
                                {
                                    var bizRule = new BR_PHR_SEL_CurrentWeight();
                                    bizRule.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                                    {
                                        ScaleID = DivisionVM != null ? DivisionVM.ScaleID : InsertVM.ScaleId
                                    });

                                    await bizRule.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent);

                                    if (bizRule.OUTDATAs.Count > 0)
                                    {
                                        if (DivisionVM != null)
                                        {
                                            DivisionVM.ScaleWeight = bizRule.OUTDATAs[0].Weight.ToString();
                                            DivisionVM.ScaleUOM = bizRule.OUTDATAs[0].UOM.ToString();
                                            DivisionVM.Scale = string.Format("{0}{1}", decimal.Parse(bizRule.OUTDATAs[0].Weight.ToString()).ToString("#.00#"), bizRule.OUTDATAs[0].UOM.ToString());
                                        }
                                        else
                                        {
                                            InsertVM.ScaleValue = bizRule.OUTDATAs[0].Weight.ToString();
                                            InsertVM.ScaleUOM = bizRule.OUTDATAs[0].UOM.ToString();
                                            InsertVM.Scale = string.Format("{0}{1}", decimal.Parse(bizRule.OUTDATAs[0].Weight.ToString()).ToString("#.00#"), bizRule.OUTDATAs[0].UOM.ToString());
                                            InsertVM.IsEnabled = true;
                                        }
                                    }
                                    else
                                    {
                                        if(DivisionVM == null)
                                            InsertVM.IsEnabled = false;
                                    }
                                }

                                (arg as ChildWindow).DialogResult = true;
                            };
                            ///

                            CommandResults["BarcodeChangedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BarcodeChangedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BarcodeChangedCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BarcodeChangedCommand") ?
                        CommandCanExecutes["BarcodeChangedCommand"] : (CommandCanExecutes["BarcodeChangedCommand"] = true);
                });
            }
        }
        #endregion


        public 반제품저울스캔팝업ViewModel(Enum strScanType)
        {

            if (strScanType.ToString().Equals(enumScanType.Output.ToString()))
            {
                _ScanType = enumScanType.Output;
            }
            else if (strScanType.ToString().Equals(enumScanType.Scale.ToString()))
            {
                _ScanType = enumScanType.Scale;
            }

            _checkStep = enumCheckStep.WaitBarcode;

        }
    }
}
