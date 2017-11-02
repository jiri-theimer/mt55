Public MustInherit Class BLMother
    Implements IFMother
    Friend Property _Error As String
    Friend Property _LastSavedPID As Integer
    Friend Property _cUser As BO.j03UserSYS
    Private Property _factory As BL.Factory

    Public ReadOnly Property ErrorMessage As String Implements IFMother.ErrorMessage
        Get
            Return _Error
        End Get
    End Property
    Public ReadOnly Property LastSavedPID As Integer Implements IFMother.LastSavedPID
        Get
            Return _LastSavedPID
        End Get
    End Property
    

    Friend ReadOnly Property Factory As BL.Factory
        Get
            If _factory Is Nothing And Not _cUser Is Nothing Then
                _factory = New BL.Factory(_cUser)
            End If
            Return _factory
        End Get
    End Property

    Public Sub RaiseAppEvent(x45ID As BO.x45IDEnum, intRecordPID As Integer, Optional strRecordName As String = Nothing, Optional strDescription As String = Nothing, Optional bolStopAutoNotification As Boolean = False, Optional lisExplicitNotifyReceivers As List(Of BO.PersonOrTeam) = Nothing)
        Dim cLogEvent As New BO.x47EventLog
        With cLogEvent
            .x45ID = x45ID
            If Not String.IsNullOrEmpty(strRecordName) Then .x47Name = strRecordName
            .x47RecordPID = intRecordPID
            If Not String.IsNullOrEmpty(strDescription) Then .x47Description = strDescription
        End With
        Dim intX47ID As Integer = Factory.x47EventLogBL.AppendToLog(cLogEvent)
        If intX47ID > 0 Then
            If bolStopAutoNotification Then Return 'u tohoto záznamu se systém nemá pokoušet o notifikaci
            'notifikovat událost mailem
            Dim cX47 As BO.x47EventLog = Factory.x47EventLogBL.Load(intX47ID)
            If cX47.x45IsAllowNotification Then 'zjistit, zda událost má globálně povolenou notifikaci
                If Not lisExplicitNotifyReceivers Is Nothing Then
                    If lisExplicitNotifyReceivers.Count = 0 Then lisExplicitNotifyReceivers = Nothing
                End If
                Factory.x46EventNotificationBL.GenerateNotifyMessages(cX47, lisExplicitNotifyReceivers)

            End If
        End If

    End Sub

    Public Function RaiseAppEvent_TailoringTestBeforeSave(cRec As Object, lisFF As List(Of BO.FreeField), strEvent As String) As Boolean
        'validační pravidla na míru pro zákazníky
        'musí existovat ve web.config (appSettings) klíč události ve tvau prefix+'_beforesave' + SQL procedura, která provádí test
        'pokud SQL procedura vrátí přes @err_ret hlášku, považuje se to za stopku k uložení záznamu
        If BO.ASS.GetConfigVal(strEvent) > "" Then
            Dim strSqlProc As String = BO.ASS.GetConfigVal(strEvent)
            Dim strGUID As String = BO.BAS.GetGUID
            If Factory.p85TempBoxBL.SaveObjectReflection2Temp(strGUID, cRec) Then
                If Not lisFF Is Nothing Then
                    For Each c In lisFF
                        Dim cTemp As New BO.p85TempBox
                        cTemp.p85GUID = strGUID
                        cTemp.p85FreeText01 = c.x28Field
                        If Not (c.DBValue Is Nothing Or c.DBValue Is System.DBNull.Value) Then
                            cTemp.p85Message = c.DBValue.ToString
                        End If
                        Factory.p85TempBoxBL.Save(cTemp)
                    Next
                End If
                'spustit uloženou proceduru ve tvaru prefix+'_beforesave'
                'pokud vrátí prázdný string, pak OK, jinak vrací chybovou hlášku
                _Error = Factory.p85TempBoxBL.RunTailoredProcedure(strGUID, strSqlProc)
                If _Error <> "" Then Return False
            End If

        End If
        Return True
    End Function
    Public Function RaiseAppEvent_TailoringAfterSave(intRecordPID As Integer, strEvent As String) As Boolean
        'validační pravidla na míru pro zákazníky
        'musí existovat ve web.config (appSettings) klíč události ve tvau prefix+'_aftersave' + SQL procedura, která provádí test
        If BO.ASS.GetConfigVal(strEvent) > "" Then
            Return Factory.p85TempBoxBL.RunTailoredProcedure(intRecordPID, BO.ASS.GetConfigVal(strEvent))
        End If
        Return True
    End Function

    
End Class
