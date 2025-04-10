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
using System.Windows.Navigation;
using ShopFloorUI;
using System.ComponentModel;

namespace 보령
{
    [Description("조제액의 원료 보충")]
    public partial class 원료보충 : ShopFloorCustomWindow
    {
        public 원료보충()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,원료보충"; }
        }
        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
