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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using LGCNS.iPharmMES.Recipe.Common;
using System.Threading.Tasks;

namespace 보령
{
    public class SVP수동검사불량유형조회ViewModel : ViewModelBase
    {
        #region Properties
        public SVP수동검사불량유형조회ViewModel()
        {
            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM = new BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM();
            _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT = new BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT();
        }

        SVP수동검사불량유형조회 _mainWnd;
        
        private BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM;
        public BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM
        {
            get { return _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM; }
            set
            {
                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM");
            }
        }
        private BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT;
        public BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT
        {
            get { return _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT; }
            set
            {
                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT = value;
                OnPropertyChanged("BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT");
            }
        }
        #endregion

        #region BizRule      
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
                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            if (arg != null && arg is SVP수동검사불량유형조회)
                            {
                                _mainWnd = arg as SVP수동검사불량유형조회;

                                IsBusy = true;

                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM.INDATAs.Clear();
                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM.OUTDATAs.Clear();
                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM.INDATAs.Add(new BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (!await _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM.Execute()) return;

                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT.INDATAs.Clear();
                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT.OUTDATAs.Clear();
                                _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT.INDATAs.Add(new BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                                });

                                if (!await _BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT.Execute()) return;
                                    
                            }
                            IsBusy = false;

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

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        public ICommand ComfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ComfirmCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ComfirmCommandAsync"] = false;
                            CommandCanExecutes["ComfirmCommandAsync"] = false;

                            //이미지 저장시 서명화면으로 인해 이미지가 잘 안보임. 그에 따른 이미지 데이터만 먼저 생성해 놓도록 함.
                            Brush background = _mainWnd.PrintArea.Background;
                            _mainWnd.PrintArea.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD4, 0xD4));
                            _mainWnd.PrintArea.BorderThickness = new System.Windows.Thickness(1);
                            _mainWnd.PrintArea.Background = new SolidColorBrush(Colors.White);
                            
                            _mainWnd.CurrentInstruction.Raw.NOTE = imageToByteArray();
                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;

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
                            
                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;

                            CommandResults["ComfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ComfirmCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ComfirmCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ComfirmCommandAsync") ?
                        CommandCanExecutes["ComfirmCommandAsync"] : (CommandCanExecutes["ComfirmCommandAsync"] = true);
                });
            }
        }

        #endregion

        #region User Define
        public byte[] imageToByteArray()
        {
            try
            {
                C1Bitmap bitmap = new C1Bitmap(new WriteableBitmap(_mainWnd.PrintMain, null));
                System.IO.Stream stream = bitmap.GetStream(C1.Silverlight.Imaging.ImageFormat.Png, true);

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
