using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;

public partial class iPharmMES : System.Web.UI.Page
{
    public string HostName { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        HostName = string.Empty;

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("<param name=\"InitParams\" value=\"HostName={0},ClientIP={1},",
            Request.UserHostName, GetIPAddress()));

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
