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

namespace 보령.UserControls
{
    public partial class CustomUIHeader : UserControl
    {
        public CustomUIHeader()
        {
            InitializeComponent();
            txtWorkRoom.Text = AuthRepositoryViewModel.Instance.RoomID;
        }

        #region Dependency Property

        #region D.P CurrentOrderInfo
        private CurrentOrderInfo _CurOrder;
        public CurrentOrderInfo CurOrder
        {
            get { return (CurrentOrderInfo)GetValue(CurOrderProperty); }
            set { SetValue(CurOrderProperty, value); }
        }

        public static readonly DependencyProperty CurOrderProperty =
            DependencyProperty.Register("CurOrder", typeof(CurrentOrderInfo), typeof(CustomUIHeader), new PropertyMetadata(OnCurOrderProperty));

        private static void OnCurOrderProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomUIHeader parent = (CustomUIHeader)d;
            //값을 전달할 타겟지정
            CurrentOrderInfo curOrder = (CurrentOrderInfo)e.NewValue;
            parent._CurOrder = curOrder;
            parent.txtBatchNo.Text = curOrder.BatchNo;
            parent.txtOrderNo.Text = curOrder.ProductionOrderID;
            parent.txtProcessName.Text = curOrder.OrderProcessSegmentName;
        }
        #endregion

        #region D.P OrderList

        public BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection OrderList
        {
            get { return (BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection)GetValue(OrderListProperty); }
            set { SetValue(OrderListProperty, value); }
        }

        public static readonly DependencyProperty OrderListProperty =
            DependencyProperty.Register("OrderList", typeof(BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection), typeof(CustomUIHeader), new PropertyMetadata(OnOrderListProperty));

        private static void OnOrderListProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomUIHeader parent = (CustomUIHeader)d;
            //값을 전달할 타겟지정
            var orderlist = (BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATACollection)e.NewValue;
            if(orderlist != null && orderlist.Count > 1)
            {
                parent.cboOrderList.ItemsSource = orderlist;
                parent.cboOrderList.Visibility = Visibility.Visible;
                parent.txtOrderNo.Visibility = Visibility.Collapsed;
                parent.lbCampaignOrder.Visibility = Visibility.Visible;

                parent.cboOrderList.SelectedIndex = 0;
            }
            else
            {
                parent.cboOrderList.Visibility = Visibility.Collapsed;
                parent.txtOrderNo.Visibility = Visibility.Visible;
                parent.lbCampaignOrder.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region D.P CanSelectOrderNo
        /// <summary>
        /// True : Combobox IsEnable True, False : Combobox IsEnable False
        /// </summary>
        public bool CanSelectOrderNo
        {
            get { return (bool)GetValue(CanSelectOrderNoProperty); }
            set { SetValue(CanSelectOrderNoProperty, value); }
        }

        public static readonly DependencyProperty CanSelectOrderNoProperty =
            DependencyProperty.Register("CanSelectOrderNo", typeof(bool), typeof(CustomUIHeader), new PropertyMetadata(OnCanSelectOrderNoProperty));

        private static void OnCanSelectOrderNoProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomUIHeader parent = (CustomUIHeader)d;
            //값을 전달할 타겟지정
            parent.cboOrderList.IsEnabled = (bool)e.NewValue;
        }
        #endregion

        #endregion

        #region Event
        private void cboOrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATA)
            {
                var curOrder = e.AddedItems[0] as BR_BRS_SEL_ProductionOrder_RECIPEISTGUID.OUTDATA;

                _CurOrder.BatchNo = curOrder.BATCHNO;
                _CurOrder.OrderID = curOrder.POID;
                _CurOrder.ProductionOrderID = curOrder.POID;

                txtBatchNo.Text = curOrder.BATCHNO;
                txtOrderNo.Text = curOrder.POID;
            }

            OrderChaged(sender, e);
        }

        /// <summary>
        /// Seletected OrderNo Chaged Event
        /// </summary>
        public event EventHandler OrderNoSelection_Changed;

        private void OrderChaged(object sender, EventArgs e)
        {
            if(OrderNoSelection_Changed != null)
            {
                OrderNoSelection_Changed(sender, e);
            }
        }
        #endregion
    }
}
