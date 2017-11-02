<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_recalc.aspx.vb" Inherits="UI.p31_recalc" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function periodcombo_setting() {

            dialog_master("periodcombo_setting.aspx");
        }



        function hardrefresh(pid, flag) {


            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                <asp:Label ID="lblP51" runat="server" CssClass="lbl" Text="Fakturační ceník:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="p51Name" runat="server" CssClass="valbold" Text="???"></asp:Label>
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>
            </td>
            <td>
                <asp:CheckBox ID="chkIncludeBin" runat="server" Text="Zobrazovat i úkony přesunuté do archivu" AutoPostBack="true" />
            </td>
        </tr>
    </table>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>

    <asp:HiddenField ID="hidPrefix" runat="server" />
     <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

