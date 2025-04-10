using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LGLSFileCopy;
using System.IO;
using System.Diagnostics;

public partial class DirectoryView : System.Web.UI.Page
{
    static string _eventLog = "iPharmMES SUI";
    static string _eventSource = "iPharmMES/SUI";

    protected void Page_Load(object sender, EventArgs e)
    {
        string parent = Request["parent"];
        string getType = Request["GetDirectoryType"];

        LGLSFileCopy.GetDirectoryType myType;

        if (getType == "0") myType = GetDirectoryType.root;
        else myType = GetDirectoryType.child;

            var manager = new LGLSFileCopy.DirectoryInfomation();

            string retList = manager.GetDirectory(myType, parent);
            hiddnFinishYn.Value = "Y";
            hiddnOperationType.Value = myType.ToString();
            hiddnResult.Value = retList;
    }
}