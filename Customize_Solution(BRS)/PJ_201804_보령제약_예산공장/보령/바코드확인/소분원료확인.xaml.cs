using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.ComponentModel;

namespace 보령
{
    [Description("각각의 칭량된 원료 확인 (PMS를 통해 출고된 각각의 원료가 제대로 출고되었는지 확인)")]
    public partial class 소분원료확인 : ShopFloorCustomWindow
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public 소분원료확인()
        {
            InitializeComponent();

            this.DataContext = new 소분원료확인ViewModel();

            _headerRowColumns = dgMaterials.Columns.Take(3).ToArray(); // header 3개의 열
            _headerColumnRows = dgMaterials.TopRows.Take(2).ToArray(); // header 2 줄
        }

        public override string TableTypeName
        {
            get { return "TABLE,소분원료확인"; }
        }

        private void dgMaterials_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            // view port columns & rows without headers
            var nonHeadersViewportCols = dgMaterials.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = dgMaterials.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

            // merge column & rows headers
            foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerColumnRows, nonHeadersViewportCols, true))
            {
                e.Merge(range);
            }
            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, nonHeadersViewportRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }
            // merge header intersection as we want, in this case, horizontally
            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, _headerColumnRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }

            var list = new List<C1.Silverlight.DataGrid.DataGridRow>();
            foreach (var row in nonHeadersViewportRows)
            {
                if (list.Count <= 0)
                {
                    list.Add(row);
                    continue;
                }
                else
                {
                    var holderItem = list[0].DataItem as BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATA;
                    var currentItem = row.DataItem as BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATA;

                    if (holderItem.MTRLID == currentItem.MTRLID)
                    {
                        list.Add(row);
                        continue;
                    }
                    else
                    {

                    }
                }
            }
        }

        private void LayoutRoot_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            dgMaterials.Refresh();
        }

        private void dgMaterials_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            if (e.Cell.Presenter.Content.GetType() == typeof(C1.Silverlight.DataGrid.DataGridRowHeaderPresenter))
            {
                System.Windows.Controls.ContentControl cc = (e.Cell.Presenter.Content as System.Windows.Controls.ContentControl);
                cc.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                cc.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                cc.Background = new SolidColorBrush(Colors.White);
            }

            var data = e.Cell.Row.DataItem as BR_PHR_SEL_ProductionOrder_Component_Summary_CHGSEQ.OUTDATA;
            
            if(data != null && Convert.ToSingle(data.REMAIN) <= 0)
            {
                e.Cell.Presenter.Background = new SolidColorBrush(Colors.DarkGray);
            }

            e.Cell.Presenter.Background = new SolidColorBrush(Colors.White);
        }

        private void dgMaterials_UnloadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            e.Cell.Presenter.Background = null;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
