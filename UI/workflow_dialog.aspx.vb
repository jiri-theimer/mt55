Public Class workflow_dialog
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

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

    Private Sub workflow_dialog_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.upload1.Factory = Master.Factory
        Me.uploadlist1.Factory = Master.Factory
        receiver1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            ViewState("guid") = BO.BAS.GetGUID
            Me.CurrentPrefix = Request.Item("prefix")
            Me.CurrentRecordPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Me.CurrentRecordPID = 0 Or Me.CurrentPrefix = "" Then
                Master.StopPage("pid or prefix is missing")
            End If

            Me.upload1.GUID = BO.BAS.GetGUID
            Me.uploadlist1.GUID = Me.upload1.GUID

            With Master
                .HeaderText = "Posunout/doplnit | " & .Factory.GetRecordCaption(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.CurrentRecordPID)
                .HeaderIcon = "Images/workflow_32.png"
                .AddToolbarButton("Potvrdit", "save", , "Images/save.png")
            End With

            RefreshRecord()

        End If

    End Sub

    Private Sub RefreshRecord()
        Dim lisRadioItems As List(Of BO.WorkflowStepPossible4User) = Master.Factory.b06WorkflowStepBL.GetPossibleWorkflowSteps4Person(Me.CurrentPrefix, Me.CurrentRecordPID, Master.Factory.SysUser.j02ID)
        For Each c In lisRadioItems
            opgB06ID.Items.Add(New ListItem(c.RadioListText, c.b06ID.ToString))
        Next
        opgB06ID.Items.Add(New ListItem("Doplnit pouze komentář nebo nahrát přílohu", ""))
        If Me.CurrentPrefix = "p56" Then
            Dim cP56 As BO.p56Task = Master.Factory.p56TaskBL.Load(Me.CurrentRecordPID)
            If cP56.o43ID <> 0 Then
                'úkol načtený přes IMAP
                Dim cO43 As BO.o43ImapRobotHistory = Master.Factory.o42ImapRuleBL.LoadHistoryByID(cP56.o43ID)
                opgB06ID.Items.Add(New ListItem(String.Format("Napsat odpověď žadateli požadavku [{0}]", cO43.o43FROM), "-1"))
            End If
        End If

        history1.RefreshData(Master.Factory, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.CurrentRecordPID)
        If history1.RowsCount = 0 Then
            panHistory.Visible = False
        End If
    End Sub

    Private Sub upload1_AfterUploadAll() Handles upload1.AfterUploadAll
        Me.uploadlist1.RefreshData_TEMP()
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            upload1.TryUploadhWaitingFilesOnClientSide()
            If panNominee.Visible Then SaveTempX69()
            receiver1.SaveTemp()

            If opgB06ID.SelectedItem Is Nothing Then
                Master.Notify("Musíte zvolit krok!", 2)
                Return
            End If
            If BO.BAS.IsNullInt(opgB06ID.SelectedValue) = 0 Then
                'pouze zapsat komentář
                If SendCommentOnly() Then
                    Master.CloseAndRefreshParent("workflow-dialog")
                End If
                Return
            End If
            If opgB06ID.SelectedValue = "-1" Then
                'odpovědět žadateli požadavku
                If SendTicketMailAnswer() Then
                    Master.CloseAndRefreshParent("workflow-dialog")
                End If
                Return
            End If
            Dim cB06 As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(BO.BAS.IsNullInt(opgB06ID.SelectedValue))
            Dim lisNominee As List(Of BO.x69EntityRole_Assign) = Nothing
            If cB06.b06IsNominee And rpNominee.Items.Count > 0 Then
                lisNominee = GetNomineeList()
            End If


            If Master.Factory.b06WorkflowStepBL.RunWorkflowStep(cB06, Me.CurrentRecordPID, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.b07Value.Text, upload1.GUID, True, lisNominee) Then
                Master.CloseAndRefreshParent("workflow-dialog")
            Else
                Master.Notify(Master.Factory.b06WorkflowStepBL.ErrorMessage, NotifyLevel.ErrorMessage)
            End If
        End If
    End Sub
    Private Function SendTicketMailAnswer() As Boolean
        Dim cRec As New BO.b07Comment
        With cRec
            .x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentPrefix)
            .b07RecordPID = Me.CurrentRecordPID
            .b07Value = Me.b07Value.Text
        End With

        With Master.Factory.b07CommentBL
            If .Save(cRec, upload1.GUID, Nothing) Then
                cRec = Master.Factory.b07CommentBL.Load(.LastSavedPID)

                If Not Master.Factory.x40MailQueueBL.SendAnswer2Ticket(cRec.b07Value, BO.BAS.GetX29FromPrefix(Me.CurrentPrefix), Me.CurrentRecordPID) Then
                    Master.Notify(Master.Factory.x40MailQueueBL.ErrorMessage, NotifyLevel.ErrorMessage)
                    Return False
                Else
                    Return True
                End If
            Else
                Master.Notify(Master.Factory.b07CommentBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Return False
            End If
        End With
    End Function
    Private Function SendCommentOnly() As Boolean
        Dim cRec As New BO.b07Comment
        With cRec
            .x29ID = BO.BAS.GetX29FromPrefix(Me.CurrentPrefix)
            .b07RecordPID = Me.CurrentRecordPID
            .b07Value = Me.b07Value.Text
        End With

        With Master.Factory.b07CommentBL
            If .Save(cRec, upload1.GUID, Me.receiver1.GetList()) Then
                Return True
            Else
                Master.Notify(Master.Factory.b07CommentBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Return False
            End If
        End With
    End Function

    Private Sub rpNominee_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpNominee.ItemCommand
        SaveTempX69()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempX69()
            End If
        End If
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
            If hidb06NomineeFlag.Value = "1" Then
                .Visible = False : e.Item.FindControl("lblNeboTymOsob").Visible = False
            Else
                .Visible = True : e.Item.FindControl("lblNeboTymOsob").Visible = True
                If .Items.Count = 0 Then
                    .DataSource = Master.Factory.j11TeamBL.GetList(New BO.myQuery).Where(Function(p) p.j11IsAllPersons = False)
                    .DataBind()
                    .Items.Insert(0, "")
                End If
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
            If hidb06NomineeFlag.Value = "1" Then
                .Flag = "masters"   'pouze nadřízení
            Else
                .Flag = "all"
            End If
        End With

        If cRec.p85OtherKey2 <> 0 Then
            CType(e.Item.FindControl("img1"), Image).ImageUrl = "Images/projectrole.png"
        End If

       
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
        End With
    End Sub

    Private Sub opgB06ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles opgB06ID.SelectedIndexChanged
        Me.panNotify.Visible = False
        If opgB06ID.SelectedValue = "" Then
            'Zapsat pouze komentář
            Me.panNotify.Visible = True
            If receiver1.RowsCount = 0 And Me.CurrentPrefix = "p56" Then
                Dim cP56 As BO.p56Task = Master.Factory.p56TaskBL.Load(Me.CurrentRecordPID)
                If cP56.j02ID_Owner <> Master.Factory.SysUser.j02ID Then
                    receiver1.AddReceiver(cP56.j02ID_Owner, 0, False)
                End If
            End If
            Return
        End If
        If opgB06ID.SelectedValue = "-1" Then
            'odpovědět žadateli požadavku na mail

            Return
        End If
        Dim cRec As BO.b06WorkflowStep = Master.Factory.b06WorkflowStepBL.Load(opgB06ID.SelectedValue)
        panNominee.Visible = cRec.b06IsNominee
        If cRec.b06IsNominee Then
            Dim cX67 As BO.x67EntityRole = Master.Factory.x67EntityRoleBL.Load(cRec.x67ID_Nominee)
            cmdAddNominee.Text = String.Format("Přidat ({0})", cX67.x67Name)
            hidb06NomineeFlag.Value = CInt(cRec.b06NomineeFlag).ToString
            
        End If
        If cRec.b06IsNomineeRequired Then
            If rpNominee.Items.Count = 0 Then InsertBlankNominee()
        End If
    End Sub

    Private Sub cmdAddNotifyReceiver_Click(sender As Object, e As EventArgs) Handles cmdAddNotifyReceiver.Click
        receiver1.AddReceiver(0, 0, False)
    End Sub
End Class