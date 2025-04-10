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
using System.ComponentModel;

namespace 보령
{
    [Description("주사제 자재환입 처리")]
    public partial class SVP자재환입처리 : ShopFloorCustomWindow
    {
        public SVP자재환입처리()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,SVP자재환입처리"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void MTRLGrid_SelectionChanged(object sender, C1.Silverlight.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            try
            {
                if (MTRLGrid.SelectedItem != null && MTRLGrid.SelectedItem is SVP자재환입처리ViewModel.PackingMTRL)
                    (this.BusyIndicator.DataContext as SVP자재환입처리ViewModel).RefreshList(this.MTRLGrid.SelectedItem as SVP자재환입처리ViewModel.PackingMTRL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void txtWeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                (this.BusyIndicator.DataContext as SVP자재환입처리ViewModel).ConvertValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(MTRLGrid.SelectedItem != null && MTRLGrid.SelectedItem is SVP자재환입처리ViewModel.PackingMTRL)                
                    (this.BusyIndicator.DataContext as SVP자재환입처리ViewModel).CompareRETRUNQTYwithINVQTY(this.MTRLGrid.SelectedItem as SVP자재환입처리ViewModel.PackingMTRL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void txtParam_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                decimal temp = 0m;

                if (e.Key == Key.Enter)
                {
                    if (decimal.TryParse(this.txtParam.Text, out temp))
                    {
                        (this.LayoutRoot.DataContext as SVP자재환입처리ViewModel).PARAM = temp.ToString("0.00#");
                        //(this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).ConvertWeight();

                        (this.BusyIndicator.DataContext as SVP자재환입처리ViewModel).ConvertValue();
                    }
                    else
                    {
                        C1.Silverlight.C1MessageBox.Show("숫자를 입력하세요");
                        this.txtParam.Text = "0";
                        this.txtParam.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
