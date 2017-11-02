Public Class x35GlobalParamDL
    Inherits DLMother

    Public Sub New(ServiceUser As BO.j03User)
        MyBase.New(ServiceUser)
    End Sub

   
    Public Overloads Function GetList(strKey1 As String, Optional strKey2 As String = "", Optional strKey3 As String = "") As IEnumerable(Of BO.x35GlobalParam)
        Dim lis As New List(Of String)
        lis.Add(strKey1)
        If strKey2 <> "" Then lis.Add(strKey2)
        If strKey3 <> "" Then lis.Add(strKey3)

        Return GetList(lis)
    End Function

    Public Overloads Function GetList(x35keys As List(Of String)) As IEnumerable(Of BO.x35GlobalParam)
        If x35keys.Count = 0 Then Return Nothing

        Dim pars As New DbParameters, x As Integer, strParsInList As String = ""
        Dim s As String = "select x35Key,x35Value,x35Description,x35ModuleFlag," & bas.RecTail("x35", "x35GlobalParam", False)
        For Each strKey As String In x35keys
            x += 1
            pars.Add("key" & x.ToString, strKey, DbType.String)
            If x = 1 Then
                strParsInList = "@key1"
            Else
                strParsInList += ",@key" & x.ToString
            End If
        Next

        s += " FROM x35GlobalParam WHERE x35Key IN (" & strParsInList & ")"

        Return _cDB.GetList(Of BO.x35GlobalParam)(s, pars)
    End Function
    Public Overloads Function GetList() As IEnumerable(Of BO.x35GlobalParam)
        Dim s As String = "select x35Key,x35Value,x35Description,x35ModuleFlag," & bas.RecTail("x35", "x35GlobalParam", False)
        s += " FROM x35GlobalParam"
        Return _cDB.GetList(Of BO.x35GlobalParam)(s)
    End Function
    Public Function UpdateValue(strKey As String, strValue As String) As Boolean
        Dim pars As New DbParameters()
        pars.Add("key", strKey, DbType.String)
        pars.Add("val", strValue, DbType.String)
        If Not _curUser Is Nothing Then
            pars.Add("login", _curUser.j03Login, DbType.String)
        Else
            pars.Add("login", "???", DbType.String)
        End If

        Return _cDB.RunSQL("UPDATE x35GlobalParam SET x35Value=@val,x35DateUpdate=getdate(),x35UserUpdate=@login WHERE x35Key=@key", pars)
    End Function
    Public Function Save(cRec As BO.x35GlobalParam) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x35ID=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("x35Key", .x35Key, DbType.String, , , True, "Klíč")
            pars.Add("x35Value", .x35Value, DbType.String, , , True, "Hodnota klíče")
            pars.Add("x35ModuleFlag", .x35ModuleFlag, DbType.Int32)
        End With

        If _cDB.SaveRecord("x35GlobalParam", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            If cRec.x35Key = "j27ID_Domestic" Then
                'srovnat pořadí měn tak, aby domácí měna byla první
                _cDB.RunSQL("UPDATE j27Currency set j27Ordinary=0")
                _cDB.RunSQL("UPDATE j27Currency SET j27Ordinary =-1 WHERE j27ID=" & cRec.x35Value)
            End If
            bas.RecoveryUserCache(_cDB, _curUser)
            Return True
        Else
            Return False
        End If

    End Function
End Class
