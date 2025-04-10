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
using LGCNS.iPharmMES.Common;

namespace WMS
{
    public partial class OutStnInfo : iPharmMESChildWindow
    {
        public OutStnInfo()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            if (Parent as StorageInOutWms != null)
            {
                (Parent as StorageInOutWms).TabOutput.Focus();
            }
            else if (Parent as WMSInOut != null)
            {
                (Parent as WMSInOut).TabOutput.Focus();
            }

            this.DialogResult = false;
        }

        private void BinVessel_Click(object sender, RoutedEventArgs e)
        {
            if ((Main.DataContext as WMSInoutViewModel) != null)
            {
                (Main.DataContext as WMSInoutViewModel)._OutType = "EMPTY";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = false;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = true;
            }
            else if (Main.DataContext as StorageInOutWmsViewModel != null)
            {
                (Main.DataContext as StorageInOutWmsViewModel)._OutType = "EMPTY";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = false;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = true;
            }
        }

        private void SubMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (Main.DataContext as WMSInoutViewModel != null)
            {
                (Main.DataContext as WMSInoutViewModel)._OutType = "FILLED";
                SubMaterial.IsEnabled = false;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = true;
            }
            else if (Main.DataContext as StorageInOutWmsViewModel != null)
            {
                (Main.DataContext as StorageInOutWmsViewModel)._OutType = "FILLED";
                SubMaterial.IsEnabled = false;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = true;
            }
        }

        private void txtOrderNoOutp_Click(object sender, RoutedEventArgs e)
        {
            if (Main.DataContext as WMSInoutViewModel != null)
            {
                (Main.DataContext as WMSInoutViewModel).OderNoBtnCommand.Execute(null);
            }
            else if (Main.DataContext as StorageInOutWmsViewModel != null)
            {
                (Main.DataContext as StorageInOutWmsViewModel).OderNoBtnCommand.Execute(null);
            }
        }

        private void TEST_Click(object sender, RoutedEventArgs e)
        {
            if (Main.DataContext as WMSInoutViewModel != null)
            {
                (Main.DataContext as WMSInoutViewModel)._OutType = "TEST";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = false;
                CLEAN.IsEnabled = true;
            }
            else if (Main.DataContext as StorageInOutWmsViewModel != null)
            {
                (Main.DataContext as StorageInOutWmsViewModel)._OutType = "TEST";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = false;
                CLEAN.IsEnabled = true;
            }

        }

        private void CLEAN_Click(object sender, RoutedEventArgs e)
        {
            if (Main.DataContext as WMSInoutViewModel != null)
            {
                (Main.DataContext as WMSInoutViewModel)._OutType = "CLEAN";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = false;
            }
            else if (Main.DataContext as StorageInOutWmsViewModel != null)
            {
                (Main.DataContext as StorageInOutWmsViewModel)._OutType = "CLEAN";
                SubMaterial.IsEnabled = true;
                BinVessel.IsEnabled = true;
                TEST.IsEnabled = true;
                CLEAN.IsEnabled = false;
            }

        }
    }
}

