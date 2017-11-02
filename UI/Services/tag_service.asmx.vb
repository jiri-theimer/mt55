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
Public Class tag_service
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function LoadTokenData(ByVal context As Object) As AutoCompleteBoxData
        'Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        'Dim filterString As String = DirectCast(context, Dictionary(Of String, Object))("filterstring").ToString()
        Dim filterString As String = DirectCast(context, Dictionary(Of String, Object))("Text").ToString()

        Dim strPrefix As String = DirectCast(context, Dictionary(Of String, Object))("prefix").ToString()

        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            Dim strLogin As String = HttpContext.Current.User.Identity.Name
            factory = New BL.Factory(, strLogin)
        End If
        If factory Is Nothing Then
            Dim nic As List(Of AutoCompleteBoxItemData) = New List(Of AutoCompleteBoxItemData)(1)
            Dim xx As New AutoCompleteBoxItemData()
            xx.Text = "Nelze ověřit přihlášeného uživatele"
            nic.Add(xx)
            Dim resNic As New AutoCompleteBoxData()
            resNic.Items = nic.ToArray()
            Return resNic
        End If

        Dim result As List(Of AutoCompleteBoxItemData) = Nothing

        Dim mq As New BO.myQuery
        If Left(filterString, 6) = "top100" Then
            Dim a() As String = Split(filterString, "-")
            strPrefix = a(1)
            mq.SearchExpression = ""
            mq.TopRecordsOnly = 100
        Else
            mq.SearchExpression = filterString
            mq.TopRecordsOnly = 50
        End If



        Dim lis As IEnumerable(Of BO.o51Tag) = factory.o51TagBL.GetList(mq, strPrefix, BO.BooleanQueryMode.NoQuery)
        result = New List(Of AutoCompleteBoxItemData)(lis.Count)
        Dim itemData As New AutoCompleteBoxItemData()
        itemData.Enabled = False

        Select Case lis.Count
            Case 0
                If (Len(filterString) > 0 And Len(filterString) < 15) Then itemData.Text = "Ani jeden štítek pro zadanou podmínku."
                If filterString = "top100" Then itemData.Text = "Žádný štítek v nabídce."
            Case Is >= mq.TopRecordsOnly
                itemData.Text = String.Format("Nalezeno více než {0} štítků. Je třeba zpřesnit podmínku hledání.", mq.TopRecordsOnly.ToString)
            Case Else
                If filterString <> "" Then itemData.Text = String.Format("Počet nalezených štítků: {0}.", lis.Count.ToString)
        End Select
        If itemData.Text <> "" Then result.Add(itemData)


        For Each c As BO.o51Tag In lis
            itemData = New AutoCompleteBoxItemData()
            itemData.Text = c.o51Name
            itemData.Value = c.PID.ToString
            result.Add(itemData)
        Next
        Dim res As New AutoCompleteBoxData()
        res.Items = result.ToArray()

        Return res
    End Function

    <WebMethod()> _
    Public Function LoadComboData(ByVal context As Object) As RadComboBoxItemData()
        Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        Dim filterString As String = (DirectCast(contextDictionary("filterstring"), String))
        Dim strPrefix As String = (DirectCast(contextDictionary("prefix"), String))
        If filterString.IndexOf("...") > 0 Then filterString = "" 'pokud jsou uvedeny 3 tečky, pak bráno jako nápovědný text pro hledání

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

        Dim mq As New BO.myQuery
        mq.SearchExpression = filterString
        mq.TopRecordsOnly = 50
        mq.Closed = BO.BooleanQueryMode.NoQuery
        If Not factory.SysUser.IsAdmin Then
            mq.j02ID_Owner = factory.SysUser.j02ID
        End If



        Dim lis As IEnumerable(Of BO.o51Tag) = factory.o51TagBL.GetList(mq, strPrefix, BO.BooleanQueryMode.NoQuery)
        result = New List(Of RadComboBoxItemData)(lis.Count)
        Dim itemData As New RadComboBoxItemData()
        itemData.Enabled = False
        Select Case lis.Count
            Case 0
                If Len(filterString) > 0 And Len(filterString) < 15 Then itemData.Text = "Ani jeden štítek pro zadanou podmínku."
            Case Is >= mq.TopRecordsOnly
                itemData.Text = String.Format("Nalezeno více než {0} štítků. Je třeba zpřesnit podmínku hledání.", mq.TopRecordsOnly.ToString)
            Case Else
                If filterString <> "" Then itemData.Text = String.Format("Počet nalezených štítků: {0}.", lis.Count.ToString)
        End Select
        If itemData.Text <> "" Then result.Add(itemData)

        For Each c As BO.o51Tag In lis
            itemData = New RadComboBoxItemData()

            itemData.Text = c.o51Name & " (" & c.Owner & "/" & BO.BAS.FD(c.DateInsert, True) & ")"

            itemData.Value = c.PID.ToString
            result.Add(itemData)
        Next

        Return result.ToArray
    End Function
End Class