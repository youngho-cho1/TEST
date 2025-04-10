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
using System.Linq;

namespace 보령
{
    public class WMS칭량자재투입팝업ViewModel : ViewModelBase
    {
        #region [Property]

        WMS칭량자재투입팝업 _PopWnd;

        public BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing.OUTDATA currentComponent;

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
                        Message = "원료 바코드를 스캔하세요";
                        break;
                    case enumCheckStep.ComponentChargeChecked:
                        IsBarcodeReaderVisible = Visibility.Visible;
                        Message = string.Format("[{0}] 원료 투입 준비됨. 다음 원료 바코드를 스캔하세요", currentComponent.MSUBLOTID);
                        break;
                    case enumCheckStep.ComponentChargeCheckCompleted:
                        IsBarcodeReaderVisible = Visibility.Collapsed;
                        _ParentVM.IsEnable_OKBtn = true;
                        _ParentVM.ScanVisibility = Visibility.Collapsed;
                        Message = "모든 원료가 투입 준비되었습니다.";
                        break;
                    default:
                        break;
                }
            }
        }
        private WMS칭량자재투입ViewModel _ParentVM;
        public WMS칭량자재투입ViewModel ParentVM
        {
            get { return _ParentVM; }
            set { _ParentVM = value; }
        }
        string _Barcode;
        public string Barcode
        {
            get { return _Barcode; }
            set
            {
                _Barcode = value;
                OnPropertyChanged("Barcode");
            }
        }

        string _Message;
        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged("Message");
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
        #endregion
        #region [ICommand]
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

                        CheckStep = enumCheckStep.WaitBarcode;
                        if (arg != null)
                        {
                            _PopWnd = arg as WMS칭량자재투입팝업;
                        }
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
                            CommandResults["BarcodeChangedCommand"] = false;
                            CommandCanExecutes["BarcodeChangedCommand"] = false;

                            ///
                            if (!string.IsNullOrEmpty(Barcode))
                            {
                                await (ChargingCommand as AsyncCommandBase).ExecuteAsync(Barcode);
                                ParentVM.isConfrim = true;
                                Barcode = string.Empty;
                                _PopWnd.DialogResult = true;
                                
                            }
                            else
                            {
                                Barcode = string.Empty;
                            }
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
        public ICommand ChargingCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ChargingCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ChargingCommand"] = false;
                            CommandCanExecutes["ChargingCommand"] = false;

                            if (arg != null)
                            {
                                string barcode = arg as string;


                                var biz = new BR_BRS_CHK_ProductionOrderBOM_NonWeighing();
                                biz.INDATAs.Add(new BR_BRS_CHK_ProductionOrderBOM_NonWeighing.INDATA()
                                {
                                    POID = ParentVM.OrderNo,
                                    MSUBLOTBCD = barcode,
                                });

                                if (await biz.Execute())
                                {
                                    if (biz.OUTDATAs.Count > 0)
                                    {
                                        foreach (var item in biz.OUTDATAs)
                                        {
                                            var match = ParentVM.filteredComponents.Where(o => o.MSUBLOTBCD == item.MSUBLOTBCD).FirstOrDefault();

                                            if (match != null)
                                            {
                                                match.ISAVAIL = item.ISAVAIL;
                                                match.SCANNED_MSUBLOTBCD = item.SCANNED_MSUBLOTBCD;
                                            }
                                        }
                                    }
                                }
                            }

                            CommandResults["ChargingCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ChargingCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ChargingCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChargingCheckCommand") ?
                        CommandCanExecutes["ChargingCheckCommand"] : (CommandCanExecutes["ChargingCheckCommand"] = true);
                });
            }
        }
        #endregion

        public WMS칭량자재투입팝업ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
        }
    }
}
