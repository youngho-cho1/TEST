using C1.Silverlight.Imaging;
using LGCNS.EZMES.ControlsLib;
using LGCNS.iPharmMES.Common;
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

namespace 보령
{
    public partial class 장비이력데이터저장Popup : iPharmMESChildWindow
    {
        public 장비이력데이터저장Popup(C1Bitmap src)
        {
            InitializeComponent();

            this.Image.Source = src.ImageSource;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
