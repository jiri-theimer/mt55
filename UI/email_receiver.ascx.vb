Public Class email_receiver
    Inherits System.Web.UI.UserControl

    Public Overridable Property Width() As String
        Get
            Return cbx1.Width.Value.ToString
        End Get
        Set(ByVal value As String)
            cbx1.Width = Unit.Parse(value)

        End Set
    End Property
    Public Overridable Property BackColor() As System.Drawing.Color
        Get
            Return cbx1.BackColor
        End Get
        Set(ByVal value As System.Drawing.Color)
            cbx1.BackColor = value

        End Set
    End Property
    Public Property Text() As String
        Get
            Return cbx1.Text
        End Get
        Set(ByVal value As String)
            cbx1.Text = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cbx1.OnClientItemsRequesting = Me.ClientID & "_OnClientItemsRequesting"
    End Sub

End Class