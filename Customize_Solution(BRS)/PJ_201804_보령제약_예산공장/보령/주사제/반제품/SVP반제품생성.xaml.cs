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
    [Description("SVP반제품생성(용기사용시작)")]
    public partial class SVP반제품생성 : ShopFloorCustomWindow
    {
        public SVP반제품생성()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,SVP반제품생성"; }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }       
    }
}
