using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.UI.WebControls;

public partial class iPharmMES : System.Web.UI.Page
{
    public string HostName { get; set; }
    public string UserName { get; set; }


    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        HostName = string.Empty;

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("<param name=\"InitParams\" value=\"HostName={0},ClientIP={1},UserAccount={2},",
            Request.UserHostName, GetIPAddress(),
            GetUserAccount()));

        for (int index = 0; index < appSettings.Count; index++)
        {
            sb.Append(appSettings.GetKey(index));
            sb.Append("=");
            sb.Append(appSettings[index]);
            sb.Append(",");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("\" />");

        WebConfig.Text = sb.ToString();
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

        //((System.Security.Principal.WindowsPrincipal)User).Identity.Name;
    }

    string GetIPAddress()
    {
        if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            return null;
        }

        string ipAddress = string.Empty;

        string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipList)) ipAddress = ipList.Split(',')[0];

        ipAddress = Request.UserHostAddress;
        ipAddress = Request.ServerVariables["REMOTE_ADDR"];

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
}
