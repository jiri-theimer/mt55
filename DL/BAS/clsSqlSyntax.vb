Public Class SqlSyntax
    Public Shared Function AddWhereToSQL(ByVal strSQL As String, ByVal strWHERE As String, Optional ByVal strOPERATOR As String = "AND") As String
        'vrací kompletní SQL, vč. modifikované klauzule WHERE
        'strOPERATOR může být OR nebo AND, když nic, tak je to AND
        Dim str As String = "", a(10) As String, o(10) As String, bolClassic As Boolean, lngStartWHERE As Integer

        If Trim(strWHERE) = "" Or Trim(strWHERE) = "()" Then
            Return strSQL
        End If
        If Trim(strSQL) = "" Then
            Return strWHERE
        End If

        a = Split(UCase(strSQL), "UNION")
        If UBound(a) <= 0 Then
            lngStartWHERE = UCase(strSQL).LastIndexOf("WHERE")
            If lngStartWHERE > 0 Then
                If InStr(Right(strSQL, Len(strSQL) - lngStartWHERE), " ON ", CompareMethod.Text) > 0 Then
                    'klauzule where byla pouze součástí JOIN klauzulí
                    lngStartWHERE = 0
                End If
            End If
            If lngStartWHERE > 0 Then
                bolClassic = True
                If InStr(1, UCase(strSQL), "GROUP BY", 1) > 0 Then
                    'v dotazu už je uvedena i klauzule ORDER BY
                    o = Split(UCase(strSQL), "GROUP BY")
                    str = o(0) & " " & strOPERATOR & " (" & strWHERE & ") GROUP BY " & o(1)
                    bolClassic = False
                End If
                If InStr(1, strSQL, "ORDER BY", CompareMethod.Text) > 0 And bolClassic Then
                    'v dotazu už je uvedena i klauzule ORDER BY
                    o = Split(UCase(strSQL), "ORDER BY")
                    str = o(0) & " " & strOPERATOR & " (" & strWHERE & ") ORDER BY " & o(1)
                    bolClassic = False
                End If
                If bolClassic Then
                    str = strSQL & " " & strOPERATOR & " (" & strWHERE & ")"
                End If
            Else
                If InStr(1, strSQL, "FROM", CompareMethod.Text) > 0 Then
                    If InStr(1, UCase(strSQL), "ORDER BY", 1) > 0 Then
                        o = Split(UCase(strSQL), "ORDER BY")
                        str = o(0) & " WHERE (" & strWHERE & ") ORDER BY " & o(1)
                    Else
                        str = strSQL & " WHERE " & strWHERE
                    End If

                Else
                    'není uvedeno ani WHERE ani SELECT
                    str = strSQL & " " & strOPERATOR & " (" & strWHERE & ")"
                End If
            End If
        Else
            'union select
            If InStr(1, a(UBound(a)), "WHERE", 1) > 0 Then
                str = strSQL & " " & strOPERATOR & " (" & strWHERE & ")"
            Else
                str = strSQL & " WHERE " & strWHERE
            End If
        End If
        Return str
    End Function


End Class

