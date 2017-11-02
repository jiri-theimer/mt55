Public Interface Ix35GlobalParamBL
    Inherits IFMother
    Function Load(strKey As String, Optional bolCreateNewIfNothing As Boolean = False) As BO.x35GlobalParam
    Sub InhaleParams(x35keys As List(Of String))
    Sub InhaleParams(strKey1 As String, Optional strKey2 As String = "", Optional strKey3 As String = "")
    Function GetValueString(strKey As String, Optional strDefaultValue As String = "") As String
    Function GetValueInteger(strKey As String, Optional intDefaultValue As Integer = 0) As Integer
    Function Save(cRec As BO.x35GlobalParam) As Boolean
    Function UpdateValue(strKey As String, strValue As String) As Boolean
    ReadOnly Property UploadFolder As String
    ReadOnly Property TempFolder As String
    ReadOnly Property ExportFolder As String
    ReadOnly Property j27ID_Invoice As Integer
    ReadOnly Property UserAuthenticationMode As BO.UserAuthenticationModeEnum


    ''' <summary>
    ''' Vrací všechny globální APP parametry najednou
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetList() As List(Of BO.x35GlobalParam)
End Interface
Class x35GlobalParamBL
    Inherits BLMother
    Implements Ix35GlobalParamBL
    Private WithEvents _cDL As DL.x35GlobalParamDL
    Private _x35params As List(Of BO.x35GlobalParam) = Nothing

    Public Sub New(ServiceUser As BO.j03User)
        _cDL = New DL.x35GlobalParamDL(ServiceUser)
    End Sub
    
    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub
    Public Function Load(strKey As String, Optional bolCreateNewIfNothing As Boolean = False) As BO.x35GlobalParam Implements Ix35GlobalParamBL.Load
        InhaleParams(strKey)
        Dim lis As IEnumerable(Of BO.x35GlobalParam) = _cDL.GetList(strKey)
        If lis.Count > 0 Then
            Return lis(0)
        Else
            If bolCreateNewIfNothing Then
                Dim c As New BO.x35GlobalParam
                c.x35ModuleFlag = BO.ModuleEnum.Other
                c.x35Key = strKey
                Return c
            Else
                Return Nothing
            End If

        End If
    End Function
    Public Overloads Sub InhaleParams(x35keys As List(Of String)) Implements Ix35GlobalParamBL.InhaleParams
        _x35params = _cDL.GetList(x35keys).ToList
    End Sub
    Public Overloads Sub InhaleParams(strKey1 As String, Optional strKey2 As String = "", Optional strKey3 As String = "") Implements Ix35GlobalParamBL.InhaleParams
        _x35params = _cDL.GetList(strKey1, strKey2, strKey3).ToList
    End Sub

    Public Function GetValueString(strKey As String, Optional strDefaultValue As String = "") As String Implements Ix35GlobalParamBL.GetValueString
        If _x35params Is Nothing Then
            _x35params = GetList().ToList  'pro jistotu načíst všechny parametry
        End If
        Dim cRec As BO.x35GlobalParam = _x35params.Find(Function(p As BO.x35GlobalParam) p.x35Key = strKey)
        If cRec Is Nothing Then
            Return strDefaultValue
        Else
            Return cRec.x35Value
        End If
    End Function
    Public Function GetValueInteger(strKey As String, Optional intDefaultValue As Integer = 0) As Integer Implements Ix35GlobalParamBL.GetValueInteger
        Dim s As String = GetValueString(strKey)
        If BO.BAS.IsNullInt(s) = 0 Then
            Return intDefaultValue
        Else
            Return BO.BAS.IsNullInt(s)
        End If
    End Function
    Public Function GetList() As List(Of BO.x35GlobalParam) Implements Ix35GlobalParamBL.GetList
        _x35params = _cDL.GetList().ToList
        Return _x35params
    End Function
    Public Function Save(cRec As BO.x35GlobalParam) As Boolean Implements Ix35GlobalParamBL.Save

        Return _cDL.Save(cRec)
    End Function
    Public Function UpdateValue(strKey As String, strValue As String) As Boolean Implements Ix35GlobalParamBL.UpdateValue
        Return _cDL.UpdateValue(strKey, strValue)
    End Function

    Public ReadOnly Property UserAuthenticationMode As BO.UserAuthenticationModeEnum Implements Ix35GlobalParamBL.UserAuthenticationMode
        Get
            Select Case GetValueString("UserAuthenticationMode", "3")
                Case "1"
                    Return BO.UserAuthenticationModeEnum.MixedMode
                Case "2"
                    Return BO.UserAuthenticationModeEnum.WindowsOnly
                Case "3"
                    Return BO.UserAuthenticationModeEnum.AnonymousOnly
                Case Else
                    Return BO.UserAuthenticationModeEnum.AnonymousOnly
            End Select

        End Get
    End Property
    Public ReadOnly Property UploadFolder As String Implements Ix35GlobalParamBL.UploadFolder
        Get
            Return GetValueString("Upload_Folder")
        End Get
    End Property
    Public ReadOnly Property TempFolder As String Implements Ix35GlobalParamBL.TempFolder
        Get
            Dim s As String = GetValueString("Upload_Folder") & "\TEMP"
            If Not System.IO.Directory.Exists(s) Then
                Try
                    System.IO.Directory.CreateDirectory(s)
                Catch ex As Exception
                    _Error = ex.Message
                End Try

            End If
            Return s
        End Get
    End Property
    Public ReadOnly Property ExportFolder As String Implements Ix35GlobalParamBL.ExportFolder
        Get
            Dim s As String = GetValueString("Upload_Folder") & "\EXPORT"
            If Not System.IO.Directory.Exists(s) Then
                Try
                    System.IO.Directory.CreateDirectory(s)
                Catch ex As Exception
                    _Error = ex.Message
                End Try

            End If
            Return s
        End Get
    End Property
    Public ReadOnly Property j27ID_Invoice As Integer Implements Ix35GlobalParamBL.j27ID_Invoice
        Get
            Return GetValueInteger("j27ID_Invoice", 2)
        End Get
    End Property
End Class
