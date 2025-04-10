using ShopFloorUI;
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

namespace 보령
{
    [ShopFloorCustomHidden]
    public partial class 현장칭량실적확인WMS전송 : ShopFloorCustomWindow
    {
        public 현장칭량실적확인WMS전송()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,현장칭량실적확인WMS전송"; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
