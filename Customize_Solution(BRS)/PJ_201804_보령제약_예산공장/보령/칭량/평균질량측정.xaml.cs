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
    [Description("공정검사(평균질량) 측정 (저울 인터페이스 및 샘플수량 입력)")]
    public partial class 평균질량측정 : ShopFloorCustomWindow
    {
        public 평균질량측정()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,평균질량측정"; }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
