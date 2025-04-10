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
using System.ComponentModel;

namespace 보령
{
    [Description("포장자재환입(일반자재)")]
    public partial class WMS일반자재환입처리 : ShopFloorCustomWindow
    {
        public WMS일반자재환입처리()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,WMS일반자재환입처리"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }       
    }
}
