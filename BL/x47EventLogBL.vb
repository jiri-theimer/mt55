
Public Interface Ix47EventLogBL
    Inherits IFMother
    Function AppendToLog(cRec As BO.x47EventLog) As Integer
    Function Load(intPID As Integer) As BO.x47EventLog
    Function GetList(mq As BO.myQueryX47, Optional intTopRecs As Integer = 0) As IEnumerable(Of BO.x47EventLog)
    Function GetObjectAlias(x29id As BO.x29IdEnum, intRecordPID As Integer) As String

End Interface
Class x47EventLogBL
    Inherits BLMother
    Implements Ix47EventLogBL
    Private WithEvents _cDL As DL.x47EventLogDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.x47EventLogDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function AppendToLog(cRec As BO.x47EventLog) As Integer Implements Ix47EventLogBL.AppendToLog
        Dim bolDeleteEvent As Boolean = False
        With cRec
            If .x29ID = BO.x29IdEnum._NotSpecified Then
                Dim cX45 As New BO.x45Event
                cX45.x45ID = .x45ID
                .x29ID = cX45.x29ID
                bolDeleteEvent = cX45.IsDeleteEvent
            End If
            If .x47RecordPID = 0 Then _Error = "Na vstupu chybí ID záznamu." : Return False
            If Not bolDeleteEvent Then
                If String.IsNullOrEmpty(.x47Name) And .x29ID > BO.x29IdEnum._NotSpecified Then
                    .x47Name = _cDL.GetObjectAlias(.x29ID, .x47RecordPID)
                End If
            End If
            'If String.IsNullOrEmpty(.x47Name) And .x29ID > BO.x29IdEnum._NotSpecified Then

            '    'Select Case .x45ID
            '    '    Case BO.x45IDEnum.p28_delete, BO.x45IDEnum.p36_delete, BO.x45IDEnum.p41_delete, BO.x45IDEnum.p51_delete, BO.x45IDEnum.p91_delete, BO.x45IDEnum.p56_delete, BO.x45IDEnum.o23_delete, BO.x45IDEnum.o22_delete, BO.x45IDEnum.j02_delete
            '    '        'pro odstraněné záznamy nehledat napřímo
            '    '    Case Else
            '    '        .x47Name = _cDL.GetObjectAlias(.x29ID, .x47RecordPID)
            '    'End Select

            'End If

            If Not String.IsNullOrEmpty(.x47Name) Then
                If String.IsNullOrEmpty(.x47NameReference) Then
                    If .x47Name.IndexOf("|") > 0 Then   'pipe odděluje případný referenční název
                        Dim a() As String = Split(.x47Name, "|")
                        .x47Name = a(0).Trim
                        .x47NameReference = a(1).Trim
                    End If
                End If
            End If
            If Not bolDeleteEvent Then
                Select Case .x29ID
                    Case BO.x29IdEnum.p56Task
                        Dim c As BO.p56Task = Factory.p56TaskBL.Load(.x47RecordPID)
                        If c.p41ID > 0 Then .x29ID_Reference = BO.x29IdEnum.p41Project : .x47RecordPID_Reference = c.p41ID
                    Case BO.x29IdEnum.o23Doc
                        Dim c As BO.o23Doc = Factory.o23DocBL.Load(.x47RecordPID)

                    Case BO.x29IdEnum.b07Comment
                        Dim c As BO.b07Comment = Factory.b07CommentBL.Load(.x47RecordPID)
                        .x29ID_Reference = c.x29ID
                        .x47RecordPID_Reference = c.b07RecordPID
                    Case BO.x29IdEnum.o22Milestone
                        Dim c As BO.o22Milestone = Factory.o22MilestoneBL.Load(.x47RecordPID)
                        If c.p41ID > 0 Then .x29ID_Reference = BO.x29IdEnum.p41Project : .x47RecordPID_Reference = c.p41ID
                        If c.p28ID > 0 Then .x29ID_Reference = BO.x29IdEnum.p28Contact : .x47RecordPID_Reference = c.p28ID
                        If c.j02ID > 0 Then .x29ID_Reference = BO.x29IdEnum.j02Person : .x47RecordPID_Reference = c.j02ID
                        If c.p91ID > 0 Then .x29ID_Reference = BO.x29IdEnum.p91Invoice : .x47RecordPID_Reference = c.p91ID
                        If c.p56ID > 0 Then .x29ID_Reference = BO.x29IdEnum.p56Task : .x47RecordPID_Reference = c.p56ID
                End Select
            End If


        End With

        Return _cDL.Create(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.x47EventLog Implements Ix47EventLogBL.Load
        Return _cDL.Load(intPID)
    End Function
  
    Public Function GetList(mq As BO.myQueryX47, Optional intTopRecs As Integer = 0) As IEnumerable(Of BO.x47EventLog) Implements Ix47EventLogBL.GetList
        Return _cDL.GetList(mq, intTopRecs)

    End Function
    Public Function GetObjectAlias(x29id As BO.x29IdEnum, intRecordPID As Integer) As String Implements Ix47EventLogBL.GetObjectAlias
        If x29id = BO.x29IdEnum._NotSpecified Or intRecordPID = 0 Then Return "????"
        Return _cDL.GetObjectAlias(x29id, intRecordPID)
    End Function


    ''Public Function FindX29IdForEvent(x45id As BO.x45IDEnum) As BO.x29IdEnum
    ''    Select Case x45id
    ''        Case BO.x45IDEnum.p28_bin, BO.x45IDEnum.p28_new, BO.x45IDEnum.p28_restore, BO.x45IDEnum.p28_delete, BO.x45IDEnum.p28_update, BO.x45IDEnum.p28_limitfee_over, BO.x45IDEnum.p28_limithours_over
    ''            Return BO.x29IdEnum.p28Contact
    ''        Case BO.x45IDEnum.p41_bin, BO.x45IDEnum.p41_limitfee_over, BO.x45IDEnum.p41_limithours_over, BO.x45IDEnum.p41_new, BO.x45IDEnum.p41_delete, BO.x45IDEnum.p41_update
    ''            Return BO.x29IdEnum.p41Project
    ''        Case BO.x45IDEnum.p51_bin, BO.x45IDEnum.p51_restore, BO.x45IDEnum.p51_update, BO.x45IDEnum.p51_delete, BO.x45IDEnum.p51_new
    ''            Return BO.x29IdEnum.p51PriceList
    ''        Case BO.x45IDEnum.p36_new, BO.x45IDEnum.p36_update, BO.x45IDEnum.p36_delete
    ''            Return BO.x29IdEnum.p36LockPeriod
    ''        Case BO.x45IDEnum.p91_bin, BO.x45IDEnum.p91_delete, BO.x45IDEnum.p91_new, BO.x45IDEnum.p91_restore, BO.x45IDEnum.p91_update
    ''            Return BO.x29IdEnum.p91Invoice
    ''        Case BO.x45IDEnum.p56_bin, BO.x45IDEnum.p56_delete, BO.x45IDEnum.p56_new, BO.x45IDEnum.p56_new, BO.x45IDEnum.p56_restore, BO.x45IDEnum.p56_update
    ''            Return BO.x29IdEnum.p56Task
    ''        Case BO.x45IDEnum.o23_bin, BO.x45IDEnum.o23_delete, BO.x45IDEnum.o23_new, BO.x45IDEnum.o23_restore, BO.x45IDEnum.o23_update
    ''            Return BO.x29IdEnum.o23Notepad
    ''        Case BO.x45IDEnum.o22_bin, BO.x45IDEnum.o22_delete, BO.x45IDEnum.o22_new, BO.x45IDEnum.o22_restore, BO.x45IDEnum.o22_update
    ''            Return BO.x29IdEnum.o22Milestone
    ''        Case BO.x45IDEnum.j02_bin, BO.x45IDEnum.j02_delete, BO.x45IDEnum.j02_new, BO.x45IDEnum.j02_restore, BO.x45IDEnum.j02_update
    ''            Return BO.x29IdEnum.j02Person
    ''        Case BO.x45IDEnum.b07_new
    ''            Return BO.x29IdEnum.b07Comment
    ''        Case Else
    ''            Return BO.x29IdEnum._NotSpecified
    ''    End Select

    ''End Function

End Class
