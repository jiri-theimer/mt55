<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="entity_framework_rec_p31.aspx.vb" Inherits="UI.entity_framework_rec_p31" %>
<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="p31_subgrid" Src="~/p31_subgrid.ascx" %>

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
            if (flag == "p31-save") {
                document.getElementById("<%=hidHardRefreshPID.ClientID%>").value = pid;
                document.getElementById("<%=hidHardRefreshFlag.ClientID%>").value = flag;
                document.getElementById('<%= cmdRefresh.ClientID%>').click();
                return;
            }
            if (flag == "p91-create") {
                window.open("p91_framework.aspx?pid=" + pid,"_top");
                return;
            }

            location.replace("entity_framework_rec_p31.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=master.datapid%>&source=<%=menu1.PageSource%>&tab=<%=menu1.CurrentTab%>");

        }

        function p31_RowSelected(sender, args) {
            ///volá se z p31_subgrid
            document.getElementById("<%=hiddatapid_p31.clientid%>").value = args.getDataKeyValue("pid");

        }

        function p31_RowDoubleClick(sender, args) {
            ///volá se z p31_subgrid
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;

            sw_decide("p31_record.aspx?pid=" + pid, "Images/worksheet.png", false);

        }
        function p31_clone() {
            ///volá se z p31_subgrid
            //var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Musíte vybrat záznam")
                return;
            }
            sw_decide("p31_record.aspx?clone=1&pid=" + pids, "Images/worksheet.png", false);

        }
        function p31_split() {
            ///volá se z p31_subgrid
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            if (pid == "") {
                alert("Musíte vybrat záznam")
                return;
            }
            sw_decide("p31_record_split.aspx?pid=" + pid, "Images/split.png", false);

        }
        function p31_entry() {
            ///volá se z p31_subgrid
            <%If Me.CurrentMasterPrefix="p41" then%>
            var url = "p31_record.aspx?pid=0&p41id=<%=Master.DataPID%>";
            <%end If%>
            <%If Me.CurrentMasterPrefix = "p56" Then%>
            var url = "p31_record.aspx?pid=0&p56id=<%=Master.DataPID%>";
            <%end If%>
            <%If Me.CurrentMasterPrefix = "p28" Then%>
            var url = "p31_record.aspx?pid=0&p28id=<%=Master.DataPID%>";
            <%end If%>
             <%If Me.CurrentMasterPrefix = "j02" Then%>
            var url = "p31_record.aspx?pid=0&j02id=<%=Master.DataPID%>";
            <%end If%>
            url = url + "&tabqueryflag=<%=gridP31.MasterTabAutoQueryFlag%>";

            sw_decide(url, "Images/worksheet.png", false);

        }
    
        function p31_subgrid_approving(pids) {
            try {
                window.parent.sw_master("p31_approving_step2.aspx?pids=" + pids, "Images/approve_32.png", true);
            }
            catch (err) {
                window.sw_master("p31_approving_step2.aspx?pids=" + pids, "Images/approve_32.png", true);
            }
        }
       
        function p31_subgrid_periodcombo() {
            sw_decide("periodcombo_setting.aspx", "Images/settings_32.png");
        }
        
        function p31_subgrid_x18query() {            
            sw_decide("x18_querybuilder.aspx?key=p31grid&prefix=p31", "Images/query.png", true);
        }
               
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <uc:entity_menu id="menu1" runat="server"></uc:entity_menu>


    <uc:p31_subgrid ID="gridP31" runat="server" AllowMultiSelect="true"></uc:p31_subgrid>


    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
        <asp:HiddenField ID="hidMasterPrefix" runat="server" />

    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />

    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
