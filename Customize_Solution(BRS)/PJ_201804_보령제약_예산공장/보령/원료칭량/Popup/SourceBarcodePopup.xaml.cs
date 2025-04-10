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
using 보령.UserControls;
//using 대응.ViewModels.DataModel.WeighInitializeModel;

namespace 보령
{
    public partial class SourceBarcodePopup : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {
        public SourceBarcodePopup()
        {
            InitializeComponent();
        }

        private void txtBarcode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            else
            {
                txtWeight.Focus();
            }
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

        private void txtBarcode_GotFocus(object sender, RoutedEventArgs e)
        {
            //txtBarcode.SelectAll();
        }
    }
}

