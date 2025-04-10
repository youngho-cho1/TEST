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
using LGCNS.iPharmMES.Common;

namespace WMS
{
    public partial class StorageInOutWms : UserControl
    {
        public StorageInOutWms()
        {
            InitializeComponent();
            txtBarcodes.UpdateLayout();
            txtBarcodes.Focus();
        }
        public string Barcode = "";
        public bool isT = false;

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.TabInput.IsSelected)
            {
                txtBarcodes.Focus();
                if (e.Key != Key.Enter)
                    txtBarcodes_KeyDown(sender, e);
            }
            else if (this.TabOutput.IsSelected)
            {
                TabOutput_KeyDown(sender, e);
            }
        }

        private void txtBarcodes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtBarcodes.SelectAll();
                ((StorageInOutWmsViewModel)LayoutRoot.DataContext).ScanBtnCommand.Execute(txtBarcodes.Text);
            }
        }

        private void dgList_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {

        }

        private void dgList_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
        }

        private void btnTabInput_Click(object sender, RoutedEventArgs e)
        {
            TabInput.IsSelected = true;
            TabOutput.IsSelected = false;

            (LayoutRoot.DataContext as StorageInOutWmsViewModel).isInEble = false;
            (LayoutRoot.DataContext as StorageInOutWmsViewModel).isOutEble = true;
            TabInput.Focus();
        }

        private void btnTabOutput_Click(object sender, RoutedEventArgs e)
        {
            TabInput.IsSelected = false;
            TabOutput.IsSelected = true;

            (LayoutRoot.DataContext as StorageInOutWmsViewModel).isInEble = true;
            (LayoutRoot.DataContext as StorageInOutWmsViewModel).isOutEble = false;
            Barcode = "";
            TabOutput.Focus();
        }

        private void btnRemove_Click_1(object sender, RoutedEventArgs e)
        {
            var Ds = LayoutRoot.DataContext as StorageInOutWmsViewModel;
            Ds.RemoveOutCommandAsync.Execute((sender as Button).DataContext);
        }

        private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Plugin.Focus();
            TabInput.Focus();
        }

        private void TabOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var Ds = LayoutRoot.DataContext as StorageInOutWmsViewModel;
                Ds.ScanSaveAffterOut(Barcode);
                Barcode = "";
                TabOutput.Focus();
            }
            else
            {
                if (Barcode != null && Barcode.Length > 0)
                {
                    Barcode = Barcode + e.Key.ToString();
                }
                else
                {
                    Barcode = e.Key.ToString();
                }
            }
        }
    }
}
