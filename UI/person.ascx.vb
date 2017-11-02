Public Class person
    Inherits System.Web.UI.UserControl

    Public Event AutoPostBack_SelectedIndexChanged(ByVal NewValue As String, ByVal OldValue As String)

    Public Overridable Property Width() As String
        Get
            Return cbx1.Width.Value.ToString
        End Get
        Set(ByVal value As String)
            cbx1.Width = Unit.Parse(value)

        End Set
    End Property
    Public Overridable Property AutoPostBack() As Boolean
        Get
            Return cbx1.AutoPostBack
        End Get
        Set(ByVal value As Boolean)
            cbx1.AutoPostBack = value

        End Set
    End Property
    Public Overrides Sub Focus()
        MyBase.Focus()
        cbx1.Focus()


    End Sub
    Public Property Flag As String
        Get
            Return hidflag.Value
        End Get
        Set(value As String)
            hidflag.Value = value
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
    Public Property Value() As String
        Get
            Return cbx1.SelectedValue
        End Get
        Set(ByVal value As String)
            cbx1.SelectedValue = value
        End Set
    End Property

    Public Property J03ID_System() As String
        Get
            Return hidj03id_system.Value
        End Get
        Set(ByVal value As String)
            hidj03id_system.Value = value
        End Set
    End Property

    Public Property IsSearchPossible() As Boolean
        Get
            Return cbx1.Enabled
        End Get
        Set(ByVal value As Boolean)
            cbx1.Enabled = value
        End Set
    End Property

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cbx1.OnClientItemsRequesting = Me.ClientID & "_OnClientItemsRequesting"
    End Sub


    Private Sub cbx1_SelectedIndexChanged(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles cbx1.SelectedIndexChanged
        RaiseEvent AutoPostBack_SelectedIndexChanged(e.Value, e.OldValue)

    End Sub

    Public Property RadCombo As Telerik.Web.UI.RadComboBox
        Get
            Return Me.cbx1
        End Get
        Set(value As Telerik.Web.UI.RadComboBox)
            Me.cbx1 = value
        End Set
    End Property
End Class