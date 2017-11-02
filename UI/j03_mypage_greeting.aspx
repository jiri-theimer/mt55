<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="j03_mypage_greeting.aspx.vb" Inherits="UI.j03_mypage_greeting" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="myscheduler" Src="~/myscheduler.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_record_readonly" Src="~/o23_record_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#slidingDiv1").hide();
            $("#show_hide1").show();

            $('#show_hide1').click(function () {
                $("#slidingDiv1").slideToggle();
            });

        });

        function sw_local(url, img, is_maximize) {
            sw_master(url, img, is_maximize);
        }


        function report() {
            sw_master("report_modal.aspx?prefix=j02&pid=<%=Master.Factory.SysUser.j02ID%>", "Images/reporting.png", true);

        }
        function sendmail() {
            sw_master("sendmail.aspx", "Images/email.png")


        }




        function hardrefresh(pid, flag) {
            if (flag == "o23-save") {
                location.replace("o23_fixwork.aspx?pid=" + pid);
                return;
            }
            location.replace("default.aspx");
        }
        function fulltext2() {
            sw_master("clue_search.aspx?fulltext=1&blank=1", "Images/search.png")
        }

        function re(pid, prefix) {
            if (prefix == 'o22')
                sw_master("o22_record.aspx?pid=" + pid, "Images/event.png")


            if (prefix == 'p56')
                location.replace("p56_framework.aspx?pid=" + pid);

        }
        function o23_create_dashboard(x18id) {

            sw_master("o23_record.aspx?pid=0&x18id=" + x18id, "Images/label.png")
        }
        function b07_reaction(b07id) {
            sw_everywhere("b07_create.aspx?parentpid=" + b07id + "&masterprefix=o23&masterpid=<%=rec1.pid%>", "Images/comment.png", true)

        }
        function o23_fullscreen() {
            location.replace("o23_fixwork.aspx?pid=<%=rec1.pid%>");
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding: 10px; background-color: white;">
        <div>
            <a id="show_hide1" class="pp2"></a>
            <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Vítejte v systému"></asp:Label>
            <img src="Images/logo_transparent.png" style="margin-left: 10px;" alt="MARKTIME" />

        </div>

        <div id="slidingDiv1" style="display:none;">
            <div class="div6">
            <asp:CheckBox ID="chkScheduler" runat="server" Text="Úkoly a termíny v mém kalendáři" AutoPostBack="true" CssClass="chk" Checked="true" />
                </div>
            <div class="div6">
                <asp:CheckBox ID="chkSearch" runat="server" Text="Vyhledávání" AutoPostBack="true" CssClass="chk" Checked="false" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkLog" runat="server" Text="Poslední významnější akce" AutoPostBack="true" CssClass="chk" Checked="true" />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkShowCharts" runat="server" AutoPostBack="true" Text="Grafy z mých hodin" Checked="true" CssClass="chk"  />
            </div>
            <div class="div6">
                <asp:CheckBox ID="chkX18" runat="server" Text="Typy dokumentů určené pro domovské stránky uživatelů" AutoPostBack="true" CssClass="chk" Checked="false" Visible="false" />
            </div>
            
        </div>
        <div style="clear: both;"></div>
        <asp:PlaceHolder ID="place_j04DashboardHtml" runat="server"></asp:PlaceHolder>




        <div style="min-height: 430px;">
            <div style="float: left;">
                <asp:Panel ID="panSearch" runat="server" CssClass="content-box2">
                    <div class="title">
                        <img src="Images/search.png" />

                    </div>
                    <div class="content">
                        <asp:Panel ID="panSearch_P41" runat="server" Visible="false">

                            <telerik:RadComboBox ID="search_p41" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat projekt..." Width="250px" OnClientSelectedIndexChanged="project_OnClientSelectedIndexChanged" OnClientItemsRequesting="project_OnClientItemsRequesting">
                                <WebServiceSettings Method="LoadComboData" Path="~/Services/project_service.asmx" UseHttpGet="false" />
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <asp:Panel ID="panSearch_p28" runat="server" Style="margin-top: 6px;" Visible="false">

                            <telerik:RadComboBox ID="search_p28" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat klienta..." Width="250px" OnClientSelectedIndexChanged="contact_OnClientSelectedIndexChanged" OnClientItemsRequesting="contact_OnClientItemsRequesting">
                                <WebServiceSettings Method="LoadComboData" Path="~/Services/contact_service.asmx" UseHttpGet="false" />
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <asp:Panel ID="panSearch_p91" runat="server" Style="margin-top: 6px;" Visible="false">

                            <telerik:RadComboBox ID="search_p91" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat fakturu..." Width="250px" OnClientSelectedIndexChanged="invoice_OnClientSelectedIndexChanged" OnClientItemsRequesting="invoice_OnClientItemsRequesting">
                                <WebServiceSettings Method="LoadComboData" Path="~/Services/invoice_service.asmx" UseHttpGet="false" />
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <asp:Panel ID="panSearch_p56" runat="server" Style="margin-top: 6px;" Visible="false">

                            <telerik:RadComboBox ID="search_p56" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat úkol..." Width="250px" OnClientSelectedIndexChanged="task_OnClientSelectedIndexChanged" OnClientItemsRequesting="task_OnClientItemsRequesting">
                                <WebServiceSettings Method="LoadComboData" Path="~/Services/task_service.asmx" UseHttpGet="false" />
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <asp:Panel ID="panSearch_j02" runat="server" Style="margin-top: 6px;" Visible="false">

                            <telerik:RadComboBox ID="search_j02" runat="server" DropDownWidth="400" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" Text="Hledat osobu..." Width="250px" OnClientSelectedIndexChanged="person_OnClientSelectedIndexChanged" OnClientItemsRequesting="person_OnClientItemsRequesting">
                                <WebServiceSettings Method="LoadComboData" Path="~/Services/person_service.asmx" UseHttpGet="false" />
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <div class="div6">
                            <button id="linkFulltext" runat="server" type="button" onclick="fulltext2()" visible="false">FULL-TEXT hledání</button>

                        </div>

                    </div>
                </asp:Panel>



            </div>
            <div style="float: left;">

                <uc:myscheduler ID="cal1" runat="server" Prefix="j02" />



            </div>

            <asp:Panel ID="panX18" runat="server" CssClass="content-box1">
                <div class="title">
                    <img src="Images/notepad.png" />
                    Vybrané typy dokumentů
                </div>
                <div class="content">
                    <table cellpadding="6">
                        <asp:Repeater ID="rpX18" runat="server">
                            <ItemTemplate>
                                <tr class="trHover">
                                    <td style="width: 33px;">
                                        <asp:Image ID="img1" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="x18Name" runat="server" CssClass="val" Font-Italic="true"></asp:Label>
                                    </td>
                                    <td>
                                        <button type="button" runat="server" id="cmdNew">
                                            <img src="Images/new.png" />Nový</button>
                                    </td>
                                    <td>
                                        <asp:HyperLink ID="linkFramework" runat="server" Text="Přehled" NavigateUrl="#"></asp:HyperLink>
                                    </td>
                                    <td>
                                        <asp:HyperLink ID="linkCalendar" runat="server" Text="Kalendář" NavigateUrl="#"></asp:HyperLink>
                                    </td>
                                </tr>

                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </asp:Panel>

            <asp:Panel ID="panNoticeBoard" runat="server" CssClass="content-box1" Visible="false">
                <div class="title">
                    <img src="Images/article.png" />
                    <asp:Label ID="lblNoticeBoardHeader" runat="server"></asp:Label>

                </div>
                <div class="content" style="">

                    <div style="clear: both;"></div>
                    <table>
                        <tr valign="top">
                            <td style="max-width: 250px;">
                                <asp:Repeater ID="rpArticle" runat="server">
                                    <ItemTemplate>
                                        <div style="min-width: 200px; margin-bottom: 10px;">
                                            <div>
                                                <asp:Label ID="timestamp" runat="server" CssClass="timestamp"></asp:Label>
                                            </div>
                                            <asp:LinkButton ID="link1" runat="server"></asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                            <td style="max-width: 600px;" id="tdRecO23" runat="server" visible="false">
                                <div style="overflow: auto; max-height: 300px;">
                                    <div style="float: right;">
                                        <button type="button" title="Plný detail" class="button-link">
                                            <img src="Images/fullscreen.png" onclick="o23_fullscreen()" /></button>
                                    </div>
                                    <uc:o23_record_readonly ID="rec1" runat="server" />

                                    <uc:b07_list ID="comments1" runat="server" JS_Create="b07_create()" JS_Reaction="b07_reaction" ShowInsertButton="false" />

                                </div>

                            </td>
                        </tr>
                    </table>


                </div>
            </asp:Panel>

            <asp:Panel ID="panO23" runat="server" CssClass="content-box1">
                <div class="title">
                    <img src="Images/notepad.png" alt="Dokument" />
                    Dokumenty s připomenutím +-1 den
                    
                    <asp:Label ID="o23Count" runat="server" CssClass="badge1"></asp:Label>
                </div>
                <div class="content">
                    <asp:Repeater ID="rpO23" runat="server">
                        <ItemTemplate>
                            <div class="div6">
                                <asp:HyperLink ID="clue1" runat="server" CssClass="reczoom" Text="i" title="Detail dokumentu"></asp:HyperLink>
                                <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
                                <asp:Image ID="img1" runat="server" ImageUrl="Images/reminder.png" ToolTip="Připomenutí" />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="panP39" runat="server" CssClass="content-box1">
                <div class="title">
                    <img src="Images/worksheet_recurrence.png" />
                    Blízké generování opakovaných odměn/paušálů/úkonů
                    <asp:Label ID="p39Count" runat="server" CssClass="badge1"></asp:Label>
                </div>
                <div class="content">

                    <asp:Repeater ID="rpP39" runat="server">
                        <ItemTemplate>
                            <div class="div6">
                                <asp:HyperLink ID="cmdProject" runat="server" CssClass="value_link"></asp:HyperLink>

                                <asp:Label ID="p39Text" runat="server" Font-Italic="true"></asp:Label>
                            </div>
                            <div class="div6">
                                <span>Kdy generovat:</span>
                                <asp:Label ID="p39DateCreate" runat="server" ForeColor="red"></asp:Label>
                                <span>Datum úkonu:</span>
                                <asp:Label ID="p39Date" runat="server" ForeColor="green"></asp:Label>
                            </div>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </asp:Panel>
            <asp:Panel ID="panX47" runat="server" CssClass="content-box1" Style="margin-left: 30px;">
                <div class="title">
                    <img src="Images/timeline.png" />
                    Poslední významnější akce
                   
                    
                </div>
                <div class="content">
                    <table cellpadding="4">
                        <asp:Repeater ID="rpX47" runat="server">
                            <ItemTemplate>
                                <tr class="trHover" valign="top">
                                    <td>
                                        <asp:Image ID="img1" runat="server" />
                                        <asp:Label ID="lbl1" runat="server" CssClass="timestamp"></asp:Label>

                                    </td>
                                    <td>
                                        <asp:HyperLink ID="linkPP1" runat="server" CssClass="pp1"></asp:HyperLink>
                                    </td>
                                    <td>
                                        <asp:HyperLink ID="link1" runat="server" CssClass="value_link"></asp:HyperLink>
                                        <asp:Label ID="lbl2" runat="server"></asp:Label>
                                        <asp:Label ID="tags" runat="server"></asp:Label>
                                    </td>



                                    <td>

                                        <asp:Label ID="timestamp" runat="server" CssClass="timestamp"></asp:Label>
                                    </td>

                                </tr>

                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                    <hr />
                    <asp:CheckBox ID="chkP41" runat="server" Text="Projekty" AutoPostBack="true" Checked="true" Visible="false" />
                    <asp:CheckBox ID="chkP28" runat="server" Text="Klienti" AutoPostBack="true" Checked="true" Visible="false" />
                    <asp:CheckBox ID="chkP91" runat="server" Text="Faktury" AutoPostBack="true" Checked="false" Visible="false" />
                    <asp:CheckBox ID="chkP56" runat="server" Text="Úkoly" AutoPostBack="true" Checked="false" Visible="false" />
                    <asp:CheckBox ID="chkO23" runat="server" Text="Dokumenty" AutoPostBack="true" Checked="false" Visible="false" />
                    <asp:CheckBox ID="chkJ02" runat="server" Text="Kontaktní osoby" AutoPostBack="true" Checked="false" Visible="false" />
                </div>
            </asp:Panel>



            <asp:Panel ID="panChart1" runat="server" Style="float: right;" Visible="false">
                <telerik:RadHtmlChart runat="server" ID="chart1" Width="600px" Font-Size="Small">
                    <ChartTitle Text="Vykázané hodiny po dnech (14 dní dozadu)">
                    </ChartTitle>
                    <PlotArea>
                        <Series>
                            <telerik:ColumnSeries Name="Hodiny Fa" DataFieldY="HodinyFa" Stacked="true">
                                <Appearance FillStyle-BackgroundColor="LightGreen"></Appearance>
                            </telerik:ColumnSeries>
                            <telerik:ColumnSeries Name="Hodiny NeFa" DataFieldY="HodinyNeFa">
                                <Appearance FillStyle-BackgroundColor="#ff9999"></Appearance>
                            </telerik:ColumnSeries>
                        </Series>
                        <XAxis DataLabelsField="Datum">
                            <LabelsAppearance RotationAngle="90" DataFormatString="dd.MM. ddd"></LabelsAppearance>
                            <MinorGridLines Visible="false" />
                            <MajorGridLines Visible="false" />
                        </XAxis>
                        <YAxis>
                            <MinorGridLines Visible="false" />
                            <MajorGridLines Visible="false" />
                        </YAxis>
                    </PlotArea>
                </telerik:RadHtmlChart>
            </asp:Panel>
            <asp:Panel ID="panChart3" runat="server" Style="float: right;" Visible="false">
                <telerik:RadHtmlChart runat="server" ID="chart3" Width="400px" Height="700px" Font-Size="10px">
                    <ChartTitle Text="Vykázané hodiny po dnech (30 dní dozadu)">
                    </ChartTitle>
                    <PlotArea>
                        <Series>
                            <telerik:BarSeries Name="Hodiny Fa" DataFieldY="HodinyFa" Stacked="true">
                                <Appearance FillStyle-BackgroundColor="LightGreen"></Appearance>
                            </telerik:BarSeries>
                            <telerik:BarSeries Name="Hodiny NeFa" DataFieldY="HodinyNeFa">
                                <Appearance FillStyle-BackgroundColor="#ff9999"></Appearance>
                            </telerik:BarSeries>
                        </Series>
                        <XAxis DataLabelsField="Datum" Reversed="true">
                            <LabelsAppearance DataFormatString="dd.MM. ddd"></LabelsAppearance>
                            <MinorGridLines Visible="false" />
                            <MajorGridLines Visible="true" />
                        </XAxis>
                        <YAxis>
                            <MinorGridLines Visible="false" />
                            <MajorGridLines Visible="false" />
                        </YAxis>
                    </PlotArea>
                </telerik:RadHtmlChart>
            </asp:Panel>
            <asp:Panel ID="panChart2" runat="server" Style="float: right;" Visible="false">
                <telerik:RadHtmlChart runat="server" ID="chart2" Width="600px">
                    <Legend>
                        <Appearance Position="Right"></Appearance>
                    </Legend>
                    <PlotArea>
                        <Series>
                            <telerik:PieSeries NameField="Podle" DataFieldY="Hodiny" StartAngle="90">
                                <LabelsAppearance Position="OutsideEnd" DataFormatString="{0} h.">
                                </LabelsAppearance>
                            </telerik:PieSeries>
                        </Series>

                    </PlotArea>
                </telerik:RadHtmlChart>
            </asp:Panel>

            <div style="float: right;">

                <asp:Image runat="server" ID="imgWelcome" ImageUrl="Images/welcome/start.jpg" Visible="false" />
            </div>



            <div style="clear: both;"></div>

            <div style="margin-top: 20px;">
                <asp:Label ID="lblBuild" runat="server" Style="color: gray;" />
                <a href="about.aspx" style="margin-left: 20px;">O aplikaci</a>


                <span style="padding-left: 30px;">&nbsp</span>
                <asp:HyperLink ID="cmdReadUpgradeInfo" runat="server" NavigateUrl="log_app_update.aspx" ImageUrl="Images/upgraded_32.png" ToolTip="Nedávno proběhla aktualizace MARKTIME. Přečti si informace o novinkách a změnách v systému."></asp:HyperLink>

                <a href="log_app_update.aspx">Historie novinek a změn v systému</a>


            </div>
        </div>

    </div>

    <script type="text/javascript">
        $(document).ready(function () {

            handleSAW();

        });



        function contact_OnClientSelectedIndexChanged(sender, eventArgs) {
            var combo = sender;
            location.replace("p28_framework.aspx?pid=" + combo.get_value());
        }
        function contact_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
                context["flag"] = "searchbox";
            }
            function invoice_OnClientSelectedIndexChanged(sender, eventArgs) {
                var combo = sender;
                location.replace("p91_framework.aspx?pid=" + combo.get_value());
            }
            function invoice_OnClientItemsRequesting(sender, eventArgs) {
                var context = eventArgs.get_context();
                var combo = sender;

                if (combo.get_value() == "")
                    context["filterstring"] = eventArgs.get_text();
                else
                    context["filterstring"] = "";

                context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
                context["flag"] = "searchbox";
            }
            function task_OnClientSelectedIndexChanged(sender, eventArgs) {
                var combo = sender;
                location.replace("p56_framework.aspx?pid=" + combo.get_value());
            }
            function task_OnClientItemsRequesting(sender, eventArgs) {
                var context = eventArgs.get_context();
                var combo = sender;

                if (combo.get_value() == "")
                    context["filterstring"] = eventArgs.get_text();
                else
                    context["filterstring"] = "";

                context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
                context["flag"] = "searchbox";
            }
            function person_OnClientSelectedIndexChanged(sender, eventArgs) {
                var combo = sender;
                location.replace("j02_framework.aspx?pid=" + combo.get_value());
            }
            function person_OnClientItemsRequesting(sender, eventArgs) {
                var context = eventArgs.get_context();
                var combo = sender;

                if (combo.get_value() == "")
                    context["filterstring"] = eventArgs.get_text();
                else
                    context["filterstring"] = "";

                context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
                context["flag"] = "searchbox";
            }
            function project_OnClientSelectedIndexChanged(sender, eventArgs) {
                var combo = sender;
                location.replace("p41_framework.aspx?pid=" + combo.get_value());
            }
            function project_OnClientItemsRequesting(sender, eventArgs) {
                var context = eventArgs.get_context();
                var combo = sender;

                if (combo.get_value() == "")
                    context["filterstring"] = eventArgs.get_text();
                else
                    context["filterstring"] = "";

                context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
                context["j02id_explicit"] = "<%=Master.Factory.SysUser.j02ID%>";
                context["flag"] = "searchbox";
            }
    </script>
</asp:Content>

