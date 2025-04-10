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
    public partial class BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            /// <summary>
            /// 불출라벨 발행가능여부
            /// </summary>
            private bool _PRINTFLAG = true;
            public bool PRINTFLAG
            {
                get { return _PRINTFLAG; }
                set { _PRINTFLAG = value; }
            }
        }
    }
}
