<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p31_scheduler.aspx.vb" Inherits="UI.p31_scheduler" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="timer" Src="~/timer.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        @media screen and (max-width: 900px) {

            #left_panel {
                width: 0px !important;
            }

            #right_panel {
                margin-left: 0px !important;
            }

            #left_panel {
                display: none !important;
            }
        }
    </style>
    <script src="Scripts/jquery.timer.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {




            var h1 = new Number;
            var h2 = new Number;
            var hh = new Number;

            h1 = $(window).height();

            ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            <%If LCase(Request.Browser.Browser) = "ie" Then%>
            hh = h1 - h2 - 4;
            <%Else%>
            hh = h1 - h2 - 2;
            <%End If%>

            self.document.getElementById("divScheduler").style.height = hh + "px";

        });




        function OnSchedulerCommand(sender, args) {
            var loadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
            loadingPanel.show(sender.get_id());

        }








        function hardrefresh(pid, flag) {

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefreshOnBehind, "", False)%>

        }

        function re(pid) {

            sw_master("p31_record.aspx?pid=" + pid, "Images/worksheet_32.png")
        }

        function clone(pid) {

            sw_master("p31_record.aspx?clone=1&scheduler=1&pid=" + pid, "Images/worksheet_32.png")
        }

        function p31_setting() {

            sw_master("p31_setting.aspx", "Images/setting_32.png")
        }

        function record_create(sender, eventArgs) {
            var firstSlot = sender.get_selectedSlots()[0];

            var lastSlot = sender.get_selectedSlots()[sender.get_selectedSlots().length - 1];
            var d1 = firstSlot.get_startTime()
            var d2 = lastSlot.get_endTime();

            //alert(firstSlot.get_startTime()+"****"+lastSlot.get_endTime());

            sw_master("p31_record.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=<%=Me.CurrentJ02ID%>", "Images/worksheet_32.png")

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

            //var start = timeSlot.get_startTime().toISOString();
            var start = timeSlot.get_startTime();
            var end = timeSlot.get_endTime();
            //var end = timeSlot.get_endTime().toISOString();
            document.getElementById("<%=hidCurTime.clientid %>").value = start;
            //alert(start+" **** "+end);
        }

        function OnClientAppointmentMoveEnd(sender, eventArgs) {
            alert("OnClientAppointmentMoveEnd");

        }

        function formattedDate(date) {
            var d = date;

            var month = '' + (d.getMonth() + 1);
            var day = '' + d.getDate();
            var year = d.getFullYear();
            var hour = d.getHours();
            var minute = d.getMinutes();




            return (day + "." + month + "." + year + "_" + "0" + hour + "." + "0" + minute);
        }

        function j02id_onchange() {
            var j02id = document.getElementById("<%=me.j02id.clientid%>").value;

            $.post("Handler/handler_userparam.ashx", { x36value: j02id, x36key: "p31_framework_detail-j02id", oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }
                location.replace("p31_scheduler.aspx");

            });

        }

        function isPartOfSchedulerAppointmentArea(htmlElement) {
            // Determines if an HTML element is part of the scheduler appointment area.
            // This can be either the rsContent or the rsAllDay div (in day and week view).
            return $telerik.$(htmlElement).parents().is("div.rsAllDay") ||
                        $telerik.$(htmlElement).parents().is("div.rsContent")
        }

        window.nodeDropping = function (sender, eventArgs) {
            var htmlElement = eventArgs.get_htmlElement();

            var scheduler = $find('<%=scheduler1.ClientID%>');

            if (isPartOfSchedulerAppointmentArea(htmlElement)) {
                //Gets the TimeSlot where an Appointment is dropped. 
                var timeSlot = scheduler.get_activeModel().getTimeSlotFromDomElement(htmlElement);
                var d1 = timeSlot.get_startTime();
                var d2 = new Date(d1);
                <%If Me.CurrentView <> SchedulerViewType.MonthView Then%>
                d2.setHours(d1.getHours() + 1);
                <%End If%>

                //Gets all the data needed for the an Appointment, from the TreeView node.
                var node = eventArgs.get_sourceNode();

                var url = "p31_record.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=<%=Me.CurrentJ02ID%>";
                <%If Me.CurrentTasksPrefix = "p56" Then%>
                url = url + "&p56id=" + node.get_value();
                <%End If%>
                <%If Me.CurrentTasksPrefix = "p41" Then%>
                url = url + "&p41id=" + node.get_value();
                <%End If%>
                sw_master(url, "Images/worksheet_32.png");

            }
            else {
                //The node was dropped elsewhere on the document.
                eventArgs.set_cancel(true);
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div id="offsetY"></div>
    <div id="divScheduler">

        <div id="left_panel" style="float: left; width: 350px;background-color:white;">
            <div style="padding-top:12px;"></div>
            <telerik:RadTabStrip ID="tabs1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true" Skin="Default">
                <Tabs>
                    <telerik:RadTab Text="Kalendář" Selected="true" Value="core"></telerik:RadTab>
                    <telerik:RadTab Text="Stopky" Value="timer"></telerik:RadTab>
                    <telerik:RadTab Text="Nastavení" Value="setting"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
                <telerik:RadPageView ID="core" runat="server" Selected="true">
                    <div class="div6">
                        <asp:DropDownList ID="j02ID" runat="server" onChange="j02id_onchange()" Style="max-width: 150px;"></asp:DropDownList>
                    </div>
                   

                    <table cellpadding="3" id="tabHours" style="background-color: white; width: 100%; border-top: solid 2px silver; border-bottom: solid 2px silver;">
                        <thead>
                            <tr style="background-color: whitesmoke;">
                                <th align="left">Den</th>
                                <th>Hodiny</th>
                            </tr>
                        </thead>
                        <asp:Repeater ID="rp1" runat="server">
                            <ItemTemplate>
                                <tr class="trHover" id="trSumRow" runat="server">
                                    <td align="left">
                                        <asp:Label ID="Date" runat="server"></asp:Label>
                                    </td>
                                    <td style="text-align: right;">
                                        <asp:Label ID="Hours" runat="server" CssClass="valbold"></asp:Label>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr style="background-color: ThreeDFace;">
                            <td>
                                <img src="Images/sum.png" /></td>
                            <td align="right">
                                <asp:Label ID="TotalHours" runat="server" CssClass="valbold"></asp:Label></td>
                        </tr>
                    </table>

                    <asp:Panel ID="panTasks" runat="server" CssClass="div6" Style="background-color: whitesmoke;">
                        <asp:Image ID="img1" runat="server" ImageUrl="Images/task.png" />
                        <asp:Label ID="lblTasksHeader" runat="server" CssClass="valbold" Text="Přetáhni do kalendáře úkol:"></asp:Label>


                    </asp:Panel>
                    <div style="max-height: 400px; overflow: auto;">
                        <telerik:RadTreeView Skin="Default" ID="tasks" runat="server" ShowLineImages="false" Style="white-space: normal; cursor: move;" SingleExpandPath="true" RenderMode="Lightweight" EnableDragAndDrop="True" Width="100%" OnClientNodeDropping="nodeDropping" EnableDragAndDropBetweenNodes="false">
                        </telerik:RadTreeView>
                    </div>
                </telerik:RadPageView>

                <telerik:RadPageView ID="timer" runat="server">
                    <uc:timer ID="timer1" runat="server" IsPanelView="true"></uc:timer>
                </telerik:RadPageView>
                <telerik:RadPageView ID="setting" runat="server">
                    <div class="div6">
                        <span>Čas v kalendáři od:</span>
                        <asp:DropDownList ID="p31_scheduler_daystarttime" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="05:00" Value="5"></asp:ListItem>
                            <asp:ListItem Text="06:00" Value="6"></asp:ListItem>
                            <asp:ListItem Text="07:00" Value="7"></asp:ListItem>
                            <asp:ListItem Text="08:00" Value="8"></asp:ListItem>
                            <asp:ListItem Text="09:00" Value="9"></asp:ListItem>
                            <asp:ListItem Text="10:00" Value="10"></asp:ListItem>
                            <asp:ListItem Text="11:00" Value="11"></asp:ListItem>
                            <asp:ListItem Text="12:00" Value="12"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="div6">
                        <span>Čas v kalendáři do:</span>
                        <asp:DropDownList ID="p31_scheduler_dayendtime" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="15:00" Value="15"></asp:ListItem>
                            <asp:ListItem Text="16:00" Value="16"></asp:ListItem>
                            <asp:ListItem Text="17:00" Value="17"></asp:ListItem>
                            <asp:ListItem Text="18:00" Value="18"></asp:ListItem>
                            <asp:ListItem Text="19:00" Value="19"></asp:ListItem>
                            <asp:ListItem Text="20:00" Value="20"></asp:ListItem>
                            <asp:ListItem Text="21:00" Value="21"></asp:ListItem>
                            <asp:ListItem Text="22:00" Value="22"></asp:ListItem>
                            <asp:ListItem Text="23:00" Value="23"></asp:ListItem>

                        </asp:DropDownList>
                    </div>
                    <div class="div6">
                        <span>Počet dní v [Multi-den]:</span>
                        <asp:DropDownList ID="p31_scheduler_multidays" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="2" Value="2"></asp:ListItem>
                            <asp:ListItem Text="3" Value="3"></asp:ListItem>
                            <asp:ListItem Text="4" Value="4"></asp:ListItem>
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="6" Value="6"></asp:ListItem>
                            <asp:ListItem Text="7" Value="7"></asp:ListItem>
                            <asp:ListItem Text="8" Value="8"></asp:ListItem>
                            <asp:ListItem Text="9" Value="9"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                            <asp:ListItem Text="12" Value="12"></asp:ListItem>
                            <asp:ListItem Text="14" Value="14"></asp:ListItem>
                            <asp:ListItem Text="16" Value="16"></asp:ListItem>
                            <asp:ListItem Text="18" Value="18"></asp:ListItem>
                            <asp:ListItem Text="20" Value="20"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <div class="div6">
                        <button type="button" onclick="p31_setting()">Nastavení k zapisování hodin</button>
                    </div>
                    <div class="div6">
                        <div>DRAG & DROP nabídka v levém panelu:</div>
                        <asp:DropDownList ID="p31_scheduler_tasks" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="TOP 10 mých projektů" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Seznam mých oblíbených projektů" Value="favourites"></asp:ListItem>
                            <asp:ListItem Text="Otevřené úkoly" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Nic nenabízet" Value="none"></asp:ListItem>
                        </asp:DropDownList>

                    </div>
                    <div class="div6">
                        <asp:Button ID="cmdExportICalendar" runat="server" CssClass="cmd" Text="Export do ICalendar" />
                    </div>
                    <div class="div6">
                        <img src="Images/help.png" /><i>Zápis do kalendáře provedete přes pravé tlačítko myši nad označenými buňkami nebo přes click do kalendáře.</i>
                    </div>
                </telerik:RadPageView>

            </telerik:RadMultiPage>





        </div>

        <div id="right_panel" style="margin-left: 350px;">
            <telerik:RadScheduler ID="scheduler1" SelectedView="WeekView" RenderMode="Auto" FirstDayOfWeek="Monday" LastDayOfWeek="Sunday" Width="100%" Height="90%" EnableViewState="false" Skin="Default" AppointmentStyleMode="Simple" ShowFooter="false" runat="server" ShowViewTabs="true" EnableAdvancedForm="false"
                Culture="cs-CZ" AllowEdit="false" AllowDelete="false" AllowInsert="false" Localization-HeaderToday="Dnes" Localization-ShowMore="více..."
                OnClientAppointmentEditing="OnClientAppointmentEditing" OnClientTimeSlotClick="record_create" OnClientTimeSlotContextMenuItemClicked="record_create"
                Localization-AllDay="Bez času od/do" Localization-HeaderMonth="Měsíc" Localization-HeaderDay="Den" Localization-HeaderWeek="Týden" Localization-HeaderMultiDay="Multi-den"
                HoursPanelTimeFormat="HH:mm" ShowNavigationPane="true" OnClientAppointmentMoveEnd="OnClientAppointmentMoveEnd" OnClientNavigationCommand="OnSchedulerCommand"
                DataSubjectField="p32Name" DataStartField="p31DateTimeFrom_Orig" DataEndField="p31DateTimeUntil_Orig" DataKeyField="pid">

                <DayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="true" />
                <WeekView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="true" />
                <MultiDayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" NumberOfDays="10" />
                <MonthView UserSelectable="true" VisibleAppointmentsPerDay="4" />
                <TimelineView UserSelectable="false" />
                <AgendaView UserSelectable="true" NumberOfDays="5" />
                <AppointmentTemplate>
                    <a class="pp1" onclick="RCM('p31',<%# Eval("ID")%>,this)"></a>
                    <a class="reczoom" rel="<%# Eval("Description")%>">i</a>
                    <a href="javascript:re(<%# Eval("ID")%>)"><%# Eval("Subject")%></a>
                    
                    

                </AppointmentTemplate>

                <TimeSlotContextMenus>
                    <telerik:RadSchedulerContextMenu>
                        <Items>
                            <telerik:RadMenuItem Text="Zapsat úkon" ImageUrl="Images/worksheet.png"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true" Text="."></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Jdi na DNES" Value="CommandGoToToday" />
                        </Items>
                    </telerik:RadSchedulerContextMenu>
                </TimeSlotContextMenus>
                <ExportSettings OpenInNewWindow="true" FileName="SchedulerExport">
                    <Pdf PageTitle="Schedule" Author="Telerik" Creator="Telerik" Title="Schedule" />
                </ExportSettings>

            </telerik:RadScheduler>
            <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" RenderMode="Lightweight" Transparency="30" BackColor="#E0E0E0">
                <div style="float: none; padding-top: 80px;">
                    <img src="Images/loading.gif" />
                    <h2>LOADING...</h2>
                </div>
            </telerik:RadAjaxLoadingPanel>
        </div>

    </div>


    <asp:Button ID="cmdHardRefreshOnBehind" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidJ02IDs" runat="server" />
    <asp:HiddenField ID="hidCurResource" runat="server" />
    <asp:HiddenField ID="hidCurTime" runat="server" />
</asp:Content>
