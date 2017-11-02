Public Class x18_items
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub x18_items_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            With Master
                .HeaderText = "Položky kategorie"
                .DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
                If .DataPID = 0 Then .StopPage("pid missing")

                If Not .Factory.TestPermission(BO.x53PermValEnum.GR_X18_Admin) Then
                    If Not .Factory.x67EntityRoleBL.TestEntityRolePermission(BO.x29IdEnum.x18EntityCategory, .DataPID, 1, False) Then
                        'v roli má oprávnění být vlastníkem všech úkolů
                        .StopPage("Nemáte oprávnění spravovat položky u této kategorie (typu dokumentu).")
                    End If
                End If

            End With


            RefreshRecord()


        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.x18EntityCategory = Master.Factory.x18EntityCategoryBL.Load(Master.DataPID)
        With cRec
            If .x23ID = 0 Then Master.StopPage("Zdroj položek není definován!")
            Me.x18Name.Text = .x18Name
            Master.HeaderText += " | " & .x18Name
            Me.hidX23ID.Value = .x23ID.ToString

        End With
      
        RefreshItems()


    End Sub
    Private Sub RefreshItems()
        rpO23.DataSource = Master.Factory.o23DocBL.GetList(New BO.myQueryO23(CInt(Me.hidX23ID.Value)))
        rpO23.DataBind()

    End Sub
End Class