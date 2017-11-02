Imports Telerik.Web.UI

Public Class j02_personalplan
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private Property _bolNeedSetupGrid As Boolean = False

    Public Class PlanMatrix
        Public Property Person As String
        
        Public Property Total As String
        Public Property PID As Integer
        Public Property RowIndex As Integer
        Public Property Col1 As Double?
        Public Property Col1P85ID As Integer?
        Public Property Col2 As Double?
        Public Property Col2P85ID As Integer?
        Public Property Col3 As Double?
        Public Property Col3P85ID As Integer?
        Public Property Col4 As Double?
        Public Property Col4P85ID As Integer?
        Public Property Col5 As Double?
        Public Property Col5P85ID As Integer?
        Public Property Col6 As Double?
        Public Property Col6P85ID As Integer?
        Public Property Col7 As Double?
        Public Property Col7P85ID As Integer?
        Public Property Col8 As Double?
        Public Property Col8P85ID As Integer?
        Public Property Col9 As Double?
        Public Property Col9P85ID As Integer?
        Public Property Col10 As Double?
        Public Property Col10P85ID As Integer?
        Public Property Col11 As Double?
        Public Property Col11P85ID As Integer?
        Public Property Col12 As Double?
        Public Property Col12P85ID As Integer?
    End Class

    Private Sub j02_personalplan_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub
    Public ReadOnly Property LimitD1 As Date
        Get
            Return DateSerial(CInt(Me.y1.SelectedValue), CInt(Me.m1.SelectedValue), 1)

        End Get
        
    End Property
    Public ReadOnly Property LimitD2 As Date
        Get
            Return LimitD1.AddMonths(12).AddDays(-1)
        End Get

    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidGUID.Value = BO.BAS.GetGUID
            With Me.y1.Items
                For i As Integer = Year(Now) - 2 To Year(Now) + 1
                    .Add(New ListItem(i.ToString, i.ToString))
                Next
            End With
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))    'j02ID
                .HeaderText = "Osobní plány"
                Master.AddToolbarButton("XLS", "xls", , "Images/xls.png")
                Master.AddToolbarButton("Uložit změny", "save", 0, "Images/save.png")
                Master.AddToolbarButton("Uložit a zavřít", "saveandclose", 0, "Images/save.png")
                Me.cbxJ11ID.DataSource = .Factory.j11TeamBL.GetList(New BO.myQuery)
                Me.cbxJ11ID.DataBind()
                Me.cbxJ11ID.Items.Insert(0, "--Filtr podle týmů--")
                With .Factory.j03UserBL
                    .InhaleUserParams("j02_personalplan-y1", "j02_personalplan-m1", "j02_personalplan-j11id")
                    basUI.SelectDropdownlistValue(Me.y1, .GetUserParam("j02_personalplan-y1", Year(Now).ToString))
                    basUI.SelectDropdownlistValue(Me.m1, .GetUserParam("j02_personalplan-m1", "1"))
                    basUI.SelectDropdownlistValue(Me.cbxJ11ID, .GetUserParam("j02_personalplan-j11id", ""))
                End With
            End With


            RefreshRecord()


        End If
    End Sub

    Private Sub SetupGrid()
        grid1.Columns.Clear()
        grid1.MasterTableView.GroupByExpressions.Clear()
        grid1.MasterTableView.ColumnGroups.Clear()
        
        
        Dim d1 As Date = Me.LimitD1, x As Integer = 1
        AddColumn("Person", "Jméno")
        AddColumn("Total", "<img src='Images/sum.png'/>")

        Dim dats As New List(Of Date)
        For i As Integer = 0 To 11
            dats.Add(d1.AddMonths(i))
        Next
        For Each d In dats
            AddNumbericTextboxColumn("Col" & x.ToString, Format(d, "M-yyyy"), "gridnumber1", True)
            x += 1
        Next
      


    End Sub
    

    Private Sub RefreshRecord()
        Dim mq As New BO.myQueryP66
        mq.D1 = Me.LimitD1
        mq.D2 = Me.LimitD2
        mq.j11ID = BO.BAS.IsNullInt(Me.cbxJ11ID.SelectedValue)
        lblPeriod.Text = Format(mq.D1, "dd.MM.yyyy") & " - " & Format(mq.D2, "dd.MM.yyyy")

        SetupGrid()

        Dim lisP66 As IEnumerable(Of BO.p66PersonalPlan) = Master.Factory.j02PersonBL.GetList_p66(mq)
        Master.Factory.p85TempBoxBL.Truncate(hidGUID.Value)
        For Each c In lisP66
            Dim cTemp As New BO.p85TempBox()
            With cTemp
                .p85GUID = hidGUID.Value
                .p85DataPID = c.PID
                .p85OtherKey1 = c.j02ID
                .p85FreeText01 = c.Person

                .p85FreeDate01 = c.p66DateFrom
                .p85FreeDate02 = c.p66DateUntil

                Select Case Me.cbxField.SelectedValue
                    Case "p66HoursInvoiced" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66HoursInvoiced)
                    Case "p66HoursBillable" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66HoursBillable)
                    Case "p66HoursNonBillable" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66HoursNonBillable)
                    Case "p66HoursTotal" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66HoursTotal)
                    Case "p66FreeNumber01" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66FreeNumber01)
                    Case "p66FreeNumber02" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66FreeNumber02)
                    Case "p66FreeNumber03" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66FreeNumber03)
                    Case "p66FreeNumber04" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66FreeNumber04)
                    Case "p66FreeNumber05" : .p85FreeFloat01 = BO.BAS.IsNullNum(c.p66FreeNumber05)
                End Select



            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
    End Sub
    Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            Dim cRec As PlanMatrix = CType(e.Item.DataItem, PlanMatrix)
            e.Item.Attributes.Item("j02id") = cRec.PID.ToString

        End If

    End Sub


    Private Sub grid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles grid1.NeedDataSource
        grid1.DataSource = GetListMatrix()
    End Sub
    Private Function GetListMatrix() As List(Of PlanMatrix)
        Dim mqJ02 As New BO.myQueryJ02, row As PlanMatrix = Nothing, x As Integer = 0, lis As New List(Of PlanMatrix)
        mqJ02.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
        mqJ02.j11ID = BO.BAS.IsNullInt(Me.cbxJ11ID.SelectedValue)
        Dim lisJ02 As IEnumerable(Of BO.j02Person) = Master.Factory.j02PersonBL.GetList(mqJ02).OrderBy(Function(p) p.j02LastName)
        For Each c In lisJ02
            row = New PlanMatrix()
            row.Person = c.FullNameDesc
            row.RowIndex = x
            row.PID = c.PID
            lis.Add(row)
            x += 1
        Next

        Dim d1 As Date = Me.LimitD1, d2 As Date = Me.LimitD2

        Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID.Value)
        Dim dats As New List(Of Date)
        For i As Integer = 0 To 11
            dats.Add(d1.AddMonths(i))
        Next

        For Each c In lisTemp
            For x = 0 To 11
                If c.p85FreeDate01 = dats(x) Then

                    row = lis.First(Function(p) p.PID = c.p85OtherKey1)
                    If c.p85FreeFloat01 <> 0 Then
                        BO.BAS.SetPropertyValue(row, "Col" & (x + 1).ToString, c.p85FreeFloat01)
                    Else
                        BO.BAS.SetPropertyValue(row, "Col" & (x + 1).ToString, Nothing)
                    End If

                    BO.BAS.SetPropertyValue(row, "Col" & (x + 1).ToString & "P85ID", c.PID)

                End If
            Next
        Next

        Return lis
    End Function


    Public Sub AddColumn(ByVal strField As String, ByVal strHeader As String, Optional strWidth As String = "")
        Dim col As New GridBoundColumn
        grid1.MasterTableView.Columns.Add(col)

        col.HeaderText = strHeader
        col.DataField = strField
        col.ReadOnly = True
        col.AllowSorting = True
        If strWidth <> "" Then col.ItemStyle.Width = Unit.Parse(strWidth)
    End Sub
    Public Sub AddNumbericTextboxColumn(strField As String, strHeader As String, strColumnEditorID As String, bolAllowSorting As Boolean, Optional ByVal strUniqueName As String = "")
        Dim col As New GridNumericColumn
        grid1.MasterTableView.Columns.Add(col)

        col.HeaderText = strHeader
        col.DataField = strField
        col.ColumnEditorID = strColumnEditorID
        If strUniqueName <> "" Then
            col.UniqueName = strUniqueName
            col.SortExpression = strUniqueName
        End If
        col.AllowSorting = bolAllowSorting
    End Sub

    Private Sub cbxField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxField.SelectedIndexChanged
        _bolNeedSetupGrid = True
        
    End Sub

    Private Sub j02_personalplan_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        hidLimitD1.Value = Format(Me.LimitD1, "dd.MM.yyyy")
        If _bolNeedSetupGrid Then
            RefreshRecord()
        End If

        If Page.IsPostBack Then
            grid1.Rebind()
        End If
        If Me.cbxJ11ID.SelectedIndex > 0 Then
            Me.cbxJ11ID.BackColor = basUI.ColorQueryRGB
        Else
            Me.cbxJ11ID.BackColor = Nothing
        End If
    End Sub

    Private Sub m1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles m1.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("j02_personalplan-m1", Me.m1.SelectedValue)
        _bolNeedSetupGrid = True
    End Sub

    Private Sub y1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles y1.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("j02_personalplan-y1", Me.y1.SelectedValue)
        _bolNeedSetupGrid = True
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "saveandclose"
                If SaveChanges() Then
                    Master.CloseAndRefreshParent("p66-save")
                End If
            Case "save"
                SaveChanges()
            Case "xls"

                grid1.ExportToExcel()

        End Select
       
    End Sub

    Private Function SaveChanges() As Boolean
        Dim mq As New BO.myQueryP66
        mq.D1 = Me.LimitD1
        mq.D2 = Me.LimitD2
        mq.j11ID = BO.BAS.IsNullInt(Me.cbxJ11ID.SelectedValue)

        Dim lisSaved As IEnumerable(Of BO.p66PersonalPlan) = Master.Factory.j02PersonBL.GetList_p66(mq)


        Dim lisP66 As New List(Of BO.p66PersonalPlan)
        Dim lisTemp As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(hidGUID.Value, False)
        For Each cTemp In lisTemp
            Dim c As New BO.p66PersonalPlan
            If cTemp.p85DataPID <> 0 Then
                c.SetPID(cTemp.p85DataPID)
                With lisSaved.Where(Function(p) p.PID = cTemp.p85DataPID)(0)
                    c.p66HoursInvoiced = .p66HoursInvoiced
                    c.p66HoursBillable = .p66HoursBillable
                    c.p66HoursNonBillable = .p66HoursNonBillable
                    c.p66HoursTotal = .p66HoursTotal
                    c.p66FreeNumber01 = .p66FreeNumber01
                    c.p66FreeNumber02 = .p66FreeNumber02
                    c.p66FreeNumber03 = .p66FreeNumber03
                    c.p66FreeNumber04 = .p66FreeNumber04
                    c.p66FreeNumber05 = .p66FreeNumber05
                End With
            End If


            c.j02ID = cTemp.p85OtherKey1
            c.p66DateFrom = cTemp.p85FreeDate01
            c.p66DateUntil = c.p66DateFrom.AddMonths(1).AddDays(-1)

            Dim valNum As Double? = Nothing
            If cTemp.p85FreeFloat01 <> 0 Then valNum = cTemp.p85FreeFloat01
            Select Case Me.cbxField.SelectedValue
                Case "p66HoursInvoiced" : c.p66HoursInvoiced = valNum
                Case "p66HoursBillable" : c.p66HoursBillable = valNum
                Case "p66HoursNonBillable" : c.p66HoursNonBillable = valNum
                Case "p66HoursTotal" : c.p66HoursTotal = valNum
                Case "p66FreeNumber01" : c.p66FreeNumber01 = valNum
                Case "p66FreeNumber02" : c.p66FreeNumber02 = valNum
                Case "p66FreeNumber03" : c.p66FreeNumber03 = valNum
                Case "p66FreeNumber04" : c.p66FreeNumber04 = valNum
                Case "p66FreeNumber05" : c.p66FreeNumber05 = valNum
            End Select

            lisP66.Add(c)

        Next

        With Master.Factory.j02PersonBL
            If .SavePersonalPlan(lisP66) Then
                Return True
            Else
                Master.Notify(.ErrorMessage, NotifyLevel.WarningMessage)
                Return False
            End If
        End With


    End Function

    Private Sub cbxJ11ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxJ11ID.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("j02_personalplan-j11id", Me.cbxJ11ID.SelectedValue)
        _bolNeedSetupGrid = True
    End Sub

   
End Class