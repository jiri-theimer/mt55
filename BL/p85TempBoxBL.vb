Public Interface Ip85TempBoxBL
    Inherits IFMother
    Function Save(cRec As BO.p85TempBox) As Boolean
    Function Load(intPID As Integer) As BO.p85TempBox
    Function LoadByGUID(strGUID As String) As BO.p85TempBox
    Function Delete(cRec As BO.p85TempBox) As Boolean
    Function UnDelete(cRec As BO.p85TempBox) As Boolean
    Function CloneOneRecord(intP85ID As Integer, strGUID_Dest As String) As Boolean
    Function Truncate(strGUID As String) As Boolean
    Function GetList(strGUID As String, Optional bolIncludeDeleted As Boolean = False) As IEnumerable(Of BO.p85TempBox)
    Sub Clone(strGUID_Source As String, strGUID_Dest As String)
    ''' <summary>
    ''' Načte hodnotu uloženého klíče z tempu
    ''' </summary>
    ''' <param name="strGUID">hodnota p85guid</param>
    ''' <returns>Vrací hodnotu p85DataPID</returns>
    ''' <remarks></remarks>
    Function LoadFromDeposit(strGUID As String) As Integer
    ''' <summary>
    ''' Uloží do tempu hodnotu klíče (fyzicky do p85DataPID)
    ''' </summary>
    ''' <param name="intDataPID"></param>
    ''' <returns>Vrací p85guid z temp tabulky p85tempbox</returns>
    ''' <remarks></remarks>
    Function SetToDeposit(intDataPID As Integer) As String
    Function SaveObjectReflection2Temp(strGUID As String, cRec As Object) As Boolean
    Function RunTailoredProcedure(strGUID As String, strProcName As String) As String
    Function RunTailoredProcedure(intRecordPID As Integer, strProcName As String) As Boolean
    Sub Recovery_ClearCompleteTemp()
End Interface
Class p85TempBoxBL
    Inherits BLMother
    Implements Ip85TempBoxBL
    Private WithEvents _cDL As DL.p85TempBoxDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.p85TempBoxDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Delete(cRec As BO.p85TempBox) As Boolean Implements Ip85TempBoxBL.Delete
        If _cDL.Delete(cRec) Then
            Return True
        Else
            _Error = _cDL.ErrorMessage
            Return False
        End If
    End Function
    Public Function UnDelete(cRec As BO.p85TempBox) As Boolean Implements Ip85TempBoxBL.UnDelete
        Return _cDL.UnDelete(cRec)
    End Function
    
    Public Function Truncate(strGUID As String) As Boolean Implements Ip85TempBoxBL.Truncate
        If _cDL.Truncate(strGUID) Then
            Return True
        Else
            _Error = _cDL.ErrorMessage
            Return False
        End If
    End Function

    Public Function GetList(strGUID As String, Optional bolIncludeDeleted As Boolean = False) As System.Collections.Generic.IEnumerable(Of BO.p85TempBox) Implements Ip85TempBoxBL.GetList
        Return _cDL.GetList(strGUID, bolIncludeDeleted)
    End Function

    Public Function Load(intPID As Integer) As BO.p85TempBox Implements Ip85TempBoxBL.Load
        If intPID <> 0 Then
            Return _cDL.Load(intPID)
        Else
            Return New BO.p85TempBox
        End If
    End Function
    Public Function LoadByGUID(strGUID As String) As BO.p85TempBox Implements Ip85TempBoxBL.LoadByGUID
        Return _cDL.LoadByGUID(strGUID)
    End Function

    Public Function Save(cRec As BO.p85TempBox) As Boolean Implements Ip85TempBoxBL.Save
        If cRec.p85GUID.Trim = "" Then
            _Error = "Chybí GUID."
        End If
        If _Error <> "" Then Return False
        Return _cDL.Save(cRec)
    End Function
    Sub Clone(strGUID_Source As String, strGUID_Dest As String) Implements Ip85TempBoxBL.Clone
        _cDL.Clone(strGUID_Source, strGUID_Dest)
    End Sub
    Function CloneOneRecord(intP85ID As Integer, strGUID_Dest As String) As Boolean Implements Ip85TempBoxBL.CloneOneRecord
        Return _cDL.CloneOneRecord(intP85ID, strGUID_Dest)
    End Function
    Function SetToDeposit(intDataPID As Integer) As String Implements Ip85TempBoxBL.SetToDeposit
        Return _cDL.SetToDeposit(intDataPID)
    End Function
    Function LoadFromDeposit(strGUID As String) As Integer Implements Ip85TempBoxBL.LoadFromDeposit
        Return _cDL.LoadFromDeposit(strGUID)
    End Function

    Function SaveObjectReflection2Temp(strGUID As String, cRec As Object) As Boolean Implements Ip85TempBoxBL.SaveObjectReflection2Temp
        Return _cDL.SaveObjectReflection2Temp(strGUID, cRec)
    End Function
    Public Overloads Function RunTailoredProcedure(strGUID As String, strProcName As String) As String Implements Ip85TempBoxBL.RunTailoredProcedure
        Return _cDL.RunTailoredProcedure(strGUID, strProcName)
    End Function
    Public Overloads Function RunTailoredProcedure(intRecordPID As Integer, strProcName As String) As Boolean Implements Ip85TempBoxBL.RunTailoredProcedure
        Return _cDL.RunTailoredProcedure(intRecordPID, strProcName)
    End Function

    Public Sub Recovery_ClearCompleteTemp() Implements Ip85TempBoxBL.Recovery_ClearCompleteTemp
        _cDL.Recovery_ClearCompleteTemp()
    End Sub
End Class
