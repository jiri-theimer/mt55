<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j03_accesslog.aspx.vb" Inherits="UI.j03_accesslog" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function periodcombo_setting() {
            
            dialog_master("periodcombo_setting.aspx");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>
            <td>
                <img src="Images/user_32.png" alt="Uživatel" />
            </td>
            <td>
                <asp:Label ID="lblUser" CssClass="framework_header_span" runat="server"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblPeriodCaption" runat="server" Text="Časové období:"></asp:Label>
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>
            </td>
            <td>
                <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                        <asp:ListItem Text="20" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="50"></asp:ListItem>
                        <asp:ListItem Text="100"></asp:ListItem>
                    </asp:DropDownList>
            </td>
        </tr>
    </table>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>

    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

