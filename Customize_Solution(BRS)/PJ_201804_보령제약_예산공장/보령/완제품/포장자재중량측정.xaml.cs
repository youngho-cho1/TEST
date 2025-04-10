using ShopFloorUI;
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


namespace 보령
{
    [ShopFloorCustomHidden]
    public partial class 포장자재중량측정 : ShopFloorCustomWindow
    {
        public 포장자재중량측정()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,포장자재중량측정"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
