Imports System.Web
Imports System.Web.Services

Public Class handler_p66_plan
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Dim strD1 As String = context.Request.Item("d1")
        Dim strD2 As String = context.Request.Item("d2")
        Dim intJ02ID As Integer = BO.BAS.IsNullInt(context.Request.Item("j02id"))
        Dim strColName As String = Trim(context.Request.Item("colName"))
        Dim dblValue As Double = BO.BAS.IsNullNum(context.Request.Item("value"))

        Dim strGUID As String = Trim(context.Request.Item("guid"))
        If strGUID = "" Or intJ02ID = 0 Or strColName = "" Or strD1 = "" Then
            context.Response.Write(" ")
            Return
        End If
        Dim datLimit1 As Date = BO.BAS.ConvertString2Date(strD1)
        Dim d As Date = datLimit1
        For i As Integer = 0 To 11
            d = datLimit1.AddMonths(i)
            If strColName = "Col" & (i + 1).ToString Then Exit For
        Next

        Dim factory As New BL.Factory(Nothing)


        Dim lis As IEnumerable(Of BO.p85TempBox) = factory.p85TempBoxBL.GetList(strGUID).Where(Function(p) p.p85OtherKey1 = intJ02ID And p.p85FreeDate01 = d)
        Dim cTemp As New BO.p85TempBox
        If lis.Count > 0 Then cTemp = lis(0)
        cTemp.p85GUID = strGUID
        cTemp.p85OtherKey1 = intJ02ID
        cTemp.p85FreeFloat01 = dblValue

        cTemp.p85FreeDate01 = d
        cTemp.p85FreeDate02 = d.AddMonths(1).AddDays(-1)
        If cTemp.p85FreeText01 = "" Then
            cTemp.p85FreeText01 = factory.j02PersonBL.Load(intJ02ID).FullNameDesc
        End If
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