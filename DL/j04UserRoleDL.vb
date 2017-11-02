Public Class j04UserRoleDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j04UserRole
        Dim s As String = "select a.*," & bas.RecTail("j04", "a") & ",x67.x67RoleValue as _x67RoleValue FROM j04userrole a INNER JOIN x67EntityRole x67 ON a.x67ID=x67.x67ID WHERE a.j04id=@j04id"
        Return _cDB.GetRecord(Of BO.j04UserRole)(s, New With {.j04id = intPID})
    End Function

    Public Function Save(cRec As BO.j04UserRole) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "j04id=@pid"
                pars.Add("pid", cRec.PID)
            End If

            With pars
                .Add("x67ID", BO.BAS.IsNullDBKey(cRec.x67ID), DbType.Int32)
                .Add("j60ID", BO.BAS.IsNullDBKey(cRec.j60ID), DbType.Int32)
                .Add("j04name", cRec.j04Name, DbType.String, , , True, "Název role")
                .Add("j04aspx_personalpage", cRec.j04Aspx_PersonalPage, DbType.String, , , True, "ASPX - osobní stránka")
                .Add("j04aspx_personalpage_mobile", cRec.j04Aspx_PersonalPage_Mobile, DbType.String, , , True, "ASPX - osobní stránka pro mobilní zařízení")
                .Add("j04Aspx_OneProjectPage", cRec.j04Aspx_OneProjectPage, DbType.String)
                .Add("j04Aspx_OneContactPage", cRec.j04Aspx_OneContactPage, DbType.String)
                .Add("j04Aspx_OneInvoicePage", cRec.j04Aspx_OneInvoicePage, DbType.String)
                .Add("j04Aspx_OnePersonPage", cRec.j04Aspx_OnePersonPage, DbType.String)
                .Add("j04IsMenu_Worksheet", cRec.j04IsMenu_Worksheet, DbType.Boolean)
                .Add("j04IsMenu_Project", cRec.j04IsMenu_Project, DbType.Boolean)
                .Add("j04IsMenu_Contact", cRec.j04IsMenu_Contact, DbType.Boolean)
                .Add("j04IsMenu_People", cRec.j04IsMenu_People, DbType.Boolean)
                .Add("j04IsMenu_Report", cRec.j04IsMenu_Report, DbType.Boolean)
                .Add("j04IsMenu_Invoice", cRec.j04IsMenu_Invoice, DbType.Boolean)
                .Add("j04IsMenu_Proforma", cRec.j04IsMenu_Proforma, DbType.Boolean)
                .Add("j04IsMenu_Notepad", cRec.j04IsMenu_Notepad, DbType.Boolean)
                .Add("j04IsMenu_MyProfile", cRec.j04IsMenu_MyProfile, DbType.Boolean)
                .Add("j04IsMenu_Task", cRec.j04IsMenu_Task, DbType.Boolean)
                .Add("j04IsMenu_More", cRec.j04IsMenu_More, DbType.Boolean)
                .Add("j04validfrom", cRec.ValidFrom, DbType.DateTime)
                .Add("j04validuntil", cRec.ValidUntil, DbType.DateTime)
            End With
            If _cDB.SaveRecord("j04UserRole", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                
                ' _cDB.RunSQL("UPDATE j03User_CacheData SET j03DateCache=dateadd(day,-1,j03DateCache) WHERE j03ID IN (SELECT j03ID FROM j03User WHERE j04ID=" & _LastSavedPID.ToString & ")")

                sc.Complete()
                Return True
            Else
                Return False
            End If
        End Using


    End Function
    Public Function Delete(intPID) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j04_delete", pars)

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j04UserRole)
        Dim s As String = "select a.*," & bas.RecTail("j04", "a") & ",x67.x67RoleValue as _x67RoleValue FROM j04userrole a INNER JOIN x67EntityRole x67 ON a.x67ID=x67.x67ID"

        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("a.j04ID", myQuery)
            strW += bas.ParseWhereValidity("j04", "a", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j04name"

        Return _cDB.GetList(Of BO.j04UserRole)(s)

    End Function

    

    Public Function GetList_BoundX53(intPID As Integer) As IEnumerable(Of BO.x53Permission)
        Dim s As String = "select a.* FROM x53Permission a inner join x68EntityRole_Permission b on a.x53ID=b.x53ID WHERE b.x67ID=@j04id ORDER BY x53Ordinary"
        Return _cDB.GetList(Of BO.x53Permission)(s, New With {.j04id = intPID})
    End Function

End Class
