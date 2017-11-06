<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="select_event_type.aspx.vb" Inherits="UI.select_event_type" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function cbx1_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["flag"] = "searchbox";
            context["j02id_explicit"] = "<%=Master.Factory.SysUser.j02ID%>";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="div6" style="height:20px;">
    <telerik:RadComboBox ID="cbx1" runat="server" Visible="false" DropDownWidth="500" Width="500px" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat..." OnClientItemsRequesting="cbx1_OnClientItemsRequesting" AutoPostBack="false">
        <WebServiceSettings Method="LoadComboData" UseHttpGet="false" />
    </telerik:RadComboBox>
    </div>

    <div class="content-box2" style="margin-top:20px;">
        <div class="title">Vyberte typ události</div>
        <div class="content">
            <asp:RadioButtonList ID="o21ID" runat="server" RepeatDirection="Vertical" DataValueField="pid" DataTextField="NameWithEntityAlias" CellPadding="8" AutoPostBack="true"></asp:RadioButtonList>
        </div>
    </div>
    <asp:HiddenField ID="hidOcas" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
