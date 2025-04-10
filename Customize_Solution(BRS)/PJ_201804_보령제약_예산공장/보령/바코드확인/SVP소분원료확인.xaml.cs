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
    [ShopFloorCustomHidden]
    [Description("칭량된 원료 확인")]
    public partial class SVP소분원료확인 : ShopFloorCustomWindow
    {
        public SVP소분원료확인()
        {
            InitializeComponent();
            this.DataContext = new SVP소분원료확인ViewModel();
        }

        public override string TableTypeName
        {
            get { return "TABLE,SVP소분원료확인"; }
        }
        
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
