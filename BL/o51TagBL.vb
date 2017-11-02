
Public Interface Io51TagBL
    Inherits IFMother
    Function Save(cRec As BO.o51Tag) As Boolean
    Function Load(intPID As Integer) As BO.o51Tag
    Function LoadByName(strName As String, intExcludePID As Integer) As BO.o51Tag
    Function Delete(intPID As Integer) As Boolean
    Function GetList(mq As BO.myQuery, strPrefix As String, usedInO52 As BO.BooleanQueryMode) As IEnumerable(Of BO.o51Tag)
    Function GetList_o52(strPrefix As String, intRecordPID As Integer) As IEnumerable(Of BO.o52TagBinding)
    Function SaveBinding(strPrefix As String, intRecordPID As Integer, o51IDs As List(Of Integer)) As Boolean
    Function SaveBatch(strPrefix As String, pids As List(Of Integer), o51IDs As List(Of Integer), bolReplace As Boolean) As Boolean
    Function ParseQueryByTags(strParsePrefix As String, strQuery As String) As BO.QueryByTags
End Interface
Class o51TagBL
    Inherits BLMother
    Implements Io51TagBL
    Private WithEvents _cDL As DL.o51TagDL


    Private Sub _cDL_OnError(strError As String) Handles _cDL.OnError
        _Error = strError
    End Sub
    Private Sub _cDL_OnSaveRecord(intLastSavedPID As Integer) Handles _cDL.OnSaveRecord
        _LastSavedPID = intLastSavedPID
    End Sub

    Public Sub New(ServiceUser As BO.j03UserSYS)
        _cDL = New DL.o51TagDL(ServiceUser)
        _cUser = ServiceUser
    End Sub
    Public Function Save(cRec As BO.o51Tag) As Boolean Implements Io51TagBL.Save
        With cRec
            If Trim(.o51Name) = "" Then
                _Error = "Chybí název štítku." : Return False
            End If
            If .PID = 0 Then
                If .j02ID_Owner = 0 Then .j02ID_Owner = _cUser.j02ID
                If Not _cDL.LoadByName(cRec.o51Name, cRec.PID) Is Nothing Then
                    _Error = String.Format("Štítek s názvem '{0}' již existuje!", .o51Name) : Return False
                End If
            End If
            If .o51ScopeFlag = 0 Then
                If Not (.o51IsJ02 Or .o51IsO23 Or .o51IsP28 Or .o51IsP31 Or .o51IsP41 Or .o51IsP56 Or .o51IsP56 Or .o51IsP90 Or .o51IsP91) Then
                    .o51ScopeFlag = 1
                End If
            End If
        End With
        
        
        Return _cDL.Save(cRec)
    End Function
    Public Function Load(intPID As Integer) As BO.o51Tag Implements Io51TagBL.Load
        Return _cDL.Load(intPID)
    End Function
    Public Function LoadByName(strName As String, intExcludePID As Integer) As BO.o51Tag Implements Io51TagBL.LoadByName
        Return _cDL.LoadByName(strName, intExcludePID)
    End Function
    
    Public Function Delete(intPID As Integer) As Boolean Implements Io51TagBL.Delete
        Return _cDL.Delete(intPID)
    End Function
    Public Function GetList(mq As BO.myQuery, strPrefix As String, usedInO52 As BO.BooleanQueryMode) As IEnumerable(Of BO.o51Tag) Implements Io51TagBL.GetList
        Return _cDL.GetList(mq, strPrefix, usedInO52)
    End Function
    Public Function GetList_o52(strPrefix As String, intRecordPID As Integer) As IEnumerable(Of BO.o52TagBinding) Implements Io51TagBL.GetList_o52
        Return _cDL.GetList_o52(strPrefix, intRecordPID)
    End Function
    Public Function SaveBinding(strPrefix As String, intRecordPID As Integer, o51IDs As List(Of Integer)) As Boolean Implements Io51TagBL.SaveBinding
        Return _cDL.SaveBinding(strPrefix, intRecordPID, o51IDs)
    End Function
    Public Function SaveBatch(strPrefix As String, pids As List(Of Integer), o51IDs As List(Of Integer), bolReplace As Boolean) As Boolean Implements Io51TagBL.SaveBatch
        If pids.Count = 0 Then
            _Error = "Na vstupu chybí předané záznamy." : Return False
        End If
        If o51IDs.Count = 0 And bolReplace = False Then
            _Error = "Na vstupu chybí vybrané štítky." : Return False
        End If
        Return _cDL.SaveBatch(strPrefix, pids, o51IDs, bolReplace)
    End Function
    Public Function ParseQueryByTags(strParsePrefix As String, strQuery As String) As BO.QueryByTags Implements Io51TagBL.ParseQueryByTags
        Dim ret As New BO.QueryByTags
        If strQuery = "" Then Return ret

        Dim sis As List(Of String) = BO.BAS.ConvertDelimitedString2List(strQuery)
        Dim pids As New List(Of Integer)
        For Each s In sis
            If Left(s, 3) = strParsePrefix Then
                pids.Add(CInt(Right(s, Len(s) - 4)))
            End If
        Next
        If pids.Count = 0 Then Return ret

        Dim mq As New BO.myQuery
        mq.PIDs = pids
        Dim lis As IEnumerable(Of BO.o51Tag) = GetList(mq, "all", BO.BooleanQueryMode.NoQuery)
        sis = New List(Of String)

        For Each c In lis
            Dim s As String = "<span class='badge_tag'"
            If c.o51BackColor <> "" Or c.o51ForeColor <> "" Then
                s += " style='"
                If c.o51BackColor <> "" Then
                    s += "background-color:" & c.o51BackColor & ";"
                End If
                If c.o51ForeColor <> "" Then
                    s += ";color:" & c.o51ForeColor & ";"
                End If
                s += "'"
            End If
            s += ">" & c.o51Name & "</span>"
            sis.Add(s)
        Next
        ret.TextInline = String.Join(", ", lis.Select(Function(p) p.o51Name))
        ret.o51IDs = pids
        ret.o51IDsInline = String.Join(",", pids)
        ret.HtmlInline = String.Join("", sis)
        Return ret
    End Function
End Class
