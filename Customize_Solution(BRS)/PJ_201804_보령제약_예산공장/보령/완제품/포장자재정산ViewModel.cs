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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ShopFloorUI;
using C1.Silverlight.Data;
using System.Linq;

namespace 보령
{
    public class 포장자재정산ViewModel : ViewModelBase
    {
        #region [Property]
        public 포장자재정산ViewModel()
        {
            _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New = new BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New();
            _PackingInfoList = new ObservableCollection<PackingInfo>();
        }
        private 포장자재정산 _mainWnd;

        private ObservableCollection<PackingInfo> _PackingInfoList;
        public ObservableCollection<PackingInfo> PackingInfoList
        {
            get { return _PackingInfoList; }
            set
            {
                _PackingInfoList = value;
                OnPropertyChanged("PackingInfoList");
            }
        }
        #endregion

        #region [BizRule]
        private BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New;


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
                            CommandCanExecutes["LoadedCommandAsync"] = false;
                            CommandResults["LoadedCommandAsync"] = false;

                            ///
                            decimal temp = 0m; // tryparse

                            if (arg != null && arg is 포장자재정산)
                            {
                                _mainWnd = arg as 포장자재정산;

                                var inputValues = InstructionModel.GetParameterSender(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                                if (inputValues == null)
                                    return;

                                _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.OUTDATAs.Clear();

                                foreach (var item in inputValues)
                                {
                                    _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.INDATAs.Add(new BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        MTRLID = item.Raw.BOMID,
                                        CHGSEQ = Convert.ToDecimal(item.Raw.EXPRESSION)
                                    });
                                }                              

                                if (await _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.Execute() != false)
                                {
                                    foreach (var item in _BR_BRS_SEL_ProductionOrderBOM_PickingInfo_New.OUTDATAs)
                                    {
                                        _PackingInfoList.Add(new PackingInfo
                                        {
                                            MTRLID = !string.IsNullOrWhiteSpace(item.MTRLID) ? item.MTRLID : "",
                                            MTRLNAME = !string.IsNullOrWhiteSpace(item.MTRLNAME) ? item.MTRLNAME : "",
                                            UOM = !string.IsNullOrWhiteSpace(item.UOM) ? item.UOM : "",
                                            PICKINGQTY = !string.IsNullOrWhiteSpace(item.TOTALPICKINGQTY) ? item.TOTALPICKINGQTY : "0",
                                            ADDQTY = !string.IsNullOrWhiteSpace(item.ADDITIONALQTY) ? item.ADDITIONALQTY : "0",
                                            SCRAPQTY = "0",
                                            SAMPLEQTY = "0",
                                            REMAINQTY = !string.IsNullOrWhiteSpace(item.RETURNQTY) ? item.RETURNQTY : "0",
                                            USEQTY = "0",
                                            // 2024.11.17  박희돈 사용하지 않아서 주석
                                            //Param = decimal.TryParse(inputValues.FirstOrDefault(x => x.Raw.BOMID.Equals(item.MTRLID)).Raw.TARGETVAL, out temp) ? temp : 1m,      
                                        });
                                    }
                                }

                                OnPropertyChanged("PackingInfoList");
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
                            CommandCanExecutes["ComfirmCommandAsync"] = false;
                            CommandResults["ComfirmCommandAsync"] = false;

                            ///
                            
                            if (_PackingInfoList != null && _PackingInfoList.Count > 0)
                            {
                                foreach (var item in _PackingInfoList)
                                {
                                    if (item.HaveNagativeNum())
                                        throw new Exception(string.Format("[{0}]{1} : 입력값에 음수가 있습니다.", item.MTRLID, item.MTRLNAME));
                                }

                                // 전자서명
                                var authHelper = new iPharmAuthCommandHelper();
                                if (_mainWnd.CurrentInstruction.Raw.INSDTTM.Equals("Y") && _mainWnd.CurrentInstruction.PhaseState.Equals("COMP"))
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
                                    "포장자재출고기록",
                                    "포장자재출고기록",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                // XML 기록
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                ds.Tables.Add(dt);
                                dt.Columns.Add(new DataColumn("MTRLID"));
                                dt.Columns.Add(new DataColumn("MTRLNAME"));
                                dt.Columns.Add(new DataColumn("UOM"));
                                dt.Columns.Add(new DataColumn("PICKING"));
                                dt.Columns.Add(new DataColumn("ADDING"));
                                dt.Columns.Add(new DataColumn("SCRAP"));
                                dt.Columns.Add(new DataColumn("SAMPLE"));
                                dt.Columns.Add(new DataColumn("REMAIN"));
                                dt.Columns.Add(new DataColumn("USING"));

                                foreach (var item in _PackingInfoList)
                                {
                                    var row = dt.NewRow();

                                    row["MTRLID"] = item.MTRLID ?? "";
                                    row["MTRLNAME"] = item.MTRLNAME ?? "";
                                    row["UOM"] = item.UOM ?? "";
                                    row["PICKING"] = item.PICKINGQTY ?? "";
                                    row["ADDING"] = item.ADDQTY ?? "";
                                    row["SCRAP"] = item.SCRAPQTY ?? "";
                                    row["SAMPLE"] = item.SAMPLEQTY ?? "";
                                    row["REMAIN"] = item.REMAINQTY ?? "";
                                    row["USING"] = item.USEQTY ?? "";

                                    dt.Rows.Add(row);
                                }

                                var xml = BizActorRuleBase.CreateXMLStream(ds);
                                var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);

                                _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                                _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                                // 지시문 입력 데이터 생성
                                var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, true);
                                if (result != enumInstructionRegistErrorType.Ok)
                                {
                                    throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                                }

                                // 창 종료
                                if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                                else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);

                            }
                            else
                                OnMessage("기록할 내용이 없습니다.");
                            ///

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
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                        CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                });
            }
        }
        #endregion

        #region [User Define]
        public class PackingInfo : BizActorDataSetBase
        {
            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    OnPropertyChanged("MTRLID");
                }
            }
            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    OnPropertyChanged("MTRLNAME");
                }
            }
            private string _UOM;
            public string UOM
            {
                get { return _UOM; }
                set
                {
                    _UOM = value;
                    OnPropertyChanged("UOM");
                }
            }
            private decimal _PICKINGQTY;
            public string PICKINGQTY
            {
                get { return string.Format("{0:#,0}", _PICKINGQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _PICKINGQTY = chk;
                        CalcScrapQty();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("PICKINGQTY");
                }
            }
            private decimal _ADDQTY;
            public string ADDQTY
            {
                get { return string.Format("{0:#,0}", _ADDQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _ADDQTY = chk;
                        CalcScrapQty();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("ADDQTY");
                }
            }
            private decimal _SCRAPQTY;
            public string SCRAPQTY
            {
                get { return string.Format("{0:#,0}", _SCRAPQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _SCRAPQTY = chk;
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("SCRAPQTY");
                }
            }
            private decimal _SAMPLEQTY;
            public string SAMPLEQTY
            {
                get { return string.Format("{0:#,0}", _SAMPLEQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _SAMPLEQTY = chk;
                        CalcScrapQty();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("SAMPLEQTY");
                }
            }
            private decimal _REMAINQTY;
            public string REMAINQTY
            {
                get { return string.Format("{0:#,0}", _REMAINQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _REMAINQTY = chk;
                        CalcScrapQty();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("REMAINQTY");
                }
            }
            private decimal _USEQTY;
            public string USEQTY
            {
                get { return string.Format("{0:#,0}", _USEQTY); }
                set
                {
                    decimal chk;
                    if (decimal.TryParse(value, out chk))
                    {
                        if (chk < 0)
                            OnMessage("입력한 값이 음수 입니다.");

                        _USEQTY = chk;
                        CalcScrapQty();
                    }
                    else
                        OnMessage("입력한 내용이 숫자형이 아닙니다.");

                    OnPropertyChanged("USEQTY");
                }
            }
            private decimal _Param;
            public decimal Param
            {
                get { return _Param; }
                set
                {
                    _Param = value;
                    OnPropertyChanged("Param");
                }
            }

            private void CalcScrapQty()
            {
                decimal scrap = this._PICKINGQTY + this._ADDQTY
                    - this._SAMPLEQTY - this._REMAINQTY - this._USEQTY;

                SCRAPQTY = scrap.ToString();
            }

            public bool HaveNagativeNum()
            {
                if (this._PICKINGQTY < 0 || this._ADDQTY < 0 || this._SCRAPQTY < 0 || this._SAMPLEQTY < 0 || this._REMAINQTY < 0 || this._USEQTY < 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}
