Public Enum myQueryO23_SpecificQuery
    _NotSpecified = 0

    AllowedForRead = 2              'pouze dokumenty, ke kterým má právo na čtení

End Enum

Public Enum myQueryO23_QuickQuery
    _NotSpecified = 0
    OpenDocs = 1
    Removed2Bin = 2
    Bind2ProjectExist = 3
    Bind2ProjectWait = 4
    Bind2ClientExist = 5
    Bind2ClientWait = 6
    Bind2InvoiceExist = 7
    Bind2InvoiceWait = 8
    Bind2WorksheetExist = 9
    Bind2WorksheetWait = 10
    Bind2PersonExist = 11
    Bind2PersonWait = 12
End Enum
Public Class myQueryO23
    Inherits myQuery
    Public Property x23ID As Integer
    Public Property b02IDs As List(Of Integer)
    Public Property p41IDs As List(Of Integer)
    Public Property j02IDs As List(Of Integer)
    Public Property Owners As List(Of Integer)
    Public Property p56IDs As List(Of Integer)
    Public Property p28IDs As List(Of Integer)

    Public Property o23GUID As String

    Public Property Record_x29ID As BO.x29IdEnum = x29IdEnum._NotSpecified
    Public Property RecordPID As Integer

    Public Property DateQueryFieldBy As String

    Public Property CalendarDateFieldStart As String
    Public Property CalendarDateFieldEnd As String
    Public Property x67ID_MyRole As Integer
    Public Property HasAnyX67Role As BO.BooleanQueryMode = BooleanQueryMode.NoQuery
    Public Property OnlySlavesPersons As BO.BooleanQueryMode = BooleanQueryMode.NoQuery

    Public Property j70ID As Integer
    Public Property SpecificQuery As BO.myQueryO23_SpecificQuery = myQueryO23_SpecificQuery._NotSpecified
    Public Property x18Value As String
    Public Property x20ID_Bound As Integer
    Public Property x20ID_UnBound As Integer
    Public Property o51IDs As List(Of Integer)

    Public Sub New(intX23ID As Integer)
        Me.x23ID = intX23ID
    End Sub

End Class
