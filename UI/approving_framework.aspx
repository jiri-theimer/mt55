<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="approving_framework.aspx.vb" Inherits="UI.approving_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .RadGrid .rgRow td {
            border-left: solid 1px silver !important;
        }

        .RadGrid .rgAltRow td {
            border-left: solid 1px silver !important;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {


           




        });



        function periodcombo_setting() {

            sw_master("periodcombo_setting.aspx", "Images/settings.png");
        }

        function RowSelected(sender, args) {

            document.getElementById("<%=hidCurPID.ClientID%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            approve_record();
        }

        function approve_record() {
            var pid = document.getElementById("<%=hidCurPID.ClientID%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán řádek.");
                return
            }
            var scope = document.getElementById("<%=Me.cbxScope.ClientID%>").value;

            sw_master("entity_modal_approving.aspx?prefix=<%=me.hidCurPrefix.Value%>&pid=" + pid + "&scope=" + scope, "Images/approve.png", true);

        }

        function approve_selected() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                alert("Není vybrán řádek.");
                return
            }
            var scope = document.getElementById("<%=Me.cbxScope.ClientID%>").value;

            sw_master("entity_modal_approving.aspx?prefix=<%=me.hidCurPrefix.Value%>&pids=" + pids + "&scope=" + scope, "", true);

        }

        function invoice_selected() {
            var pids = GetAllSelectedPIDs();
            if (pids == "" || pids == null) {
                alert("Není vybrán řádek.");
                return
            }

            sw_master("entity_modal_invoicing.aspx?prefix=<%=me.hidCurPrefix.Value%>&pids=" + pids, "", true);

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
            document.getElementById("<%=hidCurPID.clientid%>").value = pid;
            document.getElementById("<%=hidHardRefreshFlag.ClientID%>").value = flag;
            
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefreshOnBehind, "", False)%>

        }

        function OnClientTabSelected(sender, eventArgs) {
            var tab = eventArgs.get_tab();
            var prefix = tab.get_value();

            $.post("Handler/handler_userparam.ashx", { x36value: prefix, x36key: "approving_framework-prefix", oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }
                location.replace("approving_framework.aspx");

            });


        }

      

        function report() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }

            sw_master("report_modal.aspx?prefix=<%=Me.CurrentPrefix%>&pid=" + pids, "Images/report.png", true);

        }
        function p31_move2bin() {
            var pids = GetAllSelectedPIDs();
            if (pids == "") {
                alert("Není vybrán ani jeden záznam.");
                return;
            }
            var direction = "1";
            <%If Me.cbxScope.SelectedValue = "2" Then%>
            direction = "3";
            <%End If%>

            sw_master("p31_move2bin.aspx?prefix=<%=Me.CurrentPrefix%>&pid=" + pids + "&direction=" + direction, "Images/bin.png", true);
        }
        function x18_querybuilder() {
            sw_master("x18_querybuilder.aspx?key=approve&prefix=<%=Me.CurrentPrefix%>", "Images/query.png");

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="tabs1" runat="server" ShowBaseLine="true" Width="100%" Skin="Default" OnClientTabSelected="OnClientTabSelected">
        <Tabs>
            <telerik:RadTab Text="Projekty" Value="p41" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Klienti" Value="p28"></telerik:RadTab>
            <telerik:RadTab Text="Osoby" Value="j02"></telerik:RadTab>
            <telerik:RadTab Text="Úkoly" Value="p56"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <div style="background-color: white;">
        <div style="float: left;">
            <img src="Images/approve_32.png" title="Příprava fakturačních podkladů" />
            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Schvalovat úkony" Visible="false"></asp:Label>
        </div>

        <div class="commandcell" style="padding-left: 20px;">
            <asp:DropDownList ID="cbxScope" runat="server" AutoPostBack="true" BackColor="Yellow">
                <asp:ListItem Text="Rozpracované (čeká na schvalování)" Value="1"></asp:ListItem>
                <asp:ListItem Text="Schválené (čeká na fakturaci)" Value="2"></asp:ListItem>
                <asp:ListItem Text="Schválené (čeká na fakturaci), #0" Value="20"></asp:ListItem>
                <asp:ListItem Text="Schválené (čeká na fakturaci), #1" Value="21"></asp:ListItem>
                <asp:ListItem Text="Schválené (čeká na fakturaci), #2" Value="22"></asp:ListItem>
            </asp:DropDownList>

        </div>
       <div class="commandcell">
           <asp:DropDownList ID="cbxJ27ID" runat="server" AutoPostBack="true">

           </asp:DropDownList>
       </div>
        <div class="commandcell" style="margin-left: 6px; margin-right: 6px;">
            <uc:periodcombo ID="period1" runat="server" Width="180px"></uc:periodcombo>
        </div>
        <div class="commandcell" id="divQueryContainer">     
            <uc:mygrid id="query1" runat="server" prefix="p31" masterprefix="" x36key="approving_framework-j70id" ModeFlag="2" ReloadUrl="approving_framework.aspx"></uc:mygrid>       
        </div>

        <div class="commandcell" style="margin-left: 10px;">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true">
                <Items>
                    <telerik:RadMenuItem Text="Vybrané (zaškrtlé)" ImageUrl="Images/menuarrow.png">
                        <Items>
                            <telerik:RadMenuItem Text="Zahájit schvalovací/fakturační proces" Value="approve" NavigateUrl="javascript:approve_selected()" ImageUrl="Images/approve.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Fakturovat bez schvalování" Value="draft" NavigateUrl="javascript:invoice_selected()" ImageUrl="Images/invoice.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Přesunout nevyfakturované úkony do archivu" Value="bin" NavigateUrl="javascript:p31_move2bin()" ImageUrl="Images/bin.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Tisková sestava" Value="report" NavigateUrl="javascript:report()" ImageUrl="Images/report.png"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>

                    <telerik:RadMenuItem Text="Další" ImageUrl="Images/menuarrow.png" Value="more" PostBack="false">
                        <GroupSettings OffsetX="-250" />
                        <ContentTemplate>
                            <div class="content-box3">
                                <div class="title">
                                    <img src="Images/query.png" />
                                    <span>Dodatečné filtrování dat</span>
                                </div>
                                <div class="content">
                                    <div class="div6">
                                        <button type="button" onclick="x18_querybuilder()">
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

                                  <button type="button" onclick="hardrefresh(0,'xls')" title="Export do XLSX vč. souhrnů s omezovačem na maximálně 2000 záznamů">
                                      <img src="Images/xls.png" />
                                      XLS
                                  </button>
                                  <button type="button" onclick="hardrefresh(0,'pdf')" title="Export do PDF vč. souhrnů s omezovačem na maximálně 2000 záznamů">
                                      <img src="Images/pdf.png" />
                                      PDF
                                  </button>
                                   <button type="button" onclick="hardrefresh(0,'doc')" title="Export do DOCX vč. souhrnů s omezovačem na maximálně 2000 záznamů">
                                      <img src="Images/doc.png" />
                                      DOC
                                  </button>


                                </div>
                            </asp:Panel>
                            <div class="content-box3">
                                <div class="title">
                                    <img src="Images/griddesigner.png" />
                                    <span>Nastavení přehledu</span>
                                </div>
                                <div class="content">
                                    <div class="div6">
                                        <asp:RadioButtonList ID="cbxScrollingFlag" runat="server" RepeatDirection="Vertical" AutoPostBack="true">
                                            <asp:ListItem Text="Pevné ukotvení záhlaví tabulky (názvy sloupců)" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Ukotvení všeho nad tabulkou (filtrování a menu)" Value="1" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Bez podpory ukotvení" Value="0"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="div6">
                                        <asp:Label ID="lblPaging" runat="server" CssClass="lbl" Text="Stránkování:"></asp:Label>
                                        <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="20"></asp:ListItem>
                                            <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="100"></asp:ListItem>
                                            <asp:ListItem Text="200"></asp:ListItem>
                                            <asp:ListItem Text="500"></asp:ListItem>
                                        </asp:DropDownList>

                                    </div>
                                    <div class="div6">
                                        <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true" ToolTip="Souhrny podle">
                                            <asp:ListItem Text="Bez souhrnů" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Souhrny podle měny" Value="j27Code"></asp:ListItem>
                                            <asp:ListItem Text="Souhrny podle klienta projektu" Value="Client"></asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                    <div class="div6">
                                        <asp:CheckBox ID="chkKusovnik" runat="server" AutoPostBack="true" Text="Zobrazovat i honoráře z kusovníkových úkonů" />
                                    </div>
                                    <div class="div6">
                                        <asp:CheckBox ID="chkFirstLastCount" runat="server" AutoPostBack="true" Text="Zobrazovat sloupce [Datum prvního úkonu], [Datum posledního úkonu]" Checked="true" />
                                    </div>
                                    <div class="div6">
                                        <asp:CheckBox ID="chkShowTags" runat="server" AutoPostBack="true" Text="Zobrazovat štítky" Checked="false" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>

        </div>


        <div style="clear: both;"></div>

        <div style="float: left; padding-left: 6px;">
            
            <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>
        </div>
        <div style="clear: both;"></div>

        <asp:Button ID="cmdHardRefreshOnBehind" runat="server" Style="display: none;" />


        <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowDblClick="RowDoubleClick" OnRowSelected="RowSelected" AllowFilteringByColumn="true"></uc:datagrid>


    </div>

    <asp:HiddenField ID="hidCurPID" runat="server" />
    <asp:HiddenField ID="hidCurPrefix" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidX18_value" runat="server" />
</asp:Content>
