<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_scheduler.aspx.vb" Inherits="UI.mobile_scheduler" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function re(pid, prefix) {
            if (prefix == 'o22')
                location.replace("mobile_o22_framework.aspx?pid=" + pid);


            if (prefix == 'p56')
                location.replace("mobile_p56_framework.aspx?pid=" + pid);
        }

        function OnClientAppointmentEditing(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }
        function record_create(sender, eventArgs) {

            var firstSlot = sender.get_selectedSlots()[0];

            var lastSlot = sender.get_selectedSlots()[sender.get_selectedSlots().length - 1];
            var d1 = firstSlot.get_startTime()
            var d2 = lastSlot.get_endTime();

            if (d1.getHours() == 0 && d2.getHours() == 0) {
                d2.setDate(d2.getDate() - 1);
            }

            var j02id = "<%=Master.Factory.SysUser.j02ID%>";


            var url = "mobile_p56_create.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id + "&masterprefix=p41";
            url = url + "&masterpid=0";
            location.replace(url);


        }
        function OnSchedulerCommand(sender, args) {
            var loadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
            loadingPanel.show(sender.get_id());

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <nav class="navbar navbar-default" style="margin-bottom: 0px !important;">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbarOnSite">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                
                
                
                        <img src="Images/calendar.png" class="navbar-brand" />
                        <a href="mobile_scheduler.aspx" style="text-decoration:underline;" class="navbar-brand">Kalendář</a>
                        <span class="navbar-brand"><asp:Label runat="server" ID="CountP56" CssClass="badge"></asp:Label></span>
                        

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">    
                  
                  
                    <li>
                        <div>Zobrazení úkolů:</div>
                        <asp:DropDownList ID="cbxScope" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="Jsem řešitelem úkolů" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Jsem zakladatelem (vlastníkem) úkolů" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </li>           
                    <li>
                        <asp:HyperLink ID="linkCreateTask" runat="server" NavigateUrl="mobile_p56_create.aspx?source=scheduler" Text="<img src='Images/task.png' /> Nový úkol"></asp:HyperLink>
                    </li>
                    
                    
                </ul>
               
            </div>

    </nav>


    <div class="container-fluid">
        <div id="row1" class="row">



            <telerik:RadScheduler ID="scheduler1" SelectedView="DayView" RenderMode="Mobile" FirstDayOfWeek="Monday" Height="70%" LastDayOfWeek="Sunday" EnableViewState="false" Skin="Default" AppointmentStyleMode="Simple" ShowFooter="false" runat="server" ShowViewTabs="true" EnableAdvancedForm="false"
                Culture="cs-CZ" AllowEdit="false" AllowDelete="false" AllowInsert="false"
                OnClientAppointmentEditing="OnClientAppointmentEditing" OnClientTimeSlotClick="record_create"
                HoursPanelTimeFormat="HH:mm" ShowNavigationPane="true" OnClientNavigationCommand="OnSchedulerCommand"
                DataSubjectField="o22Name" DataStartField="o22DateFrom" DataEndField="o22DateUntil" DataKeyField="pid">
                <Localization HeaderAgendaDate="Datum" AllDay="Bez času od/do" HeaderMonth="Měsíc" HeaderDay="Den" HeaderMultiDay="Multi-den" HeaderWeek="Týden" ShowMore="více..." HeaderToday="Dnes" HeaderAgendaAppointment="Událost" HeaderAgendaTime="Čas" />
                <DayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="false" ShowHiddenAppointmentsIndicator="true" />
                <WeekView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="false" ShowHiddenAppointmentsIndicator="true" />
                <MultiDayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" NumberOfDays="10" />
                <TimelineView UserSelectable="true" NumberOfSlots="7" />
                <AgendaView UserSelectable="true" NumberOfDays="20" />
                <MonthView UserSelectable="true" VisibleAppointmentsPerDay="4" />

                <AppointmentTemplate>

                    <a href="javascript:re(<%# Eval("ID")%>)"><%# Eval("Subject")%></a>


                </AppointmentTemplate>
               


            </telerik:RadScheduler>
            <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" RenderMode="Mobile" Transparency="30" BackColor="#E0E0E0">
                <div style="float: none; padding-top: 80px;">
                    <img src="Images/loading.gif" />
                    <h2>LOADING...</h2>
                </div>
            </telerik:RadAjaxLoadingPanel>

        </div>
    </div>
</asp:Content>
