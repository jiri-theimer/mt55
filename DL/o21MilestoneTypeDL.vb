Public Class o21MilestoneTypeDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o21MilestoneType
        Dim s As String = "select *," & bas.RecTail("o21") & " FROM o21MilestoneType WHERE o21ID=@pid"
        Return _cDB.GetRecord(Of BO.o21MilestoneType)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("o21_delete", pars)

    End Function
    Public Function Save(cRec As BO.o21MilestoneType) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o21ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", .x29ID, DbType.Int32)
            pars.Add("o21Name", .o21Name, DbType.String, , , True, "Název")
            pars.Add("o21Flag", .o21Flag, DbType.Int32)
            pars.Add("o21Ordinary", .o21Ordinary, DbType.Int32)
            pars.Add("o21ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("o21ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("o21MilestoneType", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.o21MilestoneType)
        Dim s As String = "select *," & bas.RecTail("o21")
        s += " FROM o21MilestoneType"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("o21ID", myQuery)
            strW += bas.ParseWhereValidity("o21", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY o21Ordinary,o21Name"

        Return _cDB.GetList(Of BO.o21MilestoneType)(s)

    End Function
End Class
