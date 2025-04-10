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
    [Description("출고한 포장자재 무게를 길이로 환산한다.")]
    public partial class 출고포장자재환산 : ShopFloorCustomWindow
    {
        public 출고포장자재환산()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,출고포장자재환산"; }
        }

        // 좌측 화면 이벤트
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).LoadRollInfoCommand.Execute(this.ListPackingMTRL.SelectedItem);
        }

        // 우측 화면 이벤트
        private void btnCacel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Param_KeyDown(object sender, KeyEventArgs e)
        {
            decimal temp = 0m;

            if (e.Key == Key.Enter)
            {
                if (decimal.TryParse(this.Param.Text, out temp))
                {
                    (this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).Param = temp;
                    (this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).ConvertWeight();
                }
                else
                {
                    C1.Silverlight.C1MessageBox.Show("숫자를 입력하세요");
                    this.Param.Focus();
                }
            }
        }

        private void txtMSUBLOTBCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).RollWeightList.Where(x => x.MSUBLOTBCD.Equals(this.txtMSUBLOTBCD.Text)).FirstOrDefault() != null)
                    (this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).InsertRollWeightCommand.Execute(null);
                else
                {
                    C1.Silverlight.C1MessageBox.Show("해당되는 자료가 없습니다");
                    this.txtMSUBLOTBCD.Focus();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(this.gridPackingWeight.SelectedItem != null)
                (this.LayoutRoot.DataContext as 출고포장자재환산ViewModel).UpdateRollWeightCommand.Execute(null);
        }
    }
}
