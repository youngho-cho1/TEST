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
    public partial class SelectListPopup : LGCNS.iPharmMES.Common.iPharmMESChildWindow
    {
        public SelectListPopup()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public new void Show()
        {
            try
            {
                base.Show();

                LGCNS.iPharmMES.Common.UIObject.SetObjectLang(this, LGCNS.EZMES.Common.LogInInfo.LangID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dataGird_LoadedRowPresenter(object sender, C1.Silverlight.DataGrid.DataGridRowEventArgs e)
        {
            e.Row.Height = new C1.Silverlight.DataGrid.DataGridLength(50);
            e.Row.Presenter.FontSize = 16;
        }

        private void dataGird_LoadedColumnHeaderPresenter(object sender, C1.Silverlight.DataGrid.DataGridColumnEventArgs e)
        {
            e.Column.HeaderPresenter.FontSize = 13;
        }
    }
}

