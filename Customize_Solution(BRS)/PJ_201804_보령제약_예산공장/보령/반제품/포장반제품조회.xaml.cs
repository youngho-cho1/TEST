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
    public partial class 포장반제품조회 : ShopFloorCustomWindow
    {
        [Description("포장반제품조회")]
        public 포장반제품조회()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,포장반제품조회"; }
        }

        // 취소 버튼 클릭
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
