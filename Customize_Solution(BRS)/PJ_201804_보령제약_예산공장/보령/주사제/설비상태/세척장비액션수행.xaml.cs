using C1.Silverlight.Chart;
using C1.Silverlight.Chart.Extended;
using LGCNS.iPharmMES.Common;
using LGCNS.iPharmMES.Recipe.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Input;

namespace 보령
{
    [Description("SVP 세척 장비 액션 수행")]
    public partial class 세척장비액션수행 : ShopFloorCustomWindow
    {
        세척장비액션수행ViewModel _viewModel;

        public 세척장비액션수행()
        {
            InitializeComponent();

            _viewModel = new 세척장비액션수행ViewModel();
            this.DataContext = _viewModel;
        }
        public override string TableTypeName
        {
            get { return "TABLE,세척장비액션수행"; }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void c1pkrFromTime_GotFocus(object sender, RoutedEventArgs e)
        {
            var popup = new 세척장비시간입력Popup();

            popup.resTime = c1pkrFromTime.DateTime.HasValue ? c1pkrFromTime.DateTime.Value : DateTime.Now;

            //팝업이 닫힐 때 이벤트
            popup.Closed += (s1, e1) =>
            {
                if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                {
                    c1pkrFromTime.DateTime = Convert.ToDateTime(c1pkrFromDate.DateTime.Value.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
                }
            };

            popup.Show(); //Closed 이벤트를 발생
        }
    }
}
