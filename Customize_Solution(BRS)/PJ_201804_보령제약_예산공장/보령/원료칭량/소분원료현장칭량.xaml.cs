using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace 보령
{
    [ShopFloorCustomHidden]
    public partial class 소분원료현장칭량 : ShopFloorCustomWindow
    {
        public 소분원료현장칭량()
        {
            InitializeComponent();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
