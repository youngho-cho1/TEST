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
    public partial class BR_BRS_REG_ProductionOrderOutput_Conditional : BizActorRuleBase
    {
        public partial class INDATA : BizActorDataSetBase
        {
            public INDATA Copy()
            {
                return (INDATA)this.MemberwiseClone();
            }
        }
        public partial class LABEL_INDATA : BizActorDataSetBase
        {
            public LABEL_INDATA Copy()
            {
                return (LABEL_INDATA)this.MemberwiseClone();
            }
        }
       public partial class LABEL_Parameter : BizActorDataSetBase
        {
            public LABEL_Parameter Copy()
            {
                return (LABEL_Parameter)this.MemberwiseClone();
            }
        }

        public BR_BRS_REG_ProductionOrderOutput_Conditional IndataCopy()
        {
            var br = new BR_BRS_REG_ProductionOrderOutput_Conditional();

            foreach (INDATA item in INDATAs)
                br.INDATAs.Add(item.Copy());

            foreach (LABEL_INDATA item in LABEL_INDATAs)
                br.LABEL_INDATAs.Add(item.Copy());

            foreach (LABEL_Parameter item in LABEL_Parameters)
                br.LABEL_Parameters.Add(item.Copy());

            return br;
        }
    }
}
