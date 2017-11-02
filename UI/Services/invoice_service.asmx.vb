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
Public Class invoice_service
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
        factory.j03UserBL.InhaleUserParams("handler_search_invoice-toprecs")

        Dim result As List(Of RadComboBoxItemData) = Nothing

        Dim mq As New BO.myQueryP91
        mq.SearchExpression = filterString
        mq.TopRecordsOnly = BO.BAS.IsNullInt(factory.j03UserBL.GetUserParam("handler_search_invoice-toprecs", "20"))

        Select Case strFlag
            Case "searchbox"
                mq.SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
                mq.Closed = BO.BooleanQueryMode.NoQuery
            Case "search4o23"
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                mq.SpecificQuery = BO.myQueryP91_SpecificQuery.AllowedForRead
            Case Else
        End Select
        If strQryField = "p92id" Then
            mq.p92ID = BO.BAS.IsNullInt(strQryValue)
        End If

        Dim lis As IEnumerable(Of BO.p91Invoice) = factory.p91InvoiceBL.GetList(mq)
        result = New List(Of RadComboBoxItemData)(lis.Count)
        Dim itemData As New RadComboBoxItemData()
        itemData.Enabled = False
        Select Case lis.Count
            Case 0
                If Len(filterString) > 0 And Len(filterString) < 15 Then itemData.Text = "Ani jeden projekt pro zadanou podmínku."
            Case Is >= mq.TopRecordsOnly
                itemData.Text = String.Format("Nalezeno více než {0} faktur. Je třeba zpřesnit hledání nebo si zvýšit počet vypisovaných faktur.", mq.TopRecordsOnly.ToString)
            Case Else
                If filterString <> "" Then itemData.Text = String.Format("Počet nalezených faktur: {0}.", lis.Count.ToString)
        End Select
        If itemData.Text <> "" Then result.Add(itemData)
        For Each cRec As BO.p91Invoice In lis
            itemData = New RadComboBoxItemData()
            With cRec
                itemData.Text = .p91Code
                If .p91Client = "" Then
                    itemData.Text += " " & .p28Name
                Else
                    itemData.Text += " " & .p91Client
                End If
                itemData.Text += " (" & BO.BAS.FN(.p91Amount_TotalDue) & " " & .j27Code & ")"
                If .p91IsDraft Then itemData.Text += " DRAFT"

                itemData.Value = .PID.ToString
            End With

            result.Add(itemData)
        Next

        Return result.ToArray
    End Function

End Class