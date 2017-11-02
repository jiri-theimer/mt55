Public Class x18EntityCategoryDL
    Inherits DLMother
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function Load(intPID As Integer) As BO.x18EntityCategory
        Dim s As String = GetSQLPart1(False) & " WHERE a.x18ID=@x18id"

        Return _cDB.GetRecord(Of BO.x18EntityCategory)(s, New With {.x18id = intPID})
    End Function
    Public Function LoadByX23ID(intX23ID As Integer) As BO.x18EntityCategory
        Dim s As String = GetSQLPart1(False) & " WHERE a.x23ID=@x23id"

        Return _cDB.GetRecord(Of BO.x18EntityCategory)(s, New With {.x23id = intX23ID})
    End Function
    

    Public Function SaveX19TempBinding(intRecordPID As Integer, strTempGUID As String, lisX19 As List(Of BO.x19EntityCategory_Binding)) As Boolean
        Dim pars As New DbParameters
        pars.Add("guid", strTempGUID, DbType.String)
        pars.Add("recordpid", intRecordPID, DbType.Int32)

        _cDB.RunSQL("DELETE FROM p85TempBox WHERE p85GUID=@guid AND p85Prefix='x19' AND p85OtherKey3=@recordpid", pars)
        Dim cDLP85 As New DL.p85TempBoxDL(_curUser)

        For Each c In lisX19
            Dim cTemp As New BO.p85TempBox
            With cTemp
                .p85GUID = strTempGUID
                .p85Prefix = "x19"
                .p85OtherKey1 = c.x18ID
                .p85OtherKey2 = c.o23ID
                .p85OtherKey3 = intRecordPID
                .p85OtherKey4 = c.x20ID
            End With

            cDLP85.Save(cTemp)
        Next
        Return True
    End Function
    Public Function SaveX19Binding(x29id As BO.x29IdEnum, intRecordPID As Integer, lisX19 As List(Of BO.x19EntityCategory_Binding), x20IDs As List(Of Integer)) As Boolean
        Dim pars As New DbParameters
        pars.Add("x29id", CInt(x29id), DbType.Int32)
        pars.Add("recordpid", intRecordPID, DbType.Int32)
        pars.Add("login", _curUser.j03Login, DbType.String)

        Dim lisSaved As IEnumerable(Of BO.x19EntityCategory_Binding) = GetList_X19(x29id, intRecordPID, "", x20IDs)
        For Each c In lisX19
            Dim cRec As BO.x19EntityCategory_Binding = Nothing
            If lisSaved.Where(Function(p) p.x20ID = c.x20ID And p.o23ID = c.o23ID).Count > 0 Then
                cRec = lisSaved.Where(Function(p) p.x20ID = c.x20ID And p.o23ID = c.o23ID).First
            End If
            If cRec Is Nothing Then
                _cDB.RunSQL("INSERT INTO x19EntityCategory_Binding(x20ID,o23ID,x19RecordPID,x19UserUpdate,x19UserInsert,x19DateUpdate) VALUES(" & c.x20ID.ToString & "," & c.o23ID.ToString & ",@recordpid,@login,@login,getdate())", pars)
            End If
        Next
        For Each c In lisSaved
            Dim cRec As BO.x19EntityCategory_Binding = Nothing
            If lisX19.Where(Function(p) p.x20ID = c.x20ID And p.o23ID = c.o23ID).Count > 0 Then
                cRec = lisX19.Where(Function(p) p.x20ID = c.x20ID And p.o23ID = c.o23ID).First
            End If
            If cRec Is Nothing Then
                _cDB.RunSQL("DELETE FROM x19EntityCategory_Binding WHERE x20ID=" & c.x20ID.ToString & " AND o23ID=" & c.o23ID.ToString & " AND x19RecordPID=@recordpid", pars)
            End If

        Next
        If x29id = BO.x29IdEnum.p31Worksheet Then
            pars = New DbParameters
            pars.Add("recordpid", intRecordPID, DbType.Int32)
            _cDB.RunSQL("update p31Worksheet set o23ID_First=null WHERE p31ID=@recordpid; update a set o23ID_First=b.o23ID FROM p31Worksheet a INNER JOIN x19EntityCategory_Binding b ON a.p31ID=b.x19RecordPID INNER JOIN x20EntiyToCategory c ON b.x20ID=c.x20ID WHERE c.x29ID=331 AND b.x19RecordPID=@recordpid", pars)
        End If
        Return True
    End Function
    Public Function SaveX19Binding(into23ID As Integer, lisX19 As List(Of BO.x19EntityCategory_Binding), x20IDs As List(Of Integer)) As Boolean
        Dim pars As New DbParameters
        pars.Add("o23id", into23ID, DbType.Int32)
        pars.Add("login", _curUser.j03Login, DbType.String)

        Dim lisSaved As IEnumerable(Of BO.x19EntityCategory_Binding) = GetList_x19(into23ID, x20IDs, False)
        For Each c In lisX19
            Dim cRec As BO.x19EntityCategory_Binding = Nothing
            If lisSaved.Where(Function(p) p.x20ID = c.x20ID And p.x19RecordPID = c.x19RecordPID).Count > 0 Then
                cRec = lisSaved.Where(Function(p) p.x20ID = c.x20ID And p.x19RecordPID = c.x19RecordPID).First
            End If
            If cRec Is Nothing Then
                _cDB.RunSQL("INSERT INTO x19EntityCategory_Binding(x20ID,o23ID,x19RecordPID,x19UserUpdate,x19UserInsert,x19DateUpdate) VALUES(" & c.x20ID.ToString & ",@o23id," & c.x19RecordPID.ToString & ",@login,@login,getdate())", pars)
            End If
        Next
        For Each c In lisSaved
            Dim cRec As BO.x19EntityCategory_Binding = Nothing
            If lisX19.Where(Function(p) p.x20ID = c.x20ID And p.x19RecordPID = c.x19RecordPID).Count > 0 Then
                cRec = lisX19.Where(Function(p) p.x20ID = c.x20ID And p.x19RecordPID = c.x19RecordPID).First
            End If
            If cRec Is Nothing Then
                _cDB.RunSQL("DELETE FROM x19EntityCategory_Binding WHERE x20ID=" & c.x20ID.ToString & " AND x19RecordPID=" & c.x19RecordPID.ToString & " AND o23ID=@o23id", pars)
            End If
            ''    _cDB.RunSQL("UPDATE x19EntityCategory_Binding set x19UserUpdate=@login,x19DateUpdate=getdate() WHERE x20ID=" & c.x20ID.ToString & " AND x19RecordPID=" & c.x19RecordPID.ToString & " AND o23ID=@o23ID", pars)            
        Next
        Return True
    End Function

    Public Function Save(cRec As BO.x18EntityCategory, lisX20 As List(Of BO.x20EntiyToCategory), lisX69 As List(Of BO.x69EntityRole_Assign), lisX16 As List(Of BO.x16EntityCategory_FieldSetting)) As Boolean
        Using sc As New Transactions.TransactionScope()     'ukládání podléhá transakci
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""
            If cRec.PID <> 0 Then
                bolINSERT = False
                strW = "x18ID=@pid"
                pars.Add("pid", cRec.PID)
            End If
            With cRec
                pars.Add("x18Name", .x18Name, DbType.String, , , True, "Název")
                pars.Add("x18NameShort", .x18NameShort, DbType.String)
                pars.Add("x18Ordinary", .x18Ordinary, DbType.Int32)
                pars.Add("x18validfrom", .ValidFrom, DbType.DateTime)
                pars.Add("x18validuntil", .ValidUntil, DbType.DateTime)
                pars.Add("x18IsManyItems", .x18IsManyItems, DbType.Boolean)
                pars.Add("x18IsColors", .x18IsColors, DbType.Boolean)
                pars.Add("j02ID_Owner", BO.BAS.IsNullDBKey(.j02ID_Owner), DbType.Int32)
                pars.Add("x23ID", BO.BAS.IsNullDBKey(.x23ID), DbType.Int32)
                pars.Add("b01ID", BO.BAS.IsNullDBKey(.b01ID), DbType.Int32)
                pars.Add("x38ID", BO.BAS.IsNullDBKey(.x38ID), DbType.Int32)
                pars.Add("x31ID_Plugin", BO.BAS.IsNullDBKey(.x31ID_Plugin), DbType.Int32)
                pars.Add("x18Icon", .x18Icon, DbType.String)
                pars.Add("x18Icon32", .x18Icon32, DbType.String)
                pars.Add("x18IsClueTip", .x18IsClueTip, DbType.Boolean)
                pars.Add("x18ReportCodes", .x18ReportCodes, DbType.String)
                pars.Add("x18GridColsFlag", CInt(.x18GridColsFlag), DbType.Int32)
                pars.Add("x18EntryNameFlag", CInt(.x18EntryNameFlag), DbType.Int32)
                pars.Add("x18EntryCodeFlag", CInt(.x18EntryCodeFlag), DbType.Int32)
                pars.Add("x18EntryOrdinaryFlag", CInt(.x18EntryOrdinaryFlag), DbType.Int32)
                pars.Add("x18IsCalendar", .x18IsCalendar, DbType.Boolean)
                pars.Add("x18CalendarFieldStart", .x18CalendarFieldStart, DbType.String)
                pars.Add("x18CalendarFieldEnd", .x18CalendarFieldEnd, DbType.String)
                pars.Add("x18CalendarFieldSubject", .x18CalendarFieldSubject, DbType.String)
                pars.Add("x18CalendarResourceField", .x18CalendarResourceField, DbType.String)
                pars.Add("x18DashboardFlag", CInt(.x18DashboardFlag), DbType.Int32)
                pars.Add("x18UploadFlag", CInt(.x18UploadFlag), DbType.Int32)
                pars.Add("x18MaxOneFileSize", .x18MaxOneFileSize, DbType.Int32)
                pars.Add("x18AllowedFileExtensions", .x18AllowedFileExtensions, DbType.String)
                pars.Add("x18IsAllowEncryption", .x18IsAllowEncryption, DbType.Boolean)
                pars.Add("x18JavascriptFile", .x18JavascriptFile, DbType.String)
            End With

            If _cDB.SaveRecord("x18EntityCategory", pars, bolINSERT, strW, True, _curUser.j03Login) Then
                Dim intX18ID As Integer = _cDB.LastSavedRecordPID

                Dim lisX20Saved As IEnumerable(Of BO.x20EntiyToCategory) = GetList_x20(BO.BAS.ConvertInt2List(intX18ID))
                For Each c In lisX20
                    pars = New DbParameters
                    pars.Add("x18ID", intX18ID, DbType.Int32)
                    pars.Add("x29ID", c.x29ID, DbType.Int32)
                    pars.Add("x20Name", c.x20Name, DbType.String)
                    pars.Add("x20EntryModeFlag", CInt(c.x20EntryModeFlag), DbType.Int32)
                    pars.Add("x20GridColumnFlag", CInt(c.x20GridColumnFlag), DbType.Int32)
                    pars.Add("x20EntityPageFlag", CInt(c.x20EntityPageFlag), DbType.Int32)
                    pars.Add("x20IsMultiselect", c.x20IsMultiSelect, DbType.Boolean)
                    pars.Add("x20IsClosed", c.x20IsClosed, DbType.Boolean)
                    pars.Add("x20IsEntryRequired", c.x20IsEntryRequired, DbType.Boolean)
                    pars.Add("x20Ordinary", c.x20Ordinary, DbType.Int32)
                    pars.Add("x20EntityTypePID", BO.BAS.IsNullDBKey(c.x20EntityTypePID), DbType.Int32)
                    pars.Add("x29ID_EntityType", BO.BAS.IsNullDBKey(c.x29ID_EntityType), DbType.Int32)

                    bolINSERT = True : strW = ""
                    If lisX20Saved.Where(Function(p) p.x20ID = c.x20ID).Count > 0 Then
                        bolINSERT = False
                        strW = "x20ID=" & c.x20ID.ToString
                    End If
                    _cDB.SaveRecord("x20EntiyToCategory", pars, bolINSERT, strW, False, "", False)
                Next
                For Each c In lisX20Saved
                    If lisX20.Where(Function(p) p.x20ID = c.x20ID And p.x20ID <> 0).Count = 0 Then
                        _cDB.RunSQL("DELETE FROM x20EntiyToCategory WHERE x20ID=" & c.x20ID.ToString)
                    End If
                Next

                If Not lisX16 Is Nothing Then
                    _cDB.RunSQL("DELETE FROM x16EntityCategory_FieldSetting WHERE x18ID=" & intX18ID.ToString)
                    For Each c In lisX16
                        pars = New DbParameters
                        pars.Add("x18ID", intX18ID, DbType.Int32)
                        pars.Add("x16Name", c.x16Name, DbType.String)
                        pars.Add("x16NameGrid", c.x16NameGrid, DbType.String)
                        pars.Add("x16Field", c.x16Field, DbType.String)
                        pars.Add("x16IsEntryRequired", c.x16IsEntryRequired, DbType.Boolean)
                        pars.Add("x16IsGridField", c.x16IsGridField, DbType.Boolean)
                        pars.Add("x16IsFixedDataSource", c.x16IsFixedDataSource, DbType.Boolean)
                        pars.Add("x16DataSource", c.x16DataSource, DbType.String)
                        pars.Add("x16TextboxHeight", c.x16TextboxHeight, DbType.Int32)
                        pars.Add("x16TextboxWidth", c.x16TextboxWidth, DbType.Int32)
                        pars.Add("x16Ordinary", c.x16Ordinary, DbType.Int32)
                        pars.Add("x16Format", c.x16Format, DbType.String)
                        _cDB.SaveRecord("x16EntityCategory_FieldSetting", pars, True, "", False, "", False)
                    Next
                End If
                ''If Not lisX17 Is Nothing Then
                ''    _cDB.RunSQL("DELETE FROM x17EntityCategory_Folder WHERE x18ID=" & intX18ID.ToString)
                ''    For Each c In lisX17
                ''        pars = New DbParameters
                ''        pars.Add("x18ID", intX18ID, DbType.Int32)
                ''        pars.Add("x17Name", c.x17Name, DbType.String)
                ''        pars.Add("x17Path", c.x17Path, DbType.String)
                ''        pars.Add("x17Ordinary", c.x17Ordinary, DbType.Int32)
                ''        pars.Add("j11ID_Read", BO.BAS.IsNullDBKey(c.j11ID_Read), DbType.Int32)
                ''        pars.Add("j11ID_FullControl", BO.BAS.IsNullDBKey(c.j11ID_FullControl), DbType.Int32)
                ''        pars.Add("j11ID_CreateFiles", BO.BAS.IsNullDBKey(c.j11ID_CreateFiles), DbType.Int32)
                ''        _cDB.SaveRecord("x17EntityCategory_Folder", pars, True, "", False, "", False)
                ''    Next
                ''End If
                If Not lisX69 Is Nothing Then   'přiřazení rolí k štítku
                    bas.SaveX69(_cDB, BO.x29IdEnum.x18EntityCategory, intX18ID, lisX69, bolINSERT)
                End If
                sc.Complete()   'dokončení transakce
                Return True
            Else
                Return False
            End If
        End Using

    End Function
    Public Function Delete(intPID As Integer) As Boolean
        Dim pars As New DbParameters()
        With pars
            .Add("j03id_sys", _curUser.PID, DbType.Int32)
            .Add("pid", intPID, DbType.Int32)
            .Add("err_ret", , DbType.String, ParameterDirection.Output, 500)
        End With
        Return _cDB.RunSP("x18_delete", pars)
    End Function

    Public Function GetList(Optional myQuery As BO.myQuery = Nothing, Optional x29ID As BO.x29IdEnum = BO.x29IdEnum._NotSpecified, Optional intEntityType As Integer = 0, Optional bolInhaleAllCols As Boolean = False) As IEnumerable(Of BO.x18EntityCategory)
        Dim s As String = GetSQLPart1(bolInhaleAllCols), pars As New DbParameters
        
        Dim strW As String = bas.ParseWhereMultiPIDs("a.x18ID", myQuery)
        strW += bas.ParseWhereValidity("x18", "a", myQuery)
        If x29ID > BO.x29IdEnum._NotSpecified Then
            pars.Add("x29id", CInt(x29ID), DbType.Int32)
            strW += " AND a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=@x29id)"
        End If
        If intEntityType > 0 Then
            s += " AND (a.x18ID IN (select x18ID FROM x20EntiyToCategory WHERE x29ID=" & CInt(x29ID).ToString & " AND x20EntityTypePID=" & intEntityType.ToString & " AND x29ID_EntityType=" & GetEntityTypeX29ID(x29ID).ToString & ")"
            s += " OR a.x18ID IN (select x18ID FROM x20EntiyToCategory WHERE x29ID=@x29id AND x20EntityTypePID IS NULL))"
        End If
        If Not myQuery Is Nothing Then
            If myQuery.MyRecordsDisponible Then
                'pouze mě přístupné
                pars.Add("j02id_query", _curUser.j02ID, DbType.Int32)
                strW += " AND a.x18ID IN (SELECT x69.x69RecordPID FROM x69EntityRole_Assign x69 INNER JOIN x67EntityRole x67 ON x69.x67ID=x67.x67ID WHERE x67.x29ID=918 AND (x69.j02ID=@j02id_query OR x69.j11ID IN (SELECT j11ID FROM j12Team_Person WHERE j02ID=@j02id_query)))"   'obdržel nějakou (jakoukoliv) roli ve štítku
            End If
        End If
        
        If strW <> "" Then s += " WHERE " & bas.TrimWHERE(strW)

        s += " ORDER BY a.x18Ordinary,a.x18Name"
        Return _cDB.GetList(Of BO.x18EntityCategory)(s, pars)

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
    ''Public Function GetX29IDs(intX18ID As Integer) As IEnumerable(Of Integer)
    ''    Dim pars As New DbParameters
    ''    pars.Add("pid", intX18ID, DbType.Int32)
    ''    Return _cDB.GetList(Of BO.GetInteger)("select x29ID as Value FROM x20EntiyToCategory WHERE x18ID=@pid", pars).Select(Function(p) p.Value)
    ''End Function
    Private Function GetSQLPart1(bolInhaleAllCols As Boolean) As String
        Dim s As String = "select a.*,x23.x23Name as _x23Name,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner," & bas.RecTail("x18", "a")
        If bolInhaleAllCols Then
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=102) then 1 else 0 end) _Is_j02"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=141) then 1 else 0 end) _Is_p41"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=328) then 1 else 0 end) _Is_p28"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=331) then 1 else 0 end) _Is_p31"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=356) then 1 else 0 end) _Is_p56"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=223) then 1 else 0 end) _Is_o23"
            s += ",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=222) then 1 else 0 end) _Is_o22"
        End If
        s += " FROM x18EntityCategory a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID"
        Return s
    End Function
    Private Function GetSQLPart1_Full() As String
        Dim s As String = "select a.*,x23.x23Name as _x23Name,j02owner.j02LastName+' '+j02owner.j02FirstName as _Owner," & bas.RecTail("x18", "a")
        s += ",case when a.x18ID IN (SELECT FROM "
        s += " FROM x18EntityCategory a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID"
        Return s
    End Function

    Public Function GetList_O23(x29id As BO.x29IdEnum) As IEnumerable(Of BO.o23Doc)
        Dim s As String = "select a.*," & bas.RecTail("o23", "a") & ",x23.x23Name as _x23Name"
        s += " FROM o23Doc a INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID INNER JOIN x18EntityCategory x18 ON x23.x23ID=x18.x23ID"
        s += " WHERE x18.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=@x29id)"
        s += " ORDER BY x18.x18Ordinary,x18.x18Name,a.o23Ordinary,a.o23Name"

        Dim pars As New DbParameters
        pars.Add("x29id", CInt(x29id), DbType.Int32)
        Return _cDB.GetList(Of BO.o23Doc)(s, pars)

    End Function
    ''Public Function GetList_x20(intX18ID As Integer) As IEnumerable(Of BO.x20EntiyToCategory)
    ''    Dim lis As New List(Of Integer)
    ''    lis.Add(intX18ID)
    ''    Return GetList_x20(lis)
    ''    ''Return _cDB.GetList(Of BO.x20EntiyToCategory)("SELECT * FROM x20EntiyToCategory WHERE x18ID=@pid ORDER BY x20Ordinary", New With {.pid = intX18ID})
    ''End Function
    Public Function GetList_x20(x18IDs As List(Of Integer)) As IEnumerable(Of BO.x20EntiyToCategory)
        Return _cDB.GetList(Of BO.x20EntiyToCategory)("SELECT * FROM x20EntiyToCategory WHERE x18ID IN (" & String.Join(",", x18IDs) & ") ORDER BY x20Ordinary")
    End Function
    Public Function GetList_x20_join_x18(x29ID As BO.x29IdEnum, Optional intEntityType As Integer = 0) As IEnumerable(Of BO.x20_join_x18)
        Dim s As String = "SELECT a.*," & bas.RecTail("x18", "a") & ",x20.* FROM x18EntityCategory a INNER JOIN x20EntiyToCategory x20 ON a.x18ID=x20.x18ID"
        s += " WHERE getdate() BETWEEN a.x18ValidFrom AND a.x18ValidUntil AND x20.x29ID=" & CInt(x29ID).ToString
        If intEntityType > 0 Then
            s += " AND ((x20.x20EntityTypePID=" & intEntityType.ToString & " AND x20.x29ID_EntityType=" & GetEntityTypeX29ID(x29ID).ToString & ") OR x20.x20EntityTypePID IS NULL)"
        Else
            s += " AND (x20.x20EntityTypePID IS NULL)"
        End If
        s += " ORDER BY a.x18Ordinary,a.x18Name"

        Return _cDB.GetList(Of BO.x20_join_x18)(s)
    End Function
    Public Function GetList_x20_join_x18(intX18ID As Integer) As IEnumerable(Of BO.x20_join_x18)
        Dim pars As New DbParameters
        pars.Add("x18id", intX18ID, DbType.Int32)
        Dim s As String = "SELECT a.*," & bas.RecTail("x18", "a") & ",x20.* FROM x18EntityCategory a INNER JOIN x20EntiyToCategory x20 ON a.x18ID=x20.x18ID"
        s += " WHERE x20.x18ID=@x18id"
        s += " ORDER BY x20.x29ID"

        Return _cDB.GetList(Of BO.x20_join_x18)(s, pars)
    End Function
    Public Function GetList_x16(intX18ID As Integer) As IEnumerable(Of BO.x16EntityCategory_FieldSetting)
        Dim s As String = "SELECT * FROM x16EntityCategory_FieldSetting", pars As New DbParameters
        If intX18ID > 0 Then
            s += " WHERE x18ID=@x18id"
            pars.Add("x18id", intX18ID, DbType.Int32)
        End If
        s += " ORDER BY x16Ordinary"
        Return _cDB.GetList(Of BO.x16EntityCategory_FieldSetting)(s, pars)
    End Function
    ''Public Function GetList_x17(intX18ID As Integer) As IEnumerable(Of BO.x17EntityCategory_Folder)
    ''    Dim s As String = "SELECT * FROM x17EntityCategory_Folder", pars As New DbParameters
    ''    If intX18ID > 0 Then
    ''        s += " WHERE x18ID=@x18id"
    ''        pars.Add("x18id", intX18ID, DbType.Int32)
    ''    End If
    ''    s += " ORDER BY x18ID,x17Ordinary"
    ''    Return _cDB.GetList(Of BO.x17EntityCategory_Folder)(s, pars)
    ''End Function
    Private Function GetSQLPart1_x19(bolInhaleRecordAlias As Boolean) As String
        Dim s As String = "SELECT a.*," & bas.RecTail("x19", "a") & ",o23.o23Name as _o23Name,o23.o23Name+isnull(' ('+o23.o23Code+')','') as _NameWithCode,x20.x18ID as _x18ID,isnull(x18.x18NameShort,x18.x18Name) as _x18Name,x18.x18Icon as _x18Icon,o23.o23ForeColor as _ForeColor,o23.o23BackColor as _BackColor,x20.x29ID as _x29ID,x20.x20Name as _x20Name,x20.x20IsMultiselect as _x20IsMultiselect,x20.x20EntityPageFlag as _x20EntityPageFlag"
        If bolInhaleRecordAlias Then
            s += ",dbo.GetObjectAlias(convert(varchar(10),x20.x29ID),a.x19RecordPID) as _RecordAlias"
        End If
        s += " from x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x18EntityCategory x18 ON x20.x18ID=x18.x18ID"
        Return s
    End Function
    Public Function GetList_x19(x29id As BO.x29IdEnum, intRecordPID As Integer, strTempGUID As String, x20IDs_Query As List(Of Integer)) As IEnumerable(Of BO.x19EntityCategory_Binding)
        Dim pars As New DbParameters
        pars.Add("x29id", CInt(x29id), DbType.Int32)
        pars.Add("recordpid", intRecordPID, DbType.Int32)
        If strTempGUID = "" Then
            Dim s As String = GetSQLPart1_x19(False)
            s += " WHERE x20.x29ID=@x29id AND a.x19RecordPID=@recordpid"
            If Not x20IDs_Query Is Nothing Then
                If x20IDs_Query.Count > 0 Then s += " AND a.x20ID IN (" & String.Join(",", x20IDs_Query) & ")"
            End If
            s += " ORDER BY x18.x18Ordinary,x18.x18ID"
            Return _cDB.GetList(Of BO.x19EntityCategory_Binding)(s, pars)
        Else
            pars.Add("guid", strTempGUID, DbType.String)
            Dim s As String = "select p85.p85ID as _pid,p85.p85OtherKey1 as x18ID,p85.p85OtherKey2 as o23ID,p85.p85OtherKey3 as x19RecordPID"
            s += ",o23.o23Name as _o23Name,isnull(x18.x18NameShort,x18.x18Name) as _x18Name,o23.o23ForeColor as _ForeColor,o23.o23BackColor as _BackColor"
            s += " from p85TempBox p85 INNER JOIN x18EntityCategory x18 ON p85.p85OtherKey1=x18.x18ID INNER JOIN o23Doc o23 ON p85.p85OtherKey2=o23.o23ID"
            s += " WHERE p85.p85GUID=@guid AND p85.p85Prefix='x19' AND p85.p85OtherKey3=@recordpid ORDER BY x18.x18Ordinary,x18.x18ID"
            Return _cDB.GetList(Of BO.x19EntityCategory_Binding)(s, pars)
        End If

    End Function
    Public Function GetList_x19(into23ID As Integer, x20IDs_Query As List(Of Integer), bolInhaleRecordAlias As Boolean) As IEnumerable(Of BO.x19EntityCategory_Binding)
        Dim pars As New DbParameters
        pars.Add("o23ID", into23ID, DbType.Int32)
        Dim s As String = GetSQLPart1_x19(bolInhaleRecordAlias)
        s += " WHERE a.o23ID=@o23id"
        If Not x20IDs_Query Is Nothing Then
            If x20IDs_Query.Count > 0 Then s += " AND a.x20ID IN (" & String.Join(",", x20IDs_Query) & ")"
        End If
        s += " ORDER BY x20.x20IsMultiselect,a.x20ID,a.x19RecordPID"
        Return _cDB.GetList(Of BO.x19EntityCategory_Binding)(s, pars)
    End Function
    Public Function GetList_x19(x20IDs As List(Of Integer), bolInhaleRecordAlias As Boolean) As IEnumerable(Of BO.x19EntityCategory_Binding)
        Dim s As String = GetSQLPart1_x19(bolInhaleRecordAlias)
        s += " WHERE a.x20ID IN (" & String.Join(",", x20IDs) & ")"
        s += " ORDER BY x20.x20IsMultiselect,a.x20ID,a.x19RecordPID"
        Return _cDB.GetList(Of BO.x19EntityCategory_Binding)(s)
    End Function
    
End Class
