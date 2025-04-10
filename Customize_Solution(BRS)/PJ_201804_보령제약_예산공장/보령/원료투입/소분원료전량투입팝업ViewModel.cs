using LGCNS.iPharmMES.Common;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using ShopFloorUI;

namespace 보령
{
    public class 소분원료전량투입팝업ViewModel : ViewModelBase
    {
        //ShopFloorCustomWindow _mainWnd;

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
                        Message = string.Format("[{0}] 원료 투입 준비됨. 다음 원료 바코드를 스캔하세요", _currentComponent.MSUBLOTID);
                        break;
                    case enumCheckStep.ComponentChargeCheckCompleted:
                        IsBarcodeReaderVisible = Visibility.Collapsed;
                        ParentVM.Is_EnableOKBtn = true;
                        Message = "모든 원료가 투입 준비되었습니다.";
                        break;
                    default:
                        break;
                }
            }
        }

        BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATA _currentComponent;

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

        소분원료전량투입ViewModel _parentVM;
        public 소분원료전량투입ViewModel ParentVM
        {
            get { return _parentVM; }
            set { _parentVM = value; }
        }

        async Task<bool> IsMatchedComponent(string barcode)
        {
            var bizRule = new BR_PHR_SEL_MaterialSubLot_DSP_ContainerInfo();
            bizRule.INDATAs.Add(new BR_PHR_SEL_MaterialSubLot_DSP_ContainerInfo.INDATA()
            {
                MSUBLOTBCD = barcode
            });

            if (await bizRule.Execute() == false) return false;

            if (bizRule.OUTDATAs.Count <= 0)
            {
                Message = "바코드에 맞는 정보가 조회되지 않습니다.";
                return false;
            }

            var matchedOrderComponents = bizRule.OUTDATAs.Where(o => o.WOID == ParentVM.OrderNo).ToList();
            if (matchedOrderComponents.Count <= 0)
            {
                Message = "오더에 해당하는 원료가 아닙니다.";
                return false;
            }

            var scannedComponent = matchedOrderComponents[0];

            //if (ParentVM.FilteredComponents[0].MTRLID != scannedComponent.MTRLID)
            //{
            //    Message = string.Format("투입 대상 원료 [{0}] 가 아닙니다. ", ParentVM.FilteredComponents[0].MTRLNAME);
            //    return false;
            //}

            var matchedComponent = ParentVM.filteredComponents.Where(o => o.MSUBLOTBCD == scannedComponent.MSUBLOTBCD).FirstOrDefault();
            if (matchedComponent == null)
            {
                Message = string.Format("[{0}] 은 투입 대상 원료 목록에 없습니다.", scannedComponent.MSUBLOTBCD);
                return false;
            }

            if (scannedComponent.MSUBLOTQTY <= 0)
            {
                Message = string.Format("[{0}] 은 현재 수량이 0 으로 투입할 수 없습니다.", scannedComponent.MSUBLOTBCD);
                return false;
            }

            var bizRuleChecked = new BR_PHR_SEL_ProductionOrderDispenseSubLot_SUBLOTID();
            bizRuleChecked.INDATAs.Add(new BR_PHR_SEL_ProductionOrderDispenseSubLot_SUBLOTID.INDATA()
            {
                POID = ParentVM.MainWnd.CurrentOrder.OrderID,
                MSUBLOTID = scannedComponent.MSUBLOTID
            });

            if (await bizRuleChecked.Execute() == false || bizRuleChecked.OUTDATAs.Count <= 0)
            {
                Message = string.Format("[{0}] 에 해당하는 정보를 조회할 수 없습니다.", scannedComponent.MSUBLOTBCD);
                return false;
            }

            //if (bizRuleChecked.OUTDATAs[0].CHECKDTTM.HasValue == false)
            //{
            //    Message = string.Format("[{0}] 은 검량되지 않은 원료입니다.", scannedComponent.MSUBLOTBCD);
            //    return false;
            //}

            _currentComponent = matchedComponent;

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

                        //if (arg is ShopFloorCustomWindow) _mainWnd = arg as ShopFloorCustomWindow;

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
                            if (await IsMatchedComponent(Barcode))
                            {
                                Barcode = string.Empty;
                                await (ChargingCommand as AsyncCommandBase).ExecuteAsync(null);
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

                            ///
                            //var bizRule = new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW();
                            //bizRule.INDATAs.Add(new BR_RHR_REG_MaterialSubLot_Dispense_Charging_NEW.INDATA()
                            //{
                            //    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                            //    LANGID = AuthRepositoryViewModel.Instance.LangID,
                            //    MSUBLOTBCD = _currentComponent.SCANNED_BARCODE,
                            //    POID = ParentVM.MainWnd.CurrentOrder.ProductionOrderID,
                            //    IS_NEED_CHKWEIGHT = "Y",
                            //    IS_FULL_CHARGE = "Y",
                            //    IS_CHECKONLY = "Y"
                            //});
                            /////

                            //if (await bizRule.Execute() == false) return;

                            //_currentComponent.IS_CAN_CHARGING_CHECKED = true;
                            _currentComponent.IS_CAN_CHARGING_CHECKED_NAME = "투입가능";

                            var notCompletedItem = ParentVM.filteredComponents.Where(o =>
                              o.IS_CAN_CHARGING_CHECKED_NAME == "투입대기").FirstOrDefault();

                            if (notCompletedItem != null)
                            {
                                CheckStep = enumCheckStep.ComponentChargeChecked;
                            }
                            else
                            {
                                CheckStep = enumCheckStep.ComponentChargeCheckCompleted;
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

        public 소분원료전량투입팝업ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
        }
    }
}
