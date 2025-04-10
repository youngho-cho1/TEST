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
    public partial class LIMS샘플링확인 : ShopFloorUI.ShopFloorCustomWindow
    {
        public LIMS샘플링확인()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,LIMS샘플링확인"; }
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

