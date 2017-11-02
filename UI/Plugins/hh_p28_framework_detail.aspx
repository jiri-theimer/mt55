<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="p28_address" Src="~/p28_address.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_list" Src="~/o23_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Dim lisPars As New List(Of String)
            With lisPars                
                .Add("p28_framework_detail-pid")
                .Add("hh_p28_framework_detail-gridtype")
                .Add("periodcombo-custom_query")
                .Add("hh_p28_framework_detail-period")
                .Add("hh_p28_framework_detail-cbxGridQuery")
            End With
            With Master.Factory.j03UserBL
                .InhaleUserParams(lisPars)
                If Master.DataPID <> BO.BAS.IsNullInt(.GetUserParam("p28_framework_detail-pid", "0")) And Master.DataPID <> 0 Then
                    .SetUserParam("p28_framework_detail-pid", Master.DataPID)
                End If
                UI.basUI.SelectRadiolistValue(Me.opgGridType, .GetUserParam("hh_p28_framework_detail-gridtype", "none"))
                
                UI.basUI.SelectDropdownlistValue(Me.cbxGridQuery, .GetUserParam("hh_p28_framework_detail-cbxGridQuery"))
                
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("hh_p28_framework_detail-period")
            End With
            
            
                RefreshRecord()
        End If
    End Sub
    
    Private Sub cbxGridQuery_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGridQuery.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p28_framework_detail-cbxGridQuery", Me.cbxGridQuery.SelectedValue)
        ReloadPage()
        
    End Sub
    Private Sub ReloadPage()
        Response.Redirect("hh_p28_framework_detail.aspx?pid=" & Master.DataPID.ToString)
    End Sub
    
    
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        Me.panGridHours.Visible = False : panGridInvoices.Visible = False
        Select Case Me.opgGridType.SelectedValue
            Case "hours"
                Me.panGridHours.Visible = True
            Case "invoices"
                Me.panGridInvoices.Visible = True
        End Select
        
        If opgGridType.SelectedValue <> "none" And opgGridType.SelectedValue <> "projects" Then
            period1.Visible = True
        Else
            period1.Visible = False
        End If
    End Sub

    
    Private Sub RefreshRecord()
        Dim cRec As BO.p28Contact = Master.Factory.p28ContactBL.Load(Master.DataPID)
		if cRec is nothing then return
        
        With cRec
            Me.PageHeader.Text = .p28Name
            Me.p28Code.Text = .p28Code
            Me.p28VatID.Text = .p28VatID
            Me.p28RegID.Text = .p28RegID
            Me.Owner.Text = .Owner
        End With
        
        
        
        Dim lisO37 As IEnumerable(Of BO.o37Contact_Address) = Master.Factory.p28ContactBL.GetList_o37(Master.DataPID)
        If lisO37.Count > 0 Then
            Me.address1.FillData(lisO37)
        Else
            Me.panAddress.Visible = False
        End If
        
        Dim mqO23 As New BO.myQueryO23
        mqO23.p28ID = Master.DataPID
        Dim lisO23 As IEnumerable(Of BO.o23Notepad) = Master.Factory.o23NotepadBL.GetList(mqO23)

        If lisO23.Count > 0 Then
            notepad1.RefreshData(lisO23, Master.DataPID)
        Else
            panO23.Visible = False
        End If
        
        
        RefreshFF()
        panProjects.Visible = False
                
        Select Case Me.opgGridType.SelectedValue
            Case "hours"
                RefreshGridHours()
            Case "projects"
                panProjects.Visible = True
                RefreshProjectList()
            Case "invoices"
                RefreshGridInvoices()
        End Select
    End Sub
    
    Private Sub RefreshFF()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("pid", Master.DataPID))
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable("select p28freetext01,dbo.x25_get_text(5,case when p28FreeCombo01 is null then 0 else convert(int,p28FreeCombo01) end) as IG FROM p28Contact_FreeField WHERE p28ID=@pid", pars)
        If dt.Rows.Count > 0 Then
            ff_Partner.Text = BO.BAS.IsNull(dt.Rows(0).Item("p28freetext01"))
            ff_IG.Text = BO.BAS.IsNull(dt.Rows(0).Item("IG"))
        End If
    End Sub
    
    Private Sub RefreshProjectList()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p28id", Master.DataPID))
        
        Dim sIn As String = "SELECT ax.p41ID,sum(case when ax.p71ID IS NULL THEN p31Hours_Orig END) as hodiny_rozpr,sum(case when ax.p71ID=1 AND ax.p91ID IS NULL THEN p31Hours_Approved_Billing END) as hodiny_schval"
        sIn += " FROM p31Worksheet ax INNER JOIN p32Activity p32x ON ax.p32ID=p32x.p32ID INNER JOIN p41Project p41x ON ax.p41ID=p41x.p41ID"
        sIn += " WHERE p41x.p28ID_Client=@p28id"
        sIn += " GROUP BY ax.p41ID"
        
        Dim s As String = "SELECT a.p41ID,a.p41Name,a.p41Code,p42.p42Name,case when getdate() BETWEEN a.p41ValidFrom AND a.p41ValidUntil THEN NULL ELSE 'line-through' END as text_decoration"
        s += ",case when getdate() BETWEEN a.p41ValidFrom AND a.p41ValidUntil THEN 'block' ELSE 'none' END as display_entry"
        s += ",p31.hodiny_rozpr,case when p31.hodiny_rozpr>0 then 'block' else 'none' end as display_approve,case when p31.hodiny_schval>0 then 'block' else 'none' end as display_reapprove"
        s += ",p31.hodiny_schval"
        s += " FROM p41Project a INNER JOIN p42ProjectType p42 ON a.p42ID=p42.p42ID"
        s += " LEFT OUTER JOIN (" & sIn & ") p31 ON a.p41ID=p31.p41ID"
        s += " WHERE a.p28ID_Client=@p28id"
        s += " ORDER BY a.p41ID DESC"
        
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        rpP41.DataSource = dt
        rpP41.DataBind()
        
        'If dt.Rows.Count = 0 Then
        '    panProjects.Style.Item("display") = "none"
        'Else
           
            
        'End If
        
        'Dim lis As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mq)
        'If lis.Count = 0 Then
        '    'boxP41.Visible = False
        'Else
        '    'boxP41.Visible = True
        '    Dim intClosed As Integer = lis.Where(Function(p) p.IsClosed = True).Count
        '    If intClosed > 0 Then
        '        boxP41Title.Text = BO.BAS.OM2(Me.boxP41Title.Text, lis.Where(Function(p) p.IsClosed = False).Count.ToString & "+" & intClosed.ToString)
        '    Else
        '        boxP41Title.Text = BO.BAS.OM2(Me.boxP41Title.Text, lis.Count.ToString)
        '    End If

        'End If


    End Sub
    
    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p28_framework_detail-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub
    Private Sub opgGridType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgGridType.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p28_framework_detail-gridtype", Me.opgGridType.SelectedValue)
        ReloadPage()
    End Sub
    
    Private Sub RefreshGridHours()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p28id", Master.DataPID))
        pars.Add(New BO.PluginDbParameter("d1", period1.DateFrom))
        pars.Add(New BO.PluginDbParameter("d2", period1.DateUntil))
        
        Dim s As String = "SELECT top 1000 a.p31ID,a.p31Date,p41.p41Name,j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,a.p31Text,a.p31hours_orig,a.p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig"
        s += ",a.p91ID,a.p70ID,a.p71ID,a.p72ID_AfterApprove"
        s += " from p31WorkSheet a INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID"
        s += " where p41.p28ID_Client=@p28id AND a.p31Date BETWEEN @d1 AND @d2 AND p34.p33ID=1"
        Select Case Me.cbxGridQuery.SelectedValue
            Case "0"
            Case "1"
                s += " AND a.p71ID IS NULL"
            Case "2"
                s += " AND a.p71ID=1 AND a.p91ID IS NULL"
            Case "3"
                s += " AND a.p91ID IS NOT NULL"
        End Select
        s += " ORDER BY a.p31Date DESC"
        
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        If dt.Rows.Count = 1000 Then
            Master.Notify("Datový přehled může najednou obsahovat maximálně 1000 záznamů.", UI.NotifyLevel.InfoMessage)
        End If
        rpGridHours.DataSource = dt
        rpGridHours.DataBind()
    End Sub
    
    Private Sub RefreshGridInvoices()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p28id", Master.DataPID))
                
        Dim s As String = "SELECT a.p91ID,a.p91Code,a.p91Amount_WithoutVat,a.p91Date,p91Amount_TotalDue,j27Code"
        s += " from p91Invoice a INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " where a.p28ID=@p28id"
        s += " ORDER BY a.p91ID DESC"
        
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        rpGridInvoices.DataSource = dt
        rpGridInvoices.DataBind()
    End Sub
    
    Private Function SFN(val As Object) As String
        If val Is Nothing Then Return ""
        If val Is System.DBNull.Value Then Return ""
        Return BO.BAS.FN(val)
    End Function
    Private Function SFD(val As Object) As String
        If val Is Nothing Then Return ""
        If val Is System.DBNull.Value Then Return ""
        Return Format(val, "dd.MM.yyyy")
    End Function
    
    Private Sub rpGridHours_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpGridHours.ItemDataBound
        Dim cRec As Object = CType(e.Item.DataItem, Object)
        e.Item.FindControl("img1").Visible = False
        If cRec.item("p91ID") Is System.DBNull.Value Then
            If BO.BAS.IsNullInt(cRec.item("p71ID")) = 1 Then
                With CType(e.Item.FindControl("img1"), Image)
                    .Visible = True
                    Select Case cRec.item("p72ID_AfterApprove")
                        Case 2
                            .ImageUrl = "Images/a12.gif"
                        Case 3
                            .ImageUrl = "Images/a13.gif"
                        Case 4
                            .ImageUrl = "Images/a14.gif"
                        Case 6
                            .ImageUrl = "Images/a16.gif"
                    End Select
                End With
            End If
        Else
            With CType(e.Item.FindControl("tdSys"), HtmlTableCell)
                Select Case cRec.item("p70ID")
                    Case 2
                        .Style.Item("background-color") = "red"
                    Case 3
                        .Style.Item("background-color") = "red"
                    Case 4
                        .Style.Item("background-color") = "green"
                    Case 6
                        .Style.Item("background-color") = "pink"
                End Select
                
            End With
        End If
        
        
    End Sub
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>
    <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>

    <style type="text/css">
        .ui-autocomplete {
            width: 600px;
            height: 300px;
            overflow-y: auto;
            /* prevent horizontal scrollbar */
            overflow-x: hidden;
            font-family: 'Microsoft Sans Serif';
            z-index: 9900;
        }

        * html .ui-autocomplete {
            height: 300px;
        }


        .ui-state-hover, .ui-widget-content .ui-state-hover, .ui-widget-header .ui-state-hover, .ui-state-focus, .ui-widget-content .ui-state-focus, .ui-widget-header .ui-state-focus {
            background: #DCDCDC;
            border: none;
            border-radius: 0;
            font-weight: normal;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {

            <%If panProjects.Visible then%>
            oTable1 = $('#tabProjects').dataTable({
                bPaginate: true,
                bFilter: true,
                bStateSave: true,
                bSort: true,


                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }
            

            });
            <%end if%>

            <%If panGridHours.Visible then%>
            oTable1 = $('#tabGrid').dataTable({
                bPaginate: true,
                bFilter: true,
                bStateSave: true,
                bSort: true,


                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }


            });
            <%end If%>
            <%If panGridInvoices.Visible Then%>
            oTable1 = $('#tabGridInvoices').dataTable({
                bPaginate: true,
                bFilter: true,
                bStateSave: true,
                bSort: true,
                "aaSorting": [[1, "desc"]], // výchozí třídění podle druhého sloupce


                oLanguage: {
                    sUrl: 'Scripts/datatable/cs_CZ.txt'

                }


            });
            <%End If%>
        });


        function hardrefresh(pid, flag) {
            window.parent.open("p28_framework.aspx?pid=<%=Master.DataPID%>", "_top");


        }


        function p31_entry(p41id) {

            sw_local("p31_record.aspx?pid=0&p41id=" + p41id, "Images/worksheet_32.png");

        }
        function p31_edit(pid) {

            sw_local("p31_record.aspx?pid=" + pid, "Images/worksheet_32.png");

        }
        function o23_record(pid) {

            sw_local("o23_record.aspx?masterprefix=p28&masterpid=<%=master.datapid%>&pid=" + pid, "Images/notepad_32.png", true);

        }
        function o22_record(pid) {

            sw_local("o22_record.aspx?masterprefix=p28&masterpid=<%=master.datapid%>&pid=" + pid, "Images/calendar_32.png", true);

        }

        function p41_new() {

            sw_local("p41_create.aspx?p28id=<%=Master.DataPID%>", "Images/project_32.png", true);

        }
        function report() {

            sw_local("report_modal.aspx?prefix=p28&pid=<%=Master.DataPID%>", "Images/reporting_32.png", true);

        }
        function p31_approve_all() {

            window.parent.sw_master("p31_approving_step1.aspx?masterprefix=p28&masterpid=<%=master.DataPID%>", "Images/approve_32.png", true);
        }
        function p31_approve_p41(p41id) {

            window.parent.sw_master("p31_approving_step1.aspx?masterprefix=p41&masterpid=" + p41id, "Images/approve_32.png", true);
        }
        function p31_reapprove_p41(p41id) {

            window.parent.sw_master("p31_approving_step1.aspx?reapprove=1&masterprefix=p41&masterpid=" + p41id, "Images/approve_32.png", true);
        }
        function p31_clearapprove_p41(p41id) {

            window.parent.sw_master("p31_approving_step1.aspx?clearapprove=1&masterprefix=p41&masterpid=" + p41id, "Images/clear_32.png", true);
        }

        function search2Focus() {
            document.getElementById("search2").value = "";
            document.getElementById("search2").style.background = "yellow";
        }
        function search2Blur() {

            document.getElementById("search2").style.background = "";
            document.getElementById("search2").value = "Najít jiného klienta...";
        }

        function p91_edit(p91id) {
            window.open("p91_framework.aspx?pid=" + p91id, "_top");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="Images/contact_32.png" alt="Klient" />
            </td>
            <td>
                <asp:Label ID="PageHeader" CssClass="page_header_span" runat="server"></asp:Label>

            </td>
            <td>
                <telerik:RadMenu ID="menu1" Skin="Default" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true">
                    <Items>
                        <telerik:RadMenuItem Text="Záznam klienta" Value="record" ImageUrl="~/Images/menuarrow.png">

                            <Items>
                               
                                <telerik:RadMenuItem Text="Založit nový projekt" NavigateUrl="javascript:p41_new()" Value="cmdCreateP41"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Schvalovat rozpracovanost" NavigateUrl="javascript:p31_approve_all()" Value="cmdApproveAll"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Zapsat poznámku | přílohu" Value="cmdO23" NavigateUrl="javascript:o23_record(0);"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Zapsat kalendářovou událost" Value="cmdO22" NavigateUrl="javascript:o22_record(0);"></telerik:RadMenuItem>
                            </Items>
                        </telerik:RadMenuItem>
                    </Items>

                </telerik:RadMenu>
            </td>
            <td>
                <input id="search2" style="width: 120px;" value="Najít jiného klienta..." onfocus="search2Focus()" onblur="search2Blur()" />
            </td>
            <td>
                <a href="javascript:report()">Tisková sestava</a>
            </td>
        </tr>
    </table>
    <table>
        <tr style="vertical-align: top;">
            <td style="min-width: 300px;">
                <table cellpadding="10">
                    <tr>
                        <td>Kód klienta:</td>
                        <td>
                            <asp:Label ID="p28Code" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>IČ:</td>
                        <td>
                            <asp:Label ID="p28RegID" runat="server" CssClass="valbold"></asp:Label>
                            <span style="padding-left: 15px;">DIČ:</span>
                            <asp:Label ID="p28VatID" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Vlastník záznamu:</td>
                        <td>
                            <asp:Label ID="Owner" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Partner klienta:</td>
                        <td>
                            <asp:Label ID="ff_Partner" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Industry group:</td>
                        <td>
                            <asp:Label ID="ff_IG" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:Panel ID="panAddress" runat="server" CssClass="div6">
                    <fieldset>
                        <legend>
                            <img src="Images/address.png" />Adresy</legend>
                        <uc:p28_address ID="address1" runat="server"></uc:p28_address>
                    </fieldset>
                </asp:Panel>
                <asp:Panel ID="panO23" runat="server" CssClass="div6">
                    <fieldset>
                        <legend>
                            <img src="Images/notepad.png" />Poznámky | Přílohy</legend>
                        <uc:o23_list ID="notepad1" runat="server" EntityX29ID="p28Contact"></uc:o23_list>
                    </fieldset>

                </asp:Panel>
            </td>
        </tr>
    </table>

    

    <div class="div_radiolist_metro">
    <asp:RadioButtonList ID="opgGridType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" CellPadding="4" ForeColor="white">
                    <asp:ListItem Text="Hodiny" Value="hours"></asp:ListItem>
                    <asp:ListItem Text="Náklady" Value="expenses"></asp:ListItem>
                    <asp:ListItem Text="Faktury" Value="invoices"></asp:ListItem>
                    <asp:ListItem Text="Projekty klienta" Value="projects"></asp:ListItem>
                    <asp:ListItem Text="Nezobrazovat" Value="nothing" Selected="true"></asp:ListItem>
                </asp:RadioButtonList>
    </div>
    <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>

    <asp:Panel ID="panGridHours" runat="server">
    <asp:DropDownList ID="cbxGridQuery" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Bez filtrování" Value="0"></asp:ListItem>
        <asp:ListItem Text="Pouze rozpracované úkony" Value="1"></asp:ListItem>
        <asp:ListItem Text="Pouze schválené úkony, které čekají na fakturaci" Value="2"></asp:ListItem>
        <asp:ListItem Text="Pouze vyfakturované úkony" Value="3"></asp:ListItem>
    </asp:DropDownList>
    <table id="tabGrid">        
        <thead>
        <tr>
            <th></th>
            <th>Datum</th>
            <th>Projekt</th>
            <th>Osoba</th>
            
           
            <th>Aktivita</th>
            <th style="text-align:right;">Hodiny</th>
            <th style="text-align:right;">Sazba</th>
            <th style="text-align:right;">Částka</th>
            <th>Text</th>
        </tr>
        </thead>
        <tbody>
        <asp:Repeater ID="rpGridHours" runat="server">
            <ItemTemplate>
                <tr class="trHover">
                    <td id="tdSys" runat="server" style="width: 10px;">
                                <asp:Image ID="img1" runat="server" />
                            </td>
                    <td>
                        <a href="javascript:p31_edit(<%#Eval("p31id")%>);"><%#SFD(Eval("p31Date"))%></a>
                    </td>
                    <td>
                        <%#Eval("p41Name")%>
                    </td>
                    <td>
                        <%#Eval("Person")%>
                    </td>
                    <td>
                        <%#Eval("p32Name")%>
                    </td>
                    <td style="text-align:right;">
                        <%#SFN(Eval("p31hours_orig"))%>
                    </td>
                    <td style="text-align:right;">
                        <%#SFN(Eval("p31Rate_Billing_Orig"))%>
                        
                    </td>
                    <td style="text-align:right;">
                        <%#SFN(Eval("p31Amount_WithoutVat_Orig"))%>
                        
                    </td>
                    <td>
                        <%#Eval("p31Text")%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </tbody>
    </table>
    </asp:Panel>

    <asp:Panel ID="panProjects" runat="server">
        
        <table style="border-collapse: collapse;" class="PluginDataTable" id="tabProjects">
            <thead>
                <tr>
                    <th>Kód</th>
                    <th>Název</th>
                    <th>Rozpracovanost (hod.)</th>
                    <th>Čeká na fakturaci (hod.)</th>
                    <th>Typ</th>

                    <th></th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rpP41" runat="server">
                    <ItemTemplate>
                        <tr class="trHover" style="text-decoration: <%#Eval("text_decoration")%>;">


                            <td>
                                <a href="p41_framework.aspx?pid=<%#Eval("p41id")%>" target="_top"><%#Eval("p41Code")%></a>

                            </td>
                            <td>
                                <%#Eval("p41Name")%>

                            </td>

                            <td style="text-align: right;">
                                <a href="javascript:p31_approve_p41(<%#Eval("p41ID")%>)" style="display: <%#Eval("display_approve")%>"><%#Eval("hodiny_rozpr")%></a>
                            </td>
                            <td style="text-align: right;">

                                <a href="javascript:p31_reapprove_p41(<%#Eval("p41ID")%>)" style="display: <%#Eval("display_reapprove")%>"><%#Eval("hodiny_schval")%></a>
                            </td>

                            <td>
                                <%#Eval("p42Name")%>

                            </td>
                            <td>
                                <a href="javascript:p31_entry(<%#Eval("p41ID")%>)" style="display: <%#Eval("display_entry")%>">Zapsat úkon</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="panGridInvoices" runat="server">
        <table id="tabGridInvoices">
            <thead>
                <tr>
                    <th></th>
                             
                    <th>Vystaveno</th>
                    <th style="text-align: right;">Bez DPH</th>
                    <th style="text-align: right;">Celkem</th>
                    <th>Měna</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rpGridInvoices" runat="server">
                    <ItemTemplate>
                        <tr class="trHover">
                            
                            <td>


                                <a href="javascript:p91_edit(<%#Eval("p91id")%>);"><%#Eval("p91Code")%></a>
                            </td>
                         
                            <td>
                                <%#SFD(Eval("p91Date"))%>
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p91Amount_WithoutVat"))%>
                            </td>
                           
                            <td style="text-align: right;">
                                <%#SFN(Eval("p91Amount_TotalDue"))%>
                        
                            </td>
                            <td>
                                <%#Eval("j27Code")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </asp:Panel>

    <script type="text/javascript">
        $(function () {

            $("#search2").autocomplete({
                source: "Handler/handler_search_contact.ashx",
                minLength: 1,
                select: function (event, ui) {
                    if (ui.item) {
                        window.open("p28_framework.aspx?pid=" + ui.item.PID, "_top");
                        return false;
                    }
                }



            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                var s = "<div>";
                if (item.Closed == "1")
                    s = s + "<a style='text-decoration:line-through;'>";
                else
                    s = s + "<a>";

                s = s + __highlight(item.Project, item.FilterString);


                s = s + "</a>";



                s = s + "</div>";


                return $(s).appendTo(ul);


            };
        });

        function __highlight(s, t) {
            var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
            return s.replace(matcher, "<strong>$1</strong>");
        }

        function search2Focus() {
            document.getElementById("search2").value = "";
            document.getElementById("search2").style.background = "yellow";
        }
        function search2Blur() {

            document.getElementById("search2").style.background = "";
            document.getElementById("search2").value = "Najít jiného klienta...";
        }
    </script>
</asp:Content>
