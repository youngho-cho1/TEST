using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace 보령
{
    public partial class BR_BRS_SEL_ProductionOrder_Dispence_Component_Summary : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            private string _IS_CAN_CHARGING_CHECKED_NAME;
            public string IS_CAN_CHARGING_CHECKED_NAME
            {
                get { return _IS_CAN_CHARGING_CHECKED_NAME; }
                set { _IS_CAN_CHARGING_CHECKED_NAME = value; }

            }

            private bool _IsSelected;
            public bool IsSelected
            {
                get { return _IsSelected; }
                set
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnPropertyChanged("STATUS");
                }
            }
            private decimal _UsedWeight;
            public decimal UsedWeight
            {
                get { return _UsedWeight; }
                set
                {
                    _UsedWeight = value;
                    OnPropertyChanged("UsedWeight");
                    OnPropertyChanged("STATUS");
                }
            }
            public string STATUS
            {
                get
                {
                    if (_IsSelected)
                        return "사용중";
                    else if (_UsedWeight > 0)
                        return "사용";
                    else
                        return "대기";
                }
            }
        }
    }
}
