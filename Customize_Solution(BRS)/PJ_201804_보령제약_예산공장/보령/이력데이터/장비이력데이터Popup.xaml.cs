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

namespace 보령
{
    public partial class 장비이력데이터Popup : iPharmMESChildWindow
    {
        public 장비이력데이터Popup()
        {
            InitializeComponent();
        }

        public DateTime resTime;

        private void txtTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(ValidationValue(txtTime.Text))
                {
                    C1.Silverlight.C1MessageBox.Show(string.Format("{0:HH:mm:ss}",resTime), "시간확인", C1.Silverlight.C1MessageBoxButton.OKCancel, x => {
                        if(x == MessageBoxResult.OK)
                            this.DialogResult = true;
                    });
                }
                else
                    txtTime.Text = "";
            }
        }
        private bool ValidationValue(string target)
        {
            int temp;
            int[] time = new int[3];

            if (target.Length > 6)
            {
                txtAlert.Content = "6자리 이상을 입력하였습니다. 다시 입력하세요.";
                return false;
            }
            else
            {
                target = target.PadRight(6, '0');

                if (!int.TryParse(target, out temp))
                {
                    txtAlert.Content = "잘못된 형식을 입력하였습니다. 다시 입력하세요.";
                    return false;
                }

                time[2] = temp % 100; // 초
                time[1] = (temp / 100) % 100; // 분
                time[0] = temp / 10000; // 시

                if (!(0 <= time[0] && time[0] < 24))
                {
                    txtAlert.Content = "잘못된 형식을 입력하였습니다. 다시 입력하세요.";
                    return false;
                }
                if (!(0 <= time[1] && time[1] < 60))
                {
                    txtAlert.Content = "잘못된 형식을 입력하였습니다. 다시 입력하세요.";
                    return false;
                }
                if (!(0 <= time[2] && time[2] < 60))
                {
                    txtAlert.Content = "잘못된 형식을 입력하였습니다. 다시 입력하세요.";
                    return false;
                }

                resTime = resTime.Date.AddSeconds(time[0] * 3600 + time[1] * 60 + time[2]);

                return true;
            }
        }
    }
}

