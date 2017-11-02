Public Class x50HelpDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x50Help
       Dim s As String = "select *," & bas.RecTail("x50") & " FROM x50Help WHERE x50ID=@x50id"

        Return _cDB.GetRecord(Of BO.x50Help)(s, New With {.x50id = intPID})

    End Function
    Public Function LoadByAspx(strAspxPageFileName As String) As BO.x50Help
        Dim s As String = "select *," & bas.RecTail("x50") & " FROM x50Help WHERE x50AspxPage LIKE @aspxpage"

        Return _cDB.GetRecord(Of BO.x50Help)(s, New With {.aspxpage = strAspxPageFileName})
    End Function

    Public Function Save(cRec As BO.x50Help) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x50ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(_curUser.j02ID), DbType.Int32)
            pars.Add("x50Name", .x50Name, DbType.String, , , True, "Název")
            pars.Add("x50AspxPage", .x50AspxPage, DbType.String, , , True, "Název ASPx stránky")
            pars.Add("x50ExternalURL", .x50ExternalURL, DbType.String, , , True, "Externí URL")
            pars.Add("x50Html", .x50Html, DbType.String)
            pars.Add("x50PlainText", .x50PlainText, DbType.String)
            pars.Add("x50validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("x50validuntil", cRec.ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x50Help", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If

    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x50_delete", pars)
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x50Help)
        Dim s As String = "select *," & bas.RecTail("x50")
        s += " FROM x50Help"
        Dim strW As String = bas.ParseWhereMultiPIDs("x50ID", myQuery)
        strW += bas.ParseWhereValidity("x50", "", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)


        Return _cDB.GetList(Of BO.x50Help)(s)

    End Function
End Class
