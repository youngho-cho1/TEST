using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace LGCNS.iPharmMES.Common
{
    public partial class BR_BRS_SEL_ProductionOrder_Component_Summary : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            private string _IS_CAN_CHARGING_CHECKED_NAME;
            public string IS_CAN_CHARGING_CHECKED_NAME
            {
                get { return _IS_CAN_CHARGING_CHECKED_NAME; }
                set { _IS_CAN_CHARGING_CHECKED_NAME = value; }

            }
        }
    }
}
