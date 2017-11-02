
Public Interface Io27AttachmentBL
    Inherits IFMother
    Function UpdateRecord(cRec As BO.o27Attachment) As Boolean
    Function Load(intPID As Integer) As BO.o27Attachment
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQueryO27) As IEnumerable(Of BO.o27Attachment)

    Function UploadAndSaveUserControl(lisTempFiles As IEnumerable(Of BO.p85TempBox), x29id As BO.x29IdEnum, intDataPID As Integer) As Boolean
    Function UploadAndSaveOneFile(cRec As BO.o27Attachment, strOrigFileName As String, strOrigFullPath As String, Optional strExplicitArchiveFileName As String = "") As Boolean
    Function UploadAndSaveOnFile2Temp(strUploadGUID As String, strSourceFullPath As String, intO13ID As Integer) As Boolean
    Sub CopyRecordsToTemp(strGUID As String, intDataPID As Integer, x29id As BO.x29IdEnum)
End Interface
Class o27AttachmentBL
    Inherits BLMother
    Implements Io27AttachmentBL
    Private WithEvents _cDL As DL.o27AttachmentDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o27AttachmentDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function UpdateRecord(cRec As BO.o27Attachment) As Boolean Implements Io27AttachmentBL.UpdateRecord
        Return _cDL.UpdateOnly(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o27Attachment Implements Io27AttachmentBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements Io27AttachmentBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQueryO27) As IEnumerable(Of BO.o27Attachment) Implements Io27AttachmentBL.GetList
        Return _cDL.GetList(mq)
    End Function

    Public Function UploadAndSaveUserControl(lisTempFiles As IEnumerable(Of BO.p85TempBox), x29id As BO.x29IdEnum, intDataPID As Integer) As Boolean Implements Io27AttachmentBL.UploadAndSaveUserControl
        Dim strTempDir As String = Me.Factory.x35GlobalParam.TempFolder, x As Integer = 0
        If lisTempFiles.Count = 0 Then
            Return True
        End If
        For Each cTMP As BO.p85TempBox In lisTempFiles
            If cTMP.p85DataPID = 0 Then
                If Not cTMP.p85IsDeleted Then
                    Dim cRec As New BO.o27Attachment
                    With cRec
                        Select Case x29id
                            Case BO.x29IdEnum.b07Comment
                                .b07ID = intDataPID
                            Case BO.x29IdEnum.x31Report
                                .x31ID = intDataPID
                            Case BO.x29IdEnum.x50Help
                                .x50ID = intDataPID
                            Case BO.x29IdEnum.x40MailQueue
                                .x40ID = intDataPID
                        End Select
                        .o13ID = cTMP.p85OtherKey1
                        ''.o27GUID = cTMP.p85GUID
                        .o27GUID = BO.BAS.GetGUID   'zajištění jednoznačnosti souboru
                        .o27ContentType = cTMP.p85FreeText03
                        .o27OriginalFileName = cTMP.p85FreeText01

                        .o27Name = cTMP.p85FreeText04
                    End With
                    
                    
                    Dim strFullPath As String = strTempDir & "\" & cTMP.p85FreeText02
                    If Not UploadAndSaveOneFile(cRec, cTMP.p85FreeText01, strFullPath, cTMP.p85FreeText02) Then
                        'log4net.LogManager.GetLogger("debuglog").Error("UploadAndSaveOneFile: " & _Error & vbCrLf & "strFullPath: " & strFullPath & ", orig file name: " & cTMP.p85FreeText01)
                    End If
                End If
            Else
                Dim cRec As BO.o27Attachment = Load(cTMP.p85DataPID)
                ''cRec.o27GUID = cTMP.p85GUID
                cRec.o27GUID = BO.BAS.GetGUID   'zajištění jednoznačnosti souboru
                cRec.o27Name = cTMP.p85FreeText04
                If cTMP.p85OtherKey1 <> 0 Then cRec.o13ID = cTMP.p85OtherKey1
                If cTMP.p85IsDeleted Then
                    If Not cRec Is Nothing Then Delete(cRec.PID)
                Else
                    UpdateRecord(cRec)
                End If
            End If
            ''If x > 0 Then Threading.Thread.Sleep(1000 * 1) 'počkat po dobu 1 sekundy, jinak to zlobí
            x += 1
        Next

        Return True
    End Function

    Public Function UploadAndSaveOnFile2Temp(strUploadGUID As String, strSourceFullPath As String, intO13ID As Integer) As Boolean Implements Io27AttachmentBL.UploadAndSaveOnFile2Temp
        Dim cRec As New BO.p85TempBox(), cF As New BO.clsFile, strTempDir As String = Factory.x35GlobalParam.TempFolder
        Dim strFileName As String = cF.GetNameFromFullpath(strSourceFullPath)
        cRec.p85GUID = strUploadGUID
        cRec.p85Prefix = "o27"
        cRec.p85OtherKey1 = intO13ID
        'cRec.p85FreeText06 = cO13.o13Name

        cRec.p85FreeText01 = strFileName
        cRec.p85FreeText02 = strFileName
        'cRec.p85FreeText03 = validFile.ContentType
        'cRec.p85FreeText04 = validFile.GetFieldValue("Title")
        cRec.p85FreeNumber01 = cF.GetFileSize(strSourceFullPath)

        Factory.p85TempBoxBL.Save(cRec)
        If cF.CopyFile(strSourceFullPath, strTempDir & "\" & strFileName) Then
            Return True
        End If
        Return False
    End Function
    Public Function UploadAndSaveOneFile(cRec As BO.o27Attachment, strOrigFileName As String, strSourceServerFullPath As String, Optional strExplicitArchiveFileName As String = "") As Boolean Implements Io27AttachmentBL.UploadAndSaveOneFile
        _Error = ""
        Dim cFile As New BO.clsFile

        If Not cFile.FileExist(strSourceServerFullPath) Then
            _Error = "Předávaný soubor neexistuje."
            Return False
        End If
        'Dim strDir As String = BO.AppAssembly.GetUploadFolder()
        Dim strDir As String = Me.Factory.x35GlobalParam.UploadFolder
        If strDir = "" Then
            _Error = "V konfiguraci systému chybí definice proměnné 'UploadFolder'."
            Return False
        End If
        Dim strArchiveFolder As String = GetArchiveFolder(cRec)
        If strArchiveFolder <> "" Then
            strDir += "\" & strArchiveFolder
        End If


        If Not System.IO.Directory.Exists(strDir) Then
            Try
                System.IO.Directory.CreateDirectory(strDir)
            Catch ex As Exception
                _Error = ex.Message
                Return False
            End Try
        End If

        If _Error <> "" Then Return False

        Dim intFileSize As Integer = cFile.GetFileSize(strSourceServerFullPath)
        If strOrigFileName = "" Then
            strOrigFileName = cFile.GetNameFromFullpath(strSourceServerFullPath)
        End If
        Dim strExtension As String = cFile.GetFileExtension(strSourceServerFullPath)

        If intFileSize = 0 Then _Error = "Velikost souboru je 0b."
        If strOrigFileName = "" Then _Error = "Název souboru je povinné pole."

        If _Error <> "" Then Return False

        Dim strArchiveFileName As String = cRec.o27ArchiveFileName
        If strArchiveFileName = "" Then strArchiveFileName = strExplicitArchiveFileName
        If cRec.PID = 0 Then
            'strArchive = Format(Now, "ddMMyyyyhhmmssfff") & "_" & strOrigFileName
            If strArchiveFileName = "" Then
                If cRec.o27GUID <> "" Then
                    strArchiveFileName = cRec.o27GUID & "_" & cFile.GetNameFromFullpath(strSourceServerFullPath)
                Else
                    strArchiveFileName = BO.BAS.GetGUID & "_" & cFile.GetNameFromFullpath(strSourceServerFullPath)
                End If
            End If
            strArchiveFolder = GetArchiveFolder(cRec)
        Else
            strArchiveFolder = cRec.o27ArchiveFolder
        End If
       
       

        'log4net.LogManager.GetLogger("debuglog").Debug("Blíží se _cDL.UploadAndSave, strArchiveFolder: " & strArchiveFolder & ", strArchiveFileName: " & strArchiveFileName)

        If _cDL.UploadAndSave(cRec, strOrigFileName, strExtension, intFileSize, strArchiveFileName, strArchiveFolder) Then
            cFile.CopyFile(strSourceServerFullPath, strDir & "\" & strArchiveFileName)

            Return True
        Else
            _Error = _cDL.ErrorMessage
            Return False
        End If
    End Function

    Private Function GetArchiveFolder(cRec As BO.o27Attachment) As String
        Dim d As Date = Now, strFolderMask As String = ""
        With cRec
            If .PID = 0 Then
                If .o13ID <> 0 Then
                    Dim cO13 As BO.o13AttachmentType = Me.Factory.o13AttachmentTypeBL.Load(.o13ID)
                    If cO13.o13ArchiveFolder <> "" Then
                        strFolderMask = cO13.o13ArchiveFolder
                    End If
                    If cO13.o13IsArchiveFolderWithPeriodSuffix Then
                        If strFolderMask <> "" Then strFolderMask += "\"
                        strFolderMask += Year(Now).ToString & "\" & Right("0" & Month(Now).ToString, 2)
                    End If
                End If
            Else
                strFolderMask = .o27ArchiveFolder
            End If
        End With

        Return strFolderMask

    End Function

    Public Sub CopyRecordsToTemp(strGUID As String, intDataPID As Integer, x29id As BO.x29IdEnum) Implements Io27AttachmentBL.CopyRecordsToTemp
        _cDL.CopyRecordsToTemp(strGUID, x29id, intDataPID)
    End Sub
End Class
