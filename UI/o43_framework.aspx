<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="o43_framework.aspx.vb" Inherits="UI.o43_framework" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function record_detail() {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_master("o43_record.aspx?pid=" + pid, "Images/imap.png");

        }

        

        function RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            //nic
            //record_detail();
        }

        

        function GetAllSelectedPIDs() {

            var masterTable = $find("<%=grid1.radGridOrig.ClientID%>").get_masterTableView();
            var sel = masterTable.get_selectedItems();
            var pids = "";

            for (i = 0; i < sel.length; i++) {
                if (pids == "")
                    pids = sel[i].getDataKeyValue("pid");
                else
                    pids = pids + "," + sel[i].getDataKeyValue("pid");
            }

            return (pids);
        }

        function hardrefresh(pid, flag) {

            document.getElementById("<%=me.cmdRefresh.clientid%>").click();
           

        }
        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx", "Images/settings.png");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="height: 44px; background-color: white; border-bottom: solid 1px silver">
        <div style="float: left;">
            <img src="Images/imap_32.png" alt="Přijatá pošta" />
        </div>
        <div class="commandcell" style="padding-left: 10px;">
            <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Text="INBOX" Style="vertical-align: top;"></asp:Label>
        </div>
        <div class="commandcell" style="padding-left:10px;">
            <uc:periodcombo ID="period1" runat="server" Width="170px"></uc:periodcombo>
        </div>
        
        <div class="commandcell" style="padding-left:10px;">
            <asp:DropDownList ID="cbxQueryEntity" runat="server" AutoPostBack="true" style="width:150px;">
                <asp:ListItem Text="--Vazba zprávy--" Value=""></asp:ListItem>                
                <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                <asp:ListItem Text="Bez vazby" Value="931"></asp:ListItem>                
            </asp:DropDownList>
        </div>
        <div class="commandcell" style="padding-left: 10px;">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" runat="server" Style="z-index: 2000;" ExpandAnimation-Duration="0" ExpandAnimation-Type="none" ClickToOpen="true">
                <Items>
                    <telerik:RadMenuItem Text="ZÁZNAM" ImageUrl="Images/menuarrow.png" Visible="false">
                    <Items>
                        <telerik:RadMenuItem Text="Detail zprávy (dvoj-klik)" Value="record" PostBack="false" ImageUrl="Images/edit.png" NavigateUrl="javascript:record_detail()"></telerik:RadMenuItem>
                        
                    </Items>
                    </telerik:RadMenuItem>
                    

                    <telerik:RadMenuItem Text="DALŠÍ" ImageUrl="Images/menuarrow.png" Value="columns" PostBack="false">
                        <ContentTemplate>
                            <div style="padding: 20px;">

                                <div class="div6">
                                    <span>Stránkování:</span>
                                    <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování" TabIndex="3">
                                        <asp:ListItem Text="20"></asp:ListItem>
                                        <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="100"></asp:ListItem>
                                        <asp:ListItem Text="200"></asp:ListItem>
                                        <asp:ListItem Text="500"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>

        </div>
    </div>
    <div style="clear:both;"></div>
    <asp:HyperLink ID="linkEntity" runat="server" Visible="false"></asp:HyperLink>
    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick" AllowFilteringByColumn="true"></uc:datagrid>


    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />

    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />

    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
    <asp:Button ID="cmdBatch" runat="server" Style="display: none;" />
</asp:Content>
