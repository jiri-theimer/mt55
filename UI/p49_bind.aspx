<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p49_bind.aspx.vb" Inherits="UI.p49_bind" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function RowSelected(sender, args) {
            
            document.getElementById("<%=hidP49ID.ClientID%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            var p49id = document.getElementById("<%=Me.hidP49ID.ClientID%>").value;
            window.parent.hardrefresh(p49id,"p49-bind")
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <span>Rozpočet:</span>
        <asp:Label ID="Budget" runat="server" CssClass="valboldblue"></asp:Label>
        <span>Sešit:</span>
        <asp:Label ID="Sheet" runat="server" CssClass="valbold"></asp:Label>
        <asp:CheckBox ID="chkIncludeExtended" runat="server" AutoPostBack="true" Text="Zobrazovat i spárované položky s reálnými výkazy" />
    </div>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowDblClick="RowDoubleClick" OnRowSelected="RowSelected"></uc:datagrid>



    <asp:HiddenField ID="hidP41ID" runat="server" />
    <asp:HiddenField ID="hidP34ID" runat="server" />
    <asp:HiddenField ID="hidP49ID" runat="server" />
    <asp:HiddenField ID="hidP45ID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
