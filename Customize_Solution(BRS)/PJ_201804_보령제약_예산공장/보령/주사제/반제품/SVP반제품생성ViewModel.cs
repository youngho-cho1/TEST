using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령
{
    public class SVP반제품생성ViewModel : ViewModelBase
    {
        #region [Property]
        public SVP반제품생성ViewModel()
        {
            _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType = new BR_PHR_SEL_ProductionOrderOutput_Define_AnyType();
            _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT = new BR_BRS_REG_ProductionOrderOutput_Vessel_STRT();
            _OutputList = new ObservableCollection<OutputInformation>();
        }

        private SVP반제품생성 _mainWnd;

        private double _strgtime = 0;
        private string _LotType = "";
        private string _EqptId;
        public string EqptId
        {
            get { return _EqptId; }
            set
            {
                _EqptId = value;
                OnPropertyChanged("EqptId");
            }
        }
        private string _OutputId;
        public string OutputId
        {
            get { return _OutputId; }
            set
            {
                _OutputId = value;
                OnPropertyChanged("OutputId");
            }
        }
        private decimal _MSUBLOTQTY;
        public decimal MSUBLOTQTY
        {
            get { return _MSUBLOTQTY; }
            set
            {
                _MSUBLOTQTY = value;
                OnPropertyChanged("MSUBLOTQTY");
            }                
        }
        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set
            {
                _Unit = value;
                OnPropertyChanged("Unit");
            }
        }

        private ObservableCollection<OutputInformation> _OutputList;
        public ObservableCollection<OutputInformation> OutputList
        {
            get { return _OutputList; }
            set
            {
                _OutputList = value;
                OnPropertyChanged("OutputList");
            }
        }
        #endregion
        #region [BizRule]
        private BR_PHR_SEL_ProductionOrderOutput_Define_AnyType _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType;
        private BR_BRS_REG_ProductionOrderOutput_Vessel_STRT _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT;
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
                            IsBusy = true;

                            CommandResults["LoadedCommandAsync"] = false;
                            CommandCanExecutes["LoadedCommandAsync"] = false;

                            ///
                            if(arg != null && arg is SVP반제품생성)
                            {
                                _mainWnd = arg as SVP반제품생성;

                                OutputId = _mainWnd.CurrentInstruction.Raw.BOMID;
                                if (string.IsNullOrWhiteSpace(OutputId))
                                    OnMessage("공정중제품이 설정되지 않았습니다.");
                                else
                                {
                                    _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATAs.Clear();
                                    _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs.Clear();
                                    _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = new Guid(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                        OUTPUTTYPE = "WIP",
                                        OUTPUTID = OutputId
                                    });

                                    if(await _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.Execute() && _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs.Count > 0)
                                    {
                                        Unit = _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs[0].STDQTY_UOMNAME;
                                        _strgtime = Convert.ToDouble(_BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs[0].STRGTIME.GetValueOrDefault());
                                        _LotType = _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs[0].OUTPUTTYPE;
                                    }
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

        public ICommand CreateOutputCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["CreateOutputCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["CreateOutputCommandAsync"] = false;
                            CommandCanExecutes["CreateOutputCommandAsync"] = false;

                            ///
                            DateTime cur = await AuthRepositoryViewModel.GetDBDateTimeNow();
                            bool dupflag = false;

                            if (_MSUBLOTQTY <= 0)
                                OnMessage("수량은 0이하 일 수 없습니다.");
                            else if (string.IsNullOrWhiteSpace(EqptId))
                                OnMessage("Pallet번호를 입력하세요.");
                            else
                            {
                                foreach (var item in OutputList)
                                {
                                    if (item.VesselId.ToUpper() == _EqptId)
                                        dupflag = true;
                                }

                                if (dupflag)
                                    OnMessage("중복된 Pallet입니다.");
                                else
                                {
                                    if (OutputList != null && _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs.Count > 0)
                                    {
                                        OutputList.Add(new OutputInformation
                                        {
                                            Outputguid = _BR_PHR_SEL_ProductionOrderOutput_Define_AnyType.OUTDATAs[0].OUTPUTGUID.ToString(),
                                            MsublotQty = _MSUBLOTQTY,
                                            UOM = _Unit,
                                            VesselId = _EqptId,
                                            InsDttm = cur,
                                            ExpireDttm = cur.AddHours(_strgtime)
                                        });

                                        EqptId = "";
                                        MSUBLOTQTY = 0;
                                    }
                                }
                            }              
                            ///

                            CommandResults["CreateOutputCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["CreateOutputCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["CreateOutputCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("CreateOutputCommandAsync") ?
                       CommandCanExecutes["CreateOutputCommandAsync"] : (CommandCanExecutes["CreateOutputCommandAsync"] = true);
               });
            }
        }

        public ICommand RemoveCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RemoveCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RemoveCommandAsync"] = false;
                            CommandCanExecutes["RemoveCommandAsync"] = false;

                            ///
                            if (arg != null && arg is OutputInformation)
                                OutputList.Remove(arg as OutputInformation);                            
                            ///

                            CommandResults["RemoveCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RemoveCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RemoveCommandAsync"] = true;
                            IsBusy = false;
                        }
                    }
                }, arg =>
               {
                   return CommandCanExecutes.ContainsKey("RemoveCommandAsync") ?
                       CommandCanExecutes["RemoveCommandAsync"] : (CommandCanExecutes["RemoveCommandAsync"] = true);
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
                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN == "Y" && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");
                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "SVP반제품생성",
                                "SVP반제품생성",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            string user = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_SUI");

                            //XML 생성. 비즈룰 INDATA생성
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("Pallet번호"));
                            dt.Columns.Add(new DataColumn("수량"));
                            //dt.Columns.Add(new DataColumn("생성일자"));
                            //dt.Columns.Add(new DataColumn("유효일자"));

                            _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT.INDATAs.Clear();
                            _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT.OUTDATAs.Clear();                            

                            foreach (var item in OutputList)
                            {
                                _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT.INDATAs.Add(new BR_BRS_REG_ProductionOrderOutput_Vessel_STRT.INDATA
                                {
                                    LOTTYPE = _LotType,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    OUTPUTGUID = item.Outputguid,
                                    VESSELID = item.VesselId,
                                    INSUSER = user,
                                    MSUBLOTQTY = item.MsublotQty
                                });

                                var row = dt.NewRow();

                                row["Pallet번호"] = item.VesselId ?? "";
                                row["수량"] = item.strMsublotQty ?? "";
                                //row["생성일자"] = item.InsDttm.ToString("yyyy-MM-dd");
                                //row["유효일자"] = item.ExpireDttm.ToString("yyyy-MM-dd");

                                dt.Rows.Add(row);
                            }

                            if(await _BR_BRS_REG_ProductionOrderOutput_Vessel_STRT.Execute())
                            {
                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
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
        #region [Custom]
        public class OutputInformation : ViewModelBase
        {
            private string _Outputguid;
            public string Outputguid
            {
                get { return _Outputguid; }
                set
                {
                    _Outputguid = value;
                    OnPropertyChanged("Outputguid");
                }
            }
            private decimal _MsublotQty;
            public decimal MsublotQty
            {
                get { return _MsublotQty; }
                set
                {
                    _MsublotQty = value;
                    OnPropertyChanged("MsublotQty");
                    OnPropertyChanged("strMsublotQty");
                }
            }
            public string strMsublotQty
            {
                get { return _MsublotQty + _UOM; }
            }
            private string _UOM;
            public string UOM
            {
                get { return _UOM; }
                set
                {
                    _UOM = value;
                    OnPropertyChanged("UOM");
                    OnPropertyChanged("strMsublotQty");
                }
            }
            private string _VesselId;
            public string VesselId
            {
                get { return _VesselId; }
                set
                {
                    _VesselId = value;
                    OnPropertyChanged("VesselId");
                }
            }
            private DateTime _InsDttm;
            public DateTime InsDttm
            {
                get { return _InsDttm; }
                set
                {
                    _InsDttm = value;
                    OnPropertyChanged("InsDttm");
                }
            }
            private DateTime _ExpireDttm;
            public DateTime ExpireDttm
            {
                get { return _ExpireDttm; }
                set
                {
                    _ExpireDttm = value;
                    OnPropertyChanged("ExpireDttm");
                }
            }
        }
        #endregion
    }
}
