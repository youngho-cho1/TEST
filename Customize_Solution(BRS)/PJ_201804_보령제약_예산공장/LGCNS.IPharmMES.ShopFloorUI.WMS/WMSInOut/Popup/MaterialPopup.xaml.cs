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
    public partial class MaterialPopup : iPharmMESChildWindow
    {
        public MaterialPopup()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtGoodWt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtGoodWt.Text.Length > 0 && txtBintWt.Text.Length > 0)
                {
                    txtTotalWt.Text = (decimal.Parse(txtGoodWt.Text) + decimal.Parse(txtBintWt.Text)).ToString();
                }
            }
        }
    }
}

