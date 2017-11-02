Public Class p91_change_vat
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_change_vat_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_change_vat"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderIcon = "Images/recalc_32.png"

                .AddToolbarButton("Uložit změny", "save", , "Images/save.png")

                Me.x15ID.DataSource = .Factory.ftBL.GetList_X15(New BO.myQuery)
                Me.x15ID.DataBind()

                Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                .HeaderText = "Převést fakturu na jinou DPH sazbu | " & cRec.p91Code
            End With


        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
            If Me.NewRate.Value Is Nothing Then
                Master.Notify("Musíte specifikovat sazbu.", NotifyLevel.WarningMessage)
                Return
            End If
            Dim dblNewVatRate As Double = BO.BAS.IsNullNum(Me.NewRate.Value)
            With Master.Factory.p91InvoiceBL
                If .ChangeVat(Master.DataPID, CType(CInt(Me.x15ID.SelectedValue), BO.x15IdEnum), dblNewVatRate) Then
                    Master.CloseAndRefreshParent("p91-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With
        End If
    End Sub

    Private Sub x15ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles x15ID.SelectedIndexChanged
        Me.NewRate.DbValue = Nothing

        Dim cRec As BO.p91Invoice = Master.Factory.p91InvoiceBL.Load(Master.DataPID)
        Dim x15id As BO.x15IdEnum = CType(CInt(Me.x15ID.SelectedValue), BO.x15IdEnum)
        Dim lis As IEnumerable(Of BO.p53VatRate) = Master.Factory.p53VatRateBL.GetList(New BO.myQuery).Where(Function(p) p.j27ID = cRec.j27ID And p.x15ID = x15id)
        If lis.Count > 0 Then
            Me.NewRate.Value = lis(0).p53Value
        End If
    End Sub
End Class