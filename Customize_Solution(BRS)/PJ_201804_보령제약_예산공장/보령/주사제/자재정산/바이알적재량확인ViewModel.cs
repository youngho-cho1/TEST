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
using C1.Silverlight.Data;
using ShopFloorUI;
using System.Collections.Generic;

namespace 보령
{
    public class 바이알적재량확인ViewModel : ViewModelBase
    {
        #region [Property]
        private 바이알적재량확인 _mainWnd;
        private string _lblUOM = "적재량";
        public string lblUOM
        {
            get { return _lblUOM; }
            set
            {
                _lblUOM = value;
                NotifyPropertyChanged();
            }
        }
        private string _txtEQPTID;
        public string txtEQPTID
        {
            get { return _txtEQPTID; }
            set
            {
                _txtEQPTID = value;
                NotifyPropertyChanged();
            }
        }
        private string _txtStorageQTY;
        public string txtStorageQTY
        {
            get { return _txtStorageQTY; }
            set
            {
                _txtStorageQTY = value;
                NotifyPropertyChanged();
            }
        }
        private BufferedObservableCollection<Tray> _TrayComponents;
        public BufferedObservableCollection<Tray> TrayComponents
        {
            get { return _TrayComponents; }
            set
            {
                _TrayComponents = value;
                NotifyPropertyChanged();
            }
        }
        public 바이알적재량확인ViewModel()
        {
            _TrayComponents = new BufferedObservableCollection<Tray>();
        }
        #endregion
        #region [BizRule]
        
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
                            IsBusy = true;

                            if (arg == null || !(arg is 바이알적재량확인)) return;
                            else
                            {
                                _mainWnd = arg as 바이알적재량확인;

                                // 단위 조회
                                lblUOM = _lblUOM + "(" + "VL" + ")";

                            }

                            IsBusy = false;
                            ///

                            CommandResults["LoadedCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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
        //public ICommand SELStoragaeCommandAsync
        //{
        //    get
        //    {
        //        return new AsyncCommandBase(async arg =>
        //        {
        //            using (await AwaitableLocks["SELStoragaeCommandAsync"].EnterAsync())
        //            {
        //                try
        //                {
        //                    CommandResults["SELStoragaeCommandAsync"] = false;
        //                    CommandCanExecutes["SELStoragaeCommandAsync"] = false;

        //                    ///
        //                    IsBusy = true;
        //                    // 적재량 조회. 향후 설비 태그값으로 조회할 예정
        //                    throw new Exception("개발중");

        //                    //IsBusy = false;
        //                    ///

        //                    CommandResults["SELStoragaeCommandAsync"] = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    IsBusy = false;
        //                    CommandResults["SELStoragaeCommandAsync"] = false;
        //                    OnException(ex.Message, ex);
        //                }
        //                finally
        //                {
        //                    CommandCanExecutes["SELStoragaeCommandAsync"] = true;
        //                }
        //            }
        //        }, arg =>
        //        {
        //            return CommandCanExecutes.ContainsKey("SELStoragaeCommandAsync") ?
        //                CommandCanExecutes["SELStoragaeCommandAsync"] : (CommandCanExecutes["SELStoragaeCommandAsync"] = true);
        //        });
        //    }
        //}
        public ICommand INSTrayComponentCommand
        {
            get
            {
                return new CommandBase( arg =>
                {

                    try
                    {
                        CommandResults["ConfirmCommandAsync"] = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = false;

                        ///
                        IsBusy = true;

                        if (string.IsNullOrWhiteSpace(txtEQPTID))
                        {
                            if (_mainWnd != null) _mainWnd.txtEQPTID.Focus();
                            throw new Exception("트레이코드 값이 없습니다.");
                        }
                        else if (string.IsNullOrWhiteSpace(txtStorageQTY))
                        {
                            if (_mainWnd != null) _mainWnd.txtStorageQTY.Focus();
                            throw new Exception("적재수량 값이 없습니다.");
                        }
                        else
                        {
                            TrayComponents.Add(new Tray
                            {
                                SEQ = _TrayComponents.Count + 1,
                                EQPTID = txtEQPTID,
                                QTY = Convert.ToInt32(txtStorageQTY)
                            });

                            txtEQPTID = "";
                            _mainWnd.txtEQPTID.Focus();
                            //txtStorageQTY = "";
                        }

                        IsBusy = false;
                        ///

                        CommandResults["ConfirmCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        IsBusy = false;
                        CommandResults["ConfirmCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["ConfirmCommandAsync"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
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
                            IsBusy = true;

                            // 전자서명(기록값 변경)
                            var authHelper = new iPharmAuthCommandHelper();

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP"))
                            {
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

                            // 조회내용 기록
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_BRS_EquipmentAction_PROCEND");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "사용한설비확인및종료",
                                "사용한설비확인및종료",
                                false,
                                "EM_BRS_EquipmentAction_PROCEND",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // XML 변환
                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("적재순서"));
                            dt.Columns.Add(new DataColumn("트레이코드"));
                            dt.Columns.Add(new DataColumn("적재수량"));

                            foreach (var item in TrayComponents)
                            {
                                var row = dt.NewRow();

                                row["적재순서"] = item.SEQ.ToString() ?? "";
                                row["트레이코드"] = item.EQPTID ?? "";
                                row["적재수량"] = item.QTY.ToString("####0") ?? "";

                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            if(ds.Tables["DATA"].Rows.Count > 0)
                            {
                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;
                            }
                            else
                            {
                                _mainWnd.CurrentInstruction.Raw.ACTVAL = "정보없음";
                                _mainWnd.CurrentInstruction.Raw.NOTE = null;
                            }

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            IsBusy = false;
                            ///

                            CommandResults["ConfirmCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            IsBusy = false;
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
        #region [etc]
        public class Tray
        {
            private int _SEQ;
            public int SEQ
            {
                get { return _SEQ; }
                set { _SEQ = value; }
            }
            private string _EQPTID;
            public string EQPTID
            {
                get { return _EQPTID; }
                set { _EQPTID = value; }
            }
            private int _QTY;
            public int QTY
            {
                get { return _QTY; }
                set { _QTY = value; }
            }
        }
        #endregion
    }
}