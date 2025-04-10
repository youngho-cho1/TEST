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
    [Description("SVP 수동검사 불량유형 조회")]
    public partial class SVP수동검사불량유형조회 : ShopFloorCustomWindow
    {
        public SVP수동검사불량유형조회()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "SVP불량유형조회"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
