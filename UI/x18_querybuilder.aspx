<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="x18_querybuilder.aspx.vb" Inherits="UI.x18_querybuilder" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <span class="val">Zaškrtněte alespoň jednu kategorii</span>
        <asp:Button ID="cmdUncheckAll" runat="server" CssClass="cmd" Text="Odškrtnout vše" />
        <asp:Button ID="cmdExpandAll" runat="server" CssClass="cmd" Text="Rozbalit vše" />
        <asp:Button ID="cmdCollapseAll" runat="server" CssClass="cmd" Text="Sbalit vše" />
    </div>
    

    <telerik:RadTreeView ID="tr1" runat="server" Skin="Default" ShowLineImages="false" SingleExpandPath="false" Height="90%" CheckBoxes="true">        
    </telerik:RadTreeView>


    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidKey" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
