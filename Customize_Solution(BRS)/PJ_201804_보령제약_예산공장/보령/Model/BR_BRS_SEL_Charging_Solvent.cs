using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    public partial class BR_BRS_SEL_Charging_Solvent : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            private decimal _REALQTY = 0m;
            public decimal REALQTY
            {
                get { return _REALQTY; }
                set
                {
                    _REALQTY = value;
                    OnPropertyChanged("REALQTY");
                }
            }

            private decimal _TOTALQTY = 0m;
            public decimal TOTALQTY
            {
                get { return _TOTALQTY; }
                set
                {
                    _TOTALQTY = value;
                    OnPropertyChanged("TOTALQTY");
                }
            }
        }
    }
}
