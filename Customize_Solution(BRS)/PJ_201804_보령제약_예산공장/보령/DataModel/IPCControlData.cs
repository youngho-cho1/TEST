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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace 보령
{
    public class IPCControlData : ViewModelBase
    {
        #region Property
        public enum IPCType
        {
            Boolean,
            Numeric,
            Friability
        }
        public enum ValidationType
        {
            None,
            Equal,
            Range
        }

        private string _OPTSGUID;
        public string OPTSGUID
        {
            get { return _OPTSGUID; }
            set
            {
                _OPTSGUID = value;
                OnPropertyChanged("OPTSGUID");
            }
        }

        private string _TSTYPE;
        public string TSTYPE
        {
            get { return _TSTYPE; }
            set
            {
                _TSTYPE = value;
                OnPropertyChanged("TSTYPE");
            }
        }

        private string _TSNAME;
        /// <summary>
        /// 검사명세
        /// </summary>
        public string TSNAME
        {
            get { return _TSNAME; }
            set
            {
                _TSNAME = value;
                OnPropertyChanged("TSNAME");
            }
        }

        private decimal? _SMPQTY;
        public decimal? SMPQTY
        {
            get { return _SMPQTY; }
            set
            {
                _SMPQTY = value;
                OnPropertyChanged("SMPQTY");
            }
        }

        private string _SMPQTYUOMID;
        public string SMPQTYUOMID
        {
            get { return _SMPQTYUOMID; }
            set
            {
                _SMPQTYUOMID = value;
                OnPropertyChanged("SMPQTYUOMID");
            }
        }

        private string _SMPQTYNOTATION;
        public string SMPQTYNOTATION
        {
            get { return _SMPQTYNOTATION; }
            set
            {
                _SMPQTYNOTATION = value;
                OnPropertyChanged("SMPQTYNOTATION");
            }
        }

        private string _OPTSIGUID;
        public string OPTSIGUID
        {
            get { return _OPTSIGUID; }
            set
            {
                _OPTSIGUID = value;
                OnPropertyChanged("OPTSIGUID");
            }
        }

        private string _TINAME;
        /// <summary>
        /// 검사항목
        /// </summary>
        public string TINAME
        {
            get { return _TINAME; }
            set
            {
                _TINAME = value;
                OnPropertyChanged("TINAME");
            }
        }

        private string _Standard;
        /// <summary>
        /// 검사 기준 : ProductionOrderTestSpecificationItems.OPTSDESC
        /// </summary>
        public string Standard
        {
            get { return _Standard; }
            set
            {
                _Standard = value;
                OnPropertyChanged("Standard");
            }
        }

        private int _PRECISION;
        public int PRECISION
        {
            get { return _PRECISION; }
            set
            {
                _PRECISION = value;
                OnPropertyChanged("PRECISION");
            }
        }

        private string _UOMID;
        public string UOMID
        {
            get { return _UOMID; }
            set
            {
                _UOMID = value;
                OnPropertyChanged("UOMID");
            }
        }
        private string _NOTATION;
        /// <summary>
        /// 단위
        /// </summary>
        public string NOTATION
        {
            get { return _NOTATION; }
            set
            {
                _NOTATION = value;
                OnPropertyChanged("NOTATION");
            }
        }

        private ValidationType _VLDType;
        public ValidationType VLDType
        {
            get { return _VLDType; }
            set
            {
                _VLDType = value;
                OnPropertyChanged("VLDType");
            }
        }

        private IPCType _Type;
        public IPCType Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                OnPropertyChanged("Type");
            }
        }

        #region Specification Limit
        private decimal? _USL;
        public decimal? USL
        {
            get { return _USL; }
            set
            {
                _USL = value;
                OnPropertyChanged("USL");
            }
        }
        private decimal? _CSL;
        public decimal? CSL
        {
            get { return _CSL; }
            set
            {
                _CSL = value;
                OnPropertyChanged("CSL");
            }
        }
        private decimal? _LSL;
        public decimal? LSL
        {
            get { return _LSL; }
            set
            {
                _LSL = value;
                OnPropertyChanged("LSL");
            }
        }
        #endregion

        #region Control Limit
        private decimal? _UCL;
        public decimal? UCL
        {
            get { return _UCL; }
            set
            {
                _UCL = value;
                OnPropertyChanged("UCL");
            }
        }
        private decimal? _CCL;
        public decimal? CCL
        {
            get { return _CCL; }
            set
            {
                _CCL = value;
                OnPropertyChanged("CCL");
            }
        }
        private decimal? _LCL;
        public decimal? LCL
        {
            get { return _LCL; }
            set
            {
                _LCL = value;
                OnPropertyChanged("LCL");
            }
        }
        #endregion

        #region Warning Limit
        private decimal? _UWL;
        public decimal? UWL
        {
            get { return _UWL; }
            set
            {
                _UWL = value;
                OnPropertyChanged("UWL");
            }
        }
        private decimal? _CWL;
        public decimal? CWL
        {
            get { return _CWL; }
            set
            {
                _CWL = value;
                OnPropertyChanged("CWL");
            }
        }
        private decimal? _LWL;
        public decimal? LWL
        {
            get { return _LWL; }
            set
            {
                _LWL = value;
                OnPropertyChanged("LWL");
            }
        }
        #endregion

        private decimal? _ACTVAL;
        /// <summary>
        /// IPC 결과값
        /// </summary>
        public decimal? ACTVAL
        {
            get { return _ACTVAL; }
            set
            {
                _ACTVAL = value;
                if (_ACTVAL.HasValue)
                    ValidationCheck();
                else
                    DEVIATIONFLAG = null;
                OnPropertyChanged("ACTVAL");
            }
        }

        private string _ACTVALDESC;
        /// <summary>
        /// IPC 결과값 설명
        /// </summary>
        public string ACTVALDESC
        {
            get { return _ACTVALDESC; }
            set
            {
                _ACTVALDESC = value;                
                OnPropertyChanged("ACTDESCRIPTION");
            }
        }

        public string GetACTVAL
        {
            get
            {
                string rslt = "N/A";

                switch (Type)
                {
                    case IPCType.Boolean:
                        if (_ACTVAL.HasValue)
                        {
                            if (_ACTVAL.GetValueOrDefault() != 0)
                                rslt = "적합";
                            else
                                rslt = "부적합";
                        }
                        else
                            rslt = "N/A";
                        break;
                    case IPCType.Numeric:
                    case IPCType.Friability:
                        if (_ACTVAL.HasValue)
                        {
                            rslt = string.Format("{0:N" + _PRECISION + "}", _ACTVAL.GetValueOrDefault()); 
                        }
                        else
                            rslt = "N/A";
                        break;
                }

                return rslt;
            }
        }
        private bool? _DEVIATIONFLAG = null;
        /// <summary>
        /// 일탈여부 null : 결과없음, true : 적합, false : 부적합
        /// </summary>
        public bool? DEVIATIONFLAG
        {
            get { return _DEVIATIONFLAG; }
            set
            {
                _DEVIATIONFLAG = value;
                OnPropertyChanged("DEVIATIONFLAG");
            }
        }

        private ObservableCollection<IPCControlRawData> _RawDatas;
        public ObservableCollection<IPCControlRawData> RawDatas
        {
            get { return _RawDatas; }
            set
            {
                _RawDatas = value;
                OnPropertyChanged("RawDatas");
            }
        }

        #endregion

        #region Function
        private void ValidationCheck()
        {
            try
            {
                switch (_VLDType)
                {
                    case ValidationType.None:
                        DEVIATIONFLAG = true;
                        break;
                    case ValidationType.Equal:
                        if (_CSL.HasValue)
                        {
                            if (_CSL.Value == _ACTVAL)
                                DEVIATIONFLAG = true;
                            else
                                DEVIATIONFLAG = false;
                        }
                        else
                        {
                            DEVIATIONFLAG = null;
                            OnMessage("기준값이 없습니다.");
                        }
                        break;
                    case ValidationType.Range:
                        if (_LSL.HasValue && _USL.HasValue)
                        {
                            if (_LSL.Value <= _ACTVAL && _ACTVAL <= _USL.Value)
                                DEVIATIONFLAG = true;
                            else
                                DEVIATIONFLAG = false;
                        }
                        else if (_LSL.HasValue)
                        {
                            if (_LSL.Value <= _ACTVAL)
                                DEVIATIONFLAG = true;
                            else
                                DEVIATIONFLAG = false;
                        }
                        else if (_USL.HasValue)
                        {
                            if (_ACTVAL <= _USL.Value)
                                DEVIATIONFLAG = true;
                            else
                                DEVIATIONFLAG = false;
                        }
                        else
                        {
                            DEVIATIONFLAG = null;
                            OnMessage("기준값이 없습니다.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        public static IPCControlData SetIPCControlData(BR_BRS_SEL_ProductionOrderIPCStandard.OUTDATA param)
        {
            try
            {
                var IPCData = new IPCControlData();


                switch (param.IPCTYPE)
                {
                    case "BOOLEAN":
                        IPCData.Type = IPCControlData.IPCType.Boolean;
                        break;
                    case "NUMERIC":
                        IPCData.Type = IPCControlData.IPCType.Numeric;
                        break;
                    case "FRIABILITY":
                        IPCData.Type = IPCControlData.IPCType.Friability;
                        break;
                }

                switch (param.VALIDTYPE)
                {
                    case "QMVLTNONE":
                        IPCData.VLDType = IPCControlData.ValidationType.None;
                        break;
                    case "QMVLTEQL":
                        IPCData.VLDType = IPCControlData.ValidationType.Equal;
                        break;
                    case "QMVLTRNG":
                        IPCData.VLDType = IPCControlData.ValidationType.Range;
                        break;
                }

                IPCData.TSNAME = param.TSNAME;
                IPCData.TINAME = param.TINAME;
                IPCData.Standard = param.STANDARD;
                IPCData.PRECISION = param.PRECISION.GetValueOrDefault();
                IPCData.UOMID = param.UOMID;
                IPCData.NOTATION = param.NOTATION;
                IPCData.USL = param.USL;
                IPCData.CSL = param.CSL;
                IPCData.LSL = param.LSL;
                IPCData.UCL = param.UCL;
                IPCData.CCL = param.CCL;
                IPCData.LCL = param.LCL;
                IPCData.UWL = param.UWL;
                IPCData.CWL = param.CWL;
                IPCData.LWL = param.LWL;
                IPCData.OPTSGUID = param.OPTSGUID;
                IPCData.OPTSIGUID = param.OPTSIGUID;
                IPCData.TSTYPE = param.TSTYPE;
                IPCData.SMPQTY = param.SMPQTY;
                IPCData.SMPQTYUOMID = param.SMPQTYUOMID;
                IPCData.SMPQTYNOTATION = param.SMPQTYNOTATION;
                IPCData.RawDatas = new ObservableCollection<IPCControlRawData>();

                return IPCData;
            }
            catch (Exception ex)
            {
                ViewModelBase.Instance.OnException(ex.Message, ex);
                return null;
            }
        }
        #endregion
    }

    public class IPCControlRawData : ViewModelBase
    {
        private string _POTSIRGUID;
         public string POTSIRGUID
        {
            get { return _POTSIRGUID; }
            set
            {
                _POTSIRGUID = value;
                OnPropertyChanged("POTSIRGUID");
            }
        }

        private string _POTSIRAWGUID;
        public string POTSIRAWGUID
        {
            get { return _POTSIRAWGUID; }
            set
            {
                _POTSIRAWGUID = value;
                OnPropertyChanged("POTSIRAWGUID");
            }
        }

        private string _COLLECTID;
        public string COLLECTID
        {
            get { return _COLLECTID; }
            set
            {
                _COLLECTID = value;
                OnPropertyChanged("COLLECTID");
            }
        }

        private string _ACTVAL;
        public string ACTVAL
        {
            get { return _ACTVAL; }
            set
            {
                _ACTVAL = value;
                OnPropertyChanged("ACTVAL");
            }
        }

        private string _REASON;
        public string REASON
        {
            get { return _REASON; }
            set
            {
                _REASON = value;
                OnPropertyChanged("REASON");
            }
        }
    }
}
