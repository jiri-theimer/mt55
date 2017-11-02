Public Class j05MasterSlaveDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j05MasterSlave
        Dim s As String = GetSQLPart1()
        s += " WHERE a.j05ID=@pid"

        Return _cDB.GetRecord(Of BO.j05MasterSlave)(s, New With {.pid = intPID})
    End Function

    Public Function Save(cRec As BO.j05MasterSlave) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j05ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j02ID_Master", BO.BAS.IsNullDBKey(.j02ID_Master), DbType.Int32)
            pars.Add("j02ID_Slave", BO.BAS.IsNullDBKey(.j02ID_Slave), DbType.Int32)
            pars.Add("j11ID_Slave", BO.BAS.IsNullDBKey(.j11ID_Slave), DbType.Int32)
            pars.Add("j05Disposition_p31", .j05Disposition_p31, DbType.Int32)
            pars.Add("j05Disposition_p48", .j05Disposition_p48, DbType.Int32)

            pars.Add("j05IsCreate_p31", .j05IsCreate_p31, DbType.Boolean)
            pars.Add("j05IsCreate_p48", .j05IsCreate_p48, DbType.Boolean)

          

            pars.Add("j05validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("j05validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("j05MasterSlave", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
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
        Return _cDB.RunSP("j05_delete", pars)
    End Function


    Public Function GetList(intJ02ID_Master As Integer, intJ02ID_Slave As Integer, intJ11ID_Slave As Integer) As IEnumerable(Of BO.j05MasterSlave)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = "1=1"
       
        If intJ02ID_Master > 0 Then
            pars.Add("j02id_master", intJ02ID_Master, DbType.Int32)
            strW += " AND a.j02ID_Master=@j02id_master"
        End If
        If intJ02ID_Slave > 0 Then
            pars.Add("j02id_slave", intJ02ID_Slave, DbType.Int32)
            strW += " AND a.j02ID_Slave=@j02id_slave"
        End If
        If intJ11ID_Slave > 0 Then
            pars.Add("j11id_slave", intJ11ID_Slave, DbType.Int32)
            strW += " AND a.j11ID_Slave=@j11id_slave"
        End If


        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY j02master.j02LastName,a.j02id_master,j02slave.j02LastName"

        Return _cDB.GetList(Of BO.j05MasterSlave)(s, pars)

    End Function

    


    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("j05", "a")
        s += ",j02master.j02LastName+' '+j02master.j02FirstName+isnull(' '+j02master.j02TitleBeforeName,'') as _PersonMaster"
        s += ",j02slave.j02LastName+' '+j02slave.j02FirstName+isnull(' '+j02slave.j02TitleBeforeName,'') as _PersonSlave"
        s += ",j11slave.j11Name as _TeamSlave"
        s += " FROM j05MasterSlave a INNER JOIN j02Person j02master ON a.j02ID_Master=j02master.j02ID"
        s += " LEFT OUTER JOIN j02Person j02slave ON a.j02ID_Slave=j02slave.j02ID"
        s += " LEFT OUTER JOIN j11Team j11slave ON a.j11ID_Slave=j11slave.j11ID"
        Return s
    End Function
End Class
