using C1.Silverlight.Data;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class SVP캡핑IPCViewModel : ViewModelBase
    {
        #region Properties
        public SVP캡핑IPCViewModel()
        {
            _IPCHistorys = new IPCResult.OUTDATACollection();
        }

        SVP캡핑IPC _mainWnd;

        private IPCResult.OUTDATACollection _IPCHistorys;
        public IPCResult.OUTDATACollection IPCHistorys
        {
            get { return _IPCHistorys; }
            set
            {
                _IPCHistorys = value;
                OnPropertyChanged("IPCHistorys");
            }
        }
        private List<TimeCombobox> _COMBOItems;
        public List<TimeCombobox> COMBOItems
        {
            get { return _COMBOItems; }
            set
            {
                _COMBOItems = value;
                OnPropertyChanged("COMBOItems");
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

                            if (arg != null && arg is SVP캡핑IPC)
                            {
                                _mainWnd = arg as SVP캡핑IPC;

                                IsBusy = true;

                                COMBOItems = SetComboBox(_COMBOItems);
                            }

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
        public ICommand RowEditCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RowEditCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["RowEditCommand"] = false;
                            CommandCanExecutes["RowEditCommand"] = false;

                            IsBusy = true;
                            ///
                            ///
                            IsBusy = false;

                            CommandResults["RowEditCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RowEditCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RowEditCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RowEditCommand") ?
                        CommandCanExecutes["RowEditCommand"] : (CommandCanExecutes["RowEditCommand"] = true);
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

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "SVP캡핑IPC",
                                "SVP캡핑IPC",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // DataSet 생성
                            var ds = new DataSet();

                            // DETAIL TBL
                            var dt = new DataTable("DATA");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("구분"));
                            dt.Columns.Add(new DataColumn("시각"));
                            dt.Columns.Add(new DataColumn("Vial1캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial1LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial2캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial2LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial3캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial3LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial4캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial4LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial5캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial5LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial6캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial6LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial7캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial7LeakTest"));
                            dt.Columns.Add(new DataColumn("Vial8캡핑상태"));
                            dt.Columns.Add(new DataColumn("Vial8LeakTest"));

                            foreach (var item in IPCHistorys)
                            {
                                var row = dt.NewRow();

                                row["구분"] = item.TYPE ?? "";
                                row["시각"] = item.INSDTTM != null ? item.INSDTTM.ToString("yyyy-MM-dd HH:mm:ss") : "";
                                row["Vial1캡핑상태"] = item.VIAL1STATE == "Y" ? "적합" : "부적합";
                                row["Vial1LeakTest"] = item.VIAL1RSLT.ToString("0.##0");
                                row["Vial2캡핑상태"] = item.VIAL2STATE == "Y" ? "적합" : "부적합";
                                row["Vial2LeakTest"] = item.VIAL2RSLT.ToString("0.##0");
                                row["Vial3캡핑상태"] = item.VIAL3STATE == "Y" ? "적합" : "부적합";
                                row["Vial3LeakTest"] = item.VIAL3RSLT.ToString("0.##0");
                                row["Vial4캡핑상태"] = item.VIAL4STATE == "Y" ? "적합" : "부적합";
                                row["Vial4LeakTest"] = item.VIAL4RSLT.ToString("0.##0");
                                row["Vial5캡핑상태"] = item.VIAL5STATE == "Y" ? "적합" : "부적합";
                                row["Vial5LeakTest"] = item.VIAL5RSLT.ToString("0.##0");
                                row["Vial6캡핑상태"] = item.VIAL6STATE == "Y" ? "적합" : "부적합";
                                row["Vial6LeakTest"] = item.VIAL6RSLT.ToString("0.##0");
                                row["Vial7캡핑상태"] = item.VIAL7STATE == "Y" ? "적합" : "부적합";
                                row["Vial7LeakTest"] = item.VIAL7RSLT.ToString("0.##0");
                                row["Vial8캡핑상태"] = item.VIAL8STATE == "Y" ? "적합" : "부적합";
                                row["Vial8LeakTest"] = item.VIAL8RSLT.ToString("0.##0");

                                dt.Rows.Add(row);
                            }

                            var xml = BizActorRuleBase.CreateXMLStream(ds);
                            var bytesArray = System.Text.Encoding.UTF8.GetBytes(xml);


                            _mainWnd.CurrentInstruction.Raw.ACTVAL = _mainWnd.TableTypeName;
                            _mainWnd.CurrentInstruction.Raw.NOTE = bytesArray;

                            var result = await _mainWnd.Phase.RegistInstructionValue(_mainWnd.CurrentInstruction, false, false, true);
                            if (result != enumInstructionRegistErrorType.Ok)
                            {
                                throw new Exception(string.Format("값 등록 실패, ID={0}, 사유={1}", _mainWnd.CurrentInstruction.Raw.IRTGUID, result));
                            }

                            if (_mainWnd.Dispatcher.CheckAccess()) _mainWnd.DialogResult = true;
                            else _mainWnd.Dispatcher.BeginInvoke(() => _mainWnd.DialogResult = true);


                            IsBusy = false;
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
        private List<TimeCombobox> SetComboBox(List<TimeCombobox> cur)
        {
            cur = cur ?? new List<TimeCombobox>();
            cur.Clear();

            cur.Add(new TimeCombobox
            {
                KEY = "BEGIN",
                NAME = "초기"
            });
            cur.Add(new TimeCombobox
            {
                KEY = "MID",
                NAME = "중기"
            });
            cur.Add(new TimeCombobox
            {
                KEY = "END",
                NAME = "말기"
            });
            cur.Add(new TimeCombobox
            {
                KEY = "RESTART",
                NAME = "재시작"
            });

            return cur;
        }
        public class TimeCombobox
        {
            private string _KEY;
            public string KEY
            {
                get { return _KEY; }
                set { _KEY = value; }
            }
            private string _NAME;
            public string NAME
            {
                get { return _NAME; }
                set { _NAME = value; }
            }
        }
        public class IPCResult : BizActorRuleBase
        {
            public sealed partial class OUTDATACollection : BufferedObservableCollection<OUTDATA>
            {
            }
            private OUTDATACollection _OUTDATAs;
            [BizActorOutputSetAttribute()]
            public OUTDATACollection OUTDATAs
            {
                get
                {
                    return this._OUTDATAs;
                }
            }
            [BizActorOutputSetDefineAttribute(Order = "0")]
            [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
            public partial class OUTDATA : BizActorDataSetBase
            {
                public OUTDATA()
                {
                    RowLoadedFlag = false;
                }
                private bool _RowLoadedFlag;
                public bool RowLoadedFlag
                {
                    get
                    {
                        return this._RowLoadedFlag;
                    }
                    set
                    {
                        this._RowLoadedFlag = value;
                        this.OnPropertyChanged("_RowLoadedFlag");
                    }
                }
                private string _RowIndex;
                public string RowIndex
                {
                    get
                    {
                        return this._RowIndex;
                    }
                    set
                    {
                        this._RowIndex = value;
                        this.OnPropertyChanged("RowIndex");
                    }
                }
                private string _RowEditSec;
                public string RowEditSec
                {
                    get
                    {
                        return this._RowEditSec;
                    }
                    set
                    {
                        this._RowEditSec = value;
                        this.OnPropertyChanged("RowEditSec");
                    }
                }
                private string _TYPE;
                [BizActorOutputItemAttribute()]
                public string TYPE
                {
                    get
                    {
                        return this._TYPE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._TYPE = value;
                            this.CheckIsOriginal("TYPE", value);
                            this.OnPropertyChanged("TYPE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private DateTime _INSDTTM = DateTime.Now;
                [BizActorOutputItemAttribute()]
                public DateTime INSDTTM
                {
                    get
                    {
                        return this._INSDTTM;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._INSDTTM = value;
                            this.CheckIsOriginal("INSDTTM", value);
                            this.OnPropertyChanged("INSDTTM");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
               
                private string _VIAL1STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL1STATE
                {
                    get
                    {
                        return this._VIAL1STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL1STATE = value;
                            this.CheckIsOriginal("VIAL1STATE", value);
                            this.OnPropertyChanged("VIAL1STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL1RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL1RSLT
                {
                    get
                    {
                        return this._VIAL1RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL1RSLT = value;
                            this.CheckIsOriginal("VIAL1RSLT", value);
                            this.OnPropertyChanged("VIAL1RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL2STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL2STATE
                {
                    get
                    {
                        return this._VIAL2STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL2STATE = value;
                            this.CheckIsOriginal("VIAL2STATE", value);
                            this.OnPropertyChanged("VIAL2STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL2RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL2RSLT
                {
                    get
                    {
                        return this._VIAL2RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL2RSLT = value;
                            this.CheckIsOriginal("VIAL2RSLT", value);
                            this.OnPropertyChanged("VIAL2RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL3STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL3STATE
                {
                    get
                    {
                        return this._VIAL3STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL3STATE = value;
                            this.CheckIsOriginal("VIAL3STATE", value);
                            this.OnPropertyChanged("VIAL3STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL3RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL3RSLT
                {
                    get
                    {
                        return this._VIAL3RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL3RSLT = value;
                            this.CheckIsOriginal("VIAL3RSLT", value);
                            this.OnPropertyChanged("VIAL3RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL4STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL4STATE
                {
                    get
                    {
                        return this._VIAL4STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL4STATE = value;
                            this.CheckIsOriginal("VIAL4STATE", value);
                            this.OnPropertyChanged("VIAL4STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL4RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL4RSLT
                {
                    get
                    {
                        return this._VIAL4RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL4RSLT = value;
                            this.CheckIsOriginal("VIAL4RSLT", value);
                            this.OnPropertyChanged("VIAL4RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL5STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL5STATE
                {
                    get
                    {
                        return this._VIAL5STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL5STATE = value;
                            this.CheckIsOriginal("VIAL5STATE", value);
                            this.OnPropertyChanged("VIAL5STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL5RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL5RSLT
                {
                    get
                    {
                        return this._VIAL5RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL5RSLT = value;
                            this.CheckIsOriginal("VIAL5RSLT", value);
                            this.OnPropertyChanged("VIAL5RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL6STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL6STATE
                {
                    get
                    {
                        return this._VIAL6STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL6STATE = value;
                            this.CheckIsOriginal("VIAL6STATE", value);
                            this.OnPropertyChanged("VIAL6STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL6RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL6RSLT
                {
                    get
                    {
                        return this._VIAL6RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL6RSLT = value;
                            this.CheckIsOriginal("VIAL6RSLT", value);
                            this.OnPropertyChanged("VIAL6RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL7STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL7STATE
                {
                    get
                    {
                        return this._VIAL7STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL7STATE = value;
                            this.CheckIsOriginal("VIAL7STATE", value);
                            this.OnPropertyChanged("VIAL7STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL7RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL7RSLT
                {
                    get
                    {
                        return this._VIAL7RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL7RSLT = value;
                            this.CheckIsOriginal("VIAL7RSLT", value);
                            this.OnPropertyChanged("VIAL7RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private string _VIAL8STATE = "Y";
                [BizActorOutputItemAttribute()]
                public string VIAL8STATE
                {
                    get
                    {
                        return this._VIAL8STATE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL8STATE = value;
                            this.CheckIsOriginal("VIAL8STATE", value);
                            this.OnPropertyChanged("VIAL8STATE");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
                private decimal _VIAL8RSLT = 0;
                [BizActorOutputItemAttribute()]
                public decimal VIAL8RSLT
                {
                    get
                    {
                        return this._VIAL8RSLT;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._VIAL8RSLT = value;
                            this.CheckIsOriginal("VIAL8RSLT", value);
                            this.OnPropertyChanged("VIAL8RSLT");
                            if (RowLoadedFlag)
                            {
                                if (this.CheckIsOriginalRow())
                                {
                                    RowEditSec = "SEL";
                                }
                                else
                                {
                                    RowEditSec = "UPD";
                                }
                            }
                        }
                    }
                }
            }
            public IPCResult()
            {
                _OUTDATAs = new OUTDATACollection();
            }
        }
        #endregion
    }
}
