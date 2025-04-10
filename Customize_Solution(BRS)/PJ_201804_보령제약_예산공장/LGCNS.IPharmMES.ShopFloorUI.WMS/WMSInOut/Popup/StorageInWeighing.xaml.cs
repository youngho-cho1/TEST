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
    public partial class StorageInWeighing : iPharmMESChildWindow
    {
        public StorageInWeighing()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtTotalWeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Equals("연결실패") || (sender as TextBox).Text.Equals(""))
            {
                (sender as TextBox).FontSize = 45;
                OKButton.IsEnabled = false;
            }
            else
            {
                (sender as TextBox).FontSize = 60;
                OKButton.IsEnabled = true;
            }
        }
    }
}
