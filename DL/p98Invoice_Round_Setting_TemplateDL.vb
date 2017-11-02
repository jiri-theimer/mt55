Public Class p98Invoice_Round_Setting_TemplateDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p98Invoice_Round_Setting_Template
        Dim s As String = "select *," & bas.RecTail("p98") & " FROM p98Invoice_Round_Setting_Template WHERE p98ID=@pid"
        Return _cDB.GetRecord(Of BO.p98Invoice_Round_Setting_Template)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p98_delete", pars)

    End Function
    Public Function Save(cRec As BO.p98Invoice_Round_Setting_Template, lisP97 As List(Of BO.p97Invoice_Round_Setting)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p98ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p98Name", .p98Name, DbType.String, , , True, "Název")
            pars.Add("p98IsDefault", .p98IsDefault, DbType.Boolean)
            pars.Add("p98ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("p98ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("p98Invoice_Round_Setting_Template", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedP98ID As Integer = _cDB.LastSavedRecordPID
            If Not lisP97 Is Nothing Then   'položky zaokrouhlování
                If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p97Invoice_Round_Setting WHERE p98ID=" & intLastSavedP98ID.ToString)
                Dim x As Integer = 0
                For Each c In lisP97
                    pars = New DbParameters
                    pars.Add("p98ID", intLastSavedP98ID, DbType.Int32)
                    pars.Add("j27ID", BO.BAS.IsNullDBKey(c.j27ID), DbType.Int32)
                    pars.Add("p97AmountFlag", c.p97AmountFlag, DbType.Int32)
                    pars.Add("p97Scale", c.p97Scale, DbType.Int32)
                    If Not _cDB.SaveRecord("p97Invoice_Round_Setting", pars, True, , True, _curUser.j03Login, False) Then
                        Return False
                    End If
                Next
            End If
            If cRec.p98IsDefault Then _cDB.RunSQL("UPDATE p98Invoice_Round_Setting_Template set p98IsDefault=0 WHERE p98ID<>" & intLastSavedP98ID.ToString)
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p98Invoice_Round_Setting_Template)
        Dim s As String = "select *," & bas.RecTail("p98")
        s += " FROM p98Invoice_Round_Setting_Template"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("p98ID", myQuery)
            strW += bas.ParseWhereValidity("p98", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY p98Name"

        Return _cDB.GetList(Of BO.p98Invoice_Round_Setting_Template)(s)

    End Function

    Public Function GetList_P97(intP98ID As Integer) As IEnumerable(Of BO.p97Invoice_Round_Setting)
        Dim pars As New DbParameters
        pars.Add("p98id", intP98ID, DbType.Int32)
        Dim s As String = "SELECT a.*," & bas.RecTail("p97", "a")
        s += ",j27.j27Code as _j27Code"
        s += " FROM p97Invoice_Round_Setting a INNER JOIN j27Currency j27 ON a.j27ID=j27.j27ID"
        s += " WHERE a.p98ID=@p98id"

        Return _cDB.GetList(Of BO.p97Invoice_Round_Setting)(s, pars)

    End Function
End Class
