Imports Telerik.Web.UI

Public Class panelmenu
    Inherits System.Web.UI.UserControl

    Public ReadOnly Property IsEmpty As Boolean
        Get
            If menu1.Items.Count = 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public Property Skin As String
        Get
            Return menu1.Skin
        End Get
        Set(value As String)
            menu1.Skin = value
        End Set
    End Property

    Public Overridable Property Width() As String
        Get
            Return menu1.Width.Value.ToString
        End Get
        Set(ByVal value As String)
            menu1.Width = Unit.Parse(value)

        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Public Sub AddSeparator(strParentNodeValue As String)
        Dim n As New RadPanelItem()
        n.IsSeparator = True
        n.Text = "-"
       
        If strParentNodeValue = "" Then
            menu1.Items.Add(n)
        Else
            Dim nParent As RadPanelItem = menu1.FindItemByValue(strParentNodeValue)
            If Not nParent Is Nothing Then
                nParent.Items.Add(n)
            End If
        End If
    End Sub
    Public Overloads Sub AddItem(n As RadPanelItem, Optional strParentNodeValue As String = "")
        If strParentNodeValue = "" Then
            menu1.Items.Add(n)
        Else
            Dim nParent As RadPanelItem = menu1.FindItemByValue(strParentNodeValue)
            If Not nParent Is Nothing Then
                nParent.Items.Add(n)
            End If
        End If
    End Sub
    Public Overloads Sub AddItem(ByVal strText As String, ByVal strValue As String, Optional ByVal strNavigateURL As String = "", Optional ByVal strParentNodeValue As String = "", Optional ByVal strImageURL As String = "", Optional ByVal strToolTip As String = "", Optional strTarget As String = "")
        Dim n As New RadPanelItem(strText, strNavigateURL)
        n.Value = strValue
        n.ImageUrl = strImageURL
        n.ToolTip = strToolTip
        n.Target = strTarget
        n.SelectedCssClass = "panelmenu_selectedrow"

        If strParentNodeValue = "" Then
            menu1.Items.Add(n)
        Else

            Dim nParent As RadPanelItem = menu1.FindItemByValue(strParentNodeValue)
            If Not nParent Is Nothing Then
                ''nParent.Font.Bold = True
                nParent.Items.Add(n)
            End If
        End If

    End Sub

    Public Property SelectedValue() As String
        Get
            If menu1.SelectedItem Is Nothing Then
                Return ""
            Else
                Return menu1.SelectedItem.Value
            End If
        End Get
        Set(ByVal value As String)
            Dim n As RadPanelItem = menu1.FindItemByValue(value)
            If Not n Is Nothing Then
                n.Selected = True
                ''If Not n.Expanded And n.Items.Count > 0 Then
                ''    n.Expanded = True
                ''End If
                n.ExpandParentItems()
            End If
        End Set
    End Property


    Public Function FindItemByValue(ByVal strValue As String) As RadPanelItem
        Return menu1.FindItemByValue(strValue)
    End Function

    Public Sub SelectNodeIndex(intIndex As Integer)
        menu1.Items(intIndex).Selected = True

    End Sub


    Public Function GetSelectedPath() As String
        If menu1.SelectedItem Is Nothing Then
            Return ""
        Else
            Dim n As RadPanelItem = menu1.SelectedItem
            Dim s As String = n.Text
            If TypeOf (n.Owner) Is RadPanelItem Then
                n = DirectCast(n.Owner, RadPanelItem)
                s = n.Text & " -> " & s

                If TypeOf (n.Owner) Is RadPanelItem Then
                    n = DirectCast(n.Owner, RadPanelItem)
                    s = n.Text & " -> " & s
                End If
            End If
            
            Return s
        End If

    End Function

    Public Sub Clear()
        menu1.Items.Clear()

    End Sub

    Public Sub ExpandAll()
        menu1.ExpandMode = PanelBarExpandMode.MultipleExpandedItems
        For Each c In menu1.GetAllItems.Where(Function(p) p.Items.Count > 0)
            If Not c.Expanded Then c.Expanded = True

        Next
    End Sub
    Public Sub CollapseAll()
        menu1.AllowCollapseAllItems = True
        menu1.CollapseAllItems()
    End Sub

End Class