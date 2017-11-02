Public Interface Ip51PriceListBL
    Inherits IFMother
    Function Save(cRec As BO.p51PriceList, lisP52 As List(Of BO.p52PriceList_Item)) As Boolean
    Function Load(intPID As Integer) As BO.p51PriceList
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p51PriceList)
    Function GetList_p52(intPID As Integer) As IEnumerable(Of BO.p52PriceList_Item)
End Interface
Class p51PriceListBL
    Inherits BLMother
    Implements Ip51PriceListBL
    Private WithEvents _cDL As DL.p51PriceListDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p51PriceListDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.p51PriceList, lisP52 As List(Of BO.p52PriceList_Item)) As Boolean Implements Ip51PriceListBL.Save
        With cRec
            If Trim(.p51Name) = "" And Not .p51IsCustomTailor Then
                _Error = "Chybí název ceníku." : Return False
            End If
            If .j27ID = 0 Then _Error = "Chybí měna ceníku." : Return False
            If .p51IsMasterPriceList Then

                Dim cP41BL As New BL.p41ProjectBL(_cUser)
                Dim mq As New BO.myQueryP41
                If .PID <> 0 Then
                    mq.p51ID = .PID : mq.TopRecordsOnly = 1
                    If cP41BL.GetList(mq).Count > 0 Then
                        _Error = BO.BAS.GLX("Minimálně jeden projekt [%1%] je svázán s tímto ceníkem. MASTER ceník nemůže být svázán přímo s projektem!", cP41BL.GetList(mq)(0).FullName) : Return False
                    End If

                    If GetList().Where(Function(p) p.p51ID_Master = .PID And p.j27ID <> .j27ID).Count > 0 Then
                        _Error = "Měna MASTER ceníku musí být shodná s měnou jeho podřízených ceníků!" : Return False
                    End If
                End If
            Else
                If .p51ID_Master <> 0 Then
                    Dim cR As BO.p51PriceList = Load(.p51ID_Master)
                    If cR.j27ID <> .j27ID Then
                        _Error = BO.BAS.GLX("Měna ceníku musí být shodná s měnou jeho MASTER ceníku [%1%]!", cR.j27Code) : Return False
                    End If
                End If
            End If

        End With

        If _cDL.Save(cRec, lisP52) Then
          
            If cRec.PID = 0 Then
                Me.RaiseAppEvent(BO.x45IDEnum.p51_new, _LastSavedPID)
            Else
                Me.RaiseAppEvent(BO.x45IDEnum.p51_update, _LastSavedPID)
            End If
            Return True
        Else
            Return False
        End If
    End Function
    Public Function Load(intPID As Integer) As BO.p51PriceList Implements Ip51PriceListBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ip51PriceListBL.Delete
        Dim s As String = Me.Factory.GetRecordCaption(BO.x29IdEnum.p51PriceList, intPID) 'úschova kvůli logování historie
        If _cDL.Delete(intPID) Then
            Me.RaiseAppEvent(BO.x45IDEnum.p51_delete, intPID, s)
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.p51PriceList) Implements Ip51PriceListBL.GetList
        Return _cDL.GetList(mq)
    End Function
    Public Function GetList_p52(intPID As Integer) As IEnumerable(Of BO.p52PriceList_Item) Implements Ip51PriceListBL.GetList_p52
        Return _cDL.GetList_p52(intPID)
    End Function
End Class
