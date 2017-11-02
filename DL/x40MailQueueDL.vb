Public Class x40MailQueueDL
    Inherits DLMother
    ''Private _cB65 As BO.b65WorkflowMessage = Nothing

    Public Sub New(ServiceUser As BO.j03User)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x40MailQueue
        Dim s As String = GetSQLPart1() & " WHERE a.x40ID=@pid"

        Return _cDB.GetRecord(Of BO.x40MailQueue)(s, New With {.pid = intPID})
    End Function
    Public Function LoadByEntity(intRecordPID As Integer, x29ID As BO.x29IdEnum) As BO.x40MailQueue
        Dim pars As New DbParameters
        pars.Add("recpid", intRecordPID, DbType.Int32)
        pars.Add("x29id", CInt(x29ID), DbType.Int32)
        Dim s As String = GetSQLPart1(1) & " WHERE a.x40RecordPID=@recpid AND a.x29ID=@x29id ORDER BY a.x40ID DESC"

        Return _cDB.GetRecord(Of BO.x40MailQueue)(s, pars)
    End Function
    Public Function LoadByMessageID(strMesageID As String) As BO.x40MailQueue
        Dim s As String = GetSQLPart1() & " WHERE a.x40MessageID = @messageid"

        Return _cDB.GetRecord(Of BO.x40MailQueue)(s, New With {.messageid = strMesageID})
    End Function
    Public Function Delete(intX40ID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intX40ID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If _cDB.RunSP("x40_delete", pars) Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function UpdateMessageState(intX40ID As Integer, NewState As BO.x40StateENUM) As Boolean
        Dim cRec As BO.x40MailQueue = Load(intX40ID)
        Select Case NewState
            Case BO.x40StateENUM.InQueque
                cRec.x40ErrorMessage = ""
                cRec.x40WhenProceeded = Nothing
                cRec.x40State = BO.x40StateENUM.InQueque
            Case BO.x40StateENUM.IsStopped
                cRec.x40State = BO.x40StateENUM.IsStopped
            Case BO.x40StateENUM.WaitOnConfirm
                cRec.x40State = BO.x40StateENUM.WaitOnConfirm

        End Select
        Return Save(cRec, Nothing)
    End Function
    Public Function Save(cRec As BO.x40MailQueue, lisX43 As List(Of BO.x43MailQueue_Recipient)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x40id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            If .x40State > BO.x40StateENUM._NotSpecified Then
                pars.Add("x40State", .x40State, DbType.Int32)
            End If
            pars.Add("j03ID_Sys", BO.BAS.IsNullDBKey(.j03ID_Sys), DbType.Int32)
            pars.Add("x29ID", BO.BAS.IsNullDBKey(cRec.x29ID), DbType.Int32)
            pars.Add("o40ID", BO.BAS.IsNullDBKey(.o40ID), DbType.Int32)
            pars.Add("x40State", .x40State, DbType.Int32)
            pars.Add("x40RecordPID", BO.BAS.IsNullDBKey(.x40RecordPID), DbType.Int32)
            pars.Add("x40IsHtmlBody", cRec.x40IsHtmlBody, DbType.Boolean)

            pars.Add("x40IsAutoNotification", .x40IsAutoNotification, DbType.Boolean)

            pars.Add("x40Subject", .x40Subject, DbType.String)
            pars.Add("x40Body", .x40Body, DbType.String)
            pars.Add("x40Recipient", Trim(.x40Recipient), DbType.String)
            pars.Add("x40CC", Trim(.x40CC), DbType.String)
            pars.Add("x40BCC", Trim(.x40BCC), DbType.String)
            pars.Add("x40Attachments", .x40Attachments, DbType.String)
            pars.Add("x40MessageID", .x40MessageID, DbType.String)
            pars.Add("x40ArchiveFolder", .x40ArchiveFolder, DbType.String)

            pars.Add("x40SenderName", .x40SenderName, DbType.String)
            pars.Add("x40SenderAddress", .x40SenderAddress, DbType.String)


            pars.Add("x40WhenProceeded", BO.BAS.IsNullDBDate(.x40WhenProceeded), DbType.DateTime)
            pars.Add("x40ErrorMessage", .x40ErrorMessage, DbType.String)

        End With


        If _cDB.SaveRecord("x40MailQueue", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intX40ID As Integer = Me.LastSavedRecordPID
            If Not lisX43 Is Nothing Then
                For Each c In lisX43
                    pars = New DbParameters
                    pars.Add("x40ID", intX40ID, DbType.Int32)
                    pars.Add("x43DisplayName", c.x43DisplayName, DbType.String)
                    pars.Add("x43Email", c.x43Email, DbType.String)
                    pars.Add("x43RecipientFlag", BO.BAS.IsNullDBKey(c.x43RecipientFlag), DbType.Int32)
                    _cDB.SaveRecord("x43MailQueue_Recipient", pars, True, , , , False)
                Next

            End If
            
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function

    ''Public Function GetList_AllHisMessages(intJ03ID_Sender As Integer, intJ02ID_Person As Integer, Optional intTopRecs As Integer = 500) As IEnumerable(Of BO.x40MailQueue)
    ''    Dim s As String = GetSQLPart1(intTopRecs)
    ''    Dim pars As New DbParameters, strW As String = ""
    ''    strW += " AND (a.x40RecordPID=@j02id AND a.x29ID=102) OR a.j03ID_Sys=@j03id"
    ''    pars.Add("j02id", intJ02ID_Person, DbType.Int32)
    ''    pars.Add("j03id", intJ03ID_Sender, DbType.Int32)

    ''    s += " WHERE " & bas.TrimWHERE(strW)
    ''    s += " ORDER BY a.x40ID DESC"

    ''    Return _cDB.GetList(Of BO.x40MailQueue)(s, pars)
    ''End Function
    Public Function GetList(myQuery As BO.myQueryX40) As IEnumerable(Of BO.x40MailQueue)
        Dim s As String = GetSQLPart1(myQuery.TopRecordsOnly)
        Dim strW As String = bas.ParseWhereMultiPIDs("a.x40ID", myQuery), pars As New DbParameters
        
        With myQuery
            If .SearchExpression <> "" Then
                strW += " AND (a.x40Recipient like '%'+@expr+'%' OR a.x40Subject LIKE '%'+@expr+'%')"
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If .DateFrom > DateSerial(1900, 1, 1) Then
                strW += " AND a.x40DateInsert>=@datefrom" : pars.Add("datefrom", .DateFrom, DbType.DateTime)
            End If
            If .DateUntil < DateSerial(3000, 1, 1) Then
                strW += " AND a.x40DateInsert<@dateuntil" : pars.Add("dateuntil", .DateUntil.AddDays(1), DbType.DateTime)
            End If
          
            If Not .x29ID_RecordPID Is Nothing And .RecordPID <> 0 Then
                Select Case .x29ID_RecordPID
                    Case BO.x29IdEnum.p28Contact
                        strW += " AND ((a.x40RecordPID=@pid and a.x29ID=328) OR (a.x29ID=391 AND a.x40RecordPID IN (SELECT p91ID FROM p91Invoice WHERE p28ID=@pid)))"
                    Case BO.x29IdEnum.p41Project
                        strW += " AND ((a.x40RecordPID=@pid and a.x29ID=141) OR (a.x29ID=391 AND a.x40RecordPID IN (SELECT p91ID FROM p31Worksheet WHERE p91ID IS NOT NULL AND p41ID=@pid)) OR (a.x29ID=356 AND a.x40RecordPID IN (SELECT p56ID FROM p56Task WHERE p41ID=@pid)))"
                    Case BO.x29IdEnum.j02Person
                        Dim prs As New DbParameters
                        prs.Add("pid", .RecordPID, DbType.Int32)
                        Dim strEmail As String = _cDB.GetValueFromSQL("select j02Email FROM j02Person WHERE j02ID=@pid", prs)
                        pars.Add("email", strEmail, DbType.String)
                        strW += " AND ((a.x40RecordPID=@pid and a.x29ID=102) OR (a.x40Recipient LIKE '%'+@email+'%'))"
                    Case Else
                        strW += " AND a.x29ID=@x29id AND a.x40RecordPID=@pid"
                End Select
                pars.Add("pid", .RecordPID, DbType.Int32)
                pars.Add("x29id", .x29ID_RecordPID, DbType.Int32)
            End If
            If Not .x29ID_ExplicitQuery Is Nothing Then
                strW += " AND a.x29ID=@x29id"
                pars.Add("x29id", .x29ID_ExplicitQuery, DbType.Int32)
            End If
            If Not .x40State Is Nothing Then
                strW += " AND a.x40State=@x40state"
                pars.Add("x40state", .x40State, DbType.Int32)
            End If
            If .j03ID_MyRecords <> 0 Then
                strW += " AND (a.x40RecordPID IN (SELECT j02ID FROM j03User WHERE j03ID=@j03id) AND a.x29ID=102) OR a.j03ID_Sys=@j03id"
                pars.Add("j03id", .j03ID_MyRecords, DbType.Int32)
            End If
            If .ColumnFilteringExpression <> "" Then
                strW += " AND " & ParseFilterExpression(.ColumnFilteringExpression)
            End If

        End With
        

        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY a.x40ID DESC"

        Return _cDB.GetList(Of BO.x40MailQueue)(s, pars)

    End Function
    Private Function ParseFilterExpression(strFilter As String) As String
        Return ParseSortExpression(strFilter).Replace("[", "").Replace("]", "")
    End Function
    Private Function ParseSortExpression(strSort As String) As String
        strSort = strSort.Replace("UserInsert", "x40UserInsert").Replace("UserUpdate", "x40UserUpdate").Replace("DateInsert", "x40DateInsert").Replace("DateUpdate", "x40DateUpdate")

        Return bas.NormalizeOrderByClause(strSort)
    End Function

    Public Function GetList_Recipients(intPID As Integer) As IEnumerable(Of BO.x43MailQueue_Recipient)
        Dim pars As New DbParameters
        pars.Add("pid", intPID, DbType.Int32)
        Dim s As String = "select * FROM x43MailQueue_Recipient WHERE x40ID=@pid"
        Return _cDB.GetList(Of BO.x43MailQueue_Recipient)(s, pars)
    End Function

    Private Function GetSQLPart1(Optional intTOP As Integer = 0) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*," & bas.RecTail("x40", , False, True)
        s += ",x29Name"
        s += " FROM x40MailQueue a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID"
        Return s
    End Function

    
    


End Class
