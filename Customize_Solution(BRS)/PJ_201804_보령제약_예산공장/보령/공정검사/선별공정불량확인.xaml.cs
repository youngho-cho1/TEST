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
    public partial class 선별공정불량확인 : ShopFloorCustomWindow
    {
        [Description("선별공정불량확인")]
        public 선별공정불량확인()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,선별공정불량확인"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
