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
    public partial class 사용한설비확인및종료 : ShopFloorCustomWindow
    {
        [Description("현재 오더, 공정에서 사용중인 설비목록 중 선택한 설비만 종료(칭량 공정에서 사용)")]
        public 사용한설비확인및종료()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,사용한설비확인및종료"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void MainDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(this.MainDataGrid.SelectedItem != null)
            {
                BR_BRS_SEL_EquipmentStatus_PROC_OPSG.OUTDATA tar = this.MainDataGrid.SelectedItem as BR_BRS_SEL_EquipmentStatus_PROC_OPSG.OUTDATA;
                tar.SELFLAG = !tar.SELFLAG;

                this.MainDataGrid.SelectedItem = null;
                this.MainDataGrid.Refresh();
            }
        }
    }
}
