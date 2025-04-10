using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 보령.UserControls
{
    public class BarcodeTextBox : TextBox
    {
        public event CustomKeyDownEventHandler CustomKeyDown;

        C1.Util.Timer _tmr = new C1.Util.Timer();

        public BarcodeTextBox()
        {
            _tmr.Tick += _tmr_Tick;
            _tmr.Interval = new TimeSpan(0, 0, 0, 0, 300); // 500ms
        }

        void _tmr_Tick(object sender, EventArgs e)
        {
            _tmr.Stop();

            //if (this.Dispatcher.CheckAccess()) this.Text = "";
            //else this.Dispatcher.BeginInvoke(() => this.Text = "");

            //MessageBox.Show("Keyboard를 사용할 수 없습니다.");
        }

        DateTime _lastKeyPress = DateTime.Now;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                if (_tmr.IsEnabled == false)
                {
                    _tmr.Start();

                    base.OnKeyDown(e);
                }
                else
                {
                    _tmr.Stop();
                    _tmr.Start();

                    base.OnKeyDown(e);
                }
            }
            else
            {
                _tmr.Stop();

                base.OnKeyDown(e);
            }
        }

        public void EnableKeyIn()
        {
            var popup = new BarcodePopup();
            popup.Closed += (s, e) =>
                {
                    if (popup.DialogResult == true)
                    {
                        this.Text = popup.tbText.Text;

                        if(CustomKeyDown != null)
                            CustomKeyDown.Invoke(this, new CustomKeyDownEventArgs() { Barcode = this.Text });
                    }
                };
            popup.Show();
        }
    }

    public delegate void CustomKeyDownEventHandler(object sender, EventArgs arg);

    public class CustomKeyDownEventArgs : EventArgs
    {
        public string Barcode { get; set; }
    }

    public class CustomKeyDownAction : TriggerAction<BarcodeTextBox>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CustomKeyDownAction), new PropertyMetadata(OnCommandProperty));

        private static void OnCommandProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        protected override void Invoke(object parameter)
        {
            Command.Execute(this.AssociatedObject.Text);
        }
    }
}
