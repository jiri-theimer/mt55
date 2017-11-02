Imports System.Web
Imports System.Web.Services

Public Class SearchWorksheet
    Public Property Project As String
    Public Property Text2Html As String
    Public Property p31Text As String
    Public Property PID As String
    Public Property p31Date As String
    Public Property FilterString As String

End Class
Public Class handler_search_worksheet
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "application/json"

        Dim factory As BL.Factory = Nothing
        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            context.Response.Write(" ")
            Return
        End If
        
        Dim strFilterString As String = context.Request.Item("term")
        Dim strP41ID As String = context.Request.Item("p41id")
        Dim intJ02ID As Integer = BO.BAS.IsNullInt(context.Request.Item("j02id"))
        If intJ02ID = 0 Then intJ02ID = factory.SysUser.j02ID

        Dim mq As New BO.myQueryP31
        mq.SearchExpression = strFilterString
        If strP41ID <> "" Then
            ''mq.p41ID = CInt(strP41ID)
        End If

        mq.TopRecordsOnly = 20
        mq.j02ID = intJ02ID
        If intJ02ID <> factory.SysUser.j02ID Then
            mq.SpecificQuery = BO.myQueryP31_SpecificQuery.AllowedForRead
        End If

        Dim lisP31 As IEnumerable(Of BO.p31Worksheet) = factory.p31WorksheetBL.GetList(mq)

        Dim lis As New List(Of SearchWorksheet)
        For Each cRec In lisP31
            Dim c As New SearchWorksheet
            With cRec
                c.p31Date = BO.BAS.FD(.p31Date)
                c.p31Text = .p31Text
                'c.Text2Html = .p31Text
                ''c.Text2Html = Replace(c.Text2Html, strFilterString, "<span style='color:red;'>" & strFilterString & "</span>", , , CompareMethod.Text)
                c.Text2Html = BO.BAS.CrLfText2Html(.p31Text)

                c.PID = .PID.ToString
                If .p28ID_Client > 0 Then
                    c.Project = .ClientName & " - " & .p41Name
                Else
                    c.Project = .p41Name
                End If
            End With


            c.FilterString = strFilterString

            lis.Add(c)
        Next



        Dim jss As New System.Web.Script.Serialization.JavaScriptSerializer
        Dim s As String = jss.Serialize(lis)
        context.Response.Write(s)

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class