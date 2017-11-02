Public Class mobile_workflow_dialog
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Mobile
    Public Property CurrentPrefix As String
        Get
            Return hidPrefix.Value
        End Get
        Set(value As String)
            hidPrefix.Value = value
        End Set
    End Property
    Public Property CurrentRecordPID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.hidRecordPID.Value)
        End Get
        Set(value As Integer)
            Me.hidRecordPID.Value = value.ToString
        End Set
    End Property

    Private Sub mobile_workflow_dialog_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        receiver1.Factory = Master.Factory
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID()
            Me.CurrentPrefix = Request.Item("prefix")
            Me.CurrentRecordPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Me.CurrentRecordPID = 0 Or Me.CurrentPrefix = "" Then
                Master.StopPage("pid or prefix is missing")
            End If
            If Not Request.UrlReferrer Is Nothing Then linkBack.NavigateUrl = Request.UrlReferrer.PathAndQuery Else linkBack.Visible = False

            RefreshRecord()

            history1.RefreshData(Master.Factory, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.CurrentRecordPID)
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.RecordHeader.Text = BO.BAS.OM3(Master.Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.CurrentRecordPID), 30)
        Me.RecordHeader.NavigateUrl = "mobile_" & Me.CurrentPrefix & "_framework.aspx?pid=" & Me.CurrentRecordPID.ToString

        Dim lisRadioItems As List(Of BO.WorkflowStepPossible4User) = Master.Factory.b06WorkflowStepBL.GetPossibleWorkflowSteps4Person(Me.CurrentPrefix, Me.CurrentRecordPID, Master.Factory.SysUser.j02ID)
        For Each c In lisRadioItems
            opgB06ID.Items.Add(New ListItem(c.RadioListText, c.b06ID.ToString))
        Next
        opgB06ID.Items.Add(New ListItem("Zapsat pouze komentář", ""))

    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        receiver1.SaveTemp()

        If opgB06ID.SelectedItem Is Nothing Then
            Master.Notify("Musíte zvolit krok!", 2)
            Return
        End If
        If BO.BAS.IsNullInt(opgB06ID.SelectedValue) = 0 Then
            'pouze zapsat komentář
            If SendCommentOnly() Then
                Redirect2Source()
            End If
            Return
        End If
        Dim cB06 As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(BO.BAS.IsNullInt(opgB06ID.SelectedValue))
        Dim lisNominee As List(Of BO.x69EntityRole_Assign) = Nothing
        If cB06.b06IsNominee And rpNominee.Items.Count > 0 Then
            lisNominee = GetNomineeList()
        End If


        If Master.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, Me.CurrentRecordPID, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.b07Value.Text, "", True, lisNominee) Then
            Redirect2Source()
        Else
            Master.Notify(Master.Factory.b06WorkflowStepBL.ErrorMessage, NotifyLevel.ErrorMessage)
        End If
    End Sub

    Private Sub Redirect2Source()
        Response.Redirect(linkBack.NavigateUrl)
    End Sub

    Private Function SendCommentOnly() As Boolean
        Dim cRec As New BO.b07Comment
        With cRec
            .x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentPrefix)
            .b07RecordPID = Me.CurrentRecordPID
            .b07Value = Me.b07Value.Text
        End With

        With Master.Factory.b07CommentBL
            If .Save(cRec, "", Me.receiver1.GetList()) Then
                Return True
            Else
                Master.Notify(Master.Factory.b07CommentBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Return False
            End If
        End With
    End Function

    Private Sub cmdAddNominee_Click(sender As Object, e As EventArgs) Handles cmdAddNominee.Click
        InsertBlankNominee()
    End Sub
    Private Sub InsertBlankNominee()
        SaveTempX69()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid")
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempX69()
    End Sub
    Private Sub SaveTempX69()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        For Each ri As RepeaterItem In rpNominee.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)

            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("j02id"), UI.person).Value)
                .p85FreeText02 = CType(ri.FindControl("j02id"), UI.person).Text
                .p85OtherKey3 = BO.BAS.IsNullInt(CType(ri.FindControl("j11id"), DropDownList).SelectedValue)

            End With
            Master.Factory.p85TempBoxBL.Save(cRec)

        Next
    End Sub
    Private Sub RefreshTempX69()
        rpNominee.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        rpNominee.DataBind()
    End Sub

    Public Function GetNomineeList() As List(Of BO.x69EntityRole_Assign)
        Dim lisX69 As New List(Of BO.x69EntityRole_Assign)
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"), False)
        For Each cTMP In lisTEMP
            Dim c As New BO.x69EntityRole_Assign
            With cTMP
                If .p85OtherKey2 <> 0 Then c.j02ID = .p85OtherKey2
                If .p85OtherKey3 <> 0 Then c.j11ID = .p85OtherKey3
            End With
            lisX69.Add(c)
        Next
        Return lisX69
    End Function

    Private Sub rpNominee_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpNominee.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With

        With CType(e.Item.FindControl("j11id"), DropDownList)
            If .Items.Count = 0 Then
                .DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
                .DataBind()
                .Items.Insert(0, "")
            End If
        End With
        basUI.SelectDropdownlistValue(CType(e.Item.FindControl("j11id"), DropDownList), cRec.p85OtherKey3.ToString)
        If cRec.p85OtherKey3 <> 0 Then
            CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole_team.png"
        End If

        With CType(e.Item.FindControl("j02id"), UI.person)
            .RadCombo.Font.Bold = True
            .Visible = True
            .Value = cRec.p85OtherKey2.ToString
            .Text = cRec.p85FreeText02
        End With

        If cRec.p85OtherKey2 <> 0 Then
            CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole.png"
        End If


        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
        End With
    End Sub
    Private Sub opgB06ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgB06ID.SelectedIndexChanged
        If opgB06ID.SelectedValue = "" Then
            Me.panNotify.Visible = True
            Return
        Else
            Me.panNotify.Visible = False
        End If

        Dim cRec As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(opgB06ID.SelectedValue)
        panNominee.Visible = cRec.b06IsNominee
        If cRec.b06IsNominee Then
            Dim cX67 As BO.x67EntityRole = Master.Factory.x67EntityRoleBL.Load(cRec.x67ID_Nominee)
            cmdAddNominee.Text = "Obsadit roli (" & cX67.x67Name & ")"
        End If
        If cRec.b06IsNomineeRequired Then
            If rpNominee.Items.Count = 0 Then InsertBlankNominee()
        End If
    End Sub

    Private Sub cmdAddNotifyReceiver_Click(sender As Object, e As EventArgs) Handles cmdAddNotifyReceiver.Click
        receiver1.AddReceiver(0, 0, True)
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Redirect2Source()
    End Sub
End Class