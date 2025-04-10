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
    [Description("칭량부스의 청소, 가동 등 액션 수행")]
    public partial class 부스점검 : ShopFloorCustomWindow
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public override string TableTypeName
        {
            get { return "TABLE,부스점검"; }
        }

        public 부스점검()
        { 
            InitializeComponent();
            _headerColumnRows = c1DataGrid.TopRows.Take(2).ToArray();
            _headerRowColumns = c1DataGrid.Columns.Take(10).ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void c1DataGrid_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            var nonHeadersViewportCols = c1DataGrid.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = c1DataGrid.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

            foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerColumnRows, nonHeadersViewportCols, true))
            {
                e.Merge(range);
            }

            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, _headerColumnRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }
        }
    }
}
