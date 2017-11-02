Imports System.Web
Imports System.Web.Services

Public Class handler_p45_project
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strGUID As String = Trim(context.Request.Item("guid"))
        Dim intP85ID As Integer = BO.BAS.IsNullInt(context.Request.Item("p85id"))
        Dim strField As String = Trim(context.Request.Item("field"))
        Dim dblValue As Double = BO.BAS.IsNullNum(context.Request.Item("value"))
        If strGUID = "" Or intP85ID = 0 Or strField = "" Then
            context.Response.Write(" ")
            Return
        End If

        Dim factory As New BL.Factory(Nothing)

        Dim cTemp As BO.p85TempBox = factory.p85TempBoxBL.Load(intP85ID)
        Select Case strField
            Case "p85FreeFloat01"
                cTemp.p85FreeFloat01 = dblValue
            Case "p85FreeFloat02"
                cTemp.p85FreeFloat02 = dblValue
            Case "p85OtherKey2"
                cTemp.p85OtherKey2 = dblValue
        End Select
       
        If factory.p85TempBoxBL.Save(cTemp) Then
            context.Response.Write("1")
        End If

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class