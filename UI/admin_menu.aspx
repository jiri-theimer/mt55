<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="admin_menu.aspx.vb" Inherits="UI.admin_menu" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="TreeMenu" Src="~/TreeMenu.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function rec(pid) {
            var url = "j62_record.aspx?j60id=<%=me.CurrentJ60ID%>&pid=" + pid;
            sw_master(url, "Images/setting_32.png");
        }
        function header() {
            var url = "j60_record.aspx?pid=<%=me.CurrentJ60ID%>";
            sw_master(url, "Images/setting_32.png");
        }
        function j60_create() {
            var url = "j60_record.aspx?pid=0";
            sw_master(url, "Images/setting_32.png");
        }
        function j60_clone() {
            var url = "j60_record.aspx?pid=<%=me.CurrentJ60ID%>&clone=1";
            sw_master(url, "Images/setting_32.png");
        }

        function hardrefresh(pid, flag) {

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefreshOnBehind, "", False)%>;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <span class="lbl">MENU šablona:</span>
        <asp:DropDownList ID="cbxJ60ID" DataTextField="j60Name" DataValueField="pid" runat="server" AutoPostBack="true" Style="min-width: 200px;"></asp:DropDownList>
        <button type="button" onclick="rec(0)">Nová menu položka</button>
        <button type="button" onclick="header()">Hlavička menu</button>
        <button type="button" onclick="j60_create()">Nové menu</button>
        <button type="button" onclick="j60_clone()">Zkopírovat menu</button>
    </div>
    <p></p>
    <uc:TreeMenu ID="tree1" runat="server" SingleExpandPath="false"></uc:TreeMenu>



    <asp:LinkButton ID="cmdRefreshOnBehind" runat="server" Text="refreshonbehind" Style="display: none;"></asp:LinkButton>
</asp:Content>
