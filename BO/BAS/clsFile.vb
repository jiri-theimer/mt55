Imports System.IO
Imports System.Security.AccessControl


Public Class clsFile
    Private _Error As String
    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Function GetFileContents(ByVal FullPath As String, Optional ByRef ErrInfo As String = "", Optional ByVal bolWin1250 As Boolean = True, Optional bolReadFirstLineOnly As Boolean = False) As String

        Dim strContents As String
        Dim objReader As StreamReader
        Try

            If bolWin1250 Then
                objReader = New StreamReader(FullPath, System.Text.Encoding.GetEncoding(1250))
            Else
                objReader = New StreamReader(FullPath, System.Text.Encoding.UTF8)
            End If
            If bolReadFirstLineOnly Then
                strContents = objReader.ReadLine()
            Else
                strContents = objReader.ReadToEnd()
            End If

            objReader.Close()
            Return strContents

        Catch Ex As Exception
            _Error = Ex.Message
            ErrInfo = Ex.Message
        End Try

        Return ""

    End Function



    Public Function SaveText2File(ByVal FullPath As String, ByVal strText As String, Optional ByVal bolAppend As Boolean = False, Optional ByRef ErrInfo As String = "", Optional ByVal bolWin1250 As Boolean = True) As Boolean
        Dim objWriter As StreamWriter
        Try
            If bolWin1250 Then
                objWriter = New StreamWriter(FullPath, bolAppend, System.Text.Encoding.GetEncoding(1250))
            Else
                objWriter = New StreamWriter(FullPath, bolAppend, System.Text.Encoding.UTF8)
            End If
            objWriter.Write(strText)
            objWriter.Close()

            Return True

        Catch ex As Exception
            ErrInfo = ex.Message
            _Error = ex.Message
        End Try
        Return False
    End Function

    Public Function DeleteFile(ByVal FullPath As String) As Boolean
        If File.Exists(FullPath) Then
            Try
                File.Delete(FullPath)
                Return True
            Catch ex As Exception
                _Error = ex.Message
            End Try
        Else
            _Error = "File not found"
        End If
        Return False
    End Function

    Public Function GetNameFromFullpath(ByVal FullPath As String, Optional ByVal bolExcludeSuffix As Boolean = False, Optional ByRef strRetSuffix As String = "") As String
        Dim a() As String = Split(FullPath, "\")
        Dim s As String = a(UBound(a))
        strRetSuffix = Right(FullPath, 3)
        If bolExcludeSuffix Then
            a = Split(s, ".")
            s = a(0)
            strRetSuffix = a(UBound(a))
        End If
        Return s
    End Function

    Public Function GetFileDirectory(ByVal FullPath As String) As String
        Dim a() As String = Split(FullPath, "\")
        Dim i As Integer, s As String = ""
        For i = 0 To UBound(a) - 1
            If s <> "" Then
                s += "\" & a(i)
            Else
                s = a(i)
            End If

        Next
        Return s
    End Function


    Public Function GetFileSize(ByVal FullPath As String) As Long
        Try
            Dim info As New FileInfo(FullPath)
            Return info.Length
        Catch ex As Exception
            _Error = ex.Message
            Return 0
        End Try
    End Function

    Public Function GetFileExtension(ByVal FullPath As String) As String
        Try
            Dim info As New FileInfo(FullPath)
            Return info.Extension
        Catch ex As Exception
            _Error = ex.Message
            Return ""
        End Try
    End Function

    Public Function RenameFile(ByVal FullPath As String, ByVal NewPath As String) As Boolean
        Try
            File.Move(FullPath, NewPath)
            Return True
        Catch ex As System.IO.IOException
            _Error = ex.Message
        End Try
        Return False
    End Function
    Public Function RenameFolder(strSourceDir As String, strDestDir As String) As Boolean
        Try
            Directory.Move(strSourceDir, strDestDir)
            Return True
        Catch ex As Exception
            _Error = ex.Message
        End Try
        Return False
    End Function
    Public Function CopyFile(ByVal FullPath As String, ByVal NewPath As String) As Boolean
        Try
            File.Copy(FullPath, NewPath, True)
            Return True
        Catch ex As System.IO.IOException
            _Error = ex.Message
        End Try
        Return False
    End Function


    Public Function FileExist(ByVal FullPath As String) As Boolean
        Return File.Exists(FullPath)
    End Function


    Public Function GetFileListFromDir(ByVal strDir As String, ByVal strMask As String, Optional so As SearchOption = SearchOption.TopDirectoryOnly, Optional bolRetFullPath As Boolean = False) As List(Of String)
        Dim lis As New List(Of String)
        If Not IO.Directory.Exists(strDir) Then Return lis


        Dim di As New IO.DirectoryInfo(strDir)

        Dim diar1 As IO.FileInfo() = di.GetFiles(strMask, so)
        Dim dra As IO.FileInfo, s As String = ""
        For Each dra In diar1
            If bolRetFullPath Then
                lis.Add(dra.FullName)
            Else
                lis.Add(dra.Name)
            End If
            
        Next
        Return lis

    End Function

    Public Function GetContentType(ByVal strFullPath As String) As String
        Dim strExt As String = LCase(GetFileExtension(strFullPath))
        If Left(strExt, 1) = "." Then strExt = Right(strExt, Len(strExt) - 1)
        Select Case strExt
            Case "" : Return ""
            Case "pdf" : Return "application/pdf"
            Case "doc" : Return "application/msword"
            Case "docx" : Return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case "xlsx" : Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case "jpg", "jpeg" : Return "image/jpeg"
            Case "gif", "png", "bmp" : Return "image/" & strExt
            Case "msg" : Return "message/rfc822"
            Case "txt" : Return "text/plain"
            Case "htm", "html" : Return "text/html"
            Case Else
                Return ("application/." & strExt).Replace("..", ".")
        End Select
    End Function

    

    Public Function GetBinaryContent(strFullPath As String) As Byte()
        If Not File.Exists(strFullPath) Then Return Nothing

        Dim fi As FileInfo = New FileInfo(strFullPath)
        Dim sr As New StreamReader(strFullPath)
        Dim reader As New BinaryReader(sr.BaseStream)

        Dim ret As Byte() = reader.ReadBytes(reader.BaseStream.Length())
        reader.Close()

        Return ret


    End Function
    Public Function GetBinaryPart(strFullPath As String, intStartZeroIndex As Integer, intPartSize As Integer) As Byte()
        ''Dim bytesall() As Byte = GetBinaryContent(strFullPath)
        Dim buffer() As Byte = New Byte(intPartSize - 1) {}

        Using fs As New FileStream(strFullPath, FileMode.Open, FileAccess.Read, FileShare.None)
            If fs.Length < intStartZeroIndex + buffer.Length Then
                buffer = New Byte(fs.Length - intStartZeroIndex - 1) {}
            End If
            fs.Seek(intStartZeroIndex, SeekOrigin.Begin)
            fs.Read(buffer, 0, buffer.Length)

        End Using
        Return buffer

    End Function

    Public Function SaveBinary2File(b() As Byte, strFullPath As String) As Boolean

        System.IO.File.WriteAllBytes(strFullPath, b)
        If File.Exists(strFullPath) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Sub AppendAllBytesToFile(path As String, bytes As Byte())
        'argument-checking here.

        Using stream = New FileStream(path, FileMode.Append)
            stream.Write(bytes, 0, bytes.Length)
        End Using
    End Sub

    Public Function CreateDirectoryWithSecurity(strDir As String, lisIdentities As List(Of String), bolRead As Boolean, bolFullControl As Boolean)
        Dim securityRules As DirectorySecurity = New DirectorySecurity()
        For Each strIdentity As String In lisIdentities
            If bolRead Then
                securityRules.AddAccessRule(New FileSystemAccessRule(strIdentity, FileSystemRights.Read, AccessControlType.Allow))
            End If
            If bolFullControl Then
                securityRules.AddAccessRule(New FileSystemAccessRule(strIdentity, FileSystemRights.FullControl, AccessControlType.Allow))
            End If
        Next
        If Directory.Exists(strDir) Then
            Directory.SetAccessControl(strDir, securityRules)
        Else
            Dim di As DirectoryInfo = Directory.CreateDirectory(strDir, securityRules)

        End If

        

        Return True
    End Function


End Class




