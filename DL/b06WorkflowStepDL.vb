Public Class b06WorkflowStepDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.b06WorkflowStep
        Dim s As String = GetSQLPart1() & " WHERE a.b06ID=@b06id"
        Return _cDB.GetRecord(Of BO.b06WorkflowStep)(s, New With {.b06id = intPID})
    End Function
    Public Function LoadKickOffStep(intB01ID As Integer) As BO.b06WorkflowStep
        Dim s As String = GetSQLPart1() & " WHERE a.b06IsKickOffStep=1 AND b02.b01ID=@b01id"

        Return _cDB.GetRecord(Of BO.b06WorkflowStep)(s, New With {.b01id = intB01ID})
    End Function

    Public Function Save(cRec As BO.b06WorkflowStep, lisB08 As List(Of BO.b08WorkflowReceiverToStep), lisB11 As List(Of BO.b11WorkflowMessageToStep), lisB10 As List(Of BO.b10WorkflowCommandCatalog_Binding)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            _Error = ""
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "b06id=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("b02id", .b02ID, DbType.Int32)
                pars.Add("b02id_target", BO.BAS.IsNullDBKey(.b02ID_Target), DbType.Int32)
                pars.Add("x67ID_Nominee", BO.BAS.IsNullDBKey(.x67ID_Nominee), DbType.Int32)
                pars.Add("x67ID_Direct", BO.BAS.IsNullDBKey(.x67ID_Direct), DbType.Int32)
                pars.Add("j11ID_Direct", BO.BAS.IsNullDBKey(.j11ID_Direct), DbType.Int32)
                pars.Add("b02ID_LastReceiver_ReturnTo", BO.BAS.IsNullDBKey(.b02ID_LastReceiver_ReturnTo), DbType.Int32)
                pars.Add("f02ID", BO.BAS.IsNullDBKey(.f02ID), DbType.Int32)
                pars.Add("b06Name", .b06Name, DbType.String, , , True, "Název")
                pars.Add("b06Code", .b06Code, DbType.String)
                pars.Add("b06RunSQL", .b06RunSQL, DbType.String, , , True, "Spouštět SQL dotaz")
                pars.Add("b06ValidateBeforeRunSQL", .b06ValidateBeforeRunSQL, DbType.String, , , True, "Validační SQL dotaz před spuštětním kroku")
                pars.Add("b06ValidateBeforeErrorMessage", .b06ValidateBeforeErrorMessage, DbType.String, , , True, "Uživatelská hláška v případě nesplnění validačního dotazu")

                pars.Add("b06Ordinary", .b06Ordinary, DbType.Int32)
                pars.Add("b06ValidateAutoMoveSQL", .b06ValidateAutoMoveSQL, DbType.String, , , True, "Krok spustit automaticky při splnění SQL příkazu")

                pars.Add("b06IsCommentRequired", .b06IsCommentRequired, DbType.Boolean)
                
                pars.Add("b06IsManualStep", .b06IsManualStep, DbType.Boolean)
                pars.Add("b06IsKickOffStep", .b06IsKickOffStep, DbType.Boolean)

                pars.Add("b06IsNominee", .b06IsNominee, DbType.Boolean)
                pars.Add("b06IsNomineeRequired", .b06IsNomineeRequired, DbType.Boolean)
                pars.Add("b06NomineeFlag", CInt(.b06NomineeFlag), DbType.Int32)
                
                pars.Add("b06IsRunOneInstanceOnly", .b06IsRunOneInstanceOnly, DbType.Boolean)

                pars.Add("b06validfrom", cRec.ValidFrom, DbType.DateTime)
                pars.Add("b06validuntil", cRec.ValidUntil, DbType.DateTime)
                
            End With


            If _cDB.SaveRecord("b06WorkflowStep", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intLastSavedPID As Integer = 0
                If bolINSERT Then intLastSavedPID = _cDB.LastIdentityValue Else intLastSavedPID = cRec.PID
                If cRec.b06IsKickOffStep And cRec.b02ID <> 0 Then
                    Dim intB01ID As Integer = _cDB.GetIntegerValueFROMSQL("select b01ID FROM b02WorkflowStatus WHERE b02ID=" & cRec.b02ID.ToString)
                    _cDB.RunSQL("update b06WorkflowStep SET b06IsKickOffStep=0 WHERE b06ID<>" & intLastSavedPID.ToString & " AND b02ID IN (select b02ID FROM b02WorkflowStatus WHERE b01ID=" & intB01ID.ToString & ")")
                End If
                _cDB.RunSQL("DELETE FROM b08WorkflowReceiverToStep WHERE b06ID=" & intLastSavedPID.ToString)
                For Each c In lisB08
                    pars = New DbParameters
                    pars.Add("x67ID", BO.BAS.IsNullDBKey(c.x67ID), DbType.Int32)
                    pars.Add("b06ID", intLastSavedPID, DbType.Int32)
                    pars.Add("j11id", BO.BAS.IsNullDBKey(c.j11ID), DbType.Int32)
                    pars.Add("j04id", BO.BAS.IsNullDBKey(c.j04ID), DbType.Int32)
                    pars.Add("b08IsRecordCreator", c.b08IsRecordCreator, DbType.Boolean)
                    pars.Add("b08IsRecordOwner", c.b08IsRecordOwner, DbType.Boolean)
                    If Not _cDB.SaveRecord("b08WorkflowReceiverToStep", pars, True) Then
                        _Error = _cDB.ErrorMessage : Return False
                    End If
                Next
                _cDB.RunSQL("DELETE FROM b11WorkflowMessageToStep WHERE b06ID=" & intLastSavedPID.ToString)
                For Each c In lisB11
                    pars = New DbParameters
                    pars.Add("x67ID", BO.BAS.IsNullDBKey(c.x67ID), DbType.Int32)
                    pars.Add("j02ID", BO.BAS.IsNullDBKey(c.j02ID), DbType.Int32)
                    pars.Add("j04id", BO.BAS.IsNullDBKey(c.j04ID), DbType.Int32)
                    pars.Add("j11id", BO.BAS.IsNullDBKey(c.j11ID), DbType.Int32)
                    pars.Add("b11IsRecordCreator", c.b11IsRecordCreator, DbType.Boolean)
                    pars.Add("b11IsRecordOwner", c.b11IsRecordOwner, DbType.Boolean)
                    pars.Add("b11IsRecordCreatorByEmail", c.b11IsRecordCreatorByEmail, DbType.Boolean)
                    pars.Add("b06id", intLastSavedPID, DbType.Int32)
                    pars.Add("b65id", BO.BAS.IsNullDBKey(c.b65ID), DbType.Int32)
                    If Not _cDB.SaveRecord("b11WorkflowMessageToStep", pars, True) Then
                        _Error = _cDB.ErrorMessage : Return False
                    End If
                Next
                

                _cDB.RunSQL("DELETE from b10WorkflowCommandCatalog_Binding WHERE b06id=" & intLastSavedPID.ToString)
                For Each c In lisB10
                    pars = New DbParameters
                    pars.Add("b09ID", c.b09ID, DbType.Int32)
                    pars.Add("b06ID", intLastSavedPID, DbType.Int32)
                    pars.Add("x18ID", BO.BAS.IsNullDBKey(c.x18ID), DbType.Int32)
                    pars.Add("o23Name", c.o23Name, DbType.String)
                    pars.Add("p31ID_Template", BO.BAS.IsNullDBKey(c.p31ID_Template), DbType.Int32)
                    pars.Add("b10Worksheet_ProjectFlag", CInt(c.b10Worksheet_ProjectFlag), DbType.Int32)
                    pars.Add("b10Worksheet_PersonFlag", CInt(c.b10Worksheet_PersonFlag), DbType.Int32)
                    pars.Add("b10Worksheet_DateFlag", CInt(c.b10Worksheet_DateFlag), DbType.Int32)
                    pars.Add("b10Worksheet_p72ID", BO.BAS.IsNullDBKey(c.b10Worksheet_p72ID), DbType.Int32)
                    pars.Add("b10Worksheet_Text", c.b10Worksheet_Text, DbType.String)
                    pars.Add("b10Worksheet_HoursFlag", CInt(c.b10Worksheet_HoursFlag), DbType.Int32)
                    If Not _cDB.SaveRecord("b10WorkflowCommandCatalog_Binding", pars, True) Then
                        _Error = _cDB.ErrorMessage : Return False
                    End If
                Next

                sc.Complete()
                Return True
            Else
                _Error = _cDB.ErrorMessage
                Return False
            End If
        End Using
    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If _cDB.RunSP("b06_delete", pars) Then
            If pars.Get(Of String)("err_ret") <> "" Then
                _Error = pars.Get(Of String)("err_ret")
                Return False
            End If
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function

    Public Function GetList(intB01ID As Integer) As IEnumerable(Of BO.b06WorkflowStep)
        Dim s As String = GetSQLPart1()
        Dim pars As New DbParameters, strW As String = ""
        If intB01ID <> 0 Then
            pars.Add("b01id", intB01ID)
            strW += " AND b02.b01ID=@b01id"
        End If
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY a.b02ID,a.b06Ordinary"

        Return _cDB.GetList(Of BO.b06WorkflowStep)(s, pars)

    End Function

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*," & bas.RecTail("b06", "a") & ",b02target.b02Name as _TargetStatus,b01.x29ID as _x29ID,b01.o40ID as _o40ID"
        s += " FROM b06WorkflowStep a INNER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID"
        s += " LEFT OUTER JOIN b02WorkflowStatus b02target ON a.b02ID_Target=b02target.b02id"
        s += " LEFT OUTER JOIN b01WorkflowTemplate b01 ON b02.b01ID=b01.b01ID"
        Return s
    End Function

    Public Function GetList_Allb09IDs() As IEnumerable(Of BO.b09WorkflowCommandCatalog)
        Dim s As String = "select * FROM b09WorkflowCommandCatalog ORDER BY b09Ordinary"
        Return _cDB.GetList(Of BO.b09WorkflowCommandCatalog)(s)
    End Function
    Public Function GetList_B10(intPID As Integer) As IEnumerable(Of BO.b10WorkflowCommandCatalog_Binding)
        Dim s As String = "select a.*,b09.b09Name as _b09Name,b09.b09Code as _b09Code,b09.b09SQL as _b09SQL,b09.b09Ordinary as _b09Ordinary FROM b10WorkflowCommandCatalog_Binding a INNER JOIN b09WorkflowCommandCatalog b09 ON a.b09id=b09.b09id WHERE a.b06id=@b06id ORDER BY b09Ordinary"
        Return _cDB.GetList(Of BO.b10WorkflowCommandCatalog_Binding)(s, New With {.b06id = intPID})
    End Function
    Public Function GetList_B08(intPID As Integer) As IEnumerable(Of BO.b08WorkflowReceiverToStep)
        Return _cDB.GetList(Of BO.b08WorkflowReceiverToStep)("select * FROM b08WorkflowReceiverToStep WHERE b06ID=@b06id", New With {.b06id = intPID})
    End Function
   
    Public Function GetList_B11(intPID As Integer) As IEnumerable(Of BO.b11WorkflowMessageToStep)
        Return _cDB.GetList(Of BO.b11WorkflowMessageToStep)("select * FROM b11WorkflowMessageToStep WHERE b06ID=@b06id", New With {.b06id = intPID})
    End Function

    Public Function GetBeforeRunWorkflowSQLResult(intRecordPID As Integer, cB06 As BO.b06WorkflowStep) As Integer
        Dim strSQL As String = bas.ParseMergeSQL(cB06.b06ValidateBeforeRunSQL, intRecordPID.ToString)

        Return _cDB.GetIntegerValueFROMSQL(strSQL)

    End Function


    Public Sub RunB09Commands(intRecordPID As Integer, x29id As BO.x29IdEnum, intB06ID As Integer)
        If intB06ID = 0 Or intRecordPID = 0 Then Return
        'spuštění případných příkazů spojených s krokem

        For Each prikaz As BO.b10WorkflowCommandCatalog_Binding In GetList_B10(intB06ID)
            'spouštění pevných SQL příkazů
            If prikaz.b09SQL <> "" Then
                Dim pars As New DbParameters
                pars.Add("pid", intRecordPID, DbType.Int32)
                pars.Add("j03id_sys", _curUser.PID, DbType.Int32)
                _cDB.RunSQL(prikaz.b09SQL, pars)
            End If
            If prikaz.b09Code = "p31_create" Then   'generovat worksheet záznam
                'komplet přes SQL proceduru
                Dim pars As New DbParameters
                pars.Add("record_prefix", BO.BAS.GetDataPrefix(x29id), DbType.String)
                pars.Add("record_pid", intRecordPID, DbType.Int32)
                pars.Add("b10id", prikaz.b10ID, DbType.Int32)
                pars.Add("j03id_sys", _curUser.PID, DbType.Int32)
                pars.Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
                _cDB.RunSP("p31_create_from_workflow", pars)

            End If
        Next

    End Sub

    Public Function RunSQL(strSQL As String, intRecordPID As Integer) As Boolean
        Dim pars As New DbParameters
        pars.Add("pid", intRecordPID, DbType.Int32)
        pars.Add("j03id_sys", _curUser.PID, DbType.Int32)
        Return _cDB.RunSQL(strSQL, pars)
    End Function


    Public Function SaveStatusMove(intRecordPID As Integer, x29ID As BO.x29IdEnum, cB06 As BO.b06WorkflowStep, intB02ID_Target As Integer, bolManualStep As Boolean, strComment As String) As Boolean
        Dim pars As New DbParameters

        pars.Add("b02ID", intB02ID_Target, DbType.Int32)
        pars.Add("pid", intRecordPID)
        Select Case x29ID
            Case BO.x29IdEnum.p41Project
                Return _cDB.SaveRecord("p41Project", pars, , "p41ID=@pid")
            Case BO.x29IdEnum.p56Task
                Return _cDB.SaveRecord("p56Task", pars, , "p56ID=@pid")
            Case BO.x29IdEnum.p91Invoice
                Return _cDB.SaveRecord("p91Invoice", pars, , "p91ID=@pid")
            Case BO.x29IdEnum.p28Contact
                Return _cDB.SaveRecord("p28Contact", pars, , "p28ID=@pid")
          
            Case BO.x29IdEnum.o23Doc
                Return _cDB.SaveRecord("o23Doc", pars, , "o23ID=@pid")
            Case Else
                Return False
        End Select

    End Function

    Public Function GetListOfLastWorkflowX69(intX67ID As Integer, intRecordPID As Integer, intB02ID As Integer) As List(Of BO.x69EntityRole_Assign)
        Dim pars As New List(Of BO.PluginDbParameter), lis As New List(Of BO.x69EntityRole_Assign)
        pars.Add(New BO.PluginDbParameter("pid", intRecordPID))
        pars.Add(New BO.PluginDbParameter("b02id", intB02ID))
        pars.Add(New BO.PluginDbParameter("x67id", intX67ID))
        Dim dr As SqlClient.SqlDataReader = _cDB.GetDataReader("SELECT * FROM x70EntityRole_Assign_History WHERE x70RecordPID=@pid AND x67ID=@x67id AND b02ID=@b02id AND x70Date IN (SELECT TOP 1 x70Date FROM x70EntityRole_Assign_History WHERE x70RecordPID=@pid AND x67ID=@x67id AND b02ID=@b02id ORDER BY x70ID DESC)", , pars)
        While dr.Read
            Dim c As New BO.x69EntityRole_Assign
            c.x67ID = intX67ID
            c.x69RecordPID = intRecordPID
            If Not dr("j02ID") Is System.DBNull.Value Then c.j02ID = dr("j02ID")
            If Not dr("j07ID") Is System.DBNull.Value Then c.j07ID = dr("j07ID")
            If Not dr("j11ID") Is System.DBNull.Value Then c.j11ID = dr("j11ID")
            lis.Add(c)
        End While
        dr.Close()

        Return lis
    End Function

    Public Function GetAutoWorkflowSQLResult(intRecordPID As Integer, cB06 As BO.b06WorkflowStep) As Integer
        Dim strSQL As String = bas.ParseMergeSQL(cB06.b06ValidateAutoMoveSQL, intRecordPID.ToString)
        Return _cDB.GetIntegerValueFROMSQL(strSQL)

    End Function
End Class
