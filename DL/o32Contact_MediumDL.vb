Public Class o32Contact_MediumDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o32Contact_Medium
        Dim s As String = GetSQLPart1()
        s += " WHERE a.o32ID=@o38id"

        Return _cDB.GetRecord(Of BO.o32Contact_Medium)(s, New With {.o32id = intPID})
    End Function

    Public Function Save(cRec As BO.o32Contact_Medium) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o32ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("o33ID", BO.BAS.IsNullDBKey(.o33ID), DbType.Int32)
            pars.Add("p28ID", BO.BAS.IsNullDBKey(.p28ID), DbType.Int32)
            pars.Add("o32Value", .o32Value, DbType.String)
            pars.Add("o32Description", .o32Description, DbType.String)
            pars.Add("o32IsDefaultInInvoice", .o32IsDefaultInInvoice, DbType.Boolean)
        End With

        If _cDB.SaveRecord("o32Contact_Medium", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("o32_delete", pars)
    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*,a.o32ID as _pid,o33.o33Name as _o33Name"
        s += " FROM o32Contact_Medium a INNER JOIN o33MediumType o33 ON a.o33ID=o33.o33ID"

        Return s
    End Function
End Class
