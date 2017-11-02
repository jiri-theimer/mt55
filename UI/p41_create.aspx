<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p41_create.aspx.vb" Inherits="UI.p41_create" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_preview" Src="~/entityrole_assign_preview.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function p28_client_add() {
            document.getElementById("<%=HardRefreshWindow.ClientID%>").value = "p28_client_add";

            dialog_master("p28_record.aspx?pid=0", true)
        }
        function p28_billing_add() {
            document.getElementById("<%=HardRefreshWindow.ClientID%>").value = "p28_billing_add";

            dialog_master("p28_record.aspx?pid=0", true)

        }

        function p51_billing_add(customtailor) {
            document.getElementById("<%=HardRefreshWindow.ClientID%>").value = "p51_billing_add";

            dialog_master("p51_record.aspx?pid=0&prefix=p41&customtailor=" + customtailor, true)

        }

        function p51_edit(p51id) {
            
            dialog_master("p51_record.aspx?pid="+p51id+"&prefix=p41", true)

        }

        function hardrefresh(pid, flag) {
            document.getElementById("<%=HardRefreshPID.ClientID%>").value = pid;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true" Skin="Default">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti projektu" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Fakturační nastavení" Value="billing"></telerik:RadTab>
            <telerik:RadTab Text="Štítky ({1}), uživatelská pole ({0})" Value="ff"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="lblName" Text="Název projektu:" runat="server" CssClass="lblReq" AssociatedControlID="p41Name"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p41Name" runat="server" Style="width: 400px;"></asp:TextBox>
                        <asp:CheckBox ID="p41IsDraft" runat="server" Text="DRAFT režim" />
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblP42ID" runat="server" CssClass="lblReq" Text="Typ:"></asp:Label>
                        <asp:HyperLink ID="clue_p42" runat="server" CssClass="reczoom" Text="i" Visible="false"></asp:HyperLink>
                    </td>
                    <td>

                        <uc:datacombo ID="p42ID" runat="server" DataTextField="p42Name" DataValueField="pid" AutoPostBack="true" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblJ18ID" runat="server" CssClass="lbl" Text="Středisko:"></asp:Label>
                        <asp:HyperLink ID="clue_j18" runat="server" CssClass="reczoom" Text="i" Visible="false"></asp:HyperLink>
                    </td>
                    <td>
                        
                        <uc:datacombo ID="j18ID" runat="server" DataTextField="j18Name" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Width="400px"></uc:datacombo>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblP28ID_Client" runat="server" Text="Klient:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:contact ID="p28ID_Client" runat="server" Width="400px" Flag="client" />
                        <a href="javascript:p28_client_add()">Založit nového klienta</a>

                    </td>
                </tr>

            </table>
            <div class="div6">
                <asp:CheckBox ID="chkPlanDates" runat="server" AutoPostBack="true" Text="Definovat plán zahájení a dokončení" CssClass="chk" />
            </div>
            <asp:Panel ID="panPlanDates" runat="server">
                <table cellpadding="5" cellspacing="2">
                    <tr>
                        <td style="width: 120px;">
                            <asp:Label ID="lblPlanFrom" Text="Plánované zahájení:" runat="server" AssociatedControlID="p41PlanFrom" CssClass="lbl"></asp:Label></td>
                        <td>
                            <telerik:RadDatePicker ID="p41PlanFrom" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                            </telerik:RadDatePicker>

                            <asp:Label ID="lblPlanUntil" Text="Plánované dokončení:" runat="server" AssociatedControlID="p41PlanUntil" CssClass="lbl"></asp:Label>

                            <telerik:RadDatePicker ID="p41PlanUntil" runat="server" Width="120px" SharedCalendarID="SharedCalendar" MaxDate="1.1.3000">
                                <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                            </telerik:RadDatePicker>

                        </td>
                    </tr>
                </table>
                <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">                    
                    <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                    </SpecialDays>
                </telerik:RadCalendar>
            </asp:Panel>
            <div class="div6">
                <asp:CheckBox ID="chkDefineLimits" runat="server" Checked="false" AutoPostBack="true" Text="Definovat limity k upozornění" CssClass="chk" />
            </div>
            <asp:Panel ID="panLimits" runat="server">
                <div class="div6">
                    <asp:Label ID="lblLimitHours" runat="server" CssClass="lbl" Text="Limitní objem rozpracovaných hodin, po jehož překročení systém odešle notifikaci (upozornění):"></asp:Label>
                    <telerik:RadNumericTextBox ID="p41LimitHours_Notification" runat="server" MinValue="0" MaxValue="1000" NumberFormat-DecimalDigits="2" Value="0" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                </div>
                <div class="div6">
                    <asp:Label ID="lblLimitFee" runat="server" CssClass="lbl" Text="Limitní honorář (rozpracované hodiny x sazba), po jehož překročení systém odešle notifikaci (upozornění):"></asp:Label>
                    <telerik:RadNumericTextBox ID="p41LimitFee_Notification" runat="server" MinValue="0" MaxValue="9999999" NumberFormat-DecimalDigits="2" Value="0" Width="100px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                </div>
            </asp:Panel>
            <div class="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Obsazení projektových rolí | Oprávnění k projektu"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="p41Project"></uc:entityrole_assign>
                    
                    <div style="clear:both;">
                        <asp:Label ID="lblJ18Message" runat="server" CssClass="infoNotification"></asp:Label>
                    </div>
                </div>
            </div>
            <uc:mytags ID="tags1" ModeUi="1" Prefix="p41" runat="server" />

            <div class="div6">
                <img src="Images/tree.png" />
                <asp:Label ID="lblParent" runat="server" CssClass="lbl" Text="Nadřízený projekt:"></asp:Label>
                 <uc:project ID="p41ParentID" runat="server" Width="400px" />
            </div>
            
            <asp:Panel ID="panCloneTasks" runat="server" CssClass="content-box2">
                <div class="title">
                    <img src="Images/task.png" />
                    Zaškrtněte úkoly, které se mají naklonovat do nového projektu
                    <asp:CheckBox ID="chkShowMothersOnly" runat="server" AutoPostBack="true" Text="Pouze matky opakovaných úkolů" />
                </div>
                <div class="content">
                    <table>
                    <asp:Repeater ID="rpP56IDs" runat="server">
                       <ItemTemplate>
                           <tr class="trHover">
                               <td>
                                   <asp:CheckBox ID="chkSelect" runat="server" Text="Vybrat:" />
                               </td>
                               
                               <td>
                                   <b><%# Eval("p57Name")%></b>
                                   <asp:HiddenField ID="p56ID" runat="server" />
                               </td>
                               <td>
                                   <i style="color:blue;"><%# Eval("p56Name")%></i>
                               </td>
                               <td style="width:30px;">
                                   <asp:Image ID="img1" runat="server" ImageUrl="Images/recurrence.png" />
                               </td>
                               <td>
                                   <asp:Label ID="Receivers" runat="server"></asp:Label>
                               </td>
                               
                           </tr>
                       </ItemTemplate>
                    </asp:Repeater>
                    </table>
                </div>
            </asp:Panel>
            
        </telerik:RadPageView>

        <telerik:RadPageView ID="billing" runat="server">
            <fieldset>
                <legend>Fakturační sazby (ceník)</legend>
                <asp:RadioButtonList ID="opgPriceList" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Sazby dědit z nastavení klienta projektu" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Svázat projekt se zavedeným ceníkem" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Tento projekt má sazby na míru" Value="3"></asp:ListItem>
                </asp:RadioButtonList>
                <div class="div6">
                    <asp:Label ID="lblP51ID_Billing" runat="server" Text="Fakturační sazby (ceník):" CssClass="lbl"></asp:Label>
                    <uc:datacombo ID="p51ID_Billing" runat="server" DataTextField="NameWithCurr" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" />
                </div>
                <div class="div6">
                    <asp:HyperLink ID="cmdNewP51" runat="server" NavigateUrl="javascript:p51_billing_add()" Text="Založit nový ceník"></asp:HyperLink>
                </div>
            </fieldset>

            <table cellpadding="5" cellspacing="2">
               
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Fakturační ceník definujte v případě, že se sazby liší od ceníku klienta projektu.</span>
                    </td>
                </tr>
                <tr>
                    <td style="width:225px;">
                        <asp:Label ID="lblP87ID" runat="server" Text="Fakturační jazyk projektu:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p87ID" runat="server" DataTextField="p87Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Fakturační jazyk definujte v případě, že se liší od nastavení klienta.</span>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="panInvoiceSetting" runat="server">
                <table cellpadding="5" cellspacing="2">
                    <tr valign="top">
                        <td style="width: 225px;">
                            <asp:Label ID="lblp28ID_Billing" runat="server" Text="Odběratel faktury:" CssClass="lbl"></asp:Label>

                        </td>
                        <td>
                            <uc:contact ID="p28ID_Billing" runat="server" Width="300px" />
                            <a href="javascript:p28_billing_add()">Založit nového odběratele do klientů</a>

                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <span class="infoInForm">Vyplňte, pokud se projekt bude fakturovat jinému subjektu než klientovi projektu.</span>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lblp92ID" runat="server" Text="Výchozí typ faktury:" CssClass="lbl"></asp:Label>

                        </td>
                        <td>
                            <uc:datacombo ID="p92id" runat="server" Width="300px" DataTextField="p92Name" IsFirstEmptyRow="true" DataValueField="pid" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <span class="infoInForm">Definujte v případě, že se typ faktury projektu liší od nastavení klienta.</span>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Label ID="lblMaturity" runat="server" Text="Výchozí počet dní splatnosti faktury:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>

                            <telerik:RadNumericTextBox ID="p41InvoiceMaturityDays" runat="server" MinValue="0" MaxValue="200" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true">
                            </telerik:RadNumericTextBox>
                            <span class="infoInForm">Výchozí splatnost definujte v případě, že se liší od nastavení klienta.</span>
                        </td>
                    </tr>


                    <tr valign="top">
                        <td>
                            <asp:Label ID="lblp41InvoiceDefaultText1" runat="server" Text="Výchozí text faktury:" CssClass="lbl"></asp:Label>

                        </td>
                        <td>
                            <asp:TextBox ID="p41InvoiceDefaultText1" runat="server" TextMode="MultiLine" Style="width: 600px; height: 50px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Label ID="lblp41InvoiceDefaultText2" runat="server" Text="Výchozí technický text faktury:" CssClass="lbl"></asp:Label>

                        </td>
                        <td>
                            <asp:TextBox ID="p41InvoiceDefaultText2" runat="server" TextMode="MultiLine" Style="width: 600px; height: 30px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Fakt. status pro nefakturovatelné úkony:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="p72ID_NonBillable" runat="server">
                            <asp:ListItem Text="--Rozhodne systém--" Value=""></asp:ListItem>
                            <asp:ListItem Text="Zahrnout do paušálu" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Viditelný odpis" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Skrytý odpis" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                </table>
                <div>Fakturační poznámka projektu:</div>
                <asp:TextBox ID="p41BillingMemo" runat="server" style="width:99%;height:60px;" TextMode="MultiLine"></asp:TextBox>
            </asp:Panel>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ff" runat="server">
            <uc:freefields ID="ff1" runat="server" />
        </telerik:RadPageView>
        <telerik:RadPageView ID="other" runat="server">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td>
                        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>

                    </td>
                    <td>
                        <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Povolit vykazovat aktivity pouze pro klastr::" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="p61ID" runat="server" DataTextField="p61Name" AutoPostBack="false" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" />
                       
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Uplatňuje se pro zapisování worksheet úkonů.</span>
                        <span class="infoInForm">Smyslem klastru aktivit v projektu je zúžit uživatelům nabídku povolených aktivit pro vykazování.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp41NameShort" runat="server" Text="Zkrácený (preferovaný) název:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p41NameShort" runat="server" Style="width: 300px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Pro uživatele systém upřednostňuje zkrácený název před standardním názvem projektu.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Omezení úkonů v projektu:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="p41WorksheetOperFlag" runat="server">
                            <asp:ListItem Text="" Value="9"></asp:ListItem>
                            <asp:ListItem Text="V projektu platí zákaz vykazovat úkony" Value="1"></asp:ListItem>
                            <asp:ListItem Text="V projektu lze vykazovat úkony pouze přes úkol" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp41RobotAddress" runat="server" Text="Adresa pro robota:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p41RobotAddress" runat="server" Style="width: 200px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Adresa v kopii (CC/BCC) , podle které robot pozná, že načtená poštovní zpráva má vazbu k tomuto projektu.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Externí kód:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p41ExternalPID" runat="server" Style="width: 200px;"></asp:TextBox>
                       
                        <span class="infoInForm">Klíč záznamu z externího IS pro integraci s MT.</span>                   
                    </td>
                </tr>
            </table>
            
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <asp:HiddenField ID="HardRefreshPID" runat="server" />
    <asp:HiddenField ID="HardRefreshWindow" runat="server" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidP51ID_Tailor" runat="server" />
    <asp:HiddenField ID="hidCloneP41ID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
