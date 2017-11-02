Public Class p48_multiple_create
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p48_multiple_create_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p48_framework"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.TestNeededPermission(BO.x53PermValEnum.GR_P48_Creator)
            ViewState("guid") = BO.BAS.GetGUID()
            Dim intYear As Integer = BO.BAS.IsNullInt(Request.Item("year"))
            Dim intMonth As Integer = BO.BAS.IsNullInt(Request.Item("month"))
            Dim intProjectMask As Integer = Master.Factory.SysUser.j03ProjectMaskIndex

            If Request.Item("input") <> "" Then
                Dim s As String = Request.Item("input")
                Dim a() As String = Split(s, ","), strLastProject As String = "", intLastP41ID As Integer
                ''If Request.Item("p41id") <> "" Then intLastP41ID = BO.BAS.IsNullInt(Request.Item("p41id"))
                For i As Integer = 0 To UBound(a)
                    Dim b() As String = Split(a(i), "-")
                    Dim c As New BO.p85TempBox
                    With c
                        .p85GUID = ViewState("guid")
                        .p85FreeDate01 = DateSerial(intYear, intMonth, CInt(b(0)))
                        .p85OtherKey1 = CInt(b(1))  'j02id
                        .p85OtherKey2 = BO.BAS.IsNullInt(b(2))  'p41id
                        If .p85OtherKey2 = 0 And Request.Item("p41id") <> "" Then
                            .p85OtherKey2 = BO.BAS.IsNullInt(Request.Item("p41id"))
                        End If
                        If .p85OtherKey1 <> 0 Then
                            .p85FreeText01 = Master.Factory.j02PersonBL.Load(.p85OtherKey1).FullNameDesc
                        End If

                        If intLastP41ID <> .p85OtherKey2 And .p85OtherKey2 > 0 Then
                            strLastProject = Master.Factory.p41ProjectBL.Load(.p85OtherKey2).ProjectWithMask(intProjectMask)
                        End If
                        If .p85OtherKey2 <> 0 Then
                            .p85FreeText02 = strLastProject
                        End If
                        .p85FreeFloat01 = 8
                    End With
                    Master.Factory.p85TempBoxBL.Save(c)
                    strLastProject = c.p85FreeText02 : intLastP41ID = c.p85OtherKey2
                Next
            End If
            If Request.Item("t1") <> "" And Request.Item("t2") <> "" Then
                Dim dt1 As New BO.DateTimeByQuerystring(Request.Item("t1")), dt2 As New BO.DateTimeByQuerystring(Request.Item("t2")), intJ02ID As Integer = BO.BAS.IsNullInt(Request.Item("j02id"))
                If intJ02ID = 0 Then intJ02ID = Master.Factory.SysUser.j02ID
                Dim d As Date = dt1.DateOnly, intLastP41ID As Integer = BO.BAS.IsNullInt(Request.Item("p41id"))
                Dim strLastProject As String = ""
                If intLastP41ID <> 0 Then strLastProject = Master.Factory.p41ProjectBL.Load(intLastP41ID).ProjectWithMask(intProjectMask)
                While d <= dt2.DateOnly
                    Dim c As New BO.p85TempBox
                    With c
                        .p85GUID = ViewState("guid")
                        .p85FreeDate01 = d
                        .p85OtherKey1 = intJ02ID
                        If .p85OtherKey1 <> 0 Then
                            .p85FreeText01 = Master.Factory.j02PersonBL.Load(.p85OtherKey1).FullNameDesc
                        End If

                        .p85OtherKey2 = intLastP41ID
                        .p85FreeText02 = strLastProject
                        If dt1.TimeOnly <> dt2.TimeOnly Then
                            Dim cT As New BO.clsTime
                            .p85FreeFloat01 = cT.ShowAsDec(dt2.TimeOnly) - cT.ShowAsDec(dt1.TimeOnly)
                        Else
                            .p85FreeFloat01 = 8
                        End If

                        .p85FreeText03 = dt1.TimeOnly
                        .p85FreeText04 = dt2.TimeOnly
                    End With
                    Master.Factory.p85TempBoxBL.Save(c)
                    d = d.AddDays(1)
                End While
            End If



            With Master
                .HeaderText = "Zapsat operativní plán do vybraných dnů"
                .AddToolbarButton("Uložit do plánu", "save", , "Images/save.png")

                With .Factory.j03UserBL
                    Dim lisPars As New List(Of String)
                    With lisPars
                        .Add("p48_multiple_create-p34id")
                        .Add("p48_multiple_create-p32id")
                    End With
                    .InhaleUserParams(lisPars)


                End With

            End With


            RefreshRecord()
            RefreshTempList()
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.p34ID.DataSource = Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery).Where(Function(p) p.p33ID = BO.p33IdENUM.Cas)
        Me.p34ID.DataBind()
        Dim intDefP34ID As Integer = BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam("p48_multiple_create-p34id"))
        If intDefP34ID > 0 Then
            Me.p34ID.SelectedValue = intDefP34ID.ToString
            Handle_ChangeP34ID(intDefP34ID)
        End If
        Dim intDefP32ID As Integer = BO.BAS.IsNullInt(Master.Factory.j03UserBL.GetUserParam("p48_multiple_create-p32id"))
        If intDefP32ID > 0 Then
            Me.p32ID.SelectedValue = intDefP32ID.ToString
        End If
    End Sub

    Private Sub RefreshTempList()
        Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
        If lis.Where(Function(p) p.p85OtherKey2 = 0).Count = 0 Then
            'projekt je všude doplněn
            Me.p41ID.Visible = False
            lblP41ID.Visible = False
        Else
            If lis.Where(Function(p) p.p85OtherKey2 > 0).Count > 0 Then
                lblP41ID.Text = "Kde není projekt, doplnit:"
            End If
        End If
        rp1.DataSource = lis.OrderBy(Function(p) p.p85FreeText01).ThenBy(Function(p) p.p85FreeDate01)
        rp1.DataBind()

    End Sub

    Private Sub rp1_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rp1.ItemCommand
        Save2Temp()
        Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(e.Item.FindControl("p85id"), HiddenField).Value)
        Dim c As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)
        Master.Factory.p85TempBoxBL.Delete(c)

        RefreshTempList()
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)
        CType(e.Item.FindControl("p85id"), HiddenField).Value = cRec.PID.ToString
        CType(e.Item.FindControl("p41id"), HiddenField).Value = cRec.p85OtherKey2.ToString
        CType(e.Item.FindControl("p48Date"), Label).Text = BO.BAS.FD(cRec.p85FreeDate01)
        With CType(e.Item.FindControl("j02ID_Input"), UI.person)
            .Text = cRec.p85FreeText01
            .Value = cRec.p85OtherKey1.ToString
        End With
        ''CType(e.Item.FindControl("Person"), Label).Text = cRec.p85FreeText01
        ''CType(e.Item.FindControl("Project"), Label).Text = cRec.p85FreeText02
        With CType(e.Item.FindControl("p41ID_Input"), UI.project)
            .Text = cRec.p85FreeText02
            .Value = cRec.p85OtherKey2.ToString
        End With
        CType(e.Item.FindControl("p48Hours"), TextBox).Text = cRec.p85FreeFloat01.ToString
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Text = cRec.p85FreeText03
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Text = cRec.p85FreeText04

        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("cas_od") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID
        CType(e.Item.FindControl("p48TimeFrom"), TextBox).Attributes.Item("hodiny") = e.Item.FindControl("p48Hours").ClientID

        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("cas_od") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID
        CType(e.Item.FindControl("p48TimeUntil"), TextBox).Attributes.Item("hodiny") = e.Item.FindControl("p48Hours").ClientID

        CType(e.Item.FindControl("p48Hours"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeFrom").ClientID
        CType(e.Item.FindControl("p48Hours"), TextBox).Attributes.Item("cas_do") = e.Item.FindControl("p48TimeUntil").ClientID
        CType(e.Item.FindControl("p48Text"), TextBox).Text = cRec.p85Message

    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged
        Dim intP34ID As Integer = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        If intP34ID = 0 Then
            Me.p32ID.Clear()
        Else

            Handle_ChangeP34ID(intP34ID)
        End If
    End Sub
    Private Sub Handle_ChangeP34ID(intP34ID As Integer)
        Dim mq As New BO.myQueryP32
        mq.p34ID = intP34ID
        Me.p32ID.DataSource = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataBind()
    End Sub

    Private Sub Save2Temp()
        Dim cT As New BO.clsTime
        For Each ri As RepeaterItem In rp1.Items
            Dim intP85ID As Integer = CInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim c As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(intP85ID)
            Dim strHours As String = CType(ri.FindControl("p48Hours"), TextBox).Text
            c.p85FreeFloat01 = cT.ShowAsDec(strHours)
            c.p85FreeText03 = CType(ri.FindControl("p48TimeFrom"), TextBox).Text
            c.p85FreeText04 = CType(ri.FindControl("p48TimeUntil"), TextBox).Text
            c.p85Message = CType(ri.FindControl("p48Text"), TextBox).Text
            c.p85OtherKey2 = BO.BAS.IsNullInt(CType(ri.FindControl("p41ID_Input"), UI.project).Value)
            c.p85FreeText02 = CType(ri.FindControl("p41ID_Input"), UI.project).Text

            c.p85OtherKey1 = BO.BAS.IsNullInt(CType(ri.FindControl("j02ID_Input"), UI.person).Value)
            c.p85FreeText01 = CType(ri.FindControl("j02ID_Input"), UI.person).Text

            Master.Factory.p85TempBoxBL.Save(c)
        Next
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Save2Temp()

        Dim intP34ID As Integer = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        If intP34ID = 0 Then
            Master.Notify("Musíte vybrat sešit.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim intP41ID As Integer = BO.BAS.IsNullInt(Me.p41ID.Value)
        If intP41ID = 0 And Me.p41ID.Visible Then
            Master.Notify("Musíte vybrat projekt.", NotifyLevel.WarningMessage)
            Return
        End If
        Dim intP32ID As Integer = BO.BAS.IsNullInt(Me.p32ID.SelectedValue)

        If strButtonValue = "save" Then
            Master.Factory.j03UserBL.SetUserParam("p48_multiple_create-p34id", Me.p34ID.SelectedValue)
            Master.Factory.j03UserBL.SetUserParam("p48_multiple_create-p32id", Me.p32ID.SelectedValue)

            Dim lis As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid"))
            If lis.Count = 0 Then
                Master.Notify("Na vstupu k uložení není ani jeden záznam plánu.", NotifyLevel.WarningMessage) : Return
            End If
            If lis.Where(Function(p) p.p85FreeFloat01 <= 0).Count > 0 Then
                Master.Notify("Na vstupu je minimálně jeden záznam s nulovým objemem hodin.", NotifyLevel.WarningMessage) : Return
            End If
            Dim lisP48 As New List(Of BO.p48OperativePlan)

            For Each cTemp In lis
                Dim c As New BO.p48OperativePlan()
                With cTemp
                    c.p48Date = .p85FreeDate01
                    c.j02ID = .p85OtherKey1
                    c.p41ID = .p85OtherKey2
                    If c.p41ID = 0 Then c.p41ID = intP41ID
                    c.p34ID = intP34ID
                    c.p32ID = intP32ID
                    c.p48Hours = .p85FreeFloat01
                    c.p48TimeFrom = .p85FreeText03
                    c.p48TimeUntil = .p85FreeText04
                    c.p48Text = .p85Message
                End With
                lisP48.Add(c)
            Next
            If Not Master.Factory.p48OperativePlanBL.SaveBatch(lisP48) Then
                Master.Notify(Master.Factory.p48OperativePlanBL.ErrorMessage, NotifyLevel.ErrorMessage)
                Return
            End If
            Master.CloseAndRefreshParent()
        End If
    End Sub

    
    Private Sub p41ID_AutoPostBack_SelectedIndexChanged(NewValue As String, OldValue As String) Handles p41ID.AutoPostBack_SelectedIndexChanged
        For Each ri As RepeaterItem In rp1.Items
            With CType(ri.FindControl("p41ID_Input"), UI.project)
                If BO.BAS.IsNullInt(.Value) = 0 Then
                    .Text = Me.p41ID.Text
                    .Value = Me.p41ID.Value
                End If
            End With
        Next
    End Sub
End Class