Public Class x58UserPageDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x58UserPage
        Dim s As String = GetSQLPart1() & " WHERE a.x58ID=@pid"
        Return _cDB.GetRecord(Of BO.x58UserPage)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByJ03(intJ03ID As Integer) As BO.x58UserPage
        Dim s As String = GetSQLPart1() & " WHERE a.j03ID=@pid"
        Return _cDB.GetRecord(Of BO.x58UserPage)(s, New With {.pid = intJ03ID})
    End Function

    
    Public Function Save(cRec As BO.x58UserPage, lisX57 As List(Of BO.x57UserPageBinding)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x58ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("j03ID", .j03ID, DbType.Int32)
            pars.Add("x58Skin", .x58Skin, DbType.String)
            pars.Add("x58DockState", .x58DockState, DbType.String)
            pars.Add("x58ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x58ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("x58UserPage", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intX58ID As Integer = _cDB.LastSavedRecordPID
            If Not lisX57 Is Nothing Then
                _cDB.RunSQL("DELETE FROM x57UserPageBinding WHERE x58ID=" & intX58ID.ToString)
                For Each c In lisX57
                    pars = New DbParameters
                    pars.Add("x55id", c.x55ID, DbType.Int32)
                    pars.Add("x58id", intX58ID, DbType.Int32)
                    pars.Add("x57dockid", c.x57DockID, DbType.String)
                    _cDB.RunSQL("INSERT INTO x57UserPageBinding(x55ID,x58ID,x57DockID) VALUES(@x55id,@x58id,@x57dockid)", pars)
                Next
            End If
            Return True
        Else
            Return False
        End If

    End Function

    

    Public Function GetList_x57(intPID As Integer) As IEnumerable(Of BO.x57UserPageBinding)
        Dim s As String = "select a.*,x55.x55Name as _x55Name,x55.x55Content as _x55Content,x55.x55RecordSQL as _x55RecordSQL,x55.x55TypeFlag as _x55TypeFlag,x55.x55Height as _x55Height,x55.x55Width as _x55Width FROM x57UserPageBinding a INNER JOIN x55HtmlSnippet x55 ON a.x55ID=x55.x55ID"
        s += " WHERE a.x58ID=@pid"

        Return _cDB.GetList(Of BO.x57UserPageBinding)(s, New With {.pid = intPID})
    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "SELECT a.*," & bas.RecTail("x58", "a")
        s += " FROM x58UserPage a"

        Return s
    End Function
End Class
