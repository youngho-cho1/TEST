using ShopFloorUI;
using System.Windows.Controls;
using System.Linq;
using LGCNS.EZMES.ControlsLib;
using System.Collections.Generic;
using LGCNS.iPharmMES.Common;
using System.Windows.Media;
using System;
using System.ComponentModel;

namespace 보령
{
    [Description("현장칭량 원료 피킹정보 조회 및 불출라벨 발행")]
    public partial class 불출라벨발행 : ShopFloorCustomWindow
    {
        public 불출라벨발행()
        {
            InitializeComponent();
        }

        private void dgMaterials_LoadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            if (e.Cell.Presenter.Content.GetType() == typeof(C1.Silverlight.DataGrid.DataGridRowHeaderPresenter))
            {
                System.Windows.Controls.ContentControl cc = (e.Cell.Presenter.Content as System.Windows.Controls.ContentControl);
                cc.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                cc.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                cc.Background = new SolidColorBrush(Colors.White);
            }

            e.Cell.Presenter.Background = new SolidColorBrush(Colors.White);
        }

        private void dgMaterials_UnloadedCellPresenter(object sender, C1.Silverlight.DataGrid.DataGridCellEventArgs e)
        {
            e.Cell.Presenter.Background = null;
        }
        private void btnCacel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if(dgMaterials.ItemsSource != null && dgMaterials.ItemsSource is BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection)
                {
                    BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection items = dgMaterials.ItemsSource as BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection;
                    foreach (BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATA item in items)
                        item.PRINTFLAG = true;

                    dgMaterials.Refresh();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (dgMaterials.ItemsSource != null && dgMaterials.ItemsSource is BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection)
                {
                    BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection items = dgMaterials.ItemsSource as BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATACollection;
                    foreach (BR_BRS_GET_UDT_ProductionOrderPickingInfo_Solution.OUTDATA item in items)
                        item.PRINTFLAG = false;

                    dgMaterials.Refresh();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void PrinterList_SelectionChanged(object sender, C1.Silverlight.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            try
            {
                if (PrinterList.SelectedItem != null && PrinterList.SelectedItem is BR_PHR_SEL_System_Printer.OUTDATA)
                    txtPrinter.Text = (PrinterList.SelectedItem as BR_PHR_SEL_System_Printer.OUTDATA).PRINTERNAME;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

