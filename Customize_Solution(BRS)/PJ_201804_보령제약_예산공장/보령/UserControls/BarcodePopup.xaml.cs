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

namespace 보령.UserControls
{
    public partial class BarcodePopup : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {
        public BarcodePopup()
        {
            InitializeComponent();

            this.Loaded += BarcodePopup_Loaded;
        }

        void BarcodePopup_Loaded(object sender, RoutedEventArgs e)
        {
            if (tbText.Dispatcher.CheckAccess()) tbText.Focus();
            else tbText.Dispatcher.InvokeAsync(() => tbText.Focus());
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void tbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
            }
        }
    }
}

