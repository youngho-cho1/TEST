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

namespace 보령.DataModel.ContainerModel
{
    public abstract class AbstractContainerInformation : ViewModelBase
    {
        private string _ContainerNo;
        public string ContainerNo
        {
            get
            {
                return _ContainerNo;
            }
            set
            {
                _ContainerNo = value;
                OnPropertyChanged("ContainerNo");
            }
        }

        private string _BarCode;
        public string BarCode
        {
            get
            {
                return _BarCode;
            }
            set
            {
                _BarCode = value;
                OnPropertyChanged("BarCode");
            }
        }

        private decimal? _ContainerWeight;
        public virtual decimal? ContainerWeight
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

        private decimal? _MaterialWeight;
        public virtual decimal? MaterialWeight
        {
            get
            {
                return _MaterialWeight;
            }
            set
            {
                _MaterialWeight = value;
                OnPropertyChanged("MaterialWeight");
            }
        }

        public decimal? TotalWeight
        {
            get
            {
                return MaterialWeight + ContainerWeight;
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

        public AbstractContainerInformation()
        {
            this.PropertyChanged += (sender, arg) =>
                {
                    if (arg.PropertyName.Equals("ContainerWeight") || arg.PropertyName.Equals("MaterialWeight"))
                        OnPropertyChanged("TotalWeight");
                };
        }
    }
}
