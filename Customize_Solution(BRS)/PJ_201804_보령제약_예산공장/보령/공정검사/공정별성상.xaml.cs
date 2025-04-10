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
    [Description("공정별성상")]
    public partial class 공정별성상 : ShopFloorCustomWindow
    {
        public 공정별성상()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,공정별성상"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
