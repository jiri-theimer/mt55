Imports System.Web
Imports System.Web.Services

Public Class handler_tempbox
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strP85ID As String = Trim(context.Request.Item("p85id"))
        Dim strField As String = Trim(context.Request.Item("field"))
        Dim strValue As String = Trim(context.Request.Item("value"))
        Dim strOper As String = Trim(context.Request.Item("oper"))
        Dim strGUID As String = Trim(context.Request.Item("guid"))
        If strField = "" Or strValue = "" Then
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

        Dim cRec As BO.p85TempBox = Nothing
        If strP85ID <> "" Then
            cRec = factory.p85TempBoxBL.Load(CInt(strP85ID))
        End If
        If strGUID <> "" Then
            cRec = factory.p85TempBoxBL.LoadByGUID(strGUID)
            If cRec Is Nothing Then
                cRec = New BO.p85TempBox
                cRec.p85GUID = strGUID
            End If
        End If
        If LCase(strField).IndexOf("key") > 0 Then
            BO.BAS.SetPropertyValue(cRec, strField, CInt(strValue))
        Else
            BO.BAS.SetPropertyValue(cRec, strField, strValue)
        End If

        If strOper = "save" Then
            If factory.p85TempBoxBL.Save(cRec) Then
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