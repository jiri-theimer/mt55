Public Class _Default
    Inherits System.Web.UI.Page
    Protected WithEvents _MasterPage As Site

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _MasterPage = Me.Master
        
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            hidURL.Value = FindDefaultPage()

            Master.SiteMenuValue = "dashboard"

        End If
    End Sub

    Private Function FindDefaultPage() As String
        With Master.Factory.SysUser
            ''If .j03IsSystemAccount Then
            ''    Response.Redirect("~/sys/membership_framework.aspx")
            ''End If
            If Request.Item("quitmobile") = "" Then
                If .j03MobileForwardFlag = BO.j03MobileForwardFlagENUM.Auto Then
                    If basUI.DetectIfMobileDefice(Request) Then
                        Return "mobile_start.aspx"
                    End If
                End If
            End If

            'a
            If .PersonalPage <> "" Then
                If .PersonalPage.IndexOf(".aspx") > 0 Then
                    If LCase(.PersonalPage) = "default.aspx" Then
                        Return "j03_mypage_greeting.aspx"
                    Else
                        Return .PersonalPage
                    End If
                Else
                    Return "report_framework.aspx?defpage=1"
                End If
            Else
                Return "j03_mypage_greeting.aspx"
            End If

        End With
    End Function


    
End Class