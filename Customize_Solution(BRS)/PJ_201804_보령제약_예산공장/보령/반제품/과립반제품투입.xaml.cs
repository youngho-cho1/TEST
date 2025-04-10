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
using System.ComponentModel;

namespace 보령
{
    [Description("혼합 공정에 과립반제품 투입")]
    public partial class 과립반제품투입 : ShopFloorCustomWindow
    {

        public override string TableTypeName
        {
            get { return "TABLE,과립반제품투입"; }
        }

        public 과립반제품투입()
        {
            InitializeComponent();

            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
