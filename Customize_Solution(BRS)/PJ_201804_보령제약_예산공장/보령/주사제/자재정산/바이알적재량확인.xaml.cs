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
    public partial class 바이알적재량확인 : ShopFloorCustomWindow
    {
        [Description("주사제 공정 중 트레이에 적재된 바이알 수량을 기록")]
        public 바이알적재량확인()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,바이알적재량확인"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
