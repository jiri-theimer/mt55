Public Class entity_timeline
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Public Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
        Set(value As String)
            Me.hidPrefix.Value = value
        End Set
    End Property
    Private Sub entity_timeline_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                Me.CurrentPrefix = Request.Item("prefix")
                If .DataPID = 0 Or Me.CurrentPrefix = "" Then .StopPage("prefix or pid missing.")
                .AddToolbarButton("CHANGE-LOG", "changelog", 10, "Images/log.png", False, "changelog.aspx?prefix=" & Me.CurrentPrefix & "&pid=" & .DataPID.ToString)
                If Me.CurrentPrefix = "j02" Then
                    'najít j03id osoby
                    Dim mq As New BO.myQueryJ03
                    mq.j02ID = .DataPID
                    Dim lisJ03 As IEnumerable(Of BO.j03User) = .Factory.j03UserBL.GetList(mq)
                    If lisJ03.Count = 0 Then
                        .StopPage("Osoba nemá zavedený uživatelský účet.")
                    Else
                        .DataPID = lisJ03(0).PID
                        Me.CurrentPrefix = "j03"
                    End If
                    cmdAccessLog.Visible = True
                    cmdAccessLog.NavigateUrl = "j03_accesslog.aspx?pid=" & .DataPID.ToString
                End If
                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("entity_timeline-period")
                    .Add("periodcombo-custom_query")
                    .Add("entity_timeline-pagesize")
                End With
                .Factory.j03UserBL.InhaleUserParams(lisPars)
                period1.SetupData(.Factory, .Factory.j03UserBL.GetUserParam("periodcombo-custom_query"))
                period1.SelectedValue = .Factory.j03UserBL.GetUserParam("entity_timeline-period")

                basUI.SelectDropdownlistValue(Me.cbxPaging, .Factory.j03UserBL.GetUserParam("entity_timeline-pagesize", "20"))
            End With
            SetupGrid()
            SetupByPrefix()
        End If
    End Sub

    Public Sub SetupGrid()
        With grid1
            .ClearColumns()
            .AllowMultiSelect = False
            .radGridOrig.ShowFooter = False
            .PageSize = CInt(Me.cbxPaging.SelectedValue)
            .AddColumn("DateInsert", "Čas", BO.cfENUM.DateTime)
            .AddColumn("Person", "Kdo")
            .AddColumn("x45Name", "Událost")

            .AddColumn("x47Name", "Název")
            .AddColumn("x47NameReference", "Referenční entita")
        End With
    End Sub
    Public Sub SetupByPrefix()
        Select Case Me.CurrentPrefix
            Case "p41"
                imgEntity.ImageUrl = "Images/project_32.png"
                Master.HeaderText = "Historie projektu"
            Case "p28"
                imgEntity.ImageUrl = "Images/contact_32.png"
                Master.HeaderText = "Historie klienta"
            Case "j03"
                imgEntity.ImageUrl = "Images/person_32.png"
                Master.HeaderText = "Historie aktivit osoby"
        End Select
    End Sub


    Private Sub InhaleMyQuery(ByRef mq As BO.myQueryX47)
        If Me.CurrentPrefix = "j03" Then
            mq.j03ID = Master.DataPID
        Else
            mq.RecordPID = Master.DataPID
            mq.x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentPrefix)
        End If
        
        mq.DateFrom = period1.DateFrom
        mq.DateUntil = period1.DateUntil

    End Sub

    Private Sub grid1_NeedDataSource(sender As Object, e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        Dim mq As New BO.myQueryX47
        InhaleMyQuery(mq)
        With mq
            .TopRecordsOnly = 500
            .MG_PageSize = CInt(Me.cbxPaging.SelectedValue)
            .MG_CurrentPageIndex = grid1.radGridOrig.MasterTableView.CurrentPageIndex

        End With

        grid1.DataSource = Master.Factory.x47EventLogBL.GetList(mq)
    End Sub

    Private Sub cbxPaging_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxPaging.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("entity_timeline-pagesize", Me.cbxPaging.SelectedValue)
        SetupGrid()
        'RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub period1_OnChanged(DateFrom As Date, DateUntil As Date) Handles period1.OnChanged
        Master.Factory.j03UserBL.SetUserParam("entity_timeline-period", Me.period1.SelectedValue)
        'RecalcVirtualRowCount()
        grid1.Rebind(False)
    End Sub

    Private Sub entity_timeline_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        With Me.period1
            .Visible = True
            If .SelectedValue <> "" Then
                .BackColor = Drawing.Color.Red
            Else
                .BackColor = Nothing
            End If
        End With
    End Sub
End Class