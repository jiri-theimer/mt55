<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p91_framework_detail.aspx.vb" Inherits="UI.p91_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields_readonly" Src="~/freefields_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="alertbox" Src="~/alertbox.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugins/Plugin.css" />



    <script type="text/javascript">
        $(document).ready(function () {
            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });

            
        });

        function sw_decide(url, iconUrl, is_maximize) {
            var w=window.innerWidth;            
            if (w<1100){
                window.parent.sw_master(url, iconUrl, is_maximize);
                return;
            }
            
            <%If hidSource.Value = "2" Then%>
            window.parent.sw_master(url, iconUrl, is_maximize);
            <%Else%>
            sw_local(url, iconUrl, is_maximize);
            <%End If%>
            
        }

        function report(x31id) {
            
            sw_decide("report_modal.aspx?prefix=p91&pid=<%=Master.DataPID%>&x31id="+x31id,"Images/reporting.png",true);

        }

        function p31_entry(p34id) {
            
            sw_decide("p31_record.aspx?pid=0&p91id=<%=Master.DataPID%>&p34id="+p34id,"Images/worksheet.png",true);

        }

        function record_new() {
            
            sw_decide("p91_create.aspx","Images/invoice.png",true);

        }

        function record_edit() {
            var pid = <%=master.DataPID%>;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_decide("p91_record.aspx?pid=" + pid,"Images/invoice.png",true);

        }

        function record_new() {
            var pid = <%=master.DataPID%>;            
           
            sw_decide("p91_create_step1.aspx?prefix=p28","Images/invoice.png",true);
        }

        function changevat() {
         
            sw_decide("p91_change_vat.aspx?pid=<%=Master.DataPID%>","Images/recalc.png");

        }
        function changecurrency() {
         
            sw_decide("p91_change_currency.aspx?pid=<%=Master.DataPID%>","Images/recalc.png");

        }

        function pay() {
           
            sw_decide("p91_pay.aspx?pid=<%=Master.DataPID%>","Images/payment.png");

        }
        
        function p31_add() {           
            sw_decide("p91_add_worksheet_gateway.aspx?pid=<%=Master.DataPID%>","Images/worksheet.png");
        }
        function p31_batch(oper){       
            var p31ids=GetAllSelectedPIDs();
            if (p31ids==""){
                alert("Musíte vybrat minimálně jeden záznam.");
                return;
            }
            sw_decide("p91_remove_worksheet.aspx?oper="+oper+"&pid=<%=Master.DataPID%>&p31ids="+p31ids,"Images/cut.png");
        }
        
        function hardrefresh(pid, flag) {           
            if (flag=="p91-save" || flag=="workflow-dialog"){
                parent.window.location.replace("p91_framework.aspx?pid="+pid);
                return;
            }
            if (flag=="p91-delete"){
                parent.window.location.replace("p91_framework.aspx");
                return;
            }
            
            
            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

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
        

        function p31_RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid_p31.clientid%>").value = args.getDataKeyValue("pid");

        }

        function p31_RowDoubleClick(sender, args) {
            record_p31_edit();
        }

        function record_p31_edit() {
            var pid=document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            if (pid=="")
            {
                alert("Musíte vybrat položku z tabulky.");
                return;
            }
            sw_decide("p31_record_AI.aspx?pid="+pid,"Images/worksheet.png");

        }

     
        function o23_record(pid) {
            
            sw_decide("o23_record.aspx?masterprefix=p91&masterpid=<%=master.datapid%>&pid="+pid,"Images/notepad.png",true);

        }
        function o22_record(pid) {
            
            sw_decide("o22_record.aspx?masterprefix=p91&masterpid=<%=master.datapid%>&pid="+pid,"Images/calendar.png",true);

        }
       
       

        function b07_comment() {
            
            sw_decide("b07_create.aspx?masterprefix=p91&masterpid=<%=master.datapid%>","Images/comment.png",true)
            
        }
        function b07_reaction(b07id) {
            sw_decide("b07_create.aspx?parentpid="+b07id+"&masterprefix=p91&masterpid=<%=master.datapid%>","Images/comment.png", true)
           
        }
        function b07_delete(b07id, flag) {
            sw_decide("b07_delete.aspx?pid=" + b07id, "Images/delete.png", true)

        }
      
       
        function proforma() {           
            sw_decide("p91_proforma.aspx?pid=<%=Master.DataPID%>","Images/proforma.png");
        }
        function creditnote() {           
            sw_decide("p91_creditnote.aspx?pid=<%=Master.DataPID%>","Images/correction_down.gif");
        }
        function export_pohoda() {           
            sw_decide("p91_export2pohoda.aspx?pid=<%=Master.DataPID%>","Images/export.png");
        }
        function abo_import() {           
            sw_decide("p91_pay_aboimport.aspx","Images/payment.png");
        }
        function workflow(){            
            sw_decide("workflow_dialog.aspx?prefix=p91&pid=<%=master.datapid%>","Images/workflow.png",false);
        }
        function p31_grid(){            
            window.open("p31_grid.aspx?masterprefix=p91&masterpid=<%=Master.DataPID%>","_top")
        }
        function page_setting(){
            sw_decide("entity_framework_detail_setting.aspx?prefix=p91", "Images/setting.png",false);
        }
        function sendmail() {
            sw_decide("sendmail.aspx?prefix=p91&pid=<%=Master.DataPID%>", "Images/email.png")


        }
        function x40_record(x40id) {
            sw_decide("x40_record.aspx?pid="+x40id, "Images/email.png")

        }

        function cbxSearch_OnClientSelectedIndexChanged(sender, eventArgs){
            var combo = sender;
            var pid = combo.get_value();
            location.replace("p91_framework_detail.aspx?pid=" + pid+"&source=<%=Me.hidSource.Value%>");
        }
        function cbxSearch_OnClientItemsRequesting(sender, eventArgs){
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

        
            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";
            context["flag"] = "searchbox";
           
        }
        function cbxSearch_OnClientFocus(sender, args) {
            var combo = sender;
            var s = combo.get_text();
            if (s.indexOf("...")>0)
                combo.set_text("");
        }

        function menu_fullscreen(){
            <%If hidSource.Value = "3" Then%>
            location.replace("p91_framework.aspx?pid=<%=Master.DataPID%>");
            <%Else%>
            window.open("p91_framework_detail.aspx?pid=<%=Master.DataPID%>&saw=1","_blank");
            <%End If%>
            
        }
        function barcode() {
            sw_decide("barcode.aspx?prefix=p91&pid=<%=master.datapid%>", "Images/barcode.png", true);
        }
        function changelog() {
            window.top.sw_everywhere("changelog.aspx?prefix=p91&pid=<%=Master.DataPID%>", "Images/log.png")        
        }
        function reload(){
            location.replace("p91_framework_detail.aspx?pid=<%=Master.DataPID%>&source=<%=hidSource.Value%>","_blank");
        }
        function go2grid(){
            
            location.replace("p91_framework.aspx?pid=<%=Master.DataPID%>");
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panPM1" runat="server" CssClass="entity_menu_cm">
        <table style="padding: 0px; width: 100%;">
            <tr>
                <td style="width: 40px;">
                    <asp:HyperLink ID="pm1" runat="server" CssClass="pp2"></asp:HyperLink>
                </td>
                <td>
                    <asp:HyperLink ID="linkPM" runat="server" CssClass="entity_menu_header" NavigateUrl="#"></asp:HyperLink>
                    <button type="button" id="cmdGo2Grid" runat="server" onclick="go2grid()" style="font-size: 10px;" visible="false">
                        Zpět do přehledu
                    </button>
                </td>
                <td style="text-align: right;">
                    <button type="button" onclick="reload()" style="font-size: 10px;">
                        Obnovit stránku
                    </button>

                    <button type="button" id="cmdPageSetup" runat="server" onclick="page_setting()" style="font-size: 10px;">
                        Nastavení stránky                    
                    </button>
                </td>
                <td style="float: right; width: 40px;">

                    <asp:Image ID="Image1" runat="server" ImageUrl="Images/invoice_32.png" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <telerik:RadNavigation ID="menu1" runat="server" MenuButtonPosition="Right" Skin="Metro" EnableViewState="false">
        <Nodes>
            <telerik:NavigationNode ID="begin" Width="50px" Enabled="false" Visible="true">
            </telerik:NavigationNode>


            <telerik:NavigationNode Text="ZÁZNAM FAKTURY" ID="record">
                <Nodes>
                    <telerik:NavigationNode ID="cmdEdit" Text="Upravit kartu faktury" NavigateUrl="javascript:record_edit();" ImageUrl="Images/edit.png" ToolTip="Zahrnuje i možnost přesunutí do archivu nebo nenávratného odstranění."></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdCreateInvoice" Text="Vystavit novou fakturu" NavigateUrl="javascript:record_new();" ImageUrl="Images/new.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdPay" Text="Zapsat úhradu faktury" NavigateUrl="javascript:pay();" ImageUrl="Images/payment.png" ToolTip="Je možné zapsat úplnou nebo částečnou úhradu faktury."></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdAppendWorksheet" Text="Přidat do faktury další položky (úkony)" NavigateUrl="javascript:p31_add();" ImageUrl="Images/worksheet.png" ToolTip="Způsob, jak do faktury přidat slevu, přirážku, fixní odměnu, další časové úkony, výdaje apod."></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdChangeCurrency" Text="Převést fakturu na jinou měnu" NavigateUrl="javascript:changecurrency();" ImageUrl="Images/recalc.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdChangeVat" Text="Převést fakturu kompletně na jinou DPH sazbu" NavigateUrl="javascript:changevat();" ImageUrl="Images/recalc.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdProforma" Text="Spárovat fakturu s uhrazenou zálohou" NavigateUrl="javascript:proforma();" ImageUrl="Images/proforma.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdCreditNote" Text="Vytvořit k faktuře opravný doklad" NavigateUrl="javascript:creditnote();" ImageUrl="Images/correction_down.gif"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdReport" Text="Tisková sestava/pdf/e-mail" NavigateUrl="javascript:report('');" ImageUrl="Images/report.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdEmail" Text="Odeslat e-mail" NavigateUrl="javascript:sendmail();" ImageUrl="Images/email.png"></telerik:NavigationNode>
                </Nodes>

            </telerik:NavigationNode>
            <telerik:NavigationNode Text="DALŠÍ" ID="more">
                <Nodes>
                    <telerik:NavigationNode ID="cmdTags" Text="Štítky" ImageUrl="Images/tag.png" NavigateUrl="javascript:tags_assign()">
                    </telerik:NavigationNode>



                    <telerik:NavigationNode ID="cmdPivot" Text="WORKSHEET statistika faktury" NavigateUrl="javascript:report('');" Target="_top" ImageUrl="Images/pivot.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdX40" Text="Historie odeslané pošty" Target="_top" ImageUrl="Images/email.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdO23" Text="Vytvořit dokument" NavigateUrl="javascript:o23_record(0);" ImageUrl="Images/notepad.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdO22" Text="Zapsat událost do kalendáře" NavigateUrl="javascript:o22_record(0);" ImageUrl="Images/calendar.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdB07" Text="Zapsat komentář/poznámku" NavigateUrl="javascript:b07_comment();" ImageUrl="Images/comment.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdAboImport" Text="Import úhrad z ABO souboru" NavigateUrl="javascript:abo_import();" ImageUrl="Images/payment.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdPohoda" Text="Export do účetnictví POHODA" NavigateUrl="javascript:export_pohoda();" ImageUrl="Images/license.png"></telerik:NavigationNode>
                    <telerik:NavigationNode ID="cmdLog" Text="Historie záznamu" NavigateUrl="javascript:changelog();" ImageUrl="Images/log.png"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="cmdBarCode" Text="Čárový kód" NavigateUrl="javascript:barcode();" ImageUrl="Images/barcode.png"></telerik:NavigationNode>
                </Nodes>


            </telerik:NavigationNode>
            <telerik:NavigationNode ID="thePage" Text="STRÁNKA">
                <Nodes>
                    <telerik:NavigationNode ID="switchHeight" Text="Nastavení vzhledu stránky" ImageUrl="Images/setting.png" NavigateUrl="javascript:page_setting()">
                    </telerik:NavigationNode>
                    <telerik:NavigationNode ID="fs" NavigateUrl="javascript:menu_fullscreen()" ImageUrl="Images/fullscreen.png" Text="Otevřít v nové záložce"></telerik:NavigationNode>

                    <telerik:NavigationNode ID="reload" ImageUrl="Images/refresh.png" Text="Obnovit stránku"></telerik:NavigationNode>
                </Nodes>
            </telerik:NavigationNode>


            <telerik:NavigationNode ID="searchbox">
                <NodeTemplate></NodeTemplate>
            </telerik:NavigationNode>

        </Nodes>

    </telerik:RadNavigation>
    <asp:Image ID="imgIcon32" runat="server" ImageUrl="Images/invoice_32.png" Style="position: absolute; top: 5px; left: 5px;" />




    <div style="clear: both;"></div>




    <telerik:RadTabStrip ID="tabs1" runat="server" ShowBaseLine="true" Width="100%" Skin="Default" MultiPageID="RadMultiPage1" Style="margin-top: 10px;">
        <Tabs>
            <telerik:RadTab Text="&nbsp;&nbsp;&nbsp;Záznam faktury" Value="detail" Selected="true" ImageUrl="Images/properties.png"></telerik:RadTab>
            <telerik:RadTab Text="&nbsp;&nbsp;&nbsp;Položky faktury" Value="p31" ImageUrl="Images/worksheet.png"></telerik:RadTab>
            <telerik:RadTab Text="&nbsp;&nbsp;&nbsp;Další " Value="other" ImageUrl="Images/more.png"></telerik:RadTab>

        </Tabs>
    </telerik:RadTabStrip>
    <div style="min-height: 150px;">
        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" RenderMode="Lightweight">
            <telerik:RadPageView ID="core" runat="server" Selected="true">
                <asp:Image ID="imgDocType" runat="server" Style="position: absolute; top: 90px; left: 280px;" />
                <table cellpadding="10" cellspacing="2" id="responsive">
                    <tr valign="baseline">
                        <td style="min-width: 120px;" id="rlbl">
                            <asp:Label ID="lblCode" runat="server" Text="Číslo dokladu:" CssClass="lbl"></asp:Label>
                        </td>
                        <td style="min-width: 200px; max-width: 350px;">
                            <asp:Image ID="imgRecord" runat="server" Visible="false" />
                            <asp:Label ID="p91Code" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Button ID="cmdConvertDraft" runat="server" CssClass="cmd" Text="Převést Draft na oficiální číslo" />

                        </td>
                        <td id="rlbl">
                            <asp:Label ID="lblProject" runat="server" Text="Projekt:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Panel ID="panProjects" runat="server" Style="padding-right: 20px;">
                                <asp:Repeater ID="rpProject" runat="server">
                                    <ItemTemplate>
                                        <div>
                                            <asp:HyperLink ID="pm1" runat="server" CssClass="pp1" ToolTip="Fakturovaný projekt"></asp:HyperLink>
                                            <asp:HyperLink ID="p41Name" runat="server" NavigateUrl="#" Target="_parent" CssClass="value_link"></asp:HyperLink>
                                            <asp:HyperLink ID="clue_project" runat="server" CssClass="reczoom" Text="i" title="Detail projektu"></asp:HyperLink>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>

                        </td>
                    </tr>
                    <tr valign="top">

                        <td>
                            <asp:HyperLink ID="cmdReportInvoice" runat="server" Text="Sestava dokladu"></asp:HyperLink>

                        </td>
                        <td>
                            <asp:HyperLink ID="cmdReportAttachment" runat="server" Text="Sestava přílohy"></asp:HyperLink>
                            <div>
                                <asp:HyperLink ID="cmdReportLetter" runat="server" Text="Průvodní dopis"></asp:HyperLink>
                            </div>
                        </td>
                        <td id="rlbl">

                            <asp:Label ID="lblClient" runat="server" Text="Klient:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:HyperLink ID="pm1Client" runat="server" CssClass="pp1" ToolTip="Klient faktury"></asp:HyperLink>
                            <asp:HyperLink ID="Client" runat="server" NavigateUrl="#" Target="_parent" CssClass="value_link"></asp:HyperLink>
                            <asp:HyperLink ID="clue_client" runat="server" CssClass="reczoom" Text="i" title="Detail klienta"></asp:HyperLink>
                            <asp:Label ID="p91ClientPerson" runat="server" ToolTip="Kontaktní osoba klienta"></asp:Label>

                        </td>


                    </tr>
                    <tr id="trSourceCode" runat="server" visible="false">
                        <td>
                            <asp:Label ID="lblSourceCode" runat="server" Text="Opravovaný doklad:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>

                            <asp:HyperLink ID="SourceLink" runat="server" CssClass="value_link" Target="_top"></asp:HyperLink>
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td id="rlbl">Typ faktury:
                        </td>
                        <td>
                            <asp:Label ID="p92Name" runat="server" CssClass="valbold"></asp:Label>
                            <asp:HyperLink ID="clue_p92name" runat="server" CssClass="reczoom" Text="i" title="Detail typu faktury" Style="margin-right: 80px;"></asp:HyperLink>
                        </td>
                        <td id="rlbl">Fakturační adresa:

                        </td>
                        <td>
                            <asp:Label ID="BillingAddress" runat="server" CssClass="valbold"></asp:Label></td>
                    </tr>
                    <tr id="trWorkflow" runat="server">
                        <td>
                            <asp:Label ID="lblB02ID" runat="server" Text="Workflow stav:" CssClass="lbl"></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:Label ID="b02Name" runat="server" CssClass="valboldred"></asp:Label>
                            <br />
                            <button type="button" onclick="workflow()" class="cmd" title="Změnit stav faktury, zapsat komentář, případně další kroky, které podporuje aktuální workflow šablona...">
                                <img src="Images/workflow.png" />
                                Posunout/doplnit</button>
                        </td>
                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="lblDebt" runat="server" Text="Celkový dluh:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">

                            <asp:Label ID="p91Amount_Debt" runat="server" CssClass="valboldred"></asp:Label>
                            <asp:Label ID="j27Code_debt" runat="server" CssClass="val" Style="margin-right: 60px;"></asp:Label>
                        </td>
                        <td id="rlbl">
                            <asp:Label ID="Label12" runat="server" Text="Doručovací adresa:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="PostAddress" runat="server" CssClass="valbold"></asp:Label></td>
                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="Label2" runat="server" Text="Částka bez DPH:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">

                            <asp:Label ID="p91Amount_WithoutVat" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Label ID="j27Code_withoutvat" runat="server" CssClass="val" Style="margin-right: 60px;"></asp:Label>
                        </td>
                        <td id="rlbl">
                            <asp:Label ID="Label10" runat="server" Text="Bankovní účet:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="BankAccount" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Label ID="BankName" runat="server" CssClass="val" Style="padding-left: 20px;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="Label3" runat="server" Text="Částka DPH:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">

                            <asp:Label ID="p91Amount_Vat" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Label ID="j27Code_vat" runat="server" CssClass="val" Style="margin-right: 60px;"></asp:Label>
                        </td>
                        <td id="rlbl">
                            <asp:Label ID="Label6" runat="server" Text="Datum vystavení:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p91Date" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="Label4" runat="server" Text="Částka vč.DPH:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">

                            <asp:Label ID="p91Amount_WithVat" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Label ID="j27Code_withvat" runat="server" CssClass="val" Style="margin-right: 60px;"></asp:Label>
                        </td>
                        <td id="rlbl">
                            <asp:Label ID="Label7" runat="server" Text="Datum plnění:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p91DateSupply" runat="server" CssClass="valbold"></asp:Label>

                            <asp:Label ID="lblExchangeRate" runat="server" Text="Měnový kurz:" CssClass="lbl" Style="margin-left: 30px;"></asp:Label>
                            <asp:Label ID="p91ExchangeRate" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Button ID="cmdRecalcExchangeRate" runat="server" CssClass="cmd" Text="Aktualizovat kurz" Visible="false" />
                        </td>

                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="Label13" runat="server" Text="Zaokrouhlení:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="p91RoundFitAmount" runat="server" CssClass="valbold" Style="margin-right: 80px;"></asp:Label>

                        </td>
                        <td id="rlbl">
                            <asp:Label ID="lblp91DateMaturity" runat="server" Text="Datum splatnosti:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p91DateMaturity" runat="server" CssClass="valbold"></asp:Label>
                            <asp:HyperLink ID="SourceCode" runat="server" Visible="false"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td id="rlbl">
                            <asp:Label ID="lblProforma" runat="server" Text="Částka zálohy:" CssClass="lbl"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="p91ProformaAmount" runat="server" CssClass="valboldblue"></asp:Label>
                            <asp:Label ID="j27Code_proforma" runat="server" CssClass="val" Style="margin-right: 60px;"></asp:Label>
                        </td>
                        <td id="rlbl">
                            <asp:Label ID="lblp91DateBilled" runat="server" Text="Datum poslední úhrady:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p91DateBilled" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">

                            <asp:CheckBox ID="chkFFShowFilledOnly" runat="server" AutoPostBack="true" Text="Zobrazovat pouze vyplněná pole" />

                            <uc:freefields_readonly ID="ff1" runat="server" />

                        </td>
                    </tr>
                </table>

                <div class="div6">
                    <uc:plugin_datatable ID="plug1" TableID="tab1" runat="server"
                        ColHeaders="Položka|Částka bez DPH|DPH%|Částka DPH|Částka vč. DPH|" NoDataMessage="Žádná data."
                        ColHideRepeatedValues="0" ColTypes="S|N|N|N|N|S" ColFlexSubtotals="0|11|0|11|11|0"
                        TableCaption="" />
                </div>

                <asp:Panel ID="panText1" runat="server" CssClass="content-box1">
                    <div class="title">Text faktury</div>
                    <div class="content" style="background-color: #ffffcc;">
                        <asp:Label ID="p91Text1" runat="server" CssClass="val" Style="font-family: 'Courier New'; word-wrap: break-word; display: block; font-size: 120%;"></asp:Label>
                    </div>
                </asp:Panel>


                <asp:Panel ID="boxX18" runat="server" CssClass="content-box1">
                    <div class="title">
                        <img src="Images/label.png" style="margin-right: 10px;" /><span>Kategorie</span>
                    </div>
                    <div class="content">
                        <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>
                    </div>

                </asp:Panel>
                <div class="div6" style="clear: both;">
                    <uc:mytags ID="tags1" runat="server" Prefix="p91" />
                    <asp:HyperLink ID="linkLastX40" runat="server" Visible="false" Style="margin-left: 40px; font-weight: bold;" ToolTip="Pokus o odeslání faktury elektronicky"></asp:HyperLink>
                </div>


                <uc:b07_list ID="comments1" runat="server" ShowHeader="false" ShowInsertButton="false" JS_Reaction="b07_reaction" />
                <uc:alertbox ID="alert1" runat="server"></uc:alertbox>
                <div class="div6" style="clear: both;">
                    <asp:Label ID="p92ReportConstantText" runat="server" CssClass="val"></asp:Label>
                </div>
                <div class="div6">
                    <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link" NavigateUrl="javascript:changelog()" ToolTip="CHANGE-LOG"></asp:HyperLink>
                </div>
            </telerik:RadPageView>
            <telerik:RadPageView ID="p31" runat="server">

                <div class="div6">
                    <asp:RadioButtonList ID="opgGroupBy" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                        <asp:ListItem Text="Bez souhrnů" Value="flat" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Fakturační oddíl" Value="p95"></asp:ListItem>
                        <asp:ListItem Text="Fakturační status" Value="p70"></asp:ListItem>
                        <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                        <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                        <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                        <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                        <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                        <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div class="commandcell">
                    <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                        <asp:ListItem Text="20" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="50"></asp:ListItem>
                        <asp:ListItem Text="100"></asp:ListItem>
                    </asp:DropDownList>

                    <uc:mygrid ID="designer1" runat="server" Prefix="p31" MasterPrefix="p91" MasterPrefixFlag="2" Width="250px"></uc:mygrid>

                </div>
                <div class="commandcell">
                    <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" ClickToOpen="true" Style="z-index: 100;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None">
                        <Items>
                            <telerik:RadMenuItem Text="Přidat do faktury položku" Value="new" NavigateUrl="javascript:p31_add()" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Vybrané (zaškrtlé) položky" Value="akce" ImageUrl="Images/menuarrow.png">
                                <Items>
                                    <telerik:RadMenuItem Text="Vybrané vyjmout z faktury" Value="remove" NavigateUrl="javascript:p31_batch('cut')"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Vybrané -> zahrnout do paušálu" Value="p70-6" NavigateUrl="javascript:p31_batch('batch-6')"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Vybrané -> viditelný odpis" Value="p70-2" NavigateUrl="javascript:p31_batch('batch-2')"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Vybrané -> skrytý odpis" Value="p70-3" NavigateUrl="javascript:p31_batch('batch-3')"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                    <telerik:RadMenuItem Text="Zobrazit přehled na celou stránku" Value="fullscreen" Target="_top" NavigateUrl="p31_grid.aspx?masterprefix=p91&masterpid=<%=Master.DataPID%>"></telerik:RadMenuItem>
                                </Items>
                            </telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenu>
                </div>
                <div style="clear: both;"></div>
                <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="p31_RowSelected" OnRowDblClick="p31_RowDoubleClick"></uc:datagrid>

            </telerik:RadPageView>
            <telerik:RadPageView ID="other" runat="server">
                <div class="content-box2">
                    <div class="title">Údaje klienta uložené ve faktuře</div>
                    <div class="content">
                        <asp:Label ID="p91Client" runat="server" CssClass="valbold"></asp:Label>
                        <div>
                            <asp:Label ID="ClientIDs" runat="server" CssClass="val"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="ClientAddress" runat="server" CssClass="val"></asp:Label>
                        </div>
                    </div>
                </div>

                <table cellpadding="10" cellspacing="2" id="responsive">
                    <tr>
                        <td style="min-width: 120px;">
                            <asp:Label ID="Label5" runat="server" Text="Vlastník záznamu:" CssClass="lbl"></asp:Label>
                        </td>
                        <td style="min-width: 200px; max-width: 350px;">

                            <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblp91Datep31_From" runat="server" Text="Worksheet časový rámec:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="WorksheetRange" runat="server" CssClass="valbold"></asp:Label>
                        </td>

                    </tr>

                </table>

                <div class="content-box2">
                    <div class="title">Technický text faktury</div>
                    <div class="content" style="background-color: #ffffcc;">
                        <asp:Label ID="p91Text2" runat="server" Style="font-family: 'Courier New'; word-wrap: break-word; display: block; font-size: 120%;"></asp:Label>
                    </div>
                </div>


                <asp:Panel ID="panRoles" runat="server" CssClass="content-box1">
                    <div class="title">
                        <img src="Images/projectrole.png" style="margin-right: 10px;" />
                        Obsazení fakturačních rolí
                    </div>
                    <div class="content">
                        <uc:entityrole_assign_inline ID="roles1" runat="server" EntityX29ID="p91Invoice" NoDataText=""></uc:entityrole_assign_inline>
                    </div>
                </asp:Panel>


            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </div>






    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />

    <asp:HiddenField ID="hidCols" runat="server" />
    <asp:HiddenField ID="hidFrom" runat="server" />
    <asp:HiddenField ID="hidParentWidth" runat="server" />
    <asp:HiddenField ID="hidSource" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />


</asp:Content>
