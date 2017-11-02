Public Class p31_record_approve
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p31_record_approve_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        approve1.Factory = Master.Factory

        If Not Page.IsPostBack Then
            ViewState("p91id") = BO.BAS.IsNullInt(Request.Item("p91id"))

            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")

                Dim cRec As BO.p31Worksheet = .Factory.p31WorksheetBL.Load(.DataPID)
                approve1.InhaleRecord(cRec, True)

                If ViewState("p91id") > 0 Then
                    approve1.CommandSaveText = "Vložit úkon do faktury"
                End If
            End With

        End If
    End Sub

    Private Sub approve1_AfterSave(ByRef strErr As String) Handles approve1.AfterSave
        If ViewState("p91id") > 0 Then
            Dim pids As New List(Of Integer)
            pids.Add(Master.DataPID)
            If Master.Factory.p31WorksheetBL.AppendToInvoice(ViewState("p91id"), pids) Then
                Master.CloseAndRefreshParent("p31-append2invoice")
            End If
            
        End If
        
    End Sub
End Class