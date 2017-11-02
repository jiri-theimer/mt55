<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="timesheet_calendar" Src="~/timesheet_calendar.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            
        End If
    End Sub
    
    
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
       
    End Sub

    
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
      


        function hardrefresh(pid, flag) {
            if (flag == "j03_myprofile") {
                window.parent.open("default.aspx", "_self");
            }

            if (flag == "p31-save" || flag == "p31-delete") {
                form1.submit();
            }


        }

   

        function p31_entry(p41id) {

            sw_local("../p31_record.aspx?pid=0&p41id=" + p41id, "Images/worksheet_32.png");

        }
        function p31_edit(pid) {

            sw_local("../p31_record.aspx?pid=" + pid, "Images/worksheet_32.png");

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="../Images/plugin_32.png" />
            </td>
            <td>
                <span class="page_header_span">Havel Holasek | Implementace MARKTIME 5.0</span>
            </td>
           
        </tr>
    </table>

   

    <asp:HiddenField ID="hidLastSelectedDate" runat="server" />
</asp:Content>
