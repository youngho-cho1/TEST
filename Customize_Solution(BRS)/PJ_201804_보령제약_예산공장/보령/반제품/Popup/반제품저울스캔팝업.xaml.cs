using LGCNS.EZMES.ControlsLib;
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
    public partial class 반제품저울스캔팝업 : iPharmMESChildWindow
    {
        public 반제품저울스캔팝업()
        {
            InitializeComponent();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
