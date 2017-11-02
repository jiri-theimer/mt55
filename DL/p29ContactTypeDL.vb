Public Class p29ContactTypeDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p29ContactType
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p29ID=@p29id"

        Return _cDB.GetRecord(Of BO.p29ContactType)(s, New With {.p29id = intPID})
    End Function

    Public Function Save(cRec As BO.p29ContactType) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p29ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
                pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
                pars.Add("x38ID_Draft", BO.BAS.IsNullDBKey(.x38ID_Draft), DbType.Int32)
                pars.Add("p29Name", .p29Name, DbType.String, , , True, "Název")

                pars.Add("p29Ordinary", .p29Ordinary, DbType.Int32)
                pars.Add("p29IsDefault", .p29IsDefault, DbType.Boolean)

                pars.Add("p29validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p29validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p29ContactType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                If cRec.p29IsDefault Then
                    _cDB.RunSQL("UPDATE p29ContactType set p29IsDefault=0 WHERE p29ID<>" & _cDB.LastSavedRecordPID.ToString)
                End If

                sc.Complete()
                Return True
            Else
                Return False
            End If
        End Using
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("p29_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p29ContactType)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p29ID", myQuery)
        strW += bas.ParseWhereValidity("p29", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p29Ordinary,p29Name"

        Return _cDB.GetList(Of BO.p29ContactType)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("p29", "a")
        s += " FROM p29ContactType a"

        Return s
    End Function

    
End Class
