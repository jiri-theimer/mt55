Public Class f01FolderDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    
    Public Function Load(intPID As Integer) As BO.f01Folder
        Dim s As String = GetSQLPart1() & " WHERE a.f01ID=@f01id"

        Return _cDB.GetRecord(Of BO.f01Folder)(s, New With {.f01id = intPID})
    End Function
    Public Function LoadByEntity(intRecordPID As Integer, intF02ID As Integer) As BO.f01Folder
        Dim pars As New DbParameters
        pars.Add("recpid", intRecordPID, DbType.Int32)
        pars.Add("f02id", intF02ID, DbType.Int32)
        Dim s As String = GetSQLPart1() & " WHERE a.f01RecordPID=@recpid AND a.f02ID=@f02id"

        Return _cDB.GetRecord(Of BO.f01Folder)(s, pars)
    End Function
    Public Function Load_f02(intF02ID As Integer) As BO.f02FolderType
        Dim s As String = "select *," & bas.RecTail("f02") & " FROM f02FolderType WHERE f02ID=@f02id"

        Return _cDB.GetRecord(Of BO.f02FolderType)(s, New With {.f02id = intF02ID})
    End Function
    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,f02.f02Name as _f02Name,f02.f02SysFlag as _f02SysFlag,f02.f02RootPath as _f02RootPath," & bas.RecTail("f01", "a")
        s += " FROM f01Folder a INNER JOIN f02FolderType f02 ON a.f02ID=f02.f02ID"
        Return s
    End Function

    Public Function Save(cRec As BO.f01Folder) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "f01ID=@pid"
            pars.Add("pid", cRec.PID)
        Else
            cRec.ValidFrom = Now
            cRec.ValidUntil = DateSerial(3000, 1, 1)
        End If
        With cRec
            pars.Add("f02ID", BO.BAS.IsNullDBKey(.f02ID), DbType.Int32)
            pars.Add("f01RecordPID", BO.BAS.IsNullDBKey(.f01RecordPID), DbType.Int32)
            pars.Add("f01Name", .f01Name, DbType.String)

            pars.Add("f01validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("f01validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("f01Folder", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("f01_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQuery, Optional intRecordPID As Integer = 0, Optional x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified) As IEnumerable(Of BO.f01Folder)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("f01ID", mq)
        strW += bas.ParseWhereValidity("f01", "a", mq)
        
        If intRecordPID <> 0 Then
            pars.Add("recpid", intRecordPID, DbType.Int32)
            strW += " AND a.f01RecordPID=@recpid"
        End If
        If x29ID > BO.x29IdEnum._NotSpecified Then
            pars.Add("x29id", CInt(x29ID), DbType.Int32)
            strW += " AND f02.x29ID=@x29id"
        End If
        strW += bas.ParseWhereValidity("f01", "a", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.f01Folder)(s, pars)

    End Function

    Public Function GetList_f02(mq As BO.myQuery) As IEnumerable(Of BO.f02FolderType)
        Dim s As String = "select *," & bas.RecTail("f02")
        s += " FROM f02FolderType"
        Dim strW As String = bas.ParseWhereMultiPIDs("f02ID", mq)
        strW += bas.ParseWhereValidity("f02", "", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.f02FolderType)(s)
    End Function
End Class
