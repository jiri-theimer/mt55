Public Class clsDbBackup
    Public IsUseDumpDevice As Boolean = True
    Private _Error As String

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property

    Public Function MakeBackup(ByVal strCon As String, ByVal strBackupFileName As String, ByVal strBackupDir As String, Optional ByVal intTimeout As Integer = -1, Optional bolTestFileSystem As Boolean = True) As Boolean
        Dim strDB As String = ParseDbNameFromConString(strCon)
        If strDB = "" Then
            _Error = "nelze z connectstringu poznat jméno databáze"
            Return False
        End If

        If bolTestFileSystem Then
            If Not System.IO.Directory.Exists(strBackupDir) Then
                _Error = "Directory '" & strBackupDir & "' doesn't exist!"
                Return False
            End If
        End If
        
        If Right(strBackupDir, 1) = "\" Then strBackupDir = Left(strBackupDir, Len(strBackupDir) - 1)
        Dim cF As New BO.clsFile, strFullBackupPath As String = strBackupDir & "\" & strBackupFileName
        If bolTestFileSystem Then
            If cF.FileExist(strFullBackupPath) Then
                If Not cF.DeleteFile(strFullBackupPath) Then
                    _Error = cF.ErrorMessage
                    Return False
                End If
            End If
        End If
        

        Dim strSQL As String
        Dim cDB As New DL.DbHandler

        cDB.ChangeConString(strCon)
        If cDB.ErrorMessage <> "" Then
            _Error = cDB.ErrorMessage
            Return False
        End If

        If IsUseDumpDevice Then
            strSQL = "EXEC sp_addumpdevice 'disk','" & strDB & "_appbackup','" & strFullBackupPath & "'"
            cDB.RunSQL(strSQL, intTimeout)

            If cDB.ErrorMessage <> "" Then
                If cDB.ErrorCode = 15061 Then
                    'backup zařízení již existuje
                    strSQL = "sp_dropdevice '" & strDB & "_appbackup'"
                    cDB.RunSQL(strSQL, intTimeout)
                    strSQL = "EXEC sp_addumpdevice 'disk','" & strDB & "_appbackup','" & strFullBackupPath & "'"
                    cDB.RunSQL(strSQL, intTimeout)
                Else
                    _Error = cDB.ErrorMessage
                    Return False
                End If
            End If
            strSQL = "BACKUP DATABASE " & strDB & " TO " & strDB & "_appbackup"
        Else
            strSQL = "BACKUP DATABASE " & strDB & " TO DISK='" & strFullBackupPath & "' WITH DESCRIPTION='epis', INIT, NAME='epis'"
        End If
        cDB.RunSQL(strSQL, intTimeout)
        If cDB.ErrorMessage <> "" Then
            _Error = cDB.ErrorMessage
            Return False
        End If

        If IsUseDumpDevice Then
            strSQL = "sp_dropdevice '" & strDB & "_appbackup'"
            cDB.RunSQL(strSQL)
            If cDB.ErrorMessage <> "" Then
                _Error = cDB.ErrorMessage
                Return False
            End If
        End If
        If Not bolTestFileSystem Then
            Return True
        End If

        If System.IO.File.Exists(strFullBackupPath) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function ParseDbNameFromConString(ByVal strCon As String) As String
        Dim a() As String = Split(strCon, ";"), i As Integer, strDB As String = ""
        For i = 0 To UBound(a)
            If LCase(a(i)).IndexOf("database") >= 0 Then
                Dim b() As String = Split(a(i), "=")
                If UBound(b) > 0 Then
                    Return b(1)
                End If

            End If
        Next
        Return ""

    End Function

End Class
