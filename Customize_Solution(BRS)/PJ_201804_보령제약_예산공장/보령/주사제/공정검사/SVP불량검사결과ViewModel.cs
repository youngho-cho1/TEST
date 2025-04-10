using LGCNS.iPharmMES.Common;
using Order;
using System;
using System.Windows.Input;
using System.Linq;
using ShopFloorUI;
using System.Text;
using C1.Silverlight.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using C1.Silverlight.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    public class SVP불량검사결과ViewModel : ViewModelBase
    {
        #region Properties
        public SVP불량검사결과ViewModel()
        {
            _RejectionDetails = new RejectionDetail.OUTDATACollection();
            _RejectionSummarys = new ObservableCollection<RejectionSummary>();
        }

        SVP불량검사결과 _mainWnd;

        private RejectionDetail.OUTDATACollection _RejectionDetails;
        public RejectionDetail.OUTDATACollection RejectionDetails
        {
            get { return _RejectionDetails; }
            set
            {
                _RejectionDetails = value;
                OnPropertyChanged("RejectionDetails");
            }
        }
        private ObservableCollection<RejectionSummary> _RejectionSummarys;
        public ObservableCollection<RejectionSummary> RejectionSummarys
        {
            get { return _RejectionSummarys; }
            set
            {
                _RejectionSummarys = value;
                OnPropertyChanged("RejectionSummarys");
            }
        }
        private string _INPUTQTY;
        public string INPUTQTY
        {
            get { return _INPUTQTY; }
            set
            {
                _INPUTQTY = value;
                OnPropertyChanged("INPUTQTY");
            }
        }
        private string _REJECTIONSUM;
        public string REJECTIONSUM
        {
            get { return _REJECTIONSUM; }
            set
            {
                _REJECTIONSUM = value;
                OnPropertyChanged("REJECTIONSUM");
            }
        }
        private string _REJECTIONRATIO;
        public string REJECTIONRATIO
        {
            get { return _REJECTIONRATIO; }
            set
            {
                _REJECTIONRATIO = value;
                OnPropertyChanged("REJECTIONRATIO");
            }
        }
        private string _GOODQTY;
        public string GOODQTY
        {
            get { return _GOODQTY; }
            set
            {
                _GOODQTY = value;
                OnPropertyChanged("GOODQTY");
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

                            if (arg != null && arg is SVP불량검사결과)
                            {
                                _mainWnd = arg as SVP불량검사결과;

                                IsBusy = true;

                                RejectionDetails.Clear();

                                // SUMMARY INITIALIZE
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "섬유이물",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "흰이물",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "검은이물",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "변색",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "파손",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "용기흠",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "캡핑불량",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _RejectionSummarys.Add(new RejectionSummary
                                {
                                    REJECTIONTYPE = "기타",
                                    REJECTIONQTY = 0,
                                    REJECTIONRATIO = 0m
                                });
                                _mainWnd.RejectionSummary.Refresh();
                                // 

                            }
                            IsBusy = false;

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
                            int fiber = 0, white = 0, black = 0, discolor = 0, broken = 0, crack = 0, caprej = 0, other = 0;

                            foreach (RejectionDetail.OUTDATA item in RejectionDetails)
                            {
                                fiber += item.FIBER;
                                white += item.WHITE;
                                black += item.BLACK;
                                discolor += item.DISCOLOR;
                                broken += item.BROKEN;
                                crack += item.CRACK;
                                caprej += item.CAPREJ;
                                other += item.OTHER;
                            }

                            decimal total = fiber + white + black + discolor + broken + crack + caprej + other;
                            REJECTIONSUM = total.ToString();

                            foreach (var item in RejectionSummarys)
                            {
                                switch (item.REJECTIONTYPE)
                                {
                                    case "섬유이물":
                                        item.REJECTIONQTY = fiber;
                                        item.REJECTIONRATIO = Math.Round(fiber / total * 100, 1);
                                    break;
                                    case "흰이물":
                                        item.REJECTIONQTY = white;
                                        item.REJECTIONRATIO = Math.Round(white / total * 100);
                                        break;
                                    case "검은이물":
                                        item.REJECTIONQTY = black;
                                        item.REJECTIONRATIO = Math.Round(black / total * 100);
                                        break;
                                    case "변색":
                                        item.REJECTIONQTY = discolor;
                                        item.REJECTIONRATIO = Math.Round(discolor / total * 100);
                                        break;
                                    case "파손":
                                        item.REJECTIONQTY = broken;
                                        item.REJECTIONRATIO = Math.Round(broken / total * 100);
                                        break;
                                    case "용기흠":
                                        item.REJECTIONQTY = crack;
                                        item.REJECTIONRATIO = Math.Round(crack / total * 100);
                                        break;
                                    case "캡핑불량":
                                        item.REJECTIONQTY = caprej;
                                        item.REJECTIONRATIO = Math.Round(caprej / total * 100);
                                        break;
                                    case "기타":
                                        item.REJECTIONQTY = other;
                                        item.REJECTIONRATIO = Math.Round(other / total * 100);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            _mainWnd.RejectionSummary.Refresh();

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

                            var outputValues = InstructionModel.GetResultReceiver(_mainWnd.CurrentInstruction, _mainWnd.Instructions);

                            authHelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_ProductionOrder_SUI");

                            if (await authHelper.ClickAsync(
                                Common.enumCertificationType.Function,
                                Common.enumAccessType.Create,
                                "SVP불량검사결과",
                                "SVP불량검사결과",
                                false,
                                "OM_ProductionOrder_SUI",
                                "", null, null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            // DataSet 생성
                            var ds = new DataSet();

                            // DETAIL TBL
                            var dt = new DataTable("DATA1");
                            ds.Tables.Add(dt);

                            dt.Columns.Add(new DataColumn("검사자"));
                            dt.Columns.Add(new DataColumn("검사일자"));
                            dt.Columns.Add(new DataColumn("섬유이물"));
                            dt.Columns.Add(new DataColumn("흰이물"));
                            dt.Columns.Add(new DataColumn("검은이물"));
                            dt.Columns.Add(new DataColumn("변색"));
                            dt.Columns.Add(new DataColumn("파손"));
                            dt.Columns.Add(new DataColumn("용기흠"));
                            dt.Columns.Add(new DataColumn("캡핑불량"));
                            dt.Columns.Add(new DataColumn("기타"));

                            foreach (var item in RejectionDetails)
                            {
                                var row = dt.NewRow();

                                row["검사자"] = item.INSUSER ?? "";
                                row["검사일자"] = item.INSDTTM != null ? item.INSDTTM.ToString("yyyy-MM-dd") : "N/A";
                                row["섬유이물"] = item.FIBER;
                                row["흰이물"] = item.WHITE;
                                row["검은이물"] = item.BLACK;
                                row["변색"] = item.DISCOLOR;
                                row["파손"] = item.BROKEN ;
                                row["용기흠"] = item.CRACK;
                                row["캡핑불량"] = item.CAPREJ;
                                row["기타"] = item.OTHER;

                                dt.Rows.Add(row);
                            }

                            // SUMMARY TBL
                            var dt1 = new DataTable("DATA2");
                            ds.Tables.Add(dt1);

                            dt1.Columns.Add(new DataColumn("불량유형"));
                            dt1.Columns.Add(new DataColumn("유형별불량합계"));
                            dt1.Columns.Add(new DataColumn("유형별불량율"));

                            foreach (var item in RejectionSummarys)
                            {
                                var row1 = dt1.NewRow();

                                row1["불량유형"] = item.REJECTIONTYPE ?? "N/A";
                                row1["유형별불량합계"] = item.REJECTIONQTY;
                                row1["유형별불량율"] = item.REJECTIONRATIO;                          

                                dt1.Rows.Add(row1);
                            }

                            // TBL
                            var dt2 = new DataTable("DATA3");
                            ds.Tables.Add(dt2);

                            dt2.Columns.Add(new DataColumn("전공정인수량"));
                            dt2.Columns.Add(new DataColumn("불량총합계"));
                            dt2.Columns.Add(new DataColumn("전체불량률"));
                            dt2.Columns.Add(new DataColumn("정상제품수량"));

                            var row2 = dt2.NewRow();
                            row2["전공정인수량"] = INPUTQTY ?? "N/A";
                            row2["불량총합계"] = REJECTIONSUM ?? "N/A";
                            row2["전체불량률"] = REJECTIONRATIO ?? "N/A";
                            row2["정상제품수량"] = GOODQTY ?? "N/A";
                            dt2.Rows.Add(row2);

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
        public class RejectionDetail : BizActorRuleBase
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
                private string _INSUSER;
                [BizActorOutputItemAttribute()]
                public string INSUSER
                {
                    get
                    {
                        return this._INSUSER;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._INSUSER = value;
                            this.CheckIsOriginal("INSUSER", value);
                            this.OnPropertyChanged("INSUSER");
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
                private int _FIBER = 0;
                [BizActorOutputItemAttribute()]
                public int FIBER
                {
                    get
                    {
                        return this._FIBER;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._FIBER = value;
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
                private int _WHITE = 0;
                [BizActorOutputItemAttribute()]
                public int WHITE
                {
                    get
                    {
                        return this._WHITE;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._WHITE = value;
                            this.CheckIsOriginal("WHITE", value);
                            this.OnPropertyChanged("WHITE");
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
                private int _BLACK = 0;
                [BizActorOutputItemAttribute()]
                public int BLACK
                {
                    get
                    {
                        return this._BLACK;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._BLACK = value;
                            this.CheckIsOriginal("BLACK", value);
                            this.OnPropertyChanged("BLACK");
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
                private int _DISCOLOR = 0;
                [BizActorOutputItemAttribute()]
                public int DISCOLOR
                {
                    get
                    {
                        return this._DISCOLOR;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._DISCOLOR = value;
                            this.CheckIsOriginal("DISCOLOR", value);
                            this.OnPropertyChanged("DISCOLOR");
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
                private int _BROKEN = 0;
                [BizActorOutputItemAttribute()]
                public int BROKEN
                {
                    get
                    {
                        return this._BROKEN;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._BROKEN = value;
                            this.CheckIsOriginal("BROKEN", value);
                            this.OnPropertyChanged("BROKEN");
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
                private int _CRACK = 0;
                [BizActorOutputItemAttribute()]
                public int CRACK
                {
                    get
                    {
                        return this._CRACK;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._CRACK = value;
                            this.CheckIsOriginal("CRACK", value);
                            this.OnPropertyChanged("CRACK");
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
                private int _CAPREJ = 0;
                [BizActorOutputItemAttribute()]
                public int CAPREJ
                {
                    get
                    {
                        return this._CAPREJ;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._CAPREJ = value;
                            this.CheckIsOriginal("CAPREJ", value);
                            this.OnPropertyChanged("CAPREJ");
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
                private int _OTHER = 0;
                [BizActorOutputItemAttribute()]
                public int OTHER
                {
                    get
                    {
                        return this._OTHER;
                    }
                    set
                    {
                        if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                        {
                        }
                        else
                        {
                            this._OTHER = value;
                            this.CheckIsOriginal("BLACK", value);
                            this.OnPropertyChanged("BLACK");
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
            public RejectionDetail()
            {
                _OUTDATAs = new OUTDATACollection();
            }
        }
        public class RejectionSummary : ViewModelBase
        {             
            private string _REJECTIONTYPE;
            public string REJECTIONTYPE
            {
                get { return _REJECTIONTYPE; }
                set
                {
                    _REJECTIONTYPE = value;
                    OnPropertyChanged("REJECTIONTYPE");
                }
            }
            private int _REJECTIONQTY;
            public int REJECTIONQTY
            {
                get { return _REJECTIONQTY; }
                set
                {
                    _REJECTIONQTY = value;
                    OnPropertyChanged("REJECTIONQTY");
                }
            }
            private decimal _REJECTIONRATIO;
            public decimal REJECTIONRATIO
            {
                get { return _REJECTIONRATIO; }
                set
                {
                    _REJECTIONRATIO = value;
                    OnPropertyChanged("REJECTIONRATIO");
                }
            }
        }
        #endregion
    }
}


