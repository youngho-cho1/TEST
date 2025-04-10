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
    public partial class 반제품검량 : ShopFloorCustomWindow
    {
        public 반제품검량()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,반제품검량"; }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void txtSelContainer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var temp = (this.LayoutRoot.DataContext as 반제품검량ViewModel).ChangeTargetIBC(txtSelContainer.Text.ToUpper());
                if (temp != null)
                    GridRequestOutList.SelectedItem = temp;
                else
                    C1.Silverlight.C1MessageBox.Show(MessageTable.M[CommonMessageCode._10009].Replace("데이터", "용기번호"));
            }
        }
        private void GridRequestOutList_SelectionChanged(object sender, C1.Silverlight.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if ((sender as C1.Silverlight.DataGrid.C1DataGrid).SelectedItem != null)
                (this.LayoutRoot.DataContext as 반제품검량ViewModel).SelectedChangedCommandAsync.Execute(null);
        }
        private void btnSelectIBC_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtSelContainer.Text != null)
            {
                var temp = (this.LayoutRoot.DataContext as 반제품검량ViewModel).ChangeTargetIBC(txtSelContainer.Text.ToUpper());
                if (temp != null)
                    GridRequestOutList.SelectedItem = temp;
                else
                    C1.Silverlight.C1MessageBox.Show(MessageTable.M[CommonMessageCode._10009].Replace("데이터", "용기번호"));
            }
        }
    }
}
