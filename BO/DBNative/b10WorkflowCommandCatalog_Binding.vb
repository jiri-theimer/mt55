Public Enum b10Worksheet_ProjectENUM
    _None = 0
    ProjectInTemplate = 1
    WorkflowContext = 2
End Enum
Public Enum b10Worksheet_PersonENUM
    _None = 0
    PersonInTemplate = 1
    WorkflowContext = 2
    WorkflowCreator = 3
End Enum
Public Enum b10Worksheet_DateENUM
    _None = 0
    DateInTemplate = 1
    DateContext = 2
    Today = 3
End Enum
Public Enum b10Worksheet_HoursENUM
    _None = 0
    HoursInTemplate = 1     'výše hodin podle vzorového úkonu
    HoursPerFund = 2      'výše hodin počítat podle pracovního fondu osoby
End Enum

Public Class b10WorkflowCommandCatalog_Binding
    Public Property b10ID As Integer
    Public Property b09ID As Integer
    Public Property b02ID As Integer
    Public Property b06ID As Integer
    Private Property _b09Name As String
    Private Property _b09SQL As String
    Private Property _b09Ordinary As Integer
    Public Property p31ID_Template As Integer   'vzorový worksheet záznam pro automatické generování

    Public Property b10Worksheet_ProjectFlag As b10Worksheet_ProjectENUM = b10Worksheet_ProjectENUM._None
    Public Property b10Worksheet_PersonFlag As b10Worksheet_PersonENUM = b10Worksheet_PersonENUM._None
    Public Property b10Worksheet_DateFlag As b10Worksheet_DateENUM = b10Worksheet_DateENUM._None
    Public Property b10Worksheet_p72ID As Integer
    Public Property b10Worksheet_Text As String     'text nového worksheet záznamu
    Public Property b10Worksheet_HoursFlag As b10Worksheet_HoursENUM = b10Worksheet_HoursENUM._None
    Public Property x18ID As Integer
    Public Property o23Name As String

    Public ReadOnly Property b09Name As String
        Get
            Return _b09Name
        End Get
    End Property
    Private Property _b09Code As String
    Public ReadOnly Property b09Code As String
        Get
            Return _b09Code
        End Get
    End Property
    Public ReadOnly Property b09SQL As String
        Get
            Return _b09SQL
        End Get
    End Property
    Public ReadOnly Property b09Ordinary As Integer
        Get
            Return _b09Ordinary
        End Get
    End Property


End Class
