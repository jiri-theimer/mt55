<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="timesheet_calendar.ascx.vb" Inherits="UI.timesheet_calendar" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div>
    <div>
                
        <asp:DropDownList ID="CalCols" runat="server" AutoPostBack="true" ToolTip="Počet najednou zobrazených měsíců">
            <asp:ListItem Text="1" Value="1" Selected="true"></asp:ListItem>
            <asp:ListItem Text="2" Value="2"></asp:ListItem>
            <asp:ListItem Text="3" Value="3"></asp:ListItem>
            <asp:ListItem Text="4" Value="4"></asp:ListItem>
        </asp:DropDownList>
        
        <asp:LinkButton ID="cmdToday" Text="Dnes" runat="server" Style="padding-left: 20px;"></asp:LinkButton>
    </div>

    <telerik:RadCalendar ID="cal1" ShowRowHeaders="false" ShowColumnHeaders="true" ShowDayCellToolTips="false" runat="server" AutoPostBack="true"
        RenderMode="Auto" Skin="Metro" EnableMultiSelect="false"
        EnableNavigationAnimation="false" EnableRepeatableDaysOnClient="false"
        EnableMonthYearFastNavigation="false" EnableNavigation="true" Width="300px">

        
        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="LightBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
</div>
<asp:HiddenField ID="hidD1" runat="server" />
<asp:HiddenField ID="hidD2" runat="server" />