Public Class select_doctype
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub select_doctype_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ViewState("oper") = Request.Item("oper")
            hidOcas.Value = basUI.GetCompleteQuerystring(Request)
            hidMasterPrefix.Value = Request.Item("masterprefix")
            hidMasterPID.Value = Request.Item("masterpid")


            Dim x29id As BO.x29IdEnum = BO.x29IdEnum._NotSpecified
            If hidMasterPrefix.Value <> "" Then
                x29id = BO.BAS.GetX29FromPrefix(hidMasterPrefix.Value)
                Me.EntityName.Text = BO.BAS.GetX29EntityAlias(x29id, False)
                If hidMasterPID.Value <> "" Then
                    Me.EntityRecord.Text = Master.Factory.GetRecordCaption(x29id, BO.BAS.IsNullInt(hidMasterPID.Value))
                    panMasterEntity.Visible = True
                End If
            End If

            Dim lisX18 As IEnumerable(Of BO.x18EntityCategory) = Master.Factory.x18EntityCategoryBL.GetList(New BO.myQuery, x29id).Where(Function(p) p.x18IsManyItems = True)
            Me.x18ID.DataSource = lisX18
            Me.x18ID.DataBind()

          
            With Master
                .AddToolbarButton("Pokračovat", "continue", 0, "Images/continue.png")
            End With
        End If
    End Sub

    
    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "continue" Then
            If Me.x18ID.SelectedItem Is Nothing Then
                Master.Notify("Musíte vybrat typ dokumentu!", NotifyLevel.WarningMessage)
                Return
            End If
            Dim strURL As String = "o23_record.aspx?pid=0&x18id=" & Me.x18ID.SelectedValue
            If hidOcas.Value <> "" Then
                strURL += "&" & hidOcas.Value
            End If
            Server.Transfer(strURL, False)
        End If
    End Sub
End Class