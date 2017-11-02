<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="entity_framework_rec_p91.aspx.vb" Inherits="UI.entity_framework_rec_p91" %>
<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="p91_subgrid" Src="~/p91_subgrid.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript">
        function hardrefresh(pid, flag) {
            <%If menu1.PageSource<>"navigator" then%>
            if (flag == "<%=Me.CurrentMasterPrefix%>-create") {
                parent.window.location.replace("<%=Me.CurrentMasterPrefix%>_framework.aspx?pid=" + pid);
                return;
            }
            if (flag == "<%=Me.CurrentMasterPrefix%>-delete") {
                parent.window.location.replace("<%=Me.CurrentMasterPrefix%>_framework.aspx");
                return;
            }
            <%end If%>

            location.replace("entity_framework_rec_p91.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=master.datapid%>&source=<%=menu1.PageSource%>");

        }

        function periodcombo_setting() {

            window.parent.sw_decide("periodcombo_setting.aspx", "Images/settings_32.png");
        }
        function RowSelected_p91(sender, args) {
            document.getElementById("<%=hiddatapid_p91.ClientID%>").value = args.getDataKeyValue("pid");
        }

        function RowDoubleClick_p91(sender, args) {
            <%If Master.Factory.SysUser.j04IsMenu_Invoice Then%>
            window.open("p91_framework.aspx?pid=" + document.getElementById("<%=hiddatapid_p91.ClientID%>").value, "_top")
            <%End If%>
        }

      
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <uc:entity_menu id="menu1" runat="server"></uc:entity_menu>

    <uc:p91_subgrid ID="gridP91" runat="server" x29ID="p41Project" />



  
    <asp:HiddenField ID="hiddatapid_p91" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
</asp:Content>
