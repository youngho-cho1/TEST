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
    [Description("포장공정 샘플 수량 입력(TnT 시스템을 사용하지 않는 경우)")]
    public partial class 샘플수량수동입력 : ShopFloorCustomWindow
    {
        public 샘플수량수동입력()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,샘플수량수동입력"; }
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
