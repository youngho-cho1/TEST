using LGCNS.iPharmMES.Common;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령
{
    public partial class BR_BRS_REG_ProductionOrderOutput_LastSoluction : BizActorRuleBase
    {
        public partial class INDATA_SOLUTION : BizActorDataSetBase
        {
            public INDATA_SOLUTION Copy()
            {
                return (INDATA_SOLUTION)this.MemberwiseClone();
            }
        }
    }
}
