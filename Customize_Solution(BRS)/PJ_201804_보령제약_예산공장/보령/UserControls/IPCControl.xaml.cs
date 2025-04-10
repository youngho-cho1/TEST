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
using static 보령.IPCControlData;

namespace 보령.UserControls
{
    public partial class IPCControl : UserControl
    {
        public IPCControl()
        {
            InitializeComponent();
        }

        #region Dependency Property
        private IPCControlData _IPCDATA;
        public IPCControlData IPCDATA
        {
            get { return (IPCControlData)GetValue(IPCDATAProperty); }
            set { SetValue(IPCDATAProperty, value); }
        }

        public static readonly DependencyProperty IPCDATAProperty =
            DependencyProperty.Register("IPCDATA", typeof(IPCControlData), typeof(IPCControl), new PropertyMetadata(OnIPCDATAProperty));

        private static void OnIPCDATAProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPCControl parent = (IPCControl)d;
            //값을 전달할 타겟지정
            IPCControlData IPCDATA = (IPCControlData)e.NewValue;
            parent._IPCDATA = IPCDATA;
            // 화면 Setup
            parent.tbIPCName.Text = IPCDATA.TINAME;
            parent.tbIPCStandard.Text = IPCDATA.Standard;

            if(!IPCDATA.DEVIATIONFLAG.HasValue)
            {
                parent.tbIPCResult.Foreground = new SolidColorBrush(Colors.Black);
                parent.tbIPCResult.Text = String.IsNullOrEmpty(IPCDATA.ACTVALDESC) ? IPCDATA.GetACTVAL: IPCDATA.ACTVALDESC;
                parent.bdIPCResult.Background = new SolidColorBrush(Colors.LightGray);
                parent.btnIPCStart.Content = "검사";
            }
        }
        #endregion

        private void btnIPCStart_Click(object sender, RoutedEventArgs e)
        {
            var popup = new 공정검사결과입력();
            var popupVM = new 공정검사결과입력ViewModel(popup, IPCDATA);
            popup.BusyIn.DataContext = popupVM;
            popup.Show();
            popup.Closed += (s1, e1) =>
            {
                if(_IPCDATA.DEVIATIONFLAG.HasValue)
                {
                    if(_IPCDATA.DEVIATIONFLAG.Value)
                    {
                        tbIPCResult.Foreground = new SolidColorBrush(Colors.White);
                        tbIPCResult.Text = String.IsNullOrEmpty(_IPCDATA.ACTVALDESC) ? _IPCDATA.GetACTVAL : _IPCDATA.ACTVALDESC;
                        bdIPCResult.Background = new SolidColorBrush(Colors.Green);
                        btnIPCStart.Content = "재검사";
                    }
                    else
                    {
                        tbIPCResult.Foreground = new SolidColorBrush(Colors.White);
                        tbIPCResult.Text = String.IsNullOrEmpty(_IPCDATA.ACTVALDESC) ? _IPCDATA.GetACTVAL : _IPCDATA.ACTVALDESC;
                        bdIPCResult.Background = new SolidColorBrush(Colors.Red);
                        btnIPCStart.Content = "재검사";
                    }
                }
                else
                {
                    tbIPCResult.Foreground = new SolidColorBrush(Colors.Black);
                    tbIPCResult.Text = String.IsNullOrEmpty(_IPCDATA.ACTVALDESC) ? _IPCDATA.GetACTVAL : _IPCDATA.ACTVALDESC;
                    bdIPCResult.Background = new SolidColorBrush(Colors.LightGray);
                    btnIPCStart.Content = "검사";
                }
            };
        }
    }
}
