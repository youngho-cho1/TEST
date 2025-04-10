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

namespace WMS
{
    public partial class WashingMessageBox : C1PartWahsingWindow
    {
        public enum MessageStatusType
        {
            Error,
            Warring,
            Normal,
            initial
        }
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public WashingMessageBox()
        {
            InitializeComponent();

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public void MessageStaus(MessageStatusType type)
        {
            switch (type)
            {
                case MessageStatusType.Error:
                    imgType.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/WMS;component/Images/ico_Error.png");
                    tbMessage.Text = Message;
                    OKButton.Visibility = Visibility.Visible;
                    btninitial.Visibility = Visibility.Collapsed;
                    break;
                case MessageStatusType.Normal:
                    imgType.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/WMS;component/Images/ico_Ok.png");
                    tbMessage.Text = Message;
                    OKButton.Visibility = Visibility.Visible;
                    btninitial.Visibility = Visibility.Collapsed;
                    break;
                case MessageStatusType.Warring:
                    imgType.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/WMS;component/Images/ico_Warring.png");
                    tbMessage.Text = Message;
                    OKButton.Visibility = Visibility.Visible;
                    OKButton.Content = "강제종료";
                    btninitial.Visibility = Visibility.Collapsed;
                    break;
                case MessageStatusType.initial:
                    imgType.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/WMS;component/Images/ico_Warring.png");
                    tbMessage.Text = Message;
                    OKButton.Visibility = Visibility.Visible;
                    btninitial.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btninitial_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

