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
using LGCNS.iPharmMES.Common;

namespace WMS
{
    public partial class StorageOutWheinging : iPharmMESChildWindow
    {
        public StorageOutWheinging()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            txtBinBarcode.Focus();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if ((LayoutRoot.DataContext as StorageInOutWmsViewModel).isFirstVisbility == Visibility.Visible)
            {
                txtBinBarcode.Focus();
            }

            if ((LayoutRoot.DataContext as StorageInOutWmsViewModel).isFourthVisibility == Visibility.Visible)
            {
                txtPalletBinBarcode.Focus();
            }
        }
    }
}

