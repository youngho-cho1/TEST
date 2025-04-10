using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LGCNS.iPharmMES.Common;
using ShopFloorUI;

using LGCNS.EZMES.ControlsLib;


using C1.Silverlight;

namespace 보령
{
    [ShopFloorCustomHidden]
    public partial class 원료칭량 : ShopFloorCustomWindow
    {
        public string POID { get; set; }
        public string OPSGGUID { get; set; }
        public string BATCHNO { get; set; }
        public string MTRLID { get; set; }
        public string MTRLNAME { get; set; }
        public string COMPONENTGUID { get; set; }
        public string WEIGHINGMETHOD { get; set; }

        public 원료칭량()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get { return "TABLE,원료칭량"; }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as TextBlock).DataContext as 원료칭량ViewModel).SourceBarcodePopupCommand.Execute(null);
        }

        private void Vessel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as TextBlock).DataContext as 원료칭량ViewModel).BinBarcodePopupCommand.Execute(null);
        }

        private void btnComponentSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            btnComponentSelect.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 051, 102));
        }

        private void btnComponentSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            btnComponentSelect.Foreground = new SolidColorBrush(Colors.White);
        }

        private void SourceBarcode_MouseEnter(object sender, MouseEventArgs e)
        {
            SourceBarcode.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 051, 102));
        }

        private void SourceBarcode_MouseLeave(object sender, MouseEventArgs e)
        {
            SourceBarcode.Foreground = new SolidColorBrush(Colors.White);
        }

        private void tbTareWeight_MouseEnter(object sender, MouseEventArgs e)
        {
            tbTareWeight.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 051, 102));
        }

        private void tbTareWeight_MouseLeave(object sender, MouseEventArgs e)
        {
            tbTareWeight.Foreground = new SolidColorBrush(Colors.White);
        }

        private void tbVessel_MouseEnter(object sender, MouseEventArgs e)
        {
            tbVessel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 051, 102));
        }

        private void tbVessel_MouseLeave(object sender, MouseEventArgs e)
        {
            tbVessel.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.DataContext = new 원료칭량ViewModel();
        }
    }
}
