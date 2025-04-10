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
using LGCNS.EZMES.ControlsLib;
using System.Windows.Controls;
using System.Windows.Media;
using C1.Silverlight.DataGrid;
using System.ComponentModel;

namespace 보령
{
    [Description("현장에서 출고된 정제수에 대한 칭량 및 투입")]
    public partial class 정제수확인및투입 : ShopFloorCustomWindow
    {
        public override string TableTypeName
        {
            get { return "TABLE,정제수확인및투입"; }
        }

        public 정제수확인및투입()
        {
            InitializeComponent();

            //System.Text.StringBuilder empty = new System.Text.StringBuilder();
            //LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
