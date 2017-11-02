Public Class SysDbUpdateBL
    Private _cDL As DL.SysDbObjectDL
    Private _factory As BL.Factory
    Private _result As System.Text.StringBuilder
    Private _Error As String = ""
    Protected Class clsDBField
        Public Table As String
        Public Name As String
        Public Type As String
        Public Length As String
        Public IsNullable As Boolean
        Public Scale As String
        Public TypeID As String
        Public IsIdentity As Boolean
        Public TableID As String
        Public DefaultValue As String
        Public Prec As String
        Public cDefault As String
        Public IsComputed As Boolean
        Public Formula As String
    End Class

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property

    Public Sub New()
        _cDL = New DL.SysDbObjectDL(Nothing)
        _factory = New BL.Factory(Nothing)
        _result = New System.Text.StringBuilder
    End Sub
    Public Sub ChangeConnectString(strConString As String)
        _cDL.ChangeConnectString(strConString)
    End Sub

    Public Function RunSql_step2_sp() As Boolean
        Dim strFile As String = BO.ASS.GetApplicationRootFolder() & "\sys\wfa\sql_step2_sp.sql"
        If Not System.IO.File.Exists(strFile) Then
            _Error = "Chybí soubor sql_step2_sp.sql" : Return False
        End If
        If _cDL.RunSql_step2_sp(strFile) Then
            Dim cF As New BO.clsFile
            cF.SaveText2File(_factory.x35GlobalParam.UploadFolder & "\sql_step2_sp.log", Format(Now, "dd.MM.yyyy HH:mm"), False)
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub RunFixingSQLs_BeforeDbUpdate()
        For i As Integer = 1 To 10
            Dim strFile As String = BO.ASS.GetApplicationRootFolder() & "\sys\wfa\sql_dbupdate_before" & i.ToString & ".sql"
            If System.IO.File.Exists(strFile) Then
                _cDL.RunSql_step2_sp(strFile)
            End If
        Next
    End Sub
    Public Sub RunFixingSQLs_AfterDbUpdate()
        For i As Integer = 1 To 10
            Dim strFile As String = BO.ASS.GetApplicationRootFolder() & "\sys\wfa\sql_dbupdate_after" & i.ToString & ".sql"
            If System.IO.File.Exists(strFile) Then
                _cDL.RunSql_step2_sp(strFile)
            End If
        Next
    End Sub
    Public Function RunDbDifferenceResult(strSQL As String) As Boolean
        If _cDL.RunDbDifferenceResult(strSQL) Then
            Dim cF As New BO.clsFile
            cF.SaveText2File(_factory.x35GlobalParam.UploadFolder & "\sql_db_difference.log", Format(Now, "dd.MM.yyyy HH:mm"), False)
            Return True
        Else
            Return False
        End If
    End Function

    Public Function FindDifference() As String
        If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\syscolumns.xml") Then
            _Error = "Chybí soubor: \sys\dbupdate\syscolumns.xml" : Return ""
        End If
        If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysobjects_U.xml") Then
            _Error = "Chybí soubor: \sys\dbupdate\sysobjects_U.xml" : Return ""
        End If
        If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysDefVals.xml") Then
            _Error = "Chybí soubor: \sys\dbupdate\sysDefVals.xml" : Return ""
        End If
        If Not System.IO.File.Exists(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysobjects_D.xml") Then
            _Error = "Chybí soubor: \sys\dbupdate\sysobjects_D.xml" : Return ""
        End If

        Dim cF As New clsDBField

        Dim strSQL As String
        Dim dsDefVal As New DataSet


        dsDefVal.ReadXml(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysDefVals.xml", XmlReadMode.ReadSchema)

        Dim dr1 As New DataSet, strAlter As String = "", dbRow As DataRow
        Dim strAFs As String = "a.[name], a.[id], a.[xtype], a.[typestat], a.[xusertype],case when (b.name='nvarchar' or b.name='nchar') and a.[length]<>-1 then a.[length]/2 else a.[length] end as length, a.[xprec], a.[xscale], a.[colid], a.[xoffset], a.[bitpos], a.[reserved], a.[colstat], a.[cdefault], a.[domain], a.[number], a.[colorder], a.[autoval], a.[offset], a.[collationid], a.[language], a.[status], a.[type], a.[usertype], a.[printfmt], a.[prec], a.[scale], a.[iscomputed], a.[isoutparam], a.[isnullable], a.[collation], a.[tdscollation]"

        strSQL = "select " & strAFs & ",b.name as type_name,c.name as TableName,computed.text as formula from dbo.syscolumns a INNER JOIN systypes b ON a.xtype=b.xtype INNER JOIN sysobjects c ON a.ID=c.ID"
        strSQL += " LEFT OUTER JOIN (select a1.id,a1.colorder,a2.text FROM syscolumns a1 INNER JOIN syscomments a2 ON a1.id=a2.id AND a1.colorder=a2.number WHERE a1.iscomputed=1) computed"
        strSQL += " ON a.id=computed.id AND a.colorder=computed.colorder"
        strSQL += " WHERE a.ID IN (select id from sysobjects where xtype='U') AND a.id is not null"
        strSQL += " ORDER BY c.name"
        Dim dsDest As DataSet = _cDL.GetDataset(strSQL, "")
        Dim dsDestTable As DataSet = _cDL.GetDataset("SELECT ID,name FROM sysobjects WHERE xtype='U' order by name", "Table")

        Dim dsDestConstr As DataSet = _cDL.GetDataset("select id,name FROM sysobjects WHERE xtype='D'", "")

        dr1.ReadXml(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysobjects_U.xml", XmlReadMode.ReadSchema)

        Dim dsCatalog As New DataSet
        dsCatalog.ReadXml(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\syscolumns.xml", XmlReadMode.ReadSchema)

        Dim dsDefConstr As New DataSet
        dsDefConstr.ReadXml(BO.ASS.GetApplicationRootFolder & "\sys\dbupdate\sysobjects_D.xml", XmlReadMode.ReadSchema)

        For Each dbRow In dr1.Tables(0).Rows

            If IsNewTable(dsDestTable, dsDefVal, dbRow.Item("name").ToString, strAlter, dsCatalog, dsDefConstr) Then
                _result.AppendLine(strAlter)
            End If
        Next

        Dim strLastTab As String = "", x As Integer

        Dim hashDestTable As New Hashtable(dsDestTable.Tables(0).Rows.Count)
        For Each dbRow In dsDestTable.Tables(0).Rows
            hashDestTable.Add(LCase(dbRow.Item("name")), dbRow)
        Next

        Dim hashDest As New Hashtable(dsDest.Tables(0).Rows.Count)
        For Each dbRow In dsDest.Tables(0).Rows
            If Not hashDest.Contains(LCase(dbRow.Item("TableName") & "-" & dbRow.Item("name"))) Then
                hashDest.Add(LCase(dbRow.Item("TableName") & "-" & dbRow.Item("name")), dbRow)
            End If
        Next

        For Each dbRow In dsCatalog.Tables(0).Rows
            x += 1
            'If x > 200 Then Exit For
            If strLastTab <> dbRow.Item("TableName").ToString Then

            End If
            cF.Table = dbRow.Item("TableName").ToString
            cF.cDefault = dbRow.Item("cDefault").ToString
            cF.TableID = dbRow.Item("ID").ToString
            cF.Name = dbRow.Item("name").ToString
            cF.TypeID = dbRow.Item("xtype").ToString
            cF.Type = dbRow.Item("type_name").ToString
            If dbRow.Item("iscomputed") = 1 Then
                cF.IsComputed = True
                cF.Formula = dbRow.Item("formula").ToString
            Else
                cF.IsComputed = False
            End If
            If dbRow.Item("status") = 128 Then
                cF.IsIdentity = True
            Else
                cF.IsIdentity = False
            End If


            cF.DefaultValue = GetDefVal(dsDefVal, dbRow.Item("name").ToString, dbRow.Item("cDefault").ToString & "")
            cF.Length = dbRow.Item("length").ToString
            'If cF.Type = "nvarchar" Or cF.Type = "nchar" Then
            '    cF.Length = cF.Length / 2
            'End If
            cF.Prec = dbRow.Item("xprec").ToString
            cF.Scale = dbRow.Item("xscale").ToString
            If dbRow.Item("isnullable") = 1 Then
                cF.IsNullable = True
            Else
                cF.IsNullable = False
            End If
            If IsDifference(hashDest, hashDestTable, dsDestConstr, cF, strAlter, dsDefConstr, dsDefVal) Then
                _result.AppendLine(strAlter & vbCrLf)
            End If
            strLastTab = dbRow.Item("TableName").ToString
        Next



        hashDestTable.Clear()

        Return _result.ToString



    End Function




    Private Function IsDifference(ByVal hashDest As Hashtable, ByVal hashDestTable As Hashtable, ByVal dsDestConstr As DataSet, ByVal cF As clsDBField, ByRef strRetSQL As String, dsDefConstr As DataSet, dsDefVal As DataSet) As Boolean
        IsDifference = False
        If cF.Table = "dtproperties" Or Left(cF.Table, 3) = "sys" Then Return False

        Dim dbRow As DataRow = Nothing, bolFound As Boolean = False, bolFoundTable As Boolean, strDestConstraint As String
        strRetSQL = ""
        Dim strConstraintSQL As String = ""

        If hashDestTable.Contains(LCase(cF.Table)) Then
            bolFoundTable = True
        End If
        If Not bolFoundTable Then Return False
        If hashDest.Contains(LCase(cF.Table & "-" & cF.Name)) Then
            bolFound = True
            dbRow = hashDest.Item(LCase(cF.Table & "-" & cF.Name))
        End If



        If Not bolFound Then
            If Not cF.IsComputed Then
                strRetSQL = "ALTER TABLE dbo." & cF.Table & " ADD " & cF.Name & " [" & cF.Type & "]"
            Else
                strRetSQL = "ALTER TABLE dbo." & cF.Table & " ADD " & cF.Name & " AS " & cF.Formula & Me.AddGo2SQL("")
            End If
            IsDifference = True
        Else
            If Not cF.IsComputed Then
                Dim bolGoON As Boolean = False
                If cF.Type <> dbRow.Item("type_name") Or cF.Prec <> dbRow.Item("xprec") & "" Or cF.Scale <> dbRow.Item("xscale") & "" Or (cF.cDefault = "0" And dbRow.Item("cdefault").ToString <> "0") Or (cF.cDefault <> "0" And dbRow.Item("cdefault") = "0") Then
                    bolGoON = True

                End If
                If Not bolGoON Then
                    If IsNumeric(cF.Length) And IsNumeric(dbRow.Item("length") & "") Then
                        If BO.BAS.IsNullInt(cF.Length) > BO.BAS.IsNullInt(dbRow.Item("length")) Then
                            bolGoON = True
                        End If
                    Else
                        If cF.Length <> dbRow.Item("length") & "" Then
                            bolGoON = True
                        End If
                    End If
                End If

                If bolGoON Then
                    strDestConstraint = GetConstraintName(dsDestConstr, dbRow.Item("cDefault"))
                    If strDestConstraint <> "" Then
                        strRetSQL += "ALTER TABLE dbo." & cF.Table & " drop CONSTRAINT [" & strDestConstraint & "]" & Me.AddGo2SQL("")
                    End If
                    strRetSQL += "ALTER TABLE dbo." & cF.Table & " ALTER COLUMN " & cF.Name & " [" & cF.Type & "]"
                    If BO.BAS.IsNullInt(cF.cDefault) <> 0 Then
                        Dim strDefConstraint As String = GetConstraintName(dsDefConstr, CInt(cF.cDefault))
                        If strDefConstraint <> "" Then
                            Dim strDefVal As String = GetDefVal(dsDefVal, cF.Name, cF.cDefault)
                            strConstraintSQL = "ALTER TABLE dbo.[" & cF.Table & "] ADD CONSTRAINT [" & strDefConstraint & "] DEFAULT (" & strDefVal & ") FOR [" & cF.Name & "]" & AddGo2SQL("")

                        End If
                    End If
                    

                    IsDifference = True
                End If
            Else
                If cF.Formula <> dbRow.Item("formula").ToString Then
                    strRetSQL += "ALTER TABLE dbo." & cF.Table & " DROP COLUMN " & cF.Name & Me.AddGo2SQL("")
                    strRetSQL += "ALTER TABLE dbo." & cF.Table & " ADD " & cF.Name & " AS " & cF.Formula & Me.AddGo2SQL("")
                    IsDifference = True
                End If
            End If
        End If


        If IsDifference Then
            If Not cF.IsComputed Then
                Select Case cF.Type
                    Case "decimal"
                        strRetSQL += " (" & cF.Prec & "," & cF.Scale & ")"
                        ''If Not bolFound Then If cF.DefaultValue <> "" Then strRetSQL += " DEFAULT (" & cF.DefaultValue & ")"

                    Case "varchar", "char", "nvarchar", "nchar"
                        If cF.Length = "-1" Then
                            strRetSQL += " (MAX)"
                        Else
                            strRetSQL += " (" & cF.Length & ")"
                        End If

                        ''If Not bolFound Then If cF.DefaultValue <> "" Then strRetSQL += " DEFAULT ('" & cF.DefaultValue & "')"

                    Case "text", "ntext"
                        ''If Not bolFound Then If cF.DefaultValue <> "" Then strRetSQL += " DEFAULT ('" & cF.DefaultValue & "')"
                    Case "date", "datetime"

                    Case Else
                        ''If Not bolFound Then If cF.DefaultValue <> "" Then strRetSQL += " DEFAULT (" & cF.DefaultValue & ")"


                End Select
                If cF.IsIdentity Then
                    strRetSQL += " IDENTITY (1, 1)"
                End If
                If cF.IsNullable Then
                    strRetSQL += " NULL"
                End If
                strRetSQL += AddGo2SQL("")
                If strConstraintSQL <> "" Then
                    strRetSQL += vbCrLf & strConstraintSQL
                End If
                If bolFound And cF.DefaultValue <> "" Then
                    If cF.Type = "varchar" Or cF.Type = "char" Or cF.Type = "text" Or cF.Type = "nvarchar" Or cF.Type = "nchar" Or cF.Type = "ntext" Then
                        ''strRetSQL += vbCrLf & "ALTER TABLE dbo." & cF.Table & " ADD DEFAULT '" & cF.DefaultValue & "' FOR " & cF.Name & AddGo2SQL("")
                        strRetSQL += vbCrLf & "UPDATE dbo." & cF.Table & " SET " & cF.Name & " = '" & cF.DefaultValue & "' WHERE " & cF.Name & " IS NULL" & AddGo2SQL("")
                    Else
                        ''strRetSQL += vbCrLf & "ALTER TABLE dbo." & cF.Table & " ADD DEFAULT " & cF.DefaultValue & " FOR " & cF.Name & AddGo2SQL("")
                        strRetSQL += vbCrLf & "UPDATE dbo." & cF.Table & " SET " & cF.Name & " = " & cF.DefaultValue & " WHERE " & cF.Name & " IS NULL" & AddGo2SQL("")
                    End If
                Else
                    If bolFoundTable And cF.DefaultValue <> "" Then
                        If cF.Type = "varchar" Or cF.Type = "char" Or cF.Type = "text" Or cF.Type = "nvarchar" Or cF.Type = "nchar" Or cF.Type = "ntext" Then
                            strRetSQL += vbCrLf & "UPDATE dbo." & cF.Table & " SET " & cF.Name & " = '" & cF.DefaultValue & "' WHERE " & cF.Name & " IS NULL" & AddGo2SQL("")
                        Else
                            strRetSQL += vbCrLf & "UPDATE dbo." & cF.Table & " SET " & cF.Name & " = " & cF.DefaultValue & " WHERE " & cF.Name & " IS NULL" & AddGo2SQL("")
                        End If
                    End If
                End If

            End If
        End If

    End Function

    Private Function GetConstraintName(ByVal dsDestConstr As DataSet, ByVal ID As Long) As String
        GetConstraintName = ""
        For Each dbRow As DataRow In dsDestConstr.Tables(0).Rows
            If dbRow.Item("id") = ID Then
                Return dbRow.Item("name")
            End If
        Next
    End Function


    Private Function IsNewTable(ByVal dsDestTable As DataSet, ByVal dsDefVal As DataSet, ByVal strTable As String, ByRef strRetSQL As String, ByVal dsCatalog As DataSet, dsDefConstr As DataSet) As String
        If strTable = "dtproperties" Or Left(strTable, 3) = "sys" Then Return False

        Dim dbRow As DataRow, bolFound As Boolean = False, bolFoundTable As Boolean = False
        strRetSQL = ""
        For Each dbRow In dsDestTable.Tables(0).Rows
            If LCase(dbRow.Item("name")) = LCase(strTable) Then
                bolFoundTable = True
                Exit For
            End If
        Next
        If bolFoundTable Then Return False

        Dim cF As New clsDBField, s As String = "", bolPK As Boolean = False
        s = vbCrLf & "CREATE TABLE dbo.[" & strTable & "] ("

        Dim bolFoundInCatalog As Boolean
        For Each dbRow In dsCatalog.Tables(0).Rows
            If LCase(dbRow.Item("TableName")) = LCase(strTable) Then
                bolFoundInCatalog = True
                Exit For
            End If
        Next
        If Not bolFoundInCatalog Then Return False


        Dim bolGo As Boolean, strDefValsSQL As String = ""
        For Each dbRow In dsCatalog.Tables(0).Rows
            bolGo = False
            If LCase(dbRow.Item("TableName")) = LCase(strTable) Then
                bolGo = True
            End If
            If bolGo Then
                With dbRow
                    cF.Table = .Item("TableName")
                    cF.TableID = .Item("ID")
                    cF.Name = .Item("name")
                    cF.TypeID = .Item("xtype")
                    cF.Type = .Item("type_name")
                    If .Item("status") = 128 Then
                        cF.IsIdentity = True
                    Else
                        cF.IsIdentity = False
                    End If
                    If BO.BAS.IsNullInt(.Item("cDefault")) <> 0 Then
                        Dim strConstraint As String = GetConstraintName(dsDefConstr, CLng(.Item("cDefault")))
                        Dim strDefVal As String = GetDefVal(dsDefVal, .Item("name"), .Item("cDefault") & "")
                        strDefValsSQL += vbCrLf & " ALTER TABLE dbo.[" & strTable & "] ADD CONSTRAINT [" & strConstraint & "] DEFAULT (" & strDefVal & ") FOR [" & cF.Name & "]" & vbCrLf & "GO" & vbCrLf
                    End If
                    ''cF.DefaultValue = GetDefVal(dsDefVal, .Item("name"), .Item("cDefault") & "")
                    cF.Length = .Item("length")
                    'If cF.Type = "nvarchar" Or cF.Type = "nchar" Then
                    '    cF.Length = cF.Length / 2
                    'End If
                    cF.Prec = .Item("xprec")
                    cF.Scale = .Item("xscale")
                    If .Item("isnullable") = 1 Then
                        cF.IsNullable = True
                    Else
                        cF.IsNullable = False
                    End If

                    s += vbCrLf & "[" & cF.Name & "] " & cF.Type
                    If Not cF.IsIdentity Then
                        Select Case cF.Type

                            Case "decimal"
                                s += " (" & cF.Prec & "," & cF.Scale & ")"
                                ''If cF.DefaultValue <> "" Then s += " DEFAULT (" & cF.DefaultValue & ")"
                            Case "varchar", "nvarchar"
                                s += " (" & cF.Length & ")"
                                ''If cF.DefaultValue <> "" Then s += " DEFAULT ('" & Replace(cF.DefaultValue, "'", "") & "')"
                            Case "text", "ntext"
                                ''If cF.DefaultValue <> "" Then s += " DEFAULT ('" & Replace(cF.DefaultValue, "'", "") & "')"
                            Case Else
                                ''If cF.DefaultValue <> "" Then s += " DEFAULT (" & cF.DefaultValue & ")"


                        End Select
                        If cF.IsNullable Then
                            s += " NULL"
                        End If
                    Else
                        s += " IDENTITY (1, 1)"
                    End If
                    If LCase(cF.Name) = "rowid" And Not bolPK Then
                        s += " CONSTRAINT " & cF.Name & "_PK_SCRIPT_" & strTable & " PRIMARY KEY CLUSTERED"
                        bolPK = True
                    End If
                    If LCase(cF.Name) = LCase(Left(strTable, 3)) & "id" And Not bolPK Then
                        s += " CONSTRAINT " & cF.Name & "_PK_SCRIPT_" & strTable & " PRIMARY KEY CLUSTERED"
                        bolPK = True
                    End If
                    s += ","

                End With
            End If
        Next
        If Right(s, 1) = "," Then s = Left(s, Len(s) - 1)


        s += ")" & Me.AddGo2SQL("") & vbCrLf

        If strDefValsSQL <> "" Then
            'default values přes syntaxi constraintů
            s += strDefValsSQL
        End If


        strRetSQL = s
        Return True


    End Function

    Private Function GetDefVal(ByVal dsDefVal As DataSet, ByVal strColumnName As String, Optional ByVal strDefID As String = "") As String
        Dim dbRow As DataRow

        For Each dbRow In dsDefVal.Tables(0).Rows
            If strDefID = "" Then
                If dbRow.Item("name") = strColumnName Then
                    Return dbRow.Item("text") & ""
                    's = dbRow.Item("text") & ""
                    's = Replace(s, "(", "")
                    's = Replace(s, ")", "")
                    'Return s
                End If
            Else
                If dbRow.Item("ID") = strDefID Then
                    Return dbRow.Item("text") & ""
                    's = dbRow.Item("text") & ""
                    's = Replace(s, "(", "")
                    's = Replace(s, ")", "")
                    'Return s
                End If
            End If
        Next
        Return ""
    End Function

    Private Function AddGo2SQL(ByVal strSQL As String) As String
        Return strSQL & vbCrLf & "GO" & vbCrLf
    End Function

    
End Class
