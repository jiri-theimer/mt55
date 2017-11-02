<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p28_framework_detail.aspx.vb" Inherits="UI.p28_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="p28_address" Src="~/p28_address.ascx" %>
<%@ Register TagPrefix="uc" TagName="p28_medium" Src="~/p28_medium.ascx" %>
<%@ Register TagPrefix="uc" TagName="contactpersons" Src="~/contactpersons.ascx" %>
<%@ Register TagPrefix="uc" TagName="entity_worksheet_summary" Src="~/entity_worksheet_summary.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields_readonly" Src="~/freefields_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="alertbox" Src="~/alertbox.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="treemenu" Src="~/treemenu.ascx" %>
<%@ Register TagPrefix="uc" TagName="myscheduler" Src="~/myscheduler.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function b07_reaction(b07id) {
            sw_decide("b07_create.aspx?parentpid=" + b07id + "&masterprefix=p28&masterpid=<%=Master.datapid%>", "Images/comment.png", true)

        }
        function b07_delete(b07id, flag) {
            sw_decide("b07_delete.aspx?pid=" + b07id, "Images/delete.png", true)

        }

        function hardrefresh(pid, flag) {

            if (flag == "p28-save" || flag == "p28-create") {
                <%If menu1.PageSource = "3" Then%>
                location.replace("p28_framework_detail.aspx?pid=" + pid + "&source=3");
                <%Else%>
                parent.window.location.replace("p28_framework.aspx?pid=" + pid);
                <%End If%>
                return;
            }

            if (flag == "p28-delete") {
                parent.window.location.replace("p28_framework.aspx");
                return;
            }
            if (flag == "draft2normal") {
                document.getElementById('<%= cmdConvertDraft2Normal.ClientID%>').click();
                return;
            }

            location.replace("p28_framework_detail.aspx?pid=<%=master.datapid%>&source=<%=menu1.PageSource%>");
        }
        function childs() {
            window.open("p28_framework.aspx?masterprefix=p28&masterpid=<%=master.DataPID%>", "_top")

        }
        function vat_info(vat) {
            sw_decide("vat_registration.aspx?vat=" + vat, "Images/help.png", false);

        }
        function o48_edit() {
            sw_decide("o48_record.aspx?prefix=p28&pid=<%=master.datapid%>", "", false);
        }
        function o23_record(pid) {

            window.open("o23_framework.aspx?pid=" + pid, "_top");

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
            <asp:Label ID="boxCoreTitle" Text="Klient" runat="server"></asp:Label>

            <asp:CheckBox ID="chkFFShowFilledOnly" runat="server" AutoPostBack="true" Text="Pouze vyplněná uživatelská pole" Style="float: right;" />

        </div>
        <div class="content">
            <div style="float: left;">
                <table cellpadding="10" cellspacing="2" id="responsive">

                    <tr valign="top">

                        <td colspan="2">

                            <asp:Label ID="Contact" runat="server" CssClass="valbold"></asp:Label>

                            <asp:Label ID="p29Name" runat="server" CssClass="val"></asp:Label>
                            <asp:Image ID="imgDraft" runat="server" ImageUrl="Images/draft_icon.gif" Visible="false" AlternateText="DRAFT záznam" Style="float: right;" />
                            <asp:Panel ID="panDraftCommands" runat="server" Visible="false">
                                <button type="button" onclick="draft2normal()">
                                    Převést z režimu DRAFT na oficiální záznam
                                </button>
                            </asp:Panel>

                        </td>


                    </tr>

                    <tr id="trWorkflow" runat="server" visible="false">
                        <td>
                            <asp:Label ID="lblB02ID" runat="server" Text="Workflow stav:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="b02Name" runat="server" CssClass="valboldred"></asp:Label>
                            <img src="Images/workflow.png" />
                            <asp:HyperLink ID="cmdWorkflow" runat="server" Text="Posunout/doplnit" NavigateUrl="javascript: workflow()"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Label ID="lblX51" runat="server" Text="Fakturační ceník:" CssClass="lbl" meta:resourcekey="lblX51"></asp:Label>

                        </td>
                        <td>
                            <asp:Label ID="p51Name_Billing" runat="server" CssClass="valbold"></asp:Label>
                            <asp:HyperLink ID="clue_p51id_billing" runat="server" CssClass="reczoom" Text="i" title="Detail ceníku"></asp:HyperLink>
                            <asp:Image ID="imgFlag_Contact" runat="server" ToolTip="Fakturační jazyk" />
                        </td>

                    </tr>
                    <tr valign="top" id="trICDIC" runat="server">
                        <td>
                            <asp:Localize ID="locIC" runat="server" Text="IČ:" meta:resourcekey="locIC"></asp:Localize>
                           

                        </td>
                        <td>
                            <asp:HyperLink ID="linkIC" runat="server" Target="_blank" ToolTip="JUSTICE.cz" Visible="false"></asp:HyperLink>
                            <asp:HyperLink ID="linkARES" runat="server" Text="[ARES]" Target="_blank" Visible="false"></asp:HyperLink>

                            <asp:Label ID="lblDIC" runat="server" Text="DIČ:" meta:resourcekey="lblDIC" style="margin-left: 20px;"></asp:Label>
                            
                            <asp:HyperLink ID="linkDIC" runat="server" ToolTip="Ověření subjektu v DPH registrech" Visible="false"></asp:HyperLink>



                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Label ID="lblOwner" runat="server" CssClass="lbl" Text="Vlastník záznamu:" meta:resourcekey="lblOwner"></asp:Label>

                        </td>
                        <td>
                            <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label>
                            <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
                <uc:mytags ID="tags1" runat="server" Prefix="p28" />
                <uc:treemenu ID="tree1" runat="server" Visible="false" />
            </div>
            <div style="float: left;">
                <uc:freefields_readonly ID="ff1" runat="server" />

                <asp:HyperLink ID="linkISIR" runat="server" Text="[ISIR]" Target="_blank" Visible="false" CssClass="wake_link" ToolTip="Insolvenční restřík | JUSTICE.cz"></asp:HyperLink>
                +Monitoring:
                            <asp:HyperLink ID="linkISIR_Monitoring" runat="server" Text="NE" CssClass="wake_link" NavigateUrl="javascript:o48_edit()" ToolTip="Zapnout monitoring klienta v insolvenčním rejstříku"></asp:HyperLink>
            </div>


        </div>


    </div>





    <asp:Panel ID="panRoles" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/projectrole.png" style="margin-right: 10px;" />
            <asp:Localize ID="locObsazeniRoli" runat="server" text="Obsazení klientských rolí" meta:resourcekey="locObsazeniRoli"></asp:Localize>
        </div>
        <div class="content">
            <uc:entityrole_assign_inline ID="roles1" runat="server" EntityX29ID="p28Contact" NoDataText=""></uc:entityrole_assign_inline>
        </div>
    </asp:Panel>

    <asp:Panel ID="boxO37" runat="server" CssClass="content-box1" Style="overflow: auto; max-height: 300px;">
        <div class="title">
            <img src="Images/address.png" />
            <img src="Images/person.png" />
            <img src="Images/email.png" style="margin-right: 10px;" />
            <asp:Label ID="boxO37Title" runat="server" Text="Adresy a kontakty" meta:resourcekey="boxO37Title"></asp:Label>
        </div>
        <div class="content">
            <uc:p28_address ID="address1" runat="server"></uc:p28_address>
            <uc:p28_medium ID="medium1" runat="server"></uc:p28_medium>
            <uc:contactpersons ID="persons1" runat="server"></uc:contactpersons>
        </div>
    </asp:Panel>




    <asp:Panel ID="boxBillingMemo" runat="server" CssClass="content-box1" Style="clear: both;">
        <div class="title">
            <img src="Images/billing.png" style="margin-right: 10px;" />
            <span>Fakturační poznámka klienta</span>
        </div>
        <div class="content" style="background-color: #ffffcc;">
            <asp:Label ID="p28BillingMemo" runat="server" ForeColor="Black" Font-Italic="true" Font-Size="Larger"></asp:Label>
        </div>
    </asp:Panel>


    <uc:alertbox ID="alert1" runat="server"></uc:alertbox>

    <div style="clear: both; padding-top: 6px;">
        <div style="float: left;">
            <uc:myscheduler ID="cal1" runat="server" Prefix="p28" />
        </div>
        <asp:Panel ID="boxX18" runat="server" CssClass="content-box1">
            <div class="title">
                <img src="Images/notepad.png" /><span style="margin-right: 10px;">Dokumenty</span>
                <img src="Images/label.png" /><span>Kategorie</span>
            </div>
            <div class="content">
                <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>
            </div>

        </asp:Panel>
    </div>



    <asp:Panel ID="boxP41" runat="server" CssClass="content-box1" Style="clear: both;">
        <div class="title">
            <img src="Images/project.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP41Title" runat="server" Text="Otevřené projekty klienta" meta:resourcekey="boxP41Title"></asp:Label>

        </div>
        <asp:Panel ID="panProjects" runat="server" CssClass="content" Style="overflow: auto; max-height: 200px;">

            <asp:Repeater ID="rpP41" runat="server">
                <ItemTemplate>
                    <div style="padding: 5px; float: left;">
                        
                        <asp:HyperLink ID="linkPP1" runat="server" CssClass="pp1"></asp:HyperLink>
                        <asp:HyperLink ID="aProject" runat="server" Target="_top" CssClass="value_link"></asp:HyperLink>

                    </div>
                </ItemTemplate>
            </asp:Repeater>


        </asp:Panel>

    </asp:Panel>


    <div style="clear: both;">
        <uc:b07_list ID="comments1" runat="server" JS_Create="menu_b07_record()" JS_Reaction="b07_reaction" />
    </div>

    <asp:Panel ID="boxP31Summary" runat="server" CssClass="content-box1" Style="clear: both;">
        <div class="title">
            <img src="Images/worksheet.png" style="margin-right: 10px;" />
            <asp:Label ID="boxP31SummaryTitle" runat="server" Text="WORKSHEET Summary"></asp:Label>
        </div>
        <div class="content">
            <uc:entity_worksheet_summary ID="p31summary1" runat="server"></uc:entity_worksheet_summary>

            <div style="width: 100%;">
                
                <asp:Localize ID="locPosledniFaktura" runat="server" Text="Poslední vystavená faktura:" meta:resourcekey="locPosledniFaktura"></asp:Localize>
                
                
                <asp:HyperLink ID="linkLastInvoice" runat="server" CssClass="value_link" Target="_top"></asp:HyperLink>
            </div>
            <div style="width: 100%;">
                <asp:Localize ID="locPosledniNevyfakturovanyUkon" runat="server" Text="Poslední nevyfakturovaný úkon:" meta:resourcekey="locPosledniNevyfakturovanyUkon"></asp:Localize>
                
                
                <asp:Label ID="Last_WIP_Worksheet" runat="server" ForeColor="Brown" Style="float: right;"></asp:Label>

            </div>
        </div>
    </asp:Panel>


    <asp:Button ID="cmdConvertDraft2Normal" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidCal1ShallBeActive" Value="1" runat="server" />
</asp:Content>
