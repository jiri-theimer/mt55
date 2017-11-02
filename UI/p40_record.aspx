<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p40_record.aspx.vb" Inherits="UI.p40_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="6">
        <tr>
            <td style="width: 140px;">
                <asp:Label ID="lblP40Name" runat="server" Text="Název pravidla:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="p40name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ02ID" runat="server" Text="Osoba:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:person ID="j02ID" runat="server" Width="400px" AutoPostBack="false" Flag="p31_entry" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP34ID" runat="server" Text="Sešit:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p34ID" runat="server" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" AutoPostBack="true"></uc:datacombo>
            </td>

        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP32ID" runat="server" Text="Aktivita:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p32ID" runat="server" DataTextField="p32Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblJ27ID" runat="server" Text="Měna:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID" runat="server" DataTextField="j27Code" DataValueField="pid"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX15ID" runat="server" Text="DPH:" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x15ID" runat="server" DataTextField="x15Name" DataValueField="pid"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP40Value" runat="server" Text="Hodnota:" CssClass="lblReq"></asp:Label>
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p40Value" runat="server" NumberFormat-DecimalDigits="2" Width="80px" ShowSpinButtons="false"></telerik:RadNumericTextBox>
            </td>
        </tr>
        
      
    </table>
    <div class="innerform_light">
    <asp:RadioButtonList ID="opgGenType" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Nastavit čas opakování" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Svázat s opakovaným úkolem" Value="2"></asp:ListItem>
    </asp:RadioButtonList>
    </div>
    <asp:Panel ID="panTask" runat="server">
        <asp:Label ID="lblP56ID" runat="server" Text="Matka opakovaných úkolů:" CssClass="lbl"></asp:Label>
        <uc:datacombo ID="p56ID" runat="server" Width="400px" DataTextField="RecurNameMaskWIthTypeAndCode" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" />
        
    </asp:Panel>
    <asp:Panel ID="panDateSettings" runat="server">
        <div>
            <asp:Label ID="lblp40FirstSupplyDate" runat="server" Text="První rozhodné datum:" CssClass="lblReq"></asp:Label>

            <telerik:RadDatePicker ID="p40FirstSupplyDate" runat="server" Width="120px" SharedCalendarID="SharedCalendar" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
            </telerik:RadDatePicker>

            <asp:Label ID="lblp40LastSupplyDate" runat="server" Text="Poslední rozhodné datum:" CssClass="lblReq"></asp:Label>
            <telerik:RadDatePicker ID="p40LastSupplyDate" runat="server" Width="120px" SharedCalendarID="SharedCalendar" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
            </telerik:RadDatePicker>
        </div>
        <asp:RadioButtonList ID="p40RecurrenceType" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Value="1" Text="Denní opakování"></asp:ListItem>
            <asp:ListItem Value="2" Text="Týdenní opakování"></asp:ListItem>
            <asp:ListItem Value="3" Text="Měsíční opakování" Selected="true"></asp:ListItem>
            <asp:ListItem Value="4" Text="Čtvrtletní opakování"></asp:ListItem>
            <asp:ListItem Value="5" Text="Roční opakování"></asp:ListItem>
        </asp:RadioButtonList>
        <div class="div6">
            <asp:Label ID="lblp40GenerateDayAfterSupply" runat="server" Text="Kolik dní před/po rozhodném datu automaticky generovat úkon (-/+):" CssClass="lblReq"></asp:Label>
            <telerik:RadNumericTextBox ID="p40GenerateDayAfterSupply" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
        </div>

        <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" ShowRowHeaders="false">
        <FastNavigationSettings EnableTodayButtonSelection="false"></FastNavigationSettings>
    </telerik:RadCalendar>
    </asp:Panel>


    <div class="div6">
        <div>
            <asp:Label ID="lblp40Text" runat="server" Text="Popis worksheet úkonu:" CssClass="lbl"></asp:Label>

        </div>
        <asp:TextBox ID="p40Text" runat="server" Style="height: 60px; width: 99%;" TextMode="MultiLine"></asp:TextBox>
    </div>

    <asp:HiddenField ID="hidP41ID" runat="server" />
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
