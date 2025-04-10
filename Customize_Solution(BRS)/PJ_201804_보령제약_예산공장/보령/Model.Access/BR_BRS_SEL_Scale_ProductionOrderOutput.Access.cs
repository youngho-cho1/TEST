using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace LGCNS.iPharmMES.Common
{
    
    /// <summary>
    /// summary of BR_BRS_SEL_Scale_ProductionOrderOutput
    /// </summary>
    public partial class BR_BRS_SEL_Scale_ProductionOrderOutput : BizActorRuleBase
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
        [BizActorInputSetDefineAttribute(Order="0")]
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
            private string _LANGID;
            [BizActorInputItemAttribute()]
            public string LANGID
            {
                get
                {
                    return this._LANGID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._LANGID = value;
                        this.CheckIsOriginal("LANGID", value);
                        this.OnPropertyChanged("LANGID");
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
            private string _OUTPUTTYPE;
            [BizActorInputItemAttribute()]
            public string OUTPUTTYPE
            {
                get
                {
                    return this._OUTPUTTYPE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OUTPUTTYPE = value;
                        this.CheckIsOriginal("OUTPUTTYPE", value);
                        this.OnPropertyChanged("OUTPUTTYPE");
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
        public sealed partial class OUTDATA_OUTPUTCollection : BufferedObservableCollection<OUTDATA_OUTPUT>
        {
        }
        private OUTDATA_OUTPUTCollection _OUTDATA_OUTPUTs;
        [BizActorOutputSetAttribute()]
        public OUTDATA_OUTPUTCollection OUTDATA_OUTPUTs
        {
            get
            {
                return this._OUTDATA_OUTPUTs;
            }
        }
        [BizActorOutputSetDefineAttribute(Order="0")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class OUTDATA_OUTPUT : BizActorDataSetBase
        {
            public OUTDATA_OUTPUT()
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
            private string _VESSELID;
            [BizActorOutputItemAttribute()]
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
            private System.Nullable<System.DateTime> _IN_DT;
            [BizActorOutputItemAttribute()]
            public System.Nullable<System.DateTime> IN_DT
            {
                get
                {
                    return this._IN_DT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IN_DT = value;
                        this.CheckIsOriginal("IN_DT", value);
                        this.OnPropertyChanged("IN_DT");
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
            private System.Nullable<float> _WEIGHT;
            [BizActorOutputItemAttribute()]
            public System.Nullable<float> WEIGHT
            {
                get
                {
                    return this._WEIGHT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WEIGHT = value;
                        this.CheckIsOriginal("WEIGHT", value);
                        this.OnPropertyChanged("WEIGHT");
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
            private string _WEIGHTUOM;
            [BizActorOutputItemAttribute()]
            public string WEIGHTUOM
            {
                get
                {
                    return this._WEIGHTUOM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WEIGHTUOM = value;
                        this.CheckIsOriginal("WEIGHTUOM", value);
                        this.OnPropertyChanged("WEIGHTUOM");
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
            private System.Nullable<float> _BINWEIGHT;
            [BizActorOutputItemAttribute()]
            public System.Nullable<float> BINWEIGHT
            {
                get
                {
                    return this._BINWEIGHT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._BINWEIGHT = value;
                        this.CheckIsOriginal("BINWEIGHT", value);
                        this.OnPropertyChanged("BINWEIGHT");
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
            private string _BINWEIGHTUOM;
            [BizActorOutputItemAttribute()]
            public string BINWEIGHTUOM
            {
                get
                {
                    return this._BINWEIGHTUOM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._BINWEIGHTUOM = value;
                        this.CheckIsOriginal("BINWEIGHTUOM", value);
                        this.OnPropertyChanged("BINWEIGHTUOM");
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
            private System.Nullable<float> _GROSSWEIGHT;
            [BizActorOutputItemAttribute()]
            public System.Nullable<float> GROSSWEIGHT
            {
                get
                {
                    return this._GROSSWEIGHT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._GROSSWEIGHT = value;
                        this.CheckIsOriginal("GROSSWEIGHT", value);
                        this.OnPropertyChanged("GROSSWEIGHT");
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
            private string _GROSSWEIGHTUOM;
            [BizActorOutputItemAttribute()]
            public string GROSSWEIGHTUOM
            {
                get
                {
                    return this._GROSSWEIGHTUOM;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._GROSSWEIGHTUOM = value;
                        this.CheckIsOriginal("GROSSWEIGHTUOM", value);
                        this.OnPropertyChanged("GROSSWEIGHTUOM");
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
            private System.Nullable<System.DateTime> _INSDTTM;
            [BizActorOutputItemAttribute()]
            public System.Nullable<System.DateTime> INSDTTM
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
            private string _MSUBLOTBCD;
            [BizActorOutputItemAttribute()]
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
            private string _OUTPUTGUID;
            [BizActorOutputItemAttribute()]
            public string OUTPUTGUID
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
        [BizActorOutputSetDefineAttribute(Order="1")]
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
            private string _MTRLNAME;
            [BizActorOutputItemAttribute()]
            public string MTRLNAME
            {
                get
                {
                    return this._MTRLNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MTRLNAME = value;
                        this.CheckIsOriginal("MTRLNAME", value);
                        this.OnPropertyChanged("MTRLNAME");
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
            private System.Nullable<decimal> _MSUBLOTQTY;
            [BizActorOutputItemAttribute()]
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
            private string _ACTID;
            [BizActorOutputItemAttribute()]
            public string ACTID
            {
                get
                {
                    return this._ACTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._ACTID = value;
                        this.CheckIsOriginal("ACTID", value);
                        this.OnPropertyChanged("ACTID");
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
            private string _SOURCEMLOTID;
            [BizActorOutputItemAttribute()]
            public string SOURCEMLOTID
            {
                get
                {
                    return this._SOURCEMLOTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SOURCEMLOTID = value;
                        this.CheckIsOriginal("SOURCEMLOTID", value);
                        this.OnPropertyChanged("SOURCEMLOTID");
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
            private string _SOURCEMSUBLOTID;
            [BizActorOutputItemAttribute()]
            public string SOURCEMSUBLOTID
            {
                get
                {
                    return this._SOURCEMSUBLOTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SOURCEMSUBLOTID = value;
                        this.CheckIsOriginal("SOURCEMSUBLOTID", value);
                        this.OnPropertyChanged("SOURCEMSUBLOTID");
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
            private System.Nullable<decimal> _SOURCEMSUBLOTQTY;
            [BizActorOutputItemAttribute()]
            public System.Nullable<decimal> SOURCEMSUBLOTQTY
            {
                get
                {
                    return this._SOURCEMSUBLOTQTY;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SOURCEMSUBLOTQTY = value;
                        this.CheckIsOriginal("SOURCEMSUBLOTQTY", value);
                        this.OnPropertyChanged("SOURCEMSUBLOTQTY");
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
            private string _ISCHARGING;
            [BizActorOutputItemAttribute()]
            public string ISCHARGING
            {
                get
                {
                    return this._ISCHARGING;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._ISCHARGING = value;
                        this.CheckIsOriginal("ISCHARGING", value);
                        this.OnPropertyChanged("ISCHARGING");
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
            private string _UOMNAME;
            [BizActorOutputItemAttribute()]
            public string UOMNAME
            {
                get
                {
                    return this._UOMNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._UOMNAME = value;
                        this.CheckIsOriginal("UOMNAME", value);
                        this.OnPropertyChanged("UOMNAME");
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
        public BR_BRS_SEL_Scale_ProductionOrderOutput()
        {
            RuleName = "BR_BRS_SEL_Scale_ProductionOrderOutput";
            _INDATAs = new INDATACollection();
            _OUTDATA_OUTPUTs = new OUTDATA_OUTPUTCollection();
            _OUTDATAs = new OUTDATACollection();
        }
    }
}
