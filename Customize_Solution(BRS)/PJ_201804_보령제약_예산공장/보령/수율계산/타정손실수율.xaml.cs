using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ShopFloorUI;
using C1.Silverlight.DataGrid;
using LGCNS.EZMES.ControlsLib;

namespace 보령
{
    public partial class 타정손실수율 : ShopFloorCustomWindow
    {
        public 타정손실수율()
        {
            InitializeComponent();

            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);
        }
        public override string TableTypeName
        {
            get { return "TABLE,타정손실수율"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
