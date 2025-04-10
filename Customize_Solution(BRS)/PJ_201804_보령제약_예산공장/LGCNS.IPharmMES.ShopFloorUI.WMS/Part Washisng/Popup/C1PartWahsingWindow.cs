using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using C1.Silverlight;

namespace WMS
{
    public class C1PartWahsingWindow : C1Window, INotifyPropertyChanged
    {        
        private string _parameter = null;
        private object[] _parameters = null;
        public bool isLoaded = false;
        
        public string Parameter
        {
            get { return _parameter; }
            set { _parameter = value; }
        }

        public object[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public static readonly DependencyProperty _Title =
        DependencyProperty.Register("Title", typeof(object), typeof(C1PartWahsingWindow), new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));


        public object Title
        {
            get { return (object)GetValue(_Title); }
            set
            {
                SetValue(_Title, value);
            }
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((C1PartWahsingWindow)d).Header = e.NewValue;
            }
        }

        bool? _DialogResult;

        public new bool? DialogResult
        {
            get { return _DialogResult; }
            set { _DialogResult = value; this.Close(); }
        }

        public object Authority { get; set; }

        private Image _Msg_Error;
        
        //public Image Msg_error
        //{
        //    get { return Resources.Source("/WMS;component/Images/ico_Error.png"); }
        //}
        //public Image Msg_OK
        //{
        //    get { return Resources.Source("/WMS;component/Images/ico_Ok.png"); }
        //}
        //public Image Msg_Warring
        //{
        //    get { return Resources.Source("/WMS;component/Images/ico_Warring.png"); }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty _ObjectTag =
        DependencyProperty.Register("ObjectTag", typeof(object), typeof(C1PartWahsingWindow), new PropertyMetadata(new PropertyChangedCallback(OnObjectTagPropertyChanged)));


        public object ObjectTag
        {
            get { return (object)GetValue(_ObjectTag); }
            set
            {
                SetValue(_ObjectTag, value);
            }
        }

        private static void OnObjectTagPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public C1PartWahsingWindow() : base()
        {
            this.Closed += C1PartWahsingWindow_Closed;
            base.Loaded +=C1PartWahsingWindow_Loaded;

            this.ShowMaximizeButton = false;
            this.ShowMinimizeButton = false;
            this.ShowCloseButton = false;
            this.HeaderBackground = new SolidColorBrush(Colors.Black);
            this.HeaderForeground = new SolidColorBrush(Colors.White);
            this.ButtonBackground = new SolidColorBrush(Colors.DarkGray);
            this.ButtonForeground = new SolidColorBrush(Colors.White);
            this.MouseOverBrush = new SolidColorBrush(Colors.Black);
            this.PressedBrush = new SolidColorBrush(Colors.Black);

        }

        void C1PartWahsingWindow_Loaded(object sender, RoutedEventArgs e)
        {            
 	        this.isLoaded = true;
        }

        void C1PartWahsingWindow_Closed(object sender, EventArgs e)
        {
            this.Closed -= C1PartWahsingWindow_Closed;
        }

    }
}
