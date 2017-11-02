Public Class select_field
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Property CurrentPrefix As String
        Get
            Return Me.hidPrefix.Value
        End Get
        Set(value As String)
            Me.hidPrefix.Value = value
        End Set
    End Property

    Private Sub select_field_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.AddToolbarButton("OK", "ok", , "Images/ok.png")
            Me.CurrentPrefix = Request.Item("prefix")
            ViewState("flag") = Request.Item("flag")
            If ViewState("flag") = "" Then ViewState("flag") = "select_field"
            Dim lisAllCols As List(Of BO.GridColumn) = Master.Factory.j70QueryTemplateBL.ColumnsPallete(BO.BAS.GetX29FromPrefix(Me.CurrentPrefix))
            If Request.Item("pivot") = "1" Then
                lisAllCols = lisAllCols.Where(Function(p) p.Pivot_SelectSql <> "").ToList
            End If
            For Each c In lisAllCols.Where(Function(p) p.TreeGroup <> "").Select(Function(p) p.TreeGroup).Distinct
                Dim n As New Telerik.Web.UI.RadTreeNode(c, c)
                n.ImageUrl = "Images/folder.png"
                tree1.Nodes.Add(n)
            Next
            For Each c In lisAllCols
                Dim n As New Telerik.Web.UI.RadTreeNode(c.ColumnHeader, c.ColumnName)

                Select Case c.ColumnType
                    Case BO.cfENUM.DateTime, BO.cfENUM.DateTime
                        n.ImageUrl = "Images/type_datetime.png"
                    Case BO.cfENUM.DateOnly
                        n.ImageUrl = "Images/type_date.png"
                    Case BO.cfENUM.Numeric, BO.cfENUM.Numeric2, BO.cfENUM.Numeric0
                        n.ImageUrl = "Images/type_number.png"
                    Case BO.cfENUM.AnyString
                        n.ImageUrl = "Images/type_text.png"
                    Case BO.cfENUM.Checkbox
                        n.ImageUrl = "Images/type_checkbox.png"
                End Select
                If c.ColumnName.IndexOf("Free") > 0 Then n.ForeColor = Drawing.Color.Green
                If c.TreeGroup = "" Then
                    tree1.Nodes.Add(n)
                Else
                    tree1.FindNodeByValue(c.TreeGroup).Nodes.Add(n)
                End If

            Next
            tree1.ExpandAllNodes()
            If Request.Item("value") <> "" Then
                If Not tree1.FindNodeByValue(Request.Item("value")) Is Nothing Then
                    tree1.FindNodeByValue(Request.Item("value")).Selected = True
                End If
            End If
        End If
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            With tree1
                If .SelectedNode Is Nothing Then
                    Master.Notify("Musíte vybrat položku.", NotifyLevel.WarningMessage) : Return
                End If
                If .SelectedNode.Value = "" Or .SelectedNode.ImageUrl = "Images/folder.png" Then
                    Master.Notify("Musíte vybrat položku.", NotifyLevel.WarningMessage) : Return
                End If
            End With

            Master.CloseAndRefreshParent(ViewState("flag"), Me.tree1.SelectedNode.Value)
        End If
    End Sub
End Class