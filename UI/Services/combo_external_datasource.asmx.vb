Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Telerik.Web.UI

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class combo_external_datasource
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function LoadComboData(ByVal context As Object) As RadComboBoxItemData()
        Dim contextDictionary As IDictionary(Of String, Object) = DirectCast(context, IDictionary(Of String, Object))
        Dim filterString As String = (DirectCast(contextDictionary("filterstring"), String)).ToLower()
        Dim strX23ID As String = (DirectCast(contextDictionary("x23id"), String))
        Dim strJ03ID As String = (DirectCast(contextDictionary("j03id"), String))

        Dim cUser As New BO.j03UserSYS
        Dim factory As New BL.Factory(cUser)

        Dim result As List(Of RadComboBoxItemData) = Nothing

        Dim cX23 As BO.x23EntityField_Combo = factory.x23EntityField_ComboBL.Load(CInt(strX23ID))
        Dim strSQL As String = cX23.x23DataSource, pars As New List(Of BO.PluginDbParameter)
        pars.Add(New BO.PluginDbParameter("filterString", filterString))


        Dim dt As DataTable = factory.pluginBL.GetDataTable(strSQL, pars)


        result = New List(Of RadComboBoxItemData)(dt.Rows.Count)
        For Each dbRow As DataRow In dt.Rows
            Dim itemData As New RadComboBoxItemData()

            itemData.Text = dbRow(1).ToString
            itemData.Value = dbRow(0).ToString
            result.Add(itemData)
        Next

        Return result.ToArray
    End Function

End Class