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
using LGCNS.iPharmMES.Common;

namespace 보령
{
    public partial class SVP자재환입저울스캔팝업 : iPharmMESChildWindow
    {
        public SVP자재환입저울스캔팝업()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
