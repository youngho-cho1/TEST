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
    public class 저울스캔팝업ViewModel : ViewModelBase
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

        public 저울스캔팝업ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi = new BR_BRS_SEL_EquipmentStatus_4Scale_Multi();
            _BR_BRS_CHK_Equipment_Is_Scale = new BR_BRS_CHK_Equipment_Is_Scale();
            _저울스캔팝업 = new 저울스캔팝업();
        }

        저울점검ViewModel _ParentVM;
        public 저울점검ViewModel ParentVM
        {
            get { return _ParentVM; }
            set { _ParentVM = value; }
        }

        저울스캔팝업 _저울스캔팝업;
        public 저울스캔팝업 저울스캔팝업
        {
            get { return _저울스캔팝업; }
            set { _저울스캔팝업 = value; }
        }

        //새로운 데이터 입력
        BR_BRS_SEL_EquipmentStatus_4Scale_Multi _BR_BRS_SEL_EquipmentStatus_4Scale_Multi;
        public BR_BRS_SEL_EquipmentStatus_4Scale_Multi BR_BRS_SEL_EquipmentStatus_4Scale_Multi
        {
            get { return _BR_BRS_SEL_EquipmentStatus_4Scale_Multi; }
            set
            {
                _BR_BRS_SEL_EquipmentStatus_4Scale_Multi = value;
                NotifyPropertyChanged();
            }
        }

        //사용할수 있는 Barcode 확인 해주는 비즈룰  --------- 이 비즈룰은 반드시 확인이 필요함
        BR_BRS_CHK_Equipment_Is_Scale _BR_BRS_CHK_Equipment_Is_Scale;
        public BR_BRS_CHK_Equipment_Is_Scale BR_BRS_CHK_Equipment_Is_Scale
        {
            get { return _BR_BRS_CHK_Equipment_Is_Scale; }
            set
            {
                _BR_BRS_CHK_Equipment_Is_Scale = value;
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
                            if (arg != null)
                                저울스캔팝업 = arg as 저울스캔팝업;
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
            //저울 확인
            BR_BRS_CHK_Equipment_Is_Scale.INDATAs.Add(new BR_BRS_CHK_Equipment_Is_Scale.INDATA()
            {
                EQPTID = barcode
            });

            if (await BR_BRS_CHK_Equipment_Is_Scale.Execute() == false) return false;

            //중복 체크
            foreach (var Equipment in _ParentVM.FilteredComponents)
            {
                if (Equipment.EQPTID.ToUpper() == barcode.ToUpper())
                {
                    C1.Silverlight.C1MessageBox.Show("입력하신 저울ID가 존재합니다.", "경고", C1.Silverlight.C1MessageBoxButton.OK, (r) =>
                    {
                        저울스캔팝업.tbBarcode.Focus();
                    });
                    return false;
                }
            }

            // 새로운 데이터 조회 및 추가
            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Clear();
            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Clear();

            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULTs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA_IRTRESULT()
            {
                RECIPEISTGUID = null,
                ACTIVITYID = null,
                IRTGUID = null,
                IRTRSTGUID = null,
                ACTVAL = null
            });
            _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATAs.Add(new BR_BRS_SEL_EquipmentStatus_4Scale_Multi.INDATA()
            {
                LANGID = LogInInfo.LangID,
                EQPTID = barcode,
            });

            if (await _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.Execute() == false)
                return false;

            var notCompletedItem = _BR_BRS_SEL_EquipmentStatus_4Scale_Multi.OUTDATAs.Where(o => o.EQPTID == barcode).FirstOrDefault();

            if (notCompletedItem != null)
            {
                _ParentVM.FilteredComponents.Add(notCompletedItem);
                CheckStep = enumCheckStep.ComponentChargeChecked;
            }
            return true;
        }
    }
}
