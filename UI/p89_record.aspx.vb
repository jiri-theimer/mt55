Public Class p89_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p89_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Nastavení typu zálohy"

              
                Dim lisX38 As IEnumerable(Of BO.x38CodeLogic) = Master.Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p90Proforma)
                Me.x38ID.DataSource = lisX38.Where(Function(p) p.x38IsDraft = False)
                Me.x38ID.DataBind()
                Me.x38ID_Draft.DataSource = lisX38.Where(Function(p) p.x38IsDraft = True)
                Me.x38ID_Draft.DataBind()
                Me.x38ID_Payment.DataSource = Master.Factory.x38CodeLogicBL.GetList(BO.x29IdEnum.p82Proforma_Payment)
                Me.x38ID_Payment.DataBind()
                Dim lisX31 As IEnumerable(Of BO.x31Report) = .Factory.x31ReportBL.GetList(New BO.myQuery)
                Me.x31ID.DataSource = lisX31.Where(Function(p) p.x29ID = BO.x29IdEnum.p90Proforma)
                Me.x31ID.DataBind()

                Me.x31ID_Payment.DataSource = lisX31.Where(Function(p) p.x29ID = BO.x29IdEnum.p82Proforma_Payment)
                Me.x31ID_Payment.DataBind()
                
                Me.p93ID.DataSource = .Factory.p93InvoiceHeaderBL.GetList(New BO.myQuery)
                Me.p93ID.DataBind()
            End With

            RefreshRecord()

          
            If Master.IsRecordClone Then
                Master.DataPID = 0
            End If


        End If
    End Sub

    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then
            Return
        End If

        Dim cRec As BO.p89ProformaType = Master.Factory.p89ProformaTypeBL.Load(Master.DataPID)
        With cRec
            Me.p89Name.Text = .p89Name
            
            Me.x38ID.SelectedValue = .x38ID.ToString
            Me.x38ID_Draft.SelectedValue = .x38ID_Draft.ToString
            Me.x38ID_Payment.SelectedValue = .x38ID_Payment.ToString
            Me.x31ID.SelectedValue = .x31ID.ToString
            Me.x31ID_Payment.SelectedValue = .x31ID_Payment.ToString

            Me.p93ID.SelectedValue = .p93ID.ToString

            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
            Master.Timestamp = .Timestamp


        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p89ProformaTypeBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("p89-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p89ProformaTypeBL
            Dim cRec As BO.p89ProformaType = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p89ProformaType)
            With cRec
                .x38ID = BO.BAS.IsNullInt(Me.x38ID.SelectedValue)
                .x38ID_Draft = BO.BAS.IsNullInt(Me.x38ID_Draft.SelectedValue)
                .x38ID_Payment = BO.BAS.IsNullInt(Me.x38ID_Payment.SelectedValue)
                .j27ID = Master.Factory.x35GlobalParam.GetValueInteger("j27ID_Invoice", 2)
                .x31ID = BO.BAS.IsNullInt(Me.x31ID.SelectedValue)
                .x31ID_Payment = BO.BAS.IsNullInt(Me.x31ID_Payment.SelectedValue)
                .p93ID = BO.BAS.IsNullInt(Me.p93ID.SelectedValue)
                .p89Name = Me.p89Name.Text


                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With



            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p89-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class