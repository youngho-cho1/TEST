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
using ShopFloorUI;
using C1.Silverlight;
using LGCNS.EZMES.ControlsLib;
using C1.Silverlight.DataGrid;

namespace 보령
{
    public partial class 포장자재출고기록 : ShopFloorCustomWindow
    {
        //C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        //C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        //private List<DataGridCellsRange> target = new List<DataGridCellsRange>();

        public 포장자재출고기록()
        {
            InitializeComponent();

            //_headerRowColumns = gridPickingList.Columns.Take(2).ToArray();
            //_headerColumnRows = gridPickingList.TopRows.Take(1).ToArray();
        }
        public override string TableTypeName
        {
            get {   return "TABLE,포장자재출고기록";    }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        //private void gridPickingList_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        //{
        //    var nonHeadersViewportCols = gridPickingList.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
        //    var nonHeadersViewportRows = gridPickingList.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

        //    foreach (var range in MergingHelper.Merge(Orientation.Horizontal, nonHeadersViewportRows, _headerRowColumns, true))
        //    {
        //        e.Merge(range);
        //    }
        //    //target.Add(new DataGridCellsRange
        //    //{
        //    //    Dta
        //    //}
        //}
    }
}
