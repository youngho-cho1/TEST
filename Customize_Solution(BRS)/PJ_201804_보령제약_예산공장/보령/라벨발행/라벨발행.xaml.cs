using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls;
using LGCNS.EZMES.Common;
using LGCNS.iPharmMES.Common;
using LGCNS.EZMES.ControlsLib;
using C1.Silverlight.DataGrid;
using C1.Silverlight;
using C1.Silverlight.Data;
using System.ComponentModel;

namespace 보령
{
    [Description("라벨(공정표지판, 청소라벨) 발행")]
    public partial class 라벨발행: ShopFloorCustomWindow
    {
        public 라벨발행()
        {
            InitializeComponent();
        }

        //private void OKButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.DialogResult = true;
        //}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
