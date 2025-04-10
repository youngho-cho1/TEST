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
    [Description("타정공정 수율 관리")]
    public partial class 타정공정수율_다층정 : ShopFloorCustomWindow
    {
        public 타정공정수율_다층정()
        {
            InitializeComponent();

            this.DataContext = new 타정공정수율_다층정ViewModel();           
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

       
    }
}
