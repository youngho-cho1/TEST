using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Board_Original
{
    public partial class BoardMain : UserControl
    {
        private bool _isInBtn;
        public bool isInBtn
        {
            get { return _isInBtn; }
            set { _isInBtn = value; }
        }

        public BoardMain()
        {
            InitializeComponent();
            isInBtn = false;
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (btnGroup.Opacity == 0)
                sbFade.Begin();
            else if (btnGroup.Opacity == 1 && !isInBtn)
            {
                sbFade.Stop();
                sbFadeType01.Begin();
            }
        }

        private void btnGroup_MouseEnter(object sender, MouseEventArgs e)
        {
            isInBtn = true;
            sbFade.Stop();
            sbFadeType01.Stop();
            btnGroup.Opacity = 1;

        }

        private void btnGroup_MouseLeave(object sender, MouseEventArgs e)
        {
            isInBtn = false;
            sbFade.Stop();
            sbFadeType01.Begin();
        }
    }
}
