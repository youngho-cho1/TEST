﻿using System;
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
    [Description("포장완제품입고(TnT 시스템을 사용하지 않는 경우 실적수동입력하여 LGV호출. 수량을 입력하지 않고 공정중제품을 스캔하면 자동계산)")]
    public partial class 포장완제품입고3 : ShopFloorCustomWindow
    {
        public 포장완제품입고3()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,포장완제품입고3"; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

