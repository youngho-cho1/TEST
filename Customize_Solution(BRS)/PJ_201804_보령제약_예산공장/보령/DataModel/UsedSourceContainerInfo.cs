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
using LGCNS.iPharmMES.Common;

namespace 보령.DataModel
{
    public class UsedSourceContainerInfo : ViewModelBase
    {
        private double _Ver;
        public double Ver
        {
            get
            {
                return _Ver;
            }
            set
            {
                _Ver = value;
                OnPropertyChanged("Ver");
            }
        }

        private decimal _PotencyCoeifficient;
        public decimal PotencyCoeifficient
        {
            get
            {
                return _PotencyCoeifficient;
            }
            set
            {
                _PotencyCoeifficient = value;
                OnPropertyChanged("PotencyCoeifficient");
            }
        }

        //private double _AdjustedMaterialWeight;
        public decimal AdjustedMaterialWeight
        {
            get
            {
                return UsedWeight * PotencyCoeifficient;
            }
        }

        private string _SourceContainerNo;
        public string SourceContainerNo
        {
            get
            {
                return _SourceContainerNo;
            }
            set
            {
                _SourceContainerNo = value;
                OnPropertyChanged("SourceContainerNo");
            }
        }

        private string _SourceBarcode;
        public string SourceBarcode
        {
            get
            {
                return _SourceBarcode;
            }
            set
            {
                _SourceBarcode = value;
                OnPropertyChanged("SourceBarcode");
            }
        }

        private decimal? _ContainerWeight;
        public decimal? ContainerWeight
        {
            get
            {
                return _ContainerWeight;
            }
            set
            {
                _ContainerWeight = value;
                OnPropertyChanged("ContainerWeight");
            }
        }

        private decimal _SourceWeight;
        public decimal SourceWeight
        {
            get
            {
                return _SourceWeight;
            }
            set
            {
                _SourceWeight = value;
                OnPropertyChanged("SourceWeight");
            }
        }

        public decimal? TotalWeight
        {
            get
            {
                return SourceWeight + ContainerWeight;
            }
        }

        private string _Unit;
        public string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                _Unit = value;
                OnPropertyChanged("Unit");
            }
        }

        private decimal _UsedWeight;
        public decimal UsedWeight
        {
            get
            {
                return _UsedWeight;
            }
            set
            {
                _UsedWeight = value;
                OnPropertyChanged("UsedWeight");
            }
        }

        private double _Potency;
        public double Potency
        {
            get
            {
                return _Potency;
            }
            set
            {
                _Potency = value;
                OnPropertyChanged("Potency");
            }
        }

        public enum DispensingReason { Print, Scrap, Deplete };
        private DispensingReason _Reason;
        public DispensingReason Reason
        {
            get
            {
                return _Reason;
            }
            set
            {
                _Reason = value;
                OnPropertyChanged("Reason");
            }
        }

        private bool _IsPrint;
        public bool IsPrint
        {
            get
            {
                return _IsPrint;
            }
            set
            {
                _IsPrint = value;
                if (value)
                    Reason = DispensingReason.Print;
                OnPropertyChanged("IsPrint");
            }
        }

        private bool _IsScrap;
        public bool IsScrap
        {
            get
            {
                return _IsScrap;
            }
            set
            {
                _IsScrap = value;
                if (value)
                    Reason = DispensingReason.Scrap;
                OnPropertyChanged("IsScrap");
            }
        }

        private bool _IsDeplete = true;
        public bool IsDeplete
        {
            get
            {
                return _IsDeplete;
            }
            set
            {
                _IsDeplete = value;
                if (value)
                    Reason = DispensingReason.Deplete;
                OnPropertyChanged("IsDeplete");
            }
        }

        private bool _IsPass;
        public bool IsPass
        {
            get
            {
                return _IsPass;
            }
            set
            {
                _IsPass = value;
                OnPropertyChanged("IsPass");
            }
        }

        private decimal _RemainedWeight;
        public decimal RemainedWeight
        {
            get
            {
                return _RemainedWeight;
            }
            set
            {
                _RemainedWeight = value;
                OnPropertyChanged("RemainedWeight");
            }
        }

        private string _MaterialName;
        public string MaterialName
        {
            get
            {
                return _MaterialName;
            }
            set
            {
                _MaterialName = value;
                OnPropertyChanged("MaterialName");
            }
        }

        private string _Comments;
        public string Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                _Comments = value;
                OnPropertyChanged("Comments");
            }
        }

        private string _OrderNo;
        public string OrderNo
        {
            get
            {
                return _OrderNo;
            }
            set
            {
                _OrderNo = value;
                OnPropertyChanged("OrderNo");
            }
        }

        private string _ComponentCode;
        public string ComponentCode
        {
            get
            {
                return _ComponentCode;
            }
            set
            {
                _ComponentCode = value;
                OnPropertyChanged("ComponentCode");
            }
        }
    }
}
