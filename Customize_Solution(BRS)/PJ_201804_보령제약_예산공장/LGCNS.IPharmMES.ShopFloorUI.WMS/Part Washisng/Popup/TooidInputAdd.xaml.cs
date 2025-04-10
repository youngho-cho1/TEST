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
using LGCNS.iPharmMES.Common;

namespace WMS
{
    public partial class TooidInputAdd : iPharmMESChildWindow
    {
        public TooidInputAdd()
        {
            InitializeComponent();

            txtToolIdS.UpdateLayout();
            txtToolIdS.Focus();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtToolId_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void txtToolId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtToolIdS.Text.Length > 0)
            {
                txtToolIdS.SelectAll();
                OKButton.Command.Execute(OKButton.CommandParameter);
                this.DialogResult = true;
            }
        }

        private void iPharmMESChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Plugin.Focus();
            txtToolIdS.UpdateLayout();
        }

        private void txtToolIdS_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void iPharmMESChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            txtToolIdS.Focus();

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

