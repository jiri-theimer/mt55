<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="myscheduler.ascx.vb" Inherits="UI.myscheduler" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="content-box2" style="width: 600px;">
    <div class="title">
        <img src="Images/calendar.png" />
        
        <asp:Localize ID="locCalendar" runat="server" Text="Kalendář" meta:resourcekey="locCalendar"></asp:Localize>
        <asp:DropDownList ID="cbxNumberOfDays" runat="server" AutoPostBack="true" ToolTip="Počet maximálně zobrazovaných dnů" Style="margin-left: 40px;">
            <asp:ListItem Text="10 dní" Value="10" Selected="true"></asp:ListItem>
            <asp:ListItem Text="20 dní" Value="20"></asp:ListItem>
            <asp:ListItem Text="30 dní" Value="50"></asp:ListItem>
            <asp:ListItem Text="100 dní" Value="100"></asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="cbxFirstDay" runat="server" AutoPostBack="true" ToolTip="První zobrazený den vůči dnešku" Style="margin-left: 10px;" meta:resourcekey="cbxFirstDay">
            <asp:ListItem Text="-30" Value="-30"></asp:ListItem>
            <asp:ListItem Text="-14" Value="-14"></asp:ListItem>
            <asp:ListItem Text="-5" Value="-5"></asp:ListItem>
            <asp:ListItem Text="-1" Value="-1" Selected="true"></asp:ListItem>
            <asp:ListItem Text="0" Value="0"></asp:ListItem>            
        </asp:DropDownList>
        <asp:DropDownList ID="cbxTopRecs" runat="server" AutoPostBack="true" ToolTip="Maximální počet najednou zobrazených položek" Style="margin-left: 40px;" meta:resourcekey="cbxTopRecs">
            <asp:ListItem Text="10 položek" Value="10" Selected="true"></asp:ListItem>
            <asp:ListItem Text="20 položek" Value="20"></asp:ListItem>
            <asp:ListItem Text="50 položek" Value="50"></asp:ListItem>
            <asp:ListItem Text="100 položek" Value="100"></asp:ListItem>
        </asp:DropDownList>
        <button type="button" class="button-link" onclick="window.open('entity_scheduler.aspx?masterprefix=<%=Me.hidPrefix.Value%>&masterpid=<%=Me.hidRecordPID.Value%>','_top')" title="Přepnout na plný kalendář" style="float:right;"><img border="0" src="Images/fullscreen.png"/></button>
        
    </div>
    <div class="content" style="padding: 0px;">
        <telerik:RadScheduler ID="scheduler1" BorderStyle="none" SelectedView="AgendaView" RenderMode="Lightweight" FirstDayOfWeek="Monday" LastDayOfWeek="Sunday" Width="600px" Height="300px" EnableViewState="false" Skin="Default" AppointmentStyleMode="Simple" ShowFooter="false" runat="server" ShowViewTabs="false" EnableAdvancedForm="false" ShowHeader="true" ShowAllDayRow="true"
            AllowEdit="false" AllowDelete="false" AllowInsert="false"
            HoursPanelTimeFormat="HH:mm" ShowNavigationPane="true"
            DataSubjectField="o22Name" DataStartField="o22DateFrom" DataEndField="o22DateUntil" DataKeyField="pid">            
            <AgendaView UserSelectable="false" NumberOfDays="30" ShowDateHeaders="true" ReadOnly="true" ShowColumnHeaders="false" TimeColumnWidth="110px" DateColumnWidth="130px" />
            <DayView UserSelectable="true" DayStartTime="08:00" DayEndTime="20:00" ShowInsertArea="false" />

            <AppointmentTemplate>
                <a class="reczoom" rel="<%# Eval("Description")%>">i</a>
                <a href="javascript:re(<%# Eval("ID")%>)"><%# Eval("Subject")%></a>


            </AppointmentTemplate>


        </telerik:RadScheduler>
    </div>
</div>



<div>
    <asp:Label ID="lblNoAppointments" runat="server" Text="Za zvolené období žádné úkoly/události s termínem." CssClass="timestamp"></asp:Label>
</div>
<asp:Panel ID="panP56" runat="server" CssClass="content-box2" Visible="false" Style="width: 600px;">
    <div class="title">
        <img src="Images/task.png" alt="Úkol" />
        <asp:Localize ID="locUkolyBezTerminu" runat="server" Text="Otevřené úkoly bez termínu" meta:resourcekey="locUkolyBezTerminu"></asp:Localize>
        
                    <asp:Label ID="p56Count" runat="server" CssClass="badge1"></asp:Label>

    </div>
    <div class="content" style="overflow: auto; max-height: 200px; background-color: #F1F1F1; padding: 0px;">

        <table style="width: 100%;" cellpadding="0" cellspacing="0">

            <asp:Repeater ID="rpP56" runat="server">
                <ItemTemplate>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Project" runat="server" Font-Italic="true"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top" style="background-color: white;">
                       
                        <td style="width:20px;">
                            <asp:HyperLink ID="pm1" runat="server" CssClass="pp1"></asp:HyperLink>
                        </td>
                        <td style="max-width: 460px; padding: 4px;">
                            
                            
                            <asp:HyperLink ID="link1" runat="server" Target="_top" CssClass="value_link"></asp:HyperLink>
                            <asp:Label ID="lblB02Name" runat="server"></asp:Label>
                            <asp:Label ID="tags" runat="server"></asp:Label>
                            <div>
                                <asp:Label ID="p56PlanUntil" runat="server" ToolTip="Termín úkolu"></asp:Label>
                                <asp:Image ID="img1" runat="server" ImageUrl="Images/reminder.png" ToolTip="Připomenutí" meta:resourcekey="img1" />
                            </div>
                        </td>
                        
                       
                        
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

    </div>
</asp:Panel>
<asp:HiddenField ID="hidPrefix" runat="server" />
<asp:HiddenField ID="hidRecordPID" runat="server" />
<asp:HiddenField ID="hidDefHeight" runat="server" Value="300px" />

