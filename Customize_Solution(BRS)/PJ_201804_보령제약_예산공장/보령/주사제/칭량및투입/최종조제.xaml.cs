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
    [Description("주사용수를 투입하여 최종 조제액 조제")]
    public partial class 최종조제 : ShopFloorCustomWindow
    {
        public 최종조제()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,최종조제"; }
        }
        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
