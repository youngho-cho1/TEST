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
    [Description("캡슐선별공정 수율 관리")]
    public partial class 캡슐반제품수율 : ShopFloorCustomWindow
    {
        public 캡슐반제품수율()
        {
            InitializeComponent();

            this.DataContext = new 캡슐반제품수율ViewModel();           
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

       
    }
}
