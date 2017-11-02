Public Class p31_approving_batch_p31text
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm
    Private _curRow As Integer = 0
    Private _cTime As New BO.clsTime

    Private Sub p31_approving_batch_p31text_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                ViewState("guid") = Request.Item("guid")
                If ViewState("guid") = "" Then .StopPage("guid is missing")
                ViewState("backurl") = Request.UrlReferrer.PathAndQuery
                
                .HeaderText = "Hromadná úprava popisu, hodnoty a sazby schvalovaných úkonů"
                .HeaderIcon = "Images/approve_32.png"

                .AddToolbarButton("Zrušit změny a zpět", "goback", , "Images/back.png")
                .AddToolbarButton("Potvrdit změny a zpět", "save", , "Images/ok.png")

                .HideShowToolbarButton("close", False)

                Dim lisPars As New List(Of String)
                With lisPars
                    .Add("p31_approving_batch_p31text-sort1")
                    .Add("p31_approving_batch_p31text-sort2")
                End With
                With .Factory.j03UserBL
                    .InhaleUserParams(lisPars)
                    basUI.SelectDropdownlistValue(Me.cbxSort1, .GetUserParam("p31_approving_batch_p31text-sort1", "1"))
                    basUI.SelectDropdownlistValue(Me.cbxSort2, .GetUserParam("p31_approving_batch_p31text-sort2", ""))
                End With

            End With


            RefreshData()

        End If
    End Sub

    Private Sub RefreshData()
        Dim lis As IEnumerable(Of BO.p31Worksheet) = Master.Factory.p31WorksheetBL.GetList(New BO.myQueryP31, ViewState("guid"))
        Select Case cbxSort1.SelectedValue
            Case "1"
                lis = lis.OrderByDescending(Function(p) p.p31Date).ThenByDescending(Function(p) p.PID)
            Case "2"
                lis = lis.OrderBy(Function(p) p.p31Date).ThenByDescending(Function(p) p.PID)
            Case "3"
                lis = lis.OrderBy(Function(p) p.Person)
            Case "4"
                lis = lis.OrderBy(Function(p) p.ClientName).ThenBy(Function(p) p.p41Name)
            Case Else
        End Select
        Select Case cbxSort1.SelectedValue
            Case "1"
                lis = lis.OrderByDescending(Function(p) p.p31Date).ThenByDescending(Function(p) p.PID)
            Case "2"
                lis = lis.OrderBy(Function(p) p.p31Date).ThenByDescending(Function(p) p.PID)
            Case "3"
                lis = lis.OrderBy(Function(p) p.Person)
            Case "4"
                lis = lis.OrderBy(Function(p) p.ClientName).ThenBy(Function(p) p.p41Name)
            Case Else
        End Select
        rp1.DataSource = lis
        rp1.DataBind()
    End Sub

   

    Private Sub p72ID_OnChange(sender As Object, e As EventArgs)
        Dim cbx As DropDownList = DirectCast(sender, DropDownList)
        Dim intP72ID As Integer = BO.BAS.IsNullInt(cbx.SelectedValue)
        Dim img As Image = DirectCast(cbx.FindControl("img1"), Image)
        Dim intP31ID As Integer = CInt(DirectCast(cbx.FindControl("p31id"), HiddenField).Value)
        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadTempRecord(intP31ID, ViewState("guid"))
        Dim cP72 As New BO.p72PreBillingStatus()
        cP72.SetStatus(intP72ID)
        If cP72.ImageUrl = "" Then
            img.Visible = False
        Else
            img.ImageUrl = cP72.ImageUrl
        End If
        If cRec.p33ID = BO.p33IdENUM.Cas Or cRec.p33ID = BO.p33IdENUM.Kusovnik Then
            Select Case cP72.p72ID
                Case BO.p72IdENUM.Fakturovat, BO.p72IdENUM.FakturovatPozdeji
                    If cRec.IsRecommendedHHMM() Then
                        DirectCast(cbx.FindControl("Value_Edit"), TextBox).Text = _cTime.ShowAsHHMM(cRec.p31Value_Approved_Billing.ToString)
                    Else
                        DirectCast(cbx.FindControl("Value_Edit"), TextBox).Text = cRec.p31Value_Approved_Billing.ToString
                    End If

                    DirectCast(cbx.FindControl("Rate_Edit"), Telerik.Web.UI.RadNumericTextBox).Value = cRec.p31Rate_Billing_Approved
                Case Else
                    DirectCast(cbx.FindControl("Value_Edit"), TextBox).Text = "0"
                    DirectCast(cbx.FindControl("Rate_Edit"), Telerik.Web.UI.RadNumericTextBox).Value = 0
            End Select
            If cP72.p72ID = BO.p72IdENUM.ZahrnoutDoPausalu Then
                cbx.FindControl("Value_Edit_FixPrice").Visible = True
                cbx.FindControl("lblValue_Edit_FixPrice").Visible = True
            Else
                cbx.FindControl("Value_Edit_FixPrice").Visible = False
                cbx.FindControl("lblValue_Edit_FixPrice").Visible = False
            End If

        End If
        ClientScript.RegisterStartupScript(Me.GetType, "hash", "go2id(" & intP31ID.ToString & ");", True)

    End Sub

    Private Sub rp1_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemCreated
        AddHandler CType(e.Item.FindControl("p72ID"), DropDownList).SelectedIndexChanged, AddressOf Me.p72ID_OnChange
    End Sub

    Private Sub rp1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rp1.ItemDataBound
        _curRow += 1
        Dim cRec As BO.p31Worksheet = CType(e.Item.DataItem, BO.p31Worksheet)
        With cRec
            CType(e.Item.FindControl("p31id"), HiddenField).Value = .PID.ToString
            CType(e.Item.FindControl("RowIndex"), Label).Text = "#" & _curRow.ToString
            Dim cP72 As New BO.p72PreBillingStatus()
            cP72.SetStatus(.p72ID_AfterApprove)
            If cP72.ImageUrl = "" Then
                e.Item.FindControl("img1").Visible = False
            Else
                CType(e.Item.FindControl("img1"), Image).ImageUrl = cP72.ImageUrl
            End If


            basUI.SelectDropdownlistValue(CType(e.Item.FindControl("p72ID"), DropDownList), CInt(.p72ID_AfterApprove).ToString)

            CType(e.Item.FindControl("p31Text"), TextBox).Text = .p31Text
            CType(e.Item.FindControl("p32Name"), Label).Text = .p32Name
            CType(e.Item.FindControl("p34Name"), Label).Text = .p34Name
            CType(e.Item.FindControl("Person"), Label).Text = .Person
            CType(e.Item.FindControl("p31date"), Label).Text = BO.BAS.FD(.p31Date)
            With CType(e.Item.FindControl("Value_Edit"), TextBox)
                Select Case cRec.p33ID
                    Case BO.p33IdENUM.Cas
                        If cRec.IsRecommendedHHMM() Then
                            .Text = _cTime.ShowAsHHMM(cRec.p31Value_Approved_Billing.ToString)
                        Else
                            .Text = cRec.p31Value_Approved_Billing.ToString
                        End If
                    Case BO.p33IdENUM.Kusovnik
                        .Text = cRec.p31Value_Approved_Billing.ToString
                    Case Else
                        .Visible = False
                End Select
            End With
            With CType(e.Item.FindControl("p31Value_Orig"), Label)
                If cRec.IsRecommendedHHMM() Then
                    .Text = _cTime.ShowAsHHMM(cRec.p31Value_Orig.ToString)
                Else
                    .Text = BO.BAS.FN(cRec.p31Value_Orig)
                End If
            End With
            If cRec.p72ID_AfterApprove = BO.p72IdENUM.ZahrnoutDoPausalu Then
                If cRec.p33ID = BO.p33IdENUM.Cas And (cRec.IsRecommendedHHMM() Or Len(cRec.p31Value_FixPrice.ToString) > 5) Then
                    CType(e.Item.FindControl("Value_Edit_FixPrice"), TextBox).Text = _cTime.ShowAsHHMM(cRec.p31Value_FixPrice.ToString)
                Else
                    CType(e.Item.FindControl("Value_Edit_FixPrice"), TextBox).Text = cRec.p31Value_FixPrice.ToString
                End If

                e.Item.FindControl("Value_Edit_FixPrice").Visible = True
                e.Item.FindControl("lblValue_Edit_FixPrice").Visible = True
            Else
                e.Item.FindControl("Value_Edit_FixPrice").Visible = False
                e.Item.FindControl("lblValue_Edit_FixPrice").Visible = False
            End If


            With CType(e.Item.FindControl("Rate_Edit"), Telerik.Web.UI.RadNumericTextBox)
                If cRec.p33ID = BO.p33IdENUM.Cas Or cRec.p33ID = BO.p33IdENUM.Kusovnik Then
                    .Value = cRec.p31Rate_Billing_Approved
                Else
                    .Visible = False
                End If
            End With

            CType(e.Item.FindControl("Currency"), Label).Text = cRec.j27Code_Billing_Orig
            CType(e.Item.FindControl("Client"), Label).Text = .ClientName
            CType(e.Item.FindControl("p41Name"), Label).Text = .p41Name
        End With
        CType(e.Item.FindControl("go2pid"), HyperLink).Attributes.Item("name") = cRec.PID.ToString


    End Sub

    Private Function InhaleAR(ri As RepeaterItem) As BO.p31WorksheetApproveInput
        Dim intP31ID As Integer = CInt(CType(ri.FindControl("p31id"), HiddenField).Value)
        Dim cRec As BO.p31Worksheet = Master.Factory.p31WorksheetBL.LoadTempRecord(intP31ID, ViewState("guid"))
        Dim cApprove As New BO.p31WorksheetApproveInput(cRec.PID, cRec.p33ID)
        With cApprove
            .p31Date = cRec.p31Date
            .GUID_TempData = ViewState("guid")
            .p72id = CType(CType(ri.FindControl("p72ID"), DropDownList).SelectedValue, BO.p72IdENUM)
            If .p72id = BO.p72IdENUM._NotSpecified Then
                .p71id = BO.p71IdENUM.Nic
            Else
                .p71id = cRec.p71ID
            End If
            .p31Text = CType(ri.FindControl("p31text"), TextBox).Text
            If ri.FindControl("Value_Edit").Visible Then
                Select Case cRec.p33ID
                    Case BO.p33IdENUM.Cas
                        Dim cT As New BO.clsTime
                        .Value_Approved_Billing = cT.ShowAsDec(CType(ri.FindControl("Value_Edit"), TextBox).Text)
                    Case BO.p33IdENUM.Kusovnik
                        .Value_Approved_Billing = BO.BAS.IsNullNum(CType(ri.FindControl("Value_Edit"), TextBox).Text)
                End Select
            Else
                .Value_Approved_Billing = cRec.p31Value_Approved_Billing
            End If
            If ri.FindControl("Rate_Edit").Visible Then
                .Rate_Billing_Approved = BO.BAS.IsNullNum(CType(ri.FindControl("Rate_Edit"), Telerik.Web.UI.RadNumericTextBox).Value)
            Else
                .Rate_Billing_Approved = cRec.p31Rate_Billing_Approved
            End If
            .VatRate_Approved = cRec.p31VatRate_Approved
            .Value_Approved_Internal = cRec.p31Value_Approved_Internal
            .Rate_Internal_Approved = cRec.p31Rate_Internal_Approved
            If .p72id = BO.p72IdENUM.ZahrnoutDoPausalu Then
                If cRec.p33ID = BO.p33IdENUM.Cas Then
                    Dim cT As New BO.clsTime
                    .p31Value_FixPrice = cT.ShowAsDec(CType(ri.FindControl("Value_Edit_FixPrice"), TextBox).Text)
                Else
                    .p31Value_FixPrice = BO.BAS.IsNullNum(CType(ri.FindControl("Value_Edit_FixPrice"), TextBox).Text)
                End If
            Else
                .p31Value_FixPrice = 0
            End If
        End With
        Return cApprove
    End Function

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "save"
                Dim cT As New BO.clsTime, xx As Integer = 0

                For Each ri As RepeaterItem In rp1.Items
                    xx += 1
                    Dim cApprove As BO.p31WorksheetApproveInput = InhaleAR(ri)
                    If Not Master.Factory.p31WorksheetBL.Validate_Before_Save_Approving(cApprove, True) Then
                        Master.Notify("#" & xx.ToString & ": " & Master.Factory.p31WorksheetBL.ErrorMessage, NotifyLevel.ErrorMessage)
                        Return
                    End If
                Next

                For Each ri As RepeaterItem In rp1.Items
                    Dim cApprove As BO.p31WorksheetApproveInput = InhaleAR(ri)
                    If Master.Factory.p31WorksheetBL.Save_Approving(cApprove, True) Then

                    End If

                Next
                ReloadPage()
            Case "goback"
                ReloadPage()
        End Select


    End Sub

    Private Sub ReloadPage()
        Response.Redirect(ViewState("backurl") & "&reloadonly=1", True)
    End Sub

    Private Sub cbxSort1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSort1.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving_batch_p31text-sort1", Me.cbxSort1.SelectedValue)
        RefreshData()
    End Sub

    Private Sub cbxSort2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSort2.SelectedIndexChanged
        Master.Factory.j03UserBL.SetUserParam("p31_approving_batch_p31text-sort2", Me.cbxSort2.SelectedValue)
        RefreshData()
    End Sub
End Class