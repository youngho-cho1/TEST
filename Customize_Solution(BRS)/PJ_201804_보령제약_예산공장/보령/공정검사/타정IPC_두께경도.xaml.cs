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
    [Description("타정공정검사(두께, 직경, 경도")]
    public partial class 타정IPC_두께경도 : ShopFloorCustomWindow
    {
        public 타정IPC_두께경도()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,타정IPC_두께경도"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
