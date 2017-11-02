Imports System.Web
Imports System.Web.Services

Public Class handler_default_page
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strURL As String = Trim(context.Request.Item("url"))
        If strURL = "" Then
            context.Response.Write(" ")
            Return
        End If
        Dim a() As String = Split(strURL, "/")
        strURL = a(UBound(a))
        ''If Left(strURL, 1) = "/" Then strURL = Right(strURL, Len(strURL) - 1)

        Dim factory As BL.Factory = Nothing
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        Else
            context.Response.Write(" ")
            Return
        End If
        Dim cRec As BO.j03User = factory.j03UserBL.Load(factory.SysUser.PID)
        cRec.j03Aspx_PersonalPage = strURL
        If factory.j03UserBL.Save(cRec) Then
            context.Response.Write("1")
        End If

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class