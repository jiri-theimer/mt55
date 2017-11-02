<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p90_framework.aspx.vb" Inherits="UI.p90_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function record_edit() {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_master("p90_record.aspx?pid=" + pid, "Images/proforma.png",true);

        }

        function record_new() {
            sw_master("p90_record.aspx?pid=0", "Images/proforma.png",true);


        }

        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx", "Images/settings.png");
        }


        function RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            record_edit();
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


            location.replace("p90_framework.aspx")

        }

        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">




    <div style="height: 44px; background-color: white; border-bottom: solid 1px silver">
        <div style="float: left;">
            <img src="Images/proforma_32.png" alt="Zálohové faktury" />
        </div>
        <div class="commandcell" style="padding-left: 10px;">
            <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Text="Zálohové faktury" Style="vertical-align: top;"></asp:Label>

        </div>
        <div class="commandcell" style="padding-left:10px;">
            <uc:periodcombo ID="period1" runat="server" Width="170px"></uc:periodcombo>
        </div>

        <div class="commandcell" style="padding-left: 50px;">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" runat="server" Style="z-index: 3000;" ExpandAnimation-Duration="0" ExpandAnimation-Type="none" ClickToOpen="true">
                <Items>
                    <telerik:RadMenuItem Value="cmdNew" Text="Nový" NavigateUrl="javascript:record_new();" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>
                    

                    <telerik:RadMenuItem Text="Obnovit" Visible="false" ImageUrl="Images/refresh.png" Value="refresh" NavigateUrl="p90_framework.aspx"></telerik:RadMenuItem>

                    <telerik:RadMenuItem Text="DALŠÍ" ImageUrl="Images/menuarrow.png" Value="columns" PostBack="false">
                        <ContentTemplate>
                            <div style="padding: 20px;">
                                <div class="div6">
                                    <asp:DropDownList ID="cbxValidity" runat="server" AutoPostBack="true">
                                        <asp:ListItem Text="Zobrazovat otevřené i přesunuté do archivu" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Zobrazovat pouze otevřené (mimo archiv)" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Zobrazovat pouze přesunuté do archivu" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

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
    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick" AllowFilteringByColumn="true"></uc:datagrid>


    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
