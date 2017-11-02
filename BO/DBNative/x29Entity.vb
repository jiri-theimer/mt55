Public Enum x29IdEnum
    System = 1
    j02Person = 102
    j03User = 103
    j04UserRole = 104
    j07PersonPosition = 107
    j11Team = 111
    j18Region = 118
    j19PaymentType = 119
    j23NonPerson = 123
    j24NonPersonType = 124
    j27Currency = 127
    j61TextTemplate = 161
    j62MenuHome = 162
    p41Project = 141
    p45Budget = 345

    o22Milestone = 222
    o23Doc = 223

    o43ImapRobotHistory = 243

    o27Attachment = 227
    p28Contact = 328
    p31Worksheet = 331
    p40WorkSheet_Recurrence = 340
    p47CapacityPlan = 347
    p48OperativePlan = 348
    p49FinancialPlan = 349

    p51PriceList = 351
    p56Task = 356
    p57TaskType = 357
    p90Proforma = 390
    p82Proforma_Payment = 382
    p91Invoice = 391
    b01WorkflowTemplate = 601
    b02WorkflowStatus = 602
    b05Workflow_History = 605
    b06WorkflowStep = 606
    b07Comment = 607
    x67EntityRole = 967
    x69EntityRole_Assign = 969
    x31Report = 931
    x40MailQueue = 940
    x50Help = 950
    p36LockPeriod = 336
    p42ProjectType = 342
    p92InvoiceType = 392
    p89ProformaType = 389
    p87BillingLanguage = 387

    p29ContactType = 329
    c21FondCalendar = 421
    p34ActivityGroup = 334
    p32Activity = 332
    p95InvoiceRow = 395
    p71ApproveStatus = 371
    p72PreBillingStatus = 372
    p70BillingStatus = 370
    j70QueryTemplate = 170
    j77WorksheetStatTemplate = 177
    x18EntityCategory = 918

    x48SqlTask = 948
    Approving = 999
    _NotSpecified = 0
End Enum

Public Class x29Entity
    Inherits BOMotherFT
    Private Property _x29ID As Integer
    Public Property x29NameSingle As String
    Public Property x29TableName As String
    Public Property x29Description As String
    Public Property x29IsAttachment As Boolean
    Public Property x29IsReport As Boolean

    Public ReadOnly Property X29ID As x29IdEnum
        Get
            Try
                Return CType(_x29ID, x29IdEnum)
            Catch ex As Exception
                Return x29IdEnum._NotSpecified
            End Try
        End Get
    End Property
End Class
