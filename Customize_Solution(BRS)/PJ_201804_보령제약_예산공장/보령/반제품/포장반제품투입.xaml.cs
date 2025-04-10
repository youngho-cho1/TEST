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
    public partial class 포장반제품투입 : ShopFloorCustomWindow
    {

        #region [※ 지역변수선언부]
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;
        #endregion

        public override string TableTypeName
        {
            get { return "TABLE,포장반제품투입"; }
        }

        public 포장반제품투입()
        {
            InitializeComponent();

            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
    

    }
}
