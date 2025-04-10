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
    public class 반제품스캔팝업ViewModel : ViewModelBase
    {



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
                         Message = "용기 바코드를 스캔하세요";
                        break;
                    default:
                        break;
                }
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

        과립반제품투입ViewModel _parentVM;
        public 과립반제품투입ViewModel ParentVM
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
            if(ParentVM.BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.OUTDATAs.Count <= 0 )
            {
                Message = "생성된 Output이 존재하지 않습니다.";
                return false;
            }


            var item = ParentVM.BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.OUTDATAs.Where(o => o.VESSELID == barcode).FirstOrDefault();

            if (item == null)
            {
                Message = "Output 정보와 일치하지 않습니다.";
                return false;
            }

            item.CHECK = "True";
            ParentVM.inputBtnEnable = true;

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

      

        public 반제품스캔팝업ViewModel()
        {

            _checkStep = enumCheckStep.WaitBarcode;

        }
    }
}
