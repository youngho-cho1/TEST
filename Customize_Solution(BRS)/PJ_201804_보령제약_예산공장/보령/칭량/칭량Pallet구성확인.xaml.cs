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
    public partial class 칭량Pallet구성확인 : ShopFloorCustomWindow
    {
        public 칭량Pallet구성확인()
        {
            InitializeComponent();
        }

        private void btnclosed_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
