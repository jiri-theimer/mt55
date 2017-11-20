



'Imports Aspose.Words




Public Class pokus
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site
    'Private fileFormatProvider As IFormatProvider

    Private _lis As List(Of MyListItem)
    Private Class MyListItem
        Public Property Value As Integer
        Public Property Text As String
    End Class

    Private Sub pokus_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        
    End Sub


   
    
    
    


   
End Class