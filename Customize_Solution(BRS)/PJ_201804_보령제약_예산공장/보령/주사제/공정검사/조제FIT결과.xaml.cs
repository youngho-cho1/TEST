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

namespace 보령
{
    public partial class 조제FIT결과 : ShopFloorCustomWindow
    {
        public 조제FIT결과()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,조제FIT결과"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void txtFITCount_ValueChanged(object sender, EventArgs e)
        {
            (LayoutRoot.DataContext as 조제FIT결과ViewModel).isFITInput();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as 조제FIT결과ViewModel).isConfirmInput();
        }
    }
}
