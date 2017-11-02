Imports System.Text.RegularExpressions
Public Class clsHandleHtml
        Public ErrorMessage As String

        Public Function ExtractEmailAddressesFromString(ByVal source As String) As String()
            Dim mc As MatchCollection
            Dim i As Integer

            ' expression garnered from www.regexlib.com - thanks guys!
            mc = Regex.Matches(source, _
                "([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})")
            Dim results(mc.Count - 1) As String
            For i = 0 To results.Length - 1
                results(i) = mc(i).Value
            Next

            Return results
        End Function

        Public Function ConvertHtml2Text(ByVal source As String) As String
            Try
                Dim result As String = ""

                ' Remove HTML Development formatting
                ' Replace line breaks with space
                ' because browsers inserts space
                result = source.Replace(vbCr, " ")
                ' Replace line breaks with space
                ' because browsers inserts space
                result = result.Replace(vbLf, " ")
                ' Remove step-formatting
                result = result.Replace(vbTab, String.Empty)
                ' Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result, "( )+", " ")

                ' Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                'result = System.Text.RegularExpressions.Regex.Replace(result,
                '         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                '         string.Empty,
                '         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<script>).*(</script>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*td([^>])*>", vbTab, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*br( )*>", vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*li( )*>", vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' insert line paragraphs (double line breaks) in place
                ' if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*div([^>])*>", vbCr & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*tr([^>])*>", vbCr & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*p([^>])*>", vbCr & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' Remove remaining tags like <a>, links, images,
                ' comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result, "<[^>]*>", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result, " ", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                result = System.Text.RegularExpressions.Regex.Replace(result, "&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&lt;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&gt;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                ' Remove all others. More can be added, see

                result = System.Text.RegularExpressions.Regex.Replace(result, "&(.{2,6});", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)

                ' for testing
                'System.Text.RegularExpressions.Regex.Replace(result,
                '       this.txtRegex.Text,string.Empty,
                '       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                ' make line breaking consistent
                result = result.Replace(vbLf, vbCr)

                ' Remove extra line breaks and tabs:
                ' replace over 2 breaks with 2 and over 4 tabs with 4.
                ' Prepare first to remove any whitespaces in between
                ' the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbCr & ")( )+(" & vbCr & ")", vbCr & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbTab & ")( )+(" & vbTab & ")", vbTab & vbTab, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbTab & ")( )+(" & vbCr & ")", vbTab & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbCr & ")( )+(" & vbTab & ")", vbCr & vbTab, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                ' Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbCr & ")(" & vbTab & ")+(" & vbCr & ")", vbCr & vbCr, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                ' Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result, "(" & vbCr & ")(" & vbTab & ")+", vbCr & vbTab, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                ' Initial replacement target string for line breaks
                Dim breaks As String = vbCr & vbCr & vbCr
                ' Initial replacement target string for tabs
                Dim tabs As String = vbTab & vbTab & vbTab & vbTab & vbTab
                For index As Integer = 0 To result.Length - 1
                    result = result.Replace(breaks, vbCr & vbCr)
                    result = result.Replace(tabs, vbTab & vbTab & vbTab & vbTab)
                    breaks = breaks & vbCr
                    tabs = tabs & vbTab
                Next

                ' That's it.
                Return result
            Catch ex As Exception
                Me.ErrorMessage = ex.Message
                Return source
            End Try
        End Function


        Public Function OrezatDiakritiku(ByVal str As String) As String
            Dim s As String = ""
            For i As Integer = 1 To Len(str)
                s += OrezatDiakritiku_Znak(Mid(str, i, 1))
            Next
            Return s
        End Function

        Private Function OrezatDiakritiku_Znak(ByVal s As String) As String
            Select Case Asc(s)
                Case 205 : Return "I"
                Case 200 : Return "C"
                Case 141 : Return "T"
                Case 218 : Return "U"
                Case 210 : Return "N"
                Case 138 : Return "S"
                Case 216 : Return "R"
                Case 142 : Return "Z"
                Case 201 : Return "E"
                Case 193 : Return "A"
                Case 211 : Return "O"
                Case 232 : Return "c"
                Case 157 : Return "t"
                Case 250 : Return "u"
                Case 242 : Return "n"
                Case 154 : Return "s"
                Case 224 : Return "r"
                Case 158 : Return "z"
                Case 233 : Return "e"
                Case 225 : Return "a"
                Case 236 : Return "e"
                Case 237 : Return "i"
                Case 243 : Return "o"
                Case 249 : Return "u"
                Case Else
                    Return s
            End Select

        End Function


        Public Function ToFopCZ(ByVal s As String) As String
            'Otestujeme zda-li řetězec ke zpracování nění prázdný(délka=0).
            'detailní tabulka znaků je na: http://www.ascii.cl/htmlcodes.htm
            If s = "" Then
                Return ""
            End If

            s = Replace(s, "á", "&#225;")
            s = Replace(s, "Á", "&#193;")
            s = Replace(s, "č", "&#269;")
            s = Replace(s, "Č", "&#268;")
            s = Replace(s, "ď", "&#271;")
            s = Replace(s, "Ď", "&#270;")
            s = Replace(s, "é", "&#233;")
            s = Replace(s, "ě", "&#277;")
            s = Replace(s, "É", "&#201;")
            s = Replace(s, "Ě", "&#276;")
            s = Replace(s, "í", "&#237;")
            s = Replace(s, "Í", "&#205;")
            s = Replace(s, "ň", "&#328;")
            s = Replace(s, "Ň", "&#327;")
            s = Replace(s, "ó", "&#243;")
            s = Replace(s, "Ó", "&#211;")
            s = Replace(s, "ř", "&#345;")
            s = Replace(s, "Ř", "&#344;")
            s = Replace(s, "š", "&#353;")
            s = Replace(s, "Š", "&#352;")
            s = Replace(s, "ť", "&#357;")
            s = Replace(s, "Ť", "&#356;")
            s = Replace(s, "ú", "&#250;")
            s = Replace(s, "ů", "&#367;")
            s = Replace(s, "Ú", "&#218;")
            s = Replace(s, "Ů", "&#366;")
            s = Replace(s, "ý", "&#253;")
            s = Replace(s, "Ý", "&#221;")
            s = Replace(s, "Ů", "&#366;")
            s = Replace(s, "ž", "&#382;")
            s = Replace(s, "Ž", "&#381;")
            's = Replace(s, Chr(34), "&#34;")
            s = Replace(s, ChrW(8220), "&#8220;")
            s = Replace(s, "„", "&#8222;")
            s = Replace(s, ChrW(8211), "&#8211;")

            Return s


        End Function
    End Class
