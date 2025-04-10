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
    public partial class 무균공정작업자입력 : ShopFloorCustomWindow
    {
        public 무균공정작업자입력()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,무균공정작업자입력"; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }

        private void txtUserId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((무균공정작업자입력ViewModel)LayoutRoot.DataContext).CheckUserIdCommandAsync.Execute(txtUserId.Text);
            }
        }
    }
}
