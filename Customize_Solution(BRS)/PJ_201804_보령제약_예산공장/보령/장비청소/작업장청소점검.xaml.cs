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
    public partial class 작업장청소점검 : ShopFloorCustomWindow
    {
        [Description("작업장청소점검")]
        public 작업장청소점검()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,작업장청소점검"; }
        }

        private void btnCacel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
