using LGCNS.EZMES.ControlsLib;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [ShopFloorCustomHidden]
    [Description("칭량 및 분쇄공정의 공정 중 제품 투입 (BIN의 순중량(Net wt.) 인터페이스)")]
    public partial class 저울반제품투입 : ShopFloorCustomWindow
    {

        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;

        public override string TableTypeName
        {
            get { return "TABLE,저울반제품투입"; }
        }

        public 저울반제품투입()
        {
            InitializeComponent();

            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);
            _headerRowColumns = dgProductionOutput.Columns.Take(8).ToArray();
            _headerColumnRows = dgProductionOutput.TopRows.Take(2).ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void dgProductionOutput_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {


            // view port columns & rows without headers
            var nonHeadersViewportCols = dgProductionOutput.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = dgProductionOutput.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();

            // merge column & rows headers
            foreach (var range in MergingHelper.Merge(Orientation.Vertical, _headerColumnRows, nonHeadersViewportCols, true))
            {
                e.Merge(range);

            }

            foreach (var range in MergingHelper.Merge(Orientation.Horizontal, _headerColumnRows, _headerRowColumns, true))
            {
                e.Merge(range);
            }
        }


        private void dgProductionOutput_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
            if (((LGCNS.iPharmMES.Common.BR_BRS_SEL_Scale_ProductionOrderOutput.OUTDATA)(e.Row.DataItem)).MTRLID == "총무게(내용물)")
            {
                e.Row.DataGrid.Rows[e.Row.Index].Presenter.Background = new SolidColorBrush(Colors.LightGray);
                e.Row.DataGrid.Rows[e.Row.Index].Presenter.FontWeight = FontWeights.Bold;
                e.Row.DataGrid.Rows[e.Row.Index].Presenter.FontSize = 13;
            }

        }

        private void txtScaleValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((System.Windows.Controls.TextBox)(sender)).Text.ToString() != null)
                (((System.Windows.Controls.TextBox)(sender))).Background = new SolidColorBrush(Colors.Yellow);
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            (this.LayoutRoot.DataContext as 저울반제품투입ViewModel).IsBusyForWeight = false;
        }

    }
}
