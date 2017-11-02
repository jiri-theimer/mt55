<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="entity_framework.aspx.vb" Inherits="UI.entity_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var _initResizing = "1";

        $(document).ready(function () {


            document.getElementById("<%=Me.hidUIFlag.ClientID%>").value = "";
            <%If _curIsExport = False Then%>
            _initResizing = "0";
            <%End If%>




        });




        function loadSplitter(sender) {

            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2 - 1;

            sender.set_height(h3);

            <%If opgLayout.Value <> "3" Then%>
            var pane = sender.getPaneById("<%=contentPane.ClientID%>");
            document.getElementById("<%=Me.hidContentPaneWidth.ClientID%>").value = pane.get_width();
            pane.set_contentUrl(document.getElementById("<%=Me.hidContentPaneDefUrl.ClientID%>").value);
            <%End if%>
            
            <%If grid1.radGridOrig.ClientSettings.Scrolling.UseStaticHeaders Then%>
            pane = sender.getPaneById("<%=navigationPane.ClientID%>");
            <%=Me.grid1.ClientID%>_SetScrollingHeight_Explicit(pane.get_height() - 25);
            <%End If%>
            <%=me.grid1.ClientID%>_Scroll2SelectedRow(pane.get_height());

        }



        function RowSelected(sender, args) {
            var pid = args.getDataKeyValue("pid");
            document.getElementById("<%=hiddatapid.clientid%>").value = pid;            
            <%If opgLayout.Value = "3" Then%>
            return;
            <%End If%>

            var splitter = $find("<%= RadSplitter1.ClientID %>");
            var pane = splitter.getPaneById("<%=contentPane.ClientID%>");

            var url = "<%=Me.CurrentPrefix%>_framework_detail.aspx?pid=" + pid + "&source=<%=opgLayout.Value%>";
            pane.set_contentUrl(url);


        }

        function send_url_to_pane(url) {
            var splitter = $find("<%= RadSplitter1.ClientID %>");
            var pane = splitter.getPaneById("<%=contentPane.ClientID%>");
            url = url + "&source=<%=opgLayout.Value%>";
            pane.set_contentUrl(url);
        }

        function RowDoubleClick(sender, args) {

            var pid = args.getDataKeyValue("pid");
            location.replace("<%=Me.CurrentPrefix%>_framework_detail.aspx?pid=" + pid + "&source=<%=opgLayout.Value%>");


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

        function SavePaneWidth(w) {
            if (_initResizing == "1") {
                return;
            }


            <%If Me.opgLayout.Value = "1" Then%>
            var keyname = "<%=Me.CurrentPrefix%>_framework-navigationPane_width";
            <%Else%>
            var keyname = "<%=Me.CurrentPrefix%>_framework-contentPane_height";
            <%End If%>

            $.post("Handler/handler_userparam.ashx", { x36value: w, x36key: keyname, oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }


            });
        }

        function AfterPaneResized(sender, args) {
            <%If Me.opgLayout.Value = "1" Then%>
            var w = sender.get_width();
            <%End If%>
            <%If Me.opgLayout.Value = "2" Then%>
            var w = sender.get_height();
            <%End If%>
            SavePaneWidth(w);

        }


        function AfterPaneCollapsed(pane) {
            var w = "-1";
            SavePaneWidth(w);
        }
        function AfterPaneExpanded(pane) {
            var w = pane.get_width();
            SavePaneWidth(w);
        }

        function context_menu_callback(flag) {
            document.getElementById("<%=hidContextMenuFlag.ClientID%>").value = flag;
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdContextMenuCallback, "", False)%>;
        }

        function hardrefresh(pid, flag) {

            <%If Master.Factory.SysUser.j04IsMenu_Invoice Then%>
            if (flag == "p91-create" || flag == "p31-add-p91") {
                location.replace("p91_framework.aspx?pid=" + pid);
                return;
            }
            <%End If%>
            if (flag == "p31-save" || flag == "p31-delete") {
                var splitter = $find("<%= RadSplitter1.ClientID %>");
                var pane = splitter.getPaneById("<%=contentPane.ClientID%>");
                var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
                var url = "<%=Me.CurrentPrefix%>_framework_detail.aspx?pid=" + pid + "&source=<%=opgLayout.Value%>";
                pane.set_contentUrl(url);
                return;
            }

            if (flag == "<%=Me.CurrentPrefix%>-create" || flag == "<%=Me.CurrentPrefix%>-save") {
                location.replace("<%=Me.CurrentPrefix%>_framework.aspx?pid=" + pid);
                return;
            }

            location.replace("entity_framework.aspx?prefix=<%=Me.CurrentPrefix%>");


        }



        function batch() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }

            $.post("Handler/handler_tempbox.ashx", { guid: "<%=Me.CurrentPrefix%>_batch-pids-<%=Master.Factory.SysUser.PID%>", value: pids, field: "p85Message", oper: "save" }, function (data) {

                if (data == " " || data == "0" || data == "") {
                    return;
                }


            });



            sw_master("<%=Me.CurrentPrefix%>_batch.aspx", "Images/batch.png");
            return;

        }
        function sendmail_batch() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }

            $.post("Handler/handler_tempbox.ashx", { guid: "<%=Me.CurrentPrefix%>_batch_sendmail-pids-<%=Master.Factory.SysUser.PID%>", value: pids, field: "p85Message", oper: "save" }, function (data) {

                if (data == " " || data == "0" || data == "") {
                    return;
                }


            });

            sw_master("<%=Me.CurrentPrefix%>_batch_sendmail.aspx", "Images/email.png");
            return;
        }

        function report() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }

            sw_master("report_modal.aspx?prefix=<%=Me.CurrentPrefix%>&pids=" + pids, "Images/report.png", true);

        }

        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx");
        }
        function approve() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }
            sw_master("p31_approving_step1.aspx?masterprefix=<%=me.CurrentPrefix%>&masterpids=" + pids, "Images/approve.png", true);
        }
        function invoice() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }
            sw_master("entity_modal_invoicing.aspx?prefix=<%=me.CurrentPrefix%>&pids=" + pids, "Images/invoice.png", true);
        }
        function cbx1_OnClientSelectedIndexChanged(sender, eventArgs) {
            var combo = sender;
            var pid = combo.get_value();
            <%If opgLayout.Value = "1" Then%>
            var url = "<%=Me.CurrentPrefix%>_framework_detail.aspx?pid=" + pid + "&source=<%=opgLayout.Value%>";
            location.replace("<%=Me.CurrentPrefix%>_framework.aspx?pid=" + pid);
            <%End If%>
            <%If opgLayout.Value = "2" Then%>
            location.replace("<%=Me.CurrentPrefix%>_framework.aspx?pid=" + pid);
            <%End If%>
            <%If opgLayout.Value = "3" Then%>
            location.replace("<%=Me.CurrentPrefix%>_framework_detail.aspx?source=3&pid=" + pid);
            <%End If%>
        }
        function cbx1_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["flag"] = "searchbox";
            <%If Me.CurrentPrefix = "p41" Then%>
            context["j02id_explicit"] = "<%=Master.Factory.SysUser.j02ID%>";
            <%End If%>
        }
        function drilldown() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }
            location.replace("p31_sumgrid.aspx?masterprefix=<%=me.CurrentPrefix%>&masterpid=" + pids);

        }
        function x18_querybuilder() {
            sw_master("x18_querybuilder.aspx?key=grid&prefix=<%=Me.CurrentPrefix%>", "Images/query.png");

        }
        function o51_querybuilder() {
            sw_master("o51_querybuilder.aspx?prefix=<%=Me.CurrentPrefix%>", "Images/query.png");

        }
        function clear_o51() {
            context_menu_callback("clear_o51");

        }
        function clear_x18() {
            context_menu_callback("clear_x18");

        }
        function tags() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                $.alert("Není vybrán záznam.");
                return
            }
            sw_master("tag_binding.aspx?prefix=<%=me.CurrentPrefix%>&pids=" + pids, "Images/tag.png");

        }

        var _lastMenuTime = null;

        function menu1_Hidden(menu) {
            var d = new Date();
            _lastMenuTime = d.getTime();
        }
        function menu1_Shown(menu) {
            //nic
        }
        function menu1_handle() {
            var d = new Date();
            var n = d.getTime();
            var contextMenu = $find("<%= menu1.ClientID %>");

            if (n - _lastMenuTime < 2) {
                return;
            }

            var x = 0;
            var y = 70;
            contextMenu.showAt(x, y);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" ResizeMode="Proportional" OnClientLoad="loadSplitter" PanesBorderSize="0" Skin="Default" RenderMode="Auto" Orientation="Vertical">
        <telerik:RadPane ID="navigationPane" runat="server" Width="353px" OnClientResized="AfterPaneResized" OnClientCollapsed="AfterPaneCollapsed" OnClientExpanded="AfterPaneExpanded" BackColor="white">

            <asp:Panel ID="panSearch" runat="server" Style="min-height: 42px; background-color: #f7f7f7;">
                
                <div style="float:left;padding-top:1px;padding-left:1px;">
                    <a id="linkMenu" href="javascript:menu1_handle()" class="pp2"></a>

                </div>                

                <asp:Panel ID="panSearchbox" runat="server" CssClass="commandcell" Style="padding-left: 5px;">
                    <telerik:RadComboBox ID="cbx1" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat..." Width="100px" OnClientSelectedIndexChanged="cbx1_OnClientSelectedIndexChanged" OnClientItemsRequesting="cbx1_OnClientItemsRequesting" AutoPostBack="false">
                        <WebServiceSettings Method="LoadComboData" UseHttpGet="false" />
                    </telerik:RadComboBox>
                </asp:Panel>
                <div class="commandcell" style="padding-left: 4px;">
                    <uc:mygrid ID="designer1" runat="server" Prefix="p41" Width="170px"></uc:mygrid>


                </div>
                <div class="commandcell" style="padding-left: 4px;">
                    <uc:periodcombo ID="period1" runat="server" Width="160px"></uc:periodcombo>
                    <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>


                </div>

            </asp:Panel>

            <div style="clear: both; width: 100%;"></div>
            <div style="float: left;">
                <asp:Label ID="MasterEntity" runat="server" Visible="false"></asp:Label>
            </div>



            <div style="float: left; padding-left: 6px;">
                <asp:LinkButton ID="cmdCĺearFilter" runat="server" Text="Vyčistit sloupcový filtr" Style="font-weight: bold; color: red;" Visible="false"></asp:LinkButton>
            </div>
            <div style="clear: both; width: 100%;"></div>



            <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" Skin="Default"></uc:datagrid>

            <asp:Button ID="cmdContextMenuCallback" runat="server" Style="display: none;" />
            <asp:HiddenField ID="hidContextMenuFlag" runat="server" />
            <asp:HiddenField ID="hiddatapid" runat="server" />
            <asp:HiddenField ID="hidDefaultSorting" runat="server" />
            <asp:HiddenField ID="hidJ62ID" runat="server" />
            <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
            <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />
            <asp:HiddenField ID="hidFooterSum" runat="server" Value="" />
            <asp:HiddenField ID="hidUIFlag" runat="server" />
            <asp:HiddenField ID="hidMasterPrefix" runat="server" />
            <asp:HiddenField ID="hidMasterPID" runat="server" />
            <asp:HiddenField ID="hidCols" runat="server" />
            <asp:HiddenField ID="hidSumCols" runat="server" />
            <asp:HiddenField ID="hidAdditionalFrom" runat="server" />
            <asp:HiddenField ID="hidContentPaneWidth" runat="server" />
            <asp:HiddenField ID="hidContentPaneDefUrl" runat="server" />
            <asp:HiddenField ID="hidX18_value" runat="server" />
            <asp:HiddenField ID="hidO51IDs" runat="server" />
            <asp:HiddenField ID="opgLayout" runat="server" Value="1" />
            <telerik:RadContextMenu ID="menu1" runat="server" Skin="Metro" ExpandDelay="0" RenderMode="Lightweight" OnClientHidden="menu1_Hidden" OnClientShown="menu1_Shown">
                <CollapseAnimation Type="None" />
                <ExpandAnimation Type="None" />
                <Items>
                    <telerik:RadMenuItem Text="Rozvržení panelů" ImageUrl="Images/form.png" Value="groupLayout">
                        <Items>
                            <telerik:RadMenuItem Text="Levý panel (přehled) + pravý panel (detail)" NavigateUrl="javascript:context_menu_callback('layout1')" Value="layout1" ImageUrl="Images/unchecked.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Pouze jeden panel (přechod mezi přehledem a detailem)" NavigateUrl="javascript:context_menu_callback('layout3')" Value="layout3" ImageUrl="Images/unchecked.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Horní panel (přehled) + spodní panel (detail)" NavigateUrl="javascript:context_menu_callback('layout2')" Value="layout2" ImageUrl="Images/unchecked.png"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Export záznamů v aktuálním přehledu" ImageUrl="Images/export.png" Value="groupExport">
                        <Items>
                            <telerik:RadMenuItem Text="Export" Value="export" NavigateUrl="javascript:context_menu_callback('export')" ImageUrl="Images/export.png" ToolTip="Export do MS EXCEL, plný počet záznamů"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="XLS" Value="xls" ImageUrl="Images/xls.png" NavigateUrl="javascript:context_menu_callback('xls')" ToolTip="Export do XLS vč. souhrnů s omezovačem na maximálně 2000 záznamů"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="PDF" Value="pdf" ImageUrl="Images/pdf.png" NavigateUrl="javascript:context_menu_callback('pdf')" ToolTip="Export do PDF vč. souhrnů s omezovačem na maximálně 2000 záznamů"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="DOC" Value="doc" ImageUrl="Images/doc.png" NavigateUrl="javascript:context_menu_callback('doc')" ToolTip="Export do XLS vč. souhrnů s omezovačem na maximálně 2000 záznamů"></telerik:RadMenuItem>

                        </Items>
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Operace nad vybranými (zaškrtlými) záznamy" ImageUrl="Images/batch.png" Value="groupBatch">
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Nastavení přehledu" ImageUrl="Images/griddesigner.png" value="groupOther">
                        <ContentTemplate>
                            <div style="width: 400px; padding: 10px;background-color:#f0f8ff;border:solid 1px gray;">
                                        <div>
                                            <asp:Label ID="lblLayoutMessage" runat="server" CssClass="infoNotificationRed" Text="Z důvodu malého rozlišení displeje (pod 1280px) se automaticky zapnul režim jediného panelu s datovým přehledem." Visible="false"></asp:Label>
                                        </div>
                                        <div style="margin-top: 20px;">
                                            <span>Filtrovat přehled podle období:</span>
                                            <asp:DropDownList ID="cbxPeriodType" onchange="context_menu_callback('cbxPeriodType')" runat="server" ToolTip="Druh filtrovaného období">
                                            </asp:DropDownList>
                                        </div>
                                        <div style="margin-top: 10px;">
                                            <button type="button" onclick="o51_querybuilder()" style="width: 90px;">
                                                <img src="Images/query.png" />Štítky</button>

                                            <asp:Label ID="o51_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                        </div>
                                        <div>
                                            <button type="button" onclick="x18_querybuilder()" style="width: 90px;">
                                                <img src="Images/query.png" />Kategorie</button>

                                            <asp:Label ID="x18_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                        </div>

                                        <div style="margin-top: 20px;">
                                            <asp:DropDownList ID="cbxGroupBy" runat="server" ToolTip="Datové souhrny" DataTextField="ColumnHeader" DataValueField="ColumnField" onchange="context_menu_callback('cbxGroupBy')">
                                            </asp:DropDownList>

                                            <span class="val" style="margin-left: 50px;">Stránkování záznamů:</span>
                                            <asp:DropDownList ID="cbxPaging" runat="server" onchange="context_menu_callback('cbxPaging')">
                                                <asp:ListItem Text="20"></asp:ListItem>
                                                <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="100"></asp:ListItem>
                                                <asp:ListItem Text="200"></asp:ListItem>
                                                <asp:ListItem Text="500"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>


                                        <div style="margin-top: 20px;">
                                            <asp:DropDownList ID="chkCheckboxSelector" runat="server" onchange="context_menu_callback('chkCheckboxSelector')">
                                                <asp:ListItem Text="Pro výběr (označení) záznamů nabízet i zaškrátávací checkbox" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Výběr záznamů pouze myší a klávesami CTRL+SHFT" Value="0"></asp:ListItem>
                                            </asp:DropDownList>

                                        </div>

                                    </div>
                        </ContentTemplate>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadContextMenu>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitbar1" runat="server" CollapseMode="Forward">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="contentPane" runat="server" ShowContentDuringLoad="true">
            Detail projektu
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
