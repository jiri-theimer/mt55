Public Class p42ProjectTypeDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p42ProjectType
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p42ID=@p42id"

        Return _cDB.GetRecord(Of BO.p42ProjectType)(s, New With {.p42id = intPID})
    End Function

    Public Function Save(cRec As BO.p42ProjectType, lisP43 As List(Of BO.p43ProjectType_Workload)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "p42ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
                pars.Add("f02ID", BO.BAS.IsNullDBKey(.f02ID), DbType.Int32)
                pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
                pars.Add("x38ID_Draft", BO.BAS.IsNullDBKey(.x38ID_Draft), DbType.Int32)
                pars.Add("p42Name", .p42Name, DbType.String, , , True, "Název")
                pars.Add("p42Code", .p42Code, DbType.String, , , True, "Kód")
                pars.Add("p42Ordinary", .p42Ordinary, DbType.Int32)
                pars.Add("p42IsDefault", .p42IsDefault, DbType.Boolean)
                pars.Add("p42ArchiveFlag", CInt(.p42ArchiveFlag), DbType.Int32)
                pars.Add("p42ArchiveFlagP31", CInt(.p42ArchiveFlagP31), DbType.Int32)
                pars.Add("p42IsModule_p31", .p42IsModule_p31, DbType.Boolean)
                pars.Add("p42IsModule_o23", .p42IsModule_o23, DbType.Boolean)
                pars.Add("p42IsModule_p56", .p42IsModule_p56, DbType.Boolean)
                pars.Add("p42IsModule_p45", .p42IsModule_p45, DbType.Boolean)
                pars.Add("p42IsModule_o22", .p42IsModule_o22, DbType.Boolean)
                pars.Add("p42IsModule_p48", .p42IsModule_p48, DbType.Boolean)
                pars.Add("p42SubgridO23Flag", .p42SubgridO23Flag, DbType.Int32)

                pars.Add("p42validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("p42validuntil", .ValidUntil, DbType.DateTime)

            End With

            If _cDB.SaveRecord("p42ProjectType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                If cRec.p42IsDefault Then
                    _cDB.RunSQL("UPDATE p42ProjectType set p42IsDefault=0 WHERE p42ID<>" & _cDB.LastSavedRecordPID.ToString)
                End If
                If Not lisP43 Is Nothing Then   'Pracovní náplň typu projektu
                    If Not bolINSERT Then _cDB.RunSQL("DELETE FROM p43ProjectType_Workload WHERE p42ID=" & _cDB.LastSavedRecordPID.ToString)

                    For Each c In lisP43                        
                        If Not _cDB.RunSQL("INSERT INTO p43ProjectType_Workload(p42ID,p34ID) VALUES(" & _cDB.LastSavedRecordPID.ToString & "," & c.p34ID.ToString & ")") Then
                            Return False
                        End If
                    Next
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
        Return _cDB.RunSP("p42_delete", pars)
    End Function


    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.p42ProjectType)
        Dim s As String = GetSQLPart1()
        Dim strW As String = bas.ParseWhereMultiPIDs("a.p42ID", myQuery)
        strW += bas.ParseWhereValidity("p42", "a", myQuery)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY p42Ordinary,p42Name"

        Return _cDB.GetList(Of BO.p42ProjectType)(s)

    End Function


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,b01.b01Name as _b01Name," & bas.RecTail("p42", "a")
        s += " FROM p42ProjectType a LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID"

        Return s
    End Function

    Public Function GetList_p43(intPID As Integer) As IEnumerable(Of BO.p43ProjectType_Workload)
        Dim s As String = "select a.*," & bas.RecTail("p43", "a", False, False)
        s += " FROM p43ProjectType_Workload a"
        s += " WHERE a.p42ID=@pid"

        Return _cDB.GetList(Of BO.p43ProjectType_Workload)(s, New With {.pid = intPID})
    End Function
End Class
