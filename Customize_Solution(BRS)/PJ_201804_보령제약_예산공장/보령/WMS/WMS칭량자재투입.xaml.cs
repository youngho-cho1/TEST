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

namespace 보령
{
    public partial class WMS칭량자재투입 : ShopFloorCustomWindow
    {
        public WMS칭량자재투입()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,WMS칭량자재투입"; }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
