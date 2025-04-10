using LGCNS.EZMES.ControlsLib;
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
    public partial class TrustScaleWeightPopup : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {

        public string WEIGHT { get; set; }
        public string BWEIGHT { get; set; }

        public TrustScaleWeightPopup()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dgSourceList_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
            e.Row.Height = new C1.Silverlight.DataGrid.DataGridLength(50);
            e.Row.Presenter.FontSize = 18;
        }

        private void dgSourceList_LoadedColumnHeaderPresenter(object sender, C1.Silverlight.DataGrid.DataGridColumnEventArgs e)
        {
            e.Column.HeaderPresenter.FontSize = 15;
        }

        private void txtBarcode_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBarcode.SelectAll();
        }

    }
}

