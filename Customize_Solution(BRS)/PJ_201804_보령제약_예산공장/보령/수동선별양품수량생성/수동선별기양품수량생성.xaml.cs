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
    [Description("수동선별기양품수량생성")]
    public partial class 수동선별기양품수량생성 : ShopFloorCustomWindow
    {
        public 수동선별기양품수량생성()
        {
            InitializeComponent();

            this.DataContext = new 수동선별기양품수량생성ViewModel();           
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
