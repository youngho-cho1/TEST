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
    [Description("반제품(공정중제품)의 보관기간확인")]
    public partial class 반제품보관정보확인 : ShopFloorCustomWindow
    {
        public 반제품보관정보확인()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,반제품보관정보확인"; }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

