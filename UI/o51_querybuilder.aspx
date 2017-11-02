<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o51_querybuilder.aspx.vb" Inherits="UI.o51_querybuilder" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
   <telerik:RadTreeView ID="tr1" runat="server" Skin="Default" ShowLineImages="false" SingleExpandPath="false" Height="90%" CheckBoxes="true">               
    </telerik:RadTreeView>


    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidO51IDs" runat="server" />

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
