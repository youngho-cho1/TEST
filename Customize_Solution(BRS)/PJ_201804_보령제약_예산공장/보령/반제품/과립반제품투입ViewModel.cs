using C1.Silverlight;
using C1.Silverlight.Data;
using Equipment;
using LGCNS.EZMES.Common;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Common = LGCNS.iPharmMES.Common.Common;

namespace 보령
{
    public class 과립반제품투입ViewModel : ViewModelBase
    {

        enum enumScanType
        {
            Output
        };

        public 과립반제품투입ViewModel()
        {

            _BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID = new BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID();

            inputBtnEnable = false;
           
        }


        과립반제품투입 _mainWnd;


        BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID _BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID;
        public BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID
        {
            get { return _BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID; }
            set
            {
                _BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID = value;
                NotifyPropertyChanged();
            }
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



        bool _inputBtnEnable;
        public bool inputBtnEnable
        {
            get { return _inputBtnEnable; }
            set
            {
                _inputBtnEnable = value;
                NotifyPropertyChanged();
            }
        }
        

        public ICommand LoadedCommand
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
                            if (arg != null) _mainWnd = arg as 과립반제품투입;

                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;

                            this.OrderNo = _mainWnd.CurrentOrder.OrderID;
                            this.BatchNo = _mainWnd.CurrentOrder.BatchNo;
                            this.ProcessSegmentID = _mainWnd.CurrentOrder.OrderProcessSegmentID;

                            BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGID = _mainWnd.CurrentOrder.OrderProcessSegmentID
                            });

                            if (await BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.Execute() == false) return;

                            ScanCommandAsync.Execute(this);
                           
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


        void ScanPopup_Closed(object sender, EventArgs e)
        {
            var popup = sender as 반제품스캔팝업;
            popup.Closed -= ScanPopup_Closed;

        }


        
        public ICommand ScanCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ScanCommand"] = false;
                            CommandCanExecutes["ScanCommand"] = false;

                            ///

                            var viewmodel = new 반제품스캔팝업ViewModel()
                            {
                                ParentVM = this,
                            };

                            var ScanPopup = new 반제품스캔팝업()
                            {
                                DataContext = viewmodel
                            };
                            ScanPopup.Closed += ScanPopup_Closed;
                            ScanPopup.Show();

                            ///

                            CommandResults["ScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanCommand") ?
                        CommandCanExecutes["ScanCommand"] : (CommandCanExecutes["ScanCommand"] = true);
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
                            CommandCanExecutes["ConfirmCommand"] = false;


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
                                "반제품 투입 및 생성",
                                "반제품 투입 및 생성",
                                false ,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("반제품명"));
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("원료배치번호"));
                            //2022.12.02 김호연 QA 요청사항으로 과립 반제품 무게 제거
                            //dt.Columns.Add(new DataColumn("무게"));


                            string outputID = _mainWnd.CurrentInstruction.Raw.BOMID;
                            if (outputID == null) throw new Exception("레시피에 정의된 OUTPUT이 없습니다.");

                            var bizRuleSelectOutput = new BR_PHR_SEL_ProductionOrderOutput_Define();
                            bizRuleSelectOutput.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                OUTPUTTYPE = "WIP",
                            });

                            if (await bizRuleSelectOutput.Execute() == false) throw bizRuleSelectOutput.Exception;

                            if (bizRuleSelectOutput.OUTDATAs.Count <= 0) throw new Exception("정의된 OUTPUT이 없습니다");

                            var matchedComponent = bizRuleSelectOutput.OUTDATAs.Where(o => o.OUTPUTID == outputID).FirstOrDefault();
                            if (matchedComponent == null) throw new Exception(string.Format("[{0}] 은 정의된 OUTPUT이 아닙니다.", outputID));


                            var chargingItem = BR_PHR_SEL_ProductionOrderOutputSubLot_CHGPCSGID.OUTDATAs.Where(o => o.CHECK == "True").ToList();


                            var bizRule = new BR_BRS_REG_ProductionOrderOutput_Charging_Output();

                            foreach (var item in chargingItem)
                            {

                                float tareWeight;
                                float msumlotQty;


                                if(item.TAREWEIGHT.ToString() != string.Empty)
                                    tareWeight = float.Parse(decimal.Parse(item.TAREWEIGHT.ToString()).ToString("0.##0"));
                                else
                                    tareWeight = 0;

                                if (item.MSUBLOTQTY.ToString() != string.Empty)
                                    msumlotQty = float.Parse(decimal.Parse(item.MSUBLOTQTY.ToString()).ToString("0.##0"));
                                else
                                    msumlotQty = 0;

                                //이전공정 Qty = 0으로 만들기 
                                bizRule.INDATA_CHARGINGs.Add(new BR_BRS_REG_ProductionOrderOutput_Charging_Output.INDATA_CHARGING()
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,//item.BE_OPSGGUID,
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_OUTPUT = "Y",
                                    IS_INVEN_CHARGE = "N",
                                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                    CHECKINUSER = AuthRepositoryViewModel.GetSecondUserIDByFunctionCode("OM_ProductionOrder_Charging")
                                });


                                bizRule.INDATA_OUTPUT_ADJUSTs.Add(new BR_BRS_REG_ProductionOrderOutput_Charging_Output.INDATA_OUTPUT_ADJUST()
                               {
                                   POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                   OPSGGUID = Guid.Parse(_mainWnd.CurrentOrder.OrderProcessSegmentID),
                                   OUTPUTGUID = matchedComponent.OUTPUTGUID,
                                   IS_NEED_VESSELID = "N",
                                   IS_ONLY_TARE = "N",
                                   INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                   VESSELID = item.VESSELID,
                                   LOTTYPE = bizRuleSelectOutput.OUTDATAs[0].OUTPUTTYPE,
                                   //MSUBLOTID = item.MSUBLOTID,
                                   //MSUBLOTBCD = item.MSUBLOTBCD,
                                   MSUBLOTQTY = msumlotQty,
                                   REASON = "Create Empty Output",
                                   TAREUOMID = bizRuleSelectOutput.OUTDATAs[0].UOMID,
                                   TAREWEIGHT = tareWeight,
                                   IS_NEW = "Y",
                               });



                                var row = dt.NewRow();
                                row["반제품명"] = item.OUTPUTID != null ? item.OUTPUTID : "";
                                row["용기번호"] = item.VESSELID != null ? item.VESSELID : "";
                                row["원료배치번호"] = item.MLOTID != null ? item.MLOTID.ToString() : "";
                                //2022.12.02 김호연 QA 요청사항으로 과립 반제품 무게 제거
                                //row["무게"] = item.MSUBLOTQTY != null ? decimal.Parse(item.MSUBLOTQTY.ToString()).ToString("0.##0") : "";
                                dt.Rows.Add(row);

                            }

                            if (await bizRule.Execute() == false) throw bizRule.Exception;


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


    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString() == "N")
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.White);
            }
            else
            {
                return new SolidColorBrush(Colors.White);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
