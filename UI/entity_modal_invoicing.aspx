<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="entity_modal_invoicing.aspx.vb" Inherits="UI.entity_modal_invoicing" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="10">
        <tr>


            <td>
                <span>Období fakturovaných úkonů:</span>
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="220px"></uc:periodcombo>
            </td>

        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            Výchozí nastavení dokladu faktury
            <asp:CheckBox ID="chkNonDraft" runat="server" AutoPostBack="true" Text="Rovnou generovat oficiální čísla faktur a nikoliv draft" />
        </div>
        <div class="content">
            <table cellpadding="5" cellspacing="2">
                
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91DateSupply" Text="Datum plnění:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateSupply" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <asp:RadioButtonList ID="opgDateSupply" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Dnes" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Konec minulého měsíce" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Konec aktuálního měsíce" Value="3"></asp:ListItem>
                            <asp:ListItem Text="1.den příštího měsíce" Value="4"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91Date" Text="Datum vystavení:" runat="server" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDatePicker ID="p91Date" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <asp:RadioButtonList ID="opgDate" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Dnes" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Konec minulého měsíce" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Konec aktuálního měsíce" Value="3"></asp:ListItem>
                            <asp:ListItem Text="1.den příštího měsíce" Value="4"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91DateMaturity" Text="Datum splatnosti:" runat="server" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateMaturity" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <span>Worksheet časový rámec faktury, začátek:</span>
                        <telerik:RadDatePicker ID="p91Datep31_From" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput4" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                        <span>Konec:</span>
                        <telerik:RadDatePicker ID="p91Datep31_Until" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput5" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
            </table>
            <div>
                <asp:CheckBox ID="chkRememberDates" runat="server" Text="V mé další faktuře nabízet ty samé datumy plnění a vystavení" />
                <asp:CheckBox ID="chkRememberMaturiy" runat="server" Text="V mé další faktuře nabízet to samé datum splatnosti" />
            </div>
        </div>
       
       
    </div>
    <asp:Label ID="Errors" runat="server" CssClass="infoNotificationRed"></asp:Label>

    <table cellpadding="6">
        <tr>
            <th>Typ faktury</th>
            <th>Klient</th>
            <th>Text faktury</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr valign="top">
                    <td>
                        <asp:DropDownList ID="p92ID" runat="server" DataTextField="p92Name" DataValueField="pid"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="Entity" runat="server" CssClass="valboldblue"></asp:Label>
                        <asp:HiddenField ID="pid" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="p91text1" runat="server" TextMode="MultiLine" Style="height: 50px; width: 600px;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Amount" runat="server" CssClass="valboldblue"></asp:Label>
                    </td>
                   
                </tr>
                
            </ItemTemplate>
        </asp:Repeater>
    </table>


    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <FastNavigationSettings EnableTodayButtonSelection="true"></FastNavigationSettings>
    </telerik:RadCalendar>
    <asp:HiddenField ID="hidInputPIDS" runat="server" />
    <asp:HiddenField ID="hidX29ID" runat="server" />
    <asp:HiddenField ID="hidPrefix" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
