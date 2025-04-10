using System;
using System.Windows.Input;

using LGCNS.iPharmMES.Common;
using 보령.DataModel.ContainerModel;

namespace 보령.DataModel
{
    public class ComponentInformation : ViewModelBase
    {
        private WeighContainerInformation[] _weighContainerArray;
        private int _weighContainerIndex;

        public int CurrentContainerIdx
        {
            get
            {
                if (_weighContainerArray == null)
                    return 0;
                else
                    return _weighContainerIndex < _weighContainerArray.Length ? _weighContainerIndex : _weighContainerArray.Length - 1;
            }
        }

        public void NextContainer()
        {
            _weighContainerIndex++;
        }

        public int MaxContainerIndex
        {
            get
            {
                return _weighContainerArray == null ? -1 : _weighContainerArray.Length - 1;
            }
        }

        public WeighContainerInformation CurrentContainer
        {
            get
            {
                return _weighContainerArray[CurrentContainerIdx];
            }
        }

        public WeighContainerInformation GetWeighContainer(int index)
        {
            return _weighContainerArray[index];
        }

        public void SetWeighContainer(int index, WeighContainerInformation weighContainer)
        {
            if (index > -1 && index < _weighContainerArray.Length)
                _weighContainerArray[index] = weighContainer;
        }

        public decimal? AccumulatedWeight
        {
            get
            {
                decimal? sum = 0;
                foreach (WeighContainerInformation info in _weighContainerArray)
                {
                    sum += info.MaterialWeight;
                }
                return sum;
            }
        }

        public decimal? AccumulatedWeightByMaterial(string materialcode)
        {
            decimal? sum = 0;
            foreach (WeighContainerInformation info in _weighContainerArray)
            {
                sum += info.GetTotalUsedSourceMaterial(materialcode);
            }
            return sum;
        }

        public decimal? AdjustedAccumulatedWeight
        {
            get
            {
                decimal? sum = 0;
                foreach (WeighContainerInformation info in _weighContainerArray)
                {
                    sum += info.AdjustedMaterialWeight;
                }
                return sum;
            }
        }

        public int Seq
        {
            get;
            set;
        }

        private string _ComponentGUID;
        public string ComponentGUID
        {
            get
            {
                return _ComponentGUID;
            }
            set
            {
                _ComponentGUID = value;
                OnPropertyChanged("ComponentGUID");
            }
        }

        private string _OPSGGUID;
        public string OPSGGUID
        {
            get
            {
                return _OPSGGUID;
            }
            set
            {
                _OPSGGUID = value;
                OnPropertyChanged("OPSGGUID");
            }
        }

        private string _POID;
        public string POID
        {
            get
            {
                return _POID;
            }
            set
            {
                _POID = value;
                OnPropertyChanged("POID");
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

        private bool _IsPrimeMaterial;
        public bool IsPrimeMaterial
        {
            get
            {
                return _IsPrimeMaterial;
            }
            set
            {
                _IsPrimeMaterial = value;
                OnPropertyChanged("IsPrimeMaterial");
            }
        }

        private bool _IsEnableMix;
        public bool IsEnableMix
        {
            get
            {
                return _IsEnableMix;
            }
            set
            {
                _IsEnableMix = value;
                OnPropertyChanged("IsEnableMix");
            }
        }

        private string _RawMaterialName;
        public string RawMaterialName
        {
            get
            {
                return _RawMaterialName;
            }
            set
            {
                _RawMaterialName = value;
                OnPropertyChanged("RawMaterialName");
            }
        }

        private decimal _RequiredQuantity;
        public decimal RequiredQuantity
        {
            get
            {
                return _RequiredQuantity;
            }
            set
            {
                _RequiredQuantity = value;
                OnPropertyChanged("RequiredQuantity");
            }
        }

        private string _RequiredQuantityUnit;
        public string RequiredQuantityUnit
        {
            get
            {
                return _RequiredQuantityUnit;
            }
            set
            {
                _RequiredQuantityUnit = value;
                OnPropertyChanged("RequiredQuantityUnit");
            }
        }

        private string _RequiredQuantityUnitID;
        public string RequiredQuantityUnitID
        {
            get
            {
                return _RequiredQuantityUnitID;
            }
            set
            {
                _RequiredQuantityUnitID = value;
                OnPropertyChanged("RequiredQuantityUnitID");
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

        private string _VesselTypeID;
        public string VesselTypeID
        {
            get
            {
                return _VesselTypeID;
            }
            set
            {
                _VesselTypeID = value;
                OnPropertyChanged("VesselTypeID");
            }
        }

        private string _VesselTypeName;
        public string VesselTypeName
        {
            get
            {
                return _VesselTypeName;
            }
            set
            {
                _VesselTypeName = value;
                OnPropertyChanged("VesselTypeName");
            }
        }

        private string _materialLotID;
        public string MaterialLotID
        {
            get { return _materialLotID; }
            set
            {
                _materialLotID = value;
                NotifyPropertyChanged();
            }
        }

        private string _materialSublotID;
        public string MaterialSublotID
        {
            get { return _materialSublotID; }
            set
            {
                _materialSublotID = value;
                NotifyPropertyChanged();
            }
        }

        private double _realQuantity;
        public double RealQuantity
        {
            get { return _realQuantity; }
            set
            {
                _realQuantity = value;
                NotifyPropertyChanged();
            }
        }

        private string _realQuantityUnit;
        public string RealQuantityUnit
        {
            get { return _realQuantityUnit; }
            set
            {
                _realQuantityUnit = value;
                NotifyPropertyChanged();
            }
        }

        public ComponentInformation(int weighContainerSize = 1)
            : base()
        {
            _weighContainerArray = new WeighContainerInformation[weighContainerSize];
            _weighContainerIndex = 0;
            for (int idx = 0; idx < weighContainerSize; idx++)
                _weighContainerArray[idx] = new WeighContainerInformation();
        }

        public ICommand LoadDataCmd
        {
            get
            {
                return new CommandBase(new Action<object>((param) =>
                {
                    BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP br = new BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP();
                    br.INDATAs.Clear();
                    br.INDATAs.Add(new BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP.INDATA() { COMPONENTGUID = ComponentGUID, OPSGGUID = OPSGGUID, POID = POID });
                    br.OUTDATAs.Clear();
                    br.OnExecuteCompleted += (brname) =>
                        {
                            if (br.OUTDATAs.Count > 0)
                            {
                                MaterialCode = br.OUTDATAs[0].MTRLID;
                                //IsPrimeMaterial = false;
                                IsEnableMix = (br.OUTDATAs[0].ISBATCHCOMBINATION != null && br.OUTDATAs[0].ISBATCHCOMBINATION.Equals("Y")) ? true : false;
                                RawMaterialName = br.OUTDATAs[0].MTRLNAME;
                                RequiredQuantity = br.OUTDATAs[0].REQQTY.Value;
                                RequiredQuantityUnit = br.OUTDATAs[0].NOTATION;
                                RequiredQuantityUnitID = br.OUTDATAs[0].UOMID;
                                VesselTypeID = br.OUTDATAs[0].CONTAINERTYPE;
                                VesselTypeName = br.OUTDATAs[0].EQPTGRPNAME;

                                BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR br2 = new BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR();
                                br2.INDATAs.Clear();
                                br2.INDATAs.Add(new BR_PHR_SEL_MaterialCustomAttributeValue_SYS_ATTR.INDATA() { MTRLID = MaterialCode, MTATID = "SYS_M006" });
                                br2.OUTDATAs.Clear();
                                br2.OnExecuteCompleted += (br2name) =>
                                    {
                                        if (br.OUTDATAs.Count > 0)
                                        {
                                            double temp;
                                            if (double.TryParse(br2.OUTDATAs[0].MTATVAL1, out temp))
                                                Potency = temp;
                                        }

                                        if (LoadComplete != null)
                                            LoadComplete();
                                    };
                                br2.Execute();
                            }
                        };

                    br.Execute();
                }));
            }
        }

        public delegate void LoadCompleteHandler();
        public event LoadCompleteHandler LoadComplete;

        public override string ToString()
        {
            return this.Seq + ". " + this.RawMaterialName;
        }
    }
}
