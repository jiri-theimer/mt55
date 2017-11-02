<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="entity_scheduler.aspx.vb" Inherits="UI.entity_scheduler" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="persons" Src="~/persons.ascx" %>
<%@ Register TagPrefix="uc" TagName="projects" Src="~/projects.ascx" %>

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

        div.RadScheduler .rsMonthView .rsTodayCell {
            background-color: skyblue;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {

            $(".slidingDiv2").hide();
            $(".show_hide2").show();
            $(".show_hide3").show();
            $(".slidingDiv3").hide();

            <%If Me.CurrentMasterPrefix <> "" Then%>
            $(".show_hide2").hide();
            $(".show_hide3").hide();
            <%End If%>

            $('.show_hide2').click(function () {
                $(".slidingDiv1").hide();
                $(".slidingDiv3").hide();
                $(".slidingDiv2").slideToggle();
            });


            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv2").hide();
                $(".slidingDiv3").hide();
                $(".slidingDiv1").slideToggle();
            });

            $('.show_hide3').click(function () {
                $(".slidingDiv1").hide();
                $(".slidingDiv2").hide();
                $(".slidingDiv3").slideToggle();
            });

            <%If hidIsLoadingSetting.Value = "1" Then%>
            $('.show_hide1').click();
            document.getElementById("<%=hidIsLoadingSetting.ClientID%>").value = "";
            <%End If%>
            <%If hidIsPersonsChange.Value = "1" Then%>
            $('.show_hide2').click();
            document.getElementById("<%=hidIsPersonsChange.ClientID%>").value = "";
            <%End If%>
            <%If hidIsProjectsChange.Value = "1" Then%>
            $('.show_hide3').click();
            document.getElementById("<%=hidIsProjectsChange.ClientID%>").value = "";
            <%End If%>

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


        function sw_local(url, img, is_maximize) {
            sw_master(url, img, is_maximize);
        }


        function hardrefresh(pid, flag) {

            location.replace("entity_scheduler.aspx");

        }

        function o22_record(pid) {

            sw_master("o22_record.aspx?pid=" + pid + "&masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=me.CurrentMasterPID%>", "Images/calendar.png")
        }

        function o22_clone(pid) {

            sw_master("o22_record.aspx?clone=1&pid=" + pid, "Images/milestone.png")
        }

        function p48_record(pid) {

            sw_master("p48_multiple_edit_delete.aspx?p48ids=" + pid, "Images/oplan.png")
        }
        function p48_convert(pid) {

            sw_master("p31_record.aspx?pid=0&p48id=" + pid, "Images/worksheet.png")
        }
        function p31_record(pid) {

            sw_master("p31_record.aspx?pid=" + pid, "Images/worksheet.png")
        }

        function p56_record(pid) {
            var masterpid = "0";
            <%If Me.CurrentMasterPrefix = "p41" Then%>
            masterpid = "<%=me.CurrentMasterPID%>";
            <%End If%>
            sw_master("p56_record.aspx?masterprefix=p41&masterpid=" + masterpid + "&pid=" + pid, "Images/task.png")
        }

        function re(pid, prefix) {
            if (prefix == 'o22')
                o22_record(pid);

            if (prefix == 'p48')
                p48_record(pid);

            if (prefix == 'p56')
                p56_record(pid);
        }

        function record_create_contextmenu(sender, eventArgs) {
            var clickedItem = eventArgs.get_item();
            var val = clickedItem.get_value();

            var firstSlot = sender.get_selectedSlots()[0];

            var lastSlot = sender.get_selectedSlots()[sender.get_selectedSlots().length - 1];
            var d1 = firstSlot.get_startTime()
            var d2 = lastSlot.get_endTime();

            if (d1.getHours() == 0 && d2.getHours() == 0) {
                d2.setDate(d2.getDate() - 1);
            }

            if (val == "p56")
                return;

            var url = "";
            var j02id = "<%=Master.Factory.SysUser.j02ID%>";
            if (val == "p48") {
                url = "p48_multiple_create.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id;
            }
            if (val == "o22") {
                url = "o22_record.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id + "&masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=me.CurrentMasterPID%>";

            }
            sw_master(url, "Images/calendar.png")
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
            
            <%If cbxNewRecType.SelectedValue = "p48" Then%>
            var url = "p48_multiple_create.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id;
            <%End If%>
            <%If cbxNewRecType.SelectedValue = "o22" Then%>
            var url = "o22_record.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id + "&masterprefix=<%=Me.CurrentMasterPrefix%>&masterpid=<%=me.CurrentMasterPID%>";
            <%End If%>
            <%If cbxNewRecType.SelectedValue = "p56" Then%>
            var url = "p56_record.aspx?t1=" + formattedDate(d1) + "&t2=" + formattedDate(d2) + "&j02id=" + j02id + "&masterprefix=p41";
            <%If Me.CurrentMasterPrefix = "p41" Then%>
            url = url + "&masterpid=<%=me.CurrentMasterPID%>";
            <%Else%>
            url = url + "&masterpid=0";
            <%End If%>
            <%End If%>

            sw_master(url, "Images/calendar.png")

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
                <%If cbxNewRecType.SelectedValue = "p48" Then%>
                var url = "p48_multiple_create.aspx?d1=" + formattedDate(d1) + "&d2=" + formattedDate(d2);

                <%End If%>
                <%If cbxNewRecType.SelectedValue = "o22" Then%>
                var url = "o22_record.aspx?d1=" + formattedDate(d1) + "&d2=" + formattedDate(d2);
                <%End If%>

                sw_master(url, "Images/oplan.png");

            }
            else {
                //The node was dropped elsewhere on the document.
                eventArgs.set_cancel(true);
            }
        }

        function OnSchedulerCommand(sender, args) {
            var loadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
            loadingPanel.show(sender.get_id());

        }

        function querybuilder() {
            var j70id = document.getElementById("<%=Me.j70ID.ClientID%>").value;
            sw_master("query_builder.aspx?prefix=p56&x36key=entity_scheduler-j70id&pid=" + j70id, "Images/query.png");
            return (false);
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div id="offsetY"></div>
    <div id="divScheduler">

        <div id="left_panel" style="float: left; width: 250px;">
            <div style="float: left;">
                <img src="Images/calendar_32.png" />
            </div>
            <div class="div6" style="float: left;">

                <span class="page_header_span">Kalendář</span>
            </div>

            <div style="clear: both;"></div>
            <div>
                <asp:HyperLink ID="clue_query" runat="server" CssClass="reczoom" ToolTip="Detail filtru" Text="i"></asp:HyperLink>
                <asp:DropDownList ID="j70ID" runat="server" AutoPostBack="true" DataTextField="NameWithMark" DataValueField="pid" Style="width: 200px;" ToolTip="Pojmenovaný filtr úkolů"></asp:DropDownList>
                <asp:ImageButton ID="cmdQuery" runat="server" OnClientClick="return querybuilder()" ImageUrl="Images/query.png" ToolTip="Návrhář filtrů" CssClass="button-link" />
            </div>

            <div style="clear: both;"></div>
            <asp:Panel ID="panMasterRecord" runat="server" CssClass="div6">
                <asp:Image ID="imgMaster" runat="server" />

                <asp:HyperLink ID="MasterRecord" runat="server"></asp:HyperLink>
                <div>
                    <asp:CheckBox ID="chkIncludeChilds" runat="server" Text="Zahrnout i pod-projekty" Visible="false" CssClass="chk" AutoPostBack="true" />
                </div>
            </asp:Panel>

            <div style="clear: both;"></div>
            <div class="show_hide2" style="float: left; margin-top: 8px;">
                <button type="button">
                    <img src="Images/arrow_down.gif" alt="Výběr osob" />
                    <asp:Label ID="PersonsHeader" runat="server"></asp:Label>

                </button>

            </div>
            <div style="clear: both;"></div>
            <div class="slidingDiv2" style="padding-bottom: 20px; display: none;">

                <uc:persons ID="persons1" runat="server"></uc:persons>
            </div>

            <div style="clear: both;"></div>
            <div class="show_hide3" style="float: left; margin-top: 8px;">
                <button type="button">
                    <img src="Images/arrow_down.gif" alt="Výběr projektů" />
                    <asp:Label ID="ProjectsHeader" runat="server"></asp:Label>

                </button>

            </div>
            <div style="clear: both;"></div>
            <div class="slidingDiv3" style="padding-bottom: 20px; display: none;">
                <uc:projects ID="projects1" runat="server"></uc:projects>
            </div>

            <div style="clear: both;"></div>
            <div class="show_hide1" style="float: left; margin-top: 50px;">
                <button type="button">
                    <img src="Images/arrow_down.gif" alt="Nastavení" />
                    Nastavení

                </button>

            </div>






            <div style="clear: both;"></div>
            <div class="slidingDiv1" style="display: none;">
                <div class="content-box3">
                    <div class="title">Nastavení</div>
                    <div class="content">
                        <div>
                            Na klik v kalendáři založit:
                    
                        </div>
                        <div class="div6">
                            <asp:DropDownList ID="cbxNewRecType" runat="server" AutoPostBack="true">
                                <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                                <asp:ListItem Text="Událost v kalendáři" Value="o22"></asp:ListItem>
                                <asp:ListItem Text="Operativní plán" Value="p48"></asp:ListItem>

                            </asp:DropDownList>
                        </div>
                        <asp:Panel ID="panSettingOPlan" runat="server" CssClass="div6">
                            <asp:CheckBox ID="chkSetting_P48" runat="server" Text="Zobrazovat i operatavní plán" AutoPostBack="true" CssClass="chk" />
                        </asp:Panel>
                        <div class="div6" style="display: none;">
                            <img src="Images/calendar.png" />
                            <asp:CheckBox ID="chkSetting_O22" runat="server" Checked="true" Text="Zobrazovat kalendářové události" AutoPostBack="true" CssClass="chk" />
                        </div>
                        <div class="div6" style="display: none;">
                            <img src="Images/task.png" />
                            <asp:CheckBox ID="chkSetting_P56" runat="server" Checked="true" Text="Zobrazovat úkoly s termínem" AutoPostBack="true" CssClass="chk" />
                        </div>

                        <div class="div6">
                            <span>Začátek v rozhraní [Den/Týden/Multi]:</span>
                            <asp:DropDownList ID="entity_scheduler_daystarttime" runat="server" AutoPostBack="true">
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
                            <span>Konec v rozhraní [Den/Týden/Multi]:</span>
                            <asp:DropDownList ID="entity_scheduler_dayendtime" runat="server" AutoPostBack="true">
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
                            <asp:DropDownList ID="entity_scheduler_multidays" runat="server" AutoPostBack="true">
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
                            <span>Počet dní v [Agenda]:</span>
                            <asp:DropDownList ID="entity_scheduler_agendadays" runat="server" AutoPostBack="true">
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100" Value="100"></asp:ListItem>


                            </asp:DropDownList>
                        </div>


                        <div class="div6">
                            <asp:Button ID="cmdExportICalendar" runat="server" CssClass="cmd" Text="Export do ICalendar" />
                        </div>
                        <div class="div6">
                            <asp:Button ID="cmdExportPDF" runat="server" CssClass="cmd" Text="PDF export" />
                        </div>
                        <div class="div6">
                            <img src="Images/help.png" /><i>Zápis do kalendáře provedete přes pravé tlačítko myši nad označenými buňkami nebo přes click do kalendáře.</i>
                        </div>
                    </div>
                </div>
            </div>


        </div>
        <div id="right_panel" style="margin-left: 250px;">
            <telerik:RadScheduler ID="scheduler1" SelectedView="WeekView" RenderMode="Auto" FirstDayOfWeek="Monday" LastDayOfWeek="Sunday" Width="100%" Height="90%" EnableViewState="false" Skin="Default" AppointmentStyleMode="Simple" ShowFooter="false" runat="server" ShowViewTabs="true" EnableAdvancedForm="false"
                AllowEdit="false" AllowDelete="false" AllowInsert="false"
                OnClientAppointmentEditing="OnClientAppointmentEditing" OnClientTimeSlotClick="record_create"
                HoursPanelTimeFormat="HH:mm" ShowNavigationPane="true" OnClientAppointmentMoveEnd="OnClientAppointmentMoveEnd" OnClientNavigationCommand="OnSchedulerCommand" OnClientTimeSlotContextMenuItemClicked="record_create_contextmenu"
                DataSubjectField="o22Name" DataStartField="o22DateFrom" DataEndField="o22DateUntil" DataKeyField="pid">                
                <DayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="true" />
                <WeekView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" ShowInsertArea="true" />
                <MultiDayView UserSelectable="true" DayStartTime="08:00" DayEndTime="22:00" NumberOfDays="10" />
                <TimelineView UserSelectable="true" NumberOfSlots="7" />
                <AgendaView UserSelectable="true" NumberOfDays="20" />
                <MonthView UserSelectable="true" VisibleAppointmentsPerDay="4" />

                <AppointmentTemplate>
                    <a class="reczoom" rel="<%# Eval("Description")%>">i</a>
                    <a href="javascript:re(<%# Eval("ID")%>)"><%# Eval("Subject")%></a>


                </AppointmentTemplate>

                <TimeSlotContextMenus>
                    <telerik:RadSchedulerContextMenu>
                        <Items>
                            <telerik:RadMenuItem Text="Operativní plán" ImageUrl="Images/oplan.png" Value="p48"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Kalendářová událost" ImageUrl="Images/milestone.png" Value="o22"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Úkol" ImageUrl="Images/task.png" NavigateUrl="javascript:p56_record(0)" Value="p56"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true" Text="."></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Jdi na DNES" Value="CommandGoToToday" />
                        </Items>
                    </telerik:RadSchedulerContextMenu>
                </TimeSlotContextMenus>
                <ExportSettings OpenInNewWindow="true" FileName="MARKTIME_EXPORT">
                    <Pdf Author="MARKTIME" Creator="MARKITME" PaperSize="A4" />
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





    <asp:HiddenField ID="hidCurResource" runat="server" />
    <asp:HiddenField ID="hidCurTime" runat="server" />

    <asp:HiddenField ID="hidMasterPrefix" runat="server" />
    <asp:HiddenField ID="hidMasterPID" runat="server" />


    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidIsLoadingSetting" runat="server" Value="" />
    <asp:HiddenField ID="hidIsPersonsChange" runat="server" Value="" />
    <asp:HiddenField ID="hidIsProjectsChange" runat="server" Value="" />
</asp:Content>

