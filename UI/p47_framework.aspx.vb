Imports Telerik.Web.UI
Public Class p47_framework
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    Private _lisJ02 As List(Of BO.j02Person) = Nothing
    Private Const _MaxPersons = 25


    Private Class PlanRow
        Public Sub New(intP41ID As Integer, strProject As String)
            Me.pid = intP41ID
            Me.Project = strProject
        End Sub
        Public Property pid As Integer
        Public Property Project As String
        Public Property p42Name As String
        Public Property j18Name As String
        Public Property Client As String
        Public Property CelkemMesicFa As Double?
        Public Property CelkemMesicNeFa As Double?
        Public Property CelkemMesic As Double?
        Public Property CelkemMimoMesic As Double?
        Public Property p1 As Double?
        Public Property p2 As Double?
        Public Property p3 As Double?
        Public Property p4 As Double?
        Public Property p5 As Double?
        Public Property p6 As Double?
        Public Property p7 As Double?
        Public Property p8 As Double?
        Public Property p9 As Double?
        Public Property p10 As Double?
        Public Property p11 As Double?
        Public Property p12 As Double?
        Public Property p13 As Double?
        Public Property p14 As Double?
        Public Property p15 As Double?
        Public Property p16 As Double?
        Public Property p17 As Double?
        Public Property p18 As Double?
        Public Property p19 As Double?
        Public Property p20 As Double?
        Public Property p21 As Double?
        Public Property p22 As Double?
        Public Property p23 As Double?
        Public Property p24 As Double?
        Public Property p25 As Double?
    End Class

    Private Sub capacity_plan_one_month_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master

    End Sub
    Public ReadOnly Property CurrentMonth As Integer
        Get
            Return CInt(query_month.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property CurrentYear As Integer
        Get
            Return CInt(query_year.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property CurrentD1 As Date
        Get
            Return DateSerial(Me.CurrentYear, Me.CurrentMonth, 1)
        End Get
    End Property
    Public ReadOnly Property CurrentD2 As Date
        Get
            Return Me.CurrentD1.AddMonths(1).AddDays(-1)
        End Get
    End Property
    Public Property CurrentJ02IDs As List(Of Integer)
        Get
            If Me.hidJ02IDs.Value = "" Or Me.hidJ02IDs.Value = "0" Then
                Me.hidJ02IDs.Value = Master.Factory.SysUser.j02ID.ToString
            End If
            Dim j02ids As New List(Of Integer)
            For Each s As String In Split(Me.hidJ02IDs.Value, ",")
                j02ids.Add(CInt(s))
            Next
            Return j02ids
        End Get
        Set(value As List(Of Integer))
            Me.hidJ02IDs.Value = String.Join(",", value)
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .PageTitle = "Kapacitní plánování"
                .SiteMenuValue = "p47"
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p47_framework-query-year")
                    .Add("p47_framework-query-month")
                    .Add("p47_framework-j02ids")
                    .Add("p47_framework-groupby")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    With query_year
                        If .Items.Count = 0 Then
                            For i As Integer = -2 To 2
                                Dim intY As Integer = Year(Now) + i
                                .Items.Add(New ListItem(intY.ToString, intY.ToString))
                            Next
                        End If
                    End With
                    basUI.SelectDropdownlistValue(Me.query_year, .GetUserParam("p47_framework-query-year", Year(Now).ToString))
                    basUI.SelectDropdownlistValue(Me.query_month, .GetUserParam("p47_framework-query-month", Month(Now).ToString))
                    basUI.SelectDropdownlistValue(Me.cbxGroupBy, .GetUserParam("p47_framework-groupby"))
                    Me.hidJ02IDs.Value = .GetUserParam("p47_framework-j02ids")


                End With

            End With

            Me.j11ID_Add.DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
            Me.j11ID_Add.DataBind()
            Me.j07ID_Add.DataSource = Master.Factory.j07PersonPositionBL.GetList(New BO.myQuery)
            Me.j07ID_Add.DataBind()
            Me.j02ID_Add.Flag = "all"

            RefreshData()
        End If
    End Sub
    Private Sub query_month_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles query_month.SelectedIndexChanged
        Handle_AfterChangeMonth()

    End Sub

    Private Sub query_year_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles query_year.SelectedIndexChanged
        Handle_AfterChangeYear()

    End Sub

    Private Sub cmdNextMonth_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles cmdNextMonth.Click
        If query_month.SelectedValue = "12" Then
            If query_year.SelectedIndex < query_year.Items.Count - 1 Then
                query_year.SelectedIndex += 1
                query_month.SelectedValue = "1"
                Handle_AfterChangeYear()
            Else
                Return
            End If
        Else
            query_month.SelectedIndex += 1
        End If

        Handle_AfterChangeMonth()
    End Sub

    Private Sub cmdPrevMonth_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles cmdPrevMonth.Click
        If query_month.SelectedValue = "1" Then
            If query_year.SelectedIndex > 0 Then
                query_year.SelectedIndex = query_year.SelectedIndex - 1
                query_month.SelectedValue = "12"
                Handle_AfterChangeYear()
            Else
                Return
            End If
        Else
            query_month.SelectedIndex = query_month.SelectedIndex - 1
        End If
        Handle_AfterChangeMonth()
    End Sub

    Private Sub Handle_AfterChangeMonth()
        Master.Factory.j03UserBL.SetUserParam("p47_framework-query-month", query_month.SelectedValue)
        RefreshData()
    End Sub
    Private Sub Handle_AfterChangeYear()
        Master.Factory.j03UserBL.SetUserParam("p47_framework-query-year", query_year.SelectedValue)
        RefreshData()
    End Sub

    Private Sub cmdAppendJ02IDs_Click(sender As Object, e As EventArgs) Handles cmdAppendJ02IDs.Click
        Handle_ChangeJ02IDs(True)
    End Sub

    Private Sub cmdReplaceJ02IDs_Click(sender As Object, e As EventArgs) Handles cmdReplaceJ02IDs.Click
        Handle_ChangeJ02IDs(False)
    End Sub
    Private Sub Handle_ChangeJ02IDs(bolAppend As Boolean)
        Dim intJ11ID As Integer = BO.BAS.IsNullInt(Me.j11ID_Add.SelectedValue)
        Dim intJ07ID As Integer = BO.BAS.IsNullInt(Me.j07ID_Add.SelectedValue)
        Dim intJ02ID As Integer = BO.BAS.IsNullInt(Me.j02ID_Add.Value)
        If intJ02ID = 0 And intJ07ID = 0 And intJ11ID = 0 Then
            Master.Notify("Musíte vybrat osobu, tým nebo pozici.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim j02ids As New List(Of Integer)
        If intJ02ID > 0 Then
            j02ids.Add(intJ02ID)
        End If
        If intJ07ID > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.j07ID = intJ07ID
            For Each x In Master.Factory.j02PersonBL.GetList(mq).Select(Function(p) p.PID).ToList
                j02ids.Add(x)
            Next
        End If
        If intJ11ID <> 0 Then
            For Each x In Master.Factory.j11TeamBL.GetList_BoundJ12(intJ11ID).Select(Function(p) p.j02ID).ToList
                j02ids.Add(x)
            Next
        End If
        If j02ids.Count = 0 Then
            Master.Notify("Vstupní podmínce neodpovídá ani jeden osobní profil.", NotifyLevel.WarningMessage)
            Return
        End If
        If bolAppend Then
            AppendCurrentJ02IDs(j02ids)
        Else
            Me.CurrentJ02IDs = j02ids
        End If
        Me.SaveCurrentPersonsScope()
        _lisJ02 = Nothing
        RefreshData()

    End Sub

    Private Sub AppendCurrentJ02IDs(j02ids As List(Of Integer))
        Dim cj As List(Of Integer) = Me.CurrentJ02IDs
        For Each x In j02ids
            If cj.Where(Function(p) p = x).Count = 0 Then
                cj.Add(x)
            End If
        Next
        Me.CurrentJ02IDs = cj

    End Sub
    Private Sub SaveCurrentPersonsScope()

        Master.Factory.j03UserBL.SetUserParam("p47_framework-j02ids", Me.hidJ02IDs.Value)
    End Sub

    Private Sub InhaleLisJ02()
        If Not _lisJ02 Is Nothing Then Return
        Dim mq As New BO.myQueryJ02, x As Integer = 0
        mq.PIDs = GetJ02IDs()
        _lisJ02 = Master.Factory.j02PersonBL.GetList(mq).ToList
        If _lisJ02.Count > _MaxPersons Then
            Master.Notify("Maximální počet najednou zobrazitelných osob je " & _MaxPersons.ToString & "!", NotifyLevel.InfoMessage)
            For i As Integer = _MaxPersons + 1 To _lisJ02.Count
                _lisJ02.RemoveAt(_MaxPersons)
            Next
        End If
    End Sub
    Private Sub SetupGrid()
        InhaleLisJ02()
        Dim x As Integer = 0
        With grid1
            .ClearColumns()
            If Me.cbxGroupBy.SelectedValue <> "Client" Then
                .AddColumn("Client", "Klient")
            End If

            .AddColumn("Project", "Projekt")
            .AddColumn("CelkemMesicFa", "Fa", BO.cfENUM.Numeric0)
            .AddColumn("CelkemMesicNeFa", "NeFa", BO.cfENUM.Numeric0)
            .AddColumn("CelkemMesic", "Celkem", BO.cfENUM.Numeric0)
            For Each c In _lisJ02
                x += 1
                Dim s As String = c.j02Code
                If s = "" Then s = c.j02LastName & " " & Left(c.j02FirstName, 1) & "."
                .AddColumn("p" & x.ToString, s, BO.cfENUM.Numeric0, , , , , True)
            Next

        End With
        With Me.cbxGroupBy.SelectedItem
            SetupGrouping(.Value, .Text)
        End With

    End Sub
    Private Sub SetupGrouping(strGroupField As String, strFieldHeader As String)
        With grid1.radGridOrig.MasterTableView
            .GroupByExpressions.Clear()
            If strGroupField = "" Then Return
            .ShowGroupFooter = True
            Dim GGE As New GridGroupByExpression
            Dim fld As New GridGroupByField
            fld.FieldName = strGroupField
            fld.HeaderText = strFieldHeader

            GGE.SelectFields.Add(fld)
            GGE.GroupByFields.Add(fld)

            .GroupByExpressions.Add(GGE)
        End With
    End Sub
    Private Sub RefreshData()
        SetupGrid()
        grid1.Rebind(False)
    End Sub
    Private Function GetJ02IDs() As List(Of Integer)
        If Me.hidJ02IDs.Value = "" Then Me.hidJ02IDs.Value = Master.Factory.SysUser.j02ID.ToString
        Return BO.BAS.ConvertPIDs2List(Me.hidJ02IDs.Value)
    End Function

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        InhaleLisJ02()
        Dim mq As New BO.myQueryP47
        mq.DateFrom = Me.CurrentD1
        mq.DateUntil = Me.CurrentD2

        Dim lisP47 As IEnumerable(Of BO.p47CapacityPlan) = Master.Factory.p47CapacityPlanBL.GetList(mq).Where(Function(p) p.p47HoursTotal > 0).OrderBy(Function(p) p.p41ID)
        Dim lisPlanRow As New List(Of PlanRow), x As Integer = 0, intLastP41ID As Integer = 0, p41ids As New List(Of Integer)
        Dim cRow As PlanRow = Nothing
        For Each cP47 In lisP47
            If intLastP41ID <> cP47.p41ID Then
                cRow = New PlanRow(cP47.p41ID, cP47.Project)
                lisPlanRow.Add(cRow)
                p41ids.Add(cP47.p41ID)
            End If
            For x = 0 To _lisJ02.Count - 1
                If _lisJ02(x).PID = cP47.j02ID Then
                    BO.BAS.SetPropertyValue(cRow, "p" & (x + 1).ToString, cP47.p47HoursTotal) : Exit For
                End If
            Next
            intLastP41ID = cP47.p41ID
        Next
        Dim mqP41 As New BO.myQueryP41
        mqP41.PIDs = p41ids
        Dim lisP41 As IEnumerable(Of BO.p41Project) = Master.Factory.p41ProjectBL.GetList(mqP41)
        For Each c In lisPlanRow
            With lisP41.First(Function(p) p.PID = c.pid)
                c.CelkemMesicFa = lisP47.Where(Function(p) p.p41ID = c.pid).Sum(Function(p) p.p47HoursBillable)
                c.CelkemMesicNeFa = lisP47.Where(Function(p) p.p41ID = c.pid).Sum(Function(p) p.p47HoursNonBillable)
                c.CelkemMesic = c.CelkemMesicFa + c.CelkemMesicNeFa
                c.p42Name = .p42Name
                c.Client = .Client
                c.j18Name = .j18Name
                If .p41NameShort = "" Then
                    c.Project = .p41Name
                Else
                    c.Project = .p41NameShort
                End If
            End With
        Next

        grid1.DataSource = lisPlanRow
    End Sub

    Private Sub cbxGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxGroupBy.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p47_framework-groupby", Me.cbxGroupBy.SelectedValue)
        RefreshData()
    End Sub

    Private Sub cmdHardRefreshOnBehind_Click(sender As Object, e As EventArgs) Handles cmdHardRefreshOnBehind.Click
        grid1.Rebind(True)
    End Sub

   
  
    Private Sub cmdExport_Click(sender As Object, e As EventArgs) Handles cmdExport.Click
        grid1.radGridOrig.MasterTableView.ExportToExcel()
    End Sub

    Private Sub p47_framework_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With lblHeader
            .Text = BO.BAS.OM2(.Text, Me.CurrentMonth.ToString & "/" & Me.CurrentYear.ToString)
        End With
    End Sub
End Class