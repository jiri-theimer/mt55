Imports System.Reflection
Imports System.IO

Public Class ASS
    Public Shared Function GetApplicationRootFolder() As String
        Dim s As String = AppDomain.CurrentDomain.BaseDirectory
        If Right(s, 1) = "\" Then
            Return Left(s, Len(s) - 1)
        Else
            Return s
        End If
    End Function
    
    Public Shared Function GetFrameworkVersion() As String
        Return Environment.Version.ToString
        
    End Function

    Public Shared Function GetUIVersion(Optional bolOnlyDateTime As Boolean = False) As String
        'Dim ass As [Assembly] = [Assembly].GetExecutingAssembly()
        'Dim a() As String = Split(ass.ToString, ",")
        'Dim strAppVer As String = a(1)
        Dim strAppVer As String = "5.0"
        Dim strFile As String = GetApplicationRootFolder() & "\bin\UI.dll"
        If System.IO.File.Exists(strFile) Then
            Dim info As New FileInfo(strFile)
            If bolOnlyDateTime Then
                Return Format(info.LastWriteTime, "yyyy-MM-dd HH:mm")
            Else
                Return strAppVer & ", build: " & Format(info.LastWriteTime, "yyyy/MM/dd HH:mm")
            End If

        Else
            Return strAppVer
        End If
    End Function

    Public Shared Function GetConfigVal(ByVal strKey As String, Optional ByVal strDefault As String = "") As String
        Dim s As String = System.Configuration.ConfigurationManager.AppSettings.Item(strKey)
        If s Is Nothing Then Return strDefault
        If s = "" Then
            Return strDefault
        Else
            Return s
        End If

    End Function
End Class
