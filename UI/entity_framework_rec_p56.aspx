<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="entity_framework_rec_p56.aspx.vb" Inherits="UI.entity_framework_rec_p56" %>
<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="p56_subgrid" Src="~/p56_subgrid.ascx" %>

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
            <%End If%>

            location.replace("entity_framework_rec_p56.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=master.datapid%>&source=<%=menu1.PageSource%>");

        }

        function p56_subgrid_setting(j74id) {
            ///volá se z p56_subgrid
            sw_decide("grid_designer.aspx?prefix=p56&masterprefix=<%=Me.CurrentMasterPrefix%>&pid=" + j74id, "Images/griddesigner.png", true);
        }
        function RowSelected_p56(sender, args) {
            document.getElementById("<%=hiddatapid_p56.ClientID%>").value = args.getDataKeyValue("pid");
        }

        function RowDoubleClick_p56(sender, args) {
            var pid = document.getElementById("<%=hiddatapid_p56.ClientID%>").value;
            parent.window.location.replace("p56_framework.aspx?pid=" + pid);
            //sw_decide("p56_record.aspx?pid=" + pid, "Images/task.png", false);
        }
        function p56_subgrid_approving(pids) {
            window.parent.sw_master("p31_approving_step1.aspx?masterpid=<%=Master.DataPID%>&masterprefix=p41>&prefix=p56&pid=" + pids, "Images/approve_32.png", true);


        }

       
        function p56_record(pid, bolReturnFalse) {            
            if (pid == null)
                pid = "0";            
            sw_decide("p56_record.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=Master.DataPID%>&pid=" + pid, "Images/task.png", true);
            if (bolReturnFalse == true)
                return (false)

        }
        
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <uc:entity_menu id="menu1" runat="server"></uc:entity_menu>

    <uc:p56_subgrid ID="gridP56" runat="server" x29ID="p41Project" />


    
    <asp:HiddenField ID="hiddatapid_p56" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
</asp:Content>
