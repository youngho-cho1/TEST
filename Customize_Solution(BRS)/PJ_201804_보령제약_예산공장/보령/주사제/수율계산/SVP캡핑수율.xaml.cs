using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;

namespace 보령
{
    [Description("SVP Al-Cap 캡핑 수율 관리")]
    public partial class SVP캡핑수율 : ShopFloorCustomWindow
    {
        public SVP캡핑수율()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }


    }
}
