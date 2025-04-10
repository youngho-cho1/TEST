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
using 보령.UserControls;
using static 보령.IPCControlData;

namespace 보령
{
    public partial class 공정검사결과입력 : iPharmMESChildWindow
    {
        private IPCControlData IPCData;
        private decimal? Value;
        public 공정검사결과입력()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
