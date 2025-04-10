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
using C1.Silverlight;

namespace 보령
{
    [Description("중량선별기 샘플 기록")]
    public partial class 중량선별기범위 : ShopFloorCustomWindow
    {
        public 중량선별기범위()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,중량선별기범위"; }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void txtWeight_KeyDown(object sender, KeyEventArgs e)
        {
            string weight = this.txtWeight.Text;
            decimal param = 0m;

            if (e.Key == Key.Enter)
            {
                if (decimal.TryParse(weight, out param))
                {
                    (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).AddCartonWeight(param);

                    this.txtWeight.Text = "";
                    this.txtWeight.Focus();
                }
                else
                    C1MessageBox.Show("숫자를 입력하시오");

            }
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            string weight = this.txtWeight.Text;
            decimal param = 0m;

            if (decimal.TryParse(weight, out param))
            {
                (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).AddCartonWeight(param);

                this.txtWeight.Text = "";
                this.txtWeight.Focus();
            }
            else
                C1MessageBox.Show("숫자를 입력하시오");

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).DelCartonWeight();
        }

        private void CheckAll_Checked(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).AllCheck(true);
        }
        private void CheckAll_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).AllCheck(false);
        }
        //private void WeightGrid_SelectionChanged(object sender, C1.Silverlight.DataGrid.DataGridSelectionChangedEventArgs e)
        //{
        //    (this.LayoutRoot.DataContext as 중량선별기범위ViewModel).RowCheck(this.WeightGrid.SelectedItem);
        //}

        private void WeightGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (WeightGrid.SelectedItem != null)
            {
                (WeightGrid.SelectedItem as 중량선별기범위ViewModel.CartonWeight).CHK = !((WeightGrid.SelectedItem as 중량선별기범위ViewModel.CartonWeight).CHK);
                WeightGrid.SelectedItem = null;
            }
        }
    }
}
