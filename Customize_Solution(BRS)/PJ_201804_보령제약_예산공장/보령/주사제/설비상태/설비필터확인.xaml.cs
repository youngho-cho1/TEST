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
using C1.Silverlight.DataGrid;
using System.ComponentModel;

namespace 보령
{
    [Description("설비 필터정보 조회")]
    public partial class 설비필터확인 : ShopFloorCustomWindow
    {
        public 설비필터확인()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,설비필터확인"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void txtEqptId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && LayoutRoot.DataContext is 설비필터확인ViewModel)
                    (LayoutRoot.DataContext as 설비필터확인ViewModel).SelfilterCommand.Execute(txtFilterId.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
