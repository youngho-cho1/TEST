using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.ServiceModel.Channels;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace OutOfBrowserDemo.App
{
    public class OutOfBrowserAgentController : ApiController
    {        
        [Route("OutOfBrowserAgent/InitParams")]
        public string GetInitParams()
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings;
            HttpRequestBase request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;

            Dictionary<string, string> initParams = new Dictionary<string, string>();

            initParams.Add("HostName", request.UserHostName );
            initParams.Add("ClientIP",    GetIPAddress(request) );
            initParams.Add("UserAccount", GetUserAccount() );

            for (int index = 0; index < appSettings.Count; index++)
            {
                initParams.Add(appSettings.GetKey(index), appSettings[index]);
            }

            string result = LGCNS.iPharmMES.OutOfBrowser.OOBLib.Encrypt(JsonConvert.SerializeObject(initParams));

            return result;
        }

        string GetIPAddress(HttpRequestBase request)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            string ipAddress = string.Empty;

            string ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList)) ipAddress = ipList.Split(',')[0];

            ipAddress = request.UserHostAddress;
            ipAddress = request.ServerVariables["REMOTE_ADDR"];

            if (!string.IsNullOrEmpty(ipAddress) &&
                (ipAddress.ToLower().Equals("localhost") ||
                 ipAddress.ToLower().Equals("127.0.0.1") ||
                 ipAddress.ToLower().Equals("::1")))
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                return host
                    .AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            }
            else
            {
                return ipAddress;
            }
        }

        string GetUserAccount()
        {
            if (System.Security.Principal.WindowsIdentity.GetCurrent() != null)
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            else if (System.ServiceModel.ServiceSecurityContext.Current != null &&
                System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity != null)
            {
                return System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.Name;
            }
            else
            {
                return System.Web.HttpContext.Current.User.Identity.Name;
            }
        }        
    }
}