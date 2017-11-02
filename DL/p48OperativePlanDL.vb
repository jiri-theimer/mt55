Public Class p48OperativePlanDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p48OperativePlan
        Dim s As String = GetSQLPart1()
        s += " WHERE a.p48ID=@p48id"

        Return _cDB.GetRecord(Of BO.p48OperativePlan)(s, New With {.p48id = intPID})
    End Function

    Public Function GetList_SumPerPerson(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject)
        Dim pars As New DbParameters
        Dim strW As String = GetSqlWhere(mq, pars)
        Dim s As String = "SELECT a.j02ID,sum(p48Hours) as Hours,a.p48Date"
        s += " FROM p48OperativePlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " WHERE " & strW
        s += " GROUP BY a.j02ID,a.p48Date"
        Return _cDB.GetList(Of BO.OperativePlanSumPerPersonOrProject)(s, pars)
    End Function
    Public Function GetList_SumPerProject(mq As BO.myQueryP48) As IEnumerable(Of BO.OperativePlanSumPerPersonOrProject)
        Dim pars As New DbParameters
        Dim strW As String = GetSqlWhere(mq, pars)
        Dim s As String = "SELECT a.p41ID,sum(p48Hours) as Hours,a.p48Date"
        s += " FROM p48OperativePlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID"
        s += " WHERE " & strW
        s += " GROUP BY a.p41ID,a.p48Date"
        Return _cDB.GetList(Of BO.OperativePlanSumPerPersonOrProject)(s, pars)
    End Function
    Public Function GetList(mq As BO.myQueryP48) As IEnumerable(Of BO.p48OperativePlan)
        Dim pars As New DbParameters
        Dim strW As String = GetSqlWhere(mq, pars)
        Dim s As String = GetSQLPart1()
        s += " WHERE " & strW

        Return _cDB.GetList(Of BO.p48OperativePlan)(s, pars)
    End Function

    Private Function GetSqlWhere(mq As BO.myQueryP48, ByRef pars As DbParameters) As String
        Dim strW As String = bas.ParseWhereMultiPIDs("p48ID", mq)
        With mq
            If .DateFrom > DateSerial(1900, 1, 1) Then
                strW += " AND a.p48Date>=@datefrom" : pars.Add("datefrom", .DateFrom, DbType.DateTime)
            End If
            If .DateUntil < DateSerial(3000, 1, 1) Then
                strW += " AND a.p48Date<=@dateuntil" : pars.Add("dateuntil", .DateUntil, DbType.DateTime)
            End If
            If .p41ID > 0 Then
                pars.Add("p41id", .p41ID, DbType.Int32)
                If Not .IsIncludeChildProjects Then
                    strW += " AND a.p41ID=@p41id"
                Else
                    strW += " AND (a.p41ID=@p41id OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID=@p41id) AND (select p41TreeNext FROM p41Project WHERE p41ID=@p41id)))"
                End If
            End If
            If Not .p41IDs Is Nothing Then
                If .p41IDs.Count > 0 Then
                    strW += " AND a.p41ID IN (" & String.Join(",", .p41IDs) & ")"
                    ''If Not .IsIncludeChildProjects Then

                    ''Else
                    ''    strW += " AND (a.p41ID IN (" & String.Join(",", .p41IDs) & ") OR a.p41ID IN (SELECT p41ID FROM p41Project WHERE p41TreeIndex BETWEEN (select p41TreePrev FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & ")) AND (select p41TreeNext FROM p41Project WHERE p41ID IN (" & String.Join(",", .p41IDs) & "))))"
                    ''End If
                End If
            End If
            If .p28ID > 0 Then
                pars.Add("p28id", .p28ID, DbType.Int32)
                strW += " AND a.p41ID IN (SELECT p41ID FROM p41Project WHERE p28ID_Client=@p28id)"
            End If
            If Not .j02IDs Is Nothing Then
                If .j02IDs.Count > 0 Then
                    strW += " AND a.j02ID IN (" & String.Join(",", .j02IDs) & ")"
                End If
            End If
            If Not BO.BAS.TestPermission(_curUser, BO.x53PermValEnum.GR_P48_Reader) Then
                'nemá oprávnění číst všechny plány
                pars.Add("j02id_me", _curUser.j02ID, DbType.Int32)
                If _curUser.IsMasterPerson Then
                    strW += " AND (a.j02ID IN (SELECT j02ID_Slave FROM j05MasterSlave WHERE j02ID_Master=@j02id_me)"
                    strW += " OR a.j02ID IN (SELECT j12.j02ID FROM j12Team_Person j12 INNER JOIN j05MasterSlave xj05 ON j12.j11ID=xj05.j11ID_Slave WHERE xj05.j02ID_Master=@j02id_me)"
                    strW += " OR a.j02ID=@j02id_me)"
                Else
                    strW += " AND a.j02ID=@j02id_me"
                End If
            End If

            If .p34ID > 0 Then
                pars.Add("p34id", .p34ID, DbType.Int32)
                strW += " AND a.p34ID=@p34id"
            End If
            If .p32ID > 0 Then
                pars.Add("p32id", .p32ID, DbType.Int32)
                strW += " AND a.p32ID=@p32id"
            End If
        End With
        Return bas.TrimWHERE(strW)
    End Function

    Public Function Save(cRec As BO.p48OperativePlan) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p48ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p34ID", BO.BAS.IsNullDBKey(.p34ID), DbType.Int32)
            pars.Add("p32ID", BO.BAS.IsNullDBKey(.p32ID), DbType.Int32)
            pars.Add("p41ID", BO.BAS.IsNullDBKey(.p41ID), DbType.Int32)
            pars.Add("j02ID", BO.BAS.IsNullDBKey(.j02ID), DbType.Int32)
            pars.Add("p31ID", BO.BAS.IsNullDBKey(.p31ID), DbType.Int32)

            pars.Add("p48Date", .p48Date, DbType.DateTime)
            pars.Add("p48Hours", .p48Hours, DbType.Double)
            pars.Add("p48TimeFrom", .p48TimeFrom, DbType.String)
            pars.Add("p48TimeUntil", .p48TimeUntil, DbType.String)
            pars.Add("p48DateTimeFrom", .p48DateTimeFrom, DbType.DateTime)
            pars.Add("p48DateTimeUntil", .p48DateTimeUntil, DbType.DateTime)

            pars.Add("p48Text", .p48Text, DbType.String)

            pars.Add("p48validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("p48validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("p48OperativePlan", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("p48_delete", pars)
    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,j02.j02LastName+' '+j02.j02FirstName+isnull(' '+j02.j02TitleBeforeName,'') as _Person,p28.p28Name as _Client,case when p41.p41ParentID IS NULL THEN isnull(p41.p41NameShort,p41.p41Name) ELSE p41parent.p41Name+'->'+p41.p41Name END as _Project,p34.p34Name as _p34Name,p32.p32Name as _p32Name,p34.p34Color as _p34Color,p32.p32Color as _p32Color," & bas.RecTail("p48", "a")
        s += " FROM p48OperativePlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID"
        s += " INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID INNER JOIN p34ActivityGroup p34 ON a.p34ID=p34.p34ID LEFT OUTER JOIN p41project p41Parent ON p41.p41ParentID=p41parent.p41ID"
        s += " LEFT OUTER JOIN p32Activity p32 ON a.p32ID=p32.p32ID"
        s += " LEFT OUTER JOIN p28Contact p28 ON p41.p28ID_Client=p28.p28ID"
        Return s
    End Function
End Class
