Public Class p52_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p52_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("guid") = Request.Item("guid")
            If ViewState("guid") = "" Then Master.StopPage("GUID missing")
            With Master
                .neededPermission = BO.x53PermValEnum.GR_P51_Admin
                .HeaderText = "Položka ceníku"
                .HeaderIcon = "Images/settings_32.png"
                .AddToolbarButton("OK", "ok", , "Images/ok.png")
                .DataPID = BO.BAS.IsNullInt(Request.Item("p85id"))

                Me.p34ID.FillData(.Factory.p34ActivityGroupBL.GetList(New BO.myQuery).Where(Function(p) p.p33ID = BO.p33IdENUM.Cas Or p.p33ID = BO.p33IdENUM.Kusovnik))
                Me.j07ID.FillData(.Factory.j07PersonPositionBL.GetList(New BO.myQuery), True)

                If .DataPID = 0 Then
                    .Factory.j03UserBL.InhaleUserParams("p52_record-p34id", "p52_record-subject")
                    Me.opgSubject.SelectedValue = .Factory.j03UserBL.GetUserParam("p52_record-subject", "j02")
                    Me.p34ID.SelectedValue = .Factory.j03UserBL.GetUserParam("p52_record-p34id")
                End If
            End With

            Me.hidIsp52IsPlusAllTimeSheets.Value = "1"
            'If Me.p34ID.Rows > 3 Then
            '    'zjistit, zda nabízet checkbox p52IsPlusAllTimeSheets
            '    Dim mq As New BO.myQueryP32
            '    mq.p33ID = BO.p33IdENUM.Cas
            '    mq.Billable = BO.BooleanQueryMode.TrueQuery
            '    Dim lis As IEnumerable(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq)
            '    If lis.Select(Function(p) p.p34ID).Distinct.Count > 1 Then
            '        Me.hidIsp52IsPlusAllTimeSheets.Value = "1"
            '    End If
            'End If

            RefreshRecord()

            If Request.Item("clone") = "1" Then
                Master.DataPID = 0
            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Handle_ChangeP34()
            Return
        End If

        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(Master.DataPID)
        With cRec
            Me.p34ID.SelectedValue = .p85OtherKey3.ToString
            Handle_ChangeP34()
            If Me.p52IsPlusAllTimeSheets.Visible Then Me.p52IsPlusAllTimeSheets.Checked = .p85FreeBoolean01
            If .p85OtherKey4 <> 0 Then
                Me.chkP32.Checked = True
                Me.p32ID.SelectedValue = .p85OtherKey4.ToString
            End If


            If .p85OtherKey1 <> 0 Then
                Me.j02ID.Value = .p85OtherKey1.ToString
                Me.j02ID.Text = .p85FreeText01
                opgSubject.SelectedValue = "j02"
            End If
            If .p85OtherKey2 <> 0 Then
                Me.j07ID.SelectedValue = .p85OtherKey2.ToString
                opgSubject.SelectedValue = "j07"
            End If
            If .p85OtherKey1 = 0 And .p85OtherKey2 = 0 Then
                Me.opgSubject.SelectedValue = "all"
            End If
            Me.p52Rate.Value = .p85FreeNumber01
            Me.p52Name.Text = .p85FreeText05
        End With

    End Sub

    Private Sub Handle_ChangeP34()
        Dim mq As New BO.myQueryP32
        mq.p34id = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq).Where(Function(p) p.p32ManualFeeFlag = 0)
        Me.p32ID.DataBind()
        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(mq.p34ID)
        If cRec.p33ID = BO.p33IdENUM.Cas Then
            Me.p52IsPlusAllTimeSheets.Visible = True
        Else
            Me.p52IsPlusAllTimeSheets.Visible = False
            Me.p52IsPlusAllTimeSheets.Checked = False
        End If
        If Me.hidIsp52IsPlusAllTimeSheets.Value <> "1" Then Me.p52IsPlusAllTimeSheets.Visible = False
    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Handle_ChangeP34()
    End Sub

    Private Sub p52_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.j02ID.Visible = False : Me.j07ID.Visible = False
        Select Case opgSubject.SelectedValue
            Case "j07"
                lblSubject.Text = "Pozice osoby:"
                Me.j07ID.Visible = True
            Case "j02"
                lblSubject.Text = "Osoba:"
                Me.j02ID.Visible = True
            Case "all"
                Me.lblSubject.Text = ""
        End Select
       
        Me.lblP32ID.Visible = chkP32.Checked
        Me.p32ID.Visible = chkP32.Checked
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            If Me.chkP32.Checked And BO.BAS.IsNullInt(Me.p32ID.SelectedValue) = 0 Then
                Master.Notify("Chybí specifikace aktivity.", 1) : Return
            End If
            If opgSubject.SelectedValue = "j02" And BO.BAS.IsNullInt(Me.j02ID.Value) = 0 Then
                Master.Notify("Chybí specifikace konkrétní osoby.", 1) : Return
            End If
            If opgSubject.SelectedValue = "j07" And BO.BAS.IsNullInt(Me.j07ID.SelectedValue) = 0 Then
                Master.Notify("Chybí specifikace pozice osoby.", 1) : Return
            End If
            ''If BO.BAS.IsNullNum(Me.p52Rate.Value) = 0 Then
            ''    Master.Notify("Hodnota sazby je nula.", 1) : Return
            ''End If
            With Master.Factory.j03UserBL
                .SetUserParam("p52_record-p34id", Me.p34ID.SelectedValue)
                .SetUserParam("p52_record-subject", Me.opgSubject.SelectedValue)

            End With


            With Master.Factory.p85TempBoxBL
                Dim cRec As BO.p85TempBox = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p85TempBox)
                cRec.p85GUID = ViewState("guid")
                cRec.p85OtherKey1 = 0 : cRec.p85OtherKey2 = 0 : cRec.p85FreeText01 = "" : cRec.p85FreeText02 = ""
                Select Case opgSubject.SelectedValue
                    Case "j02"
                        cRec.p85OtherKey1 = BO.BAS.IsNullInt(Me.j02ID.Value)
                        cRec.p85FreeText01 = Master.Factory.j02PersonBL.Load(cRec.p85OtherKey1).FullNameDesc
                    Case "j07"
                        cRec.p85OtherKey2 = BO.BAS.IsNullInt(Me.j07ID.SelectedValue)
                        cRec.p85FreeText02 = Me.j07ID.Text
                    Case "all"
                End Select
                If Me.p52IsPlusAllTimeSheets.Visible Then
                    cRec.p85FreeBoolean01 = Me.p52IsPlusAllTimeSheets.Checked
                End If
                
                cRec.p85OtherKey3 = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
                cRec.p85FreeText03 = Me.p34ID.Text

                If Me.chkP32.Checked Then
                    cRec.p85OtherKey4 = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)
                    cRec.p85FreeText04 = Me.p32ID.Text
                Else
                    cRec.p85OtherKey4 = 0 : cRec.p85FreeText04 = ""
                End If
                cRec.p85FreeText05 = Me.p52Name.Text

                cRec.p85FreeNumber01 = BO.BAS.IsNullNum(Me.p52Rate.Value)
                If .Save(cRec) Then
                    Master.CloseAndRefreshParent("p52-refresh")
                Else
                    Master.Notify(.ErrorMessage, 2)
                End If
            End With

        End If
    End Sub
End Class