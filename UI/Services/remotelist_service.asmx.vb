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
Public Class remotelist_service
    Inherits System.Web.Services.WebService

    Private _prefix As String = ""
    Private _result As List(Of RadComboBoxItemData) = Nothing

    Private _topRecs As Integer = 100

    <WebMethod()> _
    Public Function LoadComboData(ByVal context As Object) As RadComboBoxItemData()
        Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        _prefix = (DirectCast(contextDictionary("prefix"), String))
        If _prefix = "" Then _prefix = "j04"
        ''Dim strQryField As String = "", strQryValue As String = ""
        ''If contextDictionary.ContainsKey("qry_field") Then
        ''    strQryField = (DirectCast(contextDictionary("qry_field"), String)).ToLower()
        ''    strQryValue = DirectCast(contextDictionary("qry_value"), String)
        ''End If

        Dim factory As BL.Factory = Nothing

        If HttpContext.Current.User.Identity.IsAuthenticated Then
            factory = New BL.Factory(HttpContext.Current.User.Identity.Name)
        End If
        If factory Is Nothing Then
            Dim nic As List(Of RadComboBoxItemData) = New List(Of RadComboBoxItemData)(1)
            Dim xx As New RadComboBoxItemData()
            xx.Text = "Nelze ověřit přihlášeného uživatele"
            nic.Add(xx)
            Return nic.ToArray
        End If


        Select Case _prefix
            Case "j07"
                FillList(factory.j07PersonPositionBL.GetList(New BO.myQuery))

            Case "p42"
                FillList(factory.p42ProjectTypeBL.GetList(New BO.myQuery))
            Case "j04"
                FillList(factory.j04UserRoleBL.GetList(New BO.myQuery))

        End Select


        Return _result.ToArray
    End Function

    Private Sub FillList(lis As IEnumerable(Of Object))
        _result = New List(Of RadComboBoxItemData)(lis.Count)

        Dim itemData As New RadComboBoxItemData()

        itemData.Enabled = False
        Select Case lis.Count
            Case 0
                itemData.Text = "Ani jedna položka pro zadanou podmínku."
            Case Is >= _topRecs
                itemData.Text = String.Format("Nalezeno více než {0} položek. Je třeba zpřesnit podmínku hledání.", _topRecs.ToString)
            Case Else
                itemData.Text = String.Format("Počet položek: {0}.", lis.Count.ToString)
        End Select
        If itemData.Text <> "" Then _result.Add(itemData)

        For Each c In lis
            itemData = New RadComboBoxItemData()
            itemData.Value = c.pid.ToString
            Select Case _prefix
                Case "j07" : itemData.Text = c.j07Name
                Case "p42" : itemData.Text = c.p42Name
                Case "j04" : itemData.Text = c.j04Name
            End Select
            ''itemData.Text += " (" & Now.ToString & ")"
            _result.Add(itemData)
        Next
    End Sub
End Class