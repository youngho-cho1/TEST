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
using LGCNS.iPharmMES.Common;

namespace 보령
{
    public partial class 반제품출고 : ShopFloorCustomWindow
    {
        public 반제품출고()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,반제품출고"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void GridContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is C1.Silverlight.DataGrid.C1DataGrid && (sender as C1.Silverlight.DataGrid.C1DataGrid).CurrentRow.DataItem is 반제품출고ViewModel.IBCInfo)
            {
                var curRow = (sender as C1.Silverlight.DataGrid.C1DataGrid).CurrentRow.DataItem as 반제품출고ViewModel.IBCInfo;
                curRow.CHK = curRow.STATUS.Equals("출고완료") ? false : !curRow.CHK;
            }
        }
    }
}
