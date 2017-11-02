Public Class j18RegionDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.j18Region
        Dim s As String = "select *," & bas.RecTail("j18") & " FROM j18Region WHERE j18ID=@pid"
        Return _cDB.GetRecord(Of BO.j18Region)(s, New With {.pid = intPID})
    End Function

    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("j18_delete", pars)

    End Function
    Public Function Save(cRec As BO.j18Region, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "j18ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j18Name", .j18Name, DbType.String, , , True, "Název")
            pars.Add("j18Code", .j18Code, DbType.String, , , True, "Kód")
            pars.Add("j17ID", BO.BAS.IsNullDBKey(.j17ID), DbType.Int32)
            pars.Add("j18Ordinary", .j18Ordinary, DbType.Int32)
            pars.Add("j18ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("j18ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("j18Region", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedJ18ID As Integer = _cDB.LastSavedRecordPID

            If Not lisX69 Is Nothing Then   'přiřazení rolí projektu
                bas.SaveX69(_cDB, BO.x29IdEnum.j18Region, intLastSavedJ18ID, lisX69, bolINSERT)
            End If
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.j18Region)
        Dim s As String = "select *," & bas.RecTail("j18")
        s += " FROM j18Region"
        If Not myQuery Is Nothing Then
            Dim strW As String = bas.ParseWhereMultiPIDs("j18ID", myQuery)
            strW += bas.ParseWhereValidity("j18", "", myQuery)
            If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        End If
        s += " ORDER BY j18Ordinary,j18Name"

        Return _cDB.GetList(Of BO.j18Region)(s)

    End Function
End Class
