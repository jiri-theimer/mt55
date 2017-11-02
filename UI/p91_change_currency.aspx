<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_change_currency.aspx.vb" Inherits="UI.p91_change_currency" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                <asp:Label ID="lblJ27ID" runat="server" CssClass="lbl" Text="Převést fakturu na měnu:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="j27ID" runat="server" DataTextField="j27Code" DataValueField="pid" AutoPostBack="true"></asp:DropDownList>
            </td>

        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
