Imports Dapper

Public Class p85TempBoxDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.p85TempBox
        If intPID = 0 Then
            Return New BO.p85TempBox()
        Else
            Dim s As String = "SELECT *," & bas.RecTail("p85") & " FROM p85Tempbox WHERE p85ID=@p85id"
            Return _cDB.GetRecord(Of BO.p85TempBox)(s, New With {.p85id = intPID})
        End If
    End Function
    Public Function LoadByGUID(strGUID As String) As BO.p85TempBox
        Dim s As String = "SELECT TOP 1 *," & bas.RecTail("p85") & " FROM p85Tempbox WHERE p85GUID=@myguid"
        Return _cDB.GetRecord(Of BO.p85TempBox)(s, New With {.myguid = strGUID})
    End Function
    Public Function LoadFromDeposit(strGUID As String) As Integer
        Dim c As BO.GetInteger = _cDB.GetRecord(Of BO.GetInteger)("select p85DataPID as Value FROM p85TempBox WHERE p85GUID=@myguid", New With {.myguid = strGUID})
        If Not c Is Nothing Then
            Return c.Value
        Else
            Return 0
        End If
    End Function
    Public Function SetToDeposit(intDataPID As Integer) As String
        Dim pars As New DbParameters
        Dim strGUID As String = Guid.NewGuid().ToString("N")
        pars.Add("datapid", intDataPID, DbType.Int32)
        pars.Add("guid", strGUID, DbType.String)
        If _cDB.SaveRecord("p85TempBox", pars, True, , True, _curUser.j03Login) Then
            Return strGUID
        Else
            _Error = _cDB.ErrorMessage
            Return ""
        End If
    End Function

    Public Function Truncate(strGUID As String) As Boolean
        Dim pars As New DbParameters
        pars.Add("guid", strGUID, DbType.String)
        If _cDB.RunSQL("if exists(select p85ID FROM p85Tempbox WHERE p85GUID=@guid) DELETE FROM p85Tempbox WHERE p85GUID=@guid", pars) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
    Public Function Delete(cRec As BO.p85TempBox) As Boolean
        Dim pars As New DbParameters
        pars.Add("p85id", cRec.PID, DbType.Int32)
        If _cDB.RunSQL("update p85Tempbox set p85IsDeleted=1 WHERE p85ID=@p85id", pars) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
    Public Function UnDelete(cRec As BO.p85TempBox) As Boolean
        Dim pars As New DbParameters
        pars.Add("p85id", cRec.PID, DbType.Int32)
        If _cDB.RunSQL("update p85Tempbox set p85IsDeleted=0 WHERE p85ID=@p85id", pars) Then
            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function
    Public Function CloneOneRecord(intP85ID As Integer, strGUID_Dest As String) As Boolean
        Dim cRec As BO.p85TempBox = Load(intP85ID)
        If cRec Is Nothing Then Return False
        cRec.p85GUID = strGUID_Dest
        cRec.p85ClonePID = cRec.PID
        cRec.SetPID(0)
        Return Save(cRec)
    End Function

    Public Function GetList(strGUID As String, Optional bolIncludeDeleted As Boolean = False) As IEnumerable(Of BO.p85TempBox)
        Dim s As String = "SELECT *," & bas.RecTail("p85") & " FROM p85Tempbox WHERE p85GUID=@guid"
        If Not bolIncludeDeleted Then
            s += " AND p85IsDeleted=0"
        End If
        Dim pars As New DL.DbParameters
        pars.Add("guid", strGUID, DbType.String)


        Return _cDB.GetList(Of BO.p85TempBox)(s, pars)

    End Function

    Public Sub Clone(strGUID_Source As String, strGUID_Dest As String)
        Dim lis As IEnumerable(Of BO.p85TempBox) = GetList(strGUID_Source, True)
        For Each cRec In lis
            cRec.p85GUID = strGUID_Dest
            cRec.p85ClonePID = cRec.PID
            cRec.SetPID(0)
            Save(cRec)
        Next
    End Sub
    Public Function SaveObjectReflection2Temp(strGUID As String, cRec As Object) As Boolean
        Dim lisNames As List(Of String) = BO.BAS.GetPropertiesNames(cRec), s As New System.Text.StringBuilder
        For Each strName As String In lisNames.Where(Function(p) p.IndexOf("Free") < 0)

            Dim val As Object = BO.BAS.GetPropertyValue(cRec, strName)
            Dim strVal As String = "NULL"
            If Not (val Is Nothing Or val Is System.DBNull.Value) Then
                strVal = BO.BAS.GS(val.ToString)
            End If
            s.AppendLine("INSERT INTO p85TempBox(p85GUID,p85FreeText01,p85Message) VALUES(" & BO.BAS.GS(strGUID) & "," & BO.BAS.GS(strName) & "," & strVal & ");")

        Next
        If s.Length > 0 Then
            Return _cDB.RunSQL(s.ToString)
        End If

        Return False
    End Function
    Public Overloads Function RunTailoredProcedure(strGUID As String, strProcName As String) As String
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("guid", strGUID, DbType.String)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        If Not _cDB.RunSP(strProcName, pars) Then
            Return _cDB.ErrorMessage
        Else
            Return ""
        End If
    End Function
    Public Overloads Function RunTailoredProcedure(intRecordPID As Integer, strProcName As String) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intRecordPID, DbType.Int32)            
        End With
        Return _cDB.RunSP(strProcName, pars)
    End Function

    Public Function Save(cRec As BO.p85TempBox) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "p85id=@pid"
            pars.Add("pid", cRec.PID)
        End If
        With cRec
            pars.Add("p85GUID", .p85GUID, DbType.String)
            pars.Add("p85Prefix", .p85Prefix, DbType.String)
            pars.Add("p85DataPID", BO.BAS.IsNullDBKey(.p85DataPID), DbType.Int32)

            pars.Add("p85OtherKey1", BO.BAS.IsNullDBKey(.p85OtherKey1), DbType.Int32)
            pars.Add("p85OtherKey2", BO.BAS.IsNullDBKey(.p85OtherKey2), DbType.Int32)
            pars.Add("p85OtherKey3", BO.BAS.IsNullDBKey(.p85OtherKey3), DbType.Int32)
            pars.Add("p85OtherKey4", BO.BAS.IsNullDBKey(.p85OtherKey4), DbType.Int32)
            pars.Add("p85OtherKey5", BO.BAS.IsNullDBKey(.p85OtherKey5), DbType.Int32)
            pars.Add("p85OtherKey6", BO.BAS.IsNullDBKey(.p85OtherKey6), DbType.Int32)
            pars.Add("p85OtherKey7", BO.BAS.IsNullDBKey(.p85OtherKey7), DbType.Int32)
            pars.Add("p85OtherKey8", BO.BAS.IsNullDBKey(.p85OtherKey8), DbType.Int32)

            pars.Add("p85ClonePID", BO.BAS.IsNullDBKey(.p85ClonePID), DbType.Int32)

            pars.Add("p85Message", .p85Message, DbType.String)
            pars.Add("p85IsFinished", .p85IsFinished, DbType.Boolean)
            pars.Add("p85IsDeleted", .p85IsDeleted, DbType.Boolean)

            pars.Add("p85FreeText01", .p85FreeText01, DbType.String)
            pars.Add("p85FreeText02", .p85FreeText02, DbType.String)
            pars.Add("p85FreeText03", .p85FreeText03, DbType.String)
            pars.Add("p85FreeText04", .p85FreeText04, DbType.String)
            pars.Add("p85FreeText05", .p85FreeText05, DbType.String)
            pars.Add("p85FreeText06", .p85FreeText06, DbType.String)
            pars.Add("p85FreeText07", .p85FreeText07, DbType.String)
            pars.Add("p85FreeText08", .p85FreeText08, DbType.String)
            pars.Add("p85FreeText09", .p85FreeText09, DbType.String)

            pars.Add("p85FreeDate01", BO.BAS.IsNullDBDate(cRec.p85FreeDate01), DbType.DateTime)
            pars.Add("p85FreeDate02", BO.BAS.IsNullDBDate(cRec.p85FreeDate02), DbType.DateTime)
            pars.Add("p85FreeDate03", BO.BAS.IsNullDBDate(cRec.p85FreeDate03), DbType.DateTime)
            pars.Add("p85FreeDate04", BO.BAS.IsNullDBDate(cRec.p85FreeDate04), DbType.DateTime)
            pars.Add("p85FreeDate05", BO.BAS.IsNullDBDate(cRec.p85FreeDate05), DbType.DateTime)

            pars.Add("p85FreeNumber01", .p85FreeNumber01, DbType.Decimal)
            pars.Add("p85FreeNumber02", .p85FreeNumber02, DbType.Decimal)
            pars.Add("p85FreeNumber03", .p85FreeNumber03, DbType.Decimal)
            pars.Add("p85FreeNumber04", .p85FreeNumber04, DbType.Decimal)

            pars.Add("p85FreeFloat01", .p85FreeFloat01, DbType.Double)
            pars.Add("p85FreeFloat02", .p85FreeFloat02, DbType.Double)
            pars.Add("p85FreeFloat03", .p85FreeFloat03, DbType.Double)

            pars.Add("p85FreeBoolean01", .p85FreeBoolean01, DbType.Boolean)
            pars.Add("p85FreeBoolean02", .p85FreeBoolean02, DbType.Boolean)
            pars.Add("p85FreeBoolean03", .p85FreeBoolean03, DbType.Boolean)
            pars.Add("p85FreeBoolean04", .p85FreeBoolean04, DbType.Boolean)
        End With

        Dim strLogin As String = "-"
        If Not _curUser Is Nothing Then
            strLogin = _curUser.j03Login
        End If
        If _cDB.SaveRecord("p85TempBox", pars, bolINSERT, strW, True, strLogin) Then

            Return True
        Else
            _Error = _cDB.ErrorMessage
            Return False
        End If
    End Function

    Public Sub Recovery_ClearCompleteTemp()
        _cDB.RunSQL("exec dbo.recovery_clear_temp")
    End Sub
End Class
