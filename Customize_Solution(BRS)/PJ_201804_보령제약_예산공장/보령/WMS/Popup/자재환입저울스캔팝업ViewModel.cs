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
    public class 자재환입저울스캔팝업ViewModel : ViewModelBase
    {
        private 자재환입저울스캔팝업 _mainWnd;

        enum enumScanType
        {
            Scale,
            Quantity
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
                        
                        if (_ScanType.ToString() == enumScanType.Scale.ToString())
                        {
                            _mainWnd.tbBarcode.Visibility = Visibility.Visible;
                            _mainWnd.tbQty.Visibility = Visibility.Collapsed;
                            _mainWnd.comboUOM.Visibility = Visibility.Collapsed;
                            _mainWnd.btnConfirm.Visibility = Visibility.Collapsed;
                            Message = "저울 바코드를 스캔하세요";
                        }
                        else if (_ScanType.ToString() == enumScanType.Quantity.ToString())
                        {
                            _mainWnd.tbBarcode.Visibility = Visibility.Collapsed;
                            _mainWnd.tbQty.Visibility = Visibility.Visible;
                            _mainWnd.comboUOM.Visibility = Visibility.Visible;
                            _mainWnd.comboUOM.SelectedItem = _mainWnd.comboUOM.Items[1];
                            _mainWnd.btnConfirm.Visibility = Visibility.Visible;
                            Message = "수량을 입력하세요";
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

        WMS자재환입처리ViewModel _ParentVM;
        public WMS자재환입처리ViewModel ParentVM
        {
            get { return _ParentVM; }
            set { _ParentVM = value; }
        }

        CurrentOrderInfo _parentODInfo;
        public CurrentOrderInfo parentODInfo
        {
            get { return _parentODInfo; }
            set
            {
                _parentODInfo = value;
            }
        }

        async Task<bool> IsMatchedComponent(string barcode)
        {

            if (ScanType.Equals(enumScanType.Scale))
            {

                var bizRule = new BR_BRS_CHK_Equipment_Is_Scale();
                bizRule.INDATAs.Add(new BR_BRS_CHK_Equipment_Is_Scale.INDATA()
                {
                    EQPTID = barcode
                });

                if (await bizRule.Execute() == false) return false;

                ParentVM.SCALEID = barcode;
            }

            return true;
        }

        public ICommand LoadedCommand
        {
            get
            {
                return new CommandBase(arg =>
                {
                    try
                    {
                        CommandResults["LoadedCommand"] = false;
                        CommandCanExecutes["LoadedCommand"] = false;

                        if (arg != null && arg is 자재환입저울스캔팝업)
                            _mainWnd = arg as 자재환입저울스캔팝업;

                        _mainWnd.comboUOM.Items.Add("kg");
                        _mainWnd.comboUOM.Items.Add("g");
                        _mainWnd.comboUOM.Items.Add("ea");

                        CheckStep = enumCheckStep.WaitBarcode;

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
                            decimal res;

                            if (await IsMatchedComponent(Barcode) == true)
                            {
                                if (ScanType.Equals(enumScanType.Scale))
                                {
                                    var bizRule = new BR_PHR_SEL_CurrentWeight();

                                    bizRule.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                                    {
                                        ScaleID = Barcode
                                    });

                                    await bizRule.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent);

                                    if (bizRule.OUTDATAs.Count > 0)
                                    {
                                        ParentVM.ScaleUOM = bizRule.OUTDATAs[0].UOM.ToString();
                                        ParentVM.ScaleWeight = decimal.Parse(bizRule.OUTDATAs[0].Weight.ToString());
                                        ParentVM.SCALEVAL = bizRule.OUTDATAs[0].Weight.ToString();
                                    }
                                }
                                else if (ScanType.Equals(enumScanType.Quantity))
                                {
                                    if (decimal.TryParse(Barcode, out res))
                                    {
                                        res = decimal.Round(res, 3);

                                        ParentVM.SCALEID = "저울미사용";
                                        ParentVM.ScaleWeight = res;
                                        ParentVM.SCALEVAL = res.ToString();
                                        ParentVM.ScaleUOM = _mainWnd.comboUOM.SelectedItem.ToString();
                                        //ParentVM.RTN_NOTE = string.Format("스캔한 자재의 단위는 입니다.");
                                    }
                                    else
                                    {
                                        throw new Exception(MessageTable.M[CommonMessageCode._10010].Replace("{0}","3"));
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



        public 자재환입저울스캔팝업ViewModel(Enum strScanType)
        {

            if (strScanType.ToString().Equals(enumScanType.Scale.ToString()))
            {
                _ScanType = enumScanType.Scale;
            }
            else if (strScanType.ToString().Equals(enumScanType.Quantity.ToString()))
            {
                _ScanType = enumScanType.Quantity;
            }

            _checkStep = enumCheckStep.WaitBarcode;

        }
    }
}
