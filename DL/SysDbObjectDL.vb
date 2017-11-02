Public Class SysDbObjectDL
    Inherits DLMother

    ''Public Sub ChangeConnectString(strConString As String)
    ''    _cDB.ChangeConString(strConString)
    ''End Sub
    Public Sub New(ServiceUser As BO.j03UserSYS)
        MyBase.New(ServiceUser)
    End Sub
    Public Function GetList_SysObjects() As IEnumerable(Of BO.SysDbObject)
        Dim s As String = "SELECT ID,name,xtype,schema_ver as version,convert(text,null) as content FROM sysobjects WHERE rtrim(xtype) IN ('V','FN','P','TR','IF') AND name not like 'dt_%' and name not like 'zzz%' and (name not like 'sys%' or name not like 'system_%') order by xtype,name"

        Dim lis As IEnumerable(Of BO.SysDbObject) = _cDB.GetList(Of BO.SysDbObject)(s)
        For Each cRec As BO.SysDbObject In lis
            Dim strContent As String = ""
            Dim dr As SqlClient.SqlDataReader = _cDB.GetDataReader("select colid,text FROM syscomments where id=" & cRec.ID.ToString & "order by colid")
            While dr.Read
                strContent += dr("text").ToString
            End While
            dr.Close()
            cRec.Content = strContent
            cRec.xType = Trim(cRec.xType)
        Next
        Return lis
    End Function
    Public Function GetList_NoIdentityTables(Optional strConString As String = "") As List(Of String)
        If strConString <> "" Then _cDB.ChangeConString(strConString)
        Dim lis As New List(Of String)
        Dim s As String = "select name from dbo.sysobjects WHERE xtype='U' AND id not in (select id FROM syscolumns where status=128) order by name"
        Dim dr As SqlClient.SqlDataReader = _cDB.GetDataReader(s)
        While dr.Read
            lis.Add(dr(0))
        End While
        dr.Close()
        Return lis
    End Function
    Public Function GetList_AllTables(Optional strConString As String = "") As List(Of String)
        If strConString <> "" Then _cDB.ChangeConString(strConString)
        Dim lis As New List(Of String)
        Dim s As String = "select name from dbo.sysobjects WHERE xtype='U' order by name"
        Dim dr As SqlClient.SqlDataReader = _cDB.GetDataReader(s)
        While dr.Read
            lis.Add(dr(0))
        End While
        dr.Close()
        Return lis
    End Function

    Public Function GetList_Columns(Optional strConString As String = "") As IEnumerable(Of BO.TableColumn)
        Dim strAFs As String = "a.[name], a.[id], a.[xtype], a.[typestat], a.[xusertype],case when b.name='nvarchar' or b.name='nchar' then a.[length]/2 else a.[length] end as length, a.[xprec], a.[xscale], a.[colid], a.[xoffset], a.[bitpos], a.[reserved], a.[colstat], a.[cdefault], a.[domain], a.[number], a.[colorder], a.[autoval], a.[offset], a.[collationid], a.[language], a.[status], a.[type], a.[usertype], a.[printfmt], a.[prec], a.[scale], a.[iscomputed], a.[isoutparam], a.[isnullable], a.[collation], a.[tdscollation]"

        Dim s As String = "select a.ID,a.[name] as Name,b.name as DBType,c.name as TableName,computed.text as Formula"
        s += ",a.isnullable,a.iscomputed,case when b.name='nvarchar' or b.name='nchar' then a.[length]/2 else a.[length] end as Size"

        s += " from dbo.syscolumns a INNER JOIN (select * from systypes where name not like 'sysname' and name not like 'hierarchyid' and name not like 'geometry') b ON a.xtype=b.xusertype INNER JOIN sysobjects c ON a.ID=c.ID"
        s += " LEFT OUTER JOIN (select a1.id,a1.colorder,a2.text FROM syscolumns a1 INNER JOIN syscomments a2 ON a1.id=a2.id AND a1.colorder=a2.number WHERE a1.iscomputed=1) computed"
        s += " ON a.id=computed.id AND a.colorder=computed.colorder"
        s += " where a.ID IN (select id from sysobjects where xtype='U') AND a.id is not null"
        s += " ORDER BY c.name,a.colorder"

        If strConString <> "" Then
            _cDB.ChangeConString(strConString)
        End If
        Return _cDB.GetList(Of BO.TableColumn)(s)

    End Function

    Public Function GetDataset(strSQL As String, strDatasetName As String) As DataSet
        Return _cDB.GetDataSet(strSQL, , , strDatasetName)
    End Function

    Public Function RunDbDifferenceResult(strSQL As String) As Boolean
        Return RunSQL_GoParsing(strSQL)
    End Function
    Public Function RunSql_step2_sp(strSqlFileFullpath As String) As Boolean
        _Error = ""
        Dim cF As New BO.clsFile
        Dim strSQL As String = cF.GetFileContents(strSqlFileFullpath)
        Return RunSQL_GoParsing(strSQL)

    End Function

    Public Function RunSQL_GoParsing(strSQL As String) As Boolean
        'je třeba ošetřit GO: vbCrLf & vbCrLf & "GO"
        Dim errs As New List(Of String)
        Dim a() As String = Split(strSQL, vbCrLf & "GO" & vbCrLf)
        For i As Integer = 0 To UBound(a)
            Dim s As String = Trim(a(i))
            s = Replace(s, vbCrLf & "GO", "")
            If s <> "" Then
                If Not _cDB.RunSQL(s) Then
                    errs.Add(_cDB.ErrorMessage)
                End If
            End If
        Next
        If errs.Count = 0 Then
            Return True
        Else
            For Each s In errs
                _Error += "<hr>" & s
                If Len(_Error) > 400 Then Return False
            Next
            Return False
        End If
    End Function

    Public Function CopyDataTableContent(dt As DataTable, Optional strExplicitDestTableName As String = "") As Boolean
        Dim errs As New List(Of String)
        If strExplicitDestTableName = "" Then strExplicitDestTableName = dt.TableName

        For Each dbRow In dt.Rows
            Dim pars As New DbParameters(), bolINSERT As Boolean = True, strW As String = ""

            For i = 0 To dt.Columns.Count - 1
                Dim bolSkip As Boolean = False, strField As String = dt.Columns(i).ColumnName
                If LCase(strField) = LCase(Left(strExplicitDestTableName, 3) & "id") Then
                    bolSkip = True
                End If
                If Not bolSkip Then
                    Dim typ As DbType = Nothing, strTypeName As String = dt.Columns(i).DataType.ToString
                    Select Case strTypeName
                        Case "System.Int32"
                            typ = DbType.Int32
                        Case "System.String"
                            typ = DbType.String
                        Case "System.Double"
                            typ = DbType.Double
                        Case "System.DateTime"
                            typ = DbType.DateTime
                        Case "System.Boolean"
                            typ = DbType.Boolean
                        Case Else
                            typ = Nothing
                    End Select

                    Dim val As Object = dbRow.item(i)
                    If LCase(strField) = LCase(Left(strExplicitDestTableName, 3) & "UserUpdate") Then
                        val = "ORIG-PID-" & dbRow.item(Left(strExplicitDestTableName, 3) & "ID")
                    End If
                    If Len(strField) = 5 And LCase(Right(strField, 2)) = "id" Then
                        Select Case LCase(Left(strField, 5))
                            Case "x29id", "x24id", "x15id", "x21id", "x53id", "x45id", "j27id", "p71id", "p70id", "p72id"
                            Case Else
                                'cizí klíč
                                Dim strTabKey As String = _cDB.GetValueFromSQL("select TOP 1 [x30TableName] FROM [x30TableCatalog] WHERE [x30TableName] LIKE '" & Left(strField, 3) & "%' and x30TableName NOT LIKE '%FreeField%'")
                                Dim intKeyVal As Integer = _cDB.GetIntegerValueFROMSQL("select " & strField & " FROM " & strTabKey & " WHERE " & Left(strField, 3) & "UserUpdate='ORIG-PID-" & val.ToString & "'")
                                ''If intKeyVal = 0 Then
                                ''    intKeyVal = _cDB.GetIntegerValueFROMSQL("select " & strField & " FROM " & strTabKey & " WHERE " & strField & "=" & val.ToString)
                                ''End If
                                If intKeyVal <> 0 Then
                                    val = intKeyVal
                                Else
                                    val = Nothing
                                End If
                        End Select

                    End If
                    If val Is System.DBNull.Value Then val = Nothing

                    pars.Add(dt.Columns(i).ColumnName, val, typ)
                End If
            Next
            If _cDB.SaveRecord(strExplicitDestTableName, pars, bolINSERT, strW, False, "", False) Then

            Else
                errs.Add(_cDB.ErrorMessage)

            End If
        Next
        If errs.Count = 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    
End Class
