<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_move2bin.aspx.vb" Inherits="UI.p31_move2bin" %>
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
    <div class="commandcell" >
        <asp:RadioButtonList ID="opgDirection" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" Font-Bold="true" CellPadding="10">
                    <asp:ListItem Text="Přesunout rozpracovanost do archivu" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Přesunout schválené do archivu" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Obnovit úkony z archivu" Value="2"></asp:ListItem>
                    
                </asp:RadioButtonList>
    </div>
    <div class="commandcell" style="padding-top:10px;">
        <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>
    </div>
    <div style="clear:both;"></div>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>

    <asp:HiddenField ID="hidPrefix" runat="server" />
     <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
    <asp:HiddenField ID="hidPIDs" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
