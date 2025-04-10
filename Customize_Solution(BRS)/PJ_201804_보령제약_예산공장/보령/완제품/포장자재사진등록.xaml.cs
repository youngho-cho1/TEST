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

namespace 보령
{
    [ShopFloorCustomHidden(false)]
    [System.ComponentModel.Description("포장 자재의 사진이미지를 저장")]
    public partial class 포장자재사진등록 : ShopFloorCustomWindow
    {
        public 포장자재사진등록()
        {
            InitializeComponent();
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

