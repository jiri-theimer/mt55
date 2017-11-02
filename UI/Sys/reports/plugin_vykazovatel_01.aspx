<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="timesheet_calendar" Src="~/timesheet_calendar.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            With Master.Factory.j03UserBL
                cal1.CalendarColumns = BO.BAS.IsNullInt(.GetUserParam("p31_framework_detail-calendarcolumns", "1"))
            End With
            Dim intPID As Integer = BO.BAS.IsNullInt(Request.Item("pid"))
            If intPID > 0 Then
                Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(intPID)
                cal1.SelectedDate = cRec.p31Date
            Else
                cal1.SelectedDate = Today
            End If
            If Request.Item("caller") <> "" Then
                cmdPersonalPage.Visible=false
            End If
        End If
    End Sub
    
    Private Sub cal1_NeedDataSource(ByRef lisGetMeWorksheetHours As IEnumerable(Of BO.p31WorksheetCalendarHours)) Handles cal1.NeedDataSource
        lisGetMeWorksheetHours = Master.Factory.p31WorksheetBL.GetList_CalendarHours(Master.Factory.SysUser.j02ID, cal1.VisibleStartDate, cal1.VisibleEndDate)
    End Sub
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        Dim d1 As Date = Me.cal1.SelectedDate
        If Me.cal1.VisibleStartDate > d1 Then d1 = Me.cal1.VisibleStartDate
        Me.CurrentPerson.Text = Master.Factory.SysUser.Person
        Me.CurrentMonth.Text = "[" & Format(d1, "MM-yyyy") & "]"
        
        tabHours1.AddDbParameter("j02id", Master.Factory.SysUser.j02ID)
        tabHours1.AddDbParameter("d1", d1)
        tabHours1.GenerateTable(Master.Factory, tabHours1_sql.Value)
        
        
        If BO.BAS.IsNullDBDate(cal1.SelectedDate) Is Nothing Then
            cal1.SelectedDate = BO.BAS.ConvertString2Date(Me.hidLastSelectedDate.Value)
        End If
        tabList.AddDbParameter("j02id", Master.Factory.SysUser.j02ID)
        tabList.AddDbParameter("d1", cal1.SelectedDate)
        tabList.TableCaption = "Přehled úkonů pro den [" & BO.BAS.FD(cal1.SelectedDate) & "]"
        tabList.GenerateTable(Master.Factory, tabList_sql.Value)
        
        
        Me.hidLastSelectedDate.Value = Format(cal1.SelectedDate, "dd.MM.yyyy")
    End Sub

    
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {

            oTable1 = $('#gridList').dataTable({
                bPaginate: false,
                bFilter: true,
                bStateSave: true,
                bSort: false,


                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }

            });






        });


        function hardrefresh(pid, flag) {
            if (flag == "j03_myprofile") {
                window.parent.open("default.aspx", "_self");
            }

            if (flag == "p31-save" || flag == "p31-delete") {
                form1.submit();
            }


        }

        function personalpage() {
            sw_local("../j03_myprofile_defaultpage.aspx", "Images/plugin_32.png")


        }

        function p31_entry(p41id) {

            sw_local("../p31_record.aspx?pid=0&p41id=" + p41id, "Images/worksheet_32.png");

        }
        function p31_edit(pid) {

            sw_local("../p31_record.aspx?pid=" + pid, "Images/worksheet_32.png");

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="../Images/plugin_32.png" />
            </td>
            <td>
                <span class="page_header_span">Vykazuji hodiny na projektech</span>
            </td>
           
        </tr>
    </table>

    <table cellpadding="10">
        <tr>
            <td style="vertical-align: top;">
                <uc:timesheet_calendar ID="cal1" runat="server" />
               
                <div class="div6">
                    <button type="button" onclick="p31_entry()">Zapsat worksheet</button>
                </div>
            </td>
            <td style="vertical-align: top;">
                <div class="div6" style="text-align: center;">
                    <asp:Label ID="CurrentPerson" runat="server" CssClass="valboldblue"></asp:Label>
                    <asp:Label ID="CurrentMonth" runat="server" CssClass="valboldred"></asp:Label>
                    <span style="padding-left:20px;"></span>
                    
                </div>
                <uc:plugin_datatable ID="tabHours1" TableID="gridHours1" runat="server"
                    ColHeaders="Klient|Projekt||Vykázané hodiny|Z toho fakturovatelné"
                    ColHideRepeatedValues="1" ColTypes="S|S|HTML|N|N" ColFlexSubtotals="1|0|0|11|11"
                    TableCaption="Vykázané hodiny na projektech" />
                
            </td>
        </tr>
    </table>

    <asp:HiddenField ID="tabHours1_sql" Visible="false" runat="server" Value="select min(p28Name),min(p41name)
        ,'<button type=button onclick=p31_entry('+convert(varchar(10),a.p41id)+')><img src='+char(34)+'../Images/worksheet.png'+char(34)+'/></button>'
        ,sum(p31Hours_Orig)
,sum(case when p32.p32IsBillable=1 THEN p31Hours_Orig END)
from
p31worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID
WHERE a.j02ID=@j02id AND month(a.p31Date)=month(@d1) AND year(a.p31Date)=year(@d1)
GROUP BY a.p41ID
ORDER BY min(p28Name),min(p41Name)" />



<uc:plugin_datatable ID="tabList" TableID="gridList" runat="server"
 ColHeaders="|Datum|Klient|Projekt|Aktivita|Hodnota|Popis"
 ColHideRepeatedValues="0" ColTypes="HTML|D|S|S|S|N|SX" ColFlexSubtotals="0|0|0|0|0|11|0"
 TableCaption="Přehled vybraných úkonů" />

<asp:HiddenField ID="tabList_sql" Visible="false" runat="server" Value="
select '<button type=button onclick=p31_edit('+convert(varchar(10),a.p31id)+')>Detail</button>'
    ,p31date,p28Name,p41Name,p32Name,p31Value_Orig,p31Text
FROM
p31worksheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID
INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID
LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID
WHERE a.j02ID=@j02id AND a.p31Date=@d1
ORDER BY p31Date" />

    <div class="div6">
        <asp:hyperlink runat="server" id="cmdPersonalPage" navigateurl="javascript:personalpage()" Text="Zvolit si jinou osobní (výchozí) stránku"></asp:hyperlink>
    </div>
    <asp:HiddenField ID="hidLastSelectedDate" runat="server" />
</asp:Content>
