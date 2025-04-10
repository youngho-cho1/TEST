using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [Description("포장 공정에서 반제품 투입 전 분할 작업")]
    public partial class 포장공정반제품분할 : ShopFloorCustomWindow
    {
        public 포장공정반제품분할()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get
            {
                return "TABLE,포장공정반제품분할";
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
