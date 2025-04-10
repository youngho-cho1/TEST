<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DirectoryView.aspx.cs" Inherits="DirectoryView" %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fileTrans.aspx.cs" Inherits="fileTrans" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
<script type="text/javascript">
    function checkFinish() {
        if (document.getElementById("hiddnFinishYn").value == "Y") {
            if (document.getElementById("hiddnOperationType").value == "root") {
                var ret = document.getElementById("hiddnResult").value;
                document.getElementById("hiddnFinishYn").value = "N";
                parent.endGetDirectoryRoot(ret);
            }
            else {
                var ret = document.getElementById("hiddnResult").value;
                document.getElementById("hiddnFinishYn").value = "N";
                parent.endGetDirectory(ret);
            }
        }
    }
</script>
</head>
<body onload="checkFinish();">
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hiddnFinishYn" runat="server" Value="N" />
        <asp:HiddenField ID="hiddnOperationType" runat="server" Value="" />
        <asp:HiddenField ID="hiddnResult" runat="server" Value="" />
    </div>
    </form>
</body>
</html>
