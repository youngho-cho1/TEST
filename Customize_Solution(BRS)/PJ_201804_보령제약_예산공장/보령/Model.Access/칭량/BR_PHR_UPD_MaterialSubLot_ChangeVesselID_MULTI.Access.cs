using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{

    /// <summary>
    /// summary of BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI
    /// </summary>
    public partial class BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI : BizActorRuleBase
    {
        public sealed partial class INDATA_MLOTCollection : BufferedObservableCollection<INDATA_MLOT>
        {
        }
        private INDATA_MLOTCollection _INDATA_MLOTs;
        [BizActorInputSetAttribute()]
        public INDATA_MLOTCollection INDATA_MLOTs
        {
            get
            {
                return this._INDATA_MLOTs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "0")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA_MLOT : BizActorDataSetBase
        {
            public INDATA_MLOT()
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
            private string _COMPONENTGUID;
            [BizActorInputItemAttribute()]
            public string COMPONENTGUID
            {
                get
                {
                    return this._COMPONENTGUID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._COMPONENTGUID = value;
                        this.CheckIsOriginal("COMPONENTGUID", value);
                        this.OnPropertyChanged("COMPONENTGUID");
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
            [BizActorInputItemAttribute()]
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
            private string _MLOTID;
            [BizActorInputItemAttribute()]
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
            private System.Nullable<decimal> _MLOTVER;
            [BizActorInputItemAttribute()]
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
            private string _INDUSER;
            [BizActorInputItemAttribute()]
            public string INDUSER
            {
                get
                {
                    return this._INDUSER;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._INDUSER = value;
                        this.CheckIsOriginal("INDUSER", value);
                        this.OnPropertyChanged("INDUSER");
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
        public sealed partial class INDATA_MSUBLOTCollection : BufferedObservableCollection<INDATA_MSUBLOT>
        {
        }
        private INDATA_MSUBLOTCollection _INDATA_MSUBLOTs;
        [BizActorInputSetAttribute()]
        public INDATA_MSUBLOTCollection INDATA_MSUBLOTs
        {
            get
            {
                return this._INDATA_MSUBLOTs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "1")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA_MSUBLOT : BizActorDataSetBase
        {
            public INDATA_MSUBLOT()
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
            private string _MLOTID;
            [BizActorInputItemAttribute()]
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
            private System.Nullable<decimal> _MSUBLOTVER;
            [BizActorInputItemAttribute()]
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
            private System.Nullable<short> _MSUBLOTSEQ;
            [BizActorInputItemAttribute()]
            public System.Nullable<short> MSUBLOTSEQ
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
            private string _COMPONENTGUID;
            [BizActorInputItemAttribute()]
            public string COMPONENTGUID
            {
                get
                {
                    return this._COMPONENTGUID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._COMPONENTGUID = value;
                        this.CheckIsOriginal("COMPONENTGUID", value);
                        this.OnPropertyChanged("COMPONENTGUID");
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
        [BizActorOutputSetDefineAttribute(Order = "0")]
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
        public BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI()
        {
            RuleName = "BR_PHR_UPD_MaterialSubLot_ChangeVesselID_MULTI";
            _INDATA_MLOTs = new INDATA_MLOTCollection();
            _INDATA_MSUBLOTs = new INDATA_MSUBLOTCollection();
            _HISDATAs = new HISDATACollection();
        }
    }
}
