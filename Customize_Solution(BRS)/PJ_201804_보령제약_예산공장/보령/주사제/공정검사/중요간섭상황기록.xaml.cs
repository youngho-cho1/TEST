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

namespace 보령
{
    [Description("중요간섭상황기록")]
    public partial class 중요간섭상황기록 : ShopFloorCustomWindow
    {
        public 중요간섭상황기록()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,중요간섭상황기록"; }
        }
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
