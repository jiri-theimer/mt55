<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p56_record.aspx.vb" Inherits="UI.p56_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function changelog() {
            dialog_master("changelog.aspx?prefix=p56&pid=<%=Master.DataPID%>", true)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Value="core" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Kategorie ({1}), uživatelská pole ({0})" Value="ff"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="RadPageView1" runat="server" Selected="true">
            <table cellpadding="5" cellspacing="2" id="responsive">
                <tr>
                    <td style="width: 140px;">
                        <asp:Label ID="lblP57ID" Text="Typ úkolu:" runat="server" CssClass="lblReq"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p57ID" runat="server" DataTextField="p57Name" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Width="300px"></uc:datacombo>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblProject" runat="server" CssClass="lbl" Text="Projekt:"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
                    </td>
                </tr>
                <tr style="border-top: dashed gray 1px;">
                    <td>
                        <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název úkolu:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p56Name" runat="server" Style="width: 540px;"></asp:TextBox>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblDateFrom" Text="Plánované zahájení:" runat="server" AssociatedControlID="p56PlanFrom" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDateTimePicker ID="p56PlanFrom" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                            <TimePopupButton Visible="true" />
                            <TimeView StartTime="06:00" EndTime="22:00" ShowHeader="false" ShowFooter="false"></TimeView>

                        </telerik:RadDateTimePicker>



                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblDateUntil" Text="Termín splnění úkolu:" runat="server" AssociatedControlID="p56PlanUntil" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="p56PlanUntil" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                            <TimePopupButton Visible="true" />
                            <TimeView StartTime="06:00" EndTime="22:00" ShowHeader="false" ShowFooter="false"></TimeView>

                        </telerik:RadDateTimePicker>



                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="chkMore" runat="server" Text="Více nastavení úkolu" AutoPostBack="true" ForeColor="blue" />
                    </td>
                </tr>





                <tr>
                    <td>
                        <asp:Label ID="lblCompletePercent" Text="Hotovo (%):" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p56CompletePercent" runat="server" NumberFormat-DecimalDigits="0" MaxValue="100" MinValue="0" IncrementSettings-Step="10" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>


                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblO22ID" Text="Milník úkolu:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="o22ID" runat="server" DataTextField="NameWithDate" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="false" Width="400px"></uc:datacombo>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblP59ID_Submitter" runat="server" CssClass="lbl" Text="Priorita zadavatele:"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="p59ID_Submitter" runat="server" DataTextField="p59Name" DataValueField="pid" IsFirstEmptyRow="true" Width="150px"></uc:datacombo>
                        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>
                        <uc:person ID="j02ID_Owner" runat="server" Width="150px" />
                    </td>
                </tr>
            </table>
            <uc:mytags ID="tags1" ModeUi="1" Prefix="p56" runat="server" />

            <asp:Panel ID="panRoles" runat="server" CssClass="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Příjemci (řešitelé) úkolu"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="p56Task"></uc:entityrole_assign>
                </div>
            </asp:Panel>

            <asp:Panel ID="panBudget" runat="server" CssClass="content-box2" Style="margin-top: 6px;">
                <div class="title">
                    <img src="Images/plan.png" width="16px" height="16px" />Plán/limity úkolu</div>

                <div class="content">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblp56Plan_Hours" runat="server" Text="Plán pracnosti v hodinách:"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox ID="p56Plan_Hours" runat="server" NumberFormat-DecimalDigits="2" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>


                            </td>
                            <td>
                                <asp:CheckBox ID="p56IsPlan_Hours_Ceiling" runat="server" Text="Zákaz překročit plán" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblp56Plan_Expenses" runat="server" Text="Plán (limit) peněžních výdajů:"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox ID="p56Plan_Expenses" runat="server" NumberFormat-DecimalDigits="2" Width="90px" ShowSpinButtons="true"></telerik:RadNumericTextBox>

                            </td>
                            <td>
                                <asp:CheckBox ID="p56IsPlan_Expenses_Ceiling" runat="server" Text="Zákaz překročit plán" />
                            </td>
                        </tr>
                    </table>
                </div>

            </asp:Panel>
            <asp:Panel ID="panDescription" runat="server" CssClass="content-box2" Style="margin-top: 6px;">
                <div class="title">Podrobný popis/zadání úkolu</div>
                <div class="content">
                    <asp:TextBox ID="p56Description" runat="server" Style="height: 90px; width: 99%;" TextMode="MultiLine"></asp:TextBox>
                </div>

            </asp:Panel>
            <div class="div6">
                <asp:CheckBox ID="p56IsNoNotify" runat="server" Text="V úkolu vypnout automatické e-mail notifikace" CssClass="chk" />
            </div>

            <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">

                <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                </SpecialDays>
            </telerik:RadCalendar>

            <asp:HiddenField ID="hidP41ID" runat="server" />

        </telerik:RadPageView>
        <telerik:RadPageView ID="ff" runat="server">

            <uc:freefields ID="ff1" runat="server" />

        </telerik:RadPageView>
        <telerik:RadPageView ID="other" runat="server">

            <table cellpadding="5" cellspacing="2" id="responsive">


                <tr>
                    <td>
                        <asp:Label ID="lblp56ReminderDate" Text="Čas připomenutí:" runat="server" AssociatedControlID="p56ReminderDate" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDateTimePicker ID="p56ReminderDate" runat="server" Width="190px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                            <TimePopupButton Visible="true" />
                            <TimeView runat="server" StartTime="06:00" EndTime="22:00" ShowHeader="false" ShowFooter="false"></TimeView>
                        </telerik:RadDateTimePicker>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOrdinary" Text="Index pořadí v rámci projektu:" runat="server" CssClass="lbl" AssociatedControlID="p56Ordinary"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p56Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Externí kód:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p56ExternalPID" runat="server" Style="width: 200px;"></asp:TextBox>

                        <span class="infoInForm">Klíč záznamu z externího IS pro integraci s MT.</span>
                    </td>
                </tr>
            </table>

            <div class="content-box2" style="margin-top: 20px;">
                <div class="title">
                    Matka (šablona), která opakovaně rodí instance tohoto úkolu
                    
                </div>
                <div class="content">
                    <div class="div6">
                        <span>Typ opakování:</span>
                        <asp:DropDownList ID="p65ID" runat="server" DataTextField="NameWithFlag" DataValueField="pid" AutoPostBack="true"></asp:DropDownList>
                        <asp:CheckBox ID="p56IsStopRecurrence" Text="Automatika pozastavena" runat="server" />
                    </div>
                    <asp:Panel ID="panRecurrence" runat="server">
                        <div class="div6">
                            <span>Maska názvu nových úkolů:</span>
                            <asp:TextBox ID="p56RecurNameMask" runat="server" Width="300px"></asp:TextBox>
                        </div>
                        <div class="div6">
                            <span>Rozhodné datum tohoto úkolu:</span>
                            <telerik:RadDateInput ID="p56RecurBaseDate" runat="server" DisplayDateFormat="d.M.yyyy" DateFormat="d.M.yyyy" Width="100px"></telerik:RadDateInput>
                            <div>
                                <i>U měsíčního opakování musí být datum vždy první den v měsíci.</i>
                            </div>
                            <div>
                                <i>U čtvrtletního opakování musí být datum první den kvartálu, tedy jedno z následujících: 1.1., 1.4., 1.7., 1.10.</i>
                            </div>
                            <div>
                                <i>U ročního opakování musí být datum vždy první den v roce.</i>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>

        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

