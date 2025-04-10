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
    public partial class WMSInOut : UserControl
    {
        private string _VesselID;

        public WMSInOut()
        {
            InitializeComponent();         
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var Ds = LayoutRoot.DataContext as WMSInoutViewModel;
            Ds.RemoveCommandAsync.Execute((sender as Button).DataContext);
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            //if (this.TabInput.IsSelected)
            //{
            //    if (e.Key == Key.Enter)
            //    {
            //        var Ds = LayoutRoot.DataContext as WMSInoutViewModel;
            //        Ds.ScanBtnCommand.Execute(_VesselID);
            //        _VesselID = "";
            //        Main.Focus();
            //    }
            //    else
            //    {
            //        if (_VesselID != null && _VesselID.Length > 0)
            //        {
            //            _VesselID = _VesselID + e.Key.ToString();
            //        }
            //        else
            //        {
            //            _VesselID = e.Key.ToString();
            //        }
            //    }
            //}
        }

        private void txtBarcodes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtBarcodes.SelectAll();
                ((WMSInoutViewModel)LayoutRoot.DataContext).ScanBtnCommand.Execute(txtBarcodes.Text);
            }
        }

        private void dgList_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            if ((e.Cell.Row.DataItem as InputData).LOTWEIGHT == 0)
            {
                e.Cell.Presenter.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            }            

        }

        private void dgList_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
        }

        private void btnTabInput_Click(object sender, RoutedEventArgs e)
        {
            TabInput.IsSelected = true;
            TabOutput.IsSelected = false;

            (LayoutRoot.DataContext as WMSInoutViewModel).isInEble = false;
            (LayoutRoot.DataContext as WMSInoutViewModel).isOutEble = true;
        }

        private void btnTabOutput_Click(object sender, RoutedEventArgs e)
        {
            TabInput.IsSelected = false;
            TabOutput.IsSelected = true;

            (LayoutRoot.DataContext as WMSInoutViewModel).isInEble = true;
            (LayoutRoot.DataContext as WMSInoutViewModel).isOutEble = false;
            
        }

        private void btnRemove_Click_1(object sender, RoutedEventArgs e)
        {
            var Ds = LayoutRoot.DataContext as WMSInoutViewModel;
            Ds.RemoveOutCommandAsync.Execute((sender as Button).DataContext);
        }

        private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Main.ActualHeight >= 800 && Main.ActualHeight < 900)
            {
                txtBatchno.FontSize = 60;
                txtIBCID.FontSize =60;
                txtPrevOpW.FontSize = 80;
                txtSubTotalWT.FontSize = 80;
                txtGoodQty.FontSize = 80;
            }
            else if (Main.ActualHeight >= 700 && Main.ActualHeight < 800)
            {
                txtBatchno.FontSize = 50;
                txtIBCID.FontSize = 50;
                txtPrevOpW.FontSize = 70;
                txtSubTotalWT.FontSize = 70;
                txtGoodQty.FontSize = 70;
            }
            else if (Main.ActualHeight >= 600 && Main.ActualHeight < 700)
            {
                txtBatchno.FontSize = 40;
                txtIBCID.FontSize = 40;
                txtPrevOpW.FontSize = 60;
                txtSubTotalWT.FontSize = 60;
                txtGoodQty.FontSize = 60;
            }
            else if (Main.ActualHeight >= 900 && Main.ActualHeight < 940)
            {
                txtBatchno.FontSize = 65;
                txtIBCID.FontSize = 65;
                txtPrevOpW.FontSize = 85;
                txtSubTotalWT.FontSize = 85;
                txtGoodQty.FontSize = 85;
            }
            else if (Main.ActualHeight >= 940)
            {
                txtBatchno.FontSize = 70;
                txtIBCID.FontSize = 70;
                txtPrevOpW.FontSize = 90;
                txtSubTotalWT.FontSize = 90;
                txtGoodQty.FontSize = 90;
            }
        }
    }
}
