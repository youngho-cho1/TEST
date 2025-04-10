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
    [Description("원료가 적재된 IBC의 반제품 무게측정(입고전 적재 원료 최종확인)")]
    public partial class 칭량IBC반제품무게측정 : ShopFloorCustomWindow
    {
        
        public 칭량IBC반제품무게측정()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,칭량IBC반제품무게측정"; }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
