using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{

    /// <summary>
    /// summary of BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT
    /// </summary>
    public partial class BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT : BizActorRuleBase
    {
        public sealed partial class INDATACollection : BufferedObservableCollection<INDATA>
        {
        }
        private INDATACollection _INDATAs;
        [BizActorInputSetAttribute()]
        public INDATACollection INDATAs
        {
            get
            {
                return this._INDATAs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "0")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA : BizActorDataSetBase
        {
            public INDATA()
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
            private string _POID;
            [BizActorInputItemAttribute()]
            public string POID
            {
                get
                {
                    return this._POID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._POID = value;
                        this.CheckIsOriginal("POID", value);
                        this.OnPropertyChanged("POID");
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
            private string _OPSGGUID;
            [BizActorInputItemAttribute()]
            public string OPSGGUID
            {
                get
                {
                    return this._OPSGGUID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OPSGGUID = value;
                        this.CheckIsOriginal("OPSGGUID", value);
                        this.OnPropertyChanged("OPSGGUID");
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
            private string _POID;
            [BizActorOutputItemAttribute()]
            public string POID
            {
                get
                {
                    return this._POID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._POID = value;
                        this.CheckIsOriginal("POID", value);
                        this.OnPropertyChanged("POID");
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
            private string _OPSGGUID;
            [BizActorOutputItemAttribute()]
            public string OPSGGUID
            {
                get
                {
                    return this._OPSGGUID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OPSGGUID = value;
                        this.CheckIsOriginal("OPSGGUID", value);
                        this.OnPropertyChanged("OPSGGUID");
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
            private string _INSPECTIONDTTM;
            [BizActorOutputItemAttribute()]
            public string INSPECTIONDTTM
            {
                get
                {
                    return this._INSPECTIONDTTM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._INSPECTIONDTTM = value;
                        this.CheckIsOriginal("INSPECTIONDTTM", value);
                        this.OnPropertyChanged("INSPECTIONDTTM");
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
            private System.Nullable<decimal> _INSPECTQTY;
            [BizActorOutputItemAttribute()]
            public System.Nullable<decimal> INSPECTQTY
            {
                get
                {
                    return this._INSPECTQTY;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._INSPECTQTY = value;
                        this.CheckIsOriginal("INSPECTQTY", value);
                        this.OnPropertyChanged("INSPECTQTY");
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
            private string _COMMENTS;
            [BizActorOutputItemAttribute()]
            public string COMMENTS
            {
                get
                {
                    return this._COMMENTS;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._COMMENTS = value;
                        this.CheckIsOriginal("COMMENTS", value);
                        this.OnPropertyChanged("COMMENTS");
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
            private string _INSDTTM;
            [BizActorOutputItemAttribute()]
            public string INSDTTM
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
        }
        public BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT()
        {
            RuleName = "BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT";
            BizName = "BR_BRS_SEL_UDT_BRS_SVP_REJECT_SUM_COMMENT";
            _INDATAs = new INDATACollection();
            _OUTDATAs = new OUTDATACollection();
        }
    }
}
