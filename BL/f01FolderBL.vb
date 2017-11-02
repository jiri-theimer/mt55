
Public Interface If01FolderBL
    Inherits IFMother
    Function CreateUpdateFolder(intRecordPID As Integer, intF02ID As Integer) As Boolean
    Function Load(intPID As Integer) As BO.f01Folder
    Function LoadByEntity(intRecordPID As Integer, intF02ID As Integer) As BO.f01Folder
    Function Load_f02(intF02ID As Integer) As BO.f02FolderType
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery, Optional intRecordPID As Integer = 0, Optional x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified) As IEnumerable(Of BO.f01Folder)
    Function GetList_f02(mq As BO.myQuery) As IEnumerable(Of BO.f02FolderType)
End Interface
Class f01FolderBL
    Inherits BLMother
    Implements If01FolderBL
    Private WithEvents _cDL As DL.f01FolderDL
    Private _cF As BO.clsFile

    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cF = New BO.clsFile
        _cDL = New DL.f01FolderDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function CreateUpdateFolder(intRecordPID As Integer, intF02ID As Integer) As Boolean Implements If01FolderBL.CreateUpdateFolder
        If intRecordPID = 0 Or intF02ID = 0 Then
            _Error = "Na vstupu chybí f01RecordPID nebo intF02ID." : Return False
        End If
        Dim cRec As BO.f01Folder = LoadByEntity(intRecordPID, intF02ID)
        If cRec Is Nothing Then
            cRec = New BO.f01Folder
            cRec.f01RecordPID = intRecordPID
            cRec.f02ID = intF02ID
        End If

        Dim mq As New BO.myQuery
        mq.Closed = BO.BooleanQueryMode.NoQuery
        Dim lisF02 As IEnumerable(Of BO.f02FolderType) = GetList_f02(mq)

        Dim cF02 As BO.f02FolderType = lisF02.Where(Function(p) p.PID = cRec.f02ID)(0), objects As New List(Of Object)
        If cF02.IsClosed Then
            _Error = String.Format("Typ složky [{0}] byl zneplatněn.", cF02.f02Name) : Return False
        End If
        Dim strFolderName As String = cF02.f02Mask, strPrefix As String = BO.BAS.GetDataPrefix(cF02.x29ID)
        If strFolderName = "" Then strFolderName = "[%" & strPrefix & "name%]"
        Select Case cF02.x29ID
            Case BO.x29IdEnum.p56Task
                objects.Add(Factory.p56TaskBL.Load(cRec.f01RecordPID))
            Case BO.x29IdEnum.j02Person
                objects.Add(Factory.j02PersonBL.Load(cRec.f01RecordPID))
            Case BO.x29IdEnum.p41Project
                objects.Add(Factory.p41ProjectBL.Load(cRec.f01RecordPID))
            Case BO.x29IdEnum.p28Contact
                objects.Add(Factory.p28ContactBL.Load(cRec.f01RecordPID))
            Case BO.x29IdEnum.p91Invoice
                objects.Add(Factory.p91InvoiceBL.Load(cRec.f01RecordPID))
            Case BO.x29IdEnum.o23Doc
                objects.Add(Factory.o23DocBL.Load(cRec.f01RecordPID))
        End Select

        Dim cM As New BO.clsMergeContent, dirs As New List(Of String), intF01ID As Integer = cRec.PID
        Dim lisChilds As IEnumerable(Of BO.f02FolderType) = lisF02.Where(Function(p) p.f02ParentID = cRec.f02ID And p.IsClosed = False)
        Dim strExistingFolder As String = FindExistingFolder(cF02, cRec)

        strFolderName = cM.MergeContent(objects, strFolderName, "")
        dirs.Add(cF02.f02RootPath & "\" & strFolderName)
        cRec.f01Name = strFolderName
        If _cDL.Save(cRec) Then
            If intF01ID = 0 Then intF01ID = _cDL.LastSavedRecordPID
        Else
            Return False
        End If

        If strExistingFolder <> "" Then
            If LCase(dirs(0)) = LCase(strExistingFolder) Then
                If lisChilds.Count = 0 Then Return True 'složka existuje -> není třeba zakládat nebo přejmenovávat
            Else
                _cF.RenameFolder(strExistingFolder, dirs(0)) 'složka projektu již existuje s jiným názvem -> je nutné ji přejmenovat
                If lisChilds.Count = 0 Then Return True
            End If

        End If

        For Each c In lisChilds  'případné pod-složky
            If c.f02Mask = "" Then c.f02Mask = c.f02Name
            strFolderName = cM.MergeContent(objects, c.f02Mask, "")
            dirs.Add(dirs(0) & "\" & strFolderName)
        Next
        For i As Integer = 0 To dirs.Count - 1
            Dim strDir As String = dirs(i)
            If Not System.IO.Directory.Exists(strDir) Then
                Try
                    System.IO.Directory.CreateDirectory(strDir)
                Catch ex As Exception
                    _Error = ex.Message : Return False
                End Try
            End If
            If i = 0 Then
                'pro složku na TOP úrovni uložíme info soubor info.mt
                CreateUpdateInfoMT(strDir, strPrefix, cRec)
            End If
        Next

        If intF01ID > 0 Then
            Return True
        Else
            Return False
        End If

    End Function
    Private Function CreateUpdateInfoMT(strDir As String, strPrefix As String, cRec As BO.f01Folder) As String
        Dim strMTInfoPath As String = strDir & "\info.mt"
        If System.IO.File.Exists(strMTInfoPath) Then
            System.IO.File.SetAttributes(strMTInfoPath, IO.FileAttributes.Normal)
        End If
        If Not _cF.SaveText2File(strMTInfoPath, "prefix: " & strPrefix & ", pid: " & cRec.f01RecordPID.ToString & vbCrLf & "timestamp: " & _cUser.j03Login & "/" & Now.ToString, False, , False) Then
            _Error = _cF.ErrorMessage : Return ""
        End If
        System.IO.File.SetAttributes(strMTInfoPath, IO.FileAttributes.Hidden)
        Return strMTInfoPath
    End Function
    Private Function FindExistingFolder(cF02 As BO.f02FolderType, cRec As BO.f01Folder) As String
        Dim lisSaved As IEnumerable(Of BO.f01Folder) = GetList(New BO.myQuery, cRec.f01RecordPID, cF02.x29ID)

        If lisSaved.Count > 0 Then
            If System.IO.Directory.Exists(cF02.f02RootPath & "\" & lisSaved(0).f01Name) Then
                Return cF02.f02RootPath & "\" & lisSaved(0).f01Name
            End If

        End If
        Return ""
        'je příliš pomalé, zastaveno:
        ''Dim strPrefix As String = BO.BAS.GetDataPrefix(cF02.x29ID)
        ''Dim marks As List(Of String) = _cF.GetFileListFromDir(cF02.f02RootPath, "info.mt", IO.SearchOption.AllDirectories, True)
        ''If marks.Count > 0 Then
        ''    'otestovat, zda již dříve nebyla vytvořena složka pro entituy x29ID a PID
        ''    For Each s In marks
        ''        Dim strFirstLine As String = _cF.GetFileContents(s, , False, True)
        ''        If strFirstLine = "prefix: " & strPrefix & ", pid: " & cRec.f01RecordPID.ToString Then
        ''            Return _cF.GetFileDirectory(s)
        ''        End If
        ''    Next
        ''End If
        ''Return ""
    End Function
    Public Function Load(intPID As Integer) As BO.f01Folder Implements If01FolderBL.Load
        Return _cDL.Load(intPID)
    End Function
    Function LoadByEntity(intRecordPID As Integer, intF02ID As Integer) As BO.f01Folder Implements If01FolderBL.LoadByEntity
        Return _cDL.LoadByEntity(intRecordPID, intF02ID)
    End Function
    Public Function Load_f02(intF02ID As Integer) As BO.f02FolderType Implements If01FolderBL.Load_f02
        Return _cDL.Load_f02(intF02ID)
    End Function
    Public Function Delete(intPID As Integer) As Boolean Implements If01FolderBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery, Optional intRecordPID As Integer = 0, Optional x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified) As IEnumerable(Of BO.f01Folder) Implements If01FolderBL.GetList
        Return _cDL.GetList(mq, intRecordPID, x29ID)
    End Function
    Public Function GetList_f02(mq As BO.myQuery) As IEnumerable(Of BO.f02FolderType) Implements If01FolderBL.GetList_f02
        Return _cDL.GetList_f02(mq)
    End Function
End Class
