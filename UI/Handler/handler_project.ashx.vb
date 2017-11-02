Imports System.Web
Imports System.Web.Services

Public Class handler_project
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            context.Response.Write(" ")
            Return
        End If

        Dim strOperation As String = Trim(context.Request.Item("oper")), strRetMsg As String = " ", intPID As Integer = BO.BAS.IsNullInt(context.Request.Item("pid"))
        If intPID = 0 Then
            context.Response.Write("Ny vstupu chybí ID projektu.")
            Return
        End If
        Select Case strOperation
            Case "favourite"
                If factory.j03UserBL.AppendOrRemoveFavouriteProject(factory.SysUser.PID, BO.BAS.ConvertPIDs2List(intPID), factory.p41ProjectBL.IsMyFavouriteProject(intPID)) Then
                    context.Response.Write("1")
                End If

            Case Else

        End Select

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class