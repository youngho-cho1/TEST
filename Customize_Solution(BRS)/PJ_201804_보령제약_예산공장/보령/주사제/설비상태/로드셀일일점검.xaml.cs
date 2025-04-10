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
    [Description("로드셀 일일점검")]
    public partial class 로드셀일일점검 : ShopFloorCustomWindow
    {
        public 로드셀일일점검()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,로드셀일일점검"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void txtEqptId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && BusyIn.DataContext is 로드셀일일점검ViewModel)
                    (BusyIn.DataContext as 로드셀일일점검ViewModel).ConnectEqptCommand.Execute(txtEqptId.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
