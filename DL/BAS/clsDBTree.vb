Imports System.Data
Imports System.Data.SqlClient

Public Class clsDBTree
    Private Property _cDB As DbHandler

    Public BasicWHERE As String
    Private m_strField_Level As String
    Private m_strField_Order As String
    Private m_strField_IdNad As String

    Public Sub New(cDB As DL.DbHandler)
        _cDB = cDB
    End Sub

    Public Sub SaveTree(ByVal strTable As String, ByVal strField_Level As String, ByVal strfield_Name As String, ByVal strField_Order As String, ByVal strField_IdNad As String, ByVal strField_ID As String, ByVal strPrimaryOrderBY_Fields As String, Optional ByVal bolUpdatePrevNext As Boolean = False, Optional ByVal strField_Prev As String = "", Optional ByVal strField_Next As String = "", Optional ByVal strRetError As String = "")
        Dim strSQL As String = ""

        strSQL = "select top 1 " & strField_ID & " from " & strTable & " where isnull(" & strField_IdNad & ",0)<>0 and " & strField_ID & "<>0"
        If BasicWHERE > "" Then strSQL += " AND " & BasicWHERE
        If _cDB.GetValueFromSQL(strSQL) = "" Then
            strSQL = " update " & strTable & " set " & strField_Level & "=1 where isnull(" & strField_Level & ",0)<>1 and " & strField_ID & "<>0"
            If BasicWHERE > "" Then strSQL += " AND " & BasicWHERE
            _cDB.RunSQL(strSQL)
            If bolUpdatePrevNext Then
                strSQL = " update " & strTable & " set " & strField_Order & "=1," & strField_Prev & " = 0, " & strField_Next & "=0 WHERE 1=1"
                If BasicWHERE > "" Then strSQL += " AND " & BasicWHERE
                _cDB.RunSQL(strSQL)
                strSQL = " update " & strTable & " set " & strField_Prev & "=" & strField_Order & "," & strField_Next & " = " & strField_Order & " WHERE " & strField_Prev & "<>" & strField_Next & " and " & strField_ID & "<>0"
                If BasicWHERE > "" Then strSQL += " AND " & BasicWHERE
                _cDB.RunSQL(strSQL)

            End If
            Return
        End If


        m_strField_Level = strField_Level
        m_strField_Order = strField_Order
        m_strField_IdNad = strField_IdNad


        strSQL = "SELECT " & strField_Order & "," & strField_ID & ",isnull(" & strField_IdNad & ",0) as " & strField_IdNad & "," & strfield_Name & "," & strField_Level & " FROM " & strTable & " where " & strField_ID & "<>0"
        If Me.BasicWHERE <> "" Then strSQL = SqlSyntax.AddWhereToSQL(strSQL, Me.BasicWHERE)
        If strPrimaryOrderBY_Fields <> "" Then strSQL = strSQL & " ORDER BY " & strPrimaryOrderBY_Fields
        Dim arr As DataSet = _cDB.GetDataSet(strSQL)
        If arr Is Nothing Then
            strRetError = _cDB.ErrorMessage
            Return
        End If

        MakeTreeOrder(arr, 1, 2, 0, 4)
        SaveTreeOrder(arr, 1, 2, 0, 4, strTable, strPrimaryOrderBY_Fields)

        arr.Clear()

        If bolUpdatePrevNext Then
            SavePrevNext(strTable, strField_ID, strField_Level, strField_Order, strField_Prev, strField_Next)
        End If

    End Sub

    Private Sub SavePrevNext(ByVal strTable As String, ByVal strField_PID As String, ByVal strField_Level As String, ByVal strField_Order As String, ByVal strField_Prev As String, ByVal strField_Next As String)
        Dim strSQL As String = ""
        strSQL = "SELECT " & strField_PID & "," & strField_Level & "," & strField_Order & "," & strField_Prev & "," & strField_Next & " FROM " & strTable
        strSQL = strSQL & " WHERE " & strField_PID & "<>0"
        If Me.BasicWHERE <> "" Then strSQL = SqlSyntax.AddWhereToSQL(strSQL, Me.BasicWHERE)
        strSQL = strSQL & " ORDER BY " & strField_Order


        Dim dbRow As DataRow, lngRows As Long
        Dim dsMaster As DataSet = _cDB.GetDataSet(strSQL, lngRows)
        Dim ds As DataSet = _cDB.GetDataSet(strSQL)


        Dim dt As DataTable, dtMaster As DataTable, lngColID As Integer = 0, lngColLevel As Integer = 1, lngColPrev As Integer = 3, lngColOrder As Integer = 2, lngColNext As Integer = 4
        Dim bolFound As Boolean, dbMasterRow As DataRow, strRunSQLs As String = "", bolForUpdate As Boolean, s As String, x As Long, lngNewNext As Long
        dt = ds.Tables(0)
        dtMaster = dsMaster.Tables(0)

        For Each dbMasterRow In dtMaster.Rows
            bolFound = False
            bolForUpdate = True
            x = 0
            For Each dbRow In dt.Rows
                x = x + 1
                If dbRow(lngColID) = dbMasterRow(lngColID) Then

                    bolFound = True
                    'zpracovávat pouze potenciálně podřízené záznamy
                    If BO.BAS.IsNullInt(dbRow(lngColOrder)) <> BO.BAS.IsNullInt(dbMasterRow(lngColPrev)) Then bolForUpdate = True
                    dbMasterRow(lngColPrev) = dbMasterRow(lngColOrder)
                    s = dbMasterRow(lngColID)

                End If
                If bolFound Then
                    s = dbRow(lngColLevel) & "-" & dbMasterRow(lngColLevel) & "-" & dbRow(lngColOrder) & "-" & dbMasterRow(lngColOrder)
                    If (dbMasterRow(lngColLevel) >= dbRow(lngColLevel) And dbRow(lngColOrder) > dbMasterRow(lngColOrder)) Or x = lngRows Then

                        'došli jsme na záznam ze stejnou úrovní, která se už nezapočítává

                        If x = lngRows Then
                            If dbRow(lngColLevel) <= dbMasterRow(lngColLevel) Then
                                lngNewNext = dbRow(lngColOrder) - 1
                            Else
                                lngNewNext = dbRow(lngColOrder)
                                dbMasterRow(lngColNext) = dbRow(lngColOrder)
                            End If

                        Else
                            lngNewNext = dbRow(lngColOrder) - 1
                        End If
                        If BO.BAS.IsNullInt(dbMasterRow(lngColNext)) <> lngNewNext Then bolForUpdate = True
                        dbMasterRow(lngColNext) = lngNewNext
                        If dbMasterRow(lngColNext) < dbMasterRow(lngColPrev) Then
                            dbMasterRow(lngColNext) = dbMasterRow(lngColOrder)
                            dbMasterRow(lngColPrev) = dbMasterRow(lngColOrder)
                        End If

                        If bolForUpdate Then
                            s = "UPDATE " & strTable
                            s = s & " SET " & strField_Prev & "=" & dbMasterRow(lngColPrev)
                            s = s & "," & strField_Next & " = " & dbMasterRow(lngColNext)
                            s = s & " WHERE " & strField_PID & "=" & dbMasterRow(lngColID)
                            If strRunSQLs <> "" Then
                                strRunSQLs = strRunSQLs & "; " & s
                            Else
                                strRunSQLs = s
                            End If
                            Exit For
                        End If
                    End If
                End If
            Next
        Next

        If strRunSQLs <> "" Then
            _cDB.RunSQL(strRunSQLs)

        End If


    End Sub

    Private Sub SaveTreeOrder(ByVal arrTreeX As DataSet, ByVal lngColID As Integer, ByVal lngColIDNad As Integer, ByVal lngColOrder As Integer, ByVal lngColLevel As Integer, ByVal strTable As String, ByVal strOrderBy As String)
        'uloží provedené změny podle stavu pole arrTree vůči datům v tabulce strTable
        Dim strPrefix As String = "", strSQL As String = "", strSQLX As String = ""

        'lngUbound = UBound(arrTreeX, 2)
        strPrefix = Left(strTable, 3)

        strSQLX = "SELECT * FROM " & strTable
        If Me.BasicWHERE <> "" Then strSQLX = SqlSyntax.AddWhereToSQL(strSQLX, Me.BasicWHERE)
        strSQLX = SqlSyntax.AddWhereToSQL(strSQLX, strPrefix & "ID NOT IN (-1,0)")


        Dim dt As DataTable
        dt = arrTreeX.Tables(0)

        Dim dbRow As DataRow, strRunSQLs As String = "", lngSQLCount As Integer
        Dim dr As SqlDataReader = _cDB.GetDataReader(strSQLX)
        While dr.Read
            For Each dbRow In dt.Rows
                If dbRow(lngColID) = dr.Item(strPrefix & "ID") Then
                    If BO.BAS.IsNullInt(dbRow(lngColOrder)) <> BO.BAS.IsNullInt(dr(m_strField_Order)) Or BO.BAS.IsNullInt(dbRow(lngColLevel)) <> BO.BAS.IsNullInt(dr.Item(m_strField_Level)) Then
                        strSQL = "UPDATE " & strTable & " SET " & m_strField_Order & " =" & BO.BAS.IsNullInt(dbRow(lngColOrder)).ToString & "," & m_strField_Level & "=" & BO.BAS.IsNullInt(dbRow(lngColLevel)).ToString
                        strSQL = strSQL & " WHERE " & strPrefix & "ID = " & dr.Item(strPrefix & "ID")

                        If strRunSQLs <> "" Then
                            strRunSQLs = strRunSQLs & "; " & strSQL
                        Else
                            strRunSQLs = strSQL
                        End If
                        lngSQLCount = lngSQLCount + 1
                    End If
                End If


            Next
        End While
        'dr.Close()

        If strRunSQLs <> "" Then
            _cDB.RunSQL(strRunSQLs)

        End If


    End Sub




    Private Sub MakeTreeOrder(ByVal arrTreeX As DataSet, ByVal lngColID As Integer, ByVal lngColIDNad As Integer, ByVal lngColOrder As Integer, ByVal lngColLevel As Integer)
        'doplní v poli do sloupce lngColOrder absolutní pořadí
        Dim lngCurCounter As Long, lngCurLevel As Long, lngUbound As Long, dbRow As DataRow

        'vyčistit připadné hodnoty ve sloupci order a level
        Dim dt As DataTable
        dt = arrTreeX.Tables(0)

        For Each dbRow In dt.Rows
            dbRow(lngColOrder) = 0
            dbRow(lngColLevel) = 0

        Next

        lngCurCounter = 0
        lngCurLevel = 0

        For Each dbRow In dt.Rows
            If dbRow.Item(lngColIDNad).ToString = "0" Or dbRow.Item(lngColIDNad).ToString = "" Then
                'nemá parent
                Dim lng As Long = 0
                If IsNumeric(dbRow(lngColIDNad).ToString) Then
                    lng = dbRow(lngColIDNad)
                End If
                HandleChild(lng, arrTreeX, lngCurCounter, lngUbound, lngColID, lngColIDNad, lngColOrder, lngColLevel, lngCurLevel)
            End If
        Next


    End Sub

    Private Function HandleChild(ByRef lngID_Value As Integer, ByVal arrTreeX As DataSet, ByRef lngCurCounter As Integer, ByRef lngUbound As Long, ByRef lngColID As Integer, ByRef lngColIDNad As Integer, ByRef lngColOrder As Integer, ByRef lngColLevel As Integer, ByRef lngCurLevel As Integer) As Boolean
        'rekurzivní fce
        Dim dbRow As DataRow

        Dim dt As DataTable
        dt = arrTreeX.Tables(0)

        For Each dbRow In dt.Rows

            If dbRow.Item(lngColIDNad).ToString = lngID_Value.ToString Then
                If dbRow(lngColOrder) = 0 Then
                    lngCurCounter = lngCurCounter + 1
                    lngCurLevel = lngCurLevel + 1
                    dbRow(lngColOrder) = lngCurCounter
                    dbRow(lngColLevel) = lngCurLevel

                    HandleChild(dbRow(lngColID), arrTreeX, lngCurCounter, lngUbound, lngColID, lngColIDNad, lngColOrder, lngColLevel, lngCurLevel)
                End If
            End If

        Next


        lngCurLevel = lngCurLevel - 1
        Return True
    End Function

    Public Function GetTopTreeID(ByVal strTable As String, ByVal strPID As String) As String
        Dim strF As String = Left(strTable, 3) & "id"
        Dim strSQL As String = "select " & strF & "nad from " & strTable & " where " & strF & "=" & strPID
        Dim strRet As String = _cDB.GetValueFromSQL(strSQL)
        While strRet <> "" And strRet <> "0"
            strSQL = "select " & strF & "nad from " & strTable & " where " & strF & "=" & strRet
            Dim s As String = _cDB.GetValueFromSQL(strSQL)
            If s <> "" And s <> "0" Then strRet = s Else Exit While
        End While
        If strRet = "" Or strRet = "0" Then strRet = strPID
        Return strRet
    End Function

    Public Function GetAllIDsNad(ByVal strIDs As String, ByVal strTable As String, Optional ByVal bolIncludeIDs As Boolean = False) As String
        'bolIncludeIDs - zda zahrnout do výsledku i zdrojové strIDs
        Dim lngUboundCount As Long, strPrefixX As String, i As Long, arrIDs() As String
        Dim strSQLX As String, strRet As String = "", strIdNad As String, strID As String

        GetAllIDsNad = ""
        If strIDs Is Nothing Then Return ""
        If strIDs = "" Then Return ""
        strPrefixX = Left(strTable, 3)

        arrIDs = Split(strIDs, ",")
        lngUboundCount = UBound(arrIDs)

        Dim dr As SqlDataReader

        For i = 0 To lngUboundCount
            strID = arrIDs(i) & ""
            strIdNad = strID
            Do Until strIdNad = "" Or strIdNad = "0"
                'dokud se nedohledá top záznam
                strSQLX = "SELECT " & strPrefixX & "IdNad FROM " & strTable & " WHERE " & strPrefixX & "Id=" & strIdNad
                dr = _cDB.GetDataReader(strSQLX)
                If Not dr.HasRows Then
                    'už není nic nad
                    strIdNad = ""
                Else
                    dr.Read()
                    strIdNad = dr.Item(0) & ""
                    If strIdNad <> "0" And strIdNad <> "" Then
                        strRet = strRet & "," & strIdNad
                    End If
                End If
                ''dr.Close()
            Loop
        Next


        If strRet & "" <> "" Then strRet = Right(strRet, Len(strRet) - 1)
        If bolIncludeIDs Then
            If strRet <> "" Then
                strRet = strRet & "," & strIDs
            Else
                strRet = strIDs
            End If
        End If
        Return strRet
    End Function



    Public Function GetAllInnerIDsPerPrevNext(ByVal strIDs As String, ByVal strTable As String, Optional ByVal strFieldID As String = "") As String
        'vrátí všechny podřízené záznamy ve stromu - používá se prev a next!!
        If strFieldID = "" Then strFieldID = Left(strTable, 3) & "ID"
        Dim ds As DataSet = Nothing, strSQL As String = "", a() As String, i As Integer, strFieldPrev As String, strFieldNext As String
        strFieldPrev = Left(strFieldID, 3) & "prev" : strFieldNext = Left(strFieldID, 3) & "next"
        Dim strFieldOrder As String = Left(strTable, 3) & "order"
        a = Split(strIDs, ",")
        For i = 0 To UBound(a)
            strSQL += ";select " & strFieldPrev & "," & strFieldNext & " FROM " & strTable & " WHERE " & strFieldID & "=" & a(i) & " AND " & strFieldPrev & "<>" & strFieldNext
        Next
        If strSQL <> "" Then
            strSQL = Right(strSQL, Len(strSQL) - 1)
            ds = _cDB.GetDataSet(strSQL)
            Dim dbRow As DataRow, strW As String = ""

            For i = 0 To UBound(a)
                For Each dbRow In ds.Tables(i).Rows
                    strW += " OR (" & strFieldOrder & ">=" & dbRow.Item(0) & " AND " & strFieldOrder & " <= " & dbRow.Item(1) & ")"
                Next
            Next
            If strW <> "" Then
                strW += " OR " & strFieldID & " IN (" & strIDs & ")"
                strW = Right(strW, Len(strW) - 3)
                Return _cDB.GetValueFromSQL("select " & strFieldID & " FROM " & strTable & " WHERE " & strW, True)
            Else
                Return strIDs
            End If
            ds.Clear()
        Else
            Return ""
        End If


    End Function


    Public Function TestInnerValueTree(ByVal lngRecordPID As Integer, ByVal lngNewIdNad As Integer, ByVal strTablename As String) As Boolean
        If lngRecordPID = 0 Or lngNewIdNad = 0 Then Return True 'pro nový záznam nemá cenu testovat
        Dim strSQL As String, strPref As String = Left(strTablename, 3)
        strSQL = "select " & strPref & "prev," & strPref & "next FROM " & strTablename & " WHERE " & strPref & "id=" & lngRecordPID

        Dim dr As SqlClient.SqlDataReader = _cDB.GetDataReader(strSQL)
        dr.Read()
        Dim lngPrev As Long = dr.Item(0), lngNext As Long = dr.Item(1)
        dr.Close()
        Dim lngTreeOrder As Integer = CLng(_cDB.GetValueFromSQL("select " & strPref & "order FROM " & strTablename & " WHERE " & strPref & "ID=" & lngNewIdNad))

        If lngPrev <= lngTreeOrder And lngNext >= lngTreeOrder Then
            If lngPrev > 0 Or lngNext > 0 Or lngTreeOrder > 0 Then Return False
        End If

        Return True
    End Function



End Class


