Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Dapper
Imports log4net


Public Class DbHandler
    Private Property _conString As String
    Private Property _IsDebugSql As Boolean = False
    Private Property _Error As String

    <ThreadStatic()> Public Shared lastException As SqlException

    Private Property _LastIdentityValue As Integer
    Private Property _LastSavedRecordPID As Integer

    Public Event OnDBError(strError As String)
    Public Event OnSaveRecord(intLastSavedPID As Integer)
    
    Public ReadOnly Property LastIdentityValue As Integer
        Get
            Return _LastIdentityValue
        End Get
    End Property
    Public ReadOnly Property LastSavedRecordPID As Integer
        Get
            Return _LastSavedRecordPID
        End Get
    End Property
    ''Public Sub ChangeLastSavedRecordPID(intNewLastSavedRecordPID As Integer)
    ''    _LastIdentityValue = intNewLastSavedRecordPID
    ''    _LastSavedRecordPID = intNewLastSavedRecordPID
    ''End Sub

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public ReadOnly Property ErrorCode As Integer
        Get
            If lastException Is Nothing Then Return 0
            Return lastException.ErrorCode
        End Get
    End Property

    Private Overloads Sub Handle_OnError(strErrorMessage As String)
        _Error = strErrorMessage
        RaiseEvent OnDBError(strErrorMessage)
        LogManager.GetLogger("sqllog").Error(strErrorMessage)
    End Sub
    Private Overloads Sub Handle_OnError(ex As System.Exception)
        _Error = ex.Message
        RaiseEvent OnDBError(ex.Message)
        LogManager.GetLogger("sqllog").Error(ex.Message)
    End Sub

    Private Overloads Sub Handle_DebugSql(strSQL As String, params As Object)
        If Not params Is Nothing Then
            LogManager.GetLogger("debuglog").Info(APS(strSQL, params))
        Else
            LogManager.GetLogger("debuglog").Info(strSQL)
        End If
    End Sub

    Private Overloads Sub Handle_OnError(ex As System.Exception, strSQL As String, params As Object)
        If TypeOf ex Is System.Exception Or TypeOf ex Is SqlException Then
            Try
                lastException = ex
            Catch ex2 As Exception

            End Try

        End If

        _Error = ex.Message

        RaiseEvent OnDBError(ex.Message)
        If Not params Is Nothing Then
            LogManager.GetLogger("sqllog").Error(APS(strSQL, params), ex)
        Else
            LogManager.GetLogger("sqllog").Error(strSQL, ex)
        End If
    End Sub

    Public Sub New()
        _conString = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationPrimary").ToString()
        If _conString = "" Then
            Handle_OnError("ApplicationPrimary connect string není definován ve web.config")
        End If
        ''If System.Configuration.ConfigurationManager.AppSettings.Item("debugsql") = "1" Then _IsDebugSql = True
    End Sub
    Public Function GetConstring() As String
        Return _conString
    End Function
    Public Sub ChangeConString(strNewConstring As String)
        _conString = strNewConstring
    End Sub
    Public Sub ChangeConString2Membership()
        _conString = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationServices").ToString()
    End Sub
    Public Sub ChangeConString2Primary()
        _conString = System.Configuration.ConfigurationManager.ConnectionStrings.Item("ApplicationPrimary").ToString()
    End Sub

    

    Public Overloads Function GetRecord(Of T)(strSQL As String) As T

        Using con As New SqlConnection(_conString)
            con.Open()
            If _IsDebugSql Then Handle_DebugSql(strSQL, Nothing)
            Dim rec As T = con.Query(Of T)(strSQL).FirstOrDefault
            con.Close()
            Return rec
        End Using

    End Function

    Public Overloads Function GetRecord(Of T)(strSQL As String, params As DbParameters, Optional bolStoredProcedure As Boolean = False) As T
        Dim cmdType As CommandType = Nothing
        If bolStoredProcedure Then cmdType = CommandType.StoredProcedure

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                If _IsDebugSql Then Handle_DebugSql(strSQL, params)
                Dim rec As T = con.Query(Of T)(strSQL, params, , , , cmdType).FirstOrDefault
                con.Close()
                Return rec
            Catch ex As Exception
                con.Close()
                Handle_OnError(ex, strSQL, params)
                Return Nothing
            End Try

        End Using

    End Function
    Public Overloads Function GetRecord(Of T)(strSQL As String, params As Object, Optional bolStoredProcedure As Boolean = False) As T
        Dim cmdType As CommandType = Nothing
        If bolStoredProcedure Then cmdType = CommandType.StoredProcedure

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                If _IsDebugSql Then Handle_DebugSql(strSQL, params)
                Dim rec As T = con.Query(Of T)(strSQL, params, , , , cmdType).FirstOrDefault
                con.Close()
                Return rec
            Catch ex As Exception
                con.Close()
                Handle_OnError(ex, strSQL, params)
                Return Nothing
            End Try
            
        End Using

    End Function

    Public Function GetDataSet(strSQL As String, Optional ByRef intRetRows As Integer = 0, Optional pars As List(Of BO.PluginDbParameter) = Nothing, Optional strDatasetName As String = "") As DataSet
        intRetRows = 0
        Dim adapter As New SqlDataAdapter

        Using con As New SqlConnection(_conString)
            con.Open()

            Dim cmd As New SqlCommand(strSQL, con)
            If Not pars Is Nothing Then
                For Each c In pars
                    cmd.Parameters.AddWithValue(c.Name, c.Value)
                Next
            End If
            If _IsDebugSql Then
                Dim p As New DbParameters
                Handle_DebugSql(strSQL, pars)
            End If

            adapter.SelectCommand = cmd

            'adapter.SelectCommand = New SqlCommand(strSQL, con)

            con.Close()
            Dim ds As New DataSet()
            If strDatasetName <> "" Then ds = New DataSet(strDatasetName)
            Try
                adapter.Fill(ds)
                intRetRows = ds.Tables(0).Rows.Count()
            Catch ex As Exception
                intRetRows = 0
                Handle_OnError(ex, strSQL, Nothing)
                Return Nothing
            End Try
            Return ds
        End Using
    End Function
    Public Function GetDataSetBySP(strSP As String, Optional ByRef intRetRows As Integer = 0, Optional pars As List(Of BO.PluginDbParameter) = Nothing, Optional strDatasetName As String = "") As DataSet
        intRetRows = 0
        Dim adapter As New SqlDataAdapter

        Using con As New SqlConnection(_conString)
            con.Open()
            Dim cmd As New SqlCommand(strSP, con)
            If Not pars Is Nothing Then
                For Each c In pars
                    cmd.Parameters.AddWithValue(c.Name, c.Value)
                Next
            End If
            cmd.CommandType = CommandType.StoredProcedure

            adapter.SelectCommand = cmd

            'adapter.SelectCommand = New SqlCommand(strSQL, con)

            con.Close()
            Dim ds As New DataSet()
            If strDatasetName <> "" Then ds = New DataSet(strDatasetName)
            Try
                adapter.Fill(ds)
                intRetRows = ds.Tables(0).Rows.Count()
            Catch ex As Exception
                intRetRows = 0
                Handle_OnError(ex, strSP, Nothing)
                Return Nothing
            End Try
            Return ds
        End Using
    End Function
    Public Function GetDataReader(ByVal strSQL As String, Optional ByVal intExplicitTimeout As Integer = -1, Optional pars As List(Of BO.PluginDbParameter) = Nothing) As SqlDataReader
        'fce vrací sqldatareader
        _Error = ""
        If strSQL = "" Then
            LogManager.GetLogger("sqllog").Warn("strSQL je na vstupu prázdný.")
            Return Nothing
        End If
        Dim con As New SqlConnection(_conString)

        con.Open()
        Dim comm As New SqlCommand(strSQL, con)
        If intExplicitTimeout > -1 Then comm.CommandTimeout = intExplicitTimeout
        If Not pars Is Nothing Then
            For Each c In pars
                comm.Parameters.AddWithValue(c.Name, c.Value)
            Next
        End If

        Try
            Return comm.ExecuteReader(Data.CommandBehavior.CloseConnection)
        Catch ex As Exception
            con.Close()
            Handle_OnError(ex, strSQL, Nothing)
            Return Nothing
        End Try

    End Function

    Public Overloads Function GetList(Of T)(strSQL As String) As IEnumerable(Of T)

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                Dim lis As IEnumerable(Of T) = con.Query(Of T)(strSQL)
                con.Close()
                Return lis
            Catch ex As Exception
                con.Close()
                Handle_OnError(ex, strSQL, Nothing)
                Return Nothing
            End Try

        End Using

    End Function
    Public Overloads Function GetList(Of T)(strSQL As String, params As DbParameters) As IEnumerable(Of T)

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                If _IsDebugSql Then Handle_DebugSql(strSQL, params)
                Dim lis As IEnumerable(Of T) = con.Query(Of T)(strSQL, params)

                con.Close()
                Return lis
            Catch ex As Exception
                con.Close()
                Handle_OnError(ex, strSQL, params)
                Return Nothing
            End Try

        End Using

    End Function
    Public Overloads Function GetList(Of T)(strSQL As String, params As Object) As IEnumerable(Of T)

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                Dim lis As IEnumerable(Of T) = con.Query(Of T)(strSQL, params)
                con.Close()
                Return lis
            Catch ex As Exception
                con.Close()
                Handle_OnError(ex, strSQL, params)
                Return Nothing
            End Try


        End Using

    End Function
    Public Overloads Function GetListBySP(Of T)(strProcName As String, params As DbParameters) As IEnumerable(Of T)

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                Dim lis As IEnumerable(Of T) = con.Query(Of T)(strProcName, params, , , , CommandType.StoredProcedure)
                con.Close()
                Return lis
            Catch ex As SqlClient.SqlException
                Handle_OnError(ex, strProcName, params)
                con.Close()
                Return Nothing
            End Try
        End Using

    End Function

    Public Function RunSP(strProcName As String, ByRef params As DbParameters, Optional strTable4ValidateFields As String = "") As Boolean
        Dim b As Boolean = False

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                If strTable4ValidateFields <> "" Then
                    If Not params.TestFieldSize(con, strTable4ValidateFields) Then
                        Handle_OnError(params.ErrorMessage)
                        con.Close()
                        Return False
                    End If
                End If

                con.Execute(strProcName, params, , , CommandType.StoredProcedure)
                b = True
                If params.ParameterNames.Where(Function(p) LCase(p) = "err_ret").Count > 0 Then
                    If params.Get(Of String)("err_ret") <> "" Then
                        _Error = params.Get(Of String)("err_ret")
                        If _Error.IndexOf("Line:") > 0 Then
                            'klasická chyba - zapisovat do error logu jako jiné sql chyby
                            Handle_OnError(_Error)
                        Else
                            'tento stav se nezapisuje do sql error logu
                            RaiseEvent OnDBError(_Error)
                        End If
                        con.Close()
                        Return False
                    End If
                End If
                
                
                con.Close()
            Catch ex As SqlClient.SqlException
                Handle_OnError(ex, strProcName, params)
                con.Close()
            End Try
        End Using

        Return b
    End Function
    Public Function RunSQL(strSQL As String, Optional params As Object = Nothing) As Boolean
        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                con.Execute(strSQL, params)
                con.Close()

                Return True
            Catch ex As SqlClient.SqlException
                Handle_OnError(ex, strSQL, params)
                con.Close()
            End Try
        End Using
        Return False

    End Function

    Public Function SaveRecord(strTable As String, params As DbParameters, Optional ByVal bolINSERT As Boolean = False, Optional ByVal strWHERE As String = "", Optional ByVal bolAutoTimestamp As Boolean = False, Optional ByVal strTimestampUser As String = "", Optional bolRefreshLastSavedPIDInfo As Boolean = True) As Boolean
        If params.ParameterNames.Count = 0 Then
            Handle_OnError("SaveRecord: params je prázdný!")
            Return False
        End If
        If bolRefreshLastSavedPIDInfo Then
            _LastIdentityValue = 0 : _LastSavedRecordPID = 0
        End If


        Dim b As Boolean = False
        Dim s As String = "", strV As String = "", strF As String = "", strPrefix As String = Left(strTable, 3)
        If bolINSERT Then
            s = "INSERT INTO " & strTable & " ("
            If bolAutoTimestamp Then
                params.Add(strPrefix & "dateinsert", Now, DbType.DateTime)
                params.Add(strPrefix & "userinsert", strTimestampUser, DbType.String)
            End If
        Else
            s = "UPDATE " & strTable & " SET "
        End If
        If bolAutoTimestamp Then
            params.Add(strPrefix & "dateupdate", Now, DbType.DateTime)
            params.Add(strPrefix & "userupdate", strTimestampUser, DbType.String)
        End If

        For Each strPar As String In params.ParameterNames
            If LCase(strPar) <> "pid" Then
                If bolINSERT Then
                    strF += "," & strPar
                    strV += ",@" & strPar
                Else
                    strV += "," & strPar & " = @" & strPar
                End If
            End If
        Next
        strV = Right(strV, Len(strV) - 1)
        If bolINSERT Then
            strF = Right(strF, Len(strF) - 1)
            s += strF & ") VALUES (" & strV & ")"
        Else
            s += strV
            If strWHERE <> "" Then
                s += " WHERE " & strWHERE
            Else
                _Error = "U aktualizačního SQL musí být specifikována WHERE podmínka."
                Return False
            End If
        End If
        If Not params.TestRecordValidity(strTable) Then
            Handle_OnError(params.ErrorMessage)
            Return False
        End If

        Using con As New SqlConnection(_conString)
            con.Open()
            Try
                If Not params.TestFieldSize(con, strTable) Then
                    Handle_OnError(params.ErrorMessage)
                    con.Close()
                    Return False
                End If
                If con.Execute(s, params) > 0 Then
                    b = True
                    If bolINSERT Then
                        Dim lis As IEnumerable(Of BO.GetInteger) = con.Query(Of BO.GetInteger)("SELECT @@IDENTITY as Value")
                        If lis.Count > 0 And bolRefreshLastSavedPIDInfo Then
                            _LastIdentityValue = lis(0).Value
                            _LastSavedRecordPID = _LastIdentityValue
                        End If
                    Else
                        If bolRefreshLastSavedPIDInfo Then _LastSavedRecordPID = params.Get(Of Integer)("pid")
                    End If
                    If bolRefreshLastSavedPIDInfo Then RaiseEvent OnSaveRecord(_LastSavedRecordPID)

                    
                Else
                    Handle_OnError("Neznámá chyba k příkazu: " & s)

                End If
            Catch ex As SqlClient.SqlException
                Handle_OnError(ex, s, params)
            End Try
            con.Close()
        End Using

        Return b
    End Function
    Private Overloads Function _GetScalarFromSQL(ByVal strSQL As String, SqlParams As SqlParameterCollection) As Object
        _Error = ""
        Using con As New SqlConnection(_conString)
            con.Open()

            Dim comm As New SqlCommand(strSQL, con)
            For Each par As SqlParameter In SqlParams
                comm.Parameters.Add(par)
            Next
            Try
                Dim o As Object = comm.ExecuteScalar
                con.Close()
                Return o
            Catch ex As SqlException
                Handle_OnError(ex, strSQL, Nothing)
                con.Close()
                Return Nothing
            End Try
        End Using
    End Function
    Private Overloads Function _GetScalarFromSQL(ByVal strSQL As String) As Object
        _Error = ""
        Using con As New SqlConnection(_conString)
            con.Open()

            Dim comm As New SqlCommand(strSQL, con)
            Try
                Dim o As Object = comm.ExecuteScalar
                con.Close()
                Return o
            Catch ex As SqlException
                Handle_OnError(ex, strSQL, Nothing)
                con.Close()
                Return Nothing
            End Try
        End Using
    End Function
    ''' <summary>
    ''' Vrací z SQL int hodnotu
    ''' </summary>
    ''' <param name="strSQL">SQL dotaz má mít pouze jeden sloupec, jeho alias hodnotou 'AS' musí být 'Value'!!! </param>
    ''' <param name="pars"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetIntegerValueFROMSQL(strSQL As String, pars As DbParameters) As Integer
        Using con As New SqlConnection(_conString)
            con.Open()
            Dim rec As BO.GetInteger = con.Query(Of BO.GetInteger)(strSQL, pars).FirstOrDefault
            con.Close()
            If rec Is Nothing Then
                Return 0
            Else
                Return rec.Value
            End If
        End Using
    End Function
    Public Overloads Function GetIntegerValueFROMSQL(strSQL As String, pars As Object) As Integer
        Using con As New SqlConnection(_conString)
            con.Open()
            Dim rec As BO.GetInteger = con.Query(Of BO.GetInteger)(strSQL, pars).FirstOrDefault
            con.Close()
            If rec Is Nothing Then
                Return 0
            Else
                Return rec.Value
            End If
        End Using
    End Function
    Public Overloads Function GetIntegerValueFROMSQL(ByVal strSQL As String) As Integer
        'vrací 1 pole typu int32 přes ADO scalar
        Dim o As Object = _GetScalarFromSQL(strSQL)
        If o Is Nothing Then Return 0
        If o Is System.DBNull.Value Then Return 0
        Return CInt(o)
    End Function
    Public Function GetDoubleValueFROMSQL(strSQL As String, pars As DbParameters) As Double
        Using con As New SqlConnection(_conString)
            con.Open()
            Dim rec As BO.GetDouble = con.Query(Of BO.GetDouble)(strSQL, pars).FirstOrDefault
            con.Close()
            If rec Is Nothing Then
                Return 0
            Else
                Return rec.Value
            End If
        End Using
    End Function
    Public Overloads Function GetValueFromSQL(strSQL As String, pars As DbParameters) As String
        Using con As New SqlConnection(_conString)
            con.Open()
            Dim rec As BO.GetString = con.Query(Of BO.GetString)(strSQL, pars).FirstOrDefault
            con.Close()
            If rec Is Nothing Then
                Return ""
            Else
                Return rec.Value
            End If
        End Using
    End Function
    Public Overloads Function GetValueFromSQL(ByVal strSQL As String, Optional ByVal bolForIN As Boolean = False, Optional ByVal bolApostrofy As Boolean = False, Optional ByVal strDelimiter As String = ",", Optional ByVal intExplicitTimeout As Integer = -1) As String
        'fce vrací hodnotu z SQL dotazu - má smysl předávat pouze jedno sloupcový sql dotaz
        'pokud je bolForIN true, pak se vrací hodnoty ze všech záznamů oddělené čárkou
        'bolApostrofy - hodnoty jsou uvozeny apostrofy
        If bolForIN Then Return _GetOneColumnReaderInString(strSQL, intExplicitTimeout, bolApostrofy, strDelimiter)

        Dim o As Object = _GetScalarFromSQL(strSQL)
        Dim strDefRet As String = ""
        If bolApostrofy Then strDefRet = "''"
        If o Is Nothing Then Return strDefRet
        If o Is System.DBNull.Value Then Return strDefRet

        If bolApostrofy Then
            Return "'" & o.ToString & "'"
        Else
            Return o.ToString
        End If
    End Function
    Private Function _GetOneColumnReaderInString(ByVal strSQL As String, ByVal intExplicitTimeout As Integer, ByVal bolApostrofy As Boolean, ByVal strDelimiter As String) As String
        _Error = ""
        Dim dr As SqlDataReader = GetDataReader(strSQL, intExplicitTimeout)
        If _Error <> "" Then
            LogManager.GetLogger("sqllog").Warn(_Error)
            Return ""
        End If

        Dim sb As New System.Text.StringBuilder, i As Integer = 0
        While dr.Read
            If i > 0 Then sb.Append(strDelimiter)
            If Not bolApostrofy Then
                sb.Append(dr.GetValue(0))
            Else
                sb.Append("'" & dr.GetValue(0) & "'")
            End If
            i += 1
        End While
        dr.Close()
        Return sb.ToString
    End Function

    ''Private Overloads Function APS(s As String, pars As DbParameters)
    ''    If Not pars Is Nothing Then
    ''        For Each strPar As String In pars.ParameterNames
    ''            s += vbCrLf & "Parameter " & strPar & ": " & pars.Get(Of Object)(strPar) & ""
    ''        Next
    ''    End If
    ''    Return s
    ''End Function
    ''Private Overloads Function APS(s As String, pars As List(Of BO.PluginDbParameter))
    ''    If Not pars Is Nothing Then
    ''        For Each c In pars
    ''            s += vbCrLf & "Parameter " & c.Name & ": " & c.Value & ""
    ''        Next
    ''    End If
    ''    Return s
    ''End Function


    Private Overloads Function APS(s As String, pars As Object)
        If Not pars Is Nothing Then
            If TypeOf pars Is DbParameters Then
                Dim p As List(Of BO.PluginDbParameter) = CType(pars, DbParameters).Convert2PluginDbParameters()
                For Each c In p
                    s += vbCrLf & "@" & c.Name & " = " & c.Value & ""
                Next
                ''For Each strPar As String In pars.ParameterNames
                ''    CType(pars, DbParameters).Convert2PluginDbParameters()
                ''    's += vbCrLf & "Parameter " & strPar & ": " & pars.Get(Of Object)(strPar) & ""
                ''Next
                Return s
            End If
            If TypeOf pars Is List(Of BO.PluginDbParameter) Then
                For Each c In pars
                    s += vbCrLf & "@" & c.Name & " = " & c.Value & ""
                Next
                Return s
            End If
            Return vbCrLf & "Parameter: " & pars.ToString
        Else
            Return ""
        End If
    End Function
   

End Class
