Public Class b07CommentDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.b07Comment
        Dim s As String = GetSQLPart1() & " WHERE a.b07ID=@pid"
        Return _cDB.GetRecord(Of BO.b07Comment)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByO43ID(intO43ID As Integer) As BO.b07Comment
        Dim s As String = GetSQLPart1() & " WHERE a.o43ID=@o43id"
        Return _cDB.GetRecord(Of BO.b07Comment)(s, New With {.o43id = intO43ID})
    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("b07_delete", pars)

    End Function
    Public Function Save(cRec As BO.b07Comment) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "b07ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x29ID", BO.BAS.IsNullDBKey(.x29ID), DbType.Int32)
            pars.Add("b07RecordPID", BO.BAS.IsNullDBKey(.b07RecordPID), DbType.Int32)
            pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
            pars.Add("b07ID_Parent", BO.BAS.IsNullDBKey(.b07ID_Parent), DbType.Int32)
            pars.Add("o43ID", BO.BAS.IsNullDBKey(.o43ID), DbType.Int32)
            pars.Add("b07Value", .b07Value, DbType.String)
            pars.Add("b07WorkflowInfo", .b07WorkflowInfo, DbType.String)
            pars.Add("b07ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("b07ValidUntil", .ValidUntil, DbType.DateTime)
        End With

        If _cDB.SaveRecord("b07Comment", pars, bolINSERT, strW, True, _curUser.j03Login) Then

            Dim cDbTree As New clsDBTree(_cDB), strDbTreeErr As String = ""
            With cDbTree
                .BasicWHERE = "x29id=" & CInt(cRec.x29ID).ToString & " AND b07RecordPID=" & cRec.b07RecordPID.ToString
                .SaveTree("b07Comment", "b07TreeLevel", "b07UserInsert", "b07TreeOrder", "b07ID_Parent", "b07ID", "b07ID DESC", True, "b07TreePrev", "b07TreeNext", strDbTreeErr)
            End With
            Return True
        Else
            Return False
        End If

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,b07TreeOrder as _b07TreeOrder,b07TreeLevel as _b07TreeLevel,b07TreePrev as _b07TreePrev,b07TreeNext as _b07TreeNext,case when a.o43ID IS NULL THEN j02owner.j02FirstName+' '+j02owner.j02LastName else o43.o43From END as _Author,case when a.o43ID IS NULL THEN j02owner.j02AvatarImage END as _Avatar," & bas.RecTail("b07", "a")
        's += ",o27.o27OriginalFileName as _o27OriginalFileName,o27.o27ID as _o27ID,o43.o43Attachments as _o43Attachments"
        s += ",o43.o43Attachments as _o43Attachments"
        s += " FROM b07Comment a INNER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID"
        's += " LEFT OUTER JOIN o27Attachment o27 ON a.b07ID=o27.b07ID LEFT OUTER JOIN o43ImapRobotHistory o43 ON a.o43ID=o43.o43ID"
        s += " LEFT OUTER JOIN o43ImapRobotHistory o43 ON a.o43ID=o43.o43ID"
        Return s
    End Function
    Public Function GetList(myQuery As BO.myQueryB07) As IEnumerable(Of BO.b07Comment)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("a.b07ID", myQuery)
        With myQuery
            If Year(.DateFrom) > 1900 Or Year(.DateUntil) < 3000 Then
                pars.Add("d1", .DateFrom, DbType.DateTime)
                pars.Add("d2", .DateUntil, DbType.DateTime)
                strW += " AND (a.b07DateInsert BETWEEN @d1 AND @d2 OR a.b07DateInsert BETWEEN @d1 AND @d2)"
            End If
            If .x29id > BO.x29IdEnum._NotSpecified Then
                pars.Add("x29id", .x29id, DbType.Int32)
                strW += " AND a.x29ID=@x29id"
            End If
            If .RecordDataPID > 0 Then
                pars.Add("recordpid", .RecordDataPID, DbType.Int32)
                strW += " AND a.b07RecordPID=@recordpid"
            End If
            If Not .recPIDs Is Nothing Then
                If .recPIDs.Count > 0 Then
                    strW += " AND a.b07RecordPID IN (" & String.Join(",", .recPIDs) & ")"
                End If
            End If
            
            If .j02ID_Owner > 0 Then
                pars.Add("ownerid", .j02ID_Owner, DbType.Int32)
                strW += " AND a.j02ID_Owner=@ownerid"
            End If
            If .b07ID_Parent <> 0 Then
                pars.Add("parentpid", .b07ID_Parent, DbType.Int32)
                strW += " AND a.b07ID_Parent=@parentpid"
            End If
        End With

        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.b07TreeOrder,a.b07ID DESC"

        Return _cDB.GetList(Of BO.b07Comment)(s, pars)

    End Function
End Class
