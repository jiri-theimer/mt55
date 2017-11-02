<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="o22_scheduler.aspx.vb" Inherits="UI.o22_scheduler" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            

        })

        function od(pid) {
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            

        }

        function OnClientAppointmentEditing(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }

        function clientTimeSlotClick(sender, eventArgs) {
            document.getElementById("<%=hidCurResource.clientid %>").value = "";
            var resource = null;
            var timeSlot = eventArgs.get_targetSlot();

            if (timeSlot.get_resource) {
                resource = timeSlot.get_resource();
                document.getElementById("<%=hidCurResource.clientid %>").value = resource.get_key();
            }

            document.getElementById("<%=hidCurTime.clientid %>").value = timeSlot.get_startTime().toISOString();



        }

        function record_new(sender, eventArgs)
        {
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <div id="divScheduler"> 
        <telerik:RadScheduler ID="scheduler1" SelectedView="MonthView" EnableViewState="true" Skin="Silk" AppointmentStyleMode="Simple" RowHeight="35px" Height="100%" ShowFooter="false" runat="server" ShowViewTabs="true" OnClientTimeSlotContextMenuItemClicked="record_new" EnableAdvancedForm="false"
            Culture="cs-CZ" AllowEdit="false" AllowDelete="false" AllowInsert="false" OnClientAppointmentEditing="OnClientAppointmentEditing" OnClientTimeSlotContextMenu="clientTimeSlotClick"            
            HoursPanelTimeFormat="HH:mm"
            DataSubjectField="o22Name" DataStartField="o22DateFrom" DataEndField="o22DateUntil" DataKeyField="pid">            
            <TimelineView SlotDuration="1" ColumnHeaderDateFormat="dd.MM ddd" NumberOfSlots="7" UserSelectable="true" />
            <MonthView UserSelectable="true" />
            <DayView UserSelectable="true" DayStartTime="08:00" DayEndTime="19:00" ShowInsertArea="true" />
            <AppointmentTemplate>
            
            <a href="javascript:od('<%# Eval("id") %>')"><%# Eval("Subject") %></a>
    
            
            </AppointmentTemplate>


            <TimeSlotContextMenus>
                <telerik:RadSchedulerContextMenu ID="menu1" runat="server">
                    <Items>
                        <telerik:RadMenuItem Text="Nová událost" Value="new"></telerik:RadMenuItem>

                    </Items>
                </telerik:RadSchedulerContextMenu>
            </TimeSlotContextMenus>


        </telerik:RadScheduler>
    </div>

    <asp:hiddenfield id="hidCurResource" runat="server" />
    <asp:hiddenfield id="hidCurTime" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
