Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Telerik.Web.UI

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class fulltext_service
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function LoadComboData(ByVal context As Object) As RadComboBoxItemData()
        Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        Dim filterString As String = (DirectCast(contextDictionary("filterstring"), String))
        filterString = Trim(filterString)
        If filterString.IndexOf("...") > 0 Then filterString = "" 'pokud jsou uvedeny 3 tečky, pak bráno jako nápovědný text pro hledání
        Dim strFlag As String = ""
        If contextDictionary.ContainsKey("flag") Then
            strFlag = (DirectCast(contextDictionary("flag"), String))
        End If
        If Len(filterString) > 15 Then filterString = ""

        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            Dim nic As List(Of RadComboBoxItemData) = New List(Of RadComboBoxItemData)(1)
            Dim xx As New RadComboBoxItemData()
            xx.Text = "Nelze ověřit přihlášeného uživatele"
            nic.Add(xx)
            Return nic.ToArray
        End If

        

        Dim result As List(Of RadComboBoxItemData) = Nothing

        Dim input As New BO.FullTextQueryInput
        input.SearchExpression = filterString
        If strFlag = "p31" Then
            input.IncludeWorksheet = True
        Else
            Dim lisPars As New List(Of String)
            lisPars.Add("handler_search_fulltext-main")
            lisPars.Add("handler_search_fulltext-invoice")
            lisPars.Add("handler_search_fulltext-task")
            lisPars.Add("handler_search_fulltext-worksheet")
            lisPars.Add("handler_search_fulltext-doc")
            With factory.j03UserBL
                .InhaleUserParams(lisPars)
                input.IncludeMain = BO.BAS.BG(.GetUserParam("handler_search_fulltext-main", "1"))
                input.IncludeInvoice = BO.BAS.BG(.GetUserParam("handler_search_fulltext-invoice", "1"))
                input.IncludeDocument = BO.BAS.BG(.GetUserParam("handler_search_fulltext-doc", "0"))
                input.IncludeWorksheet = BO.BAS.BG(.GetUserParam("handler_search_fulltext-worksheet", "1"))
                input.IncludeTask = BO.BAS.BG(.GetUserParam("handler_search_fulltext-task", "0"))
            End With
        End If
        
        Dim id As New RadComboBoxItemData()
        id.Enabled = False

        Dim lis As List(Of BO.FullTextRecord) = Nothing
        If Len(filterString) < 3 Then
            result = New List(Of RadComboBoxItemData)(1)
            lis = New List(Of BO.FullTextRecord)
            id.Text = "Musíte napsat minimálně 3 znaky."
            result.Add(id)
            Return result.ToArray
        Else
            lis = factory.ftBL.FulltextSearch(input)
        End If
        result = New List(Of RadComboBoxItemData)(lis.Count)


        Select Case lis.Count
            Case 0
                If Len(filterString) > 0 Then id.Text = "Ani jeden výskyt pro zadanou podmínku."
            Case Is >= input.TopRecs
                id.Text = String.Format("Podmínce vyhovuje více než {0} výskytů. Je třeba zpřesnit podmínku hledání.", input.TopRecs)
            Case Else
                id.Text = String.Format("Počet nalezených výskytů: {0}.", lis.Count.ToString)
        End Select
        If id.Text <> "" Then result.Add(id)

        For Each c As BO.FullTextRecord In lis
            id = New RadComboBoxItemData()
            Dim bolNameIsValue As Boolean = False
            If c.RecValue = c.RecName Then bolNameIsValue = True

            If strFlag = "p31" Then
                'vyčistit html kvůli autopostback=true
                id.Text = "Worksheet úkon: " & c.RecName & " | " & c.RecValue
                id.Value = c.RecPid.ToString
            Else
                'standardní fulltext hledání
                c.RecValue = Replace(c.RecValue, filterString, "<span style='background-color:yellow;'>" & filterString & "</span>", , , CompareMethod.Text)
                If c.RecComment <> "" Then c.RecValue += "<br>" & Replace(c.RecComment, filterString, "<span style='background-color:yellow;'>" & c.RecComment & "</span>", , , CompareMethod.Text)

                If c.Prefix = "p31" Then
                    id.Text = "Worksheet úkon: " & c.RecName
                    id.Text += "<br><i>" & c.RecValue & "</i>"

                Else
                    If Not bolNameIsValue Then
                        id.Text = BO.BAS.GetX29EntityAlias(BO.BAS.GetX29FromPrefix(c.Prefix), False) & ": " & c.RecName
                        id.Text += "<br>" & c.Field & ": " & c.RecValue
                    Else
                        id.Text += c.Field & ": " & c.RecValue
                    End If

                End If
                id.Text += "<span style='color:red;margin-left:7px;'>" & BO.BAS.FD(c.RecDateInsert, True, True) & "</span><hr>"

                id.Value = c.Prefix & "|" & c.RecPid.ToString
            End If

            
            result.Add(id)
        Next
        Return result.ToArray

    End Function


End Class