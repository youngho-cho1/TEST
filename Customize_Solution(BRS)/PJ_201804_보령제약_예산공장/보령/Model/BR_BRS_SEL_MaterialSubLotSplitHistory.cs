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
    public partial class BR_BRS_SEL_MaterialSubLotSplitHistory : BizActorRuleBase
    {
        public partial class OUTDATA
        {
            private bool _ISSELECTED = false;
            public bool ISSELECTED
            {
                get { return _ISSELECTED; }
                set
                {
                    _ISSELECTED = value;
                    OnPropertyChanged("ISSELECTED");
                }
            }
            public string PREVWEIGHT
            {
                get
                {
                    Weight temp = new Weight();
                    temp.SetWeight(this._OLDMSUBLOTQTY.GetValueOrDefault(), this._UOM, this._PRECISION.GetValueOrDefault());

                    //return temp.WeightUOMString;
                    return temp.WeightUOMStringWithSeperator;
                }
            }
            public string CURWEIGHT
            {
                get
                {
                    Weight temp = new Weight();
                    temp.SetWeight(this._MSUBLOTQTY.GetValueOrDefault(), this._UOM, this._PRECISION.GetValueOrDefault());

                    //return temp.WeightUOMString;
                    return temp.WeightUOMStringWithSeperator;
                }
            }
        }
    }
}
