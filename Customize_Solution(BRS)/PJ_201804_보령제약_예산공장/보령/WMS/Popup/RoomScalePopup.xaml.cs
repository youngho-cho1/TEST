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
    public partial class RoomScalePopup : iPharmMESChildWindow
    {
        public RoomScalePopup()
        {
            InitializeComponent();
        }

        #region [Property & BizRule]
        private string strDisplay = "";
        
        public string scaleId;
        public string UOM;
        public decimal weight;

        private DispatcherTimer _repeater = new DispatcherTimer();
        private int _repeaterInterval = 1000;

        private BR_BRS_SEL_CurrentWeight _BR_BRS_SEL_CurrentWeight = new BR_BRS_SEL_CurrentWeight();
        public BR_BRS_SEL_CurrentWeight BR_BRS_SEL_CurrentWeight
        {
            get { return _BR_BRS_SEL_CurrentWeight; }
            set { _BR_BRS_SEL_CurrentWeight = value; }
        }
        #endregion

        #region [Event]
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out _repeaterInterval) == false)
                _repeaterInterval = 2000;

            _repeater.Interval = new TimeSpan(0, 0, 0, 0, _repeaterInterval);
            _repeater.Tick += _repeater_Tick;

            _repeater.Start();

            this.Closed += (s, e1) =>
            {
                if (_repeater != null)
                    _repeater.Stop();

                _repeater = null;
            };
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            _repeater.Stop();
            _repeater = null;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _repeater.Stop();
            _repeater = null;
            this.DialogResult = false;
        }
        #endregion

        #region [Custom]
        async void _repeater_Tick(object sender, EventArgs e)
        {
            try
            {
                _repeater.Stop();

                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.INDATAs.Add(new BR_BRS_SEL_CurrentWeight.INDATA()
                {
                    ScaleID = this.scaleId
                });

                if (await _BR_BRS_SEL_CurrentWeight.Execute(exceptionHandle: LGCNS.iPharmMES.Common.Common.enumBizRuleXceptionHandleType.FailEvent) == true)
                {
                    weight = _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.HasValue ? _BR_BRS_SEL_CurrentWeight.OUTDATAs[0].Weight.Value : 0m;
                    UOM = "g";
                    txtComment.Content = string.Format("측정된 결과는 {0}{1} 입니다.", weight, "g");
                    btnConfirm.IsEnabled = true;
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                txtComment.Content = "저울 연결 실패";
                weight = 0m;
                UOM = "";
                btnConfirm.IsEnabled = false;
            }
            finally
            {
                _repeater.Start();
                _BR_BRS_SEL_CurrentWeight.INDATAs.Clear();
                _BR_BRS_SEL_CurrentWeight.OUTDATAs.Clear();
            }
        }
        #endregion
    }
}
