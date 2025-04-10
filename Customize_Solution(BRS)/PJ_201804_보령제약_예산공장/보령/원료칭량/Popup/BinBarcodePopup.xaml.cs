using LGCNS.EZMES.ControlsLib;
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
//using 보령.DataModel.WeighInitializeModel;

namespace 보령
{
    public partial class BinBarcodePopup : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {
        public BinBarcodePopup()
        {
            InitializeComponent();
        }

        private void txtWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if (e.Key != Key.Back && e.Key != Key.Shift && e.Key != Key.Unknown)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtBinBarcode_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBinBarcode.SelectAll();
        }
    }
}

