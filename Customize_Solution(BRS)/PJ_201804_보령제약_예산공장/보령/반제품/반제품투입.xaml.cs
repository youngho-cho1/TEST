using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using LGCNS.EZMES.Common;
using C1.Silverlight.Data;
using System.Text;
using C1.Silverlight;
using LGCNS.EZMES.ControlsLib;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace 보령
{
    [Description("이전 공정에서 생성된 반제품 투입 및 재고 차감한다.")]
    public partial class 반제품투입 : ShopFloorCustomWindow
    {

        public override string TableTypeName
        {
            get { return "TABLE,반제품투입"; }
        }

        public 반제품투입()
        {
            InitializeComponent();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void dgProductionOutput_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
            e.Row.DataGrid.Rows[e.Row.Index].Presenter.FontSize = 13;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((((System.Windows.Controls.TextBox)(sender))).Text == "투입대기")
            {
                (((System.Windows.Controls.TextBox)(sender))).Background = new SolidColorBrush(Colors.Green);
                (((System.Windows.Controls.TextBox)(sender))).TextAlignment = TextAlignment.Center;
                (((System.Windows.Controls.TextBox)(sender))).FontSize = 13;
                (((System.Windows.Controls.TextBox)(sender))).FontWeight = FontWeights.Bold;

            }
        }

        private void chkSel_Checked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as 반제품투입ViewModel).CheckSelect((sender as CheckBox).DataContext, true);
        }

        private void chkSel_Unchecked(object sender, RoutedEventArgs e)
        {
            (LayoutRoot.DataContext as 반제품투입ViewModel).CheckSelect((sender as CheckBox).DataContext, false);
        }
    }
}
