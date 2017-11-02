<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="select_doctype.aspx.vb" Inherits="UI.select_doctype" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:panel ID="panMasterEntity" runat="server" CssClass="content-box2" style="margin-top:20px;" Visible="false">
        <div class="title">
            <asp:Label ID="EntityName" runat="server"></asp:Label>
        </div>
        <div class="content">
            <asp:Label ID="EntityRecord" runat="server" CssClass="valboldblue"></asp:Label>
        </div>
    </asp:panel>

    <div class="content-box2" style="margin-top:20px;">
        <div class="title">Vyberte typ dokumentu</div>
        <div class="content">
            <asp:RadioButtonList ID="x18ID" runat="server" RepeatDirection="Vertical" DataValueField="pid" DataTextField="x18Name" CellPadding="6"></asp:RadioButtonList>
        </div>
    </div>
    

    <asp:HiddenField ID="hidOcas" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
