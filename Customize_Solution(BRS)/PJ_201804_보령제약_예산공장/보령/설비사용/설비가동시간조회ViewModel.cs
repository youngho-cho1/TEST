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
using System.Windows.Threading;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Collections.ObjectModel;

namespace 보령
{
    public class 설비가동시간조회ViewModel : ViewModelBase
    {
        #region [Property]
        private 설비가동시간조회 _mainWnd;
        private InstructionModel curInst;

        private ObservableCollection<LayerCharging> _ListContainer;
        public ObservableCollection<LayerCharging> ListContainer
        {
            get { return _ListContainer; }
            set
            {
                _ListContainer = value;
                OnPropertyChanged("ListContainer");
            }
        }

        private String _STDTTM;
        public String STDTTM
        {
            get { return _STDTTM; }
            set
            {
                _STDTTM = value;
                OnPropertyChanged("STDTTM");
            }
        }
        private String _EDDTTM;
        public String EDDTTM
        {
            get { return _EDDTTM; }
            set
            {
                _EDDTTM = value;
                OnPropertyChanged("EDDTTM");
            }
        }
        private string _WORKTIME;
        public string WORKTIME
        {
            get { return _WORKTIME; }
            set
            {
                _WORKTIME = value;
                OnPropertyChanged("WORKTIME");
            }
        }

        #endregion

        #region [BizRule]
        private BR_BRS_SEL_Equipment_Exec_Time _BR_BRS_SEL_Equipment_Exec_Time;
        public BR_BRS_SEL_Equipment_Exec_Time BR_BRS_SEL_Equipment_Exec_Time
        {
            get { return _BR_BRS_SEL_Equipment_Exec_Time; }
            set
            {
                _BR_BRS_SEL_Equipment_Exec_Time = value;
                OnPropertyChanged("BR_BRS_SEL_Equipment_Exec_Time.Access");
            }
        }
        #endregion
        public 설비가동시간조회ViewModel()
        {
            _BR_BRS_SEL_Equipment_Exec_Time = new 보령.BR_BRS_SEL_Equipment_Exec_Time();
            ListContainer = new ObservableCollection<보령.설비가동시간조회ViewModel.LayerCharging>();
        }

        #region [Command]
        public ICommand LoadCommandAsync
        {
            get
            {
                return new CommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["LoadCommandAsync"] = false;
                        CommandCanExecutes["LoadCommandAsync"] = false;

                        IsBusy = true;
                        ///
                        if (arg != null && arg is 설비가동시간조회)
                        {
                            _mainWnd = arg as 설비가동시간조회;

                            var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);
                            curInst = _mainWnd.CurrentInstruction;
                            
                            BR_BRS_SEL_Equipment_Exec_Time.INDATAs.Clear();
                            BR_BRS_SEL_Equipment_Exec_Time.OUTDATAs.Clear();

                            foreach (InstructionModel Inst in inputValues)
                            {
                                _BR_BRS_SEL_Equipment_Exec_Time.INDATAs.Add(new BR_BRS_SEL_Equipment_Exec_Time.INDATA
                                {
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    EQPTID = Inst.Raw.EQPTID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                });
                            }

                            if (!await BR_BRS_SEL_Equipment_Exec_Time.Execute()) return;

                            if (BR_BRS_SEL_Equipment_Exec_Time.OUTDATAs.Count > 0)
                            {
                                foreach (var item in BR_BRS_SEL_Equipment_Exec_Time.OUTDATAs)
                                {
                                    ListContainer.Add(new LayerCharging
                                    {
                                        EQPTID = item.EQPTID,
                                        STDTTM = item.STDTTM.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                        EDDTTM = item.EDDTTM.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                        WORKTIME = item.WORKTIME.ToString()
                                    });
                                }
                            }

                            IsBusy = false;
                        }
                        ///
                        CommandResults["LoadCommandAsync"] = true;
                    }
                    catch (Exception ex)
                    {
                        IsBusy = false;
                        CommandResults["LoadCommandAsync"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadCommandAsync"] = true;
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommandAsync") ?
                        CommandCanExecutes["LoadCommandAsync"] : (CommandCanExecutes["LoadCommandAsync"] = true);
                }
                    );

            }
        }
        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    try
                    {
                        CommandResults["ConfirmCommandAsync"] = false;
                        CommandCanExecutes["ConfirmCommandAsync"] = false;

                        ///
                        IsBusy = true;
                        
                        var authHelper = new iPharmAuthCommandHelper();

                        // 전자서명 요청
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

                        authHelper.InitializeAsync(Common.enumCertificationType.Role, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                        if (await authHelper.ClickAsync(
                            Common.enumCertificationType.Role,
                            Common.enumAccessType.Create,
                            "설비가동시간조회",
                            "설비가동시간조회",
                            false,
                            "OM_ProductionOrder_SUI",
                            "",
                            null, null) == false)
                        {
                            throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                        }


                        var ds = new DataSet();
                        var dt = new DataTable("DATA");

                        dt.Columns.Add(new DataColumn("장비번호"));
                        dt.Columns.Add(new DataColumn("시작시각"));
                        dt.Columns.Add(new DataColumn("종료시각"));
                        dt.Columns.Add(new DataColumn("가동시각"));
                        ds.Tables.Add(dt);

                        foreach (var item in ListContainer)
                        {
                            var row = dt.NewRow();
                            row["장비번호"] = item.EQPTID ?? "";
                            row["시작시각"] = item.STDTTM != null ? item.STDTTM : "";
                            row["종료시각"] = item.EDDTTM != null ? item.EDDTTM : "";
                            row["가동시각"] = item.WORKTIME ?? "";

                            dt.Rows.Add(row);
                        }

                        var xml = BizActorRuleBase.CreateXMLStream(ds);
                        var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                        _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                        _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                        var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);
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
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommandAsync") ?
                        CommandCanExecutes["ConfirmCommandAsync"] : (CommandCanExecutes["ConfirmCommandAsync"] = true);
                }
                    );

            }
        }

        #endregion

        #region [User Define]
        public class LayerCharging : ViewModelBase
        {
            private String _STDTTM;
            public String STDTTM
            {
                get { return _STDTTM; }
                set
                {
                    _STDTTM = value;
                    OnPropertyChanged("STDTTM");
                }
            }
            private String _EDDTTM;
            public String EDDTTM
            {
                get { return _EDDTTM; }
                set
                {
                    _EDDTTM = value;
                    OnPropertyChanged("EDDTTM");
                }
            }
            private string _WORKTIME;
            public string WORKTIME
            {
                get { return _WORKTIME; }
                set
                {
                    _WORKTIME = value;
                    OnPropertyChanged("WORKTIME");
                }
            }

            private string _EQPTID;
            public string EQPTID
            {
                get { return _EQPTID; }
                set
                {
                    _EQPTID = value;
                    OnPropertyChanged("EQPTID");
                }
            }
        }

        #endregion
    }
}
