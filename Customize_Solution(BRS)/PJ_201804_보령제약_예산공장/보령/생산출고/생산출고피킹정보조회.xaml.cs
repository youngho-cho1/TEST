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
    public partial class 생산출고피킹정보조회 : ShopFloorCustomWindow
    {
        public 생산출고피킹정보조회()
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

