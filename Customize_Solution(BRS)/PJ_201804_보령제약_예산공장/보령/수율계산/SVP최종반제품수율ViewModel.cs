using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using C1.Silverlight.Imaging;

namespace 보령
{
    public class SVP최종반제품수율ViewModel : ViewModelBase
    {
        #region Properties
        public SVP최종반제품수율ViewModel()
        {
            _BR_BRS_REG_ProductionOrderDetailYield = new BR_BRS_REG_ProductionOrderDetailYield();
            _BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi = new BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi();
        }

        SVP최종반제품수율 _mainWnd;
        private int PRECISION;

        private string _result_IN;
        public string Result_IN
        {
            get { return _result_IN; }
            set
            {
                _result_IN = value;
                CalculateYield(_result_IN, _result_OUT);
                NotifyPropertyChanged();
            }
        }

        private string _result_OUT;
        public string Result_OUT
        {
            get { return _result_OUT; }
            set
            {
                _result_OUT = value;
                CalculateYield(_result_IN, _result_OUT);
                NotifyPropertyChanged();
            }
        }

        private decimal? _result_SUM;
        public decimal? Result_SUM
        {
            get { return _result_SUM; }
            set
            {
                _result_SUM = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region BizRule
        private BR_BRS_REG_ProductionOrderDetailYield _BR_BRS_REG_ProductionOrderDetailYield;
        public BR_BRS_REG_ProductionOrderDetailYield BR_BRS_REG_ProductionOrderDetailYield
        {
            get { return _BR_BRS_REG_ProductionOrderDetailYield; }
            set { _BR_BRS_REG_ProductionOrderDetailYield = value; }
        }
        private BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi _BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi;
        #endregion
        #region Command
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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null)
                            {
                                IsBusy = true;

                                _mainWnd = arg as SVP최종반제품수율;

                                PRECISION = _mainWnd.CurrentInstruction.Raw.PRECISION.HasValue ? _mainWnd.CurrentInstruction.Raw.PRECISION.Value : 1;

                                // 현재 공정 생산량
                                var inputvalue = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                if (inputvalue.Count > 0)
                                {
                                    decimal chk;
                                    if (!string.IsNullOrWhiteSpace(inputvalue[0].Raw.ACTVAL) && decimal.TryParse(inputvalue[0].Raw.ACTVAL, out chk))
                                    {
                                        Result_OUT = inputvalue[0].Raw.ACTVAL;
                                    }
                                    else
                                        OnMessage("최종반제품량이 입력되지 않았거나 숫자로 변환이 실패했습니다.");
                                }
                                else
                                    OnMessage("최종반제품량이 입력되지 않았습니다.");
                            }

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadedCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            IsBusy = false;
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
                            IsBusy = true;

                            CommandResults["ConfirmCommandAsync"] = false;
                            CommandCanExecutes["ConfirmCommandAsync"] = false;

                            ///

                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
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

                            var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Yield");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "SVP반제품충전 수율 기록",
                                "SVP반제품충전 수율 기록",
                                false,
                                "OM_ProductionOrder_Yield",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            _BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi.INDATAs.Clear();
                            _BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi.INDATAs.Add(new BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                VERSION = 1m,
                                PODETAID = "OUTPUT_QTY",
                                PODETAVAL1 = Result_OUT,
                                INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Yield")
                            });

                            await _BR_BRS_MRG_ProductionOrderDetailAttributeValue_Multi.Execute();

                            // 수율기록
                            _BR_BRS_REG_ProductionOrderDetailYield.INDATAs.Clear();
                            _BR_BRS_REG_ProductionOrderDetailYield.INDATAs.Add(new BR_BRS_REG_ProductionOrderDetailYield.INDATA
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                COMMENT = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Yield"),
                                INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Yield"),
                                YIELD2 = Result_SUM
                            });

                            if (await BR_BRS_REG_ProductionOrderDetailYield.Execute() == true)
                            {
                                Brush background = _mainWnd.PrintArea.Background;
                                _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                                _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                                _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);


                                _mainWnd.CurrentInstruction.Raw.ACTVAL = Result_SUM.ToString();
                                _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();

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

                            IsBusy = false;
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
                            IsBusy = false;
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
        #region User Define
        private void CalculateYield(string input, string output)
        {
            try
            {
                decimal IN, OUT;

                if (decimal.TryParse(input, out IN) && decimal.TryParse(output, out OUT))
                    Result_SUM = MathExt.Round((OUT / IN) * 100, 1, MidpointRounding.AwayFromZero);

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }
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


