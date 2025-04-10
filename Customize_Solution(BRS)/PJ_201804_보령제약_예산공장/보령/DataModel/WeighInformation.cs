using System;
using System.Windows.Input;
using LGCNS.iPharmMES.Common;


namespace 보령.DataModel
{
    public class WeighInformation : ViewModelBase
    {
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

        private string _RoomID;
        public string RoomID
        {
            get
            {
                return _RoomID;
            }
            set
            {
                _RoomID = value;
                OnPropertyChanged("RoomID");
            }
        }

        private string _WeighMethod;
        public string WeighMethod
        {
            get
            {
                return _WeighMethod;
            }
            set
            {
                _WeighMethod = value;
                OnPropertyChanged("WeighMethod");
            }
        }

        private string _VesselType;
        public string VesselType
        {
            get
            {
                return _VesselType;
            }
            set
            {
                _VesselType = value;
                OnPropertyChanged("VesselType");
            }
        }

        private string _ScalePrecision;
        public string ScalePrecision
        {
            get
            {
                return _ScalePrecision;
            }
            set
            {
                _ScalePrecision = value;
                OnPropertyChanged("ScalePrecision");
            }
        }

        private string _WeighNote;
        public string WeighNote
        {
            get
            {
                return _WeighNote;
            }
            set
            {
                _WeighNote = value;
                OnPropertyChanged("WeighNote");
            }
        }

        private decimal _OverTolerance;
        public decimal OverTolerance
        {
            get
            {
                return _OverTolerance;
            }
            set
            {
                _OverTolerance = value;
                OnPropertyChanged("OverTolerance");
            }
        }

        private decimal _UnderTolerance;
        public decimal UnderTolerance
        {
            get
            {
                return _UnderTolerance;
            }
            set
            {
                _UnderTolerance = value;
                OnPropertyChanged("UnderTolerance");
            }
        }

        public ICommand LoadDataCmd
        {
            get
            {
                return new CommandBase(new Action<object>((param) =>
                {
                    if (param is BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP.OUTDATA)
                    {
                        BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP.OUTDATA info = param as BR_PHR_SEL_ProductionOrderBOM_ComponentInfo_DSP.OUTDATA;
                        VesselType = info.EQPTGRPNAME;
                        ScalePrecision = info.TOLERANCE;
                        WeighNote = info.ATTENTIONNOTE;
                        OverTolerance = (info.OVERTOLERANCE.Value - info.REQQTY.Value) / info.REQQTY.Value;
                        UnderTolerance = (info.REQQTY.Value - info.UNDERTOLERANCE.Value) / info.REQQTY.Value;
                    }
                }
                ));
            }
        }
    }
}
