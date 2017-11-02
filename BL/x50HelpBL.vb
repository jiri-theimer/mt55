Public Interface Ix50HelpBL
    Inherits IFMother
    Function Save(cRec As BO.x50Help, strUploadGUID As String) As Boolean
    Function Load(intPID As Integer) As BO.x50Help
    Function LoadByAspx(strAspxPageFileName As String) As BO.x50Help
    Function Delete(intPID As Integer) As Boolean
    Function GetList(Optional myQuery As BO.myQuery = Nothing) As IEnumerable(Of BO.x50Help)

End Interface
Class x50HelpBL
    Inherits BLMother
    Implements Ix50HelpBL
    Private WithEvents _cDL As DL.x50HelpDL

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub


    Public Sub New(ServiceUser As BO.j03User)
        _cDL = New DL.x50HelpDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    
   
    Public Function Delete(intPID As Integer) As Boolean Implements Ix50HelpBL.Delete
        Return _cDL.Delete(intPID)
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing) As System.Collections.Generic.IEnumerable(Of BO.x50Help) Implements Ix50HelpBL.GetList
        Return _cDL.GetList(myQuery)
    End Function


    Public Function Load(intPID As Integer) As BO.x50Help Implements Ix50HelpBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByAspx(strAspxPageFileName As String) As BO.x50Help Implements Ix50HelpBL.LoadByAspx
        If strAspxPageFileName = "" Then Return Nothing
        If Left(strAspxPageFileName, 1) = "/" Then strAspxPageFileName = Right(strAspxPageFileName, Len(strAspxPageFileName) - 1)
        Return _cDL.LoadByAspx(strAspxPageFileName)
    End Function

    Public Function Save(cRec As BO.x50Help, strUploadGUID As String) As Boolean Implements Ix50HelpBL.Save
        If Trim(cRec.x50Name) = "" Then
            _Error = "Název nápovědy je povinné pole."
        End If
        If Trim(cRec.x50AspxPage) = "" Then
            _Error = "Název ASPx stránky je povinné pole."
        End If
        If _Error <> "" Then Return False
        If _cDL.Save(cRec) Then
            Dim cF As New BO.clsFile
            Dim a() As String = Split(cRec.x50AspxPage, ".")
            a(0) += "_" & Format(Now, "yyyy-MM-dd-hh-mm-ss")
            cF.SaveText2File(Me.Factory.x35GlobalParam.UploadFolder & "\help_backup\" & a(0) & ".html", cRec.x50Html, , , False)
            cF.SaveText2File(Me.Factory.x35GlobalParam.UploadFolder & "\help_backup\" & a(0) & ".txt", cRec.x50PlainText, , , False)
            If strUploadGUID <> "" Then
                If Me.Factory.o27AttachmentBL.UploadAndSaveUserControl(Me.Factory.p85TempBoxBL.GetList(strUploadGUID, True), BO.x29IdEnum.x50Help, _LastSavedPID) Then

                End If
            End If
            Return True
        Else
            Return False
        End If
    End Function
End Class
