﻿using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace LGCNS.iPharmMES.Common
{

    /// <summary>
    /// summary of BR_BRS_REG_ProductionOrderOutput_Charging_Output
    /// </summary>
    public partial class BR_BRS_REG_ProductionOrderOutput_Charging_Output : BizActorRuleBase
    {
        public sealed partial class INDATA_CHARGINGCollection : BufferedObservableCollection<INDATA_CHARGING>
        {
        }
        private INDATA_CHARGINGCollection _INDATA_CHARGINGs;
        [BizActorInputSetAttribute()]
        public INDATA_CHARGINGCollection INDATA_CHARGINGs
        {
            get
            {
                return this._INDATA_CHARGINGs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "0")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA_CHARGING : BizActorDataSetBase
        {
            public INDATA_CHARGING()
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
            private System.Nullable<float> _MSUBLOTQTY;
            [BizActorInputItemAttribute()]
            public System.Nullable<float> MSUBLOTQTY
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
            private System.Nullable<System.DateTime> _INSDTTM;
            [BizActorInputItemAttribute()]
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
            private string _IS_NEED_CHKWEIGHT;
            [BizActorInputItemAttribute()]
            public string IS_NEED_CHKWEIGHT
            {
                get
                {
                    return this._IS_NEED_CHKWEIGHT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_NEED_CHKWEIGHT = value;
                        this.CheckIsOriginal("IS_NEED_CHKWEIGHT", value);
                        this.OnPropertyChanged("IS_NEED_CHKWEIGHT");
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
            private string _IS_FULL_CHARGE;
            [BizActorInputItemAttribute()]
            public string IS_FULL_CHARGE
            {
                get
                {
                    return this._IS_FULL_CHARGE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_FULL_CHARGE = value;
                        this.CheckIsOriginal("IS_FULL_CHARGE", value);
                        this.OnPropertyChanged("IS_FULL_CHARGE");
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
            private string _IS_CHECKONLY;
            [BizActorInputItemAttribute()]
            public string IS_CHECKONLY
            {
                get
                {
                    return this._IS_CHECKONLY;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_CHECKONLY = value;
                        this.CheckIsOriginal("IS_CHECKONLY", value);
                        this.OnPropertyChanged("IS_CHECKONLY");
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
            private string _IS_INVEN_CHARGE;
            [BizActorInputItemAttribute()]
            public string IS_INVEN_CHARGE
            {
                get
                {
                    return this._IS_INVEN_CHARGE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_INVEN_CHARGE = value;
                        this.CheckIsOriginal("IS_INVEN_CHARGE", value);
                        this.OnPropertyChanged("IS_INVEN_CHARGE");
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
            private string _CHECKINUSER;
            [BizActorInputItemAttribute()]
            public string CHECKINUSER
            {
                get
                {
                    return this._CHECKINUSER;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._CHECKINUSER = value;
                        this.CheckIsOriginal("CHECKINUSER", value);
                        this.OnPropertyChanged("CHECKINUSER");
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
            private string _IS_OUTPUT;
            [BizActorInputItemAttribute()]
            public string IS_OUTPUT
            {
                get
                {
                    return this._IS_OUTPUT;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IS_OUTPUT = value;
                        this.CheckIsOriginal("IS_OUTPUT", value);
                        this.OnPropertyChanged("IS_OUTPUT");
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
        public sealed partial class INDATA_OUTPUT_ADJUSTCollection : BufferedObservableCollection<INDATA_OUTPUT_ADJUST>
        {
        }
        private INDATA_OUTPUT_ADJUSTCollection _INDATA_OUTPUT_ADJUSTs;
        [BizActorInputSetAttribute()]
        public INDATA_OUTPUT_ADJUSTCollection INDATA_OUTPUT_ADJUSTs
        {
            get
            {
                return this._INDATA_OUTPUT_ADJUSTs;
            }
        }
        [BizActorInputSetDefineAttribute(Order = "1")]
        [CustomValidation(typeof(ViewModelBase), "ValidateRow")]
        public partial class INDATA_OUTPUT_ADJUST : BizActorDataSetBase
        {
            public INDATA_OUTPUT_ADJUST()
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
            private string _OUTPUTID;
            [BizActorInputItemAttribute()]
            public string OUTPUTID
            {
                get
                {
                    return this._OUTPUTID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._OUTPUTID = value;
                        this.CheckIsOriginal("OUTPUTID", value);
                        this.OnPropertyChanged("OUTPUTID");
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
            private System.Nullable<float> _MSUBLOTQTY;
            [BizActorInputItemAttribute()]
            public System.Nullable<float> MSUBLOTQTY
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
            private System.Nullable<float> _TAREWEIGHT;
            [BizActorInputItemAttribute()]
            public System.Nullable<float> TAREWEIGHT
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
        public BR_BRS_REG_ProductionOrderOutput_Charging_Output()
        {
            RuleName = "BR_BRS_REG_ProductionOrderOutput_Charging_Output";
            BizName = "반제품 투입 및 생성";
            _INDATA_CHARGINGs = new INDATA_CHARGINGCollection();
            _INDATA_OUTPUT_ADJUSTs = new INDATA_OUTPUT_ADJUSTCollection();
            _HISDATAs = new HISDATACollection();
        }
    }
}
