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
    [Description("과립반제품 수율을 기준으로 소분된 원료를 현장칭량하여 투입")]
    public partial class 소분원료분할투입 : ShopFloorCustomWindow
    {
        public 소분원료분할투입()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,소분원료분할투입"; }
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
