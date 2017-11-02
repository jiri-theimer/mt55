Imports System.Data.SqlClient

Public Class clsDumpData
    Public RetMessage As String
    Public Result As String
    Public IsIncludeGO As Boolean
    Public IsMsAccess As Boolean

    Private Function IsComputedField(ByVal strField As String, ByVal aCF() As String) As Boolean
        Dim i As Integer
        For i = 0 To UBound(aCF)
            If strField = aCF(i) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub DoDump(ByVal strDumpDefinition As String, Optional ByVal strSave2File As String = "", Optional ByVal bolAppend2File As Boolean = False)
        RetMessage = ""
        Dim strTable As String = strDumpDefinition, ss As String, s As String, i As Integer, strVal As String, strType As String
        Dim aTabs() As String, x As Integer, aP() As String, strWHERE As String = ""
        If strTable = "" Then
            RetMessage = "Specify table name"
            Return
        Else
            aTabs = Split(strTable, vbCrLf)
        End If
        Dim sb As New System.Text.StringBuilder
        Dim sd As System.Text.StringBuilder

        Dim c As New DL.DbHandler()
        Dim strSQL As String, bolIdentity As Boolean

        Dim strSQLSys As String = "select a.name from syscolumns a inner join sysobjects b on a.id=b.id where b.xtype='U' and a.iscomputed=1"

        Dim strComputedFields As String = c.GetValueFromSQL(strSQLSys, True)
        Dim aCF() As String = Split(LCase(strComputedFields), ",")

        Dim dr As SqlDataReader

        For x = 0 To UBound(aTabs)
            ss = ""
            bolIdentity = False
            aP = Split(aTabs(x), "|")
            strTable = aP(0)
            strSQL = "select TOP 1 * from " & strTable
            dr = c.GetDataReader(strSQL)
            Dim strFs As String = ""
            For i = 0 To dr.FieldCount - 1
                If strComputedFields > "" Then
                    If Not IsComputedField(LCase(dr.GetName(i)), aCF) Then
                        strFs += ",[" & dr.GetName(i) & "]"
                    End If
                Else
                    strFs += ",[" & dr.GetName(i) & "]"
                End If
            Next
            dr.Close()
            If Left(strFs, 1) = "," Then strFs = Right(strFs, Len(strFs) - 1)
            strSQL = "select " & strFs & " from " & strTable

            If UBound(aP) > 0 Then
                If aP(1) = "1" Then
                    ss += "SET IDENTITY_INSERT " & strTable & " ON"
                    bolIdentity = True
                End If
            End If
            If UBound(aP) > 1 Then
                If Trim(aP(2)) = "1" Then ss += vbCrLf & "TRUNCATE TABLE " & strTable
            End If
            If UBound(aP) > 2 Then
                If Trim(aP(3)) <> "" Then
                    strSQL += " WHERE " & aP(3)
                    Dim bolFirstDelete As Boolean = True
                    If UBound(aP) > 3 Then
                        If aP(4) = "1" Then bolFirstDelete = False
                    End If
                    If bolFirstDelete Then
                        ss += vbCrLf & "DELETE FROM " & strTable & " WHERE " & aP(3)
                        If IsIncludeGO Then
                            ss += "GO" & vbCrLf
                        End If
                    End If
                End If
            End If
            dr = c.GetDataReader(strSQL)
            sd = New System.Text.StringBuilder
            While dr.Read
                s = ""

                Dim bolInsert As Boolean = True
                If UBound(aP) > 3 Then
                    'podmínka vložení podle výskytu ID
                    If aP(4) = "1" Then
                        s = "if (select count(*) FROM " & strTable & " WHERE " & dr.GetName(0) & "=" & dr.Item(0) & ")=0"
                        s += vbCrLf
                    End If
                End If
                If bolInsert Then
                    s += "INSERT INTO " & strTable & " (" & vbCrLf
                    For i = 0 To dr.FieldCount - 1
                        If i = 0 Then
                            s += dr.GetName(i)
                        Else
                            s += "," & dr.GetName(i)
                        End If
                    Next
                    s += ")" & vbCrLf & "VALUES (" & vbCrLf
                    For i = 0 To dr.FieldCount - 1
                        If dr.IsDBNull(i) Then
                            strVal = "NULL"
                        Else


                            strType = LCase(dr.GetDataTypeName(i))
                            Select Case strType
                                Case "datetime", "date"

                                    strVal = BO.BAS.GetHashDate(dr.GetSqlDateTime(i).ToString, True)
                                Case "datetime2"
                                    strVal = BO.BAS.GetHashDate(dr.GetValue(i), True)

                                Case "varchar", "char", "text", "nvarchar", "ntext", "nchar", "uniqueidentifier"

                                    strVal = BO.BAS.GS(dr.GetSqlValue(i).ToString)
                                Case "decimal"
                                    strVal = BO.BAS.GN(dr.GetSqlDecimal(i).ToString)
                                Case "float"
                                    strVal = BO.BAS.GN(dr.GetSqlDouble(i).ToString)
                                Case "int"
                                    strVal = BO.BAS.GN(dr.GetSqlInt32(i).ToString)
                                Case "smallint"
                                    strVal = BO.BAS.GN(dr.GetSqlInt16(i).ToString)
                                Case "bit"
                                    strVal = dr.GetSqlBoolean(i).ToString
                                    If dr.GetSqlBoolean(i).ToString = "0" Or dr.GetSqlBoolean(i).ToString = "False" Then
                                        strVal = "0"
                                    Else
                                        strVal = "1"
                                    End If
                                Case Else
                                    strVal = dr.GetSqlValue(i).ToString
                            End Select
                        End If
                        If i = 0 Then
                            s += strVal
                        Else
                            s += "," & strVal
                        End If
                    Next
                    s += ")" & vbCrLf
                    sd.Append(vbCrLf & s)
                    'ss += vbCrLf & s
                    If Me.IsIncludeGO Then
                        'ss += "GO" & vbCrLf
                        sd.Append("GO" & vbCrLf)
                    End If
                End If
            End While
            dr.Close()
            ss += sd.ToString

            If bolIdentity Then ss += vbCrLf & "SET IDENTITY_INSERT " & strTable & " OFF"
            If IsMsAccess Then
                If ss <> "" Then sb.Append(vbCrLf & ss & "GO")
            Else
                If ss <> "" Then sb.Append(vbCrLf & ss)
            End If

        Next


        If strSave2File <> "" Then

            Dim cFile As New BO.clsFile
            cFile.SaveText2File(strSave2File, sb.ToString, bolAppend2File)
        End If
        Me.Result = sb.ToString
    End Sub
End Class
