<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="entity_framework_rec_o23.aspx.vb" Inherits="UI.entity_framework_rec_o23" %>
<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_subgrid" Src="~/o23_subgrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function hardrefresh(pid, flag) {

            if (flag == "<%=Me.CurrentMasterPrefix%>-create") {
                parent.window.location.replace("<%=Me.CurrentMasterPrefix%>_framework.aspx?pid=" + pid);
                return;
            }
            if (flag == "<%=Me.CurrentMasterPrefix%>-delete") {
                parent.window.location.replace("<%=Me.CurrentMasterPrefix%>_framework.aspx");
                return;
            }


            location.replace("entity_framework_rec_o23.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=master.datapid%>&source=<%=menu1.PageSource%>");

        }

       
        function RowSelected_o23(sender, args) {
            document.getElementById("<%=hiddatapid_o23.ClientID%>").value = args.getDataKeyValue("pid");
        }

        function RowDoubleClick_o23(sender, args) {
            var pid = document.getElementById("<%=hiddatapid_o23.ClientID%>").value;
            parent.window.location.replace("o23_framework.aspx?pid=" + pid);
        }



        function o23_record(pid, bolReturnFalse) {
            sw_decide("o23_record.aspx?masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=Master.DataPID%>&pid=" + pid, "Images/notepad.png", true);
            if (bolReturnFalse == true)
                return (false);

        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



    <uc:entity_menu id="menu1" runat="server"></uc:entity_menu>

    <uc:o23_subgrid ID="gridO23" runat="server" />


    <asp:HiddenField ID="hiddatapid_o23" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    
</asp:Content>
