Public Interface Ij18RegionBL
    Inherits IFMother
    Function Save(cRec As BO.j18Region, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
    Function Load(intPID As Integer) As BO.j18Region
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j18Region)

End Interface
Class j18RegionBL
    Inherits BLMother
    Implements Ij18RegionBL
    Private WithEvents _cDL As DL.j18RegionDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.j18RegionDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.j18Region, lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean Implements Ij18RegionBL.Save
        With cRec
            If Trim(.j18Name) = "" Then _Error = "Chybí název skupiny." : Return False
            If Trim(.j18Code) = "" Then _Error = "Chybí kód skupiny." : Return False

        End With
        If Not lisX69 Is Nothing Then
            If Not TestX69(lisX69) Then Return False
        End If

        Return _cDL.Save(cRec, lisX69)
    End Function

    Private Function TestX69(lisX69 As List(Of BO.x69EntityRole_Assign)) As Boolean
        Dim x As Integer = 0
        For Each c In lisX69
            x += 1
            If c.x67ID = 0 Then
                _Error = "V nastavení projektových rolí chybí v řádku " & x.ToString & " specifikace projektové role."
                Return False
            End If
            If c.j02ID = 0 And c.j11ID = 0 Then
                _Error = "V nastavení projektových rolí chybí v řádku " & x.ToString & " specifikace osoby nebo týmu."
                Return False
            End If
        Next
        Return True
    End Function
    Public Function Load(intPID As Integer) As BO.j18Region Implements Ij18RegionBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Ij18RegionBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(Optional mq As BO.myQuery = Nothing) As IEnumerable(Of BO.j18Region) Implements Ij18RegionBL.GetList
        Return _cDL.GetList(mq)
    End Function
End Class
