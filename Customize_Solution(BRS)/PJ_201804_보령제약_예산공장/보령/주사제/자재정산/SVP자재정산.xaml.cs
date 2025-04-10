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
using C1.Silverlight.DataGrid;
using System.ComponentModel;

namespace 보령
{
    [Description("주사제 자재정산")]
    public partial class SVP자재정산 : ShopFloorCustomWindow
    {
        public SVP자재정산()
        {
            InitializeComponent();
        }
        public override string TableTypeName
        {
            get { return "TABLE,SVP자재정산"; }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void gridPackingInfo_AutoGeneratingColumn(object sender, C1.Silverlight.DataGrid.DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                switch (e.Column.Name)
                {
                    case "MTRLID":
                        e.Cancel = false;
                        e.Column.IsReadOnly = true;
                        e.Column.Header = "자재코드";
                        break;
                    case "MTRLNAME":
                        e.Cancel = false;
                        e.Column.IsReadOnly = true;
                        e.Column.Header = "자재명";
                        break;
                    case "UOM":
                        e.Cancel = false;
                        e.Column.IsReadOnly = true;
                        e.Column.Header = "단위";
                        break;
                    case "PICKING":
                        e.Cancel = false;
                        //e.Column.IsReadOnly = true;
                        e.Column.Header = "인수량";
                        break;
                    case "ADDING":
                        e.Cancel = false;
                        //e.Column.IsReadOnly = true;
                        e.Column.Header = "추가량";
                        break;
                    case "SCRAP":
                        e.Cancel = false;
                        e.Column.Header = "불량(폐기)";
                        break;
                    case "SAMPLE":
                        e.Cancel = false;
                        e.Column.Header = "샘플량";
                        break;
                    case "REMAIN":
                        e.Cancel = false;
                        //e.Column.IsReadOnly = true;
                        e.Column.Header = "잔량(환입)";
                        break;
                    case "USING":
                        e.Cancel = false;
                        e.Column.Header = "사용량";
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }           
        }              
        // 수정값 입력 완료
        private void gridPackingInfo_CommittedEdit(object sender, DataGridCellEventArgs e)
        {
            try
            {
                (this.LayoutRoot.DataContext as SVP자재정산ViewModel).ConvertResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
