<%@ Page Title="About Us" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="about.aspx.vb" Inherits="UI.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="text-align: center;">
        <div style="padding-top:100px;">
            <img alt="logo" src="Images/logo_transparent.png" style="border: 0px;" />
        </div>
       
        <div class="div6">
            <asp:Label ID="lblVersion" runat="server"></asp:Label>
        </div>
        <p>
            Za vývoj jádra systému odpovídá Jiří Theimer | CleverApp s.r.o., podpora: <a href="mailto:info@marktime.cz">info@marktime.cz</a>.
        </p>
        <p>
            <a href="log_app_update.aspx">Release LOG k této verzi</a>
        </p>

        <div class="div6">

            <asp:HyperLink ID="link1" runat="server" Target="_blank" Text="www.marktime.cz" NavigateUrl="http://www.marktime.cz"></asp:HyperLink>
        </div>
    </div>
</asp:Content>
