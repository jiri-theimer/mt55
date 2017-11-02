<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o23_queue.aspx.vb" Inherits="UI.o23_queue" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        function periodcombo_setting() {

            dialog_master("periodcombo_setting.aspx", false);
        }

        function hardrefresh(pid, flag) {

            location.replace("o23_queue.aspx?masterpid=<%=Me.CurrentMasterPID%>&masterprefix=<%=me.CurrentMasterPrefix%>");

        }

        function file_preview(url) {
            ///náhled na soubor
            sw_local(url, "Images/attachment_32.png", true);

        }

        function RowSelected(sender, args) {
            var pid = args.getDataKeyValue("pid");
            document.getElementById("<%=hiddatapid.clientid%>").value = pid;


        }

        function RowDoubleClick(sender, args) {
            SelectRecordAndClose();
        }

        function SelectRecordAndClose() {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "") {
                alert("Musíte vybrat záznam dokumentu.")
                return;
            }
            window.parent.hardrefresh(pid, "o23-queue");
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <asp:panel ID="panMasterRecord" CssClass="content-box2" Visible="false">
        <div class="title">
            <asp:Label ID="EntityName" runat="server"></asp:Label>
        </div>
        <div class="content">
            <asp:Label ID="EntityRecord" runat="server" CssClass="valboldblue"></asp:Label>
        </div>
    </asp:panel>
    <div class="commandcell">
        <asp:Image ID="img1" runat="server" ImageUrl="Images/notepad_32.png" />
    </div>
    <div class="commandcell">
        <asp:Label ID="x18Name" runat="server" CssClass="page_header_span"></asp:Label>
    </div>
    <div class="commandcell" style="padding-left:20px;">
        <asp:Label ID="lblVirtualCount" runat="server" ToolTip="Počet záznamů v aktuálním přehledu" CssClass="page_header_span"></asp:Label>
    </div>
    <div class="commandcell" style="padding-left:20px;">
        <asp:DropDownList ID="cbxMode" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Spárované i nespárované" Value="3"></asp:ListItem>
            <asp:ListItem Text="Pouze nespárované dokumenty" Value="1"></asp:ListItem>
            <asp:ListItem Text="Pouze spárované dokumenty" Value="2"></asp:ListItem>            
        </asp:DropDownList>
    </div>
    <div class="commandcell" style="padding-left:20px;">
        <asp:DropDownList ID="cbxPeriodType" AutoPostBack="true" runat="server" ToolTip="Druh filtrovaného období">
        </asp:DropDownList>
        <uc:periodcombo ID="period1" runat="server" Width="160px" Visible="true"></uc:periodcombo>
    </div>

    <asp:Panel ID="panWorkflow" runat="server" CssClass="commandcell" style="padding-left:20px;">
        <div>
            <asp:DropDownList ID="cbxQueryB02ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="b02Name" Style="width: 180px;" ToolTip="Filtrovat podle aktuálního workflow stavu dokumentu"></asp:DropDownList>
        </div>

    </asp:Panel>

    <div style="clear: both;"></div>

    <div style="float: left; padding-left: 6px;">
        <asp:Label ID="CurrentPeriodQuery" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div style="float: left; padding-left: 6px;">

        <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div style="float: left; padding-left: 6px;">
        <asp:LinkButton ID="cmdCĺearFilter" runat="server" Text="<img src='Images/sweep.png'/>Vyčistit sloupcový filtr" Style="font-weight: bold; color: red;" Visible="false"></asp:LinkButton>
    </div>
    <div style="clear: both; width: 100%;"></div>

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick" Skin="Default"></uc:datagrid>



    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidX20ID" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidB01ID" runat="server" />
    <asp:HiddenField ID="hidx18GridColsFlag" runat="server" />
    <asp:HiddenField ID="hidCols" runat="server" />
    <asp:HiddenField ID="hidX23ID" runat="server" />
    <asp:HiddenField ID="hidDefaultSorting" runat="server" />
    <asp:Button ID="cmdDoubleClick" runat="server" style="display:none;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
