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
    public partial class 설비상태팝업 : iPharmMESChildWindow
    {
        private string POID;
        private string OPSGGUID;
        private string TYPE;
        private string EQPTID;
        public BR_BRS_SEL_EquipmentList_CleanCheck.OUTDATA RSLT;
        public 설비상태팝업(string poid, string opsgguid, string type, string eqptid)
        {
            InitializeComponent();
            POID = poid;
            OPSGGUID = opsgguid;
            TYPE = type;
            EQPTID = eqptid;
        }
        private async void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            BR_BRS_SEL_EquipmentList_CleanCheck bizrule = new BR_BRS_SEL_EquipmentList_CleanCheck();
            bizrule.INDATAs.Add(new BR_BRS_SEL_EquipmentList_CleanCheck.INDATA
            {
                POID = POID,
                OPSGGUID = OPSGGUID,
                EQPTTYPE = TYPE,
                EQPTID = EQPTID
            });

            if(await bizrule.Execute() == true)
                this.EQPTList.ItemsSource = bizrule.OUTDATAs;
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            RSLT = this.EQPTList.SelectedItem as BR_BRS_SEL_EquipmentList_CleanCheck.OUTDATA;
            this.DialogResult = true;
        }

        private void btnCacel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
