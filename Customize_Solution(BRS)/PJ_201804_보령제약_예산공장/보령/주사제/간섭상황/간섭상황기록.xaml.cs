using LGCNS.iPharmMES.Common;
using ShopFloorUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class 간섭상황기록 : ShopFloorCustomWindow
    {
        public override string TableTypeName
        {
            get { return "TABLE,간섭상황기록"; }
        }
        public 간섭상황기록()
        {
            InitializeComponent();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = false;
        }
        private void StrtTime_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var popup = new 간섭상황시간Popup();

                popup.resTime = StrtTime.DateTime.HasValue ? StrtTime.DateTime.Value : DateTime.Now;
                popup.Closed += (s1, e1) =>
                {
                    if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                        StrtTime.DateTime = Convert.ToDateTime(StrtDate.DateTime.Value.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
                };

                popup.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void EndTime_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var popup = new 간섭상황시간Popup();

                popup.resTime = EndTime.DateTime.HasValue ? EndTime.DateTime.Value : DateTime.Now;
                popup.Closed += (s1, e1) =>
                {
                    if (popup.DialogResult.HasValue && popup.DialogResult.Value)
                        EndTime.DateTime = Convert.ToDateTime(EndDate.DateTime.Value.ToString("yyyy-MM-dd") + popup.resTime.ToString(" HH:mm:ss"));
                };
                popup.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckDataGrid.SelectedItem != null)
            {
                간섭상황기록ViewModel.InterferSituation tar = this.CheckDataGrid.SelectedItem as 간섭상황기록ViewModel.InterferSituation;
                tar.CHK = !tar.CHK;

                this.CheckDataGrid.SelectedItem = null;
                this.CheckDataGrid.Refresh();
            }
        }
    }
}
