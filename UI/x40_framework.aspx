<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="x40_framework.aspx.vb" Inherits="UI.x40_framework" %>

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
            sw_master("x40_record.aspx?pid=" + pid, "Images/email.png");

        }

        function sendmail() {
            sw_master("sendmail.aspx?prefix=<%=me.CurrentMasterPrefix%>&pid=<%=me.CurrentMasterPID%>", "Images/email.png")


        }

        function RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            record_detail();
        }

        function batch(status) {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Musíte vybrat alespoň jednu zprávu v přehledu.");
                return;                
            }
            
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = status;
            document.getElementById("<%=me.hidHardRefreshPID.ClientID%>").value=pids;
            document.getElementById("<%=Me.cmdBatch.ClientID%>").click();
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
            //location.replace("x40_framework.aspx")

        }
        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx", "Images/settings.png");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="height: 44px; background-color: white; border-bottom: solid 1px silver">
        <div style="float: left;">
            <img src="Images/email_32.png" alt="Odeslaná pošta" />
        </div>
        <div class="commandcell" style="padding-left: 10px;">
            <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Text="OUTBOX" Style="vertical-align: top;"></asp:Label>
        </div>
        <div class="commandcell" style="padding-left:10px;">
            <uc:periodcombo ID="period1" runat="server" Width="170px"></uc:periodcombo>
        </div>
        <div class="commandcell" style="padding-left:10px;">
            <asp:DropDownList ID="cbxQueryStatus" runat="server" AutoPostBack="true">
                <asp:ListItem Text="--Status zprávy--" Value=""></asp:ListItem>
                <asp:ListItem Text="Odesílá se" Value="1"></asp:ListItem>
                <asp:ListItem Text="Odesláno" Value="3"></asp:ListItem>
                <asp:ListItem Text="Čeká na odeslání" Value="5"></asp:ListItem>
                <asp:ListItem Text="Chyba" Value="2"></asp:ListItem>
                <asp:ListItem Text="Zastaveno" Value="4"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="commandcell" style="padding-left:10px;">
            <asp:DropDownList ID="cbxQueryEntity" runat="server" AutoPostBack="true" style="width:150px;">
                <asp:ListItem Text="--Kontext zprávy--" Value=""></asp:ListItem>
                <asp:ListItem Text="Vystavená faktura" Value="391"></asp:ListItem>
                <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                <asp:ListItem Text="Bez-kontextová tisková sestava" Value="931"></asp:ListItem>
                <asp:ListItem Text="Osobní" Value="102"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="commandcell" style="padding-left: 10px;">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" runat="server" Style="z-index: 2000;" ExpandAnimation-Duration="0" ExpandAnimation-Type="none" ClickToOpen="true">
                <Items>
                    <telerik:RadMenuItem Text="Nová zpráva" Value="new" PostBack="false" ImageUrl="Images/new.png" NavigateUrl="javascript:sendmail()"></telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Vybrané (zaškrtlé)" ImageUrl="Images/menuarrow.png">
                        <Items>
                            <telerik:RadMenuItem Text="Označené změnit na [Odeslat]" Value="odeslat" NavigateUrl="javascript:batch(1)"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Označené změnit na [Čeká na odeslání]" Value="confirm" NavigateUrl="javascript:batch(5)"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Označené změnit na [Zastaveno]" Value="stop" NavigateUrl="javascript:batch(4)"></telerik:RadMenuItem>
                            
                            
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Obnovit" Value="refresh" NavigateUrl="x40_framework.aspx" ImageUrl="Images/refresh.png"></telerik:RadMenuItem>

                    <telerik:RadMenuItem Text="Další" ImageUrl="Images/menuarrow.png" Value="columns" PostBack="false">
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
