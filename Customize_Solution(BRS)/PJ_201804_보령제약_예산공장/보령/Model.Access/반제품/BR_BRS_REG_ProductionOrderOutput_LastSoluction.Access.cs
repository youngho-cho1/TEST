using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{

    /// <summary>
    /// summary of BR_BRS_REG_ProductionOrderOutput_LastSoluction
    /// </summary>
    public partial class BR_BRS_REG_ProductionOrderOutput_LastSoluction : BizActorRuleBase
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
            private string _WEIGHINGMETHOD;
            [BizActorInputItemAttribute()]
            public string WEIGHINGMETHOD
            {
                get
                {
                    return this._WEIGHINGMETHOD;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WEIGHINGMETHOD = value;
                        this.CheckIsOriginal("WEIGHINGMETHOD", value);
                        this.OnPropertyChanged("WEIGHINGMETHOD");
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
        public sealed partial class INDATA_SOLUTIONCollection : BufferedObservableCollection<INDATA_SOLUTION>
        {
        }
        private INDATA_SOLUTIONCollection _INDATA_SOLUTIONs;
        [BizActorInputSetAttribute()]
        public INDATA_SOLUTIONCollection INDATA_SOLUTIONs
        {
            get
            {
                return this._INDATA_SOLUTIONs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "1")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA_SOLUTION : BizActorDataSetBase
        {
            public INDATA_SOLUTION()
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
            private string _LOTTYPE;
            [BizActorInputItemAttribute()]
            public string LOTTYPE
            {
                get
                {
                    return this._LOTTYPE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._LOTTYPE = value;
                        this.CheckIsOriginal("LOTTYPE", value);
                        this.OnPropertyChanged("LOTTYPE");
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
            private System.Nullable<System.Guid> _OPSGGUID;
            [BizActorInputItemAttribute()]
            public System.Nullable<System.Guid> OPSGGUID
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
            private System.Nullable<System.Guid> _OUTPUTGUID;
            [BizActorInputItemAttribute()]
            public System.Nullable<System.Guid> OUTPUTGUID
            {
                get
                {
                    return this._OUTPUTGUID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OUTPUTGUID = value;
                        this.CheckIsOriginal("OUTPUTGUID", value);
                        this.OnPropertyChanged("OUTPUTGUID");
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
            private string _VESSELID;
            [BizActorInputItemAttribute()]
            public string VESSELID
            {
                get
                {
                    return this._VESSELID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._VESSELID = value;
                        this.CheckIsOriginal("VESSELID", value);
                        this.OnPropertyChanged("VESSELID");
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
            private System.Nullable<decimal> _MSUBLOTQTY;
            [BizActorInputItemAttribute()]
            public System.Nullable<decimal> MSUBLOTQTY
            {
                get
                {
                    return this._MSUBLOTQTY;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTQTY = value;
                        this.CheckIsOriginal("MSUBLOTQTY", value);
                        this.OnPropertyChanged("MSUBLOTQTY");
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
            private System.Nullable<decimal> _TAREWEIGHT;
            [BizActorInputItemAttribute()]
            public System.Nullable<decimal> TAREWEIGHT
            {
                get
                {
                    return this._TAREWEIGHT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._TAREWEIGHT = value;
                        this.CheckIsOriginal("TAREWEIGHT", value);
                        this.OnPropertyChanged("TAREWEIGHT");
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
            private string _TAREUOMID;
            [BizActorInputItemAttribute()]
            public string TAREUOMID
            {
                get
                {
                    return this._TAREUOMID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._TAREUOMID = value;
                        this.CheckIsOriginal("TAREUOMID", value);
                        this.OnPropertyChanged("TAREUOMID");
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
            private string _REASON;
            [BizActorInputItemAttribute()]
            public string REASON
            {
                get
                {
                    return this._REASON;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._REASON = value;
                        this.CheckIsOriginal("REASON", value);
                        this.OnPropertyChanged("REASON");
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
            [BizActorInputItemAttribute()]
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
            private string _IS_NEED_VESSELID;
            [BizActorInputItemAttribute()]
            public string IS_NEED_VESSELID
            {
                get
                {
                    return this._IS_NEED_VESSELID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_NEED_VESSELID = value;
                        this.CheckIsOriginal("IS_NEED_VESSELID", value);
                        this.OnPropertyChanged("IS_NEED_VESSELID");
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
            private string _IS_ONLY_TARE;
            [BizActorInputItemAttribute()]
            public string IS_ONLY_TARE
            {
                get
                {
                    return this._IS_ONLY_TARE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_ONLY_TARE = value;
                        this.CheckIsOriginal("IS_ONLY_TARE", value);
                        this.OnPropertyChanged("IS_ONLY_TARE");
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
            private string _MSUBLOTBCD;
            [BizActorInputItemAttribute()]
            public string MSUBLOTBCD
            {
                get
                {
                    return this._MSUBLOTBCD;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MSUBLOTBCD = value;
                        this.CheckIsOriginal("MSUBLOTBCD", value);
                        this.OnPropertyChanged("MSUBLOTBCD");
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
            private string _MSUBLOTSEQ;
            [BizActorInputItemAttribute()]
            public string MSUBLOTSEQ
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
            private string _MSUBLOTID;
            [BizActorInputItemAttribute()]
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
            private string _IS_NEW;
            [BizActorInputItemAttribute()]
            public string IS_NEW
            {
                get
                {
                    return this._IS_NEW;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_NEW = value;
                        this.CheckIsOriginal("IS_NEW", value);
                        this.OnPropertyChanged("IS_NEW");
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
            private string _SCALEID;
            [BizActorInputItemAttribute()]
            public string SCALEID
            {
                get
                {
                    return this._SCALEID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SCALEID = value;
                        this.CheckIsOriginal("SCALEID", value);
                        this.OnPropertyChanged("SCALEID");
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
            private string _WEIGHINGMETHOD;
            [BizActorInputItemAttribute()]
            public string WEIGHINGMETHOD
            {
                get
                {
                    return this._WEIGHINGMETHOD;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WEIGHINGMETHOD = value;
                        this.CheckIsOriginal("WEIGHINGMETHOD", value);
                        this.OnPropertyChanged("WEIGHINGMETHOD");
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
            private string _DSPSTRTDTTM;
            [BizActorInputItemAttribute()]
            public string DSPSTRTDTTM
            {
                get
                {
                    return this._DSPSTRTDTTM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DSPSTRTDTTM = value;
                        this.CheckIsOriginal("DSPSTRTDTTM", value);
                        this.OnPropertyChanged("DSPSTRTDTTM");
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
            private string _DSPENDDTTM;
            [BizActorInputItemAttribute()]
            public string DSPENDDTTM
            {
                get
                {
                    return this._DSPENDDTTM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DSPENDDTTM = value;
                        this.CheckIsOriginal("DSPENDDTTM", value);
                        this.OnPropertyChanged("DSPENDDTTM");
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
            private string _WEIGHINGROOM;
            [BizActorInputItemAttribute()]
            public string WEIGHINGROOM
            {
                get
                {
                    return this._WEIGHINGROOM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WEIGHINGROOM = value;
                        this.CheckIsOriginal("WEIGHINGROOM", value);
                        this.OnPropertyChanged("WEIGHINGROOM");
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
            private string _MSUBLOTVER;
            [BizActorOutputItemAttribute()]
            public string MSUBLOTVER
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
        }
        public sealed partial class HISDATACollection : BufferedObservableCollection<HISDATA>
        {
        }
        private HISDATACollection _HISDATAs;
        [BizActorOutputSetAttribute()]
        public HISDATACollection HISDATAs
        {
            get
            {
                return this._HISDATAs;
            }
        }
        [BizActorOutputSetDefineAttribute(Order = "1")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class HISDATA : BizActorDataSetBase
        {
            public HISDATA()
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
            private System.Nullable<long> _TRANSID;
            [BizActorOutputItemAttribute()]
            public System.Nullable<long> TRANSID
            {
                get
                {
                    return this._TRANSID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._TRANSID = value;
                        this.CheckIsOriginal("TRANSID", value);
                        this.OnPropertyChanged("TRANSID");
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
        public BR_BRS_REG_ProductionOrderOutput_LastSoluction()
        {
            RuleName = "BR_BRS_REG_ProductionOrderOutput_LastSoluction";
            _INDATAs = new INDATACollection();
            _INDATA_SOLUTIONs = new INDATA_SOLUTIONCollection();
            _OUTDATAs = new OUTDATACollection();
            _HISDATAs = new HISDATACollection();
        }
    }
}
