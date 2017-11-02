<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_grid.aspx.vb" Inherits="UI.mobile_grid" %>

<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function RowSelected(sender, args) {
            var pid = args.getDataKeyValue("pid");
            document.getElementById("<%=hiddatapid.clientid%>").value = pid;

        }

        function RowDoubleClick(sender, args) {
            //nic
        }
        function re(pid) {
            location.replace("mobile_<%=Me.CurrentPrefix%>_framework.aspx?source=mobile_grid&pid=" + pid);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <uc:mygrid id="designer1" runat="server" masterprefix="mobile_grid" AllowSettingButton="false" ModeFlag="2"></uc:mygrid>
    <uc:periodcombo ID="period1" runat="server" Width="100%"></uc:periodcombo>
    <div>
        <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
            <asp:ListItem Text="10" Selected="true"></asp:ListItem>
            <asp:ListItem Text="20"></asp:ListItem>
            <asp:ListItem Text="50"></asp:ListItem>
            <asp:ListItem Text="100"></asp:ListItem>
        </asp:DropDownList>
        <img src="Images/sum.png" />
        <asp:Label ID="lblRowsCount" runat="server"></asp:Label>
        <asp:HyperLink ID="MasterRecord" runat="server" Visible="false" CssClass="alinked"></asp:HyperLink>

    </div>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" Skin="Default"></uc:datagrid>



    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidDefaultSorting" runat="server" />
    <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
    <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />
    <asp:HiddenField ID="hidFooterSum" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
    <asp:HiddenField ID="hidCols" runat="server" />
    <asp:HiddenField ID="hidFirstLinkCol" runat="server" />
    <asp:HiddenField ID="hidAdditionalFrom" runat="server" />
    
    <asp:HiddenField ID="hidClosedQueryValue" runat="server" />
    
</asp:Content>
