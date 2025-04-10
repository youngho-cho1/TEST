using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Shapes;
using LGCNS.iPharmMES.Common;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.ServiceModel;

namespace WMS
{
    public class WMSInoutViewModel : ViewModelBase
    {

        #region[Property]

        WMSInOut _mainWnd;
        OrderBatchnoPopup _Orderbatch;

        private string _SelectedScale = "BN-OS-005";
        private DispatcherTimer _repeater;
        private int _repeaterInterval = 5000;   // 100ms -> 500ms -> 1000ms
        private string _SUnit;
        public string _OutType = "FILLED";
        public bool _ScaleOnOff = false;

        private bool _IsBusyA;
        [Browsable(false)]
        public bool IsBusyA
        {
            get
            {
                return _IsBusyA;
            }
            set
            {
                if (_IsBusyA != value)
                {
                    _IsBusyA = value;
                    OnPropertyChanged("IsBusyA");
                }
            }
        }

        private bool _IsBusyP1;
        [Browsable(false)]
        public bool IsBusyP1
        {
            get
            {
                return _IsBusyP1;
            }
            set
            {
                if (_IsBusyP1 != value)
                {
                    _IsBusyP1 = value;
                    OnPropertyChanged("IsBusyP1");
                }
            }
        }

        private bool _IsBusyP2;
        [Browsable(false)]
        public bool IsBusyP2
        {
            get
            {
                return _IsBusyP2;
            }
            set
            {
                if (_IsBusyP2 != value)
                {
                    _IsBusyP2 = value;
                    OnPropertyChanged("IsBusyP2");
                }
            }
        }

        private string _txtBarcode;
        public string txtBarcode
        {
            get { return _txtBarcode; }
            set
            {
                _txtBarcode = value;
                OnPropertyChanged("txtBarcode");
            }
        }

        private string _txtProductCode;
        public string txtProductCode
        {
            get { return _txtProductCode; }
            set
            {
                _txtProductCode = value;
                OnPropertyChanged("txtProductCode");
            }
        }

        private string _txtProdName;
        public string txtProdName
        {
            get { return _txtProdName; }
            set
            {
                _txtProdName = value;
                OnPropertyChanged("txtProdName");
            }
        }

        private string _txtOrderNo;
        public string txtOrderNo
        {
            get { return _txtOrderNo; }
            set
            {
                _txtOrderNo = value;
                OnPropertyChanged("txtOrderNo");
            }
        }

        private string _txtBatchoNop;
        public string txtBatchoNop
        {
            get { return _txtBatchoNop; }
            set
            {
                _txtBatchoNop = value;
                OnPropertyChanged("txtBatchoNop");
            }
        }

        private string _txtOp;
        public string txtOp
        {
            get { return _txtOp; }
            set
            {
                _txtOp = value;
                OnPropertyChanged("txtOp");
            }
        }

        private string _txtProductDate;
        public string txtProductDate
        {
            get { return _txtProductDate; }
            set
            {
                _txtProductDate = value;
                OnPropertyChanged("txtProductDate");
            }
        }

        private string _txtIBCIDp;
        public string txtIBCIDp
        {
            get { return _txtIBCIDp; }
            set
            {
                _txtIBCIDp = value;
                OnPropertyChanged("txtIBCIDp");
            }
        }

        private string _txtTotalWt;
        public string txtTotalWt
        {
            get { return _txtTotalWt; }
            set
            {
                _txtTotalWt = value;
                OnPropertyChanged("txtTotalWt");
            }
        }

        private string _txtBintWt;
        public string txtBintWt
        {
            get { return _txtBintWt; }
            set
            {
                _txtBintWt = value;
                OnPropertyChanged("txtBintWt");
            }
        }

        private string _txtGoodWt;
        public string txtGoodWt
        {
            get { return _txtGoodWt; }
            set
            {
                _txtGoodWt = value;
                OnPropertyChanged("txtGoodWt");
            }
        }

        private string _txtBatchNo;
        public string txtBatchNo
        {
            get { return _txtBatchNo; }
            set
            {
                _txtBatchNo = value;
                OnPropertyChanged("txtBatchNo");
            }
        }

        private string _txtIBCID;
        public string txtIBCID
        {
            get { return _txtIBCID; }
            set
            {
                _txtIBCID = value;
                OnPropertyChanged("txtIBCID");
            }
        }

        private string _txtPrevOpW;
        public string txtPrevOpW
        {
            get { return _txtPrevOpW; }
            set
            {
                _txtPrevOpW = value;
                OnPropertyChanged("txtPrevOpW");
            }
        }

        private string _txtSubTotalWT;
        public string txtSubTotalWT
        {
            get { return _txtSubTotalWT; }
            set
            {
                _txtSubTotalWT = value;
                OnPropertyChanged("txtSubTotalWT");
            }
        }

        private string _txtGoodQty;
        public string txtGoodQty
        {
            get { return _txtGoodQty; }
            set
            {
                _txtGoodQty = value;
                OnPropertyChanged("txtGoodQty");
            }
        }

        private string _txtBintType;
        public string txtBintType
        {
            get { return _txtBintType; }
            set
            {
                _txtBintType = value;
                OnPropertyChanged("txtBintType");
            }
        }

        private string _txtWSDTTM;
        public string txtWSDTTM
        {
            get { return _txtWSDTTM; }
            set
            {
                _txtWSDTTM = value;
                OnPropertyChanged("txtWSDTTM");
            }
        }

        private string _txtProdInfoOut;
        public string txtProdInfoOut
        {
            get { return _txtProdInfoOut; }
            set
            {
                _txtProdInfoOut = value;
                OnPropertyChanged("txtProdInfoOut");
            }

        }

        private string _txtOrderNoOut;
        public string txtOrderNoOut
        {
            get { return _txtOrderNoOut; }
            set
            {
                _txtOrderNoOut = value;
                OnPropertyChanged("txtOrderNoOut");
            }

        }

        private string _txtBatchNoOut;
        public string txtBatchNoOut
        {
            get { return _txtBatchNoOut; }
            set
            {
                _txtBatchNoOut = value;
                OnPropertyChanged("txtBatchNoOut");
            }

        }

        private string _txtProcessOut;
        public string txtProcessOut
        {
            get { return _txtProcessOut; }
            set
            {
                _txtProcessOut = value;
                OnPropertyChanged("txtProcessOut");
            }

        }

        private string _txtIBCIDOutput;
        public string txtIBCIDOutput
        {
            get { return _txtIBCIDOutput; }
            set
            {
                _txtIBCIDOutput = value;
                OnPropertyChanged("txtIBCIDOutput");
            }

        }

        private string _txtBinWtOut;
        public string txtBinWtOut
        {
            get { return _txtBinWtOut; }
            set
            {
                _txtBinWtOut = value;
                OnPropertyChanged("txtBinWtOut");
            }

        }

        private string _txtGoodWtout;
        public string txtGoodWtout
        {
            get { return _txtGoodWtout; }
            set
            {
                _txtGoodWtout = value;
                OnPropertyChanged("txtGoodWtout");
            }

        }

        private string _tbOrderBatch;
        public string tbOrderBatch
        {
            get { return _tbOrderBatch; }
            set
            {
                _tbOrderBatch = value;
                OnPropertyChanged("tbOrderBatch");
            }

        }

        private string _txtOrderBatch;
        public string txtOrderBatch
        {
            get { return _txtOrderBatch; }
            set
            {
                _txtOrderBatch = value;
                OnPropertyChanged("txtOrderBatch");
            }

        }

        private bool _isInEble;
        public bool isInEble
        {
            get { return _isInEble; }
            set
            {
                _isInEble = value;
                OnPropertyChanged("isInEble");
            }
        }

        private bool _isOutEble;
        public bool isOutEble
        {
            get { return _isOutEble; }
            set
            {
                _isOutEble = value;
                OnPropertyChanged("isOutEble");
            }
        }

        private string _tbIncount;
        public string tbIncount
        {
            get { return _tbIncount; }
            set
            {
                _tbIncount = value;
                OnPropertyChanged("tbIncount");
            }
        }

        private string _tbOutcount;
        public string tbOutcount
        {
            get { return _tbOutcount; }
            set
            {
                _tbOutcount = value;
                OnPropertyChanged("tbOutcount");
            }
        }

        private double _fontSizeGoodWt;
        public double fontSizeGoodWt
        {
            get { return _fontSizeGoodWt; }
            set
            {
                _fontSizeGoodWt = value;
                OnPropertyChanged("fontSizeGoodWt");
            }
        }
        #endregion

        #region[Bizrule]

        private BR_BRS_SEL_BINInfo _BR_BRS_SEL_BINInfo;
        public BR_BRS_SEL_BINInfo BR_BRS_SEL_BINInfo
        {
            get { return _BR_BRS_SEL_BINInfo; }
            set
            {
                _BR_BRS_SEL_BINInfo = value;
                OnPropertyChanged("BR_BRS_SEL_BINInfo");
            }
        }

        private ObservableCollection<InputData> _InputData;
        public ObservableCollection<InputData> InputData
        {
            get { return _InputData; }
            set
            {
                _InputData = value;
                OnPropertyChanged("INPUTDATA");
            }
        }

        private InputData _SelectedItem;
        public InputData SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private BR_BRS_SEL_YieldInfo _BR_BRS_SEL_YieldInfo;
        public BR_BRS_SEL_YieldInfo BR_BRS_SEL_YieldInfo
        {
            get { return _BR_BRS_SEL_YieldInfo; }
            set
            {
                _BR_BRS_SEL_YieldInfo = value;
                OnPropertyChanged("BR_BRS_SEL_YieldInfo");
            }
        }

        private BR_BRS_REG_WMS_Request_IN _BR_BRS_REG_WMS_Request_IN;
        public BR_BRS_REG_WMS_Request_IN BR_BRS_REG_WMS_Request_IN
        {
            get { return _BR_BRS_REG_WMS_Request_IN; }
            set
            {
                _BR_BRS_REG_WMS_Request_IN = value;
                OnPropertyChanged("BR_BRS_REG_WMS_Request_IN");
            }
        }

        private BR_PHR_SEL_CurrentWeight _BR_PHR_SEL_CurrentWeight;
        public BR_PHR_SEL_CurrentWeight BR_PHR_SEL_CurrentWeight
        {
            get { return _BR_PHR_SEL_CurrentWeight; }
            set
            {
                _BR_PHR_SEL_CurrentWeight = value;
                OnPropertyChanged("BR_PHR_SEL_CurrentWeight");
            }

        }

        private ObservableCollection<OutData> _OutData;
        public ObservableCollection<OutData> OutData
        {
            get { return _OutData; }
            set
            {
                _OutData = value;
                OnPropertyChanged("OutData");
            }
        }

        private ObservableCollection<OutData> _OutData2;
        public ObservableCollection<OutData> OutData2
        {
            get { return _OutData2; }
            set
            {
                _OutData2 = value;
                OnPropertyChanged("OutData2");
            }
        }

        private OutData _SelectedData;
        public OutData SelectedData
        {
            get { return _SelectedData; }
            set
            {
                _SelectedData = value;
                OnPropertyChanged("SelectedData");
            }
        }

        private BR_BRS_SEL_ProductionOrder_IBCList _BR_BRS_SEL_ProductionOrder_IBCList;
        public BR_BRS_SEL_ProductionOrder_IBCList BR_BRS_SEL_ProductionOrder_IBCList
        {
            get { return _BR_BRS_SEL_ProductionOrder_IBCList; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_IBCList = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_IBCList");
            }
        }

        private BR_PHR_SEL_ProductionOrderDetail_Info _BR_PHR_SEL_ProductionOrderDetail_Info;
        public BR_PHR_SEL_ProductionOrderDetail_Info BR_PHR_SEL_ProductionOrderDetail_Info
        {
            get { return _BR_PHR_SEL_ProductionOrderDetail_Info; }
            set
            {
                _BR_PHR_SEL_ProductionOrderDetail_Info = value;
                OnPropertyChanged("BR_PHR_SEL_ProductionOrderDetail_Info");
            }
        }

        private BR_PHR_SEL_EquipmentGroup _BR_PHR_SEL_EquipmentGroup;
        public BR_PHR_SEL_EquipmentGroup BR_PHR_SEL_EquipmentGroup
        {
            get { return _BR_PHR_SEL_EquipmentGroup; }
            set
            {
                _BR_PHR_SEL_EquipmentGroup = value;
                OnPropertyChanged("BR_PHR_SEL_EquipmentGroup");
            }

        }

        private BR_PHR_SEL_EquipmentGroup.OUTDATA _CboTypeItem;
        public BR_PHR_SEL_EquipmentGroup.OUTDATA CboTypeitem
        {
            get { return _CboTypeItem; }
            set
            {
                _CboTypeItem = value;
                OnPropertyChanged("CboTypeitem");
            }
        }

        private BR_BRS_SEL_ProductionOrder_OrderList _BR_BRS_SEL_ProductionOrder_OrderList;
        public BR_BRS_SEL_ProductionOrder_OrderList BR_BRS_SEL_ProductionOrder_OrderList
        {
            get { return _BR_BRS_SEL_ProductionOrder_OrderList; }
            set
            {
                _BR_BRS_SEL_ProductionOrder_OrderList = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrder_OrderList");
            }
        }

        private BR_BRS_REG_WMS_Request_OUT _BR_BRS_REG_WMS_Request_OUT;
        public BR_BRS_REG_WMS_Request_OUT BR_BRS_REG_WMS_Request_OUT
        {
            get { return _BR_BRS_REG_WMS_Request_OUT; }
            set
            {
                _BR_BRS_REG_WMS_Request_OUT = value;
                OnPropertyChanged("BR_BRS_REG_WMS_Request_OUT");
            }
        }

        #endregion

        #region[OnExecuteCompleted]

        void _BR_BRS_SEL_BINInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_BINInfo.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_SEL_BINInfo.OUTDATAs)
                {
                    switch (item.TYPE.ToUpper())
                    {
                        case "FILLED" :
                            var popup = new MaterialPopup();
                            popup.DataContext = this;
                            txtProductCode = item.MTRLID;
                            txtProdName = item.MTRLNAME;
                            txtOrderNo = item.POID;
                            txtBatchoNop = item.BATCHNO;
                            txtOp = item.OPSGNAME;
                            txtProductDate = Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm");
                            txtIBCIDp = item.VESSELID;
                            txtBintWt = item.TAREWEIGHT.ToString();
                            txtGoodWt = "";
                            txtTotalWt = "";

                            popup.OKButton.Click += (s, e) =>
                            {
                                if (!_ScaleOnOff)
                                {
                                    _InputData.Add(new InputData
                                    {
                                        RowIndex = (InputData.Count + 1).ToString(),
                                        RowEditSec = "SEL",
                                        RowLoadedFlag = false,
                                        TYPE = item.TYPE,
                                        VESSELID = item.VESSELID,
                                        VESSELTYPE = item.VESSELTYPE,
                                        VESSELNAME = item.VESSELNAME,
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        POID = item.POID,
                                        BATCHNO = item.BATCHNO,
                                        OPSGGUID = item.OPSGGUID,
                                        OPSGNAME = item.OPSGNAME,
                                        PRODDTTM = Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        WSDTTM = Convert.ToDateTime(item.WSDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        TAREWEIGHT = item.TAREWEIGHT,
                                        TOTALWEIGHT = 0,
                                        LOTWEIGHT = 0
                                    });
                                }
                                else
                                {
                                    _InputData.Add(new InputData
                                    {
                                        RowIndex = (InputData.Count + 1).ToString(),
                                        RowEditSec = "SEL",
                                        RowLoadedFlag = false,
                                        TYPE = item.TYPE,
                                        VESSELID = item.VESSELID,
                                        VESSELTYPE = item.VESSELTYPE,
                                        VESSELNAME = item.VESSELNAME,
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        POID = item.POID,
                                        BATCHNO = item.BATCHNO,
                                        OPSGGUID = item.OPSGGUID,
                                        OPSGNAME = item.OPSGNAME,
                                        PRODDTTM = Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        WSDTTM = Convert.ToDateTime(item.WSDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        TAREWEIGHT = item.TAREWEIGHT,
                                        TOTALWEIGHT = txtTotalWt != null && txtTotalWt.Replace(_SUnit, "").Length > 0 ? decimal.Parse(txtTotalWt.Replace(_SUnit, "")) : 0,
                                        LOTWEIGHT = txtGoodWt != null && txtGoodWt.Replace(_SUnit, "").Length > 0 ? decimal.Parse(txtGoodWt.Replace(_SUnit, "")) : 0
                                    });
                                }


                                if (InputData.Count > 0)
                                {
                                    _mainWnd.dgList.SelectedIndex = 0;
                                }

                                if (_repeater != null)
                                {
                                    _repeater.Stop();
                                    _repeater = null;
                                }
                            };

                            popup.CancelButton.Click += (s, e) =>
                            {
                                if (_repeater != null)
                                {
                                    _repeater.Stop();
                                    _repeater = null;
                                }
                            };

                            popup.Show();

                            break;

                        case "EMPTY" :
                            var popup2 = new BinPalletPopup();
                            popup2.DataContext = this;
                            txtIBCIDp = item.VESSELID;
                            txtBintType = item.VESSELTYPE;
                            txtWSDTTM = Convert.ToDateTime(item.WSDTTM).ToString("yyyy-MM-dd HH:mm");
                            txtBintWt = "";

                            popup2.OKButton.Click += (s, e) =>
                            {
                                if (!_ScaleOnOff)
                                {
                                    _InputData.Add(new InputData
                                    {
                                        RowIndex = (InputData.Count + 1).ToString(),
                                        RowEditSec = "SEL",
                                        RowLoadedFlag = false,
                                        TYPE = item.TYPE,
                                        VESSELID = item.VESSELID,
                                        VESSELTYPE = item.VESSELTYPE,
                                        VESSELNAME = item.VESSELNAME,
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        POID = item.POID,
                                        BATCHNO = item.BATCHNO,
                                        OPSGGUID = item.OPSGGUID,
                                        OPSGNAME = item.OPSGNAME,
                                        PRODDTTM = Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        WSDTTM = Convert.ToDateTime(item.WSDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        TAREWEIGHT = item.TAREWEIGHT,
                                        TOTALWEIGHT = 0,
                                        LOTWEIGHT = 0
                                    });
                                }
                                else
                                {
                                    _InputData.Add(new InputData
                                    {
                                        RowIndex = (InputData.Count + 1).ToString(),
                                        RowEditSec = "SEL",
                                        RowLoadedFlag = false,
                                        TYPE = item.TYPE,
                                        VESSELID = item.VESSELID,
                                        VESSELTYPE = item.VESSELTYPE,
                                        VESSELNAME = item.VESSELNAME,
                                        MTRLID = item.MTRLID,
                                        MTRLNAME = item.MTRLNAME,
                                        POID = item.POID,
                                        BATCHNO = item.BATCHNO,
                                        OPSGGUID = item.OPSGGUID,
                                        OPSGNAME = item.OPSGNAME,
                                        PRODDTTM = Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        WSDTTM = Convert.ToDateTime(item.WSDTTM).ToString("yyyy-MM-dd HH:mm"),
                                        TAREWEIGHT = item.TAREWEIGHT,
                                        TOTALWEIGHT = txtBintWt != null && txtBintWt.Length > 0 ? decimal.Parse(_txtBintWt.Replace(_SUnit, "")) : 0,
                                        LOTWEIGHT = txtBintWt != null && txtBintWt.Replace(_SUnit, "").Length > 0 ? decimal.Parse(txtBintWt.Replace(_SUnit, "")) : 0
                                    });
                                }

                                if (InputData.Count > 0)
                                {
                                    _mainWnd.dgList.SelectedIndex = 0;
                                }

                                if (_repeater != null)
                                {
                                    _repeater.Stop();
                                    _repeater = null;
                                }

                            };

                            popup2.CancelButton.Click += (s, e) =>
                            {
                                if (_repeater != null)
                                {
                                    _repeater.Stop();
                                    _repeater = null;
                                }
                            };

                            break;
                    }
                }
            }
        }
        
        void _BR_BRS_SEL_YieldInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_YieldInfo.OUTDATAs.Count > 0)
            {
                foreach(var item in _BR_BRS_SEL_YieldInfo.OUTDATAs)
                {
                    txtIBCID = _SelectedItem.VESSELID;
                    txtBatchNo = _SelectedItem.BATCHNO;
                    txtPrevOpW = item.PREOPSGWEIGHT.ToString();
                    txtSubTotalWT = item.TOTALWEIGHT.ToString()+ _SUnit;
                    txtGoodQty = item.YIELD + "%";
                }
            }
        }

        void _BR_BRS_REG_WMS_Request_IN_OnExecuteCompleted(string ruleName)
        {
        }
        
        void _BR_BRS_SEL_ProductionOrder_IBCList_OnExecuteCompleted(string ruleName)
        {
            OutData2.Clear();
            if (_BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs)
                {
                    OutData2.Add(new OutData
                    {
                        RowEditSec = item.RowEditSec,
                        RowIndex = item.RowIndex,
                        RowLoadedFlag = item.RowLoadedFlag,
                        EQPTID = item.EQPTID,
                        EQPTNAME = item.EQPTNAME,
                        PRODDTTM = item.PRODDTTM != null && item.PRODDTTM.Length > 0 ? Convert.ToDateTime(item.PRODDTTM).ToString("yyyy-MM-dd HH:mm") : null,
                        WASHINGDTTM = item.WASHINGDTTM != null && item.WASHINGDTTM.Length > 0 ? Convert.ToDateTime(item.WASHINGDTTM).ToString("yyyy-MM-dd HH:mm") : null,
                        MTRLID = item.MTRLID,
                        MTRLNAME = item.MTRLNAME,
                        OUTTYPE = _OutType == "FILLED" ? "반제품" : "빈용기",
                        IBC = item.EQPTID + " " + item.EQPTNAME,
                        CHK = false
                    });
                }
            }
        }

        #endregion

        #region [Command]

        public ICommand LoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["LoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["LoadCommand"] = false;
                            CommandCanExecutes["LoadCommand"] = false;

                            ///
                            if (arg != null) _mainWnd = arg as WMSInOut;

                            txtBarcode = "";
                            _InputData.Clear();
                            txtBatchNo = "";
                            txtIBCID = "";
                            txtPrevOpW = "";
                            txtSubTotalWT = "";
                            txtGoodQty = "";
                            _mainWnd.txtBarcodes.Focus();
                            isInEble = false;
                            isOutEble = true;
                            tbIncount = "0 건";
                            tbOutcount = "0 건";
                            ///

                            CommandResults["LoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["LoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["LoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("LoadCommand") ?
                        CommandCanExecutes["LoadCommand"] : (CommandCanExecutes["LoadCommand"] = true);
                });
            }
        }

        public ICommand ScanBtnCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanBtnCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["ScanBtnCommand"] = false;
                            CommandCanExecutes["ScanBtnCommand"] = false;

                            ///
                            if (arg == null) return;

                            if (arg.ToString().Length == 0)
                            {
                                OnMessage(string.Format("{0} [ {1} ]", MessageTable.M[CommonMessageCode._10013].ToString().Replace("%1", "Barcode"), "Barcode"));
                                return;
                            }
                            
                            _BR_BRS_SEL_BINInfo.INDATAs.Clear();
                            _BR_BRS_SEL_BINInfo.OUTDATAs.Clear();
                            _BR_BRS_SEL_BINInfo.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_BINInfo.INDATA
                            {
                                VESSELID = arg.ToString()
                            });

                            await _BR_BRS_SEL_BINInfo.Execute();

                            txtBarcode = "";
                            _mainWnd.txtBarcodes.Focus();
                            ///

                            CommandResults["ScanBtnCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanBtnCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanBtnCommand"] = true;

                            IsBusy = false;

                            isInEble = false;
                            isOutEble = true;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanBtnCommand") ?
                        CommandCanExecutes["ScanBtnCommand"] : (CommandCanExecutes["ScanBtnCommand"] = true);
                });
            }
        }

        public ICommand initialInputCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["initialInputCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["initialInputCommand"] = false;
                            CommandCanExecutes["initialInputCommand"] = false;

                            ///
                            _InputData.Clear();
                            txtIBCID = "";
                            txtBatchNo = "";
                            txtPrevOpW = "";
                            txtSubTotalWT = "";
                            txtGoodQty = "";
                            tbIncount = "0 건";
                            ///

                            CommandResults["initialInputCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["initialInputCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["initialInputCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("initialInputCommand") ?
                        CommandCanExecutes["initialInputCommand"] : (CommandCanExecutes["initialInputCommand"] = true);
                });
            }
        }

        public ICommand RemoveCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RemoveCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["RemoveCommand"] = false;
                            CommandCanExecutes["RemoveCommand"] = false;

                            ///
                            if (arg != null)
                            {
                                var temp = arg as InputData;

                                var matched = _InputData.Where(o => o.VESSELID == temp.VESSELID).FirstOrDefault();
                                if (matched != null) _InputData.Remove(matched);

                                _tbIncount = _InputData.Count.ToString() + " 건";
                                OnPropertyChanged("tbIncount");

                                if (_InputData.Count == 0)
                                {
                                    txtIBCID = "";
                                    txtBatchNo = "";
                                    txtPrevOpW = "";
                                    txtSubTotalWT = "";
                                    txtGoodQty = "";
                                }
                            }
                            ///

                            CommandResults["RemoveCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RemoveCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RemoveCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RemoveCommand") ?
                        CommandCanExecutes["RemoveCommand"] : (CommandCanExecutes["RemoveCommand"] = true);
                });
            }
        }

        public ICommand SelectChangeCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelectChangeCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SelectChangeCommand"] = false;
                            CommandCanExecutes["SelectChangeCommand"] = false;

                            ///
                            if (arg != null)
                            {
                                var tmparam = arg as C1.Silverlight.DataGrid.C1DataGrid;

                                _SelectedItem = tmparam.SelectedItem as InputData;

                                if (_SelectedItem != null)
                                {
                                    _BR_BRS_SEL_YieldInfo.INDATAs.Clear();
                                    _BR_BRS_SEL_YieldInfo.OUTDATAs.Clear();
                                    _BR_BRS_SEL_YieldInfo.INDATAs.Add(new BR_BRS_SEL_YieldInfo.INDATA
                                    {
                                        VESSELID = _SelectedItem.VESSELID
                                    });

                                    await _BR_BRS_SEL_YieldInfo.Execute();
                                }
                                else
                                {
                                    txtIBCID = "";
                                    txtBatchNo = "";
                                    txtPrevOpW = "";
                                    txtSubTotalWT = "";
                                }
                            }

                            ///

                            CommandResults["SelectChangeCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SelectChangeCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SelectChangeCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelectChangeCommand") ?
                        CommandCanExecutes["SelectChangeCommand"] : (CommandCanExecutes["SelectChangeCommand"] = true);
                });
            }
        }

        public ICommand SaveInputCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveInputCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SaveInputCommand"] = false;
                            CommandCanExecutes["SaveInputCommand"] = false;

                            ///

                            var authelper = new iPharmAuthCommandHelper();

                            authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_InOut");

                            if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                , Common.enumAccessType.Create
                                , "WMS 출고"
                                , "WMS 출고"
                                , false
                                , "EM_Equipment_InOut"
                                , ""
                                , null
                                , null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            int Chk = 0;
                            foreach (var item in _InputData)
                            {
                                if (item.LOTWEIGHT == 0)
                                {
                                    Chk = 1;
                                    break;
                                }

                                _BR_BRS_REG_WMS_Request_IN.INDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_IN.OUTDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_IN.INDATAs.Add(new BR_BRS_REG_WMS_Request_IN.INDATA
                                {
                                    VESSELID = item.VESSELID
                                });                                    
                            }
                            if (Chk == 0)
                                await _BR_BRS_REG_WMS_Request_IN.Execute();

                            ///

                            CommandResults["SaveInputCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveInputCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveInputCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveInputCommand") ?
                        CommandCanExecutes["SaveInputCommand"] : (CommandCanExecutes["SaveInputCommand"] = true);
                });
            }
        }

        public ICommand MMLoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["MMLoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["MMLoadCommand"] = false;
                            CommandCanExecutes["MMLoadCommand"] = false;

                            ///

                            if (_SelectedScale != null)
                            {
                                if (_repeater == null || _repeater.IsEnabled == false)
                                {
                                    _repeater = new DispatcherTimer();
                                    _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
                                    _repeater.Tick += async (s, e) =>
                                    {
                                        try
                                        {
                                            _BR_PHR_SEL_CurrentWeight.INDATAs.Clear();
                                            _BR_PHR_SEL_CurrentWeight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                                            {
                                                ScaleID = _SelectedScale
                                            });

                                            if (await _BR_PHR_SEL_CurrentWeight.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent, Common.enumBizRuleInDataParsingType.Property, false) && _BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                            {
                                                string curWeight = string.Format("{0:F3}", _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight);
                                                this._SUnit = _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                                if (_repeater != null && _repeater.IsEnabled)
                                                {
                                                    _ScaleOnOff = true;
                                                    fontSizeGoodWt = 40;
                                                    txtGoodWt = "";
                                                    txtGoodWt = curWeight + " " + _SUnit;
                                                    if (txtBintWt.IndexOf(_SUnit) < 0)
                                                        txtBintWt = txtBintWt + " " + _SUnit;

                                                    if (txtGoodWt.Replace(_SUnit, "").Length > 0 && txtBintWt.Replace(_SUnit, "").Length > 0)
                                                        txtTotalWt = (decimal.Parse(txtGoodWt.Replace(_SUnit, "").Replace(" ", "")) + decimal.Parse(txtBintWt.Replace(_SUnit, "").Replace(" ", ""))).ToString() + " " + _SUnit;
                                                }
                                                
                                            }
                                            else
                                            {
                                                txtGoodWt = "(저울중량) - 통신오류";
                                                fontSizeGoodWt = 25;
                                                _SUnit = "";
                                                _ScaleOnOff = false;
                                            }
                                        }
                                        catch (TimeoutException er)
                                        {
                                            _repeater.Stop();
                                            _repeater = null;
                                            OnMessage(er.Message);
                                        }
                                        catch (FaultException ef)
                                        {
                                            _repeater.Stop();
                                            _repeater = null;
                                            OnMessage(ef.Message);
                                        }
                                    };
                                    _repeater.Start();
                                }
                                else
                                {
                                    _repeater.Stop();
                                    Thread.Sleep(100);
                                    _repeater.Start();
                                }
                            }
                            ///

                            CommandResults["MMLoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["MMLoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["MMLoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("MMLoadCommand") ?
                        CommandCanExecutes["MMLoadCommand"] : (CommandCanExecutes["MMLoadCommand"] = true);
                });
            }
        }

        public ICommand BINLoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BINLoadCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["BINLoadCommand"] = false;
                            CommandCanExecutes["BINLoadCommand"] = false;

                            ///

                            if (_SelectedScale != null)
                            {
                                if (_repeater == null || _repeater.IsEnabled == false)
                                {
                                    _repeater = new DispatcherTimer();
                                    _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
                                    _repeater.Tick += async (s, e) =>
                                    {
                                        try
                                        {
                                            _BR_PHR_SEL_CurrentWeight.INDATAs.Clear();
                                            _BR_PHR_SEL_CurrentWeight.INDATAs.Add(new BR_PHR_SEL_CurrentWeight.INDATA()
                                            {
                                                ScaleID = _SelectedScale
                                            });

                                            if (await _BR_PHR_SEL_CurrentWeight.Execute(Common.enumBizRuleOutputClearMode.Always, Common.enumBizRuleXceptionHandleType.FailEvent, Common.enumBizRuleInDataParsingType.Property, false) && _BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                            {
                                                string curWeight = string.Format("{0:F3}", _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight);
                                                this._SUnit = _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                                if (_repeater != null && _repeater.IsEnabled)
                                                {
                                                    _ScaleOnOff = true;
                                                    fontSizeGoodWt = 40;
                                                    txtBintWt = curWeight + " " + _SUnit;
                                                }

                                            }
                                            else
                                            {
                                                txtBintWt = "저울중량 - 통신오류";
                                                
                                                fontSizeGoodWt = 25;
                                                _SUnit = "";
                                                _ScaleOnOff = false;
                                            }
                                        }
                                        catch (TimeoutException er)
                                        {
                                            _repeater.Stop();
                                            _repeater = null;
                                            OnMessage(er.Message);
                                        }
                                        catch (FaultException ef)
                                        {
                                            _repeater.Stop();
                                            _repeater = null;
                                            OnMessage(ef.Message);
                                        }
                                    };
                                    _repeater.Start();
                                }
                                else
                                {
                                    _repeater.Stop();
                                    Thread.Sleep(100);
                                    _repeater.Start();
                                }
                            }
                            ///

                            CommandResults["BINLoadCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BINLoadCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BINLoadCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BINLoadCommand") ?
                        CommandCanExecutes["BINLoadCommand"] : (CommandCanExecutes["BINLoadCommand"] = true);
                });
            }
        }

        public ICommand ScanOutCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["ScanOutCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyA = true;

                            CommandResults["ScanOutCommand"] = false;
                            CommandCanExecutes["ScanOutCommand"] = false;

                            ///
                            ///
                            var pop = new OutStnInfo();
                            pop.DataContext = this;
                            OutData2.Clear();

                            _BR_PHR_SEL_ProductionOrderDetail_Info.INDATAs.Clear();
                            _BR_PHR_SEL_ProductionOrderDetail_Info.OUTDATAs.Clear();
                            _BR_PHR_SEL_ProductionOrderDetail_Info.INDATAs.Add(new BR_PHR_SEL_ProductionOrderDetail_Info.INDATA
                            {
                                POID = txtOrderNoOut
                            });
                            await _BR_PHR_SEL_ProductionOrderDetail_Info.Execute();

                            _BR_PHR_SEL_EquipmentGroup.INDATAs.Clear();
                            _BR_PHR_SEL_EquipmentGroup.OUTDATAs.Clear();
                            _BR_PHR_SEL_EquipmentGroup.INDATAs.Add(new BR_PHR_SEL_EquipmentGroup.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                EQPTTYPE = "EY001"

                            });
                            await _BR_PHR_SEL_EquipmentGroup.Execute();

                            if (_BR_PHR_SEL_ProductionOrderDetail_Info.OUTDATAs.Count > 0)
                            {
                                pop.cboProcess.SelectedIndex = _BR_PHR_SEL_ProductionOrderDetail_Info.OUTDATAs.Where(o => o.OPSGNAME == txtProcessOut).Select(o => Convert.ToInt32(o.RowIndex)).FirstOrDefault();
                            }

                            if (_OutType == "FILLED")
                            {
                                
                            }
                            else
                            {
                                
                            }
                            pop.OKButton.Click += (s, e) =>
                                {
                                    OutData.Clear();
                                    foreach (var item in OutData2)
                                    {
                                        if (item.CHK)
                                        {
                                            OutData.Add(new OutData
                                            {
                                               RowEditSec = item.RowEditSec,
                                               RowLoadedFlag = item.RowLoadedFlag,
                                               RowIndex = (OutData.Count +1).ToString(),
                                               EQPTID = item.EQPTID,
                                               EQPTNAME = item.EQPTNAME,
                                               PRODDTTM = item.PRODDTTM,
                                               WASHINGDTTM = item.WASHINGDTTM,
                                               MTRLID = item.MTRLID,
                                               MTRLNAME = item.MTRLNAME,
                                               OUTTYPE =item.OUTTYPE,
                                               IBC = item.IBC
                                            });
                                        }

                                        txtProdInfoOut = item.EQPTID + "-" + item.EQPTNAME;
                                    }
                                    if (pop.cboProcess.SelectedValue != null)
                                        txtProcessOut = (pop.cboProcess.SelectedItem as BR_PHR_SEL_ProductionOrderDetail_Info.OUTDATA).OPSGNAME;
                                    if (OutData.Count > 0)
                                        _mainWnd.dgOutputList.SelectedIndex = 0;
                                };
                            pop.Show();                            

                            CommandResults["ScanOutCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["ScanOutCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["ScanOutCommand"] = true;

                            IsBusyA = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("ScanOutCommand") ?
                        CommandCanExecutes["ScanOutCommand"] : (CommandCanExecutes["ScanOutCommand"] = true);
                });
            }
        }

        public ICommand popQryBtnCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["popQryBtnCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["popQryBtnCommand"] = false;
                            CommandCanExecutes["popQryBtnCommand"] = false;

                            ///
                            _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs.Clear();

                            _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_IBCList.INDATA
                            {
                                TYPE = _OutType,
                                EQPTGRPID = CboTypeitem.EQPTGRPID
                            });
                            await _BR_BRS_SEL_ProductionOrder_IBCList.Execute();

                            ///

                            CommandResults["popQryBtnCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["popQryBtnCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["popQryBtnCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("popQryBtnCommand") ?
                        CommandCanExecutes["popQryBtnCommand"] : (CommandCanExecutes["popQryBtnCommand"] = true);
                });
            }
        }

        public ICommand BatchNoBtnCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BatchNoBtnCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["BatchNoBtnCommand"] = false;
                            CommandCanExecutes["BatchNoBtnCommand"] = false;

                            ///

                            _Orderbatch = new OrderBatchnoPopup();
                            _Orderbatch.DataContext = this;
                            tbOrderBatch = "배치번호";
                            txtOrderBatch = txtBatchNoOut;
                            _Orderbatch.Show();

                            ///

                            CommandResults["BatchNoBtnCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BatchNoBtnCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BatchNoBtnCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BatchNoBtnCommand") ?
                        CommandCanExecutes["BatchNoBtnCommand"] : (CommandCanExecutes["BatchNoBtnCommand"] = true);
                });
            }
        }

        public ICommand OderNoBtnCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["OderNoBtnCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["OderNoBtnCommand"] = false;
                            CommandCanExecutes["OderNoBtnCommand"] = false;

                            ///

                            _Orderbatch = new OrderBatchnoPopup();
                            _Orderbatch.DataContext = this;
                            tbOrderBatch = "오더번호";
                            txtOrderBatch = txtOrderNoOut;
                            _Orderbatch.Show();

                            ///

                            CommandResults["OderNoBtnCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["OderNoBtnCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["OderNoBtnCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("OderNoBtnCommand") ?
                        CommandCanExecutes["OderNoBtnCommand"] : (CommandCanExecutes["OderNoBtnCommand"] = true);
                });
            }
        }

        public ICommand OrderBatchQryCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["OrderBatchQryCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP1 = true;

                            CommandResults["OderNoBtnCommand"] = false;
                            CommandCanExecutes["OderNoBtnCommand"] = false;

                            ///
                            _BR_BRS_SEL_ProductionOrder_OrderList.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrder_OrderList.OUTDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrder_OrderList.INDATAs.Add
                                (new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_OrderList.INDATA
                                {
                                    POID = tbOrderBatch == "오더번호" ? txtOrderBatch : null,
                                    BATCHNO = tbOrderBatch == "배치번호" ? txtOrderBatch : null
                                });
                            await _BR_BRS_SEL_ProductionOrder_OrderList.Execute();

                            ///

                            CommandResults["OrderBatchQryCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["OrderBatchQryCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["OrderBatchQryCommand"] = true;

                            IsBusyP1 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("OrderBatchQryCommand") ?
                        CommandCanExecutes["OrderBatchQryCommand"] : (CommandCanExecutes["OrderBatchQryCommand"] = true);
                });
            }
        }

        public ICommand SaveOutCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SaveOutCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyA = true;

                            CommandResults["SaveOutCommand"] = false;
                            CommandCanExecutes["SaveOutCommand"] = false;

                            ///

                            var authelper = new iPharmAuthCommandHelper();

                            authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "EM_Equipment_InOut");

                            if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                , Common.enumAccessType.Create
                                , "WMS 출고"
                                , "WMS 출고"
                                , false
                                , "EM_Equipment_InOut"
                                , ""
                                , null
                                , null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            foreach (var item in OutData)
                            {
                                _BR_BRS_REG_WMS_Request_OUT.INDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_OUT.OUTDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_OUT.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_REG_WMS_Request_OUT.INDATA
                                {
                                    VESSELID = item.EQPTID
                                });
                            }
                            await _BR_BRS_REG_WMS_Request_OUT.Execute();

                            ///

                            CommandResults["SaveOutCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SaveOutCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SaveOutCommand"] = true;

                            IsBusyA = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SaveOutCommand") ?
                        CommandCanExecutes["SaveOutCommand"] : (CommandCanExecutes["SaveOutCommand"] = true);
                });
            }
        }
        
        public ICommand RemoveOutCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["RemoveOutCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyA = true;

                            CommandResults["RemoveOutCommand"] = false;
                            CommandCanExecutes["RemoveOutCommand"] = false;

                            ///
                            if (arg != null)
                            {
                                var temp = arg as OutData;
                                var matched = _OutData.Where(o => o.EQPTID == temp.EQPTID && o.RowIndex == temp.RowIndex).FirstOrDefault();
                                if (matched != null) _OutData.Remove(matched);

                                _tbOutcount = _OutData.Count.ToString() + " 건";
                                OnPropertyChanged("tbOutcount");

                                if (_OutData.Count == 0)
                                {
                                    txtIBCIDOutput = "";
                                }
                            }
                                                         

                            ///

                            CommandResults["RemoveOutCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["RemoveOutCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["RemoveOutCommand"] = true;

                            IsBusyA = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("RemoveOutCommand") ?
                        CommandCanExecutes["RemoveOutCommand"] : (CommandCanExecutes["RemoveOutCommand"] = true);
                });
            }
        }

        public ICommand SelectOutChangeCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SelectOutChangeCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyA = true;

                            CommandResults["SelectOutChangeCommand"] = false;
                            CommandCanExecutes["SelectOutChangeCommand"] = false;

                            ///
                            if (arg != null)
                            {
                                var tmparam = arg as C1.Silverlight.DataGrid.C1DataGrid;

                                _SelectedData = tmparam.SelectedItem as OutData;

                                if (_SelectedData != null)
                                {
                                    txtIBCIDOutput = _SelectedData.EQPTID + " -" + _SelectedData.EQPTNAME;
                                }
                            }

                            ///

                            CommandResults["SelectOutChangeCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SelectOutChangeCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SelectOutChangeCommand"] = true;

                            IsBusyA = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SelectOutChangeCommand") ?
                        CommandCanExecutes["SelectOutChangeCommand"] : (CommandCanExecutes["SelectOutChangeCommand"] = true);
                });
            }
        }
        #endregion

        #region[Generator]

        public WMSInoutViewModel()
        {
            _BR_BRS_SEL_BINInfo = new BR_BRS_SEL_BINInfo();
            _BR_BRS_SEL_BINInfo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_BINInfo_OnExecuteCompleted);
            _BR_BRS_SEL_YieldInfo = new BR_BRS_SEL_YieldInfo();
            _BR_BRS_SEL_YieldInfo.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_YieldInfo_OnExecuteCompleted);
            _BR_BRS_REG_WMS_Request_IN = new BR_BRS_REG_WMS_Request_IN();
            _BR_BRS_REG_WMS_Request_IN.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_REG_WMS_Request_IN_OnExecuteCompleted);
            _BR_PHR_SEL_CurrentWeight = new BR_PHR_SEL_CurrentWeight();
            _BR_BRS_SEL_ProductionOrder_IBCList = new BR_BRS_SEL_ProductionOrder_IBCList();
            _BR_BRS_SEL_ProductionOrder_IBCList.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_SEL_ProductionOrder_IBCList_OnExecuteCompleted);
            _OutData2 = new ObservableCollection<OutData>();
            _BR_PHR_SEL_ProductionOrderDetail_Info = new BR_PHR_SEL_ProductionOrderDetail_Info();
            _BR_PHR_SEL_EquipmentGroup = new BR_PHR_SEL_EquipmentGroup();
            _CboTypeItem = new LGCNS.iPharmMES.Common.BR_PHR_SEL_EquipmentGroup.OUTDATA();
            _BR_BRS_SEL_ProductionOrder_OrderList = new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_OrderList();
            _OutData = new ObservableCollection<OutData>();
            _SelectedData = new OutData();
            _InputData = new ObservableCollection<InputData>();
            _SelectedItem = new InputData();
            _BR_BRS_REG_WMS_Request_OUT = new LGCNS.iPharmMES.Common.BR_BRS_REG_WMS_Request_OUT();
            txtIBCID = "";
            txtBatchNo = "";
            txtPrevOpW = "";
            txtSubTotalWT = "";
            txtGoodQty = "";
            txtBarcode = "";
            txtProductCode = "";
            txtProdName = "";
            txtOrderNo = "";
            txtBatchoNop = "";
            txtOp = "";
            txtProductDate = "";
            txtIBCIDp = "";
            txtBintWt = "";
            txtBintType = "";
            txtWSDTTM = "";
            txtProdInfoOut = "";
            txtBatchNoOut = "";
            txtOrderNoOut = "";
            txtProcessOut = "";
            txtIBCIDOutput = "";
            txtBinWtOut = "";
            txtGoodWt = "";
            tbOrderBatch = "";
            txtOrderBatch = "";
            fontSizeGoodWt = 25;
            _Orderbatch = new OrderBatchnoPopup();
        }

        #endregion

    }

    #region [InpuData]
    public class InputData : ViewModelBase
    {
        private bool _RowLoadedFlag;
        public bool RowLoadedFlag
        {
            get
            {
                return this._RowLoadedFlag;
            }
            set
            {
                this._RowLoadedFlag = value;
                this.OnPropertyChanged("_RowLoadedFlag");
            }
        }
        private string _RowIndex;
        public string RowIndex
        {
            get
            {
                return this._RowIndex;
            }
            set
            {
                this._RowIndex = value;
                this.OnPropertyChanged("RowIndex");
            }
        }
        private string _RowEditSec;
        public string RowEditSec
        {
            get
            {
                return this._RowEditSec;
            }
            set
            {
                this._RowEditSec = value;
                this.OnPropertyChanged("RowEditSec");
            }
        }
        private string _TYPE;
        public string TYPE
        {
            get
            {
                return this._TYPE;
            }
            set
            {
                this._TYPE = value;
                this.OnPropertyChanged("TYPE");
            }
        }
        private string _VESSELID;
        public string VESSELID
        {
            get
            {
                return this._VESSELID;
            }
            set
            {
                this._VESSELID = value;
                this.OnPropertyChanged("VESSELID");
            }
        }
        private string _VESSELTYPE;
        public string VESSELTYPE
        {
            get
            {
                return this._VESSELTYPE;
            }
            set
            {
                this._VESSELTYPE = value;             
                this.OnPropertyChanged("VESSELTYPE");
            }
        }
        private string _VESSELNAME;
        public string VESSELNAME
        {
            get
            {
                return this._VESSELNAME;
            }
            set
            {
                this._VESSELNAME = value;
                this.OnPropertyChanged("VESSELNAME");
            }
        }
        private string _MTRLID;
        public string MTRLID
        {
            get
            {
                return this._MTRLID;
            }
            set
            {
                this._MTRLID = value;
                this.OnPropertyChanged("MTRLID");
            }
        }
        private string _MTRLNAME;
        public string MTRLNAME
        {
            get
            {
                return this._MTRLNAME;
            }
            set
            {
                this._MTRLNAME = value;
                this.OnPropertyChanged("MTRLNAME");
            }
        }
        private string _POID;
        public string POID
        {
            get
            {
                return this._POID;
            }
            set
            {
                this._POID = value;
                this.OnPropertyChanged("POID");
            }
        }
        private string _BATCHNO;
        public string BATCHNO
        {
            get
            {
                return this._BATCHNO;
            }
            set
            {
                this._BATCHNO = value;
                this.OnPropertyChanged("BATCHNO");
            }
        }
        private string _OPSGGUID;
        public string OPSGGUID
        {
            get
            {
                return this._OPSGGUID;
            }
            set
            {
                this._OPSGGUID = value;
                this.OnPropertyChanged("OPSGGUID");
            }
        }
        private string _OPSGNAME;
        public string OPSGNAME
        {
            get
            {
                return this._OPSGNAME;
            }
            set
            {
                this._OPSGNAME = value;
                this.OnPropertyChanged("OPSGNAME");
            }
        }
        private string _PRODDTTM;
        public string PRODDTTM
        {
            get
            {
                return this._PRODDTTM;
            }
            set
            {
                this._PRODDTTM = value;
                this.OnPropertyChanged("PRODDTTM");
            }
        }
        private string _WSDTTM;
        public string WSDTTM
        {
            get
            {
                return this._WSDTTM;
            }
            set
            {
                this._WSDTTM = value;
                this.OnPropertyChanged("WSDTTM");
            }
        }
        private System.Nullable<decimal> _TAREWEIGHT;
        public System.Nullable<decimal> TAREWEIGHT
        {
            get
            {
                return this._TAREWEIGHT;
            }
            set
            {       
                this._TAREWEIGHT = value;
                this.OnPropertyChanged("TAREWEIGHT");
            }
        }
        private System.Nullable<decimal> _TOTALWEIGHT;
        public System.Nullable<decimal> TOTALWEIGHT
        {
            get
            {
                return this._TOTALWEIGHT;
            }
            set
            {
                this._TOTALWEIGHT = value;
                this.OnPropertyChanged("TOTALWEIGHT");
            }
        }
        private System.Nullable<decimal> _LOTWEIGHT;
        public System.Nullable<decimal> LOTWEIGHT
        {
            get
            {
                return this._LOTWEIGHT;
            }
            set
            {
                this._LOTWEIGHT = value;
                this.OnPropertyChanged("LOTWEIGHT");
            }
        }
    }
    #endregion

    #region [OutData]

    public class OutData : ViewModelBase
    {
        private bool _RowLoadedFlag;
        public bool RowLoadedFlag
        {
            get
            {
                return _RowLoadedFlag;
            }
            set
            {
                _RowLoadedFlag = value;
                OnPropertyChanged("_RowLoadedFlag");
            }
        }
        private string _RowIndex;
        public string RowIndex
        {
            get
            {
                return _RowIndex;
            }
            set
            {
                _RowIndex = value;
                OnPropertyChanged("RowIndex");
            }
        }
        private string _RowEditSec;
        public string RowEditSec
        {
            get
            {
                return _RowEditSec;
            }
            set
            {
                _RowEditSec = value;
                OnPropertyChanged("RowEditSec");
            }
        }
        private string _EQPTID;
        public string EQPTID
        {
            get
            {
                return this._EQPTID;
            }
            set
            {
                this._EQPTID = value;
                this.OnPropertyChanged("EQPTID");
            }
        }
        private string _EQPTNAME;
        public string EQPTNAME
        {
            get
            {
                return this._EQPTNAME;
            }
            set
            {
                this._EQPTNAME = value;
                this.OnPropertyChanged("EQPTNAME");
            }
        }
        private string _PRODDTTM;
        public string PRODDTTM
        {
            get
            {
                return this._PRODDTTM;
            }
            set
            {
                this._PRODDTTM = value;
                this.OnPropertyChanged("PRODDTTM");
            }
        }
        private string _WASHINGDTTM;
        public string WASHINGDTTM
        {
            get
            {
                return this._WASHINGDTTM;
            }
            set
            {
                this._WASHINGDTTM = value;
                this.OnPropertyChanged("WASHINGDTTM");
            }
        }
        private string _MTRLID;
        public string MTRLID
        {
            get
            {
                return this._MTRLID;
            }
            set
            {
                this._MTRLID = value;
                this.OnPropertyChanged("MTRLID");
            }
        }
        private string _MTRLNAME;
        public string MTRLNAME
        {
            get
            {
                return this._MTRLNAME;
            }
            set
            {
                this._MTRLNAME = value;
                this.OnPropertyChanged("MTRLNAME");
            }
        }
        private string _OUTTYPE;
        public string OUTTYPE
        {
            get { return _OUTTYPE; }
            set
            {
                _OUTTYPE = value;
                OnPropertyChanged("OUTTYPE");
            }
        }
        private string _IBC;
        public string IBC
        {
            get { return _IBC; }
            set
            {
                _IBC = value;
                OnPropertyChanged("IBC");
            }
        }
        private bool _CHK;
        public bool CHK
        {
            get { return _CHK; }
            set
            {
                _CHK = value;
                OnPropertyChanged("CHK");
            }
        }
    }
    #endregion
}
