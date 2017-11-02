<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p31_timer.aspx.vb" Inherits="UI.p31_timer" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="timer" Src="~/timer.ascx" %>
<%@ Register TagPrefix="uc" TagName="p31_subgrid" Src="~/p31_subgrid.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/jquery.timer.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {

            function getTime() {
                var d = new Date;
                var hours = d.getHours();
                var mins = d.getMinutes();
                var secs = d.getSeconds();

                return ("0" + hours).slice(-2) + ":" + ("0" + mins).slice(-2) + ":" + ("0" + secs).slice(-2);
                //return h.substring(h.length-1,2) + ":" +m.substring(m.length-1,2)
            }
            setInterval(function () {
                $("#clock").html(getTime())
            }, 1000);

            $("#clock").html(getTime());
        });


        function hardrefresh(pid, flag) {

            location.replace("p31_timer.aspx");
        }
        function p31_subgrid_setting(j74id) {
            sw_master("grid_designer.aspx?prefix=p31&masterprefix=j02&pid=" + j74id, "Images/griddesigner_32.png", false);

        }
        function p31_RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid_p31.clientid%>").value = args.getDataKeyValue("pid");

        }

        function p31_RowDoubleClick(sender, args) {
            record_p31_edit();
        }

        function record_p31_edit() {
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_master("p31_record.aspx?pid=" + pid, "Images/worksheet_32.png");

        }
        function p31_entry() {
            ///volá se z p31_subgrid
            sw_master("p31_record.aspx?pid=0", "Images/worksheet_32.png", false);
            return (false);
        }
        function p31_clone() {
            ///volá se z p31_subgrid
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_master("p31_record.aspx?clone=1&pid=" + pid, "Images/worksheet_32.png", false);
            return (false);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: white; padding: 10px;">
        <table cellpadding="10">
            <tr>
                <td>
                    <asp:Image ID="img1" runat="server" ImageUrl="Images/worksheet_32.png" />
                </td>
                <td>
                    <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Worksheet Stopky"></asp:Label>
                    
                </td>
                <td>
                    <span id="clock" style="float:right;"></span>
                </td>
            </tr>
        </table>



        <div class="content-box2">
            <div class="title">
                <img src="Images/stopwatch.png" alt="Stopky" />
                <asp:Label ID="lblItemsHeader" runat="server" Text="Položky"></asp:Label>
            </div>
            <div class="content">
                <uc:timer ID="timer1" runat="server"></uc:timer>
            </div>
        </div>

    </div>
    <div class="div6">
        <asp:CheckBox ID="chkShowP31Grid" runat="server" AutoPostBack="true" Text="Zobrazovat worksheet pod-přehled" />
    </div>
    <uc:p31_subgrid ID="gridP31" runat="server" EntityX29ID="j02Person" OnRowSelected="p31_RowSelected" OnRowDblClick="p31_RowDoubleClick"></uc:p31_subgrid>

    <asp:HiddenField ID="hiddatapid_p31" runat="server" />

</asp:Content>
