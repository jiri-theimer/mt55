Public Class o38AddressDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o38Address
        Dim s As String = "SELECT *," & bas.RecTail("o38")
        s += " FROM o38Address"
        s += " WHERE o38ID=@o38id"

        Return _cDB.GetRecord(Of BO.o38Address)(s, New With {.o38id = intPID})
    End Function

    Public Function Save(cRec As BO.o38Address) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o38ID=@pid"
            pars.Add("pid", cRec.PID)
        Else
            cRec.ValidFrom = Now
            cRec.ValidUntil = DateSerial(3000, 1, 1)
        End If
        With cRec
            pars.Add("o38Name", .o38Name, DbType.String)
            pars.Add("o38Street", .o38Street, DbType.String, , , True, "Ulice")
            pars.Add("o38City", .o38City, DbType.String, , , True, "Město")
            pars.Add("o38ZIP", .o38ZIP, DbType.String, , , True, "PSČ")
            pars.Add("o38Country", .o38Country, DbType.String, , , True, "Stát")
            pars.Add("o38Description", .o38Description, DbType.String)
            pars.Add("o38AresID", .o38AresID, DbType.String)
            pars.Add("o38validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("o38validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("o38Address", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("o38_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQueryO38) As IEnumerable(Of BO.o38Address)
        Dim s As String = "SELECT * FROM o38Address", pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("o38ID", mq)
        With mq
            If .p41ID <> 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                strW += " AND o38ID IN (SELECT o38ID FROM o39Project_Address WHERE p41ID=@p41id)"
            End If
            If .p28ID <> 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND o38ID IN (SELECT o38ID FROM o37Contact_Address WHERE p28ID=@p28id)"
            End If
        End With
        strW += bas.ParseWhereValidity("o38", "", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.o38Address)(s, pars)

    End Function

    Public Function GetList_DistinctCountries() As List(Of String)
        Return _cDB.GetList(Of BO.GetString)("SELECT distinct o38Country as Value FROM o38Address WHERE o38Country IS NOT NULL").Select(Function(p) p.Value).ToList
        
    End Function
End Class
