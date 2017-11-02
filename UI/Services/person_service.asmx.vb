Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Telerik.Web.UI

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class user_service
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
        factory.j03UserBL.InhaleUserParams("handler_search_person-bin")

        Dim result As List(Of RadComboBoxItemData) = Nothing

        If strFlag = "email" Then
            'zdroj všech e-mail adres pro uc: email_receiver
            If filterString.IndexOf(";") > 0 Then filterString = Replace(filterString, ";", ",")
            Dim a() As String = Split(filterString, ",")
            filterString = Trim(a(UBound(a)))
            Dim lisE As IEnumerable(Of BO.GetString) = factory.ftBL.GetList_Emails(filterString, 50)
            result = New List(Of RadComboBoxItemData)(lisE.Count)
            For Each c In lisE
                Dim itemData As New RadComboBoxItemData()
                itemData.Text = c.Value
                result.Add(itemData)
            Next
            Return result.ToArray
        
        End If


        Dim mq As New BO.myQueryJ02
        mq.SearchExpression = filterString
        mq.TopRecordsOnly = 50
        Select Case strFlag
            Case "all2"
                mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
            Case "intra"
                mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly
            Case "search4o23"
                mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
                mq.Closed = BO.BooleanQueryMode.FalseQuery
                If strQryField = "j07id" Then
                    Select Case strQryValue
                        Case "-1"
                            mq.IntraPersons = BO.myQueryJ02_IntraPersons.IntraOnly  'pouze interní osoby
                        Case ""
                            mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
                        Case Else
                            mq.j07ID = BO.BAS.IsNullInt(strQryValue)
                    End Select

                End If
            Case "all"
                'bez omezení - všechny osoby

            Case "p31_entry"    'pouze pro zapisování worksheet
                mq.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForWorksheetEntry
            Case "p48_entry"
                mq.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForP48Entry
            Case "searchbox"
                mq.SpecificQuery = BO.myQueryP41_SpecificQuery.AllowedForRead
                mq.IntraPersons = BO.myQueryJ02_IntraPersons._NotSpecified
                If factory.j03UserBL.GetUserParam("handler_search_person-bin", "") = "1" Then
                    mq.Closed = BO.BooleanQueryMode.NoQuery
                Else
                    mq.Closed = BO.BooleanQueryMode.FalseQuery
                End If
            Case Else
                mq.SpecificQuery = BO.myQueryJ02_SpecificQuery.AllowedForRead
        End Select

        Dim lis As IEnumerable(Of BO.j02Person) = Nothing
        If strFlag = "masters" Then
            'pouze nadřízení
            lis = factory.j02PersonBL.GetList_Masters(factory.SysUser.j02ID)
        Else
            lis = factory.j02PersonBL.GetList(mq)
        End If

        result = New List(Of RadComboBoxItemData)(lis.Count)

        For Each usr As BO.j02Person In lis
            Dim itemData As New RadComboBoxItemData()
            itemData.Text = usr.FullNameDesc
            If usr.j07ID > 0 Then
                itemData.Text += " [" & usr.j07Name & "]"
            Else
                If usr.j02JobTitle <> "" Then itemData.Text += " [" & usr.j02JobTitle & "]"
            End If
            If usr.IsClosed Then
                itemData.Text = "<span class='radcomboitem_archive'>" & itemData.Text & "</span>"
            End If
            If Not usr.j02IsIntraPerson Then
                Dim lisP30 As IEnumerable(Of BO.p30Contact_Person) = factory.p30Contact_PersonBL.GetList(0, 0, usr.PID)
                If lisP30.Count > 0 Then
                    With lisP30(0)
                        If .p41ID > 0 Then
                            itemData.Text += " ..." & BO.BAS.OM3(lisP30(0).Project, 30)
                        Else
                            If .p28ID > 0 Then
                                itemData.Text += " ..." & BO.BAS.OM3(lisP30(0).p28Name, 30)
                            Else
                                itemData.Text += " ...kontaktní osoba"
                            End If
                        End If
                    End With
                End If
            End If
            

            itemData.Value = usr.PID.ToString
            result.Add(itemData)
        Next

        Return result.ToArray
    End Function

End Class