<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="admin_framework.aspx.vb" Inherits="UI.admin_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="panelmenu" Src="~/panelmenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {




        });


        function lp(prefix) {
            location.replace("admin_framework.aspx?prefix=" + prefix);
        }
        function Edit() {
            var pid = document.getElementById("<%=hiddatapid.clientid%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_master("<%=ViewState("page")%>?pid=" + pid, "Images/setting.png");

        }

        function NewRecord(bolContextMenu) {
            sw_master("<%=ViewState("page")%>?pid=0&prefix=<%=ViewState("prefix")%>", "Images/setting.png");

            if (bolContextMenu == true)
                return

            return (false);
        }

        function RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid.clientid%>").value = args.getDataKeyValue("pid");

    }

    function RowDoubleClick(sender, args) {
        Edit();
    }


    

        function hardrefresh(pid, flag) {


            var s = "<%=menu1.FindItemByValue("refresh").NavigateUrl%>";
            location.replace(s + "&go2pid=" + pid);




        }

        function run_robot() {
            sw_master("/Public/robot.aspx?blank=1");
        }

        function p66_plan() {
            sw_everywhere("j02_personalplan.aspx", "Images/plan.png");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="min-height: 44px; background-color: white; border-bottom: solid 1px silver">
        <div style="float: left;">
            <img src="Images/setting_32.png" alt="Administrace" />

            <asp:Label ID="lblFormHeader" runat="server" CssClass="page_header_span" Text="Nastavení systému"></asp:Label>

        </div>
        <div class="commandcell" style="padding-left: 33px;">

            <asp:ImageButton ID="cmdExpandAll" runat="server" ImageUrl="Images/expand.png" ToolTip="Rozbalit celé menu" CssClass="button-link" />
            <asp:ImageButton ID="cmdCollapseAll" runat="server" ImageUrl="Images/collapse.png" ToolTip="Sbalit celé menu" CssClass="button-link" />
        </div>
        <div class="commandcell" style="min-width: 200px;">

            <asp:Label ID="lblPath" runat="server" CssClass="framework_header_span" Style="padding-left: 10px;"></asp:Label>
        </div>
        <div class="commandcell">
            <asp:TextBox ID="txtSearch" runat="server" Text="" Style="width: 90px;" Visible="false"></asp:TextBox>
            <asp:ImageButton ID="cmdSearch" runat="server" ImageUrl="Images/search.png" ToolTip="Hledat" Visible="false" CssClass="button-link" />

        </div>
        <div class="commandcell" style="padding-left: 20px;">
            <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                <asp:ListItem Text="20"></asp:ListItem>
                <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                <asp:ListItem Text="100"></asp:ListItem>
                <asp:ListItem Text="200"></asp:ListItem>
                <asp:ListItem Text="500"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="commandcell" style="margin-left: 20px;">
            <telerik:RadMenu ID="menu1" RenderMode="Auto" Skin="Metro" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true">
                <Items>
                    <telerik:RadMenuItem Text="Nový" ImageUrl="Images/new4menu.png" NavigateUrl="javascript:NewRecord(true);" Value="new"></telerik:RadMenuItem>
                    
                    <telerik:RadMenuItem Text="Export" Value="export" ImageUrl="Images/export.png" PostBack="true"></telerik:RadMenuItem>
                    <telerik:RadMenuItem Text="Obnovit" ImageUrl="Images/refresh.png" Value="refresh"></telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>

        </div>
    </div>

    <div style="clear: both; width: 100%;"></div>

    <div class="left_panel">

        <uc:panelmenu ID="panelmenu1" runat="server"></uc:panelmenu>

        <asp:DropDownList ID="query_validity" runat="server" AutoPostBack="true" Style="width: 230px;">
            <asp:ListItem Text="Bez filtrování otevřené/archiv" Value=""></asp:ListItem>
            <asp:ListItem Text="Pouze otevřené položky" Value="1"></asp:ListItem>
            <asp:ListItem Text="Pouze přesunuté do archivu" Value="2"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <div class="right_panel" style="margin-left: 270px;">

        <asp:Panel ID="panDashboard" runat="server" Style="background-color: white; padding: 10px; display: table;">



            <div class="content-box1">
                <div class="title">
                    Zaokrouhlování vykazovaného času
                </div>
                <div class="content">
                    <asp:Label ID="Round2Minutes" runat="server" CssClass="valbold"></asp:Label>
                    minut
                                <button type="button" onclick="sw_master('x35_record.aspx?key=Round2Minutes','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>
            <div class="content-box1">
                <div class="title">
                    Domácí měna
                </div>
                <div class="content">
                    <asp:Label ID="j27ID_Domestic" runat="server" CssClass="valbold"></asp:Label>

                    <button type="button" onclick="sw_master('x35_record.aspx?key=j27ID_Domestic','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>
            <div class="content-box1">
                <div class="title">
                    Výchozí splatnost vystavených faktur
                </div>
                <div class="content">
                    <asp:Label ID="DefMaturityDays" runat="server" CssClass="valbold"></asp:Label>
                    dnů
                                <button type="button" onclick="sw_master('x35_record.aspx?key=DefMaturityDays','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>



            <div class="content-box1">
                <div class="title">
                    Aktivita pro dobropisované úkony ve faktuře
                </div>
                <div class="content">
                    <asp:Label ID="p32ID_CreditNote" runat="server" CssClass="valbold"></asp:Label>
                    <button type="button" onclick="sw_master('x35_record.aspx?key=p32ID_CreditNote','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>


            <div class="content-box1" style="width: 100%; max-width: none;">
                <div class="title">
                    Další fakturační jazyky
                </div>
                <asp:Panel ID="panP87" runat="server" CssClass="content">

                    <div style="float: left;">
                        <asp:Label CssClass="lbl" ID="lblLang1" runat="server" Text="Jazyk #1:"></asp:Label>
                        <asp:Label ID="BillingLang1" runat="server"></asp:Label>
                        <asp:Image ID="p87Icon1" runat="server" />
                    </div>
                    <div style="float: left;">
                        <asp:Label CssClass="lbl" ID="lblLang2" runat="server" Text="Jazyk #2:"></asp:Label>
                        <asp:Label ID="BillingLang2" runat="server"></asp:Label>
                        <asp:Image ID="p87Icon2" runat="server" />
                    </div>
                    <div style="float: left;">
                        <asp:Label CssClass="lbl" ID="lblLang3" runat="server" Text="Jazyk #3:"></asp:Label>
                        <asp:Label ID="BillingLang3" runat="server"></asp:Label>
                        <asp:Image ID="p87Icon3" runat="server" />
                    </div>
                    <div style="float: left;">
                        <asp:Label CssClass="lbl" ID="Datalabel4" runat="server" Text="Jazyk #4:"></asp:Label>
                        <asp:Label ID="BillingLang4" runat="server"></asp:Label>
                        <asp:Image ID="p87Icon4" runat="server" />
                    </div>
                    <div style="float: left; padding-left: 20px;">
                        <button type="button" onclick="sw_master('admin_p87.aspx','Images/setting_32.png')">Nastavit</button>
                    </div>
                </asp:Panel>
            </div>

            <div class="content-box1">
                <div class="title">
                    Grafické logo
                </div>
                <div class="content">
                    <telerik:RadBinaryImage ID="imgLogoPreview" runat="server" ResizeMode="None" AlternateText="Chybí soubor grafického loga!" SavedImageName="marktime_customer_logo.png" />
                    <button type="button" onclick="sw_master('admin_logofile.aspx','Images/license_32.png')">Nahrát soubor grafického loga</button>
                </div>
            </div>


          



            <div style="clear: both; width: 100%;"></div>

            <div style="width: 100%;">
                <img style="display: inline;" src="Images/license_32.png" />
                <span style="display: inline;" class="framework_header_span">Systém</span>
                
            </div>


            <div class="content-box1">
                <div class="title">
                    Verze aplikace
                            
                                
                </div>
                <div class="content">

                    <asp:Label ID="version" runat="server" CssClass="valbold"></asp:Label>

                    <span class="lbl">Počet otevřených uživatelů</span>
                    <asp:Label ID="lblRealUsersCount" runat="server" CssClass="badge1"></asp:Label>

                    <div>
                        <a href="log_app_update.aspx">Histoire změn a novinek v systému</a>

                        <a href="about.aspx" style="margin-left: 20px;">O aplikaci</a>
                    </div>
                </div>
            </div>

            <div class="content-box1">
                <div class="title">Aplikační robot</div>
                <div class="content">
                    <asp:Label CssClass="lbl" ID="Label3" runat="server" Text="Host URL pro spouštění robota:"></asp:Label>
                    <asp:Label ID="robot_host" runat="server" CssClass="valbold"></asp:Label>
                    <button type="button" onclick="sw_master('admin_robot.aspx','Images/setting_32.png')">Nastavit</button>
                    <div>
                        Čas posledního spuštění robota:
                                    <asp:Label ID="robot_cache_lastrequest" runat="server" CssClass="valbold"></asp:Label>


                    </div>
                    <div>
                        <button type="button" onclick="run_robot()">Spusit robota ručně</button>

                    </div>
                </div>
            </div>
            <div class="content-box1" style="display: none;">
                <div class="title">
                    Název databáze
                </div>
                <div class="content">
                    <asp:Label ID="AppName" runat="server" CssClass="valbold"></asp:Label>
                    <button type="button" onclick="sw_master('x35_record.aspx?key=AppName','Images/setting_32.png')">Nastavit</button>


                </div>
            </div>
            <div class="content-box1">
                <div class="title">
                    Upload složka na serveru
                </div>
                <div class="content">
                    <asp:Label ID="Upload_Folder" runat="server" CssClass="valbold"></asp:Label>
                    <button type="button" onclick="sw_master('x35_record.aspx?key=Upload_Folder','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>
            <div class="content-box1" style="display:none;">
                <div class="title">
                    Režim přihlašování/ověřování uživatelů
                </div>
                <div class="content">
                    <asp:Label ID="UserAuthenticationMode" runat="server" CssClass="valbold"></asp:Label>
                    <button type="button" onclick="sw_master('x35_record.aspx?key=UserAuthenticationMode','Images/setting_32.png')">Nastavit</button>
                </div>
            </div>



        </asp:Panel>

        <asp:Panel ID="panGRID" runat="server">
            <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected" OnRowDblClick="RowDoubleClick" PagerAlwaysVisible="true" AllowFilteringByColumn="true"></uc:datagrid>
        </asp:Panel>


    </div>

    <asp:HiddenField ID="hiddatapid" runat="server" />
    <asp:HiddenField ID="hidGo2Pid" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
