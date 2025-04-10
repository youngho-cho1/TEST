using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.ComponentModel;

namespace 보령
{
    [Description("칭량하지 않은 원료 확인 및 실적 전송")]
    public partial class 미사용원료실적전송 : ShopFloorCustomWindow
    {
        public 미사용원료실적전송()
        {
            InitializeComponent();
            this.DataContext = new 미사용원료실적전송ViewModel();
        }

        public override string TableTypeName
        {
            get { return "TABLE,미사용원료실적전송"; }
        }
        
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
