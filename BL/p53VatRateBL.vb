Public Interface Ip53VatRateBL
    Inherits IFMother
    Function Save(cRec As BO.p53VatRate) As Boolean
    Function Load(intPID As Integer) As BO.p53VatRate
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p53VatRate)
    Sub TestAndSetupVatExistence(x15id As BO.x15IdEnum?, intJ27ID As Integer)
End Interface
Class p53VatRateBL
    Inherits BLMother
    Implements Ip53VatRateBL
    Private WithEvents _cDL As DL.p53VatRateDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p53VatRateDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p53VatRate) As Boolean Implements Ip53VatRateBL.Save
        With cRec
            If .x15ID = 0 Then _Error = "Chybí hladina DPH sazby." : Return False
            If .j27ID = 0 Then _Error = "Chybí měna DPH sazby." : Return False
            If .PID = 0 Then
                .ValidFrom = DateSerial(Year(.ValidFrom), Month(.ValidFrom), Day(.ValidFrom))
            End If
        End With

        Return _cDL.Save(cRec)
    End Function
    Public Sub TestAndSetupVatExistence(x15id As BO.x15IdEnum?, intJ27ID As Integer) Implements Ip53VatRateBL.TestAndSetupVatExistence
        Dim lis As IEnumerable(Of BO.p53VatRate) = GetList(New BO.myQuery).Where(Function(p) p.j27ID = intJ27ID)
        If x15id Is Nothing Then x15id = BO.x15IdEnum.Nic
        If lis.Count = 0 Then
            Dim c As New BO.p53VatRate
            c.p53Value = 0
            c.x15ID = BO.x15IdEnum.BezDPH
            c.ValidFrom = DateSerial(2000, 1, 1) : c.ValidUntil = DateSerial(3000, 1, 1)
            c.j27ID = intJ27ID
            Save(c)
        End If
        If x15id > BO.x15IdEnum.BezDPH And lis.Where(Function(p) p.x15ID = x15id).Count = 0 Then
            If GetList(New BO.myQuery).Where(Function(p) p.x15ID = x15id And p.ValidUntil > Now).Count > 0 Then
                Dim c As New BO.p53VatRate
                c.x15ID = x15id
                c.p53Value = GetList(New BO.myQuery).Where(Function(p) p.x15ID = x15id And p.ValidUntil > Now).First.p53Value
                c.ValidFrom = DateSerial(2000, 1, 1) : c.ValidUntil = DateSerial(3000, 1, 1)
                c.j27ID = intJ27ID
                Save(c)
            End If
            
        End If
    End Sub
    Public Function Load(intPID As Integer) As BO.p53VatRate Implements Ip53VatRateBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip53VatRateBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery) As IEnumerable(Of BO.p53VatRate) Implements Ip53VatRateBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
