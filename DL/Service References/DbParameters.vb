Imports System.Data
Imports System.Data.SqlClient
Imports Dapper

Public Class DbParameters
    Inherits DynamicParameters

    Protected Class DbParameter
        Public Property Name As String
        Public Property DbType As System.Data.DbType? = Nothing
        Public Property DbValue As Object = Nothing
        Public Property Direction As ParameterDirection? = Nothing
        Public Property Size As Integer? = Nothing
        Public Property IsValidateFieldSize As Boolean = False
        Public Property NameAlias As String
    End Class
    Private _Error As String
    Private _lis As List(Of DbParameter)

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _Error
        End Get
    End Property
    Public Sub New()
        _lis = New List(Of DbParameter)
    End Sub

    Public Overloads Sub Add(name As String, Optional value As Object = Nothing, Optional dbType As System.Data.DbType? = Nothing, Optional direction As System.Data.ParameterDirection? = Nothing, Optional size As Integer? = Nothing, Optional bolValidateFieldSize As Boolean = False, Optional nameAlias As String = "")
        Dim par As New DbParameter()
        If Not value Is Nothing Then
            Select Case dbType
                Case Data.DbType.String, Data.DbType.StringFixedLength
                    If value Is System.DBNull.Value Then
                        value = Nothing
                    Else
                        If value = "" Then value = Nothing
                    End If

            End Select
        End If

        With par
            .Name = name
            .DbValue = value
            .DbType = dbType
            .Direction = direction
            .Size = size
            .IsValidateFieldSize = bolValidateFieldSize
            .NameAlias = nameAlias
        End With
        _lis.Add(par)

        MyBase.Add(name, value, dbType, direction, size)

    End Sub

    Public Function GetListInLine() As String
        Dim s As String = ""
        For Each par As DbParameter In _lis
            Try
                s += vbCrLf & par.Name & ": " & par.DbValue
            Catch ex As Exception

            End Try
        Next
        Return s
    End Function

    Public Function Convert2PluginDbParameters() As List(Of BO.PluginDbParameter)
        Dim prs As New List(Of BO.PluginDbParameter)
        For Each par As DbParameter In _lis
            prs.Add(New BO.PluginDbParameter(par.Name, par.DbValue))
        Next
        Return prs
    End Function

    Public Function TestRecordValidity(strTable As String) As Boolean
        _Error = ""
        strTable = Left(strTable, 3)
        Dim datTestValidFrom As Date? = Nothing, datTestValidUntil As Date? = Nothing
        For Each par As DbParameter In _lis
            If LCase(par.Name) = strTable & "validuntil" Then
                datTestValidUntil = par.DbValue
            End If
            If LCase(par.Name) = strTable & "validfrom" Then
                datTestValidFrom = par.DbValue
            End If
        Next
        If Not (datTestValidUntil Is Nothing And datTestValidFrom Is Nothing) Then
            If datTestValidFrom > datTestValidUntil Then
                _Error = "Datum konce platnosti záznamu musí být větší než datum začátku platnosti záznamu."
                Return False
            End If
        End If
        Return True
    End Function

    Public Function TestFieldSize(con As SqlConnection, strTable As String) As Boolean
        _Error = ""
        For Each par As DbParameter In _lis
            If par.IsValidateFieldSize And Not par.DbValue Is Nothing Then
                Dim lis As IEnumerable(Of BO.GetInteger) = con.Query(Of BO.GetInteger)("SELECT dbo.getfieldsize('" & par.Name & "','" & strTable & "') as Value")
                If lis.Count > 0 Then
                    If par.DbValue.ToString.Length > lis(0).Value And lis(0).Value > 0 Then
                        Dim strField As String = par.Name
                        If par.NameAlias <> "" Then strField = par.NameAlias
                        _Error += "<hr>Maximální počet znaků pole [" & strField & "] může být " & lis(0).Value.ToString & ". Počet předávaných znaků: " & par.DbValue.ToString.Length.ToString & "!"
                    End If
                End If
            End If
        Next
        If _Error <> "" Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function CompleteNoParamSQL(strSourceSQL As String) As String
        Dim s As String = strSourceSQL
        For Each par As DbParameter In _lis
            Dim strVal As String = ""
            Select Case par.DbType
                Case DbType.DateTime, DbType.Date, DbType.DateTime2
                    strVal = BO.BAS.GetHashDate(par.DbValue, True)
                Case DbType.Boolean
                    strVal = BO.BAS.GB(par.DbValue)
                Case DbType.Decimal, DbType.Double
                    strVal = BO.BAS.GN(par.DbValue.ToString)
                Case DbType.Int32, DbType.Int16
                    strVal = par.DbValue.ToString
                Case Else
                    strVal = BO.BAS.GS(par.DbValue)
            End Select
            s = Replace(s, "@" & par.Name, strVal, , , CompareMethod.Text)
        Next
        Return s
    End Function
End Class


