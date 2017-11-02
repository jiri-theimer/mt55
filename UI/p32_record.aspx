<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p32_record.aspx.vb" Inherits="UI.p32_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datalabel" Src="~/datalabel.ascx" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>

                <asp:Label runat="server" ID="Label5" Text="Sešit:" CssClass="lblReq" meta:resourcekey="lblP34ID"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p34ID" runat="server" AutoPostBack="true" DataTextField="p34Name"></uc:datacombo>
                <uc:datalabel ID="lblP32Code" runat="server" Text="Kód aktivity:" meta:resourcekey="lblP32Code" />
                <asp:TextBox ID="p32Code" runat="server" Style="width: 100px;"></asp:TextBox>

            </td>
        </tr>

        <tr>
            <td>
                <asp:Label runat="server" ID="Label4" Text="Název aktivity:" CssClass="lblReq"></asp:Label>

            </td>
            <td>
                <asp:TextBox ID="p32name" runat="server" Style="width: 400px;"></asp:TextBox>
                <asp:Label runat="server" ID="Label2" Text="Index pořadí:"></asp:Label>
                <telerik:RadNumericTextBox ID="p32Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr id="trManualFlag" runat="server">
            <td colspan="2">
                <div class="innerform_light">
                    <asp:RadioButtonList ID="p32ManualFeeFlag" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                        <asp:ListItem Value="0" Text="Honorář časového úkonu se počítá násobkem hodin a fakturační hodinové sazby" Selected="true"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Výši honoráře zadává uživatel ručně ve formuláři časového úkonu jako [Pevný honorář]"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <asp:Label ID="lblp32ManualFeeDefAmount" runat="server" Text="Výchozí částka pevného honoráře:" CssClass="lbl"></asp:Label>
                <telerik:RadNumericTextBox ID="p32ManualFeeDefAmount" runat="server" Width="120px"></telerik:RadNumericTextBox>
            </td>
        </tr>

        <tr>
            <td colspan="2">


                <asp:CheckBox ID="p32IsBillable" runat="server" Text="Fakturovatelné" Checked="true"></asp:CheckBox>
                <asp:CheckBox ID="p32IsTextRequired" runat="server" Text="Zadávání worksheet popisu je povinné" Checked="true"></asp:CheckBox>

            </td>
        </tr>
    </table>

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Výchozí popis úkonu" Value="0"></telerik:RadTab>
            <telerik:RadTab Text="Rozšířené nastavení aktivity" Value="1" Selected="true"></telerik:RadTab>
            <telerik:RadTab Text="Překlady do dalších fakturačních jazyků" Value="2"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="Page1" runat="server">
            <table cellpadding="5" cellspacing="2">
                <tr valign="top">
                    <td>
                        <asp:Label runat="server" ID="Label6" Text="Výchozí obsah popisu úkonu:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p32DefaultWorksheetText" runat="server" TextMode="MultiLine" Style="width: 500px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td>

                        <asp:Label runat="server" ID="Label7" Text="Nápověda pro zapisování úkonu:"></asp:Label>

                    </td>
                    <td>
                        <asp:TextBox ID="p32HelpText" runat="server" Style="width: 500px; height: 140px;" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>

        </telerik:RadPageView>
        <telerik:RadPageView ID="Page2" runat="server" Selected="true">
            <table cellpadding="5" cellspacing="2">

                <tr>
                    <td>
                        <uc:datalabel ID="Datalabel2" runat="server" Text="Hodnota úkonu musí být větší než:"></uc:datalabel>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p32Value_Minimum" runat="server" Width="90px"></telerik:RadNumericTextBox>
                        <uc:datalabel ID="Datalabel3" runat="server" Text=" a menší než:"></uc:datalabel>
                        <telerik:RadNumericTextBox ID="p32Value_Maximum" runat="server" Width="90px"></telerik:RadNumericTextBox>
                    </td>
                </tr>

                <tr>
                    <td>
                        <uc:datalabel ID="Label1" runat="server" Text="Výchozí hodnota úkonu:" GLX="602"></uc:datalabel>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p32Value_Default" runat="server" Width="90px"></telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel ID="lblX15" runat="server" Text="Fakturační sazba DPH:"></uc:datalabel>
                    </td>
                    <td>
                        <uc:datacombo ID="x15id" Width="130px" runat="server" DataTextField="x15Name" DataValueField="pid"></uc:datacombo>
                        <uc:datalabel ID="lblp35id" runat="server" Text="Metrika jednotky:" GLX="1017"></uc:datalabel>
                        <uc:datacombo ID="p35id" Width="100px" runat="server" DataTextField="p35Code" IsFirstEmptyRow="true"></uc:datacombo>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel ID="lblP95ID" runat="server" Text="Fakturační oddíl:"></uc:datalabel>
                    </td>
                    <td>
                        <uc:datacombo ID="p95id" Width="300px" runat="server" DataTextField="p95Name"></uc:datacombo>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp32AttendanceFlag" runat="server" Text="Rychlá volba v rozhraní docházky:"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="p32AttendanceFlag" runat="server">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Tlačítko v docházce (bez přesného času od-do)" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Tlačítko v docházce (nabízet přesný čas od-do)" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="Datalabel4" Text="Externí kód:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32ExternalPID" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblp32FreeText01" Text="Volné pole 1:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32FreeText01" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblp32FreeText02" Text="Volné pole 2:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32FreeText02" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblp32FreeText03" Text="Volné pole 3:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32FreeText03" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label runat="server" ID="Label3" Text="Barva:"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadColorPicker ID="p32Color" runat="server" CurrentColorText="Vybraná barva" NoColorText="Bez barvy" ShowIcon="true" Preset="Default">
                        </telerik:RadColorPicker>
                        <span class="infoInForm">Barva pro odlišení aktivity v DAYLINE zobrazení a v operativním plánování.</span>
                    </td>
                </tr>
            </table>

        </telerik:RadPageView>
        <telerik:RadPageView ID="Page3" runat="server">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="head1" runat="server" CssClass="framework_header_span" Text="Název aktivity v příloze faktury"></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang1" Text="Jazyk 1:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32Name_BillingLang1" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang2" Text="Jazyk 2:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32Name_BillingLang2" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang3" Text="Jazyk 3:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32Name_BillingLang3" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:datalabel runat="server" ID="lblLang4" Text="Jazyk 4:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32Name_BillingLang4" runat="server" Style="width: 400px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="head2" runat="server" CssClass="framework_header_span" Text="Výchozí obsah popisu úkonu"></asp:Label></td>
                </tr>
                <tr valign="top">
                    <td>
                        <uc:datalabel runat="server" ID="lblLang1a" Text="Jazyk 1:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32DefaultWorksheetText_Lang1" runat="server" TextMode="MultiLine" Style="width: 400px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <uc:datalabel runat="server" ID="lblLang2a" Text="Jazyk 2:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32DefaultWorksheetText_Lang2" runat="server" TextMode="MultiLine" Style="width: 400px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <uc:datalabel runat="server" ID="lblLang3a" Text="Jazyk 3:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32DefaultWorksheetText_Lang3" runat="server" TextMode="MultiLine" Style="width: 400px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <uc:datalabel runat="server" ID="lblLang4a" Text="Jazyk 4:"></uc:datalabel>
                    </td>
                    <td>
                        <asp:TextBox ID="p32DefaultWorksheetText_Lang4" runat="server" TextMode="MultiLine" Style="width: 400px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
