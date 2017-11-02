Public Class pageheader
    Inherits System.Web.UI.UserControl

    
    Public Property Text() As String
        Get
            Return Me.lblHeader.Text

        End Get

        Set(ByVal value As String)

            Me.lblHeader.Text = value

        End Set

    End Property
    Public Property Image() As String
        Get
            Return img1.ImageUrl

        End Get

        Set(ByVal value As String)

            img1.ImageUrl = value

        End Set

    End Property
    Public Property IsInForm As Boolean
        Get
            'If panContainer.Style("background-color") = "#D0D0D0" Then
            '    Return False
            'Else
            '    Return True
            'End If
            Return False
        End Get

        Set(ByVal value As Boolean)
            'If value Then
            '    panContainer.Style("background-color") = "threedface"
            'Else
            '    panContainer.Style("background-color") = "#D0D0D0"
            'End If
            

        End Set
    End Property




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Image = "" Then
            img1.Visible = False
        Else
            img1.Visible = True
        End If

    End Sub

End Class