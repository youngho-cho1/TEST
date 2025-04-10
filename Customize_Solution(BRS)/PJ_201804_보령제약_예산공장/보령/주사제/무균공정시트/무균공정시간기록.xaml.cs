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
using System.Windows.Data;

namespace 보령
{
    [Description("무균공정시간기록")]
    public partial class 무균공정시간기록 : ShopFloorCustomWindow
    {
        public 무균공정시간기록()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,무균공정시간기록"; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public class Double2DateTimeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is double && !((double)value).Equals(double.NaN)) return DateTime.FromOADate((double)value);

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }
        }
        
        private void c1pkrFromTime_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var popup = new 무균공정시간Popup();

                popup.resTime = c1pkrFromTime.DateTime.HasValue ? c1pkrFromTime.DateTime.Value : DateTime.Now;
                popup.Closed += (s1, e1) =>
                {
                    if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                        //c1pkrFromTime.DateTime = Convert.ToDateTime((this.DataContext as 무균공정시작시간기록ViewModel).FromDt.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
                    c1pkrFromTime.DateTime = Convert.ToDateTime(c1pkrFromDate.DateTime.Value.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
                };

                popup.Show();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
