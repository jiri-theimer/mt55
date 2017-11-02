Public Enum p31RecordDisposition
    _NoAccess = 0
    CanRead = 1
    CanEdit = 2
    CanApprove = 3
    CanApproveAndEdit = 4
End Enum
Public Enum p31RecordState
    _NotExists = 0
    Editing = 1
    Locked = 2
    Approved = 5
    Invoiced = 7
End Enum
Public Class p31WorksheetDisposition
    Public Property p31ID As Integer
    Public Property RecordDisposition As BO.p31RecordDisposition = p31RecordDisposition._NoAccess
    Public Property RecordState As BO.p31RecordState = p31RecordState._NotExists
    Public Property LockedReasonMessage As String

    Public Sub New(intP31ID As Integer)
        Me.p31ID = intP31ID
    End Sub
End Class
