Imports Telerik.Web.UI

Namespace COM
    Public Class clsTreeview
        Private _tree As RadTreeView
        Public Sub New(ByVal tree As RadTreeView)
            _tree = tree
        End Sub

        Private Function OrezatPoctyVZavorkach(ByVal s) As String
            Dim a() As String = Split(s, "(")
            Return a(0)
        End Function

        Public Function GetSelectedPath(Optional ByVal bolVynechatPoctyVzavorkach As Boolean = False) As String
            With _tree

                Dim n As Telerik.Web.UI.RadTreeNode = .SelectedNode
                If n Is Nothing Then Return ""

                Dim s As String = IIf(bolVynechatPoctyVzavorkach, OrezatPoctyVZavorkach(n.Text), n.Text)
                While Not n Is Nothing
                    n = n.ParentNode
                    If Not n Is Nothing Then
                        s = IIf(bolVynechatPoctyVzavorkach, OrezatPoctyVZavorkach(n.Text), n.Text) & " -> " & s
                        n.Expanded = True
                    End If
                End While

                Return s

            End With
        End Function
    End Class
End Namespace

