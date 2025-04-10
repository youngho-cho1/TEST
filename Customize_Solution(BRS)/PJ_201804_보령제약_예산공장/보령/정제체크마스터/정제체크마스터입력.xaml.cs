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
    //[ShopFloorCustomHidden]
    public partial class 정제체크마스터입력 : ShopFloorCustomWindow
    {

        public override string TableTypeName
        {
            get { return "TABLE,정제체크마스터입력"; }
        }
        
        public 정제체크마스터입력()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }
    }
}
