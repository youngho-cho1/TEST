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
    public partial class 빈용기출고 : ShopFloorCustomWindow
    {
        public 빈용기출고()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,빈용기출고"; }
        }
        private async void Main_Loaded(object sender, RoutedEventArgs e)
        {
            if (Phase != null)
            {
                if (await Phase.SessionCheck() != enumInstructionRegistErrorType.Ok)
                    DialogResult = false;
            }

        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void GridContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is C1.Silverlight.DataGrid.C1DataGrid)
            {
                if((sender as C1.Silverlight.DataGrid.C1DataGrid).CurrentRow != null && (sender as C1.Silverlight.DataGrid.C1DataGrid).CurrentRow.DataItem != null)
                {
                    var curRow = (sender as C1.Silverlight.DataGrid.C1DataGrid).CurrentRow.DataItem as 빈용기출고ViewModel.EmptyWIPContainer;
                    curRow.CHK = curRow.STATUS.Equals("출고완료") ? false : !curRow.CHK;
                }
            }           
        }
    }
}
