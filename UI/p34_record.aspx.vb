Public Class p34_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p34_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p32_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Worksheet sešit"

                'Me.p34IsCapacityPlan.Visible = .Factory.SysUser.AppScope.WorksheetPlanning
            End With
            Dim lis As IEnumerable(Of BO.p87BillingLanguage) = Master.Factory.ftBL.GetList_P87(New BO.myQuery)
            For Each c In lis
                CType(panLang.FindControl("lblLang" & c.p87LangIndex.ToString), Label).Text = c.p87Name
            Next

            RefreshRecord()
            Handle_ChangeP33()
            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub
    Private Sub RefreshRecord()
        Me.p33ID.FillData(Master.Factory.ftBL.GetList_P33())

        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(Master.DataPID)
        With cRec
            Me.p34name.Text = .p34Name
            Me.p34Code.Text = .p34Code

            'Me.p34IsCapacityPlan.Checked = .p34IsCapacityPlan
            Me.p33ID.SelectedValue = CInt(.p33ID).ToString
            Me.p34Ordinary.Value = .p34Ordinary
            Master.Timestamp = .Timestamp
            basUI.SelectDropdownlistValue(Me.p34ActivityEntryFlag, CInt(.p34ActivityEntryFlag).ToString)
            basUI.SelectDropdownlistValue(Me.p34IncomeStatementFlag, CInt(.p34IncomeStatementFlag).ToString)

            Me.p34Name_BillingLang1.Text = .p34Name_BillingLang1
            Me.p34Name_BillingLang2.Text = .p34Name_BillingLang2
            Me.p34Name_BillingLang3.Text = .p34Name_BillingLang3
            Me.p34Name_BillingLang4.Text = .p34Name_BillingLang4

            basUI.SetColorToPicker(Me.p34Color, .p34Color)
           

            'Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
        If cRec.p34ActivityEntryFlag = BO.p34ActivityEntryFlagENUM.AktivitaSeNezadava Then
            SetupP32Combo()

        End If
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p34ActivityGroupBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p34-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p34ActivityGroupBL
            Dim cRec As BO.p34ActivityGroup = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p34ActivityGroup)
            cRec.p34Name = Me.p34name.Text
            cRec.p33ID = BO.BAS.IsNullInt(Me.p33ID.SelectedValue)
            cRec.p34ActivityEntryFlag = BO.BAS.IsNullInt(Me.p34ActivityEntryFlag.SelectedValue)
            cRec.p34Code = Me.p34Code.Text
            cRec.p34IncomeStatementFlag = BO.BAS.IsNullInt(Me.p34IncomeStatementFlag.SelectedValue)

            'cRec.p34IsCapacityPlan = Me.p34IsCapacityPlan.Checked
            cRec.p34Ordinary = BO.BAS.IsNullInt(Me.p34Ordinary.Value)
            cRec.p34Name_BillingLang1 = Me.p34Name_BillingLang1.Text
            cRec.p34Name_BillingLang2 = Me.p34Name_BillingLang2.Text
            cRec.p34Name_BillingLang3 = Me.p34Name_BillingLang3.Text
            cRec.p34Name_BillingLang4 = Me.p34Name_BillingLang4.Text
            'cRec.ValidFrom = Master.RecordValidFrom
            'cRec.ValidUntil = Master.RecordValidUntil
            cRec.p34Color = basUI.GetColorFromPicker(Me.p34Color)

            If .Save(cRec, BO.BAS.IsNullInt(Me.p32ID.SelectedValue)) Then
                If Master.DataPID = 0 Then
                    'po založení nového sešitu ještě přesměrovat na dokončovací stránku
                    Server.Transfer("p34_after_create.aspx?pid=" & .LastSavedPID.ToString, False)
                    Return
                End If
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p34-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub Handle_ChangeP33()
        'Dim intP33ID As Integer = BO.BAS.IsNullInt(Me.p33ID.SelectedValue)
        'Dim lis As IEnumerable(Of BO.p33ActivityInputType) = Master.Factory.ftBL.GetList_P33().Where(Function(p) p.PID = intP33ID)
        'If lis.Count > 0 Then
        '    Select Case lis(0).p33Code
        '        Case "T"
        '            p34IsCapacityPlan.Text = "Sešit pro kapacitní plánování"
        '        Case "M", "MV"
        '            p34IsCapacityPlan.Text = "Sešit pro finanční plánování"
        '        Case "U"
        '            p34IsCapacityPlan.Text = "Sešit pro plánování"
        '    End Select

        'End If
        
    End Sub
    

    Private Sub p33ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p33ID.SelectedIndexChanged
        Handle_ChangeP33()
    End Sub

    Private Sub p34_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If Me.p34ActivityEntryFlag.SelectedValue = "1" Then
            Me.panDefaultActivity.Visible = True
            If Me.p32ID.Rows <= 1 Then
                SetupP32Combo()
            End If
        Else
            Me.panDefaultActivity.Visible = False
        End If
    End Sub

    Private Sub SetupP32Combo()
        Dim mq As New BO.myQueryP32
        mq.p34ID = Master.DataPID
        Dim lis As IEnumerable(Of BO.p32Activity) = Master.Factory.p32ActivityBL.GetList(mq)
        Me.p32ID.DataSource = lis
        Me.p32ID.DataBind()
        If lis.Where(Function(p) p.p32IsSystemDefault = True).Count > 0 Then
            Me.p32ID.SelectedValue = lis.Where(Function(p) p.p32IsSystemDefault = True)(0).PID.ToString
        End If
    End Sub
End Class