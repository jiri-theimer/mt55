<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o40_record.aspx.vb" Inherits="UI.o40_record" %>

<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Název účtu:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40Name" runat="server" Style="width: 300px;"></asp:TextBox>
                (jakýkoliv výraz)
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo40EmailAddress" Text="E-mail adresa:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40EmailAddress" runat="server" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" Text="Adresa SMTP serveru:" runat="server" CssClass="lblReq"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40Server" runat="server" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>


        <tr>
            <td>
                <asp:Label ID="Label3" Text="Port:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40Port" runat="server" Style="width: 50px;"></asp:TextBox>
                (Pokud není vyplněno, bere se hodnota 25.)
            </td>
        </tr>


        
        <tr>
            <td>
                <asp:Label ID="Label4" Text="SSL mód:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="o40SslModeFlag" runat="server">
                    <asp:ListItem Text="Bez SSL" Value="0"></asp:ListItem>
                    <asp:ListItem Text="SSL implicit" Value="1"></asp:ListItem>
                    <asp:ListItem Text="SSL explicit" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label5" Text="SMTP Authentication mód:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="o40SmtpAuthentication" runat="server">
                    <asp:ListItem Text="Auto" Value="0" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="CramMD5" Value="3"></asp:ListItem>
                    <asp:ListItem Text="DigestMD5" Value="2"></asp:ListItem>
                    <asp:ListItem Text="GssApi" Value="9"></asp:ListItem>
                    <asp:ListItem Text="Login" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Ntlm" Value="7"></asp:ListItem>
                    <asp:ListItem Text="OAuth20" Value="10"></asp:ListItem>
                    <asp:ListItem Text="Plain" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

      
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="o40IsVerify" runat="server" Text="Server vyžaduje ověření" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo40Login" Text="Login účtu:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40login" runat="server" Style="width: 300px;"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblo40Password" Text="Heslo:" runat="server" CssClass="lbl"></asp:Label></td>
            <td>
                <asp:TextBox ID="o40Password" runat="server" Style="width: 300px;" TextMode="Password"></asp:TextBox>
                <asp:Button ID="cmdChangePWD" runat="server" Text="Změnit heslo" CssClass="cmd" Visible="false" />
            </td>
        </tr>
    </table>
    <p></p>
    <asp:Button ID="cmdTest" runat="server" CssClass="cmd" Text="Otestovat připojení k účtu" />
    <asp:HiddenField ID="hidMyAccount" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
