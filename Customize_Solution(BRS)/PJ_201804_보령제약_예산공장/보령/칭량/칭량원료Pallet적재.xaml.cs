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
using LGCNS.EZMES.ControlsLib;
using ShopFloorUI;

namespace 보령
{
    public partial class 칭량원료Pallet적재 : ShopFloorCustomWindow
    {
        public 칭량원료Pallet적재()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,칭량원료Pallet적재"; }
        }
        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            if (Phase != null)
            {
                if (await Phase.SessionCheck() != enumInstructionRegistErrorType.Ok)
                    DialogResult = false;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
