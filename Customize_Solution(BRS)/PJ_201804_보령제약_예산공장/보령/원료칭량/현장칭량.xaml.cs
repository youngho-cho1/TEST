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
using System.ComponentModel;
using ShopFloorUI;

namespace 보령
{
    [Description("원료 현장칭량(성적번호별로 소분 후 투입)")]
    public partial class 현장칭량 : ShopFloorCustomWindow
    {
        public 현장칭량()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,현장칭량"; }
        }
        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            if (Phase != null)
            {
                if (await Phase.SessionCheck() != enumInstructionRegistErrorType.Ok)
                    DialogResult = false;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
