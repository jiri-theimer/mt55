Imports System.Web
Imports System.Web.Services

Public Class handler_userparam
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strKey As String = Trim(context.Request.Item("x36key"))
        Dim strValue As String = Trim(context.Request.Item("x36value"))
        Dim strOper As String = Trim(context.Request.Item("oper"))
        If strKey = "" Or (strKey = "" And strValue = "" And strOper = "set") Or strOper = "" Then
            context.Response.Write(" ")
            Return
        End If

        Dim factory As BL.Factory = Nothing
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            context.Response.Write(" ")
            Return
        End If

        If strOper = "get" Then
            context.Response.Write(factory.j03UserBL.GetUserParam(strKey))
        End If
        If strOper = "set" Then
            If factory.j03UserBL.SetUserParam(strKey, strValue) Then
                context.Response.Write("1")
            End If
        End If

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class