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

namespace Board
{
    public partial class SVP_RejectInfo : UserControl
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;
        C1.Silverlight.DataGrid.DataGridColumn[] _headerDetailRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerDetailColumnRows;

        public SVP_RejectInfo()
        {
            InitializeComponent();
            _headerRowColumns = dgDetail.Columns.Take(30).ToArray();
            _headerColumnRows = dgDetail.TopRows.Take(6).ToArray();
            //_headerDetailRowColumns = dgDetail2.Columns.Take(18).ToArray();
            //_headerDetailColumnRows = dgDetail2.TopRows.Take(6).ToArray();
        }

        private void dgWorkTime_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            // view port columns & rows without headers
            var nonHeadersViewportCols = dgDetail.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = dgDetail.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

            // merge column & rows headers
            foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerColumnRows, nonHeadersViewportCols, true))
            {
                e.Merge(range);

            } 

            // merge header intersection as we want, in this case, horizontally
            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, _headerColumnRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }

        }

        //private void dgDetail_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        //{
        //    // view port columns & rows without headers
        //    var nonHeadersViewportCols = dgDetail2.Viewport.Columns.Where(c => !_headerDetailRowColumns.Contains(c)).ToArray();
        //    var nonHeadersViewportRows = dgDetail2.Viewport.Rows.Where(r => !_headerDetailColumnRows.Contains(r)).ToArray();

        //    // merge column & rows headers
        //    foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerDetailColumnRows, nonHeadersViewportCols, true))
        //    {
        //        e.Merge(range);

        //    }

        //    // merge header intersection as we want, in this case, horizontally
        //    foreach (var range in MergingHelper.Merge(Orientation.Horizontal, _headerDetailColumnRows, _headerDetailRowColumns, true))
        //    {
        //        e.Merge(range);
        //    }
        //}
    }
}
