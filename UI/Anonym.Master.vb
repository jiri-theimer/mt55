Public Class Anonym
    Inherits System.Web.UI.MasterPage

    Public ReadOnly Property IsMobileDevice As Boolean
        Get
            If Me.hidIsMobile.Value = "" Then
                Me.hidIsMobile.Value = "0"
                If basUI.DetectIfMobileDefice(Request) Then
                    Me.hidIsMobile.Value = "1"
                End If
            End If
            If Me.hidIsMobile.Value = "1" Then Return True Else Return False
        End Get
    End Property

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        If Me.IsMobileDevice Then
            css1.Href = "~/Styles/SiteMobile_v2.css"
        Else
            
            css1.Href = "~/Styles/Site_v11.css"
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
        If Not Page.IsPostBack Then
            lblBuild.Text = "Version " & BO.ASS.GetUIVersion() & " | .NET framework: " & BO.ASS.GetFrameworkVersion()
            
        End If
    End Sub

End Class