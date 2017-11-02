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
Public Class notepad_service
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function LoadComboData(ByVal context As Object) As RadComboBoxItemData()
        Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        Dim filterString As String = (DirectCast(contextDictionary("filterstring"), String)).ToLower()
        Dim strFlag As String = (DirectCast(contextDictionary("flag"), String))
        If filterString.IndexOf("...") > 0 Then filterString = "" 'pokud jsou uvedeny 3 tečky, pak bráno jako nápovědný text pro hledání
        Dim strQryField As String = "", strQryValue As String = ""
        If contextDictionary.ContainsKey("qry_field") Then
            strQryField = (DirectCast(contextDictionary("qry_field"), String)).ToLower()
            strQryValue = DirectCast(contextDictionary("qry_value"), String)
        End If
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
        factory.j03UserBL.InhaleUserParams("handler_search_notepad-toprecs", "handler_search_notepad-bin")

        Dim result As List(Of RadComboBoxItemData) = Nothing
        
        Dim mq As New BO.myQueryO23(0)
        ''mq.SpecificQuery = BO.myQueryO23_SpecificQuery.AllowedForRead
        mq.SearchExpression = filterString
        mq.Closed = BO.BooleanQueryMode.FalseQuery
        mq.TopRecordsOnly = BO.BAS.IsNullInt(factory.j03UserBL.GetUserParam("handler_search_notepad-toprecs", "40"))
        If strQryField = "x23id" Then
            mq.x23ID = BO.BAS.IsNullInt(strQryValue)
        End If

        


        Dim lis As IEnumerable(Of BO.o23Doc) = factory.o23DocBL.GetList(mq)
        result = New List(Of RadComboBoxItemData)(lis.Count)
        Dim itemData As New RadComboBoxItemData()
        itemData.Enabled = False
        Select Case lis.Count
            Case 0
                If (Len(filterString) > 0 And Len(filterString) < 15) Or strQryField <> "" Then itemData.Text = "Ani jeden dokument pro zadanou podmínku."
            Case Is >= mq.TopRecordsOnly
                itemData.Text = String.Format("Nalezeno více než {0} dokumentů. Je třeba zpřesnit podmínku hledání.", mq.TopRecordsOnly.ToString)
            Case Else
                If filterString <> "" Then itemData.Text = String.Format("Počet nalezených dokumentů: {0}.", lis.Count.ToString)
        End Select
        If itemData.Text <> "" Then result.Add(itemData)


        For Each c As BO.o23Doc In lis
            itemData = New RadComboBoxItemData()

            itemData.Text = c.x23Name & ": " & c.o23Name & " (" & c.Owner & ")"


            If c.IsClosed Then
                itemData.Text = "<span class='radcomboitem_archive'>" & itemData.Text & "</span>"
            End If

            itemData.Value = c.PID.ToString
            result.Add(itemData)
        Next

        Return result.ToArray
    End Function

End Class