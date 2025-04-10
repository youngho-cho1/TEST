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
    public partial class SVP수동검사불량유형입력 : ShopFloorCustomWindow
    {
        [Description("SVP 수동검사 불량유형 입력")]
        public SVP수동검사불량유형입력()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "불량유형입력"; }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
