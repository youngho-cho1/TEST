using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    
    /// <summary>
    /// summary of BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus
    /// </summary>
    public partial class BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus : BizActorRuleBase
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
            private string _EQCLID;
            [BizActorInputItemAttribute()]
            public string EQCLID
            {
                get
                {
                    return this._EQCLID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQCLID = value;
                        this.CheckIsOriginal("EQCLID", value);
                        this.OnPropertyChanged("EQCLID");
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
            private string _EQCLIUSE;
            [BizActorInputItemAttribute()]
            public string EQCLIUSE
            {
                get
                {
                    return this._EQCLIUSE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQCLIUSE = value;
                        this.CheckIsOriginal("EQCLIUSE", value);
                        this.OnPropertyChanged("EQCLIUSE");
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
        [BizActorOutputSetDefineAttribute(Order="0")]
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
            private string _EQCLNAME;
            [BizActorOutputItemAttribute()]
            public string EQCLNAME
            {
                get
                {
                    return this._EQCLNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQCLNAME = value;
                        this.CheckIsOriginal("EQCLNAME", value);
                        this.OnPropertyChanged("EQCLNAME");
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
            private string _EQACID;
            [BizActorOutputItemAttribute()]
            public string EQACID
            {
                get
                {
                    return this._EQACID;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQACID = value;
                        this.CheckIsOriginal("EQACID", value);
                        this.OnPropertyChanged("EQACID");
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
            private string _EQACNAME;
            [BizActorOutputItemAttribute()]
            public string EQACNAME
            {
                get
                {
                    return this._EQACNAME;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQACNAME = value;
                        this.CheckIsOriginal("EQACNAME", value);
                        this.OnPropertyChanged("EQACNAME");
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
            private string _EQACDESC;
            [BizActorOutputItemAttribute()]
            public string EQACDESC
            {
                get
                {
                    return this._EQACDESC;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQACDESC = value;
                        this.CheckIsOriginal("EQACDESC", value);
                        this.OnPropertyChanged("EQACDESC");
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
            private string _EQACIUSE;
            [BizActorOutputItemAttribute()]
            public string EQACIUSE
            {
                get
                {
                    return this._EQACIUSE;
                }
                set
                {
                    if ((this.IsValid(value) == LGCNS.iPharmMES.Common.Common.enumValidationLevel.Error))
                    {
                    }
                    else
                    {
                        this._EQACIUSE = value;
                        this.CheckIsOriginal("EQACIUSE", value);
                        this.OnPropertyChanged("EQACIUSE");
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
        public BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus()
        {
            RuleName = "BR_PHR_SEL_EquipmentClassAction_Parent_ActionStatus";
            BizName = "BR_PHR_SEL_EquipmentClassAction_Parent";
            _INDATAs = new INDATACollection();
            _OUTDATAs = new OUTDATACollection();
        }
    }
}
