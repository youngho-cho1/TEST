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

using LGCNS.EZMES.ControlsLib;

namespace 보령
{
    public partial class RemainedSourceHandlingWindow : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {
        public RemainedSourceHandlingWindow()
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

        public new void Show()
        {
            base.Show();

            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, LGCNS.EZMES.Common.LogInInfo.LangID);
        }
    }
}

