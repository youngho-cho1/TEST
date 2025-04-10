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

namespace Site
{
    [Description("레시피디자이너 UI목록 팝업에서 설명칸에 조회되는 내용")]
    [ShopFloorCustomHidden]
    public partial class CustomUIDemo : ShopFloorCustomWindow
    {
        public CustomUIDemo()
        {
            InitializeComponent();
        }

        public override string TableTypeName
        {
            get
            {
                return "TABLE,CustomUIDemo";
            }
        }
    }
}
