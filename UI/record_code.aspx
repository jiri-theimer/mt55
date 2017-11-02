<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="record_code.aspx.vb" Inherits="UI.record_code" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                Kód záznamu:
            </td>
            <td>
                <asp:TextBox ID="txtCode" runat="server" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Vlastník záznamu:
            </td>
            <td>
                <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td>
                Posledních 10 záznamů:
            </td>
            <td>
                <asp:Label ID="last10" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
