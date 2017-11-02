Public Class p80_record
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalDataRecord

    Private Sub p80_record_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .neededPermission = BO.x53PermValEnum.GR_Admin
                .HeaderIcon = "Images/settings_32.png"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                .HeaderText = "Struktura rozpisu částky na faktuře"


                
            End With

            RefreshRecord()

            If Master.IsRecordClone Then
                Master.DataPID = 0

            End If


        End If
    End Sub
    Private Sub RefreshRecord()
        If Master.DataPID = 0 Then Return

        Dim cRec As BO.p80InvoiceAmountStructure = Master.Factory.p80InvoiceAmountStructureBL.Load(Master.DataPID)
        With cRec
            Me.p80Name.Text = .p80Name
           
            Me.p80IsTimeSeparate.Checked = .p80IsTimeSeparate
            Me.p80IsFeeSeparate.Checked = .p80IsFeeSeparate
            Me.p80IsExpenseSeparate.Checked = .p80IsExpenseSeparate
            Master.Timestamp = .Timestamp


            Master.InhaleRecordValidity(.ValidFrom, .ValidUntil, .DateInsert)
        End With
    End Sub
    Private Sub _MasterPage_Master_OnDelete() Handles _MasterPage.Master_OnDelete
        With Master.Factory.p80InvoiceAmountStructureBL
            If .Delete(Master.DataPID) Then
                Master.DataPID = 0
                Master.CloseAndRefreshParent("80-delete")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub

    Private Sub _MasterPage_Master_OnRefresh() Handles _MasterPage.Master_OnRefresh
        RefreshRecord()
    End Sub

    Private Sub _MasterPage_Master_OnSave() Handles _MasterPage.Master_OnSave
        With Master.Factory.p80InvoiceAmountStructureBL
            Dim cRec As BO.p80InvoiceAmountStructure = IIf(Master.DataPID <> 0, .Load(Master.DataPID), New BO.p80InvoiceAmountStructure)
            With cRec
                .p80Name = Me.p80Name.Text
                .p80IsTimeSeparate = Me.p80IsTimeSeparate.Checked
                .p80IsFeeSeparate = Me.p80IsFeeSeparate.Checked
                .p80IsExpenseSeparate = Me.p80IsExpenseSeparate.Checked
                .ValidFrom = Master.RecordValidFrom
                .ValidUntil = Master.RecordValidUntil
            End With
            If .Save(cRec) Then
                Master.DataPID = .LastSavedPID
                Master.CloseAndRefreshParent("p80-save")
            Else
                Master.Notify(.ErrorMessage, 2)
            End If
        End With
    End Sub
End Class