Imports System.ServiceModel
Imports System.Runtime.Serialization



' NOTE: You can use the "Rename" command on the context menu to change the interface name "Imtdefault" in both code and config file together.
<ServiceContract()>
Public Interface ImtService


    <OperationContract()>
        <FaultContract(GetType(FaultException))>
    Function Ping(strLogin As String, strPassword As String) As Boolean


  
    <OperationContract()>
    Function SaveTask(intPID As Integer, strExternalPID As String, fields As Dictionary(Of String, Object), receivers As List(Of BO.x69EntityRole_Assign), uploadedTempFiles As List(Of String), strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function LoadTaskExtended(intPID As Integer, strLogin As String, strPassword As String) As BO.p56TaskWsExtended
    <OperationContract()>
    Function LoadTask(intPID As Integer, strLogin As String, strPassword As String) As BO.p56Task
    <OperationContract()>
    Function LoadTaskByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p56Task

    <OperationContract()>
    Function SaveWorksheet(intPID As Integer, fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult

    <OperationContract()>
    Function ListProjects(intP28ID As Integer, bolWorksheetEnty As Boolean, strLogin As String, strPassword As String) As IEnumerable(Of BO.p41Project)
    <OperationContract()>
    Function LoadProject(intPID As Integer, strLogin As String, strPassword As String) As BO.p41Project
    <OperationContract()>
    Function LoadProjectByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p41Project
    <OperationContract()>
    Function SaveProject(intPID As Integer, fields As Dictionary(Of String, Object), receivers As List(Of BO.x69EntityRole_Assign), strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function ListClients(strLogin As String, strPassword As String) As IEnumerable(Of BO.p28Contact)
    <OperationContract()>
    Function LoadClient(intPID As Integer, strLogin As String, strPassword As String) As BO.p28Contact
    <OperationContract()>
    Function LoadClientByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.p28Contact
    <OperationContract()>
    Function SaveClient(intPID As Integer, fields As Dictionary(Of String, Object), addresses As List(Of BO.o37Contact_Address), strLogin As String, strPassword As String) As BO.ServiceResult

    <OperationContract()>
    Function ListPriorities(strLogin As String, strPassword As String) As IEnumerable(Of BO.p59Priority)
    <OperationContract()>
    Function ListTaskTypes(strLogin As String, strPassword As String) As IEnumerable(Of BO.p57TaskType)
    <OperationContract()>
    Function ListActivities(strLogin As String, strPassword As String) As IEnumerable(Of BO.p32Activity)
    <OperationContract()>
    Function ListSheets(strLogin As String, strPassword As String) As IEnumerable(Of BO.p34ActivityGroup)
    <OperationContract()>
    Function ListPersons(strLogin As String, strPassword As String) As IEnumerable(Of BO.j02Person)
    <OperationContract()>
    Function ListPersonTeams(strLogin As String, strPassword As String) As IEnumerable(Of BO.j11Team)
    <OperationContract()>
    Function ListRoles(strLogin As String, strPassword As String) As IEnumerable(Of BO.x67EntityRole)
    <OperationContract()>
    Function ListWorkflowStatuses(intB01ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.b02WorkflowStatus)
    <OperationContract()>
    Function ListWorkflowSteps(intB01ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.b06WorkflowStep)
    <OperationContract()>
    Function ListPossibleWorkflowSteps(intRecordPID As Integer, strRecordPrefix As String, intJ02ID As Integer, strLogin As String, strPassword As String) As List(Of BO.WorkflowStepPossible4User)
    <OperationContract()>
    Function ListContactPersons(strLogin As String, strPassword As String, intP28ID As Integer, intP41ID As Integer) As IEnumerable(Of BO.j02Person)
    <OperationContract()>
    Function LoadPerson(intPID As Integer, strLogin As String, strPassword As String) As BO.j02Person
    <OperationContract()>
    Function LoadPersonByExternalPID(strExternalPID As String, strLogin As String, strPassword As String) As BO.j02Person
    <OperationContract()>
    Function SavePerson(intPID As Integer, fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult

    <OperationContract()>
    Function SaveContactPerson(intJ02ID As Integer, intP28ID As Integer, strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function DeleteContactPerson(intJ02ID As Integer, intP28ID As Integer, strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function ListSheets4Project(intP41ID As Integer, intJ02ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.p34ActivityGroup)
    <OperationContract()>
    Function ListTasks4WorksheetEnty(intP41ID As Integer, intJ02ID As Integer, strLogin As String, strPassword As String) As IEnumerable(Of BO.p56Task)
    <OperationContract()>
    Function UploadBinaryToTempFile(chunkBytes As Byte(), intPartZeroIndex As Integer, intTotalSize As Integer, strArchiveFileName As String, strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function ListDocTypes(strLogin As String, strPassword As String) As IEnumerable(Of BO.x18EntityCategory)
    <OperationContract()>
    Function ListComboSource(strDataPrefix As String, strFlag As String, bolFirstEmptyRow As Boolean, intParentPID As Integer, strLogin As String, strPassword As String) As List(Of BO.ComboSource)
    <OperationContract()>
    Function SaveDocument(intPID As Integer, strExternalPID As String, uploadedTempFiles As List(Of String), fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult
    <OperationContract()>
    Function LoadMsOfficeBinding(strEntryID As String, strLogin As String, strPassword As String) As BO.MsOfficeBinding
    <OperationContract()>
    Function SaveExternalObject2Temp(strPrefix As String, strTempGUID As String, strExternalPID As String, uploadedTempFiles As List(Of String), fields As Dictionary(Of String, Object), strLogin As String, strPassword As String) As BO.ServiceResult
End Interface


''<DataContract()>
''Public Class ServiceFaultException
''    <DataMember()>
''    Public Property ErrorMessage As String
''    Public Sub New(reason As FaultReason)

''    End Sub
''End Class



