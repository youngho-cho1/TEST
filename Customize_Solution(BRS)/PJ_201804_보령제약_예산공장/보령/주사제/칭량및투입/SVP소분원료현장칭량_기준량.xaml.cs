using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace 보령
{
    [ShopFloorCustomHidden]
    [Description("주사제 제조 현장에서 현장칭량 소분원료를 칭량 및 투입을 한다.")]
    public partial class SVP소분원료현장칭량_기준량 : ShopFloorCustomWindow
    {
        public SVP소분원료현장칭량_기준량()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,SVP소분원료현장칭량_기준량"; }
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