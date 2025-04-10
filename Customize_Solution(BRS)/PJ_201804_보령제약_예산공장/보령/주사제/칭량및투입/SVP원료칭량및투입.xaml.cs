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
    [Description("주사제 제조 현장에서 현장칭량 원료를 칭량 및 투입을 한다.")]
    public partial class SVP원료칭량및투입 : ShopFloorCustomWindow
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public override string TableTypeName
        {
            get { return "TABLE,SVP원료칭량및투입"; }
        }

        public SVP원료칭량및투입()
        {
            InitializeComponent();

            _headerRowColumns = dgProductionOutput.Columns.Take(1).ToArray();
            _headerColumnRows = dgProductionOutput.TopRows.Take(1).ToArray();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dgProductionOutput_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
