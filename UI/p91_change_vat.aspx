<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_change_vat.aspx.vb" Inherits="UI.p91_change_vat" %>
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
                <asp:Label ID="lblX15ID" runat="server" CssClass="lbl" Text="Převést všechny úkony faktury na sazbu:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x15ID" runat="server" DataTextField="x15Name" DataValueField="pid" AutoPostBack="true"></asp:DropDownList>
            </td>
            <td><asp:Label ID="lblRate" runat="server" CssClass="lbl" Text="Hodnota sazby (%):"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="NewRate" runat="server" Width="40px" MinValue="0" MaxValue="30"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
