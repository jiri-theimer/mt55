<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p31_framework_detail.aspx.vb" Inherits="UI.p31_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="timesheet_calendar" Src="~/timesheet_calendar.ascx" %>
<%@ Register TagPrefix="uc" TagName="p31_subgrid" Src="~/p31_subgrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">


    <script type="text/javascript">
        $(document).ready(function () {


        });

        function sw_decide(url, iconUrl, is_maximize) {
            var isInIFrame = (window.location != window.parent.location);
            if (isInIFrame == true) {

                var w = parseInt(document.getElementById("<%=hidParentWidth.ClientID%>").value);
                var h = screen.availHeight;

                if ((w < 901 || h < 800) && w > 0) {
                    window.parent.sw_master(url, iconUrl);
                    return;
                }

                if (w < 910)
                    is_maximize = true;
            }
            sw_local(url, iconUrl, is_maximize);
        }

        function p31_entry() {
            var p41id = <%=me.p41ID.ClientID%>_get_value();

            sw_decide("p31_record.aspx?pid=0&p31date=<%=Format(Me.cal1.SelectedDate, "dd.MM.yyyy")%>&j02id=<%=Me.CurrentJ02ID%>&p41id=" + p41id, "Images/worksheet.png");

        }

        function p31_clone() {
            ///volá se z p31_subgrid
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_decide("p31_record.aspx?clone=1&pid=" + pid, "Images/worksheet.png");

        }




        function hardrefresh(pid, flag) {
            if (flag == "j74") {
                location.replace("p31_framework_detail.aspx");
                return;
            }
            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;


        }





        function p31_RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid_p31.clientid%>").value = args.getDataKeyValue("pid");

        }

        function p31_RowDoubleClick(sender, args) {
            record_p31_edit();
        }

        function record_p31_edit() {
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_decide("p31_record.aspx?pid=" + pid, "Images/worksheet.png");

        }

        function p31_subgrid_setting(j74id) {
            sw_decide("grid_designer.aspx?prefix=p31&masterprefix=j02&pid=" + j74id, "Images/griddesigner.png", true);

        }

        function j02id_onchange() {
            var j02id = document.getElementById("<%=me.j02id.clientid%>").value;

            $.post("Handler/handler_userparam.ashx", { x36value: j02id, x36key: "p31_framework_detail-j02id", oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }

                window.open("p31_framework.aspx", "_top")
            });

        }

        function p41id_onchange(sender, eventArgs) {
            //var item = eventArgs.get_item();
            p31_entry();
        }

        function report() {

            sw_decide("report_modal.aspx?prefix=j02&pid=<%=me.j02id.selectedvalue%>", "Images/reporting.png", true);

        }

        function timer_change(ctl) {
            if (ctl.checked == true)
                window.open("p31_framework.aspx?showtimer=1", "_top");
            else
                window.open("p31_framework.aspx?showtimer=0", "_top");
        }
        function grid_change(ctl) {
            if (ctl.checked == true)
                window.open("p31_framework.aspx?showgrid=1", "_top");
            else
                window.open("p31_framework.aspx?showgrid=0", "_top");
        }

        function p31_subgrid_querybuilder(j70id) {
            sw_decide("query_builder.aspx?prefix=p31&x36key=p31_subgrid-j70id&pid=" + j70id, "Images/query.png", true);

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

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="commandcell">
        <asp:Image ID="img1" runat="server" ImageUrl="Images/worksheet_32.png" />
    </div>
    <div class="commandcell" style="padding-left: 5px;">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Zapisovat úkony" meta:resourcekey="lblHeader"></asp:Label>
        <asp:DropDownList ID="j02ID" runat="server" onChange="j02id_onchange()"></asp:DropDownList>
    </div>
    <div class="commandcell" style="padding-left: 5px; padding-bottom: 1px;">
    </div>
    <div class="commandcell" style="padding-left: 5px; padding-top: 5px;">
        <asp:CheckBox ID="chkGrid" runat="server" Text="Přehled projektů/úkolů v levém panelu" AutoPostBack="false" Checked="true" onClick="grid_change(this)" meta:resourcekey="chkGrid" />
    </div>
    <div class="commandcell" style="padding-left: 5px; padding-top: 5px;" id="timer_panel">
        <asp:CheckBox ID="chkTimer" runat="server" Text="STOPKY v pravém panelu" AutoPostBack="false" Checked="true" onClick="timer_change(this)" meta:resourcekey="chkTimer" />
    </div>

    <div style="height: 10px; clear: both;"></div>

    <div style="float: left; padding-left: 6px;">
        <div>
            <uc:timesheet_calendar ID="cal1" runat="server" />
        </div>
    </div>


    <div style="float: left; padding-left: 6px;">
        <div class="content-box2">
            <div class="title">
                <asp:HyperLink ID="clue_timesheet" runat="server" CssClass="reczoom" Text="i" ToolTip="Statistika vykázaných hodin v měsíci"></asp:HyperLink>
                <asp:Label ID="StatHeader" runat="server"></asp:Label>
                <asp:HyperLink ID="cmdReport" runat="server" Text="Tisková sestava" NavigateUrl="javascript:report()" Style="float: right;" meta:resourcekey="cmdReport"></asp:HyperLink>
            </div>
            <div class="content">
                <div class="div6">
                    <asp:Label ID="lblHours_All" runat="server" meta:resourcekey="lblHours_All" Text="Vykázané hodiny:"></asp:Label>
                    <asp:Label ID="Hours_All" runat="server" CssClass="valboldblue"></asp:Label>

                    <asp:Label ID="lblHours_Billable" runat="server" Text="z toho fakturovatelné:" meta:resourcekey="lblHours_Billable"></asp:Label>
                    <asp:Label ID="Hours_Billable" runat="server" CssClass="valboldblue" ForeColor="green"></asp:Label>

                </div>
                <div class="div6">
                    <asp:Label ID="lblFond_Hours" runat="server" Text="Fond pracovní doby:" meta:resourcekey="lblFond_Hours"></asp:Label>
                    <asp:Label ID="Fond_Hours" runat="server" CssClass="valboldred"></asp:Label>

                </div>
                <div class="div6">
                    <asp:Label ID="lblUtil_Total" Text="Utilizace za všechny hodiny:" runat="server" meta:resourcekey="lblUtil_Total"></asp:Label>
                    <asp:Label ID="Util_Total" runat="server" CssClass="valboldred"></asp:Label>
                </div>
                <div class="div6">
                    <asp:Label ID="lblUtil_Billable" runat="server" Text="Utilizace za fakturovatelné hodiny:" meta:resourcekey="lblUtil_Billable"></asp:Label>
                    <asp:Label ID="Util_Billable" runat="server" CssClass="valbold" ForeColor="green"></asp:Label>
                </div>

            </div>
        </div>


        <div class="content-box2" style="margin-top: 10px;">
            <div class="title">
                <asp:Label ID="lblSearchProject" runat="server" Text="Vyhledat projekt pro nový úkon..." meta:resourcekey="lblSearchProject"></asp:Label>
            </div>
            <div class="content">
                <uc:project ID="p41ID" runat="server"  AutoPostBack="false" Flag="p31_entry" Width="330px" OnClientSelectedIndexChanged="p41id_onchange" Text="Hledat projekt..." />
            </div>
        </div>

        <div style="margin-top: 10px;">
            <button type="button" onclick="p31_entry()" id="cmdNew" runat="server">
                Nový úkon
            </button>

        </div>
    </div>



    <div style="clear: both; width: 100%;"></div>
    
    <uc:p31_subgrid ID="gridP31" runat="server" EntityX29ID="j02Person" OnRowSelected="p31_RowSelected" OnRowDblClick="p31_RowDoubleClick" AllowFullScreen="false"></uc:p31_subgrid>


    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
    <asp:HiddenField ID="hidParentWidth" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
