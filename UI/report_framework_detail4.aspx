<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Subform.Master" CodeBehind="report_framework_detail4.aspx.vb" Inherits="UI.report_framework_detail4" %>
<%@ MasterType VirtualPath="~/Subform.Master" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function x31_record() {
            sw_local("x31_record.aspx?pid=<%=me.CurrentX31ID%>", "Images/settings_32.png")
        }

        function hardrefresh(pid, flag) {
            location.replace("report_framework_detail4.aspx?x31id=<%=me.CurrentX31ID%>");
        }
        function querybuilder() {
            var j70id = "<%=Me.CurrentJ70ID%>";
            sw_local("query_builder.aspx?prefix=<%=me.hidQueryPrefix.value%>&pid=" + j70id, "Images/query_32.png");
            return (false);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                <asp:Image ID="img1" runat="server" ImageUrl="Images/xls.png" />
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="250px"></uc:periodcombo>
            </td>
          
            <td>
                <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span"></asp:Label>
                <asp:HyperLink ID="clue_query" runat="server" CssClass="reczoom" ToolTip="Detail filtru" Text="i"></asp:HyperLink>
                <asp:DropDownList ID="j70ID" runat="server" AutoPostBack="true" DataTextField="NameWithMark" DataValueField="pid" Style="width: 150px;" ToolTip="Pojmenovaný filtr"></asp:DropDownList>
                <asp:ImageButton ID="cmdQuery" runat="server" OnClientClick="return querybuilder()" ImageUrl="Images/query.png" ToolTip="Návrhář filtrů" CssClass="button-link" />
            </td>
            <td>
                <asp:HyperLink ID="cmdSetting" runat="server" Text="Nastavení šablony" NavigateUrl="javascript:x31_record()"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:Button ID="cmdGenerate" runat="server" Text="Vygenerovat XLSX soubor" CssClass="cmd" Font-Bold="true" />
    </div>
    <div class="div6" style="margin-top:30px;">
        <asp:Button ID="cmdGenerateCSV" runat="server" Text="Vygenerovat CSV soubor" CssClass="cmd" Font-Bold="true" />
        <span>Oddělovač polí v CSV souboru:</span>
        <asp:DropDownList ID="cbxDelimiter" runat="server">
            <asp:ListItem text="," Value=","></asp:ListItem>
            <asp:ListItem text=";" Value=";"></asp:ListItem>
            <asp:ListItem text="|" Value="|" Selected="true"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <asp:HiddenField ID="hidCurX31ID" runat="server" />
    <asp:HiddenField ID="hidQueryPrefix" runat="server" />
</asp:Content>
