<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="robot.aspx.vb" Inherits="UI.robot" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>
    <asp:Panel ID="panModal" runat="server" Visible="false" style="margin-top:40px;">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server" />
        <span>Spustit robota zpětně pro datum:</span>
        <telerik:RadDatePicker ID="datNow" runat="server" Width="120px">
                                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                            </telerik:RadDatePicker>

        <asp:Button ID="cmdRunNow" runat="server" Text="Spustit" CssClass="cmd" />

        <button type="button" onclick="window.close()">Zavřít</button>
    </asp:Panel>
    </form>
</body>
</html>
