using LGCNS.iPharmMES.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using MainLogIn;

namespace DebugCoverSUI
{
    public class AppStartUp
    {
        static public void InitializeApp()
        {
            LGCNS.iPharmMES.Common.Common.WF_SVR_URL = Application.Current.Host.InitParams["RecipeServiceUrl"];

            LGCNS.iPharmMES.Common.Common.ComputerName = Application.Current.Host.InitParams["HostName"];
            LGCNS.iPharmMES.Common.Common.ClientIP = Application.Current.Host.InitParams["ClientIP"];
            LGCNS.iPharmMES.Common.Common.UserAccount = Application.Current.Host.InitParams["UserAccount"];
            LGCNS.iPharmMES.Common.Common.CustomURL = Application.Current.Host.InitParams.ContainsKey("CustomURL") ? Application.Current.Host.InitParams["CustomURL"] : null;

            LGCNS.iPharmMES.Common.BizActorRuleBase.ServerName = Application.Current.Host.InitParams["BizActorServerName"];
            LGCNS.iPharmMES.Common.BizActorRuleBase.Port = Application.Current.Host.InitParams["BizActorPort"];
            LGCNS.iPharmMES.Common.BizActorRuleBase.SysID = Application.Current.Host.InitParams["BizActorSysID"];
            LGCNS.iPharmMES.Common.BizActorRuleBase.Protocol = Application.Current.Host.InitParams.ContainsKey("BizActorProtocol") ? Application.Current.Host.InitParams["BizActorProtocol"] : string.Empty;
            LGCNS.iPharmMES.Common.BizActorRuleBase.LoggingMode =
                (Common.enumLoggingMode)Enum.Parse(typeof(Common.enumLoggingMode), Application.Current.Host.InitParams["LoggingMode"], true);


            int interval = 2000;
            if (Application.Current.Host.InitParams.ContainsKey("GetWeightInterval"))
            {
                string interval_str = Application.Current.Host.InitParams["GetWeightInterval"];

                if (int.TryParse(interval_str, out interval) == false)
                {
                    interval = 2000;
                }
            }

            int checkcnt = 2;
            if (Application.Current.Host.InitParams.ContainsKey("GetWeightCheckCnt"))
            {
                string checkcnt_str = Application.Current.Host.InitParams["GetWeightCheckCnt"];

                if (int.TryParse(checkcnt_str, out checkcnt) == false)
                {
                    checkcnt = 2;
                }
            }

            //인터페이스 Interval
            Application.Current.Resources.Add("GetWeightCheckCnt", checkcnt.ToString());
            Application.Current.Resources.Add("GetWeightInterval", interval.ToString());
        }

        static public void StartOutOfBrowser()
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

    }
}
