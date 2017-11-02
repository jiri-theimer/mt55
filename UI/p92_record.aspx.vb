Public Class p92_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p92_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení typu faktury"

                Me.j27ID.DataSource = .Factory.ftBL.GetList_J27(New BO.myQuery)
                Me.j27ID.DataBind()
                Me.x15ID.DataSource = .Factory.ftBL.GetList_X15(New BO.myQuery)
                Me.x15ID.DataBind()
                Me.x15ID.ChangeItemText("", "--Nepřevádět úkony faktury na jednotnou DPH sazbu--")
                Me.j17ID.DataSource = .Factory.j17CountryBL.GetList()
                Me.j17ID.DataBind()
                Dim lisX38 As IEnumerable(Of BO.x38CodeLogic) = .Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p91Invoice)
                Me.x38ID.DataSource = lisX38.Where(Function(p) p.x38IsDraft = False)
                Me.x38ID.DataBind()
                Me.x38ID_Draft.DataSource = lisX38.Where(Function(p) p.x38IsDraft = True)
                Me.x38ID_Draft.DataBind()
                Me.x31ID_Invoice.DataSource = .Factory.x31ReportBL.GetList(New BO.myQuery).Where(Function(p) p.x29ID = BO.x29IdEnum.p91Invoice)
                Me.x31ID_Invoice.DataBind()
                Me.x31ID_Attachment.DataSource = Me.x31ID_Invoice.DataSource
                Me.x31ID_Attachment.DataBind()
                Me.x31ID_Letter.DataSource = Me.x31ID_Invoice.DataSource
                Me.x31ID_Letter.DataBind()
                Me.p93ID.DataSource = .Factory.p93InvoiceHeaderBL.GetList(New BO.myQuery)
                Me.p93ID.DataBind()
                Me.p98ID.DataSource = .Factory.p98Invoice_Round_Setting_TemplateBL.GetList(New BO.myQuery)
                Me.p98ID.DataBind()
                Me.p80ID.DataSource = .Factory.p80InvoiceAmountStructureBL.GetList(New BO.myQuery)
                Me.p80ID.DataBind()
                Me.b01ID.DataSource = Master.Factory.b01WorkflowTemplateBL.GetList().Where(Function(p) p.x29ID = BO.x29IdEnum.p91Invoice)
                Me.b01ID.DataBind()

            End With

            RefreshRecord()

            If Master.DataPID = 0 Then

            End If

            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p92InvoiceType = Master.Factory.p92InvoiceTypeBL.Load(Master.DataPID)
        With cRec
            Me.p92Name.Text = .p92Name
            If .p92InvoiceType = BO.p92InvoiceTypeENUM.ClientInvoice Then
                Me.x15ID.SelectedValue = BO.BAS.IsNullInt(.x15ID).ToString
                Me.j17ID.SelectedValue = .j17ID.ToString
            End If

            Me.j27ID.SelectedValue = .j27ID.ToString

            Me.x38ID.SelectedValue = .x38ID.ToString
            Me.x38ID_Draft.SelectedValue = .x38ID_Draft.ToString
            Me.x31ID_Attachment.SelectedValue = .x31ID_Attachment.ToString
            Me.x31ID_Invoice.SelectedValue = .x31ID_Invoice.ToString
            Me.x31ID_Letter.SelectedValue = .x31ID_Letter.ToString
            Me.p93ID.SelectedValue = .p93ID.ToString
            Me.p98ID.SelectedValue = .p98ID.ToString
            Me.p80ID.SelectedValue = .p80ID.ToString
            Me.b01ID.SelectedValue = .b01ID.ToString
            Me.p92InvoiceDefaultText1.Text = .p92InvoiceDefaultText1
            Me.p92InvoiceDefaultText2.Text = .p92InvoiceDefaultText2
            Me.p92ReportConstantText.Text = .p92ReportConstantText
            Me.p92Ordinary.Value = .p92Ordinary
            basUI.SelectRadiolistValue(Me.p92InvoiceType, CInt(.p92InvoiceType).ToString)

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp


        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p92InvoiceTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p92-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p92InvoiceTypeBL
            Dim cRec As BO.p92InvoiceType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p92InvoiceType)
            With cRec
                .x38ID = BO.BAS.IsNullInt(Me.x38ID.SelectedValue)
                .x38ID_Draft = BO.BAS.IsNullInt(Me.x38ID_Draft.SelectedValue)
                .x15ID = BO.BAS.IsNullInt(Me.x15ID.SelectedValue)
                .j27ID = BO.BAS.IsNullInt(Me.j27ID.SelectedValue)
                .j17ID = BO.BAS.IsNullInt(Me.j17ID.SelectedValue)
                .p98ID = BO.BAS.IsNullInt(Me.p98ID.SelectedValue)
                .p80ID = BO.BAS.IsNullInt(Me.p80ID.SelectedValue)
                .x31ID_Invoice = BO.BAS.IsNullInt(Me.x31ID_Invoice.SelectedValue)
                .x31ID_Attachment = BO.BAS.IsNullInt(Me.x31ID_Attachment.SelectedValue)
                .x31ID_Letter = BO.BAS.IsNullInt(Me.x31ID_Letter.SelectedValue)
                .p93ID = BO.BAS.IsNullInt(Me.p93ID.SelectedValue)
                .b01ID = BO.BAS.IsNullInt(Me.b01ID.SelectedValue)
                .p92Name = Me.p92Name.Text
                .p92Ordinary = BO.BAS.IsNullInt(Me.p92Ordinary.Value)
                .p92InvoiceType = CType(Me.p92InvoiceType.SelectedValue, BO.p92InvoiceTypeENUM)
                .p92InvoiceDefaultText1 = Trim(Me.p92InvoiceDefaultText1.Text)
                .p92InvoiceDefaultText2 = Trim(Me.p92InvoiceDefaultText2.Text)
                .p92ReportConstantText = Trim(Me.p92ReportConstantText.Text)

                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p92-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub p92_record_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Dim b As Boolean = True
        If Me.p92InvoiceType.SelectedValue = "2" Then b = False
        trX15ID.Visible = b
        trJ17ID.Visible = b
    End Sub
End Class