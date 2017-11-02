Imports Telerik.Web.UI

Public Class freefields_checkboxlist
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As ModalForm

    Private Sub freefields_checkboxlist_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Master.DataPID = BO.BAS.IsNullInt(Request.Item("pid"))
            If Master.DataPID = 0 Then
                Master.StopPage("pid (x16id) is missing.")
            End If
            hidX18ID.Value = Request.Item("x18id")
            hidValue.Value = Request.Item("value")
            hidSourceControl.Value = Request.Item("ctl")

            If BO.BAS.IsNullInt(hidX18ID.Value) = 0 Then
                Master.StopPage("x18id is missing.")
            End If
            Master.AddToolbarButton("Sbalit vše", "collapse", , "Images/collapse.png")
            Master.AddToolbarButton("Rozbalit vše", "expand", , "Images/expand.png")
            Master.AddToolbarButton("Vyčistit", "clear", , "Images/sweep.png")
            Master.AddToolbarButton("OK", "ok", , "Images/ok.png")

            RefreshRecord()

            If hidValue.Value <> "" Then
                Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(hidValue.Value, ";")
                For Each strRow As String In lis
                    Dim a() As String = Split(strRow, "->")
                    Dim n As RadTreeNode = tr1.FindNodeByText(Trim(a(0)))
                    If Not n Is Nothing Then
                        If UBound(a) = 0 Then
                            n.Checked = True                            
                        End If
                        If UBound(a) = 1 Then
                            n = n.Nodes.FindNodeByText(Trim(a(1)))
                            If Not n Is Nothing Then
                                n.Checked = True
                                While Not n.ParentNode Is Nothing
                                    n.ParentNode.Expanded = True
                                    n = n.ParentNode
                                End While
                            End If

                        End If
                    End If
                   
                Next
            End If
        End If
    End Sub

    Private Sub RefreshRecord()
        Dim cRec As BO.x16EntityCategory_FieldSetting = Master.Factory.x18EntityCategoryBL.GetList_x16(BO.BAS.IsNullInt(hidX18ID.Value)).Where(Function(p) p.x16ID = Master.DataPID).First
        Dim lis As List(Of String) = BO.BAS.ConvertDelimitedString2List(cRec.x16DataSource, vbCrLf), intLastLevel As Integer = 0, x As Integer = 0
        With Me.tr1
            .Nodes.Clear()
            For Each strRow As String In lis
                Dim a() As String = Split(strRow, "|")
                Dim intLevel As Integer = BO.BAS.IsNullInt(a(0))
                Dim n As New RadTreeNode(a(1), a(1))
                n.Checkable = True
                n.Attributes.Item("level") = intLevel
                n.Attributes.Item("rowindex") = Right("0000" & x.ToString, 4)

                Dim nParent As RadTreeNode = Nothing
                If intLevel > 0 Then
                    If tr1.GetAllNodes.Where(Function(p) p.Attributes("level") = intLevel - 1).Count > 0 Then
                        nParent = tr1.GetAllNodes.Where(Function(p) p.Attributes("level") = intLevel - 1).OrderByDescending(Function(p) p.Attributes.Item("rowindex")).First
                        nParent.Nodes.Add(n)
                    End If
                End If
                If nParent Is Nothing Then
                    tr1.Nodes.Add(n)
                End If

                intLastLevel = BO.BAS.IsNullInt(a(0))
                x += 1
            Next
            For Each n In tr1.GetAllNodes
                If n.Nodes.Count > 0 Then
                    n.Checkable = False

                End If
            Next
        End With
    End Sub

    Private Sub _MasterPage_Master_OnToolbarClick(strButtonValue As String) Handles _MasterPage.Master_OnToolbarClick
        Select Case strButtonValue
            Case "collapse"
                tr1.CollapseAllNodes()
            Case "expand"
                tr1.ExpandAllNodes()
            Case "clear"
                For Each n In tr1.CheckedNodes
                    n.Checked = False
                Next
            Case "ok"
                Master.CloseAndRefreshParent(Me.hidSourceControl.Value, GetValue())
        End Select
       
    End Sub

    Private Function GetValue()
        Dim lis As New List(Of String)
        For Each n In tr1.GetAllNodes
            If n.Checked Then
                If n.ParentNode Is Nothing Then
                    lis.Add(n.Value)
                Else
                    lis.Add(n.ParentNode.Value & "->" & n.Value)
                End If

            End If
        Next
        Return String.Join(";", lis)
    End Function
End Class