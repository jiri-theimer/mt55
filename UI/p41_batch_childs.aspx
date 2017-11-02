<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p41_batch_childs.aspx.vb" Inherits="UI.p41_batch_childs" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">
            Rozsah hromadné aktualizace
        </div>
        <div class="content">
            <div class="div6">
                <asp:CheckBox ID="chkRoles" runat="server" Text="Obsazení projektových rolí" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkP28ID" runat="server" Text="Klient projektu" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkJ18ID" runat="server" Text="Středisko projektu" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkP51ID" runat="server" Text="Ceník sazeb" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkP87ID" runat="server" Text="Fakturační jazyk" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkP92ID" runat="server" Text="Výchozí typ faktury" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkValidity" runat="server" Text="Časová platnost záznamu (archiv)" Checked="true" />
            </div>
        </div>
    </div>
    <div class="content-box2">
        <div class="title">
            Pod-projekty
        </div>
        <div class="content">
           <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
