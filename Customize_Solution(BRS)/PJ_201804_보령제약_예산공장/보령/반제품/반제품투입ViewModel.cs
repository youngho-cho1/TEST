using LGCNS.iPharmMES.Common;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using ShopFloorUI;
using System.Collections.ObjectModel;
using C1.Silverlight.Data;

namespace 보령
{
    public class 반제품투입ViewModel : ViewModelBase
    {

        반제품투입ViewModel _parentVM;
        public 반제품투입ViewModel ParentVM
        {
            get { return _parentVM; }
            set { _parentVM = value; }
        }


        반제품투입 _mainWnd;


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

        bool _btnEnabled;
        public bool BtnEnabled
        {
            get { return _btnEnabled; }
            set
            {
                _btnEnabled = value;
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


        string _bomID;
        public string BomID
        {
            get { return _bomID; }
            set
            {
                _bomID = value;
                NotifyPropertyChanged();
            }
        }

        string _orderNo;
        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                NotifyPropertyChanged();
            }
        }


        string _processSegmentID;
        public string ProcessSegmentID
        {
            get { return _processSegmentID; }
            set
            {
                _processSegmentID = value;
                NotifyPropertyChanged();
            }
        }

        BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA _matchedComponent;
        public BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA MatchedComponent
        {
            get { return _matchedComponent; }
            set
            {
                _matchedComponent = value;
                NotifyPropertyChanged();
            }
        }

        private BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI _SEL_ProductionOrderOutputSubLot;
        public BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI SEL_ProductionOrderOutputSubLot
        {
            get { return _SEL_ProductionOrderOutputSubLot; }
            set
            {
                if (_SEL_ProductionOrderOutputSubLot != value)
                {
                    _SEL_ProductionOrderOutputSubLot = value;
                    OnPropertyChanged("SEL_ProductionOrderOutputSubLot");
                }
            }
        }

        private ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.OUTDATA> _outputSubLotInfo;
        public ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.OUTDATA> OutputSubLotInfo
        {
            get { return _outputSubLotInfo; }
            set
            {
                if (_outputSubLotInfo != value)
                {
                    _outputSubLotInfo = value;
                    OnPropertyChanged("OutputSubLotInfo");
                }
            }
        }

        async Task<bool> IsMatchedComponent(string Barcode)
        {

            if (OutputSubLotInfo.Count > 0)
            {
                // 투입은 하나씩만 할 수 있도록 변경
                foreach (var info in OutputSubLotInfo)
                {
                    if (info.STATUS.Equals("투입대기"))
                    {
                        info.STATUS = "";
                    }
                }

                var item = OutputSubLotInfo.Where(o => o.VESSELID.ToUpper() == Barcode).FirstOrDefault();

                if( item == null)
                {
                    Message = "반제품 정보가 일치하지 않습니다.";
                    return false;
                }
                else if (item != null && item.STATUS.Contains("투입완료"))
                {
                    Message = "이미 투입이 완료된 반제품입니다.";
                    return false;
                }
                else
                {
                    item.STATUS = "투입대기";
                    BtnEnabled = true;
                }
            }
            else
            {
                Message = "투입 대상 반제품이 존재하지 않습니다.";
                return false;
            }


            return true;
        }


        
        public ICommand LoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadCommand"] = false;
                            CommandCanExecutes["LoadCommand"] = false;

                            ///
                            if (arg != null) _mainWnd = arg as 반제품투입;

                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;

                            this.BomID = instruction.Raw.BOMID;
                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.ProcessSegmentID = _mainWnd.CurrentOrder.OrderProcessSegmentID;

                            //반제품생성여부조회(정보)
                            _SEL_ProductionOrderOutputSubLot.INDATAs.Clear();
                            _SEL_ProductionOrderOutputSubLot.OUTDATAs.Clear();
                            _SEL_ProductionOrderOutputSubLot.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.INDATA()
                            {
                                POID = OrderNo,
                                OPSGGUID = ProcessSegmentID,
                                //OUTPUTTYPE = "WIP",
                                //ISUSEFLAG = true
                            });

                            if (await _SEL_ProductionOrderOutputSubLot.Execute() == false) throw SEL_ProductionOrderOutputSubLot.Exception;

                            OutputSubLotInfo.Clear();

                            foreach (var item in _SEL_ProductionOrderOutputSubLot.OUTDATAs)
                            {
                                OutputSubLotInfo.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.OUTDATA
                                    {
                                        CHECK = "N",
                                        POID = item.POID,
                                        OUTPUTGUID = item.OUTPUTGUID,
                                        VERSION = item.VERSION,
                                        OPSGGUID = item.OPSGGUID,
                                        OUTPUTID = item.OUTPUTID,
                                        OUTPUTDESC = item.OUTPUTDESC,
                                        OUTPUTTYPE = item.OUTPUTTYPE,
                                        MLOTID = item.MLOTID,
                                        MLOTVER = item.MLOTVER,
                                        MSUBLOTID = item.MSUBLOTID,
                                        MSUBLOTBCD = item.MSUBLOTBCD,
                                        MSUBLOTVER = item.MSUBLOTVER,
                                        MSUBLOTQTY = item.MSUBLOTQTY,
                                        MSUBLOTSEQ = item.MSUBLOTSEQ,
                                        ROWNUM = item.ROWNUM,
                                        INSDTTM_PO = item.INSDTTM_PO,
                                        INSUSER_PO = item.INSUSER_PO,
                                        INSDTTM_MS = item.INSDTTM_MS,
                                        INSUSER_MS = item.INSUSER_MS,
                                        STATUS = item.STATUS,
                                        OLDMSUBLOTQTY = item.OLDMSUBLOTQTY,
                                        VESSELID = item.VESSELID,

                                    });
                            }
                            

                            ///

                            CommandResults["LoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommand") ?
                        CommandCanExecutes["LoadCommand"] : (CommandCanExecutes["LoadCommand"] = true);
                });
            }
        }


        
        public ICommand KeyDownCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["KeyDownCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["KeyDownCommand"] = false;
                            CommandCanExecutes["KeyDownCommand"] = false;

                            ///
                            await IsMatchedComponent(Barcode.ToUpper());

                            Barcode = string.Empty;

                            ///

                            CommandResults["KeyDownCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["KeyDownCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["KeyDownCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("KeyDownCommand") ?
                        CommandCanExecutes["KeyDownCommand"] : (CommandCanExecutes["KeyDownCommand"] = true);
                });
            }
        }

        public void CheckSelect(object param, bool chk)
        {
            try
            {
                IsBusy = true;

                if (param != null)
                {
                    var tmparam = param as BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.OUTDATA;

                    if (chk)
                    {
                        tmparam.STATUS = "투입대기";
                        BtnEnabled = true;
                    }
                    else
                    {
                        tmparam.STATUS = "";
                        if (_outputSubLotInfo.Count > 0)
                        {
                            var match = _outputSubLotInfo.Where(o => o.STATUS == "투입대기").FirstOrDefault();
                            if (match != null) BtnEnabled = true;
                            else BtnEnabled = false;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                OnMessage(ex.Message);
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ICommand RecycleCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RecycleCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RecycleCommandAsync"] = false;
                            CommandCanExecutes["RecycleCommandAsync"] = false;

                            ///

                            // 반제품 투입
                            var chargingItem = OutputSubLotInfo.Where(o => o.STATUS == "투입대기").ToList();

                            var bizRule = new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC();

                           
                            chargingItem.ForEach(o =>
                            {
                                bizRule.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    MSUBLOTID = o.MSUBLOTID,
                                    MSUBLOTBCD = o.MSUBLOTBCD,
                                    MSUBLOTQTY = o.MSUBLOTQTY != null ? string.Format("{0:###.##0}",o.MSUBLOTQTY) : "",
                                    INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    INSDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_OUTPUT = "Y",
                                    IS_INVEN_CHARGE = "N",
                                    CHECKINUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    OUTPUTGUID = o.OUTPUTGUID,
                                    VESSELID = o.VESSELID,
                                    GUBUN = "RECYCLE"
                                });

                            });

                            if (await bizRule.Execute() == false) return;

                            foreach (var item in chargingItem)
                            {
                                item.STATUS = "투입완료";
                            }
                            
                            ///

                            CommandResults["RecycleCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RecycleCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RecycleCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecycleCommandAsync") ?
                        CommandCanExecutes["RecycleCommandAsync"] : (CommandCanExecutes["RecycleCommandAsync"] = true);
                });
            }
        }

        public ICommand UnuseCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["UnuseCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["UnuseCommandAsync"] = false;
                            CommandCanExecutes["UnuseCommandAsync"] = false;

                            ///
                            // 반제품 투입
                            var chargingItem = OutputSubLotInfo.Where(o => o.STATUS == "투입대기").ToList();

                            var bizRule = new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC();

                            chargingItem.ForEach(o =>
                            {

                                bizRule.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging_EQAC.INDATA
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    MSUBLOTID = o.MSUBLOTID,
                                    MSUBLOTBCD = o.MSUBLOTBCD,
                                    MSUBLOTQTY = o.MSUBLOTQTY != null ? string.Format("{0:###.##0}", o.MSUBLOTQTY) : "",
                                    INSUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    INSDTTM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_OUTPUT = "Y",
                                    IS_INVEN_CHARGE = "N",
                                    CHECKINUSER = AuthRepositoryViewModel.Instance.LoginedUserID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    OUTPUTGUID = o.OUTPUTGUID,
                                    VESSELID = o.VESSELID,
                                    GUBUN = "ENDUSE"
                                });
                            });

                            if (await bizRule.Execute() == false) return;

                             foreach (var item in chargingItem)
                            {
                                item.STATUS = "투입완료";
                            }
                            ///

                            CommandResults["UnuseCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["UnuseCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["UnuseCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("UnuseCommandAsync") ?
                        CommandCanExecutes["UnuseCommandAsync"] : (CommandCanExecutes["UnuseCommandAsync"] = true);
                });
            }
        }


        public ICommand RecordCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RecordCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RecordCommandAsync"] = false;
                            CommandCanExecutes["RecordCommandAsync"] = false;

                            ///
                            if (OutputSubLotInfo.Count <= 0) return;

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


                            ///////////////////////////반제품투입//////////////////////////////////////////////////////////////////////
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "반제품 투입",
                                "반제품 투입",
                                false ,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // 2022.12.06 김호연 반제품 하나라도 투입을 해야 기록할수 있도록 로직 수정.
                            if (OutputSubLotInfo.Where(o => (o.STATUS == "투입완료")).Count() == 0)
                            {
                                throw new Exception(string.Format("반제품 투입을 해주세요"));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("OUTPUTID"));
                            dt.Columns.Add(new DataColumn("수량"));
                            dt.Columns.Add(new DataColumn("상태"));
                            //dt.Columns.Add(new DataColumn("투입일자"));

                            OutputSubLotInfo.Where(o => (o.STATUS == "투입대기" || o.STATUS == "투입완료")).ToList().ForEach(x =>
                            {
                                var row = dt.NewRow();
                                row["OUTPUTID"] = x.OUTPUTID;
                                row["수량"] = x.STATUS != "투입완료" ? (x.MSUBLOTQTY != null ? decimal.Parse(x.MSUBLOTQTY.ToString()).ToString("#,0.0 g") : "")
                                                                     : (x.OLDMSUBLOTQTY != null ? decimal.Parse(x.OLDMSUBLOTQTY.ToString()).ToString("#,0.0 g") : "");
                                row["상태"] = String.IsNullOrWhiteSpace(x.STATUS) ? "" : x.STATUS;
                                dt.Rows.Add(row);
                            });

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
                            ///

                            CommandResults["RecordCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RecordCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RecordCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RecordCommandAsync") ?
                        CommandCanExecutes["RecordCommandAsync"] : (CommandCanExecutes["RecordCommandAsync"] = true);
                });
            }
        }
       

        public 반제품투입ViewModel()
        {
            BtnEnabled = false;

            _SEL_ProductionOrderOutputSubLot = new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI();
            _outputSubLotInfo = new ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_MULTI.OUTDATA>();
        }
    }
}
