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
using System.Collections.Generic;

namespace 보령.DataModel.ContainerModel
{
    public class WeighContainerInformation : AbstractContainerInformation
    {
        public IWeighContainerStrategy WeighContainerStrategy { get; set; }

        private List<UsedSourceContainerInfo> _usedContainerList;

        public override decimal? MaterialWeight
        {
            get
            {
                return WeighContainerStrategy.CalculateMaterialWeight(GetTotalUsedSourceMaterial(null), base.MaterialWeight);
            }
            set
            {
                base.MaterialWeight = value;
                OnPropertyChanged("AdjustedMaterialWeight");
            }
        }

        private decimal _CurrentPotencyCoefficient = 1;
        public decimal CurrentPotencyCoefficient
        {
            get
            {
                return _CurrentPotencyCoefficient;
            }
            set
            {
                _CurrentPotencyCoefficient = value;
                OnPropertyChanged("CurrentPotencyCoefficient");
                OnPropertyChanged("AdjustedMaterialWeight");
            }
        }

        private string _MaterialCode;
        public string MaterialCode
        {
            get
            {
                return _MaterialCode;
            }
            set
            {
                _MaterialCode = value;
                OnPropertyChanged("MaterialCode");
            }
        }

        public decimal? AdjustedMaterialWeight
        {
            get
            {
                return WeighContainerStrategy.CalculateAdjustedMaterialWeight(GetTotalAdjustedUsedSourceMaterial(), base.MaterialWeight, GetTotalUsedSourceMaterial(MaterialCode), CurrentPotencyCoefficient);
            }
        }

        public string WeighContainerNo
        {
            get
            {
                return this.ContainerNo;
            }
            set
            {
                this.ContainerNo = value;
                OnPropertyChanged("WeighContainerNo");
            }
        }

        public WeighContainerInformation()
            : base()
        {
            WeighContainerStrategy = new StdWeighContainerStrategy();
            _usedContainerList = new List<UsedSourceContainerInfo>();
            PropertyChanged += (sender, arg) =>
            {
                if (arg.Equals("ContainerNo"))
                    OnPropertyChanged("WeighContainerNo");
            };
        }

        public void AddUsedContainer(UsedSourceContainerInfo info)
        {
            _usedContainerList.Add(info);
        }

        public decimal? GetTotalUsedSourceMaterial(string materialCode)
        {
            decimal sum = 0;
            foreach (UsedSourceContainerInfo info in _usedContainerList)
            {
                if (string.IsNullOrEmpty(materialCode) || info.ComponentCode.Equals(materialCode))
                {
                    sum += info.UsedWeight;
                }
            }
            return sum;
        }

        public decimal? GetTotalAdjustedUsedSourceMaterial()
        {
            decimal sum = 0;
            foreach (UsedSourceContainerInfo info in _usedContainerList)
            {
                sum += info.AdjustedMaterialWeight;
            }
            return sum;
        }

        public UsedSourceContainerInfo[] GetUsedSourceContainers()
        {
            return _usedContainerList.ToArray();
        }
    }
}
