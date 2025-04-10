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
    [Description("SVP캡핑IPC")]
    public partial class SVP캡핑IPC : ShopFloorCustomWindow
    {
        public SVP캡핑IPC()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,SVP캡핑IPC"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
