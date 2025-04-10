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
    public class 포장자재저울스캔ViewModel : ViewModelBase
    {
        #region [Property]

        포장자재저울스캔 _mainWnd;
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
                        Message = "저울 바코드를 스캔하세요";
                        break;
                    case enumCheckStep.ComponentChargeChecked:
                        IsBarcodeReaderVisible = Visibility.Visible;
                        Message = "현재 저울 값은 : " + Scale + " 입니다. 닫기 버튼 클릭하세요";
                        //_mainWnd.DialogResult = true;
                        break;
                    default:
                        break;
                }
            }
        }

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

        private string _Scale;
        public string Scale
        {
            get { return _Scale; }
            set
            {
                _Scale = value;
                OnPropertyChanged("Scale");
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

        public decimal ScaleWeight;

        #endregion

        #region [Constructor]

        public 포장자재저울스캔ViewModel()
        {
            _checkStep = enumCheckStep.WaitBarcode;
            _BR_PHR_SEL_CurrentWeight = new BR_PHR_SEL_CurrentWeight();
            _BR_BRS_CHK_Equipment_Is_Scale = new BR_BRS_CHK_Equipment_Is_Scale();
        }

        #endregion

        #region [Bizrule]

        //새로운 데이터 입력
        BR_PHR_SEL_CurrentWeight _BR_PHR_SEL_CurrentWeight;
        public BR_PHR_SEL_CurrentWeight BR_PHR_SEL_CurrentWeight
        {
            get { return _BR_PHR_SEL_CurrentWeight; }
            set
            {
                _BR_PHR_SEL_CurrentWeight = value;
                OnPropertyChanged("BR_PHR_SEL_CurrentWeight");
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

        #endregion

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

                        ///
                        if (arg != null)
                            _mainWnd = arg as 포장자재저울스캔;

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
                            {
                                var Text = arg as TextBox;

                                Barcode = Text.Text;
                            }
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

        #endregion

        #region [User Event Method]

        async Task<bool> IsMatchedComponent(string barcode)
        {
            //저울 확인
            BR_BRS_CHK_Equipment_Is_Scale.INDATAs.Add(new BR_BRS_CHK_Equipment_Is_Scale.INDATA()
            {
                EQPTID = barcode
            });

            if (await BR_BRS_CHK_Equipment_Is_Scale.Execute() == false) return false;

            if (ScaleId != null && ScaleId.ToUpper() == barcode.ToUpper())
            {
                C1.Silverlight.C1MessageBox.Show("입력하신 저울ID가 존재합니다.", "경고", C1.Silverlight.C1MessageBoxButton.OK, (r) =>
                {
                    _mainWnd.tbBarcode.Focus();
                });
                return false;
            }

            // 새로운 데이터 조회 및 추가
            _BR_PHR_SEL_CurrentWeight.INDATAs.Clear();

            _BR_PHR_SEL_CurrentWeight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA
            {
                ScaleID = barcode
            });

            if (await _BR_PHR_SEL_CurrentWeight.Execute())
            {
                if (_BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                {
                    ScaleId = barcode;
                    Scale = string.Format("{0:#.##0}",_BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight) + " " + _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;
                    ScaleWeight = (decimal)_BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight;
                    CheckStep = enumCheckStep.ComponentChargeChecked;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion

    }
}
