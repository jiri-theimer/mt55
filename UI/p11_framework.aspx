<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p11_framework.aspx.vb" Inherits="UI.p11_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="p31_subgrid" Src="~/p31_subgrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".slidingDiv2").hide();
            $(".show_hide2").show();

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv2").hide();
                $(".slidingDiv1").slideToggle();
            });

            $('.show_hide2').click(function () {
                $(".slidingDiv1").hide();
                $(".slidingDiv2").slideToggle();
            });



        });

        function loadSplitter(sender) {
            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2 - 1;

            sender.set_height(h3);




        }

        function p31_entry() {

            sw_everywhere("p31_record.aspx?pid=0&p31date=<%=Format(Me.datToday.SelectedDate, "dd.MM.yyyy")%>&j02id=<%=Master.Factory.SysUser.j02ID%>", "Images/worksheet.png");

        }


        function p31_clone() {
            ///volá se z p31_subgrid
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_everywhere("p31_record.aspx?clone=1&pid=" + pid, "Images/worksheet.png");

        }




        function hardrefresh(pid, flag) {

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }





        function p31_RowSelected(sender, args) {

            document.getElementById("<%=hiddatapid_p31.clientid%>").value = args.getDataKeyValue("pid");

        }

        function p31_RowDoubleClick(sender, args) {
            record_p31_edit();
        }

        function record_p31_edit() {
            var pid = document.getElementById("<%=hiddatapid_p31.clientid%>").value;
            sw_everywhere("p31_record.aspx?pid=" + pid, "Images/worksheet.png");

        }

        function p31_subgrid_setting(j74id) {
            sw_everywhere("grid_designer.aspx?prefix=p31&masterprefix=j02&pid=" + j74id, "Images/griddesigner.png", true);

        }

        function p31_entry_attendance(p32id,p41id) {

            sw_everywhere("p31_record.aspx?pid=0&p41id="+p41id+"&p31date=<%=Format(Me.datToday.SelectedDate, "dd.MM.yyyy")%>&j02id=<%=Master.Factory.SysUser.j02ID%>&p32id=" + p32id, "Images/worksheet.png");

        }
        function p31_entry_attendance_scheduler(p32id, p41id) {
            var t1 = "<%=Format(datToday.SelectedDate,"dd.MM.yyyy")%>" + "_" + "<%=Format(Now, "HH.mm")%>";
            var t2 = "<%=Format(datToday.SelectedDate,"dd.MM.yyyy")%>" + "_" + "<%=Format(Now.AddHours(1), "HH.mm")%>";
           
            sw_everywhere("p31_record.aspx?pid=0&scheduler=1&p41id=" + p41id + "&p31date=<%=Format(Me.datToday.SelectedDate, "dd.MM.yyyy")%>&j02id=<%=Master.Factory.SysUser.j02ID%>&p32id=" + p32id+"&t1="+t1+"&t2="+t2, "Images/worksheet.png");

        }
        function report() {

            sw_everywhere("report_modal.aspx?prefix=j02&pid=<%=Master.Factory.SysUser.j02ID%>", "Images/reporting.png", true);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" ResizeMode="Proportional" OnClientLoaded="loadSplitter" PanesBorderSize="0" Skin="Default" RenderMode="Auto">
        <telerik:RadPane ID="navigationPane" runat="server" OnClientResized="" OnClientCollapsed="" OnClientExpanded="" BackColor="white">
            <div style="background-color: white; padding: 10px;">
                <table cellpadding="10">
                    <tr>
                        <td>

                            <asp:ImageButton ID="cmdRefresh" runat="server" ImageUrl="Images/worksheet_32.png" ToolTip="Obnovit" />
                        </td>
                        <td>
                            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Rozhraní docházky"></asp:Label>
                            <div>
                                <asp:HyperLink ID="cmdReport" runat="server" Text="Tisková sestava" NavigateUrl="javascript:report()" meta:resourcekey="cmdReport"></asp:HyperLink>
                            </div>
                            
                            
                            
                        </td>
                      
                        <td>
                            <telerik:RadDatePicker ID="datToday" runat="server" Width="120px" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red" AutoPostBack="true" SharedCalendarID="SharedCalendar">
                                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                            </telerik:RadDatePicker>
                        </td>
                        <td>
                            <asp:ImageButton ID="cmdPrevDay" runat="server" ImageUrl="Images/prevpage.png" ToolTip="Předchozí den" />
                            <asp:ImageButton ID="cmdNextDay" runat="server" ImageUrl="Images/nextpage.png" ToolTip="Další den" />
                        </td>
                        <td>
                            <div class="show_hide1">
                                <asp:HyperLink ID="linkStart" runat="server" CssClass="button-link" Text="Příchod" Font-Size="Large" BackColor="green" ForeColor="white"></asp:HyperLink>
                            </div>

                        </td>
                        <td>
                            <div class="slidingDiv1" style="display: none;">

                                <telerik:RadDateTimePicker ID="p11TodayStart" runat="server" Width="80px" SharedCalendarID="SharedCalendar">
                                    <DateInput ID="DateInput2" DisplayDateFormat="HH:mm" DateFormat="HH:mm" runat="server"></DateInput>
                                    <TimePopupButton Visible="true" />
                                    <DatePopupButton Visible="false" />
                                    <TimeView StartTime="05:00" EndTime="23:00" ShowHeader="false" ShowFooter="false"></TimeView>

                                </telerik:RadDateTimePicker>
                                <asp:Button ID="cmdSaveStart" runat="server" Text="Uložit" CssClass="cmd" />
                            </div>
                        </td>


                    </tr>
                    <tr valign="top">
                        <td>
                            
                        </td>
                        <td>
                            
                        </td>
                        <td></td>
                        <td></td>
                        
                        <td>
                            <div class="show_hide2">
                                <asp:HyperLink ID="linkEnd" runat="server" CssClass="button-link" Text="Odchod" Font-Size="Large" BackColor="red" ForeColor="white"></asp:HyperLink>
                            </div>
                        </td>
                        <td>
                            <div class="slidingDiv2" style="display: none;">

                                <telerik:RadDateTimePicker ID="p11TodayEnd" runat="server" Width="80px" SharedCalendarID="SharedCalendar">
                                    <DateInput ID="DateInput3" DisplayDateFormat="HH:mm" DateFormat="HH:mm" runat="server"></DateInput>
                                    <TimePopupButton Visible="true" />
                                    <DatePopupButton Visible="false" />
                                    <TimeView StartTime="05:00" EndTime="23:00" ShowHeader="false" ShowFooter="false"></TimeView>

                                </telerik:RadDateTimePicker>
                                <asp:Button ID="cmdSaveEnd" runat="server" Text="Uložit" CssClass="cmd" />
                            </div>
                        </td>
                    </tr>
                </table>


                <asp:Repeater ID="rp1" runat="server">
                    <ItemTemplate>
                        <div style="float: left; padding: 10px;">
                            <asp:HyperLink ID="link1" runat="server" CssClass="button-link" ForeColor="Brown" Style="padding: 10px;"></asp:HyperLink>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div style="clear: both;"></div>


                <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true" Style="margin-top: 50px;">
                    <Tabs>
                        <telerik:RadTab Text="Vykázáno" Selected="true" Value="core"></telerik:RadTab>
                        <telerik:RadTab Text="Nastavení" Value="billing"></telerik:RadTab>

                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
                    <telerik:RadPageView ID="core" runat="server" Selected="true">
                        <uc:p31_subgrid ID="gridP31" runat="server" EntityX29ID="j02Person" OnRowSelected="p31_RowSelected" OnRowDblClick="p31_RowDoubleClick"></uc:p31_subgrid>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageView1" runat="server">
                        <div class="div6">
                            <asp:Button ID="cmdSaveSetting" runat="server" Text="Uložit změny v nastavení" CssClass="cmd" Font-Bold="true" />
                        </div>

                        <table>
                            <tr>
                                <td>Výchozí projekt pro zapisování nefakturovatelných hodin:</td>
                                <td>
                                    <uc:project ID="p41ID_Default" runat="server" AutoPostBack="true" Flag="p31_entry" Width="330px" Text="Hledat projekt..." />
                                </td>
                            </tr>
                        </table>
                        <div class="content-box2" style="margin-top:20px;">
                            <div class="title">
                                Nastavit si tlačítka často používaných aktivit
                                <asp:Button ID="cmdAdd" runat="server" CssClass="cmd" Text="Přidat" />
                            </div>
                            <div class="content">
                                <table>
                            <asp:Repeater ID="rp2" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="p32ID" runat="server" AutoPostBack="false" DataValueField="pid" DataTextField="NameWithSheet"></asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="p32AttendanceFlag" runat="server" AutoPostBack="false">
                                                <asp:ListItem Text="Bez nabídky přesného času od-do" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Nabízet přesný čas od-do" Value="2"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="cmdDelete" runat="server" CommandName="delete" ImageUrl="Images/delete.png" CssClass="button-link" ToolTip="Odstranit položku"></asp:ImageButton>
                                            <asp:HiddenField ID="p85ID" runat="server" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                            </div>
                        </div>
                        
                    </telerik:RadPageView>
                </telerik:RadMultiPage>



                <asp:HiddenField ID="hiddatapid_p31" runat="server" />


                <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
                    <SpecialDays>
                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                    </SpecialDays>
                </telerik:RadCalendar>
            </div>
        </telerik:RadPane>
        <telerik:RadPane ID="rightPane" runat="server" Width="350px" ContentUrl="p31_framework_timer.aspx">
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
