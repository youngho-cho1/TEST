<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="iPharmMES.aspx.cs" Inherits="iPharmMES" %>
<%@ Import Namespace="System.Threading" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>iPharmMES 4.5</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>
    <script src="Silverlight.js" type="text/javascript"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Silverlight 응용 프로그램에서 처리되지 않은 오류 " + appSource + "\n";

            errMsg += "코드: " + iErrorCode + "    \n";
            errMsg += "범주: " + errorType + "       \n";
            errMsg += "메시지: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "파일: " + args.xamlFile + "     \n";
                errMsg += "줄: " + args.lineNumber + "     \n";
                errMsg += "위치: " + args.charPosition + "     \n";
            }
            else if (errorType == "RumtimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "줄: " + args.lineNumber + "     \n";
                    errMsg += "위치: " + args.charPosition + "     \n";
                }
                errMsg += "메서드 이름: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <script type="text/javascript">
        function ReportPageBridge(s) {
            var rpt = parent.parent.frames[0];
            rpt.location.href = "Report.aspx?" + s;
        }

        function doReload() {
            location.reload(true);
        }

    </script>
</head>
<body>
    <form id="form1" runat="server" style="height:100%">
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		  <param name="source" value="ClientBin/WeighingUI.xap"/>            
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
		  <param name="minRuntimeVersion" value="4.0.50826.0" />
		  <param name="autoUpgrade" value="true" />
          <!--<param name="windowless" value="true" /> -->
          <param name="Culture" value="<%=Thread.CurrentThread.CurrentCulture.Name %>"  />
          <param name="UICulture" value="<%=Thread.CurrentThread.CurrentUICulture.Name %>" />
          <asp:Literal ID="WebConfig" runat="server" />

		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Microsoft Silverlight 얻기" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe></div>
    </form>
</body>
</html>
