using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    public partial class BR_BRS_SEL_Charging_Solvent_to_Dispense : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            private Weight _INVQTY = new Weight();
            public Weight INVQTY
            {
                get { return _INVQTY; }
                set
                {
                    _INVQTY = value;
                    OnPropertyChanged("INVQTY");
                }
            }
            private Weight _CHGQTY = new Weight();
            public Weight CHGQTY
            {
                get { return _CHGQTY; }   
                set
                {
                    _CHGQTY = value;
                    OnPropertyChanged("CHGQTY");
                }                           
            }

        }
    }
}
