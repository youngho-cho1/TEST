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
    public class 포장반제품투입ViewModel : ViewModelBase
    {
        public 포장반제품투입ViewModel()
        {

            _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT = new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT();
            _outputSubLotInfo = new ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA>();
            inputBtnEnable = false;  

        }

        포장반제품투입 _mainWnd;

        private BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT;
        public BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT
        {
            get { return _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT; }
            set
            {
                if (_BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT != value)
                {
                    _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT = value;
                    OnPropertyChanged("BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT");
                }
            }
        }

        private ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA> _outputSubLotInfo;
        public ObservableCollection<BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATA> OutputSubLotInfo
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

        string _VTSID;
        public string VTSID
        {
            get { return _VTSID; }
            set
            {
                _VTSID = value;
            }
        }

        string _VESSELID;
        public string VESSELID
        {
            get { return _VESSELID; }
            set
            {
                _VESSELID = value;
            }
        }

        string _VTS_UNIT;
        public string VTS_UNIT
        {
            get { return _VTS_UNIT; }
            set
            {
                _VTS_UNIT = value;
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
                            if (arg != null) _mainWnd = arg as 포장반제품투입;

                            var instruction = _mainWnd.CurrentInstruction;
                            var phase = _mainWnd.Phase;

                            _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATAs.Add(new BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.INDATA()
                            {
                                POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                            });

                            if (await _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.Execute() == false) throw _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.Exception;

                            OutputSubLotInfo.Clear();

                            foreach (var item in _BR_BRS_SEL_ProductionOrderOutputSubLot_OPSG_FERT.OUTDATAs)
                            {
                                item.NET.SetWeight(item.MSUBLOTQTY.GetValueOrDefault(), item.UOM, 0);
                                OutputSubLotInfo.Add(item);
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
                            IsMatchedComponent(Barcode.ToUpper());

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

        public ICommand ChargingCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ChargingCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ChargingCommand"] = false;
                            CommandCanExecutes["ChargingCommand"] = false;

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


                            ///////////////////////////반제품투입//////////////////////////////////////////////////////////////////////
                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_Charging");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "반제품 투입",
                                "반제품 투입",
                                false,
                                "OM_ProductionOrder_Charging",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }


                            var ds = new DataSet();
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            // 2021.08.22 박희돈 자재ID 기록 삭제(EBR 수정 항목)
                            //dt.Columns.Add(new DataColumn("자재ID"));
                            dt.Columns.Add(new DataColumn("자재명"));
                            dt.Columns.Add(new DataColumn("OUTPUTID"));
                            dt.Columns.Add(new DataColumn("현재수량"));
                            // 2021.08.22 박희돈 IBC관리번호 - > 용기번호 로 변경
                            dt.Columns.Add(new DataColumn("용기번호"));
                            dt.Columns.Add(new DataColumn("투입일자"));
                            dt.Columns.Add(new DataColumn("작업자"));

                            var chargingItem = _outputSubLotInfo.Where(o => o.STATUS == "투입대기").ToList();

                            var bizRule = new BR_BRS_REG_MaterialSubLot_Dispense_Charging_FERT();
                            DateTime chargingdttm = await AuthRepositoryViewModel.GetDBDateTimeNow();

                            chargingItem.ForEach(o =>
                            {
                                bizRule.INDATAs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging_FERT.INDATA()
                                {
                                    LANGID = AuthRepositoryViewModel.Instance.LangID,
                                    MSUBLOTBCD = o.MSUBLOTBCD,
                                    MSUBLOTID = o.MSUBLOTID,
                                    MSUBLOTQTY = o.NET.Value,
                                    POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                    OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                    IS_NEED_CHKWEIGHT = "N",
                                    IS_FULL_CHARGE = "Y",
                                    IS_CHECKONLY = "N",
                                    IS_OUTPUT = "N",
                                    IS_INVEN_CHARGE = "Y",
                                    INSUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                    CHECKINUSER = AuthRepositoryViewModel.GetSecondUserIDByFunctionCode("OM_ProductionOrder_Charging"),
                                });

                                bizRule.INDATA_INVs.Add(new BR_BRS_REG_MaterialSubLot_Dispense_Charging_FERT.INDATA_INV()
                                {
                                    COMPONENTGUID = o.COMPONENTGUID
                                });

                                var row = dt.NewRow();
                                // 2021.08.22 박희돈 자재ID 기록 삭제(EBR 수정 항목)
                                //row["자재ID"] = o.MTRLID != null ? o.MTRLID.ToString() : "";
                                row["자재명"] = o.MTRLNAME != null ? o.MTRLNAME.ToString() : "";
                                row["OUTPUTID"] = o.OUTPUTID != null ? o.OUTPUTID.ToString() : "";
                                row["현재수량"] = o.NET.WeightUOMStringWithSeperator;
                                // 2021.08.22 박희돈 IBC관리번호 - > 용기번호 로 변경
                                row["용기번호"] = o.VESSELID != null ? o.VESSELID.ToString() : "";
                                row["투입일자"] = chargingdttm.ToString("yyyy-MM-dd HH:mm:ss");
                                row["작업자"] = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_ProductionOrder_Charging");

                                dt.Rows.Add(row);
                            });

                            if (await bizRule.Execute() == false) return;

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

                            CommandResults["ChargingCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ChargingCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ChargingCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ChargingCommand") ?
                        CommandCanExecutes["ChargingCommand"] : (CommandCanExecutes["ChargingCommand"] = true);
                });
            }
        }

        private bool IsMatchedComponent(string Barcode)
        {
            if (OutputSubLotInfo.Count > 0)
            {
                var item = OutputSubLotInfo.Where(o => o.VESSELID == Barcode || o.MSUBLOTBCD == Barcode).FirstOrDefault();

                if (item == null)
                {
                    Message = "반제품 정보가 일치하지 않습니다.";
                    return false;
                }
                else
                {
                    item.STATUS = "투입대기";
                    _mainWnd.dgProductionOutput.SelectedIndex = Convert.ToInt16(item.RowIndex);
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
    }    

}
