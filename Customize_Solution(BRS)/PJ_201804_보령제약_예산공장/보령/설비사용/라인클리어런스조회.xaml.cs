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
    [ShopFloorCustomHidden]
    public partial class 라인클리어런스조회 : ShopFloorCustomWindow
    {
        [Description("LineClearance 조회")]
        public 라인클리어런스조회()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,라인클리어런스조회"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
