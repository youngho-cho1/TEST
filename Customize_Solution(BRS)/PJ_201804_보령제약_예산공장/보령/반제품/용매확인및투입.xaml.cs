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
    [Description("현장에서 용수에 대해 칭량 및 투입을 한다.")]
    public partial class 용매확인및투입: ShopFloorCustomWindow
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public override string TableTypeName
        {
            get { return "TABLE,용매확인및투입"; }
        }

        public 용매확인및투입()
        {
            InitializeComponent();

            //System.Text.StringBuilder empty = new System.Text.StringBuilder();
            //LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);

            _headerRowColumns = dgProductionOutput.Columns.Take(1).ToArray();
            _headerColumnRows = dgProductionOutput.TopRows.Take(1).ToArray();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dgProductionOutput_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {

            // view port columns & rows without headers
            var nonHeadersViewportCols = dgProductionOutput.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = dgProductionOutput.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

            foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerColumnRows, nonHeadersViewportCols, true))
            {
                e.Merge(range);
            }
            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, nonHeadersViewportRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }
            //foreach (var range in MergingHelper.Merge(Orientation.Vertical, nonHeadersViewportRows, dgProductionOutput.Viewport.Columns.Where(c => c.Index == 0).ToArray(), false))
            //{
            //    e.Merge(range);
            //}
            //foreach (var range in MergingHelper.Merge(Orientation.Vertical, nonHeadersViewportRows, dgProductionOutput.Viewport.Columns.Where(c => c.Index == 1).ToArray(), false))
            //{
            //    e.Merge(range);
            //}

        }

        private void txtScaleValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 용매확인및투입ViewModel).ScaleTextValidation(sender as TextBox);
        }
        private void btnCharging_Click(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 용매확인및투입ViewModel).ChargingMTRL();
        }
        private void btnDivisionCharging_Click(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 용매확인및투입ViewModel).DivisionChargingMTRL();
        }
    }
}
