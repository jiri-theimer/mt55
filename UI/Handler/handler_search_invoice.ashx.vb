Imports System.Web
Imports System.Web.Services

Public Class handler_search_invoice
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
        'factory.j03UserBL.InhaleUserParams("handler_search_p-toprecs", "handler_search_project-bin")

        'context.Response.Write("Hello World!")
        Dim strFilterString As String = context.Request.Item("term")
        Dim strP28ID As String = context.Request.Item("p28id")
        Dim mq As New BO.myQueryP91
        mq.SearchExpression = strFilterString
        If strP28ID <> "" Then
            mq.p28ID = CInt(strP28ID)
        End If
        'If factory.j03UserBL.GetUserParam("handler_search_project-bin", "0") = "1" Then
        '    mq.Closed = BO.BooleanQueryMode.NoQuery
        'End If

        mq.TopRecordsOnly = 10
        Dim lisP91 As IEnumerable(Of BO.p91Invoice) = factory.p91InvoiceBL.GetList(mq)
        Dim lis As New List(Of BO.SearchBoxItem)
        Dim c As New BO.SearchBoxItem
        Select Case lisP91.Count
            Case 0
                c.ItemText = "Ani jedna faktura pro zadanou podmínku."
            Case Is >= mq.TopRecordsOnly
                c.ItemText = String.Format("Nalezeno více než {0} faktur.<br>Je třeba zpřesnit hledání nebo si zvýšit počet vypisovaných faktur.", mq.TopRecordsOnly.ToString)
            Case Else
                c.ItemText = String.Format("Počet nalezených faktur: {0}.", lisP91.Count.ToString)
        End Select
        c.FilterString = strFilterString : lis.Add(c)
        For Each cP91 In lisP91
            c = New BO.SearchBoxItem
            With cP91
                c.ItemText = .p91Code + " - " + .p91Client
                c.ItemText += " (" & BO.BAS.FN(.p91Amount_TotalDue) & " " & .j27Code & ")"
                c.ItemComment = .p91Text1
                c.PID = .PID.ToString
                If .IsClosed Then c.Closed = "1"
                If .p91IsDraft Then c.Draft = "1"
                c.ToolTip = .p92Name & ": " & BO.BAS.FD(.p91DateSupply)
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