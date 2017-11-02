Public Class p90_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p90_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ff1.Factory = Master.Factory
        tags1.Factory = Master.Factory
        If Not Page.IsPostBack Then
            ViewState("guid_p82") = BO.BAS.GetGUID()
            With Master

                .HeaderIcon = "Images/proforma_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))

                Me.j27ID.DataSource = .Factory.ftBL.GetList_J27()
                Me.j27ID.DataBind()
                Me.p89ID.DataSource = .Factory.p89ProformaTypeBL.GetList(New BO.myQuery)
                Me.p89ID.DataBind()

            End With


            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0
                ''Me.p90Amount_Billed.Value = 0
                ''Me.p90DateBilled.SelectedDate = Nothing
                ''Me.p82Code.Visible = False
                ''link_x31_dpp.Visible = False
                Me.p90Code.Visible = False : Me.link_x31id.Visible = False
                ''Me.p90TextDPP.Text = ""
            End If

        End If
    End Sub

    Private Sub RefreshRecord()
        Handle_FF()
        If Master.DataPID = 0 Then
            Dim cRecLast As BO.p90Proforma = Master.Factory.p90ProformaBL.LoadMyLastCreated()
            If Not cRecLast Is Nothing Then
                Me.j27ID.SelectedValue = cRecLast.j27ID.ToString
                Me.p89ID.SelectedValue = cRecLast.p89ID.ToString
                Me.p90VatRate.Value = cRecLast.p90VatRate
            End If
            Me.p90Date.SelectedDate = Today
            Me.p90DateMaturity.SelectedDate = Today.AddDays(10)
            Me.j02ID_Owner.Value = Master.Factory.SysUser.j02ID.ToString
            Me.j02ID_Owner.Text = Master.Factory.SysUser.PersonDesc
            If Request.Item("p28id") <> "" Then
                Me.p28ID.Value = Request.Item("p28id")
                Me.p28ID.Text = Master.Factory.GetRecordCaption(BO.x29IdEnum.p28Contact, CInt(Request.Item("p28id")), True)
            End If
            Return
        End If

        Dim cRec As BO.p90Proforma = Master.Factory.p90ProformaBL.Load(Master.DataPID)
        Dim cP89 As BO.p89ProformaType = Master.Factory.p89ProformaTypeBL.Load(cRec.p89ID)
        With cRec
            Master.HeaderText = String.Format("Záznam zálohové faktury [{0}]", .p90Code)
            Me.p90Code.Text = .p90Code
            Me.p90Code.NavigateUrl = "javascript:recordcode()"
            Me.j27ID.SelectedValue = .j27ID.ToString
            Me.p89ID.SelectedValue = .p89ID.ToString
            Me.p28ID.Value = .p28ID.ToString
            Me.p28ID.Text = .p28Name
            Me.j02ID_Owner.Value = .j02ID_Owner.ToString
            Me.j02ID_Owner.Text = .Owner
            Me.p90text1.Text = .p90Text1
            Me.p90text2.Text = .p90Text2
            ''Me.p90TextDPP.Text = .p90TextDPP
            Me.p90Date.SelectedDate = .p90Date
            If Not BO.BAS.IsNullDBDate(.p90DateMaturity) Is Nothing Then
                Me.p90DateMaturity.SelectedDate = .p90DateMaturity
            End If
            ''If Not BO.BAS.IsNullDBDate(.p90DateBilled) Is Nothing Then
            ''    Me.p90DateBilled.SelectedDate = .p90DateBilled
            ''End If

            Me.p90Amount.Value = BO.BAS.IsNullNum(.p90Amount)
            ''Me.p90Amount_Billed.Value = BO.BAS.IsNullNum(.p90Amount_Billed)
            Me.p90Amount_Vat.Value = BO.BAS.IsNullNum(.p90Amount_Vat)
            Me.p90Amount_WithoutVat.Value = BO.BAS.IsNullNum(.p90Amount_WithoutVat)
            Me.p90VatRate.Value = BO.BAS.IsNullNum(.p90VatRate)

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp

            link_x31id.NavigateUrl = "javascript:report(" & cP89.x31ID.ToString & ")"
            ''If .p82Code <> "" Then
            ''    p82Code.Visible = True : p82Code.Text = .p82Code : Me.p82Code.NavigateUrl = "javascript:dppcode(" & .p82ID.ToString & ")"
            ''    link_x31_dpp.Visible = True : link_x31_dpp.NavigateUrl = "javascript:report(" & cP89.x31ID_Payment.ToString & ")"
            ''Else
            ''    Me.p82Code.Visible = False
            ''    link_x31_dpp.Visible = False
            ''End If
        End With

        Dim lisP82 As IEnumerable(Of BO.p82Proforma_Payment) = Master.Factory.p90ProformaBL.GetList_p82(Master.DataPID)
        Master.Factory.p85TempBoxBL.Truncate(ViewState("guid_p82"))
        For Each c In lisP82
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = ViewState("guid_p82")
                .p85DataPID = c.PID
                .p85FreeText01 = c.p82Code
                .p85FreeDate01 = c.p82Date
                .p85FreeFloat01 = c.p82Amount
                .p85Message = c.p82Text
                .p85OtherKey1 = cP89.x31ID_Payment
                If Not Page.IsPostBack Then
                    If Master.IsRecordClone Then
                        .p85DataPID = 0
                    End If
                End If
            End With
            Master.Factory.p85TempBoxBL.Save(cTemp)
        Next
        RefreshTempP82()
        tags1.RefreshData(cRec.PID)
    End Sub
    Private Sub RefreshTempP82()
        Me.rpP82.DataSource = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p82"))
        Me.rpP82.DataBind()

    End Sub
    Private Sub Handle_FF()
        With RadTabStrip1.FindTabByValue("ff")
            If .Visible Then
                Dim fields As List(Of BO.FreeField) = Master.Factory.x28EntityFieldBL.GetListWithValues(BO.x29IdEnum.p90Proforma, Master.DataPID, BO.BAS.IsNullInt(Me.p89ID.SelectedValue))
                Dim lisX20X18 As IEnumerable(Of BO.x20_join_x18) = Master.Factory.x18EntityCategoryBL.GetList_x20_join_x18(BO.x29IdEnum.p90Proforma, BO.BAS.IsNullInt(Me.p89ID.SelectedValue))
                ff1.FillData(fields, lisX20X18, "p90Proforma_FreeField", Master.DataPID)
                .Text = String.Format(.Text, ff1.FieldsCount, ff1.TagsCount)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p90ProformaBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p90-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        SaveTempP82()
        With Master.Factory.p90ProformaBL
            Dim cRec As BO.p90Proforma = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p90Proforma)
            With cRec
                .p89ID = BO.BAS.IsNullInt(Me.p89ID.SelectedValue)
                .j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
                .p28ID = BO.BAS.IsNullInt(Me.p28ID.Value)
                .j02ID_Owner = BO.BAS.IsNullInt(Me.j02ID_Owner.Value)

                .p90Text1 = Me.p90text1.Text
                .p90Text2 = Me.p90text2.Text
                ''.p90TextDPP = Me.p90TextDPP.Text
                .p90Date = Me.p90Date.SelectedDate
                .p90DateMaturity = BO.BAS.IsNullDBDate(Me.p90DateMaturity.SelectedDate)

                ''.p90DateBilled = BO.BAS.IsNullDBDate(Me.p90DateBilled.SelectedDate)

                .p90Amount = BO.BAS.IsNullNum(Me.p90Amount.Value)
                ''.p90Amount_Billed = BO.BAS.IsNullNum(Me.p90Amount_Billed.Value)
                .p90Amount_Vat = BO.BAS.IsNullNum(Me.p90Amount_Vat.Value)
                .p90Amount_WithoutVat = BO.BAS.IsNullNum(Me.p90Amount_WithoutVat.Value)
                .p90VatRate = BO.BAS.IsNullNum(Me.p90VatRate.Value)

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With

            Dim lisFF As List(Of BO.FreeField) = Me.ff1.GetValues()

            Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p82"), True)
            Dim lisP82 As New List(Of BO.p82Proforma_Payment)
            For Each cTMP In lisTEMP
                Dim c As New BO.p82Proforma_Payment
                With cTMP
                    c.SetPID(cTMP.p85DataPID)
                    c.IsSetAsDeleted = .p85IsDeleted
                    c.p82Amount = .p85FreeFloat01
                    c.p82Date = .p85FreeDate01
                    c.p82Text = .p85Message
                End With
                lisP82.Add(c)
            Next

            If .Save(cRec, lisFF, lisP82) Then
                Master.DataPID = .LastSavedPID
                Master.Factory.o51TagBL.SaveBinding("p90", Master.DataPID, tags1.Geto51IDs())
                Master.Factory.x18EntityCategoryBL.SaveX19Binding(BO.x29IdEnum.p90Proforma, Master.DataPID, ff1.GetTags(), ff1.GetX20IDs)
                Master.CloseAndRefreshParent("p90-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub cmdCalc1_Click(sender As Object, e As EventArgs) Handles cmdCalc1.Click
        Dim dbl1 As Double = BO.BAS.IsNullNum(Me.p90Amount_WithoutVat.Value)
        Dim rate As Double = BO.BAS.IsNullNum(Me.p90VatRate.Value)
        Me.p90Amount_Vat.Value = dbl1 * rate / 100
        Me.p90Amount.Value = Me.p90Amount_Vat.Value + dbl1
    End Sub

    Private Sub cmdCalc2_Click(sender As Object, e As EventArgs) Handles cmdCalc2.Click
        Dim dbl1 As Double = BO.BAS.IsNullNum(Me.p90Amount.Value)
        Dim rate As Double = BO.BAS.IsNullNum(Me.p90VatRate.Value)
        Me.p90Amount_WithoutVat.Value = dbl1 / (1 + rate / 100)
        Me.p90Amount_Vat.Value = dbl1 - Me.p90Amount_WithoutVat.Value
    End Sub

    Private Sub rpP82_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rpP82.ItemCommand
        SaveTempP82()
        Dim cRec As BO.p85TempBox = Master.Factory.p85TempBoxBL.Load(BO.BAS.IsNullInt(e.CommandArgument))
        If e.CommandName = "delete" Then
            If cRec.p85DataPID > 0 Then
                Dim lisTest As IEnumerable(Of BO.p99Invoice_Proforma) = Master.Factory.p90ProformaBL.GetList_p99(0, 0, cRec.p85DataPID)
                If lisTest.Count > 0 Then
                    Master.Notify("Tuto úhradu nelze odstranit, protože již byla spárována s daňovou fakturou.", NotifyLevel.WarningMessage)
                    Return
                End If
            End If

            If Master.Factory.p85TempBoxBL.Delete(cRec) Then
                RefreshTempP82()
            End If
        End If
    End Sub
    Private Sub SaveTempP82()
        Dim lisTEMP As IEnumerable(Of BO.p85TempBox) = Master.Factory.p85TempBoxBL.GetList(ViewState("guid_p82"))
        For Each ri As RepeaterItem In rpP82.Items
            Dim intP85ID As Integer = BO.BAS.IsNullInt(CType(ri.FindControl("p85id"), HiddenField).Value)
            Dim cRec As BO.p85TempBox = lisTEMP.Where(Function(p) p.PID = intP85ID)(0)
            With cRec
                .p85FreeDate01 = CType(ri.FindControl("p82Date"), Telerik.Web.UI.RadDatePicker).SelectedDate
                .p85FreeFloat01 = CType(ri.FindControl("p82Amount"), Telerik.Web.UI.RadNumericTextBox).Value
                .p85Message = CType(ri.FindControl("p82Text"), TextBox).Text
            End With
            Master.Factory.p85TempBoxBL.Save(cRec)
        Next
    End Sub

    Private Sub rpP82_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rpP82.ItemDataBound
        Dim cRec As BO.p85TempBox = CType(e.Item.DataItem, BO.p85TempBox)

        With CType(e.Item.FindControl("del"), ImageButton)
            .CommandArgument = cRec.PID.ToString
            .CommandName = "delete"
        End With
        With cRec
            CType(e.Item.FindControl("p85id"), HiddenField).Value = .PID.ToString
            CType(e.Item.FindControl("p82Date"), Telerik.Web.UI.RadDatePicker).SelectedDate = .p85FreeDate01
            CType(e.Item.FindControl("p82Amount"), Telerik.Web.UI.RadNumericTextBox).Value = .p85FreeFloat01
            CType(e.Item.FindControl("p82Text"), TextBox).Text = .p85Message
            If .p85FreeText01 <> "" Then
                CType(e.Item.FindControl("p82Code"), HyperLink).Text = .p85FreeText01
                CType(e.Item.FindControl("p82Code"), HyperLink).NavigateUrl = "javascript:dppcode(" & .p85DataPID.ToString & ")"
            Else
                e.Item.FindControl("p82Code").Visible = False
            End If
            If .p85OtherKey1 <> 0 Then
                CType(e.Item.FindControl("link_x31_dpp"), HyperLink).NavigateUrl = "javascript:report_dpp(" & .p85OtherKey1.ToString & "," & .p85DataPID.ToString & ")"
            Else
                e.Item.FindControl("link_x31_dpp").Visible = False
            End If
        End With
    End Sub

    Private Sub cmdAddP82_Click(sender As Object, e As EventArgs) Handles cmdAddP82.Click
        SaveTempP82()
        Dim cRec As New BO.p85TempBox()
        cRec.p85GUID = ViewState("guid_p82")
        cRec.p85FreeDate01 = Today
        Master.Factory.p85TempBoxBL.Save(cRec)
        RefreshTempP82()
    End Sub
End Class