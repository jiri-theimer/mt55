<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="admin_smtp.aspx.vb" Inherits="UI.admin_smtp" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <span class="infoNotification">Pokud uživatel nemá v MARKTIME nastavený vlastní poštovní účet, použije se tato adresa:</span>
    </div>
    <div class="div6">
        <asp:Label ID="Label2" runat="server" Text="E-mail adresa odesílatele:"></asp:Label>
        <asp:TextBox ID="SMTP_SenderAddress" runat="server" Style="width: 150px;"></asp:TextBox>

    </div>
    
    
    
    <div class="div6">
        
       
        <asp:RadioButtonList ID="opgSMTP_UseWebConfigSetting" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
            <asp:ListItem Text="Nastavení SMTP serveru načítat z web.config (sekce mailSettings)" Value="1"></asp:ListItem>
            <asp:ListItem Text="Vybrat jiný SMTP účet" Value="0"></asp:ListItem>
        </asp:RadioButtonList>
    </div>
    <asp:Panel ID="panWebConfig" runat="server">
        <table cellpadding="3" cellspacing="2">
            <tr>
                <td>
                    <asp:Label ID="Datalabel1" runat="server" Text="SMTP server:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="default_server" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="Adresa odesílatele:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="default_sender" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
   


    <asp:Panel ID="panRec" runat="server">
        <table cellpadding="6" cellspacing="2">
            <tr>
                <td>
                    <asp:Label ID="lblO40ID" runat="server" Text="Vybrat zavedený SMTP účet:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="cbxO40ID" runat="server" DataValueField="pid" DataTextField="o40Name"></asp:DropDownList>
                </td>
            </tr>

            

        </table>
    </asp:Panel>

    <hr />
    <div class="div6">
        <asp:Label ID="Label1" runat="server" Text="MARKTIME url adresa uvedená jako odkaz ve zprávě:"></asp:Label>
        <asp:TextBox ID="AppHost" runat="server" Style="width: 300px;"></asp:TextBox>
        <span class="infoInForm">Tuto adresu systém vkládá do zpráv, aby se uživatel kliknutím dostal rovnou do MARKTIME.</span>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

