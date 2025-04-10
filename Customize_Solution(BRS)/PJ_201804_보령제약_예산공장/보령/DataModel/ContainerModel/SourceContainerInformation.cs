using System;
using System.Windows.Input;
using System.Threading.Tasks;
using LGCNS.iPharmMES.Common;


namespace 보령.DataModel.ContainerModel
{
    public class SourceContainerInformation : AbstractContainerInformation
    {
        private decimal? _bizRuleWeight;
        public decimal? BizRuleWeight
        {
            get
            {
                return _bizRuleWeight;
            }
            set
            {
                _bizRuleWeight = value;
                OnPropertyChanged("BizRuleWeight");
            }
        }
        private decimal? _totalUsedWeight;

        private decimal? _PotencyCoefficient = 1;
        public decimal? PotencyCoefficient
        {
            get
            {
                return _PotencyCoefficient;
            }
            set
            {
                _PotencyCoefficient = value;
                OnPropertyChanged("PotencyCoefficient");
            }
        }

        public decimal? AdjustedMaterialWeight
        {
            get
            {
                return MaterialWeight * PotencyCoefficient;
            }
        }

        public ISourceContainerStrategy SourceContainerStrategy { get; set; }

        public void SetToatolUsedWeight(decimal? totalUsedWeight)
        {
            _totalUsedWeight = totalUsedWeight;
            OnPropertyChanged("MaterialWeight");
        }

        public decimal? GetBizRuleWeight()
        {
            return _bizRuleWeight;
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

        public override decimal? MaterialWeight
        {
            get
            {
                return SourceContainerStrategy.CalculateSourceStrategy(_bizRuleWeight, base.MaterialWeight, _totalUsedWeight);
            }
            set
            {
                base.MaterialWeight = value;
            }
        }

        public override decimal? ContainerWeight
        {
            get
            {
                return base.ContainerWeight;
            }
            set
            {
            }
        }

        public string SourceContainerNo
        {
            get
            {
                return this.ContainerNo;
            }
            set
            {
                this.ContainerNo = value;
                OnPropertyChanged("SourceContainerNo");
            }
        }

        private string _SourceLotID;
        public string SourceLotID
        {
            get
            {
                return _SourceLotID;
            }
            set
            {
                _SourceLotID = value;
                OnPropertyChanged("SourceLotID");
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

        private int _Precision;
        public int Precision
        {
            get
            {
                return _Precision;
            }
            set
            {
                _Precision = value;
                OnPropertyChanged("Precision");
            }
        }

        public SourceContainerInformation()
            : base()
        {
            SourceContainerStrategy = new StdSourceContainerStrategy();
            PropertyChanged += (sender, arg) =>
            {
                if (arg.Equals("ContainerNo"))
                    OnPropertyChanged("SourceContainerNo");
            };
        }

        public ICommand LoadDataCmd
        {
            get
            {
                return new CommandBase(new Action<object>(async (param) =>
                {
                    WeighInformation weighInfo = param as WeighInformation;
                    //BR_PHR_CHK_MaterialSubLot_SourceContainer br = new BR_PHR_CHK_MaterialSubLot_SourceContainer();
                    BR_UDB_CHK_MaterialSubLot_SourceContainer br = new BR_UDB_CHK_MaterialSubLot_SourceContainer();
                    br.INDATAs.Clear();
                    //br.INDATAs.Add(new BR_PHR_CHK_MaterialSubLot_SourceContainer.INDATA() { LANGID = "ko-KR", MSUBLOTBCD = BarCode, POID = weighInfo.POID, COMPONENTGUID = weighInfo.ComponentGUID, ROOMID = RoomID });
                    br.INDATAs.Add(new BR_UDB_CHK_MaterialSubLot_SourceContainer.INDATA() { LANGID = "ko-KR", MSUBLOTBCD = BarCode, POID = weighInfo.POID, COMPONENTGUID = weighInfo.ComponentGUID, ROOMID = RoomID, USERID = AuthRepositoryViewModel.Instance.LoginedUserID });
                    br.OUTDATAs.Clear();

                    br.OnExecuteCompleted += (ruleName) =>
                    {
                        if (br.OUTDATAs.Count > 0)
                        {
                            ContainerNo = br.OUTDATAs[0].MSUBLOTID;
                            _bizRuleWeight = (decimal)br.OUTDATAs[0].MSUBLOTQTY.Value;
                            base.ContainerWeight = (decimal)br.OUTDATAs[0].TAREWEIGHT.Value;
                            //base.ContainerWeight = 0;

                            base.MaterialWeight = 0;
                            Potency = (double)(decimal)br.OUTDATAs[0].POTENCY.Value;
                            Unit = br.OUTDATAs[0].UOMNAME;
                            BarCode = br.OUTDATAs[0].MSUBLOTBCD;
                            MaterialCode = br.OUTDATAs[0].MTRLID;
                            Ver = br.OUTDATAs[0].MSUBLOTVER.Value;
                            Precision = br.OUTDATAs[0].PRECISION.Value;
                            SourceLotID = br.OUTDATAs[0].MLOTID;
                        }
                        else
                        {
                            ContainerNo = "";
                            _bizRuleWeight = Convert.ToDecimal(double.NaN);
                            base.ContainerWeight = Convert.ToDecimal(double.NaN);
                            base.MaterialWeight = Convert.ToDecimal(double.NaN);
                            Unit = "?";
                            Potency = 100;
                            Precision = 3;
                            SourceLotID = string.Empty;
                        }

                        if (LoadComplete != null)
                            LoadComplete();

                        OnPropertyChanged("MaterialWeight");
                    };

                    br.OnExecuteFailed += (ruleName, ex) =>
                    {
                        //if (LoadComplete != null)
                        //    LoadComplete();
                    };
                    await br.Execute();

                }
                ));
            }
        }
        

        public async Task LoadDataTrustCmd(WeighInformation param, string tBarcode)
        {           
            WeighInformation weighInfo = param as WeighInformation;
            //BR_PHR_CHK_MaterialSubLot_SourceContainer br = new BR_PHR_CHK_MaterialSubLot_SourceContainer();
            BR_UDB_CHK_MaterialSubLot_SourceContainer br = new BR_UDB_CHK_MaterialSubLot_SourceContainer();
            br.INDATAs.Clear();
            //br.INDATAs.Add(new BR_PHR_CHK_MaterialSubLot_SourceContainer.INDATA() { LANGID = "ko-KR", MSUBLOTBCD = BarCode, POID = weighInfo.POID, COMPONENTGUID = weighInfo.ComponentGUID, ROOMID = RoomID });
            br.INDATAs.Add(new BR_UDB_CHK_MaterialSubLot_SourceContainer.INDATA() { LANGID = "ko-KR", MSUBLOTBCD = tBarcode, POID = weighInfo.POID, COMPONENTGUID = weighInfo.ComponentGUID, ROOMID = RoomID, USERID = AuthRepositoryViewModel.Instance.LoginedUserID });
            br.OUTDATAs.Clear();

            if (await br.Execute() == false) return;

            if (br.OUTDATAs.Count > 0)
            {
                ContainerNo = br.OUTDATAs[0].MSUBLOTID;
                _bizRuleWeight = (decimal)br.OUTDATAs[0].MSUBLOTQTY.Value;
                base.ContainerWeight = (decimal)br.OUTDATAs[0].TAREWEIGHT.Value;
                base.MaterialWeight = 0;
                Potency = (double)(decimal)br.OUTDATAs[0].POTENCY.Value;
                Unit = br.OUTDATAs[0].UOMNAME;
                BarCode = br.OUTDATAs[0].MSUBLOTBCD;
                MaterialCode = br.OUTDATAs[0].MTRLID;
                Ver = br.OUTDATAs[0].MSUBLOTVER.Value;
                Precision = br.OUTDATAs[0].PRECISION.Value;
            }
            else
            {
                ContainerNo = "";
                _bizRuleWeight = Convert.ToDecimal(double.NaN);
                base.ContainerWeight = Convert.ToDecimal(double.NaN);
                base.MaterialWeight = Convert.ToDecimal(double.NaN);
                Unit = "?";
                Potency = 100;
                Precision = 3;
            }

            if (LoadComplete != null)
                LoadComplete();

            OnPropertyChanged("MaterialWeight");

            br.OnExecuteFailed += (ruleName, ex) =>
            {
                if (LoadComplete != null)
                    LoadComplete();
            };
        }

        public delegate void LoadCompleteHandler();
        public event LoadCompleteHandler LoadComplete;
    }
}
