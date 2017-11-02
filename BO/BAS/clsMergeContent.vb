Public Class clsMergeContent
    Public Function MergeContent(objects As List(Of Object), strTemplateContent As String, strLinkUrl As String) As String
        If strLinkUrl <> "" Then strTemplateContent = Replace(strTemplateContent, "[%link%]", strLinkUrl, , , CompareMethod.Text)

        Dim fields As List(Of String) = GetAllMergeFieldsInContent(strTemplateContent)
        Dim reps As New List(Of BO.EasyStringColletion)

        For Each c In objects
            For Each field As String In fields
                Dim val As Object = BO.BAS.GetPropertyValue(c, field)
                Dim strReplace As String = ""
                If Not val Is Nothing Then
                    Select Case val.GetType.ToString
                        Case "System.String"
                            strReplace = val
                        Case "System.DateTime"
                            strReplace = BO.BAS.FD(val, True)
                        Case "System.Double"
                            strReplace = BO.BAS.FN(val)
                        Case "System.Boolean"
                            If val Then strReplace = "ANO" Else strReplace = "NE"
                        Case "System.Int32"
                            strReplace = BO.BAS.FNI(val)
                        Case Else
                            strReplace = val.ToString
                    End Select
                    If strReplace <> "" Then reps.Add(New BO.EasyStringColletion(field, strReplace))
                End If
            Next
        Next
        For Each c In reps  'nahradit nalezené pole
            strTemplateContent = Replace(strTemplateContent, "[%" & c.Key & "%]", c.Value, , , CompareMethod.Text)
        Next
        For Each field In fields    'nahradit zbylá prázdná pole
            If strTemplateContent.IndexOf("[%" & field & "%]") > 0 Then
                strTemplateContent = Replace(strTemplateContent, "[%" & field & "%]", "")
            End If
        Next

        Return strTemplateContent
    End Function

    Public Function GetAllMergeFieldsInContent(ByVal strContent As String) As List(Of String)
        'vrátí seznam slučovacích polí, které se vyskytují v strContent
        Dim lisRet As New List(Of String)
        If Trim(strContent) = "" Then Return lisRet

        If strContent.IndexOf("[%") >= 0 Then
            've výchozím popisu aktivity jsou slučovací pole z projektu
            Dim matches As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(strContent, "\[%.*?\%]")
            For Each m As System.Text.RegularExpressions.Match In matches
                Dim strField As String = Replace(m.Value, "[%", "").Replace("%]", "")
                'strDefText = Replace(strDefText, m.Value, BO.BAS.GetPropertyValue(cP41, strField))
                lisRet.Add(strField)
            Next
        End If
        Return lisRet

    End Function
End Class
