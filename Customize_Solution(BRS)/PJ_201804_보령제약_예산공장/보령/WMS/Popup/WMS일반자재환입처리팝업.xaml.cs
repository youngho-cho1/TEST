using LGCNS.iPharmMES.Common;
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
    public partial class WMS일반자재환입처리팝업 : iPharmMESChildWindow
    {
        public WMS일반자재환입처리팝업()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {            
            this.DialogResult = true;
        }

    }
}
