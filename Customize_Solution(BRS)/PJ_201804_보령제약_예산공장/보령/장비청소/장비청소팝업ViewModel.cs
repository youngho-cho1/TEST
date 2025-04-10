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
    public class 장비청소팝업ViewModel : ViewModelBase
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

        public 장비청소팝업ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi = new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi();
            _BR_PHR_SEL_Equipment_GetInfo = new BR_PHR_SEL_Equipment_GetInfo();
            _장비청소팝업 = new 장비청소팝업();
        }

        장비청소상태ViewModel _ParentVM;
        public 장비청소상태ViewModel ParentVM
        {
            get { return _ParentVM; }
            set { _ParentVM = value; }
        }

        장비청소팝업 _장비청소팝업;
        public 장비청소팝업 장비청소팝업
        {
            get { return _장비청소팝업; }
            set { _장비청소팝업 = value; }
        }

        //새로운 데이터 입력
        BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi;
        public BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi
        {
            get { return _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi; }
            set
            {
                _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi = value;
                NotifyPropertyChanged();
            }
        }

        //사용할수 있는 Barcode 확인 해주는 비즈룰
        BR_PHR_SEL_Equipment_GetInfo _BR_PHR_SEL_Equipment_GetInfo;
        public BR_PHR_SEL_Equipment_GetInfo BR_PHR_SEL_Equipment_GetInfo
        {
            get { return _BR_PHR_SEL_Equipment_GetInfo; }
            set
            {
                _BR_PHR_SEL_Equipment_GetInfo = value;
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
                                장비청소팝업 = arg as 장비청소팝업;

                            if(!(string.IsNullOrWhiteSpace(장비청소팝업.tbBarcode.Text)))
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
                        장비청소팝업.tbBarcode.Focus();
                    });
                    return false;
                }
            }
            /*
            //사용 가능한 값 확인
            _BR_PHR_SEL_Equipment_GetInfo.INDATAs.Add(new BR_PHR_SEL_Equipment_GetInfo.INDATA()
            {
                LANGID = "ko-KR",
                EQPTID = barcode.ToUpper()
            });

            await _BR_PHR_SEL_Equipment_GetInfo.Execute();
            if (_BR_PHR_SEL_Equipment_GetInfo.OUTDATAs.Count == 0)
            {

                C1.Silverlight.C1MessageBox.Show("입력하신 장비ID는 사용할 수 없습니다.", "경고", C1.Silverlight.C1MessageBoxButton.OK, (r) =>
                {
                    장비청소팝업.tbBarcode.Focus();
                });
                return false;
            }
            */
            // 새로운 데이터 조회 및 추가
            var instruction = _ParentVM.mainWnd.CurrentInstruction;
            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Clear();
            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Clear();
            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULTs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA_IRTRESULT()
            {
                RECIPEISTGUID = null,
                ACTIVITYID = null,
                IRTGUID = null,
                IRTRSTGUID = null,
                ACTVAL = null
            });

            _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATAs.Add(new BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.INDATA()
            {
                LANGID = LogInInfo.LangID,
                EQPTID = barcode,
                POID = _ParentVM.mainWnd.CurrentOrder.ProductionOrderID,
                BATCHNO = _ParentVM.mainWnd.CurrentOrder.BatchNo               
            });

            if (await _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.Execute() == false)
                return false;

            var notCompletedItem = _BR_PHR_SEL_EquipmentStatus_CheckCleaned_Multi.OUTDATAs.Where(o => o.EQPTID == barcode).FirstOrDefault();

            if (notCompletedItem != null)
            {
                _ParentVM.FilteredComponents.Add(notCompletedItem);
                CheckStep = enumCheckStep.ComponentChargeChecked;
            }
            return true;
        }
    }
}
