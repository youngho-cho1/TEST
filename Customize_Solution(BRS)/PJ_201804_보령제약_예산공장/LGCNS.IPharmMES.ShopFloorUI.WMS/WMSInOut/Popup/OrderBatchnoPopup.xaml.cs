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

namespace WMS
{
    public partial class OrderBatchnoPopup : iPharmMESChildWindow
    {
        public bool isOrderBatchLoaded;

        public OrderBatchnoPopup()
        {
            InitializeComponent();

            isOrderBatchLoaded = true;
        }

        private async void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (LayoutRoot.DataContext as WMSInoutViewModel != null)
            {
                if (dgOutputList.SelectedItem != null)
                {
                    var temp = dgOutputList.SelectedItem as BR_BRS_SEL_ProductionOrder_OrderList.OUTDATA;
                    (LayoutRoot.DataContext as WMSInoutViewModel).txtBatchNoOut = temp.BATCHNO;
                    (LayoutRoot.DataContext as WMSInoutViewModel).txtOrderNoOut = temp.POID;

                    (LayoutRoot.DataContext as WMSInoutViewModel).BR_PHR_SEL_ProductionOrderDetail_Info.INDATAs.Clear();
                    (LayoutRoot.DataContext as WMSInoutViewModel).BR_PHR_SEL_ProductionOrderDetail_Info.OUTDATAs.Clear();
                    (LayoutRoot.DataContext as WMSInoutViewModel).BR_PHR_SEL_ProductionOrderDetail_Info.INDATAs.Add(new BR_PHR_SEL_ProductionOrderDetail_Info.INDATA
                    {
                        POID = (LayoutRoot.DataContext as WMSInoutViewModel).txtOrderNoOut
                    });
                    await (LayoutRoot.DataContext as WMSInoutViewModel).BR_PHR_SEL_ProductionOrderDetail_Info.Execute();
                }
            }
            else if (LayoutRoot.DataContext as StorageInOutWmsViewModel != null)
            {
                if (dgOutputList.SelectedItem != null)
                {
                    var temp = dgOutputList.SelectedItem as BR_BRS_SEL_ProductionOrder_OrderList.OUTDATA;
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtBatchNoOut = temp.BATCHNO;
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtOrderNoOut = temp.POID;

                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATAs.Clear();
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_BRS_SEL_ProductionOrderDetail_ORDER.OUTDATAs.Clear();
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATAs.Add(new BR_BRS_SEL_ProductionOrderDetail_ORDER.INDATA
                    {
                        POID = (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtOrderNoOut
                    });
                    await (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_BRS_SEL_ProductionOrderDetail_ORDER.Execute();

                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtBatchNoOut = temp.BATCHNO;
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtOrderNoOut = temp.POID;

                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Clear();
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_PHR_SEL_ProductionOrderOutput_Define.OUTDATAs.Clear();
                    (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_PHR_SEL_ProductionOrderOutput_Define.INDATAs.Add(new BR_PHR_SEL_ProductionOrderOutput_Define.INDATA
                    {
                        POID = (LayoutRoot.DataContext as StorageInOutWmsViewModel).txtOrderNoOut,
                        OPSGGUID = null
                    });
                    await (LayoutRoot.DataContext as StorageInOutWmsViewModel).BR_PHR_SEL_ProductionOrderOutput_Define.Execute();

                }
            }
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (chkiud.IsChecked == true)
                chkiud.IsChecked = false;
            else
                chkiud.IsChecked = true;
        }
    }
}

