<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_list" Src="~/o23_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>


<script runat="server">
    Protected Sub Page_Load() Handles Me.Load
        If Not Page.IsPostBack Then
            
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            Dim pars As New List(Of String)
            pars.Add("hh_p41_framework_detail-chkShowWorksheet")
            pars.Add("hh_p41_framework_detail-gridtype")
            pars.Add("periodcombo-custom_query")
            pars.Add("hh_p41_framework_detail-period")
            pars.Add("hh_p41_framework_detail-cbxGridQueryHours")
            pars.Add("hh_p41_framework_detail-cbxGridQueryExpenses")
            pars.Add("p41_framework_detail-pid")
            Master.Factory.j03UserBL.InhaleUserParams(pars)
            
                       
            With Master.Factory.j03UserBL
                If Master.DataPID <> BO.BAS.IsNullInt(.GetUserParam("p41_framework_detail-pid", "0")) And Master.DataPID <> 0 Then
                    .SetUserParam("p41_framework_detail-pid", Master.DataPID)
                End If
                Me.chkShowWorksheet.Checked = BO.BAS.BG(.GetUserParam("hh_p41_framework_detail-chkShowWorksheet", "1"))
                UI.basUI.SelectRadiolistValue(Me.opgGridType, .GetUserParam("hh_p41_framework_detail-gridtype", "none"))
                
                UI.basUI.SelectDropdownlistValue(Me.cbxGridQueryHours, .GetUserParam("hh_p41_framework_detail-cbxGridQueryHours"))
                period1.SetupData(Master.Factory, .GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .GetUserParam("hh_p41_framework_detail-period")
                UI.basUI.SelectDropdownlistValue(Me.cbxGridQueryExpenses, .GetUserParam("hh_p41_framework_detail-cbxGridQueryExpenses"))
            End With
            
            
            RefreshRecord()
        End If
    End Sub
    
    
    
    Protected Sub Page_LoadComplete() Handles Me.LoadComplete
        Me.panWorksheet.Visible = Me.chkShowWorksheet.Checked
        Me.panGridHours.Visible = False : Me.panGridExpenses.Visible = False : Me.panGridInvoices.Visible = False
        
        Select Case Me.opgGridType.SelectedValue
            Case "hours"
                Me.panGridHours.Visible = True
            Case "expenses"
                Me.panGridExpenses.Visible = True
            Case "invoices"
                Me.panGridInvoices.Visible = True
        End Select
        
        If opgGridType.SelectedValue <> "none" Or Me.chkShowWorksheet.Checked Then
            period1.Visible = True
        Else
            period1.Visible = False
        End If
        
    End Sub

    
    Private Sub RefreshRecord()
        Dim cRec As BO.p41Project = Master.Factory.p41ProjectBL.Load(Master.DataPID)
        If cRec Is Nothing Then Return
        
        With cRec
            Me.PageHeader.Text = .FullName
            If .IsClosed Then Me.PageHeader.Font.Strikeout = True
            Me.p41Code.Text = .p41Code
            If .p28ID_Client > 0 Then
                Me.p28Name.Text = .Client
                Me.p28Name.NavigateUrl = "../p28_framework.aspx?pid=" & .p28ID_Client.ToString
            Else
                Me.p28Name.Visible = False
            End If
           
            Me.p42Name.Text = .p42Name
            Me.j18Name.Text = .j18Name
            Me.b02Name.Text = .b02Name
        End With
        
        Dim mqO23 As New BO.myQueryO23
        mqO23.p41ID = Master.DataPID
        Dim lisO23 As IEnumerable(Of BO.o23Notepad) = Master.Factory.o23NotepadBL.GetList(mqO23)

        If lisO23.Count > 0 Then
            notepad1.RefreshData(lisO23, Master.DataPID)
        Else
            panO23.Visible = False
        End If
        
        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = Master.Factory.x67EntityRoleBL.GetList_x69(BO.x29IdEnum.p41Project, cRec.PID)
        Me.roles_project.RefreshData(lisX69, cRec.PID)
        
        If Me.chkShowWorksheet.Checked Then RefreshWorksheet()
        Select Case Me.opgGridType.SelectedValue
            Case "hours"
                RefreshGridHours()
            Case "expenses"
                RefreshGridExpenses()
            Case "invoices"
                RefreshGridInvoices()
        End Select
        
        RefreshLangImage(cRec)
        
        RefreshFF()
    End Sub
    
    Private Sub RefreshLangImage(cRec As BO.p41Project)
        Select Case cRec.p87ID
            Case 1  'english
                imgLang.ImageUrl = "Images/Flags/uk.gif"
            Case 2  'deutchs
                imgLang.ImageUrl = "Images/Flags/germany.gif"
            Case 3
                imgLang.ImageUrl = "Images/Flags/france.gif"
            Case 4
                imgLang.ImageUrl = "Images/Flags/russia.gif"
            Case Else
                imgLang.ImageUrl = "Images/Flags/czechrepublic.gif"
        End Select
    End Sub
    
    Private Sub RefreshGridHours()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p41id", Master.DataPID))
        pars.Add(New BO.PluginDbParameter("d1", period1.DateFrom))
        pars.Add(New BO.PluginDbParameter("d2", period1.DateUntil))
        
        Dim s As String = "SELECT a.p31ID,a.p31Date,j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,a.p31Text,a.p31hours_orig,a.p31Rate_Billing_Orig,p31Amount_WithoutVat_Orig"
        s += ",a.p71ID,a.p72ID_AfterApprove,a.p91ID,a.p70ID"
        s += " from p31WorkSheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID"
        s += " where a.p41ID=@p41id and a.p31Date BETWEEN @d1 AND @d2 AND p34.p33ID=1"
        Select Case Me.cbxGridQueryHours.SelectedValue
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
        rpGridHours.DataSource = dt
        rpGridHours.DataBind()
    End Sub
    Private Sub RefreshGridExpenses()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p41id", Master.DataPID))
        pars.Add(New BO.PluginDbParameter("d1", period1.DateFrom))
        pars.Add(New BO.PluginDbParameter("d2", period1.DateUntil))
        
        Dim s As String = "SELECT a.p31ID,a.p31Date,j02.j02LastName+' '+j02.j02FirstName as Person,p32.p32Name,a.p31Text,p31Amount_WithoutVat_Orig,p31Amount_WithVat_Orig,p31VatRate_Orig"
        s += ",a.p71ID,a.p72ID_AfterApprove,a.p91ID,a.p70ID"
        s += " from p31WorkSheet a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID"
        s += " where a.p41ID=@p41id and a.p31Date BETWEEN @d1 AND @d2 AND p32.p34ID=3"
        Select Case Me.cbxGridQueryExpenses.SelectedValue
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
        rpGridExpenses.DataSource = dt
        rpGridExpenses.DataBind()
    End Sub
    
    Private Sub RefreshGridInvoices()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p41id", Master.DataPID))
                
        Dim s As String = "SELECT a.p91ID,a.p91Code,a.p91Amount_WithoutVat,a.p91Date,p91Amount_TotalDue,j27Code"
        s += " from p91Invoice a INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " where a.p91ID IN (select p91ID FROM p31Worksheet WHERE p41ID=@p41id)"
        s += " ORDER BY a.p91ID DESC"
        
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        rpGridInvoices.DataSource = dt
        rpGridInvoices.DataBind()
    End Sub
    
    Private Sub RefreshWorksheet()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("p41id", Master.DataPID))
        pars.Add(New BO.PluginDbParameter("d1", period1.DateFrom))
        pars.Add(New BO.PluginDbParameter("d2", period1.DateUntil))
        

        Dim s As String = "select SUM(case when a.p71ID IS NULL THEN a.p31hours_orig END) as rozpracovano_hodiny"
        s += ",MIN(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_prvni"
        s += ",MAX(case when a.p71ID IS NULL THEN p31Date END) as rozpracovano_posledni"
        s += ",SUM(case when a.p71ID IS NULL AND p34.p33ID>1 THEN p31Amount_WithoutVat_Orig END) as rozpracovano_ostatni"
        s += ",SUM(case when a.p71ID IS NULL AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_castka_czk"
        s += ",SUM(case when a.p71ID IS NULL AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Orig END) as rozpracovano_castka_eur"
        s += ",SUM(case when a.p71ID IS NULL THEN 1 END) as rozpracovano_pocet"
        s += ",SUM(case when a.p71ID=1 AND a.p91ID IS NULL THEN a.p31Hours_Approved_Billing END) as schvaleno_hodiny"
        s += ",SUM(case when a.p71ID=1 AND p34.p33ID>1 THEN p31Amount_WithoutVat_Approved END) as schvaleno_ostatni"
        s += ",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND a.j27ID_Billing_Orig=2 THEN a.p31Amount_WithoutVat_Approved END) as schvaleno_castka_czk"
        s += ",SUM(case when a.p71ID=1 AND a.p91ID IS NULL AND a.j27ID_Billing_Orig=3 THEN a.p31Amount_WithoutVat_Approved END) as schvaleno_castka_eur"
        s += ",SUM(case when a.p71ID=1 AND a.p91ID IS NULL THEN 1 END) as schvaleno_pocet"
        s += ",SUM(case when a.p91ID IS NOT NULL THEN a.p31Hours_Invoiced END) as vyfakturovano_hodiny"
        s += ",SUM(case when a.p91ID IS NOT NULL THEN a.p31Amount_WithVat_Invoiced END) as vyfakturovano_castka"
        s += ",SUM(case when a.p91ID IS NOT NULL THEN 1 END) as vyfakturovano_pocet"
        s += " from p31WorkSheet a INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID"
        s += " where a.p41ID=@p41id and a.p31Date BETWEEN @d1 AND @d2"

        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable(s, pars)
        rpWorksheet.DataSource = dt
        rpWorksheet.DataBind()
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
    
    Private Sub chkShowWorksheet_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowWorksheet.CheckedChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p41_framework_detail-chkShowWorksheet", BO.BAS.GB(Me.chkShowWorksheet.Checked))
        ReloadPage()
    End Sub
    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p41_framework_detail-period", Me.period1.SelectedValue)
        ReloadPage()
    End Sub
   
    Private Sub opgGridType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgGridType.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p41_framework_detail-gridtype", Me.opgGridType.SelectedValue)
        ReloadPage()
    End Sub
    Private Sub cbxGridQueryHours_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGridQueryHours.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("hh_p41_framework_detail-cbxGridQueryHours", Me.cbxGridQueryHours.SelectedValue)
        ReloadPage()
        
    End Sub
    Private Sub ReloadPage()
        Response.Redirect("hh_p41_framework_detail.aspx?pid=" & Master.DataPID.ToString)
    End Sub
        
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
    
    Private Sub RefreshFF()
        Dim pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("pid", Master.DataPID))
        Dim dt As System.Data.DataTable = Master.Factory.pluginBL.GetDataTable("select dbo.x25_get_text(2,case when p41FreeCombo02 is null then 0 else convert(int,p41FreeCombo02) end) as BU,p41FreeText01 as Osoba FROM p41Project_FreeField WHERE p41ID=@pid", pars)
        If dt.Rows.Count > 0 Then
            ff_BU.Text = BO.BAS.IsNull(dt.Rows(0).Item("BU"))
            ''ff_Osoba.Text = BO.BAS.IsNull(dt.Rows(0).Item("Osoba"))
        End If
    End Sub
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugin.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/datatable/css/jquery.dataTables.css" />
    <script type="text/javascript" src="Scripts/datatable/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {

            <%If panGridHours.Visible Then%>
            oTable1 = $('#tabGridHours').dataTable({
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
            <%If panGridExpenses.Visible Then%>
            oTable1 = $('#tabGridExpenses').dataTable({
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

        function periodcombo_setting() {

            sw_local("periodcombo_setting.aspx", "Images/settings_32.png");
        }

        function hardrefresh(pid, flag) {
            window.parent.open("p41_framework.aspx?pid=<%=Master.DataPID%>", "_top");


        }


        function p31_entry(p41id) {

            sw_local("p31_record.aspx?pid=0&p41id=" + p41id, "Images/worksheet_32.png");

        }
        function workflow() {
            sw_local("workflow_dialog.aspx?prefix=p41&pid=<%=master.datapid%>", "Images/workflow_32.png", false);
        }
        function p31_edit(pid) {

            sw_local("p31_record.aspx?pid=" + pid, "Images/worksheet_32.png");

        }
        function o23_record(pid) {

            sw_local("o23_record.aspx?masterprefix=p41&masterpid=<%=master.datapid%>&pid=" + pid, "Images/notepad_32.png", true);

        }
        function o22_record(pid) {

            sw_local("o22_record.aspx?masterprefix=p41&masterpid=<%=master.datapid%>&pid=" + pid, "Images/calendar_32.png", true);

        }

        function p41_clone() {

            sw_local("p41_create.aspx?clone=1&pid=<%=Master.DataPID%>", "Images/project_32.png", true);

        }
        function p41_create() {

            sw_local("p41_create.aspx?pid=0", "Images/project_32.png", true);

        }
        function p41_edit() {

            sw_local("p41_record.aspx?pid=<%=Master.DataPID%>", "Images/project_32.png", true);

        }
        function report() {

            sw_local("report_modal.aspx?prefix=p41&pid=<%=Master.DataPID%>", "Images/reporting_32.png", true);

        }
        function p31_approve_all() {

            window.parent.sw_master("p31_approving_step1.aspx?masterprefix=p41&masterpid=<%=master.DataPID%>&datefrom=<%=Format(period1.DateFrom, "dd.MM.yyyy")%>&dateuntil=<%=Format(period1.DateUntil,"dd.MM.yyyy")%>", "Images/approve_32.png", true);
        }

        function p31_reapprove_all() {

            window.parent.sw_master("p31_approving_step1.aspx?reapprove=1&masterprefix=p41&masterpid=<%=master.DataPID%>&datefrom=<%=Format(period1.DateFrom, "dd.MM.yyyy")%>&dateuntil=<%=Format(period1.DateUntil,"dd.MM.yyyy")%>", "Images/approve_32.png", true);
        }
        function p31_clearapprove_all() {

            window.parent.sw_master("p31_approving_step1.aspx?clearapprove=1&masterprefix=p41&masterpid=<%=master.DataPID%>&datefrom=<%=Format(period1.DateFrom, "dd.MM.yyyy")%>&dateuntil=<%=Format(period1.DateUntil,"dd.MM.yyyy")%>", "Images/clear_32.png", true);
        }
        function p31_invoice() {

            window.parent.sw_master("p91_create_step1.aspx?nogateway=1&prefix=p41&pid=<%=master.DataPID%>", "Images/invoice_32.png", true);
        }
        function p31_recalc() {
            window.parent.sw_master("p31_recalc.aspx?prefix=p41&pid=<%=master.datapid%>", "Images/recalc_32.png", true);
        }

        function p40_record(p40id) {
            sw_local("p40_record.aspx?p41id=<%=master.datapid%>&pid=" + p40id, "Images/worksheet_recurrence_32.png", true);
        }

        function p91_edit(p91id){
            window.open("p91_framework.aspx?pid=" + p91id,"_top");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10">
        <tr>
            <td id="top">
                <img src="Images/project_32.png" alt="Detail projektu" />
            </td>
            <td>
                <asp:Label ID="PageHeader" CssClass="page_header_span" runat="server"></asp:Label>

            </td>
            <td>
                <telerik:RadMenu ID="menu1" Skin="Default" Style="z-index: 2900;" runat="server" ExpandDelay="0" ExpandAnimation-Type="None" ClickToOpen="true">
                    <Items>
                        <telerik:RadMenuItem Text="Záznam projektu" Value="record" ImageUrl="~/Images/menuarrow.png">

                            <Items>
                                <telerik:RadMenuItem Text="Upravit nastavení projektu" NavigateUrl="javascript:p41_edit()" Value="cmdEditP41"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Založit nový projekt kopírováním" NavigateUrl="javascript:p41_clone()" Value="cmdCreateP41"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Založit nový projekt bez kopírování" NavigateUrl="javascript:p41_create()" Value="cmdCreateP41_b"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Přepočítat sazby rozpracovaných úkonů" NavigateUrl="javascript:p31_recalc()" Value="cmdRecalc"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Schvalovat rozpracovanost" NavigateUrl="javascript:p31_approve_all()" Value="cmdApproveAll"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Zapsat opakovanou odměnu/paušál/úkon" Value="cmdP40" NavigateUrl="javascript:p40_record(0);"></telerik:RadMenuItem>
                                <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Zapsat poznámku | přílohu" Value="cmdO23" NavigateUrl="javascript:o23_record(0);"></telerik:RadMenuItem>
                                <telerik:RadMenuItem Text="Zapsat kalendářovou událost" Value="cmdO22" NavigateUrl="javascript:o22_record(0);"></telerik:RadMenuItem>
                            </Items>
                        </telerik:RadMenuItem>
                    </Items>

                </telerik:RadMenu>
            </td>
            <td>
                <a href="javascript:p31_entry(<%=Master.DataPID%>)">Zapsat úkon</a>
            </td>
             <td>
                <a href="javascript:workflow(<%=Master.DataPID%>)">Posunout/Doplnit</a>
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
                    <tr valign="baseline">
                        <td>Kód projektu:</td>
                        <td>
                            <asp:Label ID="p41Code" runat="server" CssClass="valbold"></asp:Label>
                            <asp:Image ID="imgLang" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>Klient:</td>
                        <td>
                            <asp:HyperLink ID="p28Name" runat="server" Target="_top"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td>Typ projektu:</td>
                        <td>
                            <asp:Label ID="p42Name" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                 
                    <tr>
                        <td>Středisko:</td>
                        <td>
                            <asp:Label ID="j18Name" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    
                    <tr>
                        <td>Stav:</td>
                        <td>
                            <asp:Label ID="b02Name" runat="server" CssClass="valboldred"></asp:Label>
                        </td>
                    </tr>
                </table>

                <asp:Panel ID="panRoles" runat="server" CssClass="div6">
                    <fieldset>
                        <legend>Projektové role</legend>
                        <uc:entityrole_assign_inline ID="roles_project" runat="server" EntityX29ID="p41Project" NoDataText="V projektu nejsou přiřazeny projektové role."></uc:entityrole_assign_inline>
                    </fieldset>

                </asp:Panel>



            </td>
            <td>

                <table cellpadding="6">
                    <tr>
                        <td>Business UNIT:</td>
                        <td>
                            <asp:Label ID="ff_BU" runat="server" CssClass="valbold"></asp:Label></td>
                    </tr>
                  
                </table>

                <div style="padding-top: 5px; padding-bottom: 5px;">
                    <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>
                </div>

                <asp:CheckBox ID="chkShowWorksheet" runat="server" AutoPostBack="true" Text="Zobrazovat worksheet statistiku" />

                <asp:Panel ID="panWorksheet" runat="server">

                    <table border="1" class="tabulka" cellpadding="4" cellspacing="2">
                        <tr>
                            <td colspan="6" style="text-align: center; background-color: ThreeDFace;">Rozpracovanost (čeká na schvalování)</td>
                        </tr>
                        <tr style="font-weight: bold;">
                            <td style="text-align: right;">Hodiny</td>
                            <td style="text-align: right;">Ostatní</td>
                            <td style="text-align: right;">CZK</td>
                            <td style="text-align: right;">EUR</td>
                            <td style="text-align: center;">Počet</td>
                            <td></td>

                        </tr>
                        <asp:Repeater ID="rpWorksheet" runat="server">
                            <ItemTemplate>
                                <tr>


                                    <td style="text-align: right;">
                                        <%#SFN(Eval("rozpracovano_hodiny"))%>
                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("rozpracovano_ostatni"))%>
                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("rozpracovano_castka_czk"))%>

                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("rozpracovano_castka_eur"))%>

                                    </td>
                                    <td>
                                        <span style="color: red;"><%#Eval("rozpracovano_pocet")%>x</span>
                                        <%#SFD(Eval("rozpracovano_prvni"))%>
                                    -
                                    <%#SFD(Eval("rozpracovano_posledni"))%>
                                    
                                    
                                    </td>
                                    <td>

                                        <a href="javascript:p31_approve_all()" style="display: <%#IIF(BO.BAS.IsNullNum(Eval("rozpracovano_pocet"))>0,"block","none")%>;">Schválit</a>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="6" style="text-align: center; background-color: ThreeDFace;">Schváleno | Čeká na fakturaci</td>
                                </tr>
                                <tr style="font-weight: bold;">
                                    <td style="text-align: right;">Hodiny</td>
                                    <td style="text-align: right;">Ostatní</td>
                                    <td style="text-align: right;">CZK</td>
                                    <td style="text-align: right;">EUR</td>
                                    <td style="text-align: right;">Počet</td>
                                    <td></td>

                                </tr>
                                <tr>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("schvaleno_hodiny"))%>
                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("rozpracovano_ostatni"))%>
                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("schvaleno_castka_czk"))%>

                                    </td>
                                    <td style="text-align: right;">
                                        <%#SFN(Eval("schvaleno_castka_eur"))%>

                                    </td>
                                    <td style="text-align: right;">
                                        <%#Eval("schvaleno_pocet")%>x
                                    
                                    </td>
                                    <td>

                                        <a href="javascript:p31_reapprove_all()" style="display: <%#IIF(BO.BAS.IsNullNum(Eval("schvaleno_pocet"))>0,"block","none")%>;">Pře-schválit</a>
                                        <a href="javascript:p31_clearapprove_all()" style="display: <%#IIF(BO.BAS.IsNullNum(Eval("schvaleno_pocet"))>0,"block","none")%>;">Vrátit do rozpracovanosti</a>
                                        <a href="javascript:p31_invoice()" style="display: <%#IIF(BO.BAS.IsNullNum(Eval("schvaleno_pocet"))>0,"block","none")%>;">Fakturovat</a>
                                    </td>

                                </tr>

                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
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
            <asp:ListItem Text="Nezobrazovat" Value="nothing" Selected="true"></asp:ListItem>
        </asp:RadioButtonList>
    </div>

    <asp:Panel ID="panGridHours" runat="server">
        <asp:DropDownList ID="cbxGridQueryHours" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Bez filtrování" Value="0"></asp:ListItem>
            <asp:ListItem Text="Pouze rozpracované úkony" Value="1"></asp:ListItem>
            <asp:ListItem Text="Pouze schválené úkony, které čekají na fakturaci" Value="2"></asp:ListItem>
            <asp:ListItem Text="Pouze vyfakturované úkony" Value="3"></asp:ListItem>
        </asp:DropDownList>
        <table id="tabGridHours">
            <thead>
                <tr>
                    <th></th>
                    <th>Datum</th>
                    <th>Osoba</th>


                    <th>Aktivita</th>
                    <th style="text-align: right;">Hodiny</th>
                    <th style="text-align: right;">Sazba</th>
                    <th style="text-align: right;">Částka</th>
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
                                <%#Eval("Person")%>
                            </td>
                            <td>
                                <%#Eval("p32Name")%>
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p31hours_orig"))%>
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p31Rate_Billing_Orig"))%>
                        
                            </td>
                            <td style="text-align: right;">
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

    <asp:Panel ID="panGridExpenses" runat="server">
        <asp:DropDownList ID="cbxGridQueryExpenses" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Bez filtrování" Value="0"></asp:ListItem>
            <asp:ListItem Text="Pouze rozpracované úkony" Value="1"></asp:ListItem>
            <asp:ListItem Text="Pouze schválené úkony, které čekají na fakturaci" Value="2"></asp:ListItem>
            <asp:ListItem Text="Pouze vyfakturované úkony" Value="3"></asp:ListItem>
        </asp:DropDownList>
        <table id="tabGridExpenses">
            <thead>
                <tr>
                    <th></th>
                    <th>Datum</th>
                    


                    <th>Aktivita</th>
                    <th style="text-align: right;">Bez DPH</th>
                    <th style="text-align: right;">Sazba DPH</th>
                    <th style="text-align: right;">Celkem</th>
                    <th>Text</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rpGridExpenses" runat="server">
                    <ItemTemplate>
                        <tr class="trHover">
                            <td id="tdSys" runat="server" style="width: 10px;">
                                <asp:Image ID="img1" runat="server" />
                            </td>
                            <td>


                                <a href="javascript:p31_edit(<%#Eval("p31id")%>);"><%#SFD(Eval("p31Date"))%></a>
                            </td>
                         
                            <td>
                                <%#Eval("p32Name")%>
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p31Amount_WithoutVat_Orig"))%>
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p31VatRate_Orig"))%>
                        
                            </td>
                            <td style="text-align: right;">
                                <%#SFN(Eval("p31Amount_WithVat_Orig"))%>
                        
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
</asp:Content>
