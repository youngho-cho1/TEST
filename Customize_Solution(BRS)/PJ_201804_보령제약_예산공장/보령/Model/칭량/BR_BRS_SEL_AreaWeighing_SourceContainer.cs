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
    public partial class BR_BRS_SEL_AreaWeighing_SourceContainer : BizActorRuleBase
    {
        public partial class OUTDATA
        {
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
                    else if(_UsedWeight > 0)
                        return "사용";
                    else
                        return "대기";
                }
            }
        }
    }
}
