Imports System.Web
Imports System.Web.Services

Public Class handler_calendar
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strOper As String = Trim(context.Request.Item("oper"))
       
        Dim factory As BL.Factory = Nothing
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(strLogin)
        End If
        If factory Is Nothing Then
            context.Response.Write(" ")
            Return
        End If


        Select Case strOper
            Case "save", ""
                Dim strO22ID As String = Trim(context.Request.Item("o22id"))
                Dim strEventID As String = Trim(context.Request.Item("eventID"))
                Dim strEventLink As String = Trim(context.Request.Item("eventLink"))


                Dim cRec As BO.o22Milestone = factory.o22MilestoneBL.Load(BO.BAS.IsNullInt(strO22ID))
                cRec.o22AppID = strEventID
                cRec.o22AppUrl = strEventLink
                If factory.o22MilestoneBL.Save(cRec, Nothing) Then
                    context.Response.Write("1")
                End If


        End Select
        

        

       

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class