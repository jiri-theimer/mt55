Public Class entityrole_assign
    Inherits System.Web.UI.UserControl
    Private Property _Factory As BL.Factory
    Private Property _Error As String
    Private _lisX67 As IEnumerable(Of BO.x67EntityRole) = Nothing
    Private _lisJ11 As IEnumerable(Of BO.j11Team) = Nothing

    Public Property Factory As BL.Factory
        Get
            Return _Factory
        End Get
        Set(value As BL.Factory)
            _Factory = value
        End Set
    End Property
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Property EmptyDataMessage As String
        Get
            Return Me.lblMessageNoData.Text
        End Get
        Set(value As String)
            Me.lblMessageNoData.Text = value
        End Set
    End Property
    Public Property EntityX29ID As BO.x29IdEnum
        Get
            If Me.hidX29ID.Value <> "" Then
                Return CInt(Me.hidX29ID.Value)
            Else
                Return BO.x29IdEnum._NotSpecified
            End If
        End Get
        Set(value As BO.x29IdEnum)
            Me.hidX29ID.Value = CInt(value).ToString
        End Set
    End Property
    Public ReadOnly Property GUID As String
        Get
            If Me.hidGUID.Value = "" Then
                Me.hidGUID.Value = BO.BAS.GetGUID
            End If
            Return Me.hidGUID.Value
        End Get
    
    End Property
    Public ReadOnly Property RowsCount As Integer
        Get
            Return rpX69.Items.Count
        End Get
    End Property
    Public Sub RefreshGUID()
        Me.hidGUID.Value = BO.BAS.GetGUID
    End Sub

    

    Public Sub AddNewRow(Optional intJ02ID As Integer = 0)
        SaveTempX69()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = Me.GUID
        cRec.p85FreeText01 = "j02"
        If intJ02ID > 0 Then
            cRec.p85OtherKey2 = intJ02ID
            cRec.p85FreeText02 = Me.Factory.j02PersonBL.Load(intJ02ID).FullNameDesc
        End If
        _Factory.p85TempBoxBL.Save(cRec)
        RefreshTempX69()
    End Sub
    Public Sub InhaleInitialData(intDataRecordPID As Integer)
        Dim lisX69 As IEnumerable(Of BO.x69EntityRole_Assign) = _Factory.x67EntityRoleBL.GetList_x69(Me.EntityX29ID, intDataRecordPID)
        If lisX69.Count > 0 Then
            _Factory.p85TempBoxBL.Truncate(Me.GUID)
            For Each c In lisX69
                Dim cTemp As New BO.p85TempBox
                With cTemp
                    .p85GUID = Me.GUID
                    .p85DataPID = c.x69ID
                    If c.j02ID <> 0 Then
                        .p85FreeText01 = "j02"
                    End If
                    If c.j11ID <> 0 And Not c.IsAllPersons Then
                        .p85FreeText01 = "j11"
                    End If
                    If (c.j11ID = 0 And c.j02ID = 0) Or c.IsAllPersons Then
                        .p85FreeText01 = "all"
                    End If
                    .p85OtherKey1 = c.x67ID
                    .p85OtherKey2 = c.j02ID
                    .p85FreeText02 = c.Person
                    .p85OtherKey3 = c.j11ID
                End With
                _Factory.p85TempBoxBL.Save(cTemp)
            Next
            RefreshTempX69()
        Else
            rpX69.DataSource = Nothing
            rpX69.DataBind()
            lblMessageNoData.Visible = True
        End If

    End Sub
    Private Sub RefreshTempX69()
        rpX69.DataSource = _Factory.p85TempBoxBL.GetList(Me.GUID)
        rpX69.DataBind()
        If rpX69.Items.Count = 0 Then
            lblMessageNoData.Visible = True
        Else
            lblMessageNoData.Visible = False
        End If
    End Sub

    Public Sub SaveCurrentTempData()
        SaveTempX69()
    End Sub
    Private Sub SaveTempX69()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = _Factory.p85TempBoxBL.GetList(Me.GUID)
        For Each ri As RepeaterItem In rpX69.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)

            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("x67id"), DropDownList).SelectedValue)
                .p85FreeText01 = CType(ri.FindControl("opgWho"), RadioButtonList).SelectedValue
                .p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("j02id"), UI.person).Value)
                .p85FreeText02 = CType(ri.FindControl("j02id"), UI.person).Text
                .p85OtherKey3 = BO.BAS.IsNullInt(CType(ri.FindControl("j11id"), DropDownList).SelectedValue)

            End With
            _Factory.p85TempBoxBL.Save(cRec)

        Next
    End Sub

    Private Sub rpX69_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpX69.ItemCommand
        SaveTempX69()
        Dim cRec As BO.p85TempBox = _Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If _Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempX69()
            End If
        End If

    End Sub



    Private Sub rpX69_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpX69.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With

        e.Item.FindControl("j11id").Visible = False
        e.Item.FindControl("j02id").Visible = False

        If cRec.p85FreeText01 = "" Then cRec.p85FreeText01 = "j02"
        CType(e.Item.FindControl("opgWho"), RadioButtonList).SelectedValue = cRec.p85FreeText01
        Select Case cRec.p85FreeText01
            Case "j11"
                CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole_team.png"
                With CType(e.Item.FindControl("j11id"), DropDownList)
                    .Visible = True
                    If .Items.Count = 0 Then
                        If _lisJ11 Is Nothing Then
                            _lisJ11 = _Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
                        End If
                    End If
                    .DataSource = _lisJ11
                    .DataBind()
                End With
                basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j11id"), DropDownList), cRec.p85OtherKey3.ToString)

            Case "j02"
                CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole.png"
                With CType(e.Item.FindControl("j02id"), UI.person)
                    .RadCombo.Font.Bold = True
                    .Visible = True
                    .Value = cRec.p85OtherKey2.ToString
                    .Text = cRec.p85FreeText02
                End With
            Case "all"
                CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/infinity.png"
        End Select

        With CType(e.Item.FindControl("x67id"), DropDownList)
            If .Items.Count = 0 Then
                If _lisX67 Is Nothing Then
                    _lisX67 = _Factory.x67EntityRoleBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = Me.EntityX29ID)
                End If
                .DataSource = _lisX67
                .DataBind()
                .Items.Insert(0, "")
            End If
            If .Items.Count > 1 Then
                If .SelectedIndex = 0 Then .SelectedIndex = 1
            End If
        End With


        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("x67id"), DropDownList), .p85OtherKey1.ToString)
            CType(e.Item.FindControl("opgWho"), RadioButtonList).Attributes.Item("p85id") = .PID.ToString
        End With
    End Sub

    Public Function GetData4Save() As List(Of BO.x69EntityRole_Assign)
        Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = _Factory.p85TempBoxBL.GetList(Me.GUID)
        Dim lisJ11_ALL As IEnumerable(Of BO.j11Team) = Nothing
        For Each cTMP In lisTEMP
            Dim c As New BO.x69EntityRole_Assign
            With cTMP
                c.x69ID = .p85DataPID
                c.IsSetAsDeleted = .p85IsDeleted
                c.x67ID = .p85OtherKey1
                Select Case .p85FreeText01
                    Case "j02"
                        c.j02ID = .p85OtherKey2
                    Case "j11"
                        c.j11ID = .p85OtherKey3
                    Case "all"
                        If lisJ11_ALL Is Nothing Then lisJ11_ALL = _Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = True)
                        If lisJ11_ALL.Count = 0 Then
                            _Error = "Z neznámých důvodů nelze přidělit roli paušálně všem osobám."
                            Return Nothing
                        Else
                            c.j11ID = lisJ11_ALL(0).PID
                        End If
                End Select


            End With
            lisX69.Add(c)
        Next
        Return lisX69
    End Function

    Protected Sub who_changed(ByVal sender As RadioButtonList, ByVal args As EventArgs)
        SaveTempX69()
        RefreshTempX69()
        sender.Focus()

    End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        
    End Sub
End Class