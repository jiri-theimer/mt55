<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="plugin_modal.aspx.vb" Inherits="UI.plugin_modal" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function loadSplitter(sender) {
            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2;

            sender.set_height(h3);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" OnClientLoaded="loadSplitter" ResizeMode="Proportional"  PanesBorderSize="0" Skin="Metro" Orientation="Horizontal">
        <telerik:RadPane ID="navigationPane" runat="server" Height="40px">
            <div class="div6">
                <span>Plugin:</span>
                <asp:DropDownList ID="x31ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="x31Name" Style="width: 350px;" BackColor="yellow"></asp:DropDownList>
                <asp:Button ID="cmdRefresh" runat="server" CssClass="cmd" Text="Obnovit" />
            </div>
            <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
            <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />
        </telerik:RadPane>
        <telerik:RadPane ID="contentPane" runat="server" ContentUrl="blank.aspx">
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
