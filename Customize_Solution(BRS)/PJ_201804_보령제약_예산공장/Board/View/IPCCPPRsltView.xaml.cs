using C1.Silverlight;
using LGCNS.iPharmMES.Common;
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
using System.Collections.ObjectModel;


namespace Board
{
    public partial class IPCCPPRsltView : UserControl
    {

        #region [※ 지역변수선언부]        
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows;
        C1.Silverlight.DataGrid.DataGridColumn[] _headerRowColumns1;
        C1.Silverlight.DataGrid.DataGridRow[] _headerColumnRows1;
        #endregion

        public IPCCPPRsltView()
        {
            InitializeComponent();
            _headerRowColumns = grd_CriticalParameter.Columns.Take(2).ToArray();
            _headerColumnRows = grd_CriticalParameter.TopRows.Take(2).ToArray();
            _headerRowColumns1 = grdOperationCheck.Columns.Take(2).ToArray();
            _headerColumnRows1 = grdOperationCheck.TopRows.Take(2).ToArray();
            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, null);
        }
        

        #region [※ Declare local vairable]
        private bool isClickcheckValue7 = true;
        private bool isGridLoaded = true;
        #endregion

        private void c1DropDown1_Loaded(object sender, RoutedEventArgs e)
        {
            var dropDown = sender as C1DropDown;
            System.Text.StringBuilder empty = new System.Text.StringBuilder();

        }

        private void c1DropDown2_Loaded(object sender, RoutedEventArgs e)
        {
            var dropDown = sender as C1DropDown;
            System.Text.StringBuilder empty = new System.Text.StringBuilder();
        }

        private void c1DropDown3_Loaded(object sender, RoutedEventArgs e)
        {
            var dropDown = sender as C1DropDown;
            System.Text.StringBuilder empty = new System.Text.StringBuilder();
        }

        private void gg_Loaded_1(object sender, RoutedEventArgs e)
        {
            isGridLoaded = true;
        }


        private void checkValue7_Checked(object sender, RoutedEventArgs e)
        {
            if (this.checkValue1 != null && checkValue2 != null && checkValue3 != null //&& checkValue5 != null
                && checkValue8 != null && checkValue9 != null && checkValue10 != null /* && checkValue11 != null*/ && checkValue12 != null)
            {
                //직접 checkbox 클릭한 경우에만 로직 수행
                if (isClickcheckValue7)
                {
                    this.checkValue1.IsChecked = true;
                    this.checkValue2.IsChecked = true;
                    this.checkValue3.IsChecked = true;
                    //this.checkValue4.IsChecked = true;
                    //this.checkValue5.IsChecked = true;


                    this.checkValue8.IsChecked = true;
                    this.checkValue9.IsChecked = true;
                    this.checkValue10.IsChecked = true;
                    //this.checkValue11.IsChecked = true;
                    this.checkValue12.IsChecked = true;
                    //this.selection.Text = "« ALL »";
                }

            }
            isClickcheckValue7 = true;

        }
        private void checkValue7_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.checkValue1 != null && checkValue2 != null && checkValue3 != null //&& checkValue5 != null
                && checkValue8 != null && checkValue9 != null && checkValue10 != null /*&& checkValue11 != null*/ && checkValue12 != null)
            {
                //직접 checkbox 클릭한 경우에만 로직 수행
                if (isClickcheckValue7)
                {
                    this.checkValue1.IsChecked = false;
                    this.checkValue2.IsChecked = false;
                    this.checkValue3.IsChecked = false;
                    //this.checkValue4.IsChecked = false;
                    //this.checkValue5.IsChecked = false;

                    this.checkValue8.IsChecked = false;
                    this.checkValue9.IsChecked = false;
                    this.checkValue10.IsChecked = false;
                    //this.checkValue11.IsChecked = false;
                    this.checkValue12.IsChecked = false;
                    //this.selection.Text = "";
                }
            }
            isClickcheckValue7 = true;

            //isClickcheckValue7 = false;
        }
        private void checkValue_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.checkValue7 != null && this.checkValue7.IsChecked == true)
            {
                isClickcheckValue7 = false;
                this.checkValue7.IsChecked = false;

            }

        }

        private void checkValue_checked(object sender, RoutedEventArgs e)
        {
            if (this.checkValue1 != null && this.checkValue2 != null && this.checkValue3 != null) // && this.checkValue5 != null)
            {
                if (this.checkValue1.IsChecked == true && this.checkValue2.IsChecked == true &&
                    this.checkValue3.IsChecked == true &&
                    /*this.checkValue5.IsChecked == true &&*/ this.checkValue8.IsChecked == true &&
                    this.checkValue9.IsChecked == true && this.checkValue10.IsChecked == true &&
                    /*this.checkValue11.IsChecked == true &&*/ this.checkValue12.IsChecked == true)
                {
                    if (!isGridLoaded)
                    {
                        isClickcheckValue7 = false;
                        this.checkValue7.IsChecked = true;
                    }
                    else
                    {
                        isGridLoaded = false;
                    }
                }
            }
        }


        private void checkAll_GridChecked(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as IPCCPPRsltViewViewModel).AllGridCheck(true);
        }

        private void checkAll_GridUnchecked(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as IPCCPPRsltViewViewModel).AllGridCheck(false);
        }

        private void grd_CriticalParameter_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            e.Cell.Presenter.Background = new SolidColorBrush(Colors.White);            
        }

        private void grd_CriticalParameter_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            // view port columns & rows without headers
            var nonHeadersViewportCols = grd_CriticalParameter.Viewport.Columns.Where(c => !_headerRowColumns.Contains(c)).ToArray();
            var nonHeadersViewportRows = grd_CriticalParameter.Viewport.Rows.Where(r => !_headerColumnRows.Contains(r)).ToArray();
                        
            // merge content
            foreach (var range in LGCNS.EZMES.ControlsLib.MergingHelper.Merge(Orientation.Vertical, nonHeadersViewportRows, grd_CriticalParameter.Viewport.Columns.Where(c => c.Index <= 3).ToArray(), false))
                e.Merge(range);

        }

        private void grdOperationCheck_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            e.Cell.Presenter.Background = new SolidColorBrush(Colors.White);
        }

        private void grdOperationCheck_MergingCells(object sender, C1.Silverlight.DataGrid.DataGridMergingCellsEventArgs e)
        {
            // view port columns & rows without headers
            var nonHeadersViewportCols = grdOperationCheck.Viewport.Columns.Where(c => !_headerRowColumns1.Contains(c)).ToArray();
            var nonHeadersViewportRows = grdOperationCheck.Viewport.Rows.Where(r => !_headerColumnRows1.Contains(r)).ToArray();
            
            // merge content
            foreach (var range in LGCNS.EZMES.ControlsLib.MergingHelper.Merge(Orientation.Vertical, nonHeadersViewportRows, grdOperationCheck.Viewport.Columns.Where(c => c.Index <= 3).ToArray(), false))
                e.Merge(range);

        }


        



    }
}


