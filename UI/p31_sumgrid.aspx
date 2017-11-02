<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p31_sumgrid.aspx.vb" Inherits="UI.p31_sumgrid" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: white;">
        <div style="float: left;">
            <img src="Images/pivot_32.png" title="Summary worksheet přehledy" />

        </div>
        <div class="commandcell" style="padding-left: 6px;">


            <asp:DropDownList ID="j77ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="j77Name" Style="width: 250px;" ToolTip="Pojmenovaná šablona statistiky" BackColor="Yellow"></asp:DropDownList>
            <button type="button" onclick="templatebuilder()" title="Návrhář statistik" class="button-link"><img src="Images/setting.png" /></button>


        </div>

        <div class="commandcell" style="padding-left: 3px;">
            <button type="button" id="cmdGUI" class="show_hide1" style="padding: 2px; border-radius: 4px; border-top: solid 1px silver; border-left: solid 1px silver; border-bottom: solid 1px gray; border-right: solid 1px gray; background: buttonface;" title="Rozvržení panelů">
                Sledované veličiny<img src="Images/arrow_down.gif" />
            </button>
        </div>
        <div class="commandcell" style="padding-left: 10px;padding-right:10px;">
            <asp:DropDownList ID="cbxPeriodType" AutoPostBack="true" runat="server" ToolTip="Druh filtrovaného období">
                <asp:ListItem Text="Datum úkonu:" Value="p31Date" Selected="true"></asp:ListItem>
                <asp:ListItem Text="Datum založení:" Value="p31DateInsert"></asp:ListItem>
                <asp:ListItem Text="Vystavení faktury:" Value="p91Date"></asp:ListItem>
                <asp:ListItem Text="DZP faktury:" Value="p91DateSupply"></asp:ListItem>
            </asp:DropDownList>
            <uc:periodcombo ID="period1" runat="server" Width="180px"></uc:periodcombo>

        </div>

        <div class="commandcell" id="divQueryContainer"> 
            <uc:mygrid id="query1" runat="server" prefix="p31" masterprefix="" x36key="p31-j70id" ModeFlag="2"></uc:mygrid>              
        </div>

        <div class="commandcell">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true">
                <Items>


                    <telerik:RadMenuItem Text="Akce nad statistikou" ImageUrl="Images/menuarrow.png" Value="more" PostBack="false">
                        <GroupSettings OffsetX="-250" />
                        <ContentTemplate>                            
                            <div class="content-box3">
                                <div class="title">
                                    <img src="Images/query.png" />
                                    <span>Dodatečné filtrování dat</span>
                                </div>
                                <div class="content">
                                    <div class="div6">
                                        <button type="button" onclick="x18_querybuilder()"><img src="Images/label.png" />Kategorie</button>
                                        <asp:ImageButton ID="cmdClearX18" runat="server" ToolTip="Vyčistit filtr kategorií" ImageUrl="Images/delete.png" Visible="false" CssClass="button-link" />
                                        <asp:Label ID="x18_querybuilder_info" runat="server" ForeColor="Red"></asp:Label>
                                   
                                    <asp:DropDownList ID="cbxTabQueryFlag" runat="server" AutoPostBack="true">
                                        <asp:ListItem Text="--Druh úkonů--" Value="p31" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="Pouze hodiny" Value="time"></asp:ListItem>
                                        <asp:ListItem Text="Pouze výdaje" Value="expense"></asp:ListItem>
                                        <asp:ListItem Text="Paušální odměny" Value="fee"></asp:ListItem>
                                        <asp:ListItem Text="Pouze kusovník" Value="kusovnik"></asp:ListItem>
                                    </asp:DropDownList>
                                    </div>
                                    
                                    
                                </div>
                            </div>
                            <div class="content-box3">
                                <div class="title">
                                    <img src="Images/pivot.png" />
                                    <span>Modelovat výstup statistiky</span>
                                </div>
                                <div class="content">
                                    <button type="button" onclick="pivot()">PIVOT nástroj</button>
                                </div>
                            </div>
                            <asp:Panel ID="panExport" runat="server" CssClass="content-box3">
                                <div class="title">
                                    <img src="Images/export.png" />
                                    <span>Export výstupu statistiky</span>
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
                                        <asp:CheckBox ID="chkFirstLastCount" runat="server" AutoPostBack="true" Text="Zobrazovat sloupce: [Datum prvního úkonu], [Datum posledního úkonu] a [Počet]" CssClass="chk" Checked="true" />
                                    </div>
                                </div>
                        </ContentTemplate>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>

        </div>
        <div style="clear: both;"></div>

        <asp:Panel ID="panQueryByEntity" runat="server" CssClass="div6" Visible="false">
            <table cellpadding="0">
                <tr>
                    <td>
                        <asp:Image ID="imgEntity" runat="server" />
                    </td>
                    <td style="padding-left: 10px;">

                        <asp:HyperLink ID="MasterRecord" runat="server"></asp:HyperLink>

                    </td>
                </tr>
            </table>
            <asp:Label ID="lblQuery" runat="server" CssClass="valboldred"></asp:Label>
        </asp:Panel>

        

        <div style="float: left; padding-left: 6px;">
            
            <asp:Label ID="CurrentQuery" runat="server" ForeColor="Red"></asp:Label>
        </div>
        <div style="clear: both;"></div>

        <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick"></uc:datagrid>

    </div>


    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    
    
    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />
    <asp:HiddenField ID="hidP31IDs" runat="server" />
    <asp:HiddenField ID="hidFrom" runat="server" />
    <asp:HiddenField ID="hidSGF" runat="server" />
    <asp:HiddenField ID="hidDD1" runat="server" />
    <asp:HiddenField ID="hidDD2" runat="server" />
    <asp:HiddenField ID="hidSumCols" runat="server" />
    <asp:HiddenField ID="hidAddCols" runat="server" />



    <asp:HiddenField ID="hidMasterAW" runat="server" />
    <asp:HiddenField ID="hidGridColumnSql" runat="server" />
    <asp:HiddenField ID="hidX18_value" runat="server" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />

    <script type="text/javascript">
        $(document).ready(function () {
           
        });

        $(window).load(function () {

            <%=me.grid1.ClientID%>_Scroll2SelectedRow();

        });



        function RowSelected(sender, args) {
            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            var grid = sender;
            var MasterTable = grid.get_masterTableView()
            var row = MasterTable.get_dataItems()[args.get_itemIndexHierarchical()];
            var cell = MasterTable.getCellByColumnUniqueName(row, "<%=Me.hidDD1.Value%>");
            var sga = cell.innerHTML;
            <%If Me.hidDD2.Value <> "" Then%>
            cell = MasterTable.getCellByColumnUniqueName(row, "<%=Me.hidDD2.Value%>");
            sga = sga + "->" + cell.innerHTML;
            <%End If%>

            go2grid(pid, sga);
        }

        function go2grid(pid, sga) {
            var sgf = document.getElementById("<%=hidSGF.ClientID%>").value;
            var j70id = "<%=query1.CurrentJ70ID%>";
            var masterprefix = document.getElementById("<%=hidMasterPrefix.ClientID%>").value;
            var masterpid = document.getElementById("<%=hidMasterPID.ClientID%>").value;
            var p31ids = document.getElementById("<%=hidP31IDs.ClientID%>").value;
            var url = "p31_grid.aspx?sgf=" + sgf + "&sgv=" + pid + "&sga=" + encodeURI(sga);
            if (masterprefix != "") {
                url = url + "&masterprefix=" + masterprefix + "&masterpid=" + masterpid
            }

            if (p31ids != "") {
                url = url + "&drilldown_p31ids=" + p31ids;
            }
            //location.replace(url);
            location.href=url;
        }

       

        function hardrefresh(pid, flag) {

            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>;

        }

        function pivot() {
            var masterprefix = document.getElementById("<%=hidMasterPrefix.ClientID%>").value;
            var masterpid = document.getElementById("<%=hidMasterPID.ClientID%>").value;
            var p31ids = document.getElementById("<%=hidP31IDs.ClientID%>").value;
            sw_master("p31_sumgrid.aspx?pivot=1" + "&masterprefix=" + masterprefix + "&masterpid=" + masterpid+"&p31ids="+p31ids, "Images/pivot.png", true);

        }
        function templatebuilder() {
            var masterprefix = document.getElementById("<%=hidMasterPrefix.ClientID%>").value;
            var masterpid = document.getElementById("<%=hidMasterPID.ClientID%>").value;
            var pid = document.getElementById("<%=me.j77ID.clientid%>").value;

            sw_master("sumgrid_designer.aspx?pid=" + pid + "&masterprefix=" + masterprefix + "&masterpid=" + masterpid, "Images/setting.png");

        }
        function x18_querybuilder() {
            sw_master("x18_querybuilder.aspx?key=p31grid&prefix=p31", "Images/query.png");

        }

        
    </script>
</asp:Content>
