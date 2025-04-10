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
using System.Windows.Navigation;
using LGCNS.iPharmMES.Common;
using System.Windows.Threading;

namespace 보령
{
    public partial class 로드셀점검팝업 : iPharmMESChildWindow
    {
        public 로드셀점검팝업()
        {
            InitializeComponent();

            // 타이머
            int interval = 2000;
            string interval_str = ShopFloorUI.App.Current.Resources["GetWeightInterval"].ToString();
            if (int.TryParse(interval_str, out interval) == false)
            {
                interval = 2000;
            }

            _DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            _DispatcherTimer.Tick += _DispatcherTimer_Tick;
            _DispatcherTimer.Start();

            this.Closed += (s, e) =>
            {
                if (_DispatcherTimer != null)
                    _DispatcherTimer.Stop();

                _DispatcherTimer = null;
            };
        }

        DispatcherTimer _DispatcherTimer = new DispatcherTimer();
        public string TAGID;
        public string EQTPID;
        async void _DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _DispatcherTimer.Stop();

                if (!string.IsNullOrWhiteSpace(TAGID))
                {
                    // 설비 태그값 조회
                    var br = new BR_PHR_SEL_RealtimeData();
                    br.INDATAs.Add(new BR_PHR_SEL_RealtimeData.INDATA
                    {
                        EQPTID = this.EQTPID,
                        TAGID = this.TAGID,
                        ACTVAL = ""
                    });

                    if(await br.Execute() == true)
                        txtTagValue.Content = br.OUTDATAs[0].ACTVAL;
                    
                    _DispatcherTimer.Start();
                }
            }
            catch (Exception ex)
            {
                if(_DispatcherTimer != null) _DispatcherTimer.Stop();
            }
        }
    }
}
