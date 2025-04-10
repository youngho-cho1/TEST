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
using System.Collections.ObjectModel;
using System.Linq;
using ShopFloorUI;

namespace 보령
{
    public class 출고포장자재환산ViewModel : ViewModelBase
    {
        #region [Property]
        private 출고포장자재환산 _mainWnd;

        private decimal _Param;
        public decimal Param
        {
            get { return _Param; }
            set
            {
                _Param = value;
                NotifyPropertyChanged();
            }
        }

        private decimal _RollCount;
        public decimal RollCount
        {
            get { return _RollCount; }
            set
            {
                _RollCount = value;
                NotifyPropertyChanged();
            }
        }

        private string _MSUBLOTBCD;
        public string MSUBLOTBCD
        {
            get { return _MSUBLOTBCD; }
            set
            {
                _MSUBLOTBCD = value;
                NotifyPropertyChanged();
            }
        }

        private decimal _Weight;
        public decimal Weight
        {
            get { return _Weight; }
            set
            {
                _Weight = value;
                NotifyPropertyChanged();
            }
        }

        private decimal _TotalLength;
        public decimal TotalLength
        {
            get { return _TotalLength; }
            set
            {
                _TotalLength = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PackingMTRLInfo> _PackingMTRLList;
        public ObservableCollection<PackingMTRLInfo> PackingMTRLList
        {
            get { return _PackingMTRLList; }
            set
            {
                _PackingMTRLList = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<RollWeightInfo> _RollWeightList;
        public ObservableCollection<RollWeightInfo> RollWeightList
        {
            get { return _RollWeightList; }
            set
            {
                _RollWeightList = value;
                NotifyPropertyChanged();
            }
        }

       //listbox
        #endregion

        #region [BizRule]
        private BR_BRS_SEL_ProductionOrder_Component_PickingInfo _BR_BRS_SEL_ProductionOrder_Component_PickingInfo;
        public BR_BRS_SEL_ProductionOrder_Component_PickingInfo BR_BRS_SEL_ProductionOrder_Component_PickingInfo
        {
            get { return _BR_BRS_SEL_ProductionOrder_Component_PickingInfo; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_Component_PickingInfo = value;
                NotifyPropertyChanged();
            }
        }

        private BR_PHR_INS_MaterialSubLotCustomAttributes_Multi _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi;
        public BR_PHR_INS_MaterialSubLotCustomAttributes_Multi BR_PHR_INS_MaterialSubLotCustomAttributes_Multi
        {
            get { return _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi; }
            set
            {
                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi = value;
                NotifyPropertyChanged();
            }
        }
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

                                if (arg != null && arg is 출고포장자재환산)
                                {
                                    _mainWnd = arg as 출고포장자재환산;

                                    _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.Clear();
                                    _PackingMTRLList.Clear();

                                    _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATAs.Add(new BR_BRS_SEL_ProductionOrder_Component_PickingInfo.INDATA
                                    {
                                        POID = _mainWnd.CurrentOrder.ProductionOrderID,
                                        OPSGGUID = _mainWnd.CurrentOrder.OrderProcessSegmentID,
                                        CHGSEQ = Convert.ToDecimal(_mainWnd.CurrentInstruction.Raw.EXPRESSION),
                                        MTRLID = _mainWnd.CurrentInstruction.Raw.BOMID
                                    });

                                    if (await _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.Execute() == true && _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.Count > 0)
                                    {
                                        var mtrllist = _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.GroupBy(id => id.MTRLID).Select(x => x.First());

                                        foreach (var items in mtrllist)
                                        {
                                            var temp_packinginfo = _BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATAs.Where(x => x.MTRLID.Equals(items.MTRLID));
                                            var temp_rollinfo = new ObservableCollection<RollWeightInfo>();

                                            foreach (BR_BRS_SEL_ProductionOrder_Component_PickingInfo.OUTDATA item in temp_packinginfo)
                                            {
                                                temp_rollinfo.Add(new RollWeightInfo
                                                {
                                                    MLOTID = item.MLOTID,
                                                    MSUBLOTBCD = item.MSUBLOTBCD,
                                                    WEIGHT = 0m,
                                                    LENGTH = 0m,
                                                    MSUBLOTID = item.MSUBLOTID,
                                                    MSUBLOTVER = item.MSUBLOTVER != null ? Convert.ToDecimal(item.MSUBLOTVER) : 0m
                                                });
                                            }

                                            _PackingMTRLList.Add(new PackingMTRLInfo
                                            {
                                                MTRLID = items.MTRLID,
                                                MTRLNAME = string.Format("({0}){1}",items.MTRLID,items.MTRLNAME),
                                                RollCount = 0m,
                                                TotalLength = 0m,
                                                RollInfo = temp_rollinfo
                                            });
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
                            }
                        }
                    }, arg =>
                    {
                        return CommandCanExecutes.ContainsKey("LoadedCommandAsync") ?
                            CommandCanExecutes["LoadedCommandAsync"] : (CommandCanExecutes["LoadedCommandAsync"] = true);
                    });
            }
        }

        public ICommand LoadRollInfoCommand
        {
            get
            {
                return new CommandBase(arg =>
                {

                    try
                    {
                        CommandCanExecutes["LoadRollInfoCommand"] = false;
                        CommandResults["LoadRollInfoCommand"] = false;

                        ///
                        if (arg != null && arg is PackingMTRLInfo)
                        {
                            var selectedMTRL = arg as PackingMTRLInfo;

                            Param = selectedMTRL.Param;
                            RollCount = selectedMTRL.RollCount;
                            TotalLength = selectedMTRL.TotalLength;
                            RollWeightList = selectedMTRL.RollInfo;
                            MSUBLOTBCD = "";
                        }
                        ///

                        CommandResults["LoadRollInfoCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        CommandResults["LoadRollInfoCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["LoadRollInfoCommand"] = true;
                    }
                    
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadRollInfoCommand") ?
                        CommandCanExecutes["LoadRollInfoCommand"] : (CommandCanExecutes["LoadRollInfoCommand"] = true);
                });
            }
        }

        public ICommand InsertRollWeightCommand
        {
            get
            {
                return new CommandBase(arg =>
                {

                    try
                    {
                        CommandCanExecutes["InsertRollWeightCommand"] = false;
                        CommandResults["InsertRollWeightCommand"] = false;

                        ///
                        _mainWnd.Busy.IsBusy = true;

                        var popup = new 출고포장자재환산popup();
                        popup.DataContext = this;
                        popup.Msublotid = _mainWnd.txtMSUBLOTBCD.Text;

                        popup.Show();

                        ///

                        CommandResults["InsertRollWeightCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        _mainWnd.Busy.IsBusy = false;
                        CommandResults["InsertRollWeightCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["InsertRollWeightCommand"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("InsertRollWeightCommand") ?
                        CommandCanExecutes["InsertRollWeightCommand"] : (CommandCanExecutes["InsertRollWeightCommand"] = true);
                });
            }
        }

        public ICommand UpdateRollWeightCommand
        {
            get
            {
                return new CommandBase(arg =>
                {

                    try
                    {
                        CommandCanExecutes["UpdateRollWeightCommand"] = false;
                        CommandResults["UpdateRollWeightCommand"] = false;

                        ///
                        _mainWnd.Busy.IsBusy = true;

                        var popup = new 출고포장자재환산popup();
                        popup.DataContext = this;
                        popup.Msublotid = (_mainWnd.gridPackingWeight.SelectedItem as RollWeightInfo).MSUBLOTBCD;

                        popup.Show();

                        ///

                        CommandResults["UpdateRollWeightCommand"] = true;
                    }
                    catch (Exception ex)
                    {
                        _mainWnd.Busy.IsBusy = false;
                        CommandResults["UpdateRollWeightCommand"] = false;
                        OnException(ex.Message, ex);
                    }
                    finally
                    {
                        CommandCanExecutes["UpdateRollWeightCommand"] = true;
                    }

                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("UpdateRollWeightCommand") ?
                        CommandCanExecutes["UpdateRollWeightCommand"] : (CommandCanExecutes["UpdateRollWeightCommand"] = true);
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
                            CommandCanExecutes["ConfirmCommandAsync"] = false;
                            CommandResults["ConfirmCommandAsync"] = false;

                            ///                            

                            if (CheckMTRLInfo())
                            {
                                
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
                                    "출고포장자재환산",
                                    "출고포장자재환산",
                                    false,
                                    "OM_ProductionOrder_SUI",
                                    "", null, null) == false)
                                {
                                    throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                }

                                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATAs.Clear();
                                _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOTs.Clear();

                                foreach (var items in _PackingMTRLList)
                                {
                                    foreach (var item in items.RollInfo)
                                    {
                                        _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATAs.Add(new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA
                                        {
                                            MTATID = "TOTALWEIGHT",
                                            MTATVAL1 = item.WEIGHT.ToString("0.##0"), // g
                                            MTATVAL2 = null
                                        });

                                        _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOTs.Add(new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.INDATA_SUBLOT
                                        {
                                            MSUBLOTID = item.MSUBLOTID,
                                            MSUBLOTVERFROM = item.MSUBLOTVER.ToString()
                                        });
                                    }
                                }

                                if (await _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi.Execute() == false) return;
                                    
                                var ds = new DataSet();
                                var dt = new DataTable("DATA");

                                ds.Tables.Add(dt);

                                dt.Columns.Add(new DataColumn("MTRLNAME"));
                                dt.Columns.Add(new DataColumn("PARAM"));
                                dt.Columns.Add(new DataColumn("TOTALLENGTH"));
                                dt.Columns.Add(new DataColumn("MLOTID"));
                                dt.Columns.Add(new DataColumn("WEIGHT"));
                                dt.Columns.Add(new DataColumn("LENGTH"));

                                foreach (var items in _PackingMTRLList)
                                {
                                    foreach (var item in items.RollInfo)
                                    {
                                        var row = dt.NewRow();
                                        row["MTRLNAME"] = items.MTRLNAME.Substring(items.MTRLNAME.IndexOf(")") + 1) != null ? items.MTRLNAME.Substring(items.MTRLNAME.IndexOf(")") + 1) : "";
                                        row["PARAM"] = items.Param.ToString("0.##0");
                                        row["TOTALLENGTH"] = items.TotalLength.ToString("0.##0m");
                                        row["MLOTID"] = string.IsNullOrWhiteSpace(item.MSUBLOTID) ? "" : item.MSUBLOTID;
                                        row["WEIGHT"] = item.WEIGHT.ToString("0.##0g");
                                        row["LENGTH"] = item.LENGTH.ToString("0.##0m");

                                        dt.Rows.Add(row);
                                    }
                                }

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

        #region [Constructor]
        public 출고포장자재환산ViewModel()
        {
            _PackingMTRLList = new ObservableCollection<PackingMTRLInfo>();
            _RollWeightList = new ObservableCollection<RollWeightInfo>();
            _BR_BRS_SEL_ProductionOrder_Component_PickingInfo = new BR_BRS_SEL_ProductionOrder_Component_PickingInfo();
            _BR_PHR_INS_MaterialSubLotCustomAttributes_Multi = new BR_PHR_INS_MaterialSubLotCustomAttributes_Multi();
        }
        #endregion

        #region [User Defined]
        public class RollWeightInfo : ViewModelBase
        {
            private string _MLOTID;
            public string MLOTID
            {
                get { return _MLOTID; }
                set
                {
                    _MLOTID = value;
                    NotifyPropertyChanged();
                }
            }

            private string _MSUBLOTBCD;
            public string MSUBLOTBCD
            {
                get { return _MSUBLOTBCD; }
                set
                {
                    _MSUBLOTBCD = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _WEIGHT;
            public decimal WEIGHT
            {
                get { return _WEIGHT; }
                set
                {
                    _WEIGHT = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _LENGTH;
            public decimal LENGTH
            {
                get { return _LENGTH; }
                set
                {
                    _LENGTH = value;
                    NotifyPropertyChanged();
                }
            }

            private string _MSUBLOTID;
            public string MSUBLOTID
            {
                get { return _MSUBLOTID; }
                set
                {
                    _MSUBLOTID = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _MSUBLOTVER;
            public decimal MSUBLOTVER
            {
                get { return _MSUBLOTVER; }
                set
                {
                    _MSUBLOTVER = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public class PackingMTRLInfo : ViewModelBase
        {
            private string _MTRLID;
            public string MTRLID
            {
                get { return _MTRLID; }
                set
                {
                    _MTRLID = value;
                    NotifyPropertyChanged();
                }
            }

            private string _MTRLNAME;
            public string MTRLNAME
            {
                get { return _MTRLNAME; }
                set
                {
                    _MTRLNAME = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _Param;
            public decimal Param
            {
                get { return _Param; }
                set
                {
                    _Param = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _RollCount;
            public decimal RollCount
            {
                get { return _RollCount; }
                set
                {
                    _RollCount = value;
                    NotifyPropertyChanged();
                }
            }

            private decimal _TotalLength;
            public decimal TotalLength
            {
                get { return _TotalLength; }
                set
                {
                    _TotalLength = value;
                    NotifyPropertyChanged();
                }
            }

            private ObservableCollection<RollWeightInfo> _RollInfo;
            public ObservableCollection<RollWeightInfo> RollInfo
            {
                get { return _RollInfo; }
                set
                {
                    _RollInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void ConvertWeight()
        {
            if (Param > 0)
            {
                this.TotalLength = 0m;

                foreach (var item in RollWeightList)
                {
                    item.LENGTH = Math.Round(item.WEIGHT / Param ,3);
                    this.TotalLength += item.LENGTH;
                }

                this.RollCount = RollWeightList.Where(x => x.LENGTH > 0).ToList().Count;

                SaveData();
            }
            else
            {
                C1.Silverlight.C1MessageBox.Show("변수(1 roll의 질량)을 입력하세요");
                SaveData();
                _mainWnd.Param.Focus();
            }

            this._mainWnd.Busy.IsBusy = false;
        }
        public void SaveData()
        {
            var temp = _mainWnd.ListPackingMTRL.SelectedItem as PackingMTRLInfo;

            temp.Param = this._Param;
            temp.RollCount = this._RollCount;
            temp.TotalLength = this._TotalLength;
            temp.RollInfo = this.RollWeightList;

            this._mainWnd.Busy.IsBusy = false;
        }

        public bool CheckMTRLInfo()
        {
            foreach (var items in _PackingMTRLList)
            {
                if (items.TotalLength <= 0)
                    return false;

                foreach (var item in items.RollInfo)
                {
                    if (item.LENGTH <= 0)
                        return false;
                }
            }

            return true;
        }
        
        #endregion
    }
}
