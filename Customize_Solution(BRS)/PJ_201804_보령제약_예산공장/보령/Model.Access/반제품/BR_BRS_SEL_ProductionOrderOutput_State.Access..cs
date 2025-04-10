using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{

    /// <summary>
    /// summary of BR_BRS_SEL_ProductionOrderOutput_State
    /// </summary>
    public partial class BR_BRS_SEL_ProductionOrderOutput_State : BizActorRuleBase
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
            private string _OPSGNAME;
            [BizActorOutputItemAttribute()]
            public string OPSGNAME
            {
                get
                {
                    return this._OPSGNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OPSGNAME = value;
                        this.CheckIsOriginal("OPSGNAME", value);
                        this.OnPropertyChanged("OPSGNAME");
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
            private string _VESSELNAME;
            [BizActorOutputItemAttribute()]
            public string VESSELNAME
            {
                get
                {
                    return this._VESSELNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._VESSELNAME = value;
                        this.CheckIsOriginal("VESSELNAME", value);
                        this.OnPropertyChanged("VESSELNAME");
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
            private string _STRGSTANDARD;
            [BizActorOutputItemAttribute()]
            public string STRGSTANDARD
            {
                get
                {
                    return this._STRGSTANDARD;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._STRGSTANDARD = value;
                        this.CheckIsOriginal("STRGSTANDARD", value);
                        this.OnPropertyChanged("STRGSTANDARD");
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
            private string _STRGDATE;
            [BizActorOutputItemAttribute()]
            public string STRGDATE
            {
                get
                {
                    return this._STRGDATE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._STRGDATE = value;
                        this.CheckIsOriginal("STRGDATE", value);
                        this.OnPropertyChanged("STRGDATE");
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
            private string _DEVIATIONYN;
            [BizActorOutputItemAttribute()]
            public string DEVIATIONYN
            {
                get
                {
                    return this._DEVIATIONYN;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DEVIATIONYN = value;
                        this.CheckIsOriginal("DEVIATIONYN", value);
                        this.OnPropertyChanged("DEVIATIONYN");
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
            private string _MLOTID;
            [BizActorOutputItemAttribute()]
            public string MLOTID
            {
                get
                {
                    return this._MLOTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MLOTID = value;
                        this.CheckIsOriginal("MLOTID", value);
                        this.OnPropertyChanged("MLOTID");
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
            private string _MTRLID;
            [BizActorOutputItemAttribute()]
            public string MTRLID
            {
                get
                {
                    return this._MTRLID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MTRLID = value;
                        this.CheckIsOriginal("MTRLID", value);
                        this.OnPropertyChanged("MTRLID");
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
            private System.Nullable<decimal> _MLOTVER;
            [BizActorOutputItemAttribute()]
            public System.Nullable<decimal> MLOTVER
            {
                get
                {
                    return this._MLOTVER;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MLOTVER = value;
                        this.CheckIsOriginal("MLOTVER", value);
                        this.OnPropertyChanged("MLOTVER");
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
            private string _MSUBLOTID;
            [BizActorOutputItemAttribute()]
            public string MSUBLOTID
            {
                get
                {
                    return this._MSUBLOTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTID = value;
                        this.CheckIsOriginal("MSUBLOTID", value);
                        this.OnPropertyChanged("MSUBLOTID");
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
            private System.Nullable<decimal> _MSUBLOTVER;
            [BizActorOutputItemAttribute()]
            public System.Nullable<decimal> MSUBLOTVER
            {
                get
                {
                    return this._MSUBLOTVER;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTVER = value;
                        this.CheckIsOriginal("MSUBLOTVER", value);
                        this.OnPropertyChanged("MSUBLOTVER");
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
            private System.Nullable<decimal> _MSUBLOTSEQ;
            [BizActorOutputItemAttribute()]
            public System.Nullable<decimal> MSUBLOTSEQ
            {
                get
                {
                    return this._MSUBLOTSEQ;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTSEQ = value;
                        this.CheckIsOriginal("MSUBLOTSEQ", value);
                        this.OnPropertyChanged("MSUBLOTSEQ");
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
            private string _MSUBLOTSTAT;
            [BizActorOutputItemAttribute()]
            public string MSUBLOTSTAT
            {
                get
                {
                    return this._MSUBLOTSTAT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTSTAT = value;
                        this.CheckIsOriginal("MSUBLOTSTAT", value);
                        this.OnPropertyChanged("MSUBLOTSTAT");
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
            private System.Nullable<System.DateTime> _EXPIRYDTTM;
            [BizActorOutputItemAttribute()]
            public System.Nullable<System.DateTime> EXPIRYDTTM
            {
                get
                {
                    return this._EXPIRYDTTM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EXPIRYDTTM = value;
                        this.CheckIsOriginal("EXPIRYDTTM", value);
                        this.OnPropertyChanged("EXPIRYDTTM");
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
        public BR_BRS_SEL_ProductionOrderOutput_State()
        {
            RuleName = "BR_BRS_SEL_ProductionOrderOutput_State";
            BizName = "BR_BRS_SEL_ProductionOrderOutput_State";
            _INDATAs = new INDATACollection();
            _OUTDATAs = new OUTDATACollection();
        }
    }
}
