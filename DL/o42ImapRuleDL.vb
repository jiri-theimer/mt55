Public Class o42ImapRuleDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.o42ImapRule
        Dim s As String = "SELECT *," & bas.RecTail("o42")
        s += " FROM o42ImapRule"
        s += " WHERE o42ID=@o42id"

        Return _cDB.GetRecord(Of BO.o42ImapRule)(s, New With {.o42id = intPID})
    End Function

    Public Function Save(cRec As BO.o42ImapRule) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "o42ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("o42Name", .o42Name, DbType.String)
            pars.Add("o42SenderAddress", .o42SenderAddress, DbType.String)
            pars.Add("o42IsUse_BCC", .o42IsUse_BCC, DbType.Boolean)
            pars.Add("o42IsUse_CC", .o42IsUse_CC, DbType.Boolean)
            pars.Add("o42IsUse_To", .o42IsUse_To, DbType.Boolean)
            pars.Add("o41ID", .o41ID, DbType.Int32)
            pars.Add("p57ID", BO.BAS.IsNullDBKey(.p57ID), DbType.Int32)
            pars.Add("x18ID", BO.BAS.IsNullDBKey(.x18ID), DbType.Int32)
            pars.Add("x67ID", BO.BAS.IsNullDBKey(.x67ID), DbType.Int32)
            pars.Add("p41ID_Default", BO.BAS.IsNullDBKey(.p41ID_Default), DbType.Int32)
            pars.Add("j02ID_Owner_Default", BO.BAS.IsNullDBKey(.j02ID_Owner_Default), DbType.Int32)

            pars.Add("o42validfrom", .ValidFrom, DbType.DateTime)
            pars.Add("o42validuntil", .ValidUntil, DbType.DateTime)

        End With

        If _cDB.SaveRecord("o42ImapRule", pars, bolINSERT, strW, True, _curUser.j03Login) Then
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
        Return _cDB.RunSP("o42_delete", pars)
    End Function

    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.o42ImapRule)
        Dim s As String = "SELECT *," & bas.RecTail("o42") & " FROM o42ImapRule", pars As New DbParameters
        Dim strW As String = bas.ParseWhereMultiPIDs("o42ID", mq)

        strW += bas.ParseWhereValidity("o42", "", mq)
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        Return _cDB.GetList(Of BO.o42ImapRule)(s, pars)

    End Function
    Public Function LoadHistoryByMessageGUID(strMessageGUID As String) As BO.o43ImapRobotHistory
        Dim s As String = GetSQLPart1_o43(1) & " WHERE a.o43MessageGUID=@guid ORDER BY a.o43ID DESC"

        Return _cDB.GetRecord(Of BO.o43ImapRobotHistory)(s, New With {.guid = strMessageGUID})
    End Function
    Public Function LoadHistoryByID(intO43ID As Integer) As BO.o43ImapRobotHistory
        Dim s As String = GetSQLPart1_o43(0) & " WHERE a.o43ID=@pid"

        Return _cDB.GetRecord(Of BO.o43ImapRobotHistory)(s, New With {.pid = intO43ID})
    End Function
    Public Function LoadHistoryByRecordGUID(strGUID As String) As BO.o43ImapRobotHistory
        Dim s As String = GetSQLPart1_o43(1) & " WHERE a.o43RecordGUID = @guid"

        Return _cDB.GetRecord(Of BO.o43ImapRobotHistory)(s, New With {.guid = strGUID})
    End Function

    Public Function InsertImport2History(cRec As BO.o43ImapRobotHistory) As Integer
        Dim pars As New DbParameters()

        With cRec
            pars.Add("o43DateImport", .o43DateImport, DbType.DateTime)
            pars.Add("o43DateMessage", .o43DateMessage, DbType.DateTime)
            pars.Add("o41ID", BO.BAS.IsNullDBKey(.o41ID), DbType.Int32)
            pars.Add("p56ID", BO.BAS.IsNullDBKey(.p56ID), DbType.Int32)
            pars.Add("o23ID", BO.BAS.IsNullDBKey(.o23ID), DbType.Int32)
            pars.Add("o43RecordGUID", .o43RecordGUID, DbType.String)
            pars.Add("o43MessageGUID", .o43MessageGUID, DbType.String)
            pars.Add("o43Subject", .o43Subject, DbType.String)
            pars.Add("o43Body_PlainText", .o43Body_PlainText, DbType.String)
            pars.Add("o43Body_Html", .o43Body_Html, DbType.String)
            pars.Add("o43FROM", .o43FROM, DbType.String)
            pars.Add("o43FROM_DisplayName", .o43FROM_DisplayName, DbType.String)
            pars.Add("o43CC", .o43CC, DbType.String)
            pars.Add("o43BCC", .o43BCC, DbType.String)
            pars.Add("o43TO", .o43TO, DbType.String)
            pars.Add("o43Attachments", .o43Attachments, DbType.String)
            pars.Add("o43Length", .o43Length, DbType.Int32)
            pars.Add("o43ImapArchiveFolder", .o43ImapArchiveFolder, DbType.String)
            pars.Add("o43ErrorMessage", .o43ErrorMessage, DbType.String)
            pars.Add("o43EmlFileName", .o43EmlFileName, DbType.String)
            pars.Add("o43MsgFileName", .o43MsgFileName, DbType.String)
        End With

        If _cDB.SaveRecord("o43ImapRobotHistory", pars, True) Then
            Return _cDB.LastIdentityValue
        Else
            Return 0
        End If

    End Function

    Public Sub ChangeRecordGuidInHistory(intO43ID As Integer, strNewGUID As String)
        Dim pars As New DbParameters
        pars.Add("pid", intO43ID, DbType.Int32)
        pars.Add("guid", strNewGUID, DbType.String)
        _cDB.RunSQL("UPDATE o43ImapRobotHistory set o43RecordGUID=@guid WHERE o43ID=@pid", pars)
    End Sub

    Public Function UpdateHistoryBind(intO43ID As Integer, intP56ID As Integer, intO23ID As Integer) As Boolean
        Dim pars As New DbParameters()
        pars.Add("pid", intO43ID)
        pars.Add("p56id", BO.BAS.IsNullDBKey(intP56ID), DbType.Int32)
        pars.Add("o23id", BO.BAS.IsNullDBKey(intO23ID), DbType.Int32)
        Return _cDB.RunSQL("UPDATE o43ImapRobotHistory set p56ID=@p56id,o23ID=@o23id WHERE o43ID=@pid", pars)
    End Function

    Private Function GetSQLPart1_o43(Optional intTOP As Integer = 0) As String
        Dim s As String = "SELECT"
        If intTOP > 0 Then s += " TOP " & intTOP.ToString
        s += " a.*,o41.o41Name as _o41Name"
        s += " FROM o43ImapRobotHistory a INNER JOIN o41InboxAccount o41 ON a.o41ID=o41.o41ID"
        Return s
    End Function
    Public Function GetList_o43(mq As BO.myQuery) As IEnumerable(Of BO.o43ImapRobotHistory)
        Dim s As String = GetSQLPart1_o43(mq.TopRecordsOnly)
        Dim strW As String = bas.ParseWhereMultiPIDs("a.o43ID", mq), pars As New DbParameters

        With mq
            If .SearchExpression <> "" Then
                strW += " AND (a.o43Body_PlainText like '%'+@expr+'%' OR a.o43Subject LIKE '%'+@expr+'%')"
                pars.Add("expr", .SearchExpression, DbType.String)
            End If
            If .DateFrom > DateSerial(1900, 1, 1) Then
                strW += " AND a.o43DateMessage>=@datefrom" : pars.Add("datefrom", .DateFrom, DbType.DateTime)
            End If
            If .DateUntil < DateSerial(3000, 1, 1) Then
                strW += " AND a.o43DateMessage<@dateuntil" : pars.Add("dateuntil", .DateUntil.AddDays(1), DbType.DateTime)
            End If

            If .ColumnFilteringExpression <> "" Then
                strW += " AND " & .ColumnFilteringExpression
            End If

        End With


        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)
        s += " ORDER BY a.o43ID DESC"

        Return _cDB.GetList(Of BO.o43ImapRobotHistory)(s, pars)

    End Function
End Class
