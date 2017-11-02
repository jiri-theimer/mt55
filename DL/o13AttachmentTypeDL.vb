Imports Dapper
Public Class o13AttachmentTypeDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o13AttachmentType
        Dim s As String = GetSQLPart1() & " WHERE a.o13ID=@pid"

        Return _cDB.GetRecord(Of BO.o13AttachmentType)(s, New With {.pid = intPID})
    End Function
    
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o13_delete", pars)

    End Function
    Public Function Save(cRec As BO.o13AttachmentType) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o13ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("o13Name", .o13Name, DbType.String, , , True, "Název")

            pars.Add("o13ArchiveFolder", .o13ArchiveFolder, DbType.String, , , True, "Název archivní složky")
            pars.Add("o13IsUniqueArchiveFileName", .o13IsUniqueArchiveFileName, DbType.Boolean)

            pars.Add("o13validfrom", cRec.ValidFrom, DbType.DateTime)
            pars.Add("o13validuntil", cRec.ValidUntil, DbType.DateTime)
        End With


        If _cDB.SaveRecord("o13AttachmentType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.o13AttachmentType)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("o13ID", myQuery)
            strW += bas.ParseWhereValidity("o13", "a", myQuery)
            
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If

       
        s += " ORDER BY a.x29ID,a.o13Name"

        Return _cDB.GetList(Of BO.o13AttachmentType)(s, pars)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("o13", "a")
        s += ",x29.x29Name as _x29Name"
        s += " FROM o13AttachmentType a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID"
        Return s
    End Function
End Class
