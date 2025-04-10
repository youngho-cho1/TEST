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
    [Description("AQL검사기록")]
    public partial class AQL검사기록 : ShopFloorCustomWindow
    {
        public AQL검사기록()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,AQL검사기록"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
