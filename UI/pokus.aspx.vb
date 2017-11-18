



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


   
    Private Sub pokus_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.j07ID.SelectedValue = "2"
    End Sub

    
    

    
    Private Sub cmdPokus_Click(sender As Object, e As EventArgs) Handles cmdPokus.Click
      

        Master.Notify("j07ID: " & Me.j07ID.SelectedValue & ", klient: " & Me.p28ID.Value)

    End Sub

    Private Sub FillList(qry As IEnumerable(Of Object))
        For Each c In qry
            Dim item As New MyListItem
            item.Value = c.pid
            item.Text = c.item("j07Name")

            _lis.Add(item)
        Next



    End Sub
End Class