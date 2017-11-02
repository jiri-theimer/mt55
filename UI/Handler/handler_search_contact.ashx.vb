Imports System.Web
Imports System.Web.Services


Public Class handler_search_contact
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
        Dim strFilterString As String = context.Request.Item("term"), strFO As String = context.Request.Item("fo")
        
        Dim mq As New BO.myQueryP28
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Select Case strFO
            Case "p28Name"
                mq.ColumnFilteringExpression = "a.p28Name LIKE '%" & strFilterString & "%'"    'hledat pouze podle názvu klienta
            Case "p28RegID"
                mq.ColumnFilteringExpression = "a.p28RegID LIKE '%" & strFilterString & "%'"    'hledat pouze podle IČ
            Case "p28VatID"
                mq.ColumnFilteringExpression = "a.p28VatID LIKE '%" & strFilterString & "%'"    'hledat pouze podle DIČ            
            Case Else
                mq.SearchExpression = strFilterString
        End Select

        'If factory.j03UserBL.GetUserParam("handler_search_project-bin", "0") = "1" Then
        '    mq.Closed = BO.BooleanQueryMode.NoQuery
        'End If

        mq.TopRecordsOnly = 20
        Dim lisP28 As IEnumerable(Of BO.p28Contact) = factory.p28ContactBL.GetList(mq)
        Dim lis As New List(Of BO.SearchBoxItem)
        Dim c As New BO.SearchBoxItem
        If strFO = "" Then
            Select Case lisP28.Count
                Case 0
                    c.ItemText = "Ani jeden klient pro zadanou podmínku."
                Case Is >= mq.TopRecordsOnly
                    c.ItemText = String.Format("Nalezeno více než {0} klientů.<br>Je třeba zpřesnit hledání nebo si zvýšit počet vypisovaných klientů.", mq.TopRecordsOnly.ToString)
                Case Else
                    c.ItemText = String.Format("Počet nalezených klientů: {0}.", lisP28.Count.ToString)
            End Select
            c.FilterString = strFilterString : lis.Add(c)
        End If
        For Each cP28 In lisP28
            c = New BO.SearchBoxItem
            With cP28
                If .p28CompanyShortName = "" Then
                    c.ItemText = .p28Name
                Else
                    c.ItemText = .p28CompanyShortName & " - " & .p28CompanyName
                End If
                c.ItemText += " (" & .p28Code & ")"
                If .p28SupplierFlag = BO.p28SupplierFlagENUM.NotClientNotSupplier Then c.Italic = "1"

                Select Case strFO
                    Case "p28RegID" : c.ItemText = .p28RegID & " - " & c.ItemText
                    Case "p28VatID" : c.ItemText = .p28VatID & " - " & c.ItemText
                End Select
                c.PID = .PID.ToString
                If .IsClosed Then c.Closed = "1"
                If .p28IsDraft Then c.Draft = "1"
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