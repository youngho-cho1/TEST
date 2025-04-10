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
    [Description("SVP 반제품 불량 유형별 결과 기록")]
    public partial class SVP불량검사결과 : ShopFloorCustomWindow
    {
        public SVP불량검사결과()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,SVP불량검사결과"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
