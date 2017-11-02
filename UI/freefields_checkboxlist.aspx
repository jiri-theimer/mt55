<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="freefields_checkboxlist.aspx.vb" Inherits="UI.freefields_checkboxlist" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadTreeView ID="tr1" runat="server" Skin="Default" ShowLineImages="true" SingleExpandPath="false" Height="90%" CheckBoxes="true">
    </telerik:RadTreeView>


    <asp:HiddenField ID="hidValue" runat="server" />
    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidSourceControl" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
