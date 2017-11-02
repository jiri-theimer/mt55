<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Subform.Master" CodeBehind="report_framework_detail1.aspx.vb" Inherits="UI.report_framework_detail1" %>

<%@ MasterType VirtualPath="~/Subform.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=11.0.17.406, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            <%If Me.divReportViewer.Visible Then%>
            var h1 = new Number;
            var h2 = new Number;
            var hh = new Number;

            h1 = $(window).height();

            ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            <%If LCase(Request.Browser.Browser) = "ie" Then%>
            hh = h1 - h2 - 4;
            <%Else%>
            hh = h1 - h2 - 2;
            <%End If%>

            self.document.getElementById("<%=Me.divReportViewer.ClientID%>").style.height = hh + "px";
            <%End If%>
        })

        $(window).load(function () {
           <%If hidOutputFullPathPdf.Value <> "" Then%>
            var s = document.getElementById("<%=hidOutputFullPathPdf.ClientID%>").value;
            document.getElementById("<%=hidOutputFullPathPdf.ClientID%>").value = "";

            sw_local("binaryfile.aspx?disposition=inline&tempfile=" + s, true);

            <%End If%>

            try {
                //hackování reportviewer - přednastavení pdf exportu jako default
                $('[id*="ReportToolbar_ExportGr_FormatList_DropDownList"]')[0].selectedIndex = 1;

                document.getElementById("MainContent_rv1_ReportToolbar_ExportGr_Export").className = "ActiveLink";

                ReportTextButton('MainContent_rv1_ReportToolbar_ExportGr_Export', 'Export', false, 'ActiveLink', 'DisabledLink');
            }
            catch (err) {
                //nic
            }

        })

        function periodcombo_setting() {
            sw_local("periodcombo_setting.aspx", "Images/settings_32.png")
        }

        function hardrefresh(pid, flag) {


            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }

        function sendbymail() {
            sw_local("sendmail.aspx?prefix=x31&pid=<%=me.CurrentX31ID%>&x31id=<%=me.CurrentX31ID%>&datfrom=<%=Format(period1.DateFrom,"dd.MM.yyyy")%>&datuntil=<%=Format(period1.DateUntil,"dd.MM.yyyy")%>", "Images/email_32.png")
        }

        function x31_record() {
            sw_local("x31_record.aspx?pid=<%=me.CurrentX31ID%>", "Images/settings_32.png");

        }

        function querybuilder() {
            var j70id = "<%=Me.CurrentJ70ID%>";
            sw_local("query_builder.aspx?prefix=<%=me.hidQueryPrefix.value%>&pid=" + j70id, "Images/query_32.png");
            return (false);
        }

        function rvprint() {
            <%=rv1.ClientID%>.PrintReport(); 
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: #F1F1F1;border-bottom:solid 1px silver;">

        <div class="commandcell">
            <asp:Image ID="img1" runat="server" ImageUrl="Images/report_32.png" />
        </div>

        <div class="commandcell" style="padding-left: 10px;">
            <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span"></asp:Label>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:HyperLink ID="linkPrint" runat="server" NavigateUrl="javascript:rvprint()" Text="Tisk"></asp:HyperLink>
                        
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:LinkButton ID="cmdPdfExport" runat="server" Text="PDF náhled"></asp:LinkButton>
                    </td>
                    <td style="padding-left: 10px;">
                        <asp:HyperLink ID="linkMail" runat="server" Text="Odeslat sestavu poštou jako PDF" NavigateUrl="javascript:sendbymail()"></asp:HyperLink>
                        
                    </td>

                    <td style="padding-left: 10px;">
                        <uc:periodcombo ID="period1" runat="server" Width="250px"></uc:periodcombo>
                    </td>
                    <td style="padding: 10px;">
                        <asp:HyperLink ID="clue_query" runat="server" CssClass="reczoom" ToolTip="Detail filtru" Text="i" Visible="false"></asp:HyperLink>
                        <asp:DropDownList ID="j70ID" runat="server" AutoPostBack="true" DataTextField="NameWithMark" DataValueField="pid" Style="width: 150px;" ToolTip="Pojmenovaný filtr" Visible="false"></asp:DropDownList>
                        <asp:ImageButton ID="cmdQuery" runat="server" OnClientClick="return querybuilder()" ImageUrl="Images/query.png" ToolTip="Návrhář filtrů" CssClass="button-link" Visible="false" />

                        <asp:HyperLink ID="cmdSetting" runat="server" Text="Nastavení šablony" NavigateUrl="javascript:x31_record()"></asp:HyperLink>
                    </td>
                </tr>



            </table>
        </div>

        <div style="clear: both;"></div>
        <div id="offsetY"></div>
    </div>
    <asp:Panel ID="divReportViewer" runat="server">
        <telerik:ReportViewer ID="rv1" runat="server" Width="100%" Height="100%" ShowParametersButton="true" ShowHistoryButtons="false" ValidateRequestMode="Disabled">            
            <Resources PrintToolTip="Tisk" ExportSelectFormatText="Exportovat do zvoleného formátu" NextPageToolTip="Další strana" PreviousPageToolTip="Předchozí strana" RefreshToolTip="Obnovit" LastPageToolTip="Poslední strana" FirstPageToolTip="První strana" TogglePageLayoutToolTip="Přepnout na náhled k tisku"></Resources>
        </telerik:ReportViewer>

    </asp:Panel>
    <asp:Panel ID="panFirstRun" runat="server" Visible="false" Style="padding: 100px;">
        <asp:Button ID="cmdRunReport" runat="server" CssClass="cmd" Text="Vygenerovat náhled sestavy podle filtru" Font-Size="Large" />
    </asp:Panel>
    <asp:HiddenField ID="flag" runat="server" />
    <asp:HiddenField ID="hidCurX31ID" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidQueryPrefix" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidOutputFullPathPdf" runat="server" />

</asp:Content>
