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
using System.Windows.Navigation;
using ShopFloorUI;

namespace 보령
{
    public partial class 반제품입고 : ShopFloorCustomWindow
    {
        public 반제품입고()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,반제품입고"; }
        }
        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            if (Phase != null)
            {
                if (await Phase.SessionCheck() != enumInstructionRegistErrorType.Ok)
                    DialogResult = false;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtVesselId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((반제품입고ViewModel)LayoutRoot.DataContext).CheckIBCInfoCommandAsync.Execute(txtVesselId.Text);
            }
        }
    }
}
