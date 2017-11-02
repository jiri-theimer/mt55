Public Class x28EntityFieldDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x28EntityField
        Dim s As String = GetSQLPart1() & " WHERE a.x28ID=@x28id"
        Return _cDB.GetRecord(Of BO.x28EntityField)(s, New With {.x28id = intPID})
    End Function
    Public Function LoadByField(strField As String) As BO.x28EntityField
        Dim s As String = GetSQLPart1() & " WHERE a.x28Field LIKE @field"
        Return _cDB.GetRecord(Of BO.x28EntityField)(s, New With {.field = strField})
    End Function
    Public Function LoadByQueryField(strField As String) As BO.x28EntityField
        Dim s As String = GetSQLPart1() & " WHERE a.x28Query_Field LIKE @field"
        Return _cDB.GetRecord(Of BO.x28EntityField)(s, New With {.field = strField})
    End Function

    Public Function FindFirstUsableField(strPrefix As String, cX24 As BO.x24DataType, intX23ID As Integer) As String
        Return _cDB.GetValueFromSQL("select dbo.x28_getFirstUsableField('" & strPrefix & "','" & LCase(cX24.x24Name) & "'," & intX23ID.ToString & ")")
    End Function

    Public Function Save(cRec As BO.x28EntityField, lisX26 As List(Of BO.x26EntityField_Binding)) As Boolean
        Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
        If cRec.PID <> 0 Then
            bolINSERT = False
            strW = "x28ID=@pid"
            pars.Add("pid", cRec.PID)
        End If

        With cRec
            pars.Add("x28Flag", .x28Flag, DbType.Int32)
            pars.Add("x28Name", .x28Name, DbType.String, , , True, "Název")
            pars.Add("x28Field", .x28Field, DbType.String)
            pars.Add("x28Ordinary", .x28Ordinary, DbType.Int32)
            pars.Add("x28ValidFrom", .ValidFrom, DbType.DateTime)
            pars.Add("x28ValidUntil", .ValidUntil, DbType.DateTime)

            pars.Add("x29id", BO.BAS.IsNullDBKey(cRec.x29ID), DbType.Int32)
            pars.Add("x27ID", BO.BAS.IsNullDBKey(.x27ID), DbType.Int32)
            pars.Add("x23ID", BO.BAS.IsNullDBKey(.x23ID), DbType.Int32)
            pars.Add("x24id", .x24ID, DbType.Int32)
            'pars.Add("x28comment", .x28Comment, DbType.String, , , True, "Komentář")
            pars.Add("x28datasource", .x28DataSource, DbType.String, , , True, "Obor hodnot")
            pars.Add("x28isfixeddatasource", .x28IsFixedDataSource, DbType.Boolean)
            pars.Add("x28IsRequired", .x28IsRequired, DbType.Boolean)
            pars.Add("x28IsAllEntityTypes", .x28IsAllEntityTypes, DbType.Boolean)

            pars.Add("x28IsPublic", .x28IsPublic, DbType.Boolean)
            pars.Add("x28NotPublic_j04IDs", .x28NotPublic_j04IDs, DbType.String)
            pars.Add("x28NotPublic_j07IDs", .x28NotPublic_j07IDs, DbType.String)
            pars.Add("x28Grid_Field", .x28Grid_Field, DbType.String)
            pars.Add("x28Grid_SqlSyntax", .x28Grid_SqlSyntax, DbType.String)
            pars.Add("x28Grid_SqlFrom", .x28Grid_SqlFrom, DbType.String)
            pars.Add("x28Pivot_SelectSql", .x28Pivot_SelectSql, DbType.String)
            pars.Add("x28Pivot_GroupBySql", .x28Pivot_GroupBySql, DbType.String)
            pars.Add("x28Query_SqlSyntax", .x28Query_SqlSyntax, DbType.String)
            pars.Add("x28Query_Field", .x28Query_Field, DbType.String)
            pars.Add("x28Query_sqlComboSource", .x28Query_sqlComboSource, DbType.String)

            pars.Add("x28textboxheight", cRec.x28TextboxHeight, DbType.Int32)
            pars.Add("x28textboxwidth", cRec.x28TextboxWidth, DbType.Int32)
            pars.Add("x28HelpText", .x28HelpText, DbType.String)

        End With



        If _cDB.SaveRecord("x28EntityField", pars, bolINSERT, strW, True, _curUser.j03Login) Then
            Dim intLastSavedPID As Integer = _cDB.LastSavedRecordPID
            If Not lisX26 Is Nothing Then
                If Not bolINSERT Then
                    _cDB.RunSQL("DELETE FROM x26EntityField_Binding WHERE x28ID=" & intLastSavedPID.ToString)
                End If
                For Each c In lisX26
                    _cDB.RunSQL("INSERT INTO x26EntityField_Binding(x28ID,x26EntityTypePID,x29ID_EntityType,x26IsEntryRequired) VALUES (" & intLastSavedPID.ToString & "," & c.x26EntityTypePID.ToString & "," & c.x29ID_EntityType.ToString & "," & BO.BAS.GB(c.x26IsEntryRequired) & ")")
                Next
            End If
            Return True

        Else
            Return False
        End If

    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x28_delete", pars)

    End Function

    Public Overloads Function GetList(x29id As BO.x29IdEnum, intEntityType As Integer, bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField)
        Dim pars As New DbParameters
        Dim s As String = InhaleGetListSQL(x29id, pars, intEntityType, bolTestUserAccess)

        Return _cDB.GetList(Of BO.x28EntityField)(s, pars)
    End Function
    Public Overloads Function GetList(intX23ID As Integer) As IEnumerable(Of BO.x28EntityField)
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        s += " WHERE a.x23ID=@x23id"
        pars.Add("x23id", intX23ID, DbType.Int32)
        Return _cDB.GetList(Of BO.x28EntityField)(s, pars)
    End Function
    Public Overloads Function GetList(x28FieldNames As List(Of String), bolTestUserAccess As Boolean) As IEnumerable(Of BO.x28EntityField)
        'zde zatím bez využití bolTestUserAccess, asi není třeba
        Dim s As String = GetSQLPart1(), pars As New DbParameters
        If x28FieldNames.Count > 0 Then
            s += " WHERE a.x28Field IN ("
            For i As Integer = 0 To x28FieldNames.Count - 1
                If i = 0 Then
                    s += BO.BAS.GS(x28FieldNames(i))
                Else
                    s += "," & BO.BAS.GS(x28FieldNames(i))
                End If
            Next
            s += ")"
        End If

        Return _cDB.GetList(Of BO.x28EntityField)(s, pars)
    End Function
    

    Private Function GetSQLPart1() As String
        Dim s As String = "select a.*,x29.x29name as _x29Name,x27.x27Name as _x27Name,lower(x24.x24Name) as _TypeName,x23.x23Name as _x23Name,x23.x23DataSource as _x23DataSource," & bas.RecTail("x28", "a")
        s += " FROM x28EntityField a inner join x29Entity x29 on a.x29id=x29.x29id inner join x24DataType x24 on a.x24id=x24.x24id"
        s += " LEFT OUTER JOIN x27EntityFieldGroup x27 on a.x27ID=x27.x27ID LEFT OUTER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID"
        Return s
    End Function

    Private Function InhaleGetListSQL(x29id As BO.x29IdEnum, ByRef pars As DbParameters, intEntityType As Integer, bolTestUserAccess As Boolean) As String
        Dim s As String = GetSQLPart1()
        If x29id > BO.x29IdEnum._NotSpecified Then
            s += " WHERE a.x29ID=@x29id"
            pars.Add("x29id", CInt(x29id), DbType.Int32)
        End If
        If intEntityType >= 0 Then
            If intEntityType = 0 Then
                s += " AND a.x28IsAllEntityTypes=1"
            Else
                s += " AND (a.x28IsAllEntityTypes=1 OR a.x28ID IN (select x28ID FROM x26EntityField_Binding WHERE x26EntityTypePID=" & intEntityType.ToString
                s += " AND x29ID_EntityType=" & GetEntityTypeX29ID(x29id).ToString & "))"

            End If
        End If
        If bolTestUserAccess Then
            s += " AND (a.x28IsPublic=1 OR ','+a.x28NotPublic_j04IDs+',' LIKE '%," & _curUser.j04ID.ToString & ",%'"
            If _curUser.j07ID <> 0 Then
                s += " OR ','+a.x28NotPublic_j07IDs+',' LIKE '%," & _curUser.j07ID.ToString & ",%'"
            End If
            s += ")"
        End If
        

        s += " ORDER BY a.x29ID,x27.x27Ordinary,a.x27ID,a.x28Ordinary,a.x28name"
        Return s
    End Function

    Private Function GetEntityTypeX29ID(x29id As BO.x29IdEnum) As Integer
        Select Case x29id
            Case BO.x29IdEnum.p41Project : Return 342
            Case BO.x29IdEnum.p28Contact : Return 329
            Case BO.x29IdEnum.p91Invoice : Return 392
            Case BO.x29IdEnum.p31Worksheet : Return 334
            Case BO.x29IdEnum.j02Person : Return 107
            Case BO.x29IdEnum.o23Doc : Return 918
            Case BO.x29IdEnum.p56Task : Return 357
            Case Else
                Return 0
        End Select
    End Function

    Public Function GetList_x26(intX28ID As Integer) As IEnumerable(Of BO.x26EntityField_Binding)
        Return _cDB.GetList(Of BO.x26EntityField_Binding)("SELECT * FROM x26EntityField_Binding WHERE x28ID=@pid", New With {.pid = intX28ID})
    End Function
    Public Function GetListWithValues(x29id As BO.x29IdEnum, intRecordPID As Integer, intEntityType As Integer, Optional strTempGUID As String = "") As List(Of BO.FreeField)
        Dim pars As New DbParameters

        Dim s As String = InhaleGetListSQL(x29id, pars, intEntityType, True)
        Dim lis As List(Of BO.FreeField) = _cDB.GetList(Of BO.FreeField)(s, pars).Where(Function(p) p.x28Flag = BO.x28FlagENUM.UserField).ToList

        If lis.Count = 0 Then Return lis

        Dim x28ids As List(Of Integer) = lis.Select(Function(p) p.PID).ToList
        Dim lisX26_Required As IEnumerable(Of BO.x26EntityField_Binding) = _cDB.GetList(Of BO.x26EntityField_Binding)("SELECT * FROM x26EntityField_Binding WHERE x26IsEntryRequired=1 AND x26EntityTypePID=" & intEntityType.ToString & " AND x29ID_EntityType=" & GetEntityTypeX29ID(x29id).ToString & " AND x28ID IN (" & String.Join(",", x28ids) & ")")
        For Each c In lisX26_Required
            lis.First(Function(p) p.PID = c.x28ID).x28IsRequired = True
        Next
        If intRecordPID = 0 Then Return lis

        Dim dr As SqlClient.SqlDataReader = Nothing
        If intRecordPID <> 0 Then
            Select Case x29id
                Case BO.x29IdEnum.p41Project
                    dr = _cDB.GetDataReader("select * from p41Project_FreeField WHERE p41ID=" & intRecordPID.ToString)
                Case BO.x29IdEnum.p28Contact
                    dr = _cDB.GetDataReader("select * from p28Contact_FreeField WHERE p28ID=" & intRecordPID.ToString)
                Case BO.x29IdEnum.p91Invoice
                    dr = _cDB.GetDataReader("select * from p91Invoice_FreeField WHERE p91ID=" & intRecordPID.ToString)
                Case BO.x29IdEnum.p90Proforma
                    dr = _cDB.GetDataReader("select * from p90Proforma_FreeField WHERE p90ID=" & intRecordPID.ToString)
                Case BO.x29IdEnum.p31Worksheet
                    If strTempGUID = "" Then
                        dr = _cDB.GetDataReader("select * from p31WorkSheet_FreeField WHERE p31ID=" & intRecordPID.ToString)
                    Else
                        dr = _cDB.GetDataReader("select * from p31WorkSheet_FreeField_Temp WHERE p31GUID='" & strTempGUID & "' AND p31ID=" & intRecordPID.ToString)
                    End If
                Case BO.x29IdEnum.p56Task
                    dr = _cDB.GetDataReader("select * from p56Task_FreeField WHERE p56ID=" & intRecordPID.ToString)
                Case BO.x29IdEnum.j02Person
                    dr = _cDB.GetDataReader("select * from j02Person_FreeField WHERE j02ID=" & intRecordPID.ToString)
                
                Case BO.x29IdEnum.o22Milestone
                    dr = _cDB.GetDataReader("select * from o22Milestone_FreeField WHERE o22ID=" & intRecordPID.ToString)
            End Select
        End If
        If Not dr Is Nothing Then
            While dr.Read
                For i As Integer = 0 To dr.FieldCount - 1
                    For Each fld As BO.FreeField In lis
                        If LCase(dr.GetName(i)) = LCase(fld.x28Field) Then
                            fld.DBValue = dr(i)
                            If fld.x23ID <> 0 Then
                                If Not dr(fld.x28Field & "Text") Is System.DBNull.Value Then
                                    fld.ComboText = dr(fld.x28Field & "Text")
                                End If
                            End If

                            Exit For
                        End If
                    Next
                Next
            End While
            dr.Close()
        End If

        Return lis

    End Function
End Class
