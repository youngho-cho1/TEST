using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace 보령
{
    [Description("각각의 칭량된 원료 투입 처리 (재고 차감)")]
    public partial class 소분원료전량투입 : ShopFloorCustomWindow
    {
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public 소분원료전량투입()
        {
            InitializeComponent();            

            _headerRowColumns = dgMaterials.Columns.Take(3).ToArray(); // header 3개의 열
            _headerColumnRows = dgMaterials.TopRows.Take(2).ToArray(); // header 2 줄
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
                    var holderItem = list[0].DataItem as BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATA;
                    var currentItem = row.DataItem as BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATA;

                    if (holderItem.MTRLID == currentItem.MTRLID)
                    {
                        list.Add(row);
                        continue;
                    }
                    else
                    {
                        //foreach (var range in MergingHelper.Merge(Orientation.Horizontal, list.ToArray(),
                        //    dgMaterials.Columns.Where(c => c.Index == 1 || c.Index == 2 || c.Index == 3).ToArray(), true))
                        //{
                        //    e.Merge(range);
                        //}

                        //list.Clear();
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

            var data = e.Cell.Row.DataItem as BR_BRS_SEL_ProductionOrder_Component_Summary.OUTDATA;
            
            if(data != null && Convert.ToSingle(data.REMAINQTY) <= 0)
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
