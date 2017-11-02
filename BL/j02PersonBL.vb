Public Interface Ij02PersonBL
    Inherits IFMother
    Function Save(cRec As BO.j02Person, lisFF As List(Of BO.FreeField)) As Boolean
    Function ValidateBeforeSave(cRec As BO.j02Person) As Boolean
    Function Load(intPID As Integer) As BO.j02Person
    Function LoadByEmail(strEmailAddress As String, Optional intJ02ID_Exclude As Integer = 0) As BO.j02Person
    Function LoadByImapRobotAddress(strRobotAddress) As BO.j02Person
    Function LoadByExternalPID(strExternalPID As String) As BO.j02Person
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryJ02) As IEnumerable(Of BO.j02Person)
    Function GetGridDataSource(myQuery As BO.myQueryJ02) As DataTable
    Function GetList_j02_join_j11(j02ids As List(Of Integer), j11ids As List(Of Integer)) As IEnumerable(Of BO.j02Person)
    Function GetEmails_j02_join_j11(j02ids As List(Of Integer), j11ids As List(Of Integer)) As List(Of BO.x43MailQueue_Recipient)
    Function GetList_j11(intJ02ID As Integer) As IEnumerable(Of BO.j11Team)
    Function GetList_Slaves(intJ02ID As Integer, bolDispCreateP31 As Boolean, dispP31 As BO.j05Disposition_p31ENUM, bolDispCreateP48 As Boolean, dispP48 As BO.j05Disposition_p48ENUM) As IEnumerable(Of BO.j02Person)
    Function GetList_Masters(intJ02ID As Integer) As IEnumerable(Of BO.j02Person)
    Function GetTeamsInLine(intJ02ID As Integer) As String
    Function GetList_AllAssignedEntityRoles(intPID As Integer, x29id_entity As BO.x29IdEnum) As IEnumerable(Of BO.x67EntityRole)
    Function LoadSumRow(intPID As Integer) As BO.j02PersonSum
    Function GetList_p66(mq As BO.myQueryP66) As IEnumerable(Of BO.p66PersonalPlan)
    Function SavePersonalPlan(lisP66 As List(Of BO.p66PersonalPlan)) As Boolean
End Interface
Class j02PersonBL
    Inherits BLMother
    Implements Ij02PersonBL
    Private WithEvents _cDL As DL.j02PersonDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j02PersonDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function ValidateBeforeSave(cRec As BO.j02Person) As Boolean Implements Ij02PersonBL.ValidateBeforeSave
        With cRec
            If Trim(.j02LastName) = "" Then _Error = "Chybí příjmení." : Return False
            If Trim(.j02FirstName) = "" Then _Error = "Chybí křestní jméno." : Return False
            If .j02IsIntraPerson Or (.j02IsIntraPerson = False And .j02IsInvoiceEmail) Then
                If Trim(.j02Email) = "" Then _Error = "Chybí e-mail adresa!" : Return False
            End If
          
            If .j02Email <> "" Then
                If Not BO.BAS.TestEMailAddress(.j02Email, _Error) Then
                    Return False
                End If
                Dim c As BO.j02Person = LoadByEmail(.j02Email, cRec.PID)
                If Not c Is Nothing Then
                    _Error = "Jiná osoba (" & c.FullNameAsc & ") již má zavedenu tuto e-mail adresu." : Return False
                End If
          
            End If
            If .j02TimesheetEntryDaysBackLimit = 0 And .j02TimesheetEntryDaysBackLimit_p34IDs <> "" Then
                _Error = "Omezení zpětného zapisování hodin není zadáno správně." : Return False
            End If
        End With

        Return True
    End Function
    Public Function Save(cRec As BO.j02Person, lisFF As List(Of BO.FreeField)) As Boolean Implements Ij02PersonBL.Save
        If Not ValidateBeforeSave(cRec) Then Return False
        If cRec.j02IsIntraPerson Then cRec.j02IsInvoiceEmail = False

        If _cDL.Save(cRec, lisFF) Then
            
            If cRec.PID = 0 Then
                Me.RaiseAppEvent(BO.x45IDEnum.j02_new, _LastSavedPID)
            Else
                Me.RaiseAppEvent(BO.x45IDEnum.j02_update, _LastSavedPID)
            End If
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.j02Person Implements Ij02PersonBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByEmail(strEmailAddress As String, Optional intJ02ID_Exclude As Integer = 0) As BO.j02Person Implements Ij02PersonBL.LoadByEmail
        Return _cDL.LoadByEmail(strEmailAddress, intJ02ID_Exclude)
    End Function
    Public Function LoadByImapRobotAddress(strRobotAddress) As BO.j02Person Implements Ij02PersonBL.LoadByImapRobotAddress
        Return _cDL.LoadByImapRobotAddress(strRobotAddress)
    End Function
    Public Function LoadByExternalPID(strExternalPID As String) As BO.j02Person Implements Ij02PersonBL.LoadByExternalPID
        Return _cDL.LoadByExternalPID(strExternalPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij02PersonBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.j02Person, intPID)
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.j02_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(mq As BO.myQueryJ02) As IEnumerable(Of BO.j02Person) Implements Ij02PersonBL.GetList
        Return _cDL.GetList(mq)
    End Function
  
    Public Function GetList_j02_join_j11(j02ids As List(Of Integer), j11ids As List(Of Integer)) As IEnumerable(Of BO.j02Person) Implements Ij02PersonBL.GetList_j02_join_j11
        Return _cDL.GetList_j02_join_j11(j02ids, j11ids)
    End Function
    Public Function GetEmails_j02_join_j11(j02ids As List(Of Integer), j11ids As List(Of Integer)) As List(Of BO.x43MailQueue_Recipient) Implements Ij02PersonBL.GetEmails_j02_join_j11
        Dim ret As New List(Of BO.x43MailQueue_Recipient)
        Dim lisJ02 As New List(Of BO.j02Person)
        Dim lisJ11WithAddress As New List(Of BO.j11Team), lisJ11WithoutAddress As New List(Of BO.j11Team)

        If j11ids.Count > 0 Then
            Dim mq As New BO.myQuery
            mq.Closed = BO.BooleanQueryMode.FalseQuery
            mq.PIDs = j11ids
            Dim lis As IEnumerable(Of BO.j11Team) = Factory.j11TeamBL.GetList(mq)
            lisJ11WithAddress = lis.Where(Function(p) p.j11Email <> "").ToList
            lisJ11WithoutAddress = lis.Where(Function(p) p.j11Email = "").ToList
        End If
        If j02ids.Count > 0 Then
            Dim mq As New BO.myQueryJ02
            mq.Closed = BO.BooleanQueryMode.FalseQuery
            mq.PIDs = j02ids
            lisJ02 = GetList(mq).Where(Function(p) p.j02Email <> "").ToList
        End If
        If lisJ11WithoutAddress.Count > 0 Then
            Dim lisAppend As List(Of BO.j02Person) = GetList_j02_join_j11(New List(Of Integer), lisJ11WithoutAddress.Select(Function(p) p.PID).ToList).ToList
            For Each c In lisAppend
                lisJ02.Add(c)
            Next
        End If


        For Each c In lisJ11WithAddress
            Factory.x40MailQueueBL.AppendRecipient(ret, c.j11Email, c.j11Name)
            If lisJ02.Count > 0 Then
                For Each cJ12 In Factory.j11TeamBL.GetList_BoundJ12(c.PID)
                    If lisJ02.Exists(Function(p) p.PID = cJ12.j02ID) Then
                        lisJ02.Remove(lisJ02.First(Function(p) p.PID = cJ12.j02ID))   'osoba je v týmu s vyplněnou e-mail adresou
                    End If
                Next
            End If

        Next
        For Each c In lisJ02
            Factory.x40MailQueueBL.AppendRecipient(ret, c.j02Email, c.FullNameAsc)
        Next
        Return ret
    End Function
    Public Function GetList_Slaves(intJ02ID As Integer, bolDispCreateP31 As Boolean, dispP31 As BO.j05Disposition_p31ENUM, bolDispCreateP48 As Boolean, dispP48 As BO.j05Disposition_p48ENUM) As IEnumerable(Of BO.j02Person) Implements Ij02PersonBL.GetList_Slaves
        Return _cDL.GetList_Slaves(intJ02ID, bolDispCreateP31, dispP31, bolDispCreateP48, dispP48)
    End Function
    Public Function GetList_Masters(intJ02ID As Integer) As IEnumerable(Of BO.j02Person) Implements Ij02PersonBL.GetList_Masters
        Return _cDL.GetList_Masters(intJ02ID)
    End Function
    Public Function GetTeamsInLine(intJ02ID As Integer) As String Implements Ij02PersonBL.GetTeamsInLine
        Return _cDL.GetTeamsInLine(intJ02ID)
    End Function
    Public Function GetList_j11(intJ02ID As Integer) As IEnumerable(Of BO.j11Team) Implements Ij02PersonBL.GetList_j11
        Return _cDL.GetList_j11(intJ02ID)
    End Function
    Public Function GetGridDataSource(myQuery As BO.myQueryJ02) As DataTable Implements Ij02PersonBL.GetGridDataSource
        Return _cDL.GetGridDataSource(myQuery)
    End Function
    Public Function GetList_AllAssignedEntityRoles(intPID As Integer, x29id_entity As BO.x29IdEnum) As IEnumerable(Of BO.x67EntityRole) Implements Ij02PersonBL.GetList_AllAssignedEntityRoles
        Return _cDL.GetList_AllAssignedEntityRoles(intPID, x29id_entity)
    End Function
    Public Function LoadSumRow(intPID As Integer) As BO.j02PersonSum Implements Ij02PersonBL.LoadSumRow
        Return _cDL.LoadSumRow(intPID)
    End Function
    Public Function GetList_p66(mq As BO.myQueryP66) As IEnumerable(Of BO.p66PersonalPlan) Implements Ij02PersonBL.GetList_p66
        Return _cDL.GetList_p66(mq)
    End Function
    Public Function SavePersonalPlan(lisP66 As List(Of BO.p66PersonalPlan)) As Boolean Implements Ij02PersonBL.SavePersonalPlan
        Return _cDL.SavePersonalPlan(lisP66)
    End Function
End Class
