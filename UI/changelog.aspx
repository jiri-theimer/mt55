<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="changelog.aspx.vb" Inherits="UI.changelog" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hidPrefix" Value="" runat="server" />

    <asp:RadioButtonList ID="opgView" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
        <asp:ListItem Text="Pouze, kde jsou hodnoty" Value="2" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Pouze, kde došlo ke změně" Value="3"></asp:ListItem>
        <asp:ListItem Text="Všechny sloupce" Value="1"></asp:ListItem>
    </asp:RadioButtonList>
    <uc:datagrid ID="g1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>

    <asp:Label ID="lblMessage" runat="server" CssClass="infoNotification"></asp:Label>
    <div class="div6">
        <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
    </div>
    <asp:panel ID="panExport" runat="server" cssclass="div6">
        <img src="Images/xls.png" alt="xls" />
        <asp:LinkButton ID="cmdXLS" runat="server" Text="XLS" ToolTip="MS-Excel export" />

        <img src="Images/pdf.png" alt="pdf" style="margin-left: 20px;" />
        <asp:LinkButton ID="cmdPDF" runat="server" Text="PDF" ToolTip="PDF export" />

        <img src="Images/doc.png" alt="doc" style="margin-left: 20px;" />
        <asp:LinkButton ID="cmdDOC" runat="server" Text="DOC" ToolTip="MS-Word export" />
    </asp:panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
