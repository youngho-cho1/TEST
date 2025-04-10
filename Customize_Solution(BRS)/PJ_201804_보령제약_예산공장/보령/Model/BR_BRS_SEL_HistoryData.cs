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
    public partial class BR_BRS_SEL_HistoryData : BizActorRuleBase
    {
        public partial class OUTDATA : BizActorDataSetBase
        {
            string _TAGNAME;
            public string TAGNAME
            {
                get { return _TAGNAME; }
                set
                {
                    _TAGNAME = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public partial class OUTDATA_SUMMARY : BizActorDataSetBase
        {
            string _TAGNAME;
            public string TAGNAME
            {
                get { return _TAGNAME; }
                set
                {
                    _TAGNAME = value;
                    NotifyPropertyChanged();
                }
            }

            string _COMMENT;
            public string COMMENT
            {
                get { return _COMMENT; }
                set
                {
                    _COMMENT = value;
                    NotifyPropertyChanged();
                }
            }

            string _MINCOL;
            public string MINCOL
            {
                get { return _MINCOL; }
                set
                {
                    _MINCOL = value;
                    NotifyPropertyChanged();
                }
            }

            string _MAXCOL;
            public string MAXCOL
            {
                get { return _MAXCOL; }
                set
                {
                    _MAXCOL = value;
                    NotifyPropertyChanged();
                }
            }

            string _DATACOUNT;
            public string DATACOUNT
            {
                get { return _DATACOUNT; }
                set
                {
                    _DATACOUNT = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
