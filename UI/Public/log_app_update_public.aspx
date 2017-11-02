<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="log_app_update_public.aspx.vb" Inherits="UI.log_app_update_public" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title runat="server" id="title1" enableviewstate="true">MARKTIME 5.0</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="x-ua-compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <link href="~/Styles/Site_v11.css" rel="stylesheet" type="text/css" />    
    <link href="~/Images/favicon.ico" rel="shortcut icon" type="image/x-icon" />
</head>
<body>
    <form id="form1" runat="server">

        <div style="background-color: white;">
            <div style="float: left;">
                <img src="../Images/information_32.png" />
                <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Historie novinek a změn v aplikaci MARKTIME 5.0"></asp:Label>
                <a href="../Account/login.aspx" style="margin-left:20px;">Přihlásit se</a>
            </div>
            

            <div style="clear: both;"></div>
            <div style="padding-left: 10px;">
                <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
            </div>
        </div>

    </form>
</body>
</html>
