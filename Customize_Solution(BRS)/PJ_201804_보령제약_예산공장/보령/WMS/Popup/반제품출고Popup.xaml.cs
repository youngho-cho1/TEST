using LGCNS.iPharmMES.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 보령
{
    public partial class 반제품출고Popup : iPharmMESChildWindow
    {
        public 반제품출고Popup()
        {
            InitializeComponent();
        }

        #region [Property & BizRule]
        public string VesselId;
        public decimal Weight;
        public string UOM;
        public VesselType curType;
        public string ScaleId;

        private decimal lower;
        private decimal upper;

        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 1000;

        private BR_BRS_GET_VESSEL_INFO_DETAIL _BR_BRS_GET_VESSEL_INFO_DETAIL = new BR_BRS_GET_VESSEL_INFO_DETAIL();
        public BR_BRS_GET_VESSEL_INFO_DETAIL BR_BRS_GET_VESSEL_INFO_DETAIL
        {
            get { return _BR_BRS_GET_VESSEL_INFO_DETAIL; }
            set
            {
                _BR_BRS_GET_VESSEL_INFO_DETAIL = value;
            }
        }

        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
        public BR_BRS_SEL_CurrentWeight BR_BRS_SEL_CurrentWeight
        {
            get { return _BR_BRS_SEL_CurrentWeight; }
            set
            {
                _BR_BRS_SEL_CurrentWeight = value;
            }
        }
        #endregion

        #region [Event]

        async private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //curType = VesselId.Contains("PAL") ? VesselType.PALLET : VesselType.IBC;
            curType = VesselType.IBC;

            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;

            this.Closed += (s, e1) =>
            {
                if (_repeater != null)
                    _repeater.Stop();

                _repeater = null;
            };

            _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Clear();
            _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs.Clear();
            _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Clear();
            _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Clear();

            _BR_BRS_GET_VESSEL_INFO_DETAIL.INDATAs.Add(new BR_BRS_GET_VESSEL_INFO_DETAIL.INDATA
            {
                VESSELID = VesselId
            });

            if (await _BR_BRS_GET_VESSEL_INFO_DETAIL.Execute() && (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATAs.Count > 0 || _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count > 0 || _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count > 0))
            {
                if (curType == VesselType.IBC)
                {
                    this.IBCInfoList.Visibility = Visibility.Visible;
                    this.PalletInfoList.Visibility = Visibility.Collapsed;
                    txtType.Content = "용기번호";
                    IBCInfoList.ItemsSource = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs;
                }
                else
                {
                    this.IBCInfoList.Visibility = Visibility.Collapsed;
                    this.PalletInfoList.Visibility = Visibility.Visible;
                    txtType.Content = "자재바코드";
                    PalletInfoList.ItemsSource = _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs;
                }
            }

            txtBCDScan.Focus();
        }

        private void txtBCDScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchItem(txtBCDScan.Text.ToUpper());
        }
        private void btnSelTarget_Click(object sender, RoutedEventArgs e)
        {
            SearchItem(txtBCDScan.Text.ToUpper());
        }

        private void btnChkTarget_Click(object sender, RoutedEventArgs e)
        {
            CheckItem();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            _repeater.Stop();
            _repeater = null;

            if (curType == VesselType.IBC)
            {
                if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count < 1 || _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count(x => x.CHECKWEIGHT.Equals("검량대기")) > 0)
                    this.DialogResult = false;
                else
                    this.DialogResult = true;

            }
            else
            {
                if (_BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs.Count < 1 || _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs.Count(x => x.CHECKWEIGHT.Equals("검량대기")) > 0)
                    this.DialogResult = false;
                else
                    this.DialogResult = true;
            }
            
        }
        #endregion

        #region [Custom]
        public enum VesselType { IBC, PALLET }

        async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA()
                {
                    ScaleID = string.IsNullOrWhiteSpace(this.ScaleId) ? "" : this.ScaleId
                });

                if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) == true)
                {
                    Weight = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : 0m;
                    txtWeight.Content = string.Format("{0:#0.00#}g", Weight);
                    UOM = "g";
                    if (curType == VesselType.IBC)
                        (this.IBCInfoList.SelectedItem as BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIP).REALQTY = Convert.ToDecimal(Weight);
                    else
                        (this.PalletInfoList.SelectedItem as BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAIL).REALQTY = Convert.ToDecimal(Weight);
                }
                else
                {
                    Weight = 0m;
                    txtWeight.Content = "저울 연결 실패";

                }
                    
            }
            catch (Exception ex)
            {
                Weight = 0m;
                UOM = "";
            }
            finally
            {
                _repeater.Start();
                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();
            }
        }

        private void SearchItem(string target)
        {
            if (curType == VesselType.IBC)
            {
                foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIPs)
                {
                    if (item.VESSELID.ToUpper().Equals(target.ToUpper()))
                    {
                        IBCInfoList.SelectedItem = item;
                        SetLowerUpper(item);
                        _repeater.Start();
                    }
                }
            }
            else
            {
                foreach (var item in _BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAILs)
                {
                    if (item.MSUBLOTBCD.ToUpper().Equals(target.ToUpper()))
                    {
                        PalletInfoList.SelectedItem = item;
                        SetLowerUpper(item);
                        _repeater.Start();
                    }
                }
            }
        }
        private void SetLowerUpper(BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIP target)
        {
            lower = target.TOTALQTY_LOWER.HasValue ? target.TOTALQTY_LOWER.Value : 0m;
            upper = target.TOTALQTY_UPPER.HasValue ? target.TOTALQTY_UPPER.Value : 0m;

            txtUpper.Content = string.Format("{0:#0.00#}g", target.TOTALQTY_UPPER);
            txtLower.Content = string.Format("{0:#0.00#}g", target.TOTALQTY_LOWER);
        }
        private void SetLowerUpper(BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAIL target)
        {
            lower = target.TOTALQTY_LOWER.HasValue ? target.TOTALQTY_LOWER.Value : 0m;
            upper = target.TOTALQTY_UPPER.HasValue ? target.TOTALQTY_UPPER.Value : 0m;

            txtUpper.Content = string.Format("{0:#0.00#}g", target.TOTALQTY_UPPER);
            txtLower.Content = string.Format("{0:#0.00#}g", target.TOTALQTY_LOWER);
        }
        private void CheckItem()
        {
            _repeater.Stop();

            if (curType == VesselType.IBC)
            {
                var temp = IBCInfoList.SelectedItem as BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_WIP;

                if (temp.CHECKWEIGHT.Equals("검량대기"))
                {
                    if (lower <= Weight && Weight <= upper)
                    {
                        temp.CHECKWEIGHT = "검량완료";
                        IBCInfoList.SelectedItem = null;
                    }
                    else
                        _repeater.Start();
                }
                else
                    _repeater.Start();
            }
            else
            {
                var temp = PalletInfoList.SelectedItem as BR_BRS_GET_VESSEL_INFO_DETAIL.OUTDATA_DETAIL;

                if (temp.CHECKWEIGHT.Equals("검량대기"))
                {
                    if (lower <= Weight && Weight <= upper)
                    {
                        temp.CHECKWEIGHT = "검량완료";
                        PalletInfoList.SelectedItem = null;
                    }
                    else
                        _repeater.Start();
                }
                else
                    _repeater.Start();
            }
        }
        #endregion


    }
}
