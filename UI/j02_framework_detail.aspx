<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="j02_framework_detail.aspx.vb" Inherits="UI.j02_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields_readonly" Src="~/freefields_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="alertbox" Src="~/alertbox.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="myscheduler" Src="~/myscheduler.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function b07_reaction(b07id) {
            sw_decide("b07_create.aspx?parentpid=" + b07id + "&masterprefix=j02&masterpid=<%=Master.datapid%>", "Images/comment_32.png", true)

        }
        function b07_delete(b07id, flag) {
            sw_decide("b07_delete.aspx?pid=" + b07id, "Images/delete_32.png", true)

        }

        function hardrefresh(pid, flag) {
            if (flag == "j02-save") {
                parent.window.location.replace("j02_framework.aspx?pid=" + pid);
                return;
            }
            if (flag == "j02-delete") {
                parent.window.location.replace("j02_framework.aspx");
                return;
            }


            location.replace("j02_framework_detail.aspx?pid=<%=master.datapid%>&source=<%=menu1.PageSource%>");
        }

        function j03_create() {
            sw_decide("j03_create.aspx?j02id=<%=master.datapid%>", "Images/user_32.png", true);
        }

        function j03_edit() {
            sw_decide("j03_record.aspx?pid=<%=Me.CurrentJ03ID%>", "Images/user_32.png", true);
        }

        function j05_record(j05id) {

            sw_decide("j05_record.aspx?pid=" + j05id, "Images/masterslave_32.png", false);

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
        function p66_plan() {
            window.parent.sw_everywhere("j02_personalplan.aspx?j02id=<%=Master.DataPID%>", "Images/plan.png");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc:entity_menu ID="menu1" runat="server"></uc:entity_menu>
    <div style="height: 10px; clear: both;"></div>

    <div class="content-box1">
        <div class="title">
            <img src="Images/properties.png" style="margin-right: 10px;" />
           <asp:Localize ID="locOsobniProfil" runat="server" text="Osobní profil" meta:resourcekey="locOsobniProfil"></asp:Localize>
            
        </div>
        <div class="content">
            <table cellpadding="10" cellspacing="2" id="responsive">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="FullNameAsc" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="j02Code" runat="server" CssClass="valbold" ForeColor="gray"></asp:Label>
                        <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link" ></asp:HyperLink>
                    </td>
                </tr>
                <tr valign="top">

                    <td>
                        <asp:Label ID="lblJ07Name" runat="server" Text="Pozice:" CssClass="lbl" meta:resourcekey="lblJ07Name"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="j07Name" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblJ18Name" runat="server" Text="Středisko:" CssClass="lbl" meta:resourcekey="lblJ18Name"></asp:Label>
                        <asp:Label ID="j18Name" runat="server" CssClass="valbold"></asp:Label>
                        <asp:Label ID="lblJ17Name" runat="server" Text="Region:" CssClass="lbl"></asp:Label>
                        <asp:Label ID="j17Name" runat="server" CssClass="valbold"></asp:Label>
                    </td>


                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblEmail" runat="server" Text="E-mail:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:HyperLink ID="j02Email" runat="server" CssClass="wake_link"></asp:HyperLink>

                    </td>
                    <td>
                        <asp:Label ID="lblFond" runat="server" Text="Fond hodin:" CssClass="lbl" meta:resourcekey="lblFond"></asp:Label>
                        <asp:Label ID="c21Name" runat="server" CssClass="valbold"></asp:Label>
                    </td>

                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSMTP" runat="server" Text="Poštovní účet:" CssClass="lbl"></asp:Label>
                    </td>
                    <td colspan="2">
                        <asp:Label ID="smtpAccount" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                    
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblTeams" runat="server" Text="Člen týmů:" CssClass="lbl" meta:resourcekey="lblTeams"></asp:Label>
                    </td>
                    <td colspan="2">
                        <asp:Label ID="TeamsInLine" runat="server" CssClass="valbold"></asp:Label>

                    </td>

                </tr>
                
                
            </table>

            <asp:Label ID="Mediums" runat="server" CssClass="val" ForeColor="Orange" Style="padding-left: 8px;"></asp:Label>
            <div>
                <uc:mytags ID="tags1" runat="server" Prefix="j02" />
            </div>
            <div>
                <asp:Label ID="Correspondence" runat="server"></asp:Label>
            </div>
            <div>
                <asp:Image ID="imgAvatar" runat="server" Visible="false" ToolTip="Avatar obrázek" />
            </div>
        </div>
    </div>
    <asp:Panel ID="panIntraPerson" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/user.png" style="margin-right: 10px;" /><asp:Label ID="lblUserHeader" runat="server" Text="Uživatelský účet" meta:resourcekey="lblUserHeader"></asp:Label>
        </div>
        <div class="content">
            <asp:Panel ID="panAccount" runat="server">
                <table cellpadding="10" cellspacing="2">
                    <tr valign="top">
                        <td style="min-width: 120px;">
                            <asp:Label ID="lblLogin" runat="server" Text="Přihlašovací jméno:" CssClass="lbl" meta:resourcekey="lblLogin"></asp:Label>
                        </td>
                        <td>

                            <asp:Label ID="j03Login" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblJ04Name" runat="server" Text="Aplikační role:" CssClass="lbl" meta:resourcekey="lblJ04Name"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="j04Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>

                    </tr>


                </table>
            </asp:Panel>
            <asp:Label ID="AccountMessage" runat="server" CssClass="infoInForm"></asp:Label>
            <asp:HyperLink ID="cmdLog" runat="server" Text="Historie aktivit" NavigateUrl="javascript: timeline()" Style="margin-left: 6px;" meta:resourcekey="cmdLog"></asp:HyperLink>
            <span style="padding-left: 40px;"></span>
            <asp:HyperLink ID="cmdAccount" runat="server" Text="Založit uživatelský účet" Visible="false"></asp:HyperLink>

            <div style="width: 100%; padding: 6px; margin-top: 10px;">
                <asp:Label ID="lblPosledniPristup" runat="server" CssClass="lbl" Text="Poslední přístup do MT:" meta:resourcekey="lblPosledniPristup"></asp:Label>
               
                <asp:Label ID="Last_Access" runat="server" ForeColor="Brown" Style="float: right;"></asp:Label>

            </div>
            <div style="width: 100%; padding: 6px;">                
                <asp:Label ID="lblNaposledyZapsanyUkon" runat="server" Text="Naposledy zapsaný úkon:" CssClass="lbl" meta:resourcekey="lblNaposledyZapsanyUkon"></asp:Label>
                <asp:Label ID="Last_Worksheet" runat="server" ForeColor="Brown" Style="float: right;"></asp:Label>

            </div>
            <div style="width: 100%; padding: 6px;">                
                <asp:Label ID="lblPocetUkolu" runat="server" CssClass="lbl" Text="Počet otevřených úkolů:" meta:resourcekey="lblPocetUkolu"></asp:Label>
                <asp:HyperLink ID="link_p56_actual_count" runat="server" Style="margin-left: 20px;"></asp:HyperLink>

            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="boxJ05" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/masterslave.png" style="margin-right: 10px;" />
            <asp:Label ID="boxJ05Title" runat="server" Text="Nadřízenost | Podřízenost" meta:resourcekey="boxJ05Title"></asp:Label>
            <asp:HyperLink ID="cmdAddJ05" runat="server" Text="Přidat" NavigateUrl="javascript:j05_record(0)" meta:resourcekey="cmdAddJ05"></asp:HyperLink>
        </div>
        <div class="content">

            <asp:Panel ID="panSlaves" runat="server" CssClass="div6">
                <img src="Images/slave.png" /><span>Podřízení:</span>
                <asp:Repeater ID="rpSlaves" runat="server">
                    <ItemTemplate>
                        <a class="value_link" href="javascript:j05_record(<%#Eval("pid")%>)"><%#Eval("PersonSlave")%><%#Eval("TeamSlave")%></a>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="SlavesInLine" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="panMasters" runat="server" CssClass="div6">
                <img src="Images/master.png" /><span>Nadřízení:</span>
                <asp:Repeater ID="rpMasters" runat="server">
                    <ItemTemplate>
                        <a class="value_link" href="javascript:j05_record(<%#Eval("pid")%>)"><%#Eval("PersonMaster")%></a>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
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


    <asp:Panel ID="boxP30" runat="server" CssClass="content-box1">
        <div class="title">
            <img src="Images/contact.png" />
            <img src="Images/project.png" style="margin-right: 10px;" />
            <asp:Localize ID="locContactPersons" runat="server" Text="Kontaktní osoba klienta/projektu" meta:resourcekey="locContactPersons"></asp:Localize>
            
        </div>
        <div class="content">
            <asp:Repeater ID="rpP30" runat="server">
                <ItemTemplate>
                    <div class="div6">
                        <asp:HyperLink ID="pm1" runat="server" CssClass="pp1"></asp:HyperLink>
                        <asp:HyperLink ID="ContactLink" runat="server" Target="_top" CssClass="value_link"></asp:HyperLink>


                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>


    <uc:alertbox ID="alert1" runat="server"></uc:alertbox>

    <div style="clear: both; padding-top: 6px;">
        <div style="float: left;">
            <uc:myscheduler ID="cal1" runat="server" Prefix="j02" />
        </div>
        <asp:Panel ID="boxX18" runat="server" CssClass="content-box1">
            <div class="title">
                <img src="Images/notepad.png"  /><span style="margin-right: 10px;"><asp:Localize ID="locDokumenty" runat="server" Text="Dokumenty" meta:resourcekey="locDokumenty"></asp:Localize></span>
                <img src="Images/label.png" /><span><asp:Localize ID="locKategorie" runat="server" Text="Kategorie" meta:resourcekey="locKategorie"></asp:Localize></span>
            </div>
            <div class="content">
                <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>
            </div>

        </asp:Panel>
    </div>
    <div style="clear: both;">
        <uc:b07_list ID="comments1" runat="server" JS_Create="menu_b07_record()" JS_Reaction="b07_reaction" />
    </div>


    <asp:HiddenField ID="hidJ03ID" runat="server" />
    <asp:HiddenField ID="hidCal1ShallBeActive" Value="1" runat="server" />
</asp:Content>
