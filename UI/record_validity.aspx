<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="record_validity.aspx.vb" Inherits="UI.record_validity" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Aktuální platnost záznamu:" runat="server"></asp:Label>
            </td>
            
            <td>
                <asp:Label ID="curDatFrom" runat="server" CssClass="valbold"></asp:Label>
            </td>
            <td> - </td>
            <td>
                <asp:Label ID="curDatUntil" runat="server" CssClass="valbold"></asp:Label>
                <asp:Image ID="imgInfinity" ImageUrl="Images/infinity_32.png" runat="server" />
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:CheckBox ID="chkExplicitDates" runat="server" Text="Zadat rozsah platnosti záznamu ručně" AutoPostBack="true" />
    </div>
    <asp:Panel ID="panExplicitDates" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="2">
            <tr>
                <td>
                    <asp:Label ID="lbl1" Text="Nový rozsah platnosti záznamu:" runat="server"></asp:Label>
                </td>
                <td>
                    <telerik:RadDateTimePicker ID="datFrom" runat="server" RenderMode="Lightweight" Width="140px" SharedCalendarID="SharedCalendarValidity" MaxDate="1.1.3000">
                        <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                        <TimePopupButton Visible="false" />

                    </telerik:RadDateTimePicker>
                </td>
                <td>-</td>
                <td>
                    <telerik:RadDateTimePicker ID="datUntil" runat="server" RenderMode="Lightweight" Width="140px" SharedCalendarID="SharedCalendarValidity" MaxDate="1.1.3000">
                        <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy HH:mm ddd" DateFormat="d.M.yyyy HH:mm ddd" runat="server"></DateInput>
                        <TimePopupButton Visible="false" />
                    </telerik:RadDateTimePicker>
                </td>
            </tr>
        </table>
        <telerik:RadCalendar ID="SharedCalendarValidity" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
            <FastNavigationSettings EnableTodayButtonSelection="true"></FastNavigationSettings>
        </telerik:RadCalendar>
    </asp:Panel>

    <p class="infoInForm" style="font-weight:bold;">Provedné změny se projeví až po uložení záznamu!</p>
    <p class="infoInForm">Kromě přesunu záznamu do archivu lze nastavit i přesný rozsah časové platnosti záznamu.</p>    
    <p class="infoInForm">Situaci, kdy záznam není časově platný, označujeme jako 'ZÁZNAM JE V ARCHIVu'.</p>
    <p class="infoInForm">Situaci, kdy záznam je časově platný, označujeme jako 'ZÁZNAM JE OTEVŘENÝ'.</p>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
