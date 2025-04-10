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

namespace 보령
{
    public partial class 포장자재정산 : ShopFloorCustomWindow
    {
        public 포장자재정산()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,포장자재정산"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
