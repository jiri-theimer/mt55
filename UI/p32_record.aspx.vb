Public Class p32_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p32_record_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p32_record"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Worksheet aktivita"

                Dim lisP87 As IEnumerable(Of BO.p87BillingLanguage) = .Factory.ftBL.GetList_P87(New BO.myQuery)
                lblLang1.Text = lisP87(0).p87Name & ":"
                lblLang2.Text = lisP87(1).p87Name & ":"
                lblLang3.Text = lisP87(2).p87Name & ":"
                lblLang4.Text = lisP87(3).p87Name & ":"
                lblLang1a.Text = lblLang1.Text : lblLang2a.Text = lblLang2.Text : lblLang3a.Text = lblLang3.Text : lblLang4a.Text = lblLang4.Text
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        Me.p34ID.FillData(Master.Factory.p34ActivityGroupBL.GetList(New BO.myQuery), True)
        Me.x15id.FillData(Master.Factory.ftBL.GetList_X15(New BO.myQuery), True)
        Me.p95id.FillData(Master.Factory.p95InvoiceRowBL.GetList(New BO.myQuery), True)
        Me.p35id.DataSource = Master.Factory.ftBL.GetList_P35()
        Me.p35id.DataBind()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p32Activity = Master.Factory.p32ActivityBL.Load(Master.DataPID)
        With cRec
            Me.p32name.Text = .p32Name
            Me.p32Code.Text = .p32Code
            Me.p32IsBillable.Checked = .p32IsBillable
            Me.p32IsTextRequired.Checked = .p32IsTextRequired
            Me.p34ID.SelectedValue = .p34ID.ToString
            Me.x15id.SelectedValue = CInt(.x15ID).ToString
            Me.p95id.SelectedValue = .p95ID.ToString

            Me.p32Ordinary.Value = .p32Ordinary
            Me.p32Value_Default.Value = .p32Value_Default
            Me.p32Value_Maximum.Value = .p32Value_Maximum
            Me.p32Value_Minimum.Value = .p32Value_Minimum
            Me.p32DefaultWorksheetText.Text = .p32DefaultWorksheetText
            Me.p32HelpText.Text = .p32HelpText

            Master.Timestamp = .Timestamp

            Me.p32Name_BillingLang1.Text = .p32Name_BillingLang1
            Me.p32Name_BillingLang2.Text = .p32Name_BillingLang2
            Me.p32Name_BillingLang3.Text = .p32Name_BillingLang3
            Me.p32Name_BillingLang4.Text = .p32Name_BillingLang4

            Me.p32DefaultWorksheetText_Lang1.Text = .p32DefaultWorksheetText_Lang1
            Me.p32DefaultWorksheetText_Lang2.Text = .p32DefaultWorksheetText_Lang2
            Me.p32DefaultWorksheetText_Lang3.Text = .p32DefaultWorksheetText_Lang3
            Me.p32DefaultWorksheetText_Lang4.Text = .p32DefaultWorksheetText_Lang4
            Me.p35id.SelectedValue = .p35ID.ToString
            Me.p32ExternalPID.Text = .p32ExternalPID
            Me.p32FreeText01.Text = .p32FreeText01
            Me.p32FreeText02.Text = .p32FreeText02
            Me.p32FreeText03.Text = .p32FreeText03
            basUI.SelectDropdownlistValue(Me.p32AttendanceFlag, CInt(.p32AttendanceFlag).ToString)
            basUI.SelectRadiolistValue(Me.p32ManualFeeFlag, .p32ManualFeeFlag.ToString)
            Me.p32ManualFeeDefAmount.Value = .p32ManualFeeDefAmount

            basUI.SetColorToPicker(Me.p32Color, .p32Color)
            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p32ActivityBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p32-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p32ActivityBL
            Dim cRec As BO.p32Activity = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p32Activity)
            With cRec
                .p32Name = Me.p32name.Text
                .p34ID = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
                .x15ID = BO.BAS.IsNullInt(Me.x15id.SelectedValue)
                .p95ID = BO.BAS.IsNullInt(Me.p95id.SelectedValue)
                If Me.p35id.Visible Then .p35ID = BO.BAS.IsNullInt(Me.p35id.SelectedValue)

                .p32Code = Me.p32Code.Text
                .p32IsBillable = Me.p32IsBillable.Checked
                .p32IsTextRequired = Me.p32IsTextRequired.Checked
                .p32Ordinary = BO.BAS.IsNullInt(Me.p32Ordinary.Value)
                .p32Value_Default = BO.BAS.IsNullNum(Me.p32Value_Default.Value)
                .p32Value_Maximum = BO.BAS.IsNullNum(Me.p32Value_Maximum.Value)
                .p32Value_Minimum = BO.BAS.IsNullNum(Me.p32Value_Minimum.Value)
                .p32DefaultWorksheetText = Me.p32DefaultWorksheetText.Text
                .p32HelpText = Me.p32HelpText.Text

                .p32Name_BillingLang1 = Me.p32Name_BillingLang1.Text
                .p32Name_BillingLang2 = Me.p32Name_BillingLang2.Text
                .p32Name_BillingLang3 = Me.p32Name_BillingLang3.Text
                .p32Name_BillingLang4 = Me.p32Name_BillingLang4.Text

                .p32DefaultWorksheetText_Lang1 = Me.p32DefaultWorksheetText_Lang1.Text
                .p32DefaultWorksheetText_Lang2 = Me.p32DefaultWorksheetText_Lang2.Text
                .p32DefaultWorksheetText_Lang3 = Me.p32DefaultWorksheetText_Lang3.Text
                .p32DefaultWorksheetText_Lang4 = Me.p32DefaultWorksheetText_Lang4.Text
                .p32FreeText01 = Me.p32FreeText01.Text
                .p32FreeText02 = Me.p32FreeText02.Text
                .p32FreeText03 = Me.p32FreeText03.Text

                .p32Color = basUI.GetColorFromPicker(Me.p32Color)
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
                .p32ExternalPID = Me.p32ExternalPID.Text
                .p32AttendanceFlag = BO.BAS.IsNullInt(Me.p32AttendanceFlag.SelectedValue)
                If Me.p32ManualFeeFlag.Visible Then
                    .p32ManualFeeFlag = CInt(Me.p32ManualFeeFlag.SelectedValue)
                    If .p32ManualFeeFlag = 1 Then
                        .p32ManualFeeDefAmount = BO.BAS.IsNullNum(Me.p32ManualFeeDefAmount.Value)
                    Else
                        .p32ManualFeeDefAmount = 0
                    End If
                Else
                    .p32ManualFeeFlag = 0 : .p32ManualFeeDefAmount = 0
                End If
                
            End With
            


            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p32-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p34ID_SelectedIndexChanged(OldValue As String, OldText As String, CurValue As String, CurText As String) Handles p34ID.SelectedIndexChanged

    End Sub

    Private Sub p32_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Me.lblp35id.Visible = False : Me.p35id.Visible = False
        trManualFlag.Visible = False : p32ManualFeeDefAmount.Visible = False

        Dim intP34ID As Integer = BO.BAS.IsNullInt(Me.p34ID.SelectedValue)
        If intP34ID > 0 Then
            Dim cP34 As BO.p34ActivityGroup = Master.Factory.p34ActivityGroupBL.Load(intP34ID)
            If cP34.p33ID = BO.p33IdENUM.Kusovnik Then
                lblp35id.Visible = True : Me.p35id.Visible = True
            End If

            If cP34.p33ID = BO.p33IdENUM.Cas Then
                Me.trManualFlag.Visible = True
                Dim b As Boolean = False
                If Me.p32ManualFeeFlag.SelectedValue = "1" Then
                    b = True
                End If
                lblp32ManualFeeDefAmount.Visible = b
                p32ManualFeeDefAmount.Visible = b
            End If
        End If

        

    End Sub
End Class