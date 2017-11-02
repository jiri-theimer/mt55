Public Class p31_approve_onerec
    Inherits System.Web.UI.UserControl
    Public Event AfterSave(ByRef strErr As String)
    Public Event CancelSave()
    Public Event BeforeSave(ByRef lisFF As List(Of BO.FreeField))


    Public ReadOnly Property CurrentP31ID As Integer
        Get
            Return BO.BAS.IsNullInt(Me.p31id.Value)
        End Get
    End Property
    Public Property IsVertical As Boolean
        Get
            Return BO.BAS.BG(Me.hidIsVertical.Value)
        End Get
        Set(value As Boolean)
            Me.hidIsVertical.Value = BO.BAS.GB(value)
            If value Then
                Me.p31Text.Style.Item("height") = "100px"
            Else
                Me.p31Text.Style.Item("height") = "50px"
            End If
        End Set
    End Property
    Public Property AllowInternalApproving As Boolean
        Get
            Return panInternalContainer.Visible
        End Get
        Set(value As Boolean)
            panInternalContainer.Visible = value
        End Set
    End Property
    Public Property GUID_TempData As String
        Get
            Return Me.hidGUID_TempData.Value
        End Get
        Set(value As String)
            Me.hidGUID_TempData.Value = value
        End Set
    End Property
    Public ReadOnly Property IsTempRecord As Boolean
        Get
            If Me.GUID_TempData <> "" Then Return True Else Return False
        End Get
    End Property
    Public Property CommandSaveText As String
        Get
            Return cmdUlozitSchvalovani.Text
        End Get
        Set(value As String)
            cmdUlozitSchvalovani.Text = value
        End Set
    End Property
    Public Property ShowCancelCommand As Boolean
        Get
            Return cmdZrusitSchvalovani.Visible
        End Get
        Set(value As Boolean)
            cmdZrusitSchvalovani.Visible = value
        End Set
    End Property
    Public Property HeaderText As String
        Get
            Return lblHeader.Text
        End Get
        Set(value As String)
            Me.lblHeader.Text = value
        End Set
    End Property

    Public ReadOnly Property CurrentP33ID As BO.p33IdENUM
        Get
            Return CType(BO.BAS.IsNullInt(Me.p33id.Value), BO.p33IdENUM)
        End Get
    End Property
    Public Property CurrentP72ID As BO.p72IdENUM
        Get
            If Me.p72id.Visible Then
                Return CType(BO.BAS.IsNullInt(Me.p72id.SelectedValue), BO.p72IdENUM)
            Else
                Return BO.p72IdENUM._NotSpecified
            End If

        End Get
        Set(value As BO.p72IdENUM)
            basUI.SelectRadiolistValue(Me.p72id, CInt(value).ToString)
        End Set
    End Property
    Public Property CurrentP71ID As BO.p71IdENUM
        Get
            Return CType(BO.BAS.IsNullInt(Me.p71id.SelectedValue), BO.p71IdENUM)
        End Get
        Set(value As BO.p71IdENUM)
            basUI.SelectRadiolistValue(Me.p71id, CInt(value).ToString)
        End Set
    End Property
    Public Property Factory As BL.Factory

    Private Sub SetValue(dblValue As Double, bolInternal As Boolean)
        If Me.CurrentP33ID = BO.p33IdENUM.Cas Then
            If Me.Factory.SysUser.TimeFormat4Read = BO.TimeFormat_ReadENUM.HHmm Or Len(dblValue.ToString) > 5 Then
                Dim cTime As New COM.clsTime
                If Not bolInternal Then
                    value_approved.Text = cTime.ShowAsHHMM(dblValue.ToString)
                Else
                    value_approved_internal.Text = cTime.ShowAsHHMM(dblValue.ToString)
                End If
            Else
                If Not bolInternal Then
                    value_approved.Text = dblValue.ToString
                Else
                    value_approved_internal.Text = dblValue.ToString
                End If
            End If
        Else
            If Not bolInternal Then
                value_approved.Text = dblValue.ToString
            Else
                value_approved_internal.Text = dblValue.ToString
            End If
        End If
    End Sub
    Private Function GetValue(bolInternal As Boolean) As Double
        Dim s As String = Me.value_approved.Text
        If bolInternal Then
            s = Me.value_approved_internal.Text
        End If
        Return BO.BAS.ConvertTimeToHours(s)
    End Function

    Private Sub DraftStatus(status As BO.p71IdENUM, cRec As BO.p31Worksheet)
        Me.CurrentP71ID = status

        If isbillable.Value = "1" Then
            Me.CurrentP72ID = BO.p72IdENUM.Fakturovat
        Else
            Me.CurrentP72ID = BO.p72IdENUM.ViditelnyOdpis
        End If

        With cRec
            SetValue(.p31Value_Orig, False)
            VatRate_Approved.Value = .p31VatRate_Orig
            Select Case .p33ID
                'Case BO.p33IdENUM.PenizeBezDPH, BO.p33IdENUM.PenizeVcDPHRozpisu
                '    VatRate_Approved.Value = .p31VatRate_Orig
                Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                    Rate_Billing_Approved.Value = .p31Rate_Billing_Orig
                    Rate_Internal_Approved.Value = .p31Rate_Internal_Orig
            End Select
            If .p72ID_AfterTrimming > BO.p72IdENUM._NotSpecified Then
                SetValue(.p31Value_Trimmed, False)
                Me.CurrentP72ID = .p72ID_AfterTrimming

            End If
            If .p33ID = BO.p33IdENUM.PenizeBezDPH Or .p33ID = BO.p33IdENUM.PenizeVcDPHRozpisu Then
                VatRate_Approved.Value = .p31VatRate_Orig

            End If

            SetValue(.p31Value_Orig, True)
        End With

        RefreshState()
    End Sub



    Public Sub InhaleRecord(cRec As BO.p31Worksheet, Optional bolDraftIfInEditing As Boolean = True)
        p31id.Value = cRec.PID.ToString
        With cRec
            Me.p31Date.SelectedDate = .p31Date
            p33id.Value = CInt(.p33ID).ToString
            SetValue(.p31Value_Approved_Billing, False)

            If .p32IsBillable Then isbillable.Value = "1" Else isbillable.Value = "0"
            Me.p32ManualFeeFlag.Value = .p32ManualFeeFlag.ToString
            Me.VatRate_Approved.Value = .p31VatRate_Approved
            Me.Rate_Billing_Approved.Value = .p31Rate_Billing_Approved
            Me.Rate_Internal_Approved.Value = .p31Rate_Internal_Approved
            SetValue(.p31Value_Approved_Internal, True)

            Me.j27Code.Text = .j27Code_Billing_Orig
            basUI.SelectRadiolistValue(Me.p31ApprovingLevel, .p31ApprovingLevel.ToString)

            If .p32ManualFeeFlag = 1 Then
                Me.ManualFee.Value = .p31Amount_WithVat_Approved
            End If

            If .p72ID_AfterApprove > BO.p72IdENUM._NotSpecified Then
                p72id.SelectedValue = CInt(.p72ID_AfterApprove).ToString
            End If
            If .p71ID > BO.p71IdENUM.Nic Then
                Me.CurrentP71ID = .p71ID

            Else
                If bolDraftIfInEditing Then
                    DraftStatus(BO.p71IdENUM.Schvaleno, cRec)
                Else
                    Me.CurrentP71ID = BO.p71IdENUM.Nic
                End If
            End If
            Me.p31Text.Text = .p31Text
            If p72id.SelectedValue = "6" Then
                If .p31Value_FixPrice = 0 Then .p31Value_FixPrice = .p31Value_Orig
                If Me.Factory.SysUser.TimeFormat4Read = BO.TimeFormat_ReadENUM.HHmm Or Len(.p31Value_FixPrice.ToString) > 5 Then
                    Dim cTime As New COM.clsTime
                    Me.value_fixprice.Text = cTime.ShowAsHHMM(.p31Value_FixPrice.ToString)
                Else
                    Me.value_fixprice.Text = .p31Value_FixPrice.ToString
                End If
            End If
        End With
        Dim p41ids As List(Of Integer) = Nothing, p28ids As List(Of Integer) = Nothing, strGUID As String = ""
        If Not Me.IsTempRecord Then
            p41ids = New List(Of Integer)
            p41ids.Add(cRec.p41ID)
            If cRec.p28ID_Client <> 0 Then
                p28ids = New List(Of Integer)
                p28ids.Add(cRec.p28ID_Client)
            End If
        Else
            strGUID = Me.GUID_TempData
        End If
        
        Dim sets As List(Of String) = Factory.p31WorksheetBL.GetList_ApprovingSet(strGUID, p41ids, p28ids)
        With Me.p31ApprovingSet
            For Each s In sets
                .Items.Add(New Telerik.Web.UI.RadComboBoxItem(s))
            Next
            .Text = cRec.p31ApprovingSet
        End With
        

        RefreshState()
    End Sub

    Private Sub RefreshState()
        Dim b As Boolean = False
        Select Case Me.CurrentP71ID
            Case BO.p71IdENUM.Neschvaleno, BO.p71IdENUM.Nic
                b = False
            Case Else
                b = True
        End Select
       
        p72id.Visible = b
        value_approved.Visible = b
        lblFakturovat.Visible = b
        lblVatRate_Approved.Visible = b
        VatRate_Approved.Visible = b
        panInternal.Visible = b
        Me.p31Text.Visible = b
        Me.lblP31Text.Visible = b
        Me.j27Code.Visible = b
        Me.p31ApprovingLevel.Visible = b
        Me.p31Date.Visible = b

        Select Case Me.CurrentP33ID
            Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                lblRate_Billing_Approved.Visible = b
                lblRate_Internal_Approved.Visible = b
                Rate_Billing_Approved.Visible = b
                Rate_Internal_Approved.Visible = b


                If Me.CurrentP33ID = BO.p33IdENUM.Cas Then
                    lblFakturovat.Text = BO.BAS.OM2(Me.lblFakturovat.Text, "hodiny")
                Else
                    lblFakturovat.Text = BO.BAS.OM2(Me.lblFakturovat.Text, "počet")
                End If
                lblVatRate_Approved.Visible = False
                VatRate_Approved.Visible = False
                If Me.p32ManualFeeFlag.Value = "1" Then
                    lblRate_Billing_Approved.Text = "Pevný honorář:"
                    Rate_Billing_Approved.Visible = False
                    Me.ManualFee.Visible = True
                Else
                    Me.ManualFee.Visible = False
                End If
            Case Else
                Me.j27Code.Visible = False
                lblRate_Billing_Approved.Visible = False
                lblRate_Internal_Approved.Visible = False
                Rate_Billing_Approved.Visible = False
                Rate_Internal_Approved.Visible = False


                lblFakturovat.Text = BO.BAS.OM2(Me.lblFakturovat.Text, "částka")
        End Select
        If (Me.CurrentP72ID = BO.p72IdENUM.ViditelnyOdpis Or Me.CurrentP72ID = BO.p72IdENUM.ZahrnoutDoPausalu Or Me.CurrentP72ID = BO.p72IdENUM.SkrytyOdpis) And p72id.Visible Then
            value_approved.Visible = False
            lblFakturovat.Visible = False
            lblVatRate_Approved.Visible = False
            VatRate_Approved.Visible = False

            lblRate_Billing_Approved.Visible = False
            Rate_Billing_Approved.Visible = False
            Me.j27Code.Visible = False
            Me.ManualFee.Visible = False
        End If
        If Me.CurrentP71ID = BO.p71IdENUM.Schvaleno And Me.CurrentP72ID = BO.p72IdENUM.ZahrnoutDoPausalu Then
            lblValue_FixPrice.Visible = True : Me.value_fixprice.Visible = True
            If Me.CurrentP33ID = BO.p33IdENUM.Cas Then lblValue_FixPrice.Text = "Hodiny zahrnuté v paušálu:"
        Else
            lblValue_FixPrice.Visible = False : Me.value_fixprice.Visible = False
        End If
    End Sub

    

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RefreshState()
        If Me.IsVertical Then
            place1.Controls.Add(New LiteralControl("</tr><tr>"))
        End If

    End Sub

   

    Protected Sub p71id_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles p71id.SelectedIndexChanged
        If Me.CurrentP72ID = BO.p72IdENUM._NotSpecified Then
            Dim cRec As BO.p31Worksheet = Me.Factory.p31WorksheetBL.Load(Me.CurrentP31ID)
            DraftStatus(Me.CurrentP71ID, cRec)
        End If

        RefreshState()
    End Sub

    Protected Sub p72id_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles p72id.SelectedIndexChanged
        Select Case Me.CurrentP33ID
            Case BO.p33IdENUM.Cas, BO.p33IdENUM.Kusovnik
                If Me.CurrentP72ID = BO.p72IdENUM.Fakturovat Then
                    'natáhnout původní sazbu nebo původní hodiny/počet
                    Dim cRec As BO.p31Worksheet = Me.Factory.p31WorksheetBL.Load(Me.CurrentP31ID)
                    If Rate_Billing_Approved.Value = 0 Then
                        Rate_Billing_Approved.Value = cRec.p31Rate_Billing_Orig
                    End If
                    If GetValue(False) = 0 Then
                        SetValue(cRec.p31Value_Orig, False)
                    End If
                End If
                If Me.CurrentP72ID = BO.p72IdENUM.ZahrnoutDoPausalu And Me.value_fixprice.Text = "" Then
                    Dim cRec As BO.p31Worksheet = Me.Factory.p31WorksheetBL.Load(Me.CurrentP31ID)
                    If Me.Factory.SysUser.TimeFormat4Read = BO.TimeFormat_ReadENUM.HHmm Or cRec.IsRecommendedHHMM() Then
                        Dim cTime As New COM.clsTime
                        Me.value_fixprice.Text = cTime.ShowAsHHMM(cRec.p31Value_Orig.ToString)
                    Else
                        Me.value_fixprice.Text = cRec.p31Value_Orig.ToString
                    End If
                End If
        End Select
        
        RefreshState()
    End Sub

    Private Sub cmdUlozitSchvalovani_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUlozitSchvalovani.Click
        Dim cApproveInput As New BO.p31WorksheetApproveInput(Me.CurrentP31ID, Me.CurrentP33ID)
        With cApproveInput
            .GUID_TempData = Me.GUID_TempData
            .p71id = Me.CurrentP71ID
            .p72id = Me.CurrentP72ID
            .Value_Approved_Billing = GetValue(False)
            .Value_Approved_Internal = GetValue(True)
            .VatRate_Approved = VatRate_Approved.Value
            .Rate_Billing_Approved = BO.BAS.IsNullNum(Me.Rate_Billing_Approved.Value)
            .Rate_Internal_Approved = BO.BAS.IsNullNum(Me.Rate_Internal_Approved.Value)
            .p31Text = Me.p31Text.Text
            .p31Date = Me.p31Date.SelectedDate
            .p31ApprovingSet = Me.p31ApprovingSet.Text
            .p31ApprovingLevel = CInt(Me.p31ApprovingLevel.SelectedValue)
            .p32ManualFeeFlag = CInt(Me.p32ManualFeeFlag.Value)
            .ManualFee_Approved = BO.BAS.IsNullNum(Me.ManualFee.Value)
            If .p72id = BO.p72IdENUM.ZahrnoutDoPausalu Then
                If Me.CurrentP33ID = BO.p33IdENUM.Cas Then
                    .p31Value_FixPrice = BO.BAS.ConvertTimeToHours(Me.value_fixprice.Text)
                Else
                    .p31Value_FixPrice = BO.BAS.IsNullNum(Me.value_fixprice.Text)
                End If
            End If

        End With
        With Me.Factory.p31WorksheetBL
            If .Save_Approving(cApproveInput, Me.IsTempRecord) Then
                RaiseEvent AfterSave("")
            Else
                RaiseEvent AfterSave(.ErrorMessage)
            End If
        End With
    End Sub

    Private Sub cmdZrusitSchvalovani_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdZrusitSchvalovani.Click
        RaiseEvent CancelSave()
    End Sub
End Class