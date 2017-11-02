Imports Telerik.Web.UI

Public Class o51_querybuilder
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub o51_querybuilderaspx_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Request.Item("prefix") = "" Then
                Master.StopPage("prefix is missing.")
            End If
            hidPrefix.Value = Request.Item("prefix")
            hidO51IDs.Value = Request.Item("o51ids")

            With Master
                .HeaderIcon = "Images/query_32.png"
                .HeaderText = "Filtrování podle štítků"
                .AddToolbarButton("Odškrtnout vše", "uncheck")
                .AddToolbarButton("Potvrdit", "ok", , "Images/ok.png")


            End With

            Dim lisCheckedVals As New List(Of String)
            With Master.Factory.j03UserBL
                .InhaleUserParams("o51_querybuilder-" & hidPrefix.Value)
                lisCheckedVals = BO.BAS.ConvertDelimitedString2List(.GetUserParam("o51_querybuilder-" & hidPrefix.Value))


            End With

            RenderNodes(hidPrefix.Value, "Štítky", True, lisCheckedVals)
            ''Select Case hidPrefix.Value
            ''    Case "p41"
            ''        RenderNodes("p28", "Štítky klienta projektu", False, lisCheckedVals)
            ''    Case "p31"
            ''        RenderNodes("p41", "Štítky projektu", False, lisCheckedVals)
            ''        RenderNodes("p28", "Štítky klienta projektu", False, lisCheckedVals)
            ''End Select


        End If
    End Sub

    Private Sub RenderNodes(strPrefix As String, strHeader As String, bolExpanded As Boolean, lisCheckedVals As List(Of String))
        Dim mq As New BO.myQuery
        Dim lis As IEnumerable(Of BO.o51Tag) = Master.Factory.o51TagBL.GetList(mq, strPrefix, BO.BooleanQueryMode.TrueQuery)

        Dim nParent As RadTreeNode = New RadTreeNode(strHeader, strPrefix)
        nParent.Checkable = False
        nParent.ImageUrl = "Images/folder.png"
        nParent.Expanded = bolExpanded
        tr1.Nodes.Add(nParent)

        For Each c In lis
            Dim n As New RadTreeNode(c.o51Name, strPrefix & "-" & c.PID.ToString)
            n.Checkable = True
            If c.IsClosed Then n.Font.Strikeout = True
            n.ToolTip = c.o51Description
            If c.o51BackColor <> "" Then
                n.BackColor = System.Drawing.ColorTranslator.FromHtml(c.o51BackColor)
            End If
            If c.o51ForeColor <> "" Then
                n.ForeColor = System.Drawing.ColorTranslator.FromHtml(c.o51ForeColor)
            End If
            If lisCheckedVals.Exists(Function(p) p = n.Value) Then
                n.Checked = True
                nParent.Expanded = True
            End If

            'n.CssClass = "badge_tag"
            nParent.Nodes.Add(n)
        Next

    End Sub


    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        If strButtonValue = "ok" Then
            Dim lis As New List(Of String)
            For Each n In Me.tr1.CheckedNodes
                lis.Add(n.Value)
            Next

            With Master.Factory.j03UserBL
                .SetUserParam("o51_querybuilder-" & hidPrefix.Value, String.Join(",", lis))

            End With
            Master.CloseAndRefreshParent("o51_querybuilder")
        End If
        If strButtonValue = "uncheck" Then
            tr1.UncheckAllNodes()
        End If
    End Sub
End Class