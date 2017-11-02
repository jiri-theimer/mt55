Public Class p31_record_AA
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_record_AA_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        approve1.Factory = Master.Factory
        ff2.Factory = Master.Factory

        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then
                    .StopPage("PID is missing.")
                End If
                .HeaderText = "Worksheet úkon"
                .HeaderIcon = "Images/worksheet_32.png"
                With .Factory.j03UserBL
                    .InhaleUserParams("p31_approving-use_internal_approving")
                    If .GetUserParam("p31_approving-use_internal_approving", "0") = "1" Then
                        approve1.AllowInternalApproving = True
                    End If
                End With


            End With

            RefreshRecord()
            comments1.RefreshData(Master.Factory, BO.x29IdEnum.p31Worksheet, Master.DataPID)

        End If
    End Sub

    Private Sub RefreshRecord()
        'zatím jenom dočasně řešená práva
        Dim cD As BO.p31WorksheetDisposition = Master.Factory.p31WorksheetBL.InhaleRecordDisposition(Master.DataPID)
        If cD.RecordDisposition = BO.p31RecordDisposition._NoAccess Then
            Master.StopPage("Uživatel nedisponuje přístupovým právem k tomuto worksheet záznamu.")
        End If
       

        Me.lblLockedReasonMessage.Text = cD.LockedReasonMessage

        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.Load(Master.DataPID)
        If cRec Is Nothing Then
            Master.StopPage("Záznam nebyl nalezen.", True)
        End If

        With cRec
            If .p71ID = BO.p71IdENUM.Nic And (cD.RecordDisposition = BO.p31RecordDisposition.CanEdit Or cD.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit) Then
                cmdText.Visible = True
            End If
            If cD.RecordDisposition = BO.p31RecordDisposition.CanApprove Or cD.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Then
                cmdApprove.Visible = True
                If .p71ID > BO.p71IdENUM.Nic Then
                    cmdApprove.Text = "Pře-schválit worksheet úkon"
                Else
                    cmdApprove.Text = "Schvalovat worksheet úkon"
                End If
                approve1.HeaderText = cmdApprove.Text


            End If
            If .IsClosed Then
                lblLockedReasonMessage.Text = "Schválený záznam byl přesunutý do archivu."
                cmdApprove.Visible = False
                Master.RadToolbar.Skin = "BlackMetroTouch"
            End If
            If cmdApprove.Visible Then
                cmdApprove.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_ApprovingDialog)
            End If

            ''lblReadonlyReason.Text = .ReadOnlyMessage

            Me.Person.Text = .Person
            ''j03_alias.ClueRelURL = "ctip_j03.aspx?pid=" & .j03ID.ToString
            Me.clue_person.Attributes("rel") = "clue_j02_record.aspx?pid=" & .j02ID.ToString
            Me.p34name.Text = .p34Name
            Me.p32name.Text = .p32Name
            update_p31date.SelectedDate = .p31Date

            Me.billable.Text = IIf(.p32IsBillable, "[fakturovatelné]", "[ne-fakturovatelné]")
            p31date.Text = BO.BAS.FD(.p31Date)

            Me.Client.Text = .ClientName
            p41Name.Text = .p41Name
            p31text.Text = Replace(.p31Text, vbCrLf, "<br>")
            p31value_orig.Text = BO.BAS.FN(.p31Value_Orig)
            If .p33ID = BO.p33IdENUM.Cas Then
                p31value_orig.Text += " (" & .p31HHMM_Orig & ")"
            End If

            If .p56ID <> 0 Then
                Me.Task.Text = .p56Name
            Else
                lblP56.Visible = False
            End If

            If cD.RecordDisposition = BO.p31RecordDisposition.CanApprove Or cD.RecordDisposition = BO.p31RecordDisposition.CanApproveAndEdit Then
                update_p31text.Text = .p31Text
            End If

            If cRec.p33ID = BO.p33IdENUM.Cas Or cRec.p33ID = BO.p33IdENUM.Kusovnik Then
                j27ident_orig.Text = "h."
                p31Rate_Billing_Orig.Text = BO.BAS.FN(cRec.p31Rate_Billing_Orig)
                rate_j27ident.Text = .j27Code_Billing_Orig

            Else
                j27ident_orig.Text = .j27Code_Billing_Orig
                lblBillingRate_Orig.Visible = False
            End If
            'If .p71ID <> 0 Then Master.HeaderIcon = "Images/is_approved_32.png"
            'If .p70ID <> 0 Then Master.HeaderIcon = "Images/is_invoiced_32.png"




            If cRec.p71ID > BO.p71IdENUM.Nic Then
                panApproved.Visible = True
                cmdApprove.Text = "Pře-schválit worksheet úkon"

                Me.p71name.Text = .p71Name
                Me.p72name.Text = .approve_p72Name

                Dim status As New BO.p72PreBillingStatus()
                status.SetStatus(.p72ID_AfterApprove)
                Me.p72img.ImageUrl = status.ImageUrl
                If .p72ID_AfterApprove = BO.p72IdENUM._NotSpecified Then Me.p72img.Visible = False

                Me.value_approved_billing.Text = BO.BAS.FN(.p31Value_Approved_Billing)
                If .p33ID = BO.p33IdENUM.Cas Then
                    Me.value_approved_billing.Text += " (" & .p31HHMM_Approved_Billing & ")"
                End If
                If .p31Value_Approved_Billing <> .p31Value_Orig Then
                    lblKorekceCaption.Visible = True
                    imgKorekce.Visible = True
                    value_korekce.Text = BO.BAS.FN(.p31Value_Approved_Billing - .p31Value_Orig)
                    If .p33ID = BO.p33IdENUM.Cas Then
                        Dim cT As New BO.clsTime
                        value_korekce.Text += " (" & cT.ShowAsHHMM(CDbl(.p31Value_Approved_Billing - .p31Value_Orig).ToString) & ")"
                    End If
                    If cRec.p31Value_Orig > .p31Value_Approved_Billing Then
                        imgKorekce.ImageUrl = "Images/correction_down.gif"
                    Else
                        imgKorekce.ImageUrl = "Images/correction_up.gif"
                    End If
                    If Not .p32IsBillable And .p31Value_Approved_Billing = 0 Then
                        imgKorekce.Visible = False : lblKorekceCaption.Visible = False : value_korekce.Visible = False
                    End If
                End If
                If .p33ID = BO.p33IdENUM.Cas Or .p33ID = BO.p33IdENUM.Kusovnik Then
                    If .p32ManualFeeFlag = 0 Then
                        rate_approved.Text = BO.BAS.FN(.p31Rate_Billing_Approved)
                    End If
                    If .p32ManualFeeFlag = 1 Then
                        rate_approved.Text = BO.BAS.FN(.p31Amount_WithoutVat_Approved)
                        lblFakturacniSazba_Approved.Text = "Schválený pevný honorář:"
                    End If

                    j27Code_Approved.Text = .j27Code_Billing_Orig
                Else
                    lblFakturacniSazba_Approved.Visible = False
                End If
                If .p33ID = BO.p33IdENUM.Cas Then
                    trInternal.Visible = True
                    value_approved_internal.Text = BO.BAS.FN(.p31Value_Approved_Internal)
                    If .p31Value_Approved_Internal <> .p31Value_Orig Then
                        imgKorekceInternal.Visible = True
                        If .p31Value_Orig > .p31Value_Approved_Internal Then
                            imgKorekceInternal.ImageUrl = "Images/correction_down.gif"
                        Else
                            imgKorekceInternal.ImageUrl = "Images/correction_up.gif"
                        End If
                    Else
                        imgKorekceInternal.Visible = False
                    End If
                Else
                    trInternal.Visible = False
                End If

                If .j02ID_ApprovedBy > 0 Then

                End If

                lblTimestamp_Approve.Text = "<img src='Images/approve.png'> " & "Schválil" & ": " & Master.Factory.j02PersonBL.Load(.j02ID_ApprovedBy).FullNameDesc & "/" & BO.BAS.FD(.p31Approved_When, True, True)

                Me.p31ApprovingLevel.Text = "#" & .p31ApprovingLevel.ToString
                If .p31ApprovingLevel > 0 Then Me.p31ApprovingLevel.CssClass = "valboldblue"
            Else
                panApproved.Visible = False
            End If
            If cRec.p70ID > BO.p70IdENUM.Nic Then
                cmdApprove.Visible = False
                cmdText.Visible = False
                panInvoiced.Visible = True
                p70name.Text = .p70Name
                Dim status As New BO.p70BillingStatus
                status.SetStatus(.p70ID)
                Me.p70name.Style.Item("background-color") = status.Color

                p91ident.Text = .p91Code
                p31amount_withoutvat_invoiced.Text = BO.BAS.FN(.p31Amount_WithoutVat_Invoiced)
                Dim cJ27I As BO.j27Currency = Master.Factory.ftBL.LoadJ27(.j27ID_Billing_Invoiced)
                j27ident_invoiced.Text = cJ27I.j27Code


            Else
                panInvoiced.Visible = False

            End If

            If Not Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_AllowRates) Then
                'uživatel nemá nárok vidět věci po schvalování a fakturaci
                panInvoiced.Visible = False
                panApproved.Visible = False
                lblBillingRate_Orig.Visible = False
                p31Rate_Billing_Orig.Visible = False
                rate_j27ident.Visible = False
            End If

           
            labels1.RefreshData(Master.Factory, BO.x29IdEnum.p31Worksheet, .PID)

            Me.linkTimestamp.Text = .Timestamp & " | Vlastník záznamu: <span class='val'>" & .Owner & "</span>"
            Master.HeaderText = .p34Name & " | " & BO.BAS.FD(.p31Date) & " | " & .Person & " | " & .p41Name

            Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p31Worksheet, Master.DataPID, cRec.p34ID)
            ff2.FillData(fields, False)
            labels1.RefreshData(Master.Factory, BO.x29IdEnum.p31Worksheet, Master.DataPID, True)
        End With
    End Sub

    Private Sub cmdSaveText_Click(sender As Object, e As EventArgs) Handles cmdSaveText.Click
        With Master.Factory.p31WorksheetBL
            If .Update_p31Text(Master.DataPID, Me.update_p31text.Text) Then
                Master.Notify("Změny v popisu úkonu uloženy.")
                RefreshRecord()
            Else
                Master.Notify(.ErrorMessage, 2)
            End If

        End With
        
    End Sub

    Private Sub cmdApprove_Click(sender As Object, e As EventArgs) Handles cmdApprove.Click
        panApproving.Visible = True
        approve1.InhaleRecord(Master.Factory.p31WorksheetBL.Load(Master.DataPID))
        cmdApprove.Visible = False
        Me.panP31Text.Visible = False
        cmdSourceRecord.Visible = Master.Factory.TestPermission(BO.x53PermValEnum.GR_P31_ApprovingDialog)
    End Sub

    Private Sub approve1_AfterSave(ByRef strErr As String) Handles approve1.AfterSave
        If strErr <> "" Then
            Master.Notify(strErr, 2)
        Else
            ''Master.Factory.p31WorksheetBL.SaveFreeFields(Master.DataPID, ff1.GetValues(), False, "")
            ''If ff1.TagsCount > 0 Then
            ''    Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p31Worksheet, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs())
            ''End If
            Master.CloseAndRefreshParent("p31-save")
        End If
        cmdApprove.Visible = True
        Me.panP31Text.Visible = True
    End Sub

    Private Sub approve1_CancelSave() Handles approve1.CancelSave
        panApproving.Visible = False
        cmdApprove.Visible = True
    End Sub

    
End Class