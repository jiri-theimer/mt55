Public Class SysObjectBL
    Private _cDL As DL.SysDbObjectDL
    Private _factory As BL.Factory

    Public Sub New()
        _cDL = New DL.SysDbObjectDL(Nothing)
        _factory = New BL.Factory(Nothing)
    End Sub

    Public Function CreateDbBackup(Optional strCon As String = "", Optional strBackupDir As String = "", Optional strBakFileName As String = "", Optional bolTestFileSystem As Boolean = True) As String
        Dim cBAK As New DL.clsDbBackup()
        cBAK.IsUseDumpDevice = False
        If strCon = "" Then strCon = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationPrimary").ToString
        If strBackupDir = "" Then strBackupDir = _factory.x35GlobalParam.TempFolder
        If strBakFileName = "" Then strBakFileName = "MARKTIME50_Backup_" & Format(Now, "ddMMyyyyHHmmss") & ".bak"
        cBAK.MakeBackup(strCon, strBakFileName, strBackupDir, , bolTestFileSystem)

        If cBAK.ErrorMessage <> "" Then
            Return cBAK.ErrorMessage
        Else
            Return strBakFileName
        End If
    End Function

    Public Sub CreateRowCountCompareScript(strSourceDbName As String, strDestDbName As String)
        Dim strFile As String = _factory.x35GlobalParam.ExportFolder & "\CompareRowCount_" & Format(Now, "dd-MM-yyyy-HH-mm-ss") & ".txt"
        Dim cFile As New BO.clsFile
        Dim lisTabs As List(Of String) = _cDL.GetList_AllTables()
        For Each strTab In lisTabs

            Dim s As String = vbCrLf & vbCrLf & "----" & strTab
            s += vbCrLf & vbCrLf & "declare @c1 varchar(100),@c2 varchar(100)"
            s += vbCrLf & "select @c1=convert(varchar(10),count(*)) FROM " & strSourceDbName & ".dbo." & strTab
            s += vbCrLf & "select @c2=convert(varchar(10),count(*)) FROM " & strDestDbName & ".dbo." & strTab
            s += vbCrLf & "if @c1<>@c2"
            s += vbCrLf & "  print 'V [" & strTab & "] je rozdíl: '+@c1+' x '+@c2"
            s += vbCrLf & vbCrLf & "GO"
            cFile.SaveText2File(strFile, s, True, , False)
        Next
    End Sub

    Public Sub CreateImportScript(strTables As String, strDestDbName As String, Optional strConSource As String = "")
        Dim strFile As String = _factory.x35GlobalParam.ExportFolder & "\ImportData_" & Format(Now, "dd-MM-yyyy-HH-mm-ss") & ".txt"
        Dim cFile As New BO.clsFile

        Dim lisCols As IEnumerable(Of BO.TableColumn) = _cDL.GetList_Columns(strConSource)
        Dim aTabs() As String = Split(strTables, vbCrLf)
        For i As Integer = 0 To UBound(aTabs)
            Dim strTab As String = Trim(aTabs(i))
            If Len(strTab) > 3 Then
                Dim s As String = vbCrLf & vbCrLf & "----" & strTab
                s += vbCrLf & vbCrLf & "declare @pred varchar(100),@po varchar(100)"
                s += vbCrLf & "select @pred=convert(varchar(10),count(*)) FROM " & strTab
                s += vbCrLf & vbCrLf & "SET IDENTITY_INSERT " & strDestDbName & ".dbo." & strTab & " ON"
                Dim strF As String = ""
                For Each c As BO.TableColumn In lisCols.Where(Function(p) LCase(p.TableName) = LCase(strTab))
                    strF += "," & c.Name
                Next
                strF = BO.BAS.OM1(strF)

                s += vbCrLf & vbCrLf & "INSERT INTO " & strDestDbName & ".dbo." & strTab & "(" & strF & ")"
                s += vbCrLf & "SELECT " & strF & vbCrLf & "FROM" & vbCrLf & strTab

                s += vbCrLf & vbCrLf & "SET IDENTITY_INSERT " & strDestDbName & ".dbo." & strTab & " OFF"
                s += vbCrLf & "select @po=convert(varchar(10),count(*)) FROM " & strDestDbName & ".dbo." & strTab
                s += vbCrLf & "print 'Počet záznamů v [" & strTab & "] před: '+@pred+' po: '+@po"
                s += vbCrLf & "if @pred<>@po"
                s += vbCrLf & "  print 'Rozdíl!!!!'"

                s += vbCrLf & vbCrLf & "GO"
                cFile.SaveText2File(strFile, s, True, , False)
            End If

        Next
    End Sub
    Public Sub CreateDeleteScript(strTables As String, strDestDbName As String)
        Dim strFile As String = _factory.x35GlobalParam.ExportFolder & "\DeleteData_" & Format(Now, "dd-MM-yyyy-HH-mm-ss") & ".txt"
        Dim cFile As New BO.clsFile

        Dim lisCols As IEnumerable(Of BO.TableColumn) = _cDL.GetList_Columns()
        Dim aTabs() As String = Split(strTables, vbCrLf)
        For i As Integer = UBound(aTabs) To 0 Step -1
            Dim strTab As String = Trim(aTabs(i))
            If Len(strTab) > 3 Then
                Dim s As String = vbCrLf & vbCrLf & "DELETE FROM " & strDestDbName & ".dbo." & strTab
                s += vbCrLf & vbCrLf & "GO"
                cFile.SaveText2File(strFile, s, True, , False)
            End If

        Next
    End Sub
    


    Public Sub CompareTables(strDestConnection As String)
        Dim lisSource As IEnumerable(Of BO.TableColumn) = _cDL.GetList_Columns()
        Dim lisDest As IEnumerable(Of BO.TableColumn) = _cDL.GetList_Columns(strDestConnection)
        Dim cFile As New BO.clsFile

        Dim strFile As String = _factory.x35GlobalParam.ExportFolder & "\TablesCompare_" & Format(Now, "dd-MM-yyyy-HH-mm-ss") & ".txt"
        If cFile.FileExist(strFile) Then
            cFile.DeleteFile(strFile)
        End If

        Dim strLastTab As String = ""
        cFile.SaveText2File(strFile, "------- Jaké tabulky a sloupce chybí v databázi -------" & vbCrLf & vbCrLf, True, , False)
        For Each cRec As BO.TableColumn In lisSource
            Dim strTab As String = cRec.TableName, strField As String = cRec.Name, bolTabFound As Boolean = True

            If lisDest.Where(Function(p As BO.TableColumn) p.TableName Like strTab).Count = 0 Then
                If strTab <> strLastTab Then
                    cFile.SaveText2File(strFile, "Chybí tabulka [" & strTab & "]" & vbCrLf & vbCrLf, True, , False)
                End If
                bolTabFound = False
            End If

            If bolTabFound Then
                If lisDest.Where(Function(p As BO.TableColumn) p.TableName Like strTab And p.Name Like strField).Count = 0 Then
                    Dim strType As String = cRec.DBType
                    Select Case LCase(strType)
                        Case "varchar", "nvarchar", "char", "nchar"
                            strType += "(" & cRec.Size.ToString & ")"
                    End Select
                    If cRec.IsNullable Then strType += " NULL"


                    cFile.SaveText2File(strFile, "V tabulce [" & strTab & "] chybí pole [" & strField & "] " & strType & vbCrLf, True, , False)
                End If
            End If
            strLastTab = strTab
        Next

        strLastTab = ""
        cFile.SaveText2File(strFile, vbCrLf & vbCrLf & "------- Jaké tabulky a sloupce jsou navíc ve srovnávací databázi -------" & vbCrLf & vbCrLf, True, , False)
        For Each cRec As BO.TableColumn In lisDest
            Dim strTab As String = cRec.TableName, strField As String = cRec.Name, bolTabFound As Boolean = True

            If lisSource.Where(Function(p As BO.TableColumn) p.TableName Like strTab).Count = 0 Then
                If strTab <> strLastTab Then
                    cFile.SaveText2File(strFile, "Ve srovnávací databázi existuje tabulka [" & strTab & "]" & vbCrLf & vbCrLf, True, , False)
                End If
                bolTabFound = False
            End If

            If bolTabFound Then
                If lisSource.Where(Function(p As BO.TableColumn) p.TableName Like strTab And p.Name Like strField).Count = 0 Then
                    Dim strType As String = cRec.DBType
                    Select Case LCase(strType)
                        Case "varchar", "nvarchar", "char", "nchar"
                            strType += "(" & cRec.Size.ToString & ")"
                    End Select
                    If cRec.IsNullable Then strType += " NULL"


                    cFile.SaveText2File(strFile, "Srovnávací databáze, tabulka [" & strTab & "] navíc obsahuje pole [" & strField & "] " & strType & vbCrLf, True, , False)
                End If
            End If
            strLastTab = strTab
        Next
    End Sub

    Public Sub GenerateCreateScripts(bolSeparateFiles As Boolean)
        Dim lis As IEnumerable(Of BO.SysDbObject) = _cDL.GetList_SysObjects()
        Dim cFile As New BO.clsFile

        Dim strDIR As String = "c:\asp2013\marktime50\ui\sys\dbupdate"

        Dim strSingleFile As String = strDIR & "\SysObjects_Latest.sql"
        If cFile.FileExist(strSingleFile) Then
            cFile.DeleteFile(strSingleFile)
        End If
        For Each cRec As BO.SysDbObject In lis
            If bolSeparateFiles Then
                Dim strFile As String = strDIR & "\" & cRec.Name & ".sql"
                cFile.SaveText2File(strFile, GetFullCreateScript(cRec), , , False)

                cFile.CopyFile(strFile, "c:\zjt\marktime50\distribuce\inno-setup\files\wfa\sql_step2_sp.sql")
            Else
                cFile.SaveText2File(strSingleFile, "----------" & cRec.xType & "---------------" & cRec.Name & "-------------------------" & vbCrLf & vbCrLf & GetFullCreateScript(cRec) & vbCrLf & vbCrLf, True, , False)

                cFile.CopyFile(strSingleFile, "c:\zjt\marktime50\distribuce\inno-setup\files\wfa\sql_step2_sp.sql")
            End If


        Next
    End Sub

    Private Function GetFullCreateScript(cRec As BO.SysDbObject) As String
        Dim s As String = "if exists (select 1 from sysobjects where  id = object_id('" & cRec.Name & "') and type = '" & cRec.xType & "')" & vbCrLf
        Select Case cRec.xType
            Case "P"
                s += " drop procedure " & cRec.Name
            Case "FN", "IF"
                s += " drop function " & cRec.Name
            Case "V"
                s += " drop view " & cRec.Name
        End Select
        s += vbCrLf & "GO" & vbCrLf & vbCrLf & vbCrLf
        s += cRec.Content
        s += vbCrLf & vbCrLf & "GO"
        Return s
    End Function


    Public Function GetDump(ByVal strDumpDefinition As String, Optional ByVal strSave2File As String = "", Optional ByVal bolAppend2File As Boolean = False, Optional bolIncludeGO As Boolean = False) As String
        Dim cDump As New DL.clsDumpData()
        cDump.IsIncludeGO = bolIncludeGO
        cDump.DoDump(strDumpDefinition)
        Return cDump.Result
    End Function

    Public Function GetList_NoIdentityTables(Optional strConString As String = "") As List(Of String)
        Return _cDL.GetList_NoIdentityTables(strConString)
    End Function

    Public Sub GenerateDistributionXmlFiles(strSourceDbName As String)
        Dim cF As New BO.clsFile, strDestDir As String = "c:\asp2013\marktime50\ui\sys\dbupdate"
        cF.SaveText2File(strDestDir & "\version.txt", Format(Now, "dd.MM.yyyy HH:mm"), , , False)

        Dim ds As DataSet = _cDL.GetDataset(String.Format("SELECT ID,name FROM {0}.dbo.sysobjects WHERE xtype='U' and name<>'dtproperties' order by name", strSourceDbName), "sysobjects")
        Dim strFile As String = strDestDir & "\sysobjects_U.xml"
        ds.WriteXml(strFile, XmlWriteMode.WriteSchema)
        _cDL.RunSQL_GoParsing("INSERT INTO x30TableCatalog(x30TableName) SELECT [name] FROM dbo.sysobjects WHERE xtype='U' and [name]<>'dtproperties' AND [name] NOT LIKE 'zzz%' AND [name] NOT IN (SELECT x30TableName FROM x30TableCatalog) order by [name]")

        ds = _cDL.GetDataset(String.Format("select id,name FROM {0}.dbo.sysobjects WHERE xtype='D'", strSourceDbName), "sysobjects")
        strFile = strDestDir & "\sysobjects_D.xml"
        ds.WriteXml(strFile, XmlWriteMode.WriteSchema)

        ds = _cDL.GetDataset(String.Format("SELECT A.id,B.name, A.text FROM {0}.dbo.syscomments A INNER JOIN {0}.dbo.syscolumns B ON A.id = B.cdefault", strSourceDbName), "sysobjects")
        strFile = strDestDir & "\sysDefVals.xml"
        ds.WriteXml(strFile, XmlWriteMode.WriteSchema)

        ds = _cDL.GetDataset(String.Format("SELECT * FROM {0}.dbo.systypes", strSourceDbName), "sysobjects")
        strFile = strDestDir & "\systypes.xml"
        ds.WriteXml(strFile, XmlWriteMode.WriteSchema)

       

        Dim strAFs As String = "a.[name], a.[id], a.[xtype], a.[typestat], a.[xusertype],case when (b.name='nvarchar' or b.name='nchar') and a.[length]<>-1 then a.[length]/2 else a.[length] end as length, a.[xprec], a.[xscale], a.[colid], a.[xoffset], a.[bitpos], a.[reserved], a.[colstat], a.[cdefault], a.[domain], a.[number], a.[colorder], a.[autoval], a.[offset], a.[collationid], a.[language], a.[status], a.[type], a.[usertype], a.[printfmt], a.[prec], a.[scale], a.[iscomputed], a.[isoutparam], a.[isnullable], a.[collation], a.[tdscollation]"

        Dim strSQL As String = "select " & strAFs & ",b.name as type_name,c.name as TableName,computed.text as formula"
        strSQL += " from {0}.dbo.syscolumns a INNER JOIN (select * from {0}.dbo.systypes where name not like 'sysname') b ON a.xtype=b.xtype INNER JOIN {0}.dbo.sysobjects c ON a.ID=c.ID"
        strSQL += " LEFT OUTER JOIN (select a1.id,a1.colorder,a2.text FROM {0}.dbo.syscolumns a1 INNER JOIN {0}.dbo.syscomments a2 ON a1.id=a2.id AND a1.colorder=a2.number WHERE a1.iscomputed=1) computed"
        strSQL += " ON a.id=computed.id AND a.colorder=computed.colorder"
        strSQL += " where a.ID IN (select id from {0}.dbo.sysobjects where xtype='U') AND a.id is not null ORDER BY c.name,a.colorder"
        ds = _cDL.GetDataset(String.Format(strSQL, strSourceDbName), "sysobjects")
        strFile = strDestDir & "\syscolumns.xml"
        ds.WriteXml(strFile, XmlWriteMode.WriteSchema)

        ds = _cDL.GetDataset(String.Format("SELECT convert(int,ID),name FROM {0}.dbo.sysobjects WHERE xtype='U' and name<>'dtproperties' order by name", strSourceDbName), "")
        Dim dsIndex As DataSet = _cDL.GetDataset("select convert(int,0) as tableid,'' as tablename,'' as index_name,'' as index_description,'' as index_keys", "sysobjects")
        Dim dbRow As DataRow
        For Each dbRow In ds.Tables(0).Rows
            Dim dsI As DataSet = _cDL.GetDataset(String.Format("exec {0}.dbo.sp_helpindex " & dbRow.Item(1).ToString, strSourceDbName), "")

            If dsI.Tables(0).Rows.Count > 0 Then
                Dim dbR As DataRow, vals As Object
                For Each dbR In dsI.Tables(0).Rows

                    dsIndex.Tables(0).NewRow()
                    Dim dbNewR As DataRow = dsIndex.Tables(0).NewRow
                    dbNewR.Item(0) = CType(dbRow.Item(0), Int32)

                    dbNewR.Item(1) = dbRow.Item(1)
                    dbNewR.Item(2) = dbR.Item(0)
                    dbNewR.Item(3) = dbR.Item(1)
                    dbNewR.Item(4) = dbR.Item(2)
                    vals = dbNewR.ItemArray
                    dsIndex.Tables(0).Rows.Add(dbNewR)
                Next
            End If

        Next


        strFile = strDestDir & "\sysindexes.xml"
        dsIndex.WriteXml(strFile, XmlWriteMode.WriteSchema)


    End Sub

   
End Class
