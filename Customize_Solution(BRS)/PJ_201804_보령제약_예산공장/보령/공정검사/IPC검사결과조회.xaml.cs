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
using System.Windows.Navigation;
using ShopFloorUI;
using System.ComponentModel;

namespace 보령
{
    [Description("IPC 검사결과조회 화면")]
    public partial class IPC검사결과조회 : ShopFloorCustomWindow
    {
        public IPC검사결과조회()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,IPC검사결과조회"; }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
