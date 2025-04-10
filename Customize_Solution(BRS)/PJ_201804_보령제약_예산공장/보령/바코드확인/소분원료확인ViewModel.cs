using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;

namespace 보령
{
    public class 소분원료확인ViewModel : ViewModelBase
    {
        #region Properties

        ShopFloorCustomWindow _mainWnd;
        public ShopFloorCustomWindow MainWnd
        {
            get { return _mainWnd; }
            set { _mainWnd = value; }
        }

        string _batchNo;
        public string BatchNo
        {
            get { return _batchNo; }
            set
            {
                _batchNo = value;
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

        string _ProcessSegmentName;
        public string ProcessSegmentName
        {
            get { return _ProcessSegmentName; }
            set
            {
                _ProcessSegmentName = value;
                NotifyPropertyChanged();
            }
        }

        bool _is_EnableOKBtn;
        public bool Is_EnableOKBtn
        {
            get { return _is_EnableOKBtn; }
            set
            {
                _is_EnableOKBtn = value;
                NotifyPropertyChanged();
            }
        }

        List<string> _currentBOMID;
        #endregion

        BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ _BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ;
        public BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ
        {
            get { return _BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ; }
            set { _BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ = value; }
        }

        BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATACollection _filteredComponents;
        public BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATACollection FilteredComponents
        {
            get { return _filteredComponents; }
            set { _filteredComponents = value; }
        }

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

                            ///



                           
                            if(arg == null)
                                return;

                            MainWnd = arg as ShopFloorCustomWindow;
                            _currentBOMID = new List<string>();
                            var paramInsts = InstructionModel.GetParameterSender(MainWnd.CurrentInstruction, MainWnd.Instructions);
                            decimal tempEXPRESSION;
                   
                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                            this.ProcessSegmentName = _mainWnd.CurrentOrder.OrderProcessSegmentName;

                            FilteredComponents.Clear();

                            if (decimal.TryParse(MainWnd.CurrentInstruction.Raw.EXPRESSION, out tempEXPRESSION))
                            {
                                BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.INDATAs.Add(new BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.INDATA()
                                {
                                    POID = MainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = MainWnd.CurrentOrder.OrderProcessSegmentID,
                                    CHGSEQ = tempEXPRESSION,
                                    MTRLID = MainWnd.CurrentInstruction.Raw.BOMID
                                });
                            }

                            foreach (var instruction in paramInsts)
                            {
                                if (decimal.TryParse(instruction.Raw.EXPRESSION, out tempEXPRESSION))
                                {
                                    BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.INDATAs.Add(new BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.INDATA()
                                    {
                                        POID = MainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = MainWnd.CurrentOrder.OrderProcessSegmentID,
                                        CHGSEQ = decimal.Parse(instruction.Raw.EXPRESSION),
                                        MTRLID = instruction.Raw.BOMID
                                    });
                                }
                            }

                            if (await BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.Execute() == false) return;

                            foreach (var outdata in BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATAs)
                            {
                                FilteredComponents.Add(outdata);
                            }

                            FilteredComponents.Where(o => Convert.ToSingle(o.REMAIN) <= 0).ToList().ForEach(o =>
                            {
                                o.IS_DEPLETED = true;
                            });


                            var notCompletedItem = FilteredComponents.Where(o =>
                               // o.IS_WEIGHT_CHECKED == true &&
                                o.IS_DEPLETED == false &&
                                o.IS_CAN_CHARGING_CHECKED == false).FirstOrDefault();

                            if (notCompletedItem != null)
                            {
                                var viewmodel = new 소분원료확인팝업ViewModel()
                                {
                                    ParentVM = this,
                                };
                                var popup = new 소분원료확인팝업()
                                {
                                    DataContext = viewmodel
                                };
                                popup.Show();
                            }
                            else
                            {
                                OnMessage("투입할 원료가 없습니다.");
                            }

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
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommand") ?
                        CommandCanExecutes["LoadedCommand"] : (CommandCanExecutes["LoadedCommand"] = true);
                });
            }
        }

        public ICommand ConfirmCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ConfirmCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ConfirmCommand"] = false;
                            //CommandCanExecutes["ConfirmCommand"] = false;

                            ///
                            var chargingItem = FilteredComponents.Where(o => o.IS_CAN_CHARGING_CHECKED == true).ToList();

                            if (chargingItem.Count <= 0) throw new Exception("확인할 원료가 없습니다.");

                            if (_mainWnd.CurrentInstruction.Raw.INSERTEDYN.Equals("Y") && _mainWnd.Phase.CurrentPhase.STATE.Equals("COMP")) // 값 수정
                            {
                                // 전자서명 요청
                                var authHelper = new iPharmAuthCommandHelper();
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

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);
                            dt.Columns.Add(new DataColumn("원료코드"));
                            dt.Columns.Add(new DataColumn("원료명"));
                            dt.Columns.Add(new DataColumn("원료시험번호"));
                            dt.Columns.Add(new DataColumn("바코드"));
                            dt.Columns.Add(new DataColumn("무게"));
                            //dt.Columns.Add(new DataColumn("단위"));

                          
                            chargingItem.ForEach(o =>
                            {
                                var row = dt.NewRow();
                                row["원료코드"] = o.MTRLID;
                                row["원료명"] = o.MTRLNAME;
                                row["원료시험번호"] = o.MLOTID;
                                row["바코드"] = o.SCANNED_BARCODE;
                                // 2021.08.22 박희돈 단위 컬럼 제거. 전체무게에 더해 단위 표시하도록 변경.
                                row["무게"] =  o.DSP + " " + o.UOM;
                                //row["단위"] = o.UOM;
                                dt.Rows.Add(row);
                            });
                            
                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                            _mainWnd.CurrentInstruction.Raw.ACTVAL = "TABLE,원료확인";
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction);

                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);
                            ///

                            CommandResults["ConfirmCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ConfirmCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ConfirmCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ConfirmCommand") ?
                        CommandCanExecutes["ConfirmCommand"] : (CommandCanExecutes["ConfirmCommand"] = true);
                });
            }
        }

        public 소분원료확인ViewModel()
        {
            _BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ = new BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ();
            _filteredComponents = new BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATACollection();

            Is_EnableOKBtn = false;
        }
    }
}


