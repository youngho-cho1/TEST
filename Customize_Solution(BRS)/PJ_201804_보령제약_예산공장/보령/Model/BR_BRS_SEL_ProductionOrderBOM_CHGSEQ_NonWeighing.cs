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

namespace LGCNS.iPharmMES.Common
{
    public partial class BR_BRS_SEL_ProductionOrderBOM_CHGSEQ_NonWeighing : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            string _QCT_NO_SEQ;
            public string QCT_NO_SEQ
            {
                get { return _QCT_NO_SEQ; }
                set { _QCT_NO_SEQ = value; }
            }
        //    string _ISAVAIL;
        //    public string ISAVAIL
        //    {
        //        get { return _ISAVAIL; }
        //        set { _ISAVAIL = value; }
        //    }
        //    string _MSUBLOTBCD;
        //    public string MSUBLOTBCD
        //    {
        //        get { return _MSUBLOTBCD; }
        //        set { _MSUBLOTBCD = value; }
        //    }
        }
    }
}
