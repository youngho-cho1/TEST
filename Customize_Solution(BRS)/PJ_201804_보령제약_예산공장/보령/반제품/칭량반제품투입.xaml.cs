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

namespace 보령
{
    public partial class 칭량반제품투입 : ShopFloorCustomWindow
    {
        public 칭량반제품투입()
        {
            InitializeComponent();
        }
        
        public override string TableTypeName
        {
            get { return "TABLE,칭량반제품투입"; }
        }

        private void txtVesselId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((칭량반제품투입ViewModel)LayoutRoot.DataContext).SearchCommandAsync.Execute(txtVesselId.Text.ToUpper());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
       
    }
}
