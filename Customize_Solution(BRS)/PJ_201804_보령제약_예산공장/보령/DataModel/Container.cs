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
    public abstract class Container : BizActorDataSetBase
    {
        private string _MsubLotId;
        public string MsubLotId
        {
            get { return _MsubLotId; }
            set
            {
                _MsubLotId = value;
                OnPropertyChanged("MsubLotId");
            }
        }
        private string _MsubLotBcd;
        public string MsubLotBcd
        {
            get { return _MsubLotBcd; }
            set
            {
                _MsubLotBcd = value;
                OnPropertyChanged("MsubLotBcd");
            }
        }
        private string _MtrlId;
        public string MtrlId
        {
            get { return _MtrlId; }
            set
            {
                _MtrlId = value;
                OnPropertyChanged("MtrlId");
            }
        }
        private string _MtrlName;
        public string MtrlName
        {
            get { return _MtrlName; }
            set
            {
                _MtrlName = value;
                OnPropertyChanged("MtrlName");
            }
        }
        private string _Barcode;
        public string Barcode
        {
            get { return _Barcode; }
            set
            {
                _Barcode = value;
                OnPropertyChanged("Barcode");
            }
        }
        private string _VesselId;
        public string VesselId
        {
            get { return _VesselId; }
            set
            {
                _VesselId = value;
                OnPropertyChanged("VesselId");
            }
        }
        private int _Precision;
        public int Precision
        {
            get { return _Precision; }
            set
            {
                _Precision = value;
                OnPropertyChanged("Precision");
            }
        }
        private string _Uom;
        public string Uom
        {
            get { return _Uom; }
            set
            {
                _Uom = value;
                OnPropertyChanged("Uom");
            }
        }
        private decimal _NetWeight;
        public decimal NetWeight
        {
            get { return _NetWeight; }
            set
            {
                _NetWeight = value;
                OnPropertyChanged("NetWeight");
            }
        }
        private decimal _TareWeight;
        public decimal TareWeight
        {
            get { return _TareWeight; }
            set
            {
                _TareWeight = value;
                OnPropertyChanged("TareWeight");
            }
        }
        public decimal GrossWeight
        {
            get { return _NetWeight + _TareWeight; }
        }

        /// <summary>
        /// 저울 하한 값 저장
        /// </summary>
        private string _MinWeight;
        public string MinWeight
        {
            get { return _MinWeight; }
            set
            {
                _MinWeight = value;
                OnPropertyChanged("MinWeight");
            }
        }

        /// <summary>
        /// 저울 상한 값 저장
        /// </summary>
        private string _MaxWeight;
        public string MaxWeight
        {
            get { return _MaxWeight; }
            set
            {
                _MaxWeight = value;
                OnPropertyChanged("MaxWeight");
            }
        }

        /// <summary>
        /// 저울의 현재 측정 값
        /// </summary>
        private decimal _CurWeight;
        public decimal CurWeight
        {
            get { return _CurWeight; }
            set
            {
                _CurWeight = value;
                OnPropertyChanged("CurWeight");
            }
        }
    }

    public abstract class WIPContainer : Container
    {
        private string _PoId;
        public string PoId
        {
            get { return _PoId; }
            set
            {
                _PoId = value;
                OnPropertyChanged("PoId");
            }
        }
        private string _OpsgGuid;
        public string OpsgGuid
        {
            get { return _OpsgGuid; }
            set
            {
                _OpsgGuid = value;
                OnPropertyChanged("OpsgGuid");
            }
        }
    }
}
