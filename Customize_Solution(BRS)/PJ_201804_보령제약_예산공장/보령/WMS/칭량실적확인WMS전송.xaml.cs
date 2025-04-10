using ShopFloorUI;
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

namespace 보령
{
    public partial class 칭량실적확인WMS전송 : ShopFloorCustomWindow
    {
        public 칭량실적확인WMS전송()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,칭량실적확인WMS전송"; }
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
