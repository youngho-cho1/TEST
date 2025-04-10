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

namespace 보령
{
    public partial class 설비가동시간체크 : ShopFloorCustomWindow
    {
        public 설비가동시간체크()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,설비가동시간체크"; } 
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            (BusyIn.DataContext as 설비가동시간체크ViewModel).TimerStop();
            this.DialogResult = true;
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            (BusyIn.DataContext as 설비가동시간체크ViewModel).TimerStop();
        }
    }
}
