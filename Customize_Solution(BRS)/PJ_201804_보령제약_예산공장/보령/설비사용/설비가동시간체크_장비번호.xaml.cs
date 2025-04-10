﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using ShopFloorUI;
using System.ComponentModel;

namespace 보령
{
    public partial class 설비가동시간체크_장비번호 : ShopFloorCustomWindow
    {
        [Description("설비가동시간체크_장비번호")]
        public 설비가동시간체크_장비번호()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,설비가동시간체크_장비번호"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            (BusyIndicator.DataContext as 설비가동시간체크_장비번호ViewModel).TimerStop();
            this.DialogResult = true;
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            (BusyIndicator.DataContext as 설비가동시간체크_장비번호ViewModel).TimerStop();
        }

        private void btnCacel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
