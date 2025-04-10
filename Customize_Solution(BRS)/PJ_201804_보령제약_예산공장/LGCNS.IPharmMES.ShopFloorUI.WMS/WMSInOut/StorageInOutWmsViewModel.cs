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
using System.Collections.Generic;

namespace WMS
{
    public class StorageInOutWmsViewModel : ViewModelBase
    {
        #region[Property]

        StorageInOutWms _mainWnd;
        StorageInWeighing _InWeighing;
        OrderBatchnoPopup _Orderbatch;
        StorageOutWheinging _OutWeghing;

        private string _SelectedScale = "BN-OS-005";
        private DispatcherTimer _repeater;
        private int _repeaterInterval = 5000;   // 100ms -> 500ms -> 1000ms
        private string _SUnit;
        public string _OutType = "FILLED";
        public bool _ScaleOnOff = false;
        private string _OPSGUID;
        private string _OUTPUTGUID;
        public string CurrentWeghing = "";

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

        private string _txtVesselWt;
        public string txtVesselWt
        {
            get { return _txtVesselWt; }
            set
            {
                _txtVesselWt = value;
                OnPropertyChanged("txtVesselWt");
            }
        }

        private string _txtConWt;
        public string txtConWt
        {
            get { return _txtConWt; }
            set
            {
                _txtConWt = value;
                OnPropertyChanged("txtConWt");
            }
        }

        private string _txtTotalWts;
        public string txtTotalWts
        {
            get { return _txtTotalWts; }
            set
            {
                _txtTotalWts = value;
                OnPropertyChanged("txtTotalWts");
            }
        }

        private string _txtYelid;
        public string txtYelid
        {
            get { return _txtYelid; }
            set
            {
                _txtYelid = value;
                OnPropertyChanged("txtYelid");
            }
        }

        private string _txtTotalWpop;
        public string txtTotalWpop
        {
            get { return _txtTotalWpop; }
            set
            {
                _txtTotalWpop = value;
                OnPropertyChanged("txtTotalWpop");
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

        private string _tbOutcount2;
        public string tbOutcount2
        {
            get { return _tbOutcount2; }
            set
            {
                _tbOutcount2 = value;
                OnPropertyChanged("tbOutcount2");
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

        private string _txtComment;
        public string txtComment
        {
            get { return _txtComment; }
            set
            {
                _txtComment = value;
                OnPropertyChanged("txtComment");
            }
        }

        private string _txtComment2;
        public string txtComment2
        {
            get { return _txtComment2; }
            set
            {
                _txtComment2 = value;
                OnPropertyChanged("txtComment2");
            }
        }

        private string _LbClrComment;
        public string LbClrComment
        {
            get { return _LbClrComment; }
            set
            {
                _LbClrComment = value;
                OnPropertyChanged("LbClrComment");
            }
        }

        private string _LbClrComment2;
        public string LbClrComment2
        {
            get { return _LbClrComment2; }
            set
            {
                _LbClrComment2 = value;
                OnPropertyChanged("LbClrComment2");
            }
        }

        private Visibility _IsVsbTest;
        public Visibility IsVsbTest
        {
            get { return _IsVsbTest; }
            set
            {
                _IsVsbTest = value;
                OnPropertyChanged("IsVsbTest");
            }
        }

        private bool _IsIncludeUnusedData;
        public bool IsIncludeUnusedData
        {
            get { return _IsIncludeUnusedData; }
            set
            {
                _IsIncludeUnusedData = value;
                OnPropertyChanged("IsIncludeUnusedData");
            }
        }

        private string _LoactionCode;
        public string LoactionCode
        {
            get { return _LoactionCode; }
            set
            {
                _LoactionCode = value;
                OnPropertyChanged("LoactionCode");
            }
        }

        private Visibility _isFirstVisbility;
        public Visibility isFirstVisbility
        {
            get { return _isFirstVisbility; }
            set
            {
                _isFirstVisbility = value;
                OnPropertyChanged("isFirstVisbility");
            }
        }

        private string _BinScanBarcodeMsg;
        public string BinScanBarcodeMsg
        {
            get { return _BinScanBarcodeMsg; }
            set
            {
                _BinScanBarcodeMsg = value;
                OnPropertyChanged("BinScanBarcodeMsg");
            }
        }

        private string _OUTWeigingBinBarcode;
        public string OUTWeigingBinBarcode
        {
            get { return _OUTWeigingBinBarcode; }
            set
            {
                _OUTWeigingBinBarcode = value;
                OnPropertyChanged("OUTWeigingBinBarcode");
            }
        }

        private Visibility _isSecondVisbility;
        public Visibility isSecondVisbility
        {
            get { return _isSecondVisbility; }
            set
            {
                _isSecondVisbility = value;
                OnPropertyChanged("isSecondVisbility");
            }
        }

        private string _BinIBCInfo;
        public string BinIBCInfo
        {
            get { return _BinIBCInfo; }
            set
            {
                _BinIBCInfo = value;
                OnPropertyChanged("BinIBCInfo");
            }
        }

        private string _txtTotalWpopOUTBIN;
        public string txtTotalWpopOUTBIN
        {
            get { return _txtTotalWpopOUTBIN; }
            set
            {
                _txtTotalWpopOUTBIN = value;
                OnPropertyChanged("txtTotalWpopOUTBIN");
            }
        }

        private Visibility _isThirdVisibility;
        public Visibility isThirdVisibility
        {
            get { return _isThirdVisibility; }
            set
            {
                _isThirdVisibility = value;
                OnPropertyChanged("isThirdVisibility");
            }
        }

        private Visibility _isFourthVisibility;
        public Visibility isFourthVisibility
        {
            get { return _isFourthVisibility; }
            set
            {
                _isFourthVisibility = value;
                OnPropertyChanged("isFourthVisibility");
            }
        }

        private string _PalletBinBarcode;
        public string PalletBinBarcode
        {
            get { return _PalletBinBarcode; }
            set
            {
                _PalletBinBarcode = value;
                OnPropertyChanged("PalletBinBarcode");
            }
        }

        private bool _isEOutWeighBtn;
        public bool isEOutWeighBtn
        {
            get { return _isEOutWeighBtn; }
            set
            {
                _isEOutWeighBtn = value;
                OnPropertyChanged("isEOutWeighBtn");
            }
        }

        private string _OutWegingMsg;
        public string OutWegingMsg
        {
            get { return _OutWegingMsg; }
            set
            {
                _OutWegingMsg = value;
                OnPropertyChanged("OutWegingMsg");
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

        private ObservableCollection<StorageInputData> _StorageInputData;
        public ObservableCollection<StorageInputData> StorageInputData
        {
            get { return _StorageInputData; }
            set
            {
                _StorageInputData = value;
                OnPropertyChanged("StorageInputData");
            }
        }

        private StorageInputData _SelectedItem;
        public StorageInputData SelectedItem
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

        private ObservableCollection<StorageOutData> _StorageOutData;
        public ObservableCollection<StorageOutData> StorageOutData
        {
            get { return _StorageOutData; }
            set
            {
                _StorageOutData = value;
                OnPropertyChanged("StorageOutData");
            }
        }

        private ObservableCollection<StorageOutData> _StorageOutData2;
        public ObservableCollection<StorageOutData> StorageOutData2
        {
            get { return _StorageOutData2; }
            set
            {
                _StorageOutData2 = value;
                OnPropertyChanged("StorageOutData2");
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

        private StorageOutData _SelectedData;
        public StorageOutData SelectedData
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

        private BR_BRS_SEL_ProductionOrderDetail_ORDER _BR_BRS_SEL_ProductionOrderDetail_ORDER;
        public BR_BRS_SEL_ProductionOrderDetail_ORDER BR_BRS_SEL_ProductionOrderDetail_ORDER
        {
            get { return _BR_BRS_SEL_ProductionOrderDetail_ORDER; }
            set
            {
                _BR_BRS_SEL_ProductionOrderDetail_ORDER = value;
                OnPropertyChanged("BR_BRS_SEL_ProductionOrderDetail_ORDER");
            }
        }

        private BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA _CboProcess;
        public BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA CboProcess
        {
            get { return _CboProcess; }
            set
            {
                _CboProcess = value;
                OnPropertyChanged("CboProcess");
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

        private BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo;
        public BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo
        {
            get { return _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo; }
            set
            {
                _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = value;
                OnPropertyChanged("BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo");
            }
        }

        private BR_PHR_SEL_System_PC_IP_ID _BR_PHR_SEL_System_PC_IP_ID;
        public BR_PHR_SEL_System_PC_IP_ID BR_PHR_SEL_System_PC_IP_ID
        {
            get { return _BR_PHR_SEL_System_PC_IP_ID; }
            set
            {
                _BR_PHR_SEL_System_PC_IP_ID = value;
                OnPropertyChanged("BR_PHR_SEL_System_PC_IP_ID");
            }
        }

        private BR_PHR_SEL_ProductionOrderOutput_Define _BR_PHR_SEL_ProductionOrderOutput_Define;
        public BR_PHR_SEL_ProductionOrderOutput_Define BR_PHR_SEL_ProductionOrderOutput_Define
        {
            get { return _BR_PHR_SEL_ProductionOrderOutput_Define; }
            set
            {
                _BR_PHR_SEL_ProductionOrderOutput_Define = value;
                OnPropertyChanged("BR_PHR_SEL_ProductionOrderOutput_Define");
            }
        }

        private BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA _CboOutput;
        public BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA CboOutput
        {
            get { return _CboOutput; }
            set
            {
                _CboOutput = value;
                OnPropertyChanged("CboOutput");
            }
        }

        private BR_BRS_REG_VESSEL_WEIGHT _BR_BRS_REG_VESSEL_WEIGHT;
        public BR_BRS_REG_VESSEL_WEIGHT BR_BRS_REG_VESSEL_WEIGHT
        {
            get { return _BR_BRS_REG_VESSEL_WEIGHT; }
            set
            {
                _BR_BRS_REG_VESSEL_WEIGHT = value;
                OnPropertyChanged("BR_BRS_REG_VESSEL_WEIGHT");
            }
        }

        private BR_PHR_SEL_Equipment_GetLocation_CBO _BR_PHR_SEL_Equipment_GetLocation_CBO;
        public BR_PHR_SEL_Equipment_GetLocation_CBO BR_PHR_SEL_Equipment_GetLocation_CBO
        {
            get { return _BR_PHR_SEL_Equipment_GetLocation_CBO; }
            set
            {
                _BR_PHR_SEL_Equipment_GetLocation_CBO = value;
                OnPropertyChanged("BR_PHR_SEL_Equipment_GetLocation_CBO");
            }
        }

        private BR_BRS_GET_VESSEL_INFO_DETAIL _BR_BRS_GET_VESSEL_INFO_DETAIL;
        public BR_BRS_GET_VESSEL_INFO_DETAIL BR_BRS_GET_VESSEL_INFO_DETAIL
        {
            get { return _BR_BRS_GET_VESSEL_INFO_DETAIL; }
            set
            {
                _BR_BRS_GET_VESSEL_INFO_DETAIL = value;
                OnPropertyChanged("BR_BRS_GET_VESSEL_INFO_DETAIL");
            }
        }

        private BR_PHR_UPD_MaterialSubLot_CheckWeight _BR_PHR_UPD_MaterialSubLot_CheckWeight;
        public BR_PHR_UPD_MaterialSubLot_CheckWeight BR_PHR_UPD_MaterialSubLot_CheckWeight
        {
            get { return _BR_PHR_UPD_MaterialSubLot_CheckWeight; }
            set
            {
                _BR_PHR_UPD_MaterialSubLot_CheckWeight = value;
                OnPropertyChanged("BR_PHR_UPD_MaterialSubLot_CheckWeight");
            }
        }

        #region[OnExecuteCompleted]

        void _BR_BRS_SEL_BINInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_BINInfo.OUTDATAs.Count > 0)
            {
                if (_StorageInputData.Count > 0)
                {
                    C1.Silverlight.C1MessageBox.Show(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "SFU1204"), "Warring", C1.Silverlight.C1MessageBoxButton.OKCancel
                        , C1.Silverlight.C1MessageBoxIcon.Warning, (MessageBoxResult) =>
                        {
                            if (MessageBoxResult == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                            else if (MessageBoxResult == MessageBoxResult.OK)
                            {
                                _StorageInputData.Clear();

                                foreach (var item in _BR_BRS_SEL_BINInfo.OUTDATAs)
                                {
                                    _StorageInputData.Add(new StorageInputData
                                    {
                                        RowIndex = (_StorageInputData.Count + 1).ToString(),
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
                                        PRODDTTM = item.PRODDTTM,
                                        WSDTTM = item.WSDTTM,
                                        TAREWEIGHT = item.TAREWEIGHT,
                                    });
                                    txtVesselWt = item.TAREWEIGHT.ToString();
                                }

                                //var popup = new StorageInWeighing();

                                //popup.DataContext = this;
                                //txtTotalWpop = "";

                                //popup.OKButton.Click += (s, e) =>
                                //{
                                //    if (txtTotalWpop != null && txtTotalWpop.Length > 0)
                                //    {
                                //        txtTotalWts = txtTotalWpop;

                                //        if (txtVesselWt.IndexOf(_SUnit) < 0 && txtVesselWt.Length > 0)
                                //            txtVesselWt = txtVesselWt + " " + _SUnit;

                                //        if (txtTotalWts.Replace(_SUnit, "").Length > 0 && txtVesselWt.Replace(_SUnit, "").Length > 0)
                                //            txtConWt = (decimal.Parse(txtTotalWts.Replace(_SUnit, "").Replace(" ", "")) - decimal.Parse(txtVesselWt.Replace(_SUnit, "").Replace(" ", ""))).ToString() + " " + _SUnit;
                                //    }

                                //    if (_repeater != null)
                                //    {
                                //        _repeater.Stop();
                                //        _repeater = null;
                                //    }
                                //};
                                //popup.CancelButton.Click += (s, e) =>
                                //{
                                //    if (_repeater != null)
                                //    {
                                //        _repeater.Stop();
                                //        _repeater = null;
                                //    }
                                //};

                                //popup.Show();
                            }
                        });
                }
                else
                {
                    _StorageInputData.Clear();

                    foreach (var item in _BR_BRS_SEL_BINInfo.OUTDATAs)
                    {
                        _StorageInputData.Add(new StorageInputData
                        {
                            RowIndex = (_StorageInputData.Count + 1).ToString(),
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
                            PRODDTTM = item.PRODDTTM,
                            WSDTTM = item.WSDTTM,
                            TAREWEIGHT = item.TAREWEIGHT,
                        });
                        txtVesselWt = item.TAREWEIGHT.ToString();
                    }

                    //var popup = new StorageInWeighing();

                    //popup.DataContext = this;
                    //txtTotalWpop = "";

                    //popup.OKButton.Click += (s, e) =>
                    //{
                    //    if (txtTotalWpop != null && txtTotalWpop.Length > 0)
                    //    {
                    //        txtTotalWts = txtTotalWpop;

                    //        if (txtVesselWt.IndexOf(_SUnit) < 0 && txtVesselWt.Length > 0)
                    //            txtVesselWt = txtVesselWt + " " + _SUnit;

                    //        if (txtTotalWts.Replace(_SUnit, "").Length > 0 && txtVesselWt.Replace(_SUnit, "").Length > 0)
                    //            txtConWt = (decimal.Parse(txtTotalWts.Replace(_SUnit, "").Replace(" ", "")) - decimal.Parse(txtVesselWt.Replace(_SUnit, "").Replace(" ", ""))).ToString() + " " + _SUnit;
                    //    }

                    //    if (_repeater != null)
                    //    {
                    //        _repeater.Stop();
                    //        _repeater = null;
                    //    }
                    //};
                    //popup.CancelButton.Click += (s, e) =>
                    //{
                    //    if (_repeater != null)
                    //    {
                    //        _repeater.Stop();
                    //        _repeater = null;
                    //    }
                    //};

                    //popup.Show();
                }

                if (StorageInputData.Count > 0)
                {
                    _mainWnd.dgList.SelectedIndex = 0;
                    tbIncount = StorageInputData.Count.ToString() + " 건";
                }
            }

        }

        void _BR_BRS_SEL_YieldInfo_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_SEL_YieldInfo.OUTDATAs.Count > 0)
            {
                foreach (var item in _BR_BRS_SEL_YieldInfo.OUTDATAs)
                {
                    txtYelid = item.YIELD + "%";
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
                        PRODDTTM = item.PRODDTTM != null ? item.PRODDTTM : "",
                        WASHINGDTTM = item.WASHINGDTTM != null ? item.WASHINGDTTM : "",
                        MTRLID = item.MTRLID,
                        MTRLNAME = item.MTRLNAME,
                        OUTTYPE = _OutType == "FILLED" ? "반제품" : "빈용기",
                        IBC = item.EQPTID,
                        CHK = false
                    });
                }
            }
        }

        void _BR_PHR_SEL_System_PC_IP_ID_OnExecuteCompleted(string ruleName)
        {
            if (_BR_PHR_SEL_System_PC_IP_ID.OUTDATAs.Count > 0)
            {
                txtComment = _BR_PHR_SEL_System_PC_IP_ID.OUTDATAs[0].ROOMNAME;
            }
        }

        void _BR_PHR_SEL_Equipment_GetLocation_CBO_OnExecuteCompleted(string ruleName)
        {
            if (_BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Count > 0)
            {
                _Orderbatch.CboLocation.SelectedIndex = 0;
                LoactionCode = _BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Select(o => o.EQPTID).FirstOrDefault();
            }
            else
            {
                _Orderbatch.CboLocation.SelectedIndex = -1;
            }
        }

        void _BR_BRS_GET_VESSEL_INFO_DETAIL_OnExecuteCompleted(string ruleName)
        {
            if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs.Count > 0)
            {
                OutWegingMsg = "";
                foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs)
                {
                    if (item.CHECKTYPE == "1")
                    {
                        isFirstVisbility = Visibility.Collapsed;
                        isSecondVisbility = Visibility.Visible;
                        isThirdVisibility = Visibility.Collapsed;
                        isFourthVisibility = Visibility.Collapsed;

                        BinIBCInfo = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].VESSELID;
                        _OutWeghing.Width = 320;
                        _OutWeghing.Height = 300;
                    }
                    else if (item.CHECKTYPE == "2")
                    {
                        isFirstVisbility = Visibility.Collapsed;
                        isSecondVisbility = Visibility.Collapsed;
                        isThirdVisibility = Visibility.Visible;
                        isFourthVisibility = Visibility.Collapsed;

                        _OutWeghing.Width = 800;
                        _OutWeghing.Height = 180;
                    }
                    else if (item.CHECKTYPE == "3")
                    {
                        isFirstVisbility = Visibility.Collapsed;
                        isSecondVisbility = Visibility.Collapsed;
                        isThirdVisibility = Visibility.Collapsed;
                        isFourthVisibility = Visibility.Visible;

                        _OutWeghing.dgDetailList.Height = 250;

                        double postiononimgx = (Application.Current.Host.Content.ActualHeight / 2) - 600;

                        _OutWeghing.Width = 1200;
                        _OutWeghing.Height = 400;

                        _OutWeghing.VerticalAlignment = VerticalAlignment.Center;
                        _OutWeghing.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                }
            }
        }

        void _BR_BRS_REG_WMS_Request_OUT_OnExecuteCompleted(string ruleName)
        {
        }
        #endregion

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
                            if (arg != null) _mainWnd = arg as StorageInOutWms;

                            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATAs.Add(new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.INDATA
                            {
                                EQPTID = AuthRepositoryViewModel.Instance.RoomID,
                                LANGID = AuthRepositoryViewModel.Instance.LoginedUserID
                            }
                            );

                            if (await _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.Execute())
                            {
                                if (_BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs.Count > 0)
                                {
                                    _SelectedScale = _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo.OUTDATAs[0].EQPTID;
                                }
                            }


                            _BR_PHR_SEL_System_PC_IP_ID.INDATAs.Clear();
                            _BR_PHR_SEL_System_PC_IP_ID.OUTDATAs.Clear();
                            _BR_PHR_SEL_System_PC_IP_ID.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_System_PC_IP_ID.INDATA
                            {
                                IPADRESS = LGCNS.iPharmMES.Common.Common.ClientIP,
                                ACCESSID = string.IsNullOrWhiteSpace(LGCNS.iPharmMES.Common.Common.UserAccount) ? null : LGCNS.iPharmMES.Common.Common.UserAccount,
                                ISUSE = "Y"
                            });

                            await _BR_PHR_SEL_System_PC_IP_ID.Execute();

                            txtBarcode = "";
                            _StorageInputData.Clear();
                            txtVesselWt = "";
                            txtConWt = "";
                            txtTotalWts = "";
                            txtYelid = "";
                            _mainWnd.txtBarcodes.Focus();
                            isInEble = false;
                            isOutEble = true;
                            tbIncount = "0 건";
                            tbOutcount = "0 건";
                            tbOutcount2 = "0 건";
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

        public ICommand WeightInputCommandAync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["WeightInputCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["WeightInputCommand"] = false;
                            CommandCanExecutes["WeightInputCommand"] = false;

                            ///
                            var popup = new StorageInWeighing();

                            popup.DataContext = this;
                            txtTotalWpop = "";

                            popup.OKButton.Click += (s, e) =>
                            {
                                if (txtTotalWpop != null && txtTotalWpop.Length > 0)
                                {
                                    txtTotalWts = txtTotalWpop;

                                    if (txtVesselWt.IndexOf(_SUnit) < 0 && txtVesselWt.Length > 0)
                                        txtVesselWt = txtVesselWt + " " + _SUnit;

                                    if (txtTotalWts.Replace(_SUnit, "").Length > 0 && txtVesselWt.Replace(_SUnit, "").Length > 0)
                                        txtConWt = (decimal.Parse(txtTotalWts.Replace(_SUnit, "").Trim()) - decimal.Parse(txtVesselWt.Replace(_SUnit, "").Trim())).ToString() + " " + _SUnit;
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
                            ///

                            CommandResults["WeightInputCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["WeightInputCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["WeightInputCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("WeightInputCommand") ?
                        CommandCanExecutes["WeightInputCommand"] : (CommandCanExecutes["WeightInputCommand"] = true);
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
                            _StorageInputData.Clear();
                            txtVesselWt = "";
                            txtConWt = "";
                            txtTotalWts = "";
                            txtYelid = "";
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

        public ICommand initialOutputCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["initialOutputCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["initialOutputCommand"] = false;
                            CommandCanExecutes["initialOutputCommand"] = false;

                            ///
                            _StorageOutData.Clear();
                            _StorageOutData2.Clear();
                            txtProdInfoOut = "";
                            txtOrderNoOut = "";
                            txtBatchNoOut = "";
                            txtProcessOut = "";
                            txtIBCIDOutput = "";
                            txtBinWtOut = "";
                            txtGoodWtout = "";
                            tbOutcount = "0 건";
                            ///

                            CommandResults["initialOutputCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["initialOutputCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["initialOutputCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("initialOutputCommand") ?
                        CommandCanExecutes["initialOutputCommand"] : (CommandCanExecutes["initialOutputCommand"] = true);
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

                                _SelectedItem = tmparam.SelectedItem as StorageInputData;

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
                                , "WMS 입고"
                                , "WMS 입고"
                                , false
                                , "EM_Equipment_InOut"
                                , ""
                                , null
                                , null) == false)
                            {
                                throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                            }

                            //if (txtTotalWts.Length == 0 || txtTotalWts == null || txtYelid.Length == 0 || txtYelid == null)
                            //{
                            //    C1.Silverlight.C1MessageBox.Show(LGCNS.iPharmMES.Common.BizMessage.GetMessageByLang(LGCNS.EZMES.Common.LogInInfo.LangID, "SFU1205"), "Warring"
                            //        , C1.Silverlight.C1MessageBoxButton.OK
                            //        , C1.Silverlight.C1MessageBoxIcon.Error);
                            //    return;
                            //}

                            foreach (var item in _StorageInputData)
                            {
                                _BR_BRS_REG_WMS_Request_IN.INDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_IN.OUTDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_IN.INDATAs.Add(new BR_BRS_REG_WMS_Request_IN.INDATA
                                {
                                    VESSELID = item.VESSELID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                    USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut")
                                });
                            }
                            if (await _BR_BRS_REG_WMS_Request_IN.Execute())
                            {
                                OnMessage("정상적으로 처리되었습니다.");
                            }


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

        public ICommand SIWLoadCommandAsync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SIWLoadCommandAsync"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["SIWLoadCommandAsync"] = false;
                            CommandCanExecutes["SIWLoadCommandAsync"] = false;

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

                                            if (await _BR_PHR_SEL_CurrentWeight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) && _BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                            {
                                                string curWeight = string.Format("{0:F3}", _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight);
                                                this._SUnit = _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                                if (_repeater != null && _repeater.IsEnabled)
                                                {
                                                    _ScaleOnOff = true;
                                                    txtTotalWpop = curWeight + " " + _SUnit;
                                                }
                                            }
                                            else
                                            {
                                                txtTotalWpop = "연결실패";
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

                            CommandResults["SIWLoadCommandAsync"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SIWLoadCommandAsync"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SIWLoadCommandAsync"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SIWLoadCommandAsync") ?
                        CommandCanExecutes["SIWLoadCommandAsync"] : (CommandCanExecutes["SIWLoadCommandAsync"] = true);
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


                            var matchedCode = AuthRepositoryViewModel.Instance.UserFunctionAuthorityList.OUTDATAs.Where(o =>
                                o.FNCTNCODE == "OM_BRS_WMS_INOUT_TEST" && o.ISCREATE == "Y"
                                ).FirstOrDefault();

                            if (matchedCode != null)
                            {
                                IsVsbTest = Visibility.Visible;
                            }
                            else
                            {
                                IsVsbTest = Visibility.Collapsed;
                            }

                            _OutData2.Clear();

                            _BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATAs.Clear();
                            _BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATAs.Add(new BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATA
                            {
                                POID = txtOrderNoOut
                            });
                            await _BR_BRS_SEL_ProductionOrderDetail_ORDER.Execute();

                            _BR_PHR_SEL_EquipmentGroup.INDATAs.Clear();
                            _BR_PHR_SEL_EquipmentGroup.OUTDATAs.Clear();
                            _BR_PHR_SEL_EquipmentGroup.INDATAs.Add(new BR_PHR_SEL_EquipmentGroup.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                EQPTTYPE = "EY001",
                                EQPTGRPIUSE = "Y"

                            });
                            _BR_PHR_SEL_EquipmentGroup.INDATAs.Add(new BR_PHR_SEL_EquipmentGroup.INDATA
                            {
                                LANGID = AuthRepositoryViewModel.Instance.LangID,
                                EQPTTYPE = "EY010",
                                EQPTGRPIUSE = "Y"

                            });

                            await _BR_PHR_SEL_EquipmentGroup.Execute();

                            _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Clear();
                            _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Clear();
                            _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA
                            {
                                POID = txtOrderNoOut,
                                OPSGGUID = _OPSGUID != null ? new Guid?(new Guid(_OPSGUID)) : null
                            });

                            await _BR_PHR_SEL_ProductionOrderOutput_Define.Execute();

                            if (_BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATAs.Count > 0)
                            {
                                pop.cboProcess.SelectedIndex = _BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATAs.Where(o => o.OPSGNAME == txtProcessOut).Select(o => Convert.ToInt32(o.RowIndex)).FirstOrDefault();
                            }

                            _OutType = "FILLED";

                            //if (_OutType == "FILLED")
                            //{

                            //}
                            //else
                            //{

                            //}
                            pop.OKButton.Click += (s, e) =>
                                {
                                    StorageOutData.Clear();
                                    foreach (var item in OutData2)
                                    {
                                        if (item.CHK)
                                        {
                                            _StorageOutData.Add(new StorageOutData
                                            {
                                                RowEditSec = item.RowEditSec,
                                                RowLoadedFlag = item.RowLoadedFlag,
                                                RowIndex = (StorageOutData.Count + 1).ToString(),
                                                EQPTID = item.EQPTID,
                                                EQPTNAME = item.EQPTNAME,
                                                PRODDTTM = item.PRODDTTM,
                                                WASHINGDTTM = item.WASHINGDTTM,
                                                MTRLID = item.MTRLID,
                                                MTRLNAME = item.MTRLNAME,
                                                OUTTYPE = item.OUTTYPE,
                                                IBC = item.IBC,
                                                Status = "Stand By",
                                            });

                                            txtProdInfoOut = item.MTRLNAME;
                                        }

                                    }
                                    if (pop.cboProcess.SelectedValue != null)
                                    {
                                        txtProcessOut = (pop.cboProcess.SelectedItem as BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA).OPSGNAME;
                                        _OPSGUID = (pop.cboProcess.SelectedItem as BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA).OPSGGUID;
                                    }
                                    if (StorageOutData.Count > 0)
                                    {
                                        _mainWnd.dgOutputList.SelectedIndex = 0;
                                        tbOutcount = StorageOutData.Count.ToString() + " 건";
                                    }

                                    _mainWnd.TabOutput.Focus();
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
                            if (_OutType.Equals("CLEAN") && CboTypeitem.EQPTGRPID != null)
                            {
                                _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Clear();
                                _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs.Clear();

                                _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_IBCList.INDATA
                                {
                                    TYPE = _OutType,
                                    EQPTGRPID = CboTypeitem.EQPTGRPID,
                                    POID = "",
                                    OPSGGUID = "",
                                    OUTPUTGUID = ""
                                });
                                await _BR_BRS_SEL_ProductionOrder_IBCList.Execute();
                            }
                            else
                            {

                                if (txtBatchNoOut != null && txtBatchNoOut.Length > 0 && txtOrderNoOut != null && txtOrderNoOut.Length > 0 &&
                                    CboProcess != null && CboProcess.OPSGGUID != null && CboOutput != null && CboOutput.OUTPUTGUID != null)
                                {
                                    _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Clear();
                                    _BR_BRS_SEL_ProductionOrder_IBCList.OUTDATAs.Clear();

                                    _BR_BRS_SEL_ProductionOrder_IBCList.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_IBCList.INDATA
                                    {
                                        TYPE = _OutType,
                                        EQPTGRPID = CboTypeitem.EQPTGRPID,
                                        POID = txtOrderNoOut,
                                        OPSGGUID = CboProcess.OPSGGUID,
                                        OUTPUTGUID = CboOutput.OUTPUTGUID.ToString()
                                    });
                                    await _BR_BRS_SEL_ProductionOrder_IBCList.Execute();
                                }
                            }
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
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.INDATAs.Clear();
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Clear();
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_Equipment_GetLocation_CBO.INDATA
                            {
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            });

                            await _BR_PHR_SEL_Equipment_GetLocation_CBO.Execute();

                            _BR_BRS_SEL_ProductionOrder_OrderList.OUTDATAs.Clear();

                            if (_BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Count > 0)
                            {
                                _Orderbatch.CboLocation.SelectedIndex = 0;
                                LoactionCode = _BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Select(o => o.EQPTID).FirstOrDefault();
                            }
                            else
                            {
                                _Orderbatch.CboLocation.SelectedIndex = -1;
                            }

                            IsIncludeUnusedData = true;

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
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.INDATAs.Clear();
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Clear();
                            _BR_PHR_SEL_Equipment_GetLocation_CBO.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_PHR_SEL_Equipment_GetLocation_CBO.INDATA
                            {
                                USERID = AuthRepositoryViewModel.Instance.LoginedUserID
                            });

                            await _BR_PHR_SEL_Equipment_GetLocation_CBO.Execute();

                            _BR_BRS_SEL_ProductionOrder_OrderList.OUTDATAs.Clear();

                            if (_BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Count > 0)
                            {
                                _Orderbatch.CboLocation.SelectedIndex = 0;
                                LoactionCode = _BR_PHR_SEL_Equipment_GetLocation_CBO.OUTDATAs.Select(o => o.EQPTID).FirstOrDefault();
                            }
                            else
                            {
                                _Orderbatch.CboLocation.SelectedIndex = -1;
                            }

                            IsIncludeUnusedData = true;

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
                                    BATCHNO = tbOrderBatch == "배치번호" ? txtOrderBatch : null,
                                    ISINCLUDECOMP = IsIncludeUnusedData ? "Y" : "N",
                                    PRODTEAMID = LoactionCode
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

                            foreach (var item in StorageOutData)
                            {
                                _BR_BRS_REG_WMS_Request_OUT.INDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_OUT.OUTDATAs.Clear();
                                _BR_BRS_REG_WMS_Request_OUT.INDATAs.Add(new LGCNS.iPharmMES.Common.BR_BRS_REG_WMS_Request_OUT.INDATA
                                {
                                    VESSELID = item.EQPTID,
                                    ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                    USERID = string.IsNullOrWhiteSpace(AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut")) ? AuthRepositoryViewModel.Instance.LoginedUserID : AuthRepositoryViewModel.GetUserIDByFunctionCode("EM_Equipment_InOut"),
                                    POID = txtOrderNoOut,
                                    OPSGGUID = CboProcess.OPSGGUID != null ? CboProcess.OPSGGUID.ToString() : "",
                                    OUTPUTGUID = CboOutput.OUTPUTGUID != null ? CboOutput.OUTPUTGUID.ToString() : "",
                                    OUTGUBUN = _OutType,
                                    GROSSWEIGHT = null,
                                    UOMID = null
                                });
                                if (await _BR_BRS_REG_WMS_Request_OUT.Execute() == true)
                                {
                                    item.isSaved = true;
                                    item.Status = "Success";
                                }
                                else
                                {
                                    item.isSaved = false;
                                    item.Status = "Error";
                                }
                            }

                            if (StorageOutData.Count > 0)
                            {
                                foreach (var item in StorageOutData)
                                {
                                    if (item.isSaved != false && item.Status == "Success")
                                    {
                                        StorageOutData2.Add(item);
                                    }
                                }

                                foreach (var item in StorageOutData2)
                                {
                                    StorageOutData.Remove(item);
                                }

                                tbOutcount = StorageOutData.Count.ToString() + " 건";
                                tbOutcount2 = StorageOutData2.Count.ToString() + " 건";
                            }
                            if (_BR_BRS_REG_WMS_Request_OUT.OUTDATAs.Count > 0)
                            {
                                OnMessage(_BR_BRS_REG_WMS_Request_OUT.OUTDATAs[0].RESULT_MSG);
                            }
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
                                var temp = arg as StorageOutData;
                                var matched = _StorageOutData.Where(o => o.EQPTID == temp.EQPTID && o.RowIndex == temp.RowIndex).FirstOrDefault();
                                if (matched != null) _StorageOutData.Remove(matched);

                                _tbOutcount = _StorageOutData.Count.ToString() + " 건";
                                OnPropertyChanged("tbOutcount");

                                if (_StorageOutData.Count == 0)
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

                                _SelectedData = tmparam.SelectedItem as StorageOutData;

                                if (_SelectedData != null)
                                {
                                    txtIBCIDOutput = _SelectedData.IBC;
                                    txtGoodWtout = SelectedData.WeightOut;
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

        public ICommand WeightOutCommandAync
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["WeightOutCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusy = true;

                            CommandResults["WeightOutCommand"] = false;
                            CommandCanExecutes["WeightOutCommand"] = false;

                            ///
                            var popup = new StorageOutWheinging();

                            isFirstVisbility = Visibility.Visible;
                            isSecondVisbility = Visibility.Collapsed;
                            isThirdVisibility = Visibility.Collapsed;
                            isFourthVisibility = Visibility.Collapsed;

                            popup.Width = 1200;
                            popup.Height = 540;

                            isEOutWeighBtn = false;

                            popup.DataContext = this;
                            BinScanBarcodeMsg = "용기 바코드를 스캔하세요";

                            popup.OKButton.Click += async (s, e) =>
                            {
                                if (isFirstVisbility == Visibility.Visible)
                                {
                                    if (CurrentWeghing != "" && CurrentWeghing.Length > 0)
                                    {
                                        _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Clear();
                                        _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs.Clear();
                                        _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Clear();
                                        _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Clear();

                                        _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Add(new BR_BRS_GET_VESSEL_INFO_DETAIL.INDATA
                                        {
                                            VESSELID = OUTWeigingBinBarcode
                                        });

                                        await _BR_BRS_GET_VESSEL_INFO_DETAIL.Execute();
                                    }
                                }
                                else if (isSecondVisbility == Visibility.Visible)
                                {

                                    var authelper = new iPharmAuthCommandHelper();

                                    authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_BRS_CheckWeight_STORGAEOUT");

                                    if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                        , Common.enumAccessType.Create
                                        , "WMS 출고 무게 측정"
                                        , "WMS 출고 무게 측정"
                                        , false
                                        , "OM_BRS_CheckWeight_STORGAEOUT"
                                        , ""
                                        , null
                                        , null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }

                                    _BR_BRS_REG_VESSEL_WEIGHT.INDATAs.Clear();
                                    _BR_BRS_REG_VESSEL_WEIGHT.OUTDATAs.Clear();

                                    _BR_BRS_REG_VESSEL_WEIGHT.INDATAs.Add(new BR_BRS_REG_VESSEL_WEIGHT.INDATA
                                    {
                                        ROOMNO = AuthRepositoryViewModel.Instance.RoomID,
                                        USERID = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_BRS_CheckWeight_STORGAEOUT"),
                                        POID = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].POID != null ? _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].POID : "",
                                        OPSGGUID = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].OPSGGUID != null ? _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].OPSGGUID : "",
                                        OUTPUTGUID = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].OUTPUTGUID != null ? _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs[0].OUTPUTGUID : "",
                                        OUTGUBUN = _OutType,
                                        GROSSWEIGHT = Convert.ToDecimal(txtTotalWpopOUTBIN.Replace(_SUnit, "")),
                                        UOMID = _SUnit,
                                        VESSELID = OUTWeigingBinBarcode,
                                        SCALEID = _SelectedScale
                                    });

                                    await _BR_BRS_REG_VESSEL_WEIGHT.Execute();

                                    popup.DialogResult = true;
                                }
                                else if (isThirdVisibility == Visibility.Visible)
                                {

                                    var authelper = new iPharmAuthCommandHelper();

                                    authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_BRS_CheckWeight_STORGAEOUT");

                                    if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                        , Common.enumAccessType.Create
                                        , "WMS 출고 무게 측정"
                                        , "WMS 출고 무게 측정"
                                        , false
                                        , "OM_BRS_CheckWeight_STORGAEOUT"
                                        , ""
                                        , null
                                        , null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }

                                    if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count > 0)
                                    {
                                        foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs)
                                        {
                                            if (item.REALQTY > 0)
                                            {
                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Clear();
                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Clear();

                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                                {
                                                    MLOTID = item.MLOTID,
                                                    MLOTVER = item.MLOTVER,
                                                    INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_BRS_CheckWeight_STORGAEOUT"),
                                                });

                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                                {
                                                    MSUBLOTID = item.MSUBLOTID,
                                                    MSUBLOTVER = item.MSUBLOTVER,
                                                    MSUBLOTSEQ = item.MSUBLOTSEQ,
                                                    MSUBLOTSTAT = item.MSUBLOTSTAT
                                                });
                                            }

                                            if (_BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Count > 0 || _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Count > 0) await _BR_PHR_UPD_MaterialSubLot_CheckWeight.Execute();
                                        }
                                    }
                                    popup.DialogResult = true;
                                }
                                else if (isFourthVisibility == Visibility.Visible)
                                {

                                    var authelper = new iPharmAuthCommandHelper();

                                    authelper.InitializeAsync(Common.enumCertificationType.Function, Common.enumAccessType.Create, "OM_BRS_CheckWeight_STORGAEOUT");

                                    if (await authelper.ClickAsync(Common.enumCertificationType.Function
                                        , Common.enumAccessType.Create
                                        , "WMS 출고 무게 측정"
                                        , "WMS 출고 무게 측정"
                                        , false
                                        , "OM_BRS_CheckWeight_STORGAEOUT"
                                        , ""
                                        , null
                                        , null) == false)
                                    {
                                        throw new Exception(string.Format("서명이 완료되지 않았습니다."));
                                    }

                                    if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count > 0)
                                    {
                                        _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Clear();
                                        _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Clear();
                                        foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs)
                                        {
                                            if (item.CHK_FLAG == "Y")
                                            {
                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOT
                                                {
                                                    MLOTID = item.MLOTID,
                                                    MLOTVER = item.MLOTVER,
                                                    INDUSER = AuthRepositoryViewModel.GetUserIDByFunctionCode("OM_BRS_CheckWeight_STORGAEOUT"),
                                                });

                                                _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Add(new BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOT
                                                {
                                                    MSUBLOTID = item.MSUBLOTID,
                                                    MSUBLOTVER = item.MSUBLOTVER,
                                                    MSUBLOTSEQ = item.MSUBLOTSEQ,
                                                    MSUBLOTSTAT = item.MSUBLOTSTAT
                                                });
                                            }
                                        }
                                        if (_BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MLOTs.Count > 0 || _BR_PHR_UPD_MaterialSubLot_CheckWeight.INDATA_MSUBLOTs.Count > 0) await _BR_PHR_UPD_MaterialSubLot_CheckWeight.Execute();
                                    }

                                    popup.DialogResult = true;
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
                            ///

                            CommandResults["WeightOutCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["WeightOutCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["WeightOutCommand"] = true;

                            IsBusy = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("WeightOutCommand") ?
                        CommandCanExecutes["WeightOutCommand"] : (CommandCanExecutes["WeightOutCommand"] = true);
                });
            }
        }

        public ICommand SectionChangedCmbCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["SectionChangedCmbCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["SectionChangedCmbCommand"] = false;
                            CommandCanExecutes["SectionChangedCmbCommand"] = false;

                            ///
                            if (arg != null && arg is BR_PHR_SEL_EquipmentGroup.OUTDATA)
                            {
                                CboTypeitem = arg as BR_PHR_SEL_EquipmentGroup.OUTDATA;
                            }
                            else if (arg != null && arg is BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA)
                            {
                                CboProcess = arg as BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA;


                                _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Clear();
                                _BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Clear();
                                _BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA
                                {
                                    POID = txtOrderNoOut,
                                    OPSGGUID = new Guid(CboProcess.OPSGGUID)
                                });

                                await _BR_PHR_SEL_ProductionOrderOutput_Define.Execute();
                            }
                            else if (arg != null && arg is BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA)
                            {
                                CboOutput = arg as BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA;
                            }
                            ///

                            CommandResults["SectionChangedCmbCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["SectionChangedCmbCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["SectionChangedCmbCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("SectionChangedCmbCommand") ?
                        CommandCanExecutes["SectionChangedCmbCommand"] : (CommandCanExecutes["SectionChangedCmbCommand"] = true);
                });
            }
        }

        public ICommand OUTWeigingLoadedCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["OUTWeigingLoadedCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["OUTWeigingLoadedCommand"] = false;
                            CommandCanExecutes["OUTWeigingLoadedCommand"] = false;

                            ///
                            if (arg != null && arg is StorageOutWheinging)
                            {
                                _OutWeghing = arg as StorageOutWheinging;
                                _OutWeghing.Width = 600;
                                _OutWeghing.Height = 225;
                                _OutWeghing.txtBinBarcode.Text = "";
                                _OutWeghing.txtBinBarcode.Text = "";
                                OutWegingMsg = "용기번호를 스캔하세요";

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

                                                if (await _BR_PHR_SEL_CurrentWeight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) && _BR_PHR_SEL_CurrentWeight.OUTDATAs.Count > 0)
                                                {
                                                    string curWeight = string.Format("{0:F3}", _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].Weight);
                                                    this._SUnit = _BR_PHR_SEL_CurrentWeight.OUTDATAs[0].UOM;

                                                    if (_repeater != null && _repeater.IsEnabled)
                                                    {
                                                        _ScaleOnOff = true;
                                                        //txtTotalWpopOUTBIN = curWeight + " " + _SUnit;
                                                        //CurrentWeghing = curWeight;
                                                        if (isFirstVisbility == Visibility.Visible)
                                                        {
                                                            CurrentWeghing = curWeight;
                                                        }
                                                        else if (isSecondVisbility == Visibility.Visible)
                                                        {
                                                            txtTotalWpopOUTBIN = curWeight + " " + _SUnit;
                                                            isEOutWeighBtn = true;
                                                        }
                                                        else if (isThirdVisibility == Visibility.Visible)
                                                        {
                                                            if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count > 0)
                                                            {
                                                                var item = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs[0];

                                                                if (item.TOTALQTY_LOWER <= Convert.ToDecimal(curWeight) && Convert.ToDecimal(curWeight) <= item.TOTALQTY_UPPER)
                                                                {
                                                                    item.REALQTY = Convert.ToDecimal(curWeight);
                                                                    item.CHECKWEIGHT = "중량일치";
                                                                    isEOutWeighBtn = true;
                                                                }
                                                                else
                                                                    isEOutWeighBtn = false;
                                                            }
                                                        }
                                                        else if (isFourthVisibility == Visibility.Visible)
                                                        {
                                                            var tmparm = _OutWeghing.dgDetailList.SelectedItem as BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAIL;

                                                            if (tmparm != null && _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count > 0)
                                                            {
                                                                foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs)
                                                                {
                                                                    if (item.MTRLID == tmparm.MTRLID && item.MLOTID == tmparm.MLOTID && item.MSUBLOTID == tmparm.MSUBLOTID)
                                                                    {
                                                                        if (item.TOTALQTY_LOWER <= Convert.ToDecimal(curWeight) && Convert.ToDecimal(curWeight) <= item.TOTALQTY_UPPER)
                                                                        {
                                                                            item.REALQTY = Convert.ToDecimal(curWeight);
                                                                            item.CHECKWEIGHT = "무게일치";
                                                                            isEOutWeighBtn = true;
                                                                            item.CHK_FLAG = "Y";
                                                                        }
                                                                        else
                                                                            isEOutWeighBtn = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    OutWegingMsg = "연결실패";
                                                    _SUnit = "";
                                                    _ScaleOnOff = false;
                                                    isEOutWeighBtn = false;
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
                            }
                            _OutWeghing.txtBinBarcode.Focus();
                            ///

                            CommandResults["OUTWeigingLoadedCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["OUTWeigingLoadedCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["OUTWeigingLoadedCommand"] = true;

                            IsBusyP2 = false;
                            _OutWeghing.txtBinBarcode.Focus();
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("OUTWeigingLoadedCommand") ?
                        CommandCanExecutes["OUTWeigingLoadedCommand"] : (CommandCanExecutes["OUTWeigingLoadedCommand"] = true);
                });
            }
        }

        public ICommand BinBarcodeScanCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["BinBarcodeScanCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["BinBarcodeScanCommand"] = false;
                            CommandCanExecutes["BinBarcodeScanCommand"] = false;

                            ///

                            if (arg != null && arg is TextBox)
                            {
                                if (CurrentWeghing != "" && CurrentWeghing.Length > 0)
                                {

                                    _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Clear();
                                    _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs.Clear();
                                    _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Clear();
                                    _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Clear();

                                    _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Add(new BR_BRS_GET_VESSEL_INFO_DETAIL.INDATA
                                    {
                                        VESSELID = (arg as TextBox).Text
                                    });

                                    await _BR_BRS_GET_VESSEL_INFO_DETAIL.Execute();
                                    OutWegingMsg = "";
                                }
                            }
                            _OutWeghing.txtBinBarcode.Focus();

                            ///

                            CommandResults["BinBarcodeScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["BinBarcodeScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["BinBarcodeScanCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("BinBarcodeScanCommand") ?
                        CommandCanExecutes["BinBarcodeScanCommand"] : (CommandCanExecutes["BinBarcodeScanCommand"] = true);
                });
            }
        }

        public ICommand PalletBinBarcodeScanCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["PalletBinBarcodeScanCommand"].EnterAsync())
                    {
                        try
                        {
                            IsBusyP2 = true;

                            CommandResults["PalletBinBarcodeScanCommand"] = false;
                            CommandCanExecutes["PalletBinBarcodeScanCommand"] = false;

                            ///
                            if (arg != null && arg is TextBox)
                            {
                                if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count > 0)
                                {
                                    foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs)
                                    {
                                        if (item.MSUBLOTBCD == (arg as TextBox).Text)
                                        {
                                            _OutWeghing.dgDetailList.SelectedIndex = Convert.ToInt16(item.RowIndex);
                                        }
                                    }
                                }
                            }
                            _OutWeghing.txtPalletBinBarcode.Focus();
                            ///

                            CommandResults["PalletBinBarcodeScanCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["PalletBinBarcodeScanCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["PalletBinBarcodeScanCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("PalletBinBarcodeScanCommand") ?
                        CommandCanExecutes["PalletBinBarcodeScanCommand"] : (CommandCanExecutes["PalletBinBarcodeScanCommand"] = true);
                });
            }
        }

        public ICommand dgOutputListClickCommand
        {
            get
            {
                return new AsyncCommandBase(async arg =>
                {
                    using (await AwaitableLocks["dgOutputListClickCommand"].EnterAsync())
                    {
                        try
                        {
                            CommandResults["dgOutputListClickCommand"] = false;
                            CommandCanExecutes["dgOutputListClickCommand"] = false;

                            ///

                            if (arg == null) return;

                            var temparm = (arg as C1.Silverlight.DataGrid.C1DataGrid).SelectedItem as OutData;

                            if (temparm == null) return;

                            if (temparm.CHK)
                            {
                                temparm.CHK = false;
                            }
                            else
                            {
                                temparm.CHK = true;
                            }
                            ///

                            CommandResults["dgOutputListClickCommand"] = true;
                        }
                        catch (Exception ex)
                        {
                            CommandResults["dgOutputListClickCommand"] = false;
                            OnException(ex.Message, ex);
                        }
                        finally
                        {
                            CommandCanExecutes["dgOutputListClickCommand"] = true;

                            IsBusyP2 = false;
                        }
                    }
                }, arg =>
                {
                    return CommandCanExecutes.ContainsKey("dgOutputListClickCommand") ?
                        CommandCanExecutes["dgOutputListClickCommand"] : (CommandCanExecutes["dgOutputListClickCommand"] = true);
                });
            }
        }
        #endregion

        #region[Generator]

        public StorageInOutWmsViewModel()
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
            _BR_BRS_SEL_ProductionOrderDetail_ORDER = new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrderDetail_ORDER();
            _BR_PHR_SEL_EquipmentGroup = new BR_PHR_SEL_EquipmentGroup();
            _CboTypeItem = new LGCNS.iPharmMES.Common.BR_PHR_SEL_EquipmentGroup.OUTDATA();
            _BR_BRS_SEL_ProductionOrder_OrderList = new LGCNS.iPharmMES.Common.BR_BRS_SEL_ProductionOrder_OrderList();
            _StorageOutData = new ObservableCollection<StorageOutData>();
            _StorageOutData2 = new ObservableCollection<StorageOutData>();
            _SelectedData = new StorageOutData();
            _StorageInputData = new ObservableCollection<StorageInputData>();
            _SelectedItem = new StorageInputData();
            _BR_BRS_REG_WMS_Request_OUT = new BR_BRS_REG_WMS_Request_OUT();
            _BR_BRS_REG_WMS_Request_OUT.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_REG_WMS_Request_OUT_OnExecuteCompleted);
            _BR_PHR_SEL_System_PC_IP_ID = new LGCNS.iPharmMES.Common.BR_PHR_SEL_System_PC_IP_ID();
            _BR_PHR_SEL_System_PC_IP_ID.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_PHR_SEL_System_PC_IP_ID_OnExecuteCompleted);
            _BR_PHR_SEL_ProductionOrderOutput_Define = new LGCNS.iPharmMES.Common.BR_PHR_SEL_ProductionOrderOutput_Define();
            txtBarcode = "";
            txtProdInfoOut = "";
            txtBatchNoOut = "";
            txtOrderNoOut = "";
            txtProcessOut = "";
            txtIBCIDOutput = "";
            txtBinWtOut = "";
            tbOrderBatch = "";
            txtOrderBatch = "";
            txtVesselWt = "";
            txtConWt = "";
            txtTotalWts = "";
            txtYelid = "";

            txtComment = "*반제품창고 1F 입출고대 : ";
            txtComment2 = "온도 : 30 습도 : 60%";
            LbClrComment = "#FFFF0000";
            LbClrComment2 = "#FF000000";

            fontSizeGoodWt = 25;
            _Orderbatch = new OrderBatchnoPopup();
            _InWeighing = new StorageInWeighing();

            _BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo = new BR_PHR_SEL_EquipmentCustomAttributeValue_ScaleInfo();
            _CboProcess = new BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATA();
            _CboOutput = new BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATA();
            _BR_BRS_REG_VESSEL_WEIGHT = new BR_BRS_REG_VESSEL_WEIGHT();
            _BR_PHR_SEL_Equipment_GetLocation_CBO = new BR_PHR_SEL_Equipment_GetLocation_CBO();
            _BR_PHR_SEL_Equipment_GetLocation_CBO.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_PHR_SEL_Equipment_GetLocation_CBO_OnExecuteCompleted);
            _BR_BRS_GET_VESSEL_INFO_DETAIL = new BR_BRS_GET_VESSEL_INFO_DETAIL();
            _BR_BRS_GET_VESSEL_INFO_DETAIL.OnExecuteCompleted += new DelegateExecuteCompleted(_BR_BRS_GET_VESSEL_INFO_DETAIL_OnExecuteCompleted);
        }


        #endregion

        #region [User Define Method]

        public void ScanSaveAffterOut(string ScanData)
        {
            try
            {
                IsBusyA = true;

                if (StorageOutData.Count > 0)
                {
                    var match = StorageOutData.Where(o => o.IBC == ScanData).FirstOrDefault();

                    if (match == null)
                    {
                        OnMessage(MessageTable.M[CommonMessageCode._10009].ToString());
                        _mainWnd.TabOutput.Focus();
                    }
                    else
                    {
                        if (match.isSaved)
                        {
                            SelectedData = match;

                            var popup = new StorageInWeighing();

                            popup.DataContext = this;
                            txtTotalWpop = "";

                            popup.OKButton.Click += (s, e) =>
                            {
                                if (txtTotalWpop != null && txtTotalWpop.Length > 0)
                                {
                                    txtGoodWtout = txtTotalWpop;
                                    SelectedData.WeightOut = txtTotalWpop;
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnMessage(ex.Message);
                IsBusyA = false;
            }
            finally
            {
                IsBusyA = false;
            }
        }

        public void Resize()
        {

        }

        #endregion

    }

    #region [StorageInputData]
    public class StorageInputData : ViewModelBase
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

    #region [StorageOutData]

    public class StorageOutData : ViewModelBase
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
        private bool _isSaved;
        public bool isSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                OnPropertyChanged("isSaved");
            }
        }
        public string _WeightOut;
        public string WeightOut
        {
            get { return _WeightOut; }
            set
            {
                _WeightOut = value;
                OnPropertyChanged("WeightOut");
            }
        }
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }
    }
    #endregion
}
