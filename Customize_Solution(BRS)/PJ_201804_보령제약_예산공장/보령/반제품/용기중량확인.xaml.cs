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
    public partial class 용기중량확인 : ShopFloorUI.ShopFloorCustomWindow
    {
        public 용기중량확인()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,용기중량확인"; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtboxIBCID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (this.LayoutRoot.DataContext as 용기중량확인ViewModel).SerachCommandAsync.Execute(this.txtboxIBCID.Text);
            }
        }
    }
}

