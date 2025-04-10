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
    public partial class BR_BRS_SEL_EquipmentStatus_PROC_OPSG : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            /// <summary>
            /// 선택 여부
            /// </summary>
            private bool _SELFLAG = false;
            public bool SELFLAG
            {
                get { return _SELFLAG; }
                set { _SELFLAG = value; }
            }
        }
    }
}
