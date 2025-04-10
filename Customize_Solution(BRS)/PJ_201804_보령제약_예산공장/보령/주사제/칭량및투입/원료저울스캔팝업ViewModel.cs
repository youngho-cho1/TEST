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
    public class 원료저울스캔팝업ViewModel : ViewModelBase
    {

        enum enumScanType
        {
            Material,
            Scale
        };

        enum enumCheckStep
        {
            WaitBarcode,
            ComponentChecked,
            End
            
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
                        if (_ScanType.ToString() == enumScanType.Material.ToString())
                        {
                            Message = "원료 바코드를 스캔하세요";
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

        SVP원료칭량및투입ViewModel _parentVM;
        public SVP원료칭량및투입ViewModel ParentVM
        {
            get { return _parentVM; }
            set { _parentVM = value; }
        }
        iPharmMESChildWindow _popup;
        public iPharmMESChildWindow Popup
        {
            get { return _popup; }
            set { _popup = value; }
        }

      
        async Task<bool> IsMatchedComponent(string barcode)
        {

            if (ScanType.Equals(enumScanType.Material))
            {
                if (ParentVM != null)
                {
                    if (ParentVM.BR_BRS_SEL_Charging_Solvent_to_Dispense.OUTDATAs.Count > 0)
                    {

                        var item = ParentVM.FilteredComponents.Where(o => o.MSUBLOTBCD == Barcode).FirstOrDefault();

                        if (item != null)
                        {
                            ParentVM.curSeletedItem = item;
                            item.CHECK = item.CHECK == "투입대기" ? "투입가능" : item.CHECK;
                            CheckStep = enumCheckStep.End;
                        }
                        else
                        {
                            Message = "원료 정보가 일치하지 않습니다.";
                            return false;
                        }
                    }
                    else
                    {
                        Message = "투입 대상 원료가 존재하지 않습니다.";
                        return false;
                    }
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

                if (ParentVM != null)
                    ParentVM.ScaleId = barcode;

                CheckStep = enumCheckStep.End;
            }
            ///

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
                            if (await IsMatchedComponent(Barcode.ToUpper()) == true)
                            {
                                if (ScanType.Equals(enumScanType.Scale))
                                {
                                    CheckStep = enumCheckStep.End;
                                }

                                if (CheckStep == enumCheckStep.End)
                                    (arg as 원료저울스캔팝업).DialogResult = true;
 
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

      

        public 원료저울스캔팝업ViewModel(Enum strScanType)
        {

            if (strScanType.ToString().Equals(enumScanType.Material.ToString()))
            {
                _ScanType = enumScanType.Material;
            }
            else if (strScanType.ToString().Equals(enumScanType.Scale.ToString()))
            {
                _ScanType = enumScanType.Scale;
            }

            _checkStep = enumCheckStep.WaitBarcode;
 
           
        }
    }
}
