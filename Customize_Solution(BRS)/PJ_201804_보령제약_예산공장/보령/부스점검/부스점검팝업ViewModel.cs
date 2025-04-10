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
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using ShopFloorUI;
using LGCNS.iPharmMES.Common;

namespace 보령
{
    public class 부스점검팝업ViewModel : ViewModelBase
    {
        enum enumCheckStep
        {
            WaitBarcode,
            ComponentChargeChecked,
            ComponentChargeCheckCompleted
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
                        Message = "장비 바코드를 스캔하세요";
                        break;
                    case enumCheckStep.ComponentChargeChecked:
                        IsBarcodeReaderVisible = Visibility.Visible;
                        Message = string.Format("확인 완료. 다음 장비 바코드를 스캔하세요");
                        break;
                    default:
                        break;
                }
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

        public 부스점검팝업ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
            _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi = new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi();
            _부스점검팝업 = new 부스점검팝업();
        }

        부스점검ViewModel _ParentVM;
        public 부스점검ViewModel ParentVM
        {
            get { return _ParentVM; }
            set { _ParentVM = value; }
        }

        부스점검팝업 _부스점검팝업;
        public 부스점검팝업 부스점검팝업
        {
            get { return _부스점검팝업; }
            set { _부스점검팝업 = value; }
        }

        //새로운 데이터 입력
        BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi;
        public BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi
        {
            get { return _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi = value;
                NotifyPropertyChanged();
            }
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

                        ///
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
                            if(arg != null)
                                부스점검팝업 = arg as 부스점검팝업;

                            if (!(string.IsNullOrWhiteSpace(부스점검팝업.tbBarcode.Text)))
                                await IsMatchedComponent(Barcode);

                            Barcode = string.Empty;
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

        async Task<bool> IsMatchedComponent(string barcode)
        {
            //중복 체크
            foreach (var Equipment in _ParentVM.FilteredComponents)
            {
                if (Equipment.EQPTID.ToUpper() == barcode.ToUpper())
                {
                    C1.Silverlight.C1MessageBox.Show("입력하신 장비ID가 존재합니다.", "경고", C1.Silverlight.C1MessageBoxButton.OK, (r) =>
                    {
                        부스점검팝업.tbBarcode.Focus();
                    });
                    return false;
                }
            }
            var instruction = _ParentVM.mainWnd.CurrentInstruction;
            _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Clear();

            _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.INDATA()
            {
                LANGID = AuthRepositoryViewModel.Instance.LangID,
                EQPTID = barcode,
                POID = ParentVM.mainWnd.CurrentOrder.ProductionOrderID,
                OPSGGUID = ParentVM.mainWnd.CurrentOrder.OrderProcessSegmentID
            });

            if (await _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.Execute() == false)
                return false;

            var notCompletedItem = _BR_BRS_SEL_EquipmentStatus_4CleanBooth_Multi.OUTDATAs.Where(o => o.EQPTID == barcode).FirstOrDefault();

            if (notCompletedItem != null)
            {
                _ParentVM.FilteredComponents.Add(notCompletedItem);
                CheckStep = enumCheckStep.ComponentChargeChecked;
            }
            return true;
        }

    }
}
