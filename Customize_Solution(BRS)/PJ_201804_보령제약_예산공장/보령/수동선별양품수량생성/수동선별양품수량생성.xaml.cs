using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace 보령
{
    [Description("각각의 칭량된 원료 투입 처리 (재고 차감)")]
    public partial class 수동선별양품수량생성 : ShopFloorCustomWindow
    {
        public 수동선별양품수량생성()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,수동선별양품수량생성"; }
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
