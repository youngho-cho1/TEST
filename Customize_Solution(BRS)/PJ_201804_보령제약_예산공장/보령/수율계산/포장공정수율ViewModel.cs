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
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using C1.Silverlight.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace 보령
{
    public class 포장공정수율ViewModel : ViewModelBase
    {
        #region [Property]
        private 포장공정수율 _mainWnd;
        private int PRECISION;

        private string _InputVal1;
        public string InputVal1
        {
            get { return _InputVal1; }
            set
            {
                _InputVal1 = value;
                NotifyPropertyChanged();
            }
        }

        private string _InputVal2;
        public string InputVal2
        {
            get { return _InputVal2; }
            set
            {
                _InputVal2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _OutputVal;
        public string OutputVal
        {
            get { return _OutputVal; }
            set
            {
                _OutputVal = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region [BizRule]

        // 테스트용 비즈룰 수정 필요
        private BR_BRS_SEL_Yield_Calculation_Weight_Packing _BR_BRS_SEL_Yield_Calculation_Weight_Packing;
        public BR_BRS_SEL_Yield_Calculation_Weight_Packing BR_BRS_SEL_Yield_Calculation_Weight_Packing
        {
            get { return _BR_BRS_SEL_Yield_Calculation_Weight_Packing; }
            set
            {
                _BR_BRS_SEL_Yield_Calculation_Weight_Packing = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region [Command]
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if (arg != null && arg is 포장공정수율)
                            {
                                _mainWnd = arg as 포장공정수율;

                                decimal temp; // using at tryparse

                                PRECISION = _mainWnd.CurrentInstruction.Raw.PRECISION.HasValue ? _mainWnd.CurrentInstruction.Raw.PRECISION.Value : 1;

                                _BR_BRS_SEL_Yield_Calculation_Weight_Packing.INDATAs.Clear();
                                _BR_BRS_SEL_Yield_Calculation_Weight_Packing.OUTDATAs.Clear();

                                _BR_BRS_SEL_Yield_Calculation_Weight_Packing.INDATAs.Add(new BR_BRS_SEL_Yield_Calculation_Weight_Packing.INDATA
                                {
                                    AVG_OPTIONITEM = "SYS_IPCTEST_ITEMID",
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    MTRLID = _mainWnd.CurrentOrder.MaterialID,
                                    INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID
                                });

                                await _BR_BRS_SEL_Yield_Calculation_Weight_Packing.Execute();

                                if (_BR_BRS_SEL_Yield_Calculation_Weight_Packing.OUTDATAs.Count > 0)
                                {
                                    InputVal1 = _BR_BRS_SEL_Yield_Calculation_Weight_Packing.OUTDATAs[0].IN_VALUE1;
                                    InputVal2 = _BR_BRS_SEL_Yield_Calculation_Weight_Packing.OUTDATAs[0].IN_VALUE2;
                                    OutputVal = decimal.TryParse(_BR_BRS_SEL_Yield_Calculation_Weight_Packing.OUTDATAs[0].OUT_VALUE, out temp) ? MathExt.Round(temp, PRECISION, MidpointRounding.AwayFromZero).ToString() : "0";
                                }
                            }

                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadedCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                                            CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }

        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///
                            if (_mainWnd != null)
                            {
                                var authHelper = new iPharmAuthCommandHelper();

                                if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정시 전자서명
                                {
                                    // 전자서명 요청
                                    authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                                    if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Function,
                                        Common.enumAccessType.Create,
                                        string.Format("기록값을 변경합니다."),
                                        string.Format("기록값 변경"),
                                        true,
                                        "OM_ProductionOrder_SUI",
                                        "", _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID, null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }
                                }

                                // 포장공정최종생산수율 전자서명
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Yield");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "포장공정수율 기록",
                                    "포장공정수율 기록",
                                    false,
                                    "OM_ProductionOrder_Yield",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // EBR 기록
                                _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);
                                _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                                _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = OutputVal;
                                _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();


                                // ref Phase에 값 전달
                                var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, false, false, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                foreach (var item in outputValues)
                                {
                                    item.Raw.ACTVAL = _mainWnd.CurrentInstruction.Raw.ACTVAL;

                                    result = await _mainWnd.Phase.RegistInstructionValue(item);
                                    if (result != enumInstructionRegistErrorType.Ok)
                                    {
                                        throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", item.Raw.IRTGUID, result));
                                    }
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommandAsync"] = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                                            CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                });
            }
        }

        #endregion

        #region [Constructor]
        public 포장공정수율ViewModel()
        {
            _BR_BRS_SEL_Yield_Calculation_Weight_Packing = new BR_BRS_SEL_Yield_Calculation_Weight_Packing();
        }
        #endregion

        #region [User Define Function]
        public byte[] imageToByteArray()
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintArea, null));
                System.IO.Stream stream = bitmap.GetStream(ImageFormat.Png, true);

                int len = (int)stream.Seek(0, SeekOrigin.End);

                byte[] datas = new byte[len];

                stream.Seek(0, SeekOrigin.Begin);

                stream.Read(datas, 0, datas.Length);

                return datas;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
