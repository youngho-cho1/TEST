using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{

    /// <summary>
    /// summary of BR_BRS_SEL_SVP_ASEPTIC_PROCESS
    /// </summary>
    public partial class BR_BRS_SEL_SVP_ASEPTIC_PROCESS : BizActorRuleBase
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
            private string _GUBUN;
            [BizActorInputItemAttribute()]
            public string GUBUN
            {
                get
                {
                    return this._GUBUN;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._GUBUN = value;
                        this.CheckIsOriginal("GUBUN", value);
                        this.OnPropertyChanged("GUBUN");
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
            private string _ChargDttm;
            [BizActorOutputItemAttribute()]
            public string ChargDttm
            {
                get
                {
                    return this._ChargDttm;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._ChargDttm = value;
                        this.CheckIsOriginal("ChargDttm", value);
                        this.OnPropertyChanged("ChargDttm");
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
            private string _UnloadingDttm;
            [BizActorOutputItemAttribute()]
            public string UnloadingDttm
            {
                get
                {
                    return this._UnloadingDttm;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._UnloadingDttm = value;
                        this.CheckIsOriginal("UnloadingDttm", value);
                        this.OnPropertyChanged("UnloadingDttm");
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
            private string _AutoClaveHoldTime;
            [BizActorOutputItemAttribute()]
            public string AutoClaveHoldTime
            {
                get
                {
                    return this._AutoClaveHoldTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AutoClaveHoldTime = value;
                        this.CheckIsOriginal("AutoClaveHoldTime", value);
                        this.OnPropertyChanged("AutoClaveHoldTime");
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
            private string _SUMAUTOCLAVEHOLDTIME;
            [BizActorOutputItemAttribute()]
            public string SUMAUTOCLAVEHOLDTIME
            {
                get
                {
                    return this._SUMAUTOCLAVEHOLDTIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SUMAUTOCLAVEHOLDTIME = value;
                        this.CheckIsOriginal("SUMAUTOCLAVEHOLDTIME", value);
                        this.OnPropertyChanged("SUMAUTOCLAVEHOLDTIME");
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
            private string _AutoDiff;
            [BizActorOutputItemAttribute()]
            public string AutoDiff
            {
                get
                {
                    return this._AutoDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AutoDiff = value;
                        this.CheckIsOriginal("AutoDiff", value);
                        this.OnPropertyChanged("AutoDiff");
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
            private string _IsolatorHoldTime;
            [BizActorOutputItemAttribute()]
            public string IsolatorHoldTime
            {
                get
                {
                    return this._IsolatorHoldTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IsolatorHoldTime = value;
                        this.CheckIsOriginal("IsolatorHoldTime", value);
                        this.OnPropertyChanged("IsolatorHoldTime");
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
            private string _SUMISOLATORHOLDTIME;
            [BizActorOutputItemAttribute()]
            public string SUMISOLATORHOLDTIME
            {
                get
                {
                    return this._SUMISOLATORHOLDTIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SUMISOLATORHOLDTIME = value;
                        this.CheckIsOriginal("SUMISOLATORHOLDTIME", value);
                        this.OnPropertyChanged("SUMISOLATORHOLDTIME");
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
            private string _IsolatorHoldDiff;
            [BizActorOutputItemAttribute()]
            public string IsolatorHoldDiff
            {
                get
                {
                    return this._IsolatorHoldDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._IsolatorHoldDiff = value;
                        this.CheckIsOriginal("IsolatorHoldDiff", value);
                        this.OnPropertyChanged("IsolatorHoldDiff");
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
            private string _SetStartTime;
            [BizActorOutputItemAttribute()]
            public string SetStartTime
            {
                get
                {
                    return this._SetStartTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SetStartTime = value;
                        this.CheckIsOriginal("SetStartTime", value);
                        this.OnPropertyChanged("SetStartTime");
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
            private string _SetEndTime;
            [BizActorOutputItemAttribute()]
            public string SetEndTime
            {
                get
                {
                    return this._SetEndTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SetEndTime = value;
                        this.CheckIsOriginal("SetEndTime", value);
                        this.OnPropertyChanged("SetEndTime");
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
            private string _SUMSETTIME;
            [BizActorOutputItemAttribute()]
            public string SUMSETTIME
            {
                get
                {
                    return this._SUMSETTIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SUMSETTIME = value;
                        this.CheckIsOriginal("SUMSETTIME", value);
                        this.OnPropertyChanged("SUMSETTIME");
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
            private string _SETDiff;
            [BizActorOutputItemAttribute()]
            public string SETDiff
            {
                get
                {
                    return this._SETDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SETDiff = value;
                        this.CheckIsOriginal("SETDiff", value);
                        this.OnPropertyChanged("SETDiff");
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
            private string _StableTime;
            [BizActorOutputItemAttribute()]
            public string StableTime
            {
                get
                {
                    return this._StableTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._StableTime = value;
                        this.CheckIsOriginal("StableTime", value);
                        this.OnPropertyChanged("StableTime");
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
            private string _SUMSTABLETIME;
            [BizActorOutputItemAttribute()]
            public string SUMSTABLETIME
            {
                get
                {
                    return this._SUMSTABLETIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SUMSTABLETIME = value;
                        this.CheckIsOriginal("SUMSTABLETIME", value);
                        this.OnPropertyChanged("SUMSTABLETIME");
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
            private string _StableDiff;
            [BizActorOutputItemAttribute()]
            public string StableDiff
            {
                get
                {
                    return this._StableDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._StableDiff = value;
                        this.CheckIsOriginal("StableDiff", value);
                        this.OnPropertyChanged("StableDiff");
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
            private string _FillTIME;
            [BizActorOutputItemAttribute()]
            public string FillTIME
            {
                get
                {
                    return this._FillTIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FillTIME = value;
                        this.CheckIsOriginal("FillTIME", value);
                        this.OnPropertyChanged("FillTIME");
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
            private string _SumFillTIME;
            [BizActorOutputItemAttribute()]
            public string SumFillTIME
            {
                get
                {
                    return this._SumFillTIME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumFillTIME = value;
                        this.CheckIsOriginal("SumFillTIME", value);
                        this.OnPropertyChanged("SumFillTIME");
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
            private string _FillDiff;
            [BizActorOutputItemAttribute()]
            public string FillDiff
            {
                get
                {
                    return this._FillDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FillDiff = value;
                        this.CheckIsOriginal("FillDiff", value);
                        this.OnPropertyChanged("FillDiff");
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
            private string _FillTIME_FDR;
            [BizActorOutputItemAttribute()]
            public string FillTIME_FDR
            {
                get
                {
                    return this._FillTIME_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FillTIME_FDR = value;
                        this.CheckIsOriginal("FillTIME_FDR", value);
                        this.OnPropertyChanged("FillTIME_FDR");
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
            private string _SumFillTIME_FDR;
            [BizActorOutputItemAttribute()]
            public string SumFillTIME_FDR
            {
                get
                {
                    return this._SumFillTIME_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumFillTIME_FDR = value;
                        this.CheckIsOriginal("SumFillTIME_FDR", value);
                        this.OnPropertyChanged("SumFillTIME_FDR");
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
            private string _FillDiff_FDR;
            [BizActorOutputItemAttribute()]
            public string FillDiff_FDR
            {
                get
                {
                    return this._FillDiff_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FillDiff_FDR = value;
                        this.CheckIsOriginal("FillDiff_FDR", value);
                        this.OnPropertyChanged("FillDiff_FDR");
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
            private string _AsepticTime;
            [BizActorOutputItemAttribute()]
            public string AsepticTime
            {
                get
                {
                    return this._AsepticTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AsepticTime = value;
                        this.CheckIsOriginal("AsepticTime", value);
                        this.OnPropertyChanged("AsepticTime");
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
            private string _SumAsepticTime;
            [BizActorOutputItemAttribute()]
            public string SumAsepticTime
            {
                get
                {
                    return this._SumAsepticTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumAsepticTime = value;
                        this.CheckIsOriginal("SumAsepticTime", value);
                        this.OnPropertyChanged("SumAsepticTime");
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
            private string _AsepticDiff;
            [BizActorOutputItemAttribute()]
            public string AsepticDiff
            {
                get
                {
                    return this._AsepticDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AsepticDiff = value;
                        this.CheckIsOriginal("AsepticDiff", value);
                        this.OnPropertyChanged("AsepticDiff");
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
            private string _AsepticTime_FDR;
            [BizActorOutputItemAttribute()]
            public string AsepticTime_FDR
            {
                get
                {
                    return this._AsepticTime_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AsepticTime_FDR = value;
                        this.CheckIsOriginal("AsepticTime_FDR", value);
                        this.OnPropertyChanged("AsepticTime_FDR");
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
            private string _SumAsepticTime_FDR;
            [BizActorOutputItemAttribute()]
            public string SumAsepticTime_FDR
            {
                get
                {
                    return this._SumAsepticTime_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumAsepticTime_FDR = value;
                        this.CheckIsOriginal("SumAsepticTime_FDR", value);
                        this.OnPropertyChanged("SumAsepticTime_FDR");
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
            private string _AsepticDiff_FDR;
            [BizActorOutputItemAttribute()]
            public string AsepticDiff_FDR
            {
                get
                {
                    return this._AsepticDiff_FDR;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._AsepticDiff_FDR = value;
                        this.CheckIsOriginal("AsepticDiff_FDR", value);
                        this.OnPropertyChanged("AsepticDiff_FDR");
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
            private string _DryHoldTime;
            [BizActorOutputItemAttribute()]
            public string DryHoldTime
            {
                get
                {
                    return this._DryHoldTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DryHoldTime = value;
                        this.CheckIsOriginal("DryHoldTime", value);
                        this.OnPropertyChanged("DryHoldTime");
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
            private string _SumDryHoldTime;
            [BizActorOutputItemAttribute()]
            public string SumDryHoldTime
            {
                get
                {
                    return this._SumDryHoldTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumDryHoldTime = value;
                        this.CheckIsOriginal("SumDryHoldTime", value);
                        this.OnPropertyChanged("SumDryHoldTime");
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
            private string _DryHoldDiff;
            [BizActorOutputItemAttribute()]
            public string DryHoldDiff
            {
                get
                {
                    return this._DryHoldDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DryHoldDiff = value;
                        this.CheckIsOriginal("DryHoldDiff", value);
                        this.OnPropertyChanged("DryHoldDiff");
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
            private string _FreezeTime;
            [BizActorOutputItemAttribute()]
            public string FreezeTime
            {
                get
                {
                    return this._FreezeTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FreezeTime = value;
                        this.CheckIsOriginal("FreezeTime", value);
                        this.OnPropertyChanged("FreezeTime");
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
            private string _SumFreezeTime;
            [BizActorOutputItemAttribute()]
            public string SumFreezeTime
            {
                get
                {
                    return this._SumFreezeTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumFreezeTime = value;
                        this.CheckIsOriginal("SumFreezeTime", value);
                        this.OnPropertyChanged("SumFreezeTime");
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
            private string _FreezeDiff;
            [BizActorOutputItemAttribute()]
            public string FreezeDiff
            {
                get
                {
                    return this._FreezeDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._FreezeDiff = value;
                        this.CheckIsOriginal("FreezeDiff", value);
                        this.OnPropertyChanged("FreezeDiff");
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
            private string _BlockTime;
            [BizActorOutputItemAttribute()]
            public string BlockTime
            {
                get
                {
                    return this._BlockTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._BlockTime = value;
                        this.CheckIsOriginal("BlockTime", value);
                        this.OnPropertyChanged("BlockTime");
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
            private string _SumBlockTime;
            [BizActorOutputItemAttribute()]
            public string SumBlockTime
            {
                get
                {
                    return this._SumBlockTime;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._SumBlockTime = value;
                        this.CheckIsOriginal("SumBlockTime", value);
                        this.OnPropertyChanged("SumBlockTime");
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
            private string _BlockDiff;
            [BizActorOutputItemAttribute()]
            public string BlockDiff
            {
                get
                {
                    return this._BlockDiff;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._BlockDiff = value;
                        this.CheckIsOriginal("BlockDiff", value);
                        this.OnPropertyChanged("BlockDiff");
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
            private string _WORKUSERID;
            [BizActorOutputItemAttribute()]
            public string WORKUSERID
            {
                get
                {
                    return this._WORKUSERID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._WORKUSERID = value;
                        this.CheckIsOriginal("WORKUSERID", value);
                        this.OnPropertyChanged("WORKUSERID");
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
            private string _PMSStartDttm;
            [BizActorOutputItemAttribute()]
            public string PMSStartDttm
            {
                get
                {
                    return this._PMSStartDttm;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._PMSStartDttm = value;
                        this.CheckIsOriginal("PMSStartDttm", value);
                        this.OnPropertyChanged("PMSStartDttm");
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
            private string _PMSEndDttm;
            [BizActorOutputItemAttribute()]
            public string PMSEndDttm
            {
                get
                {
                    return this._PMSEndDttm;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._PMSEndDttm = value;
                        this.CheckIsOriginal("PMSEndDttm", value);
                        this.OnPropertyChanged("PMSEndDttm");
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
            private string _DropGermsFirst;
            [BizActorOutputItemAttribute()]
            public string DropGermsFirst
            {
                get
                {
                    return this._DropGermsFirst;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsFirst = value;
                        this.CheckIsOriginal("DropGermsFirst", value);
                        this.OnPropertyChanged("DropGermsFirst");
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
            private string _DropGermsSecond;
            [BizActorOutputItemAttribute()]
            public string DropGermsSecond
            {
                get
                {
                    return this._DropGermsSecond;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsSecond = value;
                        this.CheckIsOriginal("DropGermsSecond", value);
                        this.OnPropertyChanged("DropGermsSecond");
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
            private string _DropGermsThird;
            [BizActorOutputItemAttribute()]
            public string DropGermsThird
            {
                get
                {
                    return this._DropGermsThird;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsThird = value;
                        this.CheckIsOriginal("DropGermsThird", value);
                        this.OnPropertyChanged("DropGermsThird");
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
            private string _DropGermsFourth;
            [BizActorOutputItemAttribute()]
            public string DropGermsFourth
            {
                get
                {
                    return this._DropGermsFourth;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsFourth = value;
                        this.CheckIsOriginal("DropGermsFourth", value);
                        this.OnPropertyChanged("DropGermsFourth");
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
            private string _DropGermsFifth;
            [BizActorOutputItemAttribute()]
            public string DropGermsFifth
            {
                get
                {
                    return this._DropGermsFifth;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsFifth = value;
                        this.CheckIsOriginal("DropGermsFifth", value);
                        this.OnPropertyChanged("DropGermsFifth");
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
            private string _DropGermsSixth;
            [BizActorOutputItemAttribute()]
            public string DropGermsSixth
            {
                get
                {
                    return this._DropGermsSixth;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsSixth = value;
                        this.CheckIsOriginal("DropGermsSixth", value);
                        this.OnPropertyChanged("DropGermsSixth");
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
            private string _DropGermsSeventh;
            [BizActorOutputItemAttribute()]
            public string DropGermsSeventh
            {
                get
                {
                    return this._DropGermsSeventh;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsSeventh = value;
                        this.CheckIsOriginal("DropGermsSeventh", value);
                        this.OnPropertyChanged("DropGermsSeventh");
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
            private string _DropGermsEighth;
            [BizActorOutputItemAttribute()]
            public string DropGermsEighth
            {
                get
                {
                    return this._DropGermsEighth;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._DropGermsEighth = value;
                        this.CheckIsOriginal("DropGermsEighth", value);
                        this.OnPropertyChanged("DropGermsEighth");
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
            private string _microbeA1;
            [BizActorOutputItemAttribute()]
            public string microbeA1
            {
                get
                {
                    return this._microbeA1;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._microbeA1 = value;
                        this.CheckIsOriginal("microbeA1", value);
                        this.OnPropertyChanged("microbeA1");
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
            private string _microbeA2;
            [BizActorOutputItemAttribute()]
            public string microbeA2
            {
                get
                {
                    return this._microbeA2;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._microbeA2 = value;
                        this.CheckIsOriginal("microbeA2", value);
                        this.OnPropertyChanged("microbeA2");
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
            private string _microbeA3;
            [BizActorOutputItemAttribute()]
            public string microbeA3
            {
                get
                {
                    return this._microbeA3;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._microbeA3 = value;
                        this.CheckIsOriginal("microbeA3", value);
                        this.OnPropertyChanged("microbeA3");
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
            private string _Nozzle1Check;
            [BizActorOutputItemAttribute()]
            public string Nozzle1Check
            {
                get
                {
                    return this._Nozzle1Check;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._Nozzle1Check = value;
                        this.CheckIsOriginal("Nozzle1Check", value);
                        this.OnPropertyChanged("Nozzle1Check");
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
            private string _Nozzle2Check;
            [BizActorOutputItemAttribute()]
            public string Nozzle2Check
            {
                get
                {
                    return this._Nozzle2Check;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._Nozzle2Check = value;
                        this.CheckIsOriginal("Nozzle2Check", value);
                        this.OnPropertyChanged("Nozzle2Check");
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
            private string _Nozzle3Check;
            [BizActorOutputItemAttribute()]
            public string Nozzle3Check
            {
                get
                {
                    return this._Nozzle3Check;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._Nozzle3Check = value;
                        this.CheckIsOriginal("Nozzle3Check", value);
                        this.OnPropertyChanged("Nozzle3Check");
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
            private string _Nozzle4Check;
            [BizActorOutputItemAttribute()]
            public string Nozzle4Check
            {
                get
                {
                    return this._Nozzle4Check;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._Nozzle4Check = value;
                        this.CheckIsOriginal("Nozzle4Check", value);
                        this.OnPropertyChanged("Nozzle4Check");
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
            private string _MicrobeUnloadingFirst;
            [BizActorOutputItemAttribute()]
            public string MicrobeUnloadingFirst
            {
                get
                {
                    return this._MicrobeUnloadingFirst;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MicrobeUnloadingFirst = value;
                        this.CheckIsOriginal("MicrobeUnloadingFirst", value);
                        this.OnPropertyChanged("MicrobeUnloadingFirst");
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
            private string _MicrobeUnloadingSecond;
            [BizActorOutputItemAttribute()]
            public string MicrobeUnloadingSecond
            {
                get
                {
                    return this._MicrobeUnloadingSecond;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MicrobeUnloadingSecond = value;
                        this.CheckIsOriginal("MicrobeUnloadingSecond", value);
                        this.OnPropertyChanged("MicrobeUnloadingSecond");
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
            private string _MicrobeUnloadingThird;
            [BizActorOutputItemAttribute()]
            public string MicrobeUnloadingThird
            {
                get
                {
                    return this._MicrobeUnloadingThird;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MicrobeUnloadingThird = value;
                        this.CheckIsOriginal("MicrobeUnloadingThird", value);
                        this.OnPropertyChanged("MicrobeUnloadingThird");
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
            private string _MicrobeUnloadingFourth;
            [BizActorOutputItemAttribute()]
            public string MicrobeUnloadingFourth
            {
                get
                {
                    return this._MicrobeUnloadingFourth;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._MicrobeUnloadingFourth = value;
                        this.CheckIsOriginal("MicrobeUnloadingFourth", value);
                        this.OnPropertyChanged("MicrobeUnloadingFourth");
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
        public BR_BRS_SEL_SVP_ASEPTIC_PROCESS()
        {
            RuleName = "BR_BRS_SEL_SVP_ASEPTIC_PROCESS";
            BizName = "BR_BRS_SEL_SVP_ASEPTIC_PROCESS";
            _INDATAs = new INDATACollection();
            _OUTDATAs = new OUTDATACollection();
        }
    }
}
