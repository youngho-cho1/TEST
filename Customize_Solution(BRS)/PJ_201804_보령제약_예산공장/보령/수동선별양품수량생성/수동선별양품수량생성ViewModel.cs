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
using LGCNS.iPharmMES.Recipe.Common;

namespace 보령
{
    public class 수동선별양품수량생성ViewModel : ViewModelBase
    {
        #region Properties

        수동선별양품수량생성 _mainWndXml;

        private int PRECISION;

        ShopFloorCustomWindow _mainWnd;
        public ShopFloorCustomWindow MainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
        }

        BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT;
        public BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT
        {
            get { return _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT; }
            set
            {
                _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT = value;
                OnPropertyChanged("BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT");
            }
        }

        BR_BRS_REG_Manual_Inspection_GoodQty _BR_BRS_REG_Manual_Inspection_GoodQty;
        public BR_BRS_REG_Manual_Inspection_GoodQty BR_BRS_REG_Manual_Inspection_GoodQty
        {
            get { return _BR_BRS_REG_Manual_Inspection_GoodQty; }
            set
            {
                _BR_BRS_REG_Manual_Inspection_GoodQty = value;
                OnPropertyChanged("BR_BRS_REG_Manual_Inspection_GoodQty");
            }
        }

        private string _result_AVG;
        public string Result_AVG
        {
            get { return _result_AVG; }
            set
            {
                _result_AVG = value;
                NotifyPropertyChanged();
            }
        }

        private string _result_OUTPUT;
        public string Result_OUTPUT
        {
            get { return _result_OUTPUT; }
            set
            {
                _result_OUTPUT = value;
                NotifyPropertyChanged();
            }
        }

        private string _result_CALC;
        public string Result_CALC
        {
            get { return _result_CALC; }
            set
            {
                _result_CALC = value;
                NotifyPropertyChanged();
            }
        }

        bool _result_AVG_ReadOnly;
        public bool Result_AVG_ReadOnly
        {
            get { return _result_AVG_ReadOnly; }
            set
            {
                _result_AVG_ReadOnly = value;
                NotifyPropertyChanged();
            }
        }

        //bool _result_OUTPUT_ReadOnly;
        //public bool Result_OUTPUT_ReadOnly
        //{
        //    get { return _result_OUTPUT_ReadOnly; }
        //    set
        //    {
        //        _result_OUTPUT_ReadOnly = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        bool _result_CALC_ReadOnly;
        public bool Result_CALC_ReadOnly
        {
            get { return _result_CALC_ReadOnly; }
            set
            {
                _result_CALC_ReadOnly = value;
                NotifyPropertyChanged();
            }
        }

        public bool _ModifyFlag = false;
        public string _ModifyComment = string.Empty;

        #endregion
        public ICommand LoadedCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadedCommand"] = false;
                            CommandCanExecutes["LoadedCommand"] = false;

                            if (arg != null)
                            {
                                MainWnd = arg as ShopFloorCustomWindow;

                                PRECISION = _mainWnd.CurrentInstruction.Raw.PRECISION.HasValue ? _mainWnd.CurrentInstruction.Raw.PRECISION.Value : 1;

                                _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.INDATAs.Clear();
                                _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.OUTDATAs.Clear();

                                _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.INDATAs.Add(new BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                await _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.Execute();

                                if (_BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.OUTDATAs.Count > 0)
                                {
                                    Result_AVG = _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.OUTDATAs[0].AVG_WEIGHT;
                                    Result_OUTPUT = _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.OUTDATAs[0].OUTPUT_WEIGHT;
                                    Result_CALC = _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT.OUTDATAs[0].RESULT_WEIGHT;
                                }
                            }

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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        // 평균질량을 입력하지 않을 수 있어서 직접 입력할 수 있게 처리
        public ICommand ModifyCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ModifyCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ModifyCommand"] = false;
                            CommandCanExecutes["ModifyCommand"] = false;

                            if (arg != null)
                            {
                                _mainWndXml = arg as 수동선별양품수량생성;

                                //수기입력 시 기존 작업 데이터를 불러오는 것이 아니므로 작업자 개인 서명이 필요(Comment 필수)
                                var authHelper = new iPharmAuthCommandHelper();
                                authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_Deviation");

                                enumRoleType inspectorRole = enumRoleType.ROLE001;

                                if (await authHelper.ClickAsync(
                                        Common.enumCertificationType.Role,
                                        Common.enumAccessType.Create,
                                        "기록값 변경 시 코멘트 작성 필요합니다. ",
                                        "수동선별기 양품수량 기록값 변경",
                                        true,
                                        "OM_ProductionOrder_Deviation",
                                        "",
                                        this._mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                        this._mainWnd.CurrentInstruction.Raw.DVTPASSYN == "Y" ? enumRoleType.ROLE001.ToString() : inspectorRole.ToString()) == false)
                                {
                                    return;
                                }

                                _mainWnd.CurrentInstruction.Raw.DVTFCYN = "Y";
                                _mainWnd.CurrentInstruction.Raw.DVTCONFIRMUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Deviation");

                                _ModifyComment = AuthRepositoryViewModel.GetCommentByFunctionCode("OM_ProductionOrder_Deviation");
                                _ModifyFlag = true;
                            }

                            //Result_AVG, Result_OUTPUT, Result_CALC ReadOnly 해제
                            //Result_OUTPUT_ReadOnly = false;
                            Result_AVG_ReadOnly = false;
                            Result_CALC_ReadOnly = false;

                            //Result_AVG, Result_OUTPUT, Result_CALC ReadOnly 값 초기화
                            //Result_OUTPUT = "";
                            //Result_AVG = "";
                            //Result_CALC = "";

                            CommandResults["ModifyCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ModifyCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ModifyCommand"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ModifyCommand") ?
                        CommandCanExecutes["ModifyCommand"] : (CommandCanExecutes["ModifyCommand"] = true);
                });
            }
        }

        // 기록 버튼 이벤트
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

                            if (arg != null)
                            {
                                _mainWndXml = arg as 수동선별양품수량생성;

                                var authHelper = new iPharmAuthCommandHelper();

                                //수율 값 저장 전자서명
                                authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Yield");

                                if (await authHelper.ClickAsync(
                                    Common.enumCertificationType.Function,
                                    Common.enumAccessType.Create,
                                    "수동선별기양품수량 기록",
                                    "수동선별기양품수량 기록",
                                    false,
                                    "OM_ProductionOrder_Yield",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                //값의 변경이 있을 때 호출
                                OnPropertyChanged("Result_AVG");
                                OnPropertyChanged("Result_CALC");

                                _BR_BRS_REG_Manual_Inspection_GoodQty.INDATAs.Clear();

                                _BR_BRS_REG_Manual_Inspection_GoodQty.INDATAs.Add(new BR_BRS_REG_Manual_Inspection_GoodQty.INDATA()
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    GOODQTY = Math.Floor(Convert.ToDouble(Result_CALC)).ToString(),
                                    USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                                });

                                if (!await _BR_BRS_REG_Manual_Inspection_GoodQty.Execute()) throw new Exception(string.Format("GoodQty 생성 중 오류가 발생했습니다."));

                                Brush background = _mainWndXml.PrintArea.Background;
                                _mainWndXml.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                                _mainWndXml.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                                _mainWndXml.PrintArea.Background = new SolidColorBrush(Colors.White);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = Math.Floor(Convert.ToDouble(Result_CALC)).ToString(); //"Image Attached" -> 소수점버림 처리
                                _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, false, false, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                if (_ModifyFlag)
                                {

                                    var bizrule = new BR_PHR_REG_InstructionComment();

                                    bizrule.IN_Comments.Add(
                                        new BR_PHR_REG_InstructionComment.IN_Comment
                                        {
                                            POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                            OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                            COMMENTTYPE = "CM008",
                                            COMMENT = _ModifyComment
                                        }
                                        );
                                    bizrule.IN_IntructionResults.Add(
                                        new BR_PHR_REG_InstructionComment.IN_IntructionResult
                                        {
                                            RECIPEISTGUID = _mainWnd.CurrentInstruction.Raw.RECIPEISTGUID,
                                            ACTIVITYID = _mainWnd.CurrentInstruction.Raw.ACTIVITYID,
                                            IRTGUID = _mainWnd.CurrentInstruction.Raw.IRTGUID,
                                            IRTRSTGUID = _mainWnd.CurrentInstruction.Raw.IRTRSTGUID,
                                            IRTSEQ = (int)_mainWnd.CurrentInstruction.Raw.IRTSEQ
                                        }
                                        );

                                    await bizrule.Execute();
                                }

                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            }

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

        public byte[] imageToByteArray()
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWndXml.PrintArea, null));
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

        public 수동선별양품수량생성ViewModel()
        {
            _BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT = new BRS_SEL_ProductionOrder_OPSG_Output_WEIGHT();
            _BR_BRS_REG_Manual_Inspection_GoodQty = new BR_BRS_REG_Manual_Inspection_GoodQty();
            //  Is_EnableOKBtn = false;

            // 기존 readOnly 설정값 유지
            //Result_OUTPUT_ReadOnly = true;
            Result_AVG_ReadOnly = true;
            Result_CALC_ReadOnly = true;
        }
    }
}