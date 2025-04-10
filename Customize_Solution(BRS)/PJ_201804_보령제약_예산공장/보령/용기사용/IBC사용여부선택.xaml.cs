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

namespace 보령
{
    [ShopFloorCustomHidden]
    [Description("IBC사용여부선택(사용안함)")]
    public partial class IBC사용여부선택 : ShopFloorCustomWindow
    {
        public IBC사용여부선택()
        {
            InitializeComponent();

            System.Text.StringBuilder empty = new System.Text.StringBuilder();
            LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, ref empty, LGCNS.EZMES.Common.LogInInfo.LangID);
        }
        
        public override string TableTypeName
        {
            get { return "TABLE,IBC사용여부선택"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        
    }
}
