<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p31_grid.aspx.vb" Inherits="UI.p31_grid" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {


            <%If designer1.Visible Then%>
            document.getElementById("cmdMyGridDesigner").style.display = "block";
            <%End If%>



        });





        function record_edit() {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "" || pid == null) {
                $.alert("Není vybrán záznam.");
                return
            }
            sw_master("p31_record.aspx?pid=" + pid, "Images/worksheet.png");

        }

        function record_new() {
            sw_master("p31_record.aspx?pid=0", "Images/worksheet.png");


        }

        function RowSelected(sender, args) {
            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            if (args.get_tableView().get_name() == "grid") {

                record_edit();
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

        function GetAllSelectedPIDs() {

            var masterTable = $find("<%=grid1.radGridOrig.ClientID%>").get_masterTableView();
            <%If Me.hidDrillDownField.Value = "" Then%>
            var sel = masterTable.get_selectedItems();
            <%Else%>

            var dataItems = masterTable.get_dataItems();
            for (var i = 0; i < dataItems.length; i++) {
                if (dataItems[i].get_nestedViews().length > 0) {
                    var sel = dataItems[i].get_nestedViews()[0].get_selectedItems();
                }
            }

            <%End If%>
            var pids = "";

            for (i = 0; i < sel.length; i++) {
                if (pids == "")
                    pids = sel[i].getDataKeyValue("pid");
                else
                    pids = pids + "," + sel[i].getDataKeyValue("pid");
            }

            return (pids);
        }

        function record_clone() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                $.alert("Není vybrán záznam.");
                return
            }
            sw_master("p31_record.aspx?clone=1&pid=" + pids, "Images/worksheet.png");

        }
        function tags() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                $.alert("Není vybrán záznam.");
                return
            }
            sw_master("tag_binding.aspx?prefix=p31&pids=" + pids, "Images/tag.png");

        }
        function move2project() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                $.alert("Není vybrán záznam.");
                return
            }
            sw_master("p31_move2project.aspx?p31ids=" + pids, "Images/cut.png");
        }
        function batch_approve(p72id) {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                $.alert("Není vybrán záznam.");
                return
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
                sw_master("p91_add_worksheet.aspx?p31ids=" + pids, "Images/billing.png");

            if (is_append == false)
                sw_master("p91_create_step1.aspx?nogateway=1&prefix=p31&masterpids=" + pids, "Images/billing.png");
        }



        function hardrefresh(pid, flag) {
            if (flag == "quickquery") {
                location.replace("p31_grid.aspx");
                return;
            }
            if (flag == "p91-create") {
                location.replace("p91_framework.aspx?pid=" + pid);
                return;
            }
            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }



        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx", "Images/settings.png");
        }


        function approving() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                $.alert("Není vybrán ani jeden záznam.");
                return;
                //return (false);
            }

            sw_master("p31_approving_step2.aspx?pids=" + pids, "Images/approve.png", true);


        }


        function drilldown(p31ids) {

            var masterprefix = document.getElementById("<%=Me.hidMasterPrefix.ClientID%>").value;
            var masterpid = document.getElementById("<%=Me.hidMasterPID.ClientID%>").value;
            var tabqueryflag = document.getElementById("<%=Me.cbxTabQueryFlag.ClientID%>").value;
            var url = "p31_sumgrid.aspx?masterprefix=" + masterprefix + "&masterpid=" + masterpid + "&tabqueryflag=" + tabqueryflag;
            if (p31ids != "")
                url = url + "&p31ids=" + p31ids;

            location.replace(url);


        }
        function drilldown_p31ids() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                $.alert("Není vybrán ani jeden záznam.");
                return;
            }
            drilldown(pids);
            //location.replace("p31_sumgrid.aspx?p31ids=" + pids);


        }
        function x18_querybuilder() {
            sw_master("x18_querybuilder.aspx?key=p31grid&prefix=p31", "Images/query.png");

        }
        function o51_querybuilder() {
            sw_master("o51_querybuilder.aspx?prefix=p31", "Images/query.png");

        }
        function clear_o51() {
            var clickButton = document.getElementById("<%=cmdClearO51.ClientID%>");
            clickButton.click();
        }

        function SelectGridRow(pid) {
            if (document.getElementById("<%=hiddatapid.clientid%>").value == pid)
                return; //záznam již je označen

            var masterTable = $find("<%=grid1.radGridOrig.ClientID%>").get_masterTableView();
            masterTable.clearSelectedItems();
            var items = masterTable.get_dataItems();
            for (var i = 0; i < items.length; i++) {
                var row = items[i];
                if (row.getDataKeyValue("pid") == pid) {
                    document.getElementById("<%=hiddatapid.clientid%>").value = pid;
                    masterTable.selectItem(masterTable.get_dataItems()[i].get_element());
                    return;
                }

            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 44px; background-color: white;">
        <div style="float: left; padding-top: 3px;">
            <img src="Images/worksheet_32.png" alt="Worksheet přehled" />
        </div>
        <div class="commandcell" style="padding-left: 10px; min-width: 40px;">
            <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Style="vertical-align: top;"></asp:Label>
        </div>



        <div class="commandcell" style="padding-left: 10px; padding-right: 10px;">
            <asp:DropDownList ID="cbxPeriodType" AutoPostBack="true" runat="server" ToolTip="Druh filtrovaného období" Style="width: 120px;">
                <asp:ListItem Text="Datum úkonu:" Value="p31Date" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Datum založení:" Value="p31DateInsert"></asp:ListItem>
                <asp:ListItem Text="Vystavení faktury:" Value="p91Date"></asp:ListItem>
                <asp:ListItem Text="DZP faktury:" Value="p91DateSupply"></asp:ListItem>
            </asp:DropDownList>
            <uc:periodcombo ID="period1" runat="server" Width="180px"></uc:periodcombo>
        </div>


        <div class="commandcell">
            <uc:mygrid ID="designer1" runat="server" Prefix="p31" MasterPrefix="" x36Key="p31-j70id"></uc:mygrid>
        </div>


        <div class="commandcell">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true" CollapseAnimation-Duration="0" CollapseAnimation-Type="None">
                <Items>

                    <telerik:RadMenuItem Value="cmdNew" Text="<%$Resources:common,Novy %>" NavigateUrl="javascript:record_new();" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>

                    <telerik:RadMenuItem Text="Vybrané (zaškrtlé)" Value="recs" ImageUrl="Images/arrow_down_menu.png">
                        <Items>
                            <telerik:RadMenuItem Value="cmdClone" Text="Kopírovat záznamy" NavigateUrl="javascript:record_clone();" ImageUrl="Images/copy.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Value="cmdApprove" Text="[SCHVÁLIT/PŘE-SCHVÁLIT]" ImageUrl="Images/approve.png">
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
                            <telerik:RadMenuItem Value="cmdInvoice" Text="[FAKTUROVAT]" ImageUrl="Images/billing.png">
                                <Items>
                                    <telerik:RadMenuItem Text="Fakturovat schválené (nová faktura)" NavigateUrl="javascript:batch_invoice(false);"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Přidat schválené do existující faktury" NavigateUrl="javascript:batch_invoice(true);"></telerik:RadMenuItem>
                                </Items>
                            </telerik:RadMenuItem>
                            <telerik:RadMenuItem Value="cmdMove" Text="Přesunout na jiný projekt" NavigateUrl="javascript:move2project();" ImageUrl="Images/cut.png" Visible="false"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Value="cmdApprove" Text="Oštítkovat" NavigateUrl="javascript:tags();" ImageUrl="Images/tag.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Value="cmdSummary" Text="Statistiky" NavigateUrl="javascript:drilldown_p31ids();" ImageUrl="Images/pivot.png"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Další" Value="more" ImageUrl="Images/arrow_down_menu.png">
                        <GroupSettings OffsetX="-220" />
                        <ContentTemplate>
                            <div class="content-box3">
                                <div class="title">
                                    <img src="Images/query.png" />
                                    <span>Dodatečné filtrování záznamů</span>
                                </div>
                                <div class="content">
                                    <div class="div6">
                                        <button type="button" onclick="o51_querybuilder()" style="width: 90px;">
                                            <img src="Images/query.png" />Štítky</button>
                                        <asp:ImageButton ID="cmdClearO51" runat="server" ToolTip="Vyčistit filtr štítků" ImageUrl="Images/delete.png" Visible="false" CssClass="button-link" />
                                        <asp:Label ID="o51_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                    <div class="div6">
                                        <button type="button" onclick="x18_querybuilder()" style="width: 90px;">
                                            <img src="Images/query.png" />Kategorie</button>
                                        <asp:ImageButton ID="cmdClearX18" runat="server" ToolTip="Vyčistit filtr kategorií" ImageUrl="Images/delete.png" Visible="false" CssClass="button-link" />
                                        <asp:Label ID="x18_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                    </div>
                                    <div class="div6">
                                        <span class="val">Omezovat přehled na druh úkonů:</span>
                                        <asp:DropDownList ID="cbxTabQueryFlag" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="" Value="p31" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Pouze hodiny" Value="time"></asp:ListItem>
                                            <asp:ListItem Text="Pouze výdaje" Value="expense"></asp:ListItem>
                                            <asp:ListItem Text="Paušální odměny" Value="fee"></asp:ListItem>
                                            <asp:ListItem Text="Pouze kusovník" Value="kusovnik"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>


                                </div>
                            </div>

                            <asp:Panel ID="panExport" runat="server" CssClass="content-box3">
                                <div class="title">
                                    <img src="Images/export.png" />
                                    Export záznamů aktuálního přehledu
                                </div>
                                <div class="content">
                                    <asp:Button ID="cmdExport" runat="server" Text="Export" ToolTip="Export do MS EXCEL tabulky, plný počet záznamů" CssClass="cmd" />
                                    <asp:Button ID="cmdXLS" runat="server" Text="XLS" ToolTip="Export do XLS vč. souhrnů s omezovačem na maximálně 2000 záznamů" CssClass="cmd" />
                                    <asp:Button ID="cmdPDF" runat="server" Text="PDF" CssClass="cmd" ToolTip="Export do PDF vč. souhrnů s omezovačem na maximálně 2000 záznamů" />
                                    <asp:Button ID="cmdDOC" runat="server" Text="DOC" ToolTip="Export do DOC vč. souhrnů s omezovačem na maximálně 2000 záznamů" CssClass="cmd" />
                                </div>
                            </asp:Panel>
                            <div class="content-box3" style="margin-top: 20px;">
                                <div class="title">
                                    <img src="Images/griddesigner.png" />
                                    <span>Sloupce v přehledu</span>

                                </div>
                                <div class="content">
                                    <asp:Panel ID="panGroupBy" runat="server" CssClass="div6" Style="float: left;">
                                        <span class="val"><%=Resources.p31_grid.DatoveSouhrny%></span>
                                        <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true" ToolTip="Datové souhrny" DataTextField="ColumnHeader" DataValueField="ColumnField">
                                        </asp:DropDownList>
                                        <div>
                                            <asp:CheckBox ID="chkGroupsAutoExpanded" runat="server" Text="<%$Resources:p31_framework,AutoRozbaleneSouhrny %>" AutoPostBack="true" Checked="true" />
                                        </div>
                                    </asp:Panel>


                                    <div class="div6" style="float: left;">
                                        <asp:Label ID="lblPaging" runat="server" CssClass="val" Text="<%$Resources:common,PocetZaznamuNaStranku %>"></asp:Label>
                                        <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování" TabIndex="3">
                                            <asp:ListItem Text="20"></asp:ListItem>
                                            <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="100"></asp:ListItem>
                                            <asp:ListItem Text="200"></asp:ListItem>
                                            <asp:ListItem Text="500"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div>
                                        <button type="button" id="cmdMyGridDesigner" onclick="mygrid_setting()" style="display: none;">Návrhář přehledu (sloupce a filtry)</button>
                                    </div>
                                </div>
                            </div>
                            <div class="content-box3">
                                <div class="title"></div>
                                <div class="content">
                                    <div class="div6">
                                        <asp:HyperLink ID="cmdSummary" runat="server" NavigateUrl="javascript:drilldown()" Text="<img src='Images/pivot.png'/> Statistika aktuálního přehledu"></asp:HyperLink>


                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadMenuItem>




                </Items>
            </telerik:RadMenu>

        </div>


        <div style="clear: both;"></div>
        <asp:Panel ID="panAdditionalQuery" runat="server" CssClass="div6" Visible="false">

            <asp:Image ID="imgEntity" runat="server" />
            <asp:HyperLink ID="MasterRecord" runat="server"></asp:HyperLink>


            <asp:Label ID="lblDrillDown" runat="server" CssClass="valboldred" Style="margin-left: 20px;"></asp:Label>
            <asp:HyperLink ID="linkDrillDown" runat="server"></asp:HyperLink>
            <asp:ImageButton ID="cmdDrillDownClear" runat="server" ToolTip="Zrušit filtr pro statistiku" ImageUrl="Images/delete.png" Visible="false" />
        </asp:Panel>

        <div style="clear: both;"></div>

        <div style="float: left; padding-left: 6px;" id="divCurrentQuery">

            <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>
        </div>
        <div style="float: left;">
            <asp:LinkButton ID="cmdCĺearFilter" runat="server" Text="<img src='Images/sweep.png'/> Vyčistit sloupcový filtr" Style="margin-left: 10px; font-weight: bold; color: red;"></asp:LinkButton>
        </div>
        <div style="clear: both;"></div>
    </div>



    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick"></uc:datagrid>


    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidDefaultSorting" runat="server" />
    <asp:HiddenField ID="hidDrillDownField" runat="server" />
    <asp:HiddenField ID="hidDrillDownP31IDs" runat="server" />
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
    <asp:HiddenField ID="hidCols" runat="server" />
    <asp:HiddenField ID="hidSumCols" runat="server" />
    <asp:HiddenField ID="hidFrom" runat="server" />
    <asp:HiddenField ID="hidMasterAW" runat="server" />

    <asp:HiddenField ID="hidFooterString" runat="server" />
    <asp:HiddenField ID="hidJ62ID" runat="server" />
    <asp:HiddenField ID="hidSGF" runat="server" />
    <asp:HiddenField ID="hidSGA" runat="server" />
    <asp:HiddenField ID="hidSGV" runat="server" />
    <asp:HiddenField ID="hidUIFlag" runat="server" />
    <asp:HiddenField ID="hidX18_value" runat="server" />
    <asp:HiddenField ID="hidO51IDs" runat="server" />

    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
