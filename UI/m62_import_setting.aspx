<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="m62_import_setting.aspx.vb" Inherits="UI.m62_import_setting" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:CheckBox ID="chkAllowImport" runat="server" AutoPostBack="true" Text="Importovat vybrané měny z kurzovního lístku ČNB" />

    <asp:Panel ID="panJ27" runat="server" CssClass="content-box2">
        <div class="title">
            Vybrané měny    
            <asp:HyperLink ID="aCNB" runat="server" Target="_blank" NavigateUrl="http://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.jsp" Text="ČNB | Kurzy devizového trhu"></asp:HyperLink>
        </div>
        <div class="content">
            <asp:CheckBoxList ID="j27ids" runat="server" DataTextField="j27Code" DataValueField="j27Code" RepeatColumns="3" CellPadding="5"></asp:CheckBoxList>
        </div>
    </asp:Panel>

    <div class="content-box2">
        <div class="title">
            Načíst kurzy pro vybraný den
            
        </div>
        <div class="content">
            <telerik:RadDatePicker ID="dat1" runat="server" Width="120px">
                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
            </telerik:RadDatePicker>
            <asp:Button ID="cmdImport" runat="server" Text="Spustit" CssClass="cmd" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
