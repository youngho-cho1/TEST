using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using LGCNS.EZMES.Common;
using C1.Silverlight.Data;
using System.Text;
using C1.Silverlight;
using System.ComponentModel;

namespace 보령
{
    [Description("설비(장비, 룸, 도구)의 청소상태 확인 및 생산시작/종료, 청소시작/종료 액션 수행")]
    public partial class 장비청소상태 : ShopFloorCustomWindow
    {
        public override string TableTypeName
        {
            get { return "TABLE,장비청소상태"; }
        }

        public 장비청소상태()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
