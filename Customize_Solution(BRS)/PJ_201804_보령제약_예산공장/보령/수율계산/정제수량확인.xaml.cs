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
    [Description("정제수량(양품, 불량) 태그 조회결과 기록")]
    public partial class 정제수량확인 : ShopFloorCustomWindow
    {
        public 정제수량확인()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,정제수량확인"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
