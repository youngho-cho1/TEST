using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;

namespace 보령
{
    [Description("주사제 조재액에 사용하는 비커의 용기 무게를 저장")]
    public partial class 비커무게측정: ShopFloorCustomWindow
    { 

        public override string TableTypeName
        {
            get { return "TABLE,비커무게측정"; }
        }

        public 비커무게측정()
        {
            InitializeComponent();
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtScaleId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && BusyIn.DataContext is 비커무게측정ViewModel)
                    (BusyIn.DataContext as 비커무게측정ViewModel).ConnectScaleCommand.Execute(txtScaleId.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void txtBeakerId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && BusyIn.DataContext is 비커무게측정ViewModel)
                    (BusyIn.DataContext as 비커무게측정ViewModel).CheckBeakerCommand.Execute(txtBeakerId.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
