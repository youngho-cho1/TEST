using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [Description("레시피확인")]
    public partial class 레시피확인 : ShopFloorCustomWindow
    {
        public 레시피확인()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "레시피확인"; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }       
    }
}
