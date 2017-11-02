Imports Telerik.Web.UI

Public Class TreeMenu
    Inherits System.Web.UI.UserControl
    Public Event MenuItemExpand(ByVal strValue As String)
    
    Public ReadOnly Property IsEmpty As Boolean
        Get
            If treeMenu.Nodes.Count = 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Property onClientNodeClicked As String
        Get
            Return treeMenu.OnClientNodeClicked
        End Get
        Set(ByVal value As String)
            treeMenu.OnClientNodeClicked = value
        End Set
    End Property
    Public Property onClientNodeDoubleClicked As String
        Get
            Return treeMenu.OnClientDoubleClick
        End Get
        Set(ByVal value As String)
            treeMenu.OnClientDoubleClick = value
        End Set
    End Property
    Public Property SingleExpandPath As Boolean
        Get
            Return treeMenu.SingleExpandPath
        End Get
        Set(value As Boolean)
            treeMenu.SingleExpandPath = value
        End Set
    End Property
    Public Property Skin As String
        Get
            Return treeMenu.Skin
        End Get
        Set(value As String)
            treeMenu.Skin = value
        End Set
    End Property
    Public Property IsSupportToggle As Boolean
        Get
            If treeMenu.OnClientNodeClicking <> "" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(value As Boolean)
            If value Then
                treeMenu.OnClientNodeClicking = "toggle_onClientNodeClicking"
            Else
                treeMenu.OnClientNodeClicking = ""
            End If
        End Set
    End Property
    


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Public Overloads Function AddItem(n As RadTreeNode, Optional strParentNodeValue As String = "") As RadTreeNode
        If strParentNodeValue = "" Then
            treeMenu.Nodes.Add(n)
        Else
            Dim nParent As Telerik.Web.UI.RadTreeNode = treeMenu.FindNodeByValue(strParentNodeValue)
            If Not nParent Is Nothing Then
                nParent.Nodes.Add(n)
            End If
        End If
        Return n
    End Function
    Public Overloads Function AddItem(ByVal strText As String, ByVal strValue As String, Optional ByVal strNavigateURL As String = "", Optional ByVal strParentNodeValue As String = "", Optional ByVal strImageURL As String = "", Optional ByVal strToolTip As String = "", Optional strTarget As String = "") As RadTreeNode
        Dim n As New Telerik.Web.UI.RadTreeNode(strText, strValue, strNavigateURL)
        n.ImageUrl = strImageURL
        n.ToolTip = strToolTip
        n.Target = strTarget
        If strParentNodeValue = "" Then
            treeMenu.Nodes.Add(n)
        Else
            Dim nParent As Telerik.Web.UI.RadTreeNode = treeMenu.FindNodeByValue(strParentNodeValue)
            If Not nParent Is Nothing Then
                nParent.Nodes.Add(n)
            Else
                treeMenu.Nodes.Add(n)
            End If
        End If
        Return n

    End Function


    Public Property SelectedValue() As String
        Get
            Return treeMenu.SelectedValue
        End Get
        Set(ByVal value As String)
            Try
                Dim n As Telerik.Web.UI.RadTreeNode = treeMenu.FindNodeByValue(value)
                If Not n Is Nothing Then
                    n.Selected = True
                    n.ExpandParentNodes()
                    If n.Nodes.Count > 0 Then n.Expanded = True
                End If


            Catch ex As Exception

            End Try
        End Set
    End Property


    Public Function FindItemByValue(ByVal strValue As String) As RadTreeNode
        Return treeMenu.FindNodeByValue(strValue)
    End Function


    Public Sub SelectNodeIndex(intIndex As Integer)
        treeMenu.Nodes(intIndex).Selected = True

    End Sub


    Public Function GetSelectedPath() As String
        Dim cT As New COM.clsTreeview(Me.treeMenu)
        Return cT.GetSelectedPath()
    End Function

    


    Private Sub treeMenu_NodeExpand(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles treeMenu.NodeExpand
        RaiseEvent MenuItemExpand(e.Node.Value)

    End Sub

    Public Sub Clear()
        treeMenu.Nodes.Clear()

    End Sub

    Public Sub ExpandAll()
        treeMenu.ExpandAllNodes()
    End Sub
    Public Sub CollapseAll()
        treeMenu.CollapseAllNodes()
    End Sub
End Class