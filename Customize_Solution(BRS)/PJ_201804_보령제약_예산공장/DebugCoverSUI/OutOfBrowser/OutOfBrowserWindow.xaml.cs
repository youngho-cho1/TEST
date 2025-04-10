using MainLogIn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DebugCoverSUI
{
    public partial class OutOfBrowserWindow : UserControl
    {
        public OutOfBrowserWindow()
        {
            this.Loaded += OutOfBrowserWindow_Loaded;
            InitializeComponent();
        }

        private void OutOfBrowserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Current.IsRunningOutOfBrowser)
            {
                App.Current.MainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                }));

                Application.Current.CheckAndDownloadUpdateCompleted += OnCheckAndDownloadUpdateCompleted;
                Application.Current.CheckAndDownloadUpdateAsync();
            }
        }

        private void OnCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                App.Current.MainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _TextMessage1.Text = "프로그램이 Update되었습니다.";
                    _TextMessage2.Text = "프로그램을 다시 시작하십시요.";
                    _ButtonExit.Visibility = Visibility.Visible;
                }));

                return;
            }

            if (e.Error != null && e.Error
                                is PlatformNotSupportedException)
            {
                MessageBox.Show("A New version of Silverlight is required"
                                + Environment.NewLine
                                + "Please visit the application home page")
                                ;
                return;
            }

            StartUp();
        }

        void StartUp()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<string>();

            try
            {
                string originalString = Application.Current.Host.Source.OriginalString.Substring(0, Application.Current.Host.Source.OriginalString.IndexOf("/ClientBin"));
                Uri hostUrl = new Uri($"{originalString}/OutOfBrowserAgent/InitParams", UriKind.Absolute);

                WebClient wc = new WebClient();
                OpenReadCompletedEventHandler handler = null;
                handler = (s, e) =>
                {
                    var sender = s as WebClient;
                    sender.OpenReadCompleted -= handler;

                    if ((e.Error == null) && (e.Cancelled == false))
                    {
                        try
                        {
                            StreamReader reader = new StreamReader(e.Result);
                            string cipherText = reader.ReadToEnd();
                            string json = LGCNS.iPharmMES.OutOfBrowser.OOBLib.Decrypt(cipherText.Trim(new char[] { '"' }));
                            var initParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                            foreach (string key in initParams.Keys)
                            {
                                Application.Current.Host.InitParams.Add(key, initParams[key]);
                            }

                            AppStartUp.InitializeApp();

                            ((Grid)Application.Current.RootVisual).Children.RemoveAt(0);
                            ((Grid)Application.Current.RootVisual).Children.Add(new Login_New());
                            tcs.TrySetResult(cipherText);
                        }
                        catch (Exception innerEx)
                        {
                            tcs.TrySetException(innerEx);
                        }
                    }
                    else
                    {
                        MessageBox.Show(e.Error.Message);

                        tcs.TrySetException(e.Error);
                    }
                };

                wc.OpenReadCompleted += handler;
                wc.OpenReadAsync(hostUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                tcs.TrySetResult(null);
            }
        }

        private void _ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.IsRunningOutOfBrowser)
            {
                if (MessageBox.Show("프로그램을 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Application.Current.MainWindow.Close();
                }
            }
        }
    }
}
