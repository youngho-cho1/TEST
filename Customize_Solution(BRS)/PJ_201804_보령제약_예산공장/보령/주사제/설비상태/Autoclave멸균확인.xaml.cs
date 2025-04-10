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
    public partial class Autoclave멸균확인 : ShopFloorCustomWindow
    {
        public Autoclave멸균확인()
        {
            InitializeComponent();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }

    }
}
