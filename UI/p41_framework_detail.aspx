<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p41_framework_detail.aspx.vb" Inherits="UI.p41_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="contactpersons" Src="~/contactpersons.ascx" %>
<%@ Register TagPrefix="uc" TagName="entity_worksheet_summary" Src="~/entity_worksheet_summary.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields_readonly" Src="~/freefields_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="alertbox" Src="~/alertbox.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="treemenu" Src="~/treemenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="myscheduler" Src="~/myscheduler.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>
<%@ Register TagPrefix="uc" TagName="folder" Src="~/f01folder.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function b07_reaction(b07id) {
            sw_decide("b07_create.aspx?parentpid=" + b07id + "&masterprefix=p41&masterpid=<%=Master.datapid%>", "Images/comment.png", true)

        }
        function b07_delete(b07id, flag) {
            sw_decide("b07_delete.aspx?pid=" + b07id, "Images/delete.png", true)

        }
        function hardrefresh(pid, flag) {
            <%If menu1.PageSource <> "navigator" Then%>
            if (flag == "p41-save" || flag == "p41-create") {
                <%If menu1.PageSource = "3" Then%>
                location.replace("p41_framework_detail.aspx?pid=" + pid + "&source=3");
                <%Else%>
                parent.window.location.replace("p41_framework.aspx?pid=" + pid);
                <%End If%>
                return;
            }

            if (flag == "p41-delete") {
                parent.window.location.replace("p41_framework.aspx");
                return;
            }
            <%End If%>

            if (flag == "draft2normal") {
                document.getElementById('<%= cmdConvertDraft2Normal.ClientID%>').click();
                return;
            }




            location.replace("p41_framework_detail.aspx?pid=<%=master.datapid%>&source=<%=menu1.PageSource%>&tab=<%=menu1.CurrentTab%>");

        }
        function childs() {
            window.open("p41_framework.aspx?masterprefix=p41&masterpid=<%=master.DataPID%>", "_top")

        }
        function o23_record(pid) {

            window.open("o23_framework.aspx?pid=" + pid, "_top");

        }
        function batch_update_childs() {
            sw_decide("p41_batch_childs.aspx?pid=<%=master.datapid%>", "Images/batch.png", true)

        }
        function p40_record(p40id) {
            sw_decide("p40_record.aspx?p41id=<%=master.datapid%>&pid=" + p40id, "Images/worksheet_recurrence.png", true);
        }
        function p40_chrono(p40id) {
            sw_decide("p40_chrono.aspx?pid=" + p40id, "Images/worksheet_recurrence.png", true);
        }
        function re(pid, prefix) {
            if (prefix == 'o22')
                sw_decide("o22_record.aspx?pid=" + pid, "Images/datepicker.png")

            if (prefix == 'p56')
                window.open("p56_framework.aspx?pid=" + pid, "_top");

        }
        function wd(pid) {
            sw_decide("workflow_dialog.aspx?pid=" + pid + "&prefix=p56", "Images/workflow.png")
        }
        function ew(p56id) {
            sw_decide("p31_record.aspx?pid=0&p56id=" + p56id, "Images/worksheet.png")
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc:entity_menu ID="menu1" runat="server"></uc:entity_menu>

    <div style="height: 10px; clear: both;"></div>
    <div class="content-box1">
        <div class="title">
            <asp:Image ID="imgRecord" runat="server" Style="margin-right: 10px;" ImageUrl="Images/properties.png" />

            <asp:Label ID="boxCoreTitle" Text="Záznam projektu" runat="server" meta:resourcekey="boxCoreTitle"></asp:Label>
            <asp:Image ID="imgFavourite" runat="server" ImageUrl="Images/favourite.png" ToolTip="Můj oblíbený projekt" style="float:right;" Visible="false" />
            
            

        </div>
        <div class="content">



            <table cellpadding="10" cellspacing="2" id="responsive">

                <tr valign="baseline">
                    <td colspan="2">

                        <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
                        <asp:Image ID="imgFlag_Project" runat="server" />
                        <asp:Image ID="imgDraft" runat="server" ImageUrl="Images/draft_icon.gif" Visible="false" AlternateText="DRAFT záznam" Style="float: right;" />
                        <asp:Panel ID="panDraftCommands" runat="server" Visible="false">
                            <button type="button" onclick="draft2normal()">
                                Převést z režimu DRAFT na oficiální záznam
                            </button>
                        </asp:Panel>

                    </td>

                </tr>

                <tr valign="baseline">
                    <td style="min-width: 120px;">

                        <asp:Label ID="lblClient" runat="server" Text="Klient:" CssClass="lbl" meta:resourcekey="lblClient"></asp:Label>

                    </td>
                    <td>
                        <asp:HyperLink ID="pmClient" runat="server" NavigateUrl="#" CssClass="pp1"></asp:HyperLink>
                        <asp:HyperLink ID="Client" runat="server" NavigateUrl="#" Target="_top" CssClass="value_link"></asp:HyperLink>
                        <asp:HyperLink ID="clue_client" runat="server" CssClass="reczoom" Text="i" title="Detail klienta"></asp:HyperLink>
                        <asp:Image ID="imgFlag_Client" runat="server" />


                        <asp:Label ID="lblClientBilling" runat="server" Text="Odběratel faktury:" CssClass="lbl" Visible="false"></asp:Label>
                        <asp:HyperLink ID="ClientBilling" runat="server" NavigateUrl="#" Target="_top" Visible="false" CssClass="value_link"></asp:HyperLink>
                    </td>

                </tr>

                <tr id="trWorkflow" runat="server">
                    <td>
                        <asp:Label ID="lblB02ID" runat="server" Text="Workflow stav:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="b02Name" runat="server" CssClass="valboldred"></asp:Label>
                        <img src="Images/workflow.png" />
                        <asp:HyperLink ID="cmdWorkflow" runat="server" Text="Posunout/doplnit" NavigateUrl="javascript: workflow()"></asp:HyperLink>
                    </td>
                </tr>
                <tr id="trPlan" runat="server" style="vertical-align: top;">
                    <td>
                        <asp:Label ID="lblPlan" runat="server" Text="Zahájení/dokončení:" CssClass="lbl" meta:resourcekey="lblPlan"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="PlanPeriod" runat="server" CssClass="val"></asp:Label>
                        <div>


                            <asp:HyperLink ID="aP48" runat="server" NavigateUrl="javascript:p48_plan()" Text="Operativní plán projektu" meta:resourcekey="aP48"></asp:HyperLink>
                        </div>

                    </td>
                </tr>
                <tr id="trP51" runat="server">
                    <td style="vertical-align: top;">
                        <asp:Label ID="lblX51" runat="server" Text="Fakturační ceník:" CssClass="lbl" meta:resourcekey="lblX51"></asp:Label>
                    </td>
                    <td>

                        <asp:Label ID="p51Name_Billing" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HyperLink ID="clue_p51id_billing" runat="server" CssClass="reczoom" Text="i" title="Detail ceníku projektu"></asp:HyperLink>
                        <asp:Label ID="lblX51_Message" runat="server" CssClass="lbl"></asp:Label>



                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblP42Name" runat="server" Text="Typ projektu:" CssClass="lbl" meta:resourcekey="lblP42Name"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="p42Name" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HyperLink ID="clue_p42name" runat="server" CssClass="reczoom" Text="i" title="Detail typu projektu"></asp:HyperLink>

                        <asp:Label ID="lblJ18Name" runat="server" Text="Středisko:" CssClass="lbl" meta:resourcekey="lblJ18Name"></asp:Label>
                        <asp:Label ID="j18Name" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HyperLink ID="clue_j18name" runat="server" CssClass="reczoom" Text="i" title="Detail střediska"></asp:HyperLink>
                        <asp:Label ID="lblP61Name" runat="server" Text="Klastr aktivit:" CssClass="lbl" meta:resourcekey="lblP61Name"></asp:Label>
                        <asp:Label ID="p61Name" runat="server" CssClass="valbold"></asp:Label>
                        <asp:HyperLink ID="clue_p61Name" runat="server" CssClass="reczoom" Text="i" title="Detail klastru aktivit" Visible="false"></asp:HyperLink>
                        
                    </td>

                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Vlastník záznamu:"></asp:Label>

                    </td>
                    <td>
                        <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label>                        
                        <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link"></asp:HyperLink>
                    </td>
                </tr>

            </table>
            <uc:mytags ID="tags1" Prefix="p41" runat="server" />
            <uc:folder ID="folder1" Prefix="p41" runat="server" />

            <uc:treemenu ID="tree1" runat="server" Visible="false" />
            <asp:HyperLink ID="linkBatchUpdateChilds" runat="server" Text="[Aktualizovat nastavení pod-projektů]" NavigateUrl="javascript:batch_update_childs()" CssClass="wake_link" Visible="false"></asp:HyperLink>
        </div>
    </div>

    <asp:Panel ID="boxP40" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/worksheet_recurrence.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP40Title" runat="server" Text="Opakované odměny/paušály/úkony"></asp:Label>
        </div>
        <div class="content">
            <asp:Repeater ID="rpP40" runat="server">
                <ItemTemplate>
                    <div class="div6">
                        <asp:HyperLink ID="p40Name" runat="server"></asp:HyperLink>
                        <asp:HyperLink ID="clue_p40" runat="server" CssClass="reczoom" Text="i"></asp:HyperLink>

                        &nbsp;&nbsp;<asp:HyperLink ID="linkChrono" runat="server" Text="Plán generování"></asp:HyperLink>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>



    <asp:Panel ID="boxP30" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/person.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP30Title" runat="server" Text="Kontaktní osoby projektu" meta:resourcekey="boxP30Title"></asp:Label>
            <asp:HyperLink ID="cmdEditP30" runat="server" NavigateUrl="javascript:p30_binding()" CssClass="wake_link" Text="[Upravit]" Style="margin-left: 20px;"></asp:HyperLink>
        </div>
        <div class="content">
            <uc:contactpersons ID="persons1" runat="server"></uc:contactpersons>
        </div>
    </asp:Panel>

    <asp:Panel ID="boxFF" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/form.png" style="margin-right: 10px;" />
            <asp:Label ID="boxFFTitle" runat="server" Text="Uživatelská pole"></asp:Label>
            <asp:CheckBox ID="chkFFShowFilledOnly" runat="server" AutoPostBack="true" Text="Zobrazovat pouze vyplněná pole" />
        </div>
        <div class="content">
            <uc:freefields_readonly ID="ff1" runat="server" />
        </div>

    </asp:Panel>

    <asp:Panel ID="boxRoles" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/projectrole.png" style="margin-right: 10px;" />
            <asp:Label ID="boxRolesTitle" runat="server" Text="Projektové role" meta:resourcekey="boxRolesTitle"></asp:Label>
        </div>
        <div class="content">
            <uc:entityrole_assign_inline ID="roles_project" runat="server" EntityX29ID="p41Project" NoDataText="V projektu nejsou přiřazeny projektové role."></uc:entityrole_assign_inline>
        </div>
    </asp:Panel>

    
    <asp:Panel ID="boxP64" runat="server" CssClass="content-box1" Style="clear: both;" Visible="false">
        <div class="title">
            <img src="Images/binder.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP64Title" runat="server" Text="Šanony"></asp:Label>
            <button type="button" class="button-link" onclick="window.open('p64_framework.aspx?masterprefix=p41&masterpid=<%=Master.DataPID%>','_top')" title="Přepnout na plný přehled šanonů" style="float: right;">
                <img border="0" src="Images/fullscreen.png" /></button>
        </div>
        <div class="content">
            <asp:Repeater ID="rpP64" runat="server">
                <ItemTemplate>

                    <asp:HyperLink ID="p64Name" runat="server" Style="margin-right: 10px;"></asp:HyperLink>


                </ItemTemplate>
            </asp:Repeater>

        </div>
    </asp:Panel>
    <div style="clear: both;"></div>
    <asp:Panel ID="boxBillingMemo" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/billing.png" style="margin-right: 10px;" />
            <span>Fakturační poznámka projektu</span>
        </div>
        <div class="content" style="background-color:#ffffcc;" >
            <asp:Label ID="p41BillingMemo" runat="server" ForeColor="Black" Font-Italic="true" Font-Size="Larger" ></asp:Label>
        </div>
    </asp:Panel>
    <asp:Panel ID="panRecurrence" runat="server" CssClass="content-box1" Visible="false">
        <div class="title">
            <img src="Images/recurrence.png" style="margin-right: 10px;" />
            <span>Matka opakovaných projektů</span>
        </div>
        <div class="content">
            <div class="div6">
                <span>Typ opakování:</span>
                <asp:Label ID="RecurrenceType" runat="server" CssClass="valbold"></asp:Label>
            </div>
            <div class="div6">
                <span>Maska názvu nových projektů:</span>
                <asp:Label ID="p41RecurNameMask" runat="server" CssClass="valbold"></asp:Label>
            </div>
            <div class="div6">
                <span>Rozhodné datum:</span>                
                <asp:Label ID="p41RecurBaseDate" runat="server" CssClass="valbold" ForeColor="DarkOrange"></asp:Label>
            </div>
            <div class="div6">
                <span>Naposledy vygenerovaný potomek:</span>
                <asp:HyperLink ID="LastChild" runat="server" NavigateUrl="#" CssClass="value_link" Target="_top"></asp:HyperLink>
            </div>
            <div class="div6">
                <span>Generování příštího potomka:</span>
                <asp:Label ID="lblNextGen" runat="server" CssClass="valbold"></asp:Label>
            </div>
        </div>
    </asp:Panel>
    <uc:alertbox ID="alert1" runat="server"></uc:alertbox>

    <div style="clear: both; padding-top: 6px;">
        <div style="float: left;">
            <uc:myscheduler ID="cal1" runat="server" Prefix="p41" />
        </div>
        <asp:panel ID="boxX18" runat="server" cssclass="content-box1">
            <div class="title">
                <img src="Images/notepad.png"  /><span style="margin-right: 10px;">Dokumenty</span>
                <img src="Images/label.png" /><span>Kategorie</span>
            </div>
            <div class="content">
                <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>
            </div>

        </asp:panel>
    </div>
    
    <div style="clear:both;">
    <uc:b07_list ID="comments1" runat="server" JS_Create="menu_b07_record()" JS_Reaction="b07_reaction" />
    </div>
    
    <asp:Panel ID="boxP31Summary" runat="server" CssClass="content-box1" style="clear: both;">
        <div class="title">
            <img src="Images/worksheet.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP31SummaryTitle" runat="server" Text="WORKSHEET Summary"></asp:Label>
            
        </div>
        <div class="content">
            <uc:entity_worksheet_summary ID="p31summary1" runat="server"></uc:entity_worksheet_summary>

            <div style="width: 100%;">
                <span class="val">Poslední vystavená faktura:</span>
                <asp:HyperLink ID="linkLastInvoice" runat="server" CssClass="value_link" Target="_top"></asp:HyperLink>
                

            </div>
            <div style="width: 100%;">
                <span class="val">Poslední nevyfakturovaný úkon:</span>
                <asp:Label ID="Last_WIP_Worksheet" runat="server" ForeColor="Brown" Style="float: right;"></asp:Label>

            </div>
        </div>
    </asp:Panel>
    

    <asp:Button ID="cmdConvertDraft2Normal" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidCal1ShallBeActive" Value="1" runat="server" />
</asp:Content>
