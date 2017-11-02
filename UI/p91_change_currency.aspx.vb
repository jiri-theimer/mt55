Public Class p91_change_currency
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub p91_change_currency_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        Master.HelpTopicID = "p91_change_currency"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")
                .HeaderIcon = "Images/recalc_32.png"

                .AddToolbarButton("Uložit změny", "save", , "Images/save.png")

                Me.j27ID.DataSource = .Factory.ftBL.GetList_J27(New BO.myQuery)
                Me.j27ID.DataBind()
                Me.j27ID.Items.Insert(0, "")
                Dim cRec As BO.p91Invoice = .Factory.p91InvoiceBL.Load(.DataPID)
                .HeaderText = "Převést fakturu na jinou měnu | " & cRec.p91Code
            End With


        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "save" Then
        
            With Master.Factory.p91InvoiceBL
                If .ChangeCurrency(Master.DataPID, BO.BAS.IsNullInt(Me.j27ID.SelectedValue)) Then
                    Master.CloseAndRefreshParent("p91-save")
                Else
                    Master.Notify(.ErrorMessage, NotifyLevel.ErrorMessage)
                End If
            End With
        End If
    End Sub
End Class