<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p31_subgrid.ascx.vb" Inherits="UI.p31_subgrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<asp:Panel runat="server" ID="panCommand" CssClass="div6">
    <div class="commandcell">
        <img src="Images/worksheet.png" alt="Worksheet" />
        <asp:Label ID="lblHeaderP31" CssClass="framework_header_span" runat="server" Text=""></asp:Label>

    </div>

    <div class="commandcell" style="margin-left: 10px; margin-right: 10px;">
        <uc:periodcombo ID="period1" runat="server" Width="150px"></uc:periodcombo>
        <asp:Label ID="ExplicitPeriod" runat="server" CssClass="valboldblue"></asp:Label>

        <asp:ImageButton ID="cmdClearExplicitPeriod" runat="server" ImageUrl="Images/close.png" ToolTip="Zrušit filtr podle kalendáře" CssClass="button-link" />
    </div>
    <div class="commandcell" id="divQueryContainer">
        <uc:mygrid ID="designer1" runat="server" Prefix="p31" MasterPrefixFlag="1"></uc:mygrid>

    </div>


    <div class="commandcell">
        <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" ClickToOpen="true" Style="z-index: 2000;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None" CollapseAnimation-Duration="0" CollapseAnimation-Type="None">
            <Items>
                <telerik:RadMenuItem Text="Nový" Value="new" NavigateUrl="javascript:p31_entry()" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Vybrané (zaškrtlé)" Value="akce" ImageUrl="Images/menuarrow.png">
                    <Items>
                        <telerik:RadMenuItem Text="Kopírovat" Value="clone" NavigateUrl="javascript:p31_clone()"></telerik:RadMenuItem>                        

                        <telerik:RadMenuItem Value="cmdApprove" Text="[SCHVÁLIT/PŘE-SCHVÁLIT]">
                                <Items>
                                    <telerik:RadMenuItem Value="cmdApproveDialog" NavigateUrl="javascript:approving();" Text="Schvalovací dialog" ImageUrl="Images/approve.png"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Fakturovat" ImageUrl="Images/a14.gif" NavigateUrl="javascript:batch_approve(4);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Zahrnout do paušálu" ImageUrl="Images/a16.gif" NavigateUrl="javascript:batch_approve(6);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Viditelný odpis" ImageUrl="Images/a12.gif" NavigateUrl="javascript:batch_approve(2);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Skrytý odpis" ImageUrl="Images/a13.gif" NavigateUrl="javascript:batch_approve(3);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Vyčistit schvalování" ImageUrl="Images/clear.png" NavigateUrl="javascript:batch_approve(0);"></telerik:RadMenuItem>
                                </Items>
                            </telerik:RadMenuItem>
                        <telerik:RadMenuItem Value="cmdInvoice" Text="[FAKTUROVAT]">
                                <Items>
                                    <telerik:RadMenuItem Text="Fakturovat schválené (nová faktura)" NavigateUrl="javascript:batch_invoice(false);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Přidat schválené do existující faktury" NavigateUrl="javascript:batch_invoice(true);"></telerik:RadMenuItem>
                                </Items>
                            </telerik:RadMenuItem>
                        <telerik:RadMenuItem Value="cmdMove" Text="Přesunout na jiný projekt" NavigateUrl="javascript:move2project_p31ids();" Visible="false"></telerik:RadMenuItem>
                        <telerik:RadMenuItem Value="cmdSummary" Text="Statistiky" NavigateUrl="javascript:drilldown_p31ids();"></telerik:RadMenuItem>

                    </Items>
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Další" ImageUrl="Images/menuarrow.png">

                    <ContentTemplate>

                        <div class="content-box3">
                            <div class="title">
                                <img src="Images/query.png" />
                                <span>Dodatečné filtrování záznamů</span>
                            </div>
                            <div class="content">
                                <div class="div6">
                                    <button type="button" onclick="p31_subgrid_x18query()">
                                        <img src="Images/label.png" />Štítky</button>
                                    <asp:ImageButton ID="cmdClearX18" runat="server" ToolTip="Vyčistit štítkovací filtr" ImageUrl="Images/delete.png" Visible="false" CssClass="button-link" />
                                    <asp:Label ID="x18_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                </div>

                            </div>
                        </div>
                        <asp:Panel ID="panExport" runat="server" CssClass="content-box3">
                            <div class="title">
                                <img src="Images/export.png" />
                                <span>Export záznamů aktuálního přehledu</span>
                            </div>
                            <div class="content">
                                <asp:Button ID="cmdExport" runat="server" Text="Export" ToolTip="Export do MS EXCEL tabulky, plný počet záznamů" CssClass="cmd" />
                                <asp:Button ID="cmdXLS" runat="server" Text="XLS" ToolTip="Export do XLS vč. souhrnů s omezovačem na maximálně 2000 záznamů" CssClass="cmd" />
                                <asp:Button ID="cmdPDF" runat="server" Text="PDF" CssClass="cmd" ToolTip="Export do PDF vč. souhrnů s omezovačem na maximálně 2000 záznamů" />
                                <asp:Button ID="cmdDOC" runat="server" Text="DOC" ToolTip="Export do DOC vč. souhrnů s omezovačem na maximálně 2000 záznamů" CssClass="cmd" />
                            </div>



                        </asp:Panel>

                        <div class="content-box3">
                            <div class="title">
                                <img src="Images/griddesigner.png" />Nastavení přehledu
                            </div>
                            <div class="content">


                                <asp:Panel ID="panGroupBy" runat="server" CssClass="div6">
                                    <span><%=Resources.common.DatoveSouhrny%>:</span>
                                    <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true">
                                        <asp:ListItem Text="<%$Resources:common,BezSouhrnu%>" Value=""></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Sesit%>" Value="p34Name"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Aktivita%>" Value="p32Name"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Osoba%>" Value="Person"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Klient%>" Value="ClientName"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Projekt %>" Value="p41Name"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Faktura%>" Value="p91Code"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Ukol%>" Value="p56Name"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,Schvalovani%>" Value="p71Name"></asp:ListItem>
                                        <asp:ListItem Text="<%$Resources:common,FaktStatus%>" Value="p70Name"></asp:ListItem>
                                        <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CheckBox ID="chkGroupsAutoExpanded" runat="server" Text="Auto-rozbalené souhrny" AutoPostBack="true" Checked="true" />

                                </asp:Panel>

                                <div class="div6">
                                    <span><%=Resources.common.Strankovani%>:</span>
                                    <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                                        <asp:ListItem Text="10"></asp:ListItem>
                                        <asp:ListItem Text="20"></asp:ListItem>
                                        <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="100"></asp:ListItem>
                                        <asp:ListItem Text="200"></asp:ListItem>
                                        <asp:ListItem Text="500"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="content-box3" style="margin-top: 20px;">
                            <div class="title"></div>
                            <div class="content">

                                <div class="div6">

                                    <asp:HyperLink ID="cmdSummary" runat="server" NavigateUrl="javascript:drilldown()" Text="<img src='Images/pivot.png' /> WORKSHEET statistika aktuálního přehledu"></asp:HyperLink>
                                </div>

                                <div class="div6">

                                    <asp:HyperLink ID="cmdFullScreen" runat="server" Text="<img src='Images/fullscreen.png' /> Zobrazit přehled na celou stránku" NavigateUrl="javascript:p31_fullscreen()"></asp:HyperLink>

                                </div>
                                <div class="div6">
                                    <asp:CheckBox ID="chkIncludeChilds" runat="server" AutoPostBack="true" Text="Zahrnout i pod-projekty" CssClass="chk" Visible="false" />
                                </div>
                            </div>


                        </div>
                    </ContentTemplate>
                </telerik:RadMenuItem>
            </Items>
        </telerik:RadMenu>

    </div>



</asp:Panel>
<div style="clear: both; width: 100%;"></div>
<div id="divCurrentQuery">


    <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>
</div>

<uc:datagrid ID="grid2" runat="server" ClientDataKeyNames="pid" OnRowSelected="p31_RowSelected" OnRowDblClick="p31_RowDoubleClick_first"></uc:datagrid>

<asp:HiddenField ID="hidMasterDataPID" runat="server" />
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidNeedRefreshP31_subgrid" runat="server" />
<asp:HiddenField ID="hidExplicitDateFrom" runat="server" Value="01.01.1900" />
<asp:HiddenField ID="hidExplicitDateUntil" runat="server" Value="01.01.3000" />

<asp:HiddenField ID="hidDefaultSorting" runat="server" />

<asp:HiddenField ID="hidDrillDownField" runat="server" />
<asp:HiddenField ID="hidCols" runat="server" />
<asp:HiddenField ID="hidSumCols" runat="server" />
<asp:HiddenField ID="hidFooterString" runat="server" />
<asp:HiddenField ID="hidFrom" runat="server" />
<asp:HiddenField ID="hidAllowFullScreen" runat="server" Value="1" />
<asp:HiddenField ID="hidMasterTabAutoQueryFlag" runat="server" />
<asp:HiddenField ID="hidX18_value" runat="server" />

<script type="text/javascript">

    $(document).ready(function () {




    });



    function periodcombo_setting() {
        p31_subgrid_periodcombo();

    }



    function approving() {
        var pids = GetAllSelectedPIDs();
        if (pids == "") {
            alert("Není vybrán ani jeden záznam.");
            return;

        }
        p31_subgrid_approving(pids);


    }

    function batch_approve(p72id) {
        var pids = GetAllSelectedPIDs();        
        if (pids == "") {
            alert("Není vybrán záznam.");
            return;
        }

        $.post("Handler/handler_approve.ashx", { pids: pids, p72id: p72id }, function (data) {

            if (data != "1") {
                alert(data);
            }
            if (data.substring(0, 1) == "_") {
                return; //v každém záznamu jsou chyby
            }
            //schváleno - refresh
            hardrefresh(null, "p31-save");

        });

    }

    function batch_invoice(is_append) {
        var pids = GetAllSelectedPIDs();
        if (pids == "" || pids == null) {
            $.alert("Není vybrán záznam.");
            return
        }
        if (is_append == true)
            sw_decide("p91_add_worksheet.aspx?p31ids=" + pids, "Images/billing.png");

        if (is_append == false)
            sw_decide("p91_create_step1.aspx?nogateway=1&prefix=p31&masterpids=" + pids, "Images/billing.png");
    }

    function GetAllSelectedPIDs() {

        var masterTable = $find("<%=grid2.radGridOrig.ClientID%>").get_masterTableView();
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


    function p31_subgrid_query() {
        alert("prázdné");
        //return (false);
    }

    function p31_RowDoubleClick_first(sender, args) {
        if (args.get_tableView().get_name() == "grid") {
            p31_RowDoubleClick();
        }
        if (args.get_tableView().get_name() == "drilldown") {
            var item = sender.get_masterTableView().get_dataItems()[args.get_itemIndexHierarchical()];

            var rowid = item.get_id();
            var firstInput = $('#' + rowid).find('input[type=submit]').filter(':visible:first');
            if (firstInput != null) {
                firstInput.click();
            }

        }

    }

    function p31_fullscreen() {
        window.open("p31_grid.aspx?masterpid=" + document.getElementById("<%=me.hidMasterDataPID.ClientID%>").value + "&masterprefix=<%=BO.BAS.GetDataPrefix(Me.EntityX29ID)%>&p31tabautoquery=<%=me.MasterTabAutoQueryFlag%>", "_top");
        return (false);
    }

    function drilldown(p31ids) {
        var j70id = "<%=designer1.CurrentJ70ID%>";

        var w = screen.availWidth - 100;
        var masterprefix = "<%=BO.BAS.GetDataPrefix(Me.EntityX29ID)%>";
        var masterpid = document.getElementById("<%=me.hidMasterDataPID.ClientID%>").value;
        var queryflag = document.getElementById("<%=hidMasterTabAutoQueryFlag.ClientID%>").value;
        var url = "p31_sumgrid.aspx?j70id=" + j70id + "&masterprefix=" + masterprefix + "&masterpid=" + masterpid + "&p31tabautoquery=" + queryflag;
        if (p31ids != null)
            url = url + "&p31ids=" + p31ids;

        window.open(url, "_top");

    }

    function drilldown_p31ids() {
        var pids = GetAllSelectedPIDs();
        if (pids == "") {
            $.alert("Není vybrán ani jeden záznam.");
            return;
        }
        drilldown(pids);



    }
    function move2project_p31ids() {
        var pids = GetAllSelectedPIDs();
        if (pids == "") {
            $.alert("Není vybrán ani jeden záznam.");
            return;
        }
        sw_decide("p31_move2project.aspx?p31ids=" + pids, "Images/cut.png");
    }

    
</script>
