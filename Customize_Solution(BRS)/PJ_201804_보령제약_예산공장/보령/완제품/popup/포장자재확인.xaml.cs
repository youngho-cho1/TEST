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
using LGCNS.iPharmMES.Common;

namespace 보령
{
    public partial class 포장자재확인 : iPharmMESChildWindow
    {
        public 포장자재확인()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
                this.DialogResult = true;
        }
    }
}

