using LGCNS.iPharmMES.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class 출고포장자재환산popup : iPharmMESChildWindow
    {
        public string Msublotid = "";

        public 출고포장자재환산popup()
        {
            InitializeComponent();
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            decimal weight = 0m;

            if(string.IsNullOrWhiteSpace(Msublotid))
                Msublotid = (this.DataContext as 출고포장자재환산ViewModel).MSUBLOTBCD;

            var temp = (this.DataContext as 출고포장자재환산ViewModel).RollWeightList.Where(x => x.MSUBLOTBCD == Msublotid).FirstOrDefault();

            if (temp != null)
            {
                if (decimal.TryParse(this.txtWeight.Text, out weight))
                {
                    temp.WEIGHT = weight;

                    (this.DataContext as 출고포장자재환산ViewModel).ConvertWeight();
                    (this.DataContext as 출고포장자재환산ViewModel).MSUBLOTBCD = "";
                    this.DialogResult = true;
                }
                else
                {
                    txtAlarm.Text = "숫자를 입력하세요";
                    txtWeight.Focus();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as 출고포장자재환산ViewModel).ConvertWeight();
            this.DialogResult = true;
        }

        private void txtWeight_KeyDown(object sender, KeyEventArgs e)
        {
            decimal weight = 0m;

            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(Msublotid))
                    Msublotid = (this.DataContext as 출고포장자재환산ViewModel).MSUBLOTBCD;

                var temp = (this.DataContext as 출고포장자재환산ViewModel).RollWeightList.Where(x => x.MSUBLOTBCD == Msublotid).FirstOrDefault();

                if (temp != null)
                {
                    if (decimal.TryParse(this.txtWeight.Text, out weight))
                    {
                        temp.WEIGHT = weight;

                        (this.DataContext as 출고포장자재환산ViewModel).ConvertWeight();
                        (this.DataContext as 출고포장자재환산ViewModel).MSUBLOTBCD = "";
                        this.DialogResult = true;
                    }
                    else
                    {
                        txtAlarm.Text = "숫자를 입력하세요";
                        txtWeight.Focus();
                    }
                }
            }
           
        }
    }
}

