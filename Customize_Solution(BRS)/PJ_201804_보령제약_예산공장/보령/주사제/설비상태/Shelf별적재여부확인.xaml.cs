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
using ShopFloorUI;
using System.ComponentModel;
using System.Windows.Data;

namespace 보령
{
    [Description("Shelf별적재여부확인")]
    public partial class Shelf별적재여부확인 : ShopFloorCustomWindow
    {
        public Shelf별적재여부확인()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,Shelf별적재여부확인"; }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }
    }
}
