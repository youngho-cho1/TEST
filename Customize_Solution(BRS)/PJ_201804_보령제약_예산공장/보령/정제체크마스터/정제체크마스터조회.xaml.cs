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
    [Description("정제체크마스터의 TAG 값을 조회하여 IPC에 저장")]
    public partial class 정제체크마스터조회 : ShopFloorCustomWindow
    {
        public 정제체크마스터조회()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,정제체크마스터조회_수정"; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }       
    }
}
